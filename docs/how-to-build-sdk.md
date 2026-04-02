[**日本語**](./how-to-build-sdk.ja.md) | [**English**](./how-to-build-sdk.md)

# How to Build the SDK
# Introduction
This section describes the build procedure using a CLI.
For the GUI build procedure, please refer to [How to Build the SDK (GUI)](how-to-build-sdk-gui.md).
# Environment Setup
## Windows
Please install the following:
* Visual Studio 2022 or higher
* PowerShell
* [Qt](https://www.qt.io/download-open-source) (*1, *2)
* [git](https://git-scm.com/downloads)
* [CMake](https://cmake.org/download/)
\*1: Qt version 6.9.0 is recommended. If using other versions, specify the root path of the corresponding Visual Studio version in the environment variable `QT_PREFIX`. (e.g., if using 6.9.1, set `set QT_PREFIX=C:\Qt\6.9.1\msvc2022_64` or `set QT_PREFIX=C:\Qt\6.9.1\msvc2022_arm64`.)
\*2: It is recommended to install both x64 and arm64 versions of Qt.
The following are optional software for faster builds:
* [Ninja](https://ninja-build.org/)
* [sccache](https://github.com/mozilla/sccache)
You can install them using [Scoop](https://scoop.sh/) with the following command:
```
scoop install ninja sccache
```
## macOS
Please install the following:
* Xcode
* [Qt](https://www.qt.io/download-open-source) (*1)
* [git](https://git-scm.com/downloads)
* [CMake](https://cmake.org/download/)
Except for Xcode and Qt, you can install them using [homebrew](https://brew.sh/) with the following command:
```
brew install git cmake
```
\*1: To make Ss6Converter-Qt a Universal Binary 2 (supporting both x64 and arm64), download Qt from the official Qt website. Qt version 6.9.0 is recommended. If using other versions, specify the Qt root path in the environment variable `QT_PREFIX`. (e.g., if using 6.9.1, set `export QT_PREFIX=~/Qt/6.9.1/macos`.)
The following are optional software for faster builds:
* [Ninja](https://ninja-build.org/)
* [sccache](https://github.com/mozilla/sccache)
You can install them using [homebrew](https://brew.sh/) with the following command:
```
brew install ninja sccache
```
# Obtaining Source Code
```
git clone --recursive https://github.com/SpriteStudio/SpriteStudio6-SDK.git
```
# Build Script Arguments
## Arguments for scripts prefixed with `release_`
The build type will be `Release`. The build type cannot be changed.
On Windows, you can specify the architecture of the executable binary in the first argument. `x64` or `arm64` can be set. If not specified, it will be the same as the architecture of the terminal running the build script.
## Arguments for scripts prefixed with `build_`
To make the build type a Debug build, set `Debug` in the first argument. For a Release build, set `Release` in the first argument. If not specified, it will default to `Debug`.
On Windows, you can specify the architecture of the executable binary in the second argument. `x64` or `arm64` can be set. If not specified, it will be the same as the architecture of the terminal running the build script.
# Building All Releases
This is a build for generating release artifacts. It builds all projects.
## Windows
Run `SpriteStudio6-SDK\Scripts\release_win.bat`.
```
.\Scripts\release_win.bat
```
It builds Ss6Converter, Ss6Converter-Qt, and Viewer2 in Release mode and stores the zipped files in `SpriteStudio6-SDK\Tools`.
## macOS
Run `SpriteStudio6-SDK/Scripts/release_macos.sh`.
```
./Scripts/release_macos.sh
```
It builds Ss6Converter, Ss6Converter-Qt, and Viewer2 in Release mode and stores the zipped files in `SpriteStudio6-SDK/Tools`.
# Building Ss6Converter
## Release Build
Builds Ss6Converter (command line) and Ss6Converter-Qt (GUI wrapper).
The default build type is `Release`.
### Windows
Run `SpriteStudio6-SDK\Scripts\release_converter_win.bat`.
The artifact will be `SpriteStudio6-SDK\Tools\Ss6Converter.zip`.
```
.\Scripts\release_converter_win.bat
```
To verify the artifact:
```
powershell Expand-Archive -Force .\Tools\Ss6Converter.zip .\Tools\nstart .\Tools\Ss6Converter
```
### macOS
Run `SpriteStudio6-SDK/Scripts/release_converter_macos.sh`.
The artifact will be `SpriteStudio6-SDK/Tools/Ss6Converter_Mac.zip`.
```
./Scripts/release_converter_macos.sh
```
To verify the artifact:
```
unzip -o ./Tools/Ss6Converter_Mac.zip -d ./Tools
open ./Tools/Ss6Converter
```
## Development Build
The default build type is `Debug`.
### Windows
To build Ss6Converter, run `SpriteStudio6-SDK\Scripts\build_converter_win.bat`.
The build working directory will be `SpriteStudio6-SDK\Build\Converter\build`.
```
.\Scripts\build_converter_win.bat
```
To verify the artifact:
```
.\Build\Converter\build\Debug\Ss6Converter.exe -h
```
To build Ss6Converter-Qt, run `SpriteStudio6-SDK\Scripts\build_convertergui_win.bat`.
The build working directory will be `SpriteStudio6-SDK\Build\Ss6ConverterGUI\build`.
```
.\Scripts\build_convertergui_win.bat
```
To verify the artifact (Note: It cannot convert because Ss6Converter is not included):
```
.\Build\Ss6ConverterGUI\Ss6ConverterGUI\build\Debug\Ss6ConverterGUI.exe
```
### macOS
To build Ss6Converter (command line), run `SpriteStudio6-SDK/Scripts/build_converter_macos.sh`.
The build working directory will be `SpriteStudio6-SDK/Build/Converter/build`.
```
./Scripts/build_converter_macos.sh
```
To verify the artifact:
```
./Build/Converter/build/Ss6Converter -h
```
To build Ss6Converter-Qt, run `SpriteStudio6-SDK/Scripts/build_convertergui_macos.sh`.
The build working directory will be `SpriteStudio6-SDK/Build/Ss6ConverterGUI/build`.
```
./Scripts/build_convertergui_macos.sh
```
To verify the artifact (Note: It cannot convert because Ss6Converter is not included):
```
open ./Build/Ss6ConverterGUI/Ss6ConverterGUI/build/Ss6ConverterGUI.app
```
# Building Viewer2
### Release Build
Builds Viewer2.
The default build type is `Release`.
#### Windows
To create a zip archive of the packaged SSViewer2, run `SpriteStudio6-SDK/Scripts/release_viewer2_win.bat`.
The artifact will be `SpriteStudio6-SDK/Tools/viewer_sample_2_Win.zip`.
The default build type is Release.
```
./Scripts/release_viewer2_win.bat
```
To verify the artifact:
```
powershell Expand-Archive -Force .\Tools\viewer_sample_2_Win.zip .\Tools\nstart .\Tools\viewer_sample_2_Win
```
#### macOS
To create a zip archive of the packaged SSViewer2, run `SpriteStudio6-SDK/Scripts/release_viewer2_macos.sh`.
The artifact will be `SpriteStudio6-SDK/Tools/viewer_sample_2_Mac.zip`.
The default build type is Release.
```
./Scripts/release_viewer2_macos.sh
```
To verify the artifact:
```
unzip -o ./Tools/viewer_sample_2_Mac.zip -d ./Tools
open ./Tools/viewer_sample_2_Mac
```
### Development Build
The default build type is `Debug`.
#### Windows
Run `SpriteStudio6-SDK\Scripts\build_viewer2_win.bat`.
Builds in `SpriteStudio6-SDK\Build\Viewer2\cmakeBuild`.
```
.\Scripts\build_viewer2_win.bat
start Build\Viewer2\cmakeBuild\SSView2_artefacts\Debug\SSViewer2.app
```
#### macOS
Run `SpriteStudio6-SDK/Scripts/build_viewer2_macos.sh`.
Builds in `SpriteStudio6-SDK/Build/Viewer2/cmakeBuild`.
```
./Scripts/build_viewer2_macos.sh
open Build/Viewer2/cmakeBuild/SSView2_artefacts/Debug/SSViewer2.app
```
