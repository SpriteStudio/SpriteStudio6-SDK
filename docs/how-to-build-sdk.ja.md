# SDKのビルド方法
# はじめに
ここでは CLI でのビルド手順について記載します。
GUI でのビルド手順については [SDKのビルド方法(GUI編)](how-to-build-sdk-gui.ja.md)を参照してください。
# 環境構築
## Windows
下記をインストールしてください。
* Visual Studio 2022 以上
* PowerShell
* [Qt](https://www.qt.io/download-open-source) (*1, *2)
* [git](https://git-scm.com/downloads)
* [CMake](https://cmake.org/download/)
\*1: Qt のバージョンは 6.9.0 を推奨しています。それ以外のバージョンを利用する場合は環境変数 `QT_PREFIX` にて 該当バージョンの Visual Studio の root path を指定してください。(e.g. 6.9.1 を利用する場合は `set QT_PREFIX=C:\Qt\6.9.1\msvc202_64` か `set QT_PREFIX=C:\Qt\6.9.1\msvc2022_arm64` と設定してください。)
\*2: x64 と arm64 の Qt を入れることを推奨しています。
下記はオプショナルになります。ビルド高速化の為のソフトウェアです。
* [Ninja](https://ninja-build.org/)
* [sccache](https://github.com/mozilla/sccache)
[Scoop](https://scoop.sh/) を利用すれば下記コマンドでインストール可能です。
```
scoop install ninja sccache
```
## macOS
下記をインストールしてください。
* Xcode
* [Qt](https://www.qt.io/download-open-source) (*1)
* [git](https://git-scm.com/downloads)
* [CMake](https://cmake.org/download/)
Xcode と Qt 以外は [homebrew](https://brew.sh/) を利用すれば下記コマンドでインストール可能です。
```
brew install git cmake
```
\*1: Ss6Converter-Qt を Universal Binary 2 (x64 と arm64 両方対応)にするには Qt 本家から Qt をダウンロードしてください。 Qt のバージョンは 6.9.0 を推奨しています。それ以外のバージョンを利用する場合は環境変数 `QT_PREFIX` にて Qt の root path を指定してください。(e.g. 6.9.1 を利用する場合は `export QT_PREFIX=~/Qt/6.9.1/macos` と設定してください。)
下記はオプショナルになります。ビルド高速化の為のソフトウェアです。
* [Ninja](https://ninja-build.org/)
* [sccache](https://github.com/mozilla/sccache)
[homebrew](https://brew.sh/) を利用すれば下記コマンドでインストール可能です。
```
brew install ninja sccache
```
# ソース取得
```
git clone --recursive https://github.com/SpriteStudio/SpriteStudio6-SDK.git
```
# ビルドスクリプトの引数
## 接頭辞が release_ のビルドスクリプトの引数
ビルドタイプは `Release` になります。ビルドタイプの変更不可です
Windows では第一引数で実行バイナリのアーキテクチャを指定できます。 `x64` か `arm64` を設定できます。指定がない場合はビルドスクリプトを実行した端末のアーキテクチャと同じものになります。
## 接頭辞が build_ のビルドスクリプトの引数
ビルドタイプをデバッグビルドにする場合は第一引数に `Debug` を設定してください。リリースビルドする場合は第一引数に `Release` を設定してください。指定がない場合は `Debug` になります。
Windows では第二引数で実行バイナリのアーキテクチャを指定できます。 `x64` か `arm64` を設定できます。指定がない場合はビルドスクリプトを実行した端末のアーキテクチャと同じものになります。
# 全リリースビルド
リリース物を生成するビルドです。全プロジェクトをビルドします。
## Windows
`SpriteStudio6-SDK\Scripts\release_win.bat` を実行します。
```
.\Scripts\release_win.bat
```
Ss6Converter, Ss6Converter-Qt と Viewer2 をリリースビルドし、zip 圧縮したファイルを `SpriteStudio6-SDK\Tools` に格納します。
## macOS
`SpriteStudio6-SDK/Scripts/release_macos.sh` を実行します。
```
./Scripts/release_macos.sh
```
Ss6Converter, Ss6Converter-Qt と Viewer2 をリリースビルドし、zip 圧縮したファイルを `SpriteStudio6-SDK/Tools` に格納します。
# Ss6Converter ビルド
## リリースビルド
Ss6Converter(コマンドライン) と Ss6Converter-Qt(GUI ラッパー) をビルドします。
デフォルトビルドタイプは `Release` になります。
### Windows
`SpriteStudio6-SDK\Scripts\release_converter_win.bat` を実行します。
成果物は `SpriteStudio6-SDK\Tools\Ss6Converter.zip` となります。
```
.\Scripts\release_converter_win.bat
```
成果物の確認方法は下記になります。
```
powershell Expand-Archive -Force .\Tools\Ss6Converter.zip .\Tools\nstart .\Tools\Ss6Converter
```
### macOS
`SpriteStudio6-SDK/Scripts/release_converter_macos.sh` を実行します。
成果物は `SpriteStudio6-SDK/Tools/Ss6Converter_Mac.zip` となります。
```
./Scripts/release_converter_macos.sh
```
成果物の確認方法は下記になります。
```
unzip -o ./Tools/Ss6Converter_Mac.zip -d ./Tools
open ./Tools/Ss6Converter
```
## 開発ビルド
デフォルトビルドタイプは `Debug` になります。
### Windows
Ss6Converter のビルドは `SpriteStudio6-SDK\Scripts\build_converter_win.bat` を実行します。
ビルド作業ディレクトリは `SpriteStudio6-SDK\Build\Converter\build` となります。
```
.\Scripts\build_converter_win.bat
```
成果物の確認方法は下記になります。
```
.\Build\Converter\build\Debug\Ss6Converter.exe -h
```
Ss6Converter-Qt のビルドは `SpriteStudio6-SDK\Scripts\build_convertergui_win.bat` を実行します。
ビルド作業ディレクトリは `SpriteStudio6-SDK\Build\Ss6ConverterGUI\build` となります。
```
.\Scripts\build_convertergui_win.bat
```
成果物の確認方法は下記になります。ただし、Ss6Converter を同封していないので変換できません。
```
.\Build\Ss6ConverterGUI\Ss6ConverterGUI\build\Debug\Ss6ConverterGUI.exe
```
### macOS
Ss6Converter (コマンドライン)のビルドは`SpriteStudio6-SDK/Scripts/build_converter_macos.sh` を実行します。
ビルド作業ディレクトリは `SpriteStudio6-SDK/Build/Converter/build` となります。
```
./Scripts/build_converter_macos.sh
```
成果物の確認方法は下記になります。
```
./Build/Converter/build/Ss6Converter -h
```
Ss6Converter-Qt のビルドは `SpriteStudio6-SDK/Scripts/build_convertergui_macos.sh` を実行します。
ビルド作業ディレクトリは `SpriteStudio6-SDK/Build/Ss6ConverterGUI/build` となります。
```
./Scripts/build_convertergui_macos.sh
```
成果物の確認方法は下記になります。ただし、Ss6Converter を同封していないので変換できません。
```
open ./Build/Ss6ConverterGUI/Ss6ConverterGUI/build/Ss6ConverterGUI.app
```
# Viewer2 ビルド
### リリースビルド
Viewer2 をビルドします。
デフォルトビルドタイプは `Release` になります。
#### Windows
SSViewer2 をパッケージングした zip アーカイブファイルを作るには `SpriteStudio6-SDK/Scripts/release_viewer2_win.bat` を実行します。
成果物は `SpriteStudio6-SDK/Tools/viewer_sample_2_Win.zip` となります。
デフォルトビルドタイプは Release になります。
```
./Scripts/release_viewer2_win.bat
```
成果物の確認方法は下記になります。
```
powershell Expand-Archive -Force .\Tools\viewer_sample_2_Win.zip .\Tools\nstart .\Tools\viewer_sample_2_Win
```
#### macOS
SSViewer2 をパッケージングした zip アーカイブファイルを作るには `SpriteStudio6-SDK/Scripts/release_viewer2_macos.sh` を実行します。
成果物は `SpriteStudio6-SDK/Tools/viewer_sample_2_Mac.zip` となります。
デフォルトビルドタイプは Release になります。
```
./Scripts/release_viewer2_macos.sh
```
成果物の確認方法は下記になります。
```
unzip -o ./Tools/viewer_sample_2_Mac.zip -d ./Tools
open ./Tools/viewer_sample_2_Mac
```
### 開発ビルド
デフォルトビルドタイプは `Debug` になります。
#### Windows
`SpriteStudio6-SDK\Scripts\build_viewer2_win.bat` を実行します。
`SpriteStudio6-SDK\Build\Viewer2\cmakeBuild` にてビルドします。
```
.\Scripts\build_viewer2_win.bat
start Build\Viewer2\cmakeBuild\SSView2_artefacts\Debug\SSViewer2.app
```
#### macOS
`SpriteStudio6-SDK/Scripts/build_viewer2_macos.sh` を実行します。
`SpriteStudio6-SDK/Build/Viewer2/cmakeBuild` にてビルドします。
```
./Scripts/build_viewer2_macos.sh
open Build/Viewer2/cmakeBuild/SSView2_artefacts/Debug/SSViewer2.app
```
