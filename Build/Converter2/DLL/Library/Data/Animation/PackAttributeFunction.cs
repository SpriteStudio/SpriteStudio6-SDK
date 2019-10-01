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
				/* ----------------------------------------------- Enums & Constants */
				#region Enums & Constants
				public enum KindTypePack
				{
					STANDARD_UNCOMPRESSED = 0,	/* Standard-Uncompressed (Plain Array) */
					STANDARD_CPE,	/* Standard-Compressed (Changing-Point Extracting) */
					CPE_FLYWEIGHT,	/* CPE & GoF-Flyweight (Commonized CPE's dictionary) */
					CPE_INTERPOLATE,	/* CPE & Interpolate (Linear,Acceleration and Deceleration: Interpolate / Other: CPE) */

					TERMINATOR,
				}
				#endregion Enums & Constants

				/* ----------------------------------------------- Functions */
				#region Functions
				public static CapacityContainer CapacityGet(KindTypePack pack)
				{
					switch(pack)
					{
						case KindTypePack.STANDARD_UNCOMPRESSED:
							return(StandardUncompressed.Capacity);

						case KindTypePack.STANDARD_CPE:
							return(StandardCPE.Capacity);

						case KindTypePack.CPE_FLYWEIGHT:
							return(CPE_Flyweight.Capacity);

						case KindTypePack.CPE_INTERPOLATE:
							return(CPE_Interpolate.Capacity);

						default:
							break;
					}
					return(null);
				}

				public static string IDGetPack(KindTypePack typePack)
				{
					switch(typePack)
					{
						case KindTypePack.STANDARD_UNCOMPRESSED:
							return(StandardUncompressed.ID);

						case KindTypePack.STANDARD_CPE:
							return(StandardCPE.ID);

						case KindTypePack.CPE_FLYWEIGHT:
							return(CPE_Flyweight.ID);

						case KindTypePack.CPE_INTERPOLATE:
							return(CPE_Interpolate.ID);

						default:
							break;
					}
					return(null);
				}

				public static void BootUpFunctionInt(ContainerInt container)
				{
					switch(container.TypePack)
					{
						case KindTypePack.STANDARD_UNCOMPRESSED:
							container.Function = StandardUncompressed.FunctionInt;
							break;

						case KindTypePack.STANDARD_CPE:
							container.Function = StandardCPE.FunctionInt;
							break;

						case KindTypePack.CPE_FLYWEIGHT:
							container.Function = CPE_Flyweight.FunctionInt;
							break;

						case KindTypePack.CPE_INTERPOLATE:
							container.Function = CPE_Interpolate.FunctionInt;
							break;

						default:
							break;
					}
				}

				public static void BootUpFunctionFloat(ContainerFloat container)
				{
					switch(container.TypePack)
					{
						case KindTypePack.STANDARD_UNCOMPRESSED:
							container.Function = StandardUncompressed.FunctionFloat;
							break;

						case KindTypePack.STANDARD_CPE:
							container.Function = StandardCPE.FunctionFloat;
							break;

						case KindTypePack.CPE_FLYWEIGHT:
							container.Function = CPE_Flyweight.FunctionFloat;
							break;

						case KindTypePack.CPE_INTERPOLATE:
							container.Function = CPE_Interpolate.FunctionFloat;
							break;

						default:
							break;
					}
				}

				public static void BootUpFunctionVector2(ContainerVector2 container)
				{
					switch(container.TypePack)
					{
						case KindTypePack.STANDARD_UNCOMPRESSED:
							container.Function = StandardUncompressed.FunctionVector2;
							break;

						case KindTypePack.STANDARD_CPE:
							container.Function = StandardCPE.FunctionVector2;
							break;

						case KindTypePack.CPE_FLYWEIGHT:
							container.Function = CPE_Flyweight.FunctionVector2;
							break;

						case KindTypePack.CPE_INTERPOLATE:
							container.Function = CPE_Interpolate.FunctionVector2;
							break;

						default:
							break;
					}
				}

				public static void BootUpFunctionVector3(ContainerVector3 container)
				{
					switch(container.TypePack)
					{
						case KindTypePack.STANDARD_UNCOMPRESSED:
							container.Function = StandardUncompressed.FunctionVector3;
							break;

						case KindTypePack.STANDARD_CPE:
							container.Function = StandardCPE.FunctionVector3;
							break;

						case KindTypePack.CPE_FLYWEIGHT:
							container.Function = CPE_Flyweight.FunctionVector3;
							break;

						case KindTypePack.CPE_INTERPOLATE:
							container.Function = CPE_Interpolate.FunctionVector3;
							break;

						default:
							break;
					}
				}

				public static void BootUpFunctionStatus(ContainerStatus container)
				{
					switch(container.TypePack)
					{
						case KindTypePack.STANDARD_UNCOMPRESSED:
							container.Function = StandardUncompressed.FunctionStatus;
							break;

						case KindTypePack.STANDARD_CPE:
							container.Function = StandardCPE.FunctionStatus;
							break;

						case KindTypePack.CPE_FLYWEIGHT:
							container.Function = null;	/* Not Support */
							break;

						case KindTypePack.CPE_INTERPOLATE:
							container.Function = null;	/* Not Support */
							break;

						default:
							break;
					}
				}

				public static void BootUpFunctionCell(ContainerCell container)
				{
					switch(container.TypePack)
					{
						case KindTypePack.STANDARD_UNCOMPRESSED:
							container.Function = StandardUncompressed.FunctionCell;
							break;

						case KindTypePack.STANDARD_CPE:
							container.Function = StandardCPE.FunctionCell;
							break;

						case KindTypePack.CPE_FLYWEIGHT:
							container.Function = null;	/* Not Support */
							break;

						case KindTypePack.CPE_INTERPOLATE:
							container.Function = null;	/* Not Support */
							break;

						default:
							break;
					}
				}

				public static void BootUpFunctionPartsColor(ContainerPartsColor container)
				{
					switch(container.TypePack)
					{
						case KindTypePack.STANDARD_UNCOMPRESSED:
							container.Function = StandardUncompressed.FunctionPartsColor;
							break;

						case KindTypePack.STANDARD_CPE:
							container.Function = StandardCPE.FunctionPartsColor;
							break;

						case KindTypePack.CPE_FLYWEIGHT:
							container.Function = CPE_Flyweight.FunctionPartsColor;
							break;

						case KindTypePack.CPE_INTERPOLATE:
							container.Function = CPE_Interpolate.FunctionPartsColor;
							break;

						default:
							break;
					}
				}

				public static void BootUpFunctionVertexCorrection(ContainerVertexCorrection container)
				{
					switch(container.TypePack)
					{
						case KindTypePack.STANDARD_UNCOMPRESSED:
							container.Function = StandardUncompressed.FunctionVertexCorrection;
							break;

						case KindTypePack.STANDARD_CPE:
							container.Function = StandardCPE.FunctionVertexCorrection;
							break;

						case KindTypePack.CPE_FLYWEIGHT:
							container.Function = CPE_Flyweight.FunctionVertexCorrection;
							break;

						case KindTypePack.CPE_INTERPOLATE:
							container.Function = CPE_Interpolate.FunctionVertexCorrection;
							break;

						default:
							break;
					}
				}

				public static void BootUpFunctionUserData(ContainerUserData container)
				{
					switch(container.TypePack)
					{
						case KindTypePack.STANDARD_UNCOMPRESSED:
							container.Function = null;	/* Not Support */
							break;

						case KindTypePack.STANDARD_CPE:
							container.Function = StandardCPE.FunctionUserData;
							break;

						case KindTypePack.CPE_FLYWEIGHT:
							container.Function = null;	/* Not Support */
							break;

						case KindTypePack.CPE_INTERPOLATE:
							container.Function = null;	/* Not Support */
							break;

						default:
							break;
					}
				}

				public static void BootUpFunctionInstance(ContainerInstance container)
				{
					switch(container.TypePack)
					{
						case KindTypePack.STANDARD_UNCOMPRESSED:
							container.Function = null;	/* Not Support */
							break;

						case KindTypePack.STANDARD_CPE:
							container.Function = StandardCPE.FunctionInstance;
							break;

						case KindTypePack.CPE_FLYWEIGHT:
							container.Function = null;	/* Not Support */
							break;

						case KindTypePack.CPE_INTERPOLATE:
							container.Function = null;	/* Not Support */
							break;

						default:
							break;
					}
				}

				public static void BootUpFunctionEffect(ContainerEffect container)
				{
					switch(container.TypePack)
					{
						case KindTypePack.STANDARD_UNCOMPRESSED:
							container.Function = null;	/* Not Support */
							break;

						case KindTypePack.STANDARD_CPE:
							container.Function = StandardCPE.FunctionEffect;
							break;

						case KindTypePack.CPE_FLYWEIGHT:
							container.Function = null;	/* Not Support */
							break;

						case KindTypePack.CPE_INTERPOLATE:
							container.Function = null;	/* Not Support */
							break;

						default:
							break;
					}
				}

				public static void BootUpFunctionDeform(ContainerDeform container)
				{
					switch(container.TypePack)
					{
						case KindTypePack.STANDARD_UNCOMPRESSED:
							container.Function = StandardUncompressed.FunctionDeform;
							break;

						case KindTypePack.STANDARD_CPE:
							container.Function = StandardCPE.FunctionDeform;
							break;

						case KindTypePack.CPE_FLYWEIGHT:
							container.Function = null;	/* Not Support */
							break;

						case KindTypePack.CPE_INTERPOLATE:
							container.Function = CPE_Interpolate.FunctionDeform;
							break;

						default:
							break;
					}
				}

				public static bool DictionaryBootUp(int indexAnimation, int indexParts, Script_SpriteStudio6_DataAnimation dataAnimation)
				{
					bool flagSuccess = true;

					flagSuccess &= StandardUncompressed.DictionaryBootUp(indexAnimation, indexParts, dataAnimation);
					flagSuccess &= StandardCPE.DictionaryBootUp(indexAnimation, indexParts, dataAnimation);
					flagSuccess &= CPE_Flyweight.DictionaryBootUp(indexAnimation, indexParts, dataAnimation);
					flagSuccess &= CPE_Interpolate.DictionaryBootUp(indexAnimation, indexParts, dataAnimation);

					return(flagSuccess);
				}

				public static bool DictionaryShutDown(int indexAnimation, int indexParts, Script_SpriteStudio6_DataAnimation dataAnimation)
				{
					bool flagSuccess = true;

					flagSuccess &= StandardUncompressed.DictionaryShutDown(indexAnimation, indexParts, dataAnimation);
					flagSuccess &= StandardCPE.DictionaryShutDown(indexAnimation, indexParts, dataAnimation);
					flagSuccess &= CPE_Flyweight.DictionaryShutDown(indexAnimation, indexParts, dataAnimation);
					flagSuccess &= CPE_Interpolate.DictionaryShutDown(indexAnimation, indexParts, dataAnimation);

					return(flagSuccess);
				}
				#endregion Functions
			}
		}
	}
}
