/**
	SpriteStudio6 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
#define MESSAGE_DATAVERSION_INVALID

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using RandomGenerator = Library_SpriteStudio6.Utility.Random.XorShift32;

[ExecuteInEditMode]
[System.Serializable]
public partial class Script_SpriteStudio6_RootEffect : Library_SpriteStudio6.Script.Root
{
	/* ----------------------------------------------- Variables & Properties */
	#region Variables & Properties
	public Script_SpriteStudio6_DataEffect DataEffect;

	public Library_SpriteStudio6.Control.Effect ControlEffect;

	public int LimitParticleDraw;
	internal int CountParticleMax = 0;	/* use only Highest-Parent-Root */

	private FlagBitStatus Status = FlagBitStatus.CLEAR;
	internal bool StatusIsValid
	{
		get
		{
			return(0 != (Status & FlagBitStatus.VALID));
		}
	}
	internal bool StatusIsPlaying
	{
		get
		{
			return(0 != (Status & FlagBitStatus.PLAYING));
		}
	}
	internal bool StatusIsPlayingInfinity
	{
		get
		{
			return(0 != (Status & FlagBitStatus.PLAYING_INFINITY));
		}
		set
		{
			Status = (true == value) ? (Status | FlagBitStatus.PLAYING_INFINITY) : (Status & ~FlagBitStatus.PLAYING_INFINITY);
		}
	}
	internal bool StatusIsUpdateRateScaleLocal
	{
		get
		{
			return(0 != (Status & FlagBitStatus.UPDATE_RATE_SCALELOCAL));
		}
	}
	internal bool StatusIsUpdateRateOpacity
	{
		get
		{
			return(0 != (Status & FlagBitStatus.UPDATE_RATE_OPACITY));
		}
	}
	internal bool StatusIsChangeTableMaterial
	{
		get
		{
			return(0 != (Status & FlagBitStatus.CHANGE_TABLEMATERIAL));
		}
	}
	internal bool StatusIsChangeCellMap
	{
		get
		{
			return(0 != (Status & FlagBitStatus.CHANGE_CELLMAP));
		}
	}

	/* MEMO: bellow 2 properties (RateOpacity/RateScaleLocal) are used to control from parent animation. */
	/*       In principle, do not change the value. Correctly operation is not guaranteed.               */
	internal float RateOpacity
	{
		get
		{
			return(RateOpacityForce);
		}
		set
		{
			RateOpacityForce = value;
			Status |= FlagBitStatus.UPDATE_RATE_OPACITY;
		}
	}
	internal Vector2 RateScaleLocal
	{
		get
		{
			return(RateScaleLocalForce);
		}
		set
		{
			RateScaleLocalForce = value;
			Status |= FlagBitStatus.UPDATE_RATE_SCALELOCAL;
		}
	}

	/* MEMO: Status of animation's play-track are diverted. (Since wasteful of redefine same content) */
	/* MEMO: "Effect" have no multi-track playing capcity. */
	internal Library_SpriteStudio6.Control.Animation.Track.FlagBitStatus StatusPlaying;
	internal float TimePerFrame;
	internal float TimeElapsed;
	public float RateTime;

	internal float Frame;
	internal float FrameRange;
	internal float FramePerSecond;

	private Library_SpriteStudio6.CallBack.FunctionTimeElapseEffect FunctionExecTimeElapse = FunctionTimeElapseDefault;
	internal Library_SpriteStudio6.CallBack.FunctionTimeElapseEffect FunctionTimeElapse
	{
		get
		{
			return(FunctionExecTimeElapse);
		}
		set
		{
			FunctionExecTimeElapse = (null != value) ? value : FunctionTimeElapseDefault;
		}
	}
	internal Library_SpriteStudio6.CallBack.FunctionPlayEndEffect FunctionPlayEnd = null;
	#endregion Variables & Properties

	/* ----------------------------------------------- MonoBehaviour-Functions */
	#region MonoBehaviour-Functions
	void Awake()
	{
		if((null == DataCellMap) || (null == DataEffect))
		{	/* Data invalid */
			return;
		}
		if((false == DataEffect.VersionCheckRuntime()) || (false == DataCellMap.VersionCheckRuntime()))
		{	/* Data-Version invalid */
#if MESSAGE_DATAVERSION_INVALID
			Debug.LogError(	"SS6PU Error(Runtime): Not supported data-version. Need to re-import data. GameObject[" + name
							+ "] Effect[" + DataEffect.Version.ToString()
							+ "] CellMap[" + DataCellMap.Version.ToString()
							+ "]"
				);
#endif
			return;
		}

		/* Awake Base-Class */
		BaseAwake();
	}

	void Start()
	{
		if(0 == (Status & FlagBitStatus.VALID))
		{
			StartMain();
		}
	}
	internal void StartMain()
	{
		/* Boot up master datas */
		/* MEMO: Reason why initial setting of ScriptableObject is done here     */
		/*        (without processing with ScriptableObject's Awake or OnEnable) */
		/*        is to stabilize execution such when re-compile.                */
		if((null == DataCellMap) || (null == DataEffect))
		{	/* Data invalid */
			goto Start_ErrorEnd;
		}
		if((false == DataCellMap.VersionCheckRuntime()) || (false == DataEffect.VersionCheckRuntime()))
		{	/* Data-Version invalid */
			goto Start_ErrorEnd;
		}
		FunctionBootUpDataEffect();

		/* Start Base-Class */
		BaseStart();

		/* Boot up Draw-Cluster */
		/* MEMO: Need to run before "ControlEffect.BootUp". */
		if(false == ClusterBootUpDraw())
		{
			goto Start_ErrorEnd;
		}

		/* Boot up Effect-Control */
		if(false == ControlEffect.BootUp(this))
		{
			goto Start_ErrorEnd;
		}
		FrameRange = (float)ControlEffect.DurationFull;

		Status |= FlagBitStatus.VALID;

		/* Play Animation Initialize */
		if(null == InstanceRootParent)
		{
			AnimationPlay();
		}

		return;

	Start_ErrorEnd:;
		Status &= ~FlagBitStatus.VALID;
		return;
	}

