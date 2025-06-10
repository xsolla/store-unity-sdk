import os
import sys
import shutil
import re
from pathlib import Path
from typing import List

def get_script_dir() -> Path:
    return Path(__file__).resolve().parent


def get_unitypackage_files() -> List[Path]:
    folder_path = get_script_dir().parent
    return list(folder_path.glob("*.unitypackage"))


def get_unpacked_asset_path(dir: Path) -> Path:
    return dir / "packagemanagermanifest" / "asset"
    

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


def modify_asset_content(file_path: Path):
    content = '{ "dependencies": {"com.unity.nuget.newtonsoft-json": "3.2.1"} }'
    file_path.write_text(content, encoding="utf-8")


def pack_package_content(export_path: Path, source_dir: Path) -> None:
    shutil.make_archive(str(export_path), 'tar', str(source_dir))
    tar_path = export_path.with_suffix(export_path.suffix + '.tar')
    shutil.move(str(tar_path), str(export_path))


def export_packages():
    packages = get_unitypackage_files()    
    for source_package_path in packages:   
        print(source_package_path)

       # Unpack the source package to temporary directory
        temp_dir = create_temp_dir()
        unpack_package_content(source_package_path, temp_dir)

        # Modify the asset
        asset_path = get_unpacked_asset_path(temp_dir)
        modify_asset_content(asset_path)

        # Create the export package with the modified asset
        pack_package_content(source_package_path, temp_dir)
        shutil.rmtree(temp_dir)


def main():
    export_packages()

if __name__ == "__main__":
    main()