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

[System.Serializable]
public class Script_SpriteStudio6_DataCellMap : ScriptableObject
{
	/* ----------------------------------------------- Variables & Properties */
	#region Variables & Properties
	public KindVersion Version;
	public Library_SpriteStudio6.Data.CellMap[] TableCellMap;
	#endregion Variables & Properties

	/* ----------------------------------------------- Functions */
	#region Functions
	public void CleanUp()
	{
		Version = (KindVersion)(-1);
		TableCellMap = null;
	}

	public bool VersionCheckRuntime()
	{
		return(((KindVersion.SUPPORT_EARLIEST <= Version) && (KindVersion.SUPPORT_LATEST >= Version)));	/* ? true : false */
	}

	public int CountGetCellMap()
	{
		return((null == TableCellMap) ? -1 : TableCellMap.Length);
	}

	public int IndexGetCellMap(string name)
	{
		if(true == string.IsNullOrEmpty(name))
		{
			return(-1);
		}

		int count = TableCellMap.Length;
		for(int i=0; i<count; i++)
		{
			if(name == TableCellMap[i].Name)
			{
				return(i);
			}
		}
		return(-1);
	}

	public Library_SpriteStudio6.Data.CellMap DataGetCellMap(int index)
	{
		return(((0 > index) || (TableCellMap.Length <= index)) ? null : TableCellMap[index]);
	}
	#endregion Functions

	/* ----------------------------------------------- Enums & Constants */
	#region Enums & Constants
	public enum KindVersion
	{
		SUPPORT_EARLIEST = CODE_010000,
		SUPPORT_LATEST = CODE_010001,

		SS5PU = 0x00000000,	/* Before SS5PU *//* (Reserved) */
		CODE_010000 = 0x00010000,	/* SS6PU Ver.0.8.0 */
		CODE_010001 = 0x00010001,	/* SS6PU Ver.0.9.0 */
			/* MEMO: Change Members "Library_SpriteStudio6.Data.CellMap.Cell.DataMesh" */
	}
	#endregion Enums & Constants
}
