#!/usr/bin/env bash



echo "START LAUNCHER"

sh "cicd/build-demo-mac.sh" "/Users/runner/test/store-unity-sdk" "2019.4.27f1" "Builds" "iOS"

# $ call %BUILD_BAT% %PROJECT_PATH% %UNITY_2019_VERSION% %BUILD_ROOT_PATH% %BUILD_TARGET_WEBGL% %BUILD_PLATFORM%