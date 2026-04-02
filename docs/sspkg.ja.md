# sspkg
## 概要
sspkgファイルはオリジナルのデータと再生用のssfbをZIPで格納したファイルになる。
## 格納状態
[https://github.com/SpriteStudio/SS6PlayerForWeb/tree/develop/TestData/MeshBone](https://github.com/SpriteStudio/SS6PlayerForWeb/tree/develop/TestData/MeshBone)
例として以下のプロジェクトデータをSS6Converterを通しsspkgオプションを指定すると以下のようになる。
```
root - + meta | | sspkg.json | | thumbnail.png | Knight.ssfb | Effect.png | Effect.ssce | Flame_effect.ssee | Knight.sspj | Knight.png | Knight.ssce | Knight_arrow.ssae | Knight_bomb.ssae | Knight_lance.ssae
```
* sspkg.json
* thumbnail.png
についてはコンバーターにより生成され同梱される。
コンバーターはこちらのSS6Converterを使用し、-sspkgオプションにより生成される
[https://github.com/SpriteStudio/SpriteStudio6-SDK/releases](https://github.com/SpriteStudio/SpriteStudio6-SDK/releases)
( 2021/02/01 ではdevelopブランチに未マージ )
thumbnail.pngは複数アニメーションが格納されている場合先頭アニメーションとなる。
## sspkg.json
```
{ "version" : "1.00", "ssversion" : "6.40", "filelist" : [ "Effect.png", "Effect.ssce", "Flame_effect.ssee", "Knight.sspj", "Knight.png", "Knight.ssce", "Knight_arrow.ssae", "Knight_bomb.ssae", "Knight_lance.ssae" ] }
}
```
ファイルリストと作成したSSのバージョン、コンバーターのバージョンが格納される
## サンプル pkg
[https://github.com/SpriteStudio/SS6PlayerForWeb/blob/develop/TestData/MeshBone/Knight.sspkg](https://github.com/SpriteStudio/SS6PlayerForWeb/blob/develop/TestData/MeshBone/Knight.sspkg)
