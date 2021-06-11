#!/bin/bash

PROJECT_PATH="$(pwd)"

BUILD_PATH=$PROJECT_PATH'/Builds/WebGL'
mkdir -p "$BUILD_PATH"

LOGS_PATH=$PROJECT_PATH'/Logs'
mkdir -p "$LOGS_PATH"

LICENSE_PATH=$PROJECT_PATH'/license.ulf'
echo "$UNITY_LICENSE" > "$LICENSE_PATH"

unity-editor -batchmode \
	-manualLicenseFile "$LICENSE_PATH" \
	-logFile "$LOGS_PATH/license.log"

unity-editor -batchmode -quit \
	-projectPath "$PROJECT_PATH" \
	-buildPath "$BUILD_PATH" \
	-buildTarget "WebGL" \
	-executeMethod Xsolla.BuildsManager.PerformBuild \
	-logFile "$LOGS_PATH/build.log"