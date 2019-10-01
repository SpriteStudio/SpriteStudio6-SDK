/**
	SpriteStudio6 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Script_SpriteStudio6_RootEffect
{
	/* ----------------------------------------------- Functions */
	#region Functions
	/* ********************************************************* */
	//! Get CellMap-Table
	/*!
	@param	flagInUse
		true == CellMap-Table of Currently in use<br>
		false == CellMap-Table of original animation data
	@retval	Return-Value
		CellMap-Table

	Get CellMap-Table.<br>
	The cell map table is array of CellMaps, and contains all the cell maps used in animation.<br>
	<br>
	"CellMap-table of original animation data" and "CellMap-table of currently in use" are same
	 unless change the contents of the currently used CellMap using the "TableSetCellMap" function etc.<br>
	(Shallow copy of "CellMap of original animation data", but unlike when use "TableCopyCellMapShallow",
	 only reference to each CellMap of original animation data are stored in another array instance)<br>
	*/
	public Library_SpriteStudio6.Data.CellMap[] TableGetCellMap(bool flagInUse=true)
	{
		if(true == flagInUse)
		{
			return(TableCellMap);
		}

		if(null == DataCellMap)
		{
			return(null);
		}

		return(DataCellMap.TableCellMap);
	}

	/* ********************************************************* */
	//! Set(Change) CellMap-Table
	/*!
	@param	tableCellMap
		Newly set CellMap-table<br>
		null == Revert to initial data
	@retval	Return-Value
		true == Success<br>
		false == Failure (Error)

	Change CellMap-Table of Currently in use.<br>
	<br>
	Attention to the following, since this function doesn't check content of CellMap.<br>
	<br>
	- number of CellMaps used in animation is need to satisfy<br>
	<br>
	- number of cells stored in each CellMap is need to satisfy number of cells used in animation<br>
	<br>
	Identifying cells and CellMaps from animation data is index-based, not name-based.<br>
	(In principle, "SpriteStudio6" sorts cell maps and cells in each cell map in alphabetical order of names.
	 As a result, 0-origined-indexes are sequentially assigned to sorted cells and CellMaps)<br>
	<br>
	Honestly, do not recommend building CellMaps from scratch without using "SpriteStudio6".<br>
	*/
	public bool TableSetCellMap(Library_SpriteStudio6.Data.CellMap[] tableCellMap)
	{
		if(null == tableCellMap)
		{	/* Revert */
			if(null == DataCellMap)
			{
				return(false);
			}

			tableCellMap = DataCellMap.TableCellMap;
			if(null == tableCellMap)
			{
				return(false);
			}
		}

		TableCellMap = tableCellMap;
		Status |= FlagBitStatus.CHANGE_CELLMAP;

		return(true);
	}

	/* ********************************************************* */
	//! Get TableCellMap's length
	/*!
	@param	flagInUse
		true == TableCellMap's length of Currently in use<br>
		false == TableCellMap's length of original animation data
	@retval	Return-Value
		TableCellMap's length

	Get TableCellMap's length.
	*/
	public int CountGetCellMap(bool flagInUse=true)
	{
		Library_SpriteStudio6.Data.CellMap[]  tableCellMap = TableGetCellMap(flagInUse);
		if(null != tableCellMap)
		{
			return(-1);
		}

		return(tableCellMap.Length);
	}

	/* ********************************************************* */
	//! Get CellMap's index
	/*!
	@param	name
		name of CellMap
	@param	flagInUse
		true == TableCellMap's length of Currently in use<br>
		false == TableCellMap's length of original animation data
	@retval	Return-Value
		index of CellMap<br>
		-1 == Not-Found or Failure (Error)

	Get CellMap's index in CellMap-table searching by name.<br>
	*/
	public int IndexGetCellMap(string name, bool flagInUse=true)
	{
		Library_SpriteStudio6.Data.CellMap[]  tableCellMap = TableGetCellMap(flagInUse);
		if(null == tableCellMap)
		{
			return(-1);
		}

		int countCellMap = tableCellMap.Length;
		for(int i=0; i<countCellMap; i++)
		{
			if(tableCellMap[i].Name == name)
			{
				return(i);
			}
		}
		return(-1);
	}

	/* ********************************************************* */
	//! Get CellMap
	/*!
	@param	indexCellMap
		index of CellMap
	@param	flagInUse
		true == TableCellMap's length of Currently in use<br>
		false == TableCellMap's length of original animation data
	@retval	Return-Value
		CellMap<br>
		null == Failure (Error)

	Get specified CellMap in TableCellMap.
	*/
	public Library_SpriteStudio6.Data.CellMap CellMapGet(int indexCellMap, bool flagInUse=true)
	{
		Library_SpriteStudio6.Data.CellMap[]  tableCellMap = TableGetCellMap(flagInUse);
		if(null != tableCellMap)
		{
			return(null);
		}

		if((0 > indexCellMap) || (tableCellMap.Length <= indexCellMap))
		{
			return(null);
		}

		return(tableCellMap[indexCellMap]);
	}

	/* ********************************************************* */
	//! Set(Change) CellMap
	/*!
	@param	indexCellMap
		index of CellMap
	@param	cellMap
		tNewly set CellMap<br>
		null == Revert to initial data
	@retval	Return-Value
		true == Success<br>
		false == Failure (Error)

	Change CellMap of Currently CellMap-table in use.<br>
	<br>
	Attention to the following, since this function doesn't check content of CellMap.<br>
	<br>
	- number of cells stored in "cellMap" is need to satisfy number of cells used in animation<br>
	<br>
	See also the notes on "TableSetCellMap".<br>
	*/
	public bool CellMapSet(int indexCellMap, Library_SpriteStudio6.Data.CellMap cellMap)
	{
		if(null != cellMap)
		{	/* Revert */
			if(null == DataCellMap)
			{
				return(false);
			}

			if((0 > indexCellMap) || (DataCellMap.TableCellMap.Length <= indexCellMap))
			{
				return(false);
			}

			cellMap = DataCellMap.TableCellMap[indexCellMap];
		}

		if(null == TableCellMap)
		{
			return(false);
		}
		if((0 > indexCellMap) || (TableCellMap.Length <= indexCellMap))
		{
			return(false);
		}

		TableCellMap[indexCellMap] = cellMap;
		Status |= FlagBitStatus.CHANGE_CELLMAP;

		return (true);
	}

	/* ********************************************************* */
	//! Shallow-Copy TableCellMap
	/*!
	@param	indexStart
		Index of the first CellMap to copy<br>
		default: 0
	@param	indexEnd
		Index of the last CellMap to copy<br>
		default: -1 (end of TableCellMap)
	@param	flagInUse
		true == Currently in use TableMaterial<br>
		false == TableMaterial in original animation data
	@retval	Return-Value
		New TableMaterial

	Creates a shallow-copy of specified range of TableCellMap.<br>
	In newly created CellMap-table, table and each CellMap are another instances,
	 but each "CellMap.TableCell" and CellMap's name is a reference to contents of original TableCellMap.<br>
	*/
	public Library_SpriteStudio6.Data.CellMap[] TableCopyCellMapShallow(int indexStart=0, int indexEnd=-1, bool flagInUse=true)
	{
		Library_SpriteStudio6.Data.CellMap[]  tableCellMap = TableGetCellMap(flagInUse);
		if(null == tableCellMap)
		{
			return(null);
		}

		if(false == IndexGetCellMapCopy(ref indexStart, ref indexEnd, tableCellMap))
		{
			return(null);
		}

		int countCellMap = (indexEnd - indexStart) + 1;
		Library_SpriteStudio6.Data.CellMap[]  tableCellMapCopy = new Library_SpriteStudio6.Data.CellMap[countCellMap];
		if(null == tableCellMapCopy)
		{
			return(null);
		}

		for(int i=0; i<countCellMap; i++)
		{
			tableCellMapCopy[i] = Library_SpriteStudio6.Utility.Cell.MapCopyShallow(tableCellMap[i + indexStart]);
			if(null == tableCellMapCopy[i])
			{
				tableCellMapCopy = null;
				return(null);
			}
		}

		return(tableCellMapCopy);
	}
	private static bool IndexGetCellMapCopy(ref int indexStart, ref int indexEnd, Library_SpriteStudio6.Data.CellMap[] tableCellMap)
	{
		if(0 > indexStart)
		{
			indexStart = 0;
		}
		if(0 > indexEnd)
		{
			indexEnd = tableCellMap.Length - 1;
		}

		int countCellMap = tableCellMap.Length;
		if((countCellMap <= indexStart) || (countCellMap <= indexEnd))
		{
			return(false);
		}

		return(true);
	}

	/* ********************************************************* */
	//! Deep-Copy TableCellMap
	/*!
	@param	indexStart
		Index of the first CellMap to copy<br>
		default: 0
	@param	indexEnd
		Index of the last CellMap to copy<br>
		default: -1 (end of TableCellMap)
	@param	flagInUse
		true == Currently in use TableMaterial<br>
		false == TableMaterial in original animation data
	@retval	Return-Value
		New TableMaterial

	Creates a deep-copy of specified range of TableCellMap.<br>
	*/
	public Library_SpriteStudio6.Data.CellMap[] TableCopyCellMapDeep(int indexStart=0, int indexEnd=-1, bool flagInUse=true)
	{
		Library_SpriteStudio6.Data.CellMap[]  tableCellMap = TableGetCellMap(flagInUse);
		if(null == tableCellMap)
		{
			return(null);
		}

		if(false == IndexGetCellMapCopy(ref indexStart, ref indexEnd, tableCellMap))
		{
			return(null);
		}

		int countCellMap = (indexEnd - indexStart) + 1;
		Library_SpriteStudio6.Data.CellMap[]  tableCellMapCopy = new Library_SpriteStudio6.Data.CellMap[countCellMap];
		if(null == tableCellMapCopy)
		{
			return(null);
		}

		for(int i=0; i<countCellMap; i++)
		{
			tableCellMapCopy[i] = Library_SpriteStudio6.Utility.Cell.MapCopyDeep(tableCellMap[i + indexStart]);
			if(null == tableCellMapCopy[i])
			{
				tableCellMapCopy = null;
				return(null);
			}
		}

		return(tableCellMapCopy);
	}

	/* ******************************************************** */
	//! Change Part's-Cell
	/*!
	@param	idParts
		Parts-ID<br>
		-1 == All parts
	@param	indexCellMap
		Cell-Map Index<br>
		-1 == Accorde to Animation-Data
	@param	indexCell
		Cell Index in Cell-Map<br>
		-1 == Accorde to Animation-Data
	@retval	Return-Value
		true == Success <br>
		false == Failure (Error)

	Change the Cell that are displayed in the parts.<br>
	<br>
	This function must be called after "Start" and "Awake" are executed.<br>
	<br>
	Caution: This function returns error when animation with "Precalculation > Fix Sprite" set to "true" at import.
	*/
	public bool CellChangeParts(int idParts, int indexCellMap, int indexCell)
	{
		if((null == DataEffect) || (null == ControlEffect.TableEmitter))
		{
			return(false);
		}

		if(0 > idParts)
		{	/* All Parts */
			int countEmitter = ControlEffect.TableEmitter.Length;
			for(int i=0; i<countEmitter; i++)
			{
				CellChangePartsMain(i, indexCellMap, indexCell);
			}

			return(true);
		}

		if(DataEffect.TableParts.Length <= idParts)
		{
			return(false);
		}

		return(CellChangePartsMain(DataEffect.TableParts[idParts].IndexEmitter, indexCellMap, indexCell));
	}
	private bool CellChangePartsMain(int indexEmitter, int indexCellMap, int indexCell)
	{
		if(0 > indexEmitter)
		{
			return(false);
		}

		ControlEffect.TableEmitter[indexEmitter].DataCellApply.IndexCellMap = indexCellMap;
		ControlEffect.TableEmitter[indexEmitter].DataCellApply.IndexCell = indexCell;
		ControlEffect.TableEmitter[indexEmitter].Status |= Library_SpriteStudio6.Control.Effect.Emitter.FlagBitStatus.CHANGE_CELL_UNREFLECTED;

		return(true);
	}
	#endregion Functions

	/* ----------------------------------------------- Classes, Structs & Interfaces */
	#region Classes, Structs & Interfaces
	public static partial class Cell
	{
		/* ----------------------------------------------- Functions */
		#region Functions
		/* ********************************************************* */
		//! Shallow-Copy CellMap-table
		/*!
		@param	tableCellMap
			original CellMap-table
		@retval	Return-Value
			New CellMap-table

		Creates shallow-copy of specified CellMap-table.<br>
		In newly created CellMap-table, table and each CellMap are another instances, and each CellMap are shallow-copy.<br>
		See also the notes on "MapCopyShallow".<br>
		*/
		public static Library_SpriteStudio6.Data.CellMap[] TableCopyMapShallow(Library_SpriteStudio6.Data.CellMap[] tableCellMap)
		{
			return(Library_SpriteStudio6.Utility.Cell.TableCopyMapShallow(tableCellMap));
		}

		/* ********************************************************* */
		//! Deep-Copy CellMap-table
		/*!
		@param	tableCellMap
			original CellMap-table
		@retval	Return-Value
			New CellMap-table

		Creates deep-copy of specified CellMap-table.<br>
		*/
		public static Library_SpriteStudio6.Data.CellMap[] TableCopyMapDeep(Library_SpriteStudio6.Data.CellMap[] tableCellMap)
		{
			return(Library_SpriteStudio6.Utility.Cell.TableCopyMapDeep(tableCellMap));
		}

		/* ********************************************************* */
		//! Shallow-Copy CellMap
		/*!
		@param	cellMap
			original CellMap
		@retval	Return-Value
			New CellMap

		Creates shallow-copy of specified CellMap.<br>
		<br>
		Created CellMap has following specifications.<br>
		<br>
		- CellMap's array is different instance<br>
		<br>
		- member "Name" and "TableCell" refer to the same instance in "cellMap"<br>
		*/
		public static Library_SpriteStudio6.Data.CellMap MapCopyShallow(Library_SpriteStudio6.Data.CellMap cellMap)
		{
			return(Library_SpriteStudio6.Utility.Cell.MapCopyShallow(cellMap));
		}

		/* ********************************************************* */
		//! Shallow-Copy CellMap
		/*!
		@param	cellMap
			original CellMap
		@retval	Return-Value
			New CellMap

		Creates deep-copy of specified CellMap.<br>
		*/
		public static Library_SpriteStudio6.Data.CellMap MapCopyDeep(Library_SpriteStudio6.Data.CellMap cellMap)
		{
			return(Library_SpriteStudio6.Utility.Cell.MapCopyDeep(cellMap));
		}

		/* ********************************************************* */
		//! Shallow-Copy Cell
		/*!
		@param	cellOutput
			destination cell
		@param	cell
			original Cell
		@retval	Return-Value
			New Cell

		Creates shallow-copy of specified cell.<br>
		<br>
		Created CellMap has following specifications.<br>
		<br>
		- member "Name" and contents of "Mesh" refer to the same instance in "cell"<br>
		*/
		public static bool CopyShallow(ref Library_SpriteStudio6.Data.CellMap.Cell cellOutput, ref Library_SpriteStudio6.Data.CellMap.Cell cell)
		{
			return(Library_SpriteStudio6.Utility.Cell.CopyShallow(ref cellOutput, ref cell));
		}

		/* ********************************************************* */
		//! Deep-Copy Cell
		/*!
		@param	cellOutput
			destination cell
		@param	cell
			original Cell
		@retval	Return-Value
			New Cell

		Creates deep-copy of specified cell.<br>
		*/
		public static bool CopyDeep(ref Library_SpriteStudio6.Data.CellMap.Cell cellOutput, ref Library_SpriteStudio6.Data.CellMap.Cell cell)
		{
			return(Library_SpriteStudio6.Utility.Cell.CopyDeep(ref cellOutput, ref cell));
		}

		/* ********************************************************* */
		//! Set Shallow-Copy Cell
		/*!
		@param	cellMapOutput
			destination CellMap
		@param	indexCellOutput
			destination index of cell
		@param	cell
			original Cell
		@retval	Return-Value
			true == Success<br>
			false == Failure (Error)

		Set a shallow-copy of "cell" in cell of "indexCellOutput" in CellMap.<br>
		*/
		public static bool CopyShallow(	Library_SpriteStudio6.Data.CellMap cellMapOutput,
										int indexCellOutput,
										ref Library_SpriteStudio6.Data.CellMap.Cell cell
									)
		{
			return(Library_SpriteStudio6.Utility.Cell.CopyShallow(cellMapOutput, indexCellOutput, ref cell));
		}

		/* ********************************************************* */
		//! Set Deep-Copy Cell
		/*!
		@param	cellMapOutput
			destination CellMap
		@param	indexCellOutput
			destination index of cell
		@param	cell
			source Cell
		@retval	Return-Value
			true == Success<br>
			false == Failure (Error)

		Set a deep-copy of "cell" in cell of "indexCellOutput" in CellMap.<br>
		*/
		public static bool CopyDeep(	Library_SpriteStudio6.Data.CellMap cellMapOutput,
										int indexCellOutput,
										ref Library_SpriteStudio6.Data.CellMap.Cell cell
									)
		{
			return(Library_SpriteStudio6.Utility.Cell.CopyDeep(cellMapOutput, indexCellOutput, ref cell));
		}

		/* ********************************************************* */
		//! Shallow-Copy Cells
		/*!
		@param	cellMapOutput
			destination CellMap
		@param	indexCellOutput
			destination index of cell
		@param	cellMap
			source CellMap
		@param	indexCell
			source index of cell
		@param	countCell
			Number of Cells to be copied
		@retval	Return-Value
			true == Success<br>
			false == Failure (Error)

		Shallow-copy multiple Cells from source CellMap to destination CellMap.<br>
		*/
		public static bool CopyShallow(	Library_SpriteStudio6.Data.CellMap cellMapOutput, 
										int indexCellOutput,
										Library_SpriteStudio6.Data.CellMap cellMap,
										int indexCell,
										int countCell
									)
		{
			return(Library_SpriteStudio6.Utility.Cell.CopyShallow(cellMapOutput, indexCellOutput, cellMap, indexCell, countCell));
		}

		/* ********************************************************* */
		//! Deep-Copy Cells
		/*!
		@param	cellMapOutput
			destination CellMap
		@param	indexCellOutput
			destination index of cell
		@param	cellMap
			source CellMap
		@param	indexCell
			source index of cell
		@param	countCell
			Number of Cells to be copied
		@retval	Return-Value
			true == Success<br>
			false == Failure (Error)

		Deep-copy multiple Cells from source CellMap to destination CellMap.<br>
		*/
		public static bool CopyDeep(	Library_SpriteStudio6.Data.CellMap cellMapOutput, 
										int indexCellOutput,
										Library_SpriteStudio6.Data.CellMap cellMap,
										int indexCell,
										int countCell
									)
		{
			return(Library_SpriteStudio6.Utility.Cell.CopyDeep(cellMapOutput, indexCellOutput, cellMap, indexCell, countCell));
		}
			#endregion Functions
	}
	#endregion Classes, Structs & Interfaces
}
