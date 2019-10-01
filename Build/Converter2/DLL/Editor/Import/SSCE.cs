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
		public static partial class SSCE
		{
			/* ----------------------------------------------- Functions */
			#region Functions
			public static Information Parse(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
												string nameFile,
												LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ
											)
			{
				const string messageLogPrefix = "Parse SSCE";
				Information informationSSCE = null;

				/* ".ssce" Load */
				if(false == System.IO.File.Exists(nameFile))
				{
					LogError(messageLogPrefix, "File Not Found", nameFile, informationSSPJ);
					goto Parse_ErrorEnd;
				}
				System.Xml.XmlDocument xmlSSCE = new System.Xml.XmlDocument();
				xmlSSCE.Load(nameFile);

				/* Check Version */
				System.Xml.XmlNode nodeRoot = xmlSSCE.FirstChild;
				nodeRoot = nodeRoot.NextSibling;
				KindVersion version = (KindVersion)(LibraryEditor_SpriteStudio6.Utility.XML.VersionGet(nodeRoot, "SpriteStudioCellMap", (int)KindVersion.ERROR, true));
				switch(version)
				{
					case KindVersion.ERROR:
						LogError(messageLogPrefix, "Version Invalid", nameFile, informationSSPJ);
						goto Parse_ErrorEnd;

					case KindVersion.CODE_000100:
					case KindVersion.CODE_010000:
					case KindVersion.CODE_010001:
						break;

					case KindVersion.CODE_020000:
						break;

					default:
						if(KindVersion.TARGET_EARLIEST > version)
						{
							version = KindVersion.TARGET_EARLIEST;
							if(true == setting.CheckVersion.FlagInvalidSSCE)
							{
								LogWarning(messageLogPrefix, "Version Too Early", nameFile, informationSSPJ);
							}
						}
						else
						{
							version = KindVersion.TARGET_LATEST;
							if(true == setting.CheckVersion.FlagInvalidSSCE)
							{
								LogWarning(messageLogPrefix, "Version Unknown", nameFile, informationSSPJ);
							}
						}
						break;
				}

				/* Create Information */
				informationSSCE = new Information();
				if(null == informationSSCE)
				{
					LogError(messageLogPrefix, "Not Enough Memory", nameFile, informationSSPJ);
					goto Parse_ErrorEnd;
				}
				informationSSCE.CleanUp();
				informationSSCE.Version = version;

				/* Get Base-Directories */
				LibraryEditor_SpriteStudio6.Utility.File.PathSplit(out informationSSCE.NameDirectory, out informationSSCE.NameFileBody, out informationSSCE.NameFileExtension, nameFile);
				informationSSCE.NameDirectory += "/";

				/* Decode Tags */
				System.Xml.NameTable nodeNameSpace = new System.Xml.NameTable();
				System.Xml.XmlNamespaceManager managerNameSpace = new System.Xml.XmlNamespaceManager(nodeNameSpace);

				string valueText = "";

				/* Get Texture Path-Name */
				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeRoot, "imagePath", managerNameSpace);
				string namePathTexture = "";
				if(true == System.IO.Path.IsPathRooted(valueText))
				{
					namePathTexture = string.Copy(valueText);
				}
				else
				{
					namePathTexture = informationSSPJ.PathGetAbsolute(valueText, LibraryEditor_SpriteStudio6.Import.KindFile.TEXTURE);
				}
				informationSSCE.IndexTexture = informationSSPJ.AddTexture(namePathTexture);

				/* Get Texture Pixel-Size */
				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeRoot, "pixelSize", managerNameSpace);
				string[] valueTextSplit = null;
				if(true == string.IsNullOrEmpty(valueText))
				{	/* Get directly from texture */
					informationSSCE.SizePixelX = -1;
					informationSSCE.SizePixelY = -1;
				}
				else
				{	/* Synchronized with Cell size */
					valueTextSplit = valueText.Split(' ');
					informationSSCE.SizePixelX = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueTextSplit[0]);
					informationSSCE.SizePixelY = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueTextSplit[1]);
				}

				/* Get Texture Premultified-Alpha */
				informationSSCE.flagConvertImagePremultipliedAlpha = informationSSPJ.flagConvertImagePremultipliedAlpha;
				informationSSCE.flagBlendImagePremultipliedAlpha = informationSSPJ.flagBlendImagePremultipliedAlpha;

				/* Get Texture Addressing */
				informationSSCE.WrapTexture = informationSSPJ.WrapTexture;
				informationSSCE.FilterTexture = informationSSPJ.FilterTexture;

				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeRoot, "overrideTexSettings", managerNameSpace);
				if(false == string.IsNullOrEmpty(valueText))
				{
					bool valueBool = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetBool(valueText);
					if(true == valueBool)
					{
						/* Get Texture Wrap-Mode */
						valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeRoot, "wrapMode", managerNameSpace);
						switch(valueText)
						{
							case "repeat":
								informationSSCE.WrapTexture = Library_SpriteStudio6.Data.Texture.KindWrap.REPEAT;
								break;

							case "mirror":
#if UNITY_2017_1_OR_NEWER
								informationSSCE.WrapTexture = Library_SpriteStudio6.Data.Texture.KindWrap.MIRROR;
								break;
#else
								/* MEMO: SS6PU is supported with Unity2017 or later, but since Nintendo-Switch's environment is Unity 5.6 at present. */
								LogWarning(messageLogPrefix, "Wrap-Mode \"Mirror\" is not Suppoted. Changed \"Clamp\"", nameFile, informationSSPJ);
								goto case "clamp";
#endif

							case "clamp":
								informationSSCE.WrapTexture = Library_SpriteStudio6.Data.Texture.KindWrap.CLAMP;
								break;

							default:
								LogWarning(messageLogPrefix, "Wrap-Mode \"" + valueText + "\" is not Suppoted. Changed \"Clamp\"", nameFile, informationSSPJ);
								goto case "clamp";
						}

						/* Get Texture Filter-Mode */
						valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeRoot, "filterMode", managerNameSpace);
						switch(valueText)
						{
							case "nearlest":
								informationSSCE.FilterTexture = Library_SpriteStudio6.Data.Texture.KindFilter.NEAREST;
								break;

							case "linear":
								informationSSCE.FilterTexture = Library_SpriteStudio6.Data.Texture.KindFilter.LINEAR;
								break;

							case "bilinear":
								informationSSCE.FilterTexture = Library_SpriteStudio6.Data.Texture.KindFilter.BILINEAR;
								break;

							default:
								goto case "linear";
						}
					}
				}

				/* Get Cells */
				List<Information.Cell> listCell = new List<Information.Cell>();
				if(null == listCell)
				{
					LogError(messageLogPrefix, "Not Enough Memory (CellMap WorkArea)", nameFile, informationSSPJ);
					goto Parse_ErrorEnd;
				}
				listCell.Clear();

				System.Xml.XmlNodeList listNode = LibraryEditor_SpriteStudio6.Utility.XML.ListGetNode(nodeRoot, "cells/cell", managerNameSpace);
				if(null == listNode)
				{
					LogError(messageLogPrefix, "Cells-Node Not Found", nameFile, informationSSPJ);
					goto Parse_ErrorEnd;
				}

				double pivotNormalizeX = 0.0;
				double pivotNormalizeY = 0.0;
				Information.Cell cell = null;
				foreach(System.Xml.XmlNode nodeCell in listNode)
				{
					cell = new Information.Cell();
					if(null == cell)
					{
						LogError(messageLogPrefix, "Not Enough Memory (Cell WorkArea)", nameFile, informationSSPJ);
						goto Parse_ErrorEnd;
					}
					cell.CleanUp();

					valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeCell, "name", managerNameSpace);
					cell.Data.Name = string.Copy(valueText);

					valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeCell, "pos", managerNameSpace);
					valueTextSplit = valueText.Split(' ');
					cell.Data.Rectangle.x = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueTextSplit[0]));
					cell.Data.Rectangle.y = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueTextSplit[1]));

					valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeCell, "size", managerNameSpace);
					valueTextSplit = valueText.Split(' ');
					cell.Data.Rectangle.width = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueTextSplit[0]));
					cell.Data.Rectangle.height = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueTextSplit[1]));

					valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeCell, "pivot", managerNameSpace);
					valueTextSplit = valueText.Split(' ');
					pivotNormalizeX = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[0]);
					pivotNormalizeY = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[1]);
					cell.Data.Pivot.x = (float)((double)cell.Data.Rectangle.width * (pivotNormalizeX + 0.5));
					cell.Data.Pivot.y = (float)((double)cell.Data.Rectangle.height * (-pivotNormalizeY + 0.5));

					valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeCell, "rotated", managerNameSpace);
					cell.Rotate = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);

					cell.Data.Mesh.CleanUp();
					valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeCell, "ismesh", managerNameSpace);
					if(false == string.IsNullOrEmpty(valueText))
					{	/* "ismesh"tag is exist (SS5's SSCE has no "ismesh") */
						if(true == LibraryEditor_SpriteStudio6.Utility.Text.ValueGetBool(valueText))
						{	/* Has Mesh */
							cell.Data.Mesh.TableCoordinate = TableGetPoint2DList(nodeCell, "meshPointList", managerNameSpace);
							cell.Data.Mesh.TableIndexVertex = TableGetTriangleList(nodeCell, "meshTriList", managerNameSpace);
						}
					}

					listCell.Add(cell);
				}
				informationSSCE.TableCell = listCell.ToArray();
				listCell.Clear();

				return(informationSSCE);

			Parse_ErrorEnd:
				if(null != informationSSCE)
				{
					informationSSCE.CleanUp();
				}
				return(null);
			}
			private static Vector2[] TableGetPoint2DList(System.Xml.XmlNode node, string namePath, System.Xml.XmlNamespaceManager manager)
			{
				System.Xml.XmlNodeList listNode = LibraryEditor_SpriteStudio6.Utility.XML.ListGetNode(node, namePath + "/value", manager);
				if(null == listNode)
				{
					return(null);
				}

				int count = listNode.Count;
				Vector2[] tablePointList = new Vector2[count];
				string valueText;
				string[] valueTextSplit;
				for(int i=0; i<count; i++)
				{
					valueText = listNode[i].InnerText;
					if(false == string.IsNullOrEmpty(valueText))
					{
						valueTextSplit = valueText.Split(' ');
						tablePointList[i].x = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueTextSplit[0]);
						tablePointList[i].y = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueTextSplit[1]);
					}
				}

				return(tablePointList);
			}
			private static int[] TableGetTriangleList(System.Xml.XmlNode node, string namePath, System.Xml.XmlNamespaceManager manager)
			{
				System.Xml.XmlNodeList listNode = LibraryEditor_SpriteStudio6.Utility.XML.ListGetNode(node, namePath + "/value", manager);
				if(null == listNode)
				{
					return(null);
				}

				int count = listNode.Count;
				int[] tableIndexList = new int[count * (int)Library_SpriteStudio6.Data.CellMap.Cell.DataMesh.Constants.COUNT_VERTEX_SURFACE];
				string valueText;
				string[] valueTextSplit;
				for(int i=0; i<count; i++)
				{
					valueText = listNode[i].InnerText;
					if(false == string.IsNullOrEmpty(valueText))
					{
						valueTextSplit = valueText.Split(' ');
						tableIndexList[(i * 3)] = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueTextSplit[0]);
						tableIndexList[(i * 3) + 1] = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueTextSplit[1]);
						tableIndexList[(i * 3) + 2] = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueTextSplit[2]);
					}
				}

				return(tableIndexList);
			}

