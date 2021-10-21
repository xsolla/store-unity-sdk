@echo OFF

set PROJECT_PATH=%1
set UNITY_VERSION=%2
set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\%UNITY_VERSION%\Editor\Unity.exe"
set LOGS_PATH=%PROJECT_PATH%\Logs\autotest_results.txt

echo "####     Start Autotests     ####"
echo "PROJECT_PATH: %PROJECT_PATH%"
echo "UNITY_VERSION: %UNITY_VERSION%"
echo "%UNITY_PATH% -batchmode -runTests -projectPath %PROJECT_PATH% -testResults %LOGS_PATH%"
call %UNITY_PATH% -batchmode -runTests -projectPath %PROJECT_PATH% -testResults %LOGS_PATH%

if errorlevel 0 echo "####  All Tests Successful   ####"
if errorlevel 2 echo "####  Some tests have failed, check the Logs  ####"
if errorlevel 3 echo "####  RUN FAILURE, check the Logs  ####"
echo "#### Autotests Ended With Errorlevel:%errorlevel% ####"
exit /b %errorlevel%