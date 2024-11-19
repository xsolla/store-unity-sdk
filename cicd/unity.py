import os
import subprocess
import utils


def execute_tests(project_dir, engine_version, report_path):
    unity_path = _get_unity_path(engine_version)
    result = subprocess.run(
        [
            unity_path,
            "-batchmode",
            "-runTests",
            "-projectPath", project_dir,
            "-testResults", report_path
        ]
    )
    return result.returncode


def build_demo(project_dir, engine_version, build_root, build_target, report_path):
    unity_path = _get_unity_path(engine_version)
    result = subprocess.run(
        [
           unity_path,
            "-batchmode",
            "-quit",
            "-projectPath", project_dir,
            "-customBuildPath", build_root,
            "-buildTarget", build_target,
            "-executeMethod", "Xsolla.DevTools.BuildsManager.Build",
            "-logFile", report_path
        ]
    )
    return result.returncode


def _get_unity_path(engine_version):
    if utils.is_macos():
        return f"/Applications/Unity/Hub/Editor/{engine_version}/Unity.app/Contents/MacOS/Unity"
    else:
        return f"C:\\Program Files\\Unity\\Hub\\Editor\\{engine_version}\\Editor\\Unity.exe"
