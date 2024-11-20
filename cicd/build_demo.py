import os
import subprocess
import sys
import unity
import utils
import diawi


utils.validate_args_count(4)
project_dir = sys.argv[1]
engine_version = sys.argv[2]
build_target = sys.argv[3]

build_root = os.path.join(project_dir, 'Builds')
unity_path = unity._get_unity_path(engine_version)
report_path = os.path.join(project_dir, 'Builds', 'build_report.log')

os.makedirs(build_root, exist_ok=True)

build_result = unity.build_demo(project_dir, engine_version, build_root, build_target, report_path)

if build_result != 0 or build_target != 'iOS':
    sys.exit(build_result)

diawi_result = diawi.build_diawi(engine_version, build_root)
sys.exit(diawi_result)