# 使用 .NET 8.0 SDK 進行建置
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# 複製 csproj 檔案並還原相依性
COPY MESSystem.API/*.csproj ./
RUN dotnet restore

# 複製所有內容並建置應用程式
COPY MESSystem.API/ ./
RUN dotnet publish -c Release -o out

# 建置執行時映像
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# 建立非 root 使用者
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# 暴露埠口
EXPOSE 10000

# 設定環境變數
ENV ASPNETCORE_URLS=http://+:10000

# 啟動應用程式
ENTRYPOINT ["dotnet", "MESSystem.API.dll"] 