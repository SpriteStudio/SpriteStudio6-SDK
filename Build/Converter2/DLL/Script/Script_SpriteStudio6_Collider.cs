/**
	SpriteStudio6 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public partial class Script_SpriteStudio6_Collider : MonoBehaviour
{
	/* ----------------------------------------------- Variables & Properties */
	#region Variables & Properties
	private GameObject InstanceGamaObject = null;
	private Collider InstanceCollider = null;

	public Script_SpriteStudio6_Root InstanceRoot;
	public int IDParts;

	public CapsuleCollider InstanceColliderCapsule;
	private float Radius = 1.0f;	/* Initial value set by importer */

	public BoxCollider InstanceColliderBox;
	private Vector3 SizeRectangle = Vector3.one;	/* Initial value set by importer */
	private Vector3 PivotRectangle = Vector3.zero;	/* Initial value set by importer */
	#endregion Variables & Properties

	/* ----------------------------------------------- MonoBehaviour-Functions */
	#region MonoBehaviour-Functions
//	void Awake()
//	{
//	}

	void Start()
	{
		if(null != InstanceColliderBox)
		{
			SizeRectangle = InstanceColliderBox.size;
			PivotRectangle = InstanceColliderBox.center;
		}
		if(null != InstanceColliderCapsule)
		{
			Radius = InstanceColliderCapsule.radius;
		}
	}

//	void Update()
//	{
//	}

// 	void LateUpdate()
//	{
//	}

	void OnTriggerEnter(Collider pair)
	{
		if((null != InstanceRoot) && (null != InstanceRoot.FunctionColliderEnter))
		{
			BootUp();
			InstanceRoot.FunctionColliderEnter(InstanceRoot, InstanceGamaObject, InstanceRoot.DataAnimation.TableParts[IDParts].Name, IDParts, InstanceCollider, pair);
		}
	}

	void OnTriggerExit(Collider pair)
	{
		if((null != InstanceRoot) && (null != InstanceRoot.FunctionColliderExit))
		{
			BootUp();
			InstanceRoot.FunctionColliderExit(InstanceRoot, InstanceGamaObject, InstanceRoot.DataAnimation.TableParts[IDParts].Name, IDParts, InstanceCollider, pair);
		}
	}

	void OnTriggerStay(Collider pair)
	{
		if((null != InstanceRoot) && (null != InstanceRoot.FunctionColliderStay))
		{
			BootUp();
			InstanceRoot.FunctionColliderStay(InstanceRoot, InstanceGamaObject, InstanceRoot.DataAnimation.TableParts[IDParts].Name, IDParts, InstanceCollider, pair);
		}
	}

	void OnCollisionEnter(Collision contacts)
	{
		if((null != InstanceRoot) && (null != InstanceRoot.FunctionCollisionEnter))
		{
			BootUp();
			InstanceRoot.FunctionCollisionEnter(InstanceRoot, InstanceGamaObject, InstanceRoot.DataAnimation.TableParts[IDParts].Name, IDParts, InstanceCollider, contacts);
		}
	}

	void OnCollisionExit(Collision contacts)
	{
		if((null != InstanceRoot) && (null != InstanceRoot.FunctionCollisionExit))
		{
			BootUp();
			InstanceRoot.FunctionCollisionExit(InstanceRoot, InstanceGamaObject, InstanceRoot.DataAnimation.TableParts[IDParts].Name, IDParts, InstanceCollider, contacts);
		}
	}

	void OnCollisionStay(Collision contacts)
	{
		if((null != InstanceRoot) && (null != InstanceRoot.FunctionCollisionStay))
		{
			BootUp();
			InstanceRoot.FunctionCollisionStay(InstanceRoot, InstanceGamaObject, InstanceRoot.DataAnimation.TableParts[IDParts].Name, IDParts, InstanceCollider, contacts);
		}
	}
	#endregion MonoBehaviour-Functions

	/* ----------------------------------------------- Functions */
	#region Functions
	private void BootUp()
	{
		if(null == InstanceGamaObject)
		{
			InstanceGamaObject = gameObject;
		}

		if(null == InstanceCollider)
		{
			if(null != InstanceColliderBox)
			{
				InstanceCollider = InstanceColliderBox;
				goto BootUp_ColliderConfirmed;
			}

			if(null != InstanceColliderCapsule)
			{
				InstanceCollider = InstanceColliderCapsule;
				goto BootUp_ColliderConfirmed;
			}
		}
	BootUp_ColliderConfirmed:;
	}

	internal bool ColliderSetEnable(bool flagSwitch)
	{
		BootUp();
		if(null == InstanceCollider)
		{
			return(false);
		}

		InstanceCollider.enabled = flagSwitch;
		return(true);
	}

	internal bool ColliderSetRectangle(ref Vector3 size, ref Vector3 pivot)
	{
		BootUp();
		if(null == InstanceColliderBox)
		{
			return(false);
		}

		/* MEMO: Deformation of collider takes time, so update as little as possible. */
		if(SizeRectangle != size)
		{
			InstanceColliderBox.size = size;
			SizeRectangle = size;
		}
		if(PivotRectangle != pivot)
		{
			InstanceColliderBox.center = pivot;
			PivotRectangle = pivot;
		}

		return(true);
	}

	internal bool ColliderSetRadius(float radius)
	{
		BootUp();
		if(null == InstanceColliderCapsule)
		{
			return(false);
		}

		/* MEMO: Deformation of collider takes time, so update as little as possible. */
		if(Radius != radius)
		{
			InstanceColliderCapsule.radius = radius;
			Radius = radius;
		}

		return(true);
	}
	#endregion Functions
}
