[**日本語**](./binary-file-format.ja.md) | [**English**](./binary-file-format.md)

# バイナリファイルのフォーマット
## 注意
情報が古いので、この内容は参考にしないでください。
## 概要
* 変換時に指定した sspj ファイルと登録されているssae, ssce すべての情報を１つにパックします。
* sspj,ssae,ssce ファイルが持つ情報のうち再生に必要な情報のみを含んでいます。
* アニメーションの補間計算はコンバート時に行われます。(実行時計算にも対応予定)
* 座標値はグローバル化せずローカル値のまま保持します。
## 詳細
* ファイル構造
ssbpファイルは次の情報で構成されています。
SS5PlayerData.hにssbp解析用の構造体が定義されています。
	+ プロジェクト設定
	- struct ProjectData
	- dataId　　　　　　　　ssbpファイルデータ確認用のID
	- version　　　　　　　ssbpファイルのバージョン
	- flags　　　　　　　　予備
	- imageBaseDir　　　　リソースパス、コンバート時の引数で設定する
	- cells　　　　　　　　セル構造体へのポインタ（配列の先頭アドレス）
	- animePacks　　　　　アニメパック構造体へのポインタ（配列の先頭アドレス）
	- numCells　　　　　　セルの数
	- numAnimePacks　　　　アニメーションの数
	+ セル
	- struct Cell
	- name　　　　　　　　セル名
	- cellMap;　　　　　　セルマップ構造体へのポインタ（配列の先頭アドレス）
	- indexInCellMap　　　セル番号
	- x　　　　　　　　　　セル画像内の開始X座標
	- y　　　　　　　　　　セル画像内の開始Y座標
	- width　　　　　　　　セルの幅
	- height　　　　　　　セルの高さ
	- reserved　　　　　　予備
	+ セルマップ
	- struct CellMap
	- name　　　　　　　　セルマップ名(ssce名)
	- imagePath　　　　　　テクスチャファイル名
	- index　　　　　　　　セルマップ番号
	- reserved　　　　　　予備
	+ アニメパック
	- struct AnimePackData
	- name　　　　　　　　　　　モーション名
	- parts　　　　　　　　　　　パーツ構造体へのポインタ
	- animations; 　　　　アニメ（モーション）構造体へのポインタ
	- numParts　　　　　　　　　パーツ数
	- numAnimations　　　　　　モーション数
	+ アニメ
	- struct AnimationData
	- name　　　　　　　　　　　モーション名
	- defaultData　　　　　　　パーツの初期値（配列の先頭アドレス）
	- frameData　　　　　　　　各フレームのパーツデータ（配列の先頭アドレス）
	- userData　　　　　　　　　ユーザーデータ（配列の先頭アドレス）
	- labelData　　　　　　　　ラベルデータ（配列の先頭アドレス）
	- numFrames　　　　　　　　総フレーム
	- fps　　　　　　　　　　　再生スピード
	- labelNum　　　　　　　　ラベルデータの数
	+ パーツ初期値
	- struct AnimationInitialData
	- index　　　　　　　　　　　パーツインデックス、SS上のフレームコントロールの順番
	- dummy　　　　　　　　　　　バイナリデータ出力用のアライメントダミーデータ
	- flags　　　　　　　　　　　非表示、X反転、Y反転フラグ
	- cellIndex　　　　　　　　　セルインデックス
	- positionX　　　　　　　　　X座標初期値
	- positionY　　　　　　　　　Y座標初期値
	- opacity　　　　　　　　　　透明度初期値
	- anchorX　　　　　　　　　　原点Xオフセット＋セルに設定された原点Xオフセット初期値
	- anchorY　　　　　　　　　　原点Yオフセット＋セルに設定された原点Yオフセット初期値
	- rotationX　　　　　　　　　X回転初期値
	- rotationY　　　　　　　　　Y回転初期値
	- rotationZ　　　　　　　　　Z回転初期値
	- scaleX　　　　　　　　　　　X拡大率初期値
	- scaleY　　　　　　　　　　　Y拡大率初期値
	- size_X　　　　　　　　　　　Xサイズ初期値
	- size_Y　　　　　　　　　　　Yサイズ初期値
	- uv_move_X　　　　　　　　　UV　X移動初期値
	- uv_move_Y　　　　　　　　　UV　Y移動初期値
	- uv_rotation　　　　　　　　UV回転初期値
	- uv_scale_X　　　　　　　　　UV　Xスケール初期値
	- uv_scale_Y　　　　　　　　　UV　Yスケール初期値
	- boundingRadius　　　　　　当たり半径初期値
	+ 各フレームのパーツデータ
	- パーツデータは可変長で初期値と異なるデータのみが格納されています。
	- ヘッダ部
	* パーツインデックス
	* フラグ
	+ フラグにはビット対応でどのデータが含まれるかが入っています。
	+ フラグの内容によってファイルからデータを読むか、初期値からデータを読むかを決定する。
	+ 詳細はSS5Player cocos2d-xに含まれるSS5Player.cppのvoid Player::setFrame(int frameNo)を参照
	+ ユーザーデータ
	- struct UserData
	- partName　　　　　　　このユーザーデータが設定されているパーツ名
	- frameNo　　　　　　　このユーザーデータが設定されているフレーム
	- flags　　　　　　　　含まれるデータの種類
	- integer　　　　　　　整数
	- rect[4]　　　　　　　矩形データ
	- point[2]　　　　　　　座標データ
	- str　　　　　　　　　文字列
	- strSize　　　　　　　文字列のサイズ
	+ ラベルデータ
	- struct LabelData
	- str　　　　　　　　　ラベル名
	- frameNo　　　　　　　ラベルを設定したフレーム
