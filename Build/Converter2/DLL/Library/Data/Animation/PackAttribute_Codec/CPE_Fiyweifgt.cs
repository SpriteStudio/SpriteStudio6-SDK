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
				public static class CPE_Flyweight
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
						false	/* Deform */
					);

					public const string ID = "CPE_Flyweight";

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
//					internal readonly static InterfaceFunctionDeform FunctionDeform = new InterfaceFunctionDeform();

					[System.Flags]
					private enum FlagBit
					{
						FRAMEKEY = 0x00007fff,
						INDEX = 0x3fff8000,

						CLEAR = 0x00000000,
					}
					[System.Flags]
					private enum FlagBitAppendVector
					{
						INDEX00 = 0x00007fff,
						INDEX01 = 0x3fff8000,

						CLEAR = 0x00000000,
					}
					[System.Flags]
					private enum FlagBitAppendPartsColor1
					{
						INDEX00 = 0x00007fff,	/* LU */
						INDEX01 = 0x3fff8000,	/* RU */

						CLEAR = 0x00000000,
					}
					private enum FlagBitAppendPartsColor2
					{
						INDEX00 = 0x00007fff,	/* RD */
						INDEX01 = 0x3fff8000,	/* RU */

						CLEAR = 0x00000000,
					}
					private enum IndexTableCodeValue
					{
						/* for int/float/Vector2/Vector3 */
						BASE = 0,
						TERMINATOR_BASE = (BASE + 1),
						VECTOR_YZ = 1,
						TERMINATOR_VECTOR = (VECTOR_YZ + 1),

						/* for Vertex Correction / PartsColor */
						VERTEX_LU = 1,
						VERTEX_RU = 2,
						VERTEX_RD = 3,
						VERTEX_LD = 4,
						TERMINATOR_VERTEXCORRECTION = (VERTEX_LD + 1),

						/* for Vertex Correction / PartsColor */
						COLOR_LURU = 1,
						COLOR_RDLD = 2,
						COLOR_POWER_LURU = 3,
						COLOR_POWER_RDLD = 4,
						TERMINATOR_PARTSCOLOR = (COLOR_POWER_RDLD + 1),
					}
					public enum Constants
					{
						VALUE_CAPACITY = ((int)FlagBit.INDEX >> 15),
					}
					#endregion Enums & Constants

					/* ----------------------------------------------- Dictionary-Functions */
					#region Dictionary-Functions
					public static bool DictionaryBootUp(int indexAnimation, int indexParts, Script_SpriteStudio6_DataAnimation dataAnimation)
					{
						/* MEMO: External dictionary is for entire animation (not each-animation/each-parts). */
						/* MEMO: "dataAnimation" is always null in BootUp. */
						if((0 > indexAnimation) && (0 > indexParts))
						{
							ListValueInt = new List<int>((int)Constants.VALUE_CAPACITY);
							ListValueFloat = new List<float>((int)Constants.VALUE_CAPACITY);
							ListValueFloatVector = new List<float>((int)Constants.VALUE_CAPACITY);
							ListValueFloatCoordinate = new List<float>((int)Constants.VALUE_CAPACITY);
							ListValueColor = new List<Color>((int)Constants.VALUE_CAPACITY);

							if((null == ListValueInt) || (null == ListValueFloat) || (null == ListValueFloatVector) || (null == ListValueFloatCoordinate) || (null == ListValueColor))
							{
								DictionaryCleanUp();
								return(false);
							}

							ListValueInt.Clear();
							ListValueFloat.Clear();
							ListValueFloatVector.Clear();
							ListValueFloatCoordinate.Clear();
							ListValueColor.Clear();
						}
						return(true);
					}
					private static void DictionaryCleanUp()
					{
						ListValueInt = null;
						ListValueFloat = null;
						ListValueFloatVector = null;
						ListValueFloatCoordinate = null;
						ListValueColor = null;
					}

					public static bool DictionaryShutDown(int indexAnimation, int indexParts, Script_SpriteStudio6_DataAnimation dataAnimation)
					{
						/* MEMO: External dictionary is for entire animation (not each-animation/each-parts). */
						/* MEMO: DataAnimation is set to non-null only when "indexAnimation=-1" and "indexParts==-1". (When animation data creating is finished) */
						if((0 > indexAnimation) && (0 > indexParts) && (null != dataAnimation))
						{
							Dictionary dictionary = new Dictionary();
							dictionary.TableValueInt = ListValueInt.ToArray();
							dictionary.TableValueFloat = ListValueFloat.ToArray();
							dictionary.TableValueFloatVector = ListValueFloatVector.ToArray();
							dictionary.TableValueFloatCoordinate = ListValueFloatCoordinate.ToArray();
							dictionary.TableValueColor = ListValueColor.ToArray();

							dataAnimation.Dictionary_CPE_Flyweight = dictionary;

							ListValueInt.Clear();
							ListValueFloat.Clear();
							ListValueFloatVector.Clear();
							ListValueFloatCoordinate.Clear();
							ListValueColor.Clear();
							DictionaryCleanUp();
						}
						return(true);
					}
					#endregion Dictionary-Functions

					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					static private List<int> ListValueInt;
					static private List<float> ListValueFloat;
					static private List<float> ListValueFloatVector;
					static private List<float> ListValueFloatCoordinate;
					static private List<Color> ListValueColor;
					#endregion Variables & Properties

					/* ----------------------------------------------- Classes, Structs & Interfaces */
					#region Classes, Structs & Interfaces
					[System.Serializable]
					/* MEMO: This structure corresponds to the Flyweight's "Factory", but values are not added at runtime.(Fixed at import) */
					/*       Therefore, named not "Factory" but "Dictionary".(Same as data-compression's "Dictionary")                      */
					public class Dictionary
					{
						/* MEMO: In order to reduce overflow, float is divided into two. */
						public int[] TableValueInt;
						public float[] TableValueFloat;
						public float[] TableValueFloatVector;	/* Vector3 */
						public float[] TableValueFloatCoordinate;	/* VertexCorrection */
						public Color[] TableValueColor;
					}

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
							return(Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Flyweight.ValueGetInt(ref outValue, ref outFrameKey, container.TableCodeValue, frame, ref argument.DataAnimation.Dictionary_CPE_Flyweight));
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
							/* MEMO: In advance, encode Standard CPE. */
							Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInt dataStandardCPE = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInt();
							dataStandardCPE.TypePack = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE;
							Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionInt(dataStandardCPE);
							dataStandardCPE.Function.Pack(dataStandardCPE, nameAttribute, countFrame, flagStatusParts, tableOrderDraw, tableOrderPreDraw, listKeyData);

							return(Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Flyweight.CompressInt(container, dataStandardCPE));
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
							return(Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Flyweight.ValueGetFloat(ref outValue, ref outFrameKey, container.TableCodeValue, frame, ref argument.DataAnimation.Dictionary_CPE_Flyweight));
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
							/* MEMO: In advance, encode Standard CPE. */
							Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerFloat dataStandardCPE = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerFloat();
							dataStandardCPE.TypePack = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE;
							Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionFloat(dataStandardCPE);
							dataStandardCPE.Function.Pack(dataStandardCPE, nameAttribute, countFrame, flagStatusParts, tableOrderDraw, tableOrderPreDraw, listKeyData);

							return(Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Flyweight.CompressFloat(container, dataStandardCPE));
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
							return(Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Flyweight.ValueGetVector2(ref outValue, ref outFrameKey, container.TableCodeValue, frame, ref argument.DataAnimation.Dictionary_CPE_Flyweight));
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
						{	/* MEMO: "listKeyData.Length" is always 2 (X, Y) */
							/* MEMO: Get values that have undergone dedicated processing and inheriting for each attribute. */
							/* MEMO: In advance, encode Standard CPE. */
							Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2 dataStandardCPE = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2();
							dataStandardCPE.TypePack = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE;
							Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVector2(dataStandardCPE);
							dataStandardCPE.Function.Pack(dataStandardCPE, nameAttribute, countFrame, flagStatusParts, tableOrderDraw, tableOrderPreDraw, listKeyData);

							return(Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Flyweight.CompressVector2(container, dataStandardCPE));
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
							return(Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Flyweight.ValueGetVector3(ref outValue, ref outFrameKey, container.TableCodeValue, frame, ref argument.DataAnimation.Dictionary_CPE_Flyweight));
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
							/* MEMO: In advance, encode Standard CPE. */
							Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3 dataStandardCPE = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3();
							dataStandardCPE.TypePack = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE;
							Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVector3(dataStandardCPE);
							dataStandardCPE.Function.Pack(dataStandardCPE, nameAttribute, countFrame, flagStatusParts, tableOrderDraw, tableOrderPreDraw, listKeyData);

							return(Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Flyweight.CompressVector3(container, dataStandardCPE));
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
							return(Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Flyweight.ValueGetPartsColor(ref outValue, ref outFrameKey, container.TableCodeValue, frame, ref argument.DataAnimation.Dictionary_CPE_Flyweight));
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
							/* MEMO: In advance, encode Standard CPE. */
							Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerPartsColor dataStandardCPE = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerPartsColor();
							dataStandardCPE.TypePack = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE;
							Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionPartsColor(dataStandardCPE);
							dataStandardCPE.Function.Pack(dataStandardCPE, nameAttribute, countFrame, flagStatusParts, tableOrderDraw, tableOrderPreDraw, listKeyData);

							return(Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Flyweight.CompressPartsColor(container, dataStandardCPE));
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
							return(Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Flyweight.ValueGetVertexCorrection(ref outValue, ref outFrameKey, container.TableCodeValue, frame, ref argument.DataAnimation.Dictionary_CPE_Flyweight));
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
							/* MEMO: In advance, encode Standard CPE. */
							Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVertexCorrection dataStandardCPE = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVertexCorrection();
							dataStandardCPE.TypePack = Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack.STANDARD_CPE;
							Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVertexCorrection(dataStandardCPE);
							dataStandardCPE.Function.Pack(dataStandardCPE, nameAttribute, countFrame, flagStatusParts, tableOrderDraw, tableOrderPreDraw, listKeyData);

							return(Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Flyweight.CompressVertecCorrection(container, dataStandardCPE));
						}
						#endregion Functions
					}

					/* MEMO: "Deform" is not supported.                                                                 */
					/*       Reason is that since data size is large per a key-data, dictionary is consumpted a lot of. */

					#endregion Classes, Structs & Interfaces

					/* ----------------------------------------------- Functions */
					#region Functions
					public static bool ValueGetInt(	ref int outValue,
													ref int outFrameKey,
													Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[] tableCodeValue,
													int frame,
													ref Dictionary dictionary
												)
					{
#if UNITY_EDITOR
						if(null == tableCodeValue)
						{
							/* MEMO: May reach before deserialization direct-after import. */
							return(false);
						}
#endif
						int[] tableStatus = tableCodeValue[(int)IndexTableCodeValue.BASE].TableCode;	/* Always 1 */
						int frameKey = -1;
						int status;
						int indexMinimum = 0;
						int indexMaximum = tableStatus.Length - 1;
						int index;
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
						frameKey = status & (int)FlagBit.FRAMEKEY;
						if(outFrameKey == frameKey)
						{
							return(false);	/* outValue is not overwritten. */
						}
						outFrameKey = frameKey;

						index = (status & (int)FlagBit.INDEX) >> 15;
						outValue = dictionary.TableValueInt[index];
						return(true);	/* outValue is overwritten. */
					}

					public static bool ValueGetFloat(	ref float outValue,
														ref int outFrameKey,
														Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[] tableCodeValue,
														int frame,
														ref Dictionary dictionary
													)
					{
#if UNITY_EDITOR
						if(null == tableCodeValue)
						{
							/* MEMO: May reach before deserialization direct-after import. */
							return(false);
						}
#endif
						int[] tableStatus = tableCodeValue[(int)IndexTableCodeValue.BASE].TableCode;	/* Always 1 */
						int frameKey = -1;
						int status;
						int indexMinimum = 0;
						int indexMaximum = tableStatus.Length - 1;
						int index;
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
						frameKey = status & (int)FlagBit.FRAMEKEY;
						if(outFrameKey == frameKey)
						{
							return(false);	/* outValue is not overwritten. */
						}
						outFrameKey = frameKey;

						index = (status & (int)FlagBit.INDEX) >> 15;
						outValue = dictionary.TableValueFloat[index];
						return(true);	/* outValue is overwritten. */
					}

					public static bool ValueGetVector2(	ref Vector2 outValue,
														ref int outFrameKey,
														Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[] tableCodeValue,
														int frame,
														ref Dictionary dictionary
													)
					{
#if UNITY_EDITOR
						if(null == tableCodeValue)
						{
							/* MEMO: May reach before deserialization direct-after import. */
							return(false);
						}
#endif
						int[] tableStatus = tableCodeValue[(int)IndexTableCodeValue.BASE].TableCode;
						int frameKey = -1;
						int status;
						int indexMinimum = 0;
						int indexMaximum = tableStatus.Length - 1;
						int index;
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
						frameKey = status & (int)FlagBit.FRAMEKEY;
						if(outFrameKey == frameKey)
						{
							return(false);	/* outValue is not overwritten. */
						}
						outFrameKey = frameKey;

						index = (status & (int)FlagBit.INDEX) >> 15;
						outValue.x = dictionary.TableValueFloat[index];

						status = tableCodeValue[(int)IndexTableCodeValue.VECTOR_YZ].TableCode[indexMinimum];
						index = (status & (int)FlagBitAppendVector.INDEX00);	/* >> 0 */
						outValue.y = dictionary.TableValueFloat[index];

						return(true);	/* outValue is overwritten. */
					}

					public static bool ValueGetVector3(	ref Vector3 outValue,
														ref int outFrameKey,
														Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[] tableCodeValue,
														int frame,
														ref Dictionary dictionary
													)
					{
#if UNITY_EDITOR
						if(null == tableCodeValue)
						{
							/* MEMO: May reach before deserialization direct-after import. */
							return(false);
						}
#endif
						int[] tableStatus = tableCodeValue[(int)IndexTableCodeValue.BASE].TableCode;
						int frameKey = -1;
						int status;
						int indexMinimum = 0;
						int indexMaximum = tableStatus.Length - 1;
						int index;
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
						frameKey = status & (int)FlagBit.FRAMEKEY;
						if(outFrameKey == frameKey)
						{
							return(false);	/* outValue is not overwritten. */
						}
						outFrameKey = frameKey;

						index = (status & (int)FlagBit.INDEX) >> 15;
						outValue.x = dictionary.TableValueFloatVector[index];

						status = tableCodeValue[(int)IndexTableCodeValue.VECTOR_YZ].TableCode[indexMinimum];
						index = (status & (int)FlagBitAppendVector.INDEX00);	/* >> 0 */
						outValue.y = dictionary.TableValueFloatVector[index];

						index = (status & (int)FlagBitAppendVector.INDEX01) >> 15;
						outValue.z = dictionary.TableValueFloatVector[index];

						return(true);	/* outValue is overwritten. */
					}

					public static bool ValueGetVertexCorrection(	ref Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection outValue,
																	ref int outFrameKey,
																	Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[] tableCodeValue,
																	int frame,
																	ref Dictionary dictionary
																)
					{
#if UNITY_EDITOR
						if(null == tableCodeValue)
						{
							/* MEMO: May reach before deserialization direct-after import. */
							return(false);
						}
#endif
						int[] tableStatus = tableCodeValue[(int)IndexTableCodeValue.BASE].TableCode;
						int frameKey = -1;
						int status;
						int indexMinimum = 0;
						int indexMaximum = tableStatus.Length - 1;
						int index;
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
						frameKey = status & (int)FlagBit.FRAMEKEY;
						if(outFrameKey == frameKey)
						{
							return(false);	/* outValue is not overwritten. */
						}
						outFrameKey = frameKey;
//						index = (status & (int)FlagBit.INDEX) >> 15;	/* There is nothing here */

						status = tableCodeValue[(int)IndexTableCodeValue.VERTEX_LU].TableCode[indexMinimum];
						index = (status & (int)FlagBitAppendVector.INDEX00);	/* >> 0 */
						outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU].x = dictionary.TableValueFloatCoordinate[index];
						index = (status & (int)FlagBitAppendVector.INDEX01) >> 15;
						outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU].y = dictionary.TableValueFloatCoordinate[index];

						status = tableCodeValue[(int)IndexTableCodeValue.VERTEX_RU].TableCode[indexMinimum];
						index = (status & (int)FlagBitAppendVector.INDEX00);	/* >> 0 */
						outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU].x = dictionary.TableValueFloatCoordinate[index];
						index = (status & (int)FlagBitAppendVector.INDEX01) >> 15;
						outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU].y = dictionary.TableValueFloatCoordinate[index];

						status = tableCodeValue[(int)IndexTableCodeValue.VERTEX_RD].TableCode[indexMinimum];
						index = (status & (int)FlagBitAppendVector.INDEX00);	/* >> 0 */
						outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD].x = dictionary.TableValueFloatCoordinate[index];
						index = (status & (int)FlagBitAppendVector.INDEX01) >> 15;
						outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD].y = dictionary.TableValueFloatCoordinate[index];

						status = tableCodeValue[(int)IndexTableCodeValue.VERTEX_LD].TableCode[indexMinimum];
						index = (status & (int)FlagBitAppendVector.INDEX00);	/* >> 0 */
						outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD].x = dictionary.TableValueFloatCoordinate[index];
						index = (status & (int)FlagBitAppendVector.INDEX01) >> 15;
						outValue.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD].y = dictionary.TableValueFloatCoordinate[index];

						return(true);	/* outValue is overwritten. */
					}

					public static bool ValueGetPartsColor(	ref Library_SpriteStudio6.Data.Animation.Attribute.PartsColor outValue,
															ref int outFrameKey,
															Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[] tableCodeValue,
															int frame,
															ref Dictionary dictionary
														)
					{
#if UNITY_EDITOR
						if(null == tableCodeValue)
						{
							/* MEMO: May reach before deserialization direct-after import. */
							return(false);
						}
#endif
						int[] tableStatus = tableCodeValue[(int)IndexTableCodeValue.BASE].TableCode;
						int frameKey = -1;
						int status;
						int indexMinimum = 0;
						int indexMaximum = tableStatus.Length - 1;
						int index;
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
						frameKey = status & (int)FlagBit.FRAMEKEY;
						if(outFrameKey == frameKey)
						{
							return(false);	/* outValue is not overwritten. */
						}
						outFrameKey = frameKey;
						outValue.Bound = Library_SpriteStudio6.KindBoundBlend.VERTEX;	/* Always handle by "Vertex" at runtime */
						outValue.Operation = (Library_SpriteStudio6.KindOperationBlend)((status & (int)FlagBit.INDEX) >> 15);

						status = tableCodeValue[(int)IndexTableCodeValue.COLOR_LURU].TableCode[indexMinimum];
						index = (status & (int)FlagBitAppendPartsColor1.INDEX00);	/* >> 0 */
						outValue.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU] = dictionary.TableValueColor[index];
						index = (status & (int)FlagBitAppendPartsColor1.INDEX01) >> 15;
						outValue.VertexColor[(int)Library_SpriteStudio6.KindVertex.RU] = dictionary.TableValueColor[index];

						status = tableCodeValue[(int)IndexTableCodeValue.COLOR_RDLD].TableCode[indexMinimum];
						index = (status & (int)FlagBitAppendPartsColor1.INDEX00);	/* >> 0 */
						outValue.VertexColor[(int)Library_SpriteStudio6.KindVertex.RD] = dictionary.TableValueColor[index];
						index = (status & (int)FlagBitAppendPartsColor1.INDEX01) >> 15;
						outValue.VertexColor[(int)Library_SpriteStudio6.KindVertex.LD] = dictionary.TableValueColor[index];

						status = tableCodeValue[(int)IndexTableCodeValue.COLOR_POWER_LURU].TableCode[indexMinimum];
						index = (status & (int)FlagBitAppendPartsColor1.INDEX00);	/* >> 0 */
						outValue.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU] = dictionary.TableValueFloat[index];
						index = (status & (int)FlagBitAppendPartsColor1.INDEX01) >> 15;
						outValue.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RU] = dictionary.TableValueFloat[index];

						status = tableCodeValue[(int)IndexTableCodeValue.COLOR_POWER_RDLD].TableCode[indexMinimum];
						index = (status & (int)FlagBitAppendPartsColor1.INDEX00);	/* >> 0 */
						outValue.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RD] = dictionary.TableValueFloat[index];
						index = (status & (int)FlagBitAppendPartsColor1.INDEX01) >> 15;
						outValue.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LD] = dictionary.TableValueFloat[index];

						return(true);	/* outValue is overwritten. */
					}

					public static bool CompressInt(	Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInt container,
													Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInt containerStandardCPE
												)
					{
						if(0 >= containerStandardCPE.TableCodeValue.Length)
						{
							container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[0];
							container.TableValue = new int[0];
							return(true);
						}
						int[] tableStatusStandardCPE = containerStandardCPE.TableCodeValue[0].TableCode;	/* Always 1 */
						int[] tablevalueStandardCPE = containerStandardCPE.TableValue;
						int countStartus = tableStatusStandardCPE.Length;

						container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[(int)IndexTableCodeValue.TERMINATOR_BASE];	/* Always 1 */
						if(null == container.TableCodeValue)
						{
							return(false);
						}
						container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode = new int[countStartus];
						if(null == container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode)
						{
							return(false);
						}
						container.TableValue = new int[0];	/* Has no (parts')value table */
						if(null == container.TableValue)
						{
							return(false);
						}
						int[] tableStatusBase = container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode;

						if(0 >= countStartus)
						{
							return(true);
						}

						int status;
						int index;
						int frame;
						int value;
						for(int i=0; i<countStartus; i++)
						{
							status = tableStatusStandardCPE[i];
							frame = status & (int)Library_SpriteStudio6.Data.Animation.PackAttribute.StandardCPE.FlagBit.FRAMEKEY;
							index = (status & (int)Library_SpriteStudio6.Data.Animation.PackAttribute.StandardCPE.FlagBit.INDEX) >> 15;
							value = tablevalueStandardCPE[index];

							index = ListSetInt(value);
							if(0 > index)
							{
								return(false);
							}
							tableStatusBase[i] = StatusGet(index, frame);
						}

						containerStandardCPE.TableCodeValue = null;	/* Purge */
						containerStandardCPE.TableValue = null;	/* Purge */

						return(true);
					}

					public static bool CompressFloat(	Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerFloat container,
														Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerFloat containerStandardCPE
													)
					{
						if(0 >= containerStandardCPE.TableCodeValue.Length)
						{
							container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[0];
							container.TableValue = new float[0];
							return(true);
						}
						int[] tableStatusStandardCPE = containerStandardCPE.TableCodeValue[0].TableCode;	/* Always 1 */
						float[] tablevalueStandardCPE = containerStandardCPE.TableValue;
						int countStartus = tableStatusStandardCPE.Length;

						container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[(int)IndexTableCodeValue.TERMINATOR_BASE];	/* Always 1 */
						if(null == container.TableCodeValue)
						{
							return(false);
						}
						container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode = new int[countStartus];
						if(null == container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode)
						{
							return(false);
						}
						container.TableValue = new float[0];	/* Has no (parts')value table */
						if(null == container.TableValue)
						{
							return(false);
						}
						int[] tableStatusBase = container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode;

						if(0 >= countStartus)
						{
							return(true);
						}

						int status;
						int index;
						int frame;
						float value;
						for(int i=0; i<countStartus; i++)
						{
							status = tableStatusStandardCPE[i];
							frame = status & (int)Library_SpriteStudio6.Data.Animation.PackAttribute.StandardCPE.FlagBit.FRAMEKEY;
							index = (status & (int)Library_SpriteStudio6.Data.Animation.PackAttribute.StandardCPE.FlagBit.INDEX) >> 15;
							value = tablevalueStandardCPE[index];

							index = ListSetFloat(value);
							if(0 > index)
							{
								return(false);
							}
							tableStatusBase[i] = StatusGet(index, frame);
						}

						containerStandardCPE.TableCodeValue = null;	/* Purge */
						containerStandardCPE.TableValue = null;	/* Purge */

						return(true);
					}

					public static bool CompressVector2(	Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2 container,
														Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2 containerStandardCPE
													)
					{
						if(0 >= containerStandardCPE.TableCodeValue.Length)
						{
							container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[0];
							container.TableValue = new Vector2[0];
							return(true);
						}
						int[] tableStatusStandardCPE = containerStandardCPE.TableCodeValue[0].TableCode;	/* Always 1 */
						Vector2[] tablevalueStandardCPE = containerStandardCPE.TableValue;
						int countStartus = tableStatusStandardCPE.Length;

						container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[(int)IndexTableCodeValue.TERMINATOR_VECTOR];
						if(null == container.TableCodeValue)
						{
							return(false);
						}
						container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode = new int[countStartus];
						if(null == container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode)
						{
							return(false);
						}
						container.TableCodeValue[(int)IndexTableCodeValue.VECTOR_YZ].TableCode = new int[countStartus];
						if(null == container.TableCodeValue[(int)IndexTableCodeValue.VECTOR_YZ].TableCode)
						{
							return(false);
						}
						container.TableValue = new Vector2[0];	/* Has no (parts')value table */
						if(null == container.TableValue)
						{
							return(false);
						}
						int[] tableStatusBase = container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode;
						int[] tableStatusVector = container.TableCodeValue[(int)IndexTableCodeValue.VECTOR_YZ].TableCode;

						if(0 >= countStartus)
						{
							return(true);
						}

						int status;
						int index;
						int frame;
						Vector2 value;
						for(int i=0; i<countStartus; i++)
						{
							status = tableStatusStandardCPE[i];
							frame = status & (int)Library_SpriteStudio6.Data.Animation.PackAttribute.StandardCPE.FlagBit.FRAMEKEY;
							index = (status & (int)Library_SpriteStudio6.Data.Animation.PackAttribute.StandardCPE.FlagBit.INDEX) >> 15;
							value = tablevalueStandardCPE[index];

							index = ListSetFloat(value.x);
							if(0 > index)
							{
								return(false);
							}
							tableStatusBase[i] = StatusGet(index, frame);

							index = ListSetFloat(value.y);
							if(0 > index)
							{
								return(false);
							}
							tableStatusVector[i] = StatusGetVector(index, 0);	/* Disuse Z */
						}

						containerStandardCPE.TableCodeValue = null;	/* Purge */
						containerStandardCPE.TableValue = null;	/* Purge */

						return(true);
					}

					public static bool CompressVector3(	Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3 container,
														Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3 containerStandardCPE
													)
					{
						if(0 >= containerStandardCPE.TableCodeValue.Length)
						{
							container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[0];
							container.TableValue = new Vector3[0];
							return(true);
						}
						int[] tableStatusStandardCPE = containerStandardCPE.TableCodeValue[0].TableCode;	/* Always 1 */
						Vector3[] tablevalueStandardCPE = containerStandardCPE.TableValue;
						int countStartus = tableStatusStandardCPE.Length;

						container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[(int)IndexTableCodeValue.TERMINATOR_VECTOR];
						if(null == container.TableCodeValue)
						{
							return(false);
						}
						container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode = new int[countStartus];
						if(null == container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode)
						{
							return(false);
						}
						container.TableCodeValue[(int)IndexTableCodeValue.VECTOR_YZ].TableCode = new int[countStartus];
						if(null == container.TableCodeValue[(int)IndexTableCodeValue.VECTOR_YZ].TableCode)
						{
							return(false);
						}
						container.TableValue = new Vector3[0];	/* Has no (parts')value table */
						if(null == container.TableValue)
						{
							return(false);
						}
						int[] tableStatusBase = container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode;
						int[] tableStatusVector = container.TableCodeValue[(int)IndexTableCodeValue.VECTOR_YZ].TableCode;

						if(0 >= countStartus)
						{
							return(true);
						}

						int status;
						int index;
						int indexZ;
						int frame;
						Vector3 value;
						for(int i=0; i<countStartus; i++)
						{
							status = tableStatusStandardCPE[i];
							frame = status & (int)Library_SpriteStudio6.Data.Animation.PackAttribute.StandardCPE.FlagBit.FRAMEKEY;
							index = (status & (int)Library_SpriteStudio6.Data.Animation.PackAttribute.StandardCPE.FlagBit.INDEX) >> 15;
							value = tablevalueStandardCPE[index];

							index = ListSetFloatVector(value.x);
							if(0 > index)
							{
								return(false);
							}
							tableStatusBase[i] = StatusGet(index, frame);

							index = ListSetFloatVector(value.y);
							if(0 > index)
							{
								return(false);
							}
							indexZ = ListSetFloatVector(value.z);
							if(0 > indexZ)
							{
								return(false);
							}
							tableStatusVector[i] = StatusGetVector(index, indexZ);
						}

						containerStandardCPE.TableCodeValue = null;	/* Purge */
						containerStandardCPE.TableValue = null;	/* Purge */

						return(true);
					}

					public static bool CompressVertecCorrection(	Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVertexCorrection container,
																	Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVertexCorrection containerStandardCPE
															)
					{
						if(0 >= containerStandardCPE.TableCodeValue.Length)
						{
							container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[0];
							container.TableValue = new Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection[0];
							return(true);
						}
						int[] tableStatusStandardCPE = containerStandardCPE.TableCodeValue[0].TableCode;	/* Always 1 */
						Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection[] tablevalueStandardCPE = containerStandardCPE.TableValue;
						int countStartus = tableStatusStandardCPE.Length;

						container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[(int)IndexTableCodeValue.TERMINATOR_VERTEXCORRECTION];
						if(null == container.TableCodeValue)
						{
							return(false);
						}
						container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode = new int[countStartus];
						if(null == container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode)
						{
							return(false);
						}
						container.TableCodeValue[(int)IndexTableCodeValue.VERTEX_LU].TableCode = new int[countStartus];
						if(null == container.TableCodeValue[(int)IndexTableCodeValue.VERTEX_LU].TableCode)
						{
							return(false);
						}
						container.TableCodeValue[(int)IndexTableCodeValue.VERTEX_RU].TableCode = new int[countStartus];
						if(null == container.TableCodeValue[(int)IndexTableCodeValue.VERTEX_RU].TableCode)
						{
							return(false);
						}
						container.TableCodeValue[(int)IndexTableCodeValue.VERTEX_RD].TableCode = new int[countStartus];
						if(null == container.TableCodeValue[(int)IndexTableCodeValue.VERTEX_RD].TableCode)
						{
							return(false);
						}
						container.TableCodeValue[(int)IndexTableCodeValue.VERTEX_LD].TableCode = new int[countStartus];
						if(null == container.TableCodeValue[(int)IndexTableCodeValue.VERTEX_LD].TableCode)
						{
							return(false);
						}
						container.TableValue = new Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection[0];	/* Has no (parts')value table */
						if(null == container.TableValue)
						{
							return(false);
						}
						int[] tableStatusBase = container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode;
						int[] tableStatusLU = container.TableCodeValue[(int)IndexTableCodeValue.VERTEX_LU].TableCode;
						int[] tableStatusRU = container.TableCodeValue[(int)IndexTableCodeValue.VERTEX_RU].TableCode;
						int[] tableStatusRD = container.TableCodeValue[(int)IndexTableCodeValue.VERTEX_RD].TableCode;
						int[] tableStatusLD = container.TableCodeValue[(int)IndexTableCodeValue.VERTEX_LD].TableCode;

						if(0 >= countStartus)
						{
							return(true);
						}

						int status;
						int index;
						int indexY;
						int frame;
						Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection value;
						for(int i=0; i<countStartus; i++)
						{
							status = tableStatusStandardCPE[i];
							frame = status & (int)Library_SpriteStudio6.Data.Animation.PackAttribute.StandardCPE.FlagBit.FRAMEKEY;
							index = (status & (int)Library_SpriteStudio6.Data.Animation.PackAttribute.StandardCPE.FlagBit.INDEX) >> 15;
							value = tablevalueStandardCPE[index];

							tableStatusBase[i] = StatusGet(0, frame);	/* Disuse index */

							index = ListSetFloatCoordinate(value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU].x);
							if(0 > index)
							{
								return(false);
							}
							indexY = ListSetFloatCoordinate(value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LU].y);
							if(0 > indexY)
							{
								return(false);
							}
							tableStatusLU[i] = StatusGetVector(index, indexY);

							index = ListSetFloatCoordinate(value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU].x);
							if(0 > index)
							{
								return(false);
							}
							indexY = ListSetFloatCoordinate(value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RU].y);
							if(0 > indexY)
							{
								return(false);
							}
							tableStatusRU[i] = StatusGetVector(index, indexY);

							index = ListSetFloatCoordinate(value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD].x);
							if(0 > index)
							{
								return(false);
							}
							indexY = ListSetFloatCoordinate(value.Coordinate[(int)Library_SpriteStudio6.KindVertex.RD].y);
							if(0 > indexY)
							{
								return(false);
							}
							tableStatusRD[i] = StatusGetVector(index, indexY);

							index = ListSetFloatCoordinate(value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD].x);
							if(0 > index)
							{
								return(false);
							}
							indexY = ListSetFloatCoordinate(value.Coordinate[(int)Library_SpriteStudio6.KindVertex.LD].y);
							if(0 > indexY)
							{
								return(false);
							}
							tableStatusLD[i] = StatusGetVector(index, indexY);
						}

						containerStandardCPE.TableCodeValue = null;	/* Purge */
						containerStandardCPE.TableValue = null;	/* Purge */

						return(true);
					}

					public static bool CompressPartsColor(	Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerPartsColor container,
															Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerPartsColor containerStandardCPE
														)
					{
						if(0 >= containerStandardCPE.TableCodeValue.Length)
						{
							container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[0];
							container.TableValue = new Library_SpriteStudio6.Data.Animation.Attribute.PartsColor[0];
							return(true);
						}
						int[] tableStatusStandardCPE = containerStandardCPE.TableCodeValue[0].TableCode;	/* Always 1 */
						Library_SpriteStudio6.Data.Animation.Attribute.PartsColor[] tablevalueStandardCPE = containerStandardCPE.TableValue;
						int countStartus = tableStatusStandardCPE.Length;

						container.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[(int)IndexTableCodeValue.TERMINATOR_PARTSCOLOR];
						if(null == container.TableCodeValue)
						{
							return(false);
						}
						container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode = new int[countStartus];
						if(null == container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode)
						{
							return(false);
						}
						container.TableCodeValue[(int)IndexTableCodeValue.COLOR_LURU].TableCode = new int[countStartus];
						if(null == container.TableCodeValue[(int)IndexTableCodeValue.COLOR_LURU].TableCode)
						{
							return(false);
						}
						container.TableCodeValue[(int)IndexTableCodeValue.COLOR_RDLD].TableCode = new int[countStartus];
						if(null == container.TableCodeValue[(int)IndexTableCodeValue.COLOR_RDLD].TableCode)
						{
							return(false);
						}
						container.TableCodeValue[(int)IndexTableCodeValue.COLOR_POWER_LURU].TableCode = new int[countStartus];
						if(null == container.TableCodeValue[(int)IndexTableCodeValue.COLOR_POWER_LURU].TableCode)
						{
							return(false);
						}
						container.TableCodeValue[(int)IndexTableCodeValue.COLOR_POWER_RDLD].TableCode = new int[countStartus];
						if(null == container.TableCodeValue[(int)IndexTableCodeValue.COLOR_POWER_RDLD].TableCode)
						{
							return(false);
						}
						container.TableValue = new Library_SpriteStudio6.Data.Animation.Attribute.PartsColor[0];	/* Has no (parts')value table */
						if(null == container.TableValue)
						{
							return(false);
						}
						int[] tableStatusBase = container.TableCodeValue[(int)IndexTableCodeValue.BASE].TableCode;
						int[] tableStatusLURU = container.TableCodeValue[(int)IndexTableCodeValue.COLOR_LURU].TableCode;
						int[] tableStatusRDLD = container.TableCodeValue[(int)IndexTableCodeValue.COLOR_RDLD].TableCode;
						int[] tableStatusPowerLURU = container.TableCodeValue[(int)IndexTableCodeValue.COLOR_POWER_LURU].TableCode;
						int[] tableStatusPowerRDLD = container.TableCodeValue[(int)IndexTableCodeValue.COLOR_POWER_RDLD].TableCode;

						if(0 >= countStartus)
						{
							return(true);
						}

						int status;
						int index;
						int indexY;
						int frame;
						Library_SpriteStudio6.Data.Animation.Attribute.PartsColor value;
						for(int i=0; i<countStartus; i++)
						{
							status = tableStatusStandardCPE[i];
							frame = status & (int)Library_SpriteStudio6.Data.Animation.PackAttribute.StandardCPE.FlagBit.FRAMEKEY;
							index = (status & (int)Library_SpriteStudio6.Data.Animation.PackAttribute.StandardCPE.FlagBit.INDEX) >> 15;
							value = tablevalueStandardCPE[index];

							tableStatusBase[i] = StatusGet((int)value.Operation, frame);

							index = ListSetColor(value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LU]);
							if(0 > index)
							{
								return(false);
							}
							indexY = ListSetColor(value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RU]);
							if(0 > indexY)
							{
								return(false);
							}
							tableStatusLURU[i] = StatusGetPartsColorLURU(index, indexY);

							index = ListSetColor(value.VertexColor[(int)Library_SpriteStudio6.KindVertex.RD]);
							if(0 > index)
							{
								return(false);
							}
							indexY = ListSetColor(value.VertexColor[(int)Library_SpriteStudio6.KindVertex.LD]);
							if(0 > indexY)
							{
								return(false);
							}
							tableStatusRDLD[i] = StatusGetPartsColorLURU(index, indexY);

							index = ListSetFloat(value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LU]);
							if(0 > index)
							{
								return(false);
							}
							indexY = ListSetFloat(value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RU]);
							if(0 > indexY)
							{
								return(false);
							}
							tableStatusPowerLURU[i] = StatusGetVector(index, indexY);

							index = ListSetFloat(value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.RD]);
							if(0 > index)
							{
								return(false);
							}
							indexY = ListSetFloat(value.RateAlpha[(int)Library_SpriteStudio6.KindVertex.LD]);
							if(0 > indexY)
							{
								return(false);
							}
							tableStatusPowerRDLD[i] = StatusGetVector(index, indexY);
						}

						containerStandardCPE.TableCodeValue = null;	/* Purge */
						containerStandardCPE.TableValue = null;	/* Purge */

						return(true);
					}

					private static int StatusGet(int index, int framekey)
					{
						return(	((index << 15) & (int)FlagBit.INDEX) | (framekey & (int)FlagBit.FRAMEKEY));
					}
					private static int StatusGetVector(int index00, int index01)
					{
						return(	((index01 << 15) & (int)FlagBitAppendVector.INDEX01) | (index00 & (int)FlagBitAppendVector.INDEX00));
					}
					private static int StatusGetPartsColorLURU(int indexLU, int indexRU)
					{
						return(	((indexRU << 15) & (int)FlagBitAppendPartsColor1.INDEX01) | (indexLU & (int)FlagBitAppendPartsColor1.INDEX00));
					}
					private static int StatusGetPartsColorRDLD(int indexRD, int indexLD)
					{
						return(	((indexLD << 15) & (int)FlagBitAppendPartsColor2.INDEX01) | (indexRD & (int)FlagBitAppendPartsColor2.INDEX00));
					}

					private static int ListSetInt(int value)
					{
						int countValue = ListValueInt.Count;
						if((int)Constants.VALUE_CAPACITY <= countValue)
						{
							return(-1);
						}

						for(int i=0; i<countValue; i++)
						{
							if(ListValueInt[i] == value)
							{
								return(i);
							}
						}
						ListValueInt.Add(value);
						return(ListValueInt.Count - 1);
					}
					private static int ListSetFloat(float value)
					{
						int countValue = ListValueFloat.Count;
						if((int)Constants.VALUE_CAPACITY <= countValue)
						{
							return(-1);
						}

						for(int i=0; i<countValue; i++)
						{
							if(ListValueFloat[i] == value)
							{
								return(i);
							}
						}
						ListValueFloat.Add(value);
						return(ListValueFloat.Count - 1);
					}
					private static int ListSetFloatVector(float value)
					{
						int countValue = ListValueFloatVector.Count;
						if((int)Constants.VALUE_CAPACITY <= countValue)
						{
							return(-1);
						}

						for(int i=0; i<countValue; i++)
						{
							if(ListValueFloatVector[i] == value)
							{
								return(i);
							}
						}
						ListValueFloatVector.Add(value);
						return(ListValueFloatVector.Count - 1);
					}
					private static int ListSetFloatCoordinate(float value)
					{
						int countValue = ListValueFloatCoordinate.Count;
						if((int)Constants.VALUE_CAPACITY <= countValue)
						{
							return(-1);
						}

						for(int i=0; i<countValue; i++)
						{
							if(ListValueFloatCoordinate[i] == value)
							{
								return(i);
							}
						}
						ListValueFloatCoordinate.Add(value);
						return(ListValueFloatCoordinate.Count - 1);
					}
					private static int ListSetColor(Color value)
					{
						int countValue = ListValueColor.Count;
						if((int)Constants.VALUE_CAPACITY <= countValue)
						{
							return(-1);
						}

						for(int i=0; i<countValue; i++)
						{
							if(ListValueColor[i] == value)
							{
								return(i);
							}
						}
						ListValueColor.Add(value);
						return(ListValueColor.Count - 1);
					}
					#endregion Functions
				}
				#endregion Classes, Structs & Interfaces
			}
		}
	}
}
