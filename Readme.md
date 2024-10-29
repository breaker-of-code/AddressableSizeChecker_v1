# Asset & Addressable Size Checker

## Overview
The **Asset & Addressable Size Checker** is a Unity Editor tool designed to help developers estimate the size of their assets or addressables in the build. It provides two tabs:

1. **Asset Size**: Allows you to select assets or folders to estimate their total size in the build.
2. **Addressable Size**: Allows you to select an addressable asset to estimate its size in the build.

## Features
- **Multiple Asset Selection**: Select multiple assets or folders and calculate their combined size.
- **Addressable Support**: Specifically estimate the size of an addressable asset.
- **Easy to Use**: Simple GUI for adding/removing assets or folders and calculating their build sizes.

## Installation
1. Copy the `AssetAddressableSizeChecker.cs` script into an `Editor` folder in your Unity project.
2. Open Unity and navigate to `Tools > Asset & Addressable Size Checker` to open the tool.

## How to Use
1. **Asset Size Tab**:
   - Use the **Add Asset/Folder** button to add assets or folders to the list.
   - Click **Calculate Total Size** to estimate the total build size of all selected assets/folders.
2. **Addressable Size Tab**:
   - Select an addressable asset using the provided field.
   - Click **Calculate Addressable Size** to estimate its build size.

## Notes
- The calculated size is an estimate based on the file sizes of all asset dependencies.
- The tool does not account for build-time optimizations or compression applied by Unity.

## Requirements
- Unity version 2019.4 or later.

## License
This tool is provided "as is" without warranty of any kind.

