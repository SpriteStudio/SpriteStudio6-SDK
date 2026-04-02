# SpriteStudio 6 SDK
Copyright © CRI Middleware Co., Ltd.

## Overview
This SDK consists of libraries and tools for flexibly using data (sspj, ssce, ssae, ssee) created with [OPTPiX SpriteStudio 6 or later](http://www.webtech.co.jp/spritestudio/) in your own playback environment.

* sspjLib: A library for parsing SpriteStudio project files (sspj).
    * Loader: Module for loading SpriteStudio 6 format files (sspj, ssce, ssae, ssee).
    * Animator: Animation module that performs coordinate transformation, interpolation calculation, etc.
    * Drawer: Drawing module using OpenGL.
* Tools: Tools created using the library.
    * Viewer: Viewer sample for Windows/Mac.
    * Converter: Converts sspj files into files for playback programs (.ssbp, .json, etc.).

### Intended Use
The purpose of this SDK is to parse sspj files to create players for new platforms, or to create tools for processing and converting data into other formats.
For use in games and apps, we provide player programs specialized for animation control.
We also provide external tools useful for animation editing, so please refer to the links below.
* [OPTPiX Help Center](http://www.webtech.co.jp/help/en/spritestudio/download/forprogrammer/)
* [SpriteStudio Repositories on GitHub](https://github.com/SpriteStudio?tab=repositories)

### Disclaimer
This SDK is maintained with the goal of being a **reference implementation**.
It maintains a certain degree of redundancy without performing optimizations for specific platforms.
Since the `develop` branch is the latest development version, it may contain some bugs.
If you need a stable version, please obtain it from [Releases](https://github.com/SpriteStudio/SpriteStudio6-SDK/releases).

## Update History

- [master](https://github.com/SpriteStudio/SpriteStudio6-SDK/commits/master)
- [develop](https://github.com/SpriteStudio/SpriteStudio6-SDK/commits/develop)

## Supported SpriteStudio Versions
Releases are created in the format **SDK Version_SpriteStudio Version** (e.g., 2.2.0_SS7.1.0).
For details, please see [Releases](https://github.com/SpriteStudio/SpriteStudio6-SDK/releases).

## Tools

- Download
  - [Latest Version](https://github.com/SpriteStudio/SpriteStudio6-SDK/releases/latest)
  - [All Versions](https://github.com/SpriteStudio/SpriteStudio6-SDK/releases)
- Usage
  - [Converter](./docs/how-to-use-converter.md)
  - [Viewer](./docs/viewer.md)
- Others
  - [SS6Player Series and Tools for Engineers](http://www.webtech.co.jp/help/en/spritestudio/download/forprogrammer/)  

## How to Build the SDK
* [How to Build the SDK](./docs/how-to-build-sdk.md)
* [(Reference) How to Build SS6Converter and Viewer](http://www.webtech.co.jp/help/ja/spritestudio/guide/output6/sdk/#viewer_build) _*Note: Jumps to the Help Center page._

## Data Structure and Class Reference
[SpriteStudio6 SDK Document](https://spritestudio.github.io/SpriteStudio6-SDK/html/index.html)

## Branch Rules
SpriteStudioSDK has `master` and `develop` branches. The latest code, including the player, is committed to `develop`.
Once judged to be a stable version, it is merged from `develop` to `master` and assigned a version number.

## Pull Requests
Pull Requests are very welcome!
There is no need to create an Issue before sending a request.
Providing details of the changes in the commit comments is sufficient.

## Open Source Libraries Used
The SDK uses the following libraries.
Please check the links below for the licenses of each library.
* Loader
    * [TinyXML2](http://www.grinninglizard.com/tinyxml/)
* Renderer
    * [OpenGL](http://www.opengl.org/)
    * [glew (Windows)](http://glew.sourceforge.net/)
* Viewer Sample GUI
    * [glfw3](http://www.glfw.org/)
    * [JUCE 7](https://juce.com/)

## SpriteStudio 5 3rdParty
We publish programs provided by the community.
Please read this before use. [SpriteStudio 5 3rdParty](http://spritestudio3rdparty.github.io/)

## Contact
Please post questions, requests, and bug reports to [Issues](https://github.com/SpriteStudio/SpriteStudio6-SDK/issues).
If you prefer to contact us privately, please use the [Help Center](https://www.webtech.co.jp/help/en/spritestudio7/inquiries/ssplayer_tool/).
If you need to send reproduction data, please do so via the Help Center.

CRI Middleware Co., Ltd.
[https://www.cri-mw.co.jp/](https://www.cri-mw.co.jp/)
Copyright © CRI Middleware Co., Ltd.
