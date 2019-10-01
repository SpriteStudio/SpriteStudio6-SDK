/**
	SpriteStudio6 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using System.Collections;
using System.Collections.Generic;
#if !_FOR_SS6CONVERTER_
using UnityEngine;
using UnityEditor;
#else
using SS6ConverterVer2_DLL;
using SS6ConverterVer2_DLL.Types;
using UnityEngine = SS6ConverterVer2_DLL.Assets;
#endif

public static partial class LibraryEditor_SpriteStudio6
{
	public static partial class Import
	{
		/* ----------------------------------------------- Classes, Structs & Interfaces */
		#region Classes, Structs & Interfaces
		public partial struct Setting
		{
			/* ----------------------------------------------- Variables & Properties */
			#region Variables & Properties
			/* Normal Options */
			public KindMode Mode;
			public GroupBasic Basic;
			public GroupPreCalculation PreCalcualation;
			public GroupConfirmOverWrite ConfirmOverWrite;
			public GroupCollider Collider;
			public GroupCheckVersion CheckVersion;

			/* Advanced Options */
			public GroupRuleNameAsset RuleNameAsset;
			public GroupRuleNameAssetFolder RuleNameAssetFolder;
			public GroupPackAttributeAnimation PackAttributeAnimation;
#if !_FOR_SS6CONVERTER_
			public GroupPresetMaterial PresetMaterial;
#endif
			#endregion Variables & Properties

			/* ----------------------------------------------- Functions */
			#region Functions
			public void CleanUp()
			{
				Mode = KindMode.SS6PU;

				Basic.CleanUp();
				PreCalcualation.CleanUp();
				ConfirmOverWrite.CleanUp();
				Collider.CleanUp();
				CheckVersion.CleanUp();

				RuleNameAsset.CleanUp();
				RuleNameAssetFolder.CleanUp();
				PackAttributeAnimation.CleanUp();
#if !_FOR_SS6CONVERTER_
				PresetMaterial.CleanUp();
#endif
			}
#if !_FOR_SS6CONVERTER_
			public bool Load()
			{
				Mode = (KindMode)(EditorPrefs.GetInt(PrefsKeyMode, (int)DefaultMode));

				Basic.Load();
				PreCalcualation.Load();
				ConfirmOverWrite.Load();
				Collider.Load();
				CheckVersion.Load();

				RuleNameAsset.Load();
				RuleNameAssetFolder.Load();
				PackAttributeAnimation.Load();
				PresetMaterial.Load();

				return(true);
			}

			public bool Save()
			{
				EditorPrefs.SetInt(PrefsKeyMode, (int)Mode);

				Basic.Save();
				PreCalcualation.Save();
				ConfirmOverWrite.Save();
				Collider.Save();
				CheckVersion.Save();

				RuleNameAsset.Save();
				RuleNameAssetFolder.Save();
				PackAttributeAnimation.Save();
				PresetMaterial.Save();

				return(true);
			}

			public string[] Export(	bool flagExportCommon,
									bool flagExportBasic,
									bool flagExportPrecalculation,
									bool flagExportCollider,
									bool flagExportConfirmOverWrite,
									bool flagExportCheckVersion,
									bool flagExportRuleNameAsset,
									bool flagExportRuleNameAssetFolder,
									bool flagPackAttributeAnimation,
									bool flagPresetMaterial
								)
			{
				string[] exportCommon = (true == flagExportCommon) ? ExportCommon() : null;
				string[] exportBasic = (true == flagExportBasic) ? Basic.Export() : null;
				string[] exportPrecalculation = (true == flagExportPrecalculation) ? PreCalcualation.Export() : null;
				string[] exportConfirmOverWrite = (true == flagExportConfirmOverWrite) ? ConfirmOverWrite.Export() : null;
				string[] exportCollider = (true == flagExportCollider) ? Collider.Export() : null;
				string[] exportCheckVersion = (true == flagExportCheckVersion) ? CheckVersion.Export() : null;
				string[] exportRuleNameAsset = (true == flagExportRuleNameAsset) ? RuleNameAsset.Export() : null;
				string[] exportRuleNameAssetFolder = (true == flagExportRuleNameAssetFolder) ? RuleNameAssetFolder.Export() : null;
				string[] exportPackAttributeAnimation = (true == flagPackAttributeAnimation) ? PackAttributeAnimation.Export() : null;
				string[] exportPresetMaterial = (true == flagPresetMaterial) ? PresetMaterial.Export() : null;

				int countCommon = (null != exportCommon) ? exportCommon.Length : 0;
				int countBasic = (null != exportBasic) ? exportBasic.Length : 0;
				int countPrecalculation = (null != exportPrecalculation) ? exportPrecalculation.Length : 0;
				int countConfirmOverWrite = (null != exportConfirmOverWrite) ? exportConfirmOverWrite.Length : 0;
				int countCollider = (null != exportCollider) ? exportCollider.Length : 0;
				int countCheckVersion = (null != exportCheckVersion) ? exportCheckVersion.Length : 0;
				int counttRuleNameAsset = (null != exportRuleNameAsset) ? exportRuleNameAsset.Length : 0;
				int counttRuleNameAssetFolder = (null != exportRuleNameAssetFolder) ? exportRuleNameAssetFolder.Length : 0;
				int countPackAttributeAnimation = (null != exportPackAttributeAnimation) ? exportPackAttributeAnimation.Length : 0;
				int countPresetMaterial = (null != exportPresetMaterial) ? exportPresetMaterial.Length : 0;

				int count = 0;
				count += countCommon;
				count += countBasic;
				count += countPrecalculation;
				count += countConfirmOverWrite;
				count += countCollider;
				count += countCheckVersion;
				count += counttRuleNameAsset;
				count += counttRuleNameAssetFolder;
				count += countPackAttributeAnimation;
				count += countPresetMaterial;

				string[] exportAll = new string[count];
				count = 0;
				for(int i=0; i<countCommon; i++)
				{
					exportAll[count] = exportCommon[i];
					count++;
				}
				for(int i=0; i<countBasic; i++)
				{
					exportAll[count] = exportBasic[i];
					count++;
				}
				for(int i=0; i<countPrecalculation; i++)
				{
					exportAll[count] = exportPrecalculation[i];
					count++;
				}
				for(int i=0; i<countConfirmOverWrite; i++)
				{
					exportAll[count] = exportConfirmOverWrite[i];
					count++;
				}
				for(int i=0; i<countCollider; i++)
				{
					exportAll[count] = exportCollider[i];
					count++;
				}
				for(int i=0; i<countCheckVersion; i++)
				{
					exportAll[count] = exportCheckVersion[i];
					count++;
				}

				for(int i=0; i<counttRuleNameAsset; i++)
				{
					exportAll[count] = exportRuleNameAsset[i];
					count++;
				}
				for(int i=0; i<counttRuleNameAssetFolder; i++)
				{
					exportAll[count] = exportRuleNameAssetFolder[i];
					count++;
				}
				for(int i=0; i<countPackAttributeAnimation; i++)
				{
					exportAll[count] = exportPackAttributeAnimation[i];
					count++;
				}
				for(int i=0; i<countPresetMaterial; i++)
				{
					exportAll[count] = exportPresetMaterial[i];
					count++;
				}

				return(exportAll);
			}

			public bool Import(string text)
			{
				string[] textArgument = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineDecodeCommand(text);
				if((null == textArgument) || (0 >= textArgument.Length))
				{	/* Invalid */
					return(false);
				}

				if(true == ImportCommon(textArgument))
				{
					return(true);
				}
				if(true == Basic.Import(textArgument))
				{
					return(true);
				}
				if(true == PreCalcualation.Import(textArgument))
				{
					return(true);
				}
				if(true == ConfirmOverWrite.Import(textArgument))
				{
					return(true);
				}
				if(true == Collider.Import(textArgument))
				{
					return(true);
				}
				if(true == CheckVersion.Import(textArgument))
				{
					return(true);
				}

				if(true == RuleNameAsset.Import(textArgument))
				{
					return(true);
				}
				if(true == RuleNameAssetFolder.Import(textArgument))
				{
					return(true);
				}
				if(true == PackAttributeAnimation.Import(textArgument))
				{
					return(true);
				}
				if(true == PresetMaterial.Import(textArgument))
				{
					return(true);
				}

				return(false);
			}

			private string[] ExportCommon()
			{
				string[] textExport = new string[1];

				textExport[0] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyMode, TextArgumentMode[(int)Mode]);

				return(textExport);
			}

			private bool ImportCommon(string[] textArgument)
			{
				switch(textArgument[0])
				{
					case TextKeyMode:
						{
							KindMode mode = ImportCommonMode(textArgument[1]);
							if(KindMode.SS6PU <= mode)
							{	/* Valid */
								Mode = mode;
							}
						}
						return(true);

					case TextKeyFolderPrevious:
						break;

					default:
						break;
				}
				return(false);
			}
			public static KindMode ImportCommonMode(string text)
			{
				for(int i=0; i<TextArgumentMode.Length; i++)
				{
					if(text == TextArgumentMode[i])
					{
						return((KindMode)i);
					}
				}
				return((KindMode)(-1));
			}

			public bool ExportFile(	string nameFile,
									bool flagExportCommon,
									bool flagExportBasic,
									bool flagExportPrecalculation,
									bool flagExportCollider,
									bool flagExportConfirmOverWrite,
									bool flagExportCheckVersion,
									bool flagExportRuleNameAsset,
									bool flagExportRuleNameAssetFolder,
									bool flagPackAttributeAnimation,
									bool flagPresetMaterial
								)
			{
				bool rv = false;
				using(System.IO.StreamWriter fileStream = new System.IO.StreamWriter(nameFile, false, System.Text.Encoding.UTF8))
				{
					int count = TextSignatureSettingFile.Length;
					for(int i=0; i<count; i++)
					{
						fileStream.WriteLine(LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeIgnore(TextSignatureSettingFile[i]));
					}

					string[] textLine = Export(	flagExportCommon,
												flagExportBasic,
												flagExportPrecalculation,
												flagExportCollider,
												flagExportConfirmOverWrite,
												flagExportCheckVersion,
												flagExportRuleNameAsset,
												flagExportRuleNameAssetFolder,
												flagPackAttributeAnimation,
												flagPresetMaterial
											);
					if(null != textLine)
					{
						count = textLine.Length;
						for(int i=0; i<count; i++)
						{
							fileStream.WriteLine(textLine[i]);
						}
					}
					rv = true;
				}

				return(rv);
			}

			public bool ImportFile(string nameFile)
			{
				bool rv = false;
				if(true == System.IO.File.Exists(nameFile))
				{
					using(System.IO.StreamReader fileStream = new System.IO.StreamReader(nameFile, System.Text.Encoding.UTF8))
					{
						string textLine = "";
						string textLineValid = "";
						while(0 <= fileStream.Peek())
						{
							textLine = fileStream.ReadLine();
							switch(LibraryEditor_SpriteStudio6.Utility.ExternalText.TypeGetLine(out textLineValid, textLine))
							{
								case LibraryEditor_SpriteStudio6.Utility.ExternalText.KindType.COMMAND:
									Import(textLineValid);
									break;

								case LibraryEditor_SpriteStudio6.Utility.ExternalText.KindType.IGNORE:
								case LibraryEditor_SpriteStudio6.Utility.ExternalText.KindType.NORMAL:
								default:
									break;
							}
						}
						rv = true;
					}
				}

				return(rv);
			}

			public static void FolderSavePrevious(string nameFolder)
			{
				LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyFolderPrevious, nameFolder);
			}

			public static void FolderLoadPrevious(out string nameFolder)
			{
				nameFolder = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyFolderPrevious, "");
			}
#endif
			public static string AssetNameNormalize(string name)
			{
				/* Spaces */
				string nameNormalize = name.Replace(" ", "");
				nameNormalize = nameNormalize.Replace("\t", "");

				/* File & Hierarchy Delimiter */
				nameNormalize = nameNormalize.Replace(":", "");
				nameNormalize = nameNormalize.Replace("/", "");
				nameNormalize = nameNormalize.Replace("\\", "");
				nameNormalize = nameNormalize.Replace(".", "");
				nameNormalize = nameNormalize.Replace("*", "");
				nameNormalize = nameNormalize.Replace("?", "");

				/* Line-Feeds */
				nameNormalize = nameNormalize.Replace("\n", "");
				nameNormalize = nameNormalize.Replace("\r", "");

				/* Quotations */
				nameNormalize = nameNormalize.Replace("\"", "");
				nameNormalize = nameNormalize.Replace("\'", "");

				return(nameNormalize);
			}
			#endregion Functions

			/* ----------------------------------------------- Enums & Constants */
			#region Enums & Constants
			public enum KindMode
			{
				SS6PU = 0,
				UNITY_NATIVE,

				TERMINATOR,
				BATCH_IMPORTER = TERMINATOR,
			}
			public enum KindAsset
			{
				/* (Common) */
				TEXTURE = 0,

				/* (Mode SS6PU) */
				PREFAB_CONTROL_ANIMATION_SS6PU,
				PREFAB_ANIMATION_SS6PU,
				PREFAB_EFFECT_SS6PU,
				DATA_CELLMAP_SS6PU,
				DATA_ANIMATION_SS6PU,
				DATA_EFFECT_SS6PU,
				MATERIAL_ANIMATION_SS6PU,
				MATERIAL_EFFECT_SS6PU,

				/* (Mode UnityNative) */
				PREFAB_CONTROL_ANIMATION_UNITYNATIVE,
				PREFAB_ANIMATION_UNITYNATIVE,
				PREFAB_EFFECT_UNITYNATIVE,
				DATA_ANIMATION_UNITYNATIVE,
				DATA_MESH_UNITYNATIVE,
				MATERIAL_ANIMATION_UNITYNATIVE,
				MATERIAL_EFFECT_UNITYNATIVE,
			}

			private const string KeyMode = "Mode";
			private const string KeyFolderPrevious = "FolderPrevious";

			private const string TextKeyPrefixCommon = "Common_";
			internal const string TextKeyMode = TextKeyPrefixCommon + KeyMode;	/* Referenced from batch-importer */
			private const string TextKeyFolderPrevious = TextKeyPrefixCommon + KeyFolderPrevious;

			public const string PrefsKeyPrefix = "SS6PU_ImportSetting_";	/* Common for all settings */
			private const string PrefsKeyPrefixCommon = PrefsKeyPrefix + TextKeyPrefixCommon;
			private const string PrefsKeyMode = PrefsKeyPrefixCommon + KeyMode;
			private const string PrefsKeyFolderPrevious = PrefsKeyPrefixCommon + KeyFolderPrevious;

			private readonly static string[] TextArgumentMode =
			{
				"SS6PU",
				"UNITY_NATIVE",

				/* "BATCH_IMPORTER" *//* Can not set the mode "batch import" from batch-importer's list. */
			};

			private const KindMode DefaultMode = KindMode.SS6PU;
			private const string DefaultFolderPrevious = "";

			private readonly static string[] TextSignatureSettingFile =
			{
				"========================================================",
				" Importer Setting-File                                  ",
				"--------------------------------------------------------",
				" Generated with \"SpriteStudio6 Player for Unity\"        ",
				" Copyright(C) Web Technology Corp. All rights reserved. ",
				"========================================================",
			};

			#endregion Enums & Constants

			/* ----------------------------------------------- Classes, Structs & Interfaces */
			#region Classes, Structs & Interfaces
			public struct GroupPreCalculation
			{
				/* ----------------------------------------------- Variables & Properties */
				#region Variables & Properties
				public bool FlagTrimTransparentPixelsCell;
				#endregion Variables & Properties

				/* ----------------------------------------------- Functions */
				#region Functions
				public GroupPreCalculation(	bool flagTrimTransparentPixelsCell
										)
				{
					FlagTrimTransparentPixelsCell = flagTrimTransparentPixelsCell;
				}

				public void CleanUp()
				{
					this = Default;
				}
#if !_FOR_SS6CONVERTER_
				public bool Load()
				{
					FlagTrimTransparentPixelsCell = EditorPrefs.GetBool(PrefsKeyFlagTrimTransparentPixelsCell, Default.FlagTrimTransparentPixelsCell);

					return(true);
				}

				public bool Save()
				{
					EditorPrefs.SetBool(PrefsKeyFlagTrimTransparentPixelsCell, FlagTrimTransparentPixelsCell);

					return(true);
				}

				public string[] Export()
				{
					string[] textEncode = new string[1];
					string textValue;

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagTrimTransparentPixelsCell);
					textEncode[0] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagTrimTransparentPixelsCell, textValue);

					return(textEncode);
				}

				public bool Import(string[] textArgument)
				{
					switch(textArgument[0])
					{
						case TextKeyFlagTrimTransparentPixelsCell:
							FlagTrimTransparentPixelsCell = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagFixSprite:	/* Obsolete command */
							return(true);

						default:
							break;
					}
					return(false);
				}
#endif
				#endregion Functions

				/* ----------------------------------------------- Enums & Constants */
				#region Enums & Constants
				/* Obsolete */	private const string KeyFlagFixSprite = "FlagFixSprite";
				private const string KeyFlagTrimTransparentPixelsCell = "FlagTrimTransparentPixelsCell";

				private const string TextKeyPrefix = "PreCalculation_";
				/* Obsolete */	private const string TextKeyFlagFixSprite = TextKeyPrefix + KeyFlagFixSprite;
				private const string TextKeyFlagTrimTransparentPixelsCell = TextKeyPrefix + KeyFlagTrimTransparentPixelsCell;

				private const string PrefsKeyPrefix = LibraryEditor_SpriteStudio6.Import.Setting.PrefsKeyPrefix + TextKeyPrefix;
				/* Obsolete */	private const string PrefsKeyFlagFixSprite = PrefsKeyPrefix + KeyFlagFixSprite;
				private const string PrefsKeyFlagTrimTransparentPixelsCell = PrefsKeyPrefix + KeyFlagTrimTransparentPixelsCell;

				private readonly static GroupPreCalculation Default = new GroupPreCalculation(
					false	/* FlagTrimTransparentPixelsCell */
				);
				#endregion Enums & Constants
			}

			public struct GroupConfirmOverWrite
			{
				/* ----------------------------------------------- Variables & Properties */
				#region Variables & Properties
				public bool FlagPrefabAnimation;
				public bool FlagPrefabEffect;
				public bool FlagDataCellMap;
				public bool FlagDataAnimation;
				public bool FlagDataEffect;
				public bool FlagMaterialAnimation;
				public bool FlagMaterialEffect;
				public bool FlagTexture;
				#endregion Variables & Properties

				/* ----------------------------------------------- Functions */
				#region Functions
				public GroupConfirmOverWrite(	bool flagPrefabAnimation,
												bool flagPrefabEffect,
												bool flagDataCellMap,
												bool flagDataAnimation,
												bool flagDataEffect,
												bool flagMaterialAnimation,
												bool flagMaterialEffect,
												bool flagTexture
											)
				{
					FlagPrefabAnimation = flagPrefabAnimation;
					FlagPrefabEffect = flagPrefabEffect;
					FlagDataCellMap = flagDataCellMap;
					FlagDataAnimation = flagDataAnimation;
					FlagDataEffect = flagDataEffect;
					FlagMaterialAnimation = flagMaterialAnimation;
					FlagMaterialEffect = flagMaterialEffect;
					FlagTexture = flagTexture;
				}

				public void CleanUp()
				{
					this = Default;
				}
#if !_FOR_SS6CONVERTER_
				public bool Load()
				{
					FlagPrefabAnimation = EditorPrefs.GetBool(PrefsKeyFlagPrefabAnimation, Default.FlagPrefabAnimation);
					FlagPrefabEffect = EditorPrefs.GetBool(PrefsKeyFlagPrefabEffect, Default.FlagPrefabEffect);
					FlagDataCellMap = EditorPrefs.GetBool(PrefsKeyFlagDataCellMap, Default.FlagDataCellMap);
					FlagDataAnimation = EditorPrefs.GetBool(PrefsKeyFlagDataAnimation, Default.FlagDataAnimation);
					FlagDataEffect = EditorPrefs.GetBool(PrefsKeyFlagDataEffect, Default.FlagDataEffect);
					FlagMaterialAnimation = EditorPrefs.GetBool(PrefsKeyFlagMaterialAnimation, Default.FlagMaterialAnimation);
					FlagMaterialEffect = EditorPrefs.GetBool(PrefsKeyFlagMaterialEffect, Default.FlagMaterialEffect);
					FlagTexture = EditorPrefs.GetBool(PrefsKeyFlagTexture, Default.FlagTexture);

					return(true);
				}

				public bool Save()
				{
					EditorPrefs.SetBool(PrefsKeyFlagPrefabAnimation, FlagPrefabAnimation);
					EditorPrefs.SetBool(PrefsKeyFlagPrefabEffect, FlagPrefabEffect);
					EditorPrefs.SetBool(PrefsKeyFlagDataCellMap, FlagDataCellMap);
					EditorPrefs.SetBool(PrefsKeyFlagDataAnimation, FlagDataAnimation);
					EditorPrefs.SetBool(PrefsKeyFlagDataEffect, FlagDataEffect);
					EditorPrefs.SetBool(PrefsKeyFlagMaterialAnimation, FlagMaterialAnimation);
					EditorPrefs.SetBool(PrefsKeyFlagMaterialEffect, FlagMaterialEffect);
					EditorPrefs.SetBool(PrefsKeyFlagTexture, FlagTexture);

					return(true);
				}

				public string[] Export()
				{
					string[] textEncode = new string[8];
					string textValue;

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagPrefabAnimation);
					textEncode[0] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagPrefabAnimation, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagPrefabEffect);
					textEncode[1] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagPrefabEffect, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagDataCellMap);
					textEncode[2] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagDataCellMap, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagDataAnimation);
					textEncode[3] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagDataAnimation, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagDataEffect);
					textEncode[4] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagDataEffect, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagMaterialAnimation);
					textEncode[5] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagMaterialAnimation, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagMaterialEffect);
					textEncode[6] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagMaterialEffect, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagTexture);
					textEncode[7] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagTexture, textValue);

					return(textEncode);
				}

				public bool Import(string[] textArgument)
				{
					switch(textArgument[0])
					{
						case TextKeyFlagPrefabAnimation:
							FlagPrefabAnimation = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagPrefabEffect:
							FlagPrefabEffect = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagDataCellMap:
							FlagDataCellMap = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagDataAnimation:
							FlagDataAnimation = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagDataEffect:
							FlagDataEffect = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagMaterialAnimation:
							FlagMaterialAnimation = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagMaterialEffect:
							FlagMaterialEffect = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagTexture:
							FlagTexture = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						default:
							break;
					}
					return(false);
				}
