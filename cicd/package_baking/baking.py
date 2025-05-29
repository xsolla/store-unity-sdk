import os
import sys
import shutil
import re
from pathlib import Path

SETTINGS_ASSET_GUID = "973cf6499b6f0ec4aa36d33b99190dad"
PACKAGE_SOURCE_NAME = "xsolla-unity-sdk"
PACKAGE_FILE_EXTENSION = "unitypackage"


def get_script_dir() -> Path:
    return Path(__file__).resolve().parent


def get_source_package_path(sdk_version: str) -> Path:
    packages_dir = get_script_dir() / "source_packages"
    package_name = f"{PACKAGE_SOURCE_NAME}-{sdk_version}.{PACKAGE_FILE_EXTENSION}"
    return packages_dir / package_name

def create_temp_dir() -> Path:
    temp_dir = get_script_dir() / "temp"
    if temp_dir.exists():
        shutil.rmtree(temp_dir)
    temp_dir.mkdir(parents=True, exist_ok=True)
    return temp_dir


def unpack_package_content(source_path: Path, target_dir: Path) -> None:
    temp_file_path = target_dir / "package.zip"
    shutil.copy(source_path, temp_file_path)

    shutil.unpack_archive(str(temp_file_path), str(target_dir), 'tar')
    temp_file_path.unlink()


def get_unpacked_asset_path(dir: Path) -> Path:
    return dir / SETTINGS_ASSET_GUID / "asset"


def modify_asset_yaml(file_path: Path, store_project_id: str, login_project_id: str, oauth_client_id: str) -> None:
    with file_path.open("r", encoding="utf-8") as file:
        content = file.read()

    # Update the Store Project Id 
    content = re.sub(r'storeProjectId:\s*\d+', f'storeProjectId: {store_project_id}', content)
    content = re.sub(r'loginId:\s*[\w-]+', f'loginId: {login_project_id}', content)
    content = re.sub(r'oauthClientId:\s*\d+', f'oauthClientId: {oauth_client_id}', content)

    with file_path.open("w", encoding="utf-8") as file:
        file.write(content)


def get_export_package_path(sdk_version: str) -> Path:
    export_package_name = f"{PACKAGE_SOURCE_NAME}-{sdk_version}.{PACKAGE_FILE_EXTENSION}"
    return get_script_dir() / export_package_name


def pack_package_content(export_path: Path, source_dir: Path) -> None:
    shutil.make_archive(str(export_path), 'tar', str(source_dir))
    tar_path = export_path.with_suffix(export_path.suffix + '.tar')
    shutil.move(str(tar_path), str(export_path))


def bake_package(sdk_version: str, store_project_id: str, login_project_id: str, oauth_client_id: str) -> Path:
    # Unpack the source package to temporary directory
    source_package_path = get_source_package_path(sdk_version)
    temp_dir = create_temp_dir()
    unpack_package_content(source_package_path, temp_dir)
    
    # Modify the asset YAML file
    asset_path = get_unpacked_asset_path(temp_dir)
    modify_asset_yaml(asset_path, store_project_id, login_project_id, oauth_client_id)

    # Create the export package with the modified asset
    export_path = get_export_package_path(sdk_version)
    pack_package_content(export_path, temp_dir)

    shutil.rmtree(temp_dir)
    return export_path


def main():
    if len(sys.argv) != 5:
        print("Required arguments count: 4, but got:", len(sys.argv) - 1)
        sys.exit(1)

    sdk_version = sys.argv[1]
    store_project_id = sys.argv[2]
    login_project_id = sys.argv[3]
    oauth_client_id = sys.argv[4]

    new_package_path = bake_package(sdk_version, store_project_id, login_project_id, oauth_client_id)

    if sys.platform == "darwin":
        os.system(f"open {new_package_path.parent}")

if __name__ == "__main__":
    main()