/**
	SpriteStudio6 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[System.Serializable]
public partial class Script_SpriteStudio6_PartsUnityNative : MonoBehaviour
{
	/* ----------------------------------------------- Variables & Properties */
	#region Variables & Properties
	public Script_SpriteStudio6_RootUnityNative PartsRoot;

	/* MEMO: Can not control except float from "AnimationClip". */
	public float OrderInLayer;

	/* MEMO: When change cell from script, change this valiable not "SpriteRenderer.sprite (or SpriteMask.sprite)". */
	public Sprite Cell;

	/* MEMO: When change cell from script, change this valiable not "SkinnedMeshRenderer.sharedMesh". */
	public Mesh CellMesh;
	public Texture2D TextureMesh;

	/* MEMO: Do not change these valiables. (Set only from importer) */
	public Transform[] TableTransformBone;

	public SpriteRenderer InstanceSpriteRenderer;
#if UNITY_2017_1_OR_NEWER
	public SpriteMask InstanceSpriteMask;
#else
	/* MEMO: Can not use "SpriteMask" in Unity5.6 or earlier.                               */
	/*       (For "Nintendo Switch" for the time being, corresponds to Unity5.6 or earlier) */
#endif
	public SkinnedMeshRenderer InstanceSkinnedMeshRenderer;
	public MeshRenderer InstanceMeshRenderer;	/* For mesh to which no bone is assigned. */
	public MeshFilter InstanceMeshFilter;	/* For mesh to which no bone is assigned. */

	private float OrderInLayerPrevious = float.NaN;
	private Sprite CellPrevious = null;
	private Mesh CellMeshPrevious = null;
	private Texture2D TextureMeshPrevious = null;

	private Mesh InstanceCellMesh = null;
	private Matrix4x4[] TableMatrixBindPose = null;

	private static MaterialPropertyBlock PropertyMaterial = null;
	private static int IDMaterialMainTexture = -1;
	private static int IDMaterialRectangleCell = -1;
	private static int IDMaterialPivotCell = -1;
	#endregion Variables & Properties

	/* ----------------------------------------------- MonoBehaviour-Functions */
	#region MonoBehaviour-Functions
//	void Awake()
//	{
//	}

	void Start()
	{
//		InstanceSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		if(null != InstanceSpriteRenderer)
		{
			goto Start_End;
		}

#if UNITY_2017_1_OR_NEWER
//		InstanceSpriteMask = gameObject.GetComponent<SpriteMask>();
		if(null != InstanceSpriteMask)
		{
			goto Start_End;
		}
#else
		/* MEMO: Can not use "SpriteMask" in Unity5.6 or earlier.                               */
		/*       (For "Nintendo Switch" for the time being, corresponds to Unity5.6 or earlier) */
#endif

//		InstanceSkinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
		if(null != InstanceSkinnedMeshRenderer)
		{
#if false
			/* Create Bind-Pose */
			Matrix4x4 matrixLocalToWorld = transform.localToWorldMatrix;
			if(null != TableTransformBone)
			{
				int countTransformBone = TableTransformBone.Length;
				TableMatrixBindPose = new Matrix4x4[countTransformBone];
				if(null == TableMatrixBindPose)
				{
					goto Start_ErrorEnd;
				}

				for(int i=0; i<countTransformBone; i++)
				{
					TableMatrixBindPose[i] = TableTransformBone[i].worldToLocalMatrix * matrixLocalToWorld;
				}
			}

			InstanceSkinnedMeshRenderer.bones = TableTransformBone;
#else
			TableMatrixBindPose = null;
#endif
 			goto Start_End;
		}

		if(null != InstanceMeshRenderer)
		{
 			goto Start_End;
		}

//	Start_ErrorEnd:;
		return;

	Start_End:;
		return;
	}