#endif
				#endregion Functions

				/* ----------------------------------------------- Enums & Constants */
				#region Enums & Constants
				private const string KeyFlagPrefabAnimation = "FlagPrefabAnimation";
				private const string KeyFlagPrefabEffect = "FlagPrefabEffect";
				private const string KeyFlagDataCellMap = "FlagDataCellMap";
				private const string KeyFlagDataAnimation = "FlagDataAnimation";
				private const string KeyFlagDataEffect = "FlagDataEffect";
				private const string KeyFlagMaterialAnimation = "FlagMaterialAnimation";
				private const string KeyFlagMaterialEffect = "FlagMaterialEffect";
				private const string KeyFlagTexture = "FlagTexture";

				private const string TextKeyPrefix = "ConfirmOverWrite_";
				private const string TextKeyFlagPrefabAnimation = TextKeyPrefix + KeyFlagPrefabAnimation;
				private const string TextKeyFlagPrefabEffect = TextKeyPrefix + KeyFlagPrefabEffect;
				private const string TextKeyFlagDataCellMap = TextKeyPrefix + KeyFlagDataCellMap;
				private const string TextKeyFlagDataAnimation = TextKeyPrefix + KeyFlagDataAnimation;
				private const string TextKeyFlagDataEffect = TextKeyPrefix + KeyFlagDataEffect;
				private const string TextKeyFlagMaterialAnimation = TextKeyPrefix + KeyFlagMaterialAnimation;
				private const string TextKeyFlagMaterialEffect = TextKeyPrefix + KeyFlagMaterialEffect;
				private const string TextKeyFlagTexture = TextKeyPrefix + KeyFlagTexture;

				private const string PrefsKeyPrefix = LibraryEditor_SpriteStudio6.Import.Setting.PrefsKeyPrefix + TextKeyPrefix;
				private const string PrefsKeyFlagPrefabAnimation = PrefsKeyPrefix + KeyFlagPrefabAnimation;
				private const string PrefsKeyFlagPrefabEffect = PrefsKeyPrefix + KeyFlagPrefabEffect;
				private const string PrefsKeyFlagDataCellMap = PrefsKeyPrefix + KeyFlagDataCellMap;
				private const string PrefsKeyFlagDataAnimation = PrefsKeyPrefix + KeyFlagDataAnimation;
				private const string PrefsKeyFlagDataEffect = PrefsKeyPrefix + KeyFlagDataEffect;
				private const string PrefsKeyFlagMaterialAnimation = PrefsKeyPrefix + KeyFlagMaterialAnimation;
				private const string PrefsKeyFlagMaterialEffect = PrefsKeyPrefix + KeyFlagMaterialEffect;
				private const string PrefsKeyFlagTexture = PrefsKeyPrefix + KeyFlagTexture;

				internal readonly static GroupConfirmOverWrite Default = new GroupConfirmOverWrite(
					false,	/* FlagPrefabAnimation */
					false,	/* FlagPrefabEffect */
					false,	/* FlagDataCellMap */
					false,	/* FlagDataAnimation */
					false,	/* FlagDataEffect */
					false,	/* FlagMaterialAnimation */
					false,	/* FlagMaterialEffect */
					false	/* FlagTexture */
				);
				#endregion Enums & Constants
			}

			public struct GroupCollider
			{
				/* ----------------------------------------------- Variables & Properties */
				#region Variables & Properties
				public bool FlagIsTrigger;
				public bool FlagAttachCollider;
				public bool FlagAttachRigidBody;
				public float SizeZ;
				#endregion Variables & Properties

				/* ----------------------------------------------- Functions */
				#region Functions
				public GroupCollider(	bool flagAttachColider,
										bool flagAttachRigidBody,
										bool flagIsTrigger,
										float sizeZ
									)
				{
					FlagAttachCollider = flagAttachColider;
					FlagAttachRigidBody = flagAttachRigidBody;
					FlagIsTrigger = flagIsTrigger;
					SizeZ = sizeZ;
				}

				public void CleanUp()
				{
					this = Default;
				}
#if !_FOR_SS6CONVERTER_
				public bool Load()
				{
					FlagAttachCollider = EditorPrefs.GetBool(PrefsKeyFlagAttachCollider, Default.FlagAttachCollider);
					FlagAttachRigidBody = EditorPrefs.GetBool(PrefsKeyFlagAttachRigidBody, Default.FlagAttachRigidBody);
					FlagIsTrigger = EditorPrefs.GetBool(PrefsKeyFlagIsTrigger, Default.FlagIsTrigger);
					SizeZ = EditorPrefs.GetFloat(PrefsKeySizeZ, Default.SizeZ);

					return(true);
				}

				public bool Save()
				{
					EditorPrefs.SetBool(PrefsKeyFlagAttachCollider, FlagAttachCollider);
					EditorPrefs.SetBool(PrefsKeyFlagAttachRigidBody, FlagAttachRigidBody);
					EditorPrefs.SetBool(PrefsKeyFlagIsTrigger, FlagIsTrigger);
					EditorPrefs.SetFloat(PrefsKeySizeZ, SizeZ);

					return(true);
				}

				public string[] Export()
				{
					string[] textEncode = new string[4];
					string textValue;

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagAttachCollider);
					textEncode[0] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagAttachCollider, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagAttachRigidBody);
					textEncode[1] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagAttachRigidBody, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagIsTrigger);
					textEncode[2] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagIsTrigger, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.FloatEncode(SizeZ);
					textEncode[3] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeySizeZ, textValue);

					return(textEncode);
				}

				public bool Import(string[] textArgument)
				{
					switch(textArgument[0])
					{
						case TextKeyFlagAttachCollider:
							FlagAttachCollider = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagAttachRigidBody:
							FlagAttachRigidBody = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagIsTrigger:
							FlagIsTrigger = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeySizeZ:
							SizeZ = LibraryEditor_SpriteStudio6.Utility.ExternalText.FloatDecode(textArgument[1]);
							return(true);

						default:
							break;
					}
					return(false);
				}
#endif
				#endregion Functions

				/* ----------------------------------------------- Enums & Constants */
				#region Enums & Constants
				private const string KeyFlagAttachCollider = "FlagAttachCollider";
				private const string KeyFlagAttachRigidBody = "FlagAttachRigidBody";
				private const string KeyFlagIsTrigger = "FlagIsTrigger";
				private const string KeySizeZ = "SizeZ";

				private const string TextKeyPrefix = "Collider_";
				private const string TextKeyFlagAttachCollider = TextKeyPrefix + KeyFlagAttachCollider;
				private const string TextKeyFlagAttachRigidBody = TextKeyPrefix + KeyFlagAttachRigidBody;
				private const string TextKeyFlagIsTrigger = TextKeyPrefix + KeyFlagIsTrigger;
				private const string TextKeySizeZ = TextKeyPrefix + KeySizeZ;

				private const string PrefsKeyPrefix = LibraryEditor_SpriteStudio6.Import.Setting.PrefsKeyPrefix + TextKeyPrefix;
				private const string PrefsKeyFlagAttachCollider = PrefsKeyPrefix + KeyFlagAttachCollider;
				private const string PrefsKeyFlagAttachRigidBody = PrefsKeyPrefix + KeyFlagAttachRigidBody;
				private const string PrefsKeyFlagIsTrigger = PrefsKeyPrefix + KeyFlagIsTrigger;
				private const string PrefsKeySizeZ = PrefsKeyPrefix + KeySizeZ;

				private readonly static GroupCollider Default = new GroupCollider(
					true,	/* FlagAttachCollider */
					false,	/* FlagAttachRigidBody */
					false,	/* FlagIsTrigger */
					1.0f	/* SizeZ */
				);
				#endregion Enums & Constants
			}

			public struct GroupCheckVersion
			{
				/* ----------------------------------------------- Variables & Properties */
				#region Variables & Properties
				public bool FlagInvalidSSPJ;
				public bool FlagInvalidSSCE;
				public bool FlagInvalidSSAE;
				public bool FlagInvalidSSEE;
				#endregion Variables & Properties

				/* ----------------------------------------------- Functions */
				#region Functions
				public GroupCheckVersion(	bool flagInvalidSSPJ,
											bool flagInvalidSSCE,
											bool flagInvalidSSAE,
											bool flagInvalidSSEE
										)
				{
					FlagInvalidSSPJ = flagInvalidSSPJ;
					FlagInvalidSSCE = flagInvalidSSCE;
					FlagInvalidSSAE = flagInvalidSSAE;
					FlagInvalidSSEE = flagInvalidSSEE;
				}

				public void CleanUp()
				{
					this = Default;
				}
#if !_FOR_SS6CONVERTER_
				public bool Load()
				{
					FlagInvalidSSPJ = EditorPrefs.GetBool(PrefsKeyFlagInvalidSSPJ, Default.FlagInvalidSSPJ);
					FlagInvalidSSCE = EditorPrefs.GetBool(PrefsKeyFlagInvalidSSCE, Default.FlagInvalidSSCE);
					FlagInvalidSSAE = EditorPrefs.GetBool(PrefsKeyFlagInvalidSSAE, Default.FlagInvalidSSAE);
					FlagInvalidSSEE = EditorPrefs.GetBool(PrefsKeyFlagInvalidSSEE, Default.FlagInvalidSSEE);

					return(true);
				}

				public bool Save()
				{
					EditorPrefs.GetBool(PrefsKeyFlagInvalidSSPJ, FlagInvalidSSPJ);
					EditorPrefs.GetBool(PrefsKeyFlagInvalidSSCE, FlagInvalidSSCE);
					EditorPrefs.GetBool(PrefsKeyFlagInvalidSSAE, FlagInvalidSSAE);
					EditorPrefs.GetBool(PrefsKeyFlagInvalidSSEE, FlagInvalidSSEE);

					return(true);
				}

				public string[] Export()
				{
					string[] textEncode = new string[4];
					string textValue;

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagInvalidSSPJ);
					textEncode[0] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagInvalidSSPJ, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagInvalidSSCE);
					textEncode[1] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagInvalidSSCE, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagInvalidSSAE);
					textEncode[2] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagInvalidSSAE, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagInvalidSSEE);
					textEncode[3] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagInvalidSSEE, textValue);

					return(textEncode);
				}

				public bool Import(string[] textArgument)
				{
					switch(textArgument[0])
					{
						case TextKeyFlagInvalidSSPJ:
							FlagInvalidSSPJ = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagInvalidSSCE:
							FlagInvalidSSCE = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagInvalidSSAE:
							FlagInvalidSSAE = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagInvalidSSEE:
							FlagInvalidSSEE = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						default:
							break;
					}
					return(false);
				}
#endif
				#endregion Functions

				/* ----------------------------------------------- Enums & Constants */
				#region Enums & Constants
				private const string KeyFlagInvalidSSPJ = "FlagInvalidSSPJ";
				private const string KeyFlagInvalidSSCE = "FlagInvalidSSCE";
				private const string KeyFlagInvalidSSAE = "FlagInvalidSSAE";
				private const string KeyFlagInvalidSSEE = "FlagInvalidSSEE";

				private const string TextKeyPrefix = "CheckVersion_";
				private const string TextKeyFlagInvalidSSPJ = TextKeyPrefix + KeyFlagInvalidSSPJ;
				private const string TextKeyFlagInvalidSSCE = TextKeyPrefix + KeyFlagInvalidSSCE;
				private const string TextKeyFlagInvalidSSAE = TextKeyPrefix + KeyFlagInvalidSSAE;
				private const string TextKeyFlagInvalidSSEE = TextKeyPrefix + KeyFlagInvalidSSEE;

				private const string PrefsKeyPrefix = LibraryEditor_SpriteStudio6.Import.Setting.PrefsKeyPrefix + TextKeyPrefix;
				private const string PrefsKeyFlagInvalidSSPJ = PrefsKeyPrefix + KeyFlagInvalidSSPJ;
				private const string PrefsKeyFlagInvalidSSCE = PrefsKeyPrefix + KeyFlagInvalidSSCE;
				private const string PrefsKeyFlagInvalidSSAE = PrefsKeyPrefix + KeyFlagInvalidSSAE;
				private const string PrefsKeyFlagInvalidSSEE = PrefsKeyPrefix + KeyFlagInvalidSSEE;

				internal readonly static GroupCheckVersion Default = new GroupCheckVersion(
					false,	/* FlagInvalidSSPJ */
					false,	/* FlagInvalidSSCE */
					false,	/* FlagInvalidSSAE */
					false	/* FlagInvalidSSEE */
				);
				#endregion Enums & Constants
			}

			public struct GroupBasic
			{
				/* ----------------------------------------------- Variables & Properties */
				#region Variables & Properties
				public bool FlagCreateControlGameObject;
				public bool FlagCreateProjectFolder;
				public bool FlagTextureReadable;
				public bool FlagCreateHolderAsset;
				public bool FlagInvisibleToHideAll;
				public bool FlagTrackAssets;
				public KindNoCreateMaterialUnreferenced NoCreateMaterialUnreferenced;
				public bool FlagDeleteMaterialUnreferenced;
				public bool FlagDisableInitialLightRenderer;
				public bool FlagTakeOverLightRenderer;
				#endregion Variables & Properties

				/* ----------------------------------------------- Functions */
				#region Functions
				public GroupBasic(	bool flagCreateControlGameObject,
									bool flagCreateProjectFolder,
									bool flagTextureReadable,
									bool flagCreateHolderAsset,
									bool flagInvisibleToHideAll,
									bool flagTrackAssets,
									KindNoCreateMaterialUnreferenced noCreateMaterialUnreferenced,
									bool flagDeleteMaterialUnreferenced,
									bool flagDisableInitialLightRenderer,
									bool flagTakeOverLightRenderer
								)
				{
					FlagCreateControlGameObject = flagCreateControlGameObject;
					FlagCreateProjectFolder = flagCreateProjectFolder;
					FlagTextureReadable = flagTextureReadable;
					FlagCreateHolderAsset = flagCreateHolderAsset;
					FlagInvisibleToHideAll = flagInvisibleToHideAll;
					FlagTrackAssets = flagTrackAssets;
					NoCreateMaterialUnreferenced = noCreateMaterialUnreferenced;
					FlagDeleteMaterialUnreferenced = flagDeleteMaterialUnreferenced;
					FlagDisableInitialLightRenderer = flagDisableInitialLightRenderer;
					FlagTakeOverLightRenderer = flagTakeOverLightRenderer;
				}

				public void CleanUp()
				{
					this = Default;
				}
#if !_FOR_SS6CONVERTER_
				public bool Load()
				{
					FlagCreateControlGameObject = EditorPrefs.GetBool(PrefsKeyFlagCreateControlGameObject, Default.FlagCreateControlGameObject);
					FlagCreateProjectFolder = EditorPrefs.GetBool(PrefsKeyFlagCreateProjectFolder, Default.FlagCreateProjectFolder);
					FlagTextureReadable = EditorPrefs.GetBool(PrefsKeyFlagTextureReadable, Default.FlagTextureReadable);
					FlagCreateHolderAsset = EditorPrefs.GetBool(PrefsKeyFlagCreateHolderAsset, Default.FlagCreateHolderAsset);
					FlagInvisibleToHideAll = EditorPrefs.GetBool(PrefsKeyFlagInvisibleToHideAll, Default.FlagInvisibleToHideAll);
					FlagTrackAssets = EditorPrefs.GetBool(PrefsKeyFlagTrackAssets, Default.FlagTrackAssets);
					string textDefault = TextGetNoCreateMaterialUnreferenced(Default.NoCreateMaterialUnreferenced);
					string text = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNoCreateMaterialUnreferenced, textDefault);
					NoCreateMaterialUnreferenced = KindGetNoCreateMaterialUnreferenced(text);
					FlagDeleteMaterialUnreferenced = EditorPrefs.GetBool(PrefsKeyFlagDeleteMaterialUnreferenced, Default.FlagDeleteMaterialUnreferenced);
					FlagDisableInitialLightRenderer = EditorPrefs.GetBool(PrefsKeyFlagDisableInitialLightRenderer, Default.FlagDisableInitialLightRenderer);
					FlagTakeOverLightRenderer = EditorPrefs.GetBool(PrefsKeyFlagTakeOverLightRenderer, Default.FlagTakeOverLightRenderer);

					return(true);
				}

				public bool Save()
				{
					EditorPrefs.SetBool(PrefsKeyFlagCreateControlGameObject, FlagCreateControlGameObject);
					EditorPrefs.SetBool(PrefsKeyFlagCreateProjectFolder, FlagCreateProjectFolder);
					EditorPrefs.SetBool(PrefsKeyFlagTextureReadable, FlagTextureReadable);
					EditorPrefs.SetBool(PrefsKeyFlagCreateHolderAsset, FlagCreateHolderAsset);
					EditorPrefs.SetBool(PrefsKeyFlagInvisibleToHideAll, FlagInvisibleToHideAll);
					EditorPrefs.SetBool(PrefsKeyFlagTrackAssets, FlagTrackAssets);
					string text = TextGetNoCreateMaterialUnreferenced(NoCreateMaterialUnreferenced);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNoCreateMaterialUnreferenced, text);
					EditorPrefs.SetBool(PrefsKeyFlagDeleteMaterialUnreferenced, FlagDeleteMaterialUnreferenced);
					EditorPrefs.SetBool(PrefsKeyFlagDisableInitialLightRenderer, FlagDisableInitialLightRenderer);
					EditorPrefs.SetBool(PrefsKeyFlagTakeOverLightRenderer, FlagTakeOverLightRenderer);

					return(true);
				}

				public string[] Export()
				{
					string[] textEncode = new string[10];
					string textValue;

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagCreateControlGameObject);
					textEncode[0] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagCreateControlGameObject, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagCreateProjectFolder);
					textEncode[1] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagCreateProjectFolder, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagTextureReadable);
					textEncode[2] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagTextureReadable, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagCreateHolderAsset);
					textEncode[3] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagCreateHolderAsset, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagInvisibleToHideAll);
					textEncode[4] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagInvisibleToHideAll, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagTrackAssets);
					textEncode[5] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagTrackAssets, textValue);

					textValue = TextGetNoCreateMaterialUnreferenced(NoCreateMaterialUnreferenced);
					textEncode[6] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNoCreateMaterialUnreferenced, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagDeleteMaterialUnreferenced);
					textEncode[7] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagDeleteMaterialUnreferenced, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagDisableInitialLightRenderer);
					textEncode[8] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagDisableInitialLightRenderer, textValue);

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagTakeOverLightRenderer);
					textEncode[9] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagTakeOverLightRenderer, textValue);

					return(textEncode);
				}

				public bool Import(string[] textArgument)
				{
					switch(textArgument[0])
					{
						case TextKeyFlagCreateControlGameObject:
							FlagCreateControlGameObject = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagCreateProjectFolder:
							FlagCreateProjectFolder = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagTextureReadable:
							FlagTextureReadable = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagCreateHolderAsset:
							FlagCreateHolderAsset = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagInvisibleToHideAll:
							FlagInvisibleToHideAll = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagTrackAssets:
							FlagTrackAssets = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyNoCreateMaterialUnreferenced:
							NoCreateMaterialUnreferenced = KindGetNoCreateMaterialUnreferenced(textArgument[1]);
							return(true);

						case TextKeyFlagDeleteMaterialUnreferenced:
							FlagDeleteMaterialUnreferenced = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagDisableInitialLightRenderer:
							FlagDisableInitialLightRenderer = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyFlagTakeOverLightRenderer:
							FlagTakeOverLightRenderer = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						default:
							break;
					}
					return(false);
				}

				private KindNoCreateMaterialUnreferenced KindGetNoCreateMaterialUnreferenced(string text)
				{
					string textUpper = text.ToUpper();
					for(KindNoCreateMaterialUnreferenced i=KindNoCreateMaterialUnreferenced.NONE; i<KindNoCreateMaterialUnreferenced.TERMINATOR; i++)
					{
						if(i.ToString() == textUpper)
						{
							return(i);
						}
					}

					/* MEMO: In case of error, return NON. */
					return(KindNoCreateMaterialUnreferenced.NONE);
				}

				private string TextGetNoCreateMaterialUnreferenced(KindNoCreateMaterialUnreferenced kind)
				{
					return(kind.ToString());
				}
