# About sspkg File Format
## Overview
The sspkg file is a ZIP archive containing the original data and an ssfb file for playback.
## Storage Structure
[https://github.com/SpriteStudio/SS6PlayerForWeb/tree/develop/TestData/MeshBone](https://github.com/SpriteStudio/SS6PlayerForWeb/tree/develop/TestData/MeshBone)
As an example, specifying the `sspkg` option through SS6Converter for the following project data results in the following structure:
```
root -
 | sspkg.json
 | thumbnail.png
 +ssfb
 | | Knight.ssfb
 |
 +org
 | | Effect.png
 | | Effect.ssce
 | | Flame_effect.ssee
 | | Knight.png
 | | Knight.ssce
 | | Knight_arrow.ssae
 | | Knight_bomb.ssae
 | | Knight_lance.ssae
```
* sspkg.json
* thumbnail.png
These are generated and bundled by the converter.
thumbnail.png will be the first animation if multiple animations are stored.
## sspkg.json
```json
{
 "version" : "1.00",
 "ssversion" : "6.40",
 "filelist" : {
  "ssfb" : [ "Knight.ssfb" ],
  "org" :[
   "Effect.png",
   "Effect.ssce",
   "Flame_effect.ssee",
   "Knight.png",
   "Knight.ssce",
   "Knight_arrow.ssae",
   "Knight_bomb.ssae",
   "Knight_lance.ssae"
  ]
 }
}
```
The file list, the version of the SpriteStudio that created it, and the version of the converter are stored.
