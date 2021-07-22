#!/bin/bash

PROJECT_PATH=$(dirname "$(pwd)")
BUILD_PLATFORM="windows"

BUILD_ROOT="$PROJECT_PATH/Builds"
mkdir -p "$BUILD_ROOT"

LOG_PATH="$BUILD_ROOT/batch-build-report.log"
if [ -f "$LOG_PATH" ]; then rm $LOG_PATH; fi

UNITY_SUCCESS_BUILDS=0
UNITY_FAILED_BUILDS=0

BATCH_SUCCESS_BUILDS=0
BATCH_FAILED_BUILDS=0

LOG()
{
	echo $@ | tee -a $LOG_PATH
}

PRINT()
{
	printf '%-12s | %-20s | %-12s\n' $@ | tee -a $LOG_PATH
}

DATE2SEC() 
{ 
	date -d "$(sed 's|-|/|g; s|:| |' <<<"$*")" +%s; 
}

LAUNCH_BUILD()
{
	COPY_ROOT="$BUILD_ROOT/_BUILD_TEMP_${BUILD_TARGET^^}-${UNITY_VERSION^^}"
	if [ -d "$COPY_ROOT" ]; then rm -Rf $COPY_ROOT; fi
	mkdir -p "$COPY_ROOT"

	cp -r "$PROJECT_PATH/Assets/" "$COPY_ROOT"
	cp -r "$PROJECT_PATH/Packages/" "$COPY_ROOT"
	cp -r "$PROJECT_PATH/ProjectSettings/" "$COPY_ROOT"

	bash "./build-universal.sh" \
		"$COPY_ROOT" \
		"$UNITY_VERSION" \
		"$BUILD_ROOT" \
		"$BUILD_TARGET" \
		"$BUILD_PLATFORM"

	if [ $? == 0 ]; then
		UNITY_SUCCESS_BUILDS=$((UNITY_SUCCESS_BUILDS+1))
		BATCH_SUCCESS_BUILDS=$((BATCH_SUCCESS_BUILDS+1))
		PRINT "$UNITY_VERSION" "$BUILD_TARGET" "Succeeded"
	else
		UNITY_FAILED_BUILDS=$((UNITY_FAILED_BUILDS+1))
		BATCH_FAILED_BUILDS=$((BATCH_FAILED_BUILDS+1))
		PRINT "$UNITY_VERSION" "$BUILD_TARGET" "Failed"
	fi

	rm -Rf $COPY_ROOT;
}

BUILD_ALL_PLATFORMS()
{
	UNITY_VERSION=$1

	UNITY_SUCCESS_BUILDS=0
	UNITY_FAILED_BUILDS=0

	BUILD_TARGET="StandaloneWindows64"
	LAUNCH_BUILD

	BUILD_TARGET="WebGL"
	LAUNCH_BUILD

	BUILD_TARGET="Android"
	LAUNCH_BUILD

	LOG ""
}

LOG ""
LOG "========================================================="
LOG "BEGIN BATCH BUILD"
LOG ""

BEGIN_TIME=$(date +"%m-%d-%Y:%H:%M:%S")

BUILD_ALL_PLATFORMS "2019.4.27f1"
BUILD_ALL_PLATFORMS "2020.3.8f1"
BUILD_ALL_PLATFORMS "2021.1.13f1"

TOTAL_BUILDS=$((BATCH_SUCCESS_BUILDS + BATCH_FAILED_BUILDS))
LOG "FINISH BATCH BUILD. TOTAL: $TOTAL_BUILDS. SUCCEEDED: $BATCH_SUCCESS_BUILDS. FAILED: $BATCH_FAILED_BUILDS"
LOG "========================================================="

FINISH_TIME=$(date +"%m-%d-%Y:%H:%M:%S")
TOTAL_TIME=$(( $(DATE2SEC "$FINISH_TIME") - $(DATE2SEC "$BEGIN_TIME") ))
printf 'DURATION: %dh:%dm:%ds\n' $((TOTAL_TIME/3600)) $((TOTAL_TIME%3600/60)) $((TOTAL_TIME%60)) | tee -a $LOG_PATH
LOG ""

read  -n 1 -p "Press any key to exit" mainmenuinput