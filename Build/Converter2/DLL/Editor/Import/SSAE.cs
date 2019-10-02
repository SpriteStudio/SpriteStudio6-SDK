/**
	SpriteStudio6 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
// #define STORE_ANIMATIONSETUP_FULL
// #define BONEINDEX_CONVERT_PARTSID
#define WARN_MESHVERTEX_COUNT
#define CHANGE_DEFORM_DECODING

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
		public static partial class SSAE
		{
			/* ----------------------------------------------- Functions */
			#region Functions
			public static Information Parse(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
												string nameFile,
												LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ
											)
			{
				const string messageLogPrefix = "Parse SSAE";
				Information informationSSAE = null;

				/* ".ssae" Load */
				if(false == System.IO.File.Exists(nameFile))
				{
					LogError(messageLogPrefix, "File Not Found", nameFile, informationSSPJ);
					goto Parse_ErrorEnd;
				}
				System.Xml.XmlDocument xmlSSAE = new System.Xml.XmlDocument();
				xmlSSAE.Load(nameFile);

				/* Check Version */
				System.Xml.XmlNode nodeRoot = xmlSSAE.FirstChild;
				nodeRoot = nodeRoot.NextSibling;
				KindVersion version = (KindVersion)(LibraryEditor_SpriteStudio6.Utility.XML.VersionGet(nodeRoot, "SpriteStudioAnimePack", (int)KindVersion.ERROR, true));
				switch(version)
				{
					case KindVersion.ERROR:
						LogError(messageLogPrefix, "Version Invalid", nameFile, informationSSPJ);
						goto Parse_ErrorEnd;

					case KindVersion.CODE_000100:
					case KindVersion.CODE_010000:
					case KindVersion.CODE_010001:
					case KindVersion.CODE_010002:
					case KindVersion.CODE_010200:
					case KindVersion.CODE_010201:
					case KindVersion.CODE_010202:
						LogError(messageLogPrefix, "\"SpriteStudio5\"'s data can not be imported.Please re-save data in \"SpriteStudio6\" and then import.", nameFile, informationSSPJ);
						goto Parse_ErrorEnd;

					case KindVersion.CODE_020000:
					case KindVersion.CODE_020001:
					case KindVersion.CODE_020003:
					case KindVersion.CODE_020004:
						break;

					default:
						if(KindVersion.TARGET_EARLIEST > version)
						{
							version = KindVersion.TARGET_EARLIEST;
							if(true == setting.CheckVersion.FlagInvalidSSAE)
							{
								LogWarning(messageLogPrefix, "Version Too Early", nameFile, informationSSPJ);
							}
						}
						else
						{
							version = KindVersion.TARGET_LATEST;
							if(true == setting.CheckVersion.FlagInvalidSSAE)
							{
								LogWarning(messageLogPrefix, "Version Unknown", nameFile, informationSSPJ);
							}
						}
						break;
				}

				/* Create Information */
				informationSSAE = new Information();
				if(null == informationSSAE)
				{
					LogError(messageLogPrefix, "Not Enough Memory", nameFile, informationSSPJ);
					goto Parse_ErrorEnd;
				}
				informationSSAE.CleanUp();
				informationSSAE.Version = version;
				informationSSAE.ListBone = new List<string>();
				if(null == informationSSAE.ListBone)
				{
					LogError(messageLogPrefix, "Not Enough Memory (Bone list)", nameFile, informationSSPJ);
					goto Parse_ErrorEnd;
				}
				informationSSAE.ListInUseCellMap = new List<Information.InUseCellMap>();
				if(null == informationSSAE.ListInUseCellMap)
				{
					LogError(messageLogPrefix, "Not Enough Memory (In-use Cell-Map list)", nameFile, informationSSPJ);
					goto Parse_ErrorEnd;
				}
				informationSSAE.ListBindMesh = new List<Information.BindMesh>();
				if(null == informationSSAE.ListBindMesh)
				{
					LogError(messageLogPrefix, "Not Enough Memory (Mesh-Bind list)", nameFile, informationSSPJ);
					goto Parse_ErrorEnd;
				}

				if(false == informationSSAE.CatalogParts.BootUp())
				{
					LogError(messageLogPrefix, "Not Enough Memory (Parts-Catalog)", nameFile, informationSSPJ);
					goto Parse_ErrorEnd;
				}

				/* Get Base-Directories */
				LibraryEditor_SpriteStudio6.Utility.File.PathSplit(out informationSSAE.NameDirectory, out informationSSAE.NameFileBody, out informationSSAE.NameFileExtension, nameFile);
				informationSSAE.NameDirectory += "/";

				/* Decode Tags */
				System.Xml.NameTable nodeNameSpace = new System.Xml.NameTable();
				System.Xml.XmlNamespaceManager managerNameSpace = new System.Xml.XmlNamespaceManager(nodeNameSpace);
				System.Xml.XmlNodeList listNode = null;

				/* Decode Parts-Data */
				listNode = LibraryEditor_SpriteStudio6.Utility.XML.ListGetNode(nodeRoot, "Model/partList/value", managerNameSpace);
				if(null == listNode)
				{
					LogError(messageLogPrefix, "PartList-Node Not Found", nameFile, informationSSPJ);
					goto Parse_ErrorEnd;
				}
				int countParts = listNode.Count;
				informationSSAE.TableParts = new Information.Parts[countParts];
				if(null == informationSSAE.TableParts)
				{
					LogError(messageLogPrefix, "Not Enough Memory (Parts-Data WorkArea)", nameFile, informationSSPJ);
					goto Parse_ErrorEnd;
				}
				foreach(System.Xml.XmlNode nodeAnimation in listNode)
				{
					/* Get Part-Data */
					int indexParts = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAnimation, "arrayIndex", managerNameSpace));
					informationSSAE.TableParts[indexParts] = ParseParts(	ref setting,
																			informationSSPJ,
																			nodeAnimation,
																			managerNameSpace,
																			informationSSAE,
																			indexParts,
																			nameFile
																		);
					if(null == informationSSAE.TableParts[indexParts])
					{
						goto Parse_ErrorEnd;
					}
				}
				for(int i=0; i<countParts; i++)
				{
					/* Fix child-parts' index table */
					informationSSAE.TableParts[i].Data.TableIDChild = informationSSAE.TableParts[i].ListIndexPartsChild.ToArray();
					informationSSAE.TableParts[i].ListIndexPartsChild.Clear();
					informationSSAE.TableParts[i].ListIndexPartsChild = null;

					/* Fix inheritance kind */
					/* MEMO: Parent-part is always fixed earlier. */
					int indexPartsParent = -1;
					switch(informationSSAE.TableParts[i].Inheritance)
					{
						case Information.Parts.KindInheritance.PARENT:
							indexPartsParent = informationSSAE.TableParts[i].Data.IDParent;
							if(0 <= indexPartsParent)
							{
								informationSSAE.TableParts[i].Inheritance = Information.Parts.KindInheritance.SELF;
								informationSSAE.TableParts[i].FlagInheritance = informationSSAE.TableParts[indexPartsParent].FlagInheritance;
							}
							break;
						case Information.Parts.KindInheritance.SELF:
							break;

						default:
							goto case Information.Parts.KindInheritance.PARENT;
					}
				}

				/* Decode Bone-List */
				listNode = LibraryEditor_SpriteStudio6.Utility.XML.ListGetNode(nodeRoot, "Model/boneList/item", managerNameSpace);
				if(null != listNode)
				{
					int countBoneList = listNode.Count;
					if(0 < countBoneList)
					{
						int indexBone;

						/* MEMO: In reality no need, but in case the bone's index is flying. */
						int indexBoneMin = int.MaxValue;
						foreach(System.Xml.XmlNode nodeBone in listNode)
						{
							/* Scan Bone-List */
							indexBone = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(nodeBone.InnerText);
							if(0 <= indexBone)
							{	/* Valid index */
								if(countBoneList <= indexBone)
								{
									countBoneList = indexBone + 1;
								}
								if(indexBoneMin > indexBone)
								{
									indexBoneMin = indexBone;
								}
							}
						}
						for(int i=0; i<countBoneList; i++)
						{
							informationSSAE.ListBone.Add(null);
						}

						/* MEMO: Some Beta versions of SS6.3 be outputting an "1 origined" index. */
						if(int.MaxValue <= indexBoneMin)
						{	/* All Invalid or no bone */
							indexBoneMin = 0;
						}
						informationSSAE.OffsetIndexBone = indexBoneMin;

						string nameBoneParts;
						foreach(System.Xml.XmlNode nodeBone in listNode)
						{
							/* Get Bone-List */
							nameBoneParts = nodeBone.Attributes["key"].Value;
							indexBone = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(nodeBone.InnerText);
							if(0 <= indexBone)
							{
								informationSSAE.ListBone[indexBone - indexBoneMin] = string.Copy(nameBoneParts);
							}
						}
					}
				}

				/* Decode MeshBind List */
				listNode = LibraryEditor_SpriteStudio6.Utility.XML.ListGetNode(nodeRoot, "Model/meshList/value", managerNameSpace);
				if(null != listNode)
				{
					string[] valueTextSplitVertex = null;
					string[] valueTextSplitParameter = null;
					int countVertex;
					int countBone;
					int countParameterBone;

					foreach(System.Xml.XmlNode nodeBindMesh in listNode)
					{
						Information.BindMesh bindMesh = new Information.BindMesh();
						bindMesh.BootUp();

						if(false == string.IsNullOrEmpty(nodeBindMesh.InnerText))
						{
							/* MEMO: Bind-Mesh data format is in following.                                                        */
							/*       VertexBlock-0, VettexBlock-1, .... VertexBlock-x, (x = number of Vertices)                    */
							/*       *) VertexBlock: n [Bone-Index Weight(%) X Y] [Bone-Index Weight(%) X Y]... (Repeat "n" times) */
							Library_SpriteStudio6.Data.Parts.Animation.BindMesh.Vertex bindVertex = new Library_SpriteStudio6.Data.Parts.Animation.BindMesh.Vertex();

							valueTextSplitVertex = nodeBindMesh.InnerText.Split(',');
							if(null != valueTextSplitVertex)
							{
								/* MEMO: Since the end always ends with "," count is one more than actual vertices. */
								countVertex = valueTextSplitVertex.Length;
								if(true == string.IsNullOrEmpty(valueTextSplitVertex[countVertex - 1]))
								{
									countVertex--;
								}

								for(int i=0; i<countVertex; i++)
								{
									valueTextSplitParameter = valueTextSplitVertex[i].Split(' ');

									/* Get Bone-count */
									/* MEMO: No-effective bones(weight=0) are counted to VertexBlock's "n(number of Bone)" sometimes, so check real data size. */
									countBone =  LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueTextSplitParameter[0]);
									countParameterBone = (valueTextSplitParameter.Length - 1) / 4;	/* "- 1": Bone-Count / "/ 4": number of Parameter */
									if(countBone != countParameterBone)	{
										countBone = countParameterBone;
									}

									if(0 < countBone)
									{	/* not Terminator */
										bindVertex.CleanUp();
										bindVertex.BootUp(countBone);

										/* Set Bones */
										int indexParameter;
										for(int j=0; j<countBone; j++)
										{
											indexParameter = (j * 4) + 1;
											bindVertex.TableBone[j].Index = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueTextSplitParameter[indexParameter + 0]) - informationSSAE.OffsetIndexBone;
											bindVertex.TableBone[j].Weight = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueTextSplitParameter[indexParameter + 1])) * 0.01f;
											bindVertex.TableBone[j].CoordinateOffset.x = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueTextSplitParameter[indexParameter + 2]);
											bindVertex.TableBone[j].CoordinateOffset.y = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueTextSplitParameter[indexParameter + 3]);
											bindVertex.TableBone[j].CoordinateOffset.z = 0.0f;
										}

										/* Add Vertex-binding */
										bindMesh.ListBindVertex.Add(bindVertex);
									}
								}
							}
						}

						informationSSAE.ListBindMesh.Add(bindMesh);
					}
				}

				/* Solve Referenced-CellMaps' index */
				listNode = LibraryEditor_SpriteStudio6.Utility.XML.ListGetNode(nodeRoot, "cellmapNames/value", managerNameSpace);
				if(null == listNode)
				{
					informationSSAE.TableIndexCellMap = null;
				}
				else
				{
					int countCellMap = listNode.Count;
					int indexCellMap = 0;
					string nameCellMap = "";

					informationSSAE.TableIndexCellMap = new int[countCellMap];
					for(int i=0; i<countCellMap; i++)
					{
						informationSSAE.TableIndexCellMap[i] = -1;
					}
					foreach(System.Xml.XmlNode nodeCellMapName in listNode)
					{
						nameCellMap = nodeCellMapName.InnerText;
						nameCellMap = informationSSPJ.PathGetAbsolute(nameCellMap, LibraryEditor_SpriteStudio6.Import.KindFile.SSCE);

						informationSSAE.TableIndexCellMap[indexCellMap] = informationSSPJ.IndexGetFileName(informationSSPJ.TableNameSSCE, nameCellMap);
						if(-1 == informationSSAE.TableIndexCellMap[indexCellMap])
						{
							LogError(messageLogPrefix, "CellMap Not Found", nameFile, informationSSPJ);
							goto Parse_ErrorEnd;
						}

						indexCellMap++;
					}
				}

				/* Animations (& Parts' Key-Frames) Get */
				listNode = LibraryEditor_SpriteStudio6.Utility.XML.ListGetNode(nodeRoot, "animeList/anime", managerNameSpace);
				if(null == listNode)
				{
					LogError(messageLogPrefix, "AnimationList-Node Not Found", nameFile, informationSSPJ);
					goto Parse_ErrorEnd;
				}

				bool flagHasSetup = false;
				string valueText;
				foreach(System.Xml.XmlNode nodeAnimation in listNode)
				{
					/* Check "SetUp" animation */
					/* MEMO: When judging "Setup"-animation, do not judge by name. (Be sure, use "isSetup" tag's value.)                      */
					/*       Especially, if SS5's SSAE including same name animation is migrated to SS6, "Setup"-animation's name is changed. */
					valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAnimation, "isSetup", managerNameSpace);
					if(false == string.IsNullOrEmpty(valueText))
					{
						if(0 < LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText))
						{
							flagHasSetup = true;
						}
					}
				}
				int countAnimation = listNode.Count;
				if(true == flagHasSetup)
				{
					/* MEMO: If has "Setup", should not be countAnimation negative. */
					countAnimation--;
				}
				informationSSAE.TableAnimation = new Information.Animation[countAnimation];
				if(null == informationSSAE.TableAnimation)
				{
					LogError(messageLogPrefix, "Not Enough Memory (Animation-Data WorkArea)", nameFile, informationSSPJ);
					goto Parse_ErrorEnd;
				}

				int indexAnimation = 0;
				Information.Animation informationAnimation = null;
				foreach(System.Xml.XmlNode nodeAnimation in listNode)
				{
					/* Animation (& Parts' Key-Frames) Get */
					flagHasSetup = false;	/* recycling *//* Is animation "Setup"-animation ? */
					informationAnimation = ParseAnimation(	ref setting,
															out flagHasSetup,
															informationSSPJ,
															nodeAnimation,
															managerNameSpace,
															informationSSAE,
															indexAnimation,
															nameFile
														);
					if(null == informationAnimation)
					{
						goto Parse_ErrorEnd;
					}

					if(true == flagHasSetup)
					{	/* "Setup" animation */
						informationSSAE.AnimationSetup = informationAnimation;
					}
					else
					{	/* Normal animation */
						informationSSAE.TableAnimation[indexAnimation] = informationAnimation;
						indexAnimation++;
					}
				}

				/* Set secondary parameters */
				/* MEMO: This process is unnecessary, since "Setup" animation has only attributes' initial values. */
				for(int i=0; i<countAnimation; i++)
				{
					/* Solving Attributes */
					/* MEMO: Complement all animation-attributes' frame 0 key-data.                           */
					/*       Applying "Setup" animation and deleting useless key-datas is processed here.     */
					/*       Process after parse all animation data since "Setup" animation is not guaranteed */
					/*        defining at top of the animation-list in SSAE.                                  */
					informationSSAE.TableAnimation[i].AttributeSolve(informationSSPJ, informationSSAE, setting.Basic.FlagInvisibleToHideAll);

					/* Set Part-Status */
					/* MEMO: Analyze key-data and set each parts' usage status. */
					/* MEMO: Execute before "DrawOrderCreate". */
					informationSSAE.TableAnimation[i].StatusSetParts(informationSSPJ, informationSSAE);

					/* Set Draw-Order */
					informationSSAE.TableAnimation[i].DrawOrderCreate(informationSSPJ, informationSSAE);
				}

				/* Gather Parts-ID by feature(usage) */
				/* MEMO: Can not gather unless Parts' "Feature" has been finalized. */
				countParts = informationSSAE.TableParts.Length;
				for(int i=0; i<countParts; i++)
				{
					switch(informationSSAE.TableParts[i].Data.Feature)
					{
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.ROOT:
							break;

						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NULL:
							informationSSAE.CatalogParts.ListIDPartsNULL.Add(i);
							break;

//						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE2:
//							informationSSAE.CatalogParts.ListIDPartsTriangle2.Add(i);
//							break;
//
//						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE4:
//							informationSSAE.CatalogParts.ListIDPartsTriangle4.Add(i);
//							break;
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL:
							informationSSAE.CatalogParts.ListIDPartsNormal.Add(i);
							break;

						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE:
							informationSSAE.CatalogParts.ListIDPartsInstance.Add(i);
							break;

						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT:
							informationSSAE.CatalogParts.ListIDPartsEffect.Add(i);
							break;

//						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE2:
//							informationSSAE.CatalogParts.ListIDPartsMaskTriangle2.Add(i);
//							break;
//
//						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE4:
//							informationSSAE.CatalogParts.ListIDPartsMaskTriangle4.Add(i);
//							break;
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK:
							informationSSAE.CatalogParts.ListIDPartsMask.Add(i);
							break;

						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.JOINT:
							informationSSAE.CatalogParts.ListIDPartsJoint.Add(i);
							break;

						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONE:
							informationSSAE.CatalogParts.ListIDPartsBone.Add(i);
							break;

						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MOVENODE:
							informationSSAE.CatalogParts.ListIDPartsMoveNode.Add(i);
							break;

						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.CONSTRAINT:
							informationSSAE.CatalogParts.ListIDPartsConstraint.Add(i);
							break;

						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONEPOINT:
							informationSSAE.CatalogParts.ListIDPartsBonePoint.Add(i);
							break;

						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH:
							informationSSAE.CatalogParts.ListIDPartsMesh.Add(i);
							break;

						default:
							break;
					}
				}

				return(informationSSAE);

			Parse_ErrorEnd:;
				informationSSAE = null;
				return(null);
			}

			private static Information.Parts ParseParts(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
															LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
															System.Xml.XmlNode nodeParts,
															System.Xml.XmlNamespaceManager managerNameSpace,
															Information informationSSAE,
															int indexParts,
															string nameFileSSAE
														)
			{
				const string messageLogPrefix = "Parse SSAE(Parts)";

				Information.Parts informationParts = new Information.Parts();
				if(null == informationParts)
				{
					LogError(messageLogPrefix, "Not Enough Memory (Parts WorkArea) Parts[" + indexParts.ToString() + "]", nameFileSSAE, informationSSPJ);
					goto ParseParts_ErrorEnd;
				}
				informationParts.CleanUp();

				/* Get Base Datas */
				string valueText = "";

				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "name", managerNameSpace);
				informationParts.Data.Name = string.Copy(valueText);

				informationParts.Data.ID = indexParts;

				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "parentIndex", managerNameSpace);
				informationParts.Data.IDParent = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);
				if(0 <= informationParts.Data.IDParent)
				{
					Information.Parts informationPartsParent = informationSSAE.TableParts[informationParts.Data.IDParent];
					informationPartsParent.ListIndexPartsChild.Add(informationParts.Data.ID);
				}

				/* Get Parts-Type */
				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "type", managerNameSpace);
				switch(valueText)
				{
					case "null":
						informationParts.Data.Feature = (0 == informationParts.Data.ID) ? Library_SpriteStudio6.Data.Parts.Animation.KindFeature.ROOT : Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NULL;
						break;

					case "normal":
						informationParts.Data.Feature = Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL;
						break;

					case "instance":
						informationParts.Data.Feature = Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE;
						break;

					case "effect":
						informationParts.Data.Feature = Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT;
						break;

					case "mask":
						informationParts.Data.Feature = Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK;
						break;

					case "mesh":
						informationParts.Data.Feature = Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH;
						break;

					case "joint":
						informationParts.Data.Feature = Library_SpriteStudio6.Data.Parts.Animation.KindFeature.JOINT;
						break;

					case "armature":
						informationParts.Data.Feature = Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONE;
						break;

					case "movenode":
						informationParts.Data.Feature = Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MOVENODE;
						break;

					case "constraint":
						informationParts.Data.Feature = Library_SpriteStudio6.Data.Parts.Animation.KindFeature.CONSTRAINT;
						break;

					case "bonepoint":
						informationParts.Data.Feature = Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONEPOINT;
						break;

					default:
						LogWarning(messageLogPrefix, "Unknown Parts-Type \"" + valueText + "\" Parts[" + indexParts.ToString() + "]", nameFileSSAE, informationSSPJ);
						goto case "null";
				}

				/* Get "Collision" Datas */
				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "boundsType", managerNameSpace);
				switch(valueText)
				{
					case "none":
						informationParts.Data.ShapeCollision = Library_SpriteStudio6.Data.Parts.Animation.KindCollision.NON;
						informationParts.Data.SizeCollisionZ = 0.0f;
						break;

					case "quad":
						informationParts.Data.ShapeCollision = Library_SpriteStudio6.Data.Parts.Animation.KindCollision.SQUARE;
						informationParts.Data.SizeCollisionZ = setting.Collider.SizeZ;
						break;

					case "aabb":
						informationParts.Data.ShapeCollision = Library_SpriteStudio6.Data.Parts.Animation.KindCollision.AABB;
						informationParts.Data.SizeCollisionZ = setting.Collider.SizeZ;
						break;

					case "circle":
						informationParts.Data.ShapeCollision = Library_SpriteStudio6.Data.Parts.Animation.KindCollision.CIRCLE;
						informationParts.Data.SizeCollisionZ = setting.Collider.SizeZ;
						break;

					case "circle_smin":
						informationParts.Data.ShapeCollision = Library_SpriteStudio6.Data.Parts.Animation.KindCollision.CIRCLE_SCALEMINIMUM;
						informationParts.Data.SizeCollisionZ = setting.Collider.SizeZ;
						break;

					case "circle_smax":
						informationParts.Data.ShapeCollision = Library_SpriteStudio6.Data.Parts.Animation.KindCollision.CIRCLE_SCALEMAXIMUM;
						informationParts.Data.SizeCollisionZ = setting.Collider.SizeZ;
						break;

					default:
						LogWarning(messageLogPrefix, "Unknown Collision Kind \"" + valueText + "\" Parts[" + indexParts.ToString() + "]", nameFileSSAE, informationSSPJ);
						goto case "none";
				}

				/* Get "Inheritance" Datas */
				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "inheritType", managerNameSpace);
				switch(valueText)
				{
					case "parent":
						{
							switch(informationSSAE.Version)
							{
								case KindVersion.CODE_010000:
								case KindVersion.CODE_010001:
								case KindVersion.CODE_010002:
								case KindVersion.CODE_010200:
								case KindVersion.CODE_010201:
								case KindVersion.CODE_010202:	/* EffectPartsCheck? */
								case KindVersion.CODE_020000:
								case KindVersion.CODE_020001:
								case KindVersion.CODE_020003:
								case KindVersion.CODE_020004:
									if(0 == informationParts.Data.ID)
									{
										informationParts.Inheritance = Information.Parts.KindInheritance.SELF;
										informationParts.FlagInheritance = Information.Parts.FlagBitInheritance.PRESET;
									}
									else
									{
										informationParts.Inheritance = Information.Parts.KindInheritance.PARENT;
										informationParts.FlagInheritance = Information.Parts.FlagBitInheritance.CLEAR;
									}
									break;
							}
						}
						break;

					case "self":
						{
							switch(informationSSAE.Version)
							{
								case KindVersion.CODE_010000:
								case KindVersion.CODE_010001:
									{
										/* MEMO: Default-Value: 0(true) */
										/*       Attributes'-Tag exists when Value is 0(false). */
										informationParts.Inheritance = Information.Parts.KindInheritance.SELF;
										informationParts.FlagInheritance = Information.Parts.FlagBitInheritance.CLEAR;
	
										System.Xml.XmlNode nodeAttribute = null;
										nodeAttribute = LibraryEditor_SpriteStudio6.Utility.XML.NodeGet(nodeParts, "ineheritRates/ALPH", managerNameSpace);
										if(null == nodeAttribute)
										{
											informationParts.FlagInheritance |= Information.Parts.FlagBitInheritance.OPACITY_RATE;
										}
	
										nodeAttribute = LibraryEditor_SpriteStudio6.Utility.XML.NodeGet(nodeParts, "ineheritRates/FLPH", managerNameSpace);
										if(null == nodeAttribute)
										{
											informationParts.FlagInheritance |= Information.Parts.FlagBitInheritance.FLIP_X;
										}
	
										nodeAttribute = LibraryEditor_SpriteStudio6.Utility.XML.NodeGet(nodeParts, "ineheritRates/FLPV", managerNameSpace);
										if(null == nodeAttribute)
										{
											informationParts.FlagInheritance |= Information.Parts.FlagBitInheritance.FLIP_Y;
										}
	
										nodeAttribute = LibraryEditor_SpriteStudio6.Utility.XML.NodeGet(nodeParts, "ineheritRates/HIDE", managerNameSpace);
										if(null == nodeAttribute)
										{
											informationParts.FlagInheritance |= Information.Parts.FlagBitInheritance.SHOW_HIDE;
										}
									}
									break;

								case KindVersion.CODE_010002:
								case KindVersion.CODE_010200:
								case KindVersion.CODE_010201:
								case KindVersion.CODE_010202:
								case KindVersion.CODE_020000:
								case KindVersion.CODE_020001:
								case KindVersion.CODE_020003:
								case KindVersion.CODE_020004:
									{
										/* MEMO: Attributes'-Tag always exists. */
										bool valueBool = false;
	
										informationParts.Inheritance = Information.Parts.KindInheritance.SELF;
										informationParts.FlagInheritance = Information.Parts.FlagBitInheritance.PRESET;
										informationParts.FlagInheritance |= Information.Parts.FlagBitInheritance.FLIP_X;
										informationParts.FlagInheritance |= Information.Parts.FlagBitInheritance.FLIP_Y;
										informationParts.FlagInheritance |= Information.Parts.FlagBitInheritance.SHOW_HIDE;

										valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "ineheritRates/ALPH", managerNameSpace);
										if(null != valueText)
										{
											valueBool = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetBool(valueText);
											if(false == valueBool)
											{
												informationParts.FlagInheritance &= ~Information.Parts.FlagBitInheritance.OPACITY_RATE;
											}
										}
	
										valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "ineheritRates/FLPH", managerNameSpace);
										if(null != valueText)
										{
											valueBool = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetBool(valueText);
											if(false == valueBool)
											{
												informationParts.FlagInheritance &= ~Information.Parts.FlagBitInheritance.FLIP_X;
											}
										}
	
										valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "ineheritRates/FLPV", managerNameSpace);
										if(null != valueText)
										{
											valueBool = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetBool(valueText);
											if(false == valueBool)
											{
												informationParts.FlagInheritance &= ~Information.Parts.FlagBitInheritance.FLIP_Y;
											}
										}
	
										valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "ineheritRates/HIDE", managerNameSpace);
										if(null != valueText)
										{
											valueBool = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetBool(valueText);
											if(false == valueBool)
											{
												informationParts.FlagInheritance &= ~Information.Parts.FlagBitInheritance.SHOW_HIDE;
											}
										}
									}
									break;
							}
						}
						break;

					default:
						LogWarning(messageLogPrefix, "Unknown Inheritance Kind \"" + valueText + "\" Parts[" + indexParts.ToString() + "]", nameFileSSAE, informationSSPJ);
						goto case "parent";
				}

				/* Get Target-Blending */
				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "alphaBlendType", managerNameSpace);
				switch(valueText)
				{
					case "mix":
						informationParts.Data.OperationBlendTarget = Library_SpriteStudio6.KindOperationBlend.MIX;
						break;

					case "mul":
						informationParts.Data.OperationBlendTarget = Library_SpriteStudio6.KindOperationBlend.MUL_NA;
						break;

					case "add":
						informationParts.Data.OperationBlendTarget = Library_SpriteStudio6.KindOperationBlend.ADD;
						break;

					case "sub":
						informationParts.Data.OperationBlendTarget = Library_SpriteStudio6.KindOperationBlend.SUB;
						break;

					case "mulalpha":
						/* MEMO: In SS5PU, "mul" was handled as Alpha-Multiply.                            */
						/*       From SS6PU, "mul" and "mulalpha" are devided so it is handled separately. */
						/*       ("mul": Non-Alpha-Multiply / "mulalpha": Alpha-Multiply)                  */
						informationParts.Data.OperationBlendTarget = Library_SpriteStudio6.KindOperationBlend.MUL;
						break;

					case "screen":
						informationParts.Data.OperationBlendTarget = Library_SpriteStudio6.KindOperationBlend.SCR;
						break;

					case "exclusion":
						informationParts.Data.OperationBlendTarget = Library_SpriteStudio6.KindOperationBlend.EXC;
						break;

					case "invert":
						informationParts.Data.OperationBlendTarget = Library_SpriteStudio6.KindOperationBlend.INV;
						break;

					default:
						LogWarning(messageLogPrefix, "Unknown Alpha-Blend Kind \"" + valueText + "\" Parts[" + indexParts.ToString() + "]", nameFileSSAE, informationSSPJ);
						goto case "mix";
				}

				/* Get Parts Show(Hide) */
				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "show", managerNameSpace);
				if(false == string.IsNullOrEmpty(valueText))
				{
					informationParts.FlagHide = !(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetBool(valueText));	/* Show -> Hide */
				}
				else
				{	/* Legacy format */
					informationParts.FlagHide = false;
				}

				/* Get Mask-Targeting */
				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "maskInfluence", managerNameSpace);
				if(false == string.IsNullOrEmpty(valueText))
				{
					informationParts.FlagMasking = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetBool(valueText);
				}
				else
				{	/* Legacy format */
					informationParts.FlagMasking = false;
				}

				/* UnderControl Data Get */
				/* MEMO: Type of data under control is determined uniquely according to Part-Type. (Mutually exclusive) */
				switch(informationParts.Data.Feature)
				{
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE:
						/* Instance-Animation Datas Get */
						valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "refAnimePack", managerNameSpace);
						if(null != valueText)
						{
							/* MEMO: CAUTION! Store only the name, because index of animation referring to can not be decided here. */
							/*       (Determined at "ModeSS6PU.ConvertData".)                                                       */
							informationParts.NameUnderControl = valueText;

							/* MEMO: Store animation names paired with references of undercontrol object("informationParts.Data.PrefabUnderControl") */
							/*        to other variable(not "informationParts.Data.NameAnimationUnderControl").                                      */
							/*       (Because I think that pair data should be stored in same place)                                                 */
							valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "refAnime", managerNameSpace);
							informationParts.NameAnimationUnderControl = (null != valueText) ? string.Copy(valueText) : "";
						}
						break;

					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT:
						/* Get Effect Datas */
						valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "refEffectName", managerNameSpace);
						if(null != valueText)
						{
							/* MEMO: Tag present but value may be empty. */
							if(false == string.IsNullOrEmpty(valueText))
							{
								/* MEMO: CAUTION! Store only the name, because index of animation referring to can not be decided here. */
								/*       (Determined at "ModeSS6PU.ConvertData".)                                                       */
								informationParts.NameUnderControl = string.Copy(valueText);
								informationParts.NameAnimationUnderControl = "";
							}
						}
						break;

					default:
						break;
				}

				/* Get Color-Label */
				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "colorLabel", managerNameSpace);
				if(null == valueText)
				{
					informationParts.Data.LabelColor.Set(Library_SpriteStudio6.Data.Parts.Animation.ColorLabel.KindForm.NON);
				}
				else
				{
					switch(valueText)
					{
						case "Red":
							informationParts.Data.LabelColor.Set(Library_SpriteStudio6.Data.Parts.Animation.ColorLabel.KindForm.RED);
							break;

						case "Orange":
							informationParts.Data.LabelColor.Set(Library_SpriteStudio6.Data.Parts.Animation.ColorLabel.KindForm.ORANGE);
							break;

						case "Yellow":
							informationParts.Data.LabelColor.Set(Library_SpriteStudio6.Data.Parts.Animation.ColorLabel.KindForm.YELLOW);
							break;

						case "Green":
							informationParts.Data.LabelColor.Set(Library_SpriteStudio6.Data.Parts.Animation.ColorLabel.KindForm.GREEN);
							break;

						case "Blue":
							informationParts.Data.LabelColor.Set(Library_SpriteStudio6.Data.Parts.Animation.ColorLabel.KindForm.BLUE);
							break;

						case "Violet":
							informationParts.Data.LabelColor.Set(Library_SpriteStudio6.Data.Parts.Animation.ColorLabel.KindForm.VIOLET);
							break;

						case "Gray":
							informationParts.Data.LabelColor.Set(Library_SpriteStudio6.Data.Parts.Animation.ColorLabel.KindForm.GRAY);
							break;

						default:
							{
								float ColorA;
								float ColorR;
								float ColorG;
								float ColorB;
								if(false == LibraryEditor_SpriteStudio6.Utility.Text.TextToColor(out ColorA, out ColorR, out ColorG, out ColorB, valueText))
								{
									LogWarning(messageLogPrefix, "Unknown Color-Label Kind \"" + valueText + "\" Parts[" + indexParts.ToString() + "]", nameFileSSAE, informationSSPJ);
									informationParts.Data.LabelColor.Set(Library_SpriteStudio6.Data.Parts.Animation.ColorLabel.KindForm.NON);
								}
								else
								{
									informationParts.Data.LabelColor.Set(new Color(ColorR, ColorG, ColorB, ColorA));
								}
							}
							break;
					}
				}

				/* Get Bone-Data */
				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "boneLength", managerNameSpace);
				if(false == string.IsNullOrEmpty(valueText))
				{
					informationParts.LengthBone = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueText);
				}
				else
				{
					informationParts.LengthBone = 0.0f;
				}

				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "bonePosition", managerNameSpace);
				if(false == string.IsNullOrEmpty(valueText))
				{
					string[] valueTextSplit = valueText.Split(' ');
					informationParts.PositionXBone = (float)LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueTextSplit[0]);
					informationParts.PositionYBone = (float)LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueTextSplit[1]);
				}
				else
				{
					informationParts.PositionXBone = 0.0f;
					informationParts.PositionYBone = 0.0f;
				}

				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "boneRotation", managerNameSpace);
				if(false == string.IsNullOrEmpty(valueText))
				{
					informationParts.RotateZBone = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueText);
				}
				else
				{
					informationParts.RotateZBone = 0.0f;
				}

				return(informationParts);

			ParseParts_ErrorEnd:;
				return(null);
			}

			private static Information.Animation ParseAnimation(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
																	out bool flagIsSetup,
																	LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
																	System.Xml.XmlNode nodeAnimation,
																	System.Xml.XmlNamespaceManager managerNameSpace,
																	Information informationSSAE,
																	int indexAnimation,
																	string nameFileSSAE
																)
			{
				const string messageLogPrefix = "Parse SSAE(Animation)";

				flagIsSetup = false;

				Information.Animation informationAnimation = new Information.Animation();
				if(null == informationAnimation)
				{
					LogError(messageLogPrefix, "Not Enough Memory (Parts WorkArea) Animation[" + indexAnimation.ToString() + "]", nameFileSSAE, informationSSPJ);
					goto ParseAnimation_ErrorEnd;
				}
				informationAnimation.CleanUp();

				/* Get Base Datas */
				string valueText;
				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAnimation, "name", managerNameSpace);
				informationAnimation.Data.Name = string.Copy(valueText);

				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAnimation, "isSetup", managerNameSpace);
				if(false == string.IsNullOrEmpty(valueText))
				{
					if(0 < LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText))
					{
						flagIsSetup = true;
					}
				}

				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAnimation, "settings/fps", managerNameSpace);
				informationAnimation.Data.FramePerSecond = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);

				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAnimation, "settings/frameCount", managerNameSpace);
				informationAnimation.Data.CountFrame = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);

				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAnimation, "settings/sortMode", managerNameSpace);
				switch(valueText)
				{
					case "prio":
						informationAnimation.Data.ModeSort = Library_SpriteStudio6.Data.Animation.KindModeSort.PRIORITY;
						break;
					case "z":
						informationAnimation.Data.ModeSort = Library_SpriteStudio6.Data.Animation.KindModeSort.POSITION_Z;
						break;
					default:
						goto case "prio";
				}

				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAnimation, "settings/canvasSize", managerNameSpace);
				if(false == string.IsNullOrEmpty(valueText))
				{
					string[] valueTextSplit = valueText.Split(' ');
					informationAnimation.Data.SizeCanvasX = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueTextSplit[0]);
					informationAnimation.Data.SizeCanvasY = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueTextSplit[1]);
				}

				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAnimation, "settings/startFrame", managerNameSpace);
				if(false == string.IsNullOrEmpty(valueText))
				{
					informationAnimation.Data.FrameValidStart = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);
				}
				else
				{
					informationAnimation.Data.FrameValidStart = 0;
				}

				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAnimation, "settings/endFrame", managerNameSpace);
				if(false == string.IsNullOrEmpty(valueText))
				{
					informationAnimation.Data.FrameValidEnd = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);
				}
				else
				{
					informationAnimation.Data.FrameValidEnd = informationAnimation.Data.CountFrame - 1;
				}

				informationAnimation.Data.CountFrameValid = (informationAnimation.Data.FrameValidEnd - informationAnimation.Data.FrameValidStart) + 1;

				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAnimation, "settings/ik_depth", managerNameSpace);
				if(false == string.IsNullOrEmpty(valueText))
				{
					informationAnimation.Data.DepthIK = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);
				}
				else
				{
					informationAnimation.Data.DepthIK = 3;	/* Default */
				}

				/* Get Labels */
				List<Library_SpriteStudio6.Data.Animation.Label> listLabel = new List<Library_SpriteStudio6.Data.Animation.Label>();
				if(null == listLabel)
				{
					LogError(messageLogPrefix, "Not Enough Memory (Animation-Label WorkArea) Animation[" + indexAnimation.ToString() + "]", nameFileSSAE, informationSSPJ);
					goto ParseAnimation_ErrorEnd;
				}
				listLabel.Clear();

				Library_SpriteStudio6.Data.Animation.Label label = new Library_SpriteStudio6.Data.Animation.Label();
				System.Xml.XmlNodeList nodeListLabel = LibraryEditor_SpriteStudio6.Utility.XML.ListGetNode(nodeAnimation, "labels/value", managerNameSpace);
				foreach(System.Xml.XmlNode nodeLabel in nodeListLabel)
				{
					valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeLabel, "name", managerNameSpace);
					if(0 > Library_SpriteStudio6.Data.Animation.Label.NameCheckReserved(valueText))
					{
						label.CleanUp();
						label.Name = string.Copy(valueText);

						valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeLabel, "time", managerNameSpace);
						label.Frame = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);

						listLabel.Add(label);
					}
					else
					{
						LogWarning(messageLogPrefix, "Used reserved Label-Name \"" + valueText + "\" Animation[" + indexAnimation.ToString() + "]", nameFileSSAE, informationSSPJ);
					}
				}
				informationAnimation.Data.TableLabel = listLabel.ToArray();
				listLabel.Clear();
				listLabel = null;

				/* Get Key-Frames */
				int countParts = informationSSAE.TableParts.Length;
				informationAnimation.TableParts = new Information.Animation.Parts[countParts];
				if(null == informationAnimation.TableParts)
				{
					LogError(messageLogPrefix, "Not Enough Memory (Animation Part Data) Animation[" + indexAnimation.ToString() + "]", nameFileSSAE, informationSSPJ);
					goto ParseAnimation_ErrorEnd;
				}
				/* MEMO: All animation part information is created here. Because parts-animation that has no key-data are not recorded in SSAE. */
				for(int i=0; i<countParts; i++)
				{
					informationAnimation.TableParts[i] = new Information.Animation.Parts();
					if(null == informationAnimation.TableParts[i])
					{
						LogError(messageLogPrefix, "Not Enough Memory (Animation Attribute WorkArea) Animation-Name[" + indexAnimation.ToString() + "]", nameFileSSAE, informationSSPJ);
						goto ParseAnimation_ErrorEnd;
					}
					informationAnimation.TableParts[i].CleanUp();
					informationAnimation.TableParts[i].BootUp();
				}

				System.Xml.XmlNodeList nodeListAnimationParts = LibraryEditor_SpriteStudio6.Utility.XML.ListGetNode(nodeAnimation, "partAnimes/partAnime", managerNameSpace);
				if(null == nodeListAnimationParts)
				{
					LogError(messageLogPrefix, "PartAnimation Node Not Found Animation[" + indexAnimation.ToString() + "]", nameFileSSAE, informationSSPJ);
					goto ParseAnimation_ErrorEnd;
				}
				int indexParts = -1;
				foreach(System.Xml.XmlNode nodeAnimationPart in nodeListAnimationParts)
				{
					valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAnimationPart, "partName", managerNameSpace);
					indexParts = informationSSAE.IndexGetParts(valueText);
					if(-1 == indexParts)
					{
						LogError(messageLogPrefix, "Part's Name Not Found \"" + valueText + "\" Animation[" + indexAnimation.ToString() + "]", nameFileSSAE, informationSSPJ);
						goto ParseAnimation_ErrorEnd;
					}

					System.Xml.XmlNode nodeAnimationAttributes = LibraryEditor_SpriteStudio6.Utility.XML.NodeGet(nodeAnimationPart, "attributes", managerNameSpace);
					if(false ==  ParseAnimationAttribute(	ref setting,
															informationSSPJ,
															nodeAnimationAttributes,
															managerNameSpace,
															informationSSAE,
															informationAnimation,
															indexParts,
															flagIsSetup,
															nameFileSSAE
														)
						)
					{
						goto ParseAnimation_ErrorEnd;
					}
				}

				return(informationAnimation);

			ParseAnimation_ErrorEnd:;
				return(null);
			}

			private static bool ParseAnimationAttribute(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
															LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
															System.Xml.XmlNode nodeAnimationAttributes,
															System.Xml.XmlNamespaceManager managerNameSpace,
															Information informationSSAE,
															Information.Animation informationAnimation,
															int indexParts,
															bool flagIsSetup,
															string nameFileSSAE
														)
			{
				const string messageLogPrefix = "Parse SSAE(Attributes)";

				Information.Animation.Parts informationAnimationParts = informationAnimation.TableParts[indexParts];

				/* Set Part's Status */
				/* MEMO: Set here at least "Not Used" flag. When this function is called, key-data exists in this part. */
				/*       Other flags are set in "Information.Animation.StatusSetParts".                                 */
				informationAnimationParts.StatusParts &= ~Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NOT_USED;

				/* Set Inheritance */
				Information.Parts parts = informationSSAE.TableParts[indexParts];
				int indexPartsParent = parts.Data.IDParent;
				if(0 <= indexPartsParent)
				{
					Information.Animation.Parts informationAnimationPartsParent = informationAnimation.TableParts[indexPartsParent];
					if(0 != (parts.FlagInheritance & Information.Parts.FlagBitInheritance.OPACITY_RATE))
					{
						informationAnimationParts.RateOpacity.Parent = informationAnimationPartsParent.RateOpacity;
					}
					if(0 != (parts.FlagInheritance & Information.Parts.FlagBitInheritance.SHOW_HIDE))
					{
						informationAnimationParts.Hide.Parent = informationAnimationPartsParent.Hide;
					}
					if(0 != (parts.FlagInheritance & Information.Parts.FlagBitInheritance.FLIP_X))
					{
						informationAnimationParts.FlipX.Parent = informationAnimationPartsParent.FlipX;
					}
					if(0 != (parts.FlagInheritance & Information.Parts.FlagBitInheritance.FLIP_Y))
					{
						informationAnimationParts.FlipY.Parent = informationAnimationPartsParent.FlipY;
					}
				}

				/* Get KeyFrame List */
				string tagText;
				string valueText;
				System.Xml.XmlNodeList listNodeAttribute = LibraryEditor_SpriteStudio6.Utility.XML.ListGetNode(nodeAnimationAttributes, "attribute", managerNameSpace);
				foreach(System.Xml.XmlNode nodeAttribute in listNodeAttribute)
				{
					/* Get Attribute-Tag */
					tagText = nodeAttribute.Attributes["tag"].Value;

					/* Get Key-Data List */
					System.Xml.XmlNodeList listNodeKey = LibraryEditor_SpriteStudio6.Utility.XML.ListGetNode(nodeAttribute, "key", managerNameSpace);
					if(null == listNodeKey)
					{
						LogWarning(messageLogPrefix, "Attribute \"" + tagText + "\" has no Key-Frame Animation-Name[" + informationAnimation.Data.Name + "]", nameFileSSAE, informationSSPJ);
						continue;
					}

					/* Get Key-Data */
					System.Xml.XmlNode nodeInterpolation = null;
					int frame = -1;
					Library_SpriteStudio6.Utility.Interpolation.KindFormula formula = Library_SpriteStudio6.Utility.Interpolation.KindFormula.NON;
					bool flagHasParameterCurve = false;
					float frameCurveStart = 0.0f;
					float valueCurveStart = 0.0f;
					float frameCurveEnd = 0.0f;
					float valueCurveEnd = 0.0f;
					string[] valueTextSplit = null;

					Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool attributeBool = null;
					Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeInt attributeInt = null;
					Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat attributeFloat = null;
					foreach(System.Xml.XmlNode nodeKey in listNodeKey)
					{
						/* Get Interpolation(Curve) Parameters */
						frame = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(nodeKey.Attributes["time"].Value);
						formula = Library_SpriteStudio6.Utility.Interpolation.KindFormula.NON;
						flagHasParameterCurve = false;
						frameCurveStart = 0.0f;
						valueCurveStart = 0.0f;
						frameCurveEnd = 0.0f;
						valueCurveEnd = 0.0f;
						nodeInterpolation = nodeKey.Attributes["ipType"];
						if(null != nodeInterpolation)
						{
							valueText = string.Copy(nodeInterpolation.Value);
							switch(valueText)
							{
								case "linear":
									formula = Library_SpriteStudio6.Utility.Interpolation.KindFormula.LINEAR;
									flagHasParameterCurve = false;
									break;

								case "hermite":
									formula = Library_SpriteStudio6.Utility.Interpolation.KindFormula.HERMITE;
									flagHasParameterCurve = true;
									break;

								case "bezier":
									formula = Library_SpriteStudio6.Utility.Interpolation.KindFormula.BEZIER;
									flagHasParameterCurve = true;
									break;

								case "acceleration":
									formula = Library_SpriteStudio6.Utility.Interpolation.KindFormula.ACCELERATE;
									flagHasParameterCurve = false;
									break;

								case "deceleration":
									formula = Library_SpriteStudio6.Utility.Interpolation.KindFormula.DECELERATE;
									flagHasParameterCurve = false;
									break;

								default:
									LogWarning(messageLogPrefix, "Unknown Interpolation \"" + valueText + "\" Frame[" + frame.ToString() + "] Animation-Name[" + informationAnimation.Data.Name + "]", nameFileSSAE, informationSSPJ);
									formula = Library_SpriteStudio6.Utility.Interpolation.KindFormula.NON;
									flagHasParameterCurve = false;
									break;
							}
							if(true == flagHasParameterCurve)
							{
								valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "curve", managerNameSpace);
								if(null == valueText)
								{
									LogWarning(messageLogPrefix, "Interpolation \"" + valueText + "\" Parameter Missing Frame[" + frame.ToString() + "] Animation-Name[" + informationAnimation.Data.Name + "]", nameFileSSAE, informationSSPJ);
									formula = Library_SpriteStudio6.Utility.Interpolation.KindFormula.NON;
									flagHasParameterCurve = false;
									frameCurveStart = 0.0f;
									valueCurveStart = 0.0f;
									frameCurveEnd = 0.0f;
									valueCurveEnd = 0.0f;
								}
								else
								{
									valueTextSplit = valueText.Split(' ');
									frameCurveStart = (float)LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[0]);
									valueCurveStart = (float)LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[1]);
									frameCurveEnd = (float)LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[2]);
									valueCurveEnd = (float)LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[3]);
								}
							}
						}

						/* Get Attribute Data */
						switch(tagText)
						{
							/* Bool-Value Attributes */
							case "HIDE":
								attributeBool = informationAnimationParts.Hide;
								goto case "_ValueBool_";
							case "FLPH":
								attributeBool = informationAnimationParts.FlipX;
								goto case "_ValueBool_";
							case "FLPV":
								attributeBool = informationAnimationParts.FlipY;
								goto case "_ValueBool_";
							case "IFLH":
								attributeBool = informationAnimationParts.TextureFlipX;
								goto case "_ValueBool_";
							case "IFLV":
								attributeBool = informationAnimationParts.TextureFlipY;
								goto case "_ValueBool_";

							case "_ValueBool_":
								{
									Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool.KeyData data = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool.KeyData();

									/* Set Interpolation-Data */
									/* MEMO: Bool-Value can't have interpolation */
									data.Formula = Library_SpriteStudio6.Utility.Interpolation.KindFormula.NON;
									data.FrameCurveStart = 0.0f;
									data.ValueCurveStart = 0.0f;
									data.FrameCurveEnd = 0.0f;
									data.ValueCurveEnd = 0.0f;

									/* Set Body-Data */
									data.Frame = frame;

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value", managerNameSpace);
									data.Value = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetBool(valueText);

									/* Add Key-Data */
									attributeBool.ListKey.Add(data);
								}
								break;

							/* Int-Value Attributes */
							case "PRIO":
								attributeInt = informationAnimationParts.Priority;
								goto case "_ValueInt_";

							case "_ValueInt_":
								{
									Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeInt.KeyData data = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeInt.KeyData();

									/* Set Interpolation-Data */
									data.Formula = formula;
									data.FrameCurveStart = frameCurveStart;
									data.ValueCurveStart = valueCurveStart;
									data.FrameCurveEnd = frameCurveEnd;
									data.ValueCurveEnd = valueCurveEnd;

									/* Set Body-Data */
									data.Frame = frame;

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value", managerNameSpace);
									data.Value = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);

									/* Add Key-Data */
									attributeInt.ListKey.Add(data);
								}
								break;

							/* Float-Value Attributes */
							case "POSX":
								attributeFloat = informationAnimationParts.PositionX;
								goto case "_ValueFloat_";
							case "POSY":
								attributeFloat = informationAnimationParts.PositionY;
								goto case "_ValueFloat_";
							case "POSZ":
								attributeFloat = informationAnimationParts.PositionZ;
								goto case "_ValueFloat_";
							case "ROTX":
								attributeFloat = informationAnimationParts.RotationX;
								goto case "_ValueFloat_";
							case "ROTY":
								attributeFloat = informationAnimationParts.RotationY;
								goto case "_ValueFloat_";
							case "ROTZ":
								attributeFloat = informationAnimationParts.RotationZ;
								goto case "_ValueFloat_";
							case "SCLX":
								attributeFloat = informationAnimationParts.ScalingX;
								goto case "_ValueFloat_";
							case "SCLY":
								attributeFloat = informationAnimationParts.ScalingY;
								goto case "_ValueFloat_";
							case "LSCX":
								attributeFloat = informationAnimationParts.ScalingXLocal;
								goto case "_ValueFloat_";
							case "LSCY":
								attributeFloat = informationAnimationParts.ScalingYLocal;
								goto case "_ValueFloat_";
							case "ALPH":
								attributeFloat = informationAnimationParts.RateOpacity;
								goto case "_ValueFloat_";
							case "LALP":
								attributeFloat = informationAnimationParts.RateOpacityLocal;
								goto case "_ValueFloat_";
							case "PVTX":
								attributeFloat = informationAnimationParts.PivotOffsetX;
								goto case "_ValueFloat_";
							case "PVTY":
								attributeFloat = informationAnimationParts.PivotOffsetY;
								goto case "_ValueFloat_";
							case "ANCX":
								attributeFloat = informationAnimationParts.AnchorPositionX;
								goto case "_ValueFloat_";
							case "ANCY":
								attributeFloat = informationAnimationParts.AnchorPositionY;
								goto case "_ValueFloat_";
							case "SIZX":
								attributeFloat = informationAnimationParts.SizeForceX;
								goto case "_ValueFloat_";
							case "SIZY":
								attributeFloat = informationAnimationParts.SizeForceY;
								goto case "_ValueFloat_";
							case "UVTX":
								attributeFloat = informationAnimationParts.TexturePositionX;
								goto case "_ValueFloat_";
							case "UVTY":
								attributeFloat = informationAnimationParts.TexturePositionY;
								goto case "_ValueFloat_";
							case "UVRZ":
								attributeFloat = informationAnimationParts.TextureRotation;
								goto case "_ValueFloat_";
							case "UVSX":
								attributeFloat = informationAnimationParts.TextureScalingX;
								goto case "_ValueFloat_";
							case "UVSY":
								attributeFloat = informationAnimationParts.TextureScalingY;
								goto case "_ValueFloat_";
							case "BNDR":
								attributeFloat = informationAnimationParts.RadiusCollision;
								goto case "_ValueFloat_";
							case "MASK":
								attributeFloat = informationAnimationParts.PowerMask;
								goto case "_ValueFloat_";

							case "_ValueFloat_":
								{
									Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat.KeyData data = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat.KeyData();

									/* Set Interpolation-Data */
									data.Formula = formula;
									data.FrameCurveStart = frameCurveStart;
									data.ValueCurveStart = valueCurveStart;
									data.FrameCurveEnd = frameCurveEnd;
									data.ValueCurveEnd = valueCurveEnd;

									/* Set Body-Data */
									data.Frame = frame;

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value", managerNameSpace);
									data.Value = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueText);

									/* Add Key-Data */
									attributeFloat.ListKey.Add(data);
								}
								break;

							/* Uniquet-Value Attributes */
							case "CELL":
								{
									Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeCell.KeyData data = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeCell.KeyData();
									data.Value.CleanUp();

									/* Set Interpolation-Data */
									data.Formula = formula;
									data.FrameCurveStart = frameCurveStart;
									data.ValueCurveStart = valueCurveStart;
									data.FrameCurveEnd = frameCurveEnd;
									data.ValueCurveEnd = valueCurveEnd;

									/* Set Body-Data */
									data.Frame = frame;

									bool flagValidCell = false;
									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/mapId", managerNameSpace);
									if(null == valueText)
									{
										data.Value.IndexCellMap = -1;
										data.Value.IndexCell = -1;
									}
									else
									{
										int indexCellMap = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);
										if(0 > indexCellMap)
										{	/* Invalid Ref.Cell */
											data.Value.IndexCellMap = -1;
											data.Value.IndexCell = -1;
										}
										else
										{
											indexCellMap = informationSSAE.TableIndexCellMap[indexCellMap];
											data.Value.IndexCellMap = indexCellMap;

											valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/name", managerNameSpace);
											if(null == valueText)
											{
												data.Value.IndexCell = -1;
											}
											else
											{
												int indexCell = informationSSPJ.TableInformationSSCE[indexCellMap].IndexGetCell(valueText);
												data.Value.IndexCell = indexCell;
												if(0 <= indexCell)
												{
													flagValidCell = true;
												}
											}
										}
									}
									if(false == flagValidCell)
									{
										LogWarning(messageLogPrefix, "Cell-Data Not Found Frame[" + frame.ToString() + "] Animation-Name[" + informationAnimation.Data.Name + "]", nameFileSSAE, informationSSPJ);
									}

									/* Add Key-Data */
									informationAnimationParts.Cell.ListKey.Add(data);

									/* Add to In-use-Cell-Map information */
									if(0 <= data.Value.IndexCellMap)
									{
										informationSSAE.InUseCellMapAdd(data.Value.IndexCellMap, parts);
									}
								}
								break;

							case "PCOL":
								{
									Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributePartsColor.KeyData data = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributePartsColor.KeyData();
									data.Value.CleanUp();
									data.Value.BootUp((int)Library_SpriteStudio6.KindVertex.TERMINATOR2);

									/* Set Interpolation-Data */
									data.Formula = formula;
									data.FrameCurveStart = frameCurveStart;
									data.ValueCurveStart = valueCurveStart;
									data.FrameCurveEnd = frameCurveEnd;
									data.ValueCurveEnd = valueCurveEnd;

									/* Set Body-Data */
									data.Frame = frame;

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/blendType", managerNameSpace);
									switch(valueText)
									{
										case "mix":
											data.Value.Operation = Library_SpriteStudio6.KindOperationBlend.MIX;
											break;

										case "mul":
											data.Value.Operation = Library_SpriteStudio6.KindOperationBlend.MUL;
											break;

										case "add":
											data.Value.Operation = Library_SpriteStudio6.KindOperationBlend.ADD;
											break;

										case "sub":
											data.Value.Operation = Library_SpriteStudio6.KindOperationBlend.SUB;
											break;

										default:
											LogWarning(messageLogPrefix, "Unknown PartsColor-Operation \"" + valueText + "\" Frame[" + frame.ToString() + "] Animation-Name[" + informationAnimation.Data.Name + "]", nameFileSSAE, informationSSPJ);
											data.Value.Operation = Library_SpriteStudio6.KindOperationBlend.MIX;
											break;
									}

									float colorA = 0.0f;
									float colorR = 0.0f;
									float colorG = 0.0f;
									float colorB = 0.0f;
									float rateAlpha = 0.0f;
									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/target", managerNameSpace);
									switch(valueText)
									{
										case "whole":
											{
												data.Value.Bound = Library_SpriteStudio6.KindBoundBlend.OVERALL;

												ParseAnimationAttributePartsColor(	out colorA, out colorR, out colorG, out colorB, out rateAlpha, data.Value.Operation, data.Value.Bound, nodeKey, "value/color", managerNameSpace);
												for(int i=0; i<(int)Library_SpriteStudio6.KindVertex.TERMINATOR2; i++)
												{
													data.Value.VertexColor[i].r = colorR;
													data.Value.VertexColor[i].g = colorG;
													data.Value.VertexColor[i].b = colorB;
													data.Value.VertexColor[i].a = colorA;
													data.Value.RateAlpha[i] = rateAlpha;
												}
											}
											break;

										case "vertex":
											{
												data.Value.Bound = Library_SpriteStudio6.KindBoundBlend.VERTEX;

												ParseAnimationAttributePartsColor(out colorA, out colorR, out colorG, out colorB, out rateAlpha, data.Value.Operation, data.Value.Bound, nodeKey, "value/LT", managerNameSpace);
												data.Value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU].r = colorR;
												data.Value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU].g = colorG;
												data.Value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU].b = colorB;
												data.Value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU].a = colorA;
												data.Value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU] = rateAlpha;

												ParseAnimationAttributePartsColor(out colorA, out colorR, out colorG, out colorB, out rateAlpha, data.Value.Operation, data.Value.Bound, nodeKey, "value/RT", managerNameSpace);
												data.Value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RU].r = colorR;
												data.Value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RU].g = colorG;
												data.Value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RU].b = colorB;
												data.Value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RU].a = colorA;
												data.Value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RU] = rateAlpha;

												ParseAnimationAttributePartsColor(out colorA, out colorR, out colorG, out colorB, out rateAlpha, data.Value.Operation, data.Value.Bound, nodeKey, "value/RB", managerNameSpace);
												data.Value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RD].r = colorR;
												data.Value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RD].g = colorG;
												data.Value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RD].b = colorB;
												data.Value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RD].a = colorA;
												data.Value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RD] = rateAlpha;

												ParseAnimationAttributePartsColor(out colorA, out colorR, out colorG, out colorB, out rateAlpha, data.Value.Operation, data.Value.Bound, nodeKey, "value/LB", managerNameSpace);
												data.Value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LD].r = colorR;
												data.Value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LD].g = colorG;
												data.Value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LD].b = colorB;
												data.Value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LD].a = colorA;
												data.Value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LD] = rateAlpha;
											}
											break;

										default:
											{
												LogWarning(messageLogPrefix, "Unknown PartsColor-Bound \"" + valueText + "\" Frame[" + frame.ToString() + "] Animation-Name[" + informationAnimation.Data.Name + "]", nameFileSSAE, informationSSPJ);
												data.Value.Bound = Library_SpriteStudio6.KindBoundBlend.OVERALL;
												for(int i=0; i<(int)Library_SpriteStudio6.KindVertex.TERMINATOR2; i++)
												{
													data.Value.VertexColor[i] = Library_SpriteStudio6.Data.Animation.Attribute.ColorClear;
												}
											}
											break;
									}

									/* Add Key-Data */
									informationAnimationParts.PartsColor.ListKey.Add(data);
								}
								break;

							case "VERT":
								{
									Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeVertexCorrection.KeyData data = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeVertexCorrection.KeyData();
									data.Value.CleanUp();
									data.Value.Coordinate = new Vector2[(int)Library_SpriteStudio6.KindVertex.TERMINATOR2];

									/* Set Interpolation-Data */
									data.Formula = formula;
									data.FrameCurveStart = frameCurveStart;
									data.ValueCurveStart = valueCurveStart;
									data.FrameCurveEnd = frameCurveEnd;
									data.ValueCurveEnd = valueCurveEnd;

									/* Set Body-Data */
									data.Frame = frame;

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/LT", managerNameSpace);
									valueTextSplit = valueText.Split(' ');
									data.Value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU].x = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[0]));
									data.Value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU].y = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[1]));

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/RT", managerNameSpace);
									valueTextSplit = valueText.Split(' ');
									data.Value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU].x = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[0]));
									data.Value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU].y = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[1]));

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/RB", managerNameSpace);
									valueTextSplit = valueText.Split(' ');
									data.Value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD].x = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[0]));
									data.Value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD].y = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[1]));

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/LB", managerNameSpace);
									valueTextSplit = valueText.Split(' ');
									data.Value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD].x = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[0]));
									data.Value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD].y = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[1]));

									/* Add Key-Data */
									informationAnimationParts.VertexCorrection.ListKey.Add(data);
								}
								break;

							case "USER":
								{
									Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeUserData.KeyData data = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeUserData.KeyData();
									data.Value.CleanUp();

									/* Set Interpolation-Data */
									/* MEMO: User-Data can't have interpolation */
									data.Formula = Library_SpriteStudio6.Utility.Interpolation.KindFormula.NON;
									data.FrameCurveStart = 0.0f;
									data.ValueCurveStart = 0.0f;
									data.FrameCurveEnd = 0.0f;
									data.ValueCurveEnd = 0.0f;

									/* Set Body-Data */
									data.Frame = frame;

									data.Value.Flags = Library_SpriteStudio6.Data.Animation.Attribute.UserData.FlagBit.CLEAR;
									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/integer", managerNameSpace);
									if(null != valueText)
									{
										data.Value.Flags |= Library_SpriteStudio6.Data.Animation.Attribute.UserData.FlagBit.NUMBER;
										if(false == int.TryParse(valueText, out data.Value.NumberInt))
										{
											uint valueUint = 0;
											if(false == uint.TryParse(valueText, out valueUint))
											{
												LogWarning(messageLogPrefix, "Broken UserData-Integer \"" + valueText + "\" Frame[" + frame.ToString() + "] Animation-Name[" + informationAnimation.Data.Name + "]", nameFileSSAE, informationSSPJ);
												data.Value.Flags &= ~Library_SpriteStudio6.Data.Animation.Attribute.UserData.FlagBit.NUMBER;
												data.Value.NumberInt = 0;
											}
											else
											{
												data.Value.NumberInt = (int)valueUint;
											}
										}
									}
									else
									{
										data.Value.Flags &= ~Library_SpriteStudio6.Data.Animation.Attribute.UserData.FlagBit.NUMBER;
										data.Value.NumberInt = 0;
									}

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/rect", managerNameSpace);
									if(null != valueText)
									{
										valueTextSplit = valueText.Split(' ');
										data.Value.Rectangle.xMin = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[0]));
										data.Value.Rectangle.yMin = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[1]));
										data.Value.Rectangle.xMax = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[2]));
										data.Value.Rectangle.yMax = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[3]));
										data.Value.Flags |= Library_SpriteStudio6.Data.Animation.Attribute.UserData.FlagBit.RECTANGLE;
									}
									else
									{
										data.Value.Rectangle.xMin = 0.0f;
										data.Value.Rectangle.yMin = 0.0f;
										data.Value.Rectangle.xMax = 0.0f;
										data.Value.Rectangle.yMax = 0.0f;
										data.Value.Flags &= ~Library_SpriteStudio6.Data.Animation.Attribute.UserData.FlagBit.RECTANGLE;
									}

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/point", managerNameSpace);
									if(null != valueText)
									{
										valueTextSplit = valueText.Split(' ');
										data.Value.Coordinate.x = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[0]));
										data.Value.Coordinate.y = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueTextSplit[1]));
										data.Value.Flags |= Library_SpriteStudio6.Data.Animation.Attribute.UserData.FlagBit.COORDINATE;
									}
									else
									{
										data.Value.Coordinate.x = 0.0f;
										data.Value.Coordinate.y = 0.0f;
										data.Value.Flags &= ~Library_SpriteStudio6.Data.Animation.Attribute.UserData.FlagBit.COORDINATE;
									}

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/string", managerNameSpace);
									if(null != valueText)
									{
										data.Value.Text = string.Copy(valueText);
										data.Value.Flags |= Library_SpriteStudio6.Data.Animation.Attribute.UserData.FlagBit.TEXT;
									}
									else
									{
										data.Value.Text = null;
										data.Value.Flags &= ~Library_SpriteStudio6.Data.Animation.Attribute.UserData.FlagBit.TEXT;
									}

									/* Add Key-Data */
									data.Value.Flags |= Library_SpriteStudio6.Data.Animation.Attribute.UserData.FlagBit.VALID;
									informationAnimationParts.UserData.ListKey.Add(data);
								}
								break;

							case "IPRM":
								{
									Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeInstance.KeyData data = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeInstance.KeyData();
									data.Value.CleanUp();

									/* Set Interpolation-Data */
									/* MEMO: Instance can't have interpolation */
									data.Formula = Library_SpriteStudio6.Utility.Interpolation.KindFormula.NON;
									data.FrameCurveStart = 0.0f;
									data.ValueCurveStart = 0.0f;
									data.FrameCurveEnd = 0.0f;
									data.ValueCurveEnd = 0.0f;

									/* Set Body-Data */
									data.Frame = frame;

									data.Value.PlayCount = -1;
									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/infinity", managerNameSpace);
									if(null != valueText)
									{
										if(true == LibraryEditor_SpriteStudio6.Utility.Text.ValueGetBool(valueText))
										{	/* Check */
											data.Value.PlayCount = 0;
										}
									}
									if(-1 == data.Value.PlayCount)
									{	/* Loop-Limited */
										valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/loopNum", managerNameSpace);
										data.Value.PlayCount = (null == valueText) ? 1 : LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);
									}

									float SignRateSpeed = 1.0f;
									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/reverse", managerNameSpace);
									if(null != valueText)
									{
										SignRateSpeed = (true == LibraryEditor_SpriteStudio6.Utility.Text.ValueGetBool(valueText)) ? -1.0f : 1.0f;
									}

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/pingpong", managerNameSpace);
									if(null == valueText)
									{
										data.Value.Flags &= ~Library_SpriteStudio6.Data.Animation.Attribute.Instance.FlagBit.PINGPONG;
									}
									else
									{
										data.Value.Flags = (true == LibraryEditor_SpriteStudio6.Utility.Text.ValueGetBool(valueText)) ?
																(data.Value.Flags | Library_SpriteStudio6.Data.Animation.Attribute.Instance.FlagBit.PINGPONG)
																: (data.Value.Flags & ~Library_SpriteStudio6.Data.Animation.Attribute.Instance.FlagBit.PINGPONG);
									}

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/independent", managerNameSpace);
									if(null == valueText)
									{
										data.Value.Flags &= ~Library_SpriteStudio6.Data.Animation.Attribute.Instance.FlagBit.INDEPENDENT;
									}
									else
									{
										data.Value.Flags = (true == LibraryEditor_SpriteStudio6.Utility.Text.ValueGetBool(valueText)) ?
																(data.Value.Flags | Library_SpriteStudio6.Data.Animation.Attribute.Instance.FlagBit.INDEPENDENT)
																: (data.Value.Flags & ~Library_SpriteStudio6.Data.Animation.Attribute.Instance.FlagBit.INDEPENDENT);
									}

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/startLabel", managerNameSpace);
									data.Value.LabelStart = (null == valueText) ? string.Copy(Library_SpriteStudio6.Data.Animation.Label.TableNameLabelReserved[(int)Library_SpriteStudio6.Data.Animation.Label.KindLabelReserved.START]) : string.Copy(valueText);

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/startOffset", managerNameSpace);
									data.Value.OffsetStart = (null == valueText) ? 0 : LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/endLabel", managerNameSpace);
									data.Value.LabelEnd = (null == valueText) ? string.Copy(Library_SpriteStudio6.Data.Animation.Label.TableNameLabelReserved[(int)Library_SpriteStudio6.Data.Animation.Label.KindLabelReserved.END]) : string.Copy(valueText);

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/endOffset", managerNameSpace);
									data.Value.OffsetEnd = (null == valueText) ? 0 : LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/speed", managerNameSpace);
									data.Value.RateTime = (null == valueText) ? 1.0f : (float)LibraryEditor_SpriteStudio6.Utility.Text.ValueGetDouble(valueText);
									data.Value.RateTime *= SignRateSpeed;

									/* Add Key-Data */
									data.Value.Flags |= Library_SpriteStudio6.Data.Animation.Attribute.Instance.FlagBit.VALID;
									informationAnimationParts.Instance.ListKey.Add(data);
								}
								break;

							case "EFCT":
								{
									Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeEffect.KeyData data = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeEffect.KeyData();
									data.Value.CleanUp();

									/* Set Interpolation-Data */
									/* MEMO: Instance can't have interpolation */
									data.Formula = Library_SpriteStudio6.Utility.Interpolation.KindFormula.NON;
									data.FrameCurveStart = 0.0f;
									data.ValueCurveStart = 0.0f;
									data.FrameCurveEnd = 0.0f;
									data.ValueCurveEnd = 0.0f;

									/* Set Body-Data */
									data.Frame = frame;

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/startTime", managerNameSpace);
									data.Value.FrameStart = (null == valueText) ? 0 : LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/speed", managerNameSpace);
									data.Value.RateTime = (null == valueText) ? 1.0f : LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueText);

									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/independent", managerNameSpace);
									if(null == valueText)
									{
										data.Value.Flags &= ~Library_SpriteStudio6.Data.Animation.Attribute.Effect.FlagBit.INDEPENDENT;
									}
									else
									{
										data.Value.Flags = (true == LibraryEditor_SpriteStudio6.Utility.Text.ValueGetBool(valueText)) ?
																(data.Value.Flags | Library_SpriteStudio6.Data.Animation.Attribute.Effect.FlagBit.INDEPENDENT)
																: (data.Value.Flags & ~Library_SpriteStudio6.Data.Animation.Attribute.Effect.FlagBit.INDEPENDENT);
									}

									/* Add Key-Data */
									data.Value.Flags |= Library_SpriteStudio6.Data.Animation.Attribute.Effect.FlagBit.VALID;
									informationAnimationParts.Effect.ListKey.Add(data);
								}
								break;

							case "DEFM":
								{
									Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeDeform.KeyData data = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeDeform.KeyData();
									data.Value.CleanUp();

									/* Set Interpolation-Data */
									/* MEMO: Instance can't have interpolation */
									data.Formula = formula;
									data.FrameCurveStart = frameCurveStart;
									data.ValueCurveStart = valueCurveStart;
									data.FrameCurveEnd = frameCurveEnd;
									data.ValueCurveEnd = valueCurveEnd;

									/* Set Body-Data */
									data.Frame = frame;

#if CHANGE_DEFORM_DECODING
									/* MEMO: "value/vsize" depends on the state when key-data was created on SpritStudio6.                                        */
									/*       (Even when value is zero, No necessarily the "number of Cell-Mesh's vertices" to be used.                            */
									/*        Especially when keys are repeatedly "copy-and-paste"-ed between parts, the value conflicts with the specification.) */
									/*       ... So ignore this value.                                                                                            */
#else
									valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/vsize", managerNameSpace);
									int countVertexMesh = (null == valueText) ? 0 : LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);
									data.Value.CountVertexMesh = countVertexMesh;
									bool flagValidVertexSize = true;
									if(0 >= parts.CountVertexDeform)
									{
										/* Set initial Vertex-Size to part */
										parts.CountVertexDeform = countVertexMesh;
									}
									else
									{
										/* Check part's Vertex-Size already set */
										if(parts.CountVertexDeform != countVertexMesh)
										{
											LogWarning(messageLogPrefix, "Invalid Deform's Verex-Size. Frame[" + frame.ToString() + "] Animation-Name[" + informationAnimation.Data.Name + "]", nameFileSSAE, informationSSPJ);
											flagValidVertexSize = false;
										}
									}

									if(true == flagValidVertexSize)
#endif
									{
										bool flagDataValid = false;
										valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeKey, "value/vchg", managerNameSpace);
										if(null != valueText)
										{
											/* MEMO: Deform data format is in following.                                                                       */
											/*       n [Vertex-Index Coordinate-X Coordinate-Y] [Vertex-Index Coordinate-X Coordinate-Y]... (Repeat "n" times) */
											valueTextSplit = valueText.Split(' ');
											int countDataVertex = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueTextSplit[0]);
											if(0 >= countDataVertex)
											{
												/* MEMO: Data whose "countDataVertex is 0" is permitted existing. */
												data.Value.TableVertex = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.DataDeform.Vertex[0];
												flagDataValid = true;
											}
											else
											{
												if(((countDataVertex * 3) + 1) <= valueTextSplit.Length)
												{
													data.Value.TableVertex = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.DataDeform.Vertex[countDataVertex];

													int indexText;
													for(int i=0; i<countDataVertex; i++)
													{
														indexText = (i * 3) + 1;	/* "* 3": Vertex-Index,Coordinate-X,Coordinate-Y */ 
														data.Value.TableVertex[i].Index = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueTextSplit[indexText + 0]);
														data.Value.TableVertex[i].Coordinate.x = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueTextSplit[indexText + 1]);
														data.Value.TableVertex[i].Coordinate.y = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueTextSplit[indexText + 2]);
													}

													flagDataValid = true;
												}
											}
										}
										if(false == flagDataValid)
										{
											LogWarning(messageLogPrefix, "Broken Deform data. Frame[" + frame.ToString() + "] Animation-Name[" + informationAnimation.Data.Name + "]", nameFileSSAE, informationSSPJ);
											data.Value.TableVertex = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.DataDeform.Vertex[0];
										}

										/* Add Key-Data */
										informationAnimationParts.Deform.ListKey.Add(data);
									}
								}
								break;

							/* Disused(Legacy) Attributes */
							case "IMGX":
							case "IMGY":
							case "IMGW":
							case "IMGH":
							case "ORFX":
							case "ORFY":
								LogWarning(messageLogPrefix, "No-Longer-Used attribute \"" + tagText + "\" Animation-Name[" + informationAnimation.Data.Name + "]", nameFileSSAE, informationSSPJ);
								break;

							case "VCOL":
								LogWarning(messageLogPrefix, "Deprecated attribute \"Color Blend\" (Data is ignored. Please use \"Parts Color\") Animation-Name[" + informationAnimation.Data.Name + "]", nameFileSSAE, informationSSPJ);
								break;

							/* Unknown Attributes */
							default:
								LogWarning(messageLogPrefix, "Unknown Attribute \"" + tagText + "\" Animation-Name[" + informationAnimation.Data.Name + "]", nameFileSSAE, informationSSPJ);
								break;
						}
					}
				}

				return(true);

//			ParseAnimationAttribute_ErrorEnd:;
//				return(false);
			}
			private static void ParseAnimationAttributePartsColor(	out float colorA,
																	out float colorR,
																	out float colorG,
																	out float colorB,
																	out float rateAlpha,
																	Library_SpriteStudio6.KindOperationBlend operation,
																	Library_SpriteStudio6.KindBoundBlend bound,
																	System.Xml.XmlNode NodeKey,
																	string NameTagBase,
																	System.Xml.XmlNamespaceManager ManagerNameSpace
																)
			{
				string valueText = "";
				float dataR;
				float dataG;
				float dataB;
				float dataA;
				float dataRate;

				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(NodeKey, NameTagBase + "/rgba", ManagerNameSpace);
				if(false == string.IsNullOrEmpty(valueText))
				{
					LibraryEditor_SpriteStudio6.Utility.Text.TextToColor(out dataA, out dataR, out dataG, out dataB, valueText);
				}
				else
				{
					dataR = 0.0f;
					dataG = 0.0f;
					dataB = 0.0f;
					dataA = 0.0f;
				}

				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(NodeKey, NameTagBase + "/rate", ManagerNameSpace);
				if(false == string.IsNullOrEmpty(valueText))
				{
					dataRate = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueText);
				}
				else
				{
					dataRate = 1.0f;
				}

				/* MEMO: Caution the specification when blend is 'Mix'. */
				switch(operation)
				{
					case Library_SpriteStudio6.KindOperationBlend.MIX:
						switch(bound)
						{
							case Library_SpriteStudio6.KindBoundBlend.OVERALL:
								/* MEMO:  */
								colorA = dataRate;
								colorR = dataR;
								colorG = dataG;
								colorB = dataB;
								rateAlpha = dataA;
								break;

							case Library_SpriteStudio6.KindBoundBlend.VERTEX:
								colorA = dataA;
								colorR = dataR;
								colorG = dataG;
								colorB = dataB;
								rateAlpha = dataRate;	/* 1.0f */
								break;

							default:
								/* MEMO: Not reach here. (To avoid warning) */
								goto case Library_SpriteStudio6.KindBoundBlend.OVERALL;
						}
						return;

					case Library_SpriteStudio6.KindOperationBlend.ADD:
					case Library_SpriteStudio6.KindOperationBlend.SUB:
					case Library_SpriteStudio6.KindOperationBlend.MUL:
						break;
				}

				colorA = 1.0f;	/* dataRate */
				colorR = dataR;
				colorG = dataG;
				colorB = dataB;
				rateAlpha = dataA;
			}

			private static void LogError(string messagePrefix, string message, string nameFile, LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ)
			{
				LibraryEditor_SpriteStudio6.Utility.Log.Error(	messagePrefix
																+ ": " + message
																+ " [" + nameFile + "]"
																+ " in \"" + informationSSPJ.FileNameGetFullPath() + "\""
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
				CODE_010001 = 0x00010001,
				CODE_010002 = 0x00010002,	/* after SS5.3.5 */
				CODE_010200 = 0x00010200,	/* after SS5.5.0 beta-3 */
				CODE_010201 = 0x00010201,	/* after SS5.7.0 beta-1 */
				CODE_010202 = 0x00010202,	/* after SS5.7.0 beta-2 */
				CODE_020000 = 0x00020000,	/* after SS6.0.0 beta */
				CODE_020001 = 0x00020001,	/* after SS6.0.0 */
				CODE_020003 = 0x00020003,	/* after SS6.2.0 */
				CODE_020004 = 0x00020004,	/* after SS6.3.0 */

				TARGET_EARLIEST = CODE_020000,
				TARGET_LATEST = CODE_020004
			};

			private const string ExtentionFile = ".ssae";
			#endregion Enums & Constants

			/* ----------------------------------------------- Classes, Structs & Interfaces */
			#region Classes, Structs & Interfaces
			public class Information
			{
				/* ----------------------------------------------- Variables & Properties */
				#region Variables & Properties
				public LibraryEditor_SpriteStudio6.Import.SSAE.KindVersion Version;

				public string NameDirectory;
				public string NameFileBody;
				public string NameFileExtension;

				public Parts[] TableParts;
				public Catalog.Parts CatalogParts;

				public int[] TableIndexCellMap;
				public Animation[] TableAnimation;
				public Animation AnimationSetup;
				public List<InUseCellMap> ListInUseCellMap;

				public int OffsetIndexBone;
				public List<string> ListBone;
				public List<BindMesh> ListBindMesh;

				public LibraryEditor_SpriteStudio6.Import.Assets<Script_SpriteStudio6_DataAnimation> DataAnimationSS6PU;
#if !_FOR_SS6CONVERTER_
				public LibraryEditor_SpriteStudio6.Import.Assets<Object> PrefabAnimationSS6PU;
				public string NameGameObjectAnimationSS6PU;
				public LibraryEditor_SpriteStudio6.Import.Assets<Object> PrefabControlAnimationSS6PU;
				public string NameGameObjectAnimationControlSS6PU;

				public LibraryEditor_SpriteStudio6.Import.Assets<AnimationClip> DataAnimationUnityNative;
				public LibraryEditor_SpriteStudio6.Import.Assets<Object> PrefabAnimationUnityNative;
				public string NameGameObjectAnimationUnityNative;
				public LibraryEditor_SpriteStudio6.Import.Assets<Object> PrefabControlAnimationUnityNative;
				public string NameGameObjectAnimationControlUnityNative;
				public Transform[] TableTransformBoneUnityNative;
				public int[] TableIDPartsBoneUnityNative;
				public int CountDrawPartsMaxUnityNative;
#endif
				#endregion Variables & Properties

				/* ----------------------------------------------- Functions */
				#region Functions
				public void CleanUp()
				{
					Version = LibraryEditor_SpriteStudio6.Import.SSAE.KindVersion.ERROR;

					NameDirectory = "";
					NameFileBody = "";
					NameFileExtension = "";

					TableParts = null;
					CatalogParts.CleanUp();

					TableIndexCellMap = null;
					TableAnimation = null;
					AnimationSetup = null;
					ListInUseCellMap = null;

					OffsetIndexBone = 0;
					ListBone = null;
					ListBindMesh = null;

					DataAnimationSS6PU.CleanUp();
					DataAnimationSS6PU.BootUp(1);	/* Always 1 */
#if !_FOR_SS6CONVERTER_
					PrefabAnimationSS6PU.CleanUp();
					PrefabAnimationSS6PU.BootUp(1);	/* Always 1 */
					NameGameObjectAnimationSS6PU = "";
					PrefabControlAnimationSS6PU.CleanUp();
					PrefabControlAnimationSS6PU.BootUp(1);	/* Always 1 */
					NameGameObjectAnimationControlSS6PU = "";

					DataAnimationUnityNative.CleanUp();
						/* MEMO: Can not BootUp until SSAE is parsed. */
					PrefabAnimationUnityNative.CleanUp();
					PrefabAnimationUnityNative.BootUp(1);	/* Always 1 */
					NameGameObjectAnimationUnityNative = "";
					PrefabControlAnimationUnityNative.CleanUp();
					PrefabControlAnimationUnityNative.BootUp(1);	/* Always 1 */
					NameGameObjectAnimationControlUnityNative = "";
					TableTransformBoneUnityNative = null;
					TableIDPartsBoneUnityNative = null;
					CountDrawPartsMaxUnityNative = 0;
#endif
				}

				public string FileNameGetFullPath()
				{
					return(NameDirectory + NameFileBody + NameFileExtension);
				}

				public int IndexGetParts(string name)
				{
					if(null != TableParts)
					{
						for(int i=0; i<TableParts.Length; i++)
						{
							if(name == TableParts[i].Data.Name)
							{
								return(i);
							}
						}
					}
					return(-1);
				}

				public void InUseCellMapAdd(int indexCellMap, Parts informationParts)
				{
					InUseCellMap value = new InUseCellMap();
					value.IndexCellMap = indexCellMap;
					value.Blend = informationParts.Data.OperationBlendTarget;

					switch(informationParts.Data.Feature)
					{
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL:
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH:
							if(false == ListInUseCellMap.Contains(value))
							{
								ListInUseCellMap.Add(value);
							}
							break;

						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK:
							value.Blend = Library_SpriteStudio6.KindOperationBlend.MASK_PRE;
							if(false == ListInUseCellMap.Contains(value))
							{
								ListInUseCellMap.Add(value);
							}
							value.Blend = Library_SpriteStudio6.KindOperationBlend.MASK;
							if(false == ListInUseCellMap.Contains(value))
							{
								ListInUseCellMap.Add(value);
							}
							break;

						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.ROOT:
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NULL:
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE:
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT:
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.JOINT:
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONE:
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MOVENODE:
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.CONSTRAINT:
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONEPOINT:
							break;
					}
				}
				#endregion Functions

				/* ----------------------------------------------- Classes, Structs & Interfaces */
				#region Classes, Structs & Interfaces
				public class Parts
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public Library_SpriteStudio6.Data.Parts.Animation Data;

					public int IndexCellMapMeshBind;
					public int IndexCellMeshBind;

					public float LengthBone;
					public float PositionXBone;
					public float PositionYBone;
					public float RotateZBone;

					public KindInheritance Inheritance;
					public FlagBitInheritance FlagInheritance;

					public List<int> ListIndexPartsChild;

					public bool FlagHide;
					public bool FlagMasking;

#if CHANGE_DEFORM_DECODING
					public bool FlagDeform;
#else
					public int CountVertexDeform;		/* MEMO: Dis-Use, Hindrance */
#endif

					/* MEMO: UnderControl == Instance, Effect */
					public string NameUnderControl;
					public string NameAnimationUnderControl;

#if !_FOR_SS6CONVERTER_
					public LibraryEditor_SpriteStudio6.Import.Assets<Mesh> DataMeshSkinnedUnityNative;

					public string NameGameObjectUnityNative;
					public GameObject GameObjectUnityNative;	/* Temporary */
					public SpriteRenderer SpriteRendererUnityNative;	/* Temporary */
#if UNITY_2017_1_OR_NEWER
					public SpriteMask SpriteMaskUnityNative;	/* Temporary */
#else
					/* MEMO: Can not use "SpriteMask" in Unity5.6 or earlier.                               */
					/*       (For "Nintendo Switch" for the time being, corresponds to Unity5.6 or earlier) */
#endif
					public SkinnedMeshRenderer SkinnedMeshRendererUnityNative;	/* Temporary */
					public MeshRenderer MeshRendererUnityNative;	/* Temporary */
					public MeshFilter MeshFilterUnityNative;	/* Temporary */
					public Script_SpriteStudio6_RootUnityNative ScriptRootUnityNative;
					public Script_SpriteStudio6_PartsUnityNative ScriptPartsUnityNative;
#endif
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public void CleanUp()
					{
						Data.CleanUp();

						IndexCellMapMeshBind = -1;
						IndexCellMeshBind = -1;

						LengthBone = 0.0f;
						PositionXBone = 0.0f;
						PositionYBone = 0.0f;
						RotateZBone = 0.0f;

						Inheritance = KindInheritance.PARENT;
						FlagInheritance = FlagBitInheritance.CLEAR;

						ListIndexPartsChild = new List<int>();
						ListIndexPartsChild.Clear();

						FlagHide = false;
						FlagMasking = false;

#if CHANGE_DEFORM_DECODING
						FlagDeform = false;
#else
						CountVertexDeform = 0;
#endif

						NameUnderControl = "";
						NameAnimationUnderControl = "";

#if !_FOR_SS6CONVERTER_
						DataMeshSkinnedUnityNative.CleanUp();
						DataMeshSkinnedUnityNative.BootUp(1);	/* Always 1 */

						NameGameObjectUnityNative = "";
						GameObjectUnityNative = null;
						SpriteRendererUnityNative = null;
#if UNITY_2017_1_OR_NEWER
						SpriteMaskUnityNative = null;
#else
						/* MEMO: Can not use "SpriteMask" in Unity5.6 or earlier.                               */
						/*       (For "Nintendo Switch" for the time being, corresponds to Unity5.6 or earlier) */
#endif
						SkinnedMeshRendererUnityNative = null;
						MeshRendererUnityNative = null;
						MeshFilterUnityNative = null;
						ScriptRootUnityNative = null;
						ScriptPartsUnityNative = null;
#endif
					}
					#endregion Functions

					/* ----------------------------------------------- Enums & Constants */
					#region Enums & Constants
					public enum KindInheritance
					{
						PARENT = 0,
						SELF
					}

					public enum FlagBitInheritance
					{
						OPACITY_RATE = 0x000000001,
						SHOW_HIDE = 0x000000002,
						FLIP_X = 0x000000010,
						FLIP_Y = 0x000000020,

						CLEAR = 0x00000000,
						ALL = OPACITY_RATE
							| SHOW_HIDE
							| FLIP_X
							| FLIP_Y,
						PRESET = OPACITY_RATE
					}
					#endregion Enums & Constants
				}

				public class Animation
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public Library_SpriteStudio6.Data.Animation Data;

					public Parts[] TableParts;
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public void CleanUp()
					{
						Data = new Library_SpriteStudio6.Data.Animation();	/* class */

						TableParts = null;
					}

					public bool AttributeSolve(	LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
												Information informationSSAE,
												bool flagInvisibleToHideAll
											)
					{
						int countParts = TableParts.Length;
						Parts animationParts = null;
						Parts animationPartsSetup = null;
						LibraryEditor_SpriteStudio6.Import.SSAE.Information.Parts parts = null;

						for(int i=0; i<countParts; i++)
						{
							parts = informationSSAE.TableParts[i];
							animationParts = TableParts[i];

							/* Get Setup-Animation */
							if(null != informationSSAE.AnimationSetup)
							{	/* Has Setup animation */
								if(null != informationSSAE.AnimationSetup.TableParts)
								{	/* Has Animation-Parts table */
									animationPartsSetup = informationSSAE.AnimationSetup.TableParts[i];
								}
							}

							/* Adjust Top-Frame Key-Data */
							animationParts.Cell.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.Cell);

							/* MEMO: In the case of "bone" parts, when no key at frame 0,  necessary to set bone's information to attributes. */
							switch(parts.Data.Feature)
							{
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.ROOT:
									goto default;
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NULL:
									goto default;
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL:
									goto default;
//								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE2:
//									goto default;
//								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE4:
//									goto default;
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE:
									goto default;
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT:
									goto default;
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK:
									goto default;
//								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE2:
//									goto default;
//								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE4:
//									goto default;
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.JOINT:
									goto default;
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONE:
									animationParts.PositionX.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.PositionX, parts.PositionXBone);
									animationParts.PositionY.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.PositionY, parts.PositionYBone);
									animationParts.PositionZ.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.PositionZ);

									animationParts.RotationX.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.RotationX);
									animationParts.RotationY.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.RotationY);
									animationParts.RotationZ.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.RotationZ, parts.RotateZBone);
									break;
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MOVENODE:
									goto default;
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.CONSTRAINT:
									goto default;
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONEPOINT:
									goto default;
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH:
									goto default;
								default:
									animationParts.PositionX.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.PositionX);
									animationParts.PositionY.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.PositionY);
									animationParts.PositionZ.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.PositionZ);

									animationParts.RotationX.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.RotationX);
									animationParts.RotationY.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.RotationY);
									animationParts.RotationZ.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.RotationZ);
									break;
							}

							animationParts.ScalingX.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.ScalingX);
							animationParts.ScalingY.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.ScalingY);

							animationParts.ScalingXLocal.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.ScalingXLocal);
							animationParts.ScalingYLocal.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.ScalingYLocal);

							animationParts.RateOpacity.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.RateOpacity);
							animationParts.RateOpacityLocal.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.RateOpacityLocal);
							animationParts.Priority.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.Priority);

							animationParts.FlipX.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.FlipX);
							animationParts.FlipY.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.FlipY);

							if((true == flagInvisibleToHideAll) && (true == parts.FlagHide))
							{	/* Parts Hide (for Editing), convert to All-Frame-Hide */
								animationParts.Hide.CleanUpKey();

								Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool.KeyData data = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool.KeyData();
								data.Formula = Library_SpriteStudio6.Utility.Interpolation.KindFormula.NON;
								data.FrameCurveStart = 0.0f;
								data.ValueCurveStart = 0.0f;
								data.FrameCurveEnd = 0.0f;
								data.ValueCurveEnd = 0.0f;
								data.Frame = 0;
								data.Value = true;
								animationParts.Hide.ListKey.Add(data);
							}
							else
							{
								animationParts.Hide.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.Hide, true, true, false);	/* "Hide" is true for the top-frames without key data.(not value of first key to appear) */
							}

							animationParts.PartsColor.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.PartsColor);

							animationParts.VertexCorrection.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.VertexCorrection);

							animationParts.PivotOffsetX.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.PivotOffsetX);
							animationParts.PivotOffsetY.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.PivotOffsetY);

							animationParts.AnchorPositionX.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.AnchorPositionX);
							animationParts.AnchorPositionY.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.AnchorPositionY);
							animationParts.SizeForceX.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.SizeForceX);
							animationParts.SizeForceY.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.SizeForceY);

							animationParts.TexturePositionX.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.TexturePositionX);
							animationParts.TexturePositionY.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.TexturePositionY);
							animationParts.TextureRotation.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.TextureRotation);
							animationParts.TextureScalingX.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.TextureScalingX);
							animationParts.TextureScalingY.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.TextureScalingY);
							animationParts.TextureFlipX.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.TextureFlipX);
							animationParts.TextureFlipY.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.TextureFlipY);

							animationParts.RadiusCollision.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.RadiusCollision);
							animationParts.PowerMask.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.PowerMask);

							/* MEMO: UserData does not complement 0 frame. */
//							animationParts.UserData.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.UserData, Library_SpriteStudio6.Data.Animation.Attribute.DefaultUseData, false, true);
							/* MEMO: Do not set at here. Set in processing for each part type. */
//							animationParts.Instance.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.Instance);
//							animationParts.Effect.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.Effect);

							animationParts.Deform.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.Deform);

							/* Delete attributes that should not exist */
							animationParts.AnchorPositionX.ListKey.Clear();	/* Unsupported */
							animationParts.AnchorPositionY.ListKey.Clear();	/* Unsupported */
							switch(parts.Data.Feature)
							{
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.ROOT:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NULL:
									parts.Data.CountMesh = 0;

									animationParts.Cell.ListKey.Clear();

									animationParts.PartsColor.ListKey.Clear();
									animationParts.Priority.ListKey.Clear();
									animationParts.VertexCorrection.ListKey.Clear();

									animationParts.PivotOffsetX.ListKey.Clear();
									animationParts.PivotOffsetY.ListKey.Clear();

									animationParts.SizeForceX.ListKey.Clear();
									animationParts.SizeForceY.ListKey.Clear();

									animationParts.TexturePositionX.ListKey.Clear();
									animationParts.TexturePositionY.ListKey.Clear();
									animationParts.TextureRotation.ListKey.Clear();
									animationParts.TextureScalingX.ListKey.Clear();
									animationParts.TextureScalingY.ListKey.Clear();
									animationParts.TextureFlipX.ListKey.Clear();
									animationParts.TextureFlipY.ListKey.Clear();

									animationParts.PowerMask.ListKey.Clear();

									animationParts.Instance.ListKey.Clear();
									animationParts.Effect.ListKey.Clear();
									animationParts.Deform.ListKey.Clear();
#if CHANGE_DEFORM_DECODING
										parts.FlagDeform = false;
#else
										parts.CountVertexDeform = 0;
#endif
									break;

								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL:
//									/* MEMO: In the case of "NORMAL", not yet decided whether "TRIANGLE 2" or "TRIANGLE 4". (Decide temporarily) */
//									if(0 >= animationParts.VertexCorrection.CountGetKey())
//									{
//										parts.Data.Feature = Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE2;
//										parts.Data.CountMesh = 2;
//									}
//									else
//									{
//										parts.Data.Feature = Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE4;
//										parts.Data.CountMesh = 4;
//									}
//									goto case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE4;
									parts.Data.CountMesh = 4;
									animationParts.PowerMask.ListKey.Clear();

									animationParts.Instance.ListKey.Clear();
									animationParts.Effect.ListKey.Clear();
									animationParts.Deform.ListKey.Clear();
#if CHANGE_DEFORM_DECODING
										parts.FlagDeform = false;
#else
										parts.CountVertexDeform = 0;
#endif
									break;

//								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE2:
//									/* MEMO: Even if temporarily decided as "TRIANGLE 2", when  used "Vertex Correction(Deformation)" in other animation, */
//									/*        change to "TRIANGLE 4".                                                                                     */
//									if(0 <= animationParts.VertexCorrection.CountGetKey())
//									{
//										parts.Data.Feature = Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE4;
//										parts.Data.CountMesh = 4;
//									}
//									goto case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE4;
//
//								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE4:
//									animationParts.PowerMask.ListKey.Clear();
//
//									animationParts.Instance.ListKey.Clear();
//									animationParts.Effect.ListKey.Clear();
//									break;

								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE:
									parts.Data.CountMesh = 0;

									animationParts.Cell.ListKey.Clear();

									animationParts.FlipX.ListKey.Clear();
									animationParts.FlipY.ListKey.Clear();

									animationParts.PartsColor.ListKey.Clear();
									animationParts.VertexCorrection.ListKey.Clear();

									animationParts.PivotOffsetX.ListKey.Clear();
									animationParts.PivotOffsetY.ListKey.Clear();

									animationParts.SizeForceX.ListKey.Clear();
									animationParts.SizeForceY.ListKey.Clear();

									animationParts.TexturePositionX.ListKey.Clear();
									animationParts.TexturePositionY.ListKey.Clear();
									animationParts.TextureRotation.ListKey.Clear();
									animationParts.TextureScalingX.ListKey.Clear();
									animationParts.TextureScalingY.ListKey.Clear();
									animationParts.TextureFlipX.ListKey.Clear();
									animationParts.TextureFlipY.ListKey.Clear();

									animationParts.PowerMask.ListKey.Clear();

									/* MEMO: In "Animation.StatusSetParts" function to be executed later,                               */
									/*        if all frames are hide status, judge that "Instance" are not used and erase all key data. */
									/*       Complement key-data for now.                                                               */
									animationParts.Instance.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.Instance, Library_SpriteStudio6.Data.Animation.Attribute.DefaultInstance, false, false);

									animationParts.Effect.ListKey.Clear();
									animationParts.Deform.ListKey.Clear();
#if CHANGE_DEFORM_DECODING
										parts.FlagDeform = false;
#else
										parts.CountVertexDeform = 0;
#endif
									break;

								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT:
									parts.Data.CountMesh = 0;

									animationParts.Cell.ListKey.Clear();

									animationParts.FlipX.ListKey.Clear();
									animationParts.FlipY.ListKey.Clear();

									animationParts.PartsColor.ListKey.Clear();
									animationParts.VertexCorrection.ListKey.Clear();

									animationParts.PivotOffsetX.ListKey.Clear();
									animationParts.PivotOffsetY.ListKey.Clear();

									animationParts.SizeForceX.ListKey.Clear();
									animationParts.SizeForceY.ListKey.Clear();

									animationParts.TexturePositionX.ListKey.Clear();
									animationParts.TexturePositionY.ListKey.Clear();
									animationParts.TextureRotation.ListKey.Clear();
									animationParts.TextureScalingX.ListKey.Clear();
									animationParts.TextureScalingY.ListKey.Clear();
									animationParts.TextureFlipX.ListKey.Clear();
									animationParts.TextureFlipY.ListKey.Clear();

									animationParts.PowerMask.ListKey.Clear();

									/* MEMO: In "Animation.StatusSetParts" function to be executed later,                             */
									/*        if all frames are hide status, judge that "Effect" are not used and erase all key data. */
									/*       Complement key-data for now.                                                             */
									animationParts.Effect.KeyDataAdjustTopFrame((null == animationPartsSetup) ? null : animationPartsSetup.Effect, Library_SpriteStudio6.Data.Animation.Attribute.DefaultEffect, false, false);

									animationParts.Instance.ListKey.Clear();
									animationParts.Deform.ListKey.Clear();
#if CHANGE_DEFORM_DECODING
										parts.FlagDeform = false;
#else
										parts.CountVertexDeform = 0;
#endif
									break;

								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK:
//									/* MEMO: In the case of "MASK", not yet decided whether "TRIANGLE 2" or "TRIANGLE 4". (Decide temporarily) */
//									if(0 <= animationParts.VertexCorrection.CountGetKey())
//									{
//										parts.Data.Feature = Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE4;
//										parts.Data.CountMesh = 4;
//									}
//									else
//									{
//										parts.Data.Feature = Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE2;
//										parts.Data.CountMesh = 2;
//									}
//									goto case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE4;
									parts.Data.CountMesh = 4;
									animationParts.Instance.ListKey.Clear();
									animationParts.Effect.ListKey.Clear();
									animationParts.Deform.ListKey.Clear();
#if CHANGE_DEFORM_DECODING
										parts.FlagDeform = false;
#else
										parts.CountVertexDeform = 0;
#endif
									break;

//								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE2:
//									/* MEMO: Even if temporarily decided as "TRIANGLE 2", when  used "Vertex Correction(Deformation)" in other animation, */
//									/*        change to "TRIANGLE 4".                                                                                     */
//									if(0 <= animationParts.VertexCorrection.CountGetKey())
//									{
//										parts.Data.Feature = Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE4;
//										parts.Data.CountMesh = 4;
//									}
//									goto case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE4;

//								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE4:
//									animationParts.Instance.ListKey.Clear();
//									animationParts.Effect.ListKey.Clear();
//									break;

								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.JOINT:
									parts.Data.CountMesh = 0;

									animationParts.Cell.ListKey.Clear();

									animationParts.FlipX.ListKey.Clear();
									animationParts.FlipY.ListKey.Clear();

									animationParts.Priority.ListKey.Clear();
									animationParts.PartsColor.ListKey.Clear();
									animationParts.VertexCorrection.ListKey.Clear();

									animationParts.PivotOffsetX.ListKey.Clear();
									animationParts.PivotOffsetY.ListKey.Clear();

									animationParts.SizeForceX.ListKey.Clear();
									animationParts.SizeForceY.ListKey.Clear();

									animationParts.TexturePositionX.ListKey.Clear();
									animationParts.TexturePositionY.ListKey.Clear();
									animationParts.TextureRotation.ListKey.Clear();
									animationParts.TextureScalingX.ListKey.Clear();
									animationParts.TextureScalingY.ListKey.Clear();
									animationParts.TextureFlipX.ListKey.Clear();
									animationParts.TextureFlipY.ListKey.Clear();

									animationParts.PowerMask.ListKey.Clear();

									animationParts.Instance.ListKey.Clear();
									animationParts.Effect.ListKey.Clear();
									animationParts.Deform.ListKey.Clear();
#if CHANGE_DEFORM_DECODING
										parts.FlagDeform = false;
#else
										parts.CountVertexDeform = 0;
#endif
									break;

								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONE:
									parts.Data.CountMesh = 0;

									animationParts.Cell.ListKey.Clear();

									animationParts.RotationX.ListKey.Clear();
									animationParts.RotationY.ListKey.Clear();

									animationParts.ScalingXLocal.ListKey.Clear();
									animationParts.ScalingYLocal.ListKey.Clear();

									animationParts.Priority.ListKey.Clear();
									animationParts.RateOpacityLocal.ListKey.Clear();

									animationParts.FlipX.ListKey.Clear();
									animationParts.FlipY.ListKey.Clear();

									animationParts.PartsColor.ListKey.Clear();
									animationParts.VertexCorrection.ListKey.Clear();

									animationParts.PivotOffsetX.ListKey.Clear();
									animationParts.PivotOffsetY.ListKey.Clear();

									animationParts.SizeForceX.ListKey.Clear();
									animationParts.SizeForceY.ListKey.Clear();

									animationParts.TexturePositionX.ListKey.Clear();
									animationParts.TexturePositionY.ListKey.Clear();
									animationParts.TextureRotation.ListKey.Clear();
									animationParts.TextureScalingX.ListKey.Clear();
									animationParts.TextureScalingY.ListKey.Clear();
									animationParts.TextureFlipX.ListKey.Clear();
									animationParts.TextureFlipY.ListKey.Clear();

									animationParts.PowerMask.ListKey.Clear();

									animationParts.Instance.ListKey.Clear();
									animationParts.Effect.ListKey.Clear();
									animationParts.Deform.ListKey.Clear();
#if CHANGE_DEFORM_DECODING
										parts.FlagDeform = false;
#else
										parts.CountVertexDeform = 0;
#endif
									break;

								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MOVENODE:
									break;
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.CONSTRAINT:
									break;
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONEPOINT:
									break;

								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH:
									animationParts.FlipX.ListKey.Clear();
									animationParts.FlipY.ListKey.Clear();

									animationParts.TexturePositionX.ListKey.Clear();
									animationParts.TexturePositionY.ListKey.Clear();
									animationParts.TextureRotation.ListKey.Clear();
									animationParts.TextureScalingX.ListKey.Clear();
									animationParts.TextureScalingY.ListKey.Clear();
									animationParts.TextureFlipX.ListKey.Clear();
									animationParts.TextureFlipY.ListKey.Clear();

									animationParts.PowerMask.ListKey.Clear();

									animationParts.Instance.ListKey.Clear();
									animationParts.Effect.ListKey.Clear();

#if CHANGE_DEFORM_DECODING
									int countVertexCell = 0;
#else
									/* MEMO: Reconfigure since there are cases where "number of Cell-Mesh's vertices" and "number of Deform-attribute's vertices" are different. */
									{
										int countVertexMesh = parts.CountVertexDeform;
										if(0 < countVertexMesh)
										{	/* use "Deform" */
											int count = animationParts.Deform.CountGetKey();
											Library_SpriteStudio6.Data.Animation.Attribute.Importer.DataDeform dataDeform = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.DataDeform();
											for(int j=0; j<count; j++)
											{
												dataDeform = animationParts.Deform.ListKey[j].Value;
												dataDeform.CountVertexMesh = countVertexMesh;
												animationParts.Deform.ListKey[j].Value = dataDeform;
											}
										}
									}
#endif

									/* MEMO: Since Mesh's cell is set in "Setup" animation, can not be changed for each animation. */
									/*       But just in case, get mesh count each animation.                                      */
									if(0 <= animationParts.Cell.CountGetKey())
									{
										int indexCellMap = animationParts.Cell.ListKey[0].Value.IndexCellMap;
										int indexCell = animationParts.Cell.ListKey[0].Value.IndexCell;
										if((0 <= indexCellMap) && (0 <= indexCell))
										{
											if(true == informationSSPJ.TableInformationSSCE[indexCellMap].TableCell[indexCell].Data.IsMesh)
											{
												countVertexCell = informationSSPJ.TableInformationSSCE[indexCellMap].TableCell[indexCell].Data.Mesh.TableCoordinate.Length;
												int countMesh = informationSSPJ.TableInformationSSCE[indexCellMap].TableCell[indexCell].Data.Mesh.CountMesh;
												if(parts.Data.CountMesh < countMesh)
												{
													parts.IndexCellMapMeshBind = indexCellMap;
													parts.IndexCellMeshBind = indexCell;

													parts.Data.CountMesh = countMesh;
												}
											}
										}
									}

#if CHANGE_DEFORM_DECODING
									/* MEMO: Attribute"Deform" has dedicated normalizer. Because has unstable-data. */
									if(0 < countVertexCell)
									{
										/* MEMO: Limit vertex indexes to the number of Cell-Mesh's vertices since "Deform" is for "Mesh" parts. */
										animationParts.Deform.Normalize(countVertexCell);
										if(0 < animationParts.Deform.CountGetKey())
										{
											parts.FlagDeform |= true;
										}
									}
#else
#endif
									break;

								default:
									break;
							}
						}

						return(true);
					}

					public bool StatusSetParts(	LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
												LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE
											)
					{
						int countFrame = Data.CountFrame;
						int countParts = TableParts.Length;
						Parts animationParts = null;

						for(int i=0; i<countParts; i++)
						{
							animationParts = TableParts[i];

							/* Set Masking */
							/* MEMO: Caution that flag is inverted. (Difference between "Mask" and "No-Mask") */
							if(true == informationSSAE.TableParts[i].FlagMasking)
							{
								animationParts.StatusParts &= ~Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NOT_MASKING;
							}
							else
							{
								animationParts.StatusParts |= Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NOT_MASKING;
							}

							bool flagInUse = false;

							/* Check Transform */
							if((0 >= animationParts.PositionX.CountGetKey())
								&& (0 >= animationParts.PositionY.CountGetKey())
								&& (0 >= animationParts.PositionZ.CountGetKey())
								)
							{
								animationParts.StatusParts |= Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_POSITION;
							}
							else
							{
								animationParts.StatusParts &= ~Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_POSITION;
								flagInUse |= true;
							}

							if((0 >= animationParts.RotationX.CountGetKey())
								&& (0 >= animationParts.RotationY.CountGetKey())
								&& (0 >= animationParts.RotationZ.CountGetKey())
								)
							{
								animationParts.StatusParts |= Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_ROTATION;
							}
							else
							{
								animationParts.StatusParts &= ~Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_ROTATION;
								flagInUse |= true;
							}

							if((0 >= animationParts.ScalingX.CountGetKey())
								&& (0 >= animationParts.ScalingY.CountGetKey())
								)
							{
								animationParts.StatusParts |= Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_SCALING;
							}
							else
							{
								animationParts.StatusParts &= ~Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_SCALING;
								flagInUse |= true;
							}

							/* Check Hidden */
							bool flagHideAll = true;
							animationParts.TableHide = new bool[countFrame];
							for(int j=0; j<countFrame; j++)
							{
								Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetBoolOR(	out animationParts.TableHide[j],
																													animationParts.Hide,
																													j,
																													true
																												);
								if(false == animationParts.TableHide[j])
								{
									flagHideAll = false;
									break;
								}
							}
							if(true == flagHideAll)
							{
								animationParts.StatusParts |= Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.HIDE_FULL;

								/* MEMO: Set "Instance" and "Effect" to unused */
								animationParts.Instance.CleanUpKey();
								animationParts.Effect.CleanUpKey();
							}
							else
							{
								animationParts.StatusParts &= ~Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.HIDE_FULL;
								flagInUse |= true;
							}

							/* Check Texture-Transform */
							if((0 >= animationParts.TexturePositionX.CountGetKey())
								&& (0 >= animationParts.TexturePositionY.CountGetKey())
								&& (0 >= animationParts.TextureScalingX.CountGetKey())
								&& (0 >= animationParts.TextureScalingY.CountGetKey())
								&& (0 >= animationParts.TextureRotation.CountGetKey())
								&& (0 >= animationParts.TextureFlipX.CountGetKey())
								&& (0 >= animationParts.TextureFlipY.CountGetKey())
								)
							{
								animationParts.StatusParts |= Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_TRANSFORMATION_TEXTURE;
							}
							else
							{
								animationParts.StatusParts &= ~Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_TRANSFORMATION_TEXTURE;
								flagInUse |= true;
							}

							/* Check UserData */
							if(0 >= animationParts.UserData.CountGetKey())
							{
								animationParts.StatusParts |= Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_USERDATA;
							}
							else
							{
								animationParts.StatusParts &= ~Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_USERDATA;
								flagInUse |= true;
							}

							/* Check PartsColor */
							if(0 >= animationParts.PartsColor.CountGetKey())
							{
								animationParts.StatusParts |= Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_PARTSCOLOR;
							}
							else
							{
								animationParts.StatusParts &= ~Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_PARTSCOLOR;
								flagInUse |= true;
							}

							/* Other Attribute */
							flagInUse |= (0 >= animationParts.Cell.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.ScalingXLocal.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.ScalingYLocal.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.RateOpacity.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.RateOpacityLocal.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.Priority.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.FlipX.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.FlipY.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.VertexCorrection.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.PivotOffsetX.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.PivotOffsetY.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.AnchorPositionX.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.AnchorPositionY.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.SizeForceX.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.SizeForceY.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.RadiusCollision.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.PowerMask.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.Instance.CountGetKey()) ? true : false;
							flagInUse |= (0 >= animationParts.Effect.CountGetKey()) ? true : false;

							/* MEMO: Attributes for Fix format absolutely do not have data at this point. */
//							flagInUse |= (0 >= animationParts.FixIndexCellMap.CountGetKey()) ? true : false;
//							flagInUse |= (0 >= animationParts.FixCoordinate.CountGetKey()) ? true : false;
//							flagInUse |= (0 >= animationParts.FixUV.CountGetKey()) ? true : false;
//							flagInUse |= (0 >= animationParts.FixSizeCollisionX.CountGetKey()) ? true : false;
//							flagInUse |= (0 >= animationParts.FixSizeCollisionY.CountGetKey()) ? true : false;
//							flagInUse |= (0 >= animationParts.Effect.FixPivotCollisionX()) ? true : false;
//							flagInUse |= (0 >= animationParts.Effect.FixPivotCollisionY()) ? true : false;

							if(false == flagInUse)
							{	/* Not Use */
								animationParts.StatusParts |= Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NOT_USED;

								/* MEMO: Just in case. Set flags when parts is not used. */
								/*       (No possibility of having key-datas)            */
								animationParts.StatusParts |= (	Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_POSITION
																| Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_ROTATION
																| Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_SCALING
																| Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.HIDE_FULL
																| Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_TRANSFORMATION_TEXTURE
																| Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_USERDATA
																| Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NO_PARTSCOLOR
															);
							}
							else
							{
								animationParts.StatusParts &= ~Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NOT_USED;
							}

							/* Set Valid */
							animationParts.StatusParts |= Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.VALID;
						}
						return(true);

//					PartsStatusSet_ErrorEnd:;
//						return(false);
					}

					public bool DrawOrderCreate(	LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
													LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE
											)
					{
						int countFrame = Data.CountFrame;
						int countParts = TableParts.Length;
						Parts animationParts = null;
						LibraryEditor_SpriteStudio6.Import.SSAE.Information.Parts parts = null;

						/* Create temporary transform-matrix (for Z-Position sort) */
						Matrix4x4[,] matrixTransform = new Matrix4x4[countParts, countFrame];
						for(int i=0; i<countParts; i++)
						{
							for(int j=0; j<countFrame; j++)
							{
								matrixTransform[i, j] = Matrix4x4.identity;
							}
						}

						/* Prepare parts to process */
						/* MEMO: "Draw" is for normal drawing.                                                   */
						/*       "PreDraw" is for drawing initial mask.                                          */
						/*        When focusing on "Mask" parts only, "Draw" and "PreDraw" are in reverse order. */
						List<int> listIndexPartsDraw = new List<int>(countParts);
						listIndexPartsDraw.Clear();
						float[][] tableDrawPriority = new float[countParts][];
						bool flagAddList = false;
						for(int i=0; i<countParts; i++)
						{
							animationParts = TableParts[i];
							parts = informationSSAE.TableParts[i];
							tableDrawPriority[i] = null;

							/* Calculate transform-matrix */
							for(int j=0; j<countFrame; j++)
							{
								DrawOrderCreateCalculateMatrixTransform(ref matrixTransform[i, j], matrixTransform, parts, animationParts, j);
							}

							/* Filter draw-parts */
							flagAddList = false;
							switch(parts.Data.Feature)
							{
								/* Non draw parts */
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.ROOT:
									/* MEMO: Create table in "Root"part so that can get first drawing part's index. (for "Draw" and "PreDraw") */
									animationParts.TableOrderDraw = new int[countFrame];
									animationParts.TableOrderPreDraw = new int[countFrame];
									for(int j=0; j<countFrame; j++)
									{
										animationParts.TableOrderDraw[j] = -1;
										animationParts.TableOrderPreDraw[j] = -1;
									}
									break;

								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NULL:
									break;

								/* Draw parts */
//								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE2:
//								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE4:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH:
									/* MEMO: Normal rendering parts only draw on "Draw". */
									/* Create Draw-Order table */
									animationParts.TableOrderDraw = new int[countFrame];
									for(int j=0; j<countFrame; j++)
									{
										animationParts.TableOrderDraw[j] = -1;
									}

									flagAddList = true;
									break;

//								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE2:
//								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE4:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK:
									/* Create Draw-Order table */
									/* MEMO:  Since "Mask" draws twice on "Draw" and "PreDraw", both tables are necessary. */
									animationParts.TableOrderDraw = new int[countFrame];
									animationParts.TableOrderPreDraw = new int[countFrame];
									for(int j=0; j<countFrame; j++)
									{
										animationParts.TableOrderDraw[j] = -1;
										animationParts.TableOrderPreDraw[j] = -1;
									}

									flagAddList = true;
									break;

								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.JOINT:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONE:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MOVENODE:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.CONSTRAINT:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONEPOINT:
									break;

								default:
									/* MEMO: No reach here. */
									break;
							}

							/* Calculate Priority */
							if(true == flagAddList)
							{
								/* Calculate all frames' priority. */
								tableDrawPriority[i] = new float[countFrame];
								switch(Data.ModeSort)
								{
									case Library_SpriteStudio6.Data.Animation.KindModeSort.PRIORITY:
										DrawOrderCreatePriority(ref tableDrawPriority[i], informationSSPJ, informationSSAE, this, animationParts);
										break;

									case Library_SpriteStudio6.Data.Animation.KindModeSort.POSITION_Z:
										/* MEMO: In case of Z-sorting, transformed position-Z are set to attribute "Priority".                    */
										/*       Because transformed position-Z are needed again as realtime sort key for animation blending etc. */
										DrawOrderCreatePositionZ(ref tableDrawPriority[i], informationSSPJ, informationSSAE, this, animationParts, matrixTransform, i);
										DrawOrderCreateAttributeSetPriority(animationParts, tableDrawPriority[i]);
										break;
								}

								/* Add as part to be processed */
								listIndexPartsDraw.Add(i);
							}
						}
						matrixTransform = null;	/* No use anymore */

						/* Decide Draw-Order Table */
						/* MEMO: "Root"part is excluded from target. */
						int countIndexPartsDraw = listIndexPartsDraw.Count;
						List<int> listIndexPartsSort = new List<int>(countIndexPartsDraw);
						listIndexPartsSort.Clear();
						List<int> listIndexPartsSortPreDraw = new List<int>(countIndexPartsDraw);
						listIndexPartsSortPreDraw.Clear();
						List<float> listPrioritySort = new List<float>(countIndexPartsDraw);
						listPrioritySort.Clear();
						for(int frame=0; frame<countFrame; frame++)
						{
							/* Extract draw parts (in this frame) */
							for(int i=0; i<countIndexPartsDraw; i++)
							{
								int indexParts = listIndexPartsDraw[i];
								parts = informationSSAE.TableParts[indexParts];
								animationParts = TableParts[indexParts];
								if(0 == (animationParts.StatusParts & Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.HIDE_FULL))
								{
									switch(parts.Data.Feature)
									{
										case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.ROOT:
										case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NULL:
											/* MEMO: No reach here. */
											break;

//										case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE2:
//										case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE4:
										case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL:
										case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH:
											/* MEMO: Not be added to list at when hide state. */
											if(false == animationParts.TableHide[frame])
											{
												listIndexPartsSort.Add(indexParts);
												listPrioritySort.Add(tableDrawPriority[indexParts][frame]);
											}
											break;

										case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE:
										case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT:
											/* MEMO: "Instance"-parts and "Effect"-parts are always updated regardless of hide state, so unconditionally added to list. */
											listIndexPartsSort.Add(indexParts);
											listPrioritySort.Add(tableDrawPriority[indexParts][frame]);
											break;

//										case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE2:
//										case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE4:
										case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK:
											/* MEMO: Not be added to list at when hide state. */
											if(false == animationParts.TableHide[frame])
											{
												listIndexPartsSort.Add(indexParts);
												listPrioritySort.Add(tableDrawPriority[indexParts][frame]);
											}
											break;

										case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.JOINT:
										case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONE:
										case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MOVENODE:
										case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.CONSTRAINT:
										case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONEPOINT:
											break;

										default:
											/* MEMO: No reach here. */
											break;
									}
								}
							}

							/* Sort (Bubble) */
							/* When the same priority, parts that has larger ID (part-index) drawed later. */
							int countIndexPartsSort = listIndexPartsSort.Count;
							for(int i=0; i<(countIndexPartsSort - 1); i++)
							{
								for(int j=(countIndexPartsSort - 1); j>i; j--)
								{
									int k = j - 1;
									if((listPrioritySort[j] < listPrioritySort[k])
										|| ((listPrioritySort[j] == listPrioritySort[k]) && (listIndexPartsSort[j] < listIndexPartsSort[k]))
										)
									{
										float tempFloat = listPrioritySort[j];
										int tempInt = listIndexPartsSort[j];

										listPrioritySort[j] = listPrioritySort[k];
										listIndexPartsSort[j] = listIndexPartsSort[k];

										listPrioritySort[k] = tempFloat;
										listIndexPartsSort[k] = tempInt;
									}
								}
							}
							listPrioritySort.Clear();

							/* Create Order for "PreDraw" */
							/* MEMO: Enough to add in reverse order  to "listIndexPartsSortPreDraw",           */
							/*        since "listIndexPartsSort" has already been sorted in the drawing order. */
							listIndexPartsSortPreDraw.Clear();
							for(int i=(countIndexPartsSort-1); i>=0 ; i--)
							{
								int indexParts = listIndexPartsSort[i];
								parts = informationSSAE.TableParts[indexParts];
								animationParts = TableParts[indexParts];
									switch(parts.Data.Feature)
								{
									case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.ROOT:
									case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NULL:
										/* MEMO: No reach here. */
										break;

//									case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE2:
//									case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE4:
									case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL:
									case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE:
									case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT:
									case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH:
										break;

//									case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE2:
//									case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE4:
									case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK:
										/* MEMO: Add "Mask"parts only */
										listIndexPartsSortPreDraw.Add(indexParts);
										break;

									case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.JOINT:
									case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONE:
									case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MOVENODE:
									case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.CONSTRAINT:
									case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONEPOINT:
										break;

									default:
										/* MEMO: No reach here. */
										break;
								}
							}

							/* Set Order for "Draw" */
							/* MEMO: In "Root"part, first-drawing part's index is stored. */
							if(0 < countIndexPartsSort)
							{
								TableParts[0].TableOrderDraw[frame] = listIndexPartsSort[0];
								for(int i=1; i<countIndexPartsSort; i++)
								{
									TableParts[listIndexPartsSort[i - 1]].TableOrderDraw[frame] = listIndexPartsSort[i];
								}
								TableParts[listIndexPartsSort[countIndexPartsSort - 1]].TableOrderDraw[frame] = -1;
							}
							else
							{
								TableParts[0].TableOrderDraw[frame] = -1;
							}

							/* Set Order for "PreDraw" */
							countIndexPartsSort = listIndexPartsSortPreDraw.Count;
							if(0 < countIndexPartsSort)
							{
								TableParts[0].TableOrderPreDraw[frame] = listIndexPartsSortPreDraw[0];
								for(int i=1; i<countIndexPartsSort; i++)
								{
									TableParts[listIndexPartsSortPreDraw[i - 1]].TableOrderPreDraw[frame] = listIndexPartsSortPreDraw[i];
								}
								TableParts[listIndexPartsSortPreDraw[countIndexPartsSort - 1]].TableOrderPreDraw[frame] = -1;
							}
							else
							{
								TableParts[0].TableOrderPreDraw[frame] = -1;
							}

							listIndexPartsSort.Clear();
							listIndexPartsSortPreDraw.Clear();
						}

						return(true);
					}
					private void DrawOrderCreateCalculateMatrixTransform(	ref Matrix4x4 matrixTransform,
																			Matrix4x4[,] matrixTransformReferential,
																			LibraryEditor_SpriteStudio6.Import.SSAE.Information.Parts informationParts,
																			LibraryEditor_SpriteStudio6.Import.SSAE.Information.Animation.Parts informationAnimationParts,
																			int frame
																		)
					{
#if !_FOR_SS6CONVERTER_
						/* MEMO: 一旦Z座標でのソートはssbpで対応しないでおく（単に行列計算が安定するまで）。 */
						Vector3 position = Vector3.zero;
						Vector3 rotationEuler = Vector3.zero;
						Vector3 scaling = Vector3.one;
						Matrix4x4 matrixLocal = Matrix4x4.identity;

						/* Get Attributes */
						informationAnimationParts.PositionX.ValueGet(out position.x, frame);
						informationAnimationParts.PositionY.ValueGet(out position.y, frame);
						informationAnimationParts.PositionZ.ValueGet(out position.z, frame);

						informationAnimationParts.RotationX.ValueGet(out rotationEuler.x, frame);
						informationAnimationParts.RotationY.ValueGet(out rotationEuler.y, frame);
						informationAnimationParts.RotationZ.ValueGet(out rotationEuler.z, frame);

						informationAnimationParts.ScalingX.ValueGet(out scaling.x, frame);
						informationAnimationParts.ScalingY.ValueGet(out scaling.y, frame);
						scaling.z = 1.0f;

						/* Solve rotation order of Euler angles */
						Quaternion rotateQuaternion;
						Library_SpriteStudio6.Utility.Math.QuaternionGetEulerAngels(out rotateQuaternion, ref rotationEuler);

						/* Calculate local-matrix */
						matrixLocal.SetTRS(position, rotateQuaternion, scaling);

						/* Calculate transform-matrix */
						int idPartsParent = informationParts.Data.IDParent;
						if(0 > idPartsParent)
						{	/* Root */
							matrixTransform = matrixLocal;
						}
						else
						{
							/* MEMO: Matrixs of parts whose ID is large has not been calculated yet. */
							matrixTransform = matrixTransformReferential[idPartsParent, frame] * matrixLocal;
						}
#endif
					}
					private void DrawOrderCreatePriority(	ref float[] tableValue,
															LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
															LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE,
															LibraryEditor_SpriteStudio6.Import.SSAE.Information.Animation informationAnimation,
															LibraryEditor_SpriteStudio6.Import.SSAE.Information.Animation.Parts informationAnimationParts
													)
					{
						int valueInt;
						int countFrame = tableValue.Length;
						for(int i=0; i<countFrame; i++)
						{
							if(false == informationAnimationParts.Priority.ValueGet(out valueInt, i))
							{
								tableValue[i] = 0.0f;
							}
							else
							{
								tableValue[i] = (float)valueInt;
							}
						}
					}
					private void DrawOrderCreatePositionZ(	ref float[] tableValue,
															LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
															LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE,
															LibraryEditor_SpriteStudio6.Import.SSAE.Information.Animation informationAnimation,
															LibraryEditor_SpriteStudio6.Import.SSAE.Information.Animation.Parts informationAnimationParts,
															Matrix4x4[,] matrixTransform,
															int idParts
													)
					{
#if !_FOR_SS6CONVERTER_
						/* MEMO: 一旦Z座標でのソートはssbpで対応しないでおく（単に行列計算が安定するまで）。 */
						int countFrame = tableValue.Length;
						Vector3 point;
						for(int i=0; i<countFrame; i++)
						{
							/* MEMO: Originally it is only necessary to extract the translation component of the matrix. */
							/*       For expansion easy, transform coordinate.                                           */
							point = Vector3.zero;
							point = matrixTransform[idParts, i].MultiplyPoint3x4(point);

							tableValue[i] = -point.z;	/* Lower value, priority is higher. */
						}
#endif
					}
					private void DrawOrderCreateAttributeSetPriority(	LibraryEditor_SpriteStudio6.Import.SSAE.Information.Animation.Parts informationAnimationParts,
																		float[] tableValue
																	)
					{
						/* MEMO: In case Z-sorting, attribute "Priority" does not originally exist. */
						/*       Creates attribute "Priority" using transformed position-Z.         */
						informationAnimationParts.Priority.CleanUpKey();

						/* MEMO: Value range of attribute "Priority" needs to be within the range of             */
						/*        "31 - Library_SpriteStudio6.Control.Animation.CountShiftSortKeyPriority" bits. */
						float value;
						int count = tableValue.Length;
						for(int i=0; i<count; i++)
						{
							/* MEMO: Keys created has no interpolation. */
							Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeInt.KeyData keyData = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeInt.KeyData();
							keyData.Frame = i;
							keyData.Formula = Library_SpriteStudio6.Utility.Interpolation.KindFormula.NON;
							keyData.FrameCurveStart = 0.0f;
							keyData.ValueCurveStart = 0.0f;
							keyData.FrameCurveEnd = 0.0f;
							keyData.ValueCurveEnd = 0.0f;

							value = Mathf.Clamp(tableValue[i], -LimitSortKeyPositionZ, LimitSortKeyPositionZ);
							value *= RateSortKeyPositionZ;
							keyData.Value = (int)(Mathf.Floor(value));

							informationAnimationParts.Priority.ListKey.Add(keyData);
						}
					}
					#endregion Functions

					/* ----------------------------------------------- Enums & Constants */
					#region Enums & Constants
					public const float RateSortKeyPositionZ = 256.0f;	/* Effective up to (about) 3rd decimal places */
					public const float LimitSortKeyPositionZ = 2000.0f;	/* Effective range of transformed position-Z. */
					#endregion Enums & Constants

					/* ----------------------------------------------- Classes, Structs & Interfaces */
					#region Classes, Structs & Interfaces
					public class Parts
					{
						/* ----------------------------------------------- Variables & Properties */
						#region Variables & Properties
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeCell Cell;

						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat PositionX;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat PositionY;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat PositionZ;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat RotationX;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat RotationY;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat RotationZ;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat ScalingX;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat ScalingY;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat ScalingXLocal;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat ScalingYLocal;

						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat RateOpacity;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat RateOpacityLocal;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeInt Priority;

						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool FlipX;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool FlipY;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool Hide;

						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributePartsColor PartsColor;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeVertexCorrection VertexCorrection;

						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat PivotOffsetX;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat PivotOffsetY;

						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat AnchorPositionX;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat AnchorPositionY;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat SizeForceX;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat SizeForceY;

						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat TexturePositionX;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat TexturePositionY;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat TextureRotation;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat TextureScalingX;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat TextureScalingY;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool TextureFlipX;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool TextureFlipY;

						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat RadiusCollision;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat PowerMask;	/* AttributeInt */

						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeUserData UserData;

						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeInstance Instance;
						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeEffect Effect;

						public Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeDeform Deform;

						public Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus StatusParts;
						public bool[] TableHide;	/* Expand "Hide"attribute in order to drawing state optimize. */
						public int[] TableOrderDraw;
						public int[] TableOrderPreDraw;
						#endregion Variables & Properties

						/* ----------------------------------------------- Functions */
						#region Functions
						public void CleanUp()
						{
							Cell = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeCell();
							Cell.CleanUp();

							PositionX = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							PositionX.CleanUp();
							PositionY = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							PositionY.CleanUp();
							PositionZ = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							PositionZ.CleanUp();
							RotationX = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							RotationX.CleanUp();
							RotationY = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							RotationY.CleanUp();
							RotationZ = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							RotationZ.CleanUp();
							ScalingX = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							ScalingX.CleanUp();
							ScalingY = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							ScalingY.CleanUp();
							ScalingXLocal = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							ScalingXLocal.CleanUp();
							ScalingYLocal = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							ScalingYLocal.CleanUp();

							RateOpacity = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							RateOpacity.CleanUp();
							RateOpacityLocal = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							RateOpacityLocal.CleanUp();
							Priority = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeInt();
							Priority.CleanUp();

							FlipX = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool();
							FlipX.CleanUp();
							FlipY = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool();
							FlipY.CleanUp();
							Hide = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool();
							Hide.CleanUp();

							PartsColor = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributePartsColor();
							PartsColor.CleanUp();
							VertexCorrection = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeVertexCorrection();
							VertexCorrection.CleanUp();

							PivotOffsetX = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							PivotOffsetX.CleanUp();
							PivotOffsetY = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							PivotOffsetY.CleanUp();

							AnchorPositionX = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							AnchorPositionX.CleanUp();
							AnchorPositionY = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							AnchorPositionY.CleanUp();
							SizeForceX = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							SizeForceX.CleanUp();
							SizeForceY = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							SizeForceY.CleanUp();

							TexturePositionX = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							TexturePositionX.CleanUp();
							TexturePositionY = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							TexturePositionY.CleanUp();
							TextureRotation = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							TextureRotation.CleanUp();
							TextureScalingX = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							TextureScalingX.CleanUp();
							TextureScalingY = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							TextureScalingY.CleanUp();
							TextureFlipX = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool();
							TextureFlipX.CleanUp();
							TextureFlipY = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool();
							TextureFlipY.CleanUp();

							RadiusCollision = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							RadiusCollision.CleanUp();
							PowerMask = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat();
							PowerMask.CleanUp();

							UserData = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeUserData();
							UserData.CleanUp();

							Instance = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeInstance();
							Instance.CleanUp();
							Effect = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeEffect();
							Effect.CleanUp();

							Deform = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeDeform();
							Deform.CleanUp();

							StatusParts = Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NOT_USED;
							TableHide = null;
							TableOrderDraw = null;
							TableOrderPreDraw = null;
						}

						public bool BootUp()
						{
							Cell.BootUp();

							PositionX.BootUp();
							PositionY.BootUp();
							PositionZ.BootUp();
							RotationX.BootUp();
							RotationY.BootUp();
							RotationZ.BootUp();
							ScalingX.BootUp();
							ScalingY.BootUp();
							ScalingXLocal.BootUp();
							ScalingYLocal.BootUp();

							RateOpacity.BootUp();
							RateOpacityLocal.BootUp();
							Priority.BootUp();

							FlipX.BootUp();
							FlipY.BootUp();
							Hide.BootUp();

							PartsColor.BootUp();
							VertexCorrection.BootUp();

							PivotOffsetX.BootUp();
							PivotOffsetY.BootUp();

							AnchorPositionX.BootUp();
							AnchorPositionY.BootUp();
							SizeForceX.BootUp();
							SizeForceY.BootUp();

							TexturePositionX.BootUp();
							TexturePositionY.BootUp();
							TextureRotation.BootUp();
							TextureScalingX.BootUp();
							TextureScalingY.BootUp();
							TextureFlipX.BootUp();
							TextureFlipY.BootUp();

							RadiusCollision.BootUp();
							PowerMask.BootUp();

							UserData.BootUp();

							Instance.BootUp();
							Effect.BootUp();

							Deform.BootUp();

							StatusParts = Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.NOT_USED;
							TableHide = null;
							TableOrderDraw = null;
							TableOrderPreDraw = null;

							return(true);
						}

						public void ShutDown()
						{
							Cell.ShutDown();

							PositionX.ShutDown();
							PositionY.ShutDown();
							PositionZ.ShutDown();
							RotationX.ShutDown();
							RotationY.ShutDown();
							RotationZ.ShutDown();
							ScalingX.ShutDown();
							ScalingY.ShutDown();
							ScalingXLocal.ShutDown();
							ScalingYLocal.ShutDown();

							RateOpacity.ShutDown();
							RateOpacityLocal.ShutDown();
							Priority.ShutDown();

							FlipX.ShutDown();
							FlipY.ShutDown();
							Hide.ShutDown();

							PartsColor.ShutDown();
							VertexCorrection.ShutDown();

							PivotOffsetX.ShutDown();
							PivotOffsetY.ShutDown();

							AnchorPositionX.ShutDown();
							AnchorPositionY.ShutDown();
							SizeForceX.ShutDown();
							SizeForceY.ShutDown();

							TexturePositionX.ShutDown();
							TexturePositionY.ShutDown();
							TextureRotation.ShutDown();
							TextureScalingX.ShutDown();
							TextureScalingY.ShutDown();
							TextureFlipX.ShutDown();
							TextureFlipY.ShutDown();

							RadiusCollision.ShutDown();
							PowerMask.ShutDown();

							UserData.ShutDown();

							Instance.ShutDown();
							Effect.ShutDown();

							Deform.ShutDown();

							StatusParts = Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus.CLEAR;
							TableHide = null;
							TableOrderDraw = null;
							TableOrderPreDraw = null;
						}
						#endregion Functions
					}
					#endregion Classes, Structs & Interfaces
				}

				public struct BindMesh
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public List<Library_SpriteStudio6.Data.Parts.Animation.BindMesh.Vertex> ListBindVertex;
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public void CleanUp()
					{
						ListBindVertex = null;
					}

					public bool BootUp()
					{
						ListBindVertex = new List<Library_SpriteStudio6.Data.Parts.Animation.BindMesh.Vertex>();
						if(null == ListBindVertex)
						{
							return(false);
						}

						ListBindVertex.Clear();
						return(true);
					}
					#endregion Functions
				}

				public struct InUseCellMap
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public int IndexCellMap;
					public Library_SpriteStudio6.KindOperationBlend Blend;
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public void CleanUp()
					{
						IndexCellMap = -1;
						Blend = Library_SpriteStudio6.KindOperationBlend.MIX;
					}
					#endregion Functions
				}

				public static class Catalog
				{
					public struct Parts
					{
						/* ----------------------------------------------- Variables & Properties */
						#region Variables & Properties
						public List<int> ListIDPartsNULL;
//						public List<int> ListIDPartsTriangle2;
//						public List<int> ListIDPartsTriangle4;
						public List<int> ListIDPartsNormal;
						public List<int> ListIDPartsInstance;
						public List<int> ListIDPartsEffect;
//						public List<int> ListIDPartsMaskTriangle2;
//						public List<int> ListIDPartsMaskTriangle4;
						public List<int> ListIDPartsMask;
						public List<int> ListIDPartsJoint;
						public List<int> ListIDPartsBone;
						public List<int> ListIDPartsMoveNode;
						public List<int> ListIDPartsConstraint;
						public List<int> ListIDPartsBonePoint;
						public List<int> ListIDPartsMesh;
						#endregion Variables & Properties

						/* ----------------------------------------------- Functions */
						#region Functions
						public void CleanUp()
						{
							ListIDPartsNULL = null;
//							ListIDPartsTriangle2 = null;
//							ListIDPartsTriangle4 = null;
							ListIDPartsNormal = null;
							ListIDPartsInstance = null;
							ListIDPartsEffect = null;
//							ListIDPartsMaskTriangle2 = null;
//							ListIDPartsMaskTriangle4 = null;
							ListIDPartsMask = null;
							ListIDPartsJoint = null;
							ListIDPartsBone = null;
							ListIDPartsMoveNode = null;
							ListIDPartsConstraint = null;
							ListIDPartsBonePoint = null;
							ListIDPartsMesh = null;
						}

						public bool BootUp()
						{
							ListIDPartsNULL = new List<int>();
//							ListIDPartsTriangle2 = new List<int>();
//							ListIDPartsTriangle4 = new List<int>();
							ListIDPartsNormal = new List<int>();
							ListIDPartsInstance = new List<int>();
							ListIDPartsEffect = new List<int>();
//							ListIDPartsMaskTriangle2 = new List<int>();
//							ListIDPartsMaskTriangle4 = new List<int>();
							ListIDPartsMask = new List<int>();
							ListIDPartsJoint = new List<int>();
							ListIDPartsBone = new List<int>();
							ListIDPartsMoveNode = new List<int>();
							ListIDPartsConstraint = new List<int>();
							ListIDPartsBonePoint = new List<int>();
							ListIDPartsMesh = new List<int>();

							if(	(null == ListIDPartsNULL)
//								|| (null == ListIDPartsTriangle2) || (null == ListIDPartsTriangle4)
								|| (null == ListIDPartsNormal)
								|| (null == ListIDPartsInstance) || (null == ListIDPartsEffect)
//								|| (null == ListIDPartsMaskTriangle2) || (null == ListIDPartsMaskTriangle4)
								|| (null == ListIDPartsMask)
								|| (null == ListIDPartsJoint) || (null == ListIDPartsBone) || (null == ListIDPartsMoveNode) || (null == ListIDPartsConstraint) || (null == ListIDPartsBonePoint) || (null == ListIDPartsMesh)
								)
							{
								return(false);
							}

							ListIDPartsNULL.Clear();
//							ListIDPartsTriangle2.Clear();
//							ListIDPartsTriangle4.Clear();
							ListIDPartsNormal.Clear();
							ListIDPartsInstance.Clear();
							ListIDPartsEffect.Clear();
//							ListIDPartsMaskTriangle2.Clear();
//							ListIDPartsMaskTriangle4.Clear();
							ListIDPartsMask.Clear();
							ListIDPartsJoint.Clear();
							ListIDPartsBone.Clear();
							ListIDPartsMoveNode.Clear();
							ListIDPartsConstraint.Clear();
							ListIDPartsBonePoint.Clear();
							ListIDPartsMesh.Clear();

							return(true);
						}
						#endregion Functions
					}
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
				public static bool AssetNameDecideData(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
														LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
														LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE,
														string nameOutputAssetFolderBase,
														Script_SpriteStudio6_DataAnimation dataOverride
													)
				{
					if(null != dataOverride)
					{	/* Specified */
						informationSSAE.DataAnimationSS6PU.TableName[0] = AssetDatabase.GetAssetPath(dataOverride);
					}
					else
					{	/* Default */
						informationSSAE.DataAnimationSS6PU.TableName[0] = setting.RuleNameAssetFolder.NameGetAssetFolder(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_ANIMATION_SS6PU, nameOutputAssetFolderBase)
																		+ setting.RuleNameAsset.NameGetAsset(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_ANIMATION_SS6PU, informationSSAE.NameFileBody, informationSSPJ.NameFileBody)
																		+ LibraryEditor_SpriteStudio6.Import.NameExtentionScriptableObject;
						dataOverride = AssetDatabase.LoadAssetAtPath<Script_SpriteStudio6_DataAnimation>(informationSSAE.DataAnimationSS6PU.TableName[0]);
					}
					informationSSAE.DataAnimationSS6PU.TableData[0] = dataOverride;
					informationSSAE.DataAnimationSS6PU.Version[0] = (null != dataOverride) ? (int)(dataOverride.Version) : (int)Script_SpriteStudio6_DataAnimation.KindVersion.SS5PU;

					return(true);

//				AssetNameDecideData_ErroeEnd:;
//					return(false);
				}

				public static bool AssetNameDecidePrefab(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
															LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
															LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE,
															string nameOutputAssetFolderBase,
															Script_SpriteStudio6_Root prefabOverride
														)
				{
					if(null != prefabOverride)
					{	/* Specified */
						informationSSAE.NameGameObjectAnimationSS6PU = string.Copy(prefabOverride.name);

						informationSSAE.PrefabAnimationSS6PU.TableName[0] = AssetDatabase.GetAssetPath(prefabOverride);
						informationSSAE.PrefabAnimationSS6PU.TableData[0] = prefabOverride;
					}
					else
					{	/* Default */
						informationSSAE.NameGameObjectAnimationSS6PU = setting.RuleNameAsset.NameGetAsset(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_ANIMATION_SS6PU, informationSSAE.NameFileBody, informationSSPJ.NameFileBody);

						informationSSAE.PrefabAnimationSS6PU.TableName[0] = setting.RuleNameAssetFolder.NameGetAssetFolder(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_ANIMATION_SS6PU, nameOutputAssetFolderBase)
																			+ informationSSAE.NameGameObjectAnimationSS6PU
																			+ LibraryEditor_SpriteStudio6.Import.NameExtensionPrefab;
						informationSSAE.PrefabAnimationSS6PU.TableData[0] = AssetDatabase.LoadAssetAtPath<GameObject>(informationSSAE.PrefabAnimationSS6PU.TableName[0]);
					}

					/* MEMO: "Control-Prefab" creates only the name. */
					informationSSAE.NameGameObjectAnimationControlSS6PU = setting.RuleNameAsset.NameGetAsset(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_CONTROL_ANIMATION_SS6PU, informationSSAE.NameFileBody, informationSSPJ.NameFileBody);
					informationSSAE.PrefabControlAnimationSS6PU.TableName[0] = setting.RuleNameAssetFolder.NameGetAssetFolder(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_CONTROL_ANIMATION_SS6PU, nameOutputAssetFolderBase)
																				+ informationSSAE.NameGameObjectAnimationControlSS6PU
																				+ LibraryEditor_SpriteStudio6.Import.NameExtensionPrefab;
					informationSSAE.PrefabControlAnimationSS6PU.TableData[0] = AssetDatabase.LoadAssetAtPath<GameObject>(informationSSAE.PrefabControlAnimationSS6PU.TableName[0]);
					informationSSAE.PrefabControlAnimationSS6PU.Version[0] = -1;

					return(true);

//				AssetNameDecideData_ErroeEnd:;
//					return(false);
				}
#endif

#if !_FOR_SS6CONVERTER_
				public static bool AssetCreateData(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
													LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
													LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE
												)
				{
//					const string messageLogPrefix = "Create Asset(Data-Animation)";

					Script_SpriteStudio6_DataAnimation dataAnimation = informationSSAE.DataAnimationSS6PU.TableData[0];
					if(null == dataAnimation)
					{
						dataAnimation = ScriptableObject.CreateInstance<Script_SpriteStudio6_DataAnimation>();
						AssetDatabase.CreateAsset(dataAnimation, informationSSAE.DataAnimationSS6PU.TableName[0]);
						informationSSAE.DataAnimationSS6PU.TableData[0] = dataAnimation;
					}

					dataAnimation.Version = Script_SpriteStudio6_DataAnimation.KindVersion.SUPPORT_LATEST;

					int countParts = informationSSAE.TableParts.Length;
					Library_SpriteStudio6.Data.Parts.Animation[] tablePartsRuntime = new Library_SpriteStudio6.Data.Parts.Animation[countParts];
					if(null == tablePartsRuntime)
					{
						goto AssetCreateData_ErrorEnd;
					}
					for(int i=0; i<countParts; i++)
					{
						tablePartsRuntime[i] = informationSSAE.TableParts[i].Data;
					}
					dataAnimation.TableParts = tablePartsRuntime;

					int countAnimation = informationSSAE.TableAnimation.Length;
					Library_SpriteStudio6.Data.Animation[] tableAnimationRuntime = new Library_SpriteStudio6.Data.Animation[countAnimation];
					if(null == tableAnimationRuntime)
					{
						goto AssetCreateData_ErrorEnd;
					}
					for(int i=0; i<countAnimation; i++)
					{
						tableAnimationRuntime[i] = informationSSAE.TableAnimation[i].Data;
					}
					dataAnimation.TableAnimation = tableAnimationRuntime;

					if(null != informationSSAE.AnimationSetup)
					{	/* Has Setup animation */
						if(null != informationSSAE.AnimationSetup.TableParts)
						{	/* Has Animation-Parts table */
							Script_SpriteStudio6_DataAnimation.DataSetup[] tableAnimationPartsSetup = new Script_SpriteStudio6_DataAnimation.DataSetup[countParts];
							if(null == tableAnimationPartsSetup)
							{
								goto AssetCreateData_ErrorEnd;
							}

							LibraryEditor_SpriteStudio6.Import.SSAE.Information.Animation.Parts informationAnimationParts = null;
#if STORE_ANIMATIONSETUP_FULL
							Library_SpriteStudio6.Data.Animation.Attribute.Status dataStatus = new Library_SpriteStudio6.Data.Animation.Attribute.Status();
#endif
							for(int i=0; i<countParts; i++)
							{
								informationAnimationParts = informationSSAE.AnimationSetup.TableParts[i];

								tableAnimationPartsSetup[i].CleanUp();

#if STORE_ANIMATIONSETUP_FULL
								{
									/* MEMO: "Status" is absolutely written. */
									bool valueAttribute;

									dataStatus.CleanUp();
									Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetBoolOR(	out valueAttribute,
																														informationAnimationParts.Hide,
																														0,
																														true
																													);
									dataStatus.Flags |= (true == valueAttribute) ? Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.HIDE : Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.CLEAR;

									Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetBoolToggle(	out valueAttribute,
																															informationAnimationParts.FlipX,
																															0
																														);
									dataStatus.Flags |= (true == valueAttribute) ? Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.FLIP_X : Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.CLEAR;

									Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetBoolToggle(	out valueAttribute,
																															informationAnimationParts.FlipY,
																															0
																														);
									dataStatus.Flags |= (true == valueAttribute) ? Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.FLIP_Y : Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.CLEAR;

									Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetBoolToggle(	out valueAttribute,
																															informationAnimationParts.TextureFlipX,
																															0
																														);
									dataStatus.Flags |= (true == valueAttribute) ? Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.FLIP_TEXTURE_X : Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.CLEAR;

									Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetBoolToggle(	out valueAttribute,
																															informationAnimationParts.TextureFlipY,
																															0
																														);
									dataStatus.Flags |= (true == valueAttribute) ? Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.FLIP_TEXTURE_Y : Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.CLEAR;

									dataStatus.Flags |= Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.ID_PARTS_NEXTDRAW;
									dataStatus.Flags |= Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.ID_PARTS_NEXTPREDRAW;

									dataStatus.Flags |= Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.VALID;

									tableAnimationPartsSetup[i].Status = dataStatus;
								}

								if(0 < informationAnimationParts.PositionX.CountGetKey())
								{
									tableAnimationPartsSetup[i].Position.x = informationAnimationParts.PositionX.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.PositionY.CountGetKey())
								{
									tableAnimationPartsSetup[i].Position.y = informationAnimationParts.PositionY.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.PositionZ.CountGetKey())
								{
									tableAnimationPartsSetup[i].Position.z = informationAnimationParts.PositionZ.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.RotationX.CountGetKey())
								{
									tableAnimationPartsSetup[i].Rotation.x = informationAnimationParts.RotationX.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.RotationY.CountGetKey())
								{
									tableAnimationPartsSetup[i].Rotation.y = informationAnimationParts.RotationY.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.RotationZ.CountGetKey())
								{
									tableAnimationPartsSetup[i].Rotation.z = informationAnimationParts.RotationZ.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.ScalingX.CountGetKey())
								{
									tableAnimationPartsSetup[i].Scaling.x = informationAnimationParts.ScalingX.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.ScalingY.CountGetKey())
								{
									tableAnimationPartsSetup[i].Scaling.y = informationAnimationParts.ScalingY.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.ScalingXLocal.CountGetKey())
								{
									tableAnimationPartsSetup[i].ScalingLocal.x = informationAnimationParts.ScalingXLocal.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.ScalingYLocal.CountGetKey())
								{
									tableAnimationPartsSetup[i].ScalingLocal.y = informationAnimationParts.ScalingYLocal.ListKey[0].Value;
								}

								/* MEMO: "RateOpacity" is absolutely written. */
								informationAnimationParts.RateOpacity.ValueGet(out tableAnimationPartsSetup[i].RateOpacity, 0);
								if(0 < informationAnimationParts.RateOpacityLocal.CountGetKey())
								{
									tableAnimationPartsSetup[i].RateOpacity = informationAnimationParts.RateOpacityLocal.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.PowerMask.CountGetKey())
								{
									tableAnimationPartsSetup[i].PowerMask = (int)informationAnimationParts.PowerMask.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.Priority.CountGetKey())
								{
									tableAnimationPartsSetup[i].Priority = informationAnimationParts.Priority.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.PartsColor.CountGetKey())
								{
									tableAnimationPartsSetup[i].PartsColor = informationAnimationParts.PartsColor.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.VertexCorrection.CountGetKey())
								{
									tableAnimationPartsSetup[i].VertexCorrection = informationAnimationParts.VertexCorrection.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.PivotOffsetX.CountGetKey())
								{
									tableAnimationPartsSetup[i].OffsetPivot.x = informationAnimationParts.PivotOffsetX.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.PivotOffsetY.CountGetKey())
								{
									tableAnimationPartsSetup[i].OffsetPivot.y = informationAnimationParts.PivotOffsetY.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.AnchorPositionX.CountGetKey())
								{
									tableAnimationPartsSetup[i].PositionAnchor.x = informationAnimationParts.AnchorPositionX.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.AnchorPositionY.CountGetKey())
								{
									tableAnimationPartsSetup[i].PositionAnchor.y = informationAnimationParts.AnchorPositionY.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.SizeForceX.CountGetKey())
								{
									tableAnimationPartsSetup[i].SizeForce.x = informationAnimationParts.SizeForceX.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.SizeForceY.CountGetKey())
								{
									tableAnimationPartsSetup[i].SizeForce.y = informationAnimationParts.SizeForceY.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.TexturePositionX.CountGetKey())
								{
									tableAnimationPartsSetup[i].PositionTexture.x = informationAnimationParts.TexturePositionX.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.TexturePositionY.CountGetKey())
								{
									tableAnimationPartsSetup[i].PositionTexture.y = informationAnimationParts.TexturePositionY.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.TextureRotation.CountGetKey())
								{
									tableAnimationPartsSetup[i].RotationTexture = informationAnimationParts.TextureRotation.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.TextureScalingX.CountGetKey())
								{
									tableAnimationPartsSetup[i].ScalingTexture.x = informationAnimationParts.TextureScalingX.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.TextureScalingY.CountGetKey())
								{
									tableAnimationPartsSetup[i].ScalingTexture.y = informationAnimationParts.TextureScalingY.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.RadiusCollision.CountGetKey())
								{
									tableAnimationPartsSetup[i].RadiusCollision = informationAnimationParts.RadiusCollision.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.Instance.CountGetKey())
								{
									tableAnimationPartsSetup[i].Instance = informationAnimationParts.Instance.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.Effect.CountGetKey())
								{
									tableAnimationPartsSetup[i].Effect = informationAnimationParts.Effect.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.Deform.CountGetKey())
								{
									/* MEMO: "Setup" data is converted here.                                      */
									/*       Because "Deform"'s format is different from animation data's format. */
									tableAnimationPartsSetup[i].Deform.CountVertexMesh = informationAnimationParts.Deform.ListKey[0].Value.CountVertexMesh;
									int countVertex = informationAnimationParts.Deform.ListKey[0].Value.TableVertex.Length;
									int[] tableIndexVertex =  new int[countVertex];
									Vector2[] tableCoordinate = new Vector2[countVertex];
									tableAnimationPartsSetup[i].Deform.TableIndexVertex = tableIndexVertex;
									tableAnimationPartsSetup[i].Deform.TableCoordinate = tableCoordinate;
									for(int j=0; j<countVertex; j++)
									{
										tableIndexVertex[j] = informationAnimationParts.Deform.ListKey[0].Value.TableVertex[i].Index;
										tableCoordinate[j] = informationAnimationParts.Deform.ListKey[0].Value.TableVertex[i].Coordinate;
									}
								}
#endif
								if(0 < informationAnimationParts.UserData.CountGetKey())
								{
									tableAnimationPartsSetup[i].UserData = informationAnimationParts.UserData.ListKey[0].Value;
								}
							}
							dataAnimation.TableAnimationPartsSetup = tableAnimationPartsSetup;
						}
					}

					dataAnimation.CatalogParts.TableIDPartsNULL = informationSSAE.CatalogParts.ListIDPartsNULL.ToArray();
//					dataAnimation.CatalogParts.TableIDPartsTriangle2 = informationSSAE.CatalogParts.ListIDPartsTriangle2.ToArray();
//					dataAnimation.CatalogParts.TableIDPartsTriangle4 = informationSSAE.CatalogParts.ListIDPartsTriangle4.ToArray();
					dataAnimation.CatalogParts.TableIDPartsNormal = informationSSAE.CatalogParts.ListIDPartsNormal.ToArray();
					dataAnimation.CatalogParts.TableIDPartsInstance = informationSSAE.CatalogParts.ListIDPartsInstance.ToArray();
					dataAnimation.CatalogParts.TableIDPartsEffect = informationSSAE.CatalogParts.ListIDPartsEffect.ToArray();
//					dataAnimation.CatalogParts.TableIDPartsMaskTriangle2 = informationSSAE.CatalogParts.ListIDPartsMaskTriangle2.ToArray();
//					dataAnimation.CatalogParts.TableIDPartsMaskTriangle4 = informationSSAE.CatalogParts.ListIDPartsMaskTriangle4.ToArray();
					dataAnimation.CatalogParts.TableIDPartsMask = informationSSAE.CatalogParts.ListIDPartsMask.ToArray();
					dataAnimation.CatalogParts.TableIDPartsJoint = informationSSAE.CatalogParts.ListIDPartsJoint.ToArray();
					dataAnimation.CatalogParts.TableIDPartsBone = informationSSAE.CatalogParts.ListIDPartsBone.ToArray();
					dataAnimation.CatalogParts.TableIDPartsMoveNode = informationSSAE.CatalogParts.ListIDPartsMoveNode.ToArray();
					dataAnimation.CatalogParts.TableIDPartsConstraint = informationSSAE.CatalogParts.ListIDPartsConstraint.ToArray();
					dataAnimation.CatalogParts.TableIDPartsBonePoint = informationSSAE.CatalogParts.ListIDPartsBonePoint.ToArray();
					dataAnimation.CatalogParts.TableIDPartsMesh = informationSSAE.CatalogParts.ListIDPartsMesh.ToArray();

					dataAnimation.TableMaterial = informationSSPJ.TableMaterialAnimationSS6PU;	/* Back up original */

					EditorUtility.SetDirty(dataAnimation);
					AssetDatabase.SaveAssets();

					return(true);

				AssetCreateData_ErrorEnd:;
					informationSSAE.DataAnimationSS6PU.TableData[0] = null;
					return(false);
				}
#else
				public static bool OutputData(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
												LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
												LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE
											)
				{
//					const string messageLogPrefix = "Create Asset(Data-Animation)";

					Script_SpriteStudio6_DataAnimation dataAnimation = new Script_SpriteStudio6_DataAnimation();
					if(null == dataAnimation)
					{
						return(false);
					}
					informationSSAE.DataAnimationSS6PU.TableData[0] = dataAnimation;

					dataAnimation.Version = Script_SpriteStudio6_DataAnimation.KindVersion.SUPPORT_LATEST;

					int countParts = informationSSAE.TableParts.Length;
					Library_SpriteStudio6.Data.Parts.Animation[] tablePartsRuntime = new Library_SpriteStudio6.Data.Parts.Animation[countParts];
					if(null == tablePartsRuntime)
					{
						goto DataOutput_ErrorEnd;
					}
					for(int i=0; i<countParts; i++)
					{
						tablePartsRuntime[i] = informationSSAE.TableParts[i].Data;
					}
					dataAnimation.TableParts = tablePartsRuntime;

					int countAnimation = informationSSAE.TableAnimation.Length;
					Library_SpriteStudio6.Data.Animation[] tableAnimationRuntime = new Library_SpriteStudio6.Data.Animation[countAnimation];
					if(null == tableAnimationRuntime)
					{
						goto DataOutput_ErrorEnd;
					}
					for(int i=0; i<countAnimation; i++)
					{
						tableAnimationRuntime[i] = informationSSAE.TableAnimation[i].Data;
					}
					dataAnimation.TableAnimation = tableAnimationRuntime;

					if(null != informationSSAE.AnimationSetup)
					{	/* Has Setup animation */
						if(null != informationSSAE.AnimationSetup.TableParts)
						{	/* Has Animation-Parts table */
							Script_SpriteStudio6_DataAnimation.DataSetup[] tableAnimationPartsSetup = new Script_SpriteStudio6_DataAnimation.DataSetup[countParts];
							if(null == tableAnimationPartsSetup)
							{
								goto DataOutput_ErrorEnd;
							}

							LibraryEditor_SpriteStudio6.Import.SSAE.Information.Animation.Parts informationAnimationParts = null;
#if STORE_ANIMATIONSETUP_FULL
							Library_SpriteStudio6.Data.Animation.Attribute.Status dataStatus = new Library_SpriteStudio6.Data.Animation.Attribute.Status();
#endif
							for(int i=0; i<countParts; i++)
							{
								informationAnimationParts = informationSSAE.AnimationSetup.TableParts[i];

								tableAnimationPartsSetup[i].CleanUp();

#if STORE_ANIMATIONSETUP_FULL
								{
									/* MEMO: "Status" is absolutely written. */
									bool valueAttribute;

									dataStatus.CleanUp();
									Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetBoolOR(	out valueAttribute,
																														informationAnimationParts.Hide,
																														0,
																														true
																													);
									dataStatus.Flags |= (true == valueAttribute) ? Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.HIDE : Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.CLEAR;

									Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetBoolToggle(	out valueAttribute,
																															informationAnimationParts.FlipX,
																															0
																														);
									dataStatus.Flags |= (true == valueAttribute) ? Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.FLIP_X : Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.CLEAR;

									Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetBoolToggle(	out valueAttribute,
																															informationAnimationParts.FlipY,
																															0
																														);
									dataStatus.Flags |= (true == valueAttribute) ? Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.FLIP_Y : Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.CLEAR;

									Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetBoolToggle(	out valueAttribute,
																															informationAnimationParts.TextureFlipX,
																															0
																														);
									dataStatus.Flags |= (true == valueAttribute) ? Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.FLIP_TEXTURE_X : Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.CLEAR;

									Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetBoolToggle(	out valueAttribute,
																															informationAnimationParts.TextureFlipY,
																															0
																														);
									dataStatus.Flags |= (true == valueAttribute) ? Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.FLIP_TEXTURE_Y : Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.CLEAR;

									dataStatus.Flags |= Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.ID_PARTS_NEXTDRAW;
									dataStatus.Flags |= Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.ID_PARTS_NEXTPREDRAW;

									dataStatus.Flags |= Library_SpriteStudio6.Data.Animation.Attribute.Status.FlagBit.VALID;

									tableAnimationPartsSetup[i].Status = dataStatus;
								}

								if(0 < informationAnimationParts.PositionX.CountGetKey())
								{
									tableAnimationPartsSetup[i].Position.x = informationAnimationParts.PositionX.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.PositionY.CountGetKey())
								{
									tableAnimationPartsSetup[i].Position.y = informationAnimationParts.PositionY.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.PositionZ.CountGetKey())
								{
									tableAnimationPartsSetup[i].Position.z = informationAnimationParts.PositionZ.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.RotationX.CountGetKey())
								{
									tableAnimationPartsSetup[i].Rotation.x = informationAnimationParts.RotationX.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.RotationY.CountGetKey())
								{
									tableAnimationPartsSetup[i].Rotation.y = informationAnimationParts.RotationY.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.RotationZ.CountGetKey())
								{
									tableAnimationPartsSetup[i].Rotation.z = informationAnimationParts.RotationZ.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.ScalingX.CountGetKey())
								{
									tableAnimationPartsSetup[i].Scaling.x = informationAnimationParts.ScalingX.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.ScalingY.CountGetKey())
								{
									tableAnimationPartsSetup[i].Scaling.y = informationAnimationParts.ScalingY.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.ScalingXLocal.CountGetKey())
								{
									tableAnimationPartsSetup[i].ScalingLocal.x = informationAnimationParts.ScalingXLocal.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.ScalingYLocal.CountGetKey())
								{
									tableAnimationPartsSetup[i].ScalingLocal.y = informationAnimationParts.ScalingYLocal.ListKey[0].Value;
								}

								/* MEMO: "RateOpacity" is absolutely written. */
								informationAnimationParts.RateOpacity.ValueGet(out tableAnimationPartsSetup[i].RateOpacity, 0);
								if(0 < informationAnimationParts.RateOpacityLocal.CountGetKey())
								{
									tableAnimationPartsSetup[i].RateOpacity = informationAnimationParts.RateOpacityLocal.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.PowerMask.CountGetKey())
								{
									tableAnimationPartsSetup[i].PowerMask = (int)informationAnimationParts.PowerMask.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.Priority.CountGetKey())
								{
									tableAnimationPartsSetup[i].Priority = informationAnimationParts.Priority.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.PartsColor.CountGetKey())
								{
									tableAnimationPartsSetup[i].PartsColor = informationAnimationParts.PartsColor.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.VertexCorrection.CountGetKey())
								{
									tableAnimationPartsSetup[i].VertexCorrection = informationAnimationParts.VertexCorrection.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.PivotOffsetX.CountGetKey())
								{
									tableAnimationPartsSetup[i].OffsetPivot.x = informationAnimationParts.PivotOffsetX.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.PivotOffsetY.CountGetKey())
								{
									tableAnimationPartsSetup[i].OffsetPivot.y = informationAnimationParts.PivotOffsetY.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.AnchorPositionX.CountGetKey())
								{
									tableAnimationPartsSetup[i].PositionAnchor.x = informationAnimationParts.AnchorPositionX.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.AnchorPositionY.CountGetKey())
								{
									tableAnimationPartsSetup[i].PositionAnchor.y = informationAnimationParts.AnchorPositionY.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.SizeForceX.CountGetKey())
								{
									tableAnimationPartsSetup[i].SizeForce.x = informationAnimationParts.SizeForceX.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.SizeForceY.CountGetKey())
								{
									tableAnimationPartsSetup[i].SizeForce.y = informationAnimationParts.SizeForceY.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.TexturePositionX.CountGetKey())
								{
									tableAnimationPartsSetup[i].PositionTexture.x = informationAnimationParts.TexturePositionX.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.TexturePositionY.CountGetKey())
								{
									tableAnimationPartsSetup[i].PositionTexture.y = informationAnimationParts.TexturePositionY.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.TextureRotation.CountGetKey())
								{
									tableAnimationPartsSetup[i].RotationTexture = informationAnimationParts.TextureRotation.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.TextureScalingX.CountGetKey())
								{
									tableAnimationPartsSetup[i].ScalingTexture.x = informationAnimationParts.TextureScalingX.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.TextureScalingY.CountGetKey())
								{
									tableAnimationPartsSetup[i].ScalingTexture.y = informationAnimationParts.TextureScalingY.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.RadiusCollision.CountGetKey())
								{
									tableAnimationPartsSetup[i].RadiusCollision = informationAnimationParts.RadiusCollision.ListKey[0].Value;
								}

								if(0 < informationAnimationParts.Instance.CountGetKey())
								{
									tableAnimationPartsSetup[i].Instance = informationAnimationParts.Instance.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.Effect.CountGetKey())
								{
									tableAnimationPartsSetup[i].Effect = informationAnimationParts.Effect.ListKey[0].Value;
								}
								if(0 < informationAnimationParts.Deform.CountGetKey())
								{
									/* MEMO: "Setup" data is converted here.                                      */
									/*       Because "Deform"'s format is different from animation data's format. */
									tableAnimationPartsSetup[i].Deform.CountVertexMesh = informationAnimationParts.Deform.ListKey[0].Value.CountVertexMesh;
									int countVertex = informationAnimationParts.Deform.ListKey[0].Value.TableVertex.Length;
									int[] tableIndexVertex =  new int[countVertex];
									Vector2[] tableCoordinate = new Vector2[countVertex];
									tableAnimationPartsSetup[i].Deform.TableIndexVertex = tableIndexVertex;
									tableAnimationPartsSetup[i].Deform.TableCoordinate = tableCoordinate;
									for(int j=0; j<countVertex; j++)
									{
										tableIndexVertex[j] = informationAnimationParts.Deform.ListKey[0].Value.TableVertex[i].Index;
										tableCoordinate[j] = informationAnimationParts.Deform.ListKey[0].Value.TableVertex[i].Coordinate;
									}
								}
#endif
								if(0 < informationAnimationParts.UserData.CountGetKey())
								{
									tableAnimationPartsSetup[i].UserData = informationAnimationParts.UserData.ListKey[0].Value;
								}
							}
							dataAnimation.TableAnimationPartsSetup = tableAnimationPartsSetup;
						}
					}

					dataAnimation.CatalogParts.TableIDPartsNULL = informationSSAE.CatalogParts.ListIDPartsNULL.ToArray();
//					dataAnimation.CatalogParts.TableIDPartsTriangle2 = informationSSAE.CatalogParts.ListIDPartsTriangle2.ToArray();
//					dataAnimation.CatalogParts.TableIDPartsTriangle4 = informationSSAE.CatalogParts.ListIDPartsTriangle4.ToArray();
					dataAnimation.CatalogParts.TableIDPartsNormal = informationSSAE.CatalogParts.ListIDPartsNormal.ToArray();
					dataAnimation.CatalogParts.TableIDPartsInstance = informationSSAE.CatalogParts.ListIDPartsInstance.ToArray();
					dataAnimation.CatalogParts.TableIDPartsEffect = informationSSAE.CatalogParts.ListIDPartsEffect.ToArray();
//					dataAnimation.CatalogParts.TableIDPartsMaskTriangle2 = informationSSAE.CatalogParts.ListIDPartsMaskTriangle2.ToArray();
//					dataAnimation.CatalogParts.TableIDPartsMaskTriangle4 = informationSSAE.CatalogParts.ListIDPartsMaskTriangle4.ToArray();
					dataAnimation.CatalogParts.TableIDPartsMask = informationSSAE.CatalogParts.ListIDPartsMask.ToArray();
					dataAnimation.CatalogParts.TableIDPartsJoint = informationSSAE.CatalogParts.ListIDPartsJoint.ToArray();
					dataAnimation.CatalogParts.TableIDPartsBone = informationSSAE.CatalogParts.ListIDPartsBone.ToArray();
					dataAnimation.CatalogParts.TableIDPartsMoveNode = informationSSAE.CatalogParts.ListIDPartsMoveNode.ToArray();
					dataAnimation.CatalogParts.TableIDPartsConstraint = informationSSAE.CatalogParts.ListIDPartsConstraint.ToArray();
					dataAnimation.CatalogParts.TableIDPartsBonePoint = informationSSAE.CatalogParts.ListIDPartsBonePoint.ToArray();
					dataAnimation.CatalogParts.TableIDPartsMesh = informationSSAE.CatalogParts.ListIDPartsMesh.ToArray();

					return(true);

				DataOutput_ErrorEnd:;
					informationSSAE.DataAnimationSS6PU.TableData[0] = null;
					return(false);
				}
#endif

#if !_FOR_SS6CONVERTER_
				public static bool AssetCreatePrefab(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
														LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
														LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE
													)
				{
					const string messageLogPrefix = "Create Asset(Prefab-Animation)";

					GameObject gameObjectRoot = null;
					Script_SpriteStudio6_Root scriptRoot = null;
					Script_SpriteStudio6_Root.InformationPlay[] informationPlayRoot = null;
					string[] nameAnimation = null;
					bool flagHideForce = false;
					bool flagPlanarization = false;
					int orderInLayer = 0;
					int limitTrack = 0;
					int indexAnimation;
					MeshRenderer meshRenderer = null;
					bool flagExistMeshRendererOriginal = false;
					UnityEngine.Rendering.LightProbeUsage lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
					UnityEngine.Rendering.ReflectionProbeUsage reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
					Transform probeAnchor = null;
					UnityEngine.Rendering.ShadowCastingMode shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
					bool receiveShadows = false;
					MotionVectorGenerationMode motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
					bool lightmapStatic = false;
					bool allowOcclusionWhenDynamic = false;
					int lightmapIndex = -1;
					Vector4 lightmapScaleOffset = Vector4.zero;
					GameObject lightProbeProxyVolumeOverride = null;
					int realtimeLightmapIndex = -1;
					Vector4 realtimeLightmapScaleOffset = Vector4.zero;

					/* Create? Update? */
					if(null == informationSSAE.PrefabAnimationSS6PU.TableData[0])
					{	/* New */
#if UNITY_2018_4_OR_NEWER || UNITY_2019_1_OR_NEWER
						/* MEMO: Process nothing, now. */
#else
						informationSSAE.PrefabAnimationSS6PU.TableData[0] = PrefabUtility.CreateEmptyPrefab(informationSSAE.PrefabAnimationSS6PU.TableName[0]);
						if(null == informationSSAE.PrefabAnimationSS6PU.TableData[0])
						{
							LogError(messageLogPrefix, "Failure to create Prefab", informationSSAE.FileNameGetFullPath(), informationSSPJ);
							goto AssetCreatePrefab_ErrorEnd;
						}
#endif
					}
					else
					{	/* Exist */
						/* MEMO: Do not instantiate old prefabs. Instantiates up to objects under control, and mixed in updated prefab. */
						gameObjectRoot = (GameObject)informationSSAE.PrefabAnimationSS6PU.TableData[0];
#if UNITY_2018_4_OR_NEWER || UNITY_2019_1_OR_NEWER
						informationSSAE.PrefabAnimationSS6PU.TableName[0] = AssetDatabase.GetAssetPath(gameObjectRoot);
#else
#endif
						scriptRoot = gameObjectRoot.GetComponent<Script_SpriteStudio6_Root>();
						if(null != scriptRoot)
						{
							flagHideForce = scriptRoot.FlagHideForce;
							flagPlanarization = scriptRoot.FlagPlanarization;
							orderInLayer = scriptRoot.OrderInLayer;
							limitTrack = scriptRoot.LimitTrack;

							if(null != scriptRoot.TableInformationPlay)
							{
								int countInformationPlay = scriptRoot.TableInformationPlay.Length;
								if(0 < countInformationPlay)
								{
									informationPlayRoot = new Script_SpriteStudio6_Root.InformationPlay[countInformationPlay];
									nameAnimation = new string[countInformationPlay];
									if((null == informationPlayRoot) || (null == nameAnimation))
									{
										LogError(messageLogPrefix, "Not Enough Memory (Play-Information BackUp)", informationSSAE.FileNameGetFullPath(), informationSSPJ);
										goto AssetCreatePrefab_ErrorEnd;
									}

									for(int i=0; i<countInformationPlay; i++)
									{
										informationPlayRoot[i].FlagSetInitial = scriptRoot.TableInformationPlay[i].FlagSetInitial;
										informationPlayRoot[i].FlagStopInitial = scriptRoot.TableInformationPlay[i].FlagStopInitial;

										nameAnimation[i] = scriptRoot.TableInformationPlay[i].NameAnimation;
										informationPlayRoot[i].NameAnimation = "";
										informationPlayRoot[i].FlagPingPong = scriptRoot.TableInformationPlay[i].FlagPingPong;
										informationPlayRoot[i].LabelStart = (false == string.IsNullOrEmpty(scriptRoot.TableInformationPlay[i].LabelStart)) ? string.Copy(scriptRoot.TableInformationPlay[i].LabelStart) : "";
										informationPlayRoot[i].FrameOffsetStart = scriptRoot.TableInformationPlay[i].FrameOffsetStart;
										informationPlayRoot[i].LabelEnd = (false == string.IsNullOrEmpty(scriptRoot.TableInformationPlay[i].LabelEnd)) ? string.Copy(scriptRoot.TableInformationPlay[i].LabelEnd) : "";
										informationPlayRoot[i].FrameOffsetEnd = scriptRoot.TableInformationPlay[i].FrameOffsetEnd;
										informationPlayRoot[i].Frame = scriptRoot.TableInformationPlay[i].Frame;
										informationPlayRoot[i].TimesPlay = scriptRoot.TableInformationPlay[i].TimesPlay;
										informationPlayRoot[i].RateTime = scriptRoot.TableInformationPlay[i].RateTime;
									}
								}
							}
						}

						meshRenderer = gameObjectRoot.GetComponent<MeshRenderer>();
						if(null != meshRenderer)
						{
							if(true == setting.Basic.FlagTakeOverLightRenderer)
							{
								flagExistMeshRendererOriginal = true;

								lightProbeUsage = meshRenderer.lightProbeUsage;
								reflectionProbeUsage = meshRenderer.reflectionProbeUsage;
								probeAnchor = meshRenderer.probeAnchor;
								shadowCastingMode = meshRenderer.shadowCastingMode;
								receiveShadows = meshRenderer.receiveShadows;
								motionVectorGenerationMode = meshRenderer.motionVectorGenerationMode;
								allowOcclusionWhenDynamic = meshRenderer.allowOcclusionWhenDynamic;
								lightmapStatic = (0 != (GameObjectUtility.GetStaticEditorFlags(gameObjectRoot) & StaticEditorFlags.LightmapStatic)) ? true : false;
								lightmapIndex = meshRenderer.lightmapIndex;
								lightmapScaleOffset = meshRenderer.lightmapScaleOffset;
								lightProbeProxyVolumeOverride = meshRenderer.lightProbeProxyVolumeOverride;
								realtimeLightmapIndex = meshRenderer.realtimeLightmapIndex;
								realtimeLightmapScaleOffset = meshRenderer.realtimeLightmapScaleOffset;
							}
						}

						gameObjectRoot = null;
						scriptRoot = null;
					}
					if(null == informationPlayRoot)
					{
						informationPlayRoot = new Script_SpriteStudio6_Root.InformationPlay[1];
						nameAnimation = new string[1];
						if(null == informationPlayRoot)
						{
							LogError(messageLogPrefix, "Not Enough Memory (Play-Information BackUp)", informationSSAE.FileNameGetFullPath(), informationSSPJ);
							goto AssetCreatePrefab_ErrorEnd;
						}

						nameAnimation[0] = "";
						informationPlayRoot[0].CleanUp();
						informationPlayRoot[0].FlagSetInitial = true;
					}

					/* Create new GameObject (Root) */
					gameObjectRoot = Library_SpriteStudio6.Utility.Asset.GameObjectCreate(informationSSAE.NameGameObjectAnimationSS6PU, false, null);	/* informationSSAE.NameFileBody */
					if(null == gameObjectRoot)
					{
						LogError(messageLogPrefix, "Failure to get Temporary-GameObject", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						goto AssetCreatePrefab_ErrorEnd;
					}
//					gameObjectRoot.name = informationSSAE.NameGameObjectAnimationSS6PU;	/* informationSSAE.NameFileBody; */	/* Give Root same name as SSAE */
					scriptRoot = gameObjectRoot.AddComponent<Script_SpriteStudio6_Root>();
					if(null == scriptRoot)
					{
						LogError(messageLogPrefix, "Failure to add component\"Script_SpriteStudio6_Root\"", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						goto AssetCreatePrefab_ErrorEnd;
					}
					if((true == setting.Basic.FlagTakeOverLightRenderer) || (true == setting.Basic.FlagDisableInitialLightRenderer))
					{
						meshRenderer = gameObjectRoot.AddComponent<MeshRenderer>();
					}

					/* Make GameObject & Parts controllers */
					/* MEMO: Should be stored keeping permutation of parent and child. */
					int countParts = informationSSAE.TableParts.Length;
					Library_SpriteStudio6.Control.Animation.Parts[] tableControlParts = new Library_SpriteStudio6.Control.Animation.Parts[countParts];
					tableControlParts[0].InstanceGameObject = gameObjectRoot;

					GameObject gameObjectParent = null;
					GameObject gameObjectParts = null;
					Script_SpriteStudio6_Collider scriptCollider = null;
					int indexPartsParent;
					bool flagAttachCollider;
					for(int i=0; i<countParts; i++)
					{
						if(0 >= i)
						{	/* "Root" */
							indexPartsParent = -1;
							gameObjectParent = null;
							gameObjectParts = gameObjectRoot;
//							gameObjectParts.name = 
//							tableControlParts[0].InstanceGameObject = gameObjectParts;
						}
						else
						{	/* Not "Root" */
							indexPartsParent = informationSSAE.TableParts[i].Data.IDParent;
							gameObjectParent = (0 <= indexPartsParent) ? tableControlParts[indexPartsParent].InstanceGameObject : null;
							gameObjectParts = Library_SpriteStudio6.Utility.Asset.GameObjectCreate(informationSSAE.TableParts[i].Data.Name, true, gameObjectParent);
							gameObjectParts.name = informationSSAE.TableParts[i].Data.Name;
							tableControlParts[i].InstanceGameObject = gameObjectParts;
						}

						scriptCollider = null;
						flagAttachCollider = false;
						if(null != gameObjectParts)
						{
							switch(informationSSAE.TableParts[i].Data.ShapeCollision)
							{
								case Library_SpriteStudio6.Data.Parts.Animation.KindCollision.NON:
									break;

								case Library_SpriteStudio6.Data.Parts.Animation.KindCollision.SQUARE:
									/* Attach Script */
									scriptCollider = gameObjectParts.AddComponent<Script_SpriteStudio6_Collider>();
									tableControlParts[i].InstanceScriptCollider = scriptCollider;
									if(null != scriptCollider)
									{
										scriptCollider.InstanceRoot = scriptRoot;
										scriptCollider.IDParts = i;

										BoxCollider collider = gameObjectParts.AddComponent<BoxCollider>();
										if(null != collider)
										{
											collider.enabled = true;
											collider.size = new Vector3(1.0f, 1.0f, setting.Collider.SizeZ);
											collider.center = Vector2.zero;
											collider.isTrigger = setting.Collider.FlagIsTrigger;

											scriptCollider.InstanceColliderBox = collider;

											flagAttachCollider = true;
										}
									}
									break;

								case Library_SpriteStudio6.Data.Parts.Animation.KindCollision.AABB:
									/* MEMO: Not Supported */
									break;

								case Library_SpriteStudio6.Data.Parts.Animation.KindCollision.CIRCLE:
									scriptCollider = gameObjectParts.AddComponent<Script_SpriteStudio6_Collider>();
									tableControlParts[i].InstanceScriptCollider = scriptCollider;
									if(null != scriptCollider)
									{
										scriptCollider.InstanceRoot = scriptRoot;
										scriptCollider.IDParts = i;

										CapsuleCollider collider = gameObjectParts.AddComponent<CapsuleCollider>();
										if(null != collider)
										{
											collider.enabled = true;
											collider.radius = 1.0f;
											collider.height = setting.Collider.SizeZ;
											collider.direction = 2;
											collider.isTrigger = setting.Collider.FlagIsTrigger;

											scriptCollider.InstanceColliderCapsule = collider;

											flagAttachCollider = true;
										}
									}
									break;

								case Library_SpriteStudio6.Data.Parts.Animation.KindCollision.CIRCLE_SCALEMINIMUM:
									/* MEMO: Not Supported */
									break;

								case Library_SpriteStudio6.Data.Parts.Animation.KindCollision.CIRCLE_SCALEMAXIMUM:
									/* MEMO: Not Supported */
									break;
							}
							if(true == flagAttachCollider)
							{
								if(true == setting.Collider.FlagAttachRigidBody)
								{
									/* Attach Rigid-Body */
									Rigidbody rigidbody = gameObjectParts.AddComponent<Rigidbody>();
									rigidbody.isKinematic = false;
									rigidbody.useGravity = false;
								}
							}
							else
							{
								if(null != scriptCollider)
								{
									/* Remove Script */
									UnityEngine.Object.Destroy(scriptCollider);
								}
							}
						}
					}

					/* Datas Set */
					scriptRoot.DataCellMap = informationSSPJ.DataCellMapSS6PU.TableData[0];
					scriptRoot.DataAnimation = informationSSAE.DataAnimationSS6PU.TableData[0];
					scriptRoot.TableMaterial = informationSSPJ.TableMaterialAnimationSS6PU;
					scriptRoot.LimitTrack = limitTrack;
					scriptRoot.TableControlParts = tableControlParts;
					tableControlParts = null;

					scriptRoot.FlagHideForce = flagHideForce;
					scriptRoot.FlagPlanarization = flagPlanarization;
					scriptRoot.OrderInLayer = orderInLayer;

					int countLimitTrack = scriptRoot.LimitGetTrack();
					scriptRoot.TableInformationPlay = new Script_SpriteStudio6_Root.InformationPlay[countLimitTrack];
					if(null == scriptRoot.TableInformationPlay)
					{
						LogError(messageLogPrefix, "Not Enough Memory (Play-Information)", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						goto AssetCreatePrefab_ErrorEnd;
					}
					for(int i=0; i<countLimitTrack; i++)
					{
						scriptRoot.TableInformationPlay[i].CleanUp();
					}
					countLimitTrack = (countLimitTrack > informationPlayRoot.Length) ? informationPlayRoot.Length : countLimitTrack;
					bool flagClearAnimation;
					for(int i=0; i<countLimitTrack; i++)
					{
						scriptRoot.TableInformationPlay[i].FlagSetInitial = informationPlayRoot[i].FlagSetInitial;
						scriptRoot.TableInformationPlay[i].FlagStopInitial = informationPlayRoot[i].FlagStopInitial;

						flagClearAnimation = false;
						if(false == string.IsNullOrEmpty(nameAnimation[i]))
						{
							indexAnimation = scriptRoot.DataAnimation.IndexGetAnimation(nameAnimation[i]);
							if(0 > indexAnimation)
							{
								flagClearAnimation = true;
							}
							informationPlayRoot[i].NameAnimation = string.Copy(nameAnimation[i]);
						}
						else
						{
							flagClearAnimation = true;
						}
						if(true == flagClearAnimation)
						{
							informationPlayRoot[i].NameAnimation = string.Copy(scriptRoot.DataAnimation.TableAnimation[0].Name);
							informationPlayRoot[i].LabelStart = "";
							informationPlayRoot[i].FrameOffsetStart = 0;
							informationPlayRoot[i].LabelEnd = "";
							informationPlayRoot[i].FrameOffsetEnd = 0;
						}

						scriptRoot.TableInformationPlay[i].NameAnimation = informationPlayRoot[i].NameAnimation;
						scriptRoot.TableInformationPlay[i].FlagPingPong = informationPlayRoot[i].FlagPingPong;
						scriptRoot.TableInformationPlay[i].LabelStart = (false == string.IsNullOrEmpty(informationPlayRoot[i].LabelStart)) ? informationPlayRoot[i].LabelStart : "";
						scriptRoot.TableInformationPlay[i].FrameOffsetStart = informationPlayRoot[i].FrameOffsetStart;
						scriptRoot.TableInformationPlay[i].LabelEnd = (false == string.IsNullOrEmpty(informationPlayRoot[i].LabelEnd)) ? informationPlayRoot[i].LabelEnd : "";
						scriptRoot.TableInformationPlay[i].FrameOffsetEnd = informationPlayRoot[i].FrameOffsetEnd;
						scriptRoot.TableInformationPlay[i].Frame = informationPlayRoot[i].Frame;
						scriptRoot.TableInformationPlay[i].TimesPlay = informationPlayRoot[i].TimesPlay;
						scriptRoot.TableInformationPlay[i].RateTime = informationPlayRoot[i].RateTime;
					}

					if(null != meshRenderer)
					{
						if(true == setting.Basic.FlagTakeOverLightRenderer)
						{
							if(true == flagExistMeshRendererOriginal)
							{
								meshRenderer.lightProbeUsage = lightProbeUsage;
								meshRenderer.reflectionProbeUsage = reflectionProbeUsage;
								meshRenderer.probeAnchor = probeAnchor;
								meshRenderer.shadowCastingMode = shadowCastingMode;
								meshRenderer.receiveShadows = receiveShadows;
								meshRenderer.motionVectorGenerationMode = motionVectorGenerationMode;
								meshRenderer.allowOcclusionWhenDynamic = allowOcclusionWhenDynamic;
								StaticEditorFlags staticFlag = GameObjectUtility.GetStaticEditorFlags(gameObjectRoot);
								GameObjectUtility.SetStaticEditorFlags(	gameObjectRoot,
																		(true == lightmapStatic) ? (staticFlag | StaticEditorFlags.LightmapStatic) : (staticFlag & ~StaticEditorFlags.LightmapStatic)
																	);
								meshRenderer.lightmapIndex = lightmapIndex;
								meshRenderer.lightmapScaleOffset = lightmapScaleOffset;
								meshRenderer.lightProbeProxyVolumeOverride = lightProbeProxyVolumeOverride;
								meshRenderer.realtimeLightmapIndex = realtimeLightmapIndex;
								meshRenderer.realtimeLightmapScaleOffset = realtimeLightmapScaleOffset;
							}
						}
						else
						{
							if(true == setting.Basic.FlagDisableInitialLightRenderer)
							{
								meshRenderer.lightProbeUsage = lightProbeUsage;
								meshRenderer.reflectionProbeUsage = reflectionProbeUsage;
								meshRenderer.probeAnchor = probeAnchor;
								meshRenderer.shadowCastingMode = shadowCastingMode;
								meshRenderer.receiveShadows = receiveShadows;
								meshRenderer.motionVectorGenerationMode = motionVectorGenerationMode;
								meshRenderer.allowOcclusionWhenDynamic = allowOcclusionWhenDynamic;
								StaticEditorFlags staticFlag = GameObjectUtility.GetStaticEditorFlags(gameObjectRoot);
								GameObjectUtility.SetStaticEditorFlags(gameObjectRoot, (staticFlag & ~StaticEditorFlags.LightmapStatic));
							}
						}
					}

					gameObjectRoot.SetActive(true);

					/* Fixing Prefab */
#if UNITY_2018_4_OR_NEWER || UNITY_2019_1_OR_NEWER
					informationSSAE.PrefabAnimationSS6PU.TableData[0] = PrefabUtility.SaveAsPrefabAsset(gameObjectRoot, informationSSAE.PrefabAnimationSS6PU.TableName[0]);
#else
					informationSSAE.PrefabAnimationSS6PU.TableData[0] = PrefabUtility.ReplacePrefab(	gameObjectRoot,
																										informationSSAE.PrefabAnimationSS6PU.TableData[0],
																										LibraryEditor_SpriteStudio6.Import.OptionPrefabReplace
																									);
#endif
					AssetDatabase.SaveAssets();

					/* Destroy Temporary */
					UnityEngine.Object.DestroyImmediate(gameObjectRoot);
					gameObjectRoot = null;

					/* Create Control-Object */
					if(true == setting.Basic.FlagCreateControlGameObject)
					{
						/* Control-Object Create */
						GameObject gameObjectControl = Library_SpriteStudio6.Utility.Asset.GameObjectCreate(informationSSAE.NameGameObjectAnimationControlSS6PU, false, null);
						if(null == gameObjectControl)
						{
							LogError(messageLogPrefix, "Failure to get Temporary-GameObject (Control)", informationSSAE.FileNameGetFullPath(), informationSSPJ);
							goto AssetCreatePrefab_ErrorEnd;
						}

						/* Attach Script & Link Prefab */
						Script_SpriteStudio6_ControlPrefab scriptControlPrefab = gameObjectControl.AddComponent<Script_SpriteStudio6_ControlPrefab>();
						scriptControlPrefab.PrefabAnimation = informationSSAE.PrefabAnimationSS6PU.TableData[0];

						/* Create Prefab */
						gameObjectControl.SetActive(true);

#if UNITY_2018_4_OR_NEWER || UNITY_2019_1_OR_NEWER
						UnityEngine.Object prefabControl = informationSSAE.PrefabControlAnimationSS6PU.TableData[0];
						if(null == prefabControl)
						{
							/* MEMO: Process nothing, now. */
						}
						else
						{
							informationSSAE.PrefabControlAnimationSS6PU.TableName[0] = AssetDatabase.GetAssetPath(prefabControl);
						}
						informationSSAE.PrefabControlAnimationSS6PU.TableData[0] = PrefabUtility.SaveAsPrefabAsset(gameObjectControl, informationSSAE.PrefabControlAnimationSS6PU.TableName[0]);
#else
						UnityEngine.Object prefabControl = informationSSAE.PrefabControlAnimationSS6PU.TableData[0];
						if(null == prefabControl)
						{
							prefabControl = PrefabUtility.CreateEmptyPrefab(informationSSAE.PrefabControlAnimationSS6PU.TableName[0]);
						}
						PrefabUtility.ReplacePrefab(	gameObjectControl,
														prefabControl,
														LibraryEditor_SpriteStudio6.Import.OptionPrefabReplace
													);
#endif
						AssetDatabase.SaveAssets();

						/* Destroy Temporary */
						UnityEngine.Object.DestroyImmediate(gameObjectControl);
						gameObjectControl = null;
					}

					return(true);

				AssetCreatePrefab_ErrorEnd:;
					if(null != gameObjectRoot)
					{
						UnityEngine.Object.DestroyImmediate(gameObjectRoot);
					}
					return(false);
				}
#endif

				public static bool ConvertData(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
												LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
												LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE
											)
				{
					const string messageLogPrefix = "Convert (Data-Animation)";

					int countParts = informationSSAE.TableParts.Length;
					int countAnimation = informationSSAE.TableAnimation.Length;

					/* Set Additional-datas (Pick up underControl Objects, Set Mesh-Bind data and etc.) */
					/* MEMO: Since order of SSAE conversion is determined in "informationSSPJ.QueueGetConvertSSAE", */
					/*        prefabs referred to in this animation has already been fixed.                         */
					LibraryEditor_SpriteStudio6.Import.SSAE.Information.Parts informationParts = null;
					for(int i=0; i<countParts; i++)
					{
						int indexUnderControl;
						informationParts = informationSSAE.TableParts[i];
						switch(informationParts.Data.Feature)
						{
							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.ROOT:
							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NULL:
//							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE2:
//							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE4:
							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL:
								break;

							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE:
								/* MEMO: Set "Instance" prefab under control. */
#if !_FOR_SS6CONVERTER_
								informationParts.Data.PrefabUnderControl = null;
								informationParts.Data.NameAnimationUnderControl = "";
								if(false == string.IsNullOrEmpty(informationParts.NameUnderControl))
								{
									indexUnderControl = informationSSPJ.IndexGetAnimation(informationParts.NameUnderControl);
									if(0 <= indexUnderControl)
									{
										informationParts.Data.PrefabUnderControl = informationSSPJ.TableInformationSSAE[indexUnderControl].PrefabAnimationSS6PU.TableData[0];
										informationParts.Data.NameAnimationUnderControl = informationParts.NameAnimationUnderControl;
									}
								}
#else
								informationParts.Data.IndexAnimationPackUnderControl = -1;
								informationParts.Data.IndexEffectUnderControl = -1;
								informationParts.Data.NameAnimationUnderControl = "";
								if(false == string.IsNullOrEmpty(informationParts.NameUnderControl))
								{
									indexUnderControl = informationSSPJ.IndexGetAnimation(informationParts.NameUnderControl);
									if(0 <= indexUnderControl)
									{
										informationParts.Data.IndexAnimationPackUnderControl = indexUnderControl;
//										informationParts.Data.IndexEffectUnderControl =
										informationParts.Data.NameAnimationUnderControl = informationParts.NameAnimationUnderControl;;
									}
								}
#endif
								break;

							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT:
								/* MEMO: Set "Effect" prefab under control. */
#if !_FOR_SS6CONVERTER_
								informationParts.Data.PrefabUnderControl = null;
								informationParts.Data.NameAnimationUnderControl = "";
								if(false == string.IsNullOrEmpty(informationParts.NameUnderControl))
								{
									indexUnderControl = informationSSPJ.IndexGetEffect(informationParts.NameUnderControl);
									if(0 <= indexUnderControl)
									{
										informationParts.Data.PrefabUnderControl = informationSSPJ.TableInformationSSEE[indexUnderControl].PrefabEffectSS6PU.TableData[0];
									}
								}
#else
								informationParts.Data.IndexAnimationPackUnderControl = -1;
								informationParts.Data.IndexEffectUnderControl = -1;
								informationParts.Data.NameAnimationUnderControl = "";
								if(false == string.IsNullOrEmpty(informationParts.NameUnderControl))
								{
									indexUnderControl = informationSSPJ.IndexGetAnimation(informationParts.NameUnderControl);
									if(0 <= indexUnderControl)
									{
//										informationParts.Data.IndexAnimationPackUnderControl =
										informationParts.Data.IndexEffectUnderControl = indexUnderControl;
//										informationParts.Data.NameAnimationUnderControl = informationParts.NameAnimationUnderControl;;
									}
								}
#endif
								break;

//							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE2:
//							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE4:
							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK:
								break;

							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.JOINT:
							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONE:
							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MOVENODE:
							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.CONSTRAINT:
							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONEPOINT:
								break;

							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH:
								/* MEMO: Set Mesh-Bind data. */
								{
									informationParts.Data.Mesh.CleanUp();

									int indexCellMap = informationParts.IndexCellMapMeshBind;
									int indexCell = informationParts.IndexCellMeshBind;
									LibraryEditor_SpriteStudio6.Import.SSCE.Information cellMap = null;
									if((0 <= indexCellMap) && (0 <= indexCell))
									{
										/* Set Vertex-index table */
										if(informationSSPJ.TableInformationSSCE.Length > indexCellMap)
										{
											/* MEMO: "cellMap.TableCell" is purged at "SSCE.ModeSS6PU.ConvertCellMap". */
											cellMap = informationSSPJ.TableInformationSSCE[indexCellMap];
											if(cellMap.Data.TableCell.Length > indexCell)
											{
												if(true == cellMap.Data.TableCell[indexCell].IsMesh)
												{
													informationParts.Data.Mesh.TableIndexVertex = cellMap.Data.TableCell[indexCell].Mesh.TableIndexVertex;
												}
											}
										}

										/* Set UV Rate */
										int countVertexMesh = 0;
										if(null != informationParts.Data.Mesh.TableIndexVertex)
										{
											countVertexMesh = cellMap.Data.TableCell[indexCell].Mesh.TableCoordinate.Length;
											informationParts.Data.Mesh.TableRateUV = new Vector2[countVertexMesh];
											if(null == informationParts.Data.Mesh.TableRateUV)
											{	/* Error */
												informationParts.Data.Mesh.TableRateUV = null;	/* new Vector2[0] */
											}
											else
											{
												Rect rectangleCell = cellMap.Data.TableCell[indexCell].Rectangle;
												float sizeInverseX = 1.0f / rectangleCell.width;
												float sizeInverseY = 1.0f / rectangleCell.height;
												Vector2 coordinate;
												for(int j=0; j<countVertexMesh; j++)
												{
													coordinate = cellMap.Data.TableCell[indexCell].Mesh.TableCoordinate[j];

													coordinate.x *= sizeInverseX;
													coordinate.y *= sizeInverseY;

													informationParts.Data.Mesh.TableRateUV[j] = coordinate;
												}
											}
										}
										informationParts.Data.Mesh.CountVertex = countVertexMesh;

										/* Set(Overwrite) Bone-Parts' ID */
										if(0 >= countVertexMesh)
										{	/* has no "Cell-Mesh" */
											informationParts.Data.Mesh.TableVertex = null;	/* new Library_SpriteStudio6.Data.Parts.Animation.BindMesh.Vertex[0] */
										}
										else
										{
											int indexBindMesh = informationSSAE.CatalogParts.ListIDPartsMesh.IndexOf(i);
											if(0 > indexBindMesh)
											{	/* Error (Parts-ID not found) */
												informationParts.Data.Mesh.TableVertex = null;	/* new Library_SpriteStudio6.Data.Parts.Animation.BindMesh.Vertex[0] */
											}
											else
											{
												if(informationSSAE.ListBindMesh.Count <= indexBindMesh)
												{	/* Mesh is not-bind / Error (Mesh-Bind not found) */
													informationParts.Data.Mesh.TableVertex = null;	/* new Library_SpriteStudio6.Data.Parts.Animation.BindMesh.Vertex[0] */
												}
												else
												{
													informationParts.Data.Mesh.TableVertex = informationSSAE.ListBindMesh[indexBindMesh].ListBindVertex.ToArray();

													/* Convert Bone-ID to Bone-PartsID */
													int countBindVertex = informationParts.Data.Mesh.TableVertex.Length;
#if WARN_MESHVERTEX_COUNT
													if((0 < countBindVertex) && (countBindVertex != countVertexMesh))
													{
														LogWarning(messageLogPrefix, "Mesh-Bind's vertices count is different from Cell's vertices count. Parts[" + informationSSAE.TableParts[i].Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
													}
#endif

													int countBone;
													string namePartsBone;
													int idPartsBone;
													for(int j=0; j<countBindVertex; j++)
													{
														countBone = informationParts.Data.Mesh.TableVertex[j].TableBone.Length;
														if(0 < countBone)
														{
															for(int k=0; k<countBone; k++)
															{
																idPartsBone = informationParts.Data.Mesh.TableVertex[j].TableBone[k].Index;
																namePartsBone = informationSSAE.ListBone[idPartsBone];
																idPartsBone = informationSSAE.IndexGetParts(namePartsBone);
#if BONEINDEX_CONVERT_PARTSID
																if(0 <= idPartsBone)
																{
																	informationParts.Data.Mesh.TableVertex[j].TableBone[k].Index = idPartsBone;
																}
#else
																/* MEMO: Before SS 6.2 (SSAE 2.00.03) this correcting is not necessary.                           */
																/*       After SS 6.3 (SSAE 2.00.04) the CatalogParts.ListIDPartsBone and bone-list(ListBone) are */
																/*        in different order, conversion is required.                                             */
																if(0 <= idPartsBone)
																{
																	int indexCatalogBone = informationSSAE.CatalogParts.ListIDPartsBone.IndexOf(idPartsBone);
																	if(0 <= indexCatalogBone)
																	{
																		informationParts.Data.Mesh.TableVertex[j].TableBone[k].Index = indexCatalogBone;
																	}
																}
#endif
															}
														}
													}
												}
											}
										}

#if CHANGE_DEFORM_DECODING
										/* Set Vertex Count for Deform */
										if(false == informationParts.FlagDeform)
										{	/* not use "Deform" */
											informationParts.Data.Mesh.CountVertexDeform = 0;
										}
										else
										{
											informationParts.Data.Mesh.CountVertexDeform = countVertexMesh;
										}
#else
										/* Set Vertex Count for Deform */
										int countVertexAttribute = informationParts.CountVertexDeform;
										if(0 >= countVertexAttribute)
										{	/* not use "Deform" */
											informationParts.Data.Mesh.CountVertexDeform = 0;
										}
										else
										{
#if WARN_MESHVERTEX_COUNT
											if(countVertexMesh != countVertexAttribute)
											{
												LogWarning(messageLogPrefix, "Mesh-Bind's vertices count is different from Cell's vertices count. Parts[" + informationSSAE.TableParts[i].Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
											}
#endif
											informationParts.Data.Mesh.CountVertexDeform = countVertexMesh;
										}
#endif
									}
								}
								break;

							default:
								break;
						}
					}

					/* Convert Animations */
					LibraryEditor_SpriteStudio6.Import.SSAE.Information.Animation informationAnimation = null;
					LibraryEditor_SpriteStudio6.Import.SSAE.Information.Animation.Parts informationAnimationParts = null;
					Library_SpriteStudio6.Data.Animation dataAnimation = null;
					int countFrame;
					for(int i=0; i<countAnimation; i++)
					{
						informationAnimation = informationSSAE.TableAnimation[i];
						dataAnimation = informationAnimation.Data;
						dataAnimation.TableParts = new Library_SpriteStudio6.Data.Animation.Parts[countParts];
						if(null == dataAnimation.TableParts)
						{
							LogError(messageLogPrefix, "Not Enough Memory (Data Animation Parts-Table) Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
							goto ConvertData_ErrorEnd;
						}
						countFrame = informationAnimation.Data.CountFrame;

						if(false == Library_SpriteStudio6.Data.Animation.PackAttribute.DictionaryBootUp(i, -1, null))
						{
							LogError(messageLogPrefix, "Failure Open PackAttribute's dictionary (for Animation)  Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
							goto ConvertData_ErrorEnd;
						}

						for(int j=0; j<countParts; j++)
						{
							informationParts = informationSSAE.TableParts[j];
							informationAnimationParts = informationAnimation.TableParts[j];
							dataAnimation.TableParts[j].StatusParts = informationAnimationParts.StatusParts;

							if(false == Library_SpriteStudio6.Data.Animation.PackAttribute.DictionaryBootUp(i, j, null))
							{
								LogError(messageLogPrefix, "Failure Open PackAttribute's dictionary (for Animation-Parts) Parts-Name[" + informationParts.Data.Name + "] Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}

							dataAnimation.TableParts[j].Status = PackAttribute.FactoryStatus(setting.PackAttributeAnimation.Status);
							if(false == dataAnimation.TableParts[j].Status.Function.Pack(	dataAnimation.TableParts[j].Status,
																							Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeStatus,
																							countFrame,
																							informationAnimationParts.StatusParts,
																							informationAnimationParts.TableOrderDraw,
																							informationAnimationParts.TableOrderPreDraw,
																							informationAnimationParts.Hide,
																							informationAnimationParts.FlipX,
																							informationAnimationParts.FlipY,
																							informationAnimationParts.TextureFlipX,
																							informationAnimationParts.TextureFlipY
																					)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"Status\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}

							dataAnimation.TableParts[j].Position = PackAttribute.FactoryVector3(setting.PackAttributeAnimation.Position);
							if(false == dataAnimation.TableParts[j].Position.Function.Pack(	dataAnimation.TableParts[j].Position,
																							Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributePosition,
																							countFrame,
																							informationAnimationParts.StatusParts,
																							informationAnimationParts.TableOrderDraw,
																							informationAnimationParts.TableOrderPreDraw,
																							informationAnimationParts.PositionX,
																							informationAnimationParts.PositionY,
																							informationAnimationParts.PositionZ
																						)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"Position\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}
							dataAnimation.TableParts[j].Rotation = PackAttribute.FactoryVector3(setting.PackAttributeAnimation.Rotation);
							if(false == dataAnimation.TableParts[j].Rotation.Function.Pack(	dataAnimation.TableParts[j].Rotation,
																							Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeRotation,
																							countFrame,
																							informationAnimationParts.StatusParts,
																							informationAnimationParts.TableOrderDraw,
																							informationAnimationParts.TableOrderPreDraw,
																							informationAnimationParts.RotationX,
																							informationAnimationParts.RotationY,
																							informationAnimationParts.RotationZ
																						)
								)
							{
								goto ConvertData_ErrorEnd;
							}
							dataAnimation.TableParts[j].Scaling = PackAttribute.FactoryVector2(setting.PackAttributeAnimation.Scaling);
							if(false == dataAnimation.TableParts[j].Scaling.Function.Pack(	dataAnimation.TableParts[j].Scaling,
																							Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeScaling,
																							countFrame,
																							informationAnimationParts.StatusParts,
																							informationAnimationParts.TableOrderDraw,
																							informationAnimationParts.TableOrderPreDraw,
																							informationAnimationParts.ScalingX,
																							informationAnimationParts.ScalingY
																						)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"Rotation\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}

							dataAnimation.TableParts[j].ScalingLocal = PackAttribute.FactoryVector2(setting.PackAttributeAnimation.ScalingLocal);
							if(false == dataAnimation.TableParts[j].ScalingLocal.Function.Pack(	dataAnimation.TableParts[j].ScalingLocal,
																								Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeScalingLocal,
																								countFrame,
																								informationAnimationParts.StatusParts,
																								informationAnimationParts.TableOrderDraw,
																								informationAnimationParts.TableOrderPreDraw,
																								informationAnimationParts.ScalingXLocal,
																								informationAnimationParts.ScalingYLocal
																							)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"Rotation\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}

							/* MEMO: "RateOpacity" and "RateOpacityLocal" never work in parallel. (always "RateOpacityLocal" takes precedence)                 */
							/*       Also, for "Mask"parts, "RateOpacity" and "RateOpacityLocal" does not work. Instead, "PowerMask" works.                    */
							/*       However, "RateOpacity" is always valid as an inheritance parameter for child-parts.                                       */
							/*       For above reasons, "RateOpacity" is used as a common storage for "RateOpacity", "RateOpacityLocal" and "PowerMask".       */
							/*                                                                                                                                 */
							/*       The point to note is that the value-range of "RateOpacity" is "0.0 to 1.0", and the equivalent "PowerMask" is "255 to 0". */
							/*       This conversion is processed by judging processing-attribute's name in ("StandardUncompress"'s) "Funtion.Pack" function.  */
							switch(informationParts.Data.Feature)
							{
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.ROOT:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NULL:
//								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE2:
//								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE4:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH:
									if(0 >= informationAnimationParts.RateOpacityLocal.CountGetKey())
									{	/* RateOpacity */
										dataAnimation.TableParts[j].RateOpacity = PackAttribute.FactoryFloat(setting.PackAttributeAnimation.RateOpacity);
										if(false == dataAnimation.TableParts[j].RateOpacity.Function.Pack(	dataAnimation.TableParts[j].RateOpacity,
																											Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeRateOpacity,
																											countFrame,
																											informationAnimationParts.StatusParts,
																											informationAnimationParts.TableOrderDraw,
																											informationAnimationParts.TableOrderPreDraw,
																											informationAnimationParts.RateOpacity
																										)
											)
										{
											LogError(messageLogPrefix, "Failure Packing Attribute \"RateOpacity\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
											goto ConvertData_ErrorEnd;
										}
									}
									else
									{	/* RateOpacity-Local */
										dataAnimation.TableParts[j].RateOpacity = PackAttribute.FactoryFloat(setting.PackAttributeAnimation.RateOpacity);
										if(false == dataAnimation.TableParts[j].RateOpacity.Function.Pack(	dataAnimation.TableParts[j].RateOpacity,
																											Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeRateOpacityLocal,
																											countFrame,
																											informationAnimationParts.StatusParts,
																											informationAnimationParts.TableOrderDraw,
																											informationAnimationParts.TableOrderPreDraw,
																											informationAnimationParts.RateOpacityLocal
																										)
											)
										{
											LogError(messageLogPrefix, "Failure Packing Attribute \"RateOpacityLocal\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
											goto ConvertData_ErrorEnd;
										}
									}
									break;

//								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE2:
//								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE4:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK:
									dataAnimation.TableParts[j].RateOpacity = PackAttribute.FactoryFloat(setting.PackAttributeAnimation.RateOpacity);
									if(false == dataAnimation.TableParts[j].RateOpacity.Function.Pack(	dataAnimation.TableParts[j].RateOpacity,
																										Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributePowerMask,
																										countFrame,
																										informationAnimationParts.StatusParts,
																										informationAnimationParts.TableOrderDraw,
																										informationAnimationParts.TableOrderPreDraw,
																										informationAnimationParts.PowerMask
																									)
										)
									{
										LogError(messageLogPrefix, "Failure Packing Attribute \"PowerMask\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
										goto ConvertData_ErrorEnd;
									}
									break;

								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.JOINT:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONE:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MOVENODE:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.CONSTRAINT:
								case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONEPOINT:
									break;

								default:
									break;
							}

							dataAnimation.TableParts[j].Priority = PackAttribute.FactoryInt(setting.PackAttributeAnimation.Priority);
							if(false == dataAnimation.TableParts[j].Priority.Function.Pack(	dataAnimation.TableParts[j].Priority,
																							Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributePriority,
																							countFrame,
																							informationAnimationParts.StatusParts,
																							informationAnimationParts.TableOrderDraw,
																							informationAnimationParts.TableOrderPreDraw,
																							informationAnimationParts.Priority
																						)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"Priority\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}

							dataAnimation.TableParts[j].PartsColor = PackAttribute.FactoryPartsColor(setting.PackAttributeAnimation.PartsColor);
							if(false == dataAnimation.TableParts[j].PartsColor.Function.Pack(	dataAnimation.TableParts[j].PartsColor,
																								Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributePartsColor,
																								countFrame,
																								informationAnimationParts.StatusParts,
																								informationAnimationParts.TableOrderDraw,
																								informationAnimationParts.TableOrderPreDraw,
																								informationAnimationParts.PartsColor
																							)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"PartsColor\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}

							dataAnimation.TableParts[j].PositionAnchor = PackAttribute.FactoryVector2(setting.PackAttributeAnimation.PositionAnchor);
							if(false == dataAnimation.TableParts[j].PositionAnchor.Function.Pack(	dataAnimation.TableParts[j].PositionAnchor,
																									Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributePositionAnchor,
																									countFrame,
																									informationAnimationParts.StatusParts,
																									informationAnimationParts.TableOrderDraw,
																									informationAnimationParts.TableOrderPreDraw,
																									informationAnimationParts.AnchorPositionX,
																									informationAnimationParts.AnchorPositionY
																								)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"PositionAnchor\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}

							dataAnimation.TableParts[j].RadiusCollision = PackAttribute.FactoryFloat(setting.PackAttributeAnimation.RadiusCollision);
							if(false == dataAnimation.TableParts[j].RadiusCollision.Function.Pack(	dataAnimation.TableParts[j].RadiusCollision,
																									Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeRadiusCollision,
																									countFrame,
																									informationAnimationParts.StatusParts,
																									informationAnimationParts.TableOrderDraw,
																									informationAnimationParts.TableOrderPreDraw,
																									informationAnimationParts.RadiusCollision
																								)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"RadiusCollision\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}

							dataAnimation.TableParts[j].UserData = PackAttribute.FactoryUserData(setting.PackAttributeAnimation.UserData);
							if(false == dataAnimation.TableParts[j].UserData.Function.Pack(	dataAnimation.TableParts[j].UserData,
																							Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeUserData,
																							countFrame,
																							informationAnimationParts.StatusParts,
																							informationAnimationParts.TableOrderDraw,
																							informationAnimationParts.TableOrderPreDraw,
																							informationAnimationParts.UserData
																						)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"UserData\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}
							dataAnimation.TableParts[j].Instance = PackAttribute.FactoryInstance(setting.PackAttributeAnimation.Instance);
							if(false == dataAnimation.TableParts[j].Instance.Function.Pack(	dataAnimation.TableParts[j].Instance,
																							Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeInstance,
																							countFrame,
																							informationAnimationParts.StatusParts,
																							informationAnimationParts.TableOrderDraw,
																							informationAnimationParts.TableOrderPreDraw,
																							informationAnimationParts.Instance
																						)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"Instance\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}
							dataAnimation.TableParts[j].Effect = PackAttribute.FactoryEffect(setting.PackAttributeAnimation.Effect);
							if(false == dataAnimation.TableParts[j].Effect.Function.Pack(	dataAnimation.TableParts[j].Effect,
																							Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeEffect,
																							countFrame,
																							informationAnimationParts.StatusParts,
																							informationAnimationParts.TableOrderDraw,
																							informationAnimationParts.TableOrderPreDraw,
																							informationAnimationParts.Effect
																					)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"Effect\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}
							dataAnimation.TableParts[j].Deform = PackAttribute.FactoryDeform(setting.PackAttributeAnimation.Deform);
							if(false == dataAnimation.TableParts[j].Deform.Function.Pack(	dataAnimation.TableParts[j].Deform,
																							Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeDeform,
																							countFrame,
																							informationAnimationParts.StatusParts,
																							informationAnimationParts.TableOrderDraw,
																							informationAnimationParts.TableOrderPreDraw,
																							informationAnimationParts.Deform
																					)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"Deform\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}

							/* MEMO: Just create, even if do not use.                            */
							/*       (Because pack format at instantiate becomes inappropriate.) */
							dataAnimation.TableParts[j].Cell = PackAttribute.FactoryCell(setting.PackAttributeAnimation.Cell);
							dataAnimation.TableParts[j].VertexCorrection = PackAttribute.FactoryVertexCorrection(setting.PackAttributeAnimation.VertexCorrection);
							dataAnimation.TableParts[j].OffsetPivot = PackAttribute.FactoryVector2(setting.PackAttributeAnimation.OffsetPivot);
							dataAnimation.TableParts[j].PositionTexture = PackAttribute.FactoryVector2(setting.PackAttributeAnimation.PositionTexture);
							dataAnimation.TableParts[j].ScalingTexture = PackAttribute.FactoryVector2(setting.PackAttributeAnimation.ScalingTexture);
							dataAnimation.TableParts[j].RotationTexture = PackAttribute.FactoryFloat(setting.PackAttributeAnimation.RotationTexture);

							dataAnimation.TableParts[j].SizeForce = PackAttribute.FactoryVector2(setting.PackAttributeAnimation.SizeForce);
							if(false == dataAnimation.TableParts[j].SizeForce.Function.Pack(	dataAnimation.TableParts[j].SizeForce,
																								Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeSizeForce,
																								countFrame,
																								informationAnimationParts.StatusParts,
																								informationAnimationParts.TableOrderDraw,
																								informationAnimationParts.TableOrderPreDraw,
																								informationAnimationParts.SizeForceX,
																								informationAnimationParts.SizeForceY
																							)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"SizeForce\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}

							if(false ==  dataAnimation.TableParts[j].Cell.Function.Pack(	dataAnimation.TableParts[j].Cell,
																							Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeCell,
																							countFrame,
																							informationAnimationParts.StatusParts,
																							informationAnimationParts.TableOrderDraw,
																							informationAnimationParts.TableOrderPreDraw,
																							informationAnimationParts.Cell
																						)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"Cell\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}

							if(false == dataAnimation.TableParts[j].VertexCorrection.Function.Pack(	dataAnimation.TableParts[j].VertexCorrection,
																									Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeVertexCorrection,
																									countFrame,
																									informationAnimationParts.StatusParts,
																									informationAnimationParts.TableOrderDraw,
																									informationAnimationParts.TableOrderPreDraw,
																									informationAnimationParts.VertexCorrection
																								)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"VertexCorrection\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}

							if(false == dataAnimation.TableParts[j].OffsetPivot.Function.Pack(	dataAnimation.TableParts[j].OffsetPivot,
																								Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeOffsetPivot,
																								countFrame,
																								informationAnimationParts.StatusParts,
																								informationAnimationParts.TableOrderDraw,
																								informationAnimationParts.TableOrderPreDraw,
																								informationAnimationParts.PivotOffsetX,
																								informationAnimationParts.PivotOffsetY
																							)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"OffsetPivot\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}

							if(false == dataAnimation.TableParts[j].PositionTexture.Function.Pack(	dataAnimation.TableParts[j].PositionTexture,
																									Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributePositionTexture,
																									countFrame,
																									informationAnimationParts.StatusParts,
																									informationAnimationParts.TableOrderDraw,
																									informationAnimationParts.TableOrderPreDraw,
																									informationAnimationParts.TexturePositionX,
																									informationAnimationParts.TexturePositionY
																								)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"PositionTexture\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}

							if(false == dataAnimation.TableParts[j].ScalingTexture.Function.Pack(	dataAnimation.TableParts[j].ScalingTexture,
																									Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeScalingTexture,
																									countFrame,
																									informationAnimationParts.StatusParts,
																									informationAnimationParts.TableOrderDraw,
																									informationAnimationParts.TableOrderPreDraw,
																									informationAnimationParts.TextureScalingX,
																									informationAnimationParts.TextureScalingY
																								)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"ScalingTexture\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}

							if(false == dataAnimation.TableParts[j].RotationTexture.Function.Pack(	dataAnimation.TableParts[j].RotationTexture,
																									Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeRotationTexture,
																									countFrame,
																									informationAnimationParts.StatusParts,
																									informationAnimationParts.TableOrderDraw,
																									informationAnimationParts.TableOrderPreDraw,
																									informationAnimationParts.TextureRotation
																								)
								)
							{
								LogError(messageLogPrefix, "Failure Packing Attribute \"RotationTexture\" Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}

							if(false == Library_SpriteStudio6.Data.Animation.PackAttribute.DictionaryShutDown(i, j, null))
							{
								LogError(messageLogPrefix, "Failure Close PackAttribute's dictionary (for Animation-Parts) Parts-Name[" + informationParts.Data.Name + "] Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto ConvertData_ErrorEnd;
							}
						}

						if(false == Library_SpriteStudio6.Data.Animation.PackAttribute.DictionaryShutDown(i, -1, null))
						{
							LogError(messageLogPrefix, "Failure Close PackAttribute's dictionary (for Animation) Animation-Name[" + informationAnimation.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
							goto ConvertData_ErrorEnd;
						}
					}

					return(true);

				ConvertData_ErrorEnd:;
					return(false);
				}
				#endregion Functions

				/* ----------------------------------------------- Classes, Structs & Interfaces */
				#region Classes, Structs & Interfaces
				public static partial class PackAttribute
				{
					/* ----------------------------------------------- Functions */
					#region Functions
					public static Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInt FactoryInt(Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack pack)
					{
						Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInt container = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInt();
						container.TypePack = pack;
						Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionInt(container);
						return(container);
					}

					public static Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerFloat FactoryFloat(Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack pack)
					{
						Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerFloat container = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerFloat();
						container.TypePack = pack;
						Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionFloat(container);
						return(container);
					}

					public static Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2 FactoryVector2(Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack pack)
					{
						Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2 container = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2();
						container.TypePack = pack;
						Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVector2(container);
						return(container);
					}

					public static Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3 FactoryVector3(Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack pack)
					{
						Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3 container = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3();
						container.TypePack = pack;
						Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVector3(container);
						return(container);
					}

					public static Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerStatus FactoryStatus(Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack pack)
					{
						Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerStatus container = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerStatus();
						container.TypePack = pack;
						Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionStatus(container);
						return(container);
					}

					public static Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerCell FactoryCell(Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack pack)
					{
						Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerCell container = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerCell();
						container.TypePack = pack;
						Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionCell(container);
						return(container);
					}

					public static Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerPartsColor FactoryPartsColor(Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack pack)
					{
						Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerPartsColor container = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerPartsColor();
						container.TypePack = pack;
						Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionPartsColor(container);
						return(container);
					}

					public static Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVertexCorrection FactoryVertexCorrection(Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack pack)
					{
						Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVertexCorrection container = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVertexCorrection();
						container.TypePack = pack;
						Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVertexCorrection(container);
						return(container);
					}

					public static Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerUserData FactoryUserData(Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack pack)
					{
						Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerUserData container = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerUserData();
						container.TypePack = pack;
						Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionUserData(container);
						return(container);
					}

					public static Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInstance FactoryInstance(Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack pack)
					{
						Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInstance container = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInstance();
						container.TypePack = pack;
						Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionInstance(container);
						return(container);
					}

					public static Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerEffect FactoryEffect(Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack pack)
					{
						Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerEffect container = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerEffect();
						container.TypePack = pack;
						Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionEffect(container);
						return(container);
					}

					public static Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerDeform FactoryDeform(Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack pack)
					{
						Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerDeform container = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerDeform();
						container.TypePack = pack;
						Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionDeform(container);
						return(container);
					}
					#endregion Functions
				}
				#endregion Classes, Structs & Interfaces
			}

			public static partial class ModeUnityNative
			{
				/* MEMO: Originally functions that should be defined in each information class. */
				/*       However, confusion tends to occur with mode increases.                 */
				/*       ... Compromised way.                                                   */

				/* ----------------------------------------------- Functions */
				#region Functions
#if !_FOR_SS6CONVERTER_
				public static bool AssetNameDecideData(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
														LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
														LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE,
														int indexAnimation,
														string nameOutputAssetFolderBase,
														AnimationClip dataOverride
													)
				{
					if(null != dataOverride)
					{	/* Specified */
						informationSSAE.DataAnimationUnityNative.TableName[indexAnimation] = AssetDatabase.GetAssetPath(dataOverride);
						informationSSAE.DataAnimationUnityNative.TableData[indexAnimation] = dataOverride;
					}
					else
					{	/* Default */
						/* Get Animation-Name (AnimationClip Name's Suffix) */
						string nameAnimation = string.Copy(informationSSAE.TableAnimation[indexAnimation].Data.Name);
						nameAnimation = LibraryEditor_SpriteStudio6.Utility.Text.NameNormalize(nameAnimation);

						informationSSAE.DataAnimationUnityNative.TableName[indexAnimation] = setting.RuleNameAssetFolder.NameGetAssetFolder(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_ANIMATION_UNITYNATIVE, nameOutputAssetFolderBase)
																							+ setting.RuleNameAsset.NameGetAsset(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_ANIMATION_UNITYNATIVE, informationSSAE.NameFileBody, informationSSPJ.NameFileBody)
																							+ "_" + nameAnimation
																							+ LibraryEditor_SpriteStudio6.Import.NameExtentionScriptableObject;
						informationSSAE.DataAnimationUnityNative.TableData[indexAnimation] = AssetDatabase.LoadAssetAtPath<AnimationClip>(informationSSAE.DataAnimationUnityNative.TableName[indexAnimation]);
					}

					return(true);

//				AssetNameDecideData_ErroeEnd:;
//					return(false);
				}

				public static bool AssetNameDecideDataMesh(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
															LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
															LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE,
															int idParts,
															string nameOutputAssetFolderBase,
															Mesh dataOverride
														)
				{
					if(Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH != informationSSAE.TableParts[idParts].Data.Feature)
					{
						return(true);
					}

					Information.Parts informationParts = informationSSAE.TableParts[idParts];
					if(null != dataOverride)
					{	/* Specified */
						informationParts.DataMeshSkinnedUnityNative.TableName[0] = AssetDatabase.GetAssetPath(dataOverride);
						informationParts.DataMeshSkinnedUnityNative.TableData[0] = dataOverride;
					}
					else
					{	/* Default */
						/* Get Animation-Name (AnimationClip Name's Suffix) */
						string nameParts = string.Copy(informationSSAE.TableParts[idParts].Data.Name);
						nameParts = LibraryEditor_SpriteStudio6.Utility.Text.NameNormalize(nameParts);

						informationParts.DataMeshSkinnedUnityNative.TableName[0] = setting.RuleNameAssetFolder.NameGetAssetFolder(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_MESH_UNITYNATIVE, nameOutputAssetFolderBase)
																					+ setting.RuleNameAsset.NameGetAsset(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_MESH_UNITYNATIVE, informationSSAE.NameFileBody, informationSSPJ.NameFileBody)
																					+ "_" + nameParts
																					+ LibraryEditor_SpriteStudio6.Import.NameExtentionMesh;
						informationParts.DataMeshSkinnedUnityNative.TableData[0] = AssetDatabase.LoadAssetAtPath<Mesh>(informationParts.DataMeshSkinnedUnityNative.TableName[0]);
					}

					return(true);

//				AssetNameDecideData_ErroeEnd:;
//					return(false);
				}

				public static bool AssetNameDecidePrefab(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
															LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
															LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE,
															string nameOutputAssetFolderBase,
															Script_SpriteStudio6_Root prefabOverride
														)
				{
					if(null != prefabOverride)
					{	/* Specified */
						informationSSAE.NameGameObjectAnimationUnityNative = string.Copy(prefabOverride.name);

						informationSSAE.PrefabAnimationUnityNative.TableName[0] = AssetDatabase.GetAssetPath(prefabOverride);
						informationSSAE.PrefabAnimationUnityNative.TableData[0] = prefabOverride;
					}
					else
					{	/* Default */
						informationSSAE.NameGameObjectAnimationUnityNative = setting.RuleNameAsset.NameGetAsset(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_ANIMATION_UNITYNATIVE, informationSSAE.NameFileBody, informationSSPJ.NameFileBody);

						informationSSAE.PrefabAnimationUnityNative.TableName[0] = setting.RuleNameAssetFolder.NameGetAssetFolder(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_ANIMATION_UNITYNATIVE, nameOutputAssetFolderBase)
																					+ informationSSAE.NameGameObjectAnimationUnityNative
																					+ LibraryEditor_SpriteStudio6.Import.NameExtensionPrefab;
						informationSSAE.PrefabAnimationUnityNative.TableData[0] = AssetDatabase.LoadAssetAtPath<GameObject>(informationSSAE.PrefabAnimationUnityNative.TableName[0]);
					}

					/* MEMO: "Control-Prefab" creates only the name. */
					informationSSAE.NameGameObjectAnimationControlUnityNative = setting.RuleNameAsset.NameGetAsset(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_CONTROL_ANIMATION_UNITYNATIVE, informationSSAE.NameFileBody, informationSSPJ.NameFileBody);
					informationSSAE.PrefabControlAnimationUnityNative.TableName[0] = setting.RuleNameAssetFolder.NameGetAssetFolder(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_CONTROL_ANIMATION_UNITYNATIVE, nameOutputAssetFolderBase)
																					+ informationSSAE.NameGameObjectAnimationControlUnityNative
																					+ LibraryEditor_SpriteStudio6.Import.NameExtensionPrefab;
					informationSSAE.PrefabControlAnimationUnityNative.TableData[0] = AssetDatabase.LoadAssetAtPath<GameObject>(informationSSAE.PrefabControlAnimationUnityNative.TableName[0]);

					return(true);

//				AssetNameDecideData_ErroeEnd:;
//					return(false);
				}

				public static bool AssetCreateData(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
													LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
													LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE,
													int indexAnimation
												)
				{
					const string messageLogPrefix = "Create Asset(Animation-Clip)";

					LibraryEditor_SpriteStudio6.Import.SSAE.Information.Animation informationAnimation = informationSSAE.TableAnimation[indexAnimation];

					AnimationClip dataAnimation = informationSSAE.DataAnimationUnityNative.TableData[indexAnimation];
					AnimationClipSettings settingAnimationClip = null;
					if(null == dataAnimation)
					{
						dataAnimation = new AnimationClip();
						AssetDatabase.CreateAsset(dataAnimation, informationSSAE.DataAnimationUnityNative.TableName[indexAnimation]);
						informationSSAE.DataAnimationUnityNative.TableData[indexAnimation] = dataAnimation;
					}
					else
					{
						settingAnimationClip = AnimationUtility.GetAnimationClipSettings(dataAnimation);
					}

					/* Create AnimationClip */
					dataAnimation.ClearCurves();
					int framePerSecond = informationAnimation.Data.FramePerSecond;
					dataAnimation.frameRate = (float)framePerSecond;

					int frameStart = informationAnimation.Data.FrameValidStart;
					int frameEnd = informationAnimation.Data.FrameValidEnd;

					int countParts = informationAnimation.TableParts.Length;
					LibraryEditor_SpriteStudio6.Import.SSAE.Information.Parts informationParts = null;
					LibraryEditor_SpriteStudio6.Import.SSAE.Information.Animation.Parts informationAnimationParts = null;
					GameObject gameObjectParts = null;
					SpriteRenderer spriteRendererParts = null;
#if UNITY_2017_1_OR_NEWER
					SpriteMask spriteMaskParts = null;
#else
					/* MEMO: Can not use "SpriteMask" in Unity5.6 or earlier.                               */
					/*       (For "Nintendo Switch" for the time being, corresponds to Unity5.6 or earlier) */
#endif
					SkinnedMeshRenderer skinnedMeshRendererParts = null;
					MeshRenderer meshRendererParts = null;
					Script_SpriteStudio6_PartsUnityNative scriptParts = null;
					string nameGameObject;

					List<AnimationEvent> listAnimationEvent = new List<AnimationEvent>();
					listAnimationEvent.Clear();

					for(int i=0; i<countParts; i++)
					{
						informationParts = informationSSAE.TableParts[i];
						informationAnimationParts = informationAnimation.TableParts[i];

						nameGameObject = informationParts.NameGameObjectUnityNative;
						gameObjectParts = informationSSAE.TableParts[i].GameObjectUnityNative;
						if(null == gameObjectParts)
						{
							continue;
						}
						spriteRendererParts = informationSSAE.TableParts[i].SpriteRendererUnityNative;
#if UNITY_2017_1_OR_NEWER
						spriteMaskParts = informationSSAE.TableParts[i].SpriteMaskUnityNative;
#else
						/* MEMO: Can not use "SpriteMask" in Unity5.6 or earlier.                               */
						/*       (For "Nintendo Switch" for the time being, corresponds to Unity5.6 or earlier) */
#endif
						skinnedMeshRendererParts = informationSSAE.TableParts[i].SkinnedMeshRendererUnityNative;
						meshRendererParts = informationSSAE.TableParts[i].MeshRendererUnityNative;
						scriptParts = informationSSAE.TableParts[i].ScriptPartsUnityNative;

						/* Set Curves */
						AssetCreateDataCurveSetPositionX(	dataAnimation,
															nameGameObject, gameObjectParts,
															informationAnimationParts.PositionX,
															indexAnimation,
															frameStart, frameEnd, framePerSecond,
															0.0f
														);
						AssetCreateDataCurveSetPositionY(	dataAnimation,
															nameGameObject, gameObjectParts,
															informationAnimationParts.PositionY,
															indexAnimation,
															frameStart, frameEnd, framePerSecond,
															0.0f
														);
						AssetCreateDataCurveSetPositionZ(	dataAnimation,
															nameGameObject, gameObjectParts,
															informationAnimationParts.PositionZ,
															indexAnimation,
															frameStart, frameEnd, framePerSecond,
															0.0f
														);

						AssetCreateDataCurveSetRotationX(	dataAnimation,
															nameGameObject, gameObjectParts,
															informationAnimationParts.RotationX,
															indexAnimation,
															frameStart, frameEnd, framePerSecond,
															0.0f
														);
						AssetCreateDataCurveSetRotationY(	dataAnimation,
															nameGameObject, gameObjectParts,
															informationAnimationParts.RotationY,
															indexAnimation,
															frameStart, frameEnd, framePerSecond,
															0.0f
														);
						AssetCreateDataCurveSetRotationZ(	dataAnimation,
															nameGameObject, gameObjectParts,
															informationAnimationParts.RotationZ,
															indexAnimation,
															frameStart, frameEnd, framePerSecond,
															0.0f
														);

						AssetCreateDataCurveSetScalingX(	dataAnimation,
															nameGameObject, gameObjectParts,
															informationAnimationParts.ScalingX,
															indexAnimation,
															frameStart, frameEnd, framePerSecond,
															1.0f
														);
						AssetCreateDataCurveSetScalingY(	dataAnimation,
															nameGameObject, gameObjectParts,
															informationAnimationParts.ScalingY,
															indexAnimation,
															frameStart, frameEnd, framePerSecond,
															1.0f
														);

						AssetCreateDataCurveSetUserData(	dataAnimation,
															listAnimationEvent,
															informationAnimationParts.UserData,
															frameStart, frameEnd, framePerSecond
														);

						switch(informationParts.Data.Feature)
						{
							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.ROOT:
							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NULL:
								break;

//							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE2:
//							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE4:
							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL:
								if(null != spriteRendererParts)
								{
									AssetCreateDataCurveSetCellSprite(	dataAnimation,
																		nameGameObject, spriteRendererParts, gameObjectParts,
																		informationAnimationParts.Cell,
																		indexAnimation,
																		frameStart, frameEnd, framePerSecond,
																		informationSSPJ
																	);

									AssetCreateDataCurveSetHide(	dataAnimation,
																	nameGameObject, typeof(SpriteRenderer), spriteRendererParts,
																	informationAnimationParts.Hide,
																	indexAnimation,
																	frameStart, frameEnd, framePerSecond,
																	true
																);

									if(0 < informationAnimationParts.RateOpacityLocal.CountGetKey())
									{	/* Local Opacity */
										AssetCreateDataCurveSetPartsColorSprite(	dataAnimation,
																					nameGameObject, spriteRendererParts,
																					informationAnimationParts.PartsColor,
																					informationAnimationParts.RateOpacityLocal,
																					indexAnimation,
																					frameStart, frameEnd, framePerSecond
																				);
									}
									else
									{	/* Opacity */
										AssetCreateDataCurveSetPartsColorSprite(	dataAnimation,
																					nameGameObject, spriteRendererParts,
																					informationAnimationParts.PartsColor,
																					informationAnimationParts.RateOpacity,
																					indexAnimation,
																					frameStart, frameEnd, framePerSecond
																				);
									}

									AssetCreateDataCurveSetLocalScale(	dataAnimation,
																		nameGameObject, typeof(SpriteRenderer),
 																		informationAnimationParts.ScalingXLocal,
																		informationAnimationParts.ScalingYLocal,
																		indexAnimation,
																		frameStart, frameEnd, framePerSecond
																	);

									AssetCreateDataCurveSetVertexCorrection(	dataAnimation,
																				nameGameObject, spriteRendererParts,
 																				informationAnimationParts.VertexCorrection,
																				indexAnimation,
																				frameStart, frameEnd, framePerSecond
																			);
								}

								AssetCreateDataCurveSetOrderDraw(	dataAnimation,
																	nameGameObject, scriptParts,
																	informationAnimation,
																	i,
																	indexAnimation,
																	frameStart, frameEnd, framePerSecond,
																	informationSSPJ,
																	informationSSAE
																);
								break;

							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE:
							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT:
								break;

//							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE2:
//							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE4:
							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK:
#if UNITY_2017_1_OR_NEWER
								if(null != spriteMaskParts)
								{
									/* MEMO: Sprite can not set to SpriteMask ? */
									/* MEMO: In the case of Sprite, variable name is fixed ? */
									AssetCreateDataCurveSetCellMask(	dataAnimation,
																		nameGameObject, spriteMaskParts, gameObjectParts,
																		informationAnimationParts.Cell,
																		indexAnimation,
																		frameStart, frameEnd, framePerSecond,
																		informationSSPJ
																	);

									AssetCreateDataCurveSetHideMask(	dataAnimation,
																		nameGameObject, typeof(SpriteMask), spriteMaskParts,
																		informationAnimationParts.Hide,
																		informationAnimationParts.PowerMask,
																		indexAnimation,
																		frameStart, frameEnd, framePerSecond,
																		true
																	);

									AssetCreateDataCurveSetPowerMask(	dataAnimation,
																		nameGameObject,  spriteMaskParts,
																		informationAnimationParts.PowerMask,
																		indexAnimation,
																		frameStart, frameEnd, framePerSecond,
																		0.0f
																	);

									/* Not Support Attributes */
									if(0 < informationAnimationParts.ScalingXLocal.CountGetKey())
									{
										LogWarning(messageLogPrefix, "Unsupported Attribute \"Local Scaling X\" with \"Mask\" parts. Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
									}
									if(0 < informationAnimationParts.ScalingYLocal.CountGetKey())
									{
										LogWarning(messageLogPrefix, "Unsupported Attribute \"Local Scaling Y\" with \"Mask\" parts. Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
									}
									if(0 < informationAnimationParts.VertexCorrection.CountGetKey())
									{
										LogWarning(messageLogPrefix, "Unsupported Attribute \"Vertex Deformation\" with \"Mask\" parts. Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
									}
								}

								AssetCreateDataCurveSetOrderDraw(	dataAnimation,
																	nameGameObject, scriptParts,
																	informationAnimation,
																	i,
																	indexAnimation,
																	frameStart, frameEnd, framePerSecond,
																	informationSSPJ,
																	informationSSAE
																);
#else
								/* MEMO: Can not use "SpriteMask" in Unity5.6 or earlier.                               */
								/*       (For "Nintendo Switch" for the time being, corresponds to Unity5.6 or earlier) */
#endif
								break;

							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.JOINT:
							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONE:
							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MOVENODE:
							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.CONSTRAINT:
							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONEPOINT:
								break;

							case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH:
								{
									Renderer rendererParts = null;
									System.Type typeRenderer;
									if(null != skinnedMeshRendererParts)
									{
										rendererParts = skinnedMeshRendererParts;
										typeRenderer = typeof(SkinnedMeshRenderer);
									}
									else
									{
										rendererParts = meshRendererParts;
										typeRenderer = typeof(MeshRenderer);
									}
									bool flagValidRenderer = (null != rendererParts);

									if(true == flagValidRenderer)
									{
										/* Cell (Texture) */
										AssetCreateDataCurveSetCellSkinnedMesh(	dataAnimation,
																				nameGameObject, typeRenderer, rendererParts, gameObjectParts,
																				informationParts.DataMeshSkinnedUnityNative.TableData[0],
																				informationAnimationParts.Cell,
																				indexAnimation,
																				frameStart, frameEnd, framePerSecond,
																				informationSSPJ
																			);

										AssetCreateDataCurveSetHide(	dataAnimation,
																		nameGameObject, typeRenderer, rendererParts,
																		informationAnimationParts.Hide,
																		indexAnimation,
																		frameStart, frameEnd, framePerSecond,
																		true
																	);

										bool flagBoundVertex = false;
										if(0 < informationAnimationParts.RateOpacityLocal.CountGetKey())
										{	/* Local Opacity */
											AssetCreateDataCurveSetPartsColorSkinnedMesh(	dataAnimation,
																							nameGameObject, typeRenderer, rendererParts,
																							informationAnimationParts.PartsColor,
																							informationAnimationParts.RateOpacityLocal,
																							indexAnimation,
																							frameStart, frameEnd, framePerSecond,
																							ref flagBoundVertex
																						);
										}
										else
										{	/* Opacity */
											AssetCreateDataCurveSetPartsColorSkinnedMesh(	dataAnimation,
																							nameGameObject, typeRenderer, rendererParts,
																							informationAnimationParts.PartsColor,
																							informationAnimationParts.RateOpacity,
																							indexAnimation,
																							frameStart, frameEnd, framePerSecond,
																							ref flagBoundVertex
																						);
										}
										if(true == flagBoundVertex)
										{
											LogWarning(messageLogPrefix, "Unsupported Data-Type \"Bound=Vertex\" Attribute \"Part Color\" with \"Mesh\" parts. Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
										}

										/* Not Support Attributes */
										if(0 < informationAnimationParts.ScalingXLocal.CountGetKey())
										{
											LogWarning(messageLogPrefix, "Unsupported Attribute \"Local Scaling X\" with \"Mesh\" parts. Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
										}
										if(0 < informationAnimationParts.ScalingYLocal.CountGetKey())
										{
											LogWarning(messageLogPrefix, "Unsupported Attribute \"Local Scaling Y\" with \"Mesh\" parts. Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
										}
										if(0 < informationAnimationParts.VertexCorrection.CountGetKey())
										{
											LogWarning(messageLogPrefix, "Unsupported Attribute \"Vertex Deformation\" with \"Mesh\" parts. Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
										}
									}
								}

								AssetCreateDataCurveSetOrderDraw(	dataAnimation,
																	nameGameObject, scriptParts,
																	informationAnimation,
																	i,
																	indexAnimation,
																	frameStart, frameEnd, framePerSecond,
																	informationSSPJ,
																	informationSSAE
																);
								break;
						}

						/* Not Support Attributes (Common) */
						if(0 < informationAnimationParts.FlipX.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"Horizontal Flip\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.FlipY.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"Vertical Flip\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.PivotOffsetX.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"X Pivot Offset\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.PivotOffsetY.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"Y Pivot Offsett\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.AnchorPositionX.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"X Anchor\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.AnchorPositionY.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"Y Anchor\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.SizeForceX.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"X Size\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.SizeForceY.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"Y Size\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.TexturePositionX.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"UV X Translation\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.TexturePositionY.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"UV Y Translation\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.TextureRotation.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"UV Rotation\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.TextureScalingX.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"UV X Scale\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.TextureScalingY.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"UV Y Scale\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.TextureFlipX.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"Image H Flip\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.TextureFlipY.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"Image V Flip\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.Instance.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"Instance\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.Effect.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"Effect\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.RadiusCollision.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"Collision Radius\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
						if(0 < informationAnimationParts.Deform.CountGetKey())
						{
							LogWarning(messageLogPrefix, "Unsupported Attribute \"Deform\" Parts[" + i.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						}
					}

					if(0 < listAnimationEvent.Count)
					{
						AnimationUtility.SetAnimationEvents(dataAnimation, listAnimationEvent.ToArray());
					}
					if(null != settingAnimationClip)
					{
						AnimationUtility.SetAnimationClipSettings(dataAnimation, settingAnimationClip);
					}
					dataAnimation.EnsureQuaternionContinuity();
					EditorUtility.SetDirty(dataAnimation);
					AssetDatabase.SaveAssets();

					return(true);

//				AssetCreateData_ErrorEnd:;
//					return(false);
				}
				private static void AssetCreateDataKeyFrameInitialize(ref Keyframe keyframe)
				{
					/* MEMO: Before Ver.1.0.28, (keyframes') curve were initialized in this function. */
					/*       But after Ver.1.0.30, curves are set in "AssetCreateCurveSetTangent".    */
					/*       (For modification due to "Keyframe.tangentMode"'s obsolete)              */
#if false
					keyframe.tangentMode = 31;
#endif
					keyframe.inTangent = float.PositiveInfinity;
					keyframe.outTangent = float.PositiveInfinity;
				}
				private static float AssetCreateDataTimeGetFrame(int frameNo, int framePerSecond)
				{
					return((float)frameNo / (float)framePerSecond);
				}
				private static void AssetCreateCurveSetTangent(AnimationCurve animationCurve)
				{
					int count = animationCurve.length;
					for(int i=0; i<count; i++)
					{
						AnimationUtility.SetKeyLeftTangentMode(animationCurve, i, AnimationUtility.TangentMode.Constant);
						AnimationUtility.SetKeyRightTangentMode(animationCurve, i, AnimationUtility.TangentMode.Constant);
					}
				}

				private static bool AssetCreateDataCurveSetHide(	AnimationClip animationClip,
																	string namePathGameObject,
																	System.Type type,
																	Renderer renderer,
																	Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool attributeBool,
																	int indexAnimation,
																	int frameStart,
																	int frameEnd,
																	int framePerSecond,
																	bool valueError
																	)
				{
					/* MEMO: Even if no attribute's key-data, key-frame is always generated.           */
					/*       (Since "Opacity" has changed even without key-data by parents' "Opacity") */
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(0 > countFrameRange)
					{
						renderer.enabled = false;
						return(true);
					}

					AnimationCurve animationCurve = new AnimationCurve();

					/* MEMO: First frame's data is forcibly get. */
					bool value;
					bool valuePrevious = true;
					if(false == Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetBoolOR(out value, attributeBool, frameStart, true))
					{
						value = false;
					}
					Keyframe keyframe = new Keyframe();
					AssetCreateDataKeyFrameInitialize(ref keyframe);
					keyframe.time = AssetCreateDataTimeGetFrame(0, framePerSecond);
					keyframe.value = (true == value) ? 0.0f : 1.0f;
					if(0 == indexAnimation)
					{
						renderer.enabled = (true == value) ? false : true;
					}

					animationCurve.AddKey(keyframe);
					valuePrevious = value;

					/* Create Frame 1 to end */
					int frame;
					for(int i=1; i<countFrameRange; i++)
					{
						frame = i + frameStart;

						if(false == Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetBoolOR(out value, attributeBool, frame, true))
						{
							value = valueError;
						}

						if(valuePrevious != value)
						{
							keyframe = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframe);
							keyframe.time = AssetCreateDataTimeGetFrame(i, framePerSecond);
							keyframe.value = (true == value) ? 0.0f : 1.0f;

							animationCurve.AddKey(keyframe);
						}

						valuePrevious = value;
					}

					AssetCreateCurveSetTangent(animationCurve);
					animationClip.SetCurve(namePathGameObject, type, "m_Enabled", animationCurve);

					return(true);
				}
				private static bool AssetCreateDataCurveSetHideMask(	AnimationClip animationClip,
																		string namePathGameObject,
																		System.Type type,
																		Renderer renderer,
																		Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool attributeBool,
																		Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat attributeMaskPower,
																		int indexAnimation,
																		int frameStart,
																		int frameEnd,
																		int framePerSecond,
																		bool valueError
																	)
				{
					/* MEMO: Even if no attribute's key-data, key-frame is always generated.           */
					/*       (Since "Opacity" has changed even without key-data by parents' "Opacity") */
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(0 > countFrameRange)
					{
						renderer.enabled = false;
						return(true);
					}

					AnimationCurve animationCurve = new AnimationCurve();

					/* MEMO: First frame's data is forcibly get. */
					bool value;
					bool valuePrevious = true;
					float valuePowerMask;
					if(false == Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetBoolOR(out value, attributeBool, frameStart, true))
					{
						value = false;
					}
					if(false == attributeMaskPower.ValueGet(out valuePowerMask, frameStart))
					{
						valuePowerMask = 255.0f;
					}
					if(0.0f >= valuePowerMask)
					{
						value = true;
					}
					Keyframe keyframe = new Keyframe();
					AssetCreateDataKeyFrameInitialize(ref keyframe);
					keyframe.time = AssetCreateDataTimeGetFrame(0, framePerSecond);
					keyframe.value = (true == value) ? 0.0f : 1.0f;
					if(0 == indexAnimation)
					{
						renderer.enabled = (true == value) ? false : true;
					}

					animationCurve.AddKey(keyframe);
					valuePrevious = value;

					/* Create Frame 1 to end */
					int frame;
					for(int i=1; i<countFrameRange; i++)
					{
						frame = i + frameStart;

						if(false == Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetBoolOR(out value, attributeBool, frame, true))
						{
							value = valueError;
						}
						if(false == attributeMaskPower.ValueGet(out valuePowerMask, frame))
						{
							valuePowerMask = 255.0f;
						}
						if(0.0f >= valuePowerMask)
						{
							value = true;
						}

						if(valuePrevious != value)
						{
							keyframe = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframe);
							keyframe.time = AssetCreateDataTimeGetFrame(i, framePerSecond);
							keyframe.value = (true == value) ? 0.0f : 1.0f;

							animationCurve.AddKey(keyframe);
						}

						valuePrevious = value;
					}

					AssetCreateCurveSetTangent(animationCurve);
					animationClip.SetCurve(namePathGameObject, type, "m_Enabled", animationCurve);

					return(true);
				}

				private static bool AssetCreateDataCurveSetPositionX(	AnimationClip animationClip,
																		string namePathGameObject,
																		GameObject gameObject,
																		Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat attributeFloat,
																		int indexAnimation,
																		int frameStart,
																		int frameEnd,
																		int framePerSecond,
																		float valueError
																	)
				{
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(1 > countFrameRange)
					{
						Vector3 position = gameObject.transform.localPosition;
						position.x = valueError;
						gameObject.transform.localPosition = position;
						return(true);
					}

					AnimationCurve animationCurve = new AnimationCurve();
					Keyframe keyframe;
					float value;

					if(0 >= attributeFloat.CountGetKey())
					{
						value = valueError;

						Vector3 position = gameObject.transform.localPosition;
						position.x = value;
						gameObject.transform.localPosition = position;

						keyframe = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframe);
						keyframe.time = AssetCreateDataTimeGetFrame(0, framePerSecond);
						keyframe.value = value;
						animationCurve.AddKey(keyframe);

						goto AssetCreateDataCurveSetPositionX_End;
					}

					float valuePrevious = float.NaN;
					int frame;
					bool flagForceSetKey;
					for(int i=0; i<countFrameRange; i++)
					{
						frame = i + frameStart;

						if(false == attributeFloat.ValueGet(out value, frame))
						{
							value = valueError;
						}

						/* MEMO: Must set before adding Curve to AnimationClip.       */
						/*       (If set later, animation is not reflected correctly) */
						if((0 == i) && (0 == indexAnimation))
						{
							Vector3 position = gameObject.transform.localPosition;
							position.x = value;
							gameObject.transform.localPosition = position;
						}

						/* MEMO: Forcibly create key-data.                  */
						/*       - first frame: for initialize              */
						flagForceSetKey = false;
						if(0 == i)
						{
							flagForceSetKey = true;
						}

						if((flagForceSetKey == true) || (valuePrevious != value))
						{
							keyframe = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframe);
							keyframe.time = AssetCreateDataTimeGetFrame(i, framePerSecond);
							keyframe.value = value;
							animationCurve.AddKey(keyframe);
						}

						valuePrevious = value;
					}
					/* MEMO: Forcibly create key-data.                  */
					/*       - end frame: for ensuring animation length */
					keyframe = new Keyframe();
					AssetCreateDataKeyFrameInitialize(ref keyframe);
					keyframe.time = AssetCreateDataTimeGetFrame(countFrameRange, framePerSecond);
					keyframe.value = valuePrevious;	/* Last value */
					animationCurve.AddKey(keyframe);

				AssetCreateDataCurveSetPositionX_End:;
					AssetCreateCurveSetTangent(animationCurve);
					animationClip.SetCurve(namePathGameObject, typeof(Transform), "localPosition.x", animationCurve);

					return(true);
				}
				private static bool AssetCreateDataCurveSetPositionY(	AnimationClip animationClip,
																		string namePathGameObject,
																		GameObject gameObject,
																		Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat attributeFloat,
																		int indexAnimation,
																		int frameStart,
																		int frameEnd,
																		int framePerSecond,
																		float valueError
																	)
				{
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(1 > countFrameRange)
					{
						Vector3 position = gameObject.transform.localPosition;
						position.y = valueError;
						gameObject.transform.localPosition = position;
						return(true);
					}

					AnimationCurve animationCurve = new AnimationCurve();
					Keyframe keyframe;
					float value;

					if(0 >= attributeFloat.CountGetKey())
					{
						value = valueError;

						Vector3 position = gameObject.transform.localPosition;
						position.y = value;
						gameObject.transform.localPosition = position;

						keyframe = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframe);
						keyframe.time = AssetCreateDataTimeGetFrame(0, framePerSecond);
						keyframe.value = value;
						animationCurve.AddKey(keyframe);

						goto AssetCreateDataCurveSetPositionY_End;
					}

					float valuePrevious = float.NaN;
					int frame;
					for(int i=0; i<countFrameRange; i++)
					{
						frame = i + frameStart;

						if(false == attributeFloat.ValueGet(out value, frame))
						{
							value = valueError;
						}

						/* MEMO: Must set before adding Curve to AnimationClip.       */
						/*       (If set later, animation is not reflected correctly) */
						if((0 == i) && (0 == indexAnimation))
						{
							Vector3 position = gameObject.transform.localPosition;
							position.y = value;
							gameObject.transform.localPosition = position;
						}

						if((0 == i) || (valuePrevious != value))
						{
							keyframe = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframe);
							keyframe.time = AssetCreateDataTimeGetFrame(i, framePerSecond);
							keyframe.value = value;
							animationCurve.AddKey(keyframe);
						}

						valuePrevious = value;
					}

				AssetCreateDataCurveSetPositionY_End:;
					AssetCreateCurveSetTangent(animationCurve);
					animationClip.SetCurve(namePathGameObject, typeof(Transform), "localPosition.y", animationCurve);

					return(true);
				}
				private static bool AssetCreateDataCurveSetPositionZ(	AnimationClip animationClip,
																		string namePathGameObject,
																		GameObject gameObject,
																		Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat attributeFloat,
																		int indexAnimation,
																		int frameStart,
																		int frameEnd,
																		int framePerSecond,
																		float valueError
																	)
				{
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(1 > countFrameRange)
					{
						Vector3 position = gameObject.transform.localPosition;
						position.z = valueError;
						gameObject.transform.localPosition = position;
						return(true);
					}

					AnimationCurve animationCurve = new AnimationCurve();
					Keyframe keyframe;
					float value;

					if(0 >= attributeFloat.CountGetKey())
					{
						value = valueError;

						Vector3 position = gameObject.transform.localPosition;
						position.z = value;
						gameObject.transform.localPosition = position;

						keyframe = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframe);
						keyframe.time = AssetCreateDataTimeGetFrame(0, framePerSecond);
						keyframe.value = value;
						animationCurve.AddKey(keyframe);

						goto AssetCreateDataCurveSetPositionZ_End;
					}

					float valuePrevious = float.NaN;
					int frame;
					for(int i=0; i<countFrameRange; i++)
					{
						frame = i + frameStart;

						if(false == attributeFloat.ValueGet(out value, frame))
						{
							value = valueError;
						}

						/* MEMO: Must set before adding Curve to AnimationClip.       */
						/*       (If set later, animation is not reflected correctly) */
						if((0 == i) && (0 == indexAnimation))
						{
							Vector3 position = gameObject.transform.localPosition;
							position.z = value;
							gameObject.transform.localPosition = position;
						}

						if((0 == i) || (valuePrevious != value))
						{
							keyframe = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframe);
							keyframe.time = AssetCreateDataTimeGetFrame(i, framePerSecond);
							keyframe.value = value;
							animationCurve.AddKey(keyframe);
						}

						valuePrevious = value;
					}

				AssetCreateDataCurveSetPositionZ_End:;
					AssetCreateCurveSetTangent(animationCurve);
					animationClip.SetCurve(namePathGameObject, typeof(Transform), "localPosition.z", animationCurve);

					return(true);
				}

				private static bool AssetCreateDataCurveSetRotationX(	AnimationClip animationClip,
																		string namePathGameObject,
																		GameObject gameObject,
																		Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat attributeFloat,
																		int indexAnimation,
																		int frameStart,
																		int frameEnd,
																		int framePerSecond,
																		float valueError
																	)
				{
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(1 > countFrameRange)
					{
						Vector3 rotation = gameObject.transform.localEulerAngles;
						rotation.x = valueError;
						gameObject.transform.localEulerAngles = rotation;
						return(true);
					}

					AnimationCurve animationCurve = new AnimationCurve();
					Keyframe keyframe;
					float value;

					if(0 >= attributeFloat.CountGetKey())
					{
						value = valueError;

						Vector3 rotation = gameObject.transform.localEulerAngles;
						rotation.x = value;
						gameObject.transform.localEulerAngles = rotation;

						keyframe = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframe);
						keyframe.time = AssetCreateDataTimeGetFrame(0, framePerSecond);
						keyframe.value = value;
						animationCurve.AddKey(keyframe);

						goto AssetCreateDataCurveSetRotationX_End;
					}

					float valuePrevious = float.NaN;
					int frame;
					for(int i=0; i<countFrameRange; i++)
					{
						frame = i + frameStart;

						if(false == attributeFloat.ValueGet(out value, frame))
						{
							value = valueError;
						}

						/* MEMO: Must set before adding Curve to AnimationClip.       */
						/*       (If set later, animation is not reflected correctly) */
						if((0 == i) && (0 == indexAnimation))
						{
							Vector3 rotation = gameObject.transform.localEulerAngles;
							rotation.x = value;
							gameObject.transform.localEulerAngles = rotation;
						}

						if((0 == i) || (valuePrevious != value))
						{
							keyframe = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframe);
							keyframe.time = AssetCreateDataTimeGetFrame(i, framePerSecond);
							keyframe.value = value;
							animationCurve.AddKey(keyframe);
						}

						valuePrevious = value;
					}

				AssetCreateDataCurveSetRotationX_End:;
					AssetCreateCurveSetTangent(animationCurve);
					animationClip.SetCurve(namePathGameObject, typeof(Transform), "localEulerAngles.x", animationCurve);

					return(true);
				}
				private static bool AssetCreateDataCurveSetRotationY(	AnimationClip animationClip,
																		string namePathGameObject,
																		GameObject gameObject,
																		Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat attributeFloat,
																		int indexAnimation,
																		int frameStart,
																		int frameEnd,
																		int framePerSecond,
																		float valueError
																	)
				{
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(1 > countFrameRange)
					{
						Vector3 rotation = gameObject.transform.localEulerAngles;
						rotation.y = valueError;
						gameObject.transform.localEulerAngles = rotation;
						return(true);
					}

					AnimationCurve animationCurve = new AnimationCurve();
					Keyframe keyframe;
					float value;

					if(0 >= attributeFloat.CountGetKey())
					{
						value = valueError;

						Vector3 rotation = gameObject.transform.localEulerAngles;
						rotation.y = value;
						gameObject.transform.localEulerAngles = rotation;

						keyframe = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframe);
						keyframe.time = AssetCreateDataTimeGetFrame(0, framePerSecond);
						keyframe.value = value;
						animationCurve.AddKey(keyframe);

						goto AssetCreateDataCurveSetRotationY_End;
					}

					float valuePrevious = float.NaN;
					int frame;
					for(int i=0; i<countFrameRange; i++)
					{
						frame = i + frameStart;

						if(false == attributeFloat.ValueGet(out value, frame))
						{
							value = valueError;
						}

						/* MEMO: Must set before adding Curve to AnimationClip.       */
						/*       (If set later, animation is not reflected correctly) */
						if((0 == i) && (0 == indexAnimation))
						{
							Vector3 rotation = gameObject.transform.localEulerAngles;
							rotation.y = value;
							gameObject.transform.localEulerAngles = rotation;
						}

						if((0 == i) || (valuePrevious != value))
						{
							keyframe = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframe);
							keyframe.time = AssetCreateDataTimeGetFrame(i, framePerSecond);
							keyframe.value = value;
							animationCurve.AddKey(keyframe);
						}

						valuePrevious = value;
					}

				AssetCreateDataCurveSetRotationY_End:;
					AssetCreateCurveSetTangent(animationCurve);
					animationClip.SetCurve(namePathGameObject, typeof(Transform), "localEulerAngles.y", animationCurve);

					return(true);
				}
				private static bool AssetCreateDataCurveSetRotationZ(	AnimationClip animationClip,
																		string namePathGameObject,
																		GameObject gameObject,
																		Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat attributeFloat,
																		int indexAnimation,
																		int frameStart,
																		int frameEnd,
																		int framePerSecond,
																		float valueError
																	)
				{
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(1 > countFrameRange)
					{
						Vector3 rotation = gameObject.transform.localEulerAngles;
						rotation.z = valueError;
						gameObject.transform.localEulerAngles = rotation;
						return(true);
					}

					AnimationCurve animationCurve = new AnimationCurve();
					Keyframe keyframe;
					float value;

					if(0 >= attributeFloat.CountGetKey())
					{
						value = valueError;

						Vector3 rotation = gameObject.transform.localEulerAngles;
						rotation.z = 0.0f;
						gameObject.transform.localEulerAngles = rotation;

						keyframe = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframe);
						keyframe.time = AssetCreateDataTimeGetFrame(0, framePerSecond);
						keyframe.value = value;
						animationCurve.AddKey(keyframe);

						goto AssetCreateDataCurveSetRotationZ_End;
					}

					float valuePrevious = float.NaN;
					int frame;
					for(int i=0; i<countFrameRange; i++)
					{
						frame = i + frameStart;

						if(false == attributeFloat.ValueGet(out value, frame))
						{
							value = valueError;
						}

						/* MEMO: Must set before adding Curve to AnimationClip.       */
						/*       (If set later, animation is not reflected correctly) */
						if((0 == i) && (0 == indexAnimation))
						{
							Vector3 rotation = gameObject.transform.localEulerAngles;
							rotation.z = value;
							gameObject.transform.localEulerAngles = rotation;
						}

						if((0 == i) || (valuePrevious != value))
						{
							keyframe = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframe);
							keyframe.time = AssetCreateDataTimeGetFrame(i, framePerSecond);
							keyframe.value = value;
							animationCurve.AddKey(keyframe);
						}

						valuePrevious = value;
					}

				AssetCreateDataCurveSetRotationZ_End:;
					AssetCreateCurveSetTangent(animationCurve);
					animationClip.SetCurve(namePathGameObject, typeof(Transform), "localEulerAngles.z", animationCurve);

					return(true);
				}

				private static bool AssetCreateDataCurveSetScalingX(	AnimationClip animationClip,
																		string namePathGameObject,
																		GameObject gameObject,
																		Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat attributeFloat,
																		int indexAnimation,
																		int frameStart,
																		int frameEnd,
																		int framePerSecond,
																		float valueError
																	)
				{
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(1 > countFrameRange)
					{
						Vector3 scaling = gameObject.transform.localScale;
						scaling.x = valueError;
						scaling.z = 1.0f;
						gameObject.transform.localScale = scaling;
						return(true);
					}

					AnimationCurve animationCurve = new AnimationCurve();
					Keyframe keyframe;
					float value;

					if(0 >= attributeFloat.CountGetKey())
					{
						value = valueError;

						Vector3 scaling = gameObject.transform.localScale;
						scaling.x = value;
						scaling.z = 1.0f;
						gameObject.transform.localScale = scaling;

						keyframe = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframe);
						keyframe.time = AssetCreateDataTimeGetFrame(0, framePerSecond);
						keyframe.value = value;
						animationCurve.AddKey(keyframe);

						goto AssetCreateDataCurveSetScalingX_End;
					}

					float valuePrevious = float.NaN;
					int frame;
					for(int i=0; i<countFrameRange; i++)
					{
						frame = i + frameStart;

						if(false == attributeFloat.ValueGet(out value, frame))
						{
							value = valueError;
						}

						/* MEMO: Must set before adding Curve to AnimationClip.       */
						/*       (If set later, animation is not reflected correctly) */
						if((0 == i) && (0 == indexAnimation))
						{
							Vector3 scaling = gameObject.transform.localScale;
							scaling.x = value;
							scaling.z = 1.0f;
							gameObject.transform.localScale = scaling;
						}

						if((0 == i) || (valuePrevious != value))
						{
							keyframe = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframe);
							keyframe.time = AssetCreateDataTimeGetFrame(i, framePerSecond);
							keyframe.value = value;
							animationCurve.AddKey(keyframe);
						}

						valuePrevious = value;
					}

				AssetCreateDataCurveSetScalingX_End:;
					AssetCreateCurveSetTangent(animationCurve);
					animationClip.SetCurve(namePathGameObject, typeof(Transform), "localScale.x", animationCurve);

					return(true);
				}
				private static bool AssetCreateDataCurveSetScalingY(	AnimationClip animationClip,
																		string namePathGameObject,
																		GameObject gameObject,
																		Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat attributeFloat,
																		int indexAnimation,
																		int frameStart,
																		int frameEnd,
																		int framePerSecond,
																		float valueError
																	)
				{
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(1 > countFrameRange)
					{
						Vector3 scaling = gameObject.transform.localScale;
						scaling.y = valueError;
						scaling.z = 1.0f;
						gameObject.transform.localScale = scaling;
						return(true);
					}

					AnimationCurve animationCurve = new AnimationCurve();
					Keyframe keyframe;
					float value;

					if(0 >= attributeFloat.CountGetKey())
					{
						value = valueError;

						Vector3 scaling = gameObject.transform.localScale;
						scaling.y = value;
						scaling.z = 1.0f;
						gameObject.transform.localScale = scaling;

						keyframe = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframe);
						keyframe.time = AssetCreateDataTimeGetFrame(0, framePerSecond);
						keyframe.value = value;
						animationCurve.AddKey(keyframe);

						goto AssetCreateDataCurveSetScalingY;
					}

					float valuePrevious = float.NaN;
					int frame;
					for(int i=0; i<countFrameRange; i++)
					{
						frame = i + frameStart;

						if(false == attributeFloat.ValueGet(out value, frame))
						{
							value = valueError;
						}

						/* MEMO: Must set before adding Curve to AnimationClip.       */
						/*       (If set later, animation is not reflected correctly) */
						if((0 == i) && (0 == indexAnimation))
						{
							Vector3 scaling = gameObject.transform.localScale;
							scaling.y = value;
							scaling.z = 1.0f;
							gameObject.transform.localScale = scaling;
						}

						if((0 == i) || (valuePrevious != value))
						{
							keyframe = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframe);
							keyframe.time = AssetCreateDataTimeGetFrame(i, framePerSecond);
							keyframe.value = value;
							animationCurve.AddKey(keyframe);
						}

						valuePrevious = value;
					}

				AssetCreateDataCurveSetScalingY:;
					AssetCreateCurveSetTangent(animationCurve);
					animationClip.SetCurve(namePathGameObject, typeof(Transform), "localScale.y", animationCurve);

					return(true);
				}

				private static bool AssetCreateDataCurveSetCellSprite(	AnimationClip animationClip,
																		string namePathGameObject,
																		SpriteRenderer spriteRenderer,
																		GameObject gameObjectParts,
																		Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeCell attributeCell,
																		int indexAnimation,
																		int frameStart,
																		int frameEnd,
																		int framePerSecond,
																		LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ
																	)
				{
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(1 > countFrameRange)
					{
						spriteRenderer.sprite = null;
						return(true);
					}

					spriteRenderer.sprite = null;

					Library_SpriteStudio6.Data.Animation.Attribute.Cell value = new Library_SpriteStudio6.Data.Animation.Attribute.Cell();
					Library_SpriteStudio6.Data.Animation.Attribute.Cell valuePrevious = new Library_SpriteStudio6.Data.Animation.Attribute.Cell();
					Library_SpriteStudio6.Data.CellMap.Cell dataCell = new Library_SpriteStudio6.Data.CellMap.Cell();

					/* MEMO: First frame's data is forcibly get. */
					if(false == attributeCell.ValueGet(out value, frameStart))
					{
						spriteRenderer.sprite = null;
						return(true);
					}

					EditorCurveBinding curveBinding = new EditorCurveBinding();
					curveBinding.type = typeof(Script_SpriteStudio6_PartsUnityNative);
					curveBinding.path = namePathGameObject;
					curveBinding.propertyName = NameScriptPropertyCell;

					List<ObjectReferenceKeyframe> listKeyFrame = new List<ObjectReferenceKeyframe>();
					ObjectReferenceKeyframe keyFrame = new ObjectReferenceKeyframe();

					bool flagValueValid = AssetCreateDataObjectGetCell(	ref keyFrame, ref dataCell,
																		value, 
																		AssetCreateDataTimeGetFrame(frameStart, framePerSecond),
																		informationSSPJ
																	);
					if(true == flagValueValid)
					{
						listKeyFrame.Add(keyFrame);
						valuePrevious = value;

						if(0 == indexAnimation)
						{
							if(null != gameObjectParts)
							{
								Script_SpriteStudio6_PartsUnityNative scriptParts = gameObjectParts.GetComponent<Script_SpriteStudio6_PartsUnityNative>();
								if(null != scriptParts)
								{
									scriptParts.Cell = (Sprite)keyFrame.value;
								}
							}
						}
					}

					int countAttribute = attributeCell.CountGetKey();
					int frame;
					for(int i=1; i<countAttribute; i++)
					{
						frame = attributeCell.ListKey[i].Frame;
						if(((frameStart + 1) <= frame) && (frameEnd >= frame))
						{
							frame -= frameStart;

							value = attributeCell.ListKey[i].Value;
							flagValueValid = AssetCreateDataObjectGetCell(	ref keyFrame, ref dataCell,
																			value,
																			AssetCreateDataTimeGetFrame(frame, framePerSecond),
																			informationSSPJ
																		);
							if(true == flagValueValid)
							{
								if((valuePrevious.IndexCellMap != value.IndexCellMap) || (valuePrevious.IndexCell != value.IndexCell))
								{
									listKeyFrame.Add(keyFrame);
									valuePrevious = value;
								}
							}
						}
					}

					AnimationUtility.SetObjectReferenceCurve(animationClip, curveBinding, listKeyFrame.ToArray());

					return(true);
				}
#if UNITY_2017_1_OR_NEWER
				private static bool AssetCreateDataCurveSetCellMask(	AnimationClip animationClip,
																		string namePathGameObject,
																		SpriteMask spriteMask,
																		GameObject gameObjectParts,
																		Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeCell attributeCell,
																		int indexAnimation,
																		int frameStart,
																		int frameEnd,
																		int framePerSecond,
																		LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ
																	)
				{
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(1 > countFrameRange)
					{
						spriteMask.sprite = null;
						return(true);
					}

					spriteMask.sprite = null;

					Library_SpriteStudio6.Data.Animation.Attribute.Cell value = new Library_SpriteStudio6.Data.Animation.Attribute.Cell();
					Library_SpriteStudio6.Data.Animation.Attribute.Cell valuePrevious = new Library_SpriteStudio6.Data.Animation.Attribute.Cell();
					Library_SpriteStudio6.Data.CellMap.Cell dataCell = new Library_SpriteStudio6.Data.CellMap.Cell();

					/* MEMO: First frame's data is forcibly get. */
					if(false == attributeCell.ValueGet(out value, frameStart))
					{
						spriteMask.sprite = null;
						return(true);
					}

					EditorCurveBinding curveBinding = new EditorCurveBinding();
					curveBinding.type = typeof(Script_SpriteStudio6_PartsUnityNative);
					curveBinding.path = namePathGameObject;
					curveBinding.propertyName = NameScriptPropertyCell;

					List<ObjectReferenceKeyframe> listKeyFrame = new List<ObjectReferenceKeyframe>();
					ObjectReferenceKeyframe keyFrame = new ObjectReferenceKeyframe();
					bool flagValueValid = AssetCreateDataObjectGetCell(	ref keyFrame, ref dataCell,
																		value,
																		AssetCreateDataTimeGetFrame(frameStart, framePerSecond),
																		informationSSPJ
																	);
					if(true == flagValueValid)
					{
						listKeyFrame.Add(keyFrame);
						valuePrevious = value;

						if(0 == indexAnimation)
						{
							if(null != gameObjectParts)
							{
								Script_SpriteStudio6_PartsUnityNative scriptParts = gameObjectParts.GetComponent<Script_SpriteStudio6_PartsUnityNative>();
								if(null != scriptParts)
								{
									scriptParts.Cell = (Sprite)keyFrame.value;
								}
							}
						}
					}

					int countAttribute = attributeCell.CountGetKey();
					int frame;
					for(int i=1; i<countAttribute; i++)
					{
						frame = attributeCell.ListKey[i].Frame;
						if(((frameStart + 1) <= frame) && (frameEnd >= frame))
						{
							frame -= frameStart;

							if((valuePrevious.IndexCellMap != value.IndexCellMap) || (valuePrevious.IndexCell != value.IndexCell))
							{
								flagValueValid = AssetCreateDataObjectGetCell(	ref keyFrame, ref dataCell,
																				value,
																				AssetCreateDataTimeGetFrame(frame, framePerSecond),
																				informationSSPJ
																			);
								if(true == flagValueValid)
								{
									listKeyFrame.Add(keyFrame);
									valuePrevious = value;
								}
							}
						}
					}

					AnimationUtility.SetObjectReferenceCurve(animationClip, curveBinding, listKeyFrame.ToArray());

					return(true);
				}
#else
				/* MEMO: Can not use "SpriteMask" in Unity5.6 or earlier.                               */
				/*       (For "Nintendo Switch" for the time being, corresponds to Unity5.6 or earlier) */
#endif
				private static bool AssetCreateDataCurveSetCellSkinnedMesh(	AnimationClip animationClip,
																			string namePathGameObject,
																			System.Type type,
																			Renderer renderer,
																			GameObject gameObjectParts,
																			Mesh meshSetUp,
																			Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeCell attributeCell,
																			int indexAnimation,
																			int frameStart,
																			int frameEnd,
																			int framePerSecond,
																			LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ
																		)
				{
					/* MEMO: Mesh can not change cells on the way (Can only be set with "SetUp" animation) */
					Library_SpriteStudio6.Data.Animation.Attribute.Cell value = new Library_SpriteStudio6.Data.Animation.Attribute.Cell();

					if(null == meshSetUp)
					{
						goto AssetCreateDataCurveSetCellSkinnedMesh_NoCreateEnd;
					}
					if(false == attributeCell.ValueGet(out value, frameStart))
					{
						goto AssetCreateDataCurveSetCellSkinnedMesh_NoCreateEnd;
					}
					if((0 > value.IndexCellMap) || (0 > value.IndexCell))
					{
						goto AssetCreateDataCurveSetCellSkinnedMesh_NoCreateEnd;
					}
					int indexTexture = informationSSPJ.TableInformationSSCE[value.IndexCellMap].IndexTexture;
					if(0 > indexTexture)
					{
						goto AssetCreateDataCurveSetCellSkinnedMesh_NoCreateEnd;
					}
					Texture2D textureMesh = informationSSPJ.TableInformationTexture[indexTexture].PrefabTexture.TableData[0];
					if(null == textureMesh)
					{
						goto AssetCreateDataCurveSetCellSkinnedMesh_NoCreateEnd;
					}

					EditorCurveBinding curveBindingMesh = new EditorCurveBinding();
					curveBindingMesh.type = typeof(Script_SpriteStudio6_PartsUnityNative);
					curveBindingMesh.path = namePathGameObject;
					curveBindingMesh.propertyName = NameScriptPropertyCellMesh;

					EditorCurveBinding curveBindingTexture = new EditorCurveBinding();
					curveBindingTexture.type = typeof(Script_SpriteStudio6_PartsUnityNative);
					curveBindingTexture.path = namePathGameObject;
					curveBindingTexture.propertyName = NameScriptPropertyTextureMesh;

					float time = AssetCreateDataTimeGetFrame(0, framePerSecond);

					List<ObjectReferenceKeyframe> listKeyFrameMesh = new List<ObjectReferenceKeyframe>();
					ObjectReferenceKeyframe keyFrameMesh = new ObjectReferenceKeyframe();
					keyFrameMesh.time = time;
					keyFrameMesh.value = (UnityEngine.Object)meshSetUp;
					listKeyFrameMesh.Add(keyFrameMesh);

					List<ObjectReferenceKeyframe> listKeyFrameTexture = new List<ObjectReferenceKeyframe>();
					ObjectReferenceKeyframe keyFrameTexture = new ObjectReferenceKeyframe();
					keyFrameTexture.time = time;
					keyFrameTexture.value = (UnityEngine.Object)textureMesh;
					listKeyFrameTexture.Add(keyFrameTexture);

					AnimationUtility.SetObjectReferenceCurve(animationClip, curveBindingMesh, listKeyFrameMesh.ToArray());
					AnimationUtility.SetObjectReferenceCurve(animationClip, curveBindingTexture, listKeyFrameTexture.ToArray());
					return(true);

				AssetCreateDataCurveSetCellSkinnedMesh_NoCreateEnd:;
					return(true);
				}
				private static bool AssetCreateDataObjectGetCell(	ref ObjectReferenceKeyframe keyFrame,
																	ref Library_SpriteStudio6.Data.CellMap.Cell dataCell,
																	Library_SpriteStudio6.Data.Animation.Attribute.Cell cell,
																	float time,
																	LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ
																)
				{
					int indexCellMap = cell.IndexCellMap;
					int indexCell = cell.IndexCell;

					if((0 > indexCellMap) || (informationSSPJ.TableInformationSSCE.Length <= indexCellMap))
					{
						dataCell.CleanUp();
						return(false);
					}
					/* MEMO: Caution that both "TableInformationSSCE.TableCell" and "TableInformationSSCE.Data.TableCell" are null(purged). */
					if((0 > indexCell) || (informationSSPJ.TableInformationSSCE[indexCellMap].TableNameSpriteUnityNative.Length <= indexCell))
					{
						dataCell.CleanUp();
						return(false);
					}

					int indexTexture = informationSSPJ.TableInformationSSCE[indexCellMap].IndexTexture;
					List<Sprite> listSprite = informationSSPJ.TableInformationTexture[indexTexture].ListSpriteUnityNative;
					string nameCell = informationSSPJ.TableInformationSSCE[indexCellMap].TableNameSpriteUnityNative[indexCell];

					int countSprite = listSprite.Count;
					int indexSprite = -1;
					for(int i=0; i<countSprite; i++)
					{
						if(listSprite[i].name == nameCell)
						{
							indexSprite = i;
							break;
						}
					}
					if(0 > indexSprite)
					{
						dataCell.CleanUp();
						return(false);
					}

					keyFrame.time = time;
					keyFrame.value = (UnityEngine.Object)listSprite[indexSprite];

					dataCell = informationSSPJ.TableInformationSSCE[indexCellMap].TableCell[indexCell].Data;
					return(true);
				}

				private static bool AssetCreateDataCurveSetOrderDraw(	AnimationClip animationClip,
																		string namePathGameObject,
																		Script_SpriteStudio6_PartsUnityNative scriptPats,
																		LibraryEditor_SpriteStudio6.Import.SSAE.Information.Animation informationAnimation,
																		int idParts,
																		int indexAnimation,
																		int frameStart,
																		int frameEnd,
																		int framePerSecond,
																		LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
																		LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE
																	)
				{
					LibraryEditor_SpriteStudio6.Import.SSAE.Information.Animation.Parts informationAnimationParts = null;
					int idPartsNext;

					int countFrameRange = (frameEnd - frameStart) + 1;
					if(1 > countFrameRange)
					{
						scriptPats.OrderInLayer = 0;
						return(true);
					}

					AnimationCurve animationCurve = new AnimationCurve();
					Keyframe keyframe;
					bool flagDraw;
					int frame;
					int value = -1;
					int valuePrevious = -1;
					for(int i=0; i<countFrameRange; i++)
					{
						frame = i + frameStart;

						informationAnimationParts = informationAnimation.TableParts[0];
						idPartsNext = informationAnimationParts.TableOrderDraw[frame];
						value = 0;
						flagDraw = false;
						while(0 < idPartsNext)
						{
							if(idPartsNext == idParts)
							{
								flagDraw = true;
								break;	/* while-loop */
							}
							else
							{
								value++;
								informationAnimationParts = informationAnimation.TableParts[idPartsNext];
								idPartsNext = informationAnimationParts.TableOrderDraw[frame];
							}
						}
						/* MEMO: Must set before adding Curve to AnimationClip.       */
						/*       (If set later, animation is not reflected correctly) */
						if((0 == i) && (0 == indexAnimation))
						{
							scriptPats.OrderInLayer = value * 10;
						}

						if(true == flagDraw)
						{
							if((0 == i) || (valuePrevious != value))
							{
								keyframe = new Keyframe();
								AssetCreateDataKeyFrameInitialize(ref keyframe);
								keyframe.time = AssetCreateDataTimeGetFrame(i, framePerSecond);
								keyframe.value = value;

								animationCurve.AddKey(keyframe);
							}

							valuePrevious = value;
							if(value > informationSSAE.CountDrawPartsMaxUnityNative)
							{
								informationSSAE.CountDrawPartsMaxUnityNative = value;
							}
						}
					}

					AssetCreateCurveSetTangent(animationCurve);
					animationClip.SetCurve(namePathGameObject, typeof(Script_SpriteStudio6_PartsUnityNative), NameScriptPropertyOrderInLayer, animationCurve);

					return(true);
				}

				private static bool AssetCreateDataCurveSetPartsColorSprite(	AnimationClip animationClip,
																				string namePathGameObject,
																				SpriteRenderer spriteRenderer,
																				Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributePartsColor attributePartsColor,
																				Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat attributeRateOpacity,
																				int indexAnimation,
																				int frameStart,
																				int frameEnd,
																				int framePerSecond
																			)
				{
					/* MEMO: Even if no attribute's key-data, key-frame is always generated.           */
					/*       (Since "Opacity" has changed even without key-data by parents' "Opacity") */
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(1 > countFrameRange)
					{
						return(true);
					}

					AnimationCurve animationCurveBlend = new AnimationCurve();
					AnimationCurve animationCurveAlpha = new AnimationCurve();
					AnimationCurve animationCurveColorALU = new AnimationCurve();
					AnimationCurve animationCurveColorRLU = new AnimationCurve();
					AnimationCurve animationCurveColorGLU = new AnimationCurve();
					AnimationCurve animationCurveColorBLU = new AnimationCurve();
					AnimationCurve animationCurveColorPowerLU = new AnimationCurve();
					AnimationCurve animationCurveColorARU = new AnimationCurve();
					AnimationCurve animationCurveColorRRU = new AnimationCurve();
					AnimationCurve animationCurveColorGRU = new AnimationCurve();
					AnimationCurve animationCurveColorBRU = new AnimationCurve();
					AnimationCurve animationCurveColorPowerRU = new AnimationCurve();
					AnimationCurve animationCurveColorARD = new AnimationCurve();
					AnimationCurve animationCurveColorRRD = new AnimationCurve();
					AnimationCurve animationCurveColorGRD = new AnimationCurve();
					AnimationCurve animationCurveColorBRD = new AnimationCurve();
					AnimationCurve animationCurveColorPowerRD = new AnimationCurve();
					AnimationCurve animationCurveColorALD = new AnimationCurve();
					AnimationCurve animationCurveColorRLD = new AnimationCurve();
					AnimationCurve animationCurveColorGLD = new AnimationCurve();
					AnimationCurve animationCurveColorBLD = new AnimationCurve();
					AnimationCurve animationCurveColorPowerLD = new AnimationCurve();

					Keyframe keyframeBlend;
					Keyframe keyframeAlpha;
					Keyframe keyframeColorALU;
					Keyframe keyframeColorRLU;
					Keyframe keyframeColorGLU;
					Keyframe keyframeColorBLU;
					Keyframe keyframeColorPowerLU;
					Keyframe keyframeColorARU;
					Keyframe keyframeColorRRU;
					Keyframe keyframeColorGRU;
					Keyframe keyframeColorBRU;
					Keyframe keyframeColorPowerRU;
					Keyframe keyframeColorARD;
					Keyframe keyframeColorRRD;
					Keyframe keyframeColorGRD;
					Keyframe keyframeColorBRD;
					Keyframe keyframeColorPowerRD;
					Keyframe keyframeColorALD;
					Keyframe keyframeColorRLD;
					Keyframe keyframeColorGLD;
					Keyframe keyframeColorBLD;
					Keyframe keyframeColorPowerLD;

					Library_SpriteStudio6.Data.Animation.Attribute.PartsColor value = new Library_SpriteStudio6.Data.Animation.Attribute.PartsColor();
					Color valueColorLU = new Color();
					Color valueColorRU = new Color();
					Color valueColorRD = new Color();
					Color valueColorLD = new Color();
					Color valueColorPreviousLU = ColorDefault;
					Color valueColorPreviousRU = ColorDefault;
					Color valueColorPreviousRD = ColorDefault;
					Color valueColorPreviousLD = ColorDefault;
					float valueColorPowerLU = 1.0f;
					float valueColorPowerRU = 1.0f;
					float valueColorPowerRD = 1.0f;
					float valueColorPowerLD = 1.0f;
					float valueColorPowerPreviousLU = float.NaN;
					float valueColorPowerPreviousRU = float.NaN;
					float valueColorPowerPreviousRD = float.NaN;
					float valueColorPowerPreviousLD = float.NaN;
					Library_SpriteStudio6.KindOperationBlend valueBlend;
					Library_SpriteStudio6.KindOperationBlend valueBlendPrevious;
					float valueOpacity;
					float valuePreviousOpacity;

					if(false == Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetFloatMultiple(out valueOpacity, attributeRateOpacity, frameStart, 1.0f))
					{
						valueOpacity = 1.0f;
					}
					if(false == attributePartsColor.ValueGet(out value, frameStart))
					{
						value = Library_SpriteStudio6.Data.Animation.Attribute.DefaultPartsColor;
					}
					valueBlend = value.Operation;

					switch(value.Bound)
					{
						case Library_SpriteStudio6.KindBoundBlend.NON:
							valueBlend = Library_SpriteStudio6.KindOperationBlend.MIX;

							valueColorLU =
							valueColorRU =
							valueColorRD =
							valueColorLD = ColorDefault;
							valueColorPowerLU =
							valueColorPowerRU =
							valueColorPowerRD =
							valueColorPowerLD = 1.0f;
							break;

						case Library_SpriteStudio6.KindBoundBlend.OVERALL:
							valueColorLU =
							valueColorRU =
							valueColorRD =
							valueColorLD = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU];
							valueColorPowerLU =
							valueColorPowerRU =
							valueColorPowerRD =
							valueColorPowerLD = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU];
							break;

						case Library_SpriteStudio6.KindBoundBlend.VERTEX:
							valueColorLU = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU];
							valueColorPowerLU = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU];

							valueColorRU = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RU];
							valueColorPowerRU = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RU];

							valueColorRD = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RD];
							valueColorPowerRD = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RD];

							valueColorLD = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LD];
							valueColorPowerLD = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LD];
							break;

						default:
							goto case Library_SpriteStudio6.KindBoundBlend.NON;
					}

					float time = AssetCreateDataTimeGetFrame(0, framePerSecond);

					valueBlendPrevious = valueBlend;
					keyframeBlend = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeBlend);
						keyframeBlend.time = time;
						keyframeBlend.value = (float)valueBlend + 0.01f;
						animationCurveBlend.AddKey(keyframeBlend);

					valuePreviousOpacity = valueOpacity;
					keyframeAlpha = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeAlpha);
						keyframeAlpha.time = time;
						keyframeAlpha.value = valueOpacity;
						animationCurveAlpha.AddKey(keyframeAlpha);

					valueColorPreviousLU = valueColorLU;
					keyframeColorALU = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorALU);
						keyframeColorALU.time = time;
						keyframeColorALU.value = valueColorLU.a;
						animationCurveColorALU.AddKey(keyframeColorALU);
					keyframeColorRLU = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorRLU);
						keyframeColorRLU.time = time;
						keyframeColorRLU.value = valueColorLU.r;
						animationCurveColorRLU.AddKey(keyframeColorRLU);
					keyframeColorGLU = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorGLU);
						keyframeColorGLU.time = time;
						keyframeColorGLU.value = valueColorLU.g;
						animationCurveColorGLU.AddKey(keyframeColorGLU);
					keyframeColorBLU = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorBLU);
						keyframeColorBLU.time = time;
						keyframeColorBLU.value = valueColorLU.b;
						animationCurveColorBLU.AddKey(keyframeColorBLU);
					valueColorPowerPreviousLU = valueColorPowerLU;
					keyframeColorPowerLU = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorPowerLU);
						keyframeColorPowerLU.time = time;
						keyframeColorPowerLU.value = valueColorPowerLU;
						animationCurveColorPowerLU.AddKey(keyframeColorPowerLU);

					valueColorPreviousRU = valueColorRU;
					keyframeColorARU = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorARU);
						keyframeColorARU.time = time;
						keyframeColorARU.value = valueColorRU.a;
						animationCurveColorARU.AddKey(keyframeColorARU);
					keyframeColorRRU = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorRRU);
						keyframeColorRRU.time = time;
						keyframeColorRRU.value = valueColorRU.r;
						animationCurveColorRRU.AddKey(keyframeColorRRU);
					keyframeColorGRU = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorGRU);
						keyframeColorGRU.time = time;
						keyframeColorGRU.value = valueColorRU.g;
						animationCurveColorGRU.AddKey(keyframeColorGRU);
					keyframeColorBRU = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorBRU);
						keyframeColorBRU.time = time;
						keyframeColorBRU.value = valueColorRU.b;
						animationCurveColorBRU.AddKey(keyframeColorBRU);
					valueColorPowerPreviousRU = valueColorPowerRU;
					keyframeColorPowerRU = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorPowerRU);
						keyframeColorPowerRU.time = time;
						keyframeColorPowerRU.value = valueColorPowerRU;
						animationCurveColorPowerRU.AddKey(keyframeColorPowerRU);

					valueColorPreviousRD = valueColorRD;
					keyframeColorARD = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorARD);
						keyframeColorARD.time = time;
						keyframeColorARD.value = valueColorRD.a;
						animationCurveColorARD.AddKey(keyframeColorARD);
					keyframeColorRRD = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorRRD);
						keyframeColorRRD.time = time;
						keyframeColorRRD.value = valueColorRD.r;
						animationCurveColorRRD.AddKey(keyframeColorRRD);
					keyframeColorGRD = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorGRD);
						keyframeColorGRD.time = time;
						keyframeColorGRD.value = valueColorRD.g;
						animationCurveColorGRD.AddKey(keyframeColorGRD);
					keyframeColorBRD = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorBRD);
						keyframeColorBRD.time = time;
						keyframeColorBRD.value = valueColorRD.b;
						animationCurveColorBRD.AddKey(keyframeColorBRD);
					valueColorPowerPreviousRD = valueColorPowerRD;
					keyframeColorPowerRD = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorPowerRD);
						keyframeColorPowerRD.time = time;
						keyframeColorPowerRD.value = valueColorPowerRD;
						animationCurveColorPowerRD.AddKey(keyframeColorPowerRD);

					valueColorPreviousLD = valueColorLD;
					keyframeColorALD = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorALD);
						keyframeColorALD.time = time;
						keyframeColorALD.value = valueColorLD.a;
						animationCurveColorALD.AddKey(keyframeColorALD);
					keyframeColorRLD = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorRLD);
						keyframeColorRLD.time = time;
						keyframeColorRLD.value = valueColorLD.r;
						animationCurveColorRLD.AddKey(keyframeColorRLD);
					keyframeColorGLD = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorGLD);
						keyframeColorGLD.time = time;
						keyframeColorGLD.value = valueColorLD.g;
						animationCurveColorGLD.AddKey(keyframeColorGLD);
					keyframeColorBLD = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorBLD);
						keyframeColorBLD.time = time;
						keyframeColorBLD.value = valueColorLD.b;
						animationCurveColorBLD.AddKey(keyframeColorBLD);
					valueColorPowerPreviousLD = valueColorPowerLD;
					keyframeColorPowerLD = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorPowerLD);
						keyframeColorPowerLD.time = time;
						keyframeColorPowerLD.value = valueColorPowerLD;
						animationCurveColorPowerLD.AddKey(keyframeColorPowerLD);

					int frame;
					for(int i=1; i<countFrameRange; i++)
					{
						frame = i + frameStart;

						if(false == Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetFloatMultiple(out valueOpacity, attributeRateOpacity, frame, 1.0f))
						{
							valueOpacity = 1.0f;
						}
						if(false == attributePartsColor.ValueGet(out value, frame))
						{
							value = Library_SpriteStudio6.Data.Animation.Attribute.DefaultPartsColor;
						}
						valueBlend = value.Operation;

						switch(value.Bound)
						{
							case Library_SpriteStudio6.KindBoundBlend.NON:
								valueBlend = Library_SpriteStudio6.KindOperationBlend.MIX;

								valueColorLU =
								valueColorRU =
								valueColorRD =
								valueColorLD = ColorDefault;
								valueColorPowerLU =
								valueColorPowerRU =
								valueColorPowerRD =
								valueColorPowerLD = 1.0f;
								break;

							case Library_SpriteStudio6.KindBoundBlend.OVERALL:
								valueColorLU =
								valueColorRU =
								valueColorRD =
								valueColorLD = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU];
								valueColorPowerLU =
								valueColorPowerRU =
								valueColorPowerRD =
								valueColorPowerLD = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU];
								break;

							case Library_SpriteStudio6.KindBoundBlend.VERTEX:
								valueColorLU = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU];
								valueColorPowerLU = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU];

								valueColorRU = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RU];
								valueColorPowerRU = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RU];

								valueColorRD = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RD];
								valueColorPowerRD = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RD];

								valueColorLD = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LD];
								valueColorPowerLD = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LD];
								break;

							default:
								goto case Library_SpriteStudio6.KindBoundBlend.NON;
						}

						time = AssetCreateDataTimeGetFrame(i, framePerSecond);

						if(valueBlendPrevious != valueBlend)
						{
							valueBlendPrevious = valueBlend;
							keyframeBlend = new Keyframe();
								AssetCreateDataKeyFrameInitialize(ref keyframeBlend);
								keyframeBlend.time = time;
								keyframeBlend.value = (float)valueBlend + 0.01f;
								animationCurveBlend.AddKey(keyframeBlend);
						}
						if(valuePreviousOpacity != valueOpacity)
						{
							valuePreviousOpacity = valueOpacity;
							keyframeAlpha = new Keyframe();
								AssetCreateDataKeyFrameInitialize(ref keyframeAlpha);
								keyframeAlpha.time = time;
								keyframeAlpha.value = valueOpacity;
								animationCurveAlpha.AddKey(keyframeAlpha);
						}

						if(valueColorPreviousLU != valueColorLU)
						{
							if(valueColorPreviousLU.a != valueColorLU.a)
							{
								keyframeColorALU = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorALU);
									keyframeColorALU.time = time;
									keyframeColorALU.value = valueColorLU.a;
									animationCurveColorALU.AddKey(keyframeColorALU);
							}

							if(valueColorPreviousLU.r != valueColorLU.r)
							{
								keyframeColorRLU = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorRLU);
									keyframeColorRLU.time = time;
									keyframeColorRLU.value = valueColorLU.r;
									animationCurveColorRLU.AddKey(keyframeColorRLU);
							}

							if(valueColorPreviousLU.g != valueColorLU.g)
							{
								keyframeColorGLU = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorGLU);
									keyframeColorGLU.time = time;
									keyframeColorGLU.value = valueColorLU.g;
									animationCurveColorGLU.AddKey(keyframeColorGLU);
							}

							if(valueColorPreviousLU.b != valueColorLU.b)
							{
								keyframeColorBLU = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorBLU);
									keyframeColorBLU.time = time;
									keyframeColorBLU.value = valueColorLU.b;
									animationCurveColorBLU.AddKey(keyframeColorBLU);
							}

							valueColorPreviousLU = valueColorLU;
						}
						if(valueColorPowerPreviousLU != valueColorPowerLU)
						{
							valueColorPowerPreviousLU = valueColorPowerLU;

							keyframeColorPowerLU = new Keyframe();
								AssetCreateDataKeyFrameInitialize(ref keyframeColorPowerLU);
								keyframeColorPowerLU.time = time;
								keyframeColorPowerLU.value = valueColorPowerLU;
								animationCurveColorPowerLU.AddKey(keyframeColorPowerLU);
						}

						if(valueColorPreviousRU != valueColorRU)
						{
							if(valueColorPreviousRU.a != valueColorRU.a)
							{
								keyframeColorARU = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorARU);
									keyframeColorARU.time = time;
									keyframeColorARU.value = valueColorRU.a;
									animationCurveColorARU.AddKey(keyframeColorARU);
							}

							if(valueColorPreviousRU.r != valueColorRU.r)
							{
								keyframeColorRRU = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorRRU);
									keyframeColorRRU.time = time;
									keyframeColorRRU.value = valueColorRU.r;
									animationCurveColorRRU.AddKey(keyframeColorRRU);
							}

							if(valueColorPreviousRU.g != valueColorRU.g)
							{
								keyframeColorGRU = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorGRU);
									keyframeColorGRU.time = time;
									keyframeColorGRU.value = valueColorRU.g;
									animationCurveColorGRU.AddKey(keyframeColorGRU);
							}

							if(valueColorPreviousRU.b != valueColorRU.b)
							{
								keyframeColorBRU = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorBRU);
									keyframeColorBRU.time = time;
									keyframeColorBRU.value = valueColorRU.b;
									animationCurveColorBRU.AddKey(keyframeColorBRU);
							}

							valueColorPreviousRU = valueColorRU;
						}
						if(valueColorPowerPreviousRU != valueColorPowerRU)
						{
							valueColorPowerPreviousRU = valueColorPowerRU;
							keyframeColorPowerRU = new Keyframe();
								AssetCreateDataKeyFrameInitialize(ref keyframeColorPowerRU);
								keyframeColorPowerRU.time = time;
								keyframeColorPowerRU.value = valueColorPowerRU;
								animationCurveColorPowerRU.AddKey(keyframeColorPowerRU);
						}

						if(valueColorPreviousRD != valueColorRD)
						{
							if(valueColorPreviousRD.a != valueColorRD.a)
							{
								keyframeColorARD = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorARD);
									keyframeColorARD.time = time;
									keyframeColorARD.value = valueColorRD.a;
									animationCurveColorARD.AddKey(keyframeColorARD);
							}

							if(valueColorPreviousRD.r != valueColorRD.r)
							{
								keyframeColorRRD = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorRRD);
									keyframeColorRRD.time = time;
									keyframeColorRRD.value = valueColorRD.r;
									animationCurveColorRRD.AddKey(keyframeColorRRD);
							}

							if(valueColorPreviousRD.g != valueColorRD.g)
							{
								keyframeColorGRD = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorGRD);
									keyframeColorGRD.time = time;
									keyframeColorGRD.value = valueColorRD.g;
									animationCurveColorGRD.AddKey(keyframeColorGRD);
							}

							if(valueColorPreviousRD.b != valueColorRD.b)
							{
								keyframeColorBRD = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorBRD);
									keyframeColorBRD.time = time;
									keyframeColorBRD.value = valueColorRD.b;
									animationCurveColorBRD.AddKey(keyframeColorBRD);
							}

							valueColorPreviousRD = valueColorRD;
						}
						if(valueColorPowerPreviousRD != valueColorPowerRD)
						{
							valueColorPowerPreviousRD = valueColorPowerRD;
							keyframeColorPowerRD = new Keyframe();
								AssetCreateDataKeyFrameInitialize(ref keyframeColorPowerRD);
								keyframeColorPowerRD.time = time;
								keyframeColorPowerRD.value = valueColorPowerRD;
								animationCurveColorPowerRD.AddKey(keyframeColorPowerRD);
						}

						if(valueColorPreviousLD != valueColorLD)
						{
							if(valueColorPreviousLD.a != valueColorLD.a)
							{
								keyframeColorALD = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorALD);
									keyframeColorALD.time = time;
									keyframeColorALD.value = valueColorLD.a;
									animationCurveColorALD.AddKey(keyframeColorALD);
							}

							if(valueColorPreviousLD.r != valueColorLD.r)
							{
								keyframeColorRLD = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorRLD);
									keyframeColorRLD.time = time;
									keyframeColorRLD.value = valueColorLD.r;
									animationCurveColorRLD.AddKey(keyframeColorRLD);
							}

							if(valueColorPreviousLD.g != valueColorLD.g)
							{
								keyframeColorGLD = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorGLD);
									keyframeColorGLD.time = time;
									keyframeColorGLD.value = valueColorLD.g;
									animationCurveColorGLD.AddKey(keyframeColorGLD);
							}

							if(valueColorPreviousLD.b != valueColorLD.b)
							{
								keyframeColorBLD = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorBLD);
									keyframeColorBLD.time = time;
									keyframeColorBLD.value = valueColorLD.b;
									animationCurveColorBLD.AddKey(keyframeColorBLD);
							}

							valueColorPreviousLD = valueColorLD;
						}
						if(valueColorPowerPreviousLD != valueColorPowerLD)
						{
							valueColorPowerPreviousLD = valueColorPowerLD;
							keyframeColorPowerLD = new Keyframe();
								AssetCreateDataKeyFrameInitialize(ref keyframeColorPowerLD);
								keyframeColorPowerLD.time = time;
								keyframeColorPowerLD.value = valueColorPowerLD;
								animationCurveColorPowerLD.AddKey(keyframeColorPowerLD);
						}
					}

					AssetCreateCurveSetTangent(animationCurveBlend);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyBlendParam + ".x", animationCurveBlend);
					AssetCreateCurveSetTangent(animationCurveAlpha);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyBlendParam + ".y", animationCurveAlpha);

					AssetCreateCurveSetTangent(animationCurveColorALU);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorLU + ".a", animationCurveColorALU);
					AssetCreateCurveSetTangent(animationCurveColorRLU);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorLU + ".r", animationCurveColorRLU);
					AssetCreateCurveSetTangent(animationCurveColorGLU);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorLU + ".g", animationCurveColorGLU);
					AssetCreateCurveSetTangent(animationCurveColorBLU);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorLU + ".b", animationCurveColorBLU);

					AssetCreateCurveSetTangent(animationCurveColorARU);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorRU + ".a", animationCurveColorARU);
					AssetCreateCurveSetTangent(animationCurveColorRRU);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorRU + ".r", animationCurveColorRRU);
					AssetCreateCurveSetTangent(animationCurveColorGRU);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorRU + ".g", animationCurveColorGRU);
					AssetCreateCurveSetTangent(animationCurveColorBRU);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorRU + ".b", animationCurveColorBRU);

					AssetCreateCurveSetTangent(animationCurveColorARD);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorRD + ".a", animationCurveColorARD);
					AssetCreateCurveSetTangent(animationCurveColorRRD);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorRD + ".r", animationCurveColorRRD);
					AssetCreateCurveSetTangent(animationCurveColorGRD);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorRD + ".g", animationCurveColorGRD);
					AssetCreateCurveSetTangent(animationCurveColorBRD);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorRD + ".b", animationCurveColorBRD);

					AssetCreateCurveSetTangent(animationCurveColorALD);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorLD + ".a", animationCurveColorALD);
					AssetCreateCurveSetTangent(animationCurveColorRLD);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorLD + ".r", animationCurveColorRLD);
					AssetCreateCurveSetTangent(animationCurveColorGLD);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorLD + ".g", animationCurveColorGLD);
					AssetCreateCurveSetTangent(animationCurveColorBLD);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorLD + ".b", animationCurveColorBLD);

					AssetCreateCurveSetTangent(animationCurveColorPowerLU);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorOpacity + ".x", animationCurveColorPowerLU);
					AssetCreateCurveSetTangent(animationCurveColorPowerRU);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorOpacity + ".y", animationCurveColorPowerRU);
					AssetCreateCurveSetTangent(animationCurveColorPowerRD);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorOpacity + ".z", animationCurveColorPowerRD);
					AssetCreateCurveSetTangent(animationCurveColorPowerLD);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyPartsColorOpacity + ".w", animationCurveColorPowerLD);

					return(true);
				}
				private static bool AssetCreateDataCurveSetPartsColorSkinnedMesh(	AnimationClip animationClip,
																					string namePathGameObject,
																					System.Type type,
																					Renderer renderer,
																					Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributePartsColor attributePartsColor,
																					Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat attributeRateOpacity,
																					int indexAnimation,
																					int frameStart,
																					int frameEnd,
																					int framePerSecond,
																					ref bool flagBoundVertex
																				)
				{
					flagBoundVertex = false;

					/* MEMO: Even if no attribute's key-data, key-frame is always generated.           */
					/*       (Since "Opacity" has changed even without key-data by parents' "Opacity") */
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(1 > countFrameRange)
					{
						return(true);
					}

					/* MEMO: "Mesh" parts do not support per vertex "Part-Color". */
					AnimationCurve animationCurveBlend = new AnimationCurve();
					AnimationCurve animationCurveAlpha = new AnimationCurve();
					AnimationCurve animationCurveColorA = new AnimationCurve();
					AnimationCurve animationCurveColorR = new AnimationCurve();
					AnimationCurve animationCurveColorG = new AnimationCurve();
					AnimationCurve animationCurveColorB = new AnimationCurve();
					AnimationCurve animationCurveColorPower = new AnimationCurve();

					Keyframe keyframeBlend;
					Keyframe keyframeAlpha;
					Keyframe keyframeColorA;
					Keyframe keyframeColorR;
					Keyframe keyframeColorG;
					Keyframe keyframeColorB;
					Keyframe keyframeColorPower;

					Library_SpriteStudio6.Data.Animation.Attribute.PartsColor value = new Library_SpriteStudio6.Data.Animation.Attribute.PartsColor();
					Color valueColor = new Color();
					Color valueColorPrevious = ColorDefault;
					float valueColorPower = 1.0f;
					float valueColorPowerPrevious = float.NaN;
					Library_SpriteStudio6.KindOperationBlend valueBlend;
					Library_SpriteStudio6.KindOperationBlend valueBlendPrevious;
					float valueOpacity;
					float valuePreviousOpacity;

					if(false == Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetFloatMultiple(out valueOpacity, attributeRateOpacity, frameStart, 1.0f))
					{
						valueOpacity = 1.0f;
					}
					if(false == attributePartsColor.ValueGet(out value, frameStart))
					{
						value = Library_SpriteStudio6.Data.Animation.Attribute.DefaultPartsColor;
					}
					valueBlend = value.Operation;

					switch(value.Bound)
					{
						case Library_SpriteStudio6.KindBoundBlend.NON:
							valueBlend = Library_SpriteStudio6.KindOperationBlend.MIX;

							valueColor = ColorDefault;
							valueColorPower = 1.0f;
							break;

						case Library_SpriteStudio6.KindBoundBlend.OVERALL:
							valueColor = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU];
							valueColorPower = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU];
							break;

						case Library_SpriteStudio6.KindBoundBlend.VERTEX:
							/* MEMO: Originally, never reaches here since can not be set for each vertex. */
							valueColor = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU];
							valueColorPower = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU];
							flagBoundVertex |= true;
							break;

						default:
							goto case Library_SpriteStudio6.KindBoundBlend.NON;
					}

					float time = AssetCreateDataTimeGetFrame(0, framePerSecond);

					valueBlendPrevious = valueBlend;
					keyframeBlend = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeBlend);
						keyframeBlend.time = time;
						keyframeBlend.value = (float)valueBlend + 0.01f;
						animationCurveBlend.AddKey(keyframeBlend);

					valuePreviousOpacity = valueOpacity;
					keyframeAlpha = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeAlpha);
						keyframeAlpha.time = time;
						keyframeAlpha.value = valueOpacity;
						animationCurveAlpha.AddKey(keyframeAlpha);

					valueColorPrevious = valueColor;
					keyframeColorA = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorA);
						keyframeColorA.time = time;
						keyframeColorA.value = valueColor.a;
						animationCurveColorA.AddKey(keyframeColorA);
					keyframeColorR = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorR);
						keyframeColorR.time = time;
						keyframeColorR.value = valueColor.r;
						animationCurveColorR.AddKey(keyframeColorR);
					keyframeColorG = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorG);
						keyframeColorG.time = time;
						keyframeColorG.value = valueColor.g;
						animationCurveColorG.AddKey(keyframeColorG);
					keyframeColorB = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorB);
						keyframeColorB.time = time;
						keyframeColorB.value = valueColor.b;
						animationCurveColorB.AddKey(keyframeColorB);
					valueColorPowerPrevious = valueColorPower;
					keyframeColorPower = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeColorPower);
						keyframeColorPower.time = time;
						keyframeColorPower.value = valueColorPower;
						animationCurveColorPower.AddKey(keyframeColorPower);

					int frame;
					for(int i=1; i<countFrameRange; i++)
					{
						frame = i + frameStart;

						if(false == Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetFloatMultiple(out valueOpacity, attributeRateOpacity, frame, 1.0f))
						{
							valueOpacity = 1.0f;
						}
						if(false == attributePartsColor.ValueGet(out value, frame))
						{
							value = Library_SpriteStudio6.Data.Animation.Attribute.DefaultPartsColor;
						}
						valueBlend = value.Operation;

						switch(value.Bound)
						{
							case Library_SpriteStudio6.KindBoundBlend.NON:
								valueBlend = Library_SpriteStudio6.KindOperationBlend.MIX;

								valueColor = ColorDefault;
								valueColorPower = 1.0f;
								break;

							case Library_SpriteStudio6.KindBoundBlend.OVERALL:
								valueColor = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU];
								valueColorPower = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU];
								break;

							case Library_SpriteStudio6.KindBoundBlend.VERTEX:
								/* MEMO: Originally, never reaches here since can not be set for each vertex. */
								valueColor = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU];
								valueColorPower = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU];
								break;

							default:
								goto case Library_SpriteStudio6.KindBoundBlend.NON;
						}

						time = AssetCreateDataTimeGetFrame(i, framePerSecond);

						if(valueBlendPrevious != valueBlend)
						{
							valueBlendPrevious = valueBlend;
							keyframeBlend = new Keyframe();
								AssetCreateDataKeyFrameInitialize(ref keyframeBlend);
								keyframeBlend.time = time;
								keyframeBlend.value = (float)valueBlend + 0.01f;
								animationCurveBlend.AddKey(keyframeBlend);
						}
						if(valuePreviousOpacity != valueOpacity)
						{
							valuePreviousOpacity = valueOpacity;
							keyframeAlpha = new Keyframe();
								AssetCreateDataKeyFrameInitialize(ref keyframeAlpha);
								keyframeAlpha.time = time;
								keyframeAlpha.value = valueOpacity;
								animationCurveAlpha.AddKey(keyframeAlpha);
						}

						if(valueColorPrevious != valueColor)
						{
							if(valueColorPrevious.a != valueColor.a)
							{
								keyframeColorA = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorA);
									keyframeColorA.time = time;
									keyframeColorA.value = valueColor.a;
									animationCurveColorA.AddKey(keyframeColorA);
							}

							if(valueColorPrevious.r != valueColor.r)
							{
								keyframeColorR = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorR);
									keyframeColorR.time = time;
									keyframeColorR.value = valueColor.r;
									animationCurveColorR.AddKey(keyframeColorR);
							}

							if(valueColorPrevious.g != valueColor.g)
							{
								keyframeColorG = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorG);
									keyframeColorG.time = time;
									keyframeColorG.value = valueColor.g;
									animationCurveColorG.AddKey(keyframeColorG);
							}

							if(valueColorPrevious.b != valueColor.b)
							{
								keyframeColorB = new Keyframe();
									AssetCreateDataKeyFrameInitialize(ref keyframeColorB);
									keyframeColorB.time = time;
									keyframeColorB.value = valueColor.b;
									animationCurveColorB.AddKey(keyframeColorB);
							}

							valueColorPrevious = valueColor;
						}
						if(valueColorPowerPrevious != valueColorPower)
						{
							valueColorPowerPrevious = valueColorPower;

							keyframeColorPower = new Keyframe();
								AssetCreateDataKeyFrameInitialize(ref keyframeColorPower);
								keyframeColorPower.time = time;
								keyframeColorPower.value = valueColorPower;
								animationCurveColorPower.AddKey(keyframeColorPower);
						}
					}

					AssetCreateCurveSetTangent(animationCurveBlend);
					animationClip.SetCurve(namePathGameObject, type, "material." + NameShaderPropertyBlendParam + ".x", animationCurveBlend);
					AssetCreateCurveSetTangent(animationCurveAlpha);
					animationClip.SetCurve(namePathGameObject, type, "material." + NameShaderPropertyBlendParam + ".y", animationCurveAlpha);
					AssetCreateCurveSetTangent(animationCurveColorPower);
					animationClip.SetCurve(namePathGameObject, type, "material." + NameShaderPropertyBlendParam + ".z", animationCurveColorPower);

					AssetCreateCurveSetTangent(animationCurveColorA);
					animationClip.SetCurve(namePathGameObject, type, "material." + NameShaderPropertyPartsColorSkinnedMesh + ".a", animationCurveColorA);
					AssetCreateCurveSetTangent(animationCurveColorR);
					animationClip.SetCurve(namePathGameObject, type, "material." + NameShaderPropertyPartsColorSkinnedMesh + ".r", animationCurveColorR);
					AssetCreateCurveSetTangent(animationCurveColorG);
					animationClip.SetCurve(namePathGameObject, type, "material." + NameShaderPropertyPartsColorSkinnedMesh + ".g", animationCurveColorG);
					AssetCreateCurveSetTangent(animationCurveColorB);
					animationClip.SetCurve(namePathGameObject, type, "material." + NameShaderPropertyPartsColorSkinnedMesh + ".b", animationCurveColorB);

					return(true);
				}

				private static bool AssetCreateDataCurveSetLocalScale(	AnimationClip animationClip,
																		string namePathGameObject,
																		System.Type type,
																		Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat attributeScaleXLocal,
																		Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat attributeScaleYLocal,
																		int indexAnimation,
																		int frameStart,
																		int frameEnd,
																		int framePerSecond
																	)
				{
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(1 > countFrameRange)
					{
						return(true);
					}

					AnimationCurve animationCurveX = new AnimationCurve();
					AnimationCurve animationCurveY = new AnimationCurve();
					Keyframe keyframeX;
					Keyframe keyframeY;

					float valueX;
					float valueXPrevious = float.NaN;
					float valueY;
					float valueYPrevious = float.NaN;
					if(false == attributeScaleXLocal.ValueGet(out valueX, frameStart))
					{
						valueX = 1.0f;
					}
					if(false == attributeScaleYLocal.ValueGet(out valueY, frameStart))
					{
						valueY = 1.0f;
					}
					valueXPrevious = valueX;
					valueYPrevious = valueY;

					keyframeX = new Keyframe();
					keyframeY = new Keyframe();

					float time = AssetCreateDataTimeGetFrame(0, framePerSecond);

					AssetCreateDataKeyFrameInitialize(ref keyframeX);
					keyframeX.time = time;
					keyframeX.value = valueX;
					animationCurveX.AddKey(keyframeX);

					AssetCreateDataKeyFrameInitialize(ref keyframeY);
					keyframeY.time = time;
					keyframeY.value = valueY;
					animationCurveY.AddKey(keyframeY);

					int frame;
					for(int i=1; i<countFrameRange; i++)
					{
						frame = i + frameStart;

						if(false == attributeScaleXLocal.ValueGet(out valueX, frame))
						{
							valueX = 1.0f;
						}
						if(false == attributeScaleYLocal.ValueGet(out valueY, frame))
						{
							valueY = 1.0f;
						}

						time = AssetCreateDataTimeGetFrame(i, framePerSecond);

						if(valueXPrevious != valueX)
						{
							AssetCreateDataKeyFrameInitialize(ref keyframeX);
							keyframeX.time = time;
							keyframeX.value = valueX;
							animationCurveX.AddKey(keyframeX);

							valueXPrevious = valueX;
						}

						if(valueYPrevious != valueY)
						{
							AssetCreateDataKeyFrameInitialize(ref keyframeY);
							keyframeY.time = time;
							keyframeY.value = valueY;
							animationCurveY.AddKey(keyframeY);

							valueYPrevious = valueY;
						}
					}

					AssetCreateCurveSetTangent(animationCurveX);
					animationClip.SetCurve(namePathGameObject, type, "material." + NameShaderPropertyCellPivotLocalScale + ".z", animationCurveX);
					AssetCreateCurveSetTangent(animationCurveY);
					animationClip.SetCurve(namePathGameObject, type, "material." + NameShaderPropertyCellPivotLocalScale + ".w", animationCurveY);

					return(true);
				}

				private static bool AssetCreateDataCurveSetUserData(	AnimationClip animationClip,
																		List<AnimationEvent> listAnimationEvent,
																		Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeUserData attributeUserData,
																		int frameStart,
																		int frameEnd,
																		int framePerSecond
																	)
				{
					if(0 >= attributeUserData.CountGetKey())
					{
						return(true);
					}
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(1 > countFrameRange)
					{
						return(true);
					}

					AnimationEvent animationEvent = new AnimationEvent();

					int countAttribute = attributeUserData.CountGetKey();
					int frame;
					for(int i=0; i<countAttribute; i++)
					{
						frame = attributeUserData.ListKey[i].Frame;
						if((frameStart <= frame) && (frameEnd >= frame))
						{
							frame -= frameStart;

							if(true == attributeUserData.ListKey[i].Value.IsNumber)
							{
								animationEvent.time = AssetCreateDataTimeGetFrame(frame, framePerSecond);
								animationEvent.intParameter = attributeUserData.ListKey[i].Value.NumberInt;
								animationEvent.functionName = "FunctionEventInt";

								listAnimationEvent.Add(animationEvent);
							}
							if(true == attributeUserData.ListKey[i].Value.IsText)
							{
								animationEvent.time = AssetCreateDataTimeGetFrame(frame, framePerSecond);
								animationEvent.stringParameter = string.Copy(attributeUserData.ListKey[i].Value.Text);
								animationEvent.functionName = "FunctionEventText";

								listAnimationEvent.Add(animationEvent);
							}
						}
					}

					return(true);
				}
#if UNITY_2017_1_OR_NEWER
				private static bool AssetCreateDataCurveSetPowerMask(	AnimationClip animationClip,
																		string namePathGameObject,
																		SpriteMask spriteMask,
																		Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat attributePowerMask,
																		int indexAnimation,
																		int frameStart,
																		int frameEnd,
																		int framePerSecond,
																		float valueError
																	)
				{
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(1 > countFrameRange)
					{
						return(true);
					}

					AnimationCurve animationCurve = new AnimationCurve();
					Keyframe keyframe;
					float value;
					float valueMask;

					if(0 >= attributePowerMask.CountGetKey())
					{
						valueMask = 0.0001f;

						spriteMask.alphaCutoff = valueMask;

						keyframe = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframe);
						keyframe.time = AssetCreateDataTimeGetFrame(0, framePerSecond);
						keyframe.value = valueMask;
						animationCurve.AddKey(keyframe);

						goto AssetCreateDataCurveSetPowerMask_End;
					}

					float valuePrevious = float.NaN;
					int frame;
					for(int i=0; i<countFrameRange; i++)
					{
						frame = i + frameStart;

						if(false == attributePowerMask.ValueGet(out value, frame))
						{
							value = valueError;
						}
						/* MEMO: Must set before adding Curve to AnimationClip.       */
						/*       (If set later, animation is not reflected correctly) */
						if((0 == i) && (0 == indexAnimation))
						{
							spriteMask.alphaCutoff = value;
						}

						if((0 == i) || (valuePrevious != value))
						{
							keyframe = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframe);
							keyframe.time = AssetCreateDataTimeGetFrame(i, framePerSecond);
							valueMask = (255.0f - Mathf.Floor(value)) * (1.0f / 255.0f);
							if(0.0f == valueMask)
							{
								valueMask = 0.0001f;
							}
							keyframe.value = valueMask;
							animationCurve.AddKey(keyframe);
						}

						valuePrevious = value;
					}

				AssetCreateDataCurveSetPowerMask_End:;
					AssetCreateCurveSetTangent(animationCurve);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteMask), "m_MaskAlphaCutoff", animationCurve);

					return(true);
				}
#else
				/* MEMO: Can not use "SpriteMask" in Unity5.6 or earlier.                               */
				/*       (For "Nintendo Switch" for the time being, corresponds to Unity5.6 or earlier) */
#endif

				private static bool AssetCreateDataCurveSetVertexCorrection(	AnimationClip animationClip,
																				string namePathGameObject,
																				SpriteRenderer spriteRenderer,
																				Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeVertexCorrection attributeVertexCorrection,
																				int indexAnimation,
																				int frameStart,
																				int frameEnd,
																				int framePerSecond
																		)
				{
					int countFrameRange = (frameEnd - frameStart) + 1;
					if(1 > countFrameRange)
					{
						return(true);
					}

					AnimationCurve animationCurveLUx = new AnimationCurve();
					AnimationCurve animationCurveLUy = new AnimationCurve();
					AnimationCurve animationCurveRUx = new AnimationCurve();
					AnimationCurve animationCurveRUy = new AnimationCurve();
					AnimationCurve animationCurveRDx = new AnimationCurve();
					AnimationCurve animationCurveRDy = new AnimationCurve();
					AnimationCurve animationCurveLDx = new AnimationCurve();
					AnimationCurve animationCurveLDy = new AnimationCurve();

					Keyframe keyframeLUx;
					Keyframe keyframeLUy;
					Keyframe keyframeRUx;
					Keyframe keyframeRUy;
					Keyframe keyframeRDx;
					Keyframe keyframeRDy;
					Keyframe keyframeLDx;
					Keyframe keyframeLDy;

					float time = AssetCreateDataTimeGetFrame(0, framePerSecond);

					if(0 >= attributeVertexCorrection.CountGetKey())
					{
						keyframeLUx = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeLUx);
						keyframeLUx.time = time;
						keyframeLUx.value = 0.0f;
						animationCurveLUx.AddKey(keyframeLUx);

						keyframeLUy = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeLUy);
						keyframeLUy.time = time;
						keyframeLUy.value = 0.0f;
						animationCurveLUy.AddKey(keyframeLUy);

						keyframeRUx = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeRUx);
						keyframeRUx.time = time;
						keyframeRUx.value = 0.0f;
						animationCurveRUx.AddKey(keyframeRUx);

						keyframeRUy = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeRUy);
						keyframeRUy.time = time;
						keyframeRUy.value = 0.0f;
						animationCurveRUy.AddKey(keyframeRUy);

						keyframeRDx = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeRDx);
						keyframeRDx.time = time;
						keyframeRDx.value = 0.0f;
						animationCurveRDx.AddKey(keyframeRDx);

						keyframeRDy = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeRDy);
						keyframeRDy.time = time;
						keyframeRDy.value = 0.0f;
						animationCurveRDy.AddKey(keyframeRDy);

						keyframeLDx = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeLDx);
						keyframeLDx.time = time;
						keyframeLDx.value = 0.0f;
						animationCurveLDx.AddKey(keyframeLDx);

						keyframeLDy = new Keyframe();
						AssetCreateDataKeyFrameInitialize(ref keyframeLDy);
						keyframeLDy.time = time;
						keyframeLDy.value = 0.0f;
						animationCurveLDy.AddKey(keyframeLDy);

						goto AssetCreateDataCurveSetVertexCorrection_End;
					}

					Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection value = new Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection();
					Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection valuePrevious = new Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection();
					valuePrevious.BootUp();
					for(int i=0; i<(int)Library_SpriteStudio6.KindVertex.TERMINATOR2; i++)
					{
						valuePrevious.Coordinate[i].x = float.NaN;
						valuePrevious.Coordinate[i].y = float.NaN;
					}
					int frame;
					float valueCoordinate;
					for(int i=0; i<countFrameRange; i++)
					{
						frame = i + frameStart;

						if(false == attributeVertexCorrection.ValueGet(out value, frame))
						{
							value = Library_SpriteStudio6.Data.Animation.Attribute.DefaultVertexCorrection;
						}

						time = AssetCreateDataTimeGetFrame(i, framePerSecond);

						valueCoordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU].x;
						if((0 == i) || (valuePrevious.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU].x != valueCoordinate))
						{
							keyframeLUx = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframeLUx);
							keyframeLUx.time = time;
							keyframeLUx.value = valueCoordinate;
							animationCurveLUx.AddKey(keyframeLUx);
						}

						valueCoordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU].y;
						if((0 == i) || (valuePrevious.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU].y != valueCoordinate))
						{
							keyframeLUy = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframeLUy);
							keyframeLUy.time = time;
							keyframeLUy.value = valueCoordinate;
							animationCurveLUy.AddKey(keyframeLUy);
						}

						valueCoordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU].x;
						if((0 == i) || (valuePrevious.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU].x != valueCoordinate))
						{
							keyframeRUx = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframeRUx);
							keyframeRUx.time = time;
							keyframeRUx.value = valueCoordinate;
							animationCurveRUx.AddKey(keyframeRUx);
						}

						valueCoordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU].y;
						if((0 == i) || (valuePrevious.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU].y != valueCoordinate))
						{
							keyframeRUy = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframeRUy);
							keyframeRUy.time = time;
							keyframeRUy.value = valueCoordinate;
							animationCurveRUy.AddKey(keyframeRUy);
						}

						valueCoordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD].x;
						if((0 == i) || (valuePrevious.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD].x != valueCoordinate))
						{
							keyframeRDx = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframeRDx);
							keyframeRDx.time = time;
							keyframeRDx.value = valueCoordinate;
							animationCurveRDx.AddKey(keyframeRDx);
						}

						valueCoordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD].y;
						if((0 == i) || (valuePrevious.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD].y != valueCoordinate))
						{
							keyframeRDy = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframeRDy);
							keyframeRDy.time = time;
							keyframeRDy.value = valueCoordinate;
							animationCurveRDy.AddKey(keyframeRDy);
						}

						valueCoordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD].x;
						if((0 == i) || (valuePrevious.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD].x != valueCoordinate))
						{
							keyframeLDx = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframeLDx);
							keyframeLDx.time = time;
							keyframeLDx.value = valueCoordinate;
							animationCurveLDx.AddKey(keyframeLDx);
						}

						valueCoordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD].y;
						if((0 == i) || (valuePrevious.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD].y != valueCoordinate))
						{
							keyframeLDy = new Keyframe();
							AssetCreateDataKeyFrameInitialize(ref keyframeLDy);
							keyframeLDy.time = time;
							keyframeLDy.value = valueCoordinate;
							animationCurveLDy.AddKey(keyframeLDy);
						}

						valuePrevious = value;
					}

				AssetCreateDataCurveSetVertexCorrection_End:;
					AssetCreateCurveSetTangent(animationCurveLUx);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyVertexOffsetLURU + ".x", animationCurveLUx);
					AssetCreateCurveSetTangent(animationCurveLUy);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyVertexOffsetLURU + ".y", animationCurveLUy);
					AssetCreateCurveSetTangent(animationCurveRUx);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyVertexOffsetLURU + ".z", animationCurveRUx);
					AssetCreateCurveSetTangent(animationCurveRUy);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyVertexOffsetLURU + ".w", animationCurveRUy);
					AssetCreateCurveSetTangent(animationCurveRDx);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyVertexOffsetRDLD + ".x", animationCurveRDx);
					AssetCreateCurveSetTangent(animationCurveRDy);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyVertexOffsetRDLD + ".y", animationCurveRDy);
					AssetCreateCurveSetTangent(animationCurveLDx);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyVertexOffsetRDLD + ".z", animationCurveLDx);
					AssetCreateCurveSetTangent(animationCurveLDy);
					animationClip.SetCurve(namePathGameObject, typeof(SpriteRenderer), "material." + NameShaderPropertyVertexOffsetRDLD + ".w", animationCurveLDy);

					return(true);
				}

				public static GameObject ConvertPartsAnimation(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
																LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
																LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE
															)
				{
					const string messageLogPrefix = "Convert Animation-Parts";

					/* Create new GameObject (Root) */
					int countParts = informationSSAE.TableParts.Length;
					GameObject gameObjectRoot = null;
					if(false == ConvertPartsAnimationGameObjectCreate(ref setting, informationSSPJ, informationSSAE, 0))
					{
						LogError(messageLogPrefix, "Failure to get Temporary-GameObject", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						goto ConvertPartsAnimation_ErrorEnd;
					}
					gameObjectRoot = informationSSAE.TableParts[0].GameObjectUnityNative;
					gameObjectRoot.name = string.Copy(informationSSAE.NameFileBody);

					for(int i=1; i<countParts; i++)
					{
						if(false == ConvertPartsAnimationGameObjectCreate(ref setting, informationSSPJ, informationSSAE, i))
						{
							LogError(messageLogPrefix, "Failure to get Temporary-GameObject", informationSSAE.FileNameGetFullPath(), informationSSPJ);
							goto ConvertPartsAnimation_ErrorEnd;
						}
					}

					return(gameObjectRoot);

				ConvertPartsAnimation_ErrorEnd:;
					for(int i=0; i<countParts; i++)
					{
						informationSSAE.TableParts[i].GameObjectUnityNative = null;
					}
					if(null != gameObjectRoot)
					{
						UnityEngine.Object.DestroyImmediate(gameObjectRoot);
						gameObjectRoot = null;
					}
					return(null);
				}
				public static bool ConvertPartsAnimationGameObjectCreate(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
																			LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
																			LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE,
																			int idParts
																		)
				{
					const string messageLogPrefix = "Convert Animation-Parts";
					bool flagNonBatch = ConvertPartsAnimationGameObjectCreateCheckBatch(ref setting,informationSSPJ, informationSSAE, idParts);
					LibraryEditor_SpriteStudio6.Import.SSAE.Information.Parts informationParts = informationSSAE.TableParts[idParts];
					LibraryEditor_SpriteStudio6.Import.SSAE.Information.Parts informationPartsParent = null;

					string name = informationParts.Data.Name;
					string nameTypeParts = "";

					informationParts.GameObjectUnityNative = null;
					informationParts.SpriteRendererUnityNative = null;
#if UNITY_2017_1_OR_NEWER
					informationParts.SpriteMaskUnityNative = null;
#else
					/* MEMO: Can not use "SpriteMask" in Unity5.6 or earlier.                               */
					/*       (For "Nintendo Switch" for the time being, corresponds to Unity5.6 or earlier) */
#endif

					GameObject gameObjectParent = null;
					int idPartsParent = informationParts.Data.IDParent;
					if(0 <= idPartsParent)
					{
						gameObjectParent = informationSSAE.TableParts[idPartsParent].GameObjectUnityNative;
						informationParts.ScriptRootUnityNative = informationSSAE.TableParts[idPartsParent].ScriptRootUnityNative;
					}
					else
					{
						gameObjectParent = null;
						informationParts.ScriptRootUnityNative = null;
					}

					switch(informationParts.Data.Feature)
					{
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.ROOT:
							goto ConvertPartsAnimationGameObjectCreate_RootCreate;

						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NULL:
							goto ConvertPartsAnimationGameObjectCreate_NULLCreate;

//						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE2:
//						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE4:
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL:
							goto ConvertPartsAnimationGameObjectCreate_SpriteCreate;

						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE:
							nameTypeParts = "Instance";
							goto ConvertPartsAnimationGameObjectCreate_NULLCreate;

						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT:
							nameTypeParts = "Effect";
							goto ConvertPartsAnimationGameObjectCreate_NULLCreate;

//						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE2:
//						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE4:
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK:
							goto ConvertPartsAnimationGameObjectCreate_MaskCreate;

						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.JOINT:
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONE:
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MOVENODE:
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.CONSTRAINT:
						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONEPOINT:
							goto ConvertPartsAnimationGameObjectCreate_NULLCreate;

						case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH:
							goto ConvertPartsAnimationGameObjectCreate_Mesh;

						default:
							break;
					}

				ConvertPartsAnimationGameObjectCreate_ErrorEnd:;
					if(null != informationParts.SkinnedMeshRendererUnityNative)
					{
						informationParts.SkinnedMeshRendererUnityNative = null;
					}
					if(null != informationParts.ScriptPartsUnityNative)
					{
						informationParts.ScriptPartsUnityNative = null;
					}
#if UNITY_2017_1_OR_NEWER
					if(null != informationParts.SpriteMaskUnityNative)
					{
						informationParts.SpriteMaskUnityNative = null;
					}
#else
					/* MEMO: Can not use "SpriteMask" in Unity5.6 or earlier.                               */
					/*       (For "Nintendo Switch" for the time being, corresponds to Unity5.6 or earlier) */
#endif
					if(null != informationParts.SpriteRendererUnityNative)
					{
						informationParts.SpriteRendererUnityNative = null;
					}
					if(null != informationParts.GameObjectUnityNative)
					{
						UnityEngine.Object.DestroyImmediate(informationParts.GameObjectUnityNative);
						informationParts.GameObjectUnityNative = null;
					}
					return(false);

				ConvertPartsAnimationGameObjectCreate_End:;
					/* Create GameObject's path */
					string nameGameObject = "";
					idPartsParent = informationParts.Data.IDParent;
					informationPartsParent = informationParts;
					while(0 <= idPartsParent)
					{	/* MEMO:  */
						if(true == string.IsNullOrEmpty(nameGameObject))
						{
							nameGameObject = informationPartsParent.GameObjectUnityNative.name;
						}
						else
						{
							nameGameObject = informationPartsParent.GameObjectUnityNative.name + "/" + nameGameObject;
						}
						informationPartsParent = informationSSAE.TableParts[idPartsParent];
						idPartsParent = informationSSAE.TableParts[idPartsParent].Data.IDParent;
					}
					informationParts.NameGameObjectUnityNative = string.Copy(nameGameObject);

					return(true);

				ConvertPartsAnimationGameObjectCreate_RootCreate:;
					informationParts.GameObjectUnityNative = Library_SpriteStudio6.Utility.Asset.GameObjectCreate(name, false, gameObjectParent);
					if(null == informationParts.GameObjectUnityNative)
					{
						goto ConvertPartsAnimationGameObjectCreate_ErrorEnd;
					}

					informationParts.ScriptRootUnityNative = informationParts.GameObjectUnityNative.AddComponent<Script_SpriteStudio6_RootUnityNative>();
					if(null == informationParts.ScriptRootUnityNative)
					{
						goto ConvertPartsAnimationGameObjectCreate_ErrorEnd;
					}

					informationParts.GameObjectUnityNative.SetActive(true);
					goto ConvertPartsAnimationGameObjectCreate_End;

				ConvertPartsAnimationGameObjectCreate_NULLCreate:;
					if(false == string.IsNullOrEmpty(nameTypeParts))
					{
						LogWarning(messageLogPrefix, "Unsupported Parts-Type \"" + nameTypeParts + "\" Parts[" + idParts.ToString() + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
					}

					informationParts.GameObjectUnityNative = Library_SpriteStudio6.Utility.Asset.GameObjectCreate(name, false, gameObjectParent);
					if(null == informationParts.GameObjectUnityNative)
					{
						goto ConvertPartsAnimationGameObjectCreate_ErrorEnd;
					}

					informationParts.GameObjectUnityNative.SetActive(true);
					goto ConvertPartsAnimationGameObjectCreate_End;

				ConvertPartsAnimationGameObjectCreate_SpriteCreate:;
					informationParts.GameObjectUnityNative = Library_SpriteStudio6.Utility.Asset.GameObjectCreate(name, false, gameObjectParent);
					if(null == informationParts.GameObjectUnityNative)
					{
						goto ConvertPartsAnimationGameObjectCreate_ErrorEnd;
					}

					informationParts.SpriteRendererUnityNative = informationParts.GameObjectUnityNative.AddComponent<SpriteRenderer>();
					if(null == informationParts.SpriteRendererUnityNative)
					{
						goto ConvertPartsAnimationGameObjectCreate_ErrorEnd;
					}
					switch(informationParts.Data.OperationBlendTarget)
					{
						case Library_SpriteStudio6.KindOperationBlend.MIX:
							informationParts.SpriteRendererUnityNative.material = (false == flagNonBatch)
																					? setting.PresetMaterial.AnimationUnityNativeMix
																					: setting.PresetMaterial.AnimationUnityNativeNonBatchMix;
							break;
						case Library_SpriteStudio6.KindOperationBlend.ADD:
							informationParts.SpriteRendererUnityNative.material = (false == flagNonBatch)
																					? setting.PresetMaterial.AnimationUnityNativeAdd
																					: setting.PresetMaterial.AnimationUnityNativeNonBatchAdd;
							break;
						case Library_SpriteStudio6.KindOperationBlend.SUB:
							informationParts.SpriteRendererUnityNative.material = (false == flagNonBatch)
																					? setting.PresetMaterial.AnimationUnityNativeSub
																					: setting.PresetMaterial.AnimationUnityNativeNonBatchSub;
							break;
						case Library_SpriteStudio6.KindOperationBlend.MUL:
							informationParts.SpriteRendererUnityNative.material = (false == flagNonBatch)
																					? setting.PresetMaterial.AnimationUnityNativeMul
																					: setting.PresetMaterial.AnimationUnityNativeNonBatchMul;
							break;
						case Library_SpriteStudio6.KindOperationBlend.MUL_NA:
							informationParts.SpriteRendererUnityNative.material = (false == flagNonBatch)
																					? setting.PresetMaterial.AnimationUnityNativeMulNA
																					: setting.PresetMaterial.AnimationUnityNativeNonBatchMulNA;
							break;
						case Library_SpriteStudio6.KindOperationBlend.SCR:
							informationParts.SpriteRendererUnityNative.material = (false == flagNonBatch)
																					? setting.PresetMaterial.AnimationUnityNativeScr
																					: setting.PresetMaterial.AnimationUnityNativeNonBatchScr;
							break;
						case Library_SpriteStudio6.KindOperationBlend.EXC:
							informationParts.SpriteRendererUnityNative.material = (false == flagNonBatch)
																					? setting.PresetMaterial.AnimationUnityNativeExc
																					: setting.PresetMaterial.AnimationUnityNativeNonBatchExc;
							break;
						case Library_SpriteStudio6.KindOperationBlend.INV:
							informationParts.SpriteRendererUnityNative.material = (false == flagNonBatch)
																					? setting.PresetMaterial.AnimationUnityNativeInv
																					: setting.PresetMaterial.AnimationUnityNativeNonBatchInv;
							break;
					}
#if UNITY_2017_1_OR_NEWER
					if(true == informationParts.FlagMasking)
					{
						informationParts.SpriteRendererUnityNative.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
					}
					else
					{
						informationParts.SpriteRendererUnityNative.maskInteraction = SpriteMaskInteraction.None;
					}
#else
					/* MEMO: Can not use "SpriteMask" in Unity5.6 or earlier.                               */
					/*       (For "Nintendo Switch" for the time being, corresponds to Unity5.6 or earlier) */
#endif

					informationParts.ScriptPartsUnityNative = informationParts.GameObjectUnityNative.AddComponent<Script_SpriteStudio6_PartsUnityNative>();
					if(null == informationParts.ScriptPartsUnityNative)
					{
						goto ConvertPartsAnimationGameObjectCreate_ErrorEnd;
					}
					informationParts.ScriptPartsUnityNative.PartsRoot = informationParts.ScriptRootUnityNative;
//					informationParts.ScriptPartsUnityNative.Cell = 
					informationParts.ScriptPartsUnityNative.CellMesh = null;	/* Disuse */
					informationParts.ScriptPartsUnityNative.TextureMesh = null;	/* Disuse */

					informationParts.GameObjectUnityNative.SetActive(true);
					goto ConvertPartsAnimationGameObjectCreate_End;

				ConvertPartsAnimationGameObjectCreate_MaskCreate:;
#if UNITY_2017_1_OR_NEWER
					informationParts.GameObjectUnityNative = Library_SpriteStudio6.Utility.Asset.GameObjectCreate(name, false, gameObjectParent);
					if(null == informationParts.GameObjectUnityNative)
					{
						goto ConvertPartsAnimationGameObjectCreate_ErrorEnd;
					}

					informationParts.SpriteMaskUnityNative = informationParts.GameObjectUnityNative.AddComponent<SpriteMask>();
					if(null == informationParts.SpriteMaskUnityNative)
					{
						goto ConvertPartsAnimationGameObjectCreate_ErrorEnd;
					}
					informationParts.SpriteMaskUnityNative.isCustomRangeActive = true;
//					informationParts.SpriteMaskUnityNative.backSortingOrder = -1;

					informationParts.ScriptPartsUnityNative = informationParts.GameObjectUnityNative.AddComponent<Script_SpriteStudio6_PartsUnityNative>();
					if(null == informationParts.ScriptPartsUnityNative)
					{
						goto ConvertPartsAnimationGameObjectCreate_ErrorEnd;
					}
					informationParts.ScriptPartsUnityNative.PartsRoot = informationParts.ScriptRootUnityNative;
//					informationParts.ScriptPartsUnityNative.Cell = 
					informationParts.ScriptPartsUnityNative.CellMesh = null;	/* Disuse */
					informationParts.ScriptPartsUnityNative.TextureMesh = null;	/* Disuse */

					informationParts.GameObjectUnityNative.SetActive(true);
					goto ConvertPartsAnimationGameObjectCreate_End;
#else
					/* MEMO: Can not use "SpriteMask" in Unity5.6 or earlier.                               */
					/*       (For "Nintendo Switch" for the time being, corresponds to Unity5.6 or earlier) */
					goto ConvertPartsAnimationGameObjectCreate_NULLCreate;
#endif

				ConvertPartsAnimationGameObjectCreate_Mesh:;
					informationParts.GameObjectUnityNative = Library_SpriteStudio6.Utility.Asset.GameObjectCreate(name, false, gameObjectParent);
					if(null == informationParts.GameObjectUnityNative)
					{
						goto ConvertPartsAnimationGameObjectCreate_ErrorEnd;
					}

					informationParts.SkinnedMeshRendererUnityNative = informationParts.GameObjectUnityNative.AddComponent<SkinnedMeshRenderer>();
					if(null == informationParts.SkinnedMeshRendererUnityNative)
					{
						goto ConvertPartsAnimationGameObjectCreate_ErrorEnd;
					}
					informationParts.SkinnedMeshRendererUnityNative.quality = SkinQuality.Bone4;
//					informationParts.SkinnedMeshRendererUnityNative.updateWhenOffscreen = true;
//					informationParts.SkinnedMeshRendererUnityNative.skinnedMotionVectors = 
					informationParts.SkinnedMeshRendererUnityNative.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
					informationParts.SkinnedMeshRendererUnityNative.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
					informationParts.SkinnedMeshRendererUnityNative.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
					informationParts.SkinnedMeshRendererUnityNative.receiveShadows = false;
//					informationParts.SkinnedMeshRendererUnityNative.motionVectorGenerationMode = 
					switch(informationParts.Data.OperationBlendTarget)
					{
						case Library_SpriteStudio6.KindOperationBlend.MIX:
							informationParts.SkinnedMeshRendererUnityNative.material = setting.PresetMaterial.SkinnedMeshUnityNativeMix;
							break;
						case Library_SpriteStudio6.KindOperationBlend.ADD:
							informationParts.SkinnedMeshRendererUnityNative.material = setting.PresetMaterial.SkinnedMeshUnityNativeAdd;
							break;
						case Library_SpriteStudio6.KindOperationBlend.SUB:
							informationParts.SkinnedMeshRendererUnityNative.material = setting.PresetMaterial.SkinnedMeshUnityNativeSub;
							break;
						case Library_SpriteStudio6.KindOperationBlend.MUL:
							informationParts.SkinnedMeshRendererUnityNative.material = setting.PresetMaterial.SkinnedMeshUnityNativeMul;
							break;
						case Library_SpriteStudio6.KindOperationBlend.MUL_NA:
							informationParts.SkinnedMeshRendererUnityNative.material = setting.PresetMaterial.SkinnedMeshUnityNativeMulNA;
							break;
						case Library_SpriteStudio6.KindOperationBlend.SCR:
							informationParts.SkinnedMeshRendererUnityNative.material = setting.PresetMaterial.SkinnedMeshUnityNativeScr;
							break;
						case Library_SpriteStudio6.KindOperationBlend.EXC:
							informationParts.SkinnedMeshRendererUnityNative.material = setting.PresetMaterial.SkinnedMeshUnityNativeExc;
							break;
						case Library_SpriteStudio6.KindOperationBlend.INV:
							informationParts.SkinnedMeshRendererUnityNative.material = setting.PresetMaterial.SkinnedMeshUnityNativeInv;
							break;
					}

					informationParts.ScriptPartsUnityNative = informationParts.GameObjectUnityNative.AddComponent<Script_SpriteStudio6_PartsUnityNative>();
					if(null == informationParts.ScriptPartsUnityNative)
					{
						goto ConvertPartsAnimationGameObjectCreate_ErrorEnd;
					}
					informationParts.ScriptPartsUnityNative.PartsRoot = informationParts.ScriptRootUnityNative;
					informationParts.ScriptPartsUnityNative.Cell = null;	/* Disuse */
//					informationParts.ScriptPartsUnityNative.CellMesh = 
//					informationParts.ScriptPartsUnityNative.TextureMesh = 

					informationParts.GameObjectUnityNative.SetActive(true);
					goto ConvertPartsAnimationGameObjectCreate_End;
				}
				private static bool ConvertPartsAnimationGameObjectCreateCheckBatch(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
																						LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
																						LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE,
																						int idParts
																				)
				{
					int countAnimation = informationSSAE.TableAnimation.Length;
					LibraryEditor_SpriteStudio6.Import.SSAE.Information.Animation.Parts informationAnimationParts = null;
					bool flagNonBatch = false;
					int frameStart;
					int frameCount;
					for(int i=0; i<countAnimation; i++)
					{
						informationAnimationParts = informationSSAE.TableAnimation[i].TableParts[idParts];
						frameStart = informationSSAE.TableAnimation[i].Data.FrameValidStart;
						frameCount = informationSSAE.TableAnimation[i].Data.CountFrameValid;

						/* MEMO: Suppress batching when attributes are used that cause inconvenience if sprites are batched. */
						flagNonBatch |= ConvertPartsAnimationGameObjectCreateCheckBatchFloat(informationAnimationParts.ScalingXLocal, frameStart, frameCount, 1.0f);
						flagNonBatch |= ConvertPartsAnimationGameObjectCreateCheckBatchFloat(informationAnimationParts.ScalingYLocal, frameStart, frameCount, 1.0f);

						flagNonBatch |= ConvertPartsAnimationGameObjectCreateCheckBatchFloatInherit(informationAnimationParts.RateOpacity, frameStart, frameCount, 1.0f);
						flagNonBatch |= ConvertPartsAnimationGameObjectCreateCheckBatchFloat(informationAnimationParts.RateOpacityLocal, frameStart, frameCount, 1.0f);

						flagNonBatch |= ConvertPartsAnimationGameObjectCreateCheckBatchVertexCorrection(informationAnimationParts.VertexCorrection, frameStart, frameCount);
						flagNonBatch |= (0 < informationAnimationParts.PartsColor.CountGetKey()) ? true : false;
					}

					return(flagNonBatch);
				}
				private static bool ConvertPartsAnimationGameObjectCreateCheckBatchFloat(	Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat attribute,
																							int frameStart,
																							int frameCount,
																							float valueDefault
																						)
				{
					float valueKey;
					for(int i=frameStart; i<(frameStart + frameCount); i++)
					{
						if(false == attribute.ValueGet(out valueKey, i))
						{	/* No-key */
							return(false);
						}

						if(valueDefault != valueKey)
						{
							return(true);
						}
					}

					return(false);
				}
				private static bool ConvertPartsAnimationGameObjectCreateCheckBatchFloatInherit(	Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat attribute,
																									int frameStart,
																									int frameCount,
																									float valueDefault
																							)
				{
					float valueKey;
					for(int i=frameStart; i<(frameStart + frameCount); i++)
					{
						if(false == Library_SpriteStudio6.Data.Animation.Attribute.Importer.Inheritance.ValueGetFloatMultiple(out valueKey, attribute, i, 1.0f))
						{	/* No-key */
							return(false);
						}

						if(valueDefault != valueKey)
						{
							return(true);
						}
					}

					return(false);
				}
				private static bool ConvertPartsAnimationGameObjectCreateCheckBatchPartsColor(	Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeVertexCorrection attribute,
																								int frameStart,
																								int frameCount
																							)
				{
					return(0 < attribute.CountGetKey());
				}
				private static bool ConvertPartsAnimationGameObjectCreateCheckBatchVertexCorrection(	Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeVertexCorrection attribute,
																										int frameStart,
																										int frameCount
																								)
				{
					Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection valueKey = new Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection();
					int countVertex;
					for(int i=frameStart; i<(frameStart + frameCount); i++)
					{
						if(false == attribute.ValueGet(out valueKey, i))
						{	/* No-key */
							return(false);
						}

						countVertex = valueKey.Coordinate.Length;
						for(int j=0; j<countVertex; j++)
						{
							if((0.0f != valueKey.Coordinate[j].x) || (0.0f != valueKey.Coordinate[j].y))
							{
								return(true);
							}
						}
					}

					return(false);
				}

				public static bool AssetCreateDataMesh(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
														LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
														LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE,
														int idParts
													)
				{
					const string messageLogPrefix = "Create Asset(Skinned-Mesh)";

					LibraryEditor_SpriteStudio6.Import.SSAE.Information.Parts informationParts = informationSSAE.TableParts[idParts];

					if(Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH != informationSSAE.TableParts[idParts].Data.Feature)
					{	/* Not "Mesh" Part */
						return(true);
					}
					int indexBindMesh = -1;
					int countCatalog = informationSSAE.CatalogParts.ListIDPartsMesh.Count;
					for(int i=0; i<countCatalog; i++)
					{
						if(idParts == informationSSAE.CatalogParts.ListIDPartsMesh[i])
						{
							indexBindMesh = i;
							break;
						}
					}
					if(0 > indexBindMesh)
					{
						LogWarning(messageLogPrefix, "Invalid Mesh Parts Parts[" + informationParts.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						return(true);
					}

					Vector2[] tableUV = null;
					Vector3[] tableCoordinate = null;
					BoneWeight[] tableBoneWeight = null;
					List<int> listIndexBone = null;
					int[] tableIndexBone = null;
					Transform[] tableTransformBoneMinimize = null;

					/* Minimize Bone */
					listIndexBone = new List<int>();
					if(null == listIndexBone)
					{
						LogError(messageLogPrefix, "Not Enough Memory (Bone Minimize-List) Parts[" + informationParts.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						goto AssetCreateDataMesh_ErrorEnd;
					}
					listIndexBone.Clear();
					int idPartsJoint = informationParts.Data.IDParent;
					AssetCreateDataMeshMinimizeBone(listIndexBone, informationSSAE, idPartsJoint);
					tableIndexBone = listIndexBone.ToArray();	/* Fix */
					listIndexBone.Clear();
					listIndexBone = null;

					int countTableIDPartsBone = tableIndexBone.Length;
					if(0 >= countTableIDPartsBone)
					{
						/* MEMO: Replace component for mesh parts not assigned bones with "MeshFilter & MeshRenderer". */
						SkinnedMeshRenderer skinnedMeshRenderer = informationParts.SkinnedMeshRendererUnityNative;
						Material material = null;
						if(null != skinnedMeshRenderer)
						{
							material = skinnedMeshRenderer.sharedMaterial;
							UnityEngine.Object.DestroyImmediate(skinnedMeshRenderer);
						}
						informationParts.SkinnedMeshRendererUnityNative = null;

						GameObject gameObject = informationParts.GameObjectUnityNative;
						informationParts.MeshFilterUnityNative = gameObject.AddComponent<MeshFilter>();
						informationParts.MeshRendererUnityNative = gameObject.AddComponent<MeshRenderer>();
						if(null != informationParts.MeshRendererUnityNative)
						{
							informationParts.MeshRendererUnityNative.sharedMaterial = material;
						}
						material = null;
					}
					tableTransformBoneMinimize = new Transform[countTableIDPartsBone];
					if(null == tableTransformBoneMinimize)
					{
						LogError(messageLogPrefix, "Not Enough Memory (Bone Minimize table) Parts[" + informationParts.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						goto AssetCreateDataMesh_ErrorEnd;
					}
					for(int i=0; i<countTableIDPartsBone; i++)
					{
						tableTransformBoneMinimize[i] = informationSSAE.TableTransformBoneUnityNative[tableIndexBone[i]];
					}
					informationParts.ScriptPartsUnityNative.TableTransformBone = tableTransformBoneMinimize;
					tableTransformBoneMinimize = null;

					/* Create Mesh */
					Mesh dataMesh = informationParts.DataMeshSkinnedUnityNative.TableData[0];
					if(null == dataMesh)
					{
						dataMesh = new Mesh();
						AssetDatabase.CreateAsset(dataMesh, informationParts.DataMeshSkinnedUnityNative.TableName[0]);
						informationParts.DataMeshSkinnedUnityNative.TableData[0] = dataMesh;
					}

					int indexCellMap = informationParts.IndexCellMapMeshBind;
					int indexCell = informationParts.IndexCellMeshBind;
					LibraryEditor_SpriteStudio6.Import.SSCE.Information cellMap = null;
					bool flagCellValid = false;
					if((0 <= indexCellMap) && (0 <= indexCell))
					{
						if(informationSSPJ.TableInformationSSCE.Length > indexCellMap)
						{
							cellMap = informationSSPJ.TableInformationSSCE[indexCellMap];
							if(cellMap.TableCell.Length > indexCell)
							{
								if(true == cellMap.TableCell[indexCell].Data.IsMesh)
								{
									flagCellValid = true;
								}
							}
						}
					}
					if(false == flagCellValid)
					{
						LogWarning(messageLogPrefix, "Invalid Cell Parts[" + informationParts.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						goto AssetCreateDataMesh_IgnoreEnd;
					}

					int countBone = informationSSAE.CatalogParts.ListIDPartsBone.Count;
					int countVertexBind = informationSSAE.ListBindMesh.Count;
					int countVertexCell = cellMap.TableCell[indexCell].Data.Mesh.TableCoordinate.Length;

					/* Get UV */
					tableUV = new Vector2[countVertexCell];
					if(null == tableUV)
					{
						LogError(messageLogPrefix, "Not Enough Memory (UV table) Parts[" + informationParts.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						goto AssetCreateDataMesh_ErrorEnd;
					}
					Rect rectangleCell = cellMap.TableCell[indexCell].Data.Rectangle;
					float sizeTextureXInverse = 1.0f / (float)cellMap.SizePixelX;
					float sizeTextureYInverse = 1.0f / (float)cellMap.SizePixelY;
					float positionYInverse = (float)cellMap.SizePixelY - rectangleCell.yMin;
					for(int i=0; i<countVertexCell; i++)
					{
						tableUV[i] = cellMap.TableCell[indexCell].Data.Mesh.TableCoordinate[i];
						tableUV[i].x = rectangleCell.xMin + tableUV[i].x;
						tableUV[i].y = positionYInverse - tableUV[i].y;

						tableUV[i].x *= sizeTextureXInverse;
						tableUV[i].y *= sizeTextureYInverse;
					}

					/* Get Coordinate */
					tableCoordinate = new Vector3[countVertexCell];
					if(null == tableUV)
					{
						LogError(messageLogPrefix, "Not Enough Memory (Coordinate table) Parts[" + informationParts.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						goto AssetCreateDataMesh_ErrorEnd;
					}
					Vector2 pivotCell = cellMap.TableCell[indexCell].Data.Pivot;
					for(int i=0; i<countVertexCell; i++)
					{
						tableCoordinate[i].x = (cellMap.TableCell[indexCell].Data.Mesh.TableCoordinate[i].x - pivotCell.x);
						tableCoordinate[i].y = -(cellMap.TableCell[indexCell].Data.Mesh.TableCoordinate[i].y - pivotCell.y);
						tableCoordinate[i].z = 0.0f;
					}

					/* Get Weight & Bone-Index */
					tableBoneWeight = new BoneWeight[countVertexCell];
					if(null == tableBoneWeight)
					{
						LogError(messageLogPrefix, "Not Enough Memory (Bone-Weight table) Parts[" + informationParts.Data.Name + "]", informationSSAE.FileNameGetFullPath(), informationSSPJ);
						goto AssetCreateDataMesh_ErrorEnd;
					}
					for(int i=0; i<countVertexCell; i++)
					{
						tableBoneWeight[i].boneIndex0 = 0;
						tableBoneWeight[i].boneIndex1 = 0;
						tableBoneWeight[i].boneIndex2 = 0;
						tableBoneWeight[i].boneIndex3 = 0;
						tableBoneWeight[i].weight0 = 1.0f;
						tableBoneWeight[i].weight1 = 0.0f;
						tableBoneWeight[i].weight2 = 0.0f;
						tableBoneWeight[i].weight3 = 0.0f;

						/* MEMO: There may be cases where the mesh is not bound. */
						if(informationSSAE.ListBindMesh[indexBindMesh].ListBindVertex.Count > i)
						{
							AssetCreateDataMeshWeightNormalize(	ref tableBoneWeight[i],
																informationSSAE.ListBindMesh[indexBindMesh].ListBindVertex[i].TableBone,
																tableIndexBone
															);
						}
					}

					/* Fix Mesh */
					dataMesh.Clear();
					dataMesh.vertices = tableCoordinate;
					dataMesh.uv = tableUV;
					dataMesh.triangles = cellMap.TableCell[indexCell].Data.Mesh.TableIndexVertex;
					dataMesh.bindposes = null;
					dataMesh.boneWeights = (0 < countTableIDPartsBone) ? tableBoneWeight : null;

					tableBoneWeight = null;
					tableCoordinate = null;
					tableUV = null;

					EditorUtility.SetDirty(dataMesh);
					AssetDatabase.SaveAssets();

					return(true);

				AssetCreateDataMesh_IgnoreEnd:;
					informationParts.DataMeshSkinnedUnityNative.TableData[0] = null;
					return(true);

				AssetCreateDataMesh_ErrorEnd:;
					if(null != tableTransformBoneMinimize)
					{
						tableTransformBoneMinimize = null;
					}
					if(null != tableIndexBone)
					{
						tableIndexBone = null;
					}
					if(null != listIndexBone)
					{
						listIndexBone.Clear();
						listIndexBone = null;
					}
					if(null != tableBoneWeight)
					{
						tableBoneWeight = null;
					}
					if(null != tableCoordinate)
					{
						tableCoordinate = null;
					}
					if(null != tableUV)
					{
						tableUV = null;
					}

					return(false);
				}
				public static bool AssetCreateDataMeshMinimizeBone(	List<int> listIndexBone,
																	LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE,
																	int idParts
																)
				{
					Information.Parts informationParts = informationSSAE.TableParts[idParts];
					int[] tableIDPartsChild = informationParts.Data.TableIDChild;
					if(null == tableIDPartsChild)
					{
						return(true);
					}
					int countIDPartsChild = tableIDPartsChild.Length;
					if(0 >= countIDPartsChild)
					{
						return(true);
					}
					int[] tableIDPartsBone = informationSSAE.TableIDPartsBoneUnityNative;
					if(null == tableIDPartsBone)
					{
						return(true);
					}
					int countTableIDPartsBone = tableIDPartsBone.Length;
					if(0 >= countIDPartsChild)
					{
						return(true);
					}

					for(int i=0; i<countIDPartsChild; i++)
					{
						for(int j=0; j<countTableIDPartsBone; j++)
						{
							/* MEMO: When not exist in "Bone" list, part is not considered "Bone". */
							/* MEMO: Child parts of parts that are not "Bone" do not affect this "Mesh" part. */
							if(tableIDPartsBone[j] == tableIDPartsChild[i])
							{
								if(false == listIndexBone.Contains(j))
								{
									listIndexBone.Add(j);
									AssetCreateDataMeshMinimizeBone(listIndexBone, informationSSAE, tableIDPartsBone[j]);
								}
								break;	/* j-loop */
							}
						}
					}
					return(true);
				}
				public static bool AssetCreateDataMeshWeightNormalize(	ref BoneWeight boneWeight,
																		Library_SpriteStudio6.Data.Parts.Animation.BindMesh.Vertex.Bone[] tableBindMeshBone,
																		int[] tableIndexBoneUse
																	)
				{
//					const int limitBone = 4;
					int countBone = tableBindMeshBone.Length;
					if(0 == countBone)
					{
						return(true);
					}

					/* MEMO: Only top 4 are extracted since SpriteStudio6's vertex has far more bones affecting. */
					int[] tableBoneIndex = new int[countBone];
					float[] tableWeight = new float[countBone];
					for(int i=0; i<countBone; i++)
					{
						tableBoneIndex[i] = tableBindMeshBone[i].Index;
						tableWeight[i] = tableBindMeshBone[i].Weight;
					}
					for(int i=0; i<(countBone - 1); i++)
					{
						for(int j=(countBone - 1); j>i; j--)
						{
							if(tableWeight[j] > tableWeight[j - 1])
							{
								int tempInt = tableBoneIndex[j];
								float tempFloat = tableWeight[j];
								tableBoneIndex[j] = tableBoneIndex[j - 1];
								tableWeight[j] = tableWeight[j - 1];
								tableBoneIndex[j - 1] = tempInt;
								tableWeight[j - 1] = tempFloat;
							}
						}
					}

					float sumWeight = 0.0f;
					if(0 < countBone)
					{
						boneWeight.boneIndex0 = tableBoneIndex[0];
						boneWeight.weight0 = tableWeight[0];
						sumWeight += boneWeight.weight0;
					}
					if(1 < countBone)
					{
						boneWeight.boneIndex1 = tableBoneIndex[1];
						boneWeight.weight1 = tableWeight[1];
						sumWeight += boneWeight.weight1;
					}
					if(2 < countBone)
					{
						boneWeight.boneIndex2 = tableBoneIndex[2];
						boneWeight.weight2 = tableWeight[2];
						sumWeight += boneWeight.weight2;
					}
					if(3 < countBone)
					{
						boneWeight.boneIndex3 = tableBoneIndex[3];
						boneWeight.weight3 = tableWeight[3];
						sumWeight += boneWeight.weight3;
					}

					/* Normalize Weight */
					if(0.0f < sumWeight)
					{
						float sumWeightInverse = 1.0f / sumWeight;
						boneWeight.weight0 *= sumWeightInverse;
						boneWeight.weight1 *= sumWeightInverse;
						boneWeight.weight2 *= sumWeightInverse;
						boneWeight.weight3 *= sumWeightInverse;
					}

					/* Replace Bone index */
					if(0 < tableIndexBoneUse.Length)
					{
						int indexBone;
						indexBone = AssetCreateDataMeshWeightNormalizeReplaceBoneIndex(boneWeight.boneIndex0, tableIndexBoneUse);
						if(0 > indexBone)
						{	/* Error */
							boneWeight.boneIndex0 = 0;
						}
						else
						{
							boneWeight.boneIndex0 = indexBone;
						}
						indexBone = AssetCreateDataMeshWeightNormalizeReplaceBoneIndex(boneWeight.boneIndex1, tableIndexBoneUse);
						if(0 > indexBone)
						{	/* Error */
							boneWeight.boneIndex1 = 0;
						}
						else
						{
							boneWeight.boneIndex1 = indexBone;
						}
						indexBone = AssetCreateDataMeshWeightNormalizeReplaceBoneIndex(boneWeight.boneIndex2, tableIndexBoneUse);
						if(0 > indexBone)
						{	/* Error */
							boneWeight.boneIndex2 = 0;
						}
						else
						{
							boneWeight.boneIndex2 = indexBone;
						}
						indexBone = AssetCreateDataMeshWeightNormalizeReplaceBoneIndex(boneWeight.boneIndex3, tableIndexBoneUse);
						if(0 > indexBone)
						{	/* Error */
							boneWeight.boneIndex3 = 0;
						}
						else
						{
							boneWeight.boneIndex3 = indexBone;
						}
					}

					return(true);
				}
				private static int AssetCreateDataMeshWeightNormalizeReplaceBoneIndex(int indexBone, int[] tableIndexBoneUse)
				{
					int countTable = tableIndexBoneUse.Length;
					for(int i=0; i<countTable; i++)
					{
						if(tableIndexBoneUse[i] == indexBone)
						{
							return(i);
						}
					}
					return(-1);
				}

				public static bool AssetCreatePrefab(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
														LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
														LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE,
														GameObject gameObjectRoot
													)
				{
					const string messageLogPrefix = "Create Asset(Prefab-Animation)";

					/* Create? Update? */
					if(null == informationSSAE.PrefabAnimationUnityNative.TableData[0])
					{	/* New */
#if UNITY_2018_4_OR_NEWER || UNITY_2019_1_OR_NEWER
						/* MEMO: Process nothing, now. */
#else
						informationSSAE.PrefabAnimationUnityNative.TableData[0] = PrefabUtility.CreateEmptyPrefab(informationSSAE.PrefabAnimationUnityNative.TableName[0]);
						if(null == informationSSAE.PrefabAnimationUnityNative.TableData[0])
						{
							LogError(messageLogPrefix, "Failure to create Prefab", informationSSAE.FileNameGetFullPath(), informationSSPJ);
							goto AssetCreatePrefab_ErrorEnd;
						}
#endif
					}
#if UNITY_2018_4_OR_NEWER || UNITY_2019_1_OR_NEWER
					else
					{
						informationSSAE.PrefabAnimationUnityNative.TableName[0] = AssetDatabase.GetAssetPath((GameObject)informationSSAE.PrefabAnimationUnityNative.TableData[0]);
					}
#else
#endif


					/* Set Root Information */
					Script_SpriteStudio6_RootUnityNative scriptRoot = gameObjectRoot.GetComponent<Script_SpriteStudio6_RootUnityNative>();
					if(null != scriptRoot)
					{
						/* Set BonePoint Transform */
						int countBonePoint = informationSSAE.CatalogParts.ListIDPartsBonePoint.Count;
						scriptRoot.TableTransformBonePoint = new Transform[countBonePoint];
						if(null == scriptRoot.TableTransformBonePoint)
						{
							LogError(messageLogPrefix, "Not Enough Memory (Bone-Point Table)", informationSSAE.FileNameGetFullPath(), informationSSPJ);
							goto AssetCreatePrefab_ErrorEnd;
						}
						int indexBonePoint;
						for(int i=0; i<countBonePoint; i++)
						{
							indexBonePoint = informationSSAE.CatalogParts.ListIDPartsBonePoint[i];
							scriptRoot.TableTransformBonePoint[i] = informationSSAE.TableParts[indexBonePoint].GameObjectUnityNative.transform;
						}

						scriptRoot.CountDrawPartsMax = informationSSAE.CountDrawPartsMaxUnityNative;
						scriptRoot.SortingOffsetPartsDraw = 1;
					}

					/* Attach Animation Holder */
					if(true == setting.Basic.FlagCreateHolderAsset)
					{
						Script_SpriteStudio6_HolderAssetUnityNative scriptHolderAsset = gameObjectRoot.AddComponent<Script_SpriteStudio6_HolderAssetUnityNative>();
						if(null == scriptHolderAsset)
						{
							LogError(messageLogPrefix, "Failure to attach Asset-Holder", informationSSAE.FileNameGetFullPath(), informationSSPJ);
							goto AssetCreatePrefab_ErrorEnd;
						}

						int countAnimationClip = informationSSAE.DataAnimationUnityNative.TableData.Length;
						scriptHolderAsset.TableAnimationClip = new AnimationClip[countAnimationClip];
						for(int i=0; i<countAnimationClip; i++)
						{
							scriptHolderAsset.TableAnimationClip[i] = informationSSAE.DataAnimationUnityNative.TableData[i];
						}
					}

					/* Fixing Parts' data */
					int countParts = informationSSAE.TableParts.Length;
					Script_SpriteStudio6_PartsUnityNative scriptParts = null;
					Information.Parts informationParts = null;
					for(int i=0; i<countParts; i++)
					{
						informationParts = informationSSAE.TableParts[i];
						scriptParts = informationParts.ScriptPartsUnityNative;
						if(null != scriptParts)
						{
							scriptParts.InstanceSpriteRenderer = informationParts.SpriteRendererUnityNative;
#if UNITY_2017_1_OR_NEWER
							scriptParts.InstanceSpriteMask = informationParts.SpriteMaskUnityNative;
#else
							/* MEMO: Can not use "SpriteMask" in Unity5.6 or earlier.                               */
							/*       (For "Nintendo Switch" for the time being, corresponds to Unity5.6 or earlier) */
#endif
							scriptParts.InstanceSkinnedMeshRenderer = informationParts.SkinnedMeshRendererUnityNative;
							scriptParts.InstanceMeshRenderer = informationParts.MeshRendererUnityNative;
							scriptParts.InstanceMeshFilter = informationParts.MeshFilterUnityNative;
						}
					}

					/* Fixing Prefab */
#if UNITY_2018_4_OR_NEWER || UNITY_2019_1_OR_NEWER
					informationSSAE.PrefabAnimationUnityNative.TableData[0] = PrefabUtility.SaveAsPrefabAsset(gameObjectRoot, informationSSAE.PrefabAnimationUnityNative.TableName[0]);
#else
					informationSSAE.PrefabAnimationUnityNative.TableData[0] = PrefabUtility.ReplacePrefab(	gameObjectRoot,
																											informationSSAE.PrefabAnimationUnityNative.TableData[0],
																											LibraryEditor_SpriteStudio6.Import.OptionPrefabReplace
																										);
#endif
					AssetDatabase.SaveAssets();

					/* Destroy Temporary */
					UnityEngine.Object.DestroyImmediate(gameObjectRoot);

					/* Create Control-Object */
					if(true == setting.Basic.FlagCreateControlGameObject)
					{
						/* Control-Object Create */
						GameObject gameObjectControl = Library_SpriteStudio6.Utility.Asset.GameObjectCreate(informationSSAE.NameGameObjectAnimationControlUnityNative, false, null);
						if(null == gameObjectControl)
						{
							LogError(messageLogPrefix, "Failure to get Temporary-GameObject (Control)", informationSSAE.FileNameGetFullPath(), informationSSPJ);
							goto AssetCreatePrefab_ErrorEnd;
						}

						/* Attach Script & Link Prefab */
						Script_SpriteStudio6_ControlPrefab scriptControlPrefab = gameObjectControl.AddComponent<Script_SpriteStudio6_ControlPrefab>();
						scriptControlPrefab.PrefabAnimation = informationSSAE.PrefabAnimationUnityNative.TableData[0];

						/* Create Prefab */
						gameObjectControl.SetActive(true);

#if UNITY_2018_4_OR_NEWER || UNITY_2019_1_OR_NEWER
						UnityEngine.Object prefabControl = informationSSAE.PrefabControlAnimationUnityNative.TableData[0];
						if(null == prefabControl)
						{
							/* MEMO: Process nothing, now. */
						}
						else
						{
							informationSSAE.PrefabControlAnimationUnityNative.TableName[0] = AssetDatabase.GetAssetPath(prefabControl);
						}
						informationSSAE.PrefabControlAnimationUnityNative.TableData[0] = PrefabUtility.SaveAsPrefabAsset(gameObjectControl, informationSSAE.PrefabControlAnimationUnityNative.TableName[0]);
#else
						UnityEngine.Object prefabControl = informationSSAE.PrefabControlAnimationUnityNative.TableData[0];
						if(null == prefabControl)
						{
							prefabControl = PrefabUtility.CreateEmptyPrefab(informationSSAE.PrefabControlAnimationUnityNative.TableName[0]);
						}
						PrefabUtility.ReplacePrefab(	gameObjectControl,
														prefabControl,
														LibraryEditor_SpriteStudio6.Import.OptionPrefabReplace
													);
#endif
						AssetDatabase.SaveAssets();

						/* Destroy Temporary */
						UnityEngine.Object.DestroyImmediate(gameObjectControl);
						gameObjectControl = null;
					}

					return(true);

				AssetCreatePrefab_ErrorEnd:;
					if(null != gameObjectRoot)
					{
						UnityEngine.Object.DestroyImmediate(gameObjectRoot);
					}
					return(false);
				}

				public static bool CreateBoneInformation(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
															LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
															LibraryEditor_SpriteStudio6.Import.SSAE.Information informationSSAE
														)
				{
					const string messageLogPrefix = "Create Asset(Bone-Information)";

					int countBone = informationSSAE.ListBone.Count;
					Transform[] tableTransform = null;
					int[] tableIDParts = null;
					if(0 <= countBone)
					{
						if(null != informationSSAE.ListBone)
						{
							tableTransform = new Transform[countBone];
							if(null == tableTransform)
							{
								LogError(messageLogPrefix, "Not Enough Memory (Transform table)", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto CreateBoneInformation_ErrorEnd;
							}

							tableIDParts = new int[countBone];
							if(null == tableTransform)
							{
								LogError(messageLogPrefix, "Not Enough Memory (Index table)", informationSSAE.FileNameGetFullPath(), informationSSPJ);
								goto CreateBoneInformation_ErrorEnd;
							}

							GameObject gameObjectBone = null;
							int indexParts;
							for(int i=0; i<countBone; i++)
							{
								tableTransform[i] = null;
								tableIDParts[i] = -1;

								indexParts = informationSSAE.IndexGetParts(informationSSAE.ListBone[i]);
								if(0 > indexParts)
								{
									continue;
								}

								gameObjectBone = informationSSAE.TableParts[indexParts].GameObjectUnityNative;
								if(null == gameObjectBone)
								{
									continue;
								}

								tableTransform[i] = gameObjectBone.transform;
								tableIDParts[i] = indexParts;
							}
						}
					}

					informationSSAE.TableTransformBoneUnityNative = tableTransform;
					informationSSAE.TableIDPartsBoneUnityNative = tableIDParts;

					return(true);

				CreateBoneInformation_ErrorEnd:;
					tableTransform = null;
					tableIDParts = null;

					return(false);
				}
#endif
				#endregion Functions

				/* ----------------------------------------------- Enums & Constants */
				#region Enums & Constants
#if !_FOR_SS6CONVERTER_
				private const string NameShaderPropertyBlendParam = "_BlendParam";
				private const string NameShaderPropertyPartsColorLU = "_PartsColor_LU";
				private const string NameShaderPropertyPartsColorRU = "_PartsColor_RU";
				private const string NameShaderPropertyPartsColorRD = "_PartsColor_RD";
				private const string NameShaderPropertyPartsColorLD = "_PartsColor_LD";
				private const string NameShaderPropertyPartsColorOpacity = "_PartsColor_Opacity";
				private const string NameShaderPropertyVertexOffsetLURU = "_VertexOffset_LURU";
				private const string NameShaderPropertyVertexOffsetRDLD = "_VertexOffset_RDLD";
				private const string NameShaderPropertyCellPivotLocalScale = "_CellPivot_LocalScale";
				private const string NameShaderPropertyCellRectangle = "_CellRectangle";
				private const string NameShaderPropertyPartsColorSkinnedMesh = "_PartsColor";

				private const string NameScriptPropertyOrderInLayer = "OrderInLayer";
				private const string NameScriptPropertyCell = "Cell";
				private const string NameScriptPropertyCellMesh = "CellMesh";
				private const string NameScriptPropertyTextureMesh = "TextureMesh";

				private readonly static Color ColorDefault = new Color(1.0f, 1.0f, 1.0f, 0.0f);
#endif
				#endregion Enums & Constants
			}
			#endregion Classes, Structs & Interfaces
		}
	}
}
