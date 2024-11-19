import sys
import yaml

def finish_with_error(message):
    print(message)
    sys.exit(1)


def validate_args_count(expected_args_count):
    if len(sys.argv) != expected_args_count:
        finish_with_error("Arguments count error. Expected: " + str(expected_args_count) + ", actual: " + str(len(sys.argv)))


def is_macos():
    return sys.platform == 'darwin' 