//	void Update()
//	{
//	}

	void LateUpdate()
	{
		if(null == PropertyMaterial)
		{
			PropertyMaterial = new MaterialPropertyBlock();

			IDMaterialMainTexture = Shader.PropertyToID("_MainTex");
			IDMaterialRectangleCell = Shader.PropertyToID("_CellRectangle");
			IDMaterialPivotCell = Shader.PropertyToID("_CellPivot_LocalScale");
		}

		/* MEMO: "SpriteRenderer", "SpriteMask" and "SkinnedMeshRenderer" do not coexist. */
		int sortingOrder = 0;
		int sortingOffsetParts = 1;
		if(null != PartsRoot)
		{
			sortingOrder = PartsRoot.SortingOrder;
			sortingOffsetParts = PartsRoot.SortingOffsetPartsDraw;
			if(0 >= sortingOffsetParts)
			{
				sortingOffsetParts = 1;
			}
		}
#if UNITY_2017_1_OR_NEWER
		int sortingOrderBase = sortingOrder;
#else
#endif
		sortingOrder += ((int)OrderInLayer) * sortingOffsetParts;

		/* Sprite (SpriteRenderer) */
		if(null != InstanceSpriteRenderer)
		{
			/* Priority Set */
			if(OrderInLayerPrevious != sortingOrder)
			{
				InstanceSpriteRenderer.sortingOrder = sortingOrder;
				OrderInLayerPrevious = sortingOrder;
			}

			if(null != PropertyMaterial)
			{
				InstanceSpriteRenderer.GetPropertyBlock(PropertyMaterial);

				/* Cell Set */
				if(CellPrevious != Cell)
				{
					/* MEMO: Not enough to just set cell to "SpriteRenderer". (Need to set texture to shader) */
					InstanceSpriteRenderer.sprite = Cell;
					PropertyMaterial.SetTexture(IDMaterialMainTexture, Cell.texture);

					Vector4 temp;
					Rect rectangleCell = Cell.rect;
					temp.x = rectangleCell.xMin;
					temp.y = rectangleCell.yMin;
					temp.z = rectangleCell.width;
					temp.w = rectangleCell.height;
					PropertyMaterial.SetVector(IDMaterialRectangleCell, temp);

					/* MEMO: Since "LocalScale" is stored together in "_CellPivot_LocalScale", overwrite value set by animation. */
					temp = PropertyMaterial.GetVector(IDMaterialPivotCell);
					Vector2 pivot = Cell.pivot;
					temp.x = pivot.x;
					temp.y = rectangleCell.height - pivot.y;
					PropertyMaterial.SetVector(IDMaterialPivotCell, temp);

					CellPrevious = Cell;
				}

				InstanceSpriteRenderer.SetPropertyBlock(PropertyMaterial);
			}

			return;
		}

		/* Mask (SpriteMask) */
#if UNITY_2017_1_OR_NEWER
		if(null != InstanceSpriteMask)
		{
			if(OrderInLayerPrevious != sortingOrder)
			{
				InstanceSpriteMask.frontSortingOrder = sortingOrder;
				InstanceSpriteMask.backSortingOrder = sortingOrderBase - PartsRoot.SortingOffsetPartsDraw;
				OrderInLayerPrevious = sortingOrder;
			}

			if(null != Cell)
			{
				InstanceSpriteMask.sprite = Cell;
				Cell = null;
			}

			return;
		}
#else
		/* MEMO: Can not use "SpriteMask" in Unity5.6 or earlier.                               */
		/*       (For "Nintendo Switch" for the time being, corresponds to Unity5.6 or earlier) */
#endif

		/* Mesh (SkinnedMeshRenderer) */
		if(null != InstanceSkinnedMeshRenderer)
		{
			if(null == TableMatrixBindPose)
			{
				/* Create Bind-Pose */
				Matrix4x4 matrixLocalToWorld = transform.localToWorldMatrix;
				if(null != TableTransformBone)
				{
					int countTransformBone = TableTransformBone.Length;
					TableMatrixBindPose = new Matrix4x4[countTransformBone];
					if(null != TableMatrixBindPose)
					{
						for(int i=0; i<countTransformBone; i++)
						{
							TableMatrixBindPose[i] = TableTransformBone[i].worldToLocalMatrix * matrixLocalToWorld;
						}
					}
				}

				InstanceSkinnedMeshRenderer.bones = TableTransformBone;
			}

			if(OrderInLayerPrevious != sortingOrder)
			{
				InstanceSkinnedMeshRenderer.sortingOrder = sortingOrder;
				OrderInLayerPrevious = sortingOrder;
			}

			if(CellMeshPrevious != CellMesh)
			{
				if(null == InstanceCellMesh)
				{
					InstanceCellMesh = new Mesh();
					if(null == InstanceCellMesh)
					{	/* Error */
						return;
					}
				}

				/* Re-Set Mesh */
				InstanceCellMesh.Clear();
				InstanceCellMesh.name = CellMesh.name;
				InstanceCellMesh.vertices = CellMesh.vertices;
				InstanceCellMesh.uv = CellMesh.uv;
				InstanceCellMesh.triangles = CellMesh.triangles;
				InstanceCellMesh.boneWeights = CellMesh.boneWeights;
				InstanceCellMesh.bindposes = TableMatrixBindPose;

				InstanceSkinnedMeshRenderer.sharedMesh = InstanceCellMesh;

				CellMeshPrevious = CellMesh;
			}

			if(TextureMeshPrevious != TextureMesh)
			{
				InstanceSkinnedMeshRenderer.GetPropertyBlock(PropertyMaterial);

				PropertyMaterial.SetTexture(IDMaterialMainTexture, TextureMesh);

				InstanceSkinnedMeshRenderer.SetPropertyBlock(PropertyMaterial);

				TextureMeshPrevious = TextureMesh;
			}

			return;
		}

		/* Mesh (SkinnedMeshRenderer) */
		if(null != InstanceMeshRenderer)
		{
			if(OrderInLayerPrevious != sortingOrder)
			{
				InstanceMeshRenderer.sortingOrder = sortingOrder;
				OrderInLayerPrevious = sortingOrder;
			}

			if(CellMeshPrevious != CellMesh)
			{
				if(null == InstanceCellMesh)
				{
					InstanceCellMesh = new Mesh();
					if(null == InstanceCellMesh)
					{	/* Error */
						return;
					}
				}

				/* Re-Set Mesh */
				InstanceCellMesh.Clear();
				InstanceCellMesh.name = CellMesh.name;
				InstanceCellMesh.vertices = CellMesh.vertices;
				InstanceCellMesh.uv = CellMesh.uv;
				InstanceCellMesh.triangles = CellMesh.triangles;
//				InstanceCellMesh.boneWeights = 
//				InstanceCellMesh.bindposes = 

				InstanceMeshFilter.sharedMesh = InstanceCellMesh;

				CellMeshPrevious = CellMesh;
			}

			if(TextureMeshPrevious != TextureMesh)
			{
				InstanceMeshRenderer.GetPropertyBlock(PropertyMaterial);

				PropertyMaterial.SetTexture(IDMaterialMainTexture, TextureMesh);

				InstanceMeshRenderer.SetPropertyBlock(PropertyMaterial);

				TextureMeshPrevious = TextureMesh;
			}

			return;
		}
	}
	#endregion MonoBehaviour-Functions

	/* ----------------------------------------------- Enums & Constants */
	#region Enums & Constants
	#endregion Enums & Constants

	/* ----------------------------------------------- Functions */
	#region Functions
	#endregion Functions
}
