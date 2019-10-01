/**
	SpriteStudio6 Converter Ver.2 (CUI application)

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
/* ========================================================================== */
/* SS6ConverterVer2のdllをCUIで動作させるためのコマンドライン処理部です。     */
/* ========================================================================== */
using System;
using System.Collections.Generic;
using System.Text;

namespace SS6ConverterVer2_CUI
{
	/* ----------------------------------------------- Enums & Constants */
	#region Enums & Constants
	/* コマンドラインへの返値 */
	public enum ExitCode	{
		SUCCESS = 0,								/* 成功 */
		ERROR = 1,									/* エラー（汎用） */
		/* エラーを拡張する場合はこの後ろにでも…… */
	}
	#endregion Enums & Constants

	/* ----------------------------------------------- Classes, Structs & Interfaces */
	#region Classes, Structs & Interfaces
	class Program
	{
		/* ----------------------------------------------- Variables & Properties */
		#region Variables & Properties
		#endregion Variables & Properties

		/* ----------------------------------------------- Functions */
		#region Functions
		static int Main(string[] args)
		{
			/* ツールのシグネチャ */
			Console.WriteLine("SS6Converter Ver.2 [SSPJ to SSBP2] Ver.0.5.0");
			Console.WriteLine(" Copyright(C) Web Technology Corp.");

			/* 引数群のチェック */
			string pathInput = string.Empty;	/* 入力ファイル（SSPJ） */
			string pathOutput = string.Empty;	/* 出力ファイル（SSBP2） */
			int argc = args.Length;
			string argsNow;
			for(int i=0; i<argc; i++)	{
				argsNow = args[i];
				if('/' == argsNow[0])	{	/*  先頭が「/」 */
					/* MEMO: 先頭だけ「-」に置き換える。 */
					argsNow = "-" + argsNow.Substring(1);
				}
				switch(argsNow)	{
					case "-i":	/* 入力ファイル指定 */
						i++;	/* 次の引数はファイル名 */
						pathInput = args[i];
						break;

					case "-o":	/* 出力ファイル指定 */
						i++;	/* 次の引数はファイル名 */
						pathOutput = args[i];
						break;

					default:
						Console.WriteLine("Warning: Invalid option.");
						break;
				}
			}

			/* パラメータ群のチェック */
			string textTemp;
			string directoryInput;
			string fileBodyInput;
			string extensionInput;
			string directoryOutput;
			string fileBodyOutput;
			string extensionOutput;

			textTemp = PathNormalize(pathInput, false);	/* 入力ファイル */
			if(true == string.IsNullOrEmpty(textTemp))	{	/* エラー */
				goto Main_UsageDisplayEnd;
			}
			pathInput = textTemp;
			if(false == LibraryEditor_SpriteStudio6.Utility.File.PathSplit(out directoryInput, out fileBodyInput, out extensionInput, pathInput))	{	/* エラー */
				Console.WriteLine("Error: Input file path is invalid. \"" + pathInput + "\"");
			}
			if(ExtensionSS6Project != extensionInput)	{	/* 拡張子が規定ではない */
				pathInput += ExtensionSS6Project;
			}
			Console.WriteLine("[Input file] " + pathInput);
			if(false == System.IO.File.Exists(pathInput))	{	/* ファイルが存在していない */
				Console.WriteLine("Error: File is not found. \"" + pathInput + "\"");

				goto Main_ErrorEnd;
			}
			/* MEMO: 再度分割しなおしておく */
			if(false == LibraryEditor_SpriteStudio6.Utility.File.PathSplit(out directoryInput, out fileBodyInput, out extensionInput, pathInput))	{	/* エラー */
				/* MEMO: ファイルの存在が確認できているので、ここにはこないはず。 */
				Console.WriteLine("Error: Input file path is invalid. \"" + pathInput + "\"");
			}

			textTemp = PathNormalize(pathOutput, false);	/* 出力ファイル */
			if(true == string.IsNullOrEmpty(textTemp))	{	/* エラー */
				/* MEMO: 出力ファイル名がない場合は全補完する。 */
				directoryOutput = string.Empty;
				fileBodyOutput = string.Empty;
				extensionOutput = string.Empty;
			} else {
				if(false == LibraryEditor_SpriteStudio6.Utility.File.PathSplit(out directoryOutput, out fileBodyOutput, out extensionOutput, textTemp))	{	/* エラー */
					/* MEMO: ファイルの存在が確認できているので、ここにはこないはず。 */
					Console.WriteLine("Error: Output file path is invalid. \"" + textTemp + "\"");
				}
			}
			pathOutput = textTemp;
			if((true == string.IsNullOrEmpty(directoryOutput)) || ("/" == directoryOutput))	{	/* ディレクトリが空もしくはカレントディレクトリ */
				/* MEMO: カレントディレクトリを適用 。    */
				/*       ※ディレクトリは正規化しておく。 */
				/* MEMO: LibraryEditor_SpriteStudio6.Utility.File.PathSplitがディレクトリ末尾に「/」を補完してくるので「/」だけの場合カレント。 */
				directoryOutput = PathNormalize(System.IO.Directory.GetCurrentDirectory(), true);
			}
			if(true == string.IsNullOrEmpty(directoryOutput))	{	/* ファイル名が空 */
				/* MEMO: 入力名と同じボディ名を設定 */
				fileBodyOutput = fileBodyInput;
			}
			if(true == string.IsNullOrEmpty(extensionOutput))	{	/* 拡張子が空 */
				/* MEMO: 規定の拡張子を適用 */
				extensionOutput = ExtensionBinary;
			} else {	/* 拡張子が存在 */
				/* MEMO: ここで勝手に規定拡張子を付けるかどうかは検討の余地あり。 */
				if(ExtensionBinary != extensionOutput)	{	/* 規定拡張子ではない */
					fileBodyOutput += extensionOutput;	/* 今の拡張子はファイル名に付けてしまう */
					extensionOutput = ExtensionBinary;
				}
			}
			pathOutput = PathNormalize((directoryOutput + fileBodyOutput + extensionOutput), false);	/* 再構築 */
			Console.WriteLine("[Input file] " + pathOutput);
			/* MEMO: 再度分割しなおしておく */
			if(false == LibraryEditor_SpriteStudio6.Utility.File.PathSplit(out directoryOutput, out fileBodyOutput, out extensionOutput, pathOutput))	{	/* エラー */
				/* MEMO: ファイルの存在が確認できているので、ここにはこないはず。 */
				Console.WriteLine("Error: Output file path is invalid. \"" + pathInput + "\"");
			}

			/* インポート設定の構築 */
			LibraryEditor_SpriteStudio6.Import.Setting setting = new LibraryEditor_SpriteStudio6.Import.Setting();
			{
				setting.Mode = LibraryEditor_SpriteStudio6.Import.Setting.KindMode.SS6PU;	/* SS6Playerモード */

				/* インポート情報: 基本設定 */
				/* ※未対応 */	setting.Basic.FlagCreateControlGameObject = false;							/* 制御用オブジェクトを作成するか */
				/* ※未対応 */	setting.Basic.FlagCreateProjectFolder = false;								/* インポート時にフォルダを掘るか */
				/* ※未対応 */	setting.Basic.FlagTextureReadable = false;									/* テクスチャを読み書き可能にするか *//* ※未対応オプション */
				/* ※未対応 */	setting.Basic.FlagCreateHolderAsset = false;								/* （Unity-Nativeモードで）アセットホルダコンポーネントを付けるか */
								setting.Basic.FlagInvisibleToHideAll = true;								/* SS6でパーツが不可視ならアトリビュート「非表示」で非表示にする */
				/* ※未対応 */	setting.Basic.FlagTrackAssets = false;										/* 上書時にアセットを追いかけて特定するか */
				/* ※未対応 */	setting.Basic.NoCreateMaterialUnreferenced = LibraryEditor_SpriteStudio6.Import.Setting.GroupBasic.KindNoCreateMaterialUnreferenced.NONE;	/* 未使用マテリアルを作らない設定 */
				/* ※未対応 */	setting.Basic.FlagDeleteMaterialUnreferenced = false;						/* 未使用マテリアルを上書時に消すか */
				/* ※未対応 */	setting.Basic.FlagDisableInitialLightRenderer = false;						/* MeshRendererのライト効果を消すか */
				/* ※未対応 */	setting.Basic.FlagTakeOverLightRenderer = false;							/* MeshRendererのライト効果を持ち越すか */

				/* インポート情報: 事前計算関係 */
				/* ※必要？ */	setting.PreCalcualation.FlagTrimTransparentPixelsCell = false;				/* セルの周囲をチェックして、セルのUVを透明領域を削るか */

				/* インポート情報: 上書時の確認関係 */
				/* MEMO: ここは対応させる必要はないはず。 */
				/* ※未対応 */	setting.ConfirmOverWrite.FlagPrefabAnimation = false;						/* 上書時に警告を出すか: アニメーションプレハブ */
				/* ※未対応 */	setting.ConfirmOverWrite.FlagPrefabEffect = false;							/* 上書時に警告を出すか: エフェクトプレハブ */
				/* ※未対応 */	setting.ConfirmOverWrite.FlagDataCellMap = false;							/* 上書時に警告を出すか: セルマップ */
				/* ※未対応 */	setting.ConfirmOverWrite.FlagDataAnimation = false;							/* 上書時に警告を出すか: アニメーション */
				/* ※未対応 */	setting.ConfirmOverWrite.FlagDataEffect = false;							/* 上書時に警告を出すか: エフェクト */
				/* ※未対応 */	setting.ConfirmOverWrite.FlagMaterialAnimation = false;						/* 上書時に警告を出すか: アニメーション用マテリアル */
				/* ※未対応 */	setting.ConfirmOverWrite.FlagMaterialEffect = false;						/* 上書時に警告を出すか: エフェクト用マテリアル */
				/* ※未対応 */	setting.ConfirmOverWrite.FlagTexture = false;								/* 上書時に警告を出すか: テクスチャ */

				/* インポート情報: コライダ関係 */
				/* MEMO: ここは対応させる必要はないはず（そもそもコライダがない）。 */
				/* ※未対応 */	setting.Collider.FlagIsTrigger = false;										/* コライダにトリガを付けるか */
				/* ※未対応 */	setting.Collider.FlagAttachCollider = false;								/* 当たり判定が付いている場合コライダを付けるか */
				/* ※未対応 */	setting.Collider.FlagAttachRigidBody = false;								/* コライダをリジッドボディにするか */
				/* ※未対応 */	setting.Collider.SizeZ = 1.0f;												/* コライダのZの厚み */

				/* インポート情報: SSxxのバージョンチェック関係 */
				/* MEMO: ここは対応させる必要はないはず？？（厳密にチェックするなら必要あるかもだけど） */
				/* ※未対応 */	setting.CheckVersion.FlagInvalidSSPJ = false;								/* SSPJの無効バージョンをエラーにする */
				/* ※未対応 */	setting.CheckVersion.FlagInvalidSSCE = false;								/* SSCEの無効バージョンをエラーにする */
				/* ※未対応 */	setting.CheckVersion.FlagInvalidSSAE = false;								/* SSAEの無効バージョンをエラーにする */
				/* ※未対応 */	setting.CheckVersion.FlagInvalidSSEE = false;								/* SSEEの無効バージョンをエラーにする */

				/* インポート情報: アセット名付与特殊名 */
				/* MEMO: ここは対応させる必要はないはず。 */
				/* ※未対応 */	setting.RuleNameAsset.FlagAttachSpecificNameSSPJ = false;					/* SSPJに特殊名称を付ける */
				/* ※未対応 */	setting.RuleNameAsset.NamePrefixTexture = "";								/* テクスチャに特殊名称を付ける */
				/* ※未対応 */	setting.RuleNameAsset.NamePrefixPrefabAnimationSS6PU = "";					/* アニメーションプレハブに特殊名称を付ける */
				/* ※未対応 */	setting.RuleNameAsset.NamePrefixPrefabEffectSS6PU = "";						/* エフェクトプレハブに特殊名称を付ける */
				/* ※未対応 */	setting.RuleNameAsset.NamePrefixDataCellMapSS6PU = "";						/* セルマップに特殊名称を付ける */
				/* ※未対応 */	setting.RuleNameAsset.NamePrefixDataAnimationSS6PU = "";					/* アニメーションに特殊名称を付ける */
				/* ※未対応 */	setting.RuleNameAsset.NamePrefixDataEffectSS6PU = "";						/* エフェクトに特殊名称を付ける */
				/* ※未対応 */	setting.RuleNameAsset.NamePrefixMaterialAnimationSS6PU = "";				/* アニメーション用マテリアルに特殊名称を付ける */
				/* ※未対応 */	setting.RuleNameAsset.NamePrefixMaterialEffectSS6PU = "";					/* エフェクト用マテリアルに特殊名称を付ける */
				/* ※未対応 */	setting.RuleNameAsset.NamePrefixPrefabAnimatorUnityNative = "";				/* （Unity-Native）アニメーションプレハブに特殊名称を付ける */
				/* ※未対応 */	setting.RuleNameAsset.NamePrefixPrefabParticleUnityNative = "";				/* （Unity-Native）パーティクルプレハブに特殊名称を付ける */
				/* ※未対応 */	setting.RuleNameAsset.NamePrefixAnimationClipUnityNative = "";				/* （Unity-Native）アニメーションクリップに特殊名称を付ける */
				/* ※未対応 */	setting.RuleNameAsset.NamePrefixSkinnedMeshUnityNative = "";				/* （Unity-Native）スキンメッシュに特殊名称を付ける */
				/* ※未対応 */	setting.RuleNameAsset.NamePrefixMaterialAnimatorUnityNative = "";			/* （Unity-Native）アニメーション用マテリアルに特殊名称を付ける */
				/* ※未対応 */	setting.RuleNameAsset.NamePrefixMaterialParticleUnityNative = "";			/* （Unity-Native）パーティクル用マテリアルに特殊名称を付ける */

				/* インポート情報: アセットフォルダ名 */
				/* MEMO: ここは対応させる必要はないはず（後からフォルダ分け格納するなら対応すればいい）。 */
				/* ※未対応 */	setting.RuleNameAssetFolder.NameFolderTexture = "";							/* テクスチャ格納フォルダの名前 */
				/* ※未対応 */	setting.RuleNameAssetFolder.NameFolderPrefabAnimationSS6PU = "";			/* アニメーションプレハブ格納フォルダの名前 */
				/* ※未対応 */	setting.RuleNameAssetFolder.NameFolderPrefabEffectSS6PU = "";				/* エフェクトプレハブ格納フォルダの名前 */
				/* ※未対応 */	setting.RuleNameAssetFolder.NameFolderDataCellMapSS6PU = "";				/* セルマップ格納フォルダの名前 */
				/* ※未対応 */	setting.RuleNameAssetFolder.NameFolderDataAnimationSS6PU = "";				/* アニメーション格納フォルダの名前 */
				/* ※未対応 */	setting.RuleNameAssetFolder.NameFolderDataEffectSS6PU = "";					/* エフェクト格納フォルダの名前 */
				/* ※未対応 */	setting.RuleNameAssetFolder.NameFolderMaterialAnimationSS6PU = "";			/* アニメーション用マテリアル格納フォルダの名前 */
				/* ※未対応 */	setting.RuleNameAssetFolder.NameFolderMaterialEffectSS6PU = "";				/* エフェクト用マテリアル格納フォルダの名前 */
				/* ※未対応 */	setting.RuleNameAssetFolder.NameFolderPrefabAnimatorUnityNative = "";		/* （Unity-Native）アニメーションプレハブ格納フォルダの名前 */
				/* ※未対応 */	setting.RuleNameAssetFolder.NameFolderPrefabParticleUnityNative = "";		/* （Unity-Native）パーティクルプレハブ格納フォルダの名前 */
				/* ※未対応 */	setting.RuleNameAssetFolder.NameFolderAnimationClipUnityNative = "";		/* （Unity-Native）アニメーションクリップ格納フォルダの名前 */
				/* ※未対応 */	setting.RuleNameAssetFolder.NameFolderSkinnedMeshUnityNative = "";			/* （Unity-Native）スキンメッシュ格納フォルダの名前 */
				/* ※未対応 */	setting.RuleNameAssetFolder.NameFolderMaterialAnimatorUnityNative = "";		/* （Unity-Native）アニメーション用マテリアル格納フォルダの名前 */
				/* ※未対応 */	setting.RuleNameAssetFolder.NameFolderMaterialParticleUnityNative = "";		/* （Unity-Native）パーティクル用マテリアル格納フォルダの名前 */

				/* インポート情報: アトリビュート圧縮方式 */
				/* MEMO: 設定時は圧縮型式に対応していないアトリビュートがあるので注意。 */
								setting.PackAttributeAnimation.Status = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE;				/* ※StandardCPEにしか対応していない */
								setting.PackAttributeAnimation.Cell = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE;					/* ※StandardCPEにしか対応していない */
								setting.PackAttributeAnimation.Position = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.CPE_INTERPOLATE;
								setting.PackAttributeAnimation.Rotation = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.CPE_INTERPOLATE;
								setting.PackAttributeAnimation.Scaling = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.CPE_INTERPOLATE;
								setting.PackAttributeAnimation.ScalingLocal = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.CPE_INTERPOLATE;
								setting.PackAttributeAnimation.RateOpacity = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.CPE_INTERPOLATE;
								setting.PackAttributeAnimation.Priority = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.CPE_INTERPOLATE;
								setting.PackAttributeAnimation.PartsColor = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.CPE_INTERPOLATE;
								setting.PackAttributeAnimation.VertexCorrection = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.CPE_INTERPOLATE;
								setting.PackAttributeAnimation.OffsetPivot = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.CPE_INTERPOLATE;
								setting.PackAttributeAnimation.PositionAnchor = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.CPE_INTERPOLATE;
								setting.PackAttributeAnimation.RadiusCollision = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.CPE_INTERPOLATE;
								setting.PackAttributeAnimation.SizeForce = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.CPE_INTERPOLATE;
								setting.PackAttributeAnimation.PositionTexture = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.CPE_INTERPOLATE;
								setting.PackAttributeAnimation.RotationTexture = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.CPE_INTERPOLATE;
								setting.PackAttributeAnimation.ScalingTexture = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.CPE_INTERPOLATE;
								setting.PackAttributeAnimation.UserData = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE;				/* ※StandardCPEにしか対応していない */
								setting.PackAttributeAnimation.Instance = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE;				/* ※StandardCPEにしか対応していない */
								setting.PackAttributeAnimation.Effect = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE;				/* ※StandardCPEにしか対応していない */
								setting.PackAttributeAnimation.Deform = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.CPE_INTERPOLATE;
			}

			/* インポート */
			if(false == LibraryEditor_SpriteStudio6.Import.Exec(	ref setting,
																	pathInput,		/* sspj名 */
																	pathOutput,		/* 出力先アセットフォルダ */
																	false			/* プログレスバーを出さない */
															)
			)	{	/* 失敗 */
				return((int)ExitCode.ERROR);
			}

			return((int)ExitCode.SUCCESS);

		Main_ErrorEnd:;
			return((int)ExitCode.ERROR);

		Main_UsageDisplayEnd:;
			Console.WriteLine("[Usage]");
			Console.WriteLine("SS6ConverterVer2 -i path [-o path]");
			Console.WriteLine();
			Console.WriteLine("[Options]");
			Console.WriteLine("-i : Input SSPJ File path");
			Console.WriteLine("-o : Output SSBP2 File path");
			Console.WriteLine();
			Console.WriteLine("[End of Help]");
			Console.WriteLine();
			return((int)ExitCode.ERROR);
		}
		private static string PathNormalize(string path, bool flagDirectory)
		{
			bool flagWrapped = false;
			if(true == path.StartsWith("\""))	{	/* 先頭が「"」 */
				flagWrapped = true;
			}
			if(true == path.EndsWith("\""))	{	/* 終了が「"」 */
				if(false == flagWrapped)	{	/* 「"」で括られていない */
					return(null);	/* エラー */
				} else {
					path = path.Substring(1, ((path.Length - 1) - 1));	/* 1文字目から最後の文字-1までを取得（最初の-1は長さから最終文字のインデックスへの変換） */
				}
			}
			path = path.Replace("\\", "/");	/* ディレクトリ区切子の変換 */

			if(true == flagDirectory)	{
				if(false == path.EndsWith("/"))	{	/* パスの最後がディレクトリ識別子でない */
					path += "/";	/* 補完 */
				}
			}

			return(path);
		}
		#endregion Functions

		/* ----------------------------------------------- Enums & Constants */
		#region Enums & Constants
		private const string ExtensionSS6Project = ".sspj";
		private const string ExtensionBinary = ".ssbp2";
		#endregion Enums & Constants

		/* ----------------------------------------------- Classes, Structs & Interfaces */
		#region Classes, Structs & Interfaces
		#endregion Classes, Structs & Interfaces

		/* ----------------------------------------------- Delegate types */
		#region Delegates
		#endregion Delegates
	}
	#endregion Classes, Structs & Interfaces

	/* ----------------------------------------------- Delegate types */
	#region Delegates
	#endregion Delegates
}
