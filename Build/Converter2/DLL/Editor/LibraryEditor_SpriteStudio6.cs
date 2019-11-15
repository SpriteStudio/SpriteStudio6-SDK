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
	/* ----------------------------------------------- Classes, Structs & Interfaces */
	#region Classes, Structs & Interfaces
	public static partial class Import
	{
		/* ----------------------------------------------- Functions */
		#region Functions
		public static bool Exec(	ref Setting setting,
									string nameInputFullPathSSPJ,
									string nameOutputAssetFolderBase,
									bool flagDisplayProgressBar = true
								)
		{
			const string messageLogPrefix = "Main";
			SSPJ.Information informationSSPJ = null;
			int countProgressNow = 0;
			int countProgressMax = 0;

            string outputfile = nameOutputAssetFolderBase;

            /* Select Project(SSPJ) */
            string nameDirectory = "";
			string nameFileBody = "";
			string nameFileExtension = "";
			nameInputFullPathSSPJ = LibraryEditor_SpriteStudio6.Utility.File.PathNormalize(nameInputFullPathSSPJ);
			LibraryEditor_SpriteStudio6.Utility.Log.Message("Importing Start [" + nameInputFullPathSSPJ + "]", true, false);	/* External-File only, no indent */

			if(false == System.IO.File.Exists(nameInputFullPathSSPJ))
			{	/* Not Found */
				LogError(messageLogPrefix, "File Not Found [" + nameInputFullPathSSPJ +"]");
				goto Exec_ErrorEnd;
			}
			LibraryEditor_SpriteStudio6.Utility.File.PathSplit(out nameDirectory, out nameFileBody, out nameFileExtension, nameInputFullPathSSPJ);

			/* Read Project (SSPJ) */
			/* MEMO: "nameDirectory" must be Absolute-Path. */
			informationSSPJ = SSPJ.Parse(ref setting, nameDirectory, nameFileBody, nameFileExtension);
			if(null == informationSSPJ)
			{
				goto Exec_ErrorEnd;
			}

			/* Get ConvertFiles-Count & ConvertProgress-Count */
			int countSSCE = informationSSPJ.TableNameSSCE.Length;
			int countSSAE = informationSSPJ.TableNameSSAE.Length;
			int countSSEE = informationSSPJ.TableNameSSEE.Length;
			int countTexture = countSSCE;
			countProgressMax += (countSSCE + countSSAE + countSSEE + 1);	/* Parse *//* +1 = Create Texture-Information */
			switch(setting.Mode)
			{
				case Setting.KindMode.SS6PU:
					{
						countProgressMax += (countSSCE + countSSAE + countSSEE);	/* Convert */
						countProgressMax += 1;	/* Create-Asset(Materials) */
						countProgressMax += (countTexture + 1 + countSSAE + countSSEE);	/* Create-Asset (Data) */
						countProgressMax += (countSSAE + countSSEE);	/* Create-Asset (Prefab) */

						if(true == setting.PreCalcualation.FlagTrimTransparentPixelsCell)
						{
							countProgressMax += countSSCE;	/* Convert-CellMap Pass 2 (PreCalculate Trim-TransparentPixels) */
						}
					}
					break;

				case Setting.KindMode.UNITY_NATIVE:
					{
						countProgressMax += countSSCE;	/* Convert (CellMap) */
						countProgressMax += countTexture;	/* Create-Asset (Texture) */
						countProgressMax += countSSAE;	/* Convert & Create-Asset (Animation) */

						if(true == setting.PreCalcualation.FlagTrimTransparentPixelsCell)
						{
							countProgressMax += countSSCE;	/* Convert-CellMap Pass 2 (PreCalculate Trim-TransparentPixels) */
						}
					}
					break;

				case Setting.KindMode.BATCH_IMPORTER:
				default:
					return(false);
			}

			/* Read CellMap(SSCE) & Collect Texture-FileNames */
			for(int i=0; i<countSSCE; i++)
			{
				ProgressBarUpdate(	"Reading SSCEs (" + (i + 1).ToString() + "/" + countSSCE.ToString() + ")",
									flagDisplayProgressBar, ref countProgressNow, countProgressMax
								);

				informationSSPJ.TableInformationSSCE[i] = SSCE.Parse(ref setting, informationSSPJ.TableNameSSCE[i], informationSSPJ);
				if(null == informationSSPJ.TableInformationSSCE[i])
				{
					goto Exec_ErrorEnd;
				}
			}

			/* Read Animation (SSAE) */
			for(int i=0; i<countSSAE; i++)
			{
				ProgressBarUpdate(	"Reading SSAEs (" + (i + 1).ToString() + "/" + countSSAE.ToString() + ")",
									flagDisplayProgressBar, ref countProgressNow, countProgressMax
								);

				informationSSPJ.TableInformationSSAE[i] = SSAE.Parse(ref setting, informationSSPJ.TableNameSSAE[i], informationSSPJ);
				if(null == informationSSPJ.TableInformationSSAE[i])
				{
					goto Exec_ErrorEnd;
				}
			}

			/* Read Effect (SSEE) */
			for(int i=0; i<countSSEE; i++)
			{
				ProgressBarUpdate(	"Reading SSEEs (" + (i + 1).ToString() + "/" + countSSEE.ToString() + ")",
									flagDisplayProgressBar, ref countProgressNow, countProgressMax
								);

				informationSSPJ.TableInformationSSEE[i] = SSEE.Parse(ref setting, informationSSPJ.TableNameSSEE[i], informationSSPJ);
				if(null == informationSSPJ.TableInformationSSEE[i])
				{
					goto Exec_ErrorEnd;
				}
			}

			/* Create Texture-Information */
			ProgressBarUpdate(	"Create Texture Information",
								flagDisplayProgressBar, ref countProgressNow, countProgressMax
							);
			if(false == informationSSPJ.InformationCreateTexture(ref setting))
			{
				goto Exec_ErrorEnd;
			}

			/* Delete Temporary (FileName Buffer) */
			informationSSPJ.TableNameSSCE = null;
			informationSSPJ.TableNameSSAE = null;
			informationSSPJ.TableNameSSEE = null;

#if false
            /* Create Destination Base-Folder */
            nameOutputAssetFolderBase = LibraryEditor_SpriteStudio6.Utility.File.AssetFolderCreate(LibraryEditor_SpriteStudio6.Utility.File.PathNormalize(nameOutputAssetFolderBase));
			if(false == nameOutputAssetFolderBase.EndsWith("/"))
			{
				nameOutputAssetFolderBase += "/";
			}
			if(true == setting.Basic.FlagCreateProjectFolder)
			{
				nameOutputAssetFolderBase += informationSSPJ.NameFileBody + "/";
			}
#endif
			/* Get Convert-Order SSAE */
			informationSSPJ.QueueConvertSSAE = informationSSPJ.QueueGetConvertSSAE(ref setting);
			if(null == informationSSPJ.QueueConvertSSAE)
			{
				goto Exec_ErrorEnd;
			}

			/* Garbage-Collection */
			LibraryEditor_SpriteStudio6.Utility.Miscellaneous.GarbageCollect();

			/* Convert & Create Assets */
			switch(setting.Mode)
			{
				case Setting.KindMode.SS6PU:
					if(false == ExecSS6PU(	ref setting,
											ref countProgressNow,
											countProgressMax,
											flagDisplayProgressBar,
											informationSSPJ,
											nameOutputAssetFolderBase
										)
						)
					{
						goto Exec_ErrorEnd;
					}
					break;

				case Setting.KindMode.UNITY_NATIVE:
					if(false == ExecUnityNative(	ref setting,
													ref countProgressNow,
													countProgressMax,
													flagDisplayProgressBar,
													informationSSPJ,
													nameOutputAssetFolderBase
												)
						)
					{
						goto Exec_ErrorEnd;
					}
					break;

				default:
					LogError(messageLogPrefix, "Invalid Convert-Mode at [" + nameInputFullPathSSPJ + "]");
					goto Exec_ErrorEnd;
			}

			countProgressNow = -1;
			ProgressBarUpdate("", flagDisplayProgressBar, ref countProgressNow, -1);

			LibraryEditor_SpriteStudio6.Utility.Log.Message("Success", true, false);	/* External-File only */

			/* Garbage-Collection */
			LibraryEditor_SpriteStudio6.Utility.Miscellaneous.GarbageCollect();

			return(true);

		Exec_ErrorEnd:;
			ProgressBarUpdate("", flagDisplayProgressBar, ref countProgressNow, -1);
			if(null != informationSSPJ)
			{
				informationSSPJ.CleanUp();
			}

			LibraryEditor_SpriteStudio6.Utility.Log.Message("Failure", true, false);	/* External-File only */

			/* Garbage-Collection */
			LibraryEditor_SpriteStudio6.Utility.Miscellaneous.GarbageCollect();

			return(false);
		}
		private static bool ExecSS6PU(	ref Setting setting,
											ref int countProgressNow,
											int countProgressMax,
											bool flagDisplayProgressBar,
											SSPJ.Information informationSSPJ,
											string nameOutputAssetFolderBase
										)
		{
			const string messageLogPrefix = "Convert-Main (SS6PU)";
#if !_FOR_SS6CONVERTER_
			string nameOutputAssetFolder = "";
			string nameOutputAssetBody = "";
			string nameOutputAssetExtention = "";
#endif
			bool flagCreateAssetData = true;

#if !_FOR_SS6CONVERTER_
			/* Decide Asset Names & Check Assets existing */
			if(false == SSPJ.ModeSS6PU.AssetNameDecide(ref setting, informationSSPJ, nameOutputAssetFolderBase))
			{
				goto ExecSS6PU_ErrorEnd;
			}
#endif

			/* Get Datas' count */
			int countTexture = informationSSPJ.TableInformationTexture.Length;
			int countSSCE = informationSSPJ.TableInformationSSCE.Length;
			int countSSAE = informationSSPJ.TableInformationSSAE.Length;
			int countSSEE = informationSSPJ.TableInformationSSEE.Length;

#if !_FOR_SS6CONVERTER_
			/* Determine unused materials */
			/* MEMO: Masking are not judged, because can not make the final decision.    */
			/*       Determined by parent animation's usage (in "Instance" or "Effect"). */
			/* MEMO: This flags(usage status) created here are finally reflected to "Import.Assets <Material> .FlagInUse" */
			/*       in processing "Create Asset: Material" located behind.                                               */
			bool[,] tableFlagUsedmaterialAnimation = new bool[countTexture, (int)Library_SpriteStudio6.KindOperationBlend.TERMINATOR_TABLEMATERIAL];
			bool[,] tableFlagUsedmaterialEffect = new bool[countTexture, (int)Library_SpriteStudio6.KindOperationBlendEffect.TERMINATOR_TABLEMATERIAL];
			if(Setting.GroupBasic.KindNoCreateMaterialUnreferenced.NONE == setting.Basic.NoCreateMaterialUnreferenced)
			{	/* Create all */
				int indexSlot;
				for(int i=0; i<countTexture; i++)
				{
					for(int j=(int)Library_SpriteStudio6.KindOperationBlend.INITIATOR; j<(int)Library_SpriteStudio6.KindOperationBlend.TERMINATOR; j++)
					{
						indexSlot = Script_SpriteStudio6_Root.Material.IndexGetTable(0, (Library_SpriteStudio6.KindOperationBlend)j, Library_SpriteStudio6.KindMasking.THROUGH);
						tableFlagUsedmaterialAnimation[i, indexSlot] = true;
					}
					for(int j=(int)Library_SpriteStudio6.KindOperationBlendEffect.INITIATOR; j<(int)Library_SpriteStudio6.KindOperationBlendEffect.TERMINATOR; j++)
					{
						indexSlot = Script_SpriteStudio6_RootEffect.Material.IndexGetTable(0, (Library_SpriteStudio6.KindOperationBlendEffect)j, Library_SpriteStudio6.KindMasking.THROUGH);
						tableFlagUsedmaterialEffect[i, indexSlot] = true;
					}
				}
			}
			else
			{	/* Create limited */
				bool flagOnlyBlend = (Setting.GroupBasic.KindNoCreateMaterialUnreferenced.BLENDING == setting.Basic.NoCreateMaterialUnreferenced);	/* ? true : false */
				int indexSlot;
				for(int i=0; i<countTexture; i++)
				{
					for(int j=(int)Library_SpriteStudio6.KindOperationBlend.INITIATOR; j<(int)Library_SpriteStudio6.KindOperationBlend.TERMINATOR; j++)
					{
						indexSlot = Script_SpriteStudio6_Root.Material.IndexGetTable(0, (Library_SpriteStudio6.KindOperationBlend)j, Library_SpriteStudio6.KindMasking.THROUGH);
						tableFlagUsedmaterialAnimation[i, indexSlot] = false;
					}
					for(int j=(int)Library_SpriteStudio6.KindOperationBlendEffect.INITIATOR; j<(int)Library_SpriteStudio6.KindOperationBlendEffect.TERMINATOR; j++)
					{
						indexSlot = Script_SpriteStudio6_RootEffect.Material.IndexGetTable(0, (Library_SpriteStudio6.KindOperationBlendEffect)j, Library_SpriteStudio6.KindMasking.THROUGH);
						tableFlagUsedmaterialEffect[i, indexSlot] = false;
					}
				}

				/* Animation */
				if(0 < countSSAE)
				{
					SSAE.Information informationSSAE = null;
					int countInUse;
					Library_SpriteStudio6.KindOperationBlend blend;
					int indexCellMap;
					int indexBlend;
					int indexTexture;
					for(int i=0; i<countSSAE; i++)
					{
						informationSSAE = informationSSPJ.TableInformationSSAE[i];
						countInUse = informationSSAE.ListInUseCellMap.Count;
						for(int j=0; j<countInUse; j++)
						{
							blend = informationSSAE.ListInUseCellMap[j].Blend;
							indexBlend = (int)(blend - Library_SpriteStudio6.KindOperationBlend.INITIATOR);
							indexCellMap = informationSSAE.ListInUseCellMap[j].IndexCellMap;
							if(false == flagOnlyBlend)
							{	/* Blend */
								for(int k=0; k<countTexture; k++)
								{
									tableFlagUsedmaterialAnimation[k, indexBlend] = true;
								}
							}
							else
							{	/* Blend & CellMaps */
								if(0 <= indexCellMap)
								{
									indexTexture = informationSSPJ.TableInformationSSCE[indexCellMap].IndexTexture;
									if(0 <= indexTexture)
									{
										tableFlagUsedmaterialAnimation[indexTexture, indexBlend] = true;
									}
								}
							}
						}
					}
				}

				/* Effect */
				if(0 < countSSEE)
				{
					SSEE.Information informationSSEE = null;
					int countInUse;
					Library_SpriteStudio6.KindOperationBlendEffect blend;
					int indexCellMap;
					int indexTexture;
					for(int i=0; i<countSSEE; i++)
					{
						informationSSEE = informationSSPJ.TableInformationSSEE[i];
						countInUse = informationSSEE.ListInUseCellMap.Count;
						for(int j=0; j<countInUse; j++)
						{
							blend = informationSSEE.ListInUseCellMap[j].Blend;
							if(false == flagOnlyBlend)
							{	/* Blend */
								for(int k=0; k<countTexture; k++)
								{
									tableFlagUsedmaterialEffect[k, (int)blend] = true;
								}
							}
							else
							{	/* Blend & CellMaps */
								indexCellMap = informationSSEE.ListInUseCellMap[j].IndexCellMap;
								if(0 <= indexCellMap)
								{
									indexTexture = informationSSPJ.TableInformationSSCE[indexCellMap].IndexTexture;
									if(0 <= indexTexture)
									{
										tableFlagUsedmaterialEffect[indexTexture, (int)blend] = true;
									}
								}
							}
						}
					}
				}
			}
#endif

#if !_FOR_SS6CONVERTER_
			/* Create Asset: Texture */
			/* MEMO: Create Texture-Assets before CellMap for "Trim Transparent-Pixel". */
			if(0 < countTexture)
			{
				/* Copy Texture files */
				SSCE.Information.Texture informationTexture = null;
				for(int i=0; i<countTexture; i++)
				{
					ProgressBarUpdate(	"Copy Textures (" + (i + 1).ToString() + "/" + countTexture.ToString() + ")",
										flagDisplayProgressBar, ref countProgressNow, countProgressMax
									);

					flagCreateAssetData = true;
					informationTexture = informationSSPJ.TableInformationTexture[i];

					/* Create-Asset */
					if(null == informationTexture.PrefabTexture.TableData[0])
					{	/* New */
						/* Create Output Asset-Folder */
						LibraryEditor_SpriteStudio6.Utility.File.PathSplit(	out nameOutputAssetFolder, out nameOutputAssetBody, out nameOutputAssetExtention,
																			informationTexture.PrefabTexture.TableName[0]
																		);
						if(true == string.IsNullOrEmpty(LibraryEditor_SpriteStudio6.Utility.File.AssetFolderCreate(nameOutputAssetFolder)))
						{
							LogError(messageLogPrefix, "Asset-Folder \"" + nameOutputAssetFolder + "\" could not be created at [" + informationSSPJ.FileNameGetFullPath() + "]");
							goto ExecSS6PU_ErrorEnd;
						}
					}
					else
					{	/* Exist */
						if(false == LibraryEditor_SpriteStudio6.Utility.File.PermissionGetConfirmDialogueOverwrite(	ref setting.ConfirmOverWrite.FlagTexture,
																													informationTexture.PrefabTexture.TableName[0],
																													"Texture"
																												)
							)
						{	/* Not overwrite */
							flagCreateAssetData = false;
							informationTexture.PrefabTexture.FlagUpdate[0] = false;
						}
					}
					if(true == flagCreateAssetData)
					{
						if(false == SSCE.AssetCreateTexture(ref setting, informationSSPJ, informationTexture))
						{
							goto ExecSS6PU_ErrorEnd;
						}
					}
				}
			}
#endif
			countProgressNow -= (countSSEE - countTexture);	/* The number of textures and SSEEs do not necessarily match. When the number is different, SSEEs are more. */

#if !_FOR_SS6CONVERTER_
			/* Create Asset: Material (Animation) */
			/* MEMO: Materials corresponding to number of target-blend types(shader) are generated for 1 texture. */
			ProgressBarUpdate(	"Create Materials",
								flagDisplayProgressBar, ref countProgressNow, countProgressMax
							);

			if((0 < countTexture) && (0 < countSSAE))
			{
				/* Create Materials */
				SSCE.Information.Texture informationTexture = null;
				int indexMaterial;
				int indexMaterialInUse;
				for(int i=0; i<countTexture; i++)
				{
					informationTexture = informationSSPJ.TableInformationTexture[i];
					for(int j=(int)Library_SpriteStudio6.KindOperationBlend.INITIATOR; j<(int)Library_SpriteStudio6.KindOperationBlend.TERMINATOR; j++)
					{
						for(int k=(int)Library_SpriteStudio6.KindMasking.THROUGH; k<(int)Library_SpriteStudio6.KindMasking.TERMINATOR; k++)
						{
							/* MEMO: The materials'  in-use checking are in fixed to "THROUGH" because "Masking" is not consided. */
							flagCreateAssetData = true;
							indexMaterial = Script_SpriteStudio6_Root.Material.IndexGetTable(0, (Library_SpriteStudio6.KindOperationBlend)j, (Library_SpriteStudio6.KindMasking)k);
							indexMaterialInUse = Script_SpriteStudio6_Root.Material.IndexGetTable(0, (Library_SpriteStudio6.KindOperationBlend)j, Library_SpriteStudio6.KindMasking.THROUGH);
								if(false == tableFlagUsedmaterialAnimation[i, indexMaterialInUse])
							{	/* Unrefererced */
								flagCreateAssetData = false;
								informationTexture.MaterialAnimationSS6PU.FlagUpdate[indexMaterial] = false;
								informationTexture.MaterialAnimationSS6PU.FlagInUse[indexMaterial] = false;
							}
							else
							{	/* Refererced */
								informationTexture.MaterialAnimationSS6PU.FlagInUse[indexMaterial] = true;

								/* Create-Asset */
								if(null == informationTexture.MaterialAnimationSS6PU.TableData[indexMaterial])
								{	/* New */
									/* Create Output Asset-Folder */
									LibraryEditor_SpriteStudio6.Utility.File.PathSplit(	out nameOutputAssetFolder, out nameOutputAssetBody, out nameOutputAssetExtention,
																						informationTexture.MaterialAnimationSS6PU.TableName[indexMaterial]
																					);
									if(true == string.IsNullOrEmpty(LibraryEditor_SpriteStudio6.Utility.File.AssetFolderCreate(nameOutputAssetFolder)))
									{
										LogError(messageLogPrefix, "Asset-Folder \"" + nameOutputAssetFolder + "\" could not be created at [" + informationSSPJ.FileNameGetFullPath() + "]");
										goto ExecSS6PU_ErrorEnd;
									}
								}
								else
								{	/* Exist */
									if(false == LibraryEditor_SpriteStudio6.Utility.File.PermissionGetConfirmDialogueOverwrite(	ref setting.ConfirmOverWrite.FlagMaterialAnimation,
																																informationTexture.MaterialAnimationSS6PU.TableName[indexMaterial],
																																"Material Animation"
																															)
										)
									{	/* Not overwrite */
										flagCreateAssetData = false;
										informationTexture.MaterialAnimationSS6PU.FlagUpdate[indexMaterial] = false;
									}
								}
							}

							if(true == flagCreateAssetData)
							{
								if(false == SSCE.ModeSS6PU.AssetCreateMaterialAnimation(	ref setting,
																							informationSSPJ,
																							informationTexture,
																							(Library_SpriteStudio6.KindOperationBlend)j,
																							(Library_SpriteStudio6.KindMasking)k
																						)
									)
								{
									LogError(messageLogPrefix, "Material-Animation [" + ((Library_SpriteStudio6.KindOperationBlend)j).ToString() + "," + ((Library_SpriteStudio6.KindMasking)k).ToString()
																+ "] could not be created at [" + informationSSPJ.FileNameGetFullPath() + "]");
									goto ExecSS6PU_ErrorEnd;
								}
							}
						}
					}
				}
			}

			/* Create Asset: Material (Effect) */
			/* MEMO: Materials corresponding to number of target-blend types(shader) are generated for 1 texture. */
			if((0 < countTexture) && (0 < countSSEE))
			{
				/* Create Materials */
				SSCE.Information.Texture informationTexture = null;
				int indexMaterial;
				int indexMaterialInUse;
				for(int i=0; i<countTexture; i++)
				{
					informationTexture = informationSSPJ.TableInformationTexture[i];
					for(int j=(int)Library_SpriteStudio6.KindOperationBlendEffect.INITIATOR; j<(int)Library_SpriteStudio6.KindOperationBlendEffect.TERMINATOR_TABLEMATERIAL; j++)
					{
						for(int k=(int)Library_SpriteStudio6.KindMasking.THROUGH; k<(int)Library_SpriteStudio6.KindMasking.TERMINATOR; k++)
						{
							/* MEMO: The materials'  in-use checking are in fixed to "THROUGH" because "Masking" is not consided. */
							flagCreateAssetData = true;
							indexMaterial = Script_SpriteStudio6_RootEffect.Material.IndexGetTable(0, (Library_SpriteStudio6.KindOperationBlendEffect)j, (Library_SpriteStudio6.KindMasking)k);
							indexMaterialInUse = Script_SpriteStudio6_RootEffect.Material.IndexGetTable(0, (Library_SpriteStudio6.KindOperationBlendEffect)j, Library_SpriteStudio6.KindMasking.THROUGH);
							if(false == tableFlagUsedmaterialEffect[i, indexMaterialInUse])
							{	/* Unrefererced */
								flagCreateAssetData = false;
								informationTexture.MaterialEffectSS6PU.FlagUpdate[indexMaterial] = false;
								informationTexture.MaterialEffectSS6PU.FlagInUse[indexMaterial] = false;
							}
							else
							{	/* Refererced */
								informationTexture.MaterialEffectSS6PU.FlagInUse[indexMaterial] = true;

								/* Create-Asset */
								if(null == informationTexture.MaterialEffectSS6PU.TableData[indexMaterial])
								{	/* New */
									/* Create Output Asset-Folder */
									LibraryEditor_SpriteStudio6.Utility.File.PathSplit(	out nameOutputAssetFolder, out nameOutputAssetBody, out nameOutputAssetExtention,
																						informationTexture.MaterialEffectSS6PU.TableName[indexMaterial]
																					);
									if(true == string.IsNullOrEmpty(LibraryEditor_SpriteStudio6.Utility.File.AssetFolderCreate(nameOutputAssetFolder)))
									{
										LogError(messageLogPrefix, "Asset-Folder \"" + nameOutputAssetFolder + "\" could not be created at [" + informationSSPJ.FileNameGetFullPath() + "]");
										goto ExecSS6PU_ErrorEnd;
									}
								}
								else
								{	/* Exist */
									if(false == LibraryEditor_SpriteStudio6.Utility.File.PermissionGetConfirmDialogueOverwrite(	ref setting.ConfirmOverWrite.FlagMaterialEffect,
																																informationTexture.MaterialEffectSS6PU.TableName[indexMaterial],
																																"Material Effect"
																															)
										)
									{	/* Not overwrite */
										flagCreateAssetData = false;
										informationTexture.MaterialEffectSS6PU.FlagUpdate[indexMaterial] = false;
									}
								}
							}

							if(true == flagCreateAssetData)
							{
								if(false == SSCE.ModeSS6PU.AssetCreateMaterialEffect(	ref setting,
																						informationSSPJ,
																						informationTexture,
																						(Library_SpriteStudio6.KindOperationBlendEffect)j,
																						(Library_SpriteStudio6.KindMasking)k
																					)
									)
								{
									LogError(messageLogPrefix, "Material-Effect [" + ((Library_SpriteStudio6.KindOperationBlend)j).ToString() + "," + ((Library_SpriteStudio6.KindMasking)k).ToString()
																+ "] could not be created at [" + informationSSPJ.FileNameGetFullPath() + "]");
									goto ExecSS6PU_ErrorEnd;
								}
							}
						}
					}
				}
			}
#endif

			/* Create Asset: CellMap */
			/* MEMO: Since informations of SSCE files are grouped in 1 CellMap data-asset, always only 1 CellMap data-asset for a SSPJ. */
			/* MEMO: Process after creating all Texture-Assets. */
			if(0 < countSSCE)
			{
				SSCE.Information informationSSCE = null;
				for(int i=0; i<countSSCE; i++)
				{
					/* MEMO: Be sure to "Convert" even when not create CellMap data-assets. Datas may be used at converting SSAE. */
					informationSSCE = informationSSPJ.TableInformationSSCE[i];

#if !_FOR_SS6CONVERTER_
					/* Convert "Trim Transparent-Pixel" */
					if(true == setting.PreCalcualation.FlagTrimTransparentPixelsCell)
					{
						ProgressBarUpdate(	"Convert SSCEs \"Trim Pixel\" (" + (i + 1).ToString() + "/" + countSSCE.ToString() + ")",
											flagDisplayProgressBar, ref countProgressNow, countProgressMax
										);

						if(false == SSCE.CellTrimTransparentPixel(ref setting, informationSSPJ, informationSSCE))
						{
							goto ExecSS6PU_ErrorEnd;
						}
					}
#endif

					/* Convert */
					ProgressBarUpdate(	"Convert SSCEs (" + (i + 1).ToString() + "/" + countSSCE.ToString() + ")",
										flagDisplayProgressBar, ref countProgressNow, countProgressMax
									);
					if(false == SSCE.ModeSS6PU.ConvertCellMap(ref setting, informationSSPJ, informationSSCE))
					{
						goto ExecSS6PU_ErrorEnd;
					}
				}

				/* Create-Asset */
				ProgressBarUpdate(	"Create Asset \"Data-CellMap\"",
									flagDisplayProgressBar, ref countProgressNow, countProgressMax
								);

				flagCreateAssetData = true;
#if !_FOR_SS6CONVERTER_
				if(null == informationSSPJ.DataCellMapSS6PU.TableData[0])
				{	/* New */
					/* Create Output Asset-Folder */
					LibraryEditor_SpriteStudio6.Utility.File.PathSplit(	out nameOutputAssetFolder, out nameOutputAssetBody, out nameOutputAssetExtention,
																		informationSSPJ.DataCellMapSS6PU.TableName[0]
																	);
					if(true == string.IsNullOrEmpty(LibraryEditor_SpriteStudio6.Utility.File.AssetFolderCreate(nameOutputAssetFolder)))
					{
						LogError(messageLogPrefix, "Asset-Folder \"" + nameOutputAssetFolder + "\" could not be created at [" + informationSSPJ.FileNameGetFullPath() + "]");
						goto ExecSS6PU_ErrorEnd;
					}
				}
				else
				{	/* Exist */
					if(false == LibraryEditor_SpriteStudio6.Utility.File.PermissionGetConfirmDialogueOverwrite(	ref setting.ConfirmOverWrite.FlagDataCellMap,
																												informationSSPJ.DataCellMapSS6PU.TableName[0],
																												"Data CellMap"
																											)
						)
					{	/* Not overwrite */
						flagCreateAssetData = false;
						informationSSPJ.DataCellMapSS6PU.FlagUpdate[0] = false;
					}
				}
				if(true == flagCreateAssetData)
				{
					if(false == SSPJ.ModeSS6PU.AssetCreateCellMap(ref setting, informationSSPJ))
					{
						goto ExecSS6PU_ErrorEnd;
					}
				}
#else
				if(false == SSPJ.ModeSS6PU.DataOutputCellMap(ref setting, informationSSPJ))
				{
					goto ExecSS6PU_ErrorEnd;
				}
#endif
			}

#if !_FOR_SS6CONVERTER_
			/* Pick up Materials */
			if(false == SSPJ.ModeSS6PU.MaterialPickUp(ref setting, informationSSPJ))
			{
				goto ExecSS6PU_ErrorEnd;
			}

			/* Delete unreferenced Materials */
			if(false == SSPJ.ModeSS6PU.MaterialDeleteUnreferenced(ref setting, informationSSPJ))
			{
				goto ExecSS6PU_ErrorEnd;
			}
#endif

			/* Create-Asset: Effect */
			/* MEMO: SSEE always has only 1 data-asset & 1 prefab. */
			if(0 < countSSEE)
			{
				SSEE.Information informationSSEE = null;
				for(int i=0; i<countSSEE; i++)
				{
					informationSSEE = informationSSPJ.TableInformationSSEE[i];

					/* Convert: Data */
					ProgressBarUpdate(	"Convert SSEEs (" + (i + 1).ToString() + "/" + countSSEE.ToString() + ")",
										flagDisplayProgressBar, ref countProgressNow, countProgressMax
									);

					if(false == SSEE.ModeSS6PU.ConvertData(ref setting, informationSSPJ, informationSSEE))
					{
						goto ExecSS6PU_ErrorEnd;
					}

					/* Create-Asset: Data */
					ProgressBarUpdate(	"Create Asset \"Data-Effect\" (" + (i + 1).ToString() + "/" + countSSEE.ToString() + ")",
										flagDisplayProgressBar, ref countProgressNow, countProgressMax
									);
#if !_FOR_SS6CONVERTER_
					flagCreateAssetData = true;
					if(null == informationSSEE.DataEffectSS6PU.TableData[0])
					{	/* New */
						/* Create Output Asset-Folder */
						LibraryEditor_SpriteStudio6.Utility.File.PathSplit(	out nameOutputAssetFolder, out nameOutputAssetBody, out nameOutputAssetExtention,
																			informationSSEE.DataEffectSS6PU.TableName[0]
																		);
						if(true == string.IsNullOrEmpty(LibraryEditor_SpriteStudio6.Utility.File.AssetFolderCreate(nameOutputAssetFolder)))
						{
							LogError(messageLogPrefix, "Asset-Folder \"" + nameOutputAssetFolder + "\" could not be created at [" + informationSSPJ.FileNameGetFullPath() + "]");
							goto ExecSS6PU_ErrorEnd;
						}
					}
					else
					{	/* Exist */
						if(false == LibraryEditor_SpriteStudio6.Utility.File.PermissionGetConfirmDialogueOverwrite(	ref setting.ConfirmOverWrite.FlagDataEffect,
																													informationSSEE.DataEffectSS6PU.TableName[0],
																													"Data Effect"
																												)
							)
						{	/* Not overwrite */
							flagCreateAssetData = false;
							informationSSEE.DataEffectSS6PU.FlagUpdate[0] = false;
						}
					}
					if(true == flagCreateAssetData)
					{
						if(false == SSEE.ModeSS6PU.AssetCreateData(ref setting, informationSSPJ, informationSSEE))
						{
							goto ExecSS6PU_ErrorEnd;
						}
					}
#else
					if(false == SSEE.ModeSS6PU.DataOutput(ref setting, informationSSPJ, informationSSEE))
					{
						goto ExecSS6PU_ErrorEnd;
					}
#endif

#if !_FOR_SS6CONVERTER_
					/* Create-Asset: Prefab */
					ProgressBarUpdate(	"Create Asset \"Prefab-Effect\" (" + (i + 1).ToString() + "/" + countSSEE.ToString() + ")",
										flagDisplayProgressBar, ref countProgressNow, countProgressMax
									);

					flagCreateAssetData = true;
					if(null == informationSSEE.PrefabEffectSS6PU.TableData[0])
					{	/* New */
						/* Create Output Asset-Folder */
						LibraryEditor_SpriteStudio6.Utility.File.PathSplit(	out nameOutputAssetFolder, out nameOutputAssetBody, out nameOutputAssetExtention,
																			informationSSEE.PrefabEffectSS6PU.TableName[0]
																		);
						if(true == string.IsNullOrEmpty(LibraryEditor_SpriteStudio6.Utility.File.AssetFolderCreate(nameOutputAssetFolder)))
						{
							LogError(messageLogPrefix, "Asset-Folder \"" + nameOutputAssetFolder + "\" could not be created at [" + informationSSPJ.FileNameGetFullPath() + "]");
							goto ExecSS6PU_ErrorEnd;
						}
					}
					else
					{	/* Exist */
						if(false == LibraryEditor_SpriteStudio6.Utility.File.PermissionGetConfirmDialogueOverwrite(	ref setting.ConfirmOverWrite.FlagDataEffect,
																													informationSSEE.PrefabEffectSS6PU.TableName[0],
																													"Prefab Effect"
																												)
							)
						{	/* Not overwrite */
							flagCreateAssetData = false;
							informationSSEE.PrefabEffectSS6PU.FlagUpdate[0] = false;
						}
					}
					if(true == flagCreateAssetData)
					{
						if(false == SSEE.ModeSS6PU.AssetCreatePrefab(ref setting, informationSSPJ, informationSSEE))
						{
							goto ExecSS6PU_ErrorEnd;
						}
					}
#endif
				}
			}

			/* Create-Asset: Animation */
			/* MEMO: SSAE always has only 1 data-asset & 1 prefab. */
			if(0 < countSSAE)
			{
				int indexSSAE;
				SSAE.Information informationSSAE = null;
				for(int i=0; i<countSSAE; i++)
				{
					indexSSAE = informationSSPJ.QueueConvertSSAE[i];
					informationSSAE = informationSSPJ.TableInformationSSAE[indexSSAE];

					/* Open Pack-Attribute's Dictionary */
					if(false == Library_SpriteStudio6.Data.Animation.PackAttribute.DictionaryBootUp(-1, -1, null))
					{
						LogError(messageLogPrefix, "Failure Open PackAttribute's dictionary (for entire-SSAE)  at [" + informationSSPJ.FileNameGetFullPath() + "]");
						goto ExecSS6PU_ErrorEnd;
					}

					/* Convert: Data */
					ProgressBarUpdate(	"Convert SSAEs (" + (i + 1).ToString() + "/" + countSSAE.ToString() + ")",
										flagDisplayProgressBar, ref countProgressNow, countProgressMax
									);

					if(false == SSAE.ModeSS6PU.ConvertData(ref setting, informationSSPJ, informationSSAE))
					{
						goto ExecSS6PU_ErrorEnd;
					}

					/* Create-Asset: Data */
					ProgressBarUpdate(	"Create Asset \"Data-Animation\" (" + (i + 1).ToString() + "/" + countSSAE.ToString() + ")",
										flagDisplayProgressBar, ref countProgressNow, countProgressMax
									);
#if !_FOR_SS6CONVERTER_
					flagCreateAssetData = true;
					if(null == informationSSAE.DataAnimationSS6PU.TableData[0])
					{	/* New */
						/* Create Output Asset-Folder */
						LibraryEditor_SpriteStudio6.Utility.File.PathSplit(	out nameOutputAssetFolder, out nameOutputAssetBody, out nameOutputAssetExtention,
																			informationSSAE.DataAnimationSS6PU.TableName[0]
																		);
						if(true == string.IsNullOrEmpty(LibraryEditor_SpriteStudio6.Utility.File.AssetFolderCreate(nameOutputAssetFolder)))
						{
							LogError(messageLogPrefix, "Asset-Folder \"" + nameOutputAssetFolder + "\" could not be created at [" + informationSSPJ.FileNameGetFullPath() + "]");
							goto ExecSS6PU_ErrorEnd;
						}
					}
					else
					{	/* Exist */
						if(false == LibraryEditor_SpriteStudio6.Utility.File.PermissionGetConfirmDialogueOverwrite(	ref setting.ConfirmOverWrite.FlagDataAnimation,
																													informationSSAE.DataAnimationSS6PU.TableName[0],
																													"Data Animation"
																												)
							)
						{	/* Not overwrite */
							flagCreateAssetData = false;
							informationSSAE.DataAnimationSS6PU.FlagUpdate[0] = false;
						}
					}
					if(true == flagCreateAssetData)
					{
						if(false == SSAE.ModeSS6PU.AssetCreateData(ref setting, informationSSPJ, informationSSAE))
						{
							goto ExecSS6PU_ErrorEnd;
						}
					}
#else
					if(false == SSAE.ModeSS6PU.OutputData(ref setting, informationSSPJ, informationSSAE))
					{
						goto ExecSS6PU_ErrorEnd;
					}
#endif

					/* Close Pack-Attribute's Dictionary */
					Script_SpriteStudio6_DataAnimation scriptDtaAnimation = informationSSAE.DataAnimationSS6PU.TableData[0];
					if(false == Library_SpriteStudio6.Data.Animation.PackAttribute.DictionaryShutDown(-1, -1, scriptDtaAnimation))
					{
						LogError(messageLogPrefix, "Failure Close PackAttribute's dictionary (for entire-SSAE)  at [" + informationSSPJ.FileNameGetFullPath() + "]");
						goto ExecSS6PU_ErrorEnd;
					}

#if !_FOR_SS6CONVERTER_
					if(null != scriptDtaAnimation)
					{	/* Re-Save */
						EditorUtility.SetDirty(scriptDtaAnimation);
						AssetDatabase.SaveAssets();
					}

					/* Create-Asset: Prefab */
					ProgressBarUpdate(	"Create Asset \"Prefab-Animation\" (" + (i + 1).ToString() + "/" + countSSAE.ToString() + ")",
										flagDisplayProgressBar, ref countProgressNow, countProgressMax
									);

					flagCreateAssetData = true;
					if(null == informationSSAE.PrefabAnimationSS6PU.TableData[0])
					{	/* New */
						/* Create Output Asset-Folder */
						LibraryEditor_SpriteStudio6.Utility.File.PathSplit(	out nameOutputAssetFolder, out nameOutputAssetBody, out nameOutputAssetExtention,
																			informationSSAE.PrefabAnimationSS6PU.TableName[0]
																		);
						if(true == string.IsNullOrEmpty(LibraryEditor_SpriteStudio6.Utility.File.AssetFolderCreate(nameOutputAssetFolder)))
						{
							LogError(messageLogPrefix, "Asset-Folder \"" + nameOutputAssetFolder + "\" could not be created at [" + informationSSPJ.FileNameGetFullPath() + "]");
							goto ExecSS6PU_ErrorEnd;
						}
					}
					else
					{	/* Exist */
						if(false == LibraryEditor_SpriteStudio6.Utility.File.PermissionGetConfirmDialogueOverwrite(	ref setting.ConfirmOverWrite.FlagDataAnimation,
																													informationSSAE.PrefabAnimationSS6PU.TableName[0],
																													"Prefab Animation"
																												)
							)
						{	/* Not overwrite */
							flagCreateAssetData = false;
							informationSSAE.PrefabAnimationSS6PU.FlagUpdate[0] = false;
						}
					}
					if(true == flagCreateAssetData)
					{
						if(false == SSAE.ModeSS6PU.AssetCreatePrefab(ref setting, informationSSPJ, informationSSAE))
						{
							goto ExecSS6PU_ErrorEnd;
						}
					}
#endif
				}
			}

#if !_FOR_SS6CONVERTER_
#else
			/* Create-Asset: Project */
			Script_SpriteStudio6_DataProject dataProject = new Script_SpriteStudio6_DataProject();
			if(null == dataProject)
			{
				return(false);
			}
			informationSSPJ.DataProject.BootUp(1);	/* Always 1 */
			informationSSPJ.DataProject.TableData[0] = dataProject;

#if !_FOR_SS6CONVERTER_
			dataProject.Version = Script_SpriteStudio6_DataProject.KindVersion.SUPPORT_LATEST;
#else
			dataProject.Version = (int)Script_SpriteStudio6_DataProject.KindVersion.SUPPORT_LATEST;
#endif

			dataProject.CellMap = informationSSPJ.DataCellMapSS6PU.TableData[0];	/* Now only 1 */

			dataProject.Animation = new Script_SpriteStudio6_DataAnimation[countSSAE];
			if(null == dataProject.Animation)
			{
				return(false);
			}
			for(int i=0; i<countSSAE; i++)
			{
				dataProject.Animation[i] = informationSSPJ.TableInformationSSAE[i].DataAnimationSS6PU.TableData[0];
			}

			dataProject.Effect = new Script_SpriteStudio6_DataEffect[countSSEE];
			if(null == dataProject.Effect)
			{
				return(false);
			}
			for(int i=0; i<countSSEE; i++)
			{
				dataProject.Effect[i] = informationSSPJ.TableInformationSSEE[i].DataEffectSS6PU.TableData[0];
			}
#endif

//            ExportSSFB2.output_ssfb2(dataProject , "c:\\temp\\test.fb2" );
            ExportSSFB2.output_ssfb2(dataProject, nameOutputAssetFolderBase);

            


            return (true);

		ExecSS6PU_ErrorEnd:;
			return(false);
		}
		private static bool ExecUnityNative(	ref Setting setting,
												ref int countProgressNow,
												int countProgressMax,
												bool flagDisplayProgressBar,
												SSPJ.Information informationSSPJ,
												string nameOutputAssetFolderBase
											)
		{
#if !_FOR_SS6CONVERTER_
			const string messageLogPrefix = "Convert-Main (UnityNative)";
			string nameOutputAssetFolder = "";
			string nameOutputAssetBody = "";
			string nameOutputAssetExtention = "";
			bool flagCreateAssetData = true;

			/* Decide Asset Names & Check Assets existing */
			if(false == SSPJ.ModeUnityNative.AssetNameDecide(ref setting, informationSSPJ, nameOutputAssetFolderBase))
			{
				goto ExecUnityNative_ErrorEnd;
			}

			/* Get Datas' count */
			int countTexture = informationSSPJ.TableInformationTexture.Length;
			int countSSCE = informationSSPJ.TableInformationSSCE.Length;
			int countSSAE = informationSSPJ.TableInformationSSAE.Length;
			int countSSEE = informationSSPJ.TableInformationSSEE.Length;

			/* Create Asset: Texture */
			/* MEMO: Create Texture-Assets before CellMap for "Trim Transparent-Pixel". */
			/* MEMO: Enable read till sprite-datas set complete. */
			if(0 < countTexture)
			{
				/* Copy Texture files */
				SSCE.Information.Texture informationTexture = null;
				for(int i=0; i<countTexture; i++)
				{
					ProgressBarUpdate(	"Copy Textures (" + (i + 1).ToString() + "/" + countTexture.ToString() + ")",
										flagDisplayProgressBar, ref countProgressNow, countProgressMax
									);

					flagCreateAssetData = true;
					informationTexture = informationSSPJ.TableInformationTexture[i];

					/* Create-Asset */
					if(null == informationTexture.PrefabTexture.TableData[0])
					{	/* New */
						/* Create Output Asset-Folder */
						LibraryEditor_SpriteStudio6.Utility.File.PathSplit(	out nameOutputAssetFolder, out nameOutputAssetBody, out nameOutputAssetExtention,
																			informationTexture.PrefabTexture.TableName[0]
																		);
						if(true == string.IsNullOrEmpty(LibraryEditor_SpriteStudio6.Utility.File.AssetFolderCreate(nameOutputAssetFolder)))
						{
							LogError(messageLogPrefix, "Asset-Folder \"" + nameOutputAssetFolder + "\" could not be created at [" + informationSSPJ.FileNameGetFullPath() + "]");
							goto ExecUnityNative_ErrorEnd;
						}
					}
					else
					{	/* Exist */
						if(false == LibraryEditor_SpriteStudio6.Utility.File.PermissionGetConfirmDialogueOverwrite(	ref setting.ConfirmOverWrite.FlagTexture,
																													informationTexture.PrefabTexture.TableName[0],
																													"Texture"
																												)
							)
						{	/* Not overwrite */
							flagCreateAssetData = false;
						}
					}
					if(true == flagCreateAssetData)
					{
						if(false == SSCE.AssetCreateTexture(ref setting, informationSSPJ, informationTexture))
						{
							goto ExecUnityNative_ErrorEnd;
						}
					}
				}
			}
			countProgressNow -= (countSSEE - countTexture);	/* The number of textures and SSEEs do not necessarily match. When the number is different, SSEEs are more. */

			/* Convert SSCEs */
			/* MEMO: Currently, SSCE does not has own data-file in "Unity-Native" mode. */
			/*       (... but might create later)                                       */
			if(0 < countSSCE)
			{
				SSCE.Information informationSSCE = null;
				for(int i=0; i<countSSCE; i++)
				{
					/* MEMO: Be sure to "Convert" even when not create CellMap data-assets. Datas may be used at converting SSAE. */
					informationSSCE = informationSSPJ.TableInformationSSCE[i];

					/* MEMO: "Trim Transparent-Pixel" processing is unnecessary since Unity's sprite trims transparent pixels automatically by mesh shape. */

					/* Convert (Create Textures' Atlas) */
					ProgressBarUpdate(	"Convert SSCEs (" + (i + 1).ToString() + "/" + countSSCE.ToString() + ")",
										flagDisplayProgressBar, ref countProgressNow, countProgressMax
									);
					if(false == SSCE.ModeUnityNative.ConvertCellMap(ref setting, informationSSPJ, informationSSCE))
					{
						goto ExecUnityNative_ErrorEnd;
					}
				}

				/* Fix Texture */
				for(int i=0; i<countTexture; i++)
				{
					/* Add Atlases to Textures */
					if(false == SSCE.ModeUnityNative.CellMapSetTexture(ref setting, informationSSPJ, i))
					{
						goto ExecUnityNative_ErrorEnd;
					}
				}
			}

			/* Create Assrts SSAEs */
			if(0 < countSSAE)
			{
				SSAE.Information informationSSAE = null;
				for(int i=0; i<countSSAE; i++)
				{
					informationSSAE = informationSSPJ.TableInformationSSAE[i];

					/* Convert Parts (Create Temporary GameObjects) */
					ProgressBarUpdate(	"Convert & Create Asset SSAEs (" + (i + 1).ToString() + "/" + countSSAE.ToString() + ")",
										flagDisplayProgressBar, ref countProgressNow, countProgressMax
									);

					GameObject gameObjectRoot = SSAE.ModeUnityNative.ConvertPartsAnimation(ref setting, informationSSPJ, informationSSAE);
					if(null == gameObjectRoot)
					{
						goto ExecUnityNative_ErrorEnd;
					}

					/* Convert SSAEs: Create Bone-Information */
					SSAE.ModeUnityNative.CreateBoneInformation(ref setting, informationSSPJ, informationSSAE);

					/* Create Asset: Mesh-Bind (Skinned-Mesh) */
					int countParts = informationSSAE.TableParts.Length;
					for(int j=0; j<countParts; j++)
					{
						flagCreateAssetData = true;
						if(	(false == string.IsNullOrEmpty(informationSSAE.TableParts[j].DataMeshSkinnedUnityNative.TableName[0]))
							&& (null == informationSSAE.TableParts[j].DataMeshSkinnedUnityNative.TableData[0])
							)
						{	/* New */
							/* Create Output Asset-Folder */
							LibraryEditor_SpriteStudio6.Utility.File.PathSplit(	out nameOutputAssetFolder, out nameOutputAssetBody, out nameOutputAssetExtention,
																				informationSSAE.TableParts[j].DataMeshSkinnedUnityNative.TableName[0]
																			);
							if(true == string.IsNullOrEmpty(LibraryEditor_SpriteStudio6.Utility.File.AssetFolderCreate(nameOutputAssetFolder)))
							{
								LogError(messageLogPrefix, "Asset-Folder \"" + nameOutputAssetFolder + "\" could not be created at [" + informationSSPJ.FileNameGetFullPath() + "]");
								goto ExecUnityNative_ErrorEnd;
							}
						}
						else
						{	/* Exist */
							if(false == LibraryEditor_SpriteStudio6.Utility.File.PermissionGetConfirmDialogueOverwrite(	ref setting.ConfirmOverWrite.FlagDataAnimation,
																														informationSSAE.TableParts[j].DataMeshSkinnedUnityNative.TableName[0],
																														"Data Skinned-Mesh"
																													)
								)
							{	/* Not overwrite */
								flagCreateAssetData = false;
								informationSSAE.TableParts[j].DataMeshSkinnedUnityNative.FlagUpdate[0] = false;
							}
						}
						if(true == flagCreateAssetData)
						{
							/* MEMO: In this process only Bind-Pose are not determined. */
							/*       Bind-Pose are determined at runtime.               */
							/* MEMO: Since processing to optimize bones used in this function to the minimum is performed, */
							/*       set each "Mesh" part's "Script_SpriteStudio6_PartsUnityNative.TableTransformBone".    */
							/*       In addition, replace component for mesh parts not assigned bones with                 */
							/*        "MeshFilter & MeshRendere" within this function.                                     */
							if(false == SSAE.ModeUnityNative.AssetCreateDataMesh(ref setting, informationSSPJ, informationSSAE, j))
							{
								goto ExecUnityNative_ErrorEnd;
							}
						}
					}

					/* Create Asset: Animation (AnimationClip) */
					int countAnimation = informationSSAE.TableAnimation.Length;
					for(int j=0; j<countAnimation; j++)
					{
						flagCreateAssetData = true;
						if(null == informationSSAE.DataAnimationUnityNative.TableData[j])
						{	/* New */
							/* Create Output Asset-Folder */
							LibraryEditor_SpriteStudio6.Utility.File.PathSplit(	out nameOutputAssetFolder, out nameOutputAssetBody, out nameOutputAssetExtention,
																				informationSSAE.DataAnimationUnityNative.TableName[j]
																			);
							if(true == string.IsNullOrEmpty(LibraryEditor_SpriteStudio6.Utility.File.AssetFolderCreate(nameOutputAssetFolder)))
							{
								LogError(messageLogPrefix, "Asset-Folder \"" + nameOutputAssetFolder + "\" could not be created at [" + informationSSPJ.FileNameGetFullPath() + "]");
								goto ExecUnityNative_ErrorEnd;
							}
						}
						else
						{	/* Exist */
							if(false == LibraryEditor_SpriteStudio6.Utility.File.PermissionGetConfirmDialogueOverwrite(	ref setting.ConfirmOverWrite.FlagDataAnimation,
																														informationSSAE.DataAnimationUnityNative.TableName[j],
																														"Data Animation"
																													)
								)
							{	/* Not overwrite */
								flagCreateAssetData = false;
								informationSSAE.DataAnimationUnityNative.FlagUpdate[j] = false;
							}
						}
						if(true == flagCreateAssetData)
						{
							if(false == SSAE.ModeUnityNative.AssetCreateData(ref setting, informationSSPJ, informationSSAE, j))
							{
								goto ExecUnityNative_ErrorEnd;
							}
						}
					}

					/* Create Asset: Animation (Prefab) */
					flagCreateAssetData = true;
					if(null == informationSSAE.PrefabAnimationUnityNative.TableData[0])
					{	/* New */
						/* Create Output Asset-Folder */
						LibraryEditor_SpriteStudio6.Utility.File.PathSplit(	out nameOutputAssetFolder, out nameOutputAssetBody, out nameOutputAssetExtention,
																			informationSSAE.PrefabAnimationUnityNative.TableName[0]
																		);
						if(true == string.IsNullOrEmpty(LibraryEditor_SpriteStudio6.Utility.File.AssetFolderCreate(nameOutputAssetFolder)))
						{
							LogError(messageLogPrefix, "Asset-Folder \"" + nameOutputAssetFolder + "\" could not be created at [" + informationSSPJ.FileNameGetFullPath() + "]");
							goto ExecUnityNative_ErrorEnd;
						}
					}
					else
					{	/* Exist */
						if(false == LibraryEditor_SpriteStudio6.Utility.File.PermissionGetConfirmDialogueOverwrite(	ref setting.ConfirmOverWrite.FlagDataAnimation,
																													informationSSAE.PrefabAnimationUnityNative.TableName[0],
																													"Prefab Animation"
																												)
							)
						{	/* Not overwrite */
							flagCreateAssetData = false;
							informationSSAE.PrefabAnimationUnityNative.FlagUpdate[0] = false;
						}
					}
					if(true == flagCreateAssetData)
					{
						if(false == SSAE.ModeUnityNative.AssetCreatePrefab(ref setting, informationSSPJ, informationSSAE, gameObjectRoot))
						{
							goto ExecUnityNative_ErrorEnd;
						}
						gameObjectRoot = null;
					}
				}
			}

			return(true);

		ExecUnityNative_ErrorEnd:;
			return(false);
#else
			return(false);
#endif
		}

		private static void LogError(string messagePrefix, string message)
		{
			LibraryEditor_SpriteStudio6.Utility.Log.Error(messagePrefix + ": " + message);
		}

		private static void LogWarning(string messagePrefix, string message)
		{
			LibraryEditor_SpriteStudio6.Utility.Log.Warning(messagePrefix + ": " + message);
		}

		public static void ProgressBarUpdate(string nameTask, bool flagSwitch, ref int step, int stepFull)
		{
#if !_FOR_SS6CONVERTER_
			LibraryEditor_SpriteStudio6.Utility.Miscellaneous.ProgressBarUpdate(	Library_SpriteStudio6.SignatureNameAsset + " Data Import",
																					nameTask,
																					flagSwitch,
																					step,
																					stepFull
																				);
#endif
			step++;
		}
#endregion Functions

		/* ----------------------------------------------- Enums & Constants */
#region Enums & Constants
		public enum KindFile
		{
			NON = -1,	/* Through */
			TEXTURE = 0,
			SSPJ,
			SSCE,
			SSAE,
			SSEE,
		}

		public const string NameExtentionMaterial = ".mat";
		public const string NameExtentionMesh = ".asset";
		public const string NameExtentionScriptableObject = ".asset";
		public const string NameExtensionPrefab = ".prefab";

		public const string NameTagSpritePackerTexture = "SpriteStudio";

#if !_FOR_SS6CONVERTER_
#if UNITY_2018_4_OR_NEWER || UNITY_2019_1_OR_NEWER
#else
		public const ReplacePrefabOptions OptionPrefabReplace = ReplacePrefabOptions.ReplaceNameBased;
#endif
#endif
#endregion Enums & Constants

		/* ----------------------------------------------- Classes, Structs & Interfaces */
#region Classes, Structs & Interfaces
		public struct Assets<_Type>
			where _Type : class
		{
			/* ----------------------------------------------- Variables & Properties */
#region Variables & Properties
			public bool[] FlagUpdate;
			public bool[] FlagInUse;						/* MEMO: Basically used only in materials */
			public int[] Version;
			public string[] TableName;
			public _Type[] TableData;
#endregion Variables & Properties

			/* ----------------------------------------------- Functions */
#region Functions
			public void CleanUp()
			{
				FlagUpdate = null;
				FlagInUse = null;
				Version = null;
				TableName = null;
				TableData = null;
			}

			public void BootUp(int count)
			{
				FlagUpdate = new bool[count];
				FlagInUse = new bool[count];
				Version = new int[count];
				TableName = new string[count];
				TableData = new _Type[count];
				for(int i=0; i<count; i++)
				{
					FlagUpdate[i] = true;
					FlagInUse[i] = true;
					Version[i] = -1;
					TableName[i] = null;
					TableData[i] = null;
				}
			}
#endregion Functions
		}

		public static partial class SSPJ
		{
			/* Part: SpriteStudio6/Editor/Import/SSPJE.cs */
		}

		public static partial class SSCE
		{
			/* Part: SpriteStudio6/Editor/Import/SSCE.cs */
		}

		public static partial class SSAE
		{
			/* Part: SpriteStudio6/Editor/Import/SSAE.cs */
		}

		public static partial class SSEE
		{
			/* Part: SpriteStudio6/Editor/Import/SSEE.cs */
		}

		public static partial class Batch
		{
			/* Part: SpriteStudio6/Editor/Import/Batch.cs */
		}

		/* Part: SpriteStudio6/Editor/Import/Setting.cs */
#endregion Classes, Structs & Interfaces
	}

	public static partial class Utility
	{
		/* ----------------------------------------------- Classes, Structs & Interfaces */
#region Classes, Structs & Interfaces
		public static partial class File
		{
			/* ----------------------------------------------- Functions */
#region Functions
#if !_FOR_SS6CONVERTER_
			public static bool NamesGetFileDialogLoad(	out string nameDirectory,
														out string nameFileBody,
														out string nameFileExtension,
														string nameDirectoryPrevious,
														string textTitleDialog,
														string filterExtension
													)
			{
				if(true == string.IsNullOrEmpty(nameDirectoryPrevious))
				{
					nameDirectoryPrevious = "";
				}

				/* Choose file */
				string fileNameFullPath = EditorUtility.OpenFilePanel(textTitleDialog, nameDirectoryPrevious, filterExtension);
				if(0 == fileNameFullPath.Length)
				{	/* Cancelled */
					nameDirectory = "";
					nameFileBody = "";
					nameFileExtension = "";

					return(false);
				}

				return(PathSplit(out nameDirectory, out nameFileBody, out nameFileExtension, fileNameFullPath));
			}

			public static bool NamesGetFileDialogSave(	out string nameDirectory,
														out string nameFileBody,
														out string nameFileExtension,
														string nameDirectoryPrevious,
														string nameFilePrevious,
														string textTitleDialog,
														string nameExtension
													)
			{
				/* Choose file */
				string fileNameFullPath = EditorUtility.SaveFilePanel(textTitleDialog, nameDirectoryPrevious, nameFilePrevious, nameExtension);
				if(0 == fileNameFullPath.Length)
				{	/* Cancelled */
					nameDirectory = "";
					nameFileBody = "";
					nameFileExtension = "";

					return(false);
				}

				return(PathSplit(out nameDirectory, out nameFileBody, out nameFileExtension, fileNameFullPath));
			}

			internal static bool PermissionGetConfirmDialogueOverwrite(ref bool flagSwitchSetting, string nameAsset, string nameTypeAsset)
			{
				if(false == flagSwitchSetting)
				{	/* No-Confirm */
					return(true);
				}

				bool rv = false;
				int KindResult = EditorUtility.DisplayDialogComplex(	"Asset already exists.",
																		"Do you want to overwrite?\n" + nameAsset,
																		"Yes",
																		"Yes, all \"" + nameTypeAsset +"\"s",
																		"No"
																	);
				switch(KindResult)
				{
					case 0:	/* Yes */
						rv = true;
						break;

					case 1:	/* All */
						flagSwitchSetting = false;
						rv = true;
						break;

					case 2:	/* No */
						rv = false;
						break;

				}
				return(rv);
			}
#endif

			public static string AssetFolderCreate(string namePath)
			{
				if(true == string.IsNullOrEmpty(namePath))
				{
					return(null);
				}
#if !_FOR_SS6CONVERTER_
#if false
				/* MEMO: When use "AssetDataBase.CreateFolder" to create folders recursively, processing may be delayed. */
				/* Create Folder Recursive */
				string namePathParent = "Assets";
				string namePathChild = "";
				string[] namePathSplit = namePath.Split(TextSplitFolder);
				int count = namePathSplit.Length;
				if(0 >= count)
				{
					return(null);
				}

				int indexTop = (NamePathRootAsset.ToLower() == namePathSplit[0].ToLower()) ? 1 : 0;
				for(int i=indexTop; i<count; i++)
				{
					namePathChild = namePathSplit[i];
					if(false == string.IsNullOrEmpty(namePathChild))
					{
						if(false == AssetDatabase.IsValidFolder(namePathParent + "/" + namePathChild))
						{
							AssetDatabase.CreateFolder(namePathParent, namePathChild);
						}
						namePathParent += "/" + namePathChild;
					}
				}

				namePathParent += "/";
				return(namePathParent);
#else
				/* MEMO: Originally, way that should not take. Use "System.IO.Directory.CreateDirectory" to create folders. */
				string[] namePathSplit = namePath.Split(TextSplitFolder);
				int count = namePathSplit.Length;
				if(0 >= count)
				{
					return(null);
				}

				/* Reconstruct path */
				int indexTop = (NamePathRootAsset.ToLower() == namePathSplit[0].ToLower()) ? 1 : 0;
				string namePathAsset = string.Copy(NamePathRootAsset);
				string namePathNative = string.Copy(NamePathRootNative);
				string namePathChild = null;
				for(int i=indexTop; i<count; i++)
				{
					namePathChild = namePathSplit[i];
					if(false == string.IsNullOrEmpty(namePathChild))
					{
						namePathNative += "/" + namePathChild;
						namePathAsset += "/" + namePathChild;
					}
				}

				/* Create folder, if not exist. */
				if(false == System.IO.Directory.Exists(namePathNative))
				{
					System.IO.Directory.CreateDirectory(namePathNative);
				}

				namePathAsset += "/";
				return(namePathAsset);
#endif
#else
				/* MEMO: When converter, directories are recursively created with direct path name. (is not asset folder name) */
				string nameDirectory = System.IO.Path.GetDirectoryName(namePath);
				if(false == System.IO.Directory.Exists(nameDirectory))
				{
					System.IO.Directory.CreateDirectory(nameDirectory);
				}

				nameDirectory = PathNormalize(nameDirectory);
				if(false == nameDirectory.EndsWith("/"))
				{
					nameDirectory += "/";
				}
				return(nameDirectory);
#endif
			}

#if !_FOR_SS6CONVERTER_
			public static string AssetPathGetSelected(string namePath=null)
			{
				string namePathAsset = "";
				if(true == string.IsNullOrEmpty(namePath))
				{	/* Now Selected Path in "Project" */
					Object objectNow = Selection.activeObject;
					if(null == objectNow)
					{	/* No Selected *//* Error */
						namePathAsset = null;
					}
					else
					{	/* Selected */
						namePathAsset = AssetDatabase.GetAssetPath(objectNow);
					}
				}
				else
				{	/* Specified */
					namePathAsset = System.String.Copy(namePath);
				}

				return(namePathAsset);
			}
#endif

			public static bool FileCopyToAsset(string nameAsset, string nameOriginalFileName, bool flagOverCopy)
			{
				System.IO.File.Copy(nameOriginalFileName, nameAsset, flagOverCopy);
				return(true);
			}

			public static bool PathSplit(	out string nameDirectory,
											out string nameFileBody,
											out string nameFileExtention,
											string namePath
										)
			{
				if(true == string.IsNullOrEmpty(namePath))
				{
					nameDirectory = "";
					nameFileBody = "";
					nameFileExtention = "";
					return(false);
				}

				string namePathNormalized = PathNormalize(namePath);
				nameDirectory = System.IO.Path.GetDirectoryName(namePathNormalized) + "/";
				nameFileBody = System.IO.Path.GetFileNameWithoutExtension(namePathNormalized);
				nameFileExtention = System.IO.Path.GetExtension(namePathNormalized);

				return(true);
			}

			public static string PathNormalize(string namePath)
			{
				string namePathNew = namePath.Replace("\\", "/");	/* "\" -> "/" */
				return(namePathNew);
			}

			public static string PathGetAbsolute(string namePath, string nameBase)
			{
				string nameCurrent = System.Environment.CurrentDirectory;
				System.Environment.CurrentDirectory = nameBase;

				string rv = System.IO.Path.GetFullPath(namePath);
				rv = PathNormalize(rv);

				System.Environment.CurrentDirectory = nameCurrent;
				return(rv);
			}

#if !_FOR_SS6CONVERTER_
			public static string PathGetAssetNative(string namePathAsset)
			{
				string namePathNative = string.Copy(NamePathRootNative);
				if(false == string.IsNullOrEmpty(namePathAsset))
				{
					namePathNative += "/" + namePathAsset.Substring(NamePathRootAsset.Length + 1);
					namePathNative = PathNormalize(namePathNative);
				}
				return(namePathNative);
			}
#endif

			public static bool PathCheckRoot(string namePath)
			{	/* MEMO: Create another function separately, since possibility that can not be checked with IsPathRooted. */
				return(System.IO.Path.IsPathRooted(namePath));
			}
#endregion Functions

			/* ----------------------------------------------- Enums & Constants */
#region Enums & Constants
			private readonly static char[] TextSplitFolder = 
			{
				'/',
				'\\',
			};

#if !_FOR_SS6CONVERTER_
			internal readonly static string NamePathRootNative = Application.dataPath;
			internal const string NamePathRootAsset = "Assets";
#endif
#endregion Enums & Constants
		}

		public static partial class Prefs
		{
			/* ----------------------------------------------- Functions */
#region Functions
#if !_FOR_SS6CONVERTER_
			public static void StringSave(string prefsKey, string text)
			{
				string text64 = System.Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(text));
				EditorPrefs.SetString(prefsKey, text64);
			}

			public static string StringLoad(string prefsKey, string textDefault)
			{
				string textDefault64 = System.Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(textDefault));
				string text64 = EditorPrefs.GetString(prefsKey, textDefault64);
				return(System.Text.UTF8Encoding.UTF8.GetString(System.Convert.FromBase64String(text64)));
			}