#endif
				#endregion Functions

				/* ----------------------------------------------- Enums & Constants */
				#region Enums & Constants
				public enum KindNoCreateMaterialUnreferenced
				{
					NONE = 0,
					BLENDING,
					BLENDING_CELLMAP,

					TERMINATOR
				}

				private const string KeyFlagCreateControlGameObject = "FlagCreateControlGameObject";
				private const string KeyFlagCreateProjectFolder = "FlagCreateProjectFolder";
				private const string KeyFlagTextureReadable = "FlagTextureReadable";
				private const string KeyFlagCreateHolderAsset = "FlagCreateHolderAsset";
				private const string KeyFlagInvisibleToHideAll = "FlagInvisibleToHideAll";
				private const string KeyFlagTrackAssets = "FlagTrackAssets";
				private const string KeyNoCreateMaterialUnreferenced = "NoCreateMaterialUnreferenced";
				private const string KeyFlagDeleteMaterialUnreferenced = "FlagDeleteMaterialUnreferenced";
				private const string KeyFlagDisableInitialLightRenderer = "FlagDisableInitialLightRenderer";
				private const string KeyFlagTakeOverLightRenderer = "FlagTakeOverLightRenderer";

				private const string TextKeyPrefix = "Basic_";
				private const string TextKeyFlagCreateControlGameObject = TextKeyPrefix + KeyFlagCreateControlGameObject;
				private const string TextKeyFlagCreateProjectFolder = TextKeyPrefix + KeyFlagCreateProjectFolder;
				private const string TextKeyFlagTextureReadable = TextKeyPrefix + KeyFlagTextureReadable;
				private const string TextKeyFlagCreateHolderAsset = TextKeyPrefix + KeyFlagCreateHolderAsset;
				private const string TextKeyFlagInvisibleToHideAll = TextKeyPrefix + KeyFlagInvisibleToHideAll;
				private const string TextKeyFlagTrackAssets = TextKeyPrefix + KeyFlagTrackAssets;
				private const string TextKeyNoCreateMaterialUnreferenced = TextKeyPrefix + KeyNoCreateMaterialUnreferenced;
				private const string TextKeyFlagDeleteMaterialUnreferenced = TextKeyPrefix + KeyFlagDeleteMaterialUnreferenced;
				private const string TextKeyFlagDisableInitialLightRenderer = TextKeyPrefix + KeyFlagDisableInitialLightRenderer;
				private const string TextKeyFlagTakeOverLightRenderer = TextKeyPrefix + KeyFlagTakeOverLightRenderer;

				private const string PrefsKeyPrefix = LibraryEditor_SpriteStudio6.Import.Setting.PrefsKeyPrefix + TextKeyPrefix;
				private const string PrefsKeyFlagCreateControlGameObject = PrefsKeyPrefix + KeyFlagCreateControlGameObject;
				private const string PrefsKeyFlagCreateProjectFolder = PrefsKeyPrefix + KeyFlagCreateProjectFolder;
				private const string PrefsKeyFlagTextureReadable = PrefsKeyPrefix + KeyFlagTextureReadable;
				private const string PrefsKeyFlagCreateHolderAsset = PrefsKeyPrefix + KeyFlagCreateHolderAsset;
				private const string PrefsKeyFlagInvisibleToHideAll = PrefsKeyPrefix + KeyFlagInvisibleToHideAll;
				private const string PrefsKeyFlagTrackAssets = PrefsKeyPrefix + KeyFlagTrackAssets;
				private const string PrefsKeyNoCreateMaterialUnreferenced = PrefsKeyPrefix + KeyNoCreateMaterialUnreferenced;
				private const string PrefsKeyFlagDeleteMaterialUnreferenced = PrefsKeyPrefix + KeyFlagDeleteMaterialUnreferenced;
				private const string PrefsKeyFlagDisableInitialLightRenderer = PrefsKeyPrefix + KeyFlagDisableInitialLightRenderer;
				private const string PrefsKeyFlagTakeOverLightRenderer = PrefsKeyPrefix + KeyFlagTakeOverLightRenderer;

				private readonly static GroupBasic Default = new GroupBasic(
					true,									/* FlagCreateControlGameObject */
					true,									/* FlagCreateProjectFolder */
					false,									/* FlagTextureReadable */
					true,									/* FlagCreateHolderAsset */
					false,									/* FlagInvisibleToHideAll */
					true,									/* FlagTrackAssets */
					KindNoCreateMaterialUnreferenced.NONE,	/* NoCreateMaterialUnreferenced */
					false,									/* FlagDeleteMaterialUnreferenced */
					false,									/* FlagDisableInitialLightRenderer */
					false									/* FlagTakeOverLightRenderer */
				);
				#endregion Enums & Constants
			}

			public struct GroupRuleNameAsset
			{
				/* ----------------------------------------------- Variables & Properties */
				#region Variables & Properties
				public bool FlagAttachSpecificNameSSPJ;

				/* Prefix (Common) */
				public string NamePrefixTexture;

				/* Prefix for SS6PU */
				public string NamePrefixPrefabAnimationSS6PU;
				public string NamePrefixPrefabEffectSS6PU;
				public string NamePrefixDataCellMapSS6PU;
				public string NamePrefixDataAnimationSS6PU;
				public string NamePrefixDataEffectSS6PU;
				public string NamePrefixMaterialAnimationSS6PU;
				public string NamePrefixMaterialEffectSS6PU;

				/* Prefix Unity-Native */
				public string NamePrefixPrefabAnimatorUnityNative;
				public string NamePrefixPrefabParticleUnityNative;
				public string NamePrefixAnimationClipUnityNative;
				public string NamePrefixSkinnedMeshUnityNative;
				public string NamePrefixMaterialAnimatorUnityNative;
				public string NamePrefixMaterialParticleUnityNative;
				#endregion Variables & Properties

				/* ----------------------------------------------- Functions */
				#region Functions
				public GroupRuleNameAsset(	bool flagAttachSpecificNameSSPJ,
											string namePrefixTexture,
											string namePrefixPrefabAnimationSS6PU,
											string namePrefixPrefabEffectSS6PU,
											string namePrefixDataCellMapSS6PU,
											string namePrefixDataAnimationSS6PU,
											string namePrefixDataEffectSS6PU,
											string namePrefixMaterialAnimationSS6PU,
											string namePrefixMaterialEffectSS6PU,
											string namePrefixPrefabAnimatorUnityNative,
											string namePrefixPrefabParticleUnityNative,
											string namePrefixAnimationClipUnityNative,
											string namePrefixSkinnedMeshUnityNative,
											string namePrefixMaterialAnimatorUnityNative,
											string namePrefixMaterialParticleUnityNative
										)
				{
					FlagAttachSpecificNameSSPJ = flagAttachSpecificNameSSPJ;

					NamePrefixTexture = namePrefixTexture;

					NamePrefixPrefabAnimationSS6PU = namePrefixPrefabAnimationSS6PU;
					NamePrefixPrefabEffectSS6PU =namePrefixPrefabEffectSS6PU ;
					NamePrefixDataCellMapSS6PU = namePrefixDataCellMapSS6PU;
					NamePrefixDataAnimationSS6PU = namePrefixDataAnimationSS6PU;
					NamePrefixDataEffectSS6PU = namePrefixDataEffectSS6PU;
					NamePrefixMaterialAnimationSS6PU = namePrefixMaterialAnimationSS6PU;
					NamePrefixMaterialEffectSS6PU = namePrefixMaterialEffectSS6PU;

					NamePrefixPrefabAnimatorUnityNative = namePrefixPrefabAnimatorUnityNative;
					NamePrefixPrefabParticleUnityNative = namePrefixPrefabParticleUnityNative;
					NamePrefixAnimationClipUnityNative = namePrefixAnimationClipUnityNative;
					NamePrefixSkinnedMeshUnityNative = namePrefixSkinnedMeshUnityNative;
					NamePrefixMaterialAnimatorUnityNative = namePrefixMaterialAnimatorUnityNative;
					NamePrefixMaterialParticleUnityNative = namePrefixMaterialParticleUnityNative;
				}

				public void CleanUp()
				{
					this = Default;
				}
#if !_FOR_SS6CONVERTER_
				public bool Load()
				{
					FlagAttachSpecificNameSSPJ = EditorPrefs.GetBool(PrefsKeyFlagAttachSpecificNameSSPJ, Default.FlagAttachSpecificNameSSPJ);

					NamePrefixTexture = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNamePrefixTexture, Default.NamePrefixTexture);

					NamePrefixPrefabAnimationSS6PU = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNamePrefixPrefabAnimationSS6PU, Default.NamePrefixPrefabAnimationSS6PU);
					NamePrefixPrefabEffectSS6PU = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNamePrefixPrefabEffectSS6PU, Default.NamePrefixPrefabEffectSS6PU);
					NamePrefixDataCellMapSS6PU = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNamePrefixDataCellMapSS6PU, Default.NamePrefixDataCellMapSS6PU);
					NamePrefixDataAnimationSS6PU = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNamePrefixDataAnimationSS6PU, Default.NamePrefixDataAnimationSS6PU);
					NamePrefixDataEffectSS6PU = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNamePrefixDataEffectSS6PU, Default.NamePrefixDataEffectSS6PU);
					NamePrefixMaterialAnimationSS6PU = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNamePrefixMaterialAnimationSS6PU, Default.NamePrefixMaterialAnimationSS6PU);
					NamePrefixMaterialEffectSS6PU = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNamePrefixMaterialEffectSS6PU, Default.NamePrefixMaterialEffectSS6PU);

					NamePrefixPrefabAnimatorUnityNative = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNamePrefixPrefabAnimatorUnityNative, Default.NamePrefixPrefabAnimatorUnityNative);
					NamePrefixPrefabParticleUnityNative = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNamePrefixPrefabParticleUnityNative, Default.NamePrefixPrefabParticleUnityNative);
					NamePrefixAnimationClipUnityNative = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNamePrefixAnimationClipUnityNative, Default.NamePrefixAnimationClipUnityNative);
					NamePrefixSkinnedMeshUnityNative = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNamePrefixSkinnedMeshUnityNative, Default.NamePrefixSkinnedMeshUnityNative);
					NamePrefixMaterialAnimatorUnityNative = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNamePrefixMaterialAnimatorUnityNative, Default.NamePrefixMaterialAnimatorUnityNative);
					NamePrefixMaterialParticleUnityNative = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNamePrefixMaterialParticleUnityNative, Default.NamePrefixMaterialParticleUnityNative);

					return(true);
				}

				public bool Save()
				{
					EditorPrefs.SetBool(PrefsKeyFlagAttachSpecificNameSSPJ, FlagAttachSpecificNameSSPJ);

					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNamePrefixTexture, NamePrefixTexture);

					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNamePrefixPrefabAnimationSS6PU, NamePrefixPrefabAnimationSS6PU);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNamePrefixPrefabEffectSS6PU, NamePrefixPrefabEffectSS6PU);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNamePrefixDataCellMapSS6PU, NamePrefixDataCellMapSS6PU);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNamePrefixDataAnimationSS6PU, NamePrefixDataAnimationSS6PU);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNamePrefixDataEffectSS6PU, NamePrefixDataEffectSS6PU);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNamePrefixMaterialAnimationSS6PU, NamePrefixMaterialAnimationSS6PU);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNamePrefixMaterialEffectSS6PU, NamePrefixMaterialEffectSS6PU);

					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNamePrefixPrefabAnimatorUnityNative, NamePrefixPrefabAnimatorUnityNative);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNamePrefixPrefabParticleUnityNative, NamePrefixPrefabParticleUnityNative);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNamePrefixAnimationClipUnityNative, NamePrefixAnimationClipUnityNative);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNamePrefixSkinnedMeshUnityNative, NamePrefixSkinnedMeshUnityNative);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNamePrefixMaterialAnimatorUnityNative, NamePrefixMaterialAnimatorUnityNative);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNamePrefixMaterialParticleUnityNative, NamePrefixMaterialParticleUnityNative);

					return(true);
				}

				public string[] Export()
				{
					string[] textEncode = new string[15];
					string textValue;

					Adjust();

					textValue = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolEncode(FlagAttachSpecificNameSSPJ);
					textEncode[0] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyFlagAttachSpecificNameSSPJ, textValue);

					textEncode[1] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNamePrefixTexture, NamePrefixTexture);

					textEncode[2] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNamePrefixPrefabAnimationSS6PU, NamePrefixPrefabAnimationSS6PU);
					textEncode[3] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNamePrefixPrefabEffectSS6PU, NamePrefixPrefabEffectSS6PU);
					textEncode[4] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNamePrefixDataCellMapSS6PU, NamePrefixDataCellMapSS6PU);
					textEncode[5] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNamePrefixDataAnimationSS6PU, NamePrefixDataAnimationSS6PU);
					textEncode[6] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNamePrefixDataEffectSS6PU, NamePrefixDataEffectSS6PU);
					textEncode[7] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNamePrefixMaterialAnimationSS6PU, NamePrefixMaterialAnimationSS6PU);
					textEncode[8] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNamePrefixMaterialEffectSS6PU, NamePrefixMaterialEffectSS6PU);

					textEncode[9] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNamePrefixPrefabAnimatorUnityNative, NamePrefixPrefabAnimatorUnityNative);
					textEncode[10] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNamePrefixPrefabParticleUnityNative, NamePrefixPrefabParticleUnityNative);
					textEncode[11] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNamePrefixAnimationClipUnityNative, NamePrefixAnimationClipUnityNative);
					textEncode[12] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNamePrefixSkinnedMeshUnityNative, NamePrefixSkinnedMeshUnityNative);
					textEncode[13] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNamePrefixMaterialAnimatorUnityNative, NamePrefixMaterialAnimatorUnityNative);
					textEncode[14] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNamePrefixMaterialParticleUnityNative, NamePrefixMaterialParticleUnityNative);

					return(textEncode);
				}

				public bool Import(string[] textArgument)
				{
					switch(textArgument[0])
					{
						case TextKeyFlagAttachSpecificNameSSPJ:
							FlagAttachSpecificNameSSPJ = LibraryEditor_SpriteStudio6.Utility.ExternalText.BoolDecode(textArgument[1]);
							return(true);

						case TextKeyNamePrefixTexture:
							NamePrefixTexture = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);

						case TextKeyNamePrefixPrefabAnimationSS6PU:
							NamePrefixPrefabAnimationSS6PU = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNamePrefixPrefabEffectSS6PU:
							NamePrefixPrefabEffectSS6PU = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNamePrefixDataCellMapSS6PU:
							NamePrefixDataCellMapSS6PU = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNamePrefixDataAnimationSS6PU:
							NamePrefixDataAnimationSS6PU = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNamePrefixDataEffectSS6PU:
							NamePrefixDataEffectSS6PU = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNamePrefixMaterialAnimationSS6PU:
							NamePrefixMaterialAnimationSS6PU = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNamePrefixMaterialEffectSS6PU:
							NamePrefixMaterialEffectSS6PU = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);

						case TextKeyNamePrefixPrefabAnimatorUnityNative:
							NamePrefixPrefabAnimatorUnityNative = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNamePrefixPrefabParticleUnityNative:
							NamePrefixPrefabParticleUnityNative = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNamePrefixAnimationClipUnityNative:
							NamePrefixAnimationClipUnityNative = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNamePrefixSkinnedMeshUnityNative:
							NamePrefixSkinnedMeshUnityNative = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNamePrefixMaterialAnimatorUnityNative:
							NamePrefixMaterialAnimatorUnityNative = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNamePrefixMaterialParticleUnityNative:
							NamePrefixMaterialParticleUnityNative = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);

						default:
							break;
					}
					return(false);
				}

				public void Adjust()
				{
//					FlagAttachSpecificNameSSPJ =

					NamePrefixTexture = Adjust(NamePrefixTexture);

					NamePrefixPrefabAnimationSS6PU = Adjust(NamePrefixPrefabAnimationSS6PU);
					NamePrefixPrefabEffectSS6PU = Adjust(NamePrefixPrefabEffectSS6PU);
					NamePrefixDataCellMapSS6PU = Adjust(NamePrefixDataCellMapSS6PU);
					NamePrefixDataAnimationSS6PU = Adjust(NamePrefixDataAnimationSS6PU);
					NamePrefixDataEffectSS6PU = Adjust(NamePrefixDataEffectSS6PU);
					NamePrefixMaterialAnimationSS6PU = Adjust(NamePrefixMaterialAnimationSS6PU);
					NamePrefixMaterialEffectSS6PU = Adjust(NamePrefixMaterialEffectSS6PU);

					NamePrefixPrefabAnimatorUnityNative = Adjust(NamePrefixPrefabAnimatorUnityNative);
					NamePrefixPrefabParticleUnityNative = Adjust(NamePrefixPrefabParticleUnityNative);
					NamePrefixAnimationClipUnityNative = Adjust(NamePrefixAnimationClipUnityNative);
					NamePrefixSkinnedMeshUnityNative = Adjust(NamePrefixSkinnedMeshUnityNative);
					NamePrefixMaterialAnimatorUnityNative = Adjust(NamePrefixMaterialAnimatorUnityNative);
					NamePrefixMaterialParticleUnityNative = Adjust(NamePrefixMaterialParticleUnityNative);
				}

				public static string Adjust(string text)
				{
					return(LibraryEditor_SpriteStudio6.Import.Setting.AssetNameNormalize(text));
				}

				public string NameGetAsset(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset kind, string nameBase, string nameSSPJ)
				{
					string name = nameBase;
					switch(kind)
					{
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.TEXTURE:
							name = NamePrefixTexture
									+ ((true == FlagAttachSpecificNameSSPJ) ? (nameSSPJ + "_") : "")
									+ nameBase;
							break;

						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_CONTROL_ANIMATION_SS6PU:
							/* MEMO: (PrefabAnimation)_Control */
							name = NamePrefixPrefabAnimationSS6PU
									+ ((true == FlagAttachSpecificNameSSPJ) ? (nameSSPJ + "_") : "")
									+ nameBase
									+ "_Control";
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_ANIMATION_SS6PU:
							name = NamePrefixPrefabAnimationSS6PU
									+ ((true == FlagAttachSpecificNameSSPJ) ? (nameSSPJ + "_") : "")
									+ nameBase;
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_EFFECT_SS6PU:
							name = NamePrefixPrefabEffectSS6PU
									+ ((true == FlagAttachSpecificNameSSPJ) ? (nameSSPJ + "_") : "")
									+ nameBase;
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_CELLMAP_SS6PU:
							name = NamePrefixDataCellMapSS6PU
									+ ((true == FlagAttachSpecificNameSSPJ) ? (nameSSPJ + "_") : "")
									+ nameBase;
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_ANIMATION_SS6PU:
							name = NamePrefixDataAnimationSS6PU
									+ ((true == FlagAttachSpecificNameSSPJ) ? (nameSSPJ + "_") : "")
									+ nameBase;
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_EFFECT_SS6PU:
							name = NamePrefixDataEffectSS6PU
									+ ((true == FlagAttachSpecificNameSSPJ) ? (nameSSPJ + "_") : "")
									+ nameBase;
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.MATERIAL_ANIMATION_SS6PU:
							name = NamePrefixMaterialAnimationSS6PU
									+ ((true == FlagAttachSpecificNameSSPJ) ? (nameSSPJ + "_") : "")
									+ nameBase;
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.MATERIAL_EFFECT_SS6PU:
							name = NamePrefixMaterialEffectSS6PU
									+ ((true == FlagAttachSpecificNameSSPJ) ? (nameSSPJ + "_") : "")
									+ nameBase;
							break;

						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_CONTROL_ANIMATION_UNITYNATIVE:
							/* MEMO: (PrefabAnimation)_Control */
							name = NamePrefixPrefabAnimatorUnityNative
									+ ((true == FlagAttachSpecificNameSSPJ) ? (nameSSPJ + "_") : "")
									+ nameBase
									+ "_Control";
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_ANIMATION_UNITYNATIVE:
							name = NamePrefixPrefabAnimatorUnityNative
									+ ((true == FlagAttachSpecificNameSSPJ) ? (nameSSPJ + "_") : "")
									+ nameBase;
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_EFFECT_UNITYNATIVE:
							name = NamePrefixPrefabParticleUnityNative
									+ ((true == FlagAttachSpecificNameSSPJ) ? (nameSSPJ + "_") : "")
									+ nameBase;
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_ANIMATION_UNITYNATIVE:
							name = NamePrefixAnimationClipUnityNative
									+ ((true == FlagAttachSpecificNameSSPJ) ? (nameSSPJ + "_") : "")
									+ nameBase;
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_MESH_UNITYNATIVE:
							name = NamePrefixSkinnedMeshUnityNative
									+ ((true == FlagAttachSpecificNameSSPJ) ? (nameSSPJ + "_") : "")
									+ nameBase;
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.MATERIAL_ANIMATION_UNITYNATIVE:
							name = NamePrefixMaterialAnimatorUnityNative
									+ ((true == FlagAttachSpecificNameSSPJ) ? (nameSSPJ + "_") : "")
									+ nameBase;
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.MATERIAL_EFFECT_UNITYNATIVE:
							name = NamePrefixMaterialParticleUnityNative
									+ ((true == FlagAttachSpecificNameSSPJ) ? (nameSSPJ + "_") : "")
									+ nameBase;
							break;

						default:
							return(null);
					}
					return(name);
				}
