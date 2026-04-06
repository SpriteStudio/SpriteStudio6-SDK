[**日本語**](./sspkg-file-format-details.ja.md) | [**English**](./sspkg-file-format-details.md)

# sspkg ファイルフォーマットについて
## 概要
sspkgファイルはオリジナルのデータと再生用のssfbをZIPで格納したファイルになる。
## 格納状態
[https://github.com/SpriteStudio/SS6PlayerForWeb/tree/develop/TestData/MeshBone](https://github.com/SpriteStudio/SS6PlayerForWeb/tree/develop/TestData/MeshBone)
例として以下のプロジェクトデータをSS6Converterを通しsspkgオプションを指定すると以下のようになる。
```
root - | sspkg.json | thumbnail.png +ssfb | | Knight.ssfb | +org | | Effect.png | Effect.ssce | Flame_effect.ssee | Knight.png | Knight.ssce | Knight_arrow.ssae | Knight_bomb.ssae | Knight_lance.ssae
```
* sspkg.json
* thumbnail.png
についてはコンバーターにより生成され同梱される。
thumbnail.pngは複数アニメーションが格納されている場合先頭アニメーションとなる。
## sspkg.json
```
{ "version" : "1.00", "ssversion" : "6.40", "filelist" : { "ssfb" : [ "Knight.ssfb" ], "org" :[ "Effect.png", "Effect.ssce", "Flame_effect.ssee", "Knight.png", "Knight.ssce", "Knight_arrow.ssae", "Knight_bomb.ssae", "Knight_lance.ssae" ] }
}
```
ファイルリストと作成したSSのバージョン、コンバーターのバージョンが格納される
