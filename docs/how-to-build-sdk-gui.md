# SDKのビルド方法(GUI編)
ここでは、GUI でのビルド手順について記載します。
GUI での開発環境の構築手順を含みます。
# 環境構築
## Windows
下記をインストールしてください。
* Visual Studio 2022 以上
* PowerShell
* [Qt](https://www.qt.io/download-open-source) (*1)(*2)
* [git](https://git-scm.com/downloads)
* [CMake](https://cmake.org/download/)
* [JUCE](https://juce.com/get-juce/download)
\*1: Qt のバージョンは 6.9.0 を推奨しています。それ以外のバージョンを利用する場合は環境変数 `QT_PREFIX` にて 該当バージョンの Visual Studio の root path を指定してください。(e.g. 6.9.1 を利用する場合は `set QT_PREFIX=C:\Qt\6.9.1\msvc2022_64` か `set QT_PREFIX=C:\Qt\6.9.1\msvc2022_arm64` と設定してください。)
\*2: Qt Creator をインストールしてください
## macOS
下記をインストールしてください。
* Xcode
* [Qt](https://www.qt.io/download-open-source) (*3)
* [git](https://git-scm.com/downloads)
* [CMake](https://cmake.org/download/)
* [JUCE](https://juce.com/get-juce/download)
Xcode と Qt 以外は [homebrew](https://brew.sh/) を利用すれば下記コマンドでインストール可能です。
```
brew install git cmake
```
\*3: Ss6Converter-Qt を Universal Binary 2 (x64 と arm64 両方対応)にするには Qt 本家から Qt をダウンロードしてください。Qt のバージョンは 6.9.0 を推奨しています。それ以外のバージョンを利用する場合は環境数 `QT_PREFIX` にて Qt の root path を指定してください。(e.g. 6.9.1 を利用する場合は `export QT_PREFIX=~/Qt/6.9.1/macos` と設定してください。)
# ソース取得
```
git clone --recursive https://github.com/SpriteStudio/SpriteStudio6-SDK.git
```
# Windows
## Ss6Converter のビルド
### フォルダの作成
`SpriteStudio6-SDK` はチェックアウトしたSDKフォルダのパスになります。
`SpriteStudio6-SDK\Build\build` フォルダを作成します。
### Cmake(cmake-gui)を実行
`Where is the source code:` に `SpriteStudio6-SDK\Build` の絶対パスを設定します。
`Where to build the binaries:` に `SpriteStudio6-SDK\Build\build` の絶対パスを設定します。
### Solution file の生成
Configure ボタンを押下します。
生成するプロジェクトのフォーマットを選択するダイアログが表示されるので `Visual Studio 16 2022` を選択し、 Finish ボタンを押します。
Generate ボタンを押下し、成功すれば `SpriteStudio6-SDK\Build\build` に Visual Studio のプロジェクト が生成されます。
### Visual Studio Solution のビルド
`SpriteStudio6-SDK\Build\build\sssdk.sln` を開きます。
ビルドターゲットを `ALL_BUILD` を選択しビルドします。
`SpriteStudio6-SDK\Build\build\Converter\{Debug|Release}` に `Ss6Converter.exe` のバイナリが生成されます。
`RUN_TEST` を実行すると、`SpriteStudio6-SDK\Build\TestData` 以下の sspj ファイルのコンバートの動作確認が実行されます。
## Ss6Converter GUI のビルド
Ss6Converter GUI の開発環境構築には [Qt](https://www.qt.io/download-open-source) と Qt Creator が必要となります。
Ss6Converter GUI は Ss6Converter の GUI ラッパーなので、 Ss6Converter バイナリが必要となります。
事前にビルドしてください。
### Qt Creator を実行
Qt Creator で `SpriteStudio6-SDK\Build\Ss6ConverterGUI\Ss6ConverterGUI\Ss6ConverterGUI.pro` を開きます。
### Qt Creator でビルド
`ビルド -> Build All Projects` を選択肢ビルドします。
`SpriteStudio6-SDK\Build\Ss6ConverterGUI\build-Ss6ConverterGUI-XXX-{Debug|Release}` ディレクトリが生成され、
Ss6Converter GUI のバイナリは `SpriteStudio6-SDK\Build\Ss6ConverterGUI\build-Ss6ConverterGUI-XXX-{Debug|Release}\Ss6ConverterGUI.app` に生成されます。
実行するには, `Ss6Converter` のバイナリを `SpriteStudio6-SDK\Build\Ss6ConverterGUI\build-Ss6ConverterGUI-XXX-{Debug|Release}` へ配置してください。
## Viewer2 のビルド
Viewer2 の開発環境構築には [JUCE](https://juce.com/get-juce/download) が必要になります。
### JUCE Projucer.exe を実行
Projucer.app を実行し、 `SpriteStudio6-SDK\Build\Viewer2\Viewer2.jucer` を開きます。
### Visual Studio Solution の生成
`Selected exporter` で `Visual Studio 2022` を選択し、 Visual Studio のアイコンをクリックすると、 sln ファイルが生成され Visual Studio が起動します。
sln ファイルは `SpriteStudio6-SDK\Build\Viewer2\Builds\VisualStudio2022\SSViewer2.sln` に生成されます。
### Solution file のビルド
`SpriteStudio6-SDK\Build\Viewer2\Builds\VisualStudio2022\SSViewer2.sln`　の `SSViewer2_App` をビルドしてください。
# macOS
## Ss6Converter のビルド
### フォルダの作成
`SpriteStudio6-SDK` はチェックアウトしたSDKフォルダのパスになります。
`SpriteStudio6-SDK/Build/build` フォルダを作成します。
### Cmake(cmake-gui)を実行
`Where is the source code:` に `SpriteStudio6-SDK/Build` の絶対パスを設定します。
`Where to build the binaries:` に `SpriteStudio6-SDK/Build/build` の絶対パスを設定します。
### xcodeproject の生成
configure ボタンを押下します。
生成するプロジェクトのフォーマットを選択するダイアログが表示されるので Xcode を選択し、 Done ボタンを押します。
成功すれば `SpriteStudio6-SDK/Build/build` に xcodeproject が生成されます。
### xcodeproject のビルド
`SpriteStudio6-SDK/Build/build/sssdk.xcodeproj` を開きます。
ビルドターゲットを `ALL_BUILD` を選択しビルドします。
`SpriteStudio6-SDK/Build/build/Converter/{Debug|Release}` に `Ss6Converter` のバイナリが生成されます。
`RUN_TEST` を実行すると、`SpriteStudio6-SDK/Build/TestData` 以下の sspj ファイルのコンバートの動作確認が実行されます。
## Ss6Converter GUI のビルド
Ss6Converter GUI の開発環境構築には [Qt](https://www.qt.io/download-open-source) と Qt Creator が必要となります。
Ss6Converter GUI は Ss6Converter の GUI ラッパーなので、 Ss6Converter バイナリが必要となります。
事前にビルドしてください。
### Qt Creator を実行
Qt Creator で `SpriteStudio6-SDK/Build/Ss6ConverterGUI/Ss6ConverterGUI/Ss6ConverterGUI.pro` を開きます。
### Qt Creator でビルド
`ビルド -> Build All Projects` を選択肢ビルドします。
`SpriteStudio6-SDK/Build/Ss6ConverterGUI/build-Ss6ConverterGUI-XXX-{Debug|Release}` ディレクトリが生成され、
Ss6Converter GUI のバイナリは `SpriteStudio6-SDK/Build/Ss6ConverterGUI/build-Ss6ConverterGUI-XXX-{Debug|Release}/Ss6ConverterGUI.app` に生成されます。
実行するには, `Ss6Converter` のバイナリを `SpriteStudio6-SDK/Build/Ss6ConverterGUI/build-Ss6ConverterGUI-XXX-{Debug|Release}/Ss6Converter` へ配置してください。
## Viewer2 のビルド
Viewer2 の開発環境構築には [JUCE](https://juce.com/get-juce/download) が必要になります。
### JUCE Projucer.app を実行
Projucer.app を実行し、 `SpriteStudio6-SDK/Build/Viewer2/Viewer2.jucer` を開きます。
### xcodeproject の生成
`Selected exporter` で `Xcode (MacOSX)` を選択し、 Xcode のアイコンをクリックすると、 xcodeproject が生成され Xcode が起動します。
xcodeproject は `SpriteStudio6-SDK/Build/Viewer2/Builds/MacOSX/SSViewer2.xcodeproj` に生成されます。
### xcodeproject のビルド
`SpriteStudio6-SDK/Build/Viewer2/Builds/MacOSX/SSViewer2.xcodeproj` の `SSViewer2 - App` をビルドしてください。