#endif
				#endregion Functions

				/* ----------------------------------------------- Enums & Constants */
				#region Enums & Constants
				private const string KeyFlagAttachSpecificNameSSPJ = "FlagAttachSpecificNameSSPJ";
				private const string KeyNamePrefixTexture = "NamePrefixTexture";
				private const string KeyNamePrefixPrefabAnimationSS6PU = "NamePrefixPrefabAnimationSS6PU";
				private const string KeyNamePrefixPrefabEffectSS6PU = "NamePrefixPrefabEffectSS6PU";
				private const string KeyNamePrefixDataCellMapSS6PU = "NamePrefixDataCellMapSS6PU";
				private const string KeyNamePrefixDataAnimationSS6PU = "NamePrefixDataAnimationSS6PU";
				private const string KeyNamePrefixDataEffectSS6PU = "NamePrefixDataEffectSS6PU";
				private const string KeyNamePrefixMaterialAnimationSS6PU = "NamePrefixMaterialAnimationSS6PU";
				private const string KeyNamePrefixMaterialEffectSS6PU = "NamePrefixMaterialEffectSS6PU";
				private const string KeyNamePrefixPrefabAnimatorUnityNative = "NamePrefixPrefabAnimatorUnityNative";
				private const string KeyNamePrefixPrefabParticleUnityNative = "NamePrefixPrefabParticleUnityNative";
				private const string KeyNamePrefixAnimationClipUnityNative = "NamePrefixAnimationClipUnityNative";
				private const string KeyNamePrefixSkinnedMeshUnityNative = "NamePrefixSkinnedMeshUnityNative";
				private const string KeyNamePrefixMaterialAnimatorUnityNative = "NamePrefixMaterialAnimatorUnityNative";
				private const string KeyNamePrefixMaterialParticleUnityNative = "NamePrefixMaterialParticleUnityNative";

				private const string TextKeyPrefix = "RuleNameAsset_";
				private const string TextKeyFlagAttachSpecificNameSSPJ = TextKeyPrefix + KeyFlagAttachSpecificNameSSPJ;
				private const string TextKeyNamePrefixTexture = TextKeyPrefix + KeyNamePrefixTexture;
				private const string TextKeyNamePrefixPrefabAnimationSS6PU = TextKeyPrefix + KeyNamePrefixPrefabAnimationSS6PU;
				private const string TextKeyNamePrefixPrefabEffectSS6PU = TextKeyPrefix + KeyNamePrefixPrefabEffectSS6PU;
				private const string TextKeyNamePrefixDataCellMapSS6PU = TextKeyPrefix + KeyNamePrefixDataCellMapSS6PU;
				private const string TextKeyNamePrefixDataAnimationSS6PU = TextKeyPrefix + KeyNamePrefixDataAnimationSS6PU;
				private const string TextKeyNamePrefixDataEffectSS6PU = TextKeyPrefix + KeyNamePrefixDataEffectSS6PU;
				private const string TextKeyNamePrefixMaterialAnimationSS6PU = TextKeyPrefix + KeyNamePrefixMaterialAnimationSS6PU;
				private const string TextKeyNamePrefixMaterialEffectSS6PU = TextKeyPrefix + KeyNamePrefixMaterialEffectSS6PU;
				private const string TextKeyNamePrefixPrefabAnimatorUnityNative = TextKeyPrefix + KeyNamePrefixPrefabAnimatorUnityNative;
				private const string TextKeyNamePrefixPrefabParticleUnityNative = TextKeyPrefix + KeyNamePrefixPrefabParticleUnityNative;
				private const string TextKeyNamePrefixAnimationClipUnityNative = TextKeyPrefix + KeyNamePrefixAnimationClipUnityNative;
				private const string TextKeyNamePrefixSkinnedMeshUnityNative = TextKeyPrefix + KeyNamePrefixSkinnedMeshUnityNative;
				private const string TextKeyNamePrefixMaterialAnimatorUnityNative = TextKeyPrefix + KeyNamePrefixMaterialAnimatorUnityNative;
				private const string TextKeyNamePrefixMaterialParticleUnityNative = TextKeyPrefix + KeyNamePrefixMaterialParticleUnityNative;

				private const string PrefsKeyPrefix = LibraryEditor_SpriteStudio6.Import.Setting.PrefsKeyPrefix + TextKeyPrefix;
				private const string PrefsKeyFlagAttachSpecificNameSSPJ = PrefsKeyPrefix + KeyFlagAttachSpecificNameSSPJ;
				private const string PrefsKeyNamePrefixTexture = PrefsKeyPrefix + KeyNamePrefixTexture;
				private const string PrefsKeyNamePrefixPrefabAnimationSS6PU = PrefsKeyPrefix + KeyNamePrefixPrefabAnimationSS6PU;
				private const string PrefsKeyNamePrefixPrefabEffectSS6PU = PrefsKeyPrefix + KeyNamePrefixPrefabEffectSS6PU;
				private const string PrefsKeyNamePrefixDataCellMapSS6PU = PrefsKeyPrefix + KeyNamePrefixDataCellMapSS6PU;
				private const string PrefsKeyNamePrefixDataAnimationSS6PU = PrefsKeyPrefix + KeyNamePrefixDataAnimationSS6PU;
				private const string PrefsKeyNamePrefixDataEffectSS6PU = PrefsKeyPrefix + KeyNamePrefixDataEffectSS6PU;
				private const string PrefsKeyNamePrefixMaterialAnimationSS6PU = PrefsKeyPrefix + KeyNamePrefixMaterialAnimationSS6PU;
				private const string PrefsKeyNamePrefixMaterialEffectSS6PU = PrefsKeyPrefix + KeyNamePrefixMaterialEffectSS6PU;
				private const string PrefsKeyNamePrefixPrefabAnimatorUnityNative = PrefsKeyPrefix + KeyNamePrefixPrefabAnimatorUnityNative;
				private const string PrefsKeyNamePrefixPrefabParticleUnityNative = PrefsKeyPrefix + KeyNamePrefixPrefabParticleUnityNative;
				private const string PrefsKeyNamePrefixAnimationClipUnityNative = PrefsKeyPrefix + KeyNamePrefixAnimationClipUnityNative;
				private const string PrefsKeyNamePrefixSkinnedMeshUnityNative = PrefsKeyPrefix + KeyNamePrefixSkinnedMeshUnityNative;
				private const string PrefsKeyNamePrefixMaterialAnimatorUnityNative = PrefsKeyPrefix + KeyNamePrefixMaterialAnimatorUnityNative;
				private const string PrefsKeyNamePrefixMaterialParticleUnityNative = PrefsKeyPrefix + KeyNamePrefixMaterialParticleUnityNative;

				private readonly static GroupRuleNameAsset Default = new GroupRuleNameAsset(
					false,	/* FlagAttachSpecificNameSSPJ */
					"",		/* NamePrefixTexture */
					"",		/* NamePrefixPrefabAnimationSS6PU */
					"pe_",	/* NamePrefixPrefabEffectSS6PU */
					"dc_",	/* NamePrefixDataCellMapSS6PU */
					"da_",	/* NamePrefixDataAnimationSS6PU */
					"de_",	/* NamePrefixDataEffectSS6PU */
					"ma_",	/* NamePrefixMaterialAnimationSS6PU */
					"me_",	/* NamePrefixMaterialEffectSS6PU */
					"ps_",	/* NamePrefixPrefabAnimatorUnityNative */
					"pp_",	/* NamePrefixPrefabParticleUnityNative */
					"ac_",	/* NamePrefixAnimationClipUnityNative */
					"sm_",	/* NamePrefixSkinnedMeshUnityNative */
					"ms_",	/* NamePrefixMaterialAnimatorUnityNative */
					"mp_"	/* NamePrefixMaterialParticleUnityNative */
				);
				#endregion Enums & Constants
			}

			public struct GroupRuleNameAssetFolder
			{
				/* ----------------------------------------------- Variables & Properties */
				#region Variables & Properties
				/* Folder Names (Common) */
				public string NameFolderTexture;

				/* Folder Names for SS6PU */
				public string NameFolderPrefabAnimationSS6PU;
				public string NameFolderPrefabEffectSS6PU;
				public string NameFolderDataCellMapSS6PU;
				public string NameFolderDataAnimationSS6PU;
				public string NameFolderDataEffectSS6PU;
				public string NameFolderMaterialAnimationSS6PU;
				public string NameFolderMaterialEffectSS6PU;

				/* Folder Names for Unity-Native */
				public string NameFolderPrefabAnimatorUnityNative;
				public string NameFolderPrefabParticleUnityNative;
				public string NameFolderAnimationClipUnityNative;
				public string NameFolderSkinnedMeshUnityNative;
				public string NameFolderMaterialAnimatorUnityNative;
				public string NameFolderMaterialParticleUnityNative;
				#endregion Variables & Properties

				/* ----------------------------------------------- Functions */
				#region Functions
				public GroupRuleNameAssetFolder(	string nameFolderTexture,
													string nameFolderPrefabAnimationSS6PU,
													string nameFolderPrefabEffectSS6PU,
													string nameFolderDataCellMapSS6PU,
													string nameFolderDataAnimationSS6PU,
													string nameFolderDataEffectSS6PU,
													string nameFolderMaterialAnimationSS6PU,
													string nameFolderMaterialEffectSS6PU,
													string nameFolderPrefabAnimatorUnityNative,
													string nameFolderPrefabParticleUnityNative,
													string nameFolderAnimationClipUnityNative,
													string nameFolderSkinnedMeshUnityNative,
													string nameFolderMaterialAnimatorUnityNative,
													string nameFolderMaterialParticleUnityNative
												)
				{
						NameFolderTexture = nameFolderTexture;

						NameFolderPrefabAnimationSS6PU = nameFolderPrefabAnimationSS6PU;
						NameFolderPrefabEffectSS6PU = nameFolderPrefabEffectSS6PU;
						NameFolderDataCellMapSS6PU = nameFolderDataCellMapSS6PU;
						NameFolderDataAnimationSS6PU = nameFolderDataAnimationSS6PU;
						NameFolderDataEffectSS6PU = nameFolderDataEffectSS6PU;
						NameFolderMaterialAnimationSS6PU = nameFolderMaterialAnimationSS6PU;
						NameFolderMaterialEffectSS6PU = nameFolderMaterialEffectSS6PU;

						NameFolderPrefabAnimatorUnityNative = nameFolderPrefabAnimatorUnityNative;
						NameFolderPrefabParticleUnityNative = nameFolderPrefabParticleUnityNative;
						NameFolderAnimationClipUnityNative = nameFolderAnimationClipUnityNative;
						NameFolderSkinnedMeshUnityNative = nameFolderSkinnedMeshUnityNative;
						NameFolderMaterialAnimatorUnityNative = nameFolderMaterialAnimatorUnityNative;
						NameFolderMaterialParticleUnityNative = nameFolderMaterialParticleUnityNative;
				}

				public void CleanUp()
				{
					this = Default;
				}
#if !_FOR_SS6CONVERTER_
				public bool Load()
				{
					NameFolderTexture = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNameFolderTexture, Default.NameFolderTexture);

					NameFolderPrefabAnimationSS6PU = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNameFolderPrefabAnimationSS6PU, Default.NameFolderPrefabAnimationSS6PU);
					NameFolderPrefabEffectSS6PU = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNameFolderPrefabEffectSS6PU, Default.NameFolderPrefabEffectSS6PU);
					NameFolderDataCellMapSS6PU = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNameFolderDataCellMapSS6PU, Default.NameFolderDataCellMapSS6PU);
					NameFolderDataAnimationSS6PU = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNameFolderDataAnimationSS6PU, Default.NameFolderDataAnimationSS6PU);
					NameFolderDataEffectSS6PU = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNameFolderDataEffectSS6PU, Default.NameFolderDataEffectSS6PU);
					NameFolderMaterialAnimationSS6PU = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNameFolderMaterialAnimationSS6PU, Default.NameFolderMaterialAnimationSS6PU);
					NameFolderMaterialEffectSS6PU = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNameFolderMaterialEffectSS6PU, Default.NameFolderMaterialEffectSS6PU);

					NameFolderPrefabAnimatorUnityNative = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNameFolderPrefabAnimatorUnityNative, Default.NameFolderPrefabAnimatorUnityNative);
					NameFolderPrefabParticleUnityNative = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNameFolderPrefabParticleUnityNative, Default.NameFolderPrefabParticleUnityNative);
					NameFolderAnimationClipUnityNative = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNameFolderAnimationClipUnityNative, Default.NameFolderAnimationClipUnityNative);
					NameFolderSkinnedMeshUnityNative = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNameFolderSkinnedMeshUnityNative, Default.NameFolderSkinnedMeshUnityNative);
					NameFolderMaterialAnimatorUnityNative = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNameFolderMaterialAnimatorUnityNative, Default.NameFolderMaterialAnimatorUnityNative);
					NameFolderMaterialParticleUnityNative = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyNameFolderMaterialParticleUnityNative, Default.NameFolderMaterialParticleUnityNative);

					Adjust();

					return(true);
				}

				public bool Save()
				{
					Adjust();

					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNameFolderTexture, NameFolderTexture);

					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNameFolderPrefabAnimationSS6PU, NameFolderPrefabAnimationSS6PU);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNameFolderPrefabEffectSS6PU, NameFolderPrefabEffectSS6PU);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNameFolderDataCellMapSS6PU, NameFolderDataCellMapSS6PU);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNameFolderDataAnimationSS6PU, NameFolderDataAnimationSS6PU);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNameFolderDataEffectSS6PU, NameFolderDataEffectSS6PU);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNameFolderMaterialAnimationSS6PU, NameFolderMaterialAnimationSS6PU);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNameFolderMaterialEffectSS6PU, NameFolderMaterialEffectSS6PU);

					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNameFolderPrefabAnimatorUnityNative, NameFolderPrefabAnimatorUnityNative);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNameFolderPrefabParticleUnityNative, NameFolderPrefabParticleUnityNative);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNameFolderAnimationClipUnityNative, NameFolderAnimationClipUnityNative);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNameFolderSkinnedMeshUnityNative, NameFolderSkinnedMeshUnityNative);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNameFolderMaterialAnimatorUnityNative, NameFolderMaterialAnimatorUnityNative);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyNameFolderMaterialParticleUnityNative, NameFolderMaterialParticleUnityNative);

					return(true);
				}

				public string[] Export()
				{
					string[] textEncode = new string[14];

					Adjust();

					textEncode[0] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNameFolderTexture, NameFolderTexture);

					textEncode[1] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNameFolderPrefabAnimationSS6PU, NameFolderPrefabAnimationSS6PU);
					textEncode[2] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNameFolderPrefabEffectSS6PU, NameFolderPrefabEffectSS6PU);
					textEncode[3] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNameFolderDataCellMapSS6PU, NameFolderDataCellMapSS6PU);
					textEncode[4] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNameFolderDataAnimationSS6PU, NameFolderDataAnimationSS6PU);
					textEncode[5] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNameFolderDataEffectSS6PU, NameFolderDataEffectSS6PU);
					textEncode[6] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNameFolderMaterialAnimationSS6PU, NameFolderMaterialAnimationSS6PU);
					textEncode[7] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNameFolderMaterialEffectSS6PU, NameFolderMaterialEffectSS6PU);

					textEncode[8] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNameFolderPrefabAnimatorUnityNative, NameFolderPrefabAnimatorUnityNative);
					textEncode[9] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNameFolderPrefabParticleUnityNative, NameFolderPrefabParticleUnityNative);
					textEncode[10] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNameFolderAnimationClipUnityNative, NameFolderAnimationClipUnityNative);
					textEncode[11] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNameFolderSkinnedMeshUnityNative, NameFolderSkinnedMeshUnityNative);
					textEncode[12] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNameFolderMaterialAnimatorUnityNative, NameFolderMaterialAnimatorUnityNative);
					textEncode[13] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyNameFolderMaterialParticleUnityNative, NameFolderMaterialParticleUnityNative);

					return(textEncode);
				}

				public bool Import(string[] textArgument)
				{
					switch(textArgument[0])
					{
						case TextKeyNameFolderTexture:
							NameFolderTexture = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);

						case TextKeyNameFolderPrefabAnimationSS6PU:
							NameFolderPrefabAnimationSS6PU = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNameFolderPrefabEffectSS6PU:
							NameFolderPrefabEffectSS6PU = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNameFolderDataCellMapSS6PU:
							NameFolderDataCellMapSS6PU = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNameFolderDataAnimationSS6PU:
							NameFolderDataAnimationSS6PU = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNameFolderDataEffectSS6PU:
							NameFolderDataEffectSS6PU = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNameFolderMaterialAnimationSS6PU:
							NameFolderMaterialAnimationSS6PU = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNameFolderMaterialEffectSS6PU:
							NameFolderMaterialEffectSS6PU = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);

						case TextKeyNameFolderPrefabAnimatorUnityNative:
							NameFolderPrefabAnimatorUnityNative = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNameFolderPrefabParticleUnityNative:
							NameFolderPrefabParticleUnityNative = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNameFolderAnimationClipUnityNative:
							NameFolderAnimationClipUnityNative = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNameFolderSkinnedMeshUnityNative:
							NameFolderSkinnedMeshUnityNative = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNameFolderMaterialAnimatorUnityNative:
							NameFolderMaterialAnimatorUnityNative = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);
						case TextKeyNameFolderMaterialParticleUnityNative:
							NameFolderMaterialParticleUnityNative = (1 >= textArgument.Length) ? "" : Adjust(textArgument[1]);
							return(true);

						default:
							break;
					}
					return(false);
				}

				public void Adjust()
				{
						NameFolderTexture = Adjust(NameFolderTexture);

						NameFolderPrefabAnimationSS6PU = Adjust(NameFolderPrefabAnimationSS6PU);
						NameFolderPrefabEffectSS6PU = Adjust(NameFolderPrefabEffectSS6PU);
						NameFolderDataCellMapSS6PU = Adjust(NameFolderDataCellMapSS6PU);
						NameFolderDataAnimationSS6PU = Adjust(NameFolderDataAnimationSS6PU);
						NameFolderDataEffectSS6PU = Adjust(NameFolderDataEffectSS6PU);
						NameFolderMaterialAnimationSS6PU = Adjust(NameFolderMaterialAnimationSS6PU);
						NameFolderMaterialEffectSS6PU = Adjust(NameFolderMaterialEffectSS6PU);

						NameFolderPrefabAnimatorUnityNative = Adjust(NameFolderPrefabAnimatorUnityNative);
						NameFolderPrefabParticleUnityNative = Adjust(NameFolderPrefabParticleUnityNative);
						NameFolderAnimationClipUnityNative = Adjust(NameFolderAnimationClipUnityNative);
						NameFolderSkinnedMeshUnityNative = Adjust(NameFolderSkinnedMeshUnityNative);
						NameFolderMaterialAnimatorUnityNative = Adjust(NameFolderMaterialAnimatorUnityNative);
						NameFolderMaterialParticleUnityNative = Adjust(NameFolderMaterialParticleUnityNative);
				}

				public static string Adjust(string text)
				{
					return(LibraryEditor_SpriteStudio6.Import.Setting.AssetNameNormalize(text));
				}

				public string NameGetAssetFolder(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset kind, string nameBase)
				{
					string name = nameBase;
					switch(kind)
					{
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.TEXTURE:
							name += NameFolderTexture + "/";
							break;

						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_CONTROL_ANIMATION_SS6PU:
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_ANIMATION_SS6PU:
							name += NameFolderPrefabAnimationSS6PU + "/";
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_EFFECT_SS6PU:
							name += NameFolderPrefabEffectSS6PU + "/";
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_CELLMAP_SS6PU:
							name += NameFolderDataCellMapSS6PU + "/";
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_ANIMATION_SS6PU:
							name += NameFolderDataAnimationSS6PU + "/";
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_EFFECT_SS6PU:
							name += NameFolderDataEffectSS6PU + "/";
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.MATERIAL_ANIMATION_SS6PU:
							name += NameFolderMaterialAnimationSS6PU + "/";
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.MATERIAL_EFFECT_SS6PU:
							name += NameFolderMaterialEffectSS6PU + "/";
							break;

						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_CONTROL_ANIMATION_UNITYNATIVE:
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_ANIMATION_UNITYNATIVE:
							name += NameFolderPrefabAnimatorUnityNative + "/";
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_EFFECT_UNITYNATIVE:
							name += NameFolderPrefabParticleUnityNative + "/";
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_ANIMATION_UNITYNATIVE:
							name += NameFolderAnimationClipUnityNative + "/";
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_MESH_UNITYNATIVE:
							name += NameFolderSkinnedMeshUnityNative + "/";
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.MATERIAL_ANIMATION_UNITYNATIVE:
							name += NameFolderMaterialAnimatorUnityNative + "/";
							break;
						case LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.MATERIAL_EFFECT_UNITYNATIVE:
							name += NameFolderMaterialParticleUnityNative + "/";
							break;

						default:
							return(null);
					}
					return(name);
				}
