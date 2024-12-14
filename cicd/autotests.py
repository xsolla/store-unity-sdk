import os
import subprocess
import sys
import unity
import utils


utils.validate_args_count(3)
project_dir = sys.argv[1]
engine_version = sys.argv[2]

report_path = os.path.join(project_dir, 'Logs', 'autotests_report.log')

tests_result = unity.execute_tests(project_dir, engine_version, report_path)
sys.exit(tests_result)