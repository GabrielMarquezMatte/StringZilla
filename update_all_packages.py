from xml.etree import ElementTree as ET
import aiofiles
import asyncio
import aiohttp
import argparse


def get_package_name(package_reference: ET.Element) -> str | None:
    return package_reference.attrib.get("Include", None)


async def get_package_version(session: aiohttp.ClientSession, package_name: str) -> str:
    async with session.get(
        f"https://api.nuget.org/v3-flatcontainer/{package_name}/index.json"
    ) as response:
        data = await response.json()
        # Return the last version without preview tag
        return str(data["versions"][-1])


async def update_package_version(
    session: aiohttp.ClientSession, package_reference: ET.Element
) -> None:
    package_name = package_reference.attrib.get("Include", None)
    if not package_name:
        return
    new_version = await get_package_version(session, package_name)
    package_reference.attrib["Version"] = new_version


async def update_all_packages(session: aiohttp.ClientSession, file_path: str) -> None:
    async with aiofiles.open(file_path, mode="r+") as f:
        content = await f.read()
        parser = ET.XMLParser(encoding="UTF-8")
        root = ET.fromstring(content, parser=parser)
        async with asyncio.TaskGroup() as group:
            for package_reference in root.findall(".//PackageReference"):
                group.create_task(update_package_version(session, package_reference))
        await f.seek(0)
        await f.write(ET.tostring(root, encoding="utf-8").decode())


async def main() -> None:
    # Get the arguments
    parser = argparse.ArgumentParser(description="Update all packages in project files")
    parser.add_argument(
        "project_files", help="The project files to update, separated by commas"
    )
    args = parser.parse_args()
    async with aiohttp.ClientSession() as session:
        async with asyncio.TaskGroup() as group:
            for project_file in args.project_files.split(","):
                group.create_task(update_all_packages(session, project_file))


if __name__ == "__main__":
    asyncio.run(main())
