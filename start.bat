@echo off
REM ====== 自動關閉 5000/3000 port ======
for %%P in (5000 3000) do (
    for /f "tokens=5" %%a in ('netstat -ano ^| findstr :%%P') do (
        echo Killing PID %%a on port %%P
        taskkill /F /PID %%a
    )
)
REM ====== 原有內容 ======
echo.
echo ================================================
echo          MES 系統 - 開發環境啟動
echo ================================================
echo.

echo [1/3] 啟動後端 API...
cd backend\MESSystem.API
start "MES API Server" cmd /k "dotnet run"
cd ..\..

echo [2/3] 等待 API 啟動...
timeout /t 5 /nobreak > nul

echo [3/3] 啟動前端應用...
cd frontend
start "MES Frontend" cmd /k "npm start"
cd ..

echo.
echo ================================================
echo              啟動完成！
echo ================================================
echo.
echo 後端 API: http://localhost:5000
echo 前端應用: http://localhost:3000
echo Swagger文件: http://localhost:5000
echo.
echo 測試帳號:
echo   帳號: admin, 密碼: 123456, 廠別: TPL
echo   帳號: user1, 密碼: 123456, 廠別: NVN  
echo   帳號: user2, 密碼: 123456, 廠別: LR
echo.
echo 按任意鍵關閉此視窗...
pause > nul 