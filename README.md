# MES 系統原型 (Prototype)

這是一個製造執行系統 (Manufacturing Execution System) 的原型專案，包含前端 React 應用程式和後端 ASP.NET Core Web API。

## 🏗️ 技術架構

### 後端 (Backend)
- **框架**: ASP.NET Core 8.0 Web API
- **資料庫**: SQLite (本地端)
- **ORM**: Entity Framework Core
- **認證**: JWT Bearer Token
- **文件**: Swagger/OpenAPI

### 前端 (Frontend)
- **框架**: React 18 + TypeScript
- **UI 框架**: Ant Design 5
- **HTTP 客戶端**: Axios
- **日期處理**: Day.js

## 🎯 核心功能

### 1. 使用者認證
- 帳號、密碼、廠別登入 (TPL/NVN/LR)
- JWT Token 驗證
- 自動登入狀態記憶

### 2. 功能選單
- **Estimated Incoming Shipment**: 進貨到廠預估表 ✅ 已實作
- **Production Overview**: 生產概況 🚧 尚未實作
- **Quality**: 品質狀況 🚧 尚未實作
- **Reports**: 報表頁面 🚧 尚未實作

### 3. 進貨到廠預估表
- 📋 完整的 CRUD 操作 (新增、查詢、修改、刪除)
- 📊 資料表格顯示
- 🔍 欄位篩選
- 🚦 到廠狀態指示燈 (綠燈：如期/已到貨，紅燈：延遲/尚未到貨)

#### 欄位說明
- **BL NO**: 提單號碼
- **Customer**: 客戶名稱
- **Style**: 款式編號
- **PO NO**: 採購單號
- **Rolls**: 布卷數量
- **ETD**: 預計出貨日
- **ETA**: 預計到港日
- **Fty ETA**: 預計到廠日
- **Arrive Status**: 到廠狀態

## 🚀 快速開始

### 前置要求
- .NET 8.0 SDK
- Node.js 16+ (前端)
- Visual Studio 2022 或 Visual Studio Code

### 執行後端 API

```bash
# 進入後端目錄
cd backend/MESSystem.API

# 還原套件
dotnet restore

# 執行應用程式
dotnet run
```

後端 API 將在 `http://localhost:5000` 啟動，Swagger 文件可在根路徑 (`/`) 查看。

### 執行前端應用

```bash
# 進入前端目錄
cd frontend

# 安裝依賴
npm install

# 啟動開發伺服器
npm start
```

前端應用將在 `http://localhost:3000` 啟動。

## 👤 測試帳號

| 帳號 | 密碼 | 廠別 |
|------|------|------|
| admin | 123456 | TPL |
| user1 | 123456 | NVN |
| user2 | 123456 | LR |

## 📁 專案結構

```
POC/
├── backend/
│   └── MESSystem.API/           # ASP.NET Core Web API
│       ├── Controllers/         # API 控制器
│       ├── Data/               # 資料庫上下文
│       ├── DTOs/               # 資料傳輸物件
│       ├── Models/             # 資料模型
│       ├── Services/           # 業務邏輯服務
│       └── Program.cs          # 應用程式入口點
├── frontend/
│   ├── public/                 # 靜態檔案
│   └── src/
│       ├── components/         # React 組件
│       ├── services/           # API 服務
│       ├── types/              # TypeScript 類型定義
│       ├── App.tsx             # 主應用程式組件
│       └── index.tsx           # React 入口點
└── MESSystem.sln               # Visual Studio 解決方案檔
```

## 🔧 開發說明

### 資料庫
使用 SQLite 作為本地資料庫，資料庫檔案會自動建立在後端專案根目錄下 (`MESSystem.db`)。

### API 文件
啟動後端後，可透過 Swagger UI 查看完整的 API 文件：
- 開發環境: `http://localhost:5000/`

### 前端開發
- 使用 TypeScript 進行類型安全的開發
- 使用 Ant Design 組件庫提供一致的使用者介面
- 使用 Axios 進行 HTTP 請求，包含攔截器處理認證和錯誤

## 📝 待辦事項

- [ ] 實作 Production Overview 模組
- [ ] 實作 Quality 模組  
- [ ] 實作 Reports 模組
- [ ] 新增單元測試
- [ ] 新增 Docker 容器化
- [ ] 新增 CI/CD 流程

## 🤝 貢獻

歡迎提交 Issue 和 Pull Request 來改善這個專案。

## 📄 授權

此專案僅供學習和原型開發使用。 