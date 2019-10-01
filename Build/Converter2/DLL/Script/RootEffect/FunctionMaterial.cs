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
	//! Get Material
	/*!
	@param	indexCellMap
		Serial-number of using CellMap
	@param	operationBlend
		Blend Operation for the target
	@retval	Return-Value
		Material

	Get specified material in TableMaterial.
	*/
	public UnityEngine.Material MaterialGet(	int indexCellMap,
												Library_SpriteStudio6.KindOperationBlendEffect operationBlend,
												Library_SpriteStudio6.KindMasking masking
										)
	{
		if(TableCellMap.Length <= indexCellMap)
		{
			return (null);
		}

		int indexMaterial = Material.IndexGetTable(indexCellMap, operationBlend, masking);
		if(0 > indexMaterial)
		{
			return (null);
		}

		return(TableMaterial[indexMaterial]);
	}

	/* ********************************************************* */
	//! Get Material-Table
	/*!
	@param	flagInUse
		true == TableMaterial's length of Currently in use<br>
		false == TableMaterial's length of original animation data
	@retval	Return-Value
		Material-Table

	Get TableMaterial.
	*/
	public UnityEngine.Material[] TableGetMaterial(bool flagInUse=true)
	{
		if(true == flagInUse)
		{
			return(TableMaterial);
		}

		if(null == DataEffect)
		{
			return(null);
		}
		return(DataEffect.TableMaterial);
	}

	/* ******************************************************** */
	//! Change TableMaterial
	/*!
	@param	tableMaterial
		New TableMaterial<br>
		null == Revert to initial data
	@retval	Return-Value
		true == Success<br>
		false == Failure (Error)

	Replace TableMaterial that is used in the Effect.<br>
	Never overwrite "Script_SpriteStudio6_RootEffect.TableMaterial" directly.<br>
	<br>
	"tableMaterial"'s length must be more than length of the initial data at least.<br>
	More than value of "Script_SpriteStudio6_RootEffect.Material.CountGetTable"function is needed. (Set number of using CellMaps to "countCellMap")<br>
	And, get TableMaterial's index with using "Script_SpriteStudio6_RootEffect.Material.IndexGetTable"function (and etc.)  when create TableMaterial.<br>
	*/
	public bool TableSetMaterial(UnityEngine.Material[] tableMaterial)
	{
		if(null == tableMaterial)
		{	/* Revert */
			if(null == DataEffect)
			{
				return(false);
			}

			tableMaterial = DataEffect.TableMaterial;
			if(null == tableMaterial)
			{
				return(false);
			}
		}

		TableMaterial = tableMaterial;
		Status |= FlagBitStatus.CHANGE_TABLEMATERIAL;
		return(true);
	}

	/* ********************************************************* */
	//! Get Material-Table length
	/*!
	@param	flagInUse
		true == TableMaterial's length of Currently in use<br>
		false == TableMaterial's length of original animation data
	@retval	Return-Value
		Material-Table length

	Get TableMaterial's length.
	*/
	public int CountGetTableMaterial(bool flagInUse=true)
	{
		UnityEngine.Material[]  tableMaterial = TableGetMaterial(flagInUse);
		if(null == tableMaterial)
		{
			return(-1);
		}

		return(tableMaterial.Length);
	}

	/* ********************************************************* */
	//! Shallow-Copy Material-Table
	/*!
	@param	flagInUse
		true == TableMaterial's length of Currently in use<br>
		false == TableMaterial's length of original animation data
	@retval	Return-Value
		Material-Table

	Shallow-copy Material-Table.<br>
	In the newly created table, table(array) is another instance,
	 but the materials are the same as materials referred to in the original table.<br>
	*/
	public UnityEngine.Material[] TableCopyMaterialShallow(bool flagInUse=true)
	{
		return(Library_SpriteStudio6.Utility.Material.TableCopyShallow(TableGetMaterial(flagInUse)));
	}

	/* ********************************************************* */
	//! Deep-Copy Material-Table
	/*!
	@param	flagInUse
		true == TableMaterial's length of Currently in use<br>
		false == TableMaterial's length of original animation data
	@retval	Return-Value
		Material-Table

	Deep-copy Material-Table.<br>
	The newly created table, table(array) is another instance and materials are new instance too.<br>
	*/
	public UnityEngine.Material[] TableCopyMaterialDeep(bool flagInUse=true)
	{
		return(Library_SpriteStudio6.Utility.Material.TableCopyDeep(TableGetMaterial(flagInUse)));
	}

	/* ********************************************************* */
	//! Get Texture-count in TableMaterial
	/*!
	@param	flagInUse
		true == Currently in use TableMaterial<br>
		false == TableMaterial in original animation data
	@retval	Return-Value
		Number of textures that can be stored in TableMaterial

	Get number of textures that can be stored in TableMaterial.
	*/
	public int CountGetTextureTableMaterial(bool flagInUse=true)
	{
		UnityEngine.Material[] tableMaterial = TableGetMaterial(flagInUse);
		if(null == tableMaterial)
		{
			return(-1);
		}
		return(Material.CountGetTexture(tableMaterial));
	}
	#endregion Functions

	/* ----------------------------------------------- Classes, Structs & Interfaces */
	#region Classes, Structs & Interfaces
	public static partial class Material
	{
		/* ----------------------------------------------- Functions */
		#region Functions
		/* ********************************************************* */
		//! Get Material-Table length
		/*!
		@param	countCellMap
			Number of CellMap-s
		@retval	Return-Value
			Material-Table length

		If give positive number to "countCellMap", returns length of materials needed to store.
		*/
		public static int CountGetTable(int countCellMap)
		{
			if(0 > countCellMap)
			{
				return (-1);
			}

			return(countCellMap * ((int)Library_SpriteStudio6.KindMasking.TERMINATOR * (int)Library_SpriteStudio6.KindOperationBlendEffect.TERMINATOR_TABLEMATERIAL));
		}

		/* ********************************************************* */
		//! Get Material-Table's index
		/*!
		@param	indexCellMap
			index of CellMap
		@param	operationBlend
			Kind of Blending
		@param	masking
			Kind of Masking
		@retval	Return-Value
			index of Material-Table

		Get material's index in Material-Table.<br>
		Caution that this function does not check upper-limit of "indexCellMap".
		*/
		public static int IndexGetTable(	int indexCellMap,
											Library_SpriteStudio6.KindOperationBlendEffect operationBlend,
											Library_SpriteStudio6.KindMasking masking
									)
		{
			if((0 > indexCellMap)
				|| (Library_SpriteStudio6.KindOperationBlendEffect.INITIATOR > operationBlend) || (Library_SpriteStudio6.KindOperationBlendEffect.TERMINATOR_TABLEMATERIAL <= operationBlend)
				|| (Library_SpriteStudio6.KindMasking.THROUGH > masking) || (Library_SpriteStudio6.KindMasking.TERMINATOR <= masking)
				)
			{
				return (-1);
			}

#if false
			return((((indexCellMap * (int)Library_SpriteStudio6.KindMasking.TERMINATOR) + (int)masking) * (int)Library_SpriteStudio6.KindOperationBlendEffect.TERMINATOR_TABLEMATERIAL) + ((int)operationBlend + (int)Library_SpriteStudio6.KindOperationBlendEffect.INITIATOR));
#else
			return((((indexCellMap * (int)Library_SpriteStudio6.KindMasking.TERMINATOR) + (int)masking) * (int)Library_SpriteStudio6.KindOperationBlendEffect.TERMINATOR_TABLEMATERIAL) + ((int)operationBlend - (int)Library_SpriteStudio6.KindOperationBlendEffect.INITIATOR));
#endif
		}

		/* ********************************************************* */
		//! Get Texture-count in Material-table
		/*!
		@param	tableMaterial
			Material-Table
		@retval	Return-Value
			Number of textures that can be stored in Material-Table

		Get number of textures that can be stored in Material-Table.
		*/
		public static int CountGetTexture(UnityEngine.Material[] tableMaterial)
		{
			if(null == tableMaterial)
			{
				return(-1);
			}
			return(tableMaterial.Length / ((int)Library_SpriteStudio6.KindMasking.TERMINATOR * (int)Library_SpriteStudio6.KindOperationBlendEffect.TERMINATOR_TABLEMATERIAL));
		}

		/* ********************************************************* */
		//! Change Texture in Material-table
		/*!
		@param	tableMaterial
			Material-Table
		@param	indexCellMap
			index of CellMap
		@param	texture
			Texture
		@param	flagMaterialNew
			Whether make a new material or overwrite<br>
			true == Create new material<br>
			false == Overwrite exist material
		@retval	Return-Value
			true == Success<br>
			false == Failure(Error)

		Change materials' texture corresponding to specified CellMap in Material-Table.<br>
		When "flagMaterialNew" is set true, new materials are created.<br>
		When false, materials are overwritten.
		*/
		public static bool TextureSet(UnityEngine.Material[] tableMaterial, int indexCellMap, Texture2D texture, bool flagMaterialNew)
		{
			int indexTop = IndexGetTable(indexCellMap, Library_SpriteStudio6.KindOperationBlendEffect.INITIATOR, Library_SpriteStudio6.KindMasking.THROUGH);
			if(0 > indexTop)
			{
				return(false);
			}

			UnityEngine.Material material;
			int index;
			for(int i=(int)Library_SpriteStudio6.KindMasking.THROUGH; i<(int)Library_SpriteStudio6.KindMasking.TERMINATOR; i++)
			{
				for(int j=(int)Library_SpriteStudio6.KindOperationBlendEffect.INITIATOR; j<(int)Library_SpriteStudio6.KindOperationBlendEffect.TERMINATOR; j++)
				{
					index = IndexGetTable(0, (Library_SpriteStudio6.KindOperationBlendEffect)j, (Library_SpriteStudio6.KindMasking)i);
					index += indexTop;

					material = tableMaterial[index];
					if(true == flagMaterialNew)
					{	/* Create Material */
#if false
						/* MEMO: Before Ver.1.0.26 */
						if(null == material)
						{
							material = new UnityEngine.Material(Library_SpriteStudio6.Data.Shader.ShaderGetEffect(	(Library_SpriteStudio6.KindOperationBlendEffect)j,
																													(Library_SpriteStudio6.KindMasking)i
																												)
															);
						}
						else
						{
							material = new UnityEngine.Material(material);
						}
#else
						/* MEMO: After Ver.1.0.26 */
						if(null == material)
						{
							material = Library_SpriteStudio6.Data.Shader.MaterialCreateEffect(	(Library_SpriteStudio6.KindOperationBlendEffect)j,
																								(Library_SpriteStudio6.KindMasking)i
																							);
						}
						else
						{
							UnityEngine.Material materialNew = new UnityEngine.Material(material);
							if(null != materialNew)
							{
								materialNew.CopyPropertiesFromMaterial(material);
							}
							material = materialNew;
						}
#endif

					}
					else
					{	/* Overwrite Material */
#if false
						/* MEMO: Before Ver.1.0.26 */
						if(null == material)
						{
							material = new UnityEngine.Material(Library_SpriteStudio6.Data.Shader.ShaderGetEffect(	(Library_SpriteStudio6.KindOperationBlendEffect)j,
																													(Library_SpriteStudio6.KindMasking)i
																												)
															);
						}
#else
						/* MEMO: After Ver.1.0.26 */
						if(null == material)
						{
							material = Library_SpriteStudio6.Data.Shader.MaterialCreateEffect(	(Library_SpriteStudio6.KindOperationBlendEffect)j,
																								(Library_SpriteStudio6.KindMasking)i
																							);
						}
#endif
					}
					material.mainTexture = texture;
					tableMaterial[index] = material;
				}
			}

			return(true);
		}
		#endregion Functions
	}
	#endregion Classes, Structs & Interfaces
}
