#!/bin/bash

echo ""
echo "#### Generate XCode project ####"
PROJECT_PATH=$1
UNITY_VERSION=$2
BUILD_ROOT=$3
BUILD_TARGET=$4

BUILD_LOG_PATH="$BUILD_ROOT/build.log"
BUILD_ROOT=$BUILD_ROOT'/'$BUILD_TARGET-$UNITY_VERSION
UNITY_PATH="/Applications/Unity/Hub/Editor/$UNITY_VERSION/Unity.app/Contents/MacOS/Unity"

echo "PROJECT_PATH: ${PROJECT_PATH}"
echo "UNITY_VERSION: ${UNITY_VERSION}"
echo "BUILD_ROOT: ${BUILD_ROOT}"
echo "BUILD_TARGET: ${BUILD_TARGET}"
echo "BUILD_LOG_PATH: ${BUILD_LOG_PATH}"
echo "UNITY_PATH: ${UNITY_PATH}"

if [ -d "$BUILD_ROOT" ]; then rm -Rf $BUILD_ROOT; fi
mkdir -p "$BUILD_ROOT"

echo $UNITY_PATH -batchmode -quit -projectPath $PROJECT_PATH -customBuildPath "$BUILD_ROOT" -buildTarget "$BUILD_TARGET" -executeMethod "Xsolla.DevTools.BuildsManager.Build" -logFile "$BUILD_LOG_PATH"
$UNITY_PATH -batchmode -quit -projectPath $PROJECT_PATH -customBuildPath "$BUILD_ROOT" -buildTarget "$BUILD_TARGET" -executeMethod "Xsolla.DevTools.BuildsManager.Build" -logFile "$BUILD_LOG_PATH"

EXIT_CODE=$?
if [ $EXIT_CODE -ne 0 ]; then
	exit $EXIT_CODE
fi

echo ""
echo "#### Build .ipa file ####"
export BUILD_ROOT
export UNITY_VERSION
bundle exec fastlane diawi_build