#if !_FOR_SS6CONVERTER_
			public static bool AssetNameDecideTexture(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
														LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
														Information.Texture informationTexture,
														string nameOutputAssetFolderBase,
														Texture2D textureOverride
													)
			{	/* MEMO: In each import mode, texture is shared. */
				if(null != textureOverride)
				{	/* Specified */
					informationTexture.PrefabTexture.TableName[0] = AssetDatabase.GetAssetPath(textureOverride);
					informationTexture.PrefabTexture.TableData[0] = textureOverride;
				}
				else
				{	/* Default */
					informationTexture.PrefabTexture.TableName[0] = setting.RuleNameAssetFolder.NameGetAssetFolder(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.TEXTURE, nameOutputAssetFolderBase)
																	+ setting.RuleNameAsset.NameGetAsset(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.TEXTURE, informationTexture.NameFileBody, informationSSPJ.NameFileBody)
																	+ informationTexture.NameFileExtension;
					/* MEMO: Can not detect Platform-Dependent Textures (such as DDS and PVR). */
					informationTexture.PrefabTexture.TableData[0] = AssetDatabase.LoadAssetAtPath<Texture2D>(informationTexture.PrefabTexture.TableName[0]);
				}

				return(true);

//			AssetNameDecideTexture_ErrorEnd:;
//				return(false);
			}

			public static bool AssetCreateTexture(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
													LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
													Information.Texture informationTexture
												)
			{	/* MEMO: In each import mode, texture is shared. */
//				const string messageLogPrefix = "Create Asset(Texture)";

				/* Copy into Asset */
				string namePathAssetNative = LibraryEditor_SpriteStudio6.Utility.File.PathGetAssetNative(informationTexture.PrefabTexture.TableName[0]);
				LibraryEditor_SpriteStudio6.Utility.File.FileCopyToAsset(	namePathAssetNative,
																			informationTexture.FileNameGetFullPath(),
																			true
																		);

				/* Set Texture-Importer */
				if(null == informationTexture.PrefabTexture.TableData[0])
				{
					AssetDatabase.ImportAsset(informationTexture.PrefabTexture.TableName[0]);
					TextureImporter importer = TextureImporter.GetAtPath(informationTexture.PrefabTexture.TableName[0]) as TextureImporter;
					if(null != importer)
					{
						importer.anisoLevel = 1;
						importer.borderMipmap = false;
						importer.convertToNormalmap = false;
						importer.fadeout = false;
						switch(informationTexture.Filter)
						{
							case Library_SpriteStudio6.Data.Texture.KindFilter.NEAREST:
								importer.filterMode = FilterMode.Point;
								break;

							case Library_SpriteStudio6.Data.Texture.KindFilter.LINEAR:
								importer.filterMode = FilterMode.Bilinear;
								break;

							default:
								/* MEMO: Errors and warnings have already been done and values have been revised. Therefore, will not come here. */
								goto AssetCreateTexture_ErrorEnd;
						}

						/* MEMO: For 5.5.0beta & later, with "Sprite" to avoid unnecessary interpolation. */
						/* MEMO: Enable read till complete SSCE precalculation. */
						importer.textureShape = TextureImporterShape.Texture2D;
						importer.isReadable = (true == setting.Basic.FlagTextureReadable) ? true : false;
						importer.mipmapEnabled = false;
						importer.maxTextureSize = 4096;
						importer.alphaSource = TextureImporterAlphaSource.FromInput;
						importer.alphaIsTransparency = true;
						importer.npotScale = TextureImporterNPOTScale.None;
						importer.textureType = TextureImporterType.Sprite;
						importer.spriteImportMode = SpriteImportMode.Multiple;
						importer.spritePackingTag = LibraryEditor_SpriteStudio6.Import.NameTagSpritePackerTexture;

						switch(informationTexture.Wrap)
						{
							case Library_SpriteStudio6.Data.Texture.KindWrap.REPEAT:
								importer.wrapMode = TextureWrapMode.Repeat;
								break;

							case Library_SpriteStudio6.Data.Texture.KindWrap.CLAMP:
								importer.wrapMode = TextureWrapMode.Clamp;
								break;

							case Library_SpriteStudio6.Data.Texture.KindWrap.MIRROR:
#if UNITY_2017_1_OR_NEWER
								importer.wrapMode = TextureWrapMode.Mirror;
#else
								/* MEMO: SS6PU is supported with Unity2017 or later, but since Nintendo-Switch's environment is Unity 5.6 at present. */
								importer.wrapMode = TextureWrapMode.Clamp;
#endif
								break;

							default:
								/* MEMO: Errors and warnings have already been done and values have been revised. Therefore, will not come here. */
								goto AssetCreateTexture_ErrorEnd;
						}
						AssetDatabase.ImportAsset(informationTexture.PrefabTexture.TableName[0], ImportAssetOptions.ForceUpdate);
					}
				}
				AssetDatabase.SaveAssets();

				informationTexture.PrefabTexture.TableData[0] = AssetDatabase.LoadAssetAtPath(informationTexture.PrefabTexture.TableName[0], typeof(Texture2D)) as Texture2D;
				if((0 >= informationTexture.SizeX) || (0 >= informationTexture.SizeY))
				{	/* Only when texture size can not be get from SSCE */
					informationTexture.SizeX = informationTexture.PrefabTexture.TableData[0].width;
					informationTexture.SizeY = informationTexture.PrefabTexture.TableData[0].height;
				}

				return(true);

			AssetCreateTexture_ErrorEnd:;
				return(false);
			}

			public static bool CellTrimTransparentPixel(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
															LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
															LibraryEditor_SpriteStudio6.Import.SSCE.Information informationSSCE
													)
			{
//				const string messageLogPrefix = "Convert Trimming-Pixel (CellMap)";

				/* MEMO: Necessary to execute ...              */
				/*       after "SSCE.AssetCreateTexture"       */ 
				/*       before "SSCE.ModeXXXX.ConvertCellMap" */
				Information.Cell[] tableCell = informationSSCE.TableCell;
				if(null == tableCell)
				{
					return(true);	/* false */
				}

				/* Get Texture & Set "Read/Write Enable" */
				int indexTexture = informationSSCE.IndexTexture;
				if(0 > indexTexture)
				{
					return(true);	/* false */
				}

				string nameAssetTexture = informationSSPJ.TableInformationTexture[indexTexture].PrefabTexture.TableName[0];
				TextureImporter importerTexture = TextureImporter.GetAtPath(nameAssetTexture) as TextureImporter;
				if(null == importerTexture)
				{
					return(true);	/* false */
				}
				importerTexture.isReadable = true;
				AssetDatabase.ImportAsset(nameAssetTexture, ImportAssetOptions.ForceUpdate);

				Texture2D texture = informationSSPJ.TableInformationTexture[indexTexture].PrefabTexture.TableData[0];
				if(null == texture)
				{
					return(true);	/* false */
				}
				int sizeXTexture = texture.width;
				int sizeYTexture = texture.height;

				/* Get new rectangle & pivot */
				Rect rectangleOld;
				Rect rectangleNew;
				Vector2 pivotOld;
				Vector2 pivotNew;
				Vector2 shiftPosition;
				bool flagEmpty;
				bool flagTransparentLine;
				Color colorPixel;
				int positionX;
				int positionY;
				int countCell = tableCell.Length;
				for(int i=0; i<countCell; i++)
				{
					/* MEMO: Do not trim Cells that have mesh.                               */
					/*       (Since vertices are bound, no way to follow when changing Cell) */
					if(true == tableCell[i].Data.IsMesh)
					{
						continue;
					}

					rectangleOld = tableCell[i].Data.Rectangle;
					rectangleNew = rectangleOld;
					pivotOld = tableCell[i].Data.Pivot;
					flagEmpty = false;

					/* Get Left */
					rectangleNew.xMin = rectangleOld.xMin;
					for(int j=0; j<rectangleOld.width; j++)
					{
						positionX = (int)rectangleOld.xMin + j;
						flagTransparentLine = true;
						for(int k=0; k<rectangleOld.height; k++)
						{
							positionY = (int)rectangleOld.yMin + k;
							colorPixel = texture.GetPixel(positionX, (sizeYTexture - 1) - positionY);
							if(0.0f < colorPixel.a)
							{
								flagTransparentLine = false;
								flagEmpty = false;
								break;
							}
						}
						if(false == flagTransparentLine)
						{
							break;
						}
						/* MEMO: Set the position of the transparent line found last for securing 1 pixel margin after trimming. */
						rectangleNew.xMin = (float)positionX;
					}
					if(true == flagEmpty)
					{	/* All pixle is transparent */
						rectangleNew = rectangleOld;
					}
					else
					{
						/* Get Right */
						rectangleNew.xMax = rectangleOld.xMax;
						for(int j=0; j<rectangleOld.width; j++)
						{
							positionX = ((int)rectangleOld.xMax - 1) - j;
							flagTransparentLine = true;
							for(int k=0; k<rectangleOld.height; k++)
							{
								positionY = (int)rectangleOld.yMin + k;
								colorPixel = texture.GetPixel(positionX, (sizeYTexture - 1) - positionY);
								if(0.0f < colorPixel.a)
								{
									flagTransparentLine = false;
									break;
								}
							}
							if(false == flagTransparentLine)
							{
								break;
							}
							/* MEMO: Set the position of the transparent line found last for securing 1 pixel margin after trimming. */
							rectangleNew.xMax = (float)(positionX + 1);
						}

						/* Get Up */
						rectangleNew.yMin = rectangleOld.yMin;
						for(int j=0; j<rectangleOld.height; j++)
						{
							positionY = (int)rectangleOld.yMin + j;
							flagTransparentLine = true;
							for(int k=0; k<rectangleOld.width; k++)
							{
								positionX = (int)rectangleOld.xMin + k;
								colorPixel = texture.GetPixel(positionX, (sizeYTexture - 1) - positionY);
								if(0.0f < colorPixel.a)
								{
									flagTransparentLine = false;
									break;
								}
							}
							if(false == flagTransparentLine)
							{
								break;
							}
							/* MEMO: Set the position of the transparent line found last for securing 1 pixel margin after trimming. */
							rectangleNew.yMin = (float)positionY;
						}

						/* Get Bottom */
						rectangleNew.yMax = rectangleOld.yMax;
						for(int j=0; j<rectangleOld.height; j++)
						{
							positionY = ((int)rectangleOld.yMax - 1) - j;
							flagTransparentLine = true;
							for(int k=0; k<rectangleOld.width; k++)
							{
								positionX = (int)rectangleOld.xMin + k;
								colorPixel = texture.GetPixel(positionX, (sizeYTexture - 1) - positionY);
								if(0.0f < colorPixel.a)
								{
									flagTransparentLine = false;
									break;
								}
							}
							if(false == flagTransparentLine)
							{
								break;
							}
							/* MEMO: Set the position of the transparent line found last for securing 1 pixel margin after trimming. */
							rectangleNew.yMax = (float)(positionY + 1);
						}
					}

					shiftPosition = rectangleNew.position - rectangleOld.position;
					pivotNew.x = pivotOld.x - shiftPosition.x;
					pivotNew.y = pivotOld.y - shiftPosition.y;
					tableCell[i].Data.Rectangle = rectangleNew;
					tableCell[i].Data.Pivot = pivotNew;
				}

				bool flagEnableRead = false;
				if(true == setting.Basic.FlagTextureReadable)
				{
					flagEnableRead = true;
				}
				importerTexture.isReadable = flagEnableRead;
				AssetDatabase.ImportAsset(nameAssetTexture, ImportAssetOptions.ForceUpdate);

				return(true);

//			CellTrimTransparentPixelt_ErrorEnd:;
//				return(false);
			}
