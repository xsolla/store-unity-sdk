#!/bin/bash

PROJECT_PATH="$(pwd)"

LOGS_PATH=$PROJECT_PATH'/Logs'
mkdir -p "$LOGS_PATH"

LICENSE_PATH=$PROJECT_PATH'/license.ulf'
echo "$UNITY_LICENSE" > "$LICENSE_PATH"

unity-editor -batchmode \
	-manualLicenseFile "$LICENSE_PATH" \
	-logFile "$LOGS_PATH/license.log"

unity-editor -batchmode -quit -nographics \
	-projectPath "$PROJECT_PATH" \
	-logFile "$LOGS_PATH/solution-generate.log" \
	-executeMethod "UnityEditor.SyncVS.SyncSolution"

wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
apt-get update
apt-get install apt-transport-https
apt-get install -y dotnet-sdk-3.1
export FrameworkPathOverride=/Library/Frameworks/Mono.framework/Versions/Current

apt-get install -y curl zip unzip
curl -LO "https://download.jetbrains.com/resharper/dotUltimate.2021.1.3/JetBrains.ReSharper.CommandLineTools.2021.1.3.zip"
unzip JetBrains.ReSharper.CommandLineTools.2021.1.3.zip -d "resharper-clt" 

SLN_PATHS=( *.sln )
REPORT_PATH="$LOGS_PATH/code-analysis-report.xml"

chmod u+x resharper-clt/inspectcode.sh
resharper-clt/inspectcode.sh "${SLN_PATHS[0]}" -o="$REPORT_PATH" -s="ERROR"

apt-get install -y xmlstarlet
ISSUES=$(xmlstarlet sel -t -v "count(/Report/IssueTypes/IssueType)" "$REPORT_PATH")
if [ "$ISSUES" -gt "0" ] 
then
	echo "ISSUES: $ISSUES" 
	exit 1
fi