//	void Update()
//	{
//	}

	void LateUpdate()
	{
		if(null == InstanceRootParent)
		{
			/* MEMO: Execute only at the "Highest Parent(not under anyone's control)"-Root part.         */
			/*       "Child"-Root parts' "LateUpdatesMain" are called from Parent's internal processing. */
			if(true == RendererBootUpDraw(false))
			{
				Matrix4x4 matrixInverseMeshRenderer = InstanceMeshRenderer.localToWorldMatrix.inverse;
				float timeElapsed = FunctionExecTimeElapse(this);
#if UNITY_EDITOR
				/* MEMO: Since time may pass even when not "Play Mode", prevents it. */
				if(false == EditorApplication.isPlaying)
				{
					timeElapsed = 0.0f;
				}
#endif
				LateUpdateMain(	timeElapsed,
								false,
								Library_SpriteStudio6.KindMasking.THROUGH,	/* FOLLOW_DATA */
								ref matrixInverseMeshRenderer,
								false
							);
			}
		}
	}
	internal void LateUpdateMain(	float timeElapsed,
									bool flagHideDefault,
									Library_SpriteStudio6.KindMasking masking,
									ref Matrix4x4 matrixCorrection,
									bool flagPlanarization
								)
	{
		if(0 == (Status & FlagBitStatus.VALID))
		{
			return;
		}
		if(null == DataEffect)
		{
			return;
		}
		if(false == DataEffect.StatusIsBootup)
		{	/* Re-import data? or Re-compile during playing animation? */
			return;
		}

		bool flagHide = flagHideDefault | FlagHideForce;

		/* Update Base */
		BaseLateUpdate(timeElapsed);

		/* Update Playing-Status */
		if(0 == (StatusPlaying & Library_SpriteStudio6.Control.Animation.Track.FlagBitStatus.PLAYING)) 
		{	/* Not-Playing */
			Status &= ~FlagBitStatus.PLAYING;
			return;
		}

		Status |= FlagBitStatus.PLAYING;
		TimeElapsed += (	(0 != (StatusPlaying & Library_SpriteStudio6.Control.Animation.Track.FlagBitStatus.PAUSING))
							|| (0 != (StatusPlaying & Library_SpriteStudio6.Control.Animation.Track.FlagBitStatus.PLAYING_START))
						) ? 0.0f : (timeElapsed * RateTime);
		Frame = TimeElapsed * FramePerSecond;
		if(0 != (Status & FlagBitStatus.PLAYING_INFINITY))
		{	/* Independent */
//			Frame %= FrameRange;
		}
		else
		{	/* Dependent */
			Frame = Mathf.Clamp(Frame, 0.0f, FrameRange);
		}

		/* Recover Draw-Cluster & Component for Rendering */
		if(null == ClusterDraw)
		{	/* Lost */
			if(false == ClusterBootUpDraw())
			{	/* Recovery failure */
				return;
			}
		}

		/* MEMO: Execute combining and drawing only at Highest-Parent-Root. */
		/* Clean Draw-Cluster & Component for Rendering */
		if(null == InstanceRootParent)
		{
			ClusterDraw.DataPurge();

//			if(false == RendererBootUpDraw(false))
//			{	/* Recovery failure */
//				return;
//			}
		}

		/* Update & Draw Effect */
		if(false == flagHide)
		{
			ControlEffect.Update(this, masking, ref matrixCorrection, flagPlanarization);
		}

		/* Mesh Combine & Set to Renderer */
		if((null == InstanceRootParent) && (null != MeshCombined))
		{
			/* Clear Mesh */
			MeshCombined.Clear();
			MeshCombined.name = NameBatchedMesh;

			if(false == flagHide)
			{
				/* MEMO: Set the material-array to null issue "NullReferenceException". Leave as. */
				if(true == ClusterDraw.MeshCombine(MeshCombined, ref TableMaterialCombined))
				{
					InstanceMeshRenderer.sharedMaterials = TableMaterialCombined;
				}
			}
			InstanceMeshFilter.sharedMesh = MeshCombined;
		}

		/* Clear transient status */
		StatusPlaying &= ~Library_SpriteStudio6.Control.Animation.Track.FlagBitStatus.PLAYING_START;
		Status &= ~(	FlagBitStatus.UPDATE_RATE_SCALELOCAL
						| FlagBitStatus.UPDATE_RATE_OPACITY
						| FlagBitStatus.CHANGE_TABLEMATERIAL
						| FlagBitStatus.CHANGE_CELLMAP
					);
	}
	#endregion MonoBehaviour-Functions

	/* ----------------------------------------------- Functions */
	#region Functions
	private void FunctionBootUpDataEffect()
	{
		if(null == DataEffect)
		{
			return;
		}
		if(true == DataEffect.StatusIsBootup)
		{
			return;
		}

		/* Recover Material */
		DataEffect.BootUpTableMaterial();

		/* Set Signature-Bootup */
		DataEffect.StatusIsBootup = true;
	}

	private bool ClusterBootUpDraw()
	{
		CountParticleMax = CountGetDrawMesh();

		if(null != InstanceRootParent)
		{	/* Child */
			ClusterDraw = InstanceRootParent.ClusterDraw;
		}
		else
		{	/* Highest-Root */
			ClusterDraw = new Library_SpriteStudio6.Draw.Cluster();
			if(null == ClusterDraw)
			{
				goto ClusterBootUpDraw_ErrorEnd;
			}
			if(false == ClusterDraw.BootUp(0, 0, CountParticleMax))
			{
				goto ClusterBootUpDraw_ErrorEnd;
			}
		}

		return(true);

	ClusterBootUpDraw_ErrorEnd:;
		CountParticleMax = 0;
		ClusterDraw = null;
		return(false);
	}
	/* MEMO: This function is used only for Inspector */
	public bool ClusterRebootDraw()
	{
		if(null != InstanceRootParent)
		{
			return(false);
		}
		return(ClusterBootUpDraw());
	}

	internal int CountGetDrawMesh()
	{
		return((0 == LimitParticleDraw) ? (int)Defaults.LIMIT_PARTICLEDRAW : LimitParticleDraw);
	}

	internal void SeedOffsetSet(uint seed)
	{
		ControlEffect.SeedOffsetSet(seed);
	}

	public static Library_SpriteStudio6.Utility.Random.Generator InstanceCreateRandom()
	{
		return(new RandomGenerator());
	}

	public static uint KeyCreateRandom()
	{
		RandomKeyMakeID++;

		/* MEMO: time(0) at C++ */
		System.DateTime TimeNow = System.DateTime.Now;
		TimeNow.ToUniversalTime();
		System.TimeSpan SecNow = TimeNow - TimeUnixEpoch;
		
		return(RandomKeyMakeID + (uint)SecNow.TotalSeconds);
	}

	internal void TimeSkip(float time, bool flagReverseParent)
	{	/* MEMO: In principle, This Function is for calling from "Library_SpriteStudio6.Control.Animation.Parts.DrawEffect". */
		TimeElapsed = time;
	}

	internal float TimeGetFramePosition(int frame)
	{	/* MEMO: In principle, This Function is for calling from "Library_SpriteStudio6.Control.Animation.Parts.DrawEffect". */
		return((float)frame * TimePerFrame);
	}

	private static float FunctionTimeElapseDefault(Script_SpriteStudio6_RootEffect scriptRoot)
	{
		return(Time.deltaTime);
	}

	/* Part: SpriteStudio6/Script/RootEffect/FunctionAnimation.cs */
	/* Part: SpriteStudio6/Script/RootEffect/FunctionCell.cs */
	/* Part: SpriteStudio6/Script/RootEffect/FunctionMaterial.cs */
	/* Part: SpriteStudio6/Script/Root/FunctionMisc.cs */
	#endregion Functions

	/* ----------------------------------------------- Enums & Constants */
	#region Enums & Constants
	public enum Constants
	{
		LIMIT_SUBEMITTER_DEPTH = 2,
		LIMIT_SUBEMITTER_COUNT = 10,
	}
	public enum Defaults
	{
		LIMIT_PARTICLEDRAW = 1024,
	}

	[System.Flags]
	private enum FlagBitStatus
	{
		VALID = 0x40000000,
		PLAYING = 0x20000000,
		PLAYING_INFINITY = 0x1000000,

		UPDATE_RATE_SCALELOCAL = 0x08000000,
		UPDATE_RATE_OPACITY = 0x04000000,

		CHANGE_TABLEMATERIAL = 0x00800000,
		CHANGE_CELLMAP = 0x00400000,

		CLEAR = 0x00000000,
	}

	private readonly static System.DateTime TimeUnixEpoch = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
	private static uint RandomKeyMakeID = 123456;
	#endregion Enums & Constants
}
