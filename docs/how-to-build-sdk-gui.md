# How to Build the SDK (GUI)
This section describes the build procedure using a GUI.
It includes steps for setting up the development environment in a GUI.
# Environment Setup
## Windows
Please install the following:
* Visual Studio 2022 or higher
* PowerShell
* [Qt](https://www.qt.io/download-open-source) (*1)(*2)
* [git](https://git-scm.com/downloads)
* [CMake](https://cmake.org/download/)
* [JUCE](https://juce.com/get-juce/download)
\*1: Qt version 6.9.0 is recommended. If using other versions, specify the root path of the corresponding Visual Studio version in the environment variable `QT_PREFIX`. (e.g., if using 6.9.1, set `set QT_PREFIX=C:\Qt\6.9.1\msvc2022_64` or `set QT_PREFIX=C:\Qt\6.9.1\msvc2022_arm64`.)
\*2: Please install Qt Creator.
## macOS
Please install the following:
* Xcode
* [Qt](https://www.qt.io/download-open-source) (*3)
* [git](https://git-scm.com/downloads)
* [CMake](https://cmake.org/download/)
* [JUCE](https://juce.com/get-juce/download)
Except for Xcode and Qt, you can install them using [homebrew](https://brew.sh/) with the following command:
```
brew install git cmake
```
\*3: To make Ss6Converter-Qt a Universal Binary 2 (supporting both x64 and arm64), download Qt from the official Qt website. Qt version 6.9.0 is recommended. If using other versions, specify the Qt root path in the environment variable `QT_PREFIX`. (e.g., if using 6.9.1, set `export QT_PREFIX=~/Qt/6.9.1/macos`.)
# Obtaining Source Code
```
git clone --recursive https://github.com/SpriteStudio/SpriteStudio6-SDK.git
```
# Windows
## Building Ss6Converter
### Creating Folders
`SpriteStudio6-SDK` refers to the path of the checked-out SDK folder.
Create the `SpriteStudio6-SDK\Build\build` folder.
### Run CMake (cmake-gui)
Set the absolute path of `SpriteStudio6-SDK\Build` in `Where is the source code:`.
Set the absolute path of `SpriteStudio6-SDK\Build\build` in `Where to build the binaries:`.
### Generating Solution File
Press the Configure button.
A dialog to select the project format will appear; select `Visual Studio 17 2022` and press Finish.
Press the Generate button. If successful, a Visual Studio project will be generated in `SpriteStudio6-SDK\Build\build`.
### Building Visual Studio Solution
Open `SpriteStudio6-SDK\Build\build\sssdk.sln`.
Select `ALL_BUILD` as the build target and build it.
The `Ss6Converter.exe` binary will be generated in `SpriteStudio6-SDK\Build\build\Converter\{Debug|Release}`.
Running `RUN_TEST` will execute a conversion test for sspj files under `SpriteStudio6-SDK\Build\TestData`.
## Building Ss6Converter GUI
[Qt](https://www.qt.io/download-open-source) and Qt Creator are required to set up the development environment for Ss6Converter GUI.
Since Ss6Converter GUI is a GUI wrapper for Ss6Converter, the Ss6Converter binary is required.
Please build it in advance.
### Run Qt Creator
Open `SpriteStudio6-SDK\Build\Ss6ConverterGUI\Ss6ConverterGUI\Ss6ConverterGUI.pro` in Qt Creator.
### Build in Qt Creator
Select `Build -> Build All Projects` to build.
The `SpriteStudio6-SDK\Build\Ss6ConverterGUI\build-Ss6ConverterGUI-XXX-{Debug|Release}` directory will be generated, and the Ss6Converter GUI binary will be generated in `SpriteStudio6-SDK\Build\Ss6ConverterGUI\build-Ss6ConverterGUI-XXX-{Debug|Release}\Ss6ConverterGUI.app`.
To run it, place the `Ss6Converter` binary into `SpriteStudio6-SDK\Build\Ss6ConverterGUI\build-Ss6ConverterGUI-XXX-{Debug|Release}`.
## Building Viewer2
[JUCE](https://juce.com/get-juce/download) is required for setting up the development environment for Viewer2.
### Run JUCE Projucer.exe
Run Projucer.exe and open `SpriteStudio6-SDK\Build\Viewer2\Viewer2.jucer`.
### Generating Visual Studio Solution
Select `Visual Studio 2022` in `Selected exporter` and click the Visual Studio icon. An sln file will be generated and Visual Studio will start.
The sln file is generated at `SpriteStudio6-SDK\Build\Viewer2\Builds\VisualStudio2022\SSViewer2.sln`.
### Building Solution File
Build `SSViewer2_App` in `SpriteStudio6-SDK\Build\Viewer2\Builds\VisualStudio2022\SSViewer2.sln`.
# macOS
## Building Ss6Converter
### Creating Folders
`SpriteStudio6-SDK` refers to the path of the checked-out SDK folder.
Create the `SpriteStudio6-SDK/Build/build` folder.
### Run CMake (cmake-gui)
Set the absolute path of `SpriteStudio6-SDK/Build` in `Where is the source code:`.
Set the absolute path of `SpriteStudio6-SDK/Build/build` in `Where to build the binaries:`.
### Generating xcodeproject
Press the Configure button.
A dialog to select the project format will appear; select Xcode and press Done.
If successful, an xcodeproject will be generated in `SpriteStudio6-SDK/Build/build`.
### Building xcodeproject
Open `SpriteStudio6-SDK/Build/build/sssdk.xcodeproj`.
Select `ALL_BUILD` as the build target and build it.
The `Ss6Converter` binary will be generated in `SpriteStudio6-SDK/Build/build/Converter/{Debug|Release}`.
Running `RUN_TEST` will execute a conversion test for sspj files under `SpriteStudio6-SDK/Build/TestData`.
## Building Ss6Converter GUI
[Qt](https://www.qt.io/download-open-source) and Qt Creator are required to set up the development environment for Ss6Converter GUI.
Since Ss6Converter GUI is a GUI wrapper for Ss6Converter, the Ss6Converter binary is required.
Please build it in advance.
### Run Qt Creator
Open `SpriteStudio6-SDK/Build/Ss6ConverterGUI/Ss6ConverterGUI/Ss6ConverterGUI.pro` in Qt Creator.
### Build in Qt Creator
Select `Build -> Build All Projects` to build.
The `SpriteStudio6-SDK/Build/Ss6ConverterGUI/build-Ss6ConverterGUI-XXX-{Debug|Release}` directory will be generated, and the Ss6Converter GUI binary will be generated in `SpriteStudio6-SDK/Build/Ss6ConverterGUI/build-Ss6ConverterGUI-XXX-{Debug|Release}/Ss6ConverterGUI.app`.
To run it, place the `Ss6Converter` binary into `SpriteStudio6-SDK/Build/Ss6ConverterGUI/build-Ss6ConverterGUI-XXX-{Debug|Release}/Ss6Converter`.
## Building Viewer2
[JUCE](https://juce.com/get-juce/download) is required for setting up the development environment for Viewer2.
### Run JUCE Projucer.app
Run Projucer.app and open `SpriteStudio6-SDK/Build/Viewer2/Viewer2.jucer`.
### Generating xcodeproject
Select `Xcode (MacOSX)` in `Selected exporter` and click the Xcode icon. An xcodeproject will be generated and Xcode will start.
The xcodeproject is generated at `SpriteStudio6-SDK/Build/Viewer2/Builds/MacOSX/SSViewer2.xcodeproj`.
### Building xcodeproject
Build `SSViewer2 - App` in `SpriteStudio6-SDK/Build/Viewer2/Builds/MacOSX/SSViewer2.xcodeproj`.
