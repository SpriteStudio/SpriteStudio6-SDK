# SpriteStudio 6 SDK
Copyright © CRI Middleware Co., Ltd.

## 概要
本SDKは、[OPTPiX SpriteStudio 6以降](http://www.webtech.co.jp/spritestudio/)で製作されたデータ(sspj,ssce,ssae,ssee)をユーザーの皆様の再生環境に応じて柔軟にご利用いただくためのライブラリ、およびツール群で構成されます。

* sspjLib :SpriteStudioのプロジェクトファイルsspjを解析するためのライブラリです。
    * ローダー：SpriteStudio 6 形式のファイル(sspj,ssce,ssae,ssee）の読み込みモジュール
    * アニメータ：座標変換、補間計算などを行うアニメーションモジュール
    * ドローワー：OpenGL を利用した描画モジュール
* Tools：ライブラリを使用して作成されたツールです。
    * ビューアー：Windows/Mac 用ビューアーサンプル
    * コンバータ：sspjファイルから再生プログラム用のファイル(.ssbp .json等)へ変換します。

### 想定する用途
本SDKの用途は、sspjを解析して新たなプラットフォーム用プレイヤーを作成したり、別のフォーマットにデータを加工、変換するツールの作成となります。
ゲームやアプリで使用する場合は、アニメーション制御に特化したプレイヤープログラムを公開しています。
またアニメーション編集に役立つ外部ツールも公開していますので、以下のリンクを参照してみてください。
* [OPTPiX ヘルプセンター](http://www.webtech.co.jp/help/ja/spritestudio/download/forprogrammer/)
* [GitHubのSpriteStudioリポジトリ](https://github.com/SpriteStudio?tab=repositories)

### 免責事項
本SDKは**リファレンス実装**を目指してメンテナンスされています。
特定のプラットフォームを意識した最適化は行わず、ある程度の冗長性を維持しています。
develop ブランチは最新の開発版のため、何らかの不具合が含まれている可能性があります。
安定版が必要な場合[Release](https://github.com/SpriteStudio/SpriteStudio6-SDK/releases)から取得してください。

## 対応する SpriteStudio のバージョン
**SDKバージョン_SpriteStudioバージョン** の形式 (例: 2.2.0_SS7.1.0) でリリースが作られています。
詳しくは[Releases](https://github.com/SpriteStudio/SpriteStudio6-SDK/releases) をご覧ください。

## SDKのビルド方法
* [SDKのビルド方法](./docs/SDKのビルド方法.md)
* [(参考) SS6Converter、ビューアーをビルドする方法](http://www.webtech.co.jp/help/ja/spritestudio/guide/output6/sdk/#viewer_build) _※ヘルプセンターのページにジャンプします。_

## ssbpファイルを利用する場合の注意点
ssbpファイルにはフォーマットバージョンが存在し、ご利用の再生プログラム(SS6Player for Cocos2d-x および ssbpLib)のバージョンに応じて対応したフォーマットのファイルを出力するコンバータ(本SDK同梱)を利用する必要があります。
下記表を確認の上、該当するバージョンのコンバータを使用してください。
[各バージョンの対応表はこちら](./docs/コンバータの使い方.md#%E3%83%97%E3%83%AC%E3%82%A4%E3%83%A4%E3%83%BC%E3%81%A8%E3%81%AE%E4%BA%92%E6%8F%9B%E6%80%A7%E3%81%AB%E3%81%A4%E3%81%84%E3%81%A6)

## データ構造とクラスリファレンス
[SpriteStudio6 SDK Document](https://spritestudio.github.io/SpriteStudio6-SDK/html/index.html)

## ブランチルール
SpriteStudioSDKにはmasterとdevelopがあり、プレイヤーを含む最新のコードはdevelopにコミットされます。
安定版と判断した段階で、developからmasterへマージしてバージョン番号を付与しています。

## Pull Request
Pull Request は大歓迎です！
リクエスト送信前にIssue化しておく必要はありません。
変更内容はコミット時のコメントに記入していただければ十分です。

## 使用しているオープンソースライブラリ
SDKでは以下のライブラリを使用しています。
各ライブラリのライセンス等については下記リンクよりご確認ください。
* ローダー
    * [TinyXML2](http://www.grinninglizard.com/tinyxml/)
* レンダラー
    * [OpenGL](http://www.opengl.org/)
    * [glew ( Windows )](http://glew.sourceforge.net/)
* ビューアーサンプルのGUI
    * [glfw3](http://www.glfw.org/)
    * [JUCE 7](https://juce.com/)

## SpriteStudio 5 3rdParty
ご好意で提供していただいたプログラムを公開しています。
ご使用の前にこちらをお読みください。[SpriteStudio 5 3rdParty](http://spritestudio3rdparty.github.io/)

## お問い合わせ
ご質問、ご要望、不具合のご報告は [Issues](https://github.com/SpriteStudio/SpriteStudio6-SDK/issues) に投稿してください。
非公開でのお問い合わせを希望される場合は、[ヘルプセンター](https://www.webtech.co.jp/help/ja/spritestudio7/inquiries/ssplayer_tool/) よりお寄せください。
再現データなどの送付が必要な場合も、上記ヘルプセンター経由でファイルを送信してください。

株式会社ＣＲＩ・ミドルウェア
[https://www.cri-mw.co.jp/](https://www.cri-mw.co.jp/)
Copyright © CRI Middleware Co., Ltd.