#endif

			private static void LogError(string messagePrefix, string message, string nameFile, LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ)
			{
				LibraryEditor_SpriteStudio6.Utility.Log.Error(	messagePrefix
																+ ": " + message
																+ " [" + nameFile + "]"
																+ " in <" + informationSSPJ.FileNameGetFullPath() + ">"
															);
			}

			private static void LogWarning(string messagePrefix, string message, string nameFile, LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ)
			{
				LibraryEditor_SpriteStudio6.Utility.Log.Warning(	messagePrefix
																	+ ": " + message
																	+ " [" + nameFile + "]"
																	+ " in \"" + informationSSPJ.FileNameGetFullPath() + "\""
																);
			}
			#endregion Functions

			/* ----------------------------------------------- Enums & Constants */
			#region Enums & Constants
			public enum KindVersion
			{
				ERROR = 0x00000000,
				CODE_000100 = 0x00000100,	/* under-development SS5 */
				CODE_010000 = 0x00010000,	/* after SS5.0.0 */
				CODE_010001 = 0x00010200,	/* after SS5.5.0 beta-3 */
				CODE_020000 = 0x00020000,	/* after SS6.0.0 */

				TARGET_EARLIEST = CODE_000100,
				TARGET_LATEST = CODE_020000
			}

			private const string ExtentionFile = ".ssce";
			#endregion Enums & Constants

			/* ----------------------------------------------- Classes, Structs & Interfaces */
			#region Classes, Structs & Interfaces
			public class Information
			{
				/* ----------------------------------------------- Variables & Properties */
				#region Variables & Properties
				public LibraryEditor_SpriteStudio6.Import.SSCE.KindVersion Version;
				public Library_SpriteStudio6.Data.CellMap Data;

				public string NameDirectory;
				public string NameFileBody;
				public string NameFileExtension;

				public int IndexTexture;
				public int SizePixelX;	/* Temporary */
				public int SizePixelY;	/* Temporary */
				public Library_SpriteStudio6.Data.Texture.KindWrap WrapTexture;
				public Library_SpriteStudio6.Data.Texture.KindFilter FilterTexture;
				public bool flagConvertImagePremultipliedAlpha;
				public bool flagBlendImagePremultipliedAlpha;

				public Cell[] TableCell;

				public string[] TableNameSpriteUnityNative;	/* Temporary */
				#endregion Variables & Properties

				/* ----------------------------------------------- Functions */
				#region Functions
				public void CleanUp()
				{
					Version = LibraryEditor_SpriteStudio6.Import.SSCE.KindVersion.ERROR;
					Data = new Library_SpriteStudio6.Data.CellMap();
					Data.CleanUp();

					NameDirectory = "";
					NameFileBody = "";
					NameFileExtension = "";

					IndexTexture = -1;
					WrapTexture = (Library_SpriteStudio6.Data.Texture.KindWrap)(-1);
					FilterTexture = (Library_SpriteStudio6.Data.Texture.KindFilter)(-1);
					flagConvertImagePremultipliedAlpha = false;
					flagBlendImagePremultipliedAlpha = false;

					TableCell = null;

					TableNameSpriteUnityNative = null;
				}

				public string FileNameGetFullPath()
				{
					return(NameDirectory + NameFileBody + NameFileExtension);
				}

				public int IndexGetCell(string name)
				{
					if(null != TableCell)
					{
						int count = TableCell.Length;
						for(int i=0; i<count; i++)
						{
							if(name == TableCell[i].Data.Name)
							{
								return(i);
							}
						}
					}
					else
					{
						int count = Data.TableCell.Length;
						for(int i=0; i<count; i++)
						{
							if(name == Data.TableCell[i].Name)
							{
								return(i);
							}
						}
					}
					return(-1);
				}
				#endregion Functions

				/* ----------------------------------------------- Classes, Structs & Interfaces */
				#region Classes, Structs & Interfaces
				public class Cell
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public Library_SpriteStudio6.Data.CellMap.Cell Data;
					public float Rotate;
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public void CleanUp()
					{
						Data.CleanUp();
						Rotate = 0.0f;
					}
					#endregion Functions
				}

				public class Texture
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public string Name;

					public string NameDirectory;
					public string NameFileBody;
					public string NameFileExtension;

					public Library_SpriteStudio6.Data.Texture.KindWrap Wrap;
					public Library_SpriteStudio6.Data.Texture.KindFilter Filter;

					public int SizeX;
					public int SizeY;

