@echo OFF

echo "####        Autotests        ####"
set PROJECT_PATH=%1
set UNITY_VERSION=%2
set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\%UNITY_VERSION%\Editor\Unity.exe"
set WORK_DIR=C:\Users\Runner\Desktop\RunnerWorkDir\ProjectTemp
set LOGS_PATH=%PROJECT_PATH%\Logs\autotest_results.txt

echo "####   Copy To Project Temp  ####"
xcopy Assets\ %WORK_DIR%\Assets\ /e >nul 2>&1
xcopy Packages\ %WORK_DIR%\Packages\ /e >nul 2>&1
xcopy ProjectSettings\ %WORK_DIR%\ProjectSettings\ /e >nul 2>&1

echo "####     Start Autotests     ####"
echo "PROJECT_PATH: %PROJECT_PATH%"
echo "UNITY_VERSION: %UNITY_VERSION%"
echo "%UNITY_PATH% -batchmode -runTests -projectPath %WORK_DIR% -testResults %LOGS_PATH%"
call %UNITY_PATH% -batchmode -runTests -projectPath %WORK_DIR% -testResults %LOGS_PATH%
set TEST_ERROR=%errorlevel%
echo "####      End Autotests      ####"
echo "####      Error Level:%TEST_ERROR%      ####"
echo "####        Full Log:        ####"
for /F "tokens=* delims=" %%x in (%LOGS_PATH%) do echo %%x

echo "####     Remove WORK_DIR     ####"
rmdir /s /q %WORK_DIR%

exit /b %TEST_ERROR%