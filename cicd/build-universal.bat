@echo OFF

echo Start Build
set PROJECT_PATH=%1
set UNITY_VERSION=%2
set BUILD_ROOT=%3
set BUILD_TARGET=%4
set PLATFORM=%5
set BUILD_ROOT=%PROJECT_PATH%\%BUILD_ROOT%\%BUILD_TARGET%-%UNITY_VERSION%

echo "PROJECT_PATH: %PROJECT_PATH%"
echo "UNITY_VERSION: %UNITY_VERSION%"
echo "BUILD_ROOT: %BUILD_ROOT%"
echo "BUILD_TARGET: %BUILD_TARGET%"
echo "PLATFORM: %PLATFORM%"

mkdir %BUILD_ROOT%

set BUILD_LOG_PATH=%BUILD_ROOT%\build.log

set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\%UNITY_VERSION%\Editor\Unity.exe"
echo %UNITY_PATH%

echo "%UNITY_PATH% -batchmode -quit -projectPath %PROJECT_PATH% -customBuildPath "%BUILD_ROOT%" -customBuildTarget "%BUILD_TARGET%" -executeMethod Xsolla.BuildsManager.Build -logFile "%BUILD_LOG_PATH%""
call %UNITY_PATH% -batchmode -quit -projectPath %PROJECT_PATH% -customBuildPath "%BUILD_ROOT%" -buildTarget "%BUILD_TARGET%" -executeMethod Xsolla.BuildsManager.Build -logFile "%BUILD_LOG_PATH%" 

exit /b %errorlevel%