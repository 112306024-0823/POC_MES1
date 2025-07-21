#!/bin/bash

echo ""
echo "================================================"
echo "          MES 系統 - 開發環境啟動"
echo "================================================"
echo ""

echo "[1/3] 啟動後端 API..."
cd backend/MESSystem.API
gnome-terminal --title="MES API Server" -- bash -c "dotnet run; exec bash" &
cd ../..

echo "[2/3] 等待 API 啟動..."
sleep 5

echo "[3/3] 啟動前端應用..."
cd frontend
gnome-terminal --title="MES Frontend" -- bash -c "npm start; exec bash" &
cd ..

echo ""
echo "================================================"
echo "              啟動完成！"
echo "================================================"
echo ""
echo "後端 API: http://localhost:5000"
echo "前端應用: http://localhost:3000"
echo "Swagger文件: http://localhost:5000"
echo ""
echo "測試帳號:"
echo "  帳號: admin, 密碼: 123456, 廠別: TPL"
echo "  帳號: user1, 密碼: 123456, 廠別: NVN"  
echo "  帳號: user2, 密碼: 123456, 廠別: LR"
echo ""
echo "按 Ctrl+C 停止此腳本"

# 保持腳本運行
wait 