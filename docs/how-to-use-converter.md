[**日本語**](./how-to-use-converter.ja.md) | [**English**](./how-to-use-converter.md)

# How to Use the Converter
# Ss6Converter
## Notices
* 2021/04/07 We have released a new converter that significantly reduces the output data size. Please also check [Ss6ConverterVer2](https://github.com/SpriteStudio/Ss6ConverterVer2/wiki).
* 2019/02/25 The feature to output in C language source file format is planned to be removed in the near future. This is because its use cases are limited, and it is currently considered not worth the maintenance cost. If you are a user who wish to continue using this with the latest SDK, please contact us from [here](../README.md#contact).
## Overview
Ss6Converter is a tool for converting animation data created with OPTPiX SpriteStudio 6 into formats for runtimes (ssbp, json, ssfb).
The data output by this converter is for general purposes and is not intended for a specific player only.
Since the [source files](https://github.com/SpriteStudio/SpriteStudio6-SDK/tree/master/Build/Converter) are public, you can customize it freely.
## How to Obtain
Please download the Ss6Converter archive for your platform from [Releases](https://github.com/SpriteStudio/SpriteStudio6-SDK/releases).
The latest version is [here](https://github.com/SpriteStudio/SpriteStudio6-SDK/releases/latest).
## Confirmed Operating Environments
* Confirmed OS versions
* Windows version: 10
## Usage Procedures
### Using the GUI
![2019-09-26_15h45_48](https://user-images.githubusercontent.com/5117608/65668943-a11bd380-e07d-11e9-8268-e05d2281f917.png)
1. Run `Ss6ConverterGUI`.
2. Drag and drop the sspj files you want to convert to register them.
3. Select the required file format. (Choose the format supported by the player program you are using)
4. Press the `Convert Start` button to begin conversion.
5. When the status shows "Convert Success!", files with the same name and the selected extension (.ssbp/.json/.ssfb, etc.) will be created in the sspj folder.
### Using the Command Line
Start the command prompt and execute it as follows:
`Ss6Converter **Full Path** of the target sspj file [Options]...`
#### Options
* -o `Output folder path`
	+ If omitted, it will be output to the same folder as the sspj file.
* -f `Specify output file format`
	+ Specify one of ssbp, json, or ssfb. *As of 2019/09/26
	+ Select the format supported by the player program you are using.
#### Execution Example
```
Ss6Converter C:\ss_anime\test.sspj -o D:\output_path\ -f ssfb
```
Converts the `C:\ss_anime\test.sspj` file and the ssae, ssce, and ssee files registered in this sspj file into a single ssfb file, and saves it in the `D:\output_path\` folder.
Upon successful conversion, a file with the same name and the selected extension (.ssbp/.json/.ssfb, etc.) will be created in the output folder.

## About Output Files
The output file contains all converted *.ssae, *.ssce, and *.ssee files registered in test.sspj.
Reference images (image files) are not included, so please place them in an appropriate location when using them on the player side.

## Compatibility between ssbp Files and Players
ssbp files have format versions, and if changes occur due to updates, they may become unplayable on the player you are using.
Referring to the table below, please obtain the SDK corresponding to the SpriteStudio version supported by your player and use the converter included in it.

**Users of SS6Player for Cocos2d-x and ssbpLib should be particularly careful.**

| ssbp Format Version | [SS6 SDK Version](https://github.com/SpriteStudio/SpriteStudio6-SDK/releases) | [SpriteStudio](https://www.webtech.co.jp/help/en/spritestudio/download/ss6download/) | Remarks |
| --- | --- | --- | --- |
| 11 | 1.6.x | 6.2.0 | |
| 10 | 1.5.x | 6.2.0 | |
| 9 | 1.4.x | 6.1.3 | |
| 8 | 1.3.x | 6.1 | |
| 7 | 1.2.x | 6.0 | |
| 6 | 1.1.x | 6.0 | |
| 5 | 1.0.x | 6.0 | *Does not support mesh data. |
* How to deal with cases where animation cannot be played due to different ssbp file versions:
	+ Obtain and replace with the latest version of the player, or merge the differences. All animation data needs to be re-converted.
	+ Use a combination of a player and converter that have been confirmed to work, and merge only the parts of the player that need updating.

## Precautions for Data Creation
* Do not use double-byte characters in file names.
* Do not use double-byte characters in cell names, part names, or label names.
* 16-color PNG is not supported. It may be converted to 16-color PNG if saved with compression.
If conversion is not possible, try saving without compression.
* Always register one cell list in the cell map.
Without a cell list, the player will fail to parse the ssbp file.

## File Format
- [ssbp Format](binary-file-format.md)
- [sspkg Format](sspkg.md)

## How to Build the Converter
* [Build Procedure](how-to-build-sdk.md)
