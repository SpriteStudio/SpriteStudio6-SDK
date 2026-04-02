[**日本語**](./plistConverter.ja.md) | [**English**](./plistConverter.md)

# plistConverter
## Overview
plistConverter is a tool for converting cell information (plist) output by the sprite sheet feature of OPTPiX SpriteStudio 6 into an XML format that is easy to use with the PlayStation®Mobile SDK.
Since the source files are public, you can customize it freely.
* Disclaimer
This software is an **evaluation version** distributed for the purpose of operation verification and evaluation. Please use it with the understanding that it may contain some bugs.
## How to Obtain
It can be used on Windows.
The executable file is included in `tools/plistConverter.zip`.
The source files are included in `Build/plistConverter`.
To build, open `plistConverter.sln` inside `plistConverter_src.zip` with Visual Studio Express 2013 for Windows Desktop.
## Usage Procedures
* Creating a Sprite Sheet in SpriteStudio 6
Select Menu > OPTPiX Labs > Generate Sprite Sheet.
Check "Trim Transparent".
Set "Output Format" to "Cocos2d-x".
Set the texture size, etc., and press the "Export" button.
A plist and a png will be output.
* Converting the plist
Start the command prompt and run `plistConverter.exe`.
Set the path of the plist in the first argument.
An XML file will be created in the same folder.
* Integration into a Game
Add the created XML and PNG files to your project.
We have released a sample program that displays images from the output XML file.
[Please refer to this for details.](https://github.com/SpriteStudio/SS5PlayerForPlayStationMobile/wiki/textuerpack)
