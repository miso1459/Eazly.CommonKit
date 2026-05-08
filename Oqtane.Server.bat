@echo off
set PORT=5000

netstat -ano | find ":%PORT%" | find "LISTENING" > nul
if not errorlevel 1 (
    echo 포트 %PORT% 이미 사용 중
    start "" "http://localhost:%PORT%"
    exit /b
)

pushd "%~dp0Oqtane.Framework.10.1.2.Install"
start "" Oqtane.Server.exe
popd

timeout /t 5 > nul
start "" "http://localhost:%PORT%"