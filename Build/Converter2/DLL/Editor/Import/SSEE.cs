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
		public static partial class SSEE
		{
			/* ----------------------------------------------- Functions */
			#region Functions
			public static Information Parse(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
												string nameFile,
												LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ
											)
			{
				const string messageLogPrefix = "Parse SSEE";
				Information informationSSEE = null;

				/* ".ssee" Load */
				if(false == System.IO.File.Exists(nameFile))
				{
					LogError(messageLogPrefix, "File Not Found", nameFile, informationSSPJ);
					goto Parse_ErrorEnd;
				}
				System.Xml.XmlDocument xmlSSEE = new System.Xml.XmlDocument();
				xmlSSEE.Load(nameFile);

				/* Check Version */
				System.Xml.XmlNode nodeRoot = xmlSSEE.FirstChild;
				nodeRoot = nodeRoot.NextSibling;
				KindVersion version = (KindVersion)(LibraryEditor_SpriteStudio6.Utility.XML.VersionGet(nodeRoot, "SpriteStudioEffect", (int)KindVersion.ERROR, true));
#if false
				/* MEMO: Strict version check */
				switch(version)
				{
					case KindVersion.ERROR:
						LogError(messageLogPrefix, "Version Invalid", nameFile, informationSSPJ);
						goto Parse_ErrorEnd;

					case KindVersion.CODE_010000:
					case KindVersion.CODE_010001:
						LogError(messageLogPrefix, "Version Disused", nameFile, informationSSPJ);
						goto Parse_ErrorEnd;

					case KindVersion.CODE_010002:
						LogError(messageLogPrefix, "Version Unsupported (SpriteStudio5.5/5.6)", nameFile, informationSSPJ);
						goto Parse_ErrorEnd;

					case KindVersion.CODE_010100:
						break;

					case KindVersion.CODE_020000:
						break;

					default:
						if(KindVersion.TARGET_EARLIEST > version)
						{
							version = KindVersion.TARGET_EARLIEST;
							if(true == setting.CheckVersion.FlagInvalidSSEE)
							{
								LogWarning(messageLogPrefix, "Version Too Early", nameFile, informationSSPJ);
							}
						}
						else
						{
							version = KindVersion.TARGET_LATEST;
							if(true == setting.CheckVersion.FlagInvalidSSEE)
							{
								LogWarning(messageLogPrefix, "Version Unknown", nameFile, informationSSPJ);
							}
						}
						break;
				}
#else
				/* MEMO: Loose version check                                                       */
				/*       If you check strictly, there are a lot of datas that can not be imported. */
				switch(version)
				{
					case KindVersion.ERROR:
						LogError(messageLogPrefix, "Version Invalid", nameFile, informationSSPJ);
						goto Parse_ErrorEnd;

					case KindVersion.CODE_010000:
					case KindVersion.CODE_010001:
					case KindVersion.CODE_010002:
						/* MEMO: Read all as Ver.1.01.00. */
						version = KindVersion.CODE_010100;
						break;

					case KindVersion.CODE_010100:
						break;

					case KindVersion.CODE_020000:
						break;

					default:
						if(KindVersion.TARGET_EARLIEST > version)
						{
							version = KindVersion.TARGET_EARLIEST;
							if(true == setting.CheckVersion.FlagInvalidSSEE)
							{
								LogWarning(messageLogPrefix, "Version Too Early", nameFile, informationSSPJ);
							}
						}
						else
						{
							version = KindVersion.TARGET_LATEST;
							if(true == setting.CheckVersion.FlagInvalidSSEE)
							{
								LogWarning(messageLogPrefix, "Version Unknown", nameFile, informationSSPJ);
							}
						}
						break;
				}
#endif

				/* Create Information */
				informationSSEE = new Information();
				if(null == informationSSEE)
				{
					LogError(messageLogPrefix, "Not Enough Memory", nameFile, informationSSPJ);
					goto Parse_ErrorEnd;
				}
				informationSSEE.CleanUp();
				informationSSEE.Version = version;
				informationSSEE.ListInUseCellMap = new List<Information.InUseCellMap>();
				if(null == informationSSEE.ListInUseCellMap)
				{
					LogError(messageLogPrefix, "Not Enough Memory (In-use Cell-Map list)", nameFile, informationSSPJ);
					goto Parse_ErrorEnd;
				}

				/* Get Base-Directories */
				LibraryEditor_SpriteStudio6.Utility.File.PathSplit(out informationSSEE.NameDirectory, out informationSSEE.NameFileBody, out informationSSEE.NameFileExtension, nameFile);
				informationSSEE.NameDirectory += "/";

				/* Decode Tags */
				System.Xml.NameTable nodeNameSpace = new System.Xml.NameTable();
				System.Xml.XmlNamespaceManager managerNameSpace = new System.Xml.XmlNamespaceManager(nodeNameSpace);
				System.Xml.XmlNodeList nodeList = null;

				/* Get Base-Data */
				string valueText = "";
				switch(informationSSEE.Version)
				{
					case KindVersion.CODE_010002:
						break;

					case KindVersion.CODE_010100:
					case KindVersion.CODE_020000:
					{	/* SS5.7 */
						valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeRoot, "effectData/lockRandSeed", managerNameSpace);
						if(false == string.IsNullOrEmpty(valueText))
						{
							informationSSEE.SeedRandom = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);
						}

						valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeRoot, "effectData/isLockRandSeed", managerNameSpace);
						if(false == string.IsNullOrEmpty(valueText))
						{
							informationSSEE.FlagLockSeed = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetBool(valueText);
						}

						valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeRoot, "effectData/fps", managerNameSpace);
						if(false == string.IsNullOrEmpty(valueText))
						{
							informationSSEE.FramePerSecond = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);
						}

						valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeRoot, "effectData/renderVersion", managerNameSpace);
						if(false == string.IsNullOrEmpty(valueText))
						{
							informationSSEE.VersionRenderer = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);
						}

						valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeRoot, "effectData/layoutScaleX", managerNameSpace);
						if(false == string.IsNullOrEmpty(valueText))
						{
							informationSSEE.ScaleLayout.x = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText)) / 100.0f;
						}

						valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeRoot, "effectData/layoutScaleY", managerNameSpace);
						if(false == string.IsNullOrEmpty(valueText))
						{
							informationSSEE.ScaleLayout.y = (float)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText)) / 100.0f;
						}

						/* Get Parts-Data */
						nodeList = LibraryEditor_SpriteStudio6.Utility.XML.ListGetNode(nodeRoot, "effectData/nodeList/node", managerNameSpace);
						if(null == nodeList)
						{
							LogError(messageLogPrefix, "PartList-Node Not Found", nameFile, informationSSPJ);
							goto Parse_ErrorEnd;
						}
						informationSSEE.TableParts = new Information.Parts[nodeList.Count];
						if(null == informationSSEE.TableParts)
						{
							LogError(messageLogPrefix, "Not Enough Memory (Parts-Data WorkArea)", nameFile, informationSSPJ);
							goto Parse_ErrorEnd;
						}
						foreach(System.Xml.XmlNode nodeParts in nodeList)
						{
							/* Get Part-ID */
							int indexParts = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "arrayIndex", managerNameSpace));

							/* Get Part-Data */
							informationSSEE.TableParts[indexParts] = ParseParts(	ref setting,
																					informationSSPJ,
																					nodeParts,
																					managerNameSpace,
																					informationSSEE,
																					indexParts,
																					nameFile
																				);
							if(null == informationSSEE.TableParts[indexParts])
							{
								goto Parse_ErrorEnd;
							}
						}
					}
					break;
				}

				/* Limit Sub-Emitters */
				if(false == informationSSEE.EmitterLimit(ref setting, informationSSPJ))
				{
					goto Parse_ErrorEnd;
				}

				/* Fix Parts' child list */
				int countParts = informationSSEE.TableParts.Length;
				for(int i=0; i<countParts; i++)
				{
					informationSSEE.TableParts[i].Data.TableIDChild = informationSSEE.TableParts[i].ListIndexPartsChild.ToArray();
					informationSSEE.TableParts[i].ListIndexPartsChild = null;
				}

				return(informationSSEE);

			Parse_ErrorEnd:;
				return(null);
			}

			private static Information.Parts ParseParts(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
															LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
															System.Xml.XmlNode nodeParts,
															System.Xml.XmlNamespaceManager managerNameSpace,
															Information informationSSEE,
															int indexParts,
															string nameFileSSEE
														)
			{
				const string messageLogPrefix = "Parse SSEE(Parts)";

				/* Create Information */
				Information.Parts informationParts = new Information.Parts();
				if(null == informationParts)
				{
					LogError(messageLogPrefix, "Not Enough Memory (Parts WorkArea)", nameFileSSEE, informationSSPJ);
					goto ParseParts_ErrorEnd;
				}
				informationParts.CleanUp();
				informationParts.BootUp();

				/* Get Base-Datas */
				string valueText = "";

				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "name", managerNameSpace);
				informationParts.Data.Name = string.Copy(valueText);

				informationParts.Data.ID = indexParts;

				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "parentIndex", managerNameSpace);
				informationParts.Data.IDParent = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);
				if(0 <= informationParts.Data.IDParent)
				{
					Information.Parts informationPartsParent = informationSSEE.TableParts[informationParts.Data.IDParent];
					informationPartsParent.ListIndexPartsChild.Add(informationParts.Data.ID);
				}

				Library_SpriteStudio6.KindOperationBlendEffect operationBlendTarget = Library_SpriteStudio6.KindOperationBlendEffect.NON;
				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "behavior/BlendType", managerNameSpace);
				if(false == string.IsNullOrEmpty(valueText))
				{
					switch(valueText)
					{
						case "Mix":
							operationBlendTarget = Library_SpriteStudio6.KindOperationBlendEffect.MIX;
							break;

						case "Add":
							operationBlendTarget = Library_SpriteStudio6.KindOperationBlendEffect.ADD;
							break;

						default:
							LogWarning(messageLogPrefix, "Unknown Alpha-Blend Kind \"" + valueText + "\" Parts[" + indexParts.ToString() + "]", nameFileSSEE, informationSSPJ);
							goto case "Mix";
					}
				}

				string nameCellMap = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "behavior/CellMapName", managerNameSpace);
				string nameCell = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "behavior/CellName", managerNameSpace);

				/* Get Parts-Kind */
				valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeParts, "type", managerNameSpace);
				switch(valueText)
				{
					case "Root":
						if(true == string.IsNullOrEmpty(informationParts.Data.Name))
						{	/* Default-Name */
							informationParts.Data.Name = "Root";
						}
						informationParts.Data.Feature = Library_SpriteStudio6.Data.Parts.Effect.KindFeature.ROOT;
						informationParts.DataEmitter = null;
						break;

					case "Emmiter":	/* "Emitter" */
						if(true == string.IsNullOrEmpty(informationParts.Data.Name))
						{	/* Default-Name */
							informationParts.Data.Name = "Emitter";
						}
						informationParts.Data.Feature = Library_SpriteStudio6.Data.Parts.Effect.KindFeature.EMITTER;
						informationParts.DataEmitter = ParsePartsEmitter(	ref setting,
																			informationSSPJ,
																			nodeParts,
																			managerNameSpace,
																			informationSSEE,
																			informationParts,
																			operationBlendTarget,
																			nameCellMap,
																			nameCell,
																			indexParts,
																			nameFileSSEE
																		);
						if(null == informationParts.DataEmitter)
						{
							goto ParseParts_ErrorEnd;
						}
						break;

					case "Particle":
						if(true == string.IsNullOrEmpty(informationParts.Data.Name))
						{	/* Default-Name */
							informationParts.Data.Name = "Particle";
						}
						informationParts.Data.Feature = Library_SpriteStudio6.Data.Parts.Effect.KindFeature.PARTICLE;
						informationParts.DataEmitter = null;
						break;

					default:
						/* MEMO: Error */
						if(true == string.IsNullOrEmpty(informationParts.Data.Name))
						{	/* Default-Name */
							informationParts.Data.Name = "Error";
						}
						informationParts.Data.Feature = (Library_SpriteStudio6.Data.Parts.Effect.KindFeature)(-1);
						informationParts.DataEmitter = null;
						break;
				}

				return(informationParts);

			ParseParts_ErrorEnd:;
				return(null);
			}

			private static Information.Parts.Emitter ParsePartsEmitter(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
																		LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
																		System.Xml.XmlNode nodeParts,
																		System.Xml.XmlNamespaceManager managerNameSpace,
																		Information informationSSEE,
																		Information.Parts informationParts,
																		Library_SpriteStudio6.KindOperationBlendEffect operationBlendTarget,
																		string nameCellMap,
																		string nameCell,
																		int indexParts,
																		string nameFileSSEE
																	)
			{
				const string messageLogPrefix = "Parse SSEE(Emitter)";

				/* Create Information */
				Information.Parts.Emitter informationEmitter = new Information.Parts.Emitter();
				if(null == informationEmitter)
				{
					LogError(messageLogPrefix, "Not Enough Memory (Parts WorkArea)", nameFileSSEE, informationSSPJ);
					goto ParsePartsEmitter_ErrorEnd;
				}
				informationEmitter.CleanUp();

				/* Get Base Datas */
				string nameDirectoryCellMap;
				string nameFileBodyCellMap;
				string nameFileExtentionCellMap;
				LibraryEditor_SpriteStudio6.Utility.File.PathSplit(out nameDirectoryCellMap, out nameFileBodyCellMap, out nameFileExtentionCellMap, nameCellMap);	/* Remove ".ssce" */

				informationEmitter.Data.FlagData = Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.CLEAR;
				informationEmitter.Data.OperationBlendTarget = operationBlendTarget;
				informationEmitter.NameCellMap = nameFileBodyCellMap;
				informationEmitter.NameCell = nameCell;

				string valueText = "";
				valueText = nameCellMap;
				if(false == string.IsNullOrEmpty(valueText))
				{
					valueText = informationSSPJ.PathGetAbsolute(valueText, LibraryEditor_SpriteStudio6.Import.KindFile.SSCE);
					informationEmitter.Data.IndexCellMap = informationSSPJ.IndexGetFileName(informationSSPJ.TableNameSSCE, valueText);

					/* Add to In-use-Cell-Map information */
					if(0 <= informationEmitter.Data.IndexCellMap)
					{
						informationSSEE.InUseCellMapAdd(informationEmitter.Data.IndexCellMap, informationEmitter);
					}
				}
				informationEmitter.Data.IndexCell = -1;

				System.Xml.XmlNode nodeEmitterAttributes = LibraryEditor_SpriteStudio6.Utility.XML.NodeGet(nodeParts, "behavior/list", managerNameSpace);
				System.Xml.XmlNodeList listNodeAttribute = LibraryEditor_SpriteStudio6.Utility.XML.ListGetNode(nodeEmitterAttributes, "value", managerNameSpace);
				foreach(System.Xml.XmlNode nodeAttribute in listNodeAttribute)
				{
					/* Get Attribute-Classification */
					valueText = nodeAttribute.Attributes["name"].Value;
					switch(valueText)
					{
						case "Basic":
							{
								informationEmitter.Data.FlagData |= Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.BASIC;

								valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAttribute, "priority", managerNameSpace);
								if(false == string.IsNullOrEmpty(valueText))
								{
									informationEmitter.PriorityParticle = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueText);
								}

								valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAttribute, "maximumParticle", managerNameSpace);
								if(false == string.IsNullOrEmpty(valueText))
								{
									informationEmitter.Data.CountParticleMax = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);
								}

								valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAttribute, "attimeCreate", managerNameSpace);
								if(false == string.IsNullOrEmpty(valueText))
								{
									informationEmitter.Data.CountParticleEmit = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);
								}

								valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAttribute, "interval", managerNameSpace);
								if(false == string.IsNullOrEmpty(valueText))
								{
									informationEmitter.Data.Interval = (int)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueText));
								}

								valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAttribute, "lifetime", managerNameSpace);
								if(false == string.IsNullOrEmpty(valueText))
								{
									informationEmitter.Data.DurationEmitter = (int)(LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueText));
								}

								ParsePartsEmitterRangeGetFloat(ref informationEmitter.Data.Speed.Main, ref informationEmitter.Data.Speed.Sub, informationSSPJ, nodeAttribute, "speed", managerNameSpace, indexParts, nameFileSSEE);

								ParsePartsEmitterRangeGetFloat(ref informationEmitter.Data.DurationParticle.Main, ref informationEmitter.Data.DurationParticle.Sub, informationSSPJ, nodeAttribute, "lifespan", managerNameSpace, indexParts, nameFileSSEE);

								valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAttribute, "angle", managerNameSpace);
								if(false == string.IsNullOrEmpty(valueText))
								{
									informationEmitter.Data.Angle.Main = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueText);
								}

								valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAttribute, "angleVariance", managerNameSpace);
								if(false == string.IsNullOrEmpty(valueText))
								{
									informationEmitter.Data.Angle.Sub = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueText);
								}
							}
							break;

						case "OverWriteSeed":
							{
								informationEmitter.Data.FlagData |= Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.SEEDRANDOM;

								valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAttribute, "Seed", managerNameSpace);
								if(false == string.IsNullOrEmpty(valueText))
								{
									informationEmitter.Data.SeedRandom = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);
								}
							}
							break;

						case "Delay":
							{
								informationEmitter.Data.FlagData |= Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.DELAY;

								valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAttribute, "DelayTime", managerNameSpace);
								if(false == string.IsNullOrEmpty(valueText))
								{
									informationEmitter.Data.Delay = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetInt(valueText);
								}
							}
							break;

						case "Gravity":
							{
								informationEmitter.Data.FlagData |= Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.GRAVITY_DIRECTION;

								valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAttribute, "Gravity", managerNameSpace);
								if(false == string.IsNullOrEmpty(valueText))
								{
									string[] valueTextSplit = valueText.Split(' ');
									if(2 == valueTextSplit.Length)
									{
										informationEmitter.Data.GravityDirectional.x = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueTextSplit[0]);
										informationEmitter.Data.GravityDirectional.y = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueTextSplit[1]);
									}
									else
									{
										LogWarning(messageLogPrefix, "Broken Emitter-Attribute \"Gravity/Gravity\" Parts[" + indexParts.ToString() + "]", nameFileSSEE, informationSSPJ);
									}
								}
							}
							break;

						case "init_position":
							{
								informationEmitter.Data.FlagData |= Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.POSITION;

								ParsePartsEmitterRangeGetFloat(ref informationEmitter.Data.Position.Main.x, ref informationEmitter.Data.Position.Sub.x, informationSSPJ, nodeAttribute, "OffsetX", managerNameSpace, indexParts, nameFileSSEE);
								ParsePartsEmitterRangeGetFloat(ref informationEmitter.Data.Position.Main.y, ref informationEmitter.Data.Position.Sub.y, informationSSPJ, nodeAttribute, "OffsetY", managerNameSpace, indexParts, nameFileSSEE);
							}
							break;

						case "trans_speed":
							{
								informationEmitter.Data.FlagData |= Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.SPEED_FLUCTUATION;

								ParsePartsEmitterRangeGetFloat(ref informationEmitter.Data.SpeedFluctuation.Main, ref informationEmitter.Data.SpeedFluctuation.Sub, informationSSPJ, nodeAttribute, "Speed", managerNameSpace, indexParts, nameFileSSEE);
							}
							break;

						case "init_rotation":
							{
								informationEmitter.Data.FlagData |= Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.ROTATION;

								ParsePartsEmitterRangeGetFloat(ref informationEmitter.Data.Rotation.Main, ref informationEmitter.Data.Rotation.Sub, informationSSPJ, nodeAttribute, "Rotation", managerNameSpace, indexParts, nameFileSSEE);

								ParsePartsEmitterRangeGetFloat(ref informationEmitter.Data.RotationFluctuation.Main, ref informationEmitter.Data.RotationFluctuation.Sub, informationSSPJ, nodeAttribute, "RotationAdd", managerNameSpace, indexParts, nameFileSSEE);
						}
						break;

						case "trans_rotation":
							{
								informationEmitter.Data.FlagData |= Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.ROTATION_FLUCTUATION;

								valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAttribute, "RotationFactor", managerNameSpace);
								if(false == string.IsNullOrEmpty(valueText))
								{
									informationEmitter.Data.RotationFluctuationRate = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueText);
								}

								valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAttribute, "EndLifeTimePer", managerNameSpace);
								if(false == string.IsNullOrEmpty(valueText))
								{
									/* MEMO: Percent -> Rate */
									informationEmitter.Data.RotationFluctuationRateTime = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueText) * 0.01f;
								}
							}
							break;

						case "add_tangentiala":
							{
								informationEmitter.Data.FlagData |= Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.TANGENTIALACCELATION;

								ParsePartsEmitterRangeGetFloat(ref informationEmitter.Data.RateTangentialAcceleration.Main, ref informationEmitter.Data.RateTangentialAcceleration.Sub, informationSSPJ, nodeAttribute, "Acceleration", managerNameSpace, indexParts, nameFileSSEE);
							}
							break;

						case "add_pointgravity":
							{
								informationEmitter.Data.FlagData |= Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.GRAVITY_POINT;

								valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAttribute, "Position", managerNameSpace);
								if(false == string.IsNullOrEmpty(valueText))
								{
									string[] valueTextSplit = valueText.Split(' ');
									if(2 == valueTextSplit.Length)
									{
										informationEmitter.Data.GravityPointPosition.x = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueTextSplit[0]);
										informationEmitter.Data.GravityPointPosition.y = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueTextSplit[1]);
									}
									else
									{
										LogWarning(messageLogPrefix, "Broken Emitter-Attribute \"add_pointgravity/Position\" Parts[" + indexParts.ToString() + "]", nameFileSSEE, informationSSPJ);
									}
								}

								valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAttribute, "Power", managerNameSpace);
								if(false == string.IsNullOrEmpty(valueText))
								{
									informationEmitter.Data.GravityPointPower = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueText);
								}
							}
							break;

						case "init_vertexcolor":
							{
								informationEmitter.Data.FlagData |= Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.COLORVERTEX;

								ParsePartsEmitterRangeGetColor(ref informationEmitter.Data.ColorVertex.Main, ref informationEmitter.Data.ColorVertex.Sub, informationSSPJ, nodeAttribute, "Color", managerNameSpace, indexParts, nameFileSSEE);
							}
							break;

						case "trans_vertexcolor":
							{
								informationEmitter.Data.FlagData |= Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.COLORVERTEX_FLUCTUATION;

								ParsePartsEmitterRangeGetColor(ref informationEmitter.Data.ColorVertexFluctuation.Main, ref informationEmitter.Data.ColorVertexFluctuation.Sub, informationSSPJ, nodeAttribute, "Color", managerNameSpace, indexParts, nameFileSSEE);
							}
							break;

						case "init_size":
							{
								informationEmitter.Data.FlagData |= Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.SCALE_START;

								ParsePartsEmitterRangeGetFloat(ref informationEmitter.Data.ScaleStart.Main.x, ref informationEmitter.Data.ScaleStart.Sub.x, informationSSPJ, nodeAttribute, "SizeX", managerNameSpace, indexParts, nameFileSSEE);

								ParsePartsEmitterRangeGetFloat(ref informationEmitter.Data.ScaleStart.Main.y, ref informationEmitter.Data.ScaleStart.Sub.y, informationSSPJ, nodeAttribute, "SizeY", managerNameSpace, indexParts, nameFileSSEE);

								ParsePartsEmitterRangeGetFloat(ref informationEmitter.Data.ScaleRateStart.Main, ref informationEmitter.Data.ScaleRateStart.Sub, informationSSPJ, nodeAttribute, "ScaleFactor", managerNameSpace, indexParts, nameFileSSEE);
							}
							break;

						case "trans_size":
							{
								informationEmitter.Data.FlagData |= Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.SCALE_END;

								ParsePartsEmitterRangeGetFloat(ref informationEmitter.Data.ScaleEnd.Main.x, ref informationEmitter.Data.ScaleEnd.Sub.x, informationSSPJ, nodeAttribute, "SizeX", managerNameSpace, indexParts, nameFileSSEE);

								ParsePartsEmitterRangeGetFloat(ref informationEmitter.Data.ScaleEnd.Main.y, ref informationEmitter.Data.ScaleEnd.Sub.y, informationSSPJ, nodeAttribute, "SizeY", managerNameSpace, indexParts, nameFileSSEE);

								ParsePartsEmitterRangeGetFloat(ref informationEmitter.Data.ScaleRateEnd.Main, ref informationEmitter.Data.ScaleRateEnd.Sub, informationSSPJ, nodeAttribute, "ScaleFactor", managerNameSpace, indexParts, nameFileSSEE);
							}
							break;

						case "trans_colorfade":
							{
								informationEmitter.Data.FlagData |= Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.FADEALPHA;

								Library_SpriteStudio6.Data.Effect.Emitter.RangeFloat rangeTemp = new Library_SpriteStudio6.Data.Effect.Emitter.RangeFloat();
								ParsePartsEmitterRangeGetFloat(ref rangeTemp.Main, ref rangeTemp.Sub, informationSSPJ, nodeAttribute, "disprange", managerNameSpace, indexParts, nameFileSSEE);
								informationEmitter.Data.AlphaFadeStart = rangeTemp.Main * 0.01f;
								informationEmitter.Data.AlphaFadeEnd = (rangeTemp.Main + rangeTemp.Sub) * 0.01f;
							}
							break;

						case "TurnToDirection":
							{
								informationEmitter.Data.FlagData |= Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.TURNDIRECTION;

								valueText = LibraryEditor_SpriteStudio6.Utility.XML.TextGetNode(nodeAttribute, "Rotation", managerNameSpace);
								if(false == string.IsNullOrEmpty(valueText))
								{
									informationEmitter.Data.TurnDirectionFluctuation = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueText);
								}
							}
							break;

						case "InfiniteEmit":
							{
								informationEmitter.Data.FlagData |= Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.EMIT_INFINITE;
							}
							break;

						default:
							LogWarning(messageLogPrefix, "Unknown Attribute \"" + valueText + "\" Parts[" + indexParts.ToString() + "]", nameFileSSEE, informationSSPJ);
							break;
					}
				}

				return(informationEmitter);

			ParsePartsEmitter_ErrorEnd:;
				return(null);
			}
			private static bool ParsePartsEmitterRangeGetFloat(	ref float outputMain,
																ref float outputSub,
																LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
																System.Xml.XmlNode nodeAttribute,
																string name,
																System.Xml.XmlNamespaceManager managerNameSpace,
																int indexParts,
																string nameFileSSEE
															)
			{
				const string messageLogPrefix = "Parse SSEE(Emitter)";

				bool flagValid = true;
				string valueText = "";
				System.Xml.XmlNode nodeNow = LibraryEditor_SpriteStudio6.Utility.XML.NodeGet(nodeAttribute, name, managerNameSpace);
				if(null != nodeNow)
				{
					valueText = nodeNow.Attributes["value"].Value;
					if(false == string.IsNullOrEmpty(valueText))
					{
						outputMain = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueText);
					}
					else
					{
						flagValid = false;
					}
	
					valueText = nodeNow.Attributes["subvalue"].Value;
					if(false == string.IsNullOrEmpty(valueText))
					{
						outputSub = LibraryEditor_SpriteStudio6.Utility.Text.ValueGetFloat(valueText);
					}
					else
					{
						flagValid = false;
					}
	
					if(outputMain > outputSub)
					{
						float floatTemp = outputSub;
						outputSub = outputMain;
						outputMain = floatTemp;
					}
					outputSub -= outputMain;
				}
				if(false == flagValid)
				{
					LogWarning(messageLogPrefix, "Broken Emitter-Attribute's Parameter \"" + name + "\" Parts[" + indexParts.ToString() + "]", nameFileSSEE, informationSSPJ);
				}
				return(flagValid);
			}
			private static bool ParsePartsEmitterRangeGetColor(	ref Color outputMain,
																ref Color outputSub,
																LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
																System.Xml.XmlNode nodeAttribute,
																string name,
																System.Xml.XmlNamespaceManager managerNameSpace,
																int indexParts,
																string nameFileSSEE
															)
			{
				const string messageLogPrefix = "Parse SSEE(Emitter)";

				bool flagValid = true;
				string valueText = "";
				System.Xml.XmlNode nodeNow = LibraryEditor_SpriteStudio6.Utility.XML.NodeGet(nodeAttribute, name, managerNameSpace);
				if(null != nodeNow)
				{
					uint colorTemp;
					valueText = nodeNow.Attributes["value"].Value;
					if(false == string.IsNullOrEmpty(valueText))
					{
						colorTemp = LibraryEditor_SpriteStudio6.Utility.Text.HexToUInt(valueText);
						outputMain.a = ((float)((colorTemp >> 24) & 0xff)) / 255.0f;
						outputMain.r = ((float)((colorTemp >> 16) & 0xff)) / 255.0f;
						outputMain.g = ((float)((colorTemp >> 8) & 0xff)) / 255.0f;
						outputMain.b = ((float)(colorTemp & 0xff)) / 255.0f;
					}
					else
					{
						flagValid = false;
					}
	
					valueText = nodeNow.Attributes["subvalue"].Value;
					if(false == string.IsNullOrEmpty(valueText))
					{
						colorTemp = LibraryEditor_SpriteStudio6.Utility.Text.HexToUInt(valueText);
						outputSub.a = ((float)((colorTemp >> 24) & 0xff)) / 255.0f;
						outputSub.r = ((float)((colorTemp >> 16) & 0xff)) / 255.0f;
						outputSub.g = ((float)((colorTemp >> 8) & 0xff)) / 255.0f;
						outputSub.b = ((float)(colorTemp & 0xff)) / 255.0f;
					}
					else
					{
						flagValid = false;
					}
	
					float floatTemp;
					if(outputMain.a > outputSub.a)
					{
						floatTemp = outputSub.a;
						outputSub.a = outputMain.a;
						outputMain.a = floatTemp;
					}
					if(outputMain.r > outputSub.r)
					{
						floatTemp = outputSub.r;
						outputSub.r = outputMain.r;
						outputMain.r = floatTemp;
					}
					if(outputMain.g > outputSub.g)
					{
						floatTemp = outputSub.g;
						outputSub.g = outputMain.g;
						outputMain.g = floatTemp;
					}
					if(outputMain.b > outputSub.b)
					{
						floatTemp = outputSub.b;
						outputSub.b = outputMain.b;
						outputMain.b = floatTemp;
					}
	
					outputSub.a -= outputMain.a;
					outputSub.r -= outputMain.r;
					outputSub.g -= outputMain.g;
					outputSub.b -= outputMain.b;
				}
				if(false == flagValid)
				{
					LogWarning(messageLogPrefix, "Broken Emitter-Attribute's Parameter \"" + name + "\" Parts[" + indexParts.ToString() + "]", nameFileSSEE, informationSSPJ);
				}
				return(flagValid);
			}

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
				CODE_010000 = 0x00010000,	/* (Disuse) */
				CODE_010001 = 0x00010001,	/* (Disuse) */
				CODE_010002 = 0x00010002,	/* after SS5.5 (Unsupported) */
				CODE_010100 = 0x00010100,	/* after SS5.7 */
				CODE_020000 = 0x00020000,	/* after SS6.0 */

				TARGET_EARLIEST = CODE_010000,
				TARGET_LATEST = CODE_020000
			};

			public enum Constants
			{
				LIMIT_SUBEMITTER_DEPTH = 2,
				LIMIT_SUBEMITTER_COUNT = 10,
			}

			private const string ExtentionFile = ".ssee";
			#endregion Enums & Constants

			/* ----------------------------------------------- Classes, Structs & Interfaces */
			#region Classes, Structs & Interfaces
			public class Information
			{
				/* ----------------------------------------------- Variables & Properties */
				#region Variables & Properties
				public LibraryEditor_SpriteStudio6.Import.SSEE.KindVersion Version;

				public string NameDirectory;
				public string NameFileBody;
				public string NameFileExtension;

				public int VersionRenderer;
				public Vector2 ScaleLayout;
				public int SeedRandom;
				public bool FlagLockSeed;
				public int FramePerSecond;

				public Parts[] TableParts;

				public List<InUseCellMap> ListInUseCellMap;

				public Library_SpriteStudio6.Data.Parts.Effect[] TablePartsSS6PU;
				public Library_SpriteStudio6.Data.Effect.Emitter[] TableEmitterSS6PU;
				public int[] TableIndexEmitterOrderDrawSS6PU;

				public LibraryEditor_SpriteStudio6.Import.Assets<Script_SpriteStudio6_DataEffect> DataEffectSS6PU;
#if !_FOR_SS6CONVERTER_
				public LibraryEditor_SpriteStudio6.Import.Assets<Object> PrefabEffectSS6PU;
#endif
				#endregion Variables & Properties

				/* ----------------------------------------------- Functions */
				#region Functions
				public void CleanUp()
				{
					Version = LibraryEditor_SpriteStudio6.Import.SSEE.KindVersion.ERROR;

					NameDirectory = "";
					NameFileBody = "";
					NameFileExtension = "";

					VersionRenderer = 0;
					ScaleLayout = Vector2.one;
					SeedRandom = 0;
					FlagLockSeed = false;;
					FramePerSecond = 60;

					TableParts = null;

					ListInUseCellMap = null;

					TablePartsSS6PU = null;
					TableEmitterSS6PU = null;
					TableIndexEmitterOrderDrawSS6PU = null;

					DataEffectSS6PU.CleanUp();
					DataEffectSS6PU.BootUp(1);	/* Always 1 */
#if !_FOR_SS6CONVERTER_
					PrefabEffectSS6PU.CleanUp();
					PrefabEffectSS6PU.BootUp(1);	/* Always 1 */
#endif
				}

				public string FileNameGetFullPath()
				{
					return(NameDirectory + NameFileBody + NameFileExtension);
				}

				public bool EmitterLimit(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
											LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ
										)
				{
					/* MEMO: "Effect"data has limitation of "up to 10 parallel sub-emitters" and "Emitter hierarchy up to 2 levels". */
					/*       Sub-emitters for beyond limitations are ignored.                                                        */
					return(true);

//				EmitterLimit_ErrorEnd:;
//					return(false);
				}

				public void InUseCellMapAdd(int indexCellMap, Parts.Emitter informationEmitter)
				{
					InUseCellMap value = new InUseCellMap();
					value.IndexCellMap = indexCellMap;
					value.Blend = informationEmitter.Data.OperationBlendTarget;

					if(false == ListInUseCellMap.Contains(value))
					{
						ListInUseCellMap.Add(value);
					}
				}
				#endregion Functions

				/* ----------------------------------------------- Classes, Structs & Interfaces */
				#region Classes, Structs & Interfaces
				public class Parts
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public Library_SpriteStudio6.Data.Parts.Effect Data;

					public List<int> ListIndexPartsChild;	/* Temporary */
					public Emitter DataEmitter;
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public void CleanUp()
					{
						Data.CleanUp();

						ListIndexPartsChild = null;
						DataEmitter = null;
					}

					public bool BootUp()
					{
						ListIndexPartsChild = new List<int>();
						ListIndexPartsChild.Clear();

						DataEmitter = new Emitter();
						DataEmitter.CleanUp();

						return(true);

//					BootUp_ErrorEnd:;
//						return(false);
					}
					#endregion Functions

					/* ----------------------------------------------- Classes, Structs & Interfaces */
					#region Classes, Structs & Interfaces
					public class Emitter
					{
						/* ----------------------------------------------- Variables & Properties */
						#region Variables & Properties
						public Library_SpriteStudio6.Data.Effect.Emitter Data;

						public float PriorityParticle;
						public string NameCellMap;
						public string NameCell;
						#endregion Variables & Properties

						/* ----------------------------------------------- Functions */
						#region Functions
						public void CleanUp()
						{
							Data.CleanUp();

							PriorityParticle = 64.0f;
							NameCellMap = "";
							NameCell = "";
						}
						#endregion Functions
					}
					#endregion Classes, Structs & Interfaces
				}

				public struct InUseCellMap
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public int IndexCellMap;
					public Library_SpriteStudio6.KindOperationBlendEffect Blend;
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public void CleanUp()
					{
						IndexCellMap = -1;
						Blend = Library_SpriteStudio6.KindOperationBlendEffect.MIX;
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
				public static bool AssetNameDecideData(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
														LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
														LibraryEditor_SpriteStudio6.Import.SSEE.Information informationSSEE,
														string nameOutputAssetFolderBase,
														Script_SpriteStudio6_DataEffect dataOverride
													)
				{
					if(null != dataOverride)
					{	/* Specified */
						informationSSEE.DataEffectSS6PU.TableName[0] = AssetDatabase.GetAssetPath(dataOverride);
					}
					else
					{	/* Default */
						informationSSEE.DataEffectSS6PU.TableName[0] = setting.RuleNameAssetFolder.NameGetAssetFolder(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_EFFECT_SS6PU, nameOutputAssetFolderBase)
																		+ setting.RuleNameAsset.NameGetAsset(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.DATA_EFFECT_SS6PU, informationSSEE.NameFileBody, informationSSPJ.NameFileBody)
																		+ LibraryEditor_SpriteStudio6.Import.NameExtentionScriptableObject;
						dataOverride = AssetDatabase.LoadAssetAtPath<Script_SpriteStudio6_DataEffect>(informationSSEE.DataEffectSS6PU.TableName[0]);
					}

					informationSSEE.DataEffectSS6PU.TableData[0] = dataOverride;
					informationSSEE.DataEffectSS6PU.Version[0] = (null != dataOverride) ? (int)(dataOverride.Version) : (int)Script_SpriteStudio6_DataEffect.KindVersion.SS5PU;

					return(true);

//				AssetNameDecide_ErroeEnd:;
//					return(false);
				}

				public static bool AssetNameDecidePrefab(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
														LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
														LibraryEditor_SpriteStudio6.Import.SSEE.Information informationSSEE,
														string nameOutputAssetFolderBase,
														Script_SpriteStudio6_RootEffect prefabOverride
													)
				{
					if(null != prefabOverride)
					{	/* Specified */
						informationSSEE.PrefabEffectSS6PU.TableName[0] = AssetDatabase.GetAssetPath(prefabOverride);
						informationSSEE.PrefabEffectSS6PU.TableData[0] = prefabOverride;
					}
					else
					{	/* Default */
						informationSSEE.PrefabEffectSS6PU.TableName[0] = setting.RuleNameAssetFolder.NameGetAssetFolder(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_EFFECT_SS6PU, nameOutputAssetFolderBase)
																		+ setting.RuleNameAsset.NameGetAsset(LibraryEditor_SpriteStudio6.Import.Setting.KindAsset.PREFAB_EFFECT_SS6PU, informationSSEE.NameFileBody, informationSSPJ.NameFileBody)
																		+ LibraryEditor_SpriteStudio6.Import.NameExtensionPrefab;
						informationSSEE.PrefabEffectSS6PU.TableData[0] = AssetDatabase.LoadAssetAtPath<GameObject>(informationSSEE.PrefabEffectSS6PU.TableName[0]);
					}

					/* MEMO: "Control-Prefab" creates only the name. */
					informationSSEE.PrefabEffectSS6PU.Version[0] = -1;

					return(true);

//				AssetNameDecide_ErroeEnd:;
//					return(false);
				}
#endif

#if !_FOR_SS6CONVERTER_
				public static bool AssetCreateData(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
													LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
													LibraryEditor_SpriteStudio6.Import.SSEE.Information informationSSEE
												)
				{
//					const string messageLogPrefix = "Create Asset(Data-Effect)";

					Script_SpriteStudio6_DataEffect dataEffect = informationSSEE.DataEffectSS6PU.TableData[0];
					if(null == dataEffect)
					{
						dataEffect = ScriptableObject.CreateInstance<Script_SpriteStudio6_DataEffect>();
						AssetDatabase.CreateAsset(dataEffect, informationSSEE.DataEffectSS6PU.TableName[0]);
						informationSSEE.DataEffectSS6PU.TableData[0] = dataEffect;
					}

					dataEffect.Version = Script_SpriteStudio6_DataEffect.KindVersion.SUPPORT_LATEST;
					dataEffect.TableParts = informationSSEE.TablePartsSS6PU;
					dataEffect.TableEmitter = informationSSEE.TableEmitterSS6PU;
					dataEffect.TableIndexEmitterOrderDraw = informationSSEE.TableIndexEmitterOrderDrawSS6PU;

					dataEffect.FlagData = Script_SpriteStudio6_DataEffect.FlagBit.CLEAR;
					dataEffect.FlagData |= (true == informationSSEE.FlagLockSeed) ? Script_SpriteStudio6_DataEffect.FlagBit.SEEDRANDOM_LOCK : Script_SpriteStudio6_DataEffect.FlagBit.CLEAR;
					dataEffect.SeedRandom = informationSSEE.SeedRandom;
					dataEffect.CountFramePerSecond = informationSSEE.FramePerSecond;
					dataEffect.ScaleLayout = informationSSEE.ScaleLayout;
					dataEffect.VersionRenderer = informationSSEE.VersionRenderer;
					dataEffect.CountMaxParticle = 0;

					dataEffect.TableMaterial = informationSSPJ.TableMaterialEffectSS6PU;

					EditorUtility.SetDirty(dataEffect);
					AssetDatabase.SaveAssets();

					return(true);

//				AssetCreateData_ErrorEnd:;
//					return(false);
				}
#else
				public static bool DataOutput(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
												LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
												LibraryEditor_SpriteStudio6.Import.SSEE.Information informationSSEE
											)
				{
//					const string messageLogPrefix = "Create Asset(Data-Effect)";

					Script_SpriteStudio6_DataEffect dataEffect = new Script_SpriteStudio6_DataEffect();
					if(null == dataEffect)
					{
						return(false);
					}
					informationSSEE.DataEffectSS6PU.TableData[0] = dataEffect;

					dataEffect.Version = Script_SpriteStudio6_DataEffect.KindVersion.SUPPORT_LATEST;
					dataEffect.TableParts = informationSSEE.TablePartsSS6PU;
					dataEffect.TableEmitter = informationSSEE.TableEmitterSS6PU;
					dataEffect.TableIndexEmitterOrderDraw = informationSSEE.TableIndexEmitterOrderDrawSS6PU;

					dataEffect.FlagData = Script_SpriteStudio6_DataEffect.FlagBit.CLEAR;
					dataEffect.FlagData |= (true == informationSSEE.FlagLockSeed) ? Script_SpriteStudio6_DataEffect.FlagBit.SEEDRANDOM_LOCK : Script_SpriteStudio6_DataEffect.FlagBit.CLEAR;
					dataEffect.SeedRandom = informationSSEE.SeedRandom;
					dataEffect.CountFramePerSecond = informationSSEE.FramePerSecond;
					dataEffect.ScaleLayout = informationSSEE.ScaleLayout;
					dataEffect.VersionRenderer = informationSSEE.VersionRenderer;
					dataEffect.CountMaxParticle = 0;

					return(true);
				}
#endif

#if !_FOR_SS6CONVERTER_
				public static bool AssetCreatePrefab(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
														LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
														LibraryEditor_SpriteStudio6.Import.SSEE.Information informationSSEE
													)
				{
					const string messageLogPrefix = "Create Asset(Prefab-Effect)";

					GameObject gameObjectRoot = null;
					Script_SpriteStudio6_RootEffect scriptRoot = null;
					int limitParticleLimit = 0;
					float rateTime = 1.0f;
					bool flagHideForce = false;
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
					if(null == informationSSEE.PrefabEffectSS6PU.TableData[0])
					{	/* New */
#if UNITY_2018_4_OR_NEWER || UNITY_2019_1_OR_NEWER
						/* MEMO: Process nothing, now. */
#else
						informationSSEE.PrefabEffectSS6PU.TableData[0] = PrefabUtility.CreateEmptyPrefab(informationSSEE.PrefabEffectSS6PU.TableName[0]);

						if(null == informationSSEE.PrefabEffectSS6PU.TableData[0])
						{
							LogError(messageLogPrefix, "Failure to create Prefab", informationSSEE.FileNameGetFullPath(), informationSSPJ);
							goto AssetCreatePrefab_ErrorEnd;
						}
#endif
					}
					else
					{	/* Exist */
						/* MEMO: Do not instantiate old prefabs. Instantiates up to objects under control, and mixed in updated prefab. */
						gameObjectRoot = (GameObject)informationSSEE.PrefabEffectSS6PU.TableData[0];
#if UNITY_2018_4_OR_NEWER || UNITY_2019_1_OR_NEWER
						informationSSEE.PrefabEffectSS6PU.TableName[0] = AssetDatabase.GetAssetPath(gameObjectRoot);
#else
#endif
						scriptRoot = gameObjectRoot.GetComponent<Script_SpriteStudio6_RootEffect>();
						if(null != scriptRoot)
						{
							limitParticleLimit = scriptRoot.LimitParticleDraw;
							flagHideForce = scriptRoot.FlagHideForce;
							rateTime = scriptRoot.RateTime;
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

					gameObjectRoot = Library_SpriteStudio6.Utility.Asset.GameObjectCreate(informationSSEE.NameFileBody, false, null);
					if(null == gameObjectRoot)
					{
						LogError(messageLogPrefix, "Failure to get Temporary-GameObject", informationSSEE.FileNameGetFullPath(), informationSSPJ);
						goto AssetCreatePrefab_ErrorEnd;
					}
					gameObjectRoot.name = informationSSEE.NameFileBody;	/* Give Root same name as SSEE */
					scriptRoot = gameObjectRoot.AddComponent<Script_SpriteStudio6_RootEffect>();
					if(null == scriptRoot)
					{
						LogError(messageLogPrefix, "Failure to add component\"Script_SpriteStudio6_RootEffect\"", informationSSEE.FileNameGetFullPath(), informationSSPJ);
						goto AssetCreatePrefab_ErrorEnd;
					}
					if((true == setting.Basic.FlagTakeOverLightRenderer) || (true == setting.Basic.FlagDisableInitialLightRenderer))
					{
						meshRenderer = gameObjectRoot.AddComponent<MeshRenderer>();
					}

					/* Datas Set */
					scriptRoot.DataCellMap = informationSSPJ.DataCellMapSS6PU.TableData[0];
					scriptRoot.DataEffect = informationSSEE.DataEffectSS6PU.TableData[0];
					scriptRoot.TableMaterial = informationSSPJ.TableMaterialEffectSS6PU;

					scriptRoot.LimitParticleDraw = limitParticleLimit;
					scriptRoot.FlagHideForce = flagHideForce;
					scriptRoot.RateTime = rateTime;

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
					informationSSEE.PrefabEffectSS6PU.TableData[0] = PrefabUtility.SaveAsPrefabAsset(gameObjectRoot, informationSSEE.PrefabEffectSS6PU.TableName[0]);
#else
					informationSSEE.PrefabEffectSS6PU.TableData[0] = PrefabUtility.ReplacePrefab(	gameObjectRoot,
																									informationSSEE.PrefabEffectSS6PU.TableData[0],
																									LibraryEditor_SpriteStudio6.Import.OptionPrefabReplace
																								);
#endif
					AssetDatabase.SaveAssets();

					/* Destroy Temporary */
					UnityEngine.Object.DestroyImmediate(gameObjectRoot);
					gameObjectRoot = null;

					return(true);

				AssetCreatePrefab_ErrorEnd:;
					return(false);
				}
#endif
				public static bool ConvertData(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
												LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
												LibraryEditor_SpriteStudio6.Import.SSEE.Information informationSSEE
											)
				{
					const string messageLogPrefix = "Convert (Data-Effect)";

					/* Rebuild Parts */
					/* MEMO: Effect data has only parts "Root" and "Emitter". */
					int countParts = informationSSEE.TableParts.Length;
					int countPartsRebuild = 0;
					int countEmitter = 0;
					int[] tableIndexPartsRebuild = new int[countParts];
					WorkAreaRebuildPartsConvertData[] workAreaRebuildParts = new WorkAreaRebuildPartsConvertData[countParts];
					for(int i=0; i<countParts; i++)
					{
						workAreaRebuildParts[i].CleanUp();

						tableIndexPartsRebuild[i] = -1;
					}
					for(int i=0; i<countParts; i++)
					{
						switch(informationSSEE.TableParts[i].Data.Feature)
						{
							case Library_SpriteStudio6.Data.Parts.Effect.KindFeature.ROOT:
								workAreaRebuildParts[countPartsRebuild].IndexParts = i;
								workAreaRebuildParts[countPartsRebuild].IndexPartsChildParicle = -1;
								workAreaRebuildParts[countPartsRebuild].IndexEmitter = -1;	/* Has no "Emitter" */

								tableIndexPartsRebuild[i] = countPartsRebuild;
								countPartsRebuild++;
								break;

							case Library_SpriteStudio6.Data.Parts.Effect.KindFeature.EMITTER:
								/* MEMO: "Emitter" always has only 1 child(definitely "Particle"). */
								workAreaRebuildParts[countPartsRebuild].IndexParts = i;
								workAreaRebuildParts[countPartsRebuild].IndexPartsChildParicle = informationSSEE.TableParts[i].Data.TableIDChild[0];
								workAreaRebuildParts[countPartsRebuild].IndexEmitter = countEmitter;
								countEmitter++;

								tableIndexPartsRebuild[i] = countPartsRebuild;
								countPartsRebuild++;
								break;

							case Library_SpriteStudio6.Data.Parts.Effect.KindFeature.PARTICLE:
								/* MEMO: Parent(Emitter) has already been set to "tableIndexPartsRebuild". */
								/* MEMO: "Particle" always has parent. */
								tableIndexPartsRebuild[i] = tableIndexPartsRebuild[informationSSEE.TableParts[i].Data.IDParent];
								break;

							default:
								continue;
						}
					}

					Information.Parts[] tablePartsNew = new Information.Parts[countPartsRebuild];
					Information.Parts partsNew = null; 
					Information.Parts partsOriginal = null;
					int indexPartsTemp;
					int countPartsChild;
					for(int i=0; i<countPartsRebuild; i++)
					{
						partsNew = new Information.Parts();
						partsNew.CleanUp();

						partsOriginal = informationSSEE.TableParts[workAreaRebuildParts[i].IndexParts];

						partsNew.Data.Name = string.Copy(partsOriginal.Data.Name);
						partsNew.Data.ID = i;
						indexPartsTemp = partsOriginal.Data.IDParent;
						partsNew.Data.IDParent = (-1 == indexPartsTemp) ? -1 : tableIndexPartsRebuild[indexPartsTemp];
						partsNew.Data.Feature = partsOriginal.Data.Feature;
						partsNew.Data.IndexEmitter = workAreaRebuildParts[i].IndexEmitter;

						partsNew.DataEmitter = partsOriginal.DataEmitter;
						indexPartsTemp = workAreaRebuildParts[i].IndexPartsChildParicle;	/* Sub-emitters are "Particle"'s children. */
						indexPartsTemp = (-1 == indexPartsTemp) ? workAreaRebuildParts[i].IndexParts : indexPartsTemp;
						countPartsChild = informationSSEE.TableParts[indexPartsTemp].Data.TableIDChild.Length;
						partsNew.Data.TableIDChild = new int[countPartsChild];
						for(int j=0; j<countPartsChild; j++)
						{
							partsNew.Data.TableIDChild[j] = tableIndexPartsRebuild[informationSSEE.TableParts[indexPartsTemp].Data.TableIDChild[j]];
						}

						partsNew.DataEmitter = partsOriginal.DataEmitter;

						tablePartsNew[i] = partsNew;
					}

					/* Tidy up & Create "Emitter"s for Runtime */
					WorkAreaBuildDrawOrderConvertData[] tableDrawOrderEmitter = new WorkAreaBuildDrawOrderConvertData[countEmitter];
					Library_SpriteStudio6.Data.Effect.Emitter[] tableDataEmitter = new Library_SpriteStudio6.Data.Effect.Emitter[countEmitter];
					Information.Parts.Emitter[] tableEmitter = new Information.Parts.Emitter[countEmitter];
					Information.Parts.Emitter emitterOriginal = null;
					for(int i=0; i<countEmitter; i++)
					{
						tableEmitter[i] = null;
						tableDrawOrderEmitter[i].CleanUp();
					}
					for(int i=0; i<countPartsRebuild; i++)
					{
						indexPartsTemp = workAreaRebuildParts[i].IndexEmitter;
						if(-1 == indexPartsTemp)
						{
							continue;
						}

						emitterOriginal = informationSSEE.TableParts[workAreaRebuildParts[i].IndexParts].DataEmitter;
						tableDataEmitter[indexPartsTemp].CleanUp();
						tableDataEmitter[indexPartsTemp] = emitterOriginal.Data;
						tableDataEmitter[indexPartsTemp].IndexCellMap = informationSSPJ.IndexGetCellMap(emitterOriginal.NameCellMap);
						if(-1 == tableDataEmitter[indexPartsTemp].IndexCellMap)
						{
							tableDataEmitter[indexPartsTemp].IndexCellMap = -1;
							tableDataEmitter[indexPartsTemp].IndexCell = -1;
						}
						else
						{
							tableDataEmitter[indexPartsTemp].IndexCell = informationSSPJ.TableInformationSSCE[tableDataEmitter[indexPartsTemp].IndexCellMap].IndexGetCell(emitterOriginal.NameCell);
							if(-1 == tableDataEmitter[indexPartsTemp].IndexCell)
							{
								tableDataEmitter[indexPartsTemp].IndexCellMap = -1;
								tableDataEmitter[indexPartsTemp].IndexCell = -1;
							}
						}

						/* Correct Priority */
						tableDrawOrderEmitter[indexPartsTemp].IndexEmitter = indexPartsTemp;
						tableDrawOrderEmitter[indexPartsTemp].PriorityParticle = emitterOriginal.PriorityParticle;
					}
					informationSSEE.TableEmitterSS6PU = tableDataEmitter;

					/* Tidy up & Create Parts for Runtime */
					Library_SpriteStudio6.Data.Parts.Effect[] tableDataParts = new Library_SpriteStudio6.Data.Parts.Effect[countPartsRebuild];
					for(int i=0; i<countPartsRebuild; i++)
					{
						tableDataParts[i].CleanUp();
						tableDataParts[i] = tablePartsNew[i].Data;
					}
					informationSSEE.TablePartsSS6PU = tableDataParts;

					/* Create Pattern-Offset & Emt-Pattern */
					if(false == ConvertDataCalculateInAdvance(ref setting, informationSSPJ, informationSSEE))
					{
						LogError(messageLogPrefix, "Failure to Generate PatternEmit Datas", informationSSEE.FileNameGetFullPath(), informationSSPJ);
						goto ConvertSS6PU_ErroeEnd;
					}

					/* Create Draw-Order */
					List<WorkAreaBuildDrawOrderConvertData> listDrawOrderEmitter = new List<WorkAreaBuildDrawOrderConvertData>();
					listDrawOrderEmitter.Clear();
					ConvertDataDrawOrderSortLayerParts(listDrawOrderEmitter, tableDrawOrderEmitter, informationSSEE.TablePartsSS6PU, 0);
					int countDrawOrderSort = listDrawOrderEmitter.Count;
					WorkAreaBuildDrawOrderConvertData tempDrawOrder;
					for(int i=0; i<(countDrawOrderSort - 1); i++)
					{
						for(int j=(countDrawOrderSort - 1); j>i; j--)
						{
							int k = j - 1;
							if(listDrawOrderEmitter[j].PriorityParticle < listDrawOrderEmitter[k].PriorityParticle)
							{
								tempDrawOrder = listDrawOrderEmitter[j];
								listDrawOrderEmitter[j] = listDrawOrderEmitter[k];
								listDrawOrderEmitter[k] = tempDrawOrder;
							}
						}
					}
					int[] tableIndexEmitterOrderDraw = new int[countDrawOrderSort];
					for(int i=0; i<countDrawOrderSort; i++)
					{
						tableIndexEmitterOrderDraw[i] = listDrawOrderEmitter[i].IndexEmitter;
					}
					informationSSEE.TableIndexEmitterOrderDrawSS6PU = tableIndexEmitterOrderDraw;
					tableIndexEmitterOrderDraw = null;
					tableDrawOrderEmitter = null;
					listDrawOrderEmitter.Clear();
					listDrawOrderEmitter = null;

					return(true);

				ConvertSS6PU_ErroeEnd:;
					return(false);
				}
				private static bool ConvertDataCalculateInAdvance(	ref LibraryEditor_SpriteStudio6.Import.Setting setting,
																	LibraryEditor_SpriteStudio6.Import.SSPJ.Information informationSSPJ,
																	LibraryEditor_SpriteStudio6.Import.SSEE.Information informationSSEE
																)
				{	/* MEMO: Use "Calculate In Advance" to distinguish from "Precalculate". */
					int countEmitter = informationSSEE.TableEmitterSS6PU.Length;
					bool flagLockSeed = false;
					uint seedRandom = 0;

#if !_FOR_SS6CONVERTER_
					Library_SpriteStudio6.Utility.Random.Generator random = Script_SpriteStudio6_RootEffect.InstanceCreateRandom();
#else
					/* MEMO: Provisionally, create directly. (Because I don't want to implement "Script_SpriteStudio6_RootEffect") */
					Library_SpriteStudio6.Utility.Random.Generator random = new Library_SpriteStudio6.Utility.Random.XorShift32();
#endif
					Library_SpriteStudio6.Data.Effect.Emitter.PatternEmit[] tablePatternEmit = null;
					long[] tableSeedParticle = null;
					for(int i=0; i<countEmitter; i++)
					{
						/* Fixed Random-Seed */
						flagLockSeed = false;
						if(0 != (informationSSEE.TableEmitterSS6PU[i].FlagData & Library_SpriteStudio6.Data.Effect.Emitter.FlagBit.SEEDRANDOM))
						{	/* Seed Overwrite */
							seedRandom = (uint)informationSSEE.TableEmitterSS6PU[i].SeedRandom + (uint)Library_SpriteStudio6.Data.Effect.Emitter.Constant.SEED_MAGIC;
							flagLockSeed = true;
						}
						else
						{
							if(true == informationSSEE.FlagLockSeed)
							{	/* Seed Locked */
								/* MEMO: Overwritten to the Effect's Seed. */
								seedRandom = ((uint)informationSSEE.SeedRandom + 1) * (uint)Library_SpriteStudio6.Data.Effect.Emitter.Constant.SEED_MAGIC;
								flagLockSeed = true;
							}
						}

						/* Calcurate Table-Offset-Pattern */
						informationSSEE.TableEmitterSS6PU[i].TableGetPatternOffset(ref informationSSEE.TableEmitterSS6PU[i].TablePatternOffset);

						/* Set to Data */
						if(true == flagLockSeed)
						{	/* Fixed Data */
							/* Calcurate Table-Datas */
							informationSSEE.TableEmitterSS6PU[i].TableGetPatternEmit(	ref tablePatternEmit,
																						ref tableSeedParticle,
																						random,
																						seedRandom
																					);
							informationSSEE.TableEmitterSS6PU[i].TablePatternEmit = tablePatternEmit;
							informationSSEE.TableEmitterSS6PU[i].TableSeedParticle = tableSeedParticle;
						}
						else
						{	/* Calculate on runtime */
							informationSSEE.TableEmitterSS6PU[i].TablePatternEmit = null;
							informationSSEE.TableEmitterSS6PU[i].TableSeedParticle = null;
						}
						tablePatternEmit = null;
						tableSeedParticle = null;
					}

					return(true);

//				ConvertDataCalculateInAdvance_ErrorEnd:;
//					return(false);
				}
				private static void ConvertDataDrawOrderSortLayerParts(	List<WorkAreaBuildDrawOrderConvertData> listOutput,
																		WorkAreaBuildDrawOrderConvertData[] tableInput,
																		Library_SpriteStudio6.Data.Parts.Effect[] tableParts,
																		int indexParts
																	)
				{
					/* MEMO: Need to add child emitters after parent emitters. */
					int[] indexPartsChild = tableParts[indexParts].TableIDChild;
					int indexEmitter;

					for(int i=0; i<indexPartsChild.Length; i++)
					{
						indexEmitter = tableParts[indexPartsChild[i]].IndexEmitter;
						listOutput.Add(tableInput[indexEmitter]);
					}

					for(int i=0; i<indexPartsChild.Length; i++)
					{
						ConvertDataDrawOrderSortLayerParts(listOutput, tableInput, tableParts, indexPartsChild[i]);
					}
				}
				#endregion Functions

				/* ----------------------------------------------- Classes, Structs & Interfaces */
				#region Classes, Structs & Interfaces
				private struct WorkAreaRebuildPartsConvertData
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public int IndexParts;
					public int IndexPartsChildParicle;	/* only Emitter */
					public int IndexEmitter;
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public void CleanUp()
					{
						IndexParts = -1;
						IndexPartsChildParicle = -1;
						IndexEmitter = -1;
					}
					#endregion Functions
				}

				private struct WorkAreaBuildDrawOrderConvertData
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public int IndexEmitter;
					public float PriorityParticle;
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public void CleanUp()
					{
						IndexEmitter = -1;
						PriorityParticle = float.NaN;
					}
					#endregion Functions
				}
				#endregion Classes, Structs & Interfaces
			}
			#endregion Classes, Structs & Interfaces
		}
	}
}
