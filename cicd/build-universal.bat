@echo OFF

echo Start Build
set PROJECT_PATH=%1
set UNITY_VERSION=%2
set BUILD_ROOT=%3
set BUILD_TARGET=%4
set PLATFORM=%5
set BUILD_ROOT=%PROJECT_PATH%\%BUILD_ROOT%\%BUILD_TARGET%-%UNITY_VERSION%
set WORK_DIR=C:\Users\Runner\Desktop\RunnerWorkDir\ProjectTemp

echo "PROJECT_PATH: %PROJECT_PATH%"
echo "UNITY_VERSION: %UNITY_VERSION%"
echo "BUILD_ROOT: %BUILD_ROOT%"
echo "BUILD_TARGET: %BUILD_TARGET%"
echo "PLATFORM: %PLATFORM%"

mkdir %BUILD_ROOT%

echo "Сopy project to ProjectTemp"
xcopy Assets\ %WORK_DIR%\Assets\ /e >nul 2>&1
xcopy Packages\ %WORK_DIR%\Packages\ /e >nul 2>&1
xcopy ProjectSettings\ %WORK_DIR%\ProjectSettings\ /e >nul 2>&1

set BUILD_LOG_PATH=%BUILD_ROOT%\build.log

set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\%UNITY_VERSION%\Editor\Unity.exe"
echo %UNITY_PATH%

set LOGS_PATH=%PROJECT_PATH%\Logs

echo "%UNITY_PATH% -batchmode -quit -projectPath %WORK_DIR% -customBuildPath "%BUILD_ROOT%" -customBuildTarget "%BUILD_TARGET%" -executeMethod Xsolla.BuildsManager.Build -logFile "%BUILD_LOG_PATH%""
call %UNITY_PATH% -batchmode -quit -projectPath %WORK_DIR% -customBuildPath "%BUILD_ROOT%" -customBuildTarget "%BUILD_TARGET%" -executeMethod Xsolla.BuildsManager.Build -logFile "%BUILD_LOG_PATH%"

echo "Remove WORK_DIR"
rmdir /s /q %WORK_DIR%

exit /b %errorlevel%