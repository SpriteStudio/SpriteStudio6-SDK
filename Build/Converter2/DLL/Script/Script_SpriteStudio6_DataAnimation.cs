/**
	SpriteStudio6 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
// #define STORE_ANIMATIONSETUP_FULL

using System.Collections;
using System.Collections.Generic;
#if !_FOR_SS6CONVERTER_
using UnityEngine;
#else
using SS6ConverterVer2_DLL;
using SS6ConverterVer2_DLL.Types;
using UnityEngine = SS6ConverterVer2_DLL.Assets;
#endif

[System.Serializable]
public class Script_SpriteStudio6_DataAnimation : ScriptableObject
{
	/* ----------------------------------------------- Variables & Properties */
	#region Variables & Properties
#if !_FOR_SS6CONVERTER_
	public KindVersion Version;
#else
	public int Version;
#endif

#if !_FOR_SS6CONVERTER_
	public Material[] TableMaterial;
#endif

	public Library_SpriteStudio6.Data.Parts.Animation[] TableParts;
	public Library_SpriteStudio6.Data.Parts.Animation.Catalog CatalogParts;

	public Library_SpriteStudio6.Data.Animation[] TableAnimation;
	public DataSetup[] TableAnimationPartsSetup;
	internal bool IsAnimationSetup
	{
		get
		{
			return((null != TableAnimationPartsSetup) && (0 < TableAnimationPartsSetup.Length));
		}
	}

	/* PackAttribute's Dictionaries */
//	public Library_SpriteStudio6.Data.Animation.PackAttribute.StandardUncompressed.Dictionary Dictionary_StandardUnCompressed;
//	public Library_SpriteStudio6.Data.Animation.PackAttribute.StandardCPE.Dictionary Dictionary_StandardCPE;
	public Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Flyweight.Dictionary Dictionary_CPE_Flyweight;