#endif
				#endregion Functions

				/* ----------------------------------------------- Enums & Constants */
				#region Enums & Constants
				private const string KeyNameFolderTexture = "NameFolderTexture";
				private const string KeyNameFolderPrefabAnimationSS6PU = "NameFolderPrefabAnimationSS6PU";
				private const string KeyNameFolderPrefabEffectSS6PU = "NameFolderPrefabEffectSS6PU";
				private const string KeyNameFolderDataCellMapSS6PU = "NameFolderDataCellMapSS6PU";
				private const string KeyNameFolderDataAnimationSS6PU = "NameFolderDataAnimationSS6PU";
				private const string KeyNameFolderDataEffectSS6PU = "NameFolderDataEffectSS6PU";
				private const string KeyNameFolderMaterialAnimationSS6PU = "NameFolderMaterialAnimationSS6PU";
				private const string KeyNameFolderMaterialEffectSS6PU = "NameFolderMaterialEffectSS6PU";
				private const string KeyNameFolderPrefabAnimatorUnityNative = "NameFolderPrefabAnimatorUnityNative";
				private const string KeyNameFolderPrefabParticleUnityNative = "NameFolderPrefabParticleUnityNative";
				private const string KeyNameFolderAnimationClipUnityNative = "NameFolderAnimationClipUnityNative";
				private const string KeyNameFolderSkinnedMeshUnityNative = "NameFolderSkinnedMeshUnityNative";
				private const string KeyNameFolderMaterialAnimatorUnityNative = "NameFolderMaterialAnimatorUnityNative";
				private const string KeyNameFolderMaterialParticleUnityNative = "NameFolderMaterialParticleUnityNative";

				private const string TextKeyPrefix = "RuleNameAssetFolder_";
				private const string TextKeyNameFolderTexture = TextKeyPrefix + KeyNameFolderTexture;
				private const string TextKeyNameFolderPrefabAnimationSS6PU = TextKeyPrefix + KeyNameFolderPrefabAnimationSS6PU;
				private const string TextKeyNameFolderPrefabEffectSS6PU = TextKeyPrefix + KeyNameFolderPrefabEffectSS6PU;
				private const string TextKeyNameFolderDataCellMapSS6PU = TextKeyPrefix + KeyNameFolderDataCellMapSS6PU;
				private const string TextKeyNameFolderDataAnimationSS6PU = TextKeyPrefix + KeyNameFolderDataAnimationSS6PU;
				private const string TextKeyNameFolderDataEffectSS6PU = TextKeyPrefix + KeyNameFolderDataEffectSS6PU;
				private const string TextKeyNameFolderMaterialAnimationSS6PU = TextKeyPrefix + KeyNameFolderMaterialAnimationSS6PU;
				private const string TextKeyNameFolderMaterialEffectSS6PU = TextKeyPrefix + KeyNameFolderMaterialEffectSS6PU;
				private const string TextKeyNameFolderPrefabAnimatorUnityNative = TextKeyPrefix + KeyNameFolderPrefabAnimatorUnityNative;
				private const string TextKeyNameFolderPrefabParticleUnityNative = TextKeyPrefix + KeyNameFolderPrefabParticleUnityNative;
				private const string TextKeyNameFolderAnimationClipUnityNative = TextKeyPrefix + KeyNameFolderAnimationClipUnityNative;
				private const string TextKeyNameFolderSkinnedMeshUnityNative = TextKeyPrefix + KeyNameFolderSkinnedMeshUnityNative;
				private const string TextKeyNameFolderMaterialAnimatorUnityNative = TextKeyPrefix + KeyNameFolderMaterialAnimatorUnityNative;
				private const string TextKeyNameFolderMaterialParticleUnityNative = TextKeyPrefix + KeyNameFolderMaterialParticleUnityNative;

				private const string PrefsKeyPrefix = LibraryEditor_SpriteStudio6.Import.Setting.PrefsKeyPrefix + TextKeyPrefix;
				private const string PrefsKeyNameFolderTexture = PrefsKeyPrefix + KeyNameFolderTexture;
				private const string PrefsKeyNameFolderPrefabAnimationSS6PU = PrefsKeyPrefix + KeyNameFolderPrefabAnimationSS6PU;
				private const string PrefsKeyNameFolderPrefabEffectSS6PU = PrefsKeyPrefix + KeyNameFolderPrefabEffectSS6PU;
				private const string PrefsKeyNameFolderDataCellMapSS6PU = PrefsKeyPrefix + KeyNameFolderDataCellMapSS6PU;
				private const string PrefsKeyNameFolderDataAnimationSS6PU = PrefsKeyPrefix + KeyNameFolderDataAnimationSS6PU;
				private const string PrefsKeyNameFolderDataEffectSS6PU = PrefsKeyPrefix + KeyNameFolderDataEffectSS6PU;
				private const string PrefsKeyNameFolderMaterialAnimationSS6PU = PrefsKeyPrefix + KeyNameFolderMaterialAnimationSS6PU;
				private const string PrefsKeyNameFolderMaterialEffectSS6PU = PrefsKeyPrefix + KeyNameFolderMaterialEffectSS6PU;
				private const string PrefsKeyNameFolderPrefabAnimatorUnityNative = PrefsKeyPrefix + KeyNameFolderPrefabAnimatorUnityNative;
				private const string PrefsKeyNameFolderPrefabParticleUnityNative = PrefsKeyPrefix + KeyNameFolderPrefabParticleUnityNative;
				private const string PrefsKeyNameFolderAnimationClipUnityNative = PrefsKeyPrefix + KeyNameFolderAnimationClipUnityNative;
				private const string PrefsKeyNameFolderSkinnedMeshUnityNative = PrefsKeyPrefix + KeyNameFolderSkinnedMeshUnityNative;
				private const string PrefsKeyNameFolderMaterialAnimatorUnityNative = PrefsKeyPrefix + KeyNameFolderMaterialAnimatorUnityNative;
				private const string PrefsKeyNameFolderMaterialParticleUnityNative = PrefsKeyPrefix + KeyNameFolderMaterialParticleUnityNative;

				private readonly static GroupRuleNameAssetFolder Default = new GroupRuleNameAssetFolder(
					"Texture",				/* NameFolderTexture */
					"PrefabAnimation",		/* NameFolderPrefabAnimationSS6PU */
					"PrefabEffect",			/* NameFolderPrefabEffectSS6PU */
					"DataCellMap",			/* NameFolderDataCellMapSS6PU */
					"DataAnimation",		/* NameFolderDataAnimationSS6PU */
					"DataEffect",			/* NameFolderDataEffectSS6PU */
					"Material",				/* NameFolderMaterialAnimationSS6PU */
					"Material",				/* NameFolderMaterialEffectSS6PU */
					"PrefabSprite",			/* NameFolderPrefabAnimatorUnityNative */
					"PrefabParticle",		/* NameFolderPrefabParticleUnityNative */
					"DataAnimationClip",	/* NameFolderAnimationClipUnityNative */
					"DataMesh",				/* NameFolderSkinnedMeshUnityNative */
					"Material",				/* NameFolderMaterialAnimatorUnityNative */
					"Material"				/* NameFolderMaterialParticleUnityNative */
				);
				#endregion Enums & Constants
			}

			public struct GroupPackAttributeAnimation
			{
				/* ----------------------------------------------- Variables & Properties */
				#region Variables & Properties
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack Status;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack Cell;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack Position;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack Rotation;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack Scaling;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack ScalingLocal;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack RateOpacity;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack Priority;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack PartsColor;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack VertexCorrection;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack OffsetPivot;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack PositionAnchor;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack RadiusCollision;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack SizeForce;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack PositionTexture;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack RotationTexture;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack ScalingTexture;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack UserData;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack Instance;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack Effect;
				public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack Deform;
				#endregion Variables & Properties

				/* ----------------------------------------------- Functions */
				#region Functions
				public GroupPackAttributeAnimation(	Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack status,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack cell,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack position,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack rotation,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack scaling,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack scalingLocal,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack rateOpacity,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack priority,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack partsColor,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack vertexCorrection,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack offsetPivot,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack positionAnchor,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack sizeForce,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack positionTexture,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack rotationTexture,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack scalingTexture,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack radiusCollision,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack userData,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack instance,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack effect,
													Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack deform
												)
				{
					Status = status;
					Cell = cell;
					Position = position;
					Rotation = rotation;
					Scaling = scaling;
					ScalingLocal = scalingLocal;
					RateOpacity = rateOpacity;
					Priority = priority;
					PartsColor = partsColor;
					VertexCorrection = vertexCorrection;
					OffsetPivot = offsetPivot;
					PositionAnchor = positionAnchor;
					SizeForce = sizeForce;
					PositionTexture = positionTexture;
					RotationTexture = rotationTexture;
					ScalingTexture = scalingTexture;
					RadiusCollision = radiusCollision;
					UserData = userData;
					Instance = instance;
					Effect = effect;
					Deform = deform;
				}

				public void CleanUp()
				{
					this = Default;
				}
#if !_FOR_SS6CONVERTER_
				public bool Load()
				{
					Status = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyStatus, (int)Default.Status));
					Cell = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyCell, (int)Default.Cell));
					Position = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyPosition, (int)Default.Position));
					Rotation = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyRotation, (int)Default.Rotation));
					Scaling = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyScaling, (int)Default.Scaling));
					ScalingLocal = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyScalingLocal, (int)Default.ScalingLocal));
					RateOpacity = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyRateOpacity, (int)Default.RateOpacity));
					Priority = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyPriority, (int)Default.Priority));
					PartsColor = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyPartsColor, (int)Default.PartsColor));
					VertexCorrection = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyVertexCorrection, (int)Default.VertexCorrection));
					OffsetPivot = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyOffsetPivot, (int)Default.OffsetPivot));
					PositionAnchor = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyPositionAnchor, (int)Default.PositionAnchor));
					SizeForce = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeySizeForce, (int)Default.SizeForce));
					PositionTexture = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyPositionTexture, (int)Default.PositionTexture));
					RotationTexture = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyRotationTexture, (int)Default.RotationTexture));
					ScalingTexture = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyScalingTexture, (int)Default.ScalingTexture));
					RadiusCollision = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyRadiusCollision, (int)Default.RadiusCollision));
					UserData = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyUserData, (int)Default.UserData));
					Instance = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyInstance, (int)Default.Instance));
					Effect = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyEffect, (int)Default.Effect));
					Deform = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(EditorPrefs.GetInt(PrefsKeyDeform, (int)Default.Deform));

					return(true);
				}

				public bool Save()
				{
					EditorPrefs.SetInt(PrefsKeyStatus, (int)Status);
					EditorPrefs.SetInt(PrefsKeyPosition, (int)Position);
					EditorPrefs.SetInt(PrefsKeyRotation, (int)Rotation);
					EditorPrefs.SetInt(PrefsKeyScaling, (int)Scaling);
					EditorPrefs.SetInt(PrefsKeyScalingLocal, (int)ScalingLocal);
					EditorPrefs.SetInt(PrefsKeyRateOpacity, (int)RateOpacity);
					EditorPrefs.SetInt(PrefsKeyPriority, (int)Priority);
					EditorPrefs.SetInt(PrefsKeyPartsColor, (int)PartsColor);
					EditorPrefs.SetInt(PrefsKeyPositionAnchor, (int)PositionAnchor);
					EditorPrefs.SetInt(PrefsKeyRadiusCollision, (int)RadiusCollision);
					EditorPrefs.SetInt(PrefsKeyUserData, (int)UserData);
					EditorPrefs.SetInt(PrefsKeyInstance, (int)Instance);
					EditorPrefs.SetInt(PrefsKeyEffect, (int)Effect);
					EditorPrefs.SetInt(PrefsKeyDeform, (int)Deform);

					EditorPrefs.SetInt(PrefsKeyCell, (int)Cell);
					EditorPrefs.SetInt(PrefsKeySizeForce, (int)SizeForce);
					EditorPrefs.SetInt(PrefsKeyVertexCorrection, (int)VertexCorrection);
					EditorPrefs.SetInt(PrefsKeyOffsetPivot, (int)OffsetPivot);
					EditorPrefs.SetInt(PrefsKeyPositionTexture, (int)PositionTexture);
					EditorPrefs.SetInt(PrefsKeyScalingTexture, (int)ScalingTexture);
					EditorPrefs.SetInt(PrefsKeyRotationTexture, (int)RotationTexture);

					return(true);
				}

				public string[] Export()
				{
					string[] textEncode = new string[21];
					string textValue;

					textValue = NameGetPackKind(Status);
					textEncode[0] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyStatus, textValue);

					textValue = NameGetPackKind(Cell);
					textEncode[1] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyCell, textValue);

					textValue = NameGetPackKind(Position);
					textEncode[2] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyPosition, textValue);

					textValue = NameGetPackKind(Rotation);
					textEncode[3] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyRotation, textValue);

					textValue = NameGetPackKind(Scaling);
					textEncode[4] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyScaling, textValue);

					textValue = NameGetPackKind(ScalingLocal);
					textEncode[5] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyScalingLocal, textValue);

					textValue = NameGetPackKind(RateOpacity);
					textEncode[6] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyRateOpacity, textValue);

					textValue = NameGetPackKind(Priority);
					textEncode[7] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyPriority, textValue);

					textValue = NameGetPackKind(PartsColor);
					textEncode[8] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyPartsColor, textValue);

					textValue = NameGetPackKind(VertexCorrection);
					textEncode[9] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyVertexCorrection, textValue);

					textValue = NameGetPackKind(OffsetPivot);
					textEncode[10] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyOffsetPivot, textValue);

					textValue = NameGetPackKind(PositionAnchor);
					textEncode[11] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyPositionAnchor, textValue);

					textValue = NameGetPackKind(SizeForce);
					textEncode[12] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeySizeForce, textValue);

					textValue = NameGetPackKind(PositionTexture);
					textEncode[13] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyPositionTexture, textValue);

					textValue = NameGetPackKind(RotationTexture);
					textEncode[14] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyRotationTexture, textValue);

					textValue = NameGetPackKind(ScalingTexture);
					textEncode[15] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyScalingTexture, textValue);

					textValue = NameGetPackKind(RadiusCollision);
					textEncode[16] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyRadiusCollision, textValue);

					textValue = NameGetPackKind(UserData);
					textEncode[17] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyUserData, textValue);

					textValue = NameGetPackKind(Instance);
					textEncode[18] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyInstance, textValue);

					textValue = NameGetPackKind(Effect);
					textEncode[19] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyEffect, textValue);

					textValue = NameGetPackKind(Deform);
					textEncode[20] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyDeform, textValue);

					return(textEncode);
				}

				public bool Import(string[] textArgument)
				{
					switch(textArgument[0])
					{
						case TextKeyStatus:
							Status = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyPlainCell:	/* Obsolete command */
						case TextKeyCell:
							Cell = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyPosition:
							Position = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyRotation:
							Rotation = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyScaling:
							Scaling = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyScalingLocal:
							ScalingLocal = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyRateOpacity:
							RateOpacity = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyPriority:
							Priority = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyPartsColor:
							PartsColor = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyPlainVertexCorrection:	/* Obsolete command */
						case TextKeyVertexCorrection:
							VertexCorrection = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyPlainOffsetPivot:	/* Obsolete command */
						case TextKeyOffsetPivot:
							OffsetPivot = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyPositionAnchor:
							PositionAnchor = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyPlainSizeForce:	/* Obsolete command */
						case TextKeySizeForce:
							SizeForce = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyPlainPositionTexture:	/* Obsolete command */
						case TextKeyPositionTexture:
							PositionTexture = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyPlainRotationTexture:	/* Obsolete command */
						case TextKeyRotationTexture:
							RotationTexture = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyPlainScalingTexture:	/* Obsolete command */
						case TextKeyScalingTexture:
							ScalingTexture = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyRadiusCollision:
							RadiusCollision = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyUserData:
							UserData = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyInstance:
							Instance = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyEffect:
							Effect = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyDeform:
							Deform = KindGetPackName(textArgument[1]);
							return(true);

						case TextKeyFixIndexCellMap:	/* Obsolete command */
						case TextKeyFixCoordinate:	/* Obsolete command */
						case TextKeyFixUV0:	/* Obsolete command */
						case TextKeyFixSizeCollision:	/* Obsolete command */
						case TextKeyFixPivotCollision:	/* Obsolete command */
							return(true);

						default:
							break;
					}
					return(false);
				}

				public void Adjust()
				{
					int countPack = (int)Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.TERMINATOR;
					Library_SpriteStudio6.Data.Animation.PackAttribute.CapacityContainer[] capacityPack = new Library_SpriteStudio6.Data.Animation.PackAttribute.CapacityContainer[countPack];
					for(int i=0; i<countPack; i++)
					{
						capacityPack[i] = Library_SpriteStudio6.Data.Animation.PackAttribute.CapacityGet((Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)i);
					}

					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack PackError = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE;	/* all Attributes Support */
					if(false == capacityPack[(int)Status].Status)
					{
						Status = PackError;
					}
					if(false == capacityPack[(int)Cell].Cell)
					{
						Cell = PackError;
					}
					if(false == capacityPack[(int)Position].Position)
					{
						Position = PackError;
					}
					if(false == capacityPack[(int)Rotation].Rotation)
					{
						Rotation = PackError;
					}
					if(false == capacityPack[(int)Scaling].Scaling)
					{
						Scaling = PackError;
					}
					if(false == capacityPack[(int)ScalingLocal].ScalingLocal)
					{
						ScalingLocal = PackError;
					}
					if(false == capacityPack[(int)RateOpacity].RateOpacity)
					{
						RateOpacity = PackError;
					}
					if(false == capacityPack[(int)Priority].Priority)
					{
						Priority = PackError;
					}
					if(false == capacityPack[(int)PartsColor].PartsColor)
					{
						PartsColor = PackError;
					}
					if(false == capacityPack[(int)VertexCorrection].VertexCorrection)
					{
						VertexCorrection = PackError;
					}
					if(false == capacityPack[(int)OffsetPivot].OffsetPivot)
					{
						OffsetPivot = PackError;
					}
					if(false == capacityPack[(int)PositionAnchor].PositionAnchor)
					{
						PositionAnchor = PackError;
					}
					if(false == capacityPack[(int)SizeForce].SizeForce)
					{
						SizeForce = PackError;
					}
					if(false == capacityPack[(int)PositionTexture].PositionTexture)
					{
						PositionTexture = PackError;
					}
					if(false == capacityPack[(int)RotationTexture].RotationTexture)
					{
						RotationTexture = PackError;
					}
					if(false == capacityPack[(int)ScalingTexture].ScalingTexture)
					{
						ScalingTexture = PackError;
					}
					if(false == capacityPack[(int)RadiusCollision].RadiusCollision)
					{
						RadiusCollision = PackError;
					}
					if(false == capacityPack[(int)UserData].UserData)
					{
						UserData = PackError;
					}
					if(false == capacityPack[(int)Instance].Instance)
					{
						Instance = PackError;
					}
					if(false == capacityPack[(int)Effect].Effect)
					{
						Effect = PackError;
					}
					if(false == capacityPack[(int)Deform].Deform)
					{
						Deform = PackError;
					}
				}

				private static void BootUpNamePack()
				{
					int countPack = (int)Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.TERMINATOR;
					if(null == TableNamePack)
					{
						TableNamePack = new string[countPack];
						for(int i=0; i<countPack; i++)
						{
							TableNamePack[i] = Library_SpriteStudio6.Data.Animation.PackAttribute.IDGetPack((Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)i);
							
						}
					}
				}
				private static string NameGetPackKind(Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack pack)
				{
					BootUpNamePack();
					return(TableNamePack[(int)pack]);
				}
				private static Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack KindGetPackName(string name)
				{
					BootUpNamePack();
					int count = TableNamePack.Length;
					for(int i=0; i<count; i++)
					{
						if(name == TableNamePack[i])
						{
							return((Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)i);
						}
					}
					return((Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(-1));
				}
#endif
				#endregion Functions

				/* ----------------------------------------------- Enums & Constants */
				#region Enums & Constants
#if !_FOR_SS6CONVERTER_
				private static string[] TableNamePack = null;
#endif

				private const string KeyStatus = "Status";
				private const string KeyCell = "Cell";
				private const string KeyPosition = "Position";
				private const string KeyRotation = "Rotation";
				private const string KeyScaling = "Scaling";
				private const string KeyScalingLocal = "ScalingLocal";
				private const string KeyRateOpacity = "RateOpacity";
				private const string KeyPriority = "Priority";
				private const string KeyPartsColor = "PartsColor";
				private const string KeyVertexCorrection = "VertexCorrection";
				private const string KeyOffsetPivot = "OffsetPivot";
				private const string KeyPositionAnchor = "PositionAnchor";
				private const string KeySizeForce = "SizeForce";
				private const string KeyPositionTexture = "PositionTexture";
				private const string KeyRotationTexture = "RotationTexture";
				private const string KeyScalingTexture = "ScalingTexture";
				private const string KeyRadiusCollision = "RadiusCollision";
				private const string KeyUserData = "UserData";
				private const string KeyInstance = "Instance";
				private const string KeyEffect = "Effect";
				private const string KeyDeform = "Deform";
				/* Obsolete */	private const string KeyPlainCell = "PlainCell";
				/* Obsolete */	private const string KeyPlainSizeForce = "PlainSizeForce";
				/* Obsolete */	private const string KeyPlainVertexCorrection = "PlainVertexCorrection";
				/* Obsolete */	private const string KeyPlainOffsetPivot = "PlainOffsetPivot";
				/* Obsolete */	private const string KeyPlainPositionTexture = "PlainPositionTexture";
				/* Obsolete */	private const string KeyPlainScalingTexture = "PlainScalingTexture";
				/* Obsolete */	private const string KeyPlainRotationTexture = "PlainRotationTexture";
				/* Obsolete */	private const string KeyFixIndexCellMap = "FixIndexCellMap";
				/* Obsolete */	private const string KeyFixCoordinate = "FixCoordinate";
				/* Obsolete */	private const string KeyFixUV0 = "FixUV0";
				/* Obsolete */	private const string KeyFixSizeCollision = "FixSizeCollision";
				/* Obsolete */	private const string KeyFixPivotCollision = "FixPivotCollision";

				private const string TextKeyPrefix = "PackAttributeAnimation_";
				private const string TextKeyStatus = TextKeyPrefix + KeyStatus;
				private const string TextKeyCell = TextKeyPrefix + KeyCell;
				private const string TextKeyPosition = TextKeyPrefix + KeyPosition;
				private const string TextKeyRotation = TextKeyPrefix + KeyRotation;
				private const string TextKeyScaling = TextKeyPrefix + KeyScaling;
				private const string TextKeyScalingLocal = TextKeyPrefix + KeyScalingLocal;
				private const string TextKeyRateOpacity = TextKeyPrefix + KeyRateOpacity;
				private const string TextKeyPriority = TextKeyPrefix + KeyPriority;
				private const string TextKeyPartsColor = TextKeyPrefix + KeyPartsColor;
				private const string TextKeyVertexCorrection = TextKeyPrefix + KeyVertexCorrection;
				private const string TextKeyOffsetPivot = TextKeyPrefix + KeyOffsetPivot;
				private const string TextKeyPositionAnchor = TextKeyPrefix + KeyPositionAnchor;
				private const string TextKeySizeForce = TextKeyPrefix + KeySizeForce;
				private const string TextKeyPositionTexture = TextKeyPrefix + KeyPositionTexture;
				private const string TextKeyRotationTexture = TextKeyPrefix + KeyRotationTexture;
				private const string TextKeyScalingTexture = TextKeyPrefix + KeyScalingTexture;
				private const string TextKeyRadiusCollision = TextKeyPrefix + KeyRadiusCollision;
				private const string TextKeyUserData = TextKeyPrefix + KeyUserData;
				private const string TextKeyInstance = TextKeyPrefix + KeyInstance;
				private const string TextKeyEffect = TextKeyPrefix + KeyEffect;
				private const string TextKeyDeform = TextKeyPrefix + KeyDeform;
				/* Obsolete */	private const string TextKeyPlainCell = TextKeyPrefix + KeyPlainCell;
				/* Obsolete */	private const string TextKeyPlainSizeForce = TextKeyPrefix + KeyPlainSizeForce;
				/* Obsolete */	private const string TextKeyPlainVertexCorrection = TextKeyPrefix + KeyPlainVertexCorrection;
				/* Obsolete */	private const string TextKeyPlainOffsetPivot = TextKeyPrefix + KeyPlainOffsetPivot;
				/* Obsolete */	private const string TextKeyPlainPositionTexture = TextKeyPrefix + KeyPlainPositionTexture;
				/* Obsolete */	private const string TextKeyPlainScalingTexture = TextKeyPrefix + KeyPlainScalingTexture;
				/* Obsolete */	private const string TextKeyPlainRotationTexture = TextKeyPrefix + KeyPlainRotationTexture;
				/* Obsolete */	private const string TextKeyFixIndexCellMap = TextKeyPrefix + KeyFixIndexCellMap;
				/* Obsolete */	private const string TextKeyFixCoordinate = TextKeyPrefix + KeyFixCoordinate;
				/* Obsolete */	private const string TextKeyFixUV0 = TextKeyPrefix + KeyFixUV0;
				/* Obsolete */	private const string TextKeyFixSizeCollision = TextKeyPrefix + KeyFixSizeCollision;
				/* Obsolete */	private const string TextKeyFixPivotCollision = TextKeyPrefix + KeyFixPivotCollision;

				private const string PrefsKeyPrefix = LibraryEditor_SpriteStudio6.Import.Setting.PrefsKeyPrefix + TextKeyPrefix;
				private const string PrefsKeyStatus = PrefsKeyPrefix + KeyStatus;
				private const string PrefsKeyCell = PrefsKeyPrefix + KeyCell;
				private const string PrefsKeyPosition = PrefsKeyPrefix + KeyPosition;
				private const string PrefsKeyRotation = PrefsKeyPrefix + KeyRotation;
				private const string PrefsKeyScaling = PrefsKeyPrefix + KeyScaling;
				private const string PrefsKeyScalingLocal = PrefsKeyPrefix + KeyScalingLocal;
				private const string PrefsKeyRateOpacity = PrefsKeyPrefix + KeyRateOpacity;
				private const string PrefsKeyPriority = PrefsKeyPrefix + KeyPriority;
				private const string PrefsKeyPartsColor = PrefsKeyPrefix + KeyPartsColor;
				private const string PrefsKeyVertexCorrection = PrefsKeyPrefix + KeyVertexCorrection;
				private const string PrefsKeyOffsetPivot = PrefsKeyPrefix + KeyOffsetPivot;
				private const string PrefsKeyPositionAnchor = PrefsKeyPrefix + KeyPositionAnchor;
				private const string PrefsKeySizeForce = PrefsKeyPrefix + KeySizeForce;
				private const string PrefsKeyPositionTexture = PrefsKeyPrefix + KeyPositionTexture;
				private const string PrefsKeyRotationTexture = PrefsKeyPrefix + KeyRotationTexture;
				private const string PrefsKeyScalingTexture = PrefsKeyPrefix + KeyScalingTexture;
				private const string PrefsKeyRadiusCollision = PrefsKeyPrefix + KeyRadiusCollision;
				private const string PrefsKeyUserData = PrefsKeyPrefix + KeyUserData;
				private const string PrefsKeyInstance = PrefsKeyPrefix + KeyInstance;
				private const string PrefsKeyEffect = PrefsKeyPrefix + KeyEffect;
				private const string PrefsKeyDeform = PrefsKeyPrefix + KeyDeform;
				/* Obsolete */	private const string PrefsKeyPlainCell = PrefsKeyPrefix + KeyPlainCell;
				/* Obsolete */	private const string PrefsKeyPlainSizeForce = PrefsKeyPrefix + KeyPlainSizeForce;
				/* Obsolete */	private const string PrefsKeyPlainVertexCorrection = PrefsKeyPrefix + KeyPlainVertexCorrection;
				/* Obsolete */	private const string PrefsKeyPlainOffsetPivot = PrefsKeyPrefix + KeyPlainOffsetPivot;
				/* Obsolete */	private const string PrefsKeyPlainPositionTexture = PrefsKeyPrefix + KeyPlainPositionTexture;
				/* Obsolete */	private const string PrefsKeyPlainScalingTexture = PrefsKeyPrefix + KeyPlainScalingTexture;
				/* Obsolete */	private const string PrefsKeyPlainRotationTexture = PrefsKeyPrefix + KeyPlainRotationTexture;
				/* Obsolete */	private const string PrefsKeyFixIndexCellMap = PrefsKeyPrefix + KeyFixIndexCellMap;
				/* Obsolete */	private const string PrefsKeyFixCoordinate = PrefsKeyPrefix + KeyFixCoordinate;
				/* Obsolete */	private const string PrefsKeyFixUV0 = PrefsKeyPrefix + KeyFixUV0;
				/* Obsolete */	private const string PrefsKeyFixSizeCollision = PrefsKeyPrefix + KeyFixSizeCollision;
				/* Obsolete */	private const string PrefsKeyFixPivotCollision = PrefsKeyPrefix + KeyFixPivotCollision;

				private readonly static GroupPackAttributeAnimation Default = new GroupPackAttributeAnimation(
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* Status */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* Cell */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* Position */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* Rotation */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* Scaling */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* ScalingLocal */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* RateOpacity */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* Priority */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* PartsColor */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* VertexCorrection */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* OffsetPivot */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* PositionAnchor */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* SizeForce */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* PositionTexture */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* RotationTexture */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* ScalingTexture */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* RadiusCollision */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* UserData */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* Instance */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE,	/* Effect */
					Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.CPE_INTERPOLATE	/* Deform */
				);
				#endregion Enums & Constants
			}
