/**
	SpriteStudio6 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using System.Collections;
using System.Collections.Generic;
#if !_FOR_SS6CONVERTER_
using UnityEngine;
#else
using SS6ConverterVer2_DLL;
using SS6ConverterVer2_DLL.Types;
using UnityEngine = SS6ConverterVer2_DLL.Assets;
#endif

public static partial class Library_SpriteStudio6
{
	public static partial class Data
	{
		public partial class Animation
		{
			public static partial class PackAttribute
			{
				/* ----------------------------------------------- Classes, Structs & Interfaces */
				#region Classes, Structs & Interfaces
				public static class CPE_Interpolate
				{
					/* ----------------------------------------------- Enums & Constants */
					#region Enums & Constants
					public readonly static Library_SpriteStudio6.Data.Animation.PackAttribute.CapacityContainer Capacity = new Library_SpriteStudio6.Data.Animation.PackAttribute.CapacityContainer(
						false,	/* Status */
						false,	/* Cell */
						true,	/* Position */
						true,	/* Rotation */
						true,	/* Scaling */
						true,	/* ScalingLocal */
						true,	/* RateOpacity */
						true,	/* Priority */
						true,	/* PartsColor */
						true,	/* VertexCorrection */
						true,	/* OffsetPivot */
						true,	/* PositionAnchor */
						true,	/* SizeForce */
						true,	/* PositionTexture */
						true,	/* RotationTexture */
						true,	/* ScalingTexture */
						true,	/* RadiusCollision */
						false,	/* UserData (Trigger) */
						false,	/* Instance (Trigger) */
						false,	/* Effect (Trigger) */
						true	/* Deform */
					);

					public const string ID = "CPE_Interpolate";

					internal readonly static InterfaceFunctionInt FunctionInt = new InterfaceFunctionInt();
					internal readonly static InterfaceFunctionFloat FunctionFloat = new InterfaceFunctionFloat();
					internal readonly static InterfaceFunctionVector2 FunctionVector2 = new InterfaceFunctionVector2();
					internal readonly static InterfaceFunctionVector3 FunctionVector3 = new InterfaceFunctionVector3();
//					internal readonly static InterfaceFunctionStatus FunctionStatus = new InterfaceFunctionStatus();
//					internal readonly static InterfaceFunctionCell FunctionCell = new InterfaceFunctionCell();
					internal readonly static InterfaceFunctionPartsColor FunctionPartsColor = new InterfaceFunctionPartsColor();
					internal readonly static InterfaceFunctionVertexCorrection FunctionVertexCorrection = new InterfaceFunctionVertexCorrection();
//					internal readonly static InterfaceFunctionUserData FunctionUserData = new InterfaceFunctionUserData();
//					internal readonly static InterfaceFunctionInstance FunctionInstance = new InterfaceFunctionInstance();
//					internal readonly static InterfaceFunctionEffect FunctionEffect = new InterfaceFunctionEffect();
					internal readonly static InterfaceFunctionDeform FunctionDeform = new InterfaceFunctionDeform();

					[System.Flags]
					private enum FlagBit
					{
						FRAMEKEY = 0x00007fff,
						INDEX = 0x07ff8000,
						FORMULA = 0x38000000,

						CLEAR = 0x00000000,
					}
					private enum FlagBitShift
					{
						FRAMEKEY = 0,
						INDEX = 15,
						FORMULA = 27,
					}
					private enum KindFormula
					{
						LINEAR = 0,
						ACCELERATE,
						DECELERATE,

						_RESERVED_04,
						_RESERVED_03,
						_RESERVED_02,
						_RESERVED_01,

						CPE = 7,	/* -1 (Raw) */
					}
					private enum KindElementVector
					{
						X = 0,
						Y,
						Z,
						W
					}
					#endregion Enums & Constants

					/* ----------------------------------------------- Dictionary-Functions */
					#region Dictionary-Functions
					public static bool DictionaryBootUp(int indexAnimation, int indexParts, Script_SpriteStudio6_DataAnimation dataAnimation)
					{
						/* MEMO: Have no external dictionary. */
						return(true);
					}

					public static bool DictionaryShutDown(int indexAnimation, int indexParts, Script_SpriteStudio6_DataAnimation dataAnimation)
					{
						/* MEMO: Have no external dictionary. */
						return(true);
					}
					#endregion Dictionary-Functions

					/* ----------------------------------------------- Classes, Structs & Interfaces */
					#region Classes, Structs & Interfaces
					public class InterfaceFunctionInt : Library_SpriteStudio6.Data.Animation.PackAttribute.InterfaceContainerInt
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public bool ValueGet(	ref int outValue,
												ref int outFrameKey,
												Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInt container,
												ref Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer argument
											)
						{
							if(0 >= container.TableCodeValue.Length)
							{
								return(false);
							}
							int frame = argument.Frame;
							if((0 <= outFrameKey) && (frame == argument.FramePrevious))
							{
								return(false);
							}
							if(true == Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.IntGetInt(ref outValue, container.TableCodeValue[0].TableCode, container.TableValue, frame, outFrameKey))
							{
								outFrameKey = frame;
								return(true);
							}
							return(false);
						}

						public bool ValueGetIndex(	ref int outValue,
													ref int outFrameKey,
													int index,
													Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInt container,
													ref Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer argument
												)
						{
							/* MEMO: Not Support */
							/* MEMO: Originally need to implemented, but since this function is used only for "UserData", */
							/*       there is no problem at present. (This format does not support to "UserData")         */
							return(false);
						}

						public int CountGetValue(Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInt container)
						{
							/* MEMO: Not Support */
							/* MEMO: Originally need to implemented, but since this function is used only for "UserData", */
							/*       there is no problem at present. (This format does not support to "UserData")         */
							return(-1);
						}

						public bool Pack(	Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInt container,
											string nameAttribute,
											int countFrame,
											Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus flagStatusParts,
											int[] tableOrderDraw,
											int[] tableOrderPreDraw,
											params Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeInt[] listKeyData
										)
						{	/* MEMO: "listKeyData.Length" is always 1 */
							/* MEMO: Get values that have undergone dedicated processing and inheriting for each attribute. */
							if(0 >= countFrame)
							{
								container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[0];
								container.TableValue = new int[0];
								return(true);
							}
							container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[1];	/* Always 1 */

							Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInt dataUncompressed = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInt();
							dataUncompressed.TypePack = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_UNCOMPRESSED;
							Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionInt(dataUncompressed);
							dataUncompressed.Function.Pack(dataUncompressed, nameAttribute, countFrame, flagStatusParts, tableOrderDraw, tableOrderPreDraw, listKeyData);	/* Always 1 */

							bool flagSuccess = true;
							List<int> listValue = new List<int>(countFrame);
							listValue.Clear();
							flagSuccess &= Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.CompressInt(	out container.TableCodeValue[0].TableCode,
																															listValue,
																															dataUncompressed.TableValue,
																															listKeyData[0]	/* Always 1 */
																														);
							container.TableValue = listValue.ToArray();
							return(flagSuccess);
						}
						#endregion Functions
					}

					public class InterfaceFunctionFloat : Library_SpriteStudio6.Data.Animation.PackAttribute.InterfaceContainerFloat
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public bool ValueGet(	ref float outValue,
												ref int outFrameKey,
												Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerFloat container,
												ref Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer argument
											)
						{
							if(0 >= container.TableCodeValue.Length)
							{
								return(false);
							}
							int frame = argument.Frame;
							if((0 <= outFrameKey) && (frame == argument.FramePrevious))
							{
								return(false);
							}
							if(true == Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.FloatGetFloat(ref outValue, container.TableCodeValue[0].TableCode, container.TableValue, frame, outFrameKey))
							{
								outFrameKey = frame;
								return(true);
							}
							return(false);
						}

						public bool ValueGetIndex(	ref float outValue,
													ref int outFrameKey,
													int index,
													Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerFloat container,
													ref Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer argument
												)
						{
							/* MEMO: Not Support */
							/* MEMO: Originally need to implemented, but since this function is used only for "UserData", */
							/*       there is no problem at present. (This format does not support to "UserData")         */
							return(false);
						}

						public int CountGetValue(Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerFloat container)
						{
							/* MEMO: Not Support */
							/* MEMO: Originally need to implemented, but since this function is used only for "UserData", */
							/*       there is no problem at present. (This format does not support to "UserData")         */
							return(-1);
						}

						public bool Pack(	Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerFloat container,
											string nameAttribute,
											int countFrame,
											Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus flagStatusParts,
											int[] tableOrderDraw,
											int[] tableOrderPreDraw,
											params Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat[] listKeyData
										)
						{	/* MEMO: "listKeyData.Length" is always 1 */
							/* MEMO: Get values that have undergone dedicated processing and inheriting for each attribute. */
							if(0 >= countFrame)
							{
								container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[0];
								container.TableValue = new float[0];
								return(true);
							}
							container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[1];	/* Always 1 */

							Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerFloat dataUncompressed = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerFloat();
							dataUncompressed.TypePack = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_UNCOMPRESSED;
							Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionFloat(dataUncompressed);
							dataUncompressed.Function.Pack(dataUncompressed, nameAttribute, countFrame, flagStatusParts, tableOrderDraw, tableOrderPreDraw, listKeyData);	/* Always 1 */

							bool flagSuccess = true;
							switch(nameAttribute)
							{
								case Library_SpriteStudio6.Data.Animation.Attribute.Importer.NameAttributeRateOpacity:
									{	/* MEMO: This format can not store "inheritance" data. */
										/* MEMO: Forcibly change to "Standard CPE" format. */
										container.TypePack = KindTypePack.STANDARD_CPE;
										flagSuccess &= Library_SpriteStudio6.Data.Animation.PackAttribute.StandardCPE.Compress(	out container.TableCodeValue,
																																out container.TableValue,
																																dataUncompressed.TableValue
																															);
									}
									break;

								default:
									{
										List<float> listValue = new List<float>(countFrame);
										listValue.Clear();
										flagSuccess &= Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.CompressFloat(	out container.TableCodeValue[0].TableCode,
																																			listValue,
																																			dataUncompressed.TableValue,
																																			listKeyData[0]	/* Always 1 */
																																	);
										container.TableValue = listValue.ToArray();
									}
									break;
							}
							return(flagSuccess);
						}
						#endregion Functions
					}

					public class InterfaceFunctionVector2 : Library_SpriteStudio6.Data.Animation.PackAttribute.InterfaceContainerVector2
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public bool ValueGet(	ref Vector2 outValue,
												ref int outFrameKey,
												Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2 container,
												ref Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer argument
											)
						{
							if(0 >= container.TableCodeValue.Length)
							{
								return(false);
							}
							int frame = argument.Frame;
							if((0 <= outFrameKey) && (frame == argument.FramePrevious))
							{
								return(false);
							}
							bool flagUpdated = false;
							flagUpdated |= Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.FloatGetVector2(ref outValue.x, container.TableCodeValue[(int)KindElementVector.X].TableCode, container.TableValue, frame, outFrameKey);
							flagUpdated |= Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.FloatGetVector2(ref outValue.y, container.TableCodeValue[(int)KindElementVector.Y].TableCode, container.TableValue, frame, outFrameKey);
							if(true == flagUpdated)
							{
								outFrameKey = frame;
								return(true);
							}
							return(false);
						}

						public bool ValueGetIndex(	ref Vector2 outValue,
													ref int outFrameKey,
													int index,
													Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2 container,
													ref Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer argument
												)
						{
							/* MEMO: Not Support */
							/* MEMO: Originally need to implemented, but since this function is used only for "UserData", */
							/*       there is no problem at present. (This format does not support to "UserData")         */
							return(false);
						}

						public int CountGetValue(Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2 container)
						{
							/* MEMO: Not Support */
							/* MEMO: Originally need to implemented, but since this function is used only for "UserData", */
							/*       there is no problem at present. (This format does not support to "UserData")         */
							return(-1);
						}

						public bool Pack(	Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2 container,
											string nameAttribute,
											int countFrame,
											Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus flagStatusParts,
											int[] tableOrderDraw,
											int[] tableOrderPreDraw,
											params Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat[] listKeyData
										)
						{	/* MEMO: "listKeyData.Length" is always 2 (X, Y) *//* MEMO: No inheritance is related to attribute stored in this type. */
							/* MEMO: Get values that have undergone dedicated processing and inheriting for each attribute. */
							if(0 >= countFrame)
							{
								container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[0];
								container.TableValue = new Vector2[0];
								return(true);
							}
							container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[(int)KindElementVector.Y + 1];

							Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2 dataUncompressed = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2();
							dataUncompressed.TypePack = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_UNCOMPRESSED;
							Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVector2(dataUncompressed);
							dataUncompressed.Function.Pack(dataUncompressed, nameAttribute, countFrame, flagStatusParts, tableOrderDraw, tableOrderPreDraw, listKeyData);

							bool flagSuccess = true;
							List<Vector2> listValue = new List<Vector2>(countFrame);
							listValue.Clear();
							int elementVectorNext = (int)KindElementVector.X;
							flagSuccess &= Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.CompressVector2(	out container.TableCodeValue[(int)KindElementVector.X].TableCode,
																																listValue,
																																ref elementVectorNext,
																																dataUncompressed.TableValue,
																																listKeyData[0],	/* Source .x */
																																(int)KindElementVector.X
																															);
							flagSuccess &= Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.CompressVector2(	out container.TableCodeValue[(int)KindElementVector.Y].TableCode,
																																listValue,
																																ref elementVectorNext,
																																dataUncompressed.TableValue,
																																listKeyData[1],	/* Source .y */
																																(int)KindElementVector.Y
																															);
							container.TableValue = listValue.ToArray();

							return(flagSuccess);
						}
						#endregion Functions
					}

					public class InterfaceFunctionVector3 : Library_SpriteStudio6.Data.Animation.PackAttribute.InterfaceContainerVector3
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public bool ValueGet(	ref Vector3 outValue,
												ref int outFrameKey,
												Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3 container,
												ref Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer argument
											)
						{
							if(0 >= container.TableCodeValue.Length)
							{
								return(false);
							}
							int frame = argument.Frame;
							if((0 <= outFrameKey) && (frame == argument.FramePrevious))
							{
								return(false);
							}
							bool flagUpdated = false;
							flagUpdated |= Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.FloatGetVector3(ref outValue.x, container.TableCodeValue[(int)KindElementVector.X].TableCode, container.TableValue, frame, outFrameKey);
							flagUpdated |= Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.FloatGetVector3(ref outValue.y, container.TableCodeValue[(int)KindElementVector.Y].TableCode, container.TableValue, frame, outFrameKey);
							flagUpdated |= Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.FloatGetVector3(ref outValue.z, container.TableCodeValue[(int)KindElementVector.Z].TableCode, container.TableValue, frame, outFrameKey);
							if(true == flagUpdated)
							{
								outFrameKey = frame;
								return(true);
							}
							return(false);
						}

						public bool ValueGetIndex(	ref Vector3 outValue,
													ref int outFrameKey,
													int index,
													Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3 container,
													ref Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer argument
												)
						{
							/* MEMO: Not Support */
							/* MEMO: Originally need to implemented, but since this function is used only for "UserData", */
							/*       there is no problem at present. (This format does not support to "UserData")         */
							return(false);
						}

						public int CountGetValue(Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3 container)
						{
							/* MEMO: Not Support */
							/* MEMO: Originally need to implemented, but since this function is used only for "UserData", */
							/*       there is no problem at present. (This format does not support to "UserData")         */
							return(-1);
						}

						public bool Pack(	Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3 container,
											string nameAttribute,
											int countFrame,
											Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus flagStatusParts,
											int[] tableOrderDraw,
											int[] tableOrderPreDraw,
											params Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat[] listKeyData
										)
						{	/* MEMO: "listKeyData.Length" is always 3 (X, Y, Z) */
							/* MEMO: Get values that have undergone dedicated processing and inheriting for each attribute. */
							if(0 >= countFrame)
							{
								container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[0];
								container.TableValue = new Vector3[0];
								return(true);
							}
							container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[(int)KindElementVector.Z + 1];

							Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3 dataUncompressed = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3();
							dataUncompressed.TypePack = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_UNCOMPRESSED;
							Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVector3(dataUncompressed);
							dataUncompressed.Function.Pack(dataUncompressed, nameAttribute, countFrame, flagStatusParts, tableOrderDraw, tableOrderPreDraw, listKeyData);

							bool flagSuccess = true;
							List<Vector3> listValue = new List<Vector3>(countFrame);
							listValue.Clear();
							int elementVectorNext = (int)KindElementVector.X;
							flagSuccess &= Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.CompressVector3(	out container.TableCodeValue[(int)KindElementVector.X].TableCode,
																																listValue,
																																ref elementVectorNext,
																																dataUncompressed.TableValue,
																																listKeyData[0],	/* Source .x */
																																(int)KindElementVector.X
																															);
							flagSuccess &= Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.CompressVector3(	out container.TableCodeValue[(int)KindElementVector.Y].TableCode,
																																listValue,
																																ref elementVectorNext,
																																dataUncompressed.TableValue,
																																listKeyData[1],	/* Source .y */
																																(int)KindElementVector.Y
																															);
							flagSuccess &= Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.CompressVector3(	out container.TableCodeValue[(int)KindElementVector.Z].TableCode,
																																listValue,
																																ref elementVectorNext,
																																dataUncompressed.TableValue,
																																listKeyData[2],	/* Source .z */
																																(int)KindElementVector.Z
																															);
							container.TableValue = listValue.ToArray();

							return(flagSuccess);
						}
						#endregion Functions
					}

					public class InterfaceFunctionPartsColor : Library_SpriteStudio6.Data.Animation.PackAttribute.InterfaceContainerPartsColor
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public bool ValueGet(	ref Library_SpriteStudio6.Data.Animation.Attribute.PartsColor outValue,
												ref int outFrameKey,
												Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerPartsColor container,
												ref Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer argument
											)
						{
							if(0 >= container.TableCodeValue.Length)
							{
								return(false);
							}
							int frame = argument.Frame;
							if((0 <= outFrameKey) && (frame == argument.FramePrevious))
							{
								return(false);
							}
							if(true == Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.ValueGetPartsColor(ref outValue, container.TableCodeValue[0].TableCode, container.TableValue, frame, outFrameKey))
							{
								outFrameKey = frame;
								return(true);
							}
							return(false);
						}

						public bool ValueGetIndex(	ref Library_SpriteStudio6.Data.Animation.Attribute.PartsColor outValue,
													ref int outFrameKey,
													int index,
													Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerPartsColor container,
													ref Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer argument
												)
						{
							/* MEMO: Not Support */
							/* MEMO: Originally need to implemented, but since this function is used only for "UserData", */
							/*       there is no problem at present. (This format does not support to "UserData")         */
							return(false);
						}

						public int CountGetValue(Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerPartsColor container)
						{
							/* MEMO: Not Support */
							/* MEMO: Originally need to implemented, but since this function is used only for "UserData", */
							/*       there is no problem at present. (This format does not support to "UserData")         */
							return(-1);
						}

						public bool Pack(	Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerPartsColor container,
											string nameAttribute,
											int countFrame,
											Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus flagStatusParts,
											int[] tableOrderDraw,
											int[] tableOrderPreDraw,
											params Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributePartsColor[] listKeyData
										)
						{	/* MEMO: "listKeyData.Length" is always 1 */
							/* MEMO: Get values that have undergone dedicated processing and inheriting for each attribute. */
							if(0 >= countFrame)
							{
								container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[0];
								container.TableValue = new Library_SpriteStudio6.Data.Animation.Attribute.PartsColor[0];
								return(true);
							}
							container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[1];	/* Always 1 */

							Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerPartsColor dataUncompressed = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerPartsColor();
							dataUncompressed.TypePack = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_UNCOMPRESSED;
							Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionPartsColor(dataUncompressed);
							dataUncompressed.Function.Pack(dataUncompressed, nameAttribute, countFrame, flagStatusParts, tableOrderDraw, tableOrderPreDraw, listKeyData);	/* Always 1 */

							bool flagSuccess = true;
							List<Library_SpriteStudio6.Data.Animation.Attribute.PartsColor> listValue = new List<Library_SpriteStudio6.Data.Animation.Attribute.PartsColor>(countFrame);
							listValue.Clear();
							flagSuccess &= Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.CompressPartsColor(	out container.TableCodeValue[0].TableCode,
																																	listValue,
																																	dataUncompressed.TableValue,
																																	listKeyData[0]	/* Always 1 */
																																);
							container.TableValue = listValue.ToArray();

							return(flagSuccess);
						}
						#endregion Functions
					}

					public class InterfaceFunctionVertexCorrection : Library_SpriteStudio6.Data.Animation.PackAttribute.InterfaceContainerVertexCorrection
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public bool ValueGet(	ref Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection outValue,
												ref int outFrameKey,
												Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVertexCorrection container,
												ref Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer argument
											)
						{
							if(0 >= container.TableCodeValue.Length)
							{
								return(false);
							}
							int frame = argument.Frame;
							if((0 <= outFrameKey) && (frame == argument.FramePrevious))
							{
								return(false);
							}
							if(true == Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.ValueGetVertexCorrection(ref outValue, container.TableCodeValue[0].TableCode, container.TableValue, frame, outFrameKey))
							{
								outFrameKey = frame;
								return(true);
							}
							return(false);
						}

						public bool ValueGetIndex(	ref Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection outValue,
													ref int outFrameKey,
													int index,
													Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVertexCorrection container,
													ref Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer argument
												)
						{
							/* MEMO: Not Support */
							/* MEMO: Originally need to implemented, but since this function is used only for "UserData", */
							/*       there is no problem at present. (This format does not support to "UserData")         */
							return(false);
						}

						public int CountGetValue(Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVertexCorrection container)
						{
							/* MEMO: Not Support */
							/* MEMO: Originally need to implemented, but since this function is used only for "UserData", */
							/*       there is no problem at present. (This format does not support to "UserData")         */
							return(-1);
						}

						public bool Pack(	Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVertexCorrection container,
											string nameAttribute,
											int countFrame,
											Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus flagStatusParts,
											int[] tableOrderDraw,
											int[] tableOrderPreDraw,
											params Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeVertexCorrection[] listKeyData
										)
						{	/* MEMO: "listKeyData.Length" is always 1 */
							/* MEMO: Get values that have undergone dedicated processing and inheriting for each attribute. */
							if(0 >= countFrame)
							{
								container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[0];
								container.TableValue = new Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection[0];
								return(true);
							}
							container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[1];	/* Always 1 */

							Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVertexCorrection dataUncompressed = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVertexCorrection();
							dataUncompressed.TypePack = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_UNCOMPRESSED;
							Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVertexCorrection(dataUncompressed);
							dataUncompressed.Function.Pack(dataUncompressed, nameAttribute, countFrame, flagStatusParts, tableOrderDraw, tableOrderPreDraw, listKeyData);	/* Always 1 */

							bool flagSuccess = true;
							List<Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection> listValue = new List<Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection>(countFrame);
							listValue.Clear();
							flagSuccess &= Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.CompressVertexCorrection(	out container.TableCodeValue[0].TableCode,
																																		listValue,
																																		dataUncompressed.TableValue,
																																		listKeyData[0]	/* Always 1 */
																																	);
							container.TableValue = listValue.ToArray();

							return(flagSuccess);
						}
						#endregion Functions
					}

					public class InterfaceFunctionDeform : Library_SpriteStudio6.Data.Animation.PackAttribute.InterfaceContainerDeform
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public bool ValueGet(	ref Library_SpriteStudio6.Data.Animation.Attribute.Deform outValue,
												ref int outFrameKey,
												Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerDeform container,
												ref Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer argument
											)
						{
							if(0 >= container.TableCodeValue.Length)
							{
								return(false);
							}
							int countVertexMesh = container.CountVertexMesh;
							if(0 >= countVertexMesh)
							{
								return(false);	/* outValue is not overwritten. */
							}
							int frame = argument.Frame;
							if((0 <= outFrameKey) && (frame == argument.FramePrevious))
							{
								return(false);
							}
							/* MEMO: Must be "outValue.TableCoordinate.Length == container.CountVertexMesh". */
							if((null == outValue.TableCoordinate) || (countVertexMesh > outValue.TableCoordinate.Length))
							{	/* Error */
								return(false);	/* outValue is not overwritten. */
							}

							if(true == Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.ValueGetDeform(ref outValue, container.TableCodeValue[0].TableCode, container.TableValue, container, frame, outFrameKey))
							{
								outFrameKey = frame;
								return(true);
							}
							return(false);
						}

						public bool ValueGetIndex(	ref Library_SpriteStudio6.Data.Animation.Attribute.Deform outValue,
													ref int outFrameKey,
													int index,
													Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerDeform container,
													ref Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer argument
												)
						{
							/* MEMO: Not Support */
							/* MEMO: Originally need to implemented, but since this function is used only for "UserData", */
							/*       there is no problem at present. (This format does not support to "UserData")         */
							return(false);
						}

						public int CountGetValue(Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerDeform container)
						{
							/* MEMO: Not Support */
							/* MEMO: Originally need to implemented, but since this function is used only for "UserData", */
							/*       there is no problem at present. (This format does not support to "UserData")         */
							return(-1);
						}

						public bool Pack(	Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerDeform container,
											string nameAttribute,
											int countFrame,
											Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus flagStatusParts,
											int[] tableOrderDraw,
											int[] tableOrderPreDraw,
											params Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeDeform[] listKeyData
										)
						{	/* MEMO: "listKeyData.Length" is always 1 */
							/* MEMO: Get values that have undergone dedicated processing and inheriting for each attribute. */
							if(0 >= countFrame)
							{
								container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[0];
								container.TableValue = new Library_SpriteStudio6.Data.Animation.Attribute.Deform[0];
								return(true);
							}
							container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[1];	/* Always 1 */

							Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerDeform dataUncompressed = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerDeform();
							dataUncompressed.TypePack = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_UNCOMPRESSED;
							Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionDeform(dataUncompressed);
							dataUncompressed.Function.Pack(dataUncompressed, nameAttribute, countFrame, flagStatusParts, tableOrderDraw, tableOrderPreDraw, listKeyData);	/* Always 1 */

							/* MEMO: Copy data except for each frame data. */
							container.CountVertexMesh = dataUncompressed.CountVertexMesh;
							container.TableIndexVertex = dataUncompressed.TableIndexVertex;

							bool flagSuccess = true;
							List<Library_SpriteStudio6.Data.Animation.Attribute.Deform> listValue = new List<Library_SpriteStudio6.Data.Animation.Attribute.Deform>(countFrame);
							listValue.Clear();
							flagSuccess &= Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.CompressDeform(	out container.TableCodeValue[0].TableCode,
																																listValue,
																																dataUncompressed.TableValue,
																																listKeyData[0]	/* Always 1 */
																															);
							container.TableValue = listValue.ToArray();

							return(flagSuccess);
						}
						#endregion Functions
					}
					#endregion Classes, Structs & Interfaces

					/* ----------------------------------------------- Functions */
					#region Functions
					public static bool IntGetInt(ref int outValue, int[] tableStatus, int[] tableValue, int frame, int framePrevious)
					{
#if UNITY_EDITOR
						if((null == tableValue) || (null == tableStatus))
						{
							/* MEMO: May reach before deserialization direct-after import. */
							return(false);
						}
#endif
						if(0 >= tableStatus.Length)
						{
							return(false);
						}

						KindFormula formula;
						int frameKey = -1;
						int frameKeyEnd;
						int indexStatusLast = tableStatus.Length - 1;
						int status;
						int statusEnd;
						int indexMinimum = 0;
						int indexMaximum = indexStatusLast;
						int index;
						int indexEnd;
						float rate;
						float value;
						float valueEnd;
						while(indexMinimum != indexMaximum)
						{
							index = indexMinimum + indexMaximum;
							index = (index >> 1) + (index & 1);	/* (index / 2) + (index % 2) */
							frameKey = tableStatus[index] & (int)FlagBit.FRAMEKEY;
							if(frame == frameKey)
							{
								indexMinimum = indexMaximum = index;
							}
							else
							{
								if((frame < frameKey) || (-1 == frameKey))
								{
									indexMaximum = index - 1;
								}
								else
								{
									indexMinimum = index;
								}
							}
						}

						status = tableStatus[indexMinimum];
						frameKey = status & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
						index = (status & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
						if(indexStatusLast <= indexMinimum)
						{	/* Not Interpolate */
							formula = KindFormula.CPE;
						}
						else
						{
							formula = (KindFormula)((status & (int)FlagBit.FORMULA) >> (int)FlagBitShift.FORMULA);
						}

						switch(formula)
						{
							case KindFormula.LINEAR:
								value = (float)tableValue[index];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = (float)tableValue[indexEnd];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);

								/* outValue = Library_SpriteStudio6.Utility.Interpolation.Linear(value, valueEnd, rate); */
								outValue = (int)(((valueEnd - value) * rate) + value);

								return(true);	/* Updated */

							case KindFormula.ACCELERATE:
								value = (float)tableValue[index];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = (float)tableValue[indexEnd];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);

								/* outValue = Library_SpriteStudio6.Utility.Interpolation.Accelerate(value, valueEnd, rate); */
								rate *= rate;
								outValue = (int)(((valueEnd - value) * rate) + value);

								return(true);	/* Updated */

							case KindFormula.DECELERATE:
								value = (float)tableValue[index];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = (float)tableValue[indexEnd];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);

								/* outValue = Library_SpriteStudio6.Utility.Interpolation.Decelerate(value, valueEnd, rate); */
								rate = 1.0f - rate;
								rate *= rate;
								rate = 1.0f - rate;
								outValue = (int)(((valueEnd - value) * rate) + value);

								return(true);	/* Updated */

							case KindFormula._RESERVED_04:
								break;
							case KindFormula._RESERVED_03:
								break;
							case KindFormula._RESERVED_02:
								break;
							case KindFormula._RESERVED_01:
								break;

							case KindFormula.CPE:
								if(framePrevious == frameKey)
								{
									return(false);	/* Not Updated. */
								}

								outValue = tableValue[index];
								return(true);	/* Updated */
						}

						return(false);
					}

					public static bool FloatGetFloat(ref float outValue, int[] tableStatus, float[] tableValue, int frame, int framePrevious)
					{
#if UNITY_EDITOR
						if((null == tableValue) || (null == tableStatus))
						{
							/* MEMO: May reach before deserialization direct-after import. */
							return(false);
						}
#endif
						if(0 >= tableStatus.Length)
						{
							return(false);
						}

						KindFormula formula;
						int frameKey = -1;
						int frameKeyEnd;
						int indexStatusLast = tableStatus.Length - 1;
						int status;
						int statusEnd;
						int indexMinimum = 0;
						int indexMaximum = indexStatusLast;
						int index;
						int indexEnd;
						float rate;
						float value;
						float valueEnd;
						while(indexMinimum != indexMaximum)
						{
							index = indexMinimum + indexMaximum;
							index = (index >> 1) + (index & 1);	/* (index / 2) + (index % 2) */
							frameKey = tableStatus[index] & (int)FlagBit.FRAMEKEY;
							if(frame == frameKey)
							{
								indexMinimum = indexMaximum = index;
							}
							else
							{
								if((frame < frameKey) || (-1 == frameKey))
								{
									indexMaximum = index - 1;
								}
								else
								{
									indexMinimum = index;
								}
							}
						}

						status = tableStatus[indexMinimum];
						frameKey = status & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
						index = (status & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
						if(indexStatusLast <= indexMinimum)
						{	/* Not Interpolate */
							formula = KindFormula.CPE;
						}
						else
						{
							formula = (KindFormula)((status & (int)FlagBit.FORMULA) >> (int)FlagBitShift.FORMULA);
						}

						switch(formula)
						{
							case KindFormula.LINEAR:
								value = tableValue[index];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = tableValue[indexEnd];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);

								/* outValue = Library_SpriteStudio6.Utility.Interpolation.Linear(value, valueEnd, rate); */
								outValue = ((valueEnd - value) * rate) + value;

								return(true);	/* Updated */

							case KindFormula.ACCELERATE:
								value = tableValue[index];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = tableValue[indexEnd];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);

								/* outValue = Library_SpriteStudio6.Utility.Interpolation.Accelerate(value, valueEnd, rate); */
								rate *= rate;
								outValue = ((valueEnd - value) * rate) + value;

								return(true);	/* Updated */

							case KindFormula.DECELERATE:
								value = tableValue[index];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = tableValue[indexEnd];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);

								/* outValue = Library_SpriteStudio6.Utility.Interpolation.Decelerate(value, valueEnd, rate); */
								rate = 1.0f - rate;
								rate *= rate;
								rate = 1.0f - rate;
								outValue = ((valueEnd - value) * rate) + value;

								return(true);	/* Updated */

							case KindFormula._RESERVED_04:
								break;
							case KindFormula._RESERVED_03:
								break;
							case KindFormula._RESERVED_02:
								break;
							case KindFormula._RESERVED_01:
								break;

							case KindFormula.CPE:
								if(framePrevious == frameKey)
								{
									return(false);	/* Not Updated. */
								}

								outValue = tableValue[index];
								return(true);	/* Updated */
						}

						return(false);
					}

					public static bool FloatGetVector2(ref float outValue, int[] tableStatus, Vector2[] tableValue, int frame, int framePrevious)
					{
#if UNITY_EDITOR
						if((null == tableValue) || (null == tableStatus))
						{
							/* MEMO: May reach before deserialization direct-after import. */
							return(false);
						}
#endif
						if(0 >= tableStatus.Length)
						{
							return(false);
						}

						KindFormula formula;
						int frameKey = -1;
						int frameKeyEnd;
						int indexStatusLast = tableStatus.Length - 1;
						int status;
						int statusEnd;
						int indexMinimum = 0;
						int indexMaximum = indexStatusLast;
						int index;
						int indexEnd;
						float rate;
						float value;
						float valueEnd;
						while(indexMinimum != indexMaximum)
						{
							index = indexMinimum + indexMaximum;
							index = (index >> 1) + (index & 1);	/* (index / 2) + (index % 2) */
							frameKey = tableStatus[index] & (int)FlagBit.FRAMEKEY;
							if(frame == frameKey)
							{
								indexMinimum = indexMaximum = index;
							}
							else
							{
								if((frame < frameKey) || (-1 == frameKey))
								{
									indexMaximum = index - 1;
								}
								else
								{
									indexMinimum = index;
								}
							}
						}

						status = tableStatus[indexMinimum];
						frameKey = status & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
						index = (status & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
						if(indexStatusLast <= indexMinimum)
						{	/* Not Interpolate */
							formula = KindFormula.CPE;
						}
						else
						{
							formula = (KindFormula)((status & (int)FlagBit.FORMULA) >> (int)FlagBitShift.FORMULA);
						}

						switch(formula)
						{
							case KindFormula.LINEAR:
								value = tableValue[(index / 2)][(index % 2)];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = tableValue[indexEnd / 2][indexEnd % 2];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);

								/* outValue = Library_SpriteStudio6.Utility.Interpolation.Linear(value, valueEnd, rate); */
								outValue = ((valueEnd - value) * rate) + value;

								return(true);	/* Updated */

							case KindFormula.ACCELERATE:
								value = tableValue[(index / 2)][(index % 2)];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = tableValue[indexEnd / 2][indexEnd % 2];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);

								/* outValue = Library_SpriteStudio6.Utility.Interpolation.Accelerate(value, valueEnd, rate); */
								rate *= rate;
								outValue = ((valueEnd - value) * rate) + value;

								return(true);	/* Updated */

							case KindFormula.DECELERATE:
								value = tableValue[(index / 2)][(index % 2)];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = tableValue[indexEnd / 2][indexEnd % 2];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);

								/* outValue = Library_SpriteStudio6.Utility.Interpolation.Decelerate(value, valueEnd, rate); */
								rate = 1.0f - rate;
								rate *= rate;
								rate = 1.0f - rate;
								outValue = ((valueEnd - value) * rate) + value;

								return(true);	/* Updated */

							case KindFormula._RESERVED_04:
								break;
							case KindFormula._RESERVED_03:
								break;
							case KindFormula._RESERVED_02:
								break;
							case KindFormula._RESERVED_01:
								break;

							case KindFormula.CPE:
								/* MEMO: Always outputs a value since Vector2 also is tupple. */
								outValue = tableValue[(index / 2)][(index % 2)];

								if(framePrevious == frameKey)
								{
									return(false);	/* Not Updated. */
								}
								return(true);	/* Updated */
						}

						return(false);
					}

					public static bool FloatGetVector3(ref float outValue, int[] tableStatus, Vector3[] tableValue, int frame, int framePrevious)
					{
#if UNITY_EDITOR
						if((null == tableValue) || (null == tableStatus))
						{
							/* MEMO: May reach before deserialization direct-after import. */
							return(false);
						}
#endif
						if(0 >= tableStatus.Length)
						{
							return(false);
						}

						KindFormula formula;
						int frameKey = -1;
						int frameKeyEnd;
						int indexStatusLast = tableStatus.Length - 1;
						int status;
						int statusEnd;
						int indexMinimum = 0;
						int indexMaximum = indexStatusLast;
						int index;
						int indexEnd;
						float rate;
						float value;
						float valueEnd;
						while(indexMinimum != indexMaximum)
						{
							index = indexMinimum + indexMaximum;
							index = (index >> 1) + (index & 1);	/* (index / 2) + (index % 2) */
							frameKey = tableStatus[index] & (int)FlagBit.FRAMEKEY;
							if(frame == frameKey)
							{
								indexMinimum = indexMaximum = index;
							}
							else
							{
								if((frame < frameKey) || (-1 == frameKey))
								{
									indexMaximum = index - 1;
								}
								else
								{
									indexMinimum = index;
								}
							}
						}

						status = tableStatus[indexMinimum];
						frameKey = status & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
						index = (status & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
						if(indexStatusLast <= indexMinimum)
						{	/* Not Interpolate */
							formula = KindFormula.CPE;
						}
						else
						{
							formula = (KindFormula)((status & (int)FlagBit.FORMULA) >> (int)FlagBitShift.FORMULA);
						}

						switch(formula)
						{
							case KindFormula.LINEAR:
								value = tableValue[(index / 3)][(index % 3)];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = tableValue[indexEnd / 3][indexEnd % 3];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);

								/* outValue = Library_SpriteStudio6.Utility.Interpolation.Linear(value, valueEnd, rate); */
								outValue = ((valueEnd - value) * rate) + value;
								return(true);	/* Updated */

							case KindFormula.ACCELERATE:
								value = tableValue[(index / 3)][(index % 3)];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = tableValue[indexEnd / 3][indexEnd % 3];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);

								/* outValue = Library_SpriteStudio6.Utility.Interpolation.Accelerate(value, valueEnd, rate); */
								rate *= rate;
								outValue = ((valueEnd - value) * rate) + value;

								return(true);	/* Updated */

							case KindFormula.DECELERATE:
								value = tableValue[(index / 3)][(index % 3)];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = tableValue[indexEnd / 3][indexEnd % 3];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);

								/* outValue = Library_SpriteStudio6.Utility.Interpolation.Decelerate(value, valueEnd, rate); */
								rate = 1.0f - rate;
								rate *= rate;
								rate = 1.0f - rate;
								outValue = ((valueEnd - value) * rate) + value;

								return(true);	/* Updated */

							case KindFormula._RESERVED_04:
								break;
							case KindFormula._RESERVED_03:
								break;
							case KindFormula._RESERVED_02:
								break;
							case KindFormula._RESERVED_01:
								break;

							case KindFormula.CPE:
								if(framePrevious == frameKey)
								{
									return(false);	/* Not Updated. */
								}

								outValue = tableValue[(index / 3)][(index % 3)];
								return(true);	/* Updated */
						}

						return(false);
					}

					public static bool ValueGetPartsColor(	ref Library_SpriteStudio6.Data.Animation.Attribute.PartsColor outValue,
															int[] tableStatus,
															Library_SpriteStudio6.Data.Animation.Attribute.PartsColor[] tableValue,
															int frame,
															int framePrevious
														)
					{
#if UNITY_EDITOR
						if((null == tableValue) || (null == tableStatus))
						{
							/* MEMO: May reach before deserialization direct-after import. */
							return(false);
						}
#endif
						if(0 >= tableStatus.Length)
						{
							return(false);
						}

						KindFormula formula;
						int frameKey = -1;
						int frameKeyEnd;
						int indexStatusLast = tableStatus.Length - 1;
						int status;
						int statusEnd;
						int indexMinimum = 0;
						int indexMaximum = indexStatusLast;
						int index;
						int indexEnd;
						float rate;
						Library_SpriteStudio6.Data.Animation.Attribute.PartsColor value;
						Library_SpriteStudio6.Data.Animation.Attribute.PartsColor valueEnd;
						Color colorVertex;
						float alhpaVertex;
						while(indexMinimum != indexMaximum)
						{
							index = indexMinimum + indexMaximum;
							index = (index >> 1) + (index & 1);	/* (index / 2) + (index % 2) */
							frameKey = tableStatus[index] & (int)FlagBit.FRAMEKEY;
							if(frame == frameKey)
							{
								indexMinimum = indexMaximum = index;
							}
							else
							{
								if((frame < frameKey) || (-1 == frameKey))
								{
									indexMaximum = index - 1;
								}
								else
								{
									indexMinimum = index;
								}
							}
						}

						status = tableStatus[indexMinimum];
						frameKey = status & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
						index = (status & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
						if(indexStatusLast <= indexMinimum)
						{	/* Not Interpolate */
							formula = KindFormula.CPE;
						}
						else
						{
							formula = (KindFormula)((status & (int)FlagBit.FORMULA) >> (int)FlagBitShift.FORMULA);
						}

						/* MEMO: Since this process is called many times, formula-functions are        */
						/*        inlined without using "Library_SpriteStudio6.Utility.Interpolation". */
						/*       (Optimizing for speed.)                                               */
						switch(formula)
						{
							case KindFormula.LINEAR:
								value = tableValue[index];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = tableValue[indexEnd];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);
								outValue.Operation = value.Operation;
								outValue.Bound = value.Bound;

								colorVertex = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU];
								outValue.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU] = ((valueEnd.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU] - colorVertex) * rate) + colorVertex;
								colorVertex = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RU];
								outValue.VertexColor[(int)Library_SpriteStudio6.KindVertex.RU] = ((valueEnd.VertexColor[(int)Library_SpriteStudio6.KindVertex.RU] - colorVertex) * rate) + colorVertex;
								colorVertex = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RD];
								outValue.VertexColor[(int)Library_SpriteStudio6.KindVertex.RD] = ((valueEnd.VertexColor[(int)Library_SpriteStudio6.KindVertex.RD] - colorVertex) * rate) + colorVertex;
								colorVertex = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LD];
								outValue.VertexColor[(int)Library_SpriteStudio6.KindVertex.LD] = ((valueEnd.VertexColor[(int)Library_SpriteStudio6.KindVertex.LD] - colorVertex) * rate) + colorVertex;

								alhpaVertex = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU];
								outValue.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU] = ((valueEnd.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU] - alhpaVertex) * rate) + alhpaVertex;
								alhpaVertex = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RU];
								outValue.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RU] = ((valueEnd.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RU] - alhpaVertex) * rate) + alhpaVertex;
								alhpaVertex = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RD];
								outValue.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RD] = ((valueEnd.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RD] - alhpaVertex) * rate) + alhpaVertex;
								alhpaVertex = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LD];
								outValue.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LD] = ((valueEnd.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LD] - alhpaVertex) * rate) + alhpaVertex;

								return(true);	/* Updated */

							case KindFormula.ACCELERATE:
								value = tableValue[index];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = tableValue[indexEnd];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);
								rate *= rate;
								outValue.Operation = value.Operation;
								outValue.Bound = value.Bound;

								colorVertex = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU];
								outValue.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU] = ((valueEnd.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU] - colorVertex) * rate) + colorVertex;
								colorVertex = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RU];
								outValue.VertexColor[(int)Library_SpriteStudio6.KindVertex.RU] = ((valueEnd.VertexColor[(int)Library_SpriteStudio6.KindVertex.RU] - colorVertex) * rate) + colorVertex;
								colorVertex = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RD];
								outValue.VertexColor[(int)Library_SpriteStudio6.KindVertex.RD] = ((valueEnd.VertexColor[(int)Library_SpriteStudio6.KindVertex.RD] - colorVertex) * rate) + colorVertex;
								colorVertex = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LD];
								outValue.VertexColor[(int)Library_SpriteStudio6.KindVertex.LD] = ((valueEnd.VertexColor[(int)Library_SpriteStudio6.KindVertex.LD] - colorVertex) * rate) + colorVertex;

								alhpaVertex = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU];
								outValue.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU] = ((valueEnd.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU] - alhpaVertex) * rate) + alhpaVertex;
								alhpaVertex = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RU];
								outValue.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RU] = ((valueEnd.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RU] - alhpaVertex) * rate) + alhpaVertex;
								alhpaVertex = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RD];
								outValue.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RD] = ((valueEnd.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RD] - alhpaVertex) * rate) + alhpaVertex;
								alhpaVertex = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LD];
								outValue.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LD] = ((valueEnd.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LD] - alhpaVertex) * rate) + alhpaVertex;

								return(true);	/* Updated */

							case KindFormula.DECELERATE:
								value = tableValue[index];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = tableValue[indexEnd];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);
								rate = 1.0f - rate;
								rate *= rate;
								rate = 1.0f - rate;
								outValue.Operation = value.Operation;
								outValue.Bound = value.Bound;

								colorVertex = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU];
								outValue.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU] = ((valueEnd.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU] - colorVertex) * rate) + colorVertex;
								colorVertex = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RU];
								outValue.VertexColor[(int)Library_SpriteStudio6.KindVertex.RU] = ((valueEnd.VertexColor[(int)Library_SpriteStudio6.KindVertex.RU] - colorVertex) * rate) + colorVertex;
								colorVertex = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RD];
								outValue.VertexColor[(int)Library_SpriteStudio6.KindVertex.RD] = ((valueEnd.VertexColor[(int)Library_SpriteStudio6.KindVertex.RD] - colorVertex) * rate) + colorVertex;
								colorVertex = value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LD];
								outValue.VertexColor[(int)Library_SpriteStudio6.KindVertex.LD] = ((valueEnd.VertexColor[(int)Library_SpriteStudio6.KindVertex.LD] - colorVertex) * rate) + colorVertex;

								alhpaVertex = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU];
								outValue.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU] = ((valueEnd.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU] - alhpaVertex) * rate) + alhpaVertex;
								alhpaVertex = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RU];
								outValue.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RU] = ((valueEnd.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RU] - alhpaVertex) * rate) + alhpaVertex;
								alhpaVertex = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RD];
								outValue.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RD] = ((valueEnd.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RD] - alhpaVertex) * rate) + alhpaVertex;
								alhpaVertex = value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LD];
								outValue.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LD] = ((valueEnd.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LD] - alhpaVertex) * rate) + alhpaVertex;

								return(true);	/* Updated */

							case KindFormula._RESERVED_04:
								break;
							case KindFormula._RESERVED_03:
								break;
							case KindFormula._RESERVED_02:
								break;
							case KindFormula._RESERVED_01:
								break;

							case KindFormula.CPE:
								if(framePrevious == frameKey)
								{
									return(false);	/* Not Updated. */
								}

								/* MEMO: Even if has reference to array, since caller manages buffer, no problem to shallow copy. */
								outValue = tableValue[index];
								return(true);	/* Updated */
						}

						return(false);
					}

					public static bool ValueGetVertexCorrection(	ref Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection outValue,
																	int[] tableStatus,
																	Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection[] tableValue,
																	int frame,
																	int framePrevious
																)
					{
#if UNITY_EDITOR
						if((null == tableValue) || (null == tableStatus))
						{
							/* MEMO: May reach before deserialization direct-after import. */
							return(false);
						}
#endif
						if(0 >= tableStatus.Length)
						{
							return(false);
						}

						KindFormula formula;
						int frameKey = -1;
						int frameKeyEnd;
						int indexStatusLast = tableStatus.Length - 1;
						int status;
						int statusEnd;
						int indexMinimum = 0;
						int indexMaximum = indexStatusLast;
						int index;
						int indexEnd;
						float rate;
						Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection value;
						Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection valueEnd;
						Vector2 coordinate;
						while(indexMinimum != indexMaximum)
						{
							index = indexMinimum + indexMaximum;
							index = (index >> 1) + (index & 1);	/* (index / 2) + (index % 2) */
							frameKey = tableStatus[index] & (int)FlagBit.FRAMEKEY;
							if(frame == frameKey)
							{
								indexMinimum = indexMaximum = index;
							}
							else
							{
								if((frame < frameKey) || (-1 == frameKey))
								{
									indexMaximum = index - 1;
								}
								else
								{
									indexMinimum = index;
								}
							}
						}

						status = tableStatus[indexMinimum];
						frameKey = status & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
						index = (status & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
						if(indexStatusLast <= indexMinimum)
						{	/* Not Interpolate */
							formula = KindFormula.CPE;
						}
						else
						{
							formula = (KindFormula)((status & (int)FlagBit.FORMULA) >> (int)FlagBitShift.FORMULA);
						}

						/* MEMO: Since this process is called many times, formula-functions are        */
						/*        inlined without using "Library_SpriteStudio6.Utility.Interpolation". */
						/*       (Optimizing for speed.)                                               */
						switch(formula)
						{
							case KindFormula.LINEAR:
								value = tableValue[index];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = tableValue[indexEnd];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);
								coordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU];
								outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU] = ((valueEnd.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU] - coordinate) * rate) + coordinate;
								coordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU];
								outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU] = ((valueEnd.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU] - coordinate) * rate) + coordinate;
								coordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD];
								outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD] = ((valueEnd.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD] - coordinate) * rate) + coordinate;
								coordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD];
								outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD] = ((valueEnd.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD] - coordinate) * rate) + coordinate;

								return(true);	/* Updated */

							case KindFormula.ACCELERATE:
								value = tableValue[index];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = tableValue[indexEnd];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);
								rate *= rate;
								coordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU];
								outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU] = ((valueEnd.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU] - coordinate) * rate) + coordinate;
								coordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU];
								outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU] = ((valueEnd.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU] - coordinate) * rate) + coordinate;
								coordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD];
								outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD] = ((valueEnd.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD] - coordinate) * rate) + coordinate;
								coordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD];
								outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD] = ((valueEnd.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD] - coordinate) * rate) + coordinate;

								return(true);	/* Updated */

							case KindFormula.DECELERATE:
								value = tableValue[index];

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								valueEnd = tableValue[indexEnd];

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);
								rate = 1.0f - rate;
								rate *= rate;
								rate = 1.0f - rate;
								coordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU];
								outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU] = ((valueEnd.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU] - coordinate) * rate) + coordinate;
								coordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU];
								outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU] = ((valueEnd.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU] - coordinate) * rate) + coordinate;
								coordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD];
								outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD] = ((valueEnd.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD] - coordinate) * rate) + coordinate;
								coordinate = value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD];
								outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD] = ((valueEnd.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD] - coordinate) * rate) + coordinate;

								return(true);	/* Updated */

							case KindFormula._RESERVED_04:
								break;
							case KindFormula._RESERVED_03:
								break;
							case KindFormula._RESERVED_02:
								break;
							case KindFormula._RESERVED_01:
								break;

							case KindFormula.CPE:
								if(framePrevious == frameKey)
								{
									return(false);	/* Not Updated. */
								}

								/* MEMO: Even if has reference to array, since caller manages buffer, no problem to shallow copy. */
								outValue = tableValue[index];
								return(true);	/* Updated */
						}

						return(false);
					}

					public static bool ValueGetDeform(	ref Library_SpriteStudio6.Data.Animation.Attribute.Deform outValue,
														int[] tableStatus,
														Library_SpriteStudio6.Data.Animation.Attribute.Deform[] tableValue,
														Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerDeform container,
														int frame,
														int framePrevious
														
													)
					{
#if UNITY_EDITOR
						if((null == tableValue) || (null == tableStatus))
						{
							/* MEMO: May reach before deserialization direct-after import. */
							return(false);
						}
#endif
						if(0 >= tableStatus.Length)
						{
							return(false);
						}

						KindFormula formula;
						int frameKey = -1;
						int frameKeyEnd;
						int indexStatusLast = tableStatus.Length - 1;
						int status;
						int statusEnd;
						int indexMinimum = 0;
						int indexMaximum = indexStatusLast;
						int index;
						int indexEnd;
						float rate;
						Vector2[] tableCoordinateStart;
						Vector2[] tableCoordinateEnd;
						while(indexMinimum != indexMaximum)
						{
							index = indexMinimum + indexMaximum;
							index = (index >> 1) + (index & 1);	/* (index / 2) + (index % 2) */
							frameKey = tableStatus[index] & (int)FlagBit.FRAMEKEY;
							if(frame == frameKey)
							{
								indexMinimum = indexMaximum = index;
							}
							else
							{
								if((frame < frameKey) || (-1 == frameKey))
								{
									indexMaximum = index - 1;
								}
								else
								{
									indexMinimum = index;
								}
							}
						}

						status = tableStatus[indexMinimum];
						frameKey = status & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
						index = (status & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
						if(indexStatusLast <= indexMinimum)
						{	/* Not Interpolate */
							formula = KindFormula.CPE;
						}
						else
						{
							formula = (KindFormula)((status & (int)FlagBit.FORMULA) >> (int)FlagBitShift.FORMULA);
						}

						/* MEMO: Since this process is called many times, formula-functions are        */
						/*        inlined without using "Library_SpriteStudio6.Utility.Interpolation". */
						/*       (Optimizing for speed.)                                               */
						int[] tableIndexVertex = container.TableIndexVertex;
						int countVertexChange = tableIndexVertex.Length;
						Vector2[] tableCoordinateOutput = outValue.TableCoordinate;
						Vector2 coordinate;
						switch(formula)
						{
							case KindFormula.LINEAR:
								tableCoordinateStart = tableValue[index].TableCoordinate;

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								tableCoordinateEnd = tableValue[indexEnd].TableCoordinate;

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);
								for(int i=0; i<countVertexChange; i++)
								{
									coordinate = tableCoordinateStart[i];
									tableCoordinateOutput[tableIndexVertex[i]] = ((tableCoordinateEnd[i] - coordinate) * rate) + coordinate;
								}

								return(true);	/* Updated */

							case KindFormula.ACCELERATE:
								tableCoordinateStart = tableValue[index].TableCoordinate;

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								tableCoordinateEnd = tableValue[indexEnd].TableCoordinate;

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);
								rate *= rate;
								for(int i=0; i<countVertexChange; i++)
								{
									coordinate = tableCoordinateStart[i];
									tableCoordinateOutput[tableIndexVertex[i]] = ((tableCoordinateEnd[i] - coordinate) * rate) + coordinate;
								}

								return(true);	/* Updated */

							case KindFormula.DECELERATE:
								tableCoordinateStart = tableValue[index].TableCoordinate;

								indexMinimum++;
								statusEnd = tableStatus[indexMinimum];
								frameKeyEnd = statusEnd & (int)FlagBit.FRAMEKEY;	/* >> (int)FlagBitShift.FRAMEKEY; */
								indexEnd = (statusEnd & (int)FlagBit.INDEX) >> (int)FlagBitShift.INDEX;
								tableCoordinateEnd = tableValue[indexEnd].TableCoordinate;

								rate = (float)(frame - frameKey) / (float)(frameKeyEnd - frameKey);
								rate = 1.0f - rate;
								rate *= rate;
								rate = 1.0f - rate;
								for(int i=0; i<countVertexChange; i++)
								{
									coordinate = tableCoordinateStart[i];
									tableCoordinateOutput[tableIndexVertex[i]] = ((tableCoordinateEnd[i] - coordinate) * rate) + coordinate;
								}

								return(true);	/* Updated */

							case KindFormula._RESERVED_04:
								break;
							case KindFormula._RESERVED_03:
								break;
							case KindFormula._RESERVED_02:
								break;
							case KindFormula._RESERVED_01:
								break;

							case KindFormula.CPE:
								if(framePrevious == frameKey)
								{
									return(false);	/* Not Updated. */
								}

								/* MEMO: Even if has reference to array, since caller manages buffer, no problem to shallow copy. */
								tableCoordinateStart = tableValue[index].TableCoordinate;
								for(int i=0; i<countVertexChange; i++)
								{
									tableCoordinateOutput[tableIndexVertex[i]] = tableCoordinateStart[i];
								}
								return(true);	/* Updated */
						}

						return(false);
					}

					public static bool CompressInt(	out int[] tableCodeValue,
													List<int> listValue,
													int[] tableValueUncompressed,
													Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeInt listKeyData
												)
					{
						/* MEMO: Do not use "listKeyData.ListKey[n].Value".       */
						/*       (default-value and "Inheritance" are not solved) */
						int countFrame = tableValueUncompressed.Length;
						int countKeyData = listKeyData.CountGetKey();
						if(0 >= countKeyData)
						{	/* No key-data */
							/* MEMO: Do not mess "listValue". */
							tableCodeValue = new int[0];
							return(true);
						}

						List<int> listStatus = new List<int>(countFrame);
						listStatus.Clear();

						int index;
						int frame;
						int frameNext;
						int status;
						int value;
						Library_SpriteStudio6.Utility.Interpolation.KindFormula formulaSource;
						KindFormula formula;
						for(int i=0; i<countKeyData; i++)
						{
							if((countKeyData - 1) <= i)
							{	/* Doesn't have next key */
								formulaSource = Utility.Interpolation.KindFormula.NON;
								frame = listKeyData.ListKey[i].Frame;
								frameNext = countFrame;

								i = countKeyData;	/* Force end */
							}
							else
							{
								formulaSource = listKeyData.ListKey[i].Formula;
								frame = listKeyData.ListKey[i].Frame;
								frameNext = listKeyData.ListKey[i + 1].Frame;
							}
							if(countFrame <= frame)
							{	/* Error */
								break;
							}

							/* Set data each formula */
							switch(formulaSource)
							{
								/* Type: Interpolate */
								case Utility.Interpolation.KindFormula.LINEAR:
									formula = KindFormula.LINEAR;

									value = tableValueUncompressed[frame];
									index = ListSetValue(listValue, value);

									if(tableValueUncompressed[frameNext] == value)
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								case Utility.Interpolation.KindFormula.ACCELERATE:
									formula = KindFormula.ACCELERATE;

									value = tableValueUncompressed[frame];
									index = ListSetValue(listValue, value);

									if(tableValueUncompressed[frameNext] == value)
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								case Utility.Interpolation.KindFormula.DECELERATE:
									formula = KindFormula.DECELERATE;

									value = tableValueUncompressed[frame];
									index = ListSetValue(listValue, value);

									if(tableValueUncompressed[frameNext] == value)
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								/* Type: CPE */
								case Utility.Interpolation.KindFormula.NON:
								case Utility.Interpolation.KindFormula.HERMITE:
								case Utility.Interpolation.KindFormula.BEZIER:
									formula = KindFormula.CPE;

									value = int.MinValue;
									for(int j=frame; j<frameNext; j++)
									{
										if(tableValueUncompressed[j] != value)
										{
											value = tableValueUncompressed[j];
											index = ListSetValue(listValue, value);

											status = StatusGet(formula, index, j);
											listStatus.Add(status);
										}
									}
									break;
							}
						}

						tableCodeValue = listStatus.ToArray();
						listStatus.Clear();
						listStatus = null;

						return(true);
					}
					public static bool CompressFloat(	out int[] tableCodeValue,
														List<float> listValue,
														float[] tableValueUncompressed,
														Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat listKeyData
													)
					{
						int countFrame = tableValueUncompressed.Length;
						int countKeyData = listKeyData.CountGetKey();
						if(0 >= countKeyData)
						{	/* No key-data */
							/* MEMO: Do not mess "listValue". */
							tableCodeValue = new int[0];
							return(true);
						}

						List<int> listStatus = new List<int>(countFrame);
						listStatus.Clear();

						int index;
						int frame;
						int frameNext;
						int status;
						float value;
						Library_SpriteStudio6.Utility.Interpolation.KindFormula formulaSource;
						KindFormula formula;
						for(int i=0; i<countKeyData; i++)
						{
							if((countKeyData - 1) <= i)
							{	/* Doesn't have next key */
								formulaSource = Utility.Interpolation.KindFormula.NON;
								frame = listKeyData.ListKey[i].Frame;
								frameNext = countFrame;

								i = countKeyData;	/* Force end */
							}
							else
							{
								formulaSource = listKeyData.ListKey[i].Formula;
								frame = listKeyData.ListKey[i].Frame;
								frameNext = listKeyData.ListKey[i + 1].Frame;
							}
							if(countFrame <= frame)
							{	/* Error */
								break;
							}

							/* Set data each formula */
							switch(formulaSource)
							{
								/* Type: Interpolate */
								case Utility.Interpolation.KindFormula.LINEAR:
									formula = KindFormula.LINEAR;

									value = tableValueUncompressed[frame];
									index = ListSetValue(listValue, value);

									if(tableValueUncompressed[frameNext] == value)
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								case Utility.Interpolation.KindFormula.ACCELERATE:
									formula = KindFormula.ACCELERATE;

									value = tableValueUncompressed[frame];
									index = ListSetValue(listValue, value);

									if(tableValueUncompressed[frameNext] == value)
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								case Utility.Interpolation.KindFormula.DECELERATE:
									formula = KindFormula.DECELERATE;

									value = tableValueUncompressed[frame];
									index = ListSetValue(listValue, value);

									if(tableValueUncompressed[frameNext] == value)
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								/* Type: CPE */
								case Utility.Interpolation.KindFormula.NON:
								case Utility.Interpolation.KindFormula.HERMITE:
								case Utility.Interpolation.KindFormula.BEZIER:
									formula = KindFormula.CPE;

									value = float.NaN;
									for(int j=frame; j<frameNext; j++)
									{
										if(tableValueUncompressed[j] != value)
										{
											value = tableValueUncompressed[j];
											index = ListSetValue(listValue, value);

											status = StatusGet(formula, index, j);
											listStatus.Add(status);
										}
									}
									break;
							}
						}

						tableCodeValue = listStatus.ToArray();
						listStatus.Clear();
						listStatus = null;

						return(true);
					}
					public static bool CompressVector2(	out int[] tableCodeValue,
														List<Vector2> listValue,
														ref int elementVectorNext, 
														Vector2[] tableValueUncompressed,
														Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat listKeyData,
														int elementVectorKeyData
													)
					{
						int countFrame = tableValueUncompressed.Length;
						int countKeyData = listKeyData.CountGetKey();
						if(0 >= countKeyData)
						{	/* No key-data */
							/* MEMO: Do not mess "listValue" and "elementVectorNext". */
							tableCodeValue = new int[0];
							return(true);
						}

						List<int> listStatus = new List<int>(countFrame);
						listStatus.Clear();

						int index;
						int frame;
						int frameNext;
						int status;
						float value;
						Library_SpriteStudio6.Utility.Interpolation.KindFormula formulaSource;
						KindFormula formula;
						for(int i=0; i<countKeyData; i++)
						{
							if((countKeyData - 1) <= i)
							{	/* Doesn't have next key */
								formulaSource = Utility.Interpolation.KindFormula.NON;
								frame = listKeyData.ListKey[i].Frame;
								frameNext = countFrame;

								i = countKeyData;	/* Force end */
							}
							else
							{
								formulaSource = listKeyData.ListKey[i].Formula;
								frame = listKeyData.ListKey[i].Frame;
								frameNext = listKeyData.ListKey[i + 1].Frame;
							}
							if(countFrame <= frame)
							{	/* Error */
								break;
							}

							/* Set data each formula */
							switch(formulaSource)
							{
								/* Type: Interpolate */
								case Utility.Interpolation.KindFormula.LINEAR:
									formula = KindFormula.LINEAR;

									value = tableValueUncompressed[frame][elementVectorKeyData];
									index = ListSetValueVector2(listValue, ref elementVectorNext, value);

									if(tableValueUncompressed[frameNext][elementVectorKeyData] == value)
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								case Utility.Interpolation.KindFormula.ACCELERATE:
									formula = KindFormula.ACCELERATE;

									value = tableValueUncompressed[frame][elementVectorKeyData];
									index = ListSetValueVector2(listValue, ref elementVectorNext, value);

									if(tableValueUncompressed[frameNext][elementVectorKeyData] == value)
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								case Utility.Interpolation.KindFormula.DECELERATE:
									formula = KindFormula.DECELERATE;

									value = tableValueUncompressed[frame][elementVectorKeyData];
									index = ListSetValueVector2(listValue, ref elementVectorNext, value);

									if(tableValueUncompressed[frameNext][elementVectorKeyData] == value)
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								/* Type: CPE */
								case Utility.Interpolation.KindFormula.NON:
								case Utility.Interpolation.KindFormula.HERMITE:
								case Utility.Interpolation.KindFormula.BEZIER:
									formula = KindFormula.CPE;

									value = float.NaN;
									for(int j=frame; j<frameNext; j++)
									{
										if(tableValueUncompressed[j][elementVectorKeyData] != value)
										{
											value = tableValueUncompressed[j][elementVectorKeyData];
											index = ListSetValueVector2(listValue, ref elementVectorNext, value);

											status = StatusGet(formula, index, j);
											listStatus.Add(status);
										}
									}
									break;
							}
						}

						tableCodeValue = listStatus.ToArray();
						listStatus.Clear();
						listStatus = null;

						return(true);
					}
					public static bool CompressVector3(	out int[] tableCodeValue,
														List<Vector3> listValue,
														ref int elementVectorNext, 
														Vector3[] tableValueUncompressed,
														Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat listKeyData,
														int elementVectorKeyData
													)
					{
						int countFrame = tableValueUncompressed.Length;
						int countKeyData = listKeyData.CountGetKey();
						if(0 >= countKeyData)
						{	/* No key-data */
							/* MEMO: Do not mess "listValue" and "elementVectorNext". */
							tableCodeValue = new int[0];
							return(true);
						}

						List<int> listStatus = new List<int>(countFrame);
						listStatus.Clear();

						int index;
						int frame;
						int frameNext;
						int status;
						float value;
						Library_SpriteStudio6.Utility.Interpolation.KindFormula formulaSource;
						KindFormula formula;
						for(int i=0; i<countKeyData; i++)
						{
							if((countKeyData - 1) <= i)
							{	/* Doesn't have next key */
								formulaSource = Utility.Interpolation.KindFormula.NON;
								frame = listKeyData.ListKey[i].Frame;
								frameNext = countFrame;

								i = countKeyData;	/* Force end */
							}
							else
							{
								formulaSource = listKeyData.ListKey[i].Formula;
								frame = listKeyData.ListKey[i].Frame;
								frameNext = listKeyData.ListKey[i + 1].Frame;
							}
							if(countFrame <= frame)
							{	/* Error */
								break;
							}

							/* Set data each formula */
							switch(formulaSource)
							{
								/* Type: Interpolate */
								case Utility.Interpolation.KindFormula.LINEAR:
									formula = KindFormula.LINEAR;

									value = tableValueUncompressed[frame][elementVectorKeyData];
									index = ListSetValueVector3(listValue, ref elementVectorNext, value);

									if(tableValueUncompressed[frameNext][elementVectorKeyData] == value)
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								case Utility.Interpolation.KindFormula.ACCELERATE:
									formula = KindFormula.ACCELERATE;

									value = tableValueUncompressed[frame][elementVectorKeyData];
									index = ListSetValueVector3(listValue, ref elementVectorNext, value);

									if(tableValueUncompressed[frameNext][elementVectorKeyData] == value)
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								case Utility.Interpolation.KindFormula.DECELERATE:
									formula = KindFormula.DECELERATE;

									value = tableValueUncompressed[frame][elementVectorKeyData];
									index = ListSetValueVector3(listValue, ref elementVectorNext, value);

									if(tableValueUncompressed[frameNext][elementVectorKeyData] == value)
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								/* Type: CPE */
								case Utility.Interpolation.KindFormula.NON:
								case Utility.Interpolation.KindFormula.HERMITE:
								case Utility.Interpolation.KindFormula.BEZIER:
									formula = KindFormula.CPE;

									value = int.MinValue;
									for(int j=frame; j<frameNext; j++)
									{
										if(tableValueUncompressed[j][elementVectorKeyData] != value)
										{
											value = tableValueUncompressed[j][elementVectorKeyData];
											index = ListSetValueVector3(listValue, ref elementVectorNext, value);

											status = StatusGet(formula, index, j);
											listStatus.Add(status);
										}
									}
									break;
							}
						}

						tableCodeValue = listStatus.ToArray();
						listStatus.Clear();
						listStatus = null;

						return(true);
					}
					public static bool CompressPartsColor(	out int[] tableCodeValue,
															List<Library_SpriteStudio6.Data.Animation.Attribute.PartsColor> listValue,
															Library_SpriteStudio6.Data.Animation.Attribute.PartsColor[] tableValueUncompressed,
															Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributePartsColor listKeyData
														)
					{
						int countFrame = tableValueUncompressed.Length;
						int countKeyData = listKeyData.CountGetKey();
						if(0 >= countKeyData)
						{	/* No key-data */
							/* MEMO: Do not mess "listValue". */
							tableCodeValue = new int[0];
							return(true);
						}

						List<int> listStatus = new List<int>(countFrame);
						listStatus.Clear();

						int index;
						int frame;
						int frameNext;
						int status;
						Library_SpriteStudio6.Data.Animation.Attribute.PartsColor value = new Library_SpriteStudio6.Data.Animation.Attribute.PartsColor();
						Library_SpriteStudio6.Utility.Interpolation.KindFormula formulaSource;
						KindFormula formula;
						for(int i=0; i<countKeyData; i++)
						{
							if((countKeyData - 1) <= i)
							{	/* Doesn't have next key */
								formulaSource = Utility.Interpolation.KindFormula.NON;
								frame = listKeyData.ListKey[i].Frame;
								frameNext = countFrame;

								i = countKeyData;	/* Force end */
							}
							else
							{
								formulaSource = listKeyData.ListKey[i].Formula;
								frame = listKeyData.ListKey[i].Frame;
								frameNext = listKeyData.ListKey[i + 1].Frame;
							}
							if(countFrame <= frame)
							{	/* Error */
								break;
							}

							/* Set data each formula */
							switch(formulaSource)
							{
								/* Type: Interpolate */
								case Utility.Interpolation.KindFormula.LINEAR:
									formula = KindFormula.LINEAR;

									value = tableValueUncompressed[frame];
									index = ListSetValue(listValue, value);

									if(true == value.Equals(tableValueUncompressed[frameNext]))
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								case Utility.Interpolation.KindFormula.ACCELERATE:
									formula = KindFormula.ACCELERATE;

									value = tableValueUncompressed[frame];
									index = ListSetValue(listValue, value);

									if(true == value.Equals(tableValueUncompressed[frameNext]))
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								case Utility.Interpolation.KindFormula.DECELERATE:
									formula = KindFormula.DECELERATE;

									value = tableValueUncompressed[frame];
									index = ListSetValue(listValue, value);

									if(true == value.Equals(tableValueUncompressed[frameNext]))
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								/* Type: CPE */
								case Utility.Interpolation.KindFormula.NON:
								case Utility.Interpolation.KindFormula.HERMITE:
								case Utility.Interpolation.KindFormula.BEZIER:
									formula = KindFormula.CPE;

									value.CleanUp();
									for(int j=frame; j<frameNext; j++)
									{
										if(	(null == value.VertexColor)	/* Not set */
											|| (false == value.Equals(tableValueUncompressed[j]))
											)
										{
											value = tableValueUncompressed[j];
											index = ListSetValue(listValue, value);

											status = StatusGet(formula, index, j);
											listStatus.Add(status);
										}
									}
									break;
							}
						}

						tableCodeValue = listStatus.ToArray();
						listStatus.Clear();
						listStatus = null;

						return(true);
					}
					public static bool CompressVertexCorrection(	out int[] tableCodeValue,
																	List<Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection> listValue,
																	Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection[] tableValueUncompressed,
																	Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeVertexCorrection listKeyData
															)
					{
						int countFrame = tableValueUncompressed.Length;
						int countKeyData = listKeyData.CountGetKey();
						if(0 >= countKeyData)
						{	/* No key-data */
							/* MEMO: Do not mess "listValue". */
							tableCodeValue = new int[0];
							return(true);
						}

						List<int> listStatus = new List<int>(countFrame);
						listStatus.Clear();

						int index;
						int frame;
						int frameNext;
						int status;
						Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection value = new Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection();
						Library_SpriteStudio6.Utility.Interpolation.KindFormula formulaSource;
						KindFormula formula;
						for(int i=0; i<countKeyData; i++)
						{
							if((countKeyData - 1) <= i)
							{	/* Doesn't have next key */
								formulaSource = Utility.Interpolation.KindFormula.NON;
								frame = listKeyData.ListKey[i].Frame;
								frameNext = countFrame;

								i = countKeyData;	/* Force end */
							}
							else
							{
								formulaSource = listKeyData.ListKey[i].Formula;
								frame = listKeyData.ListKey[i].Frame;
								frameNext = listKeyData.ListKey[i + 1].Frame;
							}
							if(countFrame <= frame)
							{	/* Error */
								break;
							}

							/* Set data each formula */
							switch(formulaSource)
							{
								/* Type: Interpolate */
								case Utility.Interpolation.KindFormula.LINEAR:
									formula = KindFormula.LINEAR;

									value = tableValueUncompressed[frame];
									index = ListSetValue(listValue, value);

									if(true == value.Equals(tableValueUncompressed[frameNext]))
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								case Utility.Interpolation.KindFormula.ACCELERATE:
									formula = KindFormula.ACCELERATE;

									value = tableValueUncompressed[frame];
									index = ListSetValue(listValue, value);

									if(true == value.Equals(tableValueUncompressed[frameNext]))
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								case Utility.Interpolation.KindFormula.DECELERATE:
									formula = KindFormula.DECELERATE;

									value = tableValueUncompressed[frame];
									index = ListSetValue(listValue, value);

									if(true == value.Equals(tableValueUncompressed[frameNext]))
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								/* Type: CPE */
								case Utility.Interpolation.KindFormula.NON:
								case Utility.Interpolation.KindFormula.HERMITE:
								case Utility.Interpolation.KindFormula.BEZIER:
									formula = KindFormula.CPE;

									value.CleanUp();
									for(int j=frame; j<frameNext; j++)
									{
										if(	(null == value.Coordinate)	/* Not set */
											|| (false == value.Equals(tableValueUncompressed[j]))
											)
										{
											value = tableValueUncompressed[j];
											index = ListSetValue(listValue, value);

											status = StatusGet(formula, index, j);
											listStatus.Add(status);
										}
									}
									break;
							}
						}

						tableCodeValue = listStatus.ToArray();
						listStatus.Clear();
						listStatus = null;

						return(true);
					}
					public static bool CompressDeform(	out int[] tableCodeValue,
														List<Library_SpriteStudio6.Data.Animation.Attribute.Deform> listValue,
														Library_SpriteStudio6.Data.Animation.Attribute.Deform[] tableValueUncompressed,
														Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeDeform listKeyData
													)
					{
						int countFrame = tableValueUncompressed.Length;
						int countKeyData = listKeyData.CountGetKey();
						if(0 >= countKeyData)
						{	/* No key-data */
							/* MEMO: Do not mess "listValue". */
							tableCodeValue = new int[0];
							return(true);
						}

						List<int> listStatus = new List<int>(countFrame);
						listStatus.Clear();

						int index;
						int frame;
						int frameNext;
						int status;
						Library_SpriteStudio6.Data.Animation.Attribute.Deform value = new Library_SpriteStudio6.Data.Animation.Attribute.Deform();
						Library_SpriteStudio6.Utility.Interpolation.KindFormula formulaSource;
						KindFormula formula;
						for(int i=0; i<countKeyData; i++)
						{
							if((countKeyData - 1) <= i)
							{	/* Doesn't have next key */
								formulaSource = Utility.Interpolation.KindFormula.NON;
								frame = listKeyData.ListKey[i].Frame;
								frameNext = countFrame;

								i = countKeyData;	/* Force end */
							}
							else
							{
								formulaSource = listKeyData.ListKey[i].Formula;
								frame = listKeyData.ListKey[i].Frame;
								frameNext = listKeyData.ListKey[i + 1].Frame;
							}
							if(countFrame <= frame)
							{	/* Error */
								break;
							}

							/* Set data each formula */
							switch(formulaSource)
							{
								/* Type: Interpolate */
								case Utility.Interpolation.KindFormula.LINEAR:
									formula = KindFormula.LINEAR;

									value = tableValueUncompressed[frame];
									index = ListSetValue(listValue, value);

									if(true == value.Equals(tableValueUncompressed[frameNext]))
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								case Utility.Interpolation.KindFormula.ACCELERATE:
									formula = KindFormula.ACCELERATE;

									value = tableValueUncompressed[frame];
									index = ListSetValue(listValue, value);

									if(true == value.Equals(tableValueUncompressed[frameNext]))
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								case Utility.Interpolation.KindFormula.DECELERATE:
									formula = KindFormula.DECELERATE;

									value = tableValueUncompressed[frame];
									index = ListSetValue(listValue, value);

									if(true == value.Equals(tableValueUncompressed[frameNext]))
									{	/* Not change in range */
										formula = KindFormula.CPE;
									}
									status = StatusGet(formula, index, frame);
									listStatus.Add(status);
									break;

								/* Type: CPE */
								case Utility.Interpolation.KindFormula.NON:
								case Utility.Interpolation.KindFormula.HERMITE:
								case Utility.Interpolation.KindFormula.BEZIER:
									formula = KindFormula.CPE;

									value.CleanUp();
									for(int j=frame; j<frameNext; j++)
									{
										if(	(null == value.TableCoordinate)	/* Not set */
											|| (false == value.Equals(tableValueUncompressed[j]))
											)
										{
											value = tableValueUncompressed[j];
											index = ListSetValue(listValue, value);

											status = StatusGet(formula, index, j);
											listStatus.Add(status);
										}
									}
									break;
							}
						}

						tableCodeValue = listStatus.ToArray();
						listStatus.Clear();
						listStatus = null;

						return(true);
					}
					private static int StatusGet(KindFormula formula, int index, int framekey)
					{
						return(	(((int)formula << (int)FlagBitShift.FORMULA) & (int)FlagBit.FORMULA)
								| ((index << (int)FlagBitShift.INDEX) & (int)FlagBit.INDEX)
								| ((framekey /* << (int)FlagBitShift.FRAMEKEY */) & (int)FlagBit.FRAMEKEY)
							);
					}
					private static int ListSetValue<_Type>(List<_Type> listValue, _Type value)
					{
						int countValue = listValue.Count;
						for(int i=0; i<countValue; i++)
						{
							if(true == listValue[i].Equals(value))
							{
								return(i);
							}
						}
						listValue.Add(value);
						return(listValue.Count - 1);
					}
					private static int ListSetValueVector2(List<Vector2> listValue, ref int elementVectorNext, float value)
					{
						int countValue = ((listValue.Count - 1) * 2) + elementVectorNext;
						int indexVector;
						int elementVector;
						for(int i=0; i<countValue; i++)
						{
							indexVector = i / 2;
							elementVector = i % 2;
							if(listValue[indexVector][elementVector] == value)
							{
								return(i);
							}
						}

						if(0 == elementVectorNext)
						{
							listValue.Add(Vector2.zero);
						}
						indexVector = listValue.Count;
						indexVector--;

						Vector2 valueVector2 = listValue[indexVector];
						valueVector2[elementVectorNext] = value;
						listValue[indexVector] = valueVector2;

						elementVector = elementVectorNext;	/* Stock */
						elementVectorNext++;
						elementVectorNext %= 2;

						return((indexVector * 2) + elementVector);
					}
					private static int ListSetValueVector3(List<Vector3> listValue, ref int elementVectorNext, float value)
					{
						int countValue = ((listValue.Count - 1) * 3) + elementVectorNext;
						int indexVector;
						int elementVector;
						for(int i=0; i<countValue; i++)
						{
							indexVector = i / 3;
							elementVector = i % 3;
							if(listValue[indexVector][elementVector] == value)
							{
								return(i);
							}
						}

						if(0 == elementVectorNext)
						{
							listValue.Add(Vector3.zero);
						}
						indexVector = listValue.Count;
						indexVector--;

						Vector3 valueVector3 = listValue[indexVector];
						valueVector3[elementVectorNext] = value;
						listValue[indexVector] = valueVector3;

						elementVector = elementVectorNext;	/* Stock */
						elementVectorNext++;
						elementVectorNext %= 3;

						return((indexVector * 3) + elementVector);
					}
					#endregion Functions
				}
				#endregion Classes, Structs & Interfaces
			}
		}
	}
}
