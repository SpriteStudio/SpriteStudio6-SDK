# SpriteStudio 6 SDK

# 初めに

このドキュメントはOPTPiX SpriteStudioのデータを取り扱うためのC++ライブラリのドキュメントです。
また、SpriteStudioから排出されるXMLデータの構造を記載するドキュメントとなります。

# 本SDKの構成について

本SDKは、[OPTPiX SpriteStudio 6以降](http://www.webtech.co.jp/spritestudio/)以降で製作されたデータ(sspj,ssce,ssae,ssee)をユーザーの皆様の再生環境に応じて柔軟にご利用いただくためのライブラリ、およびツール群で構成されます。

- sspjLib :SpriteStudioのプロジェクトファイルsspjを解析するためのライブラリです。
    - ローダー：SpriteStudio 6 形式のファイル(sspj,ssce,ssae,ssee）の読み込みモジュール
    - アニメータ：座標変換、補間計算などを行うアニメーションモジュール
    - ドローワー：OpenGL を利用した描画モジュール
- Tools：ライブラリを使用して作成されたツールです。
    - ビューアー：Windows/Mac 用ビューアーサンプル
    - コンバータ：sspjファイルから再生プログラム用のファイル(.ssbp .json等)へ変換します。

# SpriteStudioのXMLデータ構造について

基本的にsssdkのクラス名、メンバー名はXML内ドキュメントのタグと一致しております。
そのためXML内のタグの意味を知りたい場合、このドキュメントのクラスのメンバー、又は列挙型のドキュメントを参照下さい。

OPTPiX SpriteStuidoで作成されるファイル形式はsspj、ssae、ssceの三種類となります。
旧形式のssaxにつきましては、別途あるssaxのドキュメントを参照下さい。
SpriteStudioのユーザーサポートページにございます。）
 
# sspjファイルについて

sspjは、SpriteStudioのプロジェクトファイルです。
このファイルには、プロジェクトに格納されているアニメーション、テクスチャ、セルマップの参照先の記載、
参照ファイル名の格納が行われています。

# ssaeファイルについて

ssaeファイルは、アニメーションデータのパーツの内容とその組み合わせを記述した「Model」部と「Model」を使用してキーフレームアニメーションを記載したAnimation部で構成されています。
Model部はXMLドキュメント中の `<Model>` で定義され、Animation部はXMLドキュメント中の `<animeList>` 以下 `<anime>` で定義されます。
`<animeList>` のブロックには、 `<anime>` が複数格納される場合があります。（同一のModelでアニメーションを作成している場合）

```xml
<SpriteStudioAnimePack>
    <settings>
        アニメーションの再生設定
    </settings>
    <name>アニメーション名称</name>
    <Model>
        <partList>
        アニメーションを構成するパーツの種別、組み合わせデータの記述
            :
            :
        </partList>
    </Model>
    <cellmapNames>
        アニメーションで使用しているセルマップのファイル名リスト
            :
            :
    </cellmapNames>
    <animeList>
        <anime>
            アニメーションデータ
                :
                :
        </anime>
    </animeList>
</SpriteStudioAnimePack>
```

## partListブロックの解説
partListブロックは、１つのパーツを<value>と定義しパーツの配列として機能しています。
valueの中のタグについては、SsPartのクラス説明ページを参照下さい。

## animeブロックの解説
 
<anime>ブロックは、アニメーションの再生FPSや名称などの設定値とそれぞれのキーフレームでパーツに適用するアトリビュート値が格納されています。
キーフレームのアトリビュート値は<partAnimes>ブロックに記載されています。
<partAnimes>は<partAnime>のコンテナとなっており、<partAnime>はモデルで定義された各パーツに対応したアトリビュート値がキーフレーム値と一緒に記載されています。

(例）
```xml
<attribute tag="POSX">
    <key time="0" ipType="linear">
        <value>0</value>
    </key>
</attribute>
```

 ここで、アトリビュートはPOSX=パーツの座標Xを示しており、そのキーフレームはtime=0の場所、補完方法はliner=線形補間となっており、
 その値はvalue=0となっています。このvalue値がパーツの座標Xに適用されます。
 このブロックはキーフレームがエディタ上で作成されている場合、作成されただけここに記載が行われます。
 アトリビュートの種類については、SsAttributeKind で検索、補完方法については、SsInterpolationTypeを参照下さい。
 
# ssceファイルについて
 ssceファイルは、OPTPiX SpriteStudioで作成されたセルマップの情報が記述されているファイルです。
 各セルは、<cells>のブロック内で一つづつ<cell>要素で記述が行われています。
 
例）
```xml
<cells>
    <cell>
        <name>arm_l</name>
        <pos>128 0</pos>
        <size>128 192</size>
        <pivot>0.4296875 -0.354166656732559</pivot>
        <rotated>0</rotated>
        <orgImageName></orgImageName>
        <posStable>0</posStable>
    </cell>
    <cell>
        :
        :
    </cell>
    :
    :
</cells>
```

 基本的にssceは、複雑なフォーマットにはなっておらず、cellの配列として記述が行われておりますので、
 各タグを本ドキュメントで検索頂けるとご使用いただけます。

# sseeファイルについて
T.B.D