#if !_FOR_SS6CONVERTER_
			public struct GroupPresetMaterial
			{
				/* ----------------------------------------------- Variables & Properties */
				#region Variables & Properties
//				/* Obsolete */	public Material AnimationSS6PUStencilPreDraw;
//				/* Obsolete */	public Material AnimationSS6PUStencilDraw;
				public Material AnimationSS6PUThroughStencilPreDraw;
				public Material AnimationSS6PUThroughStencilDraw;
				public Material AnimationSS6PUThroughMix;
				public Material AnimationSS6PUThroughAdd;
				public Material AnimationSS6PUThroughSub;
				public Material AnimationSS6PUThroughMul;
				public Material AnimationSS6PUThroughMulNA;
				public Material AnimationSS6PUThroughScr;
				public Material AnimationSS6PUThroughExc;
				public Material AnimationSS6PUThroughInv;
				public Material AnimationSS6PUMaskStencilPreDraw;
				public Material AnimationSS6PUMaskStencilDraw;
				public Material AnimationSS6PUMaskMix;
				public Material AnimationSS6PUMaskAdd;
				public Material AnimationSS6PUMaskSub;
				public Material AnimationSS6PUMaskMul;
				public Material AnimationSS6PUMaskMulNA;
				public Material AnimationSS6PUMaskScr;
				public Material AnimationSS6PUMaskExc;
				public Material AnimationSS6PUMaskInv;

				public Material EffectSS6PUThroughMix;
				public Material EffectSS6PUThroughAdd;
				public Material EffectSS6PUMaskMix;
				public Material EffectSS6PUMaskAdd;

				public Material AnimationUnityNativeMix;
				public Material AnimationUnityNativeAdd;
				public Material AnimationUnityNativeSub;
				public Material AnimationUnityNativeMul;
				public Material AnimationUnityNativeMulNA;
				public Material AnimationUnityNativeScr;
				public Material AnimationUnityNativeExc;
				public Material AnimationUnityNativeInv;
				public Material AnimationUnityNativeNonBatchMix;
				public Material AnimationUnityNativeNonBatchAdd;
				public Material AnimationUnityNativeNonBatchSub;
				public Material AnimationUnityNativeNonBatchMul;
				public Material AnimationUnityNativeNonBatchMulNA;
				public Material AnimationUnityNativeNonBatchScr;
				public Material AnimationUnityNativeNonBatchExc;
				public Material AnimationUnityNativeNonBatchInv;

				public Material SkinnedMeshUnityNativeMix;
				public Material SkinnedMeshUnityNativeAdd;
				public Material SkinnedMeshUnityNativeSub;
				public Material SkinnedMeshUnityNativeMul;
				public Material SkinnedMeshUnityNativeMulNA;
				public Material SkinnedMeshUnityNativeScr;
				public Material SkinnedMeshUnityNativeExc;
				public Material SkinnedMeshUnityNativeInv;
				#endregion Variables & Properties

				/* ----------------------------------------------- Functions */
				#region Functions
				public GroupPresetMaterial(	string animationSS6PUThroughStencilPreDraw,
											string animationSS6PUThroughStencilDraw,
											string animationSS6PUThroughMix,
											string animationSS6PUThroughAdd,
											string animationSS6PUThroughSub,
											string animationSS6PUThroughMul,
											string animationSS6PUThroughMulNA,
											string animationSS6PUThroughScr,
											string animationSS6PUThroughExc,
											string animationSS6PUThroughInv,
											string animationSS6PUMaskStencilPreDraw,
											string animationSS6PUMaskStencilDraw,
											string animationSS6PUMaskMix,
											string animationSS6PUMaskAdd,
											string animationSS6PUMaskSub,
											string animationSS6PUMaskMul,
											string animationSS6PUMaskMulNA,
											string animationSS6PUMaskScr,
											string animationSS6PUMaskExc,
											string animationSS6PUMaskInv,
											string effectSS6PUThroughMix,
											string effectSS6PUThroughAdd,
											string effectSS6PUMaskMix,
											string effectSS6PUMaskAdd,
											string animationUnityNativeMix,
											string animationUnityNativeAdd,
											string animationUnityNativeSub,
											string animationUnityNativeMul,
											string animationUnityNativeMulNA,
											string animationUnityNativeScr,
											string animationUnityNativeExc,
											string animationUnityNativeInv,
											string animationUnityNativeNonBatchMix,
											string animationUnityNativeNonBatchAdd,
											string animationUnityNativeNonBatchSub,
											string animationUnityNativeNonBatchMul,
											string animationUnityNativeNonBatchMulNA,
											string animationUnityNativeNonBatchScr,
											string animationUnityNativeNonBatchExc,
											string animationUnityNativeNonBatchInv,
											string skinnedMeshUnityNativeMix,
											string skinnedMeshUnityNativeAdd,
											string skinnedMeshUnityNativeSub,
											string skinnedMeshUnityNativeMul,
											string skinnedMeshUnityNativeMulNA,
											string skinnedMeshUnityNativeScr,
											string skinnedMeshUnityNativeExc,
											string skinnedMeshUnityNativeInv
										)
				{
					AnimationSS6PUThroughStencilPreDraw = MaterialLoadPath(PathGet(animationSS6PUThroughStencilPreDraw));
					AnimationSS6PUThroughStencilDraw = MaterialLoadPath(PathGet(animationSS6PUThroughStencilDraw));
					AnimationSS6PUThroughMix = MaterialLoadPath(PathGet(animationSS6PUThroughMix));
					AnimationSS6PUThroughAdd = MaterialLoadPath(PathGet(animationSS6PUThroughAdd));
					AnimationSS6PUThroughSub = MaterialLoadPath(PathGet(animationSS6PUThroughSub));
					AnimationSS6PUThroughMul = MaterialLoadPath(PathGet(animationSS6PUThroughMul));
					AnimationSS6PUThroughMulNA = MaterialLoadPath(PathGet(animationSS6PUThroughMulNA));
					AnimationSS6PUThroughScr = MaterialLoadPath(PathGet(animationSS6PUThroughScr));
					AnimationSS6PUThroughExc = MaterialLoadPath(PathGet(animationSS6PUThroughExc));
					AnimationSS6PUThroughInv = MaterialLoadPath(PathGet(animationSS6PUThroughInv));
					AnimationSS6PUMaskStencilPreDraw = MaterialLoadPath(PathGet(animationSS6PUMaskStencilPreDraw));
					AnimationSS6PUMaskStencilDraw = MaterialLoadPath(PathGet(animationSS6PUMaskStencilDraw));
					AnimationSS6PUMaskMix = MaterialLoadPath(PathGet(animationSS6PUMaskMix));
					AnimationSS6PUMaskAdd = MaterialLoadPath(PathGet(animationSS6PUMaskAdd));
					AnimationSS6PUMaskSub = MaterialLoadPath(PathGet(animationSS6PUMaskSub));
					AnimationSS6PUMaskMul = MaterialLoadPath(PathGet(animationSS6PUMaskMul));
					AnimationSS6PUMaskMulNA = MaterialLoadPath(PathGet(animationSS6PUMaskMulNA));
					AnimationSS6PUMaskScr = MaterialLoadPath(PathGet(animationSS6PUMaskScr));
					AnimationSS6PUMaskExc = MaterialLoadPath(PathGet(animationSS6PUMaskExc));
					AnimationSS6PUMaskInv = MaterialLoadPath(PathGet(animationSS6PUMaskInv));

					EffectSS6PUThroughMix = MaterialLoadPath(PathGet(effectSS6PUThroughMix));
					EffectSS6PUThroughAdd = MaterialLoadPath(PathGet(effectSS6PUThroughAdd));
					EffectSS6PUMaskMix = MaterialLoadPath(PathGet(effectSS6PUMaskMix));
					EffectSS6PUMaskAdd = MaterialLoadPath(PathGet(effectSS6PUMaskAdd));

					AnimationUnityNativeMix = MaterialLoadPath(PathGet(animationUnityNativeMix));
					AnimationUnityNativeAdd = MaterialLoadPath(PathGet(animationUnityNativeAdd));
					AnimationUnityNativeSub = MaterialLoadPath(PathGet(animationUnityNativeSub));
					AnimationUnityNativeMul = MaterialLoadPath(PathGet(animationUnityNativeMul));
					AnimationUnityNativeMulNA = MaterialLoadPath(PathGet(animationUnityNativeMulNA));
					AnimationUnityNativeScr = MaterialLoadPath(PathGet(animationUnityNativeScr));
					AnimationUnityNativeExc = MaterialLoadPath(PathGet(animationUnityNativeExc));
					AnimationUnityNativeInv = MaterialLoadPath(PathGet(animationUnityNativeInv));

					AnimationUnityNativeNonBatchMix = MaterialLoadPath(PathGet(animationUnityNativeNonBatchMix));
					AnimationUnityNativeNonBatchAdd = MaterialLoadPath(PathGet(animationUnityNativeNonBatchAdd));
					AnimationUnityNativeNonBatchSub = MaterialLoadPath(PathGet(animationUnityNativeNonBatchSub));
					AnimationUnityNativeNonBatchMul = MaterialLoadPath(PathGet(animationUnityNativeNonBatchMul));
					AnimationUnityNativeNonBatchMulNA = MaterialLoadPath(PathGet(animationUnityNativeNonBatchMulNA));
					AnimationUnityNativeNonBatchScr = MaterialLoadPath(PathGet(animationUnityNativeNonBatchScr));
					AnimationUnityNativeNonBatchExc = MaterialLoadPath(PathGet(animationUnityNativeNonBatchExc));
					AnimationUnityNativeNonBatchInv = MaterialLoadPath(PathGet(animationUnityNativeNonBatchInv));

					SkinnedMeshUnityNativeMix = MaterialLoadPath(PathGet(skinnedMeshUnityNativeMix));
					SkinnedMeshUnityNativeAdd = MaterialLoadPath(PathGet(skinnedMeshUnityNativeAdd));
					SkinnedMeshUnityNativeSub = MaterialLoadPath(PathGet(skinnedMeshUnityNativeSub));
					SkinnedMeshUnityNativeMul = MaterialLoadPath(PathGet(skinnedMeshUnityNativeMul));
					SkinnedMeshUnityNativeMulNA = MaterialLoadPath(PathGet(skinnedMeshUnityNativeMulNA));
					SkinnedMeshUnityNativeScr = MaterialLoadPath(PathGet(skinnedMeshUnityNativeScr));
					SkinnedMeshUnityNativeExc = MaterialLoadPath(PathGet(skinnedMeshUnityNativeExc));
					SkinnedMeshUnityNativeInv = MaterialLoadPath(PathGet(skinnedMeshUnityNativeInv));
				}

				public void CleanUp()
				{
					this = Default;
				}

				public bool Load()
				{
					string guid = "";

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUThroughStencilPreDraw, GUIDGetMaterial(Default.AnimationSS6PUThroughStencilPreDraw));
					AnimationSS6PUThroughStencilPreDraw = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUThroughStencilDraw, GUIDGetMaterial(Default.AnimationSS6PUThroughStencilDraw));
					AnimationSS6PUThroughStencilDraw = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUThroughMix, GUIDGetMaterial(Default.AnimationSS6PUThroughMix));
					AnimationSS6PUThroughMix = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUThroughAdd, GUIDGetMaterial(Default.AnimationSS6PUThroughAdd));
					AnimationSS6PUThroughAdd = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUThroughSub, GUIDGetMaterial(Default.AnimationSS6PUThroughSub));
					AnimationSS6PUThroughSub = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUThroughMul, GUIDGetMaterial(Default.AnimationSS6PUThroughMul));
					AnimationSS6PUThroughMul = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUThroughMulNA, GUIDGetMaterial(Default.AnimationSS6PUThroughMulNA));
					AnimationSS6PUThroughMulNA = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUThroughScr, GUIDGetMaterial(Default.AnimationSS6PUThroughScr));
					AnimationSS6PUThroughScr = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUThroughExc, GUIDGetMaterial(Default.AnimationSS6PUThroughExc));
					AnimationSS6PUThroughExc = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUThroughInv, GUIDGetMaterial(Default.AnimationSS6PUThroughInv));
					AnimationSS6PUThroughInv = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUMaskStencilPreDraw, GUIDGetMaterial(Default.AnimationSS6PUMaskStencilPreDraw));
					AnimationSS6PUMaskStencilPreDraw = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUMaskStencilDraw, GUIDGetMaterial(Default.AnimationSS6PUMaskStencilDraw));
					AnimationSS6PUMaskStencilDraw = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUMaskMix, GUIDGetMaterial(Default.AnimationSS6PUMaskMix));
					AnimationSS6PUMaskMix = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUMaskAdd, GUIDGetMaterial(Default.AnimationSS6PUMaskAdd));
					AnimationSS6PUMaskAdd = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUMaskSub, GUIDGetMaterial(Default.AnimationSS6PUMaskSub));
					AnimationSS6PUMaskSub = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUMaskMul, GUIDGetMaterial(Default.AnimationSS6PUMaskMul));
					AnimationSS6PUMaskMul = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUMaskMulNA, GUIDGetMaterial(Default.AnimationSS6PUMaskMulNA));
					AnimationSS6PUMaskMulNA = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUMaskScr, GUIDGetMaterial(Default.AnimationSS6PUMaskScr));
					AnimationSS6PUMaskScr = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUMaskExc, GUIDGetMaterial(Default.AnimationSS6PUMaskExc));
					AnimationSS6PUMaskExc = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationSS6PUMaskInv, GUIDGetMaterial(Default.AnimationSS6PUMaskInv));
					AnimationSS6PUMaskInv = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyEffectSS6PUThroughMix, GUIDGetMaterial(Default.EffectSS6PUThroughMix));
					EffectSS6PUThroughMix = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyEffectSS6PUThroughAdd, GUIDGetMaterial(Default.EffectSS6PUThroughAdd));
					EffectSS6PUThroughAdd = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyEffectSS6PUMaskMix, GUIDGetMaterial(Default.EffectSS6PUMaskMix));
					EffectSS6PUMaskMix = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyEffectSS6PUMaskAdd, GUIDGetMaterial(Default.EffectSS6PUMaskAdd));
					EffectSS6PUMaskAdd = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationUnityNativeMix, GUIDGetMaterial(Default.AnimationUnityNativeMix));
					AnimationUnityNativeMix = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationUnityNativeAdd, GUIDGetMaterial(Default.AnimationUnityNativeAdd));
					AnimationUnityNativeAdd = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationUnityNativeSub, GUIDGetMaterial(Default.AnimationUnityNativeSub));
					AnimationUnityNativeSub = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationUnityNativeMul, GUIDGetMaterial(Default.AnimationUnityNativeMul));
					AnimationUnityNativeMul = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationUnityNativeMulNA, GUIDGetMaterial(Default.AnimationUnityNativeMulNA));
					AnimationUnityNativeMulNA = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationUnityNativeScr, GUIDGetMaterial(Default.AnimationUnityNativeScr));
					AnimationUnityNativeScr = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationUnityNativeExc, GUIDGetMaterial(Default.AnimationUnityNativeExc));
					AnimationUnityNativeExc = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationUnityNativeInv, GUIDGetMaterial(Default.AnimationUnityNativeInv));
					AnimationUnityNativeInv = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationUnityNativeNonBatchMix, GUIDGetMaterial(Default.AnimationUnityNativeNonBatchMix));
					AnimationUnityNativeNonBatchMix = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationUnityNativeNonBatchAdd, GUIDGetMaterial(Default.AnimationUnityNativeNonBatchAdd));
					AnimationUnityNativeNonBatchAdd = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationUnityNativeNonBatchSub, GUIDGetMaterial(Default.AnimationUnityNativeNonBatchSub));
					AnimationUnityNativeNonBatchSub = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationUnityNativeNonBatchMul, GUIDGetMaterial(Default.AnimationUnityNativeNonBatchMul));
					AnimationUnityNativeNonBatchMul = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationUnityNativeNonBatchMulNA, GUIDGetMaterial(Default.AnimationUnityNativeNonBatchMulNA));
					AnimationUnityNativeNonBatchMulNA = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationUnityNativeNonBatchScr, GUIDGetMaterial(Default.AnimationUnityNativeNonBatchScr));
					AnimationUnityNativeNonBatchScr = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationUnityNativeNonBatchExc, GUIDGetMaterial(Default.AnimationUnityNativeNonBatchExc));
					AnimationUnityNativeNonBatchExc = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeyAnimationUnityNativeNonBatchInv, GUIDGetMaterial(Default.AnimationUnityNativeNonBatchInv));
					AnimationUnityNativeNonBatchInv = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeySkinnedMeshUnityNativeMix, GUIDGetMaterial(Default.SkinnedMeshUnityNativeMix));
					SkinnedMeshUnityNativeMix = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeySkinnedMeshUnityNativeAdd, GUIDGetMaterial(Default.SkinnedMeshUnityNativeAdd));
					SkinnedMeshUnityNativeAdd = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeySkinnedMeshUnityNativeSub, GUIDGetMaterial(Default.SkinnedMeshUnityNativeSub));
					SkinnedMeshUnityNativeSub = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeySkinnedMeshUnityNativeMul, GUIDGetMaterial(Default.SkinnedMeshUnityNativeMul));
					SkinnedMeshUnityNativeMul = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeySkinnedMeshUnityNativeMulNA, GUIDGetMaterial(Default.SkinnedMeshUnityNativeMulNA));
					SkinnedMeshUnityNativeMulNA = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeySkinnedMeshUnityNativeScr, GUIDGetMaterial(Default.SkinnedMeshUnityNativeScr));
					SkinnedMeshUnityNativeScr = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeySkinnedMeshUnityNativeExc, GUIDGetMaterial(Default.SkinnedMeshUnityNativeExc));
					SkinnedMeshUnityNativeExc = MaterialGetGUID(guid);

					guid = LibraryEditor_SpriteStudio6.Utility.Prefs.StringLoad(PrefsKeySkinnedMeshUnityNativeInv, GUIDGetMaterial(Default.SkinnedMeshUnityNativeInv));
					SkinnedMeshUnityNativeInv = MaterialGetGUID(guid);

					return(true);
				}

				public bool Save()
				{
					string guid = "";

					guid = GUIDGetMaterial(AnimationSS6PUThroughStencilPreDraw);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUThroughStencilPreDraw, guid);

					guid = GUIDGetMaterial(AnimationSS6PUThroughStencilDraw);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUThroughStencilDraw, guid);

					guid = GUIDGetMaterial(AnimationSS6PUThroughMix);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUThroughMix, guid);

					guid = GUIDGetMaterial(AnimationSS6PUThroughAdd);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUThroughAdd, guid);

					guid = GUIDGetMaterial(AnimationSS6PUThroughSub);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUThroughSub, guid);

					guid = GUIDGetMaterial(AnimationSS6PUThroughMul);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUThroughMul, guid);

					guid = GUIDGetMaterial(AnimationSS6PUThroughMulNA);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUThroughMulNA, guid);

					guid = GUIDGetMaterial(AnimationSS6PUThroughScr);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUThroughScr, guid);

					guid = GUIDGetMaterial(AnimationSS6PUThroughExc);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUThroughExc, guid);

					guid = GUIDGetMaterial(AnimationSS6PUThroughInv);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUThroughInv, guid);

					guid = GUIDGetMaterial(AnimationSS6PUMaskStencilPreDraw);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUMaskStencilPreDraw, guid);

					guid = GUIDGetMaterial(AnimationSS6PUMaskStencilDraw);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUMaskStencilDraw, guid);

					guid = GUIDGetMaterial(AnimationSS6PUMaskMix);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUMaskMix, guid);

					guid = GUIDGetMaterial(AnimationSS6PUMaskAdd);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUMaskAdd, guid);

					guid = GUIDGetMaterial(AnimationSS6PUMaskSub);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUMaskSub, guid);

					guid = GUIDGetMaterial(AnimationSS6PUMaskMul);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUMaskMul, guid);

					guid = GUIDGetMaterial(AnimationSS6PUMaskMulNA);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUMaskMulNA, guid);

					guid = GUIDGetMaterial(AnimationSS6PUMaskScr);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUMaskScr, guid);

					guid = GUIDGetMaterial(AnimationSS6PUMaskExc);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUMaskExc, guid);

					guid = GUIDGetMaterial(AnimationSS6PUMaskInv);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationSS6PUMaskInv, guid);

					guid = GUIDGetMaterial(EffectSS6PUThroughMix);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyEffectSS6PUThroughMix, guid);

					guid = GUIDGetMaterial(EffectSS6PUThroughAdd);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyEffectSS6PUThroughAdd, guid);

					guid = GUIDGetMaterial(EffectSS6PUMaskMix);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyEffectSS6PUMaskMix, guid);

					guid = GUIDGetMaterial(EffectSS6PUMaskAdd);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyEffectSS6PUMaskAdd, guid);

					guid = GUIDGetMaterial(AnimationUnityNativeMix);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationUnityNativeMix, guid);

					guid = GUIDGetMaterial(AnimationUnityNativeAdd);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationUnityNativeAdd, guid);

					guid = GUIDGetMaterial(AnimationUnityNativeSub);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationUnityNativeSub, guid);

					guid = GUIDGetMaterial(AnimationUnityNativeMul);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationUnityNativeMul, guid);

					guid = GUIDGetMaterial(AnimationUnityNativeMulNA);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationUnityNativeMulNA, guid);

					guid = GUIDGetMaterial(AnimationUnityNativeScr);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationUnityNativeScr, guid);

					guid = GUIDGetMaterial(AnimationUnityNativeExc);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationUnityNativeExc, guid);

					guid = GUIDGetMaterial(AnimationUnityNativeInv);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationUnityNativeInv, guid);

					guid = GUIDGetMaterial(AnimationUnityNativeNonBatchMix);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationUnityNativeNonBatchMix, guid);

					guid = GUIDGetMaterial(AnimationUnityNativeNonBatchAdd);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationUnityNativeNonBatchAdd, guid);

					guid = GUIDGetMaterial(AnimationUnityNativeNonBatchSub);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationUnityNativeNonBatchSub, guid);

					guid = GUIDGetMaterial(AnimationUnityNativeNonBatchMul);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationUnityNativeNonBatchMul, guid);

					guid = GUIDGetMaterial(AnimationUnityNativeNonBatchMulNA);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationUnityNativeNonBatchMulNA, guid);

					guid = GUIDGetMaterial(AnimationUnityNativeNonBatchScr);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationUnityNativeNonBatchScr, guid);

					guid = GUIDGetMaterial(AnimationUnityNativeNonBatchExc);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationUnityNativeNonBatchExc, guid);

					guid = GUIDGetMaterial(AnimationUnityNativeNonBatchInv);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeyAnimationUnityNativeNonBatchInv, guid);

					guid = GUIDGetMaterial(SkinnedMeshUnityNativeMix);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeySkinnedMeshUnityNativeMix, guid);

					guid = GUIDGetMaterial(SkinnedMeshUnityNativeAdd);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeySkinnedMeshUnityNativeAdd, guid);

					guid = GUIDGetMaterial(SkinnedMeshUnityNativeSub);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeySkinnedMeshUnityNativeSub, guid);

					guid = GUIDGetMaterial(SkinnedMeshUnityNativeMul);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeySkinnedMeshUnityNativeMul, guid);

					guid = GUIDGetMaterial(SkinnedMeshUnityNativeMulNA);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeySkinnedMeshUnityNativeMulNA, guid);

					guid = GUIDGetMaterial(SkinnedMeshUnityNativeScr);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeySkinnedMeshUnityNativeScr, guid);

					guid = GUIDGetMaterial(SkinnedMeshUnityNativeExc);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeySkinnedMeshUnityNativeExc, guid);

					guid = GUIDGetMaterial(SkinnedMeshUnityNativeInv);
					LibraryEditor_SpriteStudio6.Utility.Prefs.StringSave(PrefsKeySkinnedMeshUnityNativeInv, guid);

					return(true);
				}

				public string[] Export()
				{
					string[] textEncode = new string[48];
					string textValue;

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUThroughStencilPreDraw));
					textEncode[0] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUThroughStencilPreDraw, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUThroughStencilDraw));
					textEncode[1] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUThroughStencilDraw, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUThroughMix));
					textEncode[2] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUThroughMix, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUThroughAdd));
					textEncode[3] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUThroughAdd, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUThroughSub));
					textEncode[4] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUThroughSub, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUThroughMul));
					textEncode[5] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUThroughMul, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUThroughMulNA));
					textEncode[6] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUThroughMulNA, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUThroughScr));
					textEncode[7] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUThroughScr, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUThroughExc));
					textEncode[8] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUThroughExc, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUThroughInv));
					textEncode[9] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUThroughInv, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUMaskStencilPreDraw));
					textEncode[10] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUMaskStencilPreDraw, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUMaskStencilDraw));
					textEncode[11] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUMaskStencilDraw, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUMaskMix));
					textEncode[12] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUMaskMix, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUMaskAdd));
					textEncode[13] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUMaskAdd, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUMaskSub));
					textEncode[14] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUMaskSub, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUMaskMul));
					textEncode[15] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUMaskMul, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUMaskMulNA));
					textEncode[16] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUMaskMulNA, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUMaskScr));
					textEncode[17] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUMaskScr, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUMaskExc));
					textEncode[18] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUMaskExc, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationSS6PUMaskInv));
					textEncode[19] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationSS6PUMaskInv, textValue);

					textValue = PathGetForExport(PathGetMaterial(EffectSS6PUThroughMix));
					textEncode[20] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyEffectSS6PUThroughMix, textValue);

					textValue = PathGetForExport(PathGetMaterial(EffectSS6PUThroughAdd));
					textEncode[21] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyEffectSS6PUThroughAdd, textValue);

					textValue = PathGetForExport(PathGetMaterial(EffectSS6PUMaskMix));
					textEncode[22] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyEffectSS6PUMaskMix, textValue);

					textValue = PathGetForExport(PathGetMaterial(EffectSS6PUMaskAdd));
					textEncode[23] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyEffectSS6PUMaskAdd, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationUnityNativeMix));
					textEncode[24] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationUnityNativeMix, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationUnityNativeAdd));
					textEncode[25] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationUnityNativeAdd, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationUnityNativeSub));
					textEncode[26] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationUnityNativeSub, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationUnityNativeMul));
					textEncode[27] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationUnityNativeMul, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationUnityNativeMulNA));
					textEncode[28] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationUnityNativeMulNA, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationUnityNativeScr));
					textEncode[29] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationUnityNativeScr, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationUnityNativeExc));
					textEncode[30] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationUnityNativeExc, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationUnityNativeInv));
					textEncode[31] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationUnityNativeInv, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationUnityNativeNonBatchMix));
					textEncode[32] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationUnityNativeNonBatchMix, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationUnityNativeNonBatchAdd));
					textEncode[33] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationUnityNativeNonBatchAdd, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationUnityNativeNonBatchSub));
					textEncode[34] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationUnityNativeNonBatchSub, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationUnityNativeNonBatchMul));
					textEncode[35] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationUnityNativeNonBatchMul, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationUnityNativeNonBatchMulNA));
					textEncode[36] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationUnityNativeNonBatchMulNA, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationUnityNativeNonBatchScr));
					textEncode[37] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationUnityNativeNonBatchScr, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationUnityNativeNonBatchExc));
					textEncode[38] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationUnityNativeNonBatchExc, textValue);

					textValue = PathGetForExport(PathGetMaterial(AnimationUnityNativeNonBatchInv));
					textEncode[39] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeyAnimationUnityNativeNonBatchInv, textValue);

					textValue = PathGetForExport(PathGetMaterial(SkinnedMeshUnityNativeMix));
					textEncode[40] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeySkinnedMeshUnityNativeMix, textValue);

					textValue = PathGetForExport(PathGetMaterial(SkinnedMeshUnityNativeAdd));
					textEncode[41] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeySkinnedMeshUnityNativeAdd, textValue);

					textValue = PathGetForExport(PathGetMaterial(SkinnedMeshUnityNativeSub));
					textEncode[42] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeySkinnedMeshUnityNativeSub, textValue);

					textValue = PathGetForExport(PathGetMaterial(SkinnedMeshUnityNativeMul));
					textEncode[43] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeySkinnedMeshUnityNativeMul, textValue);

					textValue = PathGetForExport(PathGetMaterial(SkinnedMeshUnityNativeMulNA));
					textEncode[44] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeySkinnedMeshUnityNativeMulNA, textValue);

					textValue = PathGetForExport(PathGetMaterial(SkinnedMeshUnityNativeScr));
					textEncode[45] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeySkinnedMeshUnityNativeScr, textValue);

					textValue = PathGetForExport(PathGetMaterial(SkinnedMeshUnityNativeExc));
					textEncode[46] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeySkinnedMeshUnityNativeExc, textValue);

					textValue = PathGetForExport(PathGetMaterial(SkinnedMeshUnityNativeInv));
					textEncode[47] = LibraryEditor_SpriteStudio6.Utility.ExternalText.LineEncodeCommand(TextKeySkinnedMeshUnityNativeInv, textValue);

					return(textEncode);
				}

				public bool Import(string[] textArgument)
				{
					string namePathMaterial = textArgument[1];
					switch(textArgument[0])
					{
						case TextKeyAnimationSS6PUThroughStencilPreDraw:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUThroughStencilPreDraw));
							}
							AnimationSS6PUThroughStencilPreDraw = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUThroughStencilDraw:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUThroughStencilDraw));
							}
							AnimationSS6PUThroughStencilDraw = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUThroughMix:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUThroughMix));
							}
							AnimationSS6PUThroughMix = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUThroughAdd:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUThroughAdd));
							}
							AnimationSS6PUThroughAdd = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUThroughSub:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUThroughSub));
							}
							AnimationSS6PUThroughSub = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUThroughMul:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUThroughMul));
							}
							AnimationSS6PUThroughMul = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUThroughMulNA:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUThroughMulNA));
							}
							AnimationSS6PUThroughMulNA = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUThroughScr:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUThroughScr));
							}
							AnimationSS6PUThroughScr = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUThroughExc:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUThroughExc));
							}
							AnimationSS6PUThroughExc = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUThroughInv:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUThroughInv));
							}
							AnimationSS6PUThroughInv = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUMaskStencilPreDraw:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUMaskStencilPreDraw));
							}
							AnimationSS6PUMaskStencilPreDraw = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUMaskStencilDraw:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUMaskStencilDraw));
							}
							AnimationSS6PUMaskStencilDraw = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUMaskMix:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUMaskMix));
							}
							AnimationSS6PUMaskMix = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUMaskAdd:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUMaskAdd));
							}
							AnimationSS6PUMaskAdd = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUMaskSub:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUMaskSub));
							}
							AnimationSS6PUMaskSub = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUMaskMul:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUMaskMul));
							}
							AnimationSS6PUMaskMul = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUMaskMulNA:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUMaskMulNA));
							}
							AnimationSS6PUMaskMulNA = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUMaskScr:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUMaskScr));
							}
							AnimationSS6PUMaskScr = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUMaskExc:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUMaskExc));
							}
							AnimationSS6PUMaskExc = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationSS6PUMaskInv:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationSS6PUMaskInv));
							}
							AnimationSS6PUMaskInv = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyEffectSS6PUThroughMix:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.EffectSS6PUThroughMix));
							}
							EffectSS6PUThroughMix = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyEffectSS6PUThroughAdd:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.EffectSS6PUThroughAdd));
							}
							EffectSS6PUThroughAdd = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyEffectSS6PUMaskMix:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.EffectSS6PUMaskMix));
							}
							EffectSS6PUMaskMix = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyEffectSS6PUMaskAdd:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.EffectSS6PUMaskAdd));
							}
							EffectSS6PUMaskAdd = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationUnityNativeMix:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationUnityNativeMix));
							}
							AnimationUnityNativeMix = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationUnityNativeAdd:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationUnityNativeAdd));
							}
							AnimationUnityNativeAdd = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationUnityNativeSub:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationUnityNativeSub));
							}
							AnimationUnityNativeSub = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationUnityNativeMul:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationUnityNativeMul));
							}
							AnimationUnityNativeMul = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationUnityNativeMulNA:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationUnityNativeMulNA));
							}
							AnimationUnityNativeMulNA = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationUnityNativeScr:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationUnityNativeScr));
							}
							AnimationUnityNativeScr = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationUnityNativeExc:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationUnityNativeExc));
							}
							AnimationUnityNativeExc = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationUnityNativeInv:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationUnityNativeInv));
							}
							AnimationUnityNativeInv = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationUnityNativeNonBatchMix:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationUnityNativeNonBatchMix));
							}
							AnimationUnityNativeNonBatchMix = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationUnityNativeNonBatchAdd:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationUnityNativeNonBatchAdd));
							}
							AnimationUnityNativeNonBatchAdd = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationUnityNativeNonBatchSub:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationUnityNativeNonBatchSub));
							}
							AnimationUnityNativeNonBatchSub = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationUnityNativeNonBatchMul:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationUnityNativeNonBatchMul));
							}
							AnimationUnityNativeNonBatchMul = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationUnityNativeNonBatchMulNA:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationUnityNativeNonBatchMulNA));
							}
							AnimationUnityNativeNonBatchMulNA = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationUnityNativeNonBatchScr:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationUnityNativeNonBatchScr));
							}
							AnimationUnityNativeNonBatchScr = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationUnityNativeNonBatchExc:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationUnityNativeNonBatchExc));
							}
							AnimationUnityNativeNonBatchExc = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeyAnimationUnityNativeNonBatchInv:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.AnimationUnityNativeNonBatchInv));
							}
							AnimationUnityNativeNonBatchInv = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeySkinnedMeshUnityNativeMix:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.SkinnedMeshUnityNativeMix));
							}
							SkinnedMeshUnityNativeMix = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeySkinnedMeshUnityNativeAdd:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.SkinnedMeshUnityNativeAdd));
							}
							SkinnedMeshUnityNativeAdd = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeySkinnedMeshUnityNativeSub:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.SkinnedMeshUnityNativeSub));
							}
							SkinnedMeshUnityNativeSub = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeySkinnedMeshUnityNativeMul:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.SkinnedMeshUnityNativeMul));
							}
							SkinnedMeshUnityNativeMul = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeySkinnedMeshUnityNativeMulNA:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.SkinnedMeshUnityNativeMulNA));
							}
							SkinnedMeshUnityNativeMulNA = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeySkinnedMeshUnityNativeScr:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.SkinnedMeshUnityNativeScr));
							}
							SkinnedMeshUnityNativeScr = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeySkinnedMeshUnityNativeExc:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.SkinnedMeshUnityNativeExc));
							}
							SkinnedMeshUnityNativeExc = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						case TextKeySkinnedMeshUnityNativeInv:
							if(true == string.IsNullOrEmpty(namePathMaterial))
							{
								namePathMaterial = PathGetForExport(PathGetMaterial(Default.SkinnedMeshUnityNativeInv));
							}
							SkinnedMeshUnityNativeInv = MaterialLoadPath(PathGet(namePathMaterial));
							return(true);

						default:
							break;
					}

					return(false);
				}

				private static Material MaterialLoadPath(string path)
				{
					Material material = AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material;
					return(material);
				}

				private static string PathGetMaterial(Material material)
				{
					if(null == material)
					{
						return("");
					}
					return(AssetDatabase.GetAssetPath(material));
				}

				private static string GUIDGetMaterial(Material material)
				{
					string guidMaterial = "";
					if(null != material)
					{
						string namePathMaterial = PathGetMaterial(material);
						if(true == string.IsNullOrEmpty(namePathMaterial))
						{
							return("");
						}
						guidMaterial = AssetDatabase.AssetPathToGUID(namePathMaterial);
					}
					return(guidMaterial);
				}

				private static Material MaterialGetGUID(string guid)
				{
					if(true == string.IsNullOrEmpty(guid))
					{
						return(null);
					}

					string namePathMaterial = AssetDatabase.GUIDToAssetPath(guid);
					return(MaterialLoadPath(namePathMaterial));
				}

				private static string PathGet(string namePath)
				{
					string name = namePath;
					if(false == name.StartsWith("/"))
					{
						name = "/" + name;
					}
					return(namePathRoot + name);
				}

				private static string PathGetForExport(string namePath)
				{
					return(namePath.Remove(0, namePathRoot.Length));
				}
				#endregion Functions

				/* ----------------------------------------------- Enums & Constants */
				#region Enums & Constants
				private const string KeyKindAnimation = "Animation";
				private const string KeyKindEffect = "Effect";
				private const string KeyKindSkinnedMesh = "SkinnedMesh";

				private const string KeyModeSS6PU = "SS6PU";
				private const string KeyModeUnityNative = "UnityNative";

				private const string KeyMaskingMask = "Mask";
				private const string KeyMaskingThrough = "Through";

				private const string KeyNonBatch = "NonBatch";

				private const string KeyOperationStencilPreDraw = "StencilPreDraw";
				private const string KeyOperationStencilDraw = "StencilDraw";
				private const string KeyOperationMix = "Mix";
				private const string KeyOperationAdd = "Add";
				private const string KeyOperationSub = "Sub";
				private const string KeyOperationMul = "Mul";
				private const string KeyOperationMulNA = "MulNA";
				private const string KeyOperationScr = "Scr";
				private const string KeyOperationExc = "Exc";
				private const string KeyOperationInv = "Inv";

