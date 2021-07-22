#!/bin/bash

PROJECT_PATH=$1
UNITY_VERSION=$2
BUILD_ROOT=$3
BUILD_TARGET=$4
PLATFORM=$5

BUILD_ROOT=$BUILD_ROOT'/'$BUILD_TARGET-$UNITY_VERSION
if [ -d "$BUILD_ROOT" ]; then rm -Rf $BUILD_ROOT; fi
mkdir -p "$BUILD_ROOT"

BUILD_LOG_PATH="$BUILD_ROOT/build.log"

if [ "$PLATFORM" == "docker" ]; then
	LICENSE_PATH=$PROJECT_PATH'/license.ulf'
	LICENCE_LOG_PATH="$BUILD_ROOT/license.log"
	echo "$UNITY_LICENSE" > "$LICENSE_PATH"
	unity-editor -batchmode -manualLicenseFile "$LICENSE_PATH" -logFile "$LICENCE_LOG_PATH"
	unity-editor -batchmode -quit -projectPath "$PROJECT_PATH" -customBuildPath "$BUILD_ROOT" -customBuildTarget "$BUILD_TARGET" -executeMethod Xsolla.BuildsManager.Build -logFile "$BUILD_LOG_PATH"
else
	UNITY_PATH="C:/Program Files/Unity/Hub/Editor/$UNITY_VERSION/Editor/Unity.exe"
	"$UNITY_PATH" -batchmode -quit -projectPath "$PROJECT_PATH" -customBuildPath "$BUILD_ROOT" -customBuildTarget "$BUILD_TARGET" -executeMethod Xsolla.BuildsManager.Build -logFile "$BUILD_LOG_PATH"
fi

EXIT_CODE=$?
exit $EXIT_CODE