#if !_FOR_SS6CONVERTER_
					public LibraryEditor_SpriteStudio6.Import.Assets<Texture2D> PrefabTexture;
					public LibraryEditor_SpriteStudio6.Import.Assets<Material> MaterialAnimationSS6PU;
					public LibraryEditor_SpriteStudio6.Import.Assets<Material> MaterialEffectSS6PU;
					public LibraryEditor_SpriteStudio6.Import.Assets<Material> MaterialAnimationUnityNative;

					public List<SpriteMetaData> ListSpriteMetaDataUnityNative;	/* Temporary */
					public List<Sprite> ListSpriteUnityNative;
#endif
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public void CleanUp()
					{
						Name = "";

						NameDirectory = "";
						NameFileBody = "";
						NameFileExtension = "";

						Wrap = (Library_SpriteStudio6.Data.Texture.KindWrap)(-1);
						Filter = (Library_SpriteStudio6.Data.Texture.KindFilter)(-1);

						SizeX = -1;
						SizeY = -1;

#if !_FOR_SS6CONVERTER_
						PrefabTexture.CleanUp();
						PrefabTexture.BootUp(1);	/* Always 1 */

						int countMaterial;
						MaterialAnimationSS6PU.CleanUp();
						countMaterial = Script_SpriteStudio6_Root.Material.CountGetTable(1);
						MaterialAnimationSS6PU.BootUp(countMaterial);
						MaterialEffectSS6PU.CleanUp();
						countMaterial = Script_SpriteStudio6_RootEffect.Material.CountGetTable(1);
						MaterialEffectSS6PU.BootUp(countMaterial);

						/* MEMO: "Unity-Native" mode does not have "MASK_PRE" and "MASK". */
						MaterialAnimationUnityNative.CleanUp();
						countMaterial = (int)Library_SpriteStudio6.KindOperationBlend.TERMINATOR;
						MaterialAnimationUnityNative.BootUp(countMaterial);

						ListSpriteMetaDataUnityNative = null;
						ListSpriteUnityNative = null;
#endif
					}

					public string FileNameGetFullPath()
					{
						return(NameDirectory + NameFileBody + NameFileExtension);
					}
					#endregion Functions
				}
				#endregion Classes, Structs & Interfaces
			}

			public static partial class ModeSS6PU
			{
				/* MEMO: Originally functions that should be defined in each information class. */
				/*       However, confusion tends to occur with mode increases.                 */
				/*       ... Compromised way.                                                   */

				/* ----------------------------------------------- Functions */
				#region Functions
#if !_FOR_SS6CONVERTER_
				public static bool AssetNameDecideMaterialAnimation(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
																		LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
																		Information.Texture informationTexture,
																		string nameOutputAssetFolderBase,
																		Library_SpriteStudio6.KindOperationBlend operationTarget,
																		Library_SpriteStudio6.KindMasking masking,
																		Material materialOverride,
																		Script_SpriteStudio6_DataAnimation.KindVersion versionOverride
																	)
				{
					int indexTable = Script_SpriteStudio6_Root.Material.IndexGetTable(0, operationTarget, masking);
					string namePathDefault = setting.RuleNameAssetFolder.NameGetAssetFolder(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.MATERIAL_ANIMATION_SS6PU, nameOutputAssetFolderBase)
												+ setting.RuleNameAsset.NameGetAsset(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.MATERIAL_ANIMATION_SS6PU, informationTexture.NameFileBody, informationSSPJ.NameFileBody)
												+ "_" + NameKindMasking[(int)masking]
												+ "_" + operationTarget.ToString()
												+ LibraryEditor_SpriteStudio6.Import.NameExtentionMaterial;

					if(null != materialOverride)
					{	/* Specified */
						informationTexture.MaterialAnimationSS6PU.TableName[indexTable] = AssetDatabase.GetAssetPath(materialOverride);
					}
					else
					{	/* Default */
						informationTexture.MaterialAnimationSS6PU.TableName[indexTable] = namePathDefault;
						materialOverride = AssetDatabase.LoadAssetAtPath<Material>(informationTexture.MaterialAnimationSS6PU.TableName[indexTable]);
					}
					informationTexture.MaterialAnimationSS6PU.TableData[indexTable] = materialOverride;
					if(null == materialOverride)
					{
						versionOverride = Script_SpriteStudio6_DataAnimation.KindVersion.SS5PU;
					}
					informationTexture.MaterialAnimationSS6PU.Version[indexTable] = (int)versionOverride;

					/* Version Patch */
					/* MEMO: At updating old version data, Special processing if can not solve by simply overwriting. */
					/* MEMO: Here, asset paths and references only.                                    */
					/*       Materials to be created will be solved in "AssetCreateMaterialAnimation". */
					if(Script_SpriteStudio6_DataAnimation.KindVersion.SS5PU == versionOverride)
					{
						goto AssetNameDecideMaterialAnimation_End;
					}

					if(Script_SpriteStudio6_DataAnimation.KindVersion.CODE_010005 > versionOverride)
					{	/* Before 1.0.27 */
						/* MEMO: "Mask" parts' materials are separated for masking and nonmasking. */
						if((Library_SpriteStudio6.KindOperationBlend.MASK_PRE == operationTarget) || (Library_SpriteStudio6.KindOperationBlend.MASK == operationTarget))
						{
							switch(masking)
							{
								case Library_SpriteStudio6.KindMasking.THROUGH:
									break;

								case Library_SpriteStudio6.KindMasking.MASK:
									/* MEMO: Forcibly make a new. */
									informationTexture.MaterialAnimationSS6PU.TableName[indexTable] = namePathDefault;
									informationTexture.MaterialAnimationSS6PU.TableData[indexTable] = null;
									informationTexture.MaterialAnimationSS6PU.Version[indexTable] = (int)Script_SpriteStudio6_DataAnimation.KindVersion.SS5PU;;
									break;
							}
						}
						goto AssetNameDecideMaterialAnimation_End;
					}

				AssetNameDecideMaterialAnimation_End:;
					return(true);

//				AssetNameDecideMaterialAnimation_ErrorEnd:;
//					return(false);
				}

				public static bool AssetNameDecideMaterialEffect(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
																	LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
																	Information.Texture informationTexture,
																	string nameOutputAssetFolderBase,
																	Library_SpriteStudio6.KindOperationBlendEffect operationTarget,
																	Library_SpriteStudio6.KindMasking masking,
																	Material materialOverride,
																	Script_SpriteStudio6_DataEffect.KindVersion versionOverride
																)
				{
					int indexTable = Script_SpriteStudio6_RootEffect.Material.IndexGetTable(0, operationTarget, masking);
					string namePathDefault = setting.RuleNameAssetFolder.NameGetAssetFolder(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.MATERIAL_EFFECT_SS6PU, nameOutputAssetFolderBase)
												+ setting.RuleNameAsset.NameGetAsset(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.MATERIAL_EFFECT_SS6PU, informationTexture.NameFileBody, informationSSPJ.NameFileBody)
												+ "_" + NameKindMasking[(int)masking]
												+ "_" + operationTarget.ToString()
												+ LibraryEditor_SpriteStudio6.Import.NameExtentionMaterial;

					if(null != materialOverride)
					{	/* Specified */
						informationTexture.MaterialEffectSS6PU.TableName[indexTable] = AssetDatabase.GetAssetPath(materialOverride);
					}
					else
					{	/* Default */
						informationTexture.MaterialEffectSS6PU.TableName[indexTable] = namePathDefault;
						materialOverride = AssetDatabase.LoadAssetAtPath<Material>(informationTexture.MaterialEffectSS6PU.TableName[indexTable]);
					}
					informationTexture.MaterialEffectSS6PU.TableData[indexTable] = materialOverride;
					if(null == materialOverride)
					{
						versionOverride = Script_SpriteStudio6_DataEffect.KindVersion.SS5PU;
					}
					informationTexture.MaterialEffectSS6PU.Version[indexTable] = (int)versionOverride;

					/* Version Patch */
					/* MEMO: At updating old version data, Special processing if can not solve by simply overwriting. */
					/* MEMO: Here, asset paths and references only.                                 */
					/*       Materials to be created will be solved in "AssetCreateMaterialEffect". */
					if(Script_SpriteStudio6_DataEffect.KindVersion.SS5PU == versionOverride)
					{
						goto AssetNameDecideMaterialEffect_End;
					}

				AssetNameDecideMaterialEffect_End:;
					return(true);

//				AssetNameDecideMaterialEffect_ErrorEnd:;
//					return(false);
				}

				public static bool AssetCreateMaterialAnimation(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
																	LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
																	Information.Texture informationTexture,
																	Library_SpriteStudio6.KindOperationBlend operationTarget,
																	Library_SpriteStudio6.KindMasking masking
																)
				{
//					const string messageLogPrefix = "Create Asset(Material-Animation)";

					int indexTable = Script_SpriteStudio6_Root.Material.IndexGetTable(0, operationTarget, masking);
					string namePropertyCheck = AssetCreateMaterialAnimationGetPropertyShader(operationTarget, masking);
					Material material = null;
					material = informationTexture.MaterialAnimationSS6PU.TableData[indexTable];
					if(null == material)
					{
						material = AssetCreateMaterialAnimationNewMaterial(ref setting, informationSSPJ, informationTexture, operationTarget, masking);
						if(null == material)
						{
							goto AssetCreateMaterialAnimation_ErrorEnd;
						}

						AssetDatabase.CreateAsset(material, informationTexture.MaterialAnimationSS6PU.TableName[indexTable]);
						informationTexture.MaterialAnimationSS6PU.TableData[indexTable] = AssetDatabase.LoadAssetAtPath<Material>(informationTexture.MaterialAnimationSS6PU.TableName[indexTable]);
					}

					/* Version Patch */
					/* MEMO: At updating old version data, Special processing if can not solve by simply overwriting. */
					if(null != material)
					{
						bool flagPatched = false;
						Material materialNew = null;

						/* MEMO: New shader is applied from version 1.0.26.                                                                      */
						/*       Differences are identified by the existence of a property which does not exist in the shader before Ver.1.0.26. */
						if((false == flagPatched) && (false == material.HasProperty(namePropertyCheck)))
						{	/* Before Ver.1.0.26 shaders */
							/* Replace to new shader */
							materialNew = AssetCreateMaterialAnimationNewMaterial(ref setting, informationSSPJ, informationTexture, operationTarget, masking);
							if(null == materialNew)
							{
								goto AssetCreateMaterialAnimation_ErrorEnd;
							}
							material.shader = materialNew.shader;
							material.CopyPropertiesFromMaterial(materialNew); 
							flagPatched = true;
						}

						if((false == flagPatched) && ((int)Script_SpriteStudio6_DataAnimation.KindVersion.CODE_010005 > informationTexture.MaterialAnimationSS6PU.Version[indexTable]))
						{	/* Before Ver.1.0.27 shaders */
							/* MEMO: Rewrite material's properties since stencil-test's setting is changed. */
							materialNew = AssetCreateMaterialAnimationNewMaterial(ref setting, informationSSPJ, informationTexture, operationTarget, masking);
							if(null == materialNew)
							{
								goto AssetCreateMaterialAnimation_ErrorEnd;
							}

							int idPropertyCompareStencil = Library_SpriteStudio6.Data.Shader.IDPropertyCompareStencil;
							int idPropertyStencilOperation = Library_SpriteStudio6.Data.Shader.IDPropertyStencilOperation;
							float valueProperty;
							switch(operationTarget)
							{
								case Library_SpriteStudio6.KindOperationBlend.MASK_PRE:
								case Library_SpriteStudio6.KindOperationBlend.MASK:
									valueProperty = materialNew.GetFloat(idPropertyStencilOperation);
									material.SetFloat(idPropertyStencilOperation, valueProperty);
									break;

								case Library_SpriteStudio6.KindOperationBlend.MIX:
								case Library_SpriteStudio6.KindOperationBlend.ADD:
								case Library_SpriteStudio6.KindOperationBlend.SUB:
								case Library_SpriteStudio6.KindOperationBlend.MUL:
								case Library_SpriteStudio6.KindOperationBlend.MUL_NA:
								case Library_SpriteStudio6.KindOperationBlend.SCR:
								case Library_SpriteStudio6.KindOperationBlend.EXC:
								case Library_SpriteStudio6.KindOperationBlend.INV:
									valueProperty = materialNew.GetFloat(idPropertyCompareStencil);
									material.SetFloat(idPropertyCompareStencil, valueProperty);
									break;
							}

							flagPatched = true;
						}

						material.mainTexture = informationTexture.PrefabTexture.TableData[0];
						EditorUtility.SetDirty(material);
						AssetDatabase.SaveAssets();
					}

					return(true);

				AssetCreateMaterialAnimation_ErrorEnd:;
					return(false);
				}
				private static string AssetCreateMaterialAnimationGetPropertyShader(	Library_SpriteStudio6.KindOperationBlend operationTarget,
																						Library_SpriteStudio6.KindMasking masking
																				)
				{
					switch(operationTarget)
					{
						case Library_SpriteStudio6.KindOperationBlend.MASK_PRE:
						case Library_SpriteStudio6.KindOperationBlend.MASK:
							return(Library_SpriteStudio6.Data.Shader.NamePropertyStencilOperation);	/* after Ver.1.0.26 */

						case Library_SpriteStudio6.KindOperationBlend.MIX:
						case Library_SpriteStudio6.KindOperationBlend.ADD:
						case Library_SpriteStudio6.KindOperationBlend.SUB:
						case Library_SpriteStudio6.KindOperationBlend.MUL:
						case Library_SpriteStudio6.KindOperationBlend.MUL_NA:
						case Library_SpriteStudio6.KindOperationBlend.SCR:
						case Library_SpriteStudio6.KindOperationBlend.EXC:
						case Library_SpriteStudio6.KindOperationBlend.INV:
							return(Library_SpriteStudio6.Data.Shader.NamePropertyBlendOperation);	/* after Ver.1.0.26 */

						default:
							break;
					}
					return(null);
				}
				private static Material AssetCreateMaterialAnimationNewMaterial(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
																					LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
																					Information.Texture informationTexture,
																					Library_SpriteStudio6.KindOperationBlend operationTarget,
																					Library_SpriteStudio6.KindMasking masking
																			)
				{
					Material material = null;

					switch(operationTarget)
					{
						case Library_SpriteStudio6.KindOperationBlend.MASK_PRE:
							switch(masking)
							{
								case Library_SpriteStudio6.KindMasking.THROUGH:
									material = setting.PresetMaterial.AnimationSS6PUThroughStencilPreDraw;
									break;

								case Library_SpriteStudio6.KindMasking.MASK:
									material = setting.PresetMaterial.AnimationSS6PUMaskStencilPreDraw;
									break;

								default:
									material = null;
									break;
							}
							break;

						case Library_SpriteStudio6.KindOperationBlend.MASK:
							switch(masking)
							{
								case Library_SpriteStudio6.KindMasking.THROUGH:
									material = setting.PresetMaterial.AnimationSS6PUThroughStencilDraw;
									break;

								case Library_SpriteStudio6.KindMasking.MASK:
									material = setting.PresetMaterial.AnimationSS6PUMaskStencilDraw;
									break;

								default:
									material = null;
									break;
							}
							break;

						case Library_SpriteStudio6.KindOperationBlend.MIX:
							switch(masking)
							{
								case Library_SpriteStudio6.KindMasking.THROUGH:
									material = setting.PresetMaterial.AnimationSS6PUThroughMix;
									break;

								case Library_SpriteStudio6.KindMasking.MASK:
									material = setting.PresetMaterial.AnimationSS6PUMaskMix;
									break;

								default:
									material = null;
									break;
							}
							break;

						case Library_SpriteStudio6.KindOperationBlend.ADD:
							switch(masking)
							{
								case Library_SpriteStudio6.KindMasking.THROUGH:
									material = setting.PresetMaterial.AnimationSS6PUThroughAdd;
									break;

								case Library_SpriteStudio6.KindMasking.MASK:
									material = setting.PresetMaterial.AnimationSS6PUMaskAdd;
									break;

								default:
									material = null;
									break;
							}
							break;

						case Library_SpriteStudio6.KindOperationBlend.SUB:
							switch(masking)
							{
								case Library_SpriteStudio6.KindMasking.THROUGH:
									material = setting.PresetMaterial.AnimationSS6PUThroughSub;
									break;

								case Library_SpriteStudio6.KindMasking.MASK:
									material = setting.PresetMaterial.AnimationSS6PUMaskSub;
									break;

								default:
									material = null;
									break;
							}
							break;

						case Library_SpriteStudio6.KindOperationBlend.MUL:
							switch(masking)
							{
								case Library_SpriteStudio6.KindMasking.THROUGH:
									material = setting.PresetMaterial.AnimationSS6PUThroughMul;
									break;

								case Library_SpriteStudio6.KindMasking.MASK:
									material = setting.PresetMaterial.AnimationSS6PUMaskMul;
									break;

								default:
									material = null;
									break;
							}
							break;

						case Library_SpriteStudio6.KindOperationBlend.MUL_NA:
							switch(masking)
							{
								case Library_SpriteStudio6.KindMasking.THROUGH:
									material = setting.PresetMaterial.AnimationSS6PUThroughMulNA;
									break;

								case Library_SpriteStudio6.KindMasking.MASK:
									material = setting.PresetMaterial.AnimationSS6PUMaskMulNA;
									break;

								default:
									material = null;
									break;
							}
							break;

						case Library_SpriteStudio6.KindOperationBlend.SCR:
							switch(masking)
							{
								case Library_SpriteStudio6.KindMasking.THROUGH:
									material = setting.PresetMaterial.AnimationSS6PUThroughScr;
									break;

								case Library_SpriteStudio6.KindMasking.MASK:
									material = setting.PresetMaterial.AnimationSS6PUMaskScr;
									break;

								default:
									material = null;
									break;
							}
							break;

						case Library_SpriteStudio6.KindOperationBlend.EXC:
							switch(masking)
							{
								case Library_SpriteStudio6.KindMasking.THROUGH:
									material = setting.PresetMaterial.AnimationSS6PUThroughExc;
									break;

								case Library_SpriteStudio6.KindMasking.MASK:
									material = setting.PresetMaterial.AnimationSS6PUMaskExc;
									break;

								default:
									material = null;
									break;
							}
							break;

						case Library_SpriteStudio6.KindOperationBlend.INV:
							switch(masking)
							{
								case Library_SpriteStudio6.KindMasking.THROUGH:
									material = setting.PresetMaterial.AnimationSS6PUThroughInv;
									break;

								case Library_SpriteStudio6.KindMasking.MASK:
									material = setting.PresetMaterial.AnimationSS6PUMaskInv;
									break;

								default:
									material = null;
									break;
							}
							break;

						default:
							material = null;
							break;
					}

					if(null != material)
					{
						material = new Material(material);
					}

					return(material);
				}

				public static bool AssetCreateMaterialEffect(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
																LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
																Information.Texture informationTexture,
																Library_SpriteStudio6.KindOperationBlendEffect operationTarget,
																Library_SpriteStudio6.KindMasking masking
															)
				{
//					const string messageLogPrefix = "Create Asset(Material-Effect)";

					int indexTable = Script_SpriteStudio6_RootEffect.Material.IndexGetTable(0, operationTarget, masking);
					string namePropertyCheck = AssetCreateMaterialEffectGetPropertyShader(operationTarget, masking);
					Material material = null;
					material = informationTexture.MaterialEffectSS6PU.TableData[indexTable];
					if(null == material)
					{
						material = AssetCreateMaterialEffectNewMaterial(ref setting, informationSSPJ, informationTexture, operationTarget, masking);

						if(null == material)
						{
							goto AssetCreateMaterialEffect_ErrorEnd;
						}

						AssetDatabase.CreateAsset(material, informationTexture.MaterialEffectSS6PU.TableName[indexTable]);
						informationTexture.MaterialEffectSS6PU.TableData[indexTable] = AssetDatabase.LoadAssetAtPath<Material>(informationTexture.MaterialEffectSS6PU.TableName[indexTable]);
					}

					if(null != material)
					{
						bool flagPatched = false;
						Material materialNew = null;

						/* MEMO: New shader is applied from version 1.0.26.                                                                      */
						/*       Differences are identified by the existence of a property which does not exist in the shader before Ver.1.0.26. */
						if((false == flagPatched) && (false == material.HasProperty(namePropertyCheck)))
						{	/* Before Ver.1.0.26 shaders */
							/* Replace to new shader */
							materialNew = AssetCreateMaterialEffectNewMaterial(ref setting, informationSSPJ, informationTexture, operationTarget, masking);
							if(null == materialNew)
							{
								goto AssetCreateMaterialEffect_ErrorEnd;
							}
							material.shader = materialNew.shader;
							material.CopyPropertiesFromMaterial(materialNew); 
							flagPatched = true;
						}

						if((false == flagPatched) && ((int)Script_SpriteStudio6_DataAnimation.KindVersion.CODE_010005 > informationTexture.MaterialAnimationSS6PU.Version[indexTable]))
						{	/* Before Ver.1.0.27 shaders */
							/* MEMO: Rewrite material's properties since stencil-test's setting is changed. */
							materialNew = AssetCreateMaterialEffectNewMaterial(ref setting, informationSSPJ, informationTexture, operationTarget, masking);
							if(null == materialNew)
							{
								goto AssetCreateMaterialEffect_ErrorEnd;
							}

							int idPropertyCompareStencil = Library_SpriteStudio6.Data.Shader.IDPropertyCompareStencil;
							float valueProperty;
							switch(operationTarget)
							{
								case Library_SpriteStudio6.KindOperationBlendEffect.MIX:
								case Library_SpriteStudio6.KindOperationBlendEffect.ADD:
									valueProperty = materialNew.GetFloat(idPropertyCompareStencil);
									material.SetFloat(idPropertyCompareStencil, valueProperty);
									break;
							}

							flagPatched = true;
						}

						material.mainTexture = informationTexture.PrefabTexture.TableData[0];
						EditorUtility.SetDirty(material);
						AssetDatabase.SaveAssets();
					}

					return(true);

				AssetCreateMaterialEffect_ErrorEnd:;
					return(false);
				}
				private static string AssetCreateMaterialEffectGetPropertyShader(	Library_SpriteStudio6.KindOperationBlendEffect operationTarget,
																					Library_SpriteStudio6.KindMasking masking
																				)
				{
					return(Library_SpriteStudio6.Data.Shader.NamePropertyBlendSource);	/* after Ver.1.0.26 */
				}
				private static Material AssetCreateMaterialEffectNewMaterial(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
																				LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
																				Information.Texture informationTexture,
																				Library_SpriteStudio6.KindOperationBlendEffect operationTarget,
																				Library_SpriteStudio6.KindMasking masking
																			)
				{
					Material material = null;

					switch(operationTarget)
					{
						case Library_SpriteStudio6.KindOperationBlendEffect.MIX:
							switch(masking)
							{
								case Library_SpriteStudio6.KindMasking.THROUGH:
									material = setting.PresetMaterial.EffectSS6PUThroughMix;
									break;

								case Library_SpriteStudio6.KindMasking.MASK:
									material = setting.PresetMaterial.EffectSS6PUMaskMix;
									break;

								default:
									material = null;
									break;
							}
							break;

						case Library_SpriteStudio6.KindOperationBlendEffect.ADD:
							switch(masking)
							{
								case Library_SpriteStudio6.KindMasking.THROUGH:
									material = setting.PresetMaterial.EffectSS6PUThroughAdd;
									break;

								case Library_SpriteStudio6.KindMasking.MASK:
									material = setting.PresetMaterial.EffectSS6PUMaskAdd;
									break;

								default:
									material = null;
									break;
							}
							break;

						default:
							material = null;
							break;
					}

					if(null != material)
					{
						material = new Material(material);
					}

					return(material);
				}
#endif

				public static bool ConvertCellMap(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
													LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
													LibraryEditor_SpriteStudio6.Import.SSCE.Information informationSSCE
												)
				{	/* Convert-SS6PU Pass-1 ... Transfer necessary data from the temporary. */
//					const string messageLogPrefix = "Convert (CellMap)";

					LibraryEditor_SpriteStudio6.Import.SSCE.Information.Texture informationTexture = null;	/* "UnityEngine.Texture" and my "Texture", class-names are conflict unless fully-qualified. */
					if(0 <= informationSSCE.IndexTexture)
					{
						informationTexture = informationSSPJ.TableInformationTexture[informationSSCE.IndexTexture];
						int countCell = informationSSCE.TableCell.Length;

						informationSSCE.Data.Name = string.Copy(informationSSCE.NameFileBody);
						informationSSCE.Data.SizeOriginal.x = (float)informationTexture.SizeX;
						informationSSCE.Data.SizeOriginal.y = (float)informationTexture.SizeY;
						informationSSCE.Data.TableCell = new Library_SpriteStudio6.Data.CellMap.Cell[countCell];

						for(int i=0; i<countCell; i++)
						{
							informationSSCE.Data.TableCell[i].CleanUp();
							informationSSCE.Data.TableCell[i] = informationSSCE.TableCell[i].Data;
						}
						informationSSCE.TableCell = null;	/* Purge WorkArea */
					}
					return(true);

//				ConvertCellMap_ErrorEnd:;
//					return(false);
				}
				#endregion Functions

				/* ----------------------------------------------- Enums & Constants */
				#region Enums & Constants
				private readonly static string[] NameKindMasking = new string[(int)Library_SpriteStudio6.KindMasking.TERMINATOR]
				{
					"T",
					"M"
				};
				#endregion Enums & Constants
			}

			public static partial class ModeUnityNative
			{
#if !_FOR_SS6CONVERTER_
				/* MEMO: Originally functions that should be defined in each information class. */
				/*       However, confusion tends to occur with mode increases.                 */
				/*       ... Compromised way.                                                   */

				/* ----------------------------------------------- Functions */
				#region Functions
				public static bool AssetNameDecideMaterialAnimation(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
																		LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
																		Information.Texture informationTexture,
																		string nameOutputAssetFolderBase,
																		Library_SpriteStudio6.KindOperationBlend operationTarget,
																		Material materialOverride
																	)
				{
					/* MEMO: "Unity-Native" mode does not have "MASK_PRE" and "MASK". */
					int indexTable = (int)operationTarget;
					if(null != materialOverride)
					{	/* Specified */
						informationTexture.MaterialAnimationUnityNative.TableName[indexTable] = AssetDatabase.GetAssetPath(materialOverride);
						informationTexture.MaterialAnimationUnityNative.TableData[indexTable] = materialOverride;
					}
					else
					{	/* Default */
						informationTexture.MaterialAnimationUnityNative.TableName[indexTable] = setting.RuleNameAssetFolder.NameGetAssetFolder(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.MATERIAL_ANIMATION_UNITYNATIVE, nameOutputAssetFolderBase)
																								+ setting.RuleNameAsset.NameGetAsset(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.MATERIAL_ANIMATION_UNITYNATIVE, informationTexture.NameFileBody, informationSSPJ.NameFileBody)
																								+ "_" + operationTarget.ToString()
																								+ LibraryEditor_SpriteStudio6.Import.NameExtentionMaterial;
						informationTexture.MaterialAnimationUnityNative.TableData[indexTable] = AssetDatabase.LoadAssetAtPath<Material>(informationTexture.MaterialAnimationUnityNative.TableName[indexTable]);
					}

					return(true);

//				AssetNameDecideMaterialAnimation_ErrorEnd:;
//					return(false);
				}

				public static bool AssetCreateMaterialAnimation(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
																	LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
																	Information.Texture informationTexture,
																	Library_SpriteStudio6.KindOperationBlend operationTarget
																)
				{
#if false
					const string messageLogPrefix = "Create Asset(Material-Animation)";

					/* MEMO: "Unity-Native" mode does not have "MASK_PRE" and "MASK". */
					int indexTable = (int)operationTarget;
					Material material = null;
					material = informationTexture.MaterialAnimationUnityNative.TableData[indexTable];
					if(null == material)
					{
						material = new Material(Library_SpriteStudio6.Data.Shader.ShaderGetAnimationUnityNative(operationTarget));

						AssetDatabase.CreateAsset(material, informationTexture.MaterialAnimationUnityNative.TableName[indexTable]);
						informationTexture.MaterialAnimationUnityNative.TableData[indexTable] = AssetDatabase.LoadAssetAtPath<Material>(informationTexture.MaterialAnimationUnityNative.TableName[indexTable]);
					}

					material.mainTexture = informationTexture.PrefabTexture.TableData[0];
					EditorUtility.SetDirty(material);
					AssetDatabase.SaveAssets();
#endif
					return(true);

//				AssetCreateMaterialAnimation_ErrorEnd:;
//					return(false);
				}

				public static bool ConvertCellMap(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
													LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
													LibraryEditor_SpriteStudio6.Import.SSCE.Information informationSSCE
												)
				{
					const string messageLogPrefix = "Convert (CellMap)";

					List<SpriteMetaData> listSpriteMetaData = null;
					string[] tableNameSprite = null;

					LibraryEditor_SpriteStudio6.Import.SSCE.Information.Texture informationTexture = null;	/* "UnityEngine.Texture" and my "Texture", class-names are conflict unless fully-qualified. */
					if(0 <= informationSSCE.IndexTexture)
					{
						informationTexture = informationSSPJ.TableInformationTexture[informationSSCE.IndexTexture];
						if(null == informationTexture.ListSpriteMetaDataUnityNative)
						{
							informationTexture.ListSpriteMetaDataUnityNative = new List<SpriteMetaData>();
							if(null == informationTexture.ListSpriteMetaDataUnityNative)
							{
								LogError(messageLogPrefix, "Not Enough Memory (CellMap WorkArea)", informationSSCE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertCellMap_ErrorEnd;
							}
							informationTexture.ListSpriteMetaDataUnityNative.Clear();
						}

						listSpriteMetaData = informationTexture.ListSpriteMetaDataUnityNative;
						SpriteMetaData spriteMetaData = new SpriteMetaData();
						Vector2 sizeTexture = new Vector2((float)informationTexture.SizeX, (float)informationTexture.SizeY);
						Vector2 sizeInverseTexture;
						sizeInverseTexture.x = 1.0f / sizeTexture.x;
						sizeInverseTexture.y = 1.0f / sizeTexture.y;
						Vector2 sizeCell;
						Vector2 positionCell;
						Vector2 pivotCell;
						Rect rectangleCell;
						int countCell = informationSSCE.TableCell.Length;

						informationSSCE.TableNameSpriteUnityNative = new string[countCell];
						if(null == informationSSCE.TableNameSpriteUnityNative)
						{
							LogError(messageLogPrefix, "Not Enough Memory (CellMap WorkArea)", informationSSCE.FileNameGetFullPath(), informationSSPJ);
							goto ConvertCellMap_ErrorEnd;
						}
						tableNameSprite = informationSSCE.TableNameSpriteUnityNative;
						for(int i=0; i<countCell; i++)
						{
							tableNameSprite[i] = null;
						}

						string nameCell;
						for(int i=0; i<countCell; i++)
						{
							nameCell = ConvertCellMapNameCreate(listSpriteMetaData, informationSSCE.TableCell[i].Data.Name);
							if(null == nameCell)
							{
								LogError(messageLogPrefix, "Name Decision Falure [" + informationSSCE.TableCell[i].Data.Name + "]", informationSSCE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertCellMap_ErrorEnd;
							}

							spriteMetaData.name = string.Copy(nameCell);
							rectangleCell = informationSSCE.TableCell[i].Data.Rectangle;
							positionCell = rectangleCell.position;
							sizeCell = rectangleCell.size;
							positionCell.y = sizeTexture.y - (positionCell.y + sizeCell.y);
							rectangleCell.position = positionCell;
							rectangleCell.size = sizeCell;
							spriteMetaData.rect = rectangleCell;

							pivotCell.x = informationSSCE.TableCell[i].Data.Pivot.x / sizeCell.x;
							pivotCell.y = 1.0f - (informationSSCE.TableCell[i].Data.Pivot.y / sizeCell.y);

							spriteMetaData.pivot = pivotCell;
							spriteMetaData.alignment = (int)SpriteAlignment.Custom;

							listSpriteMetaData.Add(spriteMetaData);

							tableNameSprite[i] = string.Copy(nameCell);
						}
//						informationSSCE.TableCell = null;	/* Purge WorkArea */
					}
					return(true);

				ConvertCellMap_ErrorEnd:;
					if(null != tableNameSprite)
					{
						informationSSCE.TableNameSpriteUnityNative = null;
						tableNameSprite = null;
					}

					if(null != listSpriteMetaData)
					{
						informationTexture.ListSpriteMetaDataUnityNative = null;
						listSpriteMetaData.Clear();
						listSpriteMetaData = null;
					}
					return(false);
				}
				private static string ConvertCellMapNameCreate(List<SpriteMetaData> listSpriteMetaData, string nameCell)
				{
					bool flagRetry = true;
					int countRetry = 0;
					string name = nameCell;
					while(true == flagRetry)
					{
						flagRetry = false;

						int countListSpriteMetaData = listSpriteMetaData.Count;
						for(int i=0; i<countListSpriteMetaData; i++)
						{
							if(listSpriteMetaData[i].name == name)
							{
								countRetry++;
								name = nameCell + "_" + countRetry.ToString();
								flagRetry = true;
								break;	/* i-loop */
							}
						}
					}

					return(name);
				}

				public static bool CellMapSetTexture(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
														LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
														int indexTexture
													)
				{
					const string messageLogPrefix = "Add Texture-Atlas";

					LibraryEditor_SpriteStudio6.Import.SSCE.Information.Texture informationTexture = informationSSPJ.TableInformationTexture[indexTexture];
					if(null == informationTexture)
					{
						/* MEMO: Usually do not reach here. */
						LogError(messageLogPrefix, "Texture information vanished. index[" + indexTexture.ToString() + "]", "", informationSSPJ);
						goto CellMapSetTexture_ErrorEnd;
					}
					if(null == informationTexture.ListSpriteMetaDataUnityNative)
					{	/* Has no Cell */
						return(true);
					}
					if(null == informationTexture.PrefabTexture.TableData[0])
					{
						LogError(messageLogPrefix, "Texture asset lost", informationTexture.FileNameGetFullPath(), informationSSPJ);
						goto CellMapSetTexture_ErrorEnd;
					}

					TextureImporter importer = TextureImporter.GetAtPath(informationTexture.PrefabTexture.TableName[0]) as TextureImporter;
					importer.spriteImportMode = SpriteImportMode.Multiple;
					importer.spritePackingTag = LibraryEditor_SpriteStudio6.Import.NameTagSpritePackerTexture;
					importer.spritesheet = informationTexture.ListSpriteMetaDataUnityNative.ToArray();
					importer.spritePixelsPerUnit = 1.0f;
					bool flagEnableRead = false;
					if(true == setting.Basic.FlagTextureReadable)
					{
						flagEnableRead = true;
					}
					importer.isReadable = flagEnableRead;

					EditorUtility.SetDirty(importer);
					AssetDatabase.ImportAsset(informationTexture.PrefabTexture.TableName[0], ImportAssetOptions.ForceUpdate);	/* Re-Import */
					AssetDatabase.SaveAssets();

					/* Purge temporary */
					informationTexture.ListSpriteMetaDataUnityNative.Clear();
					informationTexture.ListSpriteMetaDataUnityNative = null;

					/* Get Sprites */
					/* MEMO: Do not access this list by Cell's index, since sorting-method is likely to be different from "SpriteStudio6". */
					/*       (Search by the name of informationSSCE.TableNameSpriteUnityNative and get index)                              */
					if(null == informationTexture.ListSpriteUnityNative)
					{
						informationTexture.ListSpriteUnityNative = new List<Sprite>();
						if(null == informationTexture.ListSpriteUnityNative)
						{
							LogError(messageLogPrefix, "Not Enough Memory", informationTexture.FileNameGetFullPath(), informationSSPJ);
							goto CellMapSetTexture_ErrorEnd;
						}
					}
					List<Sprite> listSprite =  informationTexture.ListSpriteUnityNative;
					listSprite.Clear();
					{
						UnityEngine.Object[] tableObjectTexture = AssetDatabase.LoadAllAssetsAtPath(informationTexture.PrefabTexture.TableName[0]);
						foreach(UnityEngine.Object objectTexture in tableObjectTexture)
						{
							if(true == objectTexture.GetType().Equals(typeof(Sprite)))
							{
								listSprite.Add((Sprite)objectTexture);
							}
						}
					}

					return(true);

				CellMapSetTexture_ErrorEnd:;
					return(false);
				}
				#endregion Functions
#endif
			}
			#endregion Classes, Structs & Interfaces
		}
	}
}
