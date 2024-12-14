import os
import subprocess


def build_diawi(engine_version, build_root):
    os.environ['UNITY_VERSION'] = engine_version
    os.environ['BUILD_ROOT'] = build_root
    
    result = subprocess.run(
            ["bundle", 
            "exec", "fastlane", 
            "diawi_build"
            ]
        )
    
    return result.returncode