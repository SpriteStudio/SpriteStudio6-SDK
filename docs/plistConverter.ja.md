[**日本語**](./plistConverter.ja.md) | [**English**](./plistConverter.md)

# plistConverter
## 概要
plistConverter は OPTPiX SpriteStudio 6 のスプライトシート機能で出力されたセル情報(plist)をPlayStationMobile SDK で使用しやすいxml形式に変換するコンバータです。
ソースファイルを公開していますので自由にカスタマイズすることが可能です。
* 免責事項
本ソフトウェアは**評価版**であり、動作検証、評価を目的として配布しているため、
何らかの不具合が含まれている可能性があることをご了承いただいた上で、
ご利用いただきますようお願い致します。
## 取得方法
Windowsで使用する事ができます。
tools/plistConverter.zipに実行ファイルが含まれています。
Build/plistConverterにソースファイルが含まれています。
Visual Studio Express 2013 for Windows DesktopでplistConverter_src.zipないのplistConverter.slnを開きビルドしてください。
## 使用手順
* Sprite Studio 6でスプライトシートの作成
メニュー＞OPTPiX Labs＞スプライトシートを生成を選択します。
Trim Transparentをチェックします。
Output FormatをCocos2d-xにします。
TextureSize等を設定し、Exportボタンを押します。
plistとpngが出力されます。
* plistのコンバート
コマンドプロンプトを起動し、plistConverter.exe を実行します。
第一引数に plist のパスを設定します。
同じフォルダにxmlファイルが作成されます。
* ゲームへの組み込み
作成したxmlファイルとpngファイルをプロジェクトへ追加します。
出力したxmlファイルから画像を表示するサンプルプログラムを公開しています。
[こちらをご参照ください。](https://github.com/SpriteStudio/SS5PlayerForPlayStationMobile/wiki/textuerpack)
