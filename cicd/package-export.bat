set PROJECT_PATH=%1
set UNITY_VERSION=%2
set PACKAGE_NAME=%3
set NEW_PROJ_PATH=%4
set PACKAGE_PATH=%PROJECT_PATH%\Logs\%PACKAGE_NAME%
set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\%UNITY_VERSION%\Editor\Unity.exe"

echo ""
echo "####     Export Package     ####"
set LOGS_PATH=%PROJECT_PATH%\Logs\export_results.txt
echo "PROJECT_PATH: %PROJECT_PATH%"
echo "UNITY_VERSION: %UNITY_VERSION%"
echo "%UNITY_PATH% -batchmode -projectPath %PROJECT_PATH% -exportPackage "Assets\Xsolla" "Assets\DevTools" %PACKAGE_PATH% -logFile %LOGS_PATH% -quit"
call %UNITY_PATH% -batchmode -projectPath %PROJECT_PATH% -exportPackage "Assets\Xsolla" "Assets\DevTools" %PACKAGE_PATH% -logFile %LOGS_PATH% -quit
echo "## Export Ended With Errorlevel:%errorlevel% ##"
if not %errorlevel%==0 goto end

echo ""
echo "####     Create New Project     ####"
echo "NEW_PROJ_PATH: %NEW_PROJ_PATH%"
if exist %NEW_PROJ_PATH%\ (
echo "## Deleting existing folder"
@RD /S /Q %NEW_PROJ_PATH%
)
echo "## Creating new folder"
mkdir %NEW_PROJ_PATH%
echo "## Creating new project"
set LOGS_PATH=%PROJECT_PATH%\Logs\create_results.txt
echo "%UNITY_PATH% -batchmode -createProject %NEW_PROJ_PATH% -logFile %LOGS_PATH% -quit"
call %UNITY_PATH% -batchmode -createProject %NEW_PROJ_PATH% -logFile %LOGS_PATH% -quit
echo "## Create Ended With Errorlevel:%errorlevel% ##"
if not %errorlevel%==0 goto end

echo ""
echo "####     Import Package     ####"
set LOGS_PATH=%PROJECT_PATH%\Logs\import_results.txt
echo "%UNITY_PATH% -batchmode -projectPath %NEW_PROJ_PATH% -importPackage %PACKAGE_PATH% -logFile %LOGS_PATH% -quit"
call %UNITY_PATH% -batchmode -projectPath %NEW_PROJ_PATH% -importPackage %PACKAGE_PATH% -logFile %LOGS_PATH% -quit
echo "## Import Ended With Errorlevel:%errorlevel% ##"
if not %errorlevel%==0 goto end

echo ""
echo "####     Copy Manifest     ####"
if exist %NEW_PROJ_PATH%\Packages\ (
echo "## Deleting existing folder"
@RD /S /Q %NEW_PROJ_PATH%\Packages\
)
echo "mkdir %NEW_PROJ_PATH%\Packages"
mkdir %NEW_PROJ_PATH%\Packages
echo "xcopy /s %PROJECT_PATH%\Packages %NEW_PROJ_PATH%\Packages"
xcopy /s /e /q %PROJECT_PATH%\Packages %NEW_PROJ_PATH%\Packages
echo "## Copy Ended With Errorlevel:%errorlevel% ##"

:end
exit /b %errorlevel%