//	public Library_SpriteStudio6.Data.Animation.PackAttribute.CPE_Interpolate.Dictionary Dictionary_CPE_Interpolate;

	/* MEMO: Use "delegate" instead of bool because value is cleared each compiling. */
	private FunctionSignatureBootUpFunction SignatureBootUpFunction = null;
	internal bool StatusIsBootup
	{
		get
		{
			return((null != SignatureBootUpFunction) ? true : false);
		}
		set
		{
			if(true == value)
			{
				SignatureBootUpFunction = FunctionBootUpSignature;
			}
			else
			{
				SignatureBootUpFunction = null;
			}
		}
	}
	#endregion Variables & Properties

	/* ----------------------------------------------- ScriptableObject-Functions */
	#region ScriptableObject-Functions
	void Start()
	{
		/* Awake Base-Class */
		CountGetPartsSprite();
		StatusIsBootup = true;
	}
	#endregion ScriptableObject-Functions

	/* ----------------------------------------------- Functions */
	#region Functions
	public void CleanUp()
	{
#if !_FOR_SS6CONVERTER_
		Version = (KindVersion)(-1);
#else
		Version = -1;
#endif
#if !_FOR_SS6CONVERTER_
		TableMaterial = null;
#endif

		TableParts = null;
		CatalogParts.CleanUp();

		TableAnimation = null;
		TableAnimationPartsSetup = null;

		SignatureBootUpFunction = null;
	}

	public bool VersionCheckRuntime()
	{
#if !_FOR_SS6CONVERTER_
		return(((KindVersion.SUPPORT_EARLIEST <= Version) && (KindVersion.SUPPORT_LATEST >= Version)));	/* ? true : false */
#else
		return((((int)KindVersion.SUPPORT_EARLIEST <= Version) && ((int)KindVersion.SUPPORT_LATEST >= Version)));	/* ? true : false */
#endif
	}

	public int CountGetParts()
	{
		return(TableParts.Length);
	}

	public int CountGetPartsSprite()
	{
		int count = 0;
#if !_FOR_SS6CONVERTER_
		if(KindVersion.CODE_010001 > Version)
#else
		if((int)KindVersion.CODE_010001 > Version)
#endif
		{	/* "CatalogParts" not implemented */
			int countParts = TableParts.Length;
			for(int i=0; i<countParts; i++)
			{
				switch(TableParts[i].Feature)
				{
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.ROOT:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NULL:
						break;

//					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE2:
//					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE4:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL:
						count++;
						break;

					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT:
						break;

//					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE2:
//					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE4:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK:
						/* MEMO: "Mask"s are drawn twice(Predraw + Draw). */
						count += 2;
						break;

					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.JOINT:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONE:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MOVENODE:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.CONSTRAINT:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONEPOINT:
						break;

					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH:
						/* MEMO: Not count. (not sprite) */
						break;
				}
			}
		}
		else
		{
//			count =		CatalogParts.TableIDPartsTriangle2.Length
//						+ CatalogParts.TableIDPartsTriangle4.Length
//						+ (	CatalogParts.TableIDPartsMaskTriangle2.Length
//							+ CatalogParts.TableIDPartsMaskTriangle4.Length
//							) * 2;
			count = CatalogParts.TableIDPartsNormal.Length + (CatalogParts.TableIDPartsMask.Length * 2);
		}
		return(count);
	}

	public int CountGetPartsDraw()
	{
		int count = 0;
#if !_FOR_SS6CONVERTER_
		if(KindVersion.CODE_010001 > Version)
#else
		if((int)KindVersion.CODE_010001 > Version)
#endif
		{	/* "CatalogParts" not implemented */
			int countParts = TableParts.Length;
			for(int i=0; i<countParts; i++)
			{
				switch(TableParts[i].Feature)
				{
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.ROOT:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NULL:
						break;

//					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE2:
//					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE4:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL:
						count++;
						break;

					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT:
						count++;
						break;

//					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE2:
//					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE4:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK:
						count++;
						break;

					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.JOINT:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONE:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MOVENODE:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.CONSTRAINT:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONEPOINT:
						break;

					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH:
						/* MEMO: Not count. (not sprite) */
						break;
				}
			}
		}
		else
		{
//			count =		CatalogParts.TableIDPartsTriangle2.Length
//						+ CatalogParts.TableIDPartsTriangle4.Length
//						+ CatalogParts.TableIDPartsMaskTriangle2.Length
//						+ CatalogParts.TableIDPartsMaskTriangle4.Length;
			count = CatalogParts.TableIDPartsNormal.Length + CatalogParts.TableIDPartsMask.Length;
		}
		return(count);
	}

	public int CountGetPartsPreDraw()
	{
		int count = 0;
#if !_FOR_SS6CONVERTER_
		if(KindVersion.CODE_010001 > Version)
#else
		if((int)KindVersion.CODE_010001 > Version)
#endif
		{	/* "CatalogParts" not implemented */
			int countParts = TableParts.Length;
			for(int i=0; i<countParts; i++)
			{
				switch(TableParts[i].Feature)
				{
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.ROOT:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NULL:
						break;

//					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE2:
//					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL_TRIANGLE4:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL:
						break;

					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT:
						break;

//					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE2:
//					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK_TRIANGLE4:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK:
						count++;
						break;

					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.JOINT:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONE:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MOVENODE:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.CONSTRAINT:
					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONEPOINT:
						break;

					case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH:
						break;
				}
			}
		}
		else
		{
//			count =		CatalogParts.TableIDPartsMaskTriangle2.Length
//						+ CatalogParts.TableIDPartsMaskTriangle4.Length;
			count = CatalogParts.TableIDPartsNormal.Length;
		}
		return(count);
	}

	public int IndexGetParts(string name)
	{
		if(true == string.IsNullOrEmpty(name))
		{
			return(-1);
		}

		int count = TableParts.Length;
		for(int i=0; i<count; i++)
		{
			if(name == TableParts[i].Name)
			{
				return(i);
			}
		}
		return(-1);
	}

	public int CountGetAnimation()
	{
		return(TableAnimation.Length);
	}

	public int IndexGetAnimation(string name)
	{
		if(true == string.IsNullOrEmpty(name))
		{
			return(-1);
		}

		int count = TableAnimation.Length;
		for(int i=0; i<count; i++)
		{
			if(name == TableAnimation[i].Name)
			{
				return(i);
			}
		}
		return(-1);
	}

	internal void BootUpTableMaterial()
	{
#if UNITY_EDITOR
		/* Reassignment for shader lost */
		/* MEMO: This process will not work unless on editor. */
		int countTableMaterial = (null != TableMaterial) ? TableMaterial.Length : 0;
		Material material = null;
		for(int i=0; i<countTableMaterial; i++)
		{
			material = TableMaterial[i];
			if(null != material)
			{
				material.shader = Shader.Find(material.shader.name);
			}
		}
		material = null;
#endif
	}

	internal void BootUpInterfaceAttribute()
	{
		int countAnimation = TableAnimation.Length;
		int countParts = TableParts.Length;
		for(int i=0; i<countAnimation; i++)
		{
			for(int j=0; j<countParts; j++)
			{
				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionStatus(TableAnimation[i].TableParts[j].Status);

				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionCell(TableAnimation[i].TableParts[j].Cell);

				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVector3(TableAnimation[i].TableParts[j].Position);
				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVector3(TableAnimation[i].TableParts[j].Rotation);
				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVector2(TableAnimation[i].TableParts[j].Scaling);
				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVector2(TableAnimation[i].TableParts[j].ScalingLocal);

				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionFloat(TableAnimation[i].TableParts[j].RateOpacity);
				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionInt(TableAnimation[i].TableParts[j].Priority);
				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionPartsColor(TableAnimation[i].TableParts[j].PartsColor);
				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVertexCorrection(TableAnimation[i].TableParts[j].VertexCorrection);

				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVector2(TableAnimation[i].TableParts[j].OffsetPivot);
				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVector2(TableAnimation[i].TableParts[j].PositionAnchor);
				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVector2(TableAnimation[i].TableParts[j].SizeForce);
				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVector2(TableAnimation[i].TableParts[j].PositionTexture);
				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionFloat(TableAnimation[i].TableParts[j].RotationTexture);
				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionVector2(TableAnimation[i].TableParts[j].ScalingTexture);

				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionFloat(TableAnimation[i].TableParts[j].RadiusCollision);

				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionUserData(TableAnimation[i].TableParts[j].UserData);
				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionInstance(TableAnimation[i].TableParts[j].Instance);
				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionEffect(TableAnimation[i].TableParts[j].Effect);
				Library_SpriteStudio6.Data.Animation.PackAttribute.BootUpFunctionDeform(TableAnimation[i].TableParts[j].Deform);
			}
		}
	}

	private static void FunctionBootUpSignature()
	{
		/* Dummy-Function */
	}
	#endregion Functions

	/* ----------------------------------------------- Enums & Constants */
	#region Enums & Constants
	public enum KindVersion
	{
		SUPPORT_EARLIEST = CODE_010100,
		SUPPORT_LATEST = CODE_010100,

		SS5PU = 0,	/* Before SS5PU *//* (Reserved) */
		CODE_010000 = 0x00010000,	/* SS6PU Ver.0.8.0 */
		CODE_010001 = 0x00010001,	/* SS6PU Ver.0.9.0 */
			/* MEMO: Add "Library_SpriteStudio6.Data.Parts.Animation.CountMesh" */
			/* MEMO: Add "Library_SpriteStudio6.Data.Parts.Animation.Catalog" */
		CODE_010002 = 0x00010002,	/* SS6PU Ver.0.9.8 */
			/* MEMO: Add "PackAttribute's Dictionaries" */
		CODE_010003 = 0x00010003,	/* SS6PU Ver.1.0.23 */
			/* MEMO: Add "DataSetup" */
		CODE_010004 = 0x00010004,	/* SS6PU Ver.1.0.26 */
			/* MEMO: Shaders changed to integrated type */
		CODE_010005 = 0x00010005,	/* SS6PU Ver.1.0.27 */
			/* MEMO: Shader's stencil-test are changed.  */
			/* MEMO: "Mask" parts' materials are separated for masking and nonmasking. */

		/* MEMO: Before "CODE_010005", can not play on SS6PU Ver.1.1.0. */
		CODE_010100 = 0x00010100,	/* SS6PU Ver.1.1.0 */
			/* MEMO: Abolition of (NORMAL/MASK)_TRIANGLE2" "_ TRIANGLE4".                                                     */
			/*       Always divide "Normal (Sprite)" and "Mask" into 4 triangles according to SpriteStudio6's specifications. */
			/* MEMO: Support "Mesh Deformation" */
			/* MEMO: Support "Z-Position" Priority */
	}
	#endregion Enums & Constants

	/* ----------------------------------------------- Classes, Structs & Interfaces */
	#region Classes, Structs & Interfaces
	[System.Serializable]
	public struct DataSetup
	{
		/* ----------------------------------------------- Variables & Properties */
		#region Variables & Properties
#if STORE_ANIMATIONSETUP_FULL
		public Library_SpriteStudio6.Data.Animation.Attribute.Status Status;

		public Vector3 Position;
		public Vector3 Rotation;
		public Vector2 Scaling;
		public Vector2 ScalingLocal;

		public float RateOpacity;
		public float RateOpacityLocal;
		public int PowerMask;
		public int Priority;
		public Library_SpriteStudio6.Data.Animation.Attribute.PartsColor PartsColor;
		public Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection VertexCorrection;

		public Vector2 OffsetPivot;
		public Vector2 PositionAnchor;
		public Vector2 SizeForce;
		public Vector2 PositionTexture;
		public float RotationTexture;
		public Vector2 ScalingTexture;

		public float RadiusCollision;

		public Library_SpriteStudio6.Data.Animation.Attribute.Instance Instance;
		public Library_SpriteStudio6.Data.Animation.Attribute.Effect Effect;
		public DataDeform Deform;
#endif
		public Library_SpriteStudio6.Data.Animation.Attribute.UserData UserData;

#if STORE_ANIMATIONSETUP_FULL
		public bool IsStatus
		{
			get
			{
				return(Status.IsValid);
			}
		}
		public bool IsPositionX
		{
			get
			{
				return(!float.IsNaN(Position.x));
			}
		}
		public bool IsPositionY
		{
			get
			{
				return(!float.IsNaN(Position.y));
			}
		}
		public bool IsPositionZ
		{
			get
			{
				return(!float.IsNaN(Position.z));
			}
		}
		public bool IsRotationX
		{
			get
			{
				return(!float.IsNaN(Rotation.x));
			}
		}
		public bool IsRotationY
		{
			get
			{
				return(!float.IsNaN(Rotation.y));
			}
		}
		public bool IsRotationZ
		{
			get
			{
				return(!float.IsNaN(Rotation.z));
			}
		}
		public bool IsScalingX
		{
			get
			{
				return(!float.IsNaN(Scaling.x));
			}
		}
		public bool IsScalingY
		{
			get
			{
				return(!float.IsNaN(Scaling.y));
			}
		}
		public bool IsScalingLocalX
		{
			get
			{
				return(!float.IsNaN(ScalingLocal.x));
			}
		}
		public bool IsScalingLocalY
		{
			get
			{
				return(!float.IsNaN(ScalingLocal.y));
			}
		}
		public bool IsRateOpacity
		{
			get
			{
				return(!float.IsNaN(RateOpacity));
			}
		}
		public bool IsRateOpacityLocal
		{
			get
			{
				return(!float.IsNaN(RateOpacityLocal));
			}
		}
		public bool IsPowerMask
		{
			get
			{
				return(int.MinValue != PowerMask);
			}
		}
		public bool IsPriority
		{
			get
			{
				return(int.MinValue != Priority);
			}
		}
		public bool IsPartsColor
		{
			get
			{
				return(Library_SpriteStudio6.KindBoundBlend.NON != PartsColor.Bound);
			}
		}
		public bool IsVertexCorrection
		{
			get
			{
				return((null != VertexCorrection.Coordinate) && (0 < VertexCorrection.Coordinate.Length));
			}
		}
		public bool IsOffsetPivotX
		{
			get
			{
				return(!float.IsNaN(OffsetPivot.x));
			}
		}
		public bool IsOffsetPivotY
		{
			get
			{
				return(!float.IsNaN(OffsetPivot.y));
			}
		}
		public bool IsPositionAnchorX
		{
			get
			{
				return(!float.IsNaN(PositionAnchor.x));
			}
		}
		public bool IsPositionAnchorY
		{
			get
			{
				return(!float.IsNaN(PositionAnchor.y));
			}
		}
		public bool IsSizeForceX
		{
			get
			{
				return(!float.IsNaN(SizeForce.x));
			}
		}
		public bool IsSizeForceY
		{
			get
			{
				return(!float.IsNaN(SizeForce.y));
			}
		}
		public bool IsPositionTextureX
		{
			get
			{
				return(!float.IsNaN(PositionTexture.x));
			}
		}
		public bool IsPositionTextureY
		{
			get
			{
				return(!float.IsNaN(PositionTexture.y));
			}
		}
		public bool IsRotationTexture
		{
			get
			{
				return(!float.IsNaN(RotationTexture));
			}
		}
		public bool IsScalingTextureX
		{
			get
			{
				return(!float.IsNaN(ScalingTexture.x));
			}
		}
		public bool IsScalingTextureY
		{
			get
			{
				return(!float.IsNaN(ScalingTexture.y));
			}
		}
		public bool IsRadiusCollision
		{
			get
			{
				return(!float.IsNaN(RadiusCollision));
			}
		}
		public bool IsInstance
		{
			get
			{
				return(Instance.IsValid);
			}
		}
		public bool IsEffect
		{
			get
			{
				return(Effect.IsValid);
			}
		}
		public bool IsDeform
		{
			get
			{
				return(0 < Deform.CountVertexMesh);
			}
		}
#endif

		public bool IsUserData
		{
			get
			{
				return(UserData.IsValid);
			}
		}
		#endregion Variables & Properties

		/* ----------------------------------------------- Functions */
		#region Functions
		public void CleanUp()
		{
#if STORE_ANIMATIONSETUP_FULL
			int valueInitialInt = int.MinValue;
			float valueInitialFloat = float.NaN;
			Vector2 valueInitialVector2 = new Vector2(float.NaN, float.NaN);
			Vector3 valueInitialVector3 = new Vector3(float.NaN, float.NaN, float.NaN);

			Status = Library_SpriteStudio6.Data.Animation.Attribute.DefaultStatus;

			Position = valueInitialVector3;
			Rotation = valueInitialVector3;
			Scaling = valueInitialVector2;
			ScalingLocal = valueInitialVector2;

			RateOpacity = valueInitialFloat;
			RateOpacityLocal = valueInitialFloat;
			PowerMask = valueInitialInt;
			Priority = valueInitialInt;
			PartsColor.CleanUp();	/* = Library_SpriteStudio6.Data.Animation.Attribute.DefaultPartsColor; */
			VertexCorrection.CleanUp();

			OffsetPivot = valueInitialVector2;
			PositionAnchor = valueInitialVector2;
			SizeForce = valueInitialVector2;
			PositionTexture = valueInitialVector2;
			RotationTexture = valueInitialFloat;
			ScalingTexture = valueInitialVector2;

			RadiusCollision = valueInitialFloat;

			Instance = Library_SpriteStudio6.Data.Animation.Attribute.DefaultInstance;
			Effect = Library_SpriteStudio6.Data.Animation.Attribute.DefaultEffect;
			Deform = DefaultDeform;
#endif
			UserData = Library_SpriteStudio6.Data.Animation.Attribute.DefaultUseData;
		}
		#endregion Functions

		/* ----------------------------------------------- Enums & Constants */
		#region Enums & Constants
		public readonly static DataDeform DefaultDeform = new DataDeform(0);
		#endregion Enums & Constants

		/* ----------------------------------------------- Classes, Structs & Interfaces */
		#region Classes, Structs & Interfaces
		public struct DataDeform
		{
			/* ----------------------------------------------- Variables & Properties */
			#region Variables & Properties
			public int CountVertexMesh;
			public int[] TableIndexVertex;
			public Vector2[] TableCoordinate;
			#endregion Variables & Properties

			/* ----------------------------------------------- Functions */
			#region Functions
			public DataDeform(int dummy)
			{
				CountVertexMesh = 0;
				TableIndexVertex = null;
				TableCoordinate = null;
			}

			public void CleanUp()
			{
				CountVertexMesh = 0;
				TableIndexVertex = null;
				TableCoordinate = null;
			}
			#endregion Functions
		}
		#endregion Classes, Structs & Interfaces
	}
	#endregion Classes, Structs & Interfaces

	/* ----------------------------------------------- Delegates */
	#region Delegates
	private delegate void FunctionSignatureBootUpFunction();
	#endregion Delegates
}