//				/* Obsolete */	private const string KeyAnimationSS6PUStencilPreDraw = KeyKindAnimation + KeyModeSS6PU + KeyOperationStencilPreDraw;
//				/* Obsolete */	private const string KeyAnimationSS6PUStencilDraw = KeyKindAnimation + KeyModeSS6PU + KeyOperationStencilDraw;
				private const string KeyAnimationSS6PUThroughStencilPreDraw = KeyKindAnimation + KeyModeSS6PU + KeyMaskingThrough + KeyOperationStencilPreDraw;
				private const string KeyAnimationSS6PUThroughStencilDraw = KeyKindAnimation + KeyModeSS6PU + KeyMaskingThrough + KeyOperationStencilDraw;
				private const string KeyAnimationSS6PUThroughMix = KeyKindAnimation + KeyModeSS6PU + KeyMaskingThrough + KeyOperationMix;
				private const string KeyAnimationSS6PUThroughAdd = KeyKindAnimation + KeyModeSS6PU + KeyMaskingThrough + KeyOperationAdd;
				private const string KeyAnimationSS6PUThroughSub = KeyKindAnimation + KeyModeSS6PU + KeyMaskingThrough + KeyOperationSub;
				private const string KeyAnimationSS6PUThroughMul = KeyKindAnimation + KeyModeSS6PU + KeyMaskingThrough + KeyOperationMul;
				private const string KeyAnimationSS6PUThroughMulNA = KeyKindAnimation + KeyModeSS6PU + KeyMaskingThrough + KeyOperationMulNA;
				private const string KeyAnimationSS6PUThroughScr = KeyKindAnimation + KeyModeSS6PU + KeyMaskingThrough + KeyOperationScr;
				private const string KeyAnimationSS6PUThroughExc = KeyKindAnimation + KeyModeSS6PU + KeyMaskingThrough + KeyOperationExc;
				private const string KeyAnimationSS6PUThroughInv = KeyKindAnimation + KeyModeSS6PU + KeyMaskingThrough + KeyOperationInv;
				private const string KeyAnimationSS6PUMaskStencilPreDraw = KeyKindAnimation + KeyModeSS6PU + KeyMaskingMask + KeyOperationStencilPreDraw;
				private const string KeyAnimationSS6PUMaskStencilDraw = KeyKindAnimation + KeyModeSS6PU + KeyMaskingMask + KeyOperationStencilDraw;
				private const string KeyAnimationSS6PUMaskMix = KeyKindAnimation + KeyModeSS6PU + KeyMaskingMask + KeyOperationMix;
				private const string KeyAnimationSS6PUMaskAdd = KeyKindAnimation + KeyModeSS6PU + KeyMaskingMask + KeyOperationAdd;
				private const string KeyAnimationSS6PUMaskSub = KeyKindAnimation + KeyModeSS6PU + KeyMaskingMask + KeyOperationSub;
				private const string KeyAnimationSS6PUMaskMul = KeyKindAnimation + KeyModeSS6PU + KeyMaskingMask + KeyOperationMul;
				private const string KeyAnimationSS6PUMaskMulNA = KeyKindAnimation + KeyModeSS6PU + KeyMaskingMask + KeyOperationMulNA;
				private const string KeyAnimationSS6PUMaskScr = KeyKindAnimation + KeyModeSS6PU + KeyMaskingMask + KeyOperationScr;
				private const string KeyAnimationSS6PUMaskExc = KeyKindAnimation + KeyModeSS6PU + KeyMaskingMask + KeyOperationExc;
				private const string KeyAnimationSS6PUMaskInv = KeyKindAnimation + KeyModeSS6PU + KeyMaskingMask + KeyOperationInv;
				private const string KeyEffectSS6PUThroughMix = KeyKindEffect + KeyModeSS6PU + KeyMaskingThrough + KeyOperationMix;
				private const string KeyEffectSS6PUThroughAdd = KeyKindEffect + KeyModeSS6PU + KeyMaskingThrough + KeyOperationAdd;
				private const string KeyEffectSS6PUMaskMix = KeyKindEffect + KeyModeSS6PU + KeyMaskingMask + KeyOperationMix;
				private const string KeyEffectSS6PUMaskAdd = KeyKindEffect + KeyModeSS6PU + KeyMaskingMask + KeyOperationAdd;

				private const string KeyAnimationUnityNativeMix = KeyKindAnimation + KeyModeUnityNative + KeyOperationMix;
				private const string KeyAnimationUnityNativeAdd = KeyKindAnimation + KeyModeUnityNative + KeyOperationAdd;
				private const string KeyAnimationUnityNativeSub = KeyKindAnimation + KeyModeUnityNative + KeyOperationSub;
				private const string KeyAnimationUnityNativeMul = KeyKindAnimation + KeyModeUnityNative + KeyOperationMul;
				private const string KeyAnimationUnityNativeMulNA = KeyKindAnimation + KeyModeUnityNative + KeyOperationMulNA;
				private const string KeyAnimationUnityNativeScr = KeyKindAnimation + KeyModeUnityNative + KeyOperationScr;
				private const string KeyAnimationUnityNativeExc = KeyKindAnimation + KeyModeUnityNative + KeyOperationExc;
				private const string KeyAnimationUnityNativeInv = KeyKindAnimation + KeyModeUnityNative + KeyOperationInv;
				private const string KeyAnimationUnityNativeNonBatchMix = KeyKindAnimation + KeyModeUnityNative + KeyNonBatch + KeyOperationMix;
				private const string KeyAnimationUnityNativeNonBatchAdd = KeyKindAnimation + KeyModeUnityNative + KeyNonBatch + KeyOperationAdd;
				private const string KeyAnimationUnityNativeNonBatchSub = KeyKindAnimation + KeyModeUnityNative + KeyNonBatch + KeyOperationSub;
				private const string KeyAnimationUnityNativeNonBatchMul = KeyKindAnimation + KeyModeUnityNative + KeyNonBatch + KeyOperationMul;
				private const string KeyAnimationUnityNativeNonBatchMulNA = KeyKindAnimation + KeyModeUnityNative + KeyNonBatch + KeyOperationMulNA;
				private const string KeyAnimationUnityNativeNonBatchScr = KeyKindAnimation + KeyModeUnityNative + KeyNonBatch + KeyOperationScr;
				private const string KeyAnimationUnityNativeNonBatchExc = KeyKindAnimation + KeyModeUnityNative + KeyNonBatch + KeyOperationExc;
				private const string KeyAnimationUnityNativeNonBatchInv = KeyKindAnimation + KeyModeUnityNative + KeyNonBatch + KeyOperationInv;
				private const string KeySkinnedMeshUnityNativeMix = KeyKindSkinnedMesh + KeyModeUnityNative + KeyOperationMix;
				private const string KeySkinnedMeshUnityNativeAdd = KeyKindSkinnedMesh + KeyModeUnityNative + KeyOperationAdd;
				private const string KeySkinnedMeshUnityNativeSub = KeyKindSkinnedMesh + KeyModeUnityNative + KeyOperationSub;
				private const string KeySkinnedMeshUnityNativeMul = KeyKindSkinnedMesh + KeyModeUnityNative + KeyOperationMul;
				private const string KeySkinnedMeshUnityNativeMulNA = KeyKindSkinnedMesh + KeyModeUnityNative + KeyOperationMulNA;
				private const string KeySkinnedMeshUnityNativeScr = KeyKindSkinnedMesh + KeyModeUnityNative + KeyOperationScr;
				private const string KeySkinnedMeshUnityNativeExc = KeyKindSkinnedMesh + KeyModeUnityNative + KeyOperationExc;
				private const string KeySkinnedMeshUnityNativeInv = KeyKindSkinnedMesh + KeyModeUnityNative + KeyOperationInv;

				private const string TextKeyPrefix = "PresetMaterial_";