#endif
#endregion Functions
		}

		public static partial class Text
		{
			/* ----------------------------------------------- Functions */
#region Functions
			public static bool ValueGetBool<_Type>(_Type source)
			{
				return((0 != ValueGetInt(source)) ? true : false);
			}

			public static byte ValueGetByte<_Type>(_Type source)
			{
				return(System.Convert.ToByte(source));
			}

			public static int ValueGetInt<_Type>(_Type source)
			{
				return(System.Convert.ToInt32(source));
			}

			public static uint ValueGetUInt<_Type>(_Type source)
			{
				return(System.Convert.ToUInt32(source));
			}

			public static float ValueGetFloat<_Type>(_Type source)
			{
				return(System.Convert.ToSingle(source));
			}

			internal static double ValueGetDouble<_Type>(_Type source)
			{
				return(System.Convert.ToDouble(source));
			}

			public static int HexToInt(string text)
			{
				return(System.Convert.ToInt32(text, 16));
			}

			public static uint HexToUInt(string text)
			{
				return(System.Convert.ToUInt32(text, 16));
			}

			public static bool TextToBool(string text)
			{
				bool rv = false;
				try
				{
					rv = System.Convert.ToBoolean(text);
				}
				catch(System.FormatException)
				{
					rv = (0 == System.Convert.ToInt32(text));
				}
				return(rv);
			}

			public static bool TextToColor(out float colorA, out float colorR, out float colorG, out float colorB, string text)
			{
				uint ARGB = LibraryEditor_SpriteStudio6.Utility.Text.HexToUInt(text);
				colorA = (float)((ARGB >> 24) & 0xff) / 255.0f;
				colorR = (float)((ARGB >> 16) & 0xff) / 255.0f;
				colorG = (float)((ARGB >> 8) & 0xff) / 255.0f;
				colorB = (float)(ARGB & 0xff) / 255.0f;
				return(true);
			}

			public static int TextToVersion(string text)
			{	/* MEMO: Text = "Major:1"."Minor:2"."Revison:2" */
				string[] item = text.Split('.');
				if (3 != item.Length)
				{
					return(-1);
				}

				int versionMajor = HexToInt(item[0]);
				int versionMinor = HexToInt(item[1]);
				int revision = HexToInt(item[2]);
				return((versionMajor << 16) | (versionMinor << 8) | revision);
			}

			public static string DataNameGetFromPath(	string namePath,
														bool flagRuleOld = false
													)
			{
				string rv = "";
				if(true == string.IsNullOrEmpty(namePath))
				{
					return("");
				}

				string nameNewDirectory = "";
				string nameNewFileBody = "";
				string nameNewFileExtention = "";
				LibraryEditor_SpriteStudio6.Utility.File.PathSplit(out nameNewDirectory, out nameNewFileBody, out nameNewFileExtention, namePath);
				if((true == string.IsNullOrEmpty(nameNewDirectory)) || (true == flagRuleOld))
				{
					rv = nameNewFileBody;
				}
				else
				{
					rv = nameNewDirectory + "/" + nameNewFileBody;
				}
				rv = rv.Replace("\\", "/");	/* "\" -> "/" */

				rv = rv.Replace("../", "_");	/* "../" -> "_" */
				rv = rv.Replace("/", "_");	/* "/" -> "_" */
				return(rv);
			}

			public static string NameNormalize(string name)
			{
				if(true == string.IsNullOrEmpty(name))
				{
					return("");
				}

				string rv = string.Copy(name);
				rv = rv.Replace(":", "_");	/* "/" -> "_" */
				rv = rv.Replace("\\", "/");	/* "\" -> "/" */
				rv = rv.Replace("/", "_");	/* "/" -> "_" */
				rv = rv.Replace(".", "_");	/* "." -> "_" */
				rv = rv.Replace(",", "_");	/* "," -> "_" */
				rv = rv.Replace("\"", "_");	/* """ -> "_" */
				rv = rv.Replace("|", "_");	/* "|" -> "_" */
				rv = rv.Replace("[", "_");	/* "[" -> "_" */
				rv = rv.Replace("]", "_");	/* "]" -> "_" */
				rv = rv.Replace(";", "_");	/* ";" -> "_" */
				rv = rv.Replace("=", "_");	/* "=" -> "_" */
				rv = rv.Replace(" ", "_");	/* " " -> "_" */
				rv = rv.Replace("~", "_");	/* " " -> "_" */
				rv = rv.Replace("$", "_");	/* "$" -> "_" */
				rv = rv.Replace("@", "_");	/* "@" -> "_" */
				rv = rv.Replace("&", "_");	/* "&" -> "_" */
				rv = rv.Replace("\0x00", "_");	/* NULL -> "_" */

				return(rv);
			}
#endregion Functions
		}

		public static partial class XML
		{
			/* ----------------------------------------------- Functions */
#region Functions
			public static System.Xml.XmlNodeList ListGetNode(System.Xml.XmlNode node, string namePath, System.Xml.XmlNamespaceManager manager)
			{
				return(node.SelectNodes(namePath, manager));
			}

			public static System.Xml.XmlNode NodeGet(System.Xml.XmlNode node, string namePath, System.Xml.XmlNamespaceManager manager)
			{
				return(node.SelectSingleNode(namePath, manager));
			}

			public static string TextGetNode(System.Xml.XmlNode node, string namePath, System.Xml.XmlNamespaceManager manager)
			{
				System.Xml.XmlNode nodeNow = NodeGet(node, namePath, manager);
				return((null != nodeNow) ? nodeNow.InnerText : null);
			}

			public static int VersionGet(System.Xml.XmlNode nodeRoot, string nameTag, int errorValue, bool flagMaskRevision)
			{
				System.Xml.XmlAttributeCollection attributeNodeRoot = nodeRoot.Attributes;
				if(nameTag != nodeRoot.Name)
				{
					return(errorValue);
				}

				System.Xml.XmlNode nodeVersion = attributeNodeRoot["version"];
				string versionText = nodeVersion.Value;
				int version = LibraryEditor_SpriteStudio6.Utility.Text.TextToVersion(versionText);
				if(-1 == version)
				{
					return(errorValue);
				}

				if(true == flagMaskRevision)
				{
					version &= ~0x000000ff;
				}
				return(version);
			}
#endregion Functions
		}

		public static partial class ExternalText
		{
			/* ----------------------------------------------- Functions */
#region Functions
			public static KindType TypeGetLine(out string textValid, string text)
			{
				textValid = TextTrim(text);
				if(true == string.IsNullOrEmpty(textValid))
				{	/* Space */
					goto TypeGetLine_EndIgnore;
				}

				char prefixLine = textValid[0];
				switch(prefixLine)
				{
					case PrefixChangeCommand:
						return(KindType.COMMAND);

					case PrefixRemarks:
						goto TypeGetLine_EndIgnore;

					default:
						break;
				}

				return(KindType.NORMAL);

			TypeGetLine_EndIgnore:;
				textValid = "";
				return(KindType.IGNORE);
			}

			public static string LineEncodeIgnore(string text)
			{
				return(PrefixRemarks + " " + text);
			}

			public static string LineEncodeCommand(params string[] textArgument)
			{
				string text = "";
				int count = textArgument.Length;
				bool flagSeparator = false;
				for(int i=0; i<count; i++)
				{
					if(0 == i)
					{
						text += PrefixChangeCommand;
					}
					text += " " + textArgument[i];
					if((count - 1) > i)
					{
						text += " ";
						text += (false == flagSeparator) ? SeparatorCommand[(int)KindSeparator.COMMAND] : SeparatorCommand[(int)KindSeparator.ARGUMENT];

						flagSeparator = true;
					}
				}
				return(text);
			}

			public static string[] LineDecodeCommand(string textLine)
			{
				string[] textArgument = textLine.Split(SeparatorCommand);
				if(null != textArgument)
				{
					int count = textArgument.Length;
					int countValid = 0;
					for(int i=0; i<count; i++)
					{
						textArgument[i] = textArgument[i].Trim(PrefixChangeCommand);
						textArgument[i] = TextTrim(textArgument[i]);
						if(true == string.IsNullOrEmpty(textArgument[i]))
						{	/* Empty */
							textArgument[i] = null;
						}
						else
						{	/* Exist */
							countValid++;
						}
					}
					if(0 >= countValid)
					{
						return(null);
					}

					string[] textArgumentValid = new string[countValid];
					countValid = 0;
					for(int i=0; i<count; i++)
					{
						if(null != textArgument[i])
						{
							textArgumentValid[countValid] = textArgument[i];
							countValid++;
						}
					}
					textArgument = textArgumentValid;
				}
				return(textArgument);
			}

			public static string TextTrim(string text)
			{
				return(text.Trim(IgnoreText));
			}

			public static string BoolEncode(bool value)
			{
				return((true == value) ? ArgumentBoolTrue : ArgumentBoolFalse);
			}

			public static bool BoolDecode(string text)
			{
				switch(text)
				{
					case ArgumentBoolTrue:
						return(true);

					case ArgumentBoolFalse:
						return(false);

					default:
						break;
				}
				return(false);
			}

			public static string IntEncode(int value)
			{
				return(value.ToString());
			}

			public static int IntDecode(string text)
			{
				int value;
				if(false == int.TryParse(text, out value))
				{
					value = 0;
				}
				return(value);
			}

			public static string FloatEncode(float value)
			{
				return(value.ToString());
			}

			public static float FloatDecode(string text)
			{
				float value;
				if(false == float.TryParse(text, out value))
				{
					value = 0.0f;
				}
				return(value);
			}
#endregion Functions

			/* ----------------------------------------------- Enums & Constants */
#region Enums & Constants
			public enum KindType
			{
				NORMAL = 0,
				IGNORE,
				COMMAND,
			}
			public enum KindSeparator
			{
				COMMAND = 0,
				ARGUMENT,
			}

			private const char PrefixChangeCommand = '?';
			private const char PrefixRemarks = '*';
			private readonly static char[] SeparatorCommand =
			{	/* [KindSeparator] */
				'>',	/* between Command and 1st-argument */
				',',	/* between arguments */
			};
			private readonly static char[] IgnoreText =
			{
				' ',
				'\t',
			};

			private const string ArgumentBoolTrue = "true";
			private const string ArgumentBoolFalse = "false";
#endregion Enums & Constants
		}

		public static partial class Log
		{
			/* ----------------------------------------------- Variables & Properties */
#region Variables & Properties
			public static System.IO.StreamWriter StreamExternal = null;
#endregion Variables & Properties

			/* ----------------------------------------------- Functions */
#region Functions
			public static void Error(string message, bool flagExternalOnly=false, bool flagIndentExternal=true)
			{
#if !_FOR_SS6CONVERTER_
				string text = "SS6PU Error: " + message;
				if(false == flagExternalOnly)
				{
					Debug.LogError(text);
				}
				if(null != StreamExternal)
				{
					if(true == flagIndentExternal)
					{
						text = "\t" + text;
					}
					StreamExternal.WriteLine(text);
				}
#endif
			}

			public static void Warning(string message, bool flagExternalOnly=false, bool flagIndentExternal=true)
			{
#if !_FOR_SS6CONVERTER_
				string text = "SS6PU Warning: " + message;
				if(false == flagExternalOnly)
				{
					Debug.LogWarning(text);
				}
				if(null != StreamExternal)
				{
					if(true == flagIndentExternal)
					{
						text = "\t" + text;
					}
					StreamExternal.WriteLine(text);
				}
#endif
			}

			public static void Message(string message, bool flagExternalOnly=false, bool flagIndentExternal=true)
			{
#if !_FOR_SS6CONVERTER_
				string text = "SS6PU-Message: " + message;
				if(false == flagExternalOnly)
				{
					Debug.Log(text);
				}
				if(null != StreamExternal)
				{
					if(true == flagIndentExternal)
					{
						text = "\t" + text;
					}
					StreamExternal.WriteLine(text);
				}
#endif
			}
#endregion Functions
		}
