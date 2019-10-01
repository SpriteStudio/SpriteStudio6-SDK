/**
	SpriteStudio6 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public partial class Script_SpriteStudio6_ControlPrefab : MonoBehaviour
{
	/* ----------------------------------------------- Variables & Properties */
	#region Variables & Properties
	public Object PrefabAnimation;
	#endregion Variables & Properties

	/* ----------------------------------------------- MonoBehaviour-Functions */
	#region MonoBehaviour-Functions
//	void Awake()
//	{
//	}

	void Start()
	{
		if(null != PrefabAnimation)
		{
			GameObject instanceGameObject = Library_SpriteStudio6.Utility.Asset.PrefabInstantiate((GameObject)PrefabAnimation, null, gameObject, false);
			if(null != instanceGameObject)
			{
				/* Instantiate Under-Control Prefab */
				Script_SpriteStudio6_Root instanceScriptRoot = instanceGameObject.GetComponent<Script_SpriteStudio6_Root>();
				if(null != instanceScriptRoot)
				{
					instanceScriptRoot.InstanceGameObjectControl = gameObject;
				}
			}
		}
	}
	
//	void Update()
//	{
//	}

//	void LateUpdate()
//	{
//	}
	#endregion MonoBehaviour-Functions
}
