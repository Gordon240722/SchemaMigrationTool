# 專案背景與角色
本專案為我個人開發的 Side Project，目的是學習並實作多租戶架構下的資料庫 Migration 管理與 CI/CD 自動化流程，參考自我在實務工作中遇到的痛點

# 流程
每次執行 migrate 指令：
1. 讀取租戶清單
2. 對每個租戶執行：
   - 檢查 Version Table（每個 schema 獨立）</br>
   - 套用 FluentMigrator 指定 schema 的 migration

# 前置作業 
- 安裝 Visual Studio
- 安裝 .NET 6 SDK，執行以下命令查看：
   > dotnet --list-sdks
- 看到類似以下輸出，確認已安裝 .NET 6.x 版本：
  > 6.0.xxx [路徑]

# 建立專案 
- 專案類型：主控台應用程式（Console App）
- 主要用途：執行多 schema 的資料庫 migration
- 可以在 CI/CD 流程中自動執行
- 專案位置 - 解決方案檔案（.sln）通常放在專案的父層目錄，專案本體放在子層目錄
- 其他資訊
  - 使用 .NET 6 框架（目前仍受支援）
  - 可啟用容器支援（Docker），Dockerfile 稍後新增

# 多租戶 Migration 實作 
本專案起初使用 EF Core 進行 Migration，但在面對多租戶（Multi-Tenant）結構下，EF Core 的 Migration 機制難以做到每個 schema 隔離、版本追蹤獨立，於是改採更專業的遷移管理工具
### 為什麼使用 FluentMigrator？ 
- EF Core 是 ORM，但不是專職 Migration 管理工具
- FluentMigrator 支援：
  - 自定義版本表（Version Table）結構
  - 多 schema 隔離管理
  - CLI 無依賴執行、好整合進 CI/CD ### 架構重點
  - 每個租戶使用不同的 schema（如：tenant_a、tenant_b）
  - 每個 schema 擁有自己獨立的 Migration 記錄表
  - 使用 IVersionTableMetaData 自定 Version Table
  - 使用 ApplicationContext 傳入租戶資訊
## 多租戶 Migration 實作過程 
### 問題：不同租戶共用 VersionInfo 表 
在初始版本中，所有租戶共用 dbo.VersionInfo，導致： 
- 第一個租戶執行成功後
- 其他租戶跳過 migration 或產生 Table Already Exists 錯誤
### 嘗試方案一：自定義 Version 表
```
public class TenantVersionTableMetaData : IVersionTableMetaData
{
    public string SchemaName => _schema;
    public string TableName => "VersionInfo";
    // 其他略
}
```
- 每個租戶設定自己的 schema
- 使用 ApplicationContext 傳遞 schema
- Migration 檔案內使用 .InSchema(_schema) 明確指定 schema
### 實作後效果： 
每個 schema 有自己的 VersionInfo 表, 同一份 Migration 可以被套用到每個 schema，彼此不影響 
## GitHub Actions 自動化 Migration 
本專案使用 GitHub Actions 實作 CI/CD，實現每次 push 自動執行多租戶資料庫遷移
- 使用 dotnet publish 包含設定檔與 DLL
- 解決 build 無法複製 appsettings.json 的問題
- 支援自動遷移多租戶 schema
## 進階功能：資料初始化與環境配置 
1. 每個租戶建立預設資料（Seed Data） 使用 FluentMigrator 的 Insert.IntoTable().InSchema(...) 插入初始資料
2. Migration Log（遷移紀錄表) 記錄每次遷移結果與狀態，避免重複執行與問題排查
3. 環境變數與 CI/CD Secret 管理
  - 本機: .env + dotenv.net
  - GitHub Actions: 使用 secrets.DEFAULT_CONNECTION 設定連線字串

# 自動化部署與執行：GitHub Actions + Docker 
本專案支援自動打包成 Docker Image 並推送至 GitHub Packages（GHCR），也可在本地或 CI/CD 環境中以容器執行資料庫 Migration 
## Docker 化專案 使用官方 SQL Server Docker Image 建立資料庫： 
> docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
## 建立 Docker Image：
### Dockerfile（已在專案中提供）
```
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SchemaMigrationTool.dll"]
```
### GitHub Actions: CI/CD 自動推送 Image 至 GHCR 
1. 建立 Secret（用於登入 GHCR）
2. 撰寫 CI 工作流程 yml
3. 在本地執行 Container + Migration
   1. 登入 GHCR：
      > echo <your_pat_token> | docker login ghcr.io -u <your-username> --password-stdin
   3. 建立 Network 讓 App & DB 通訊：
      > docker network create app-network
   5. 啟動 SQL Server 並加入網路：
      > docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<your-password>" --network app-network --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
   7. 建立資料庫：
      > docker run -it --rm --network app-network mcr.microsoft.com/mssql-tools \ /opt/mssql-tools/bin/sqlcmd -S sqlserver -U sa -P "<your-password>" \ -Q "CREATE DATABASE TenantSampleDb"
   9. 執行 Migration：
       > docker run --rm --network app-network \ -e DB_HOST=sqlserver \ -e DB_PORT=1433 \ -e DB_USER=sa \ -e DB_PASS=<your-password> \ ghcr.io/<your-username>/schema-migrator:latest migrate
### 結果驗證
```
正在執行 Schema Migrate: TenantA（Schema: tenant_a）
...
CreateTable Users
...
CreateTable MigrationLog
租戶 TenantA 的資料庫遷移完成
...
租戶 TenantB 的資料庫遷移完成
```