#if !_FOR_SS6CONVERTER_
		public static partial class Inspector
		{
			/* ----------------------------------------------- Functions */
#region Functions
			public static void TableMaterialAnimation(Material[] tableMaterial, SerializedProperty propertyTableMaterial, int levelIndent)
			{
				if(null == tableMaterial)
				{
					EditorGUI.indentLevel = levelIndent;
					EditorGUILayout.LabelField("(Data Missing)");
				}
				else
				{
					EditorGUI.indentLevel = levelIndent;

					int countShader = (int)(Library_SpriteStudio6.KindOperationBlend.TERMINATOR_TABLEMATERIAL);
					int countBlock = (int)Library_SpriteStudio6.KindMasking.TERMINATOR * countShader;
					int indexMaterialBlock;
					int indexMaterial;
					SerializedProperty propertyMaterial;

					int countTexture = Script_SpriteStudio6_Root.Material.CountGetTexture(tableMaterial);
					for(int i=0; i<countTexture; i++)
					{
						indexMaterialBlock = i * countBlock;
						EditorGUILayout.LabelField(	"Material No ["
													+ indexMaterialBlock.ToString()
													+ "-"
													+ (indexMaterialBlock + (countBlock - 1)).ToString()
													+ "]: Texture-"
													+ i.ToString()
												);

						EditorGUI.indentLevel = levelIndent + 1;
						for(int j=0; j<(int)Library_SpriteStudio6.KindMasking.TERMINATOR; j++)
						{
							for(int k=0; k<countShader; k++)
							{
								indexMaterial = indexMaterialBlock + (j * countShader) + k;
								propertyMaterial = propertyTableMaterial.GetArrayElementAtIndex(indexMaterial);
								propertyMaterial.objectReferenceValue = (Material)(EditorGUILayout.ObjectField(	NameItemTableMaterialAnimation[j][k],
																												propertyMaterial.objectReferenceValue,
																												typeof(Material),
																												false
																											)
																		);
							}
							EditorGUILayout.Space();
						}
						EditorGUI.indentLevel = levelIndent;
					}
				}
			}

			public static void TableMaterialEffect(Material[] tableMaterial, SerializedProperty propertyTableMaterial, int levelIndent)
			{
				if(null == tableMaterial)
				{
					EditorGUI.indentLevel = levelIndent;
					EditorGUILayout.LabelField("(Data Missing)");
				}
				else
				{
					EditorGUI.indentLevel = levelIndent;

					int countShader = (int)(Library_SpriteStudio6.KindOperationBlendEffect.TERMINATOR_TABLEMATERIAL);
					int countBlock = (int)Library_SpriteStudio6.KindMasking.TERMINATOR * countShader;
					int indexMaterialBlock;
					int indexMaterial;
					SerializedProperty propertyMaterial;

					int countTexture = Script_SpriteStudio6_RootEffect.Material.CountGetTexture(tableMaterial);
					for(int i=0; i<countTexture; i++)
					{
						indexMaterialBlock = i * countBlock;
						EditorGUILayout.LabelField(	"Material No ["
													+ indexMaterialBlock.ToString()
													+ "-"
													+ (indexMaterialBlock + (countBlock - 1)).ToString()
													+ "]: Texture-"
													+ i.ToString()
												);

						EditorGUI.indentLevel = levelIndent + 1;
						for(int j=0; j<(int)Library_SpriteStudio6.KindOperationBlendEffect.TERMINATOR; j++)
						{
							for(int k=0; k<countShader; k++)
							{
								indexMaterial = indexMaterialBlock + (j * countShader) + k;
								propertyMaterial = propertyTableMaterial.GetArrayElementAtIndex(indexMaterial);
								propertyMaterial.objectReferenceValue = (Material)(EditorGUILayout.ObjectField(	NameItemTableMaterialEffect[j][k],
																												propertyMaterial.objectReferenceValue,
																												typeof(Material),
																												false
																											)
																		);
							}
							EditorGUILayout.Space();
						}
						EditorGUI.indentLevel = levelIndent;
					}

				}
			}
#endregion Functions

			/* ----------------------------------------------- Enums & Constants */
#region Enums & Constants
			private readonly static string[][] NameItemTableMaterialAnimation = new string[2][]
			{
				new string[(int)Library_SpriteStudio6.KindOperationBlend.TERMINATOR_TABLEMATERIAL]
				{
					"Mask-Pre",
					"Mask",
					"Mix(T)",
					"Add(T)",
					"Sub(T)",
					"Mul(T)",
					"MulPA(T)",
					"Scr(T)",
					"Exc(T)",
					"Inv(T)",
				},
				new string[(int)Library_SpriteStudio6.KindOperationBlend.TERMINATOR_TABLEMATERIAL]
				{
					"Mask-Pre",
					"Mask",
					"Mix(M)",
					"Add(M)",
					"Sub(M)",
					"Mul(M)",
					"MulPA(M)",
					"Scr(M)",
					"Exc(M)",
					"Inv(M)",
				}
			};

			private readonly static string[][] NameItemTableMaterialEffect = new string[2][]
			{
				new string[(int)Library_SpriteStudio6.KindOperationBlendEffect.TERMINATOR_TABLEMATERIAL]
				{
					"Mix(T)",
					"Add(T)",
				},
				new string[(int)Library_SpriteStudio6.KindOperationBlendEffect.TERMINATOR_TABLEMATERIAL]
				{
					"Mix(M)",
					"Add(M)",
				}
			};
#endregion Enums & Constants
		}
#endif

		public static partial class Miscellaneous
		{
			/* ----------------------------------------------- Functions */
#region Functions
			public static void ProgressBarUpdate(string title, string nameTask, bool flagSwitch, int step, int stepFull)
			{
#if !_FOR_SS6CONVERTER_
				if(false == flagSwitch)
				{
					return;
				}

				if((-1 == step) || (-1 == stepFull))
				{
					EditorUtility.ClearProgressBar();
					return;
				}

				EditorUtility.DisplayProgressBar(title, nameTask, ((float)step / (float)stepFull));
#endif
			}

			public static void GarbageCollect()
			{
#if !_FOR_SS6CONVERTER_
				System.GC.Collect();
				System.GC.WaitForPendingFinalizers();
#endif
				System.GC.Collect();
			}
#endregion Functions
		}
#endregion Classes, Structs & Interfaces
	}
#endregion Classes, Structs & Interfaces
}