//				/* Obsolete */	private const string TextKeyAnimationSS6PUStencilPreDraw = TextKeyPrefix + KeyAnimationSS6PUStencilPreDraw;
//				/* Obsolete */	private const string TextKeyAnimationSS6PUStencilDraw = TextKeyPrefix + KeyAnimationSS6PUStencilDraw;
				private const string TextKeyAnimationSS6PUThroughStencilPreDraw = TextKeyPrefix + KeyAnimationSS6PUThroughStencilPreDraw;
				private const string TextKeyAnimationSS6PUThroughStencilDraw = TextKeyPrefix + KeyAnimationSS6PUThroughStencilDraw;
				private const string TextKeyAnimationSS6PUThroughMix = TextKeyPrefix + KeyAnimationSS6PUThroughMix;
				private const string TextKeyAnimationSS6PUThroughAdd = TextKeyPrefix + KeyAnimationSS6PUThroughAdd;
				private const string TextKeyAnimationSS6PUThroughSub = TextKeyPrefix + KeyAnimationSS6PUThroughSub;
				private const string TextKeyAnimationSS6PUThroughMul = TextKeyPrefix + KeyAnimationSS6PUThroughMul;
				private const string TextKeyAnimationSS6PUThroughMulNA = TextKeyPrefix + KeyAnimationSS6PUThroughMulNA;
				private const string TextKeyAnimationSS6PUThroughScr = TextKeyPrefix + KeyAnimationSS6PUThroughScr;
				private const string TextKeyAnimationSS6PUThroughExc = TextKeyPrefix + KeyAnimationSS6PUThroughExc;
				private const string TextKeyAnimationSS6PUThroughInv = TextKeyPrefix + KeyAnimationSS6PUThroughInv;
				private const string TextKeyAnimationSS6PUMaskStencilPreDraw = TextKeyPrefix + KeyAnimationSS6PUMaskStencilPreDraw;
				private const string TextKeyAnimationSS6PUMaskStencilDraw = TextKeyPrefix + KeyAnimationSS6PUMaskStencilDraw;
				private const string TextKeyAnimationSS6PUMaskMix = TextKeyPrefix + KeyAnimationSS6PUMaskMix;
				private const string TextKeyAnimationSS6PUMaskAdd = TextKeyPrefix + KeyAnimationSS6PUMaskAdd;
				private const string TextKeyAnimationSS6PUMaskSub = TextKeyPrefix + KeyAnimationSS6PUMaskSub;
				private const string TextKeyAnimationSS6PUMaskMul = TextKeyPrefix + KeyAnimationSS6PUMaskMul;
				private const string TextKeyAnimationSS6PUMaskMulNA = TextKeyPrefix + KeyAnimationSS6PUMaskMulNA;
				private const string TextKeyAnimationSS6PUMaskScr = TextKeyPrefix + KeyAnimationSS6PUMaskScr;
				private const string TextKeyAnimationSS6PUMaskExc = TextKeyPrefix + KeyAnimationSS6PUMaskExc;
				private const string TextKeyAnimationSS6PUMaskInv = TextKeyPrefix + KeyAnimationSS6PUMaskInv;

				private const string TextKeyEffectSS6PUThroughMix = TextKeyPrefix + KeyEffectSS6PUThroughMix;
				private const string TextKeyEffectSS6PUThroughAdd = TextKeyPrefix + KeyEffectSS6PUThroughAdd;
				private const string TextKeyEffectSS6PUMaskMix = TextKeyPrefix + KeyEffectSS6PUMaskMix;
				private const string TextKeyEffectSS6PUMaskAdd = TextKeyPrefix + KeyEffectSS6PUMaskAdd;

				private const string TextKeyAnimationUnityNativeMix = TextKeyPrefix + KeyAnimationUnityNativeMix;
				private const string TextKeyAnimationUnityNativeAdd = TextKeyPrefix + KeyAnimationUnityNativeAdd;
				private const string TextKeyAnimationUnityNativeSub = TextKeyPrefix + KeyAnimationUnityNativeSub;
				private const string TextKeyAnimationUnityNativeMul = TextKeyPrefix + KeyAnimationUnityNativeMul;
				private const string TextKeyAnimationUnityNativeMulNA = TextKeyPrefix + KeyAnimationUnityNativeMulNA;
				private const string TextKeyAnimationUnityNativeScr = TextKeyPrefix + KeyAnimationUnityNativeScr;
				private const string TextKeyAnimationUnityNativeExc = TextKeyPrefix + KeyAnimationUnityNativeExc;
				private const string TextKeyAnimationUnityNativeInv = TextKeyPrefix + KeyAnimationUnityNativeInv;
				private const string TextKeyAnimationUnityNativeNonBatchMix = TextKeyPrefix + KeyAnimationUnityNativeNonBatchMix;
				private const string TextKeyAnimationUnityNativeNonBatchAdd = TextKeyPrefix + KeyAnimationUnityNativeNonBatchAdd;
				private const string TextKeyAnimationUnityNativeNonBatchSub = TextKeyPrefix + KeyAnimationUnityNativeNonBatchSub;
				private const string TextKeyAnimationUnityNativeNonBatchMul = TextKeyPrefix + KeyAnimationUnityNativeNonBatchMul;
				private const string TextKeyAnimationUnityNativeNonBatchMulNA = TextKeyPrefix + KeyAnimationUnityNativeNonBatchMulNA;
				private const string TextKeyAnimationUnityNativeNonBatchScr = TextKeyPrefix + KeyAnimationUnityNativeNonBatchScr;
				private const string TextKeyAnimationUnityNativeNonBatchExc = TextKeyPrefix + KeyAnimationUnityNativeNonBatchExc;
				private const string TextKeyAnimationUnityNativeNonBatchInv = TextKeyPrefix + KeyAnimationUnityNativeNonBatchInv;
				private const string TextKeySkinnedMeshUnityNativeMix = TextKeyPrefix + KeySkinnedMeshUnityNativeMix;
				private const string TextKeySkinnedMeshUnityNativeAdd = TextKeyPrefix + KeySkinnedMeshUnityNativeAdd;
				private const string TextKeySkinnedMeshUnityNativeSub = TextKeyPrefix + KeySkinnedMeshUnityNativeSub;
				private const string TextKeySkinnedMeshUnityNativeMul = TextKeyPrefix + KeySkinnedMeshUnityNativeMul;
				private const string TextKeySkinnedMeshUnityNativeMulNA = TextKeyPrefix + KeySkinnedMeshUnityNativeMulNA;
				private const string TextKeySkinnedMeshUnityNativeScr = TextKeyPrefix + KeySkinnedMeshUnityNativeScr;
				private const string TextKeySkinnedMeshUnityNativeExc = TextKeyPrefix + KeySkinnedMeshUnityNativeExc;
				private const string TextKeySkinnedMeshUnityNativeInv = TextKeyPrefix + KeySkinnedMeshUnityNativeInv;

				private const string PrefsKeyPrefix = LibraryEditor_SpriteStudio6.Import.Setting.PrefsKeyPrefix + TextKeyPrefix;

//				/* Obsolete */	private const string PrefsKeyAnimationSS6PUStencilPreDraw = PrefsKeyPrefix + KeyAnimationSS6PUStencilPreDraw;
//				/* Obsolete */	private const string PrefsKeyAnimationSS6PUStencilDraw = PrefsKeyPrefix + KeyAnimationSS6PUStencilDraw;
				private const string PrefsKeyAnimationSS6PUThroughStencilPreDraw = PrefsKeyPrefix + KeyAnimationSS6PUThroughStencilPreDraw;
				private const string PrefsKeyAnimationSS6PUThroughStencilDraw = PrefsKeyPrefix + KeyAnimationSS6PUThroughStencilDraw;
				private const string PrefsKeyAnimationSS6PUThroughMix = PrefsKeyPrefix + KeyAnimationSS6PUThroughMix;
				private const string PrefsKeyAnimationSS6PUThroughAdd = PrefsKeyPrefix + KeyAnimationSS6PUThroughAdd;
				private const string PrefsKeyAnimationSS6PUThroughSub = PrefsKeyPrefix + KeyAnimationSS6PUThroughSub;
				private const string PrefsKeyAnimationSS6PUThroughMul = PrefsKeyPrefix + KeyAnimationSS6PUThroughMul;
				private const string PrefsKeyAnimationSS6PUThroughMulNA = PrefsKeyPrefix + KeyAnimationSS6PUThroughMulNA;
				private const string PrefsKeyAnimationSS6PUThroughScr = PrefsKeyPrefix + KeyAnimationSS6PUThroughScr;
				private const string PrefsKeyAnimationSS6PUThroughExc = PrefsKeyPrefix + KeyAnimationSS6PUThroughExc;
				private const string PrefsKeyAnimationSS6PUThroughInv = PrefsKeyPrefix + KeyAnimationSS6PUThroughInv;
				private const string PrefsKeyAnimationSS6PUMaskStencilPreDraw = PrefsKeyPrefix + KeyAnimationSS6PUMaskStencilPreDraw;
				private const string PrefsKeyAnimationSS6PUMaskStencilDraw = PrefsKeyPrefix + KeyAnimationSS6PUMaskStencilDraw;
				private const string PrefsKeyAnimationSS6PUMaskMix = PrefsKeyPrefix + KeyAnimationSS6PUMaskMix;
				private const string PrefsKeyAnimationSS6PUMaskAdd = PrefsKeyPrefix + KeyAnimationSS6PUMaskAdd;
				private const string PrefsKeyAnimationSS6PUMaskSub = PrefsKeyPrefix + KeyAnimationSS6PUMaskSub;
				private const string PrefsKeyAnimationSS6PUMaskMul = PrefsKeyPrefix + KeyAnimationSS6PUMaskMul;
				private const string PrefsKeyAnimationSS6PUMaskMulNA = PrefsKeyPrefix + KeyAnimationSS6PUMaskMulNA;
				private const string PrefsKeyAnimationSS6PUMaskScr = PrefsKeyPrefix + KeyAnimationSS6PUMaskScr;
				private const string PrefsKeyAnimationSS6PUMaskExc = PrefsKeyPrefix + KeyAnimationSS6PUMaskExc;
				private const string PrefsKeyAnimationSS6PUMaskInv = PrefsKeyPrefix + KeyAnimationSS6PUMaskInv;

				private const string PrefsKeyEffectSS6PUThroughMix = PrefsKeyPrefix + KeyEffectSS6PUThroughMix;
				private const string PrefsKeyEffectSS6PUThroughAdd = PrefsKeyPrefix + KeyEffectSS6PUThroughAdd;
				private const string PrefsKeyEffectSS6PUMaskMix = PrefsKeyPrefix + KeyEffectSS6PUMaskMix;
				private const string PrefsKeyEffectSS6PUMaskAdd = PrefsKeyPrefix + KeyEffectSS6PUMaskAdd;

				private const string PrefsKeyAnimationUnityNativeMix = PrefsKeyPrefix + KeyAnimationUnityNativeMix;
				private const string PrefsKeyAnimationUnityNativeAdd = PrefsKeyPrefix + KeyAnimationUnityNativeAdd;
				private const string PrefsKeyAnimationUnityNativeSub = PrefsKeyPrefix + KeyAnimationUnityNativeSub;
				private const string PrefsKeyAnimationUnityNativeMul = PrefsKeyPrefix + KeyAnimationUnityNativeMul;
				private const string PrefsKeyAnimationUnityNativeMulNA = PrefsKeyPrefix + KeyAnimationUnityNativeMulNA;
				private const string PrefsKeyAnimationUnityNativeScr = PrefsKeyPrefix + KeyAnimationUnityNativeScr;
				private const string PrefsKeyAnimationUnityNativeExc = PrefsKeyPrefix + KeyAnimationUnityNativeExc;
				private const string PrefsKeyAnimationUnityNativeInv = PrefsKeyPrefix + KeyAnimationUnityNativeInv;
				private const string PrefsKeyAnimationUnityNativeNonBatchMix = PrefsKeyPrefix + KeyAnimationUnityNativeNonBatchMix;
				private const string PrefsKeyAnimationUnityNativeNonBatchAdd = PrefsKeyPrefix + KeyAnimationUnityNativeNonBatchAdd;
				private const string PrefsKeyAnimationUnityNativeNonBatchSub = PrefsKeyPrefix + KeyAnimationUnityNativeNonBatchSub;
				private const string PrefsKeyAnimationUnityNativeNonBatchMul = PrefsKeyPrefix + KeyAnimationUnityNativeNonBatchMul;
				private const string PrefsKeyAnimationUnityNativeNonBatchMulNA = PrefsKeyPrefix + KeyAnimationUnityNativeNonBatchMulNA;
				private const string PrefsKeyAnimationUnityNativeNonBatchScr = PrefsKeyPrefix + KeyAnimationUnityNativeNonBatchScr;
				private const string PrefsKeyAnimationUnityNativeNonBatchExc = PrefsKeyPrefix + KeyAnimationUnityNativeNonBatchExc;
				private const string PrefsKeyAnimationUnityNativeNonBatchInv = PrefsKeyPrefix + KeyAnimationUnityNativeNonBatchInv;
				private const string PrefsKeySkinnedMeshUnityNativeMix = PrefsKeyPrefix + KeySkinnedMeshUnityNativeMix;
				private const string PrefsKeySkinnedMeshUnityNativeAdd = PrefsKeyPrefix + KeySkinnedMeshUnityNativeAdd;
				private const string PrefsKeySkinnedMeshUnityNativeSub = PrefsKeyPrefix + KeySkinnedMeshUnityNativeSub;
				private const string PrefsKeySkinnedMeshUnityNativeMul = PrefsKeyPrefix + KeySkinnedMeshUnityNativeMul;
				private const string PrefsKeySkinnedMeshUnityNativeMulNA = PrefsKeyPrefix + KeySkinnedMeshUnityNativeMulNA;
				private const string PrefsKeySkinnedMeshUnityNativeScr = PrefsKeyPrefix + KeySkinnedMeshUnityNativeScr;
				private const string PrefsKeySkinnedMeshUnityNativeExc = PrefsKeyPrefix + KeySkinnedMeshUnityNativeExc;
				private const string PrefsKeySkinnedMeshUnityNativeInv = PrefsKeyPrefix + KeySkinnedMeshUnityNativeInv;

				private const string namePathRoot = "Assets";

				private readonly static GroupPresetMaterial Default = new GroupPresetMaterial(
					"SpriteStudio6/Material/SS6PU/Stencil_SpriteStudio6_Thru_PreDraw.mat",			/* AnimationSS6PUThroughStencilPreDraw */
					"SpriteStudio6/Material/SS6PU/Stencil_SpriteStudio6_Thru_Draw.mat",				/* AnimationSS6PUStencilDraw */
					"SpriteStudio6/Material/SS6PU/Sprite_SpriteStudio6_Thru_MIX.mat",				/* AnimationSS6PUThroughMix */
					"SpriteStudio6/Material/SS6PU/Sprite_SpriteStudio6_Thru_ADD.mat",				/* AnimationSS6PUThroughAdd */
					"SpriteStudio6/Material/SS6PU/Sprite_SpriteStudio6_Thru_SUB.mat",				/* AnimationSS6PUThroughSub */
					"SpriteStudio6/Material/SS6PU/Sprite_SpriteStudio6_Thru_MUL.mat",				/* AnimationSS6PUThroughMul */
					"SpriteStudio6/Material/SS6PU/Sprite_SpriteStudio6_Thru_MUL_NA.mat",			/* AnimationSS6PUThroughMulNA */
					"SpriteStudio6/Material/SS6PU/Sprite_SpriteStudio6_Thru_SCR.mat",				/* AnimationSS6PUThroughScr */
					"SpriteStudio6/Material/SS6PU/Sprite_SpriteStudio6_Thru_EXC.mat",				/* AnimationSS6PUThroughExc */
					"SpriteStudio6/Material/SS6PU/Sprite_SpriteStudio6_Thru_INV.mat",				/* AnimationSS6PUThroughInv */
					"SpriteStudio6/Material/SS6PU/Stencil_SpriteStudio6_Mask_PreDraw.mat",			/* AnimationSS6PUMaskStencilPreDraw */
					"SpriteStudio6/Material/SS6PU/Stencil_SpriteStudio6_Mask_Draw.mat",				/* AnimationSS6PUMaskStencilDraw */
					"SpriteStudio6/Material/SS6PU/Sprite_SpriteStudio6_Mask_MIX.mat",				/* AnimationSS6PUMaskMix */
					"SpriteStudio6/Material/SS6PU/Sprite_SpriteStudio6_Mask_ADD.mat",				/* AnimationSS6PUMaskAdd */
					"SpriteStudio6/Material/SS6PU/Sprite_SpriteStudio6_Mask_SUB.mat",				/* AnimationSS6PUMaskSub */
					"SpriteStudio6/Material/SS6PU/Sprite_SpriteStudio6_Mask_MUL.mat",				/* AnimationSS6PUMaskMul */
					"SpriteStudio6/Material/SS6PU/Sprite_SpriteStudio6_Mask_MUL_NA.mat",			/* AnimationSS6PUMaskMulNA */
					"SpriteStudio6/Material/SS6PU/Sprite_SpriteStudio6_Mask_SCR.mat",				/* AnimationSS6PUMaskScr */
					"SpriteStudio6/Material/SS6PU/Sprite_SpriteStudio6_Mask_EXC.mat",				/* AnimationSS6PUMaskExc */
					"SpriteStudio6/Material/SS6PU/Sprite_SpriteStudio6_Mask_INV.mat",				/* AnimationSS6PUMaskInv */

					"SpriteStudio6/Material/SS6PU/Effect_SpriteStudio6_Thru_MIX.mat",				/* EffectSS6PUThroughMix */
					"SpriteStudio6/Material/SS6PU/Effect_SpriteStudio6_Thru_ADD.mat",				/* EffectSS6PUThroughAdd */
					"SpriteStudio6/Material/SS6PU/Effect_SpriteStudio6_Mask_MIX.mat",				/* EffectSS6PUMaskMix */
					"SpriteStudio6/Material/SS6PU/Effect_SpriteStudio6_Mask_ADD.mat",				/* EffectSS6PUMaskAdd */

					"SpriteStudio6/Material/UnityNative/Sprite_UnityNative_MIX.mat",				/* AnimationUnityNativeMix */
					"SpriteStudio6/Material/UnityNative/Sprite_UnityNative_ADD.mat",				/* AnimationUnityNativeAdd */
					"SpriteStudio6/Material/UnityNative/Sprite_UnityNative_SUB.mat",				/* AnimationUnityNativeSub */
					"SpriteStudio6/Material/UnityNative/Sprite_UnityNative_MUL.mat",				/* AnimationUnityNativeMul */
					"SpriteStudio6/Material/UnityNative/Sprite_UnityNative_MUL_NA.mat",				/* AnimationUnityNativeMulNA */
					"SpriteStudio6/Material/UnityNative/Sprite_UnityNative_SCR.mat",				/* AnimationUnityNativeScr */
					"SpriteStudio6/Material/UnityNative/Sprite_UnityNative_EXC.mat",				/* AnimationUnityNativeExc */
					"SpriteStudio6/Material/UnityNative/Sprite_UnityNative_INV.mat",				/* AnimationUnityNativeInv */
					"SpriteStudio6/Material/UnityNative/Sprite_UnityNative_NonBatch_MIX.mat",		/* AnimationUnityNativeNonBatchMix */
					"SpriteStudio6/Material/UnityNative/Sprite_UnityNative_NonBatch_ADD.mat",		/* AnimationUnityNativeNonBatchAdd */
					"SpriteStudio6/Material/UnityNative/Sprite_UnityNative_NonBatch_SUB.mat",		/* AnimationUnityNativeNonBatchSub */
					"SpriteStudio6/Material/UnityNative/Sprite_UnityNative_NonBatch_MUL.mat",		/* AnimationUnityNativeNonBatchMul */
					"SpriteStudio6/Material/UnityNative/Sprite_UnityNative_NonBatch_MUL_NA.mat",	/* AnimationUnityNativeNonBatchMulNA */
					"SpriteStudio6/Material/UnityNative/Sprite_UnityNative_NonBatch_SCR.mat",		/* AnimationUnityNativeNonBatchScr */
					"SpriteStudio6/Material/UnityNative/Sprite_UnityNative_NonBatch_EXC.mat",		/* AnimationUnityNativeNonBatchExc */
					"SpriteStudio6/Material/UnityNative/Sprite_UnityNative_NonBatch_INV.mat",		/* AnimationUnityNativeNonBatchInv */

					"SpriteStudio6/Material/UnityNative/SkinnedMesh_UnityNative_MIX.mat",			/* SkinnedMeshUnityNativeMix */
					"SpriteStudio6/Material/UnityNative/SkinnedMesh_UnityNative_ADD.mat",			/* SkinnedMeshUnityNativeAdd */
					"SpriteStudio6/Material/UnityNative/SkinnedMesh_UnityNative_SUB.mat",			/* SkinnedMeshUnityNativeSub */
					"SpriteStudio6/Material/UnityNative/SkinnedMesh_UnityNative_MUL.mat",			/* SkinnedMeshUnityNativeMul */
					"SpriteStudio6/Material/UnityNative/SkinnedMesh_UnityNative_MUL_NA.mat",		/* SkinnedMeshUnityNativeMulNA */
					"SpriteStudio6/Material/UnityNative/SkinnedMesh_UnityNative_SCR.mat",			/* SkinnedMeshUnityNativeScr */
					"SpriteStudio6/Material/UnityNative/SkinnedMesh_UnityNative_EXC.mat",			/* SkinnedMeshUnityNativeExc */
					"SpriteStudio6/Material/UnityNative/SkinnedMesh_UnityNative_INV.mat"			/* SkinnedMeshUnityNativeInv */
				);
				#endregion Enums & Constants
			}
#endif
			#endregion Classes, Structs & Interfaces
		}
		#endregion Classes, Structs & Interfaces
	}
}
