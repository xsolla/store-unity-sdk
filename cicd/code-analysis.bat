@echo OFF

rem using system variables:
rem RESHARPER_2021.1.3_BIN
rem UNITY_2019.4.27F1_BIN
rem XMLSTARLET_1.6.1_BIN

set PROJECT_PATH=%cd%

set LOGS_PATH=%PROJECT_PATH%\Logs
mkdir "%LOGS_PATH%"

"%UNITY_2019.4.27F1_BIN%" -batchmode -nographics -logFile "%LOGS_PATH%\solution-generate.log" -executeMethod "UnityEditor.SyncVS.SyncSolution" -projectPath "%PROJECT_PATH%" -quit

set REPORT_PATH="%LOGS_PATH%\code-analysis-report.xml"

%RESHARPER_2021.1.3_BIN% "store-unity-sdk.sln" -o="%REPORT_PATH%" -s="ERROR" --dotnetcoresdk=3.1.412

FOR /F %%i IN ('%XMLSTARLET_1.6.1_BIN% sel -t -v "count(/Report/IssueTypes/IssueType)" %REPORT_PATH%') DO set ISSUES=%%i

echo "ISSUES: %ISSUES%"
if %ISSUES% NEQ 0 ( 
	exit 1
 )
