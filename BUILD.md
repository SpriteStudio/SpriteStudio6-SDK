# はじめに
ここでは　CLI でのビルド手順について記載します。

GUI でのビルド手順については Wiki の [SDKのビルド方法](https://github.com/SpriteStudio/SpriteStudio6-SDK/wiki/SDK%E3%81%AE%E3%83%93%E3%83%AB%E3%83%89%E6%96%B9%E6%B3%95)を参照してください。

# ビルド準備
## Widnwos

1. 下記をインストールしてください。
    - Visual Studio 2017 以上
    - [chocolatey](https://chocolatey.org/)
    - Qt
    - git
2. `SpriteStudio6-SDK\Build\Depends\prepare.bat` を管理者権限で実行してください。
3. `SpriteStudio6-SDK\Build\Depends\allDownload.bat` を一般ユーザーで実行してください。

## macOS

1. 下記をインストールしてください。
    - Xcode
    - [Homebrew](https://brew.sh)
2. `SpriteStudio6-SDK/Build/Dependsprepare_macos.sh` を一般ユーザーで実行してください。


# ソース取得

```bash
git clone --recursive https://github.com/SpriteStudio/SpriteStudio6-SDK.git
```


# Ss6Converter ビルド
## Windows

Ss6Converter (コマンドライン)のビルドは`SpriteStudio6-SDK\Scripts\build_converter_win.bat` を実行します。
ビルド作業ディレクトリは `SpriteStudio6-SDK\Build\Converter\build` となります。

Ss6Converter-Qt のビルドは `SpriteStudio6-SDK\Scripts\build_convertergui_win.bat` を実行します。
ビルド作業ディレクトリは `SpriteStudio6-SDK\Build\Ss6ConverterGUI\` となります。

## macOS

Ss6Converter (コマンドライン)のビルドは`SpriteStudio6-SDK/Scripts/build_converter_macos.sh` を実行します。
ビルド作業ディレクトリは `SpriteStudio6-SDK/Build/Converter/build` となります。

Ss6Converter-Qt のビルドは `SpriteStudio6-SDK/Scripts/build_convertergui_macos.sh` を実行します。
ビルド作業ディレクトリは `SpriteStudio6-SDK/Build/Ss6ConverterGUI` となります。

# Viewer ビルド
## Windows
`SpriteStudio6-SDK\Scripts\build_viewer_win.bat` を実行します。
`SpriteStudio6-SDK\Build\Viewer\build` にてビルドします。

## macOS
`SpriteStudio6-SDK/Scripts/build_viewer_macos.sh` を実行します。
`SpriteStudio6-SDK/Build/Viewer/build` にてビルドします。

# Viewer2 ビルド
## Windows
T.B.D

## macOS
`SpriteStudio6-SDK/Scripts/prepare_build_viewer2_macos.sh` を実行し、ビルド前の準備をします。
`SpriteStudio6-SDK/Scripts/build_viewer2_macos.sh` を実行します。
`SpriteStudio6-SDK/Build/Viewer2/cmakeBuild` にてビルドします。\

# リリースビルド
## Windows

`SpriteStudio6-SDK\Scripts\release_win.bat` を実行します。

Ss6Converter, Ss6Converter-Qt と Viewer, Viewer2 をリリースビルドし、zip 圧縮したファイルを `SpriteStudio6-SDK\Tools` に格納します。

## macOS
`SpriteStudio6-SDK/Scripts/release_macos.sh` を実行します。

Ss6Converter, Ss6Converter-Qt と Viewer, Viewer2 をリリースビルドし、dmg や zip 圧縮したファイルを `SpriteStudio6-SDK/Tools` に格納します。
