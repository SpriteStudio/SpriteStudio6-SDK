/**
	SpriteStudio6 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Script_SpriteStudio6_Root
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
												Library_SpriteStudio6.KindOperationBlend operationBlend,
												Library_SpriteStudio6.KindMasking masking
										)
	{
		if(TableCellMap.Length <= indexCellMap)
		{
			return(null);
		}

		int indexMaterial = Material.IndexGetTable(indexCellMap, operationBlend, masking);
		if(0 > indexMaterial)
		{
			return(null);
		}

		return(TableMaterial[indexMaterial]);
	}

	/* ********************************************************* */
	//! Get TableMaterial
	/*!
	@param	flagInUse
		true == TableMaterial of Currently in use<br>
		false == TableMaterial of original animation data
	@retval	Return-Value
		Material-Table

	Get TableMaterial.<br>
	*/
	public UnityEngine.Material[] TableGetMaterial(bool flagInUse=true)
	{
		if(true == flagInUse)
		{
			return(TableMaterial);
		}

		if(null == DataAnimation)
		{
			return(null);
		}
		return(DataAnimation.TableMaterial);
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

	Replace TableMaterial that is used in the Animation.<br>
	Never overwrite "Script_SpriteStudio6_Root.TableMaterial" directly.<br>
	<br>
	"tableMaterial"'s length must be more than length of the initial data at least.<br>
	More than value of "Script_SpriteStudio6_Root.Material.CountGetTable"function is needed. (Set number of using CellMaps to "countCellMap")<br>
	And, get TableMaterial's index with using "Script_SpriteStudio6_Root.Material.IndexGetTable"function (and etc.)  when create TableMaterial.<br>
	<br>
	To change TableMaterial of "Instance" or "Effect" in animation, use "TableMaterialChangeInstance" or "TableMaterialChangeEffect".<br>
	*/
	public bool TableSetMaterial(UnityEngine.Material[] tableMaterial)
	{
		if(null == tableMaterial)
		{	/* Revert */
			if(null == DataAnimation)
			{
				return(false);
			}

			tableMaterial = DataAnimation.TableMaterial;
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
	//! Get TableMaterial's length
	/*!
	@param	flagInUse
		true == TableMaterial's length of Currently in use<br>
		false == TableMaterial's length of original animation data
	@retval	Return-Value
		TableMaterial's length

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
	//! Shallow-Copy TableMaterial
	/*!
	@param	flagInUse
		true == Currently in use TableMaterial<br>
		false == TableMaterial in original animation data
	@retval	Return-Value
		New TableMaterial

	Shallow-copy TableMaterial.<br>
	In the newly created table, new table(array) is another instance,
	 but the materials are the same as materials referred to in the original table.<br>
	*/
	public UnityEngine.Material[] TableCopyMaterialShallow(bool flagInUse=true)
	{
		return(Library_SpriteStudio6.Utility.Material.TableCopyShallow(TableGetMaterial(flagInUse)));
	}

	/* ********************************************************* */
	//! Deep-Copy TableMaterial
	/*!
	@param	flagInUse
		true == Currently in use TableMaterial<br>
		false == TableMaterial in original animation data
	@retval	Return-Value
		New TableMaterial

	Deep-copy TableMaterial.<br>
	The newly created table, new table(array) is another instance and materials are new instance too.<br>
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

	/* ******************************************************** */
	//! Change "Instance"'s TableMaterial
	/*!
	@param	idParts
		Instance-Part's ID<br>
		-1 == Scan all parts
	@param	tableMaterial
		New TableMaterial<br>
		null == Revert to initial data
	@param	flagInvolveChildInstance
		true == also set to child "Instance" of the specified "Instance"<br>
		false == Only specified "Instance"
	@retval	Return-Value
		true == Success<br>
		false == Failure (Error)

	Replace "Instance"'s TableMaterial that is used in the Animation.<br>
	"idParts" is ID of part controlling "Instance".<br>
	<br>
	The other notes are the same as "TableSetMaterial".<br>
	*/
	public bool TableSetMaterialInstance(int idParts, UnityEngine.Material[] tableMaterial, bool flagInvolveChildInstance=true)
	{
		if(null == tableMaterial)
		{	/* Revert */
			if(null == DataAnimation)
			{
				return(false);
			}

			tableMaterial = DataAnimation.TableMaterial;
			if(null == tableMaterial)
			{
				return(false);
			}
		}

		if(0 > idParts)
		{
			if(null == TableControlParts)
			{
				return(false);
			}
			int countParts = TableControlParts.Length;

			bool flagSuccess = true;
			for(int i=0; i<countParts; i++)
			{
				flagSuccess &= TablSetMaterialInstanceMain(idParts, tableMaterial, flagInvolveChildInstance);
			}

			return(flagSuccess);
		}

		return(TablSetMaterialInstanceMain(idParts, tableMaterial, flagInvolveChildInstance));
	}
	private bool TablSetMaterialInstanceMain(int idParts, UnityEngine.Material[] tableMaterial, bool flagInvolveChildInstance=true)
	{
		Script_SpriteStudio6_Root instanceRootInstance = null;
		bool flagSuccess = true;

		if((null == TableControlParts) || (0 > idParts) || (TableControlParts.Length <= idParts))
		{
			return(false);
		}

		if(Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE == DataAnimation.TableParts[idParts].Feature)
		{
			instanceRootInstance = TableControlParts[idParts].InstanceRootUnderControl;
			if(null == instanceRootInstance)
			{
				return(true);
			}

			flagSuccess &= instanceRootInstance.TableSetMaterial(tableMaterial);
			if(true == flagInvolveChildInstance)
			{
				flagSuccess &= instanceRootInstance.TableSetMaterialInstance(-1, tableMaterial, true);
			}
		}

		return(true);
	}

	/* ******************************************************** */
	//! Change "Effect"'s TableMaterial
	/*!
	@param	idParts
		Effect-Part's ID<br>
		-1 == Scan all parts
	@param	tableMaterial
		New TableMaterial<br>
		null == Revert to initial data
	@retval	Return-Value
		true == Success<br>
		false == Failure (Error)

	Replace "Effect"'s TableMaterial that is used in the Animation.<br>
	"idParts" is ID of part controlling "Effect".<br>
	<br>
	The other notes are the same as "TableMaterialChange".<br>
	*/
	public bool TableSetMaterialEffect(int idParts, UnityEngine.Material[] tableMaterial)
	{
		if(null == tableMaterial)
		{	/* Revert */
			if(null == DataAnimation)
			{
				return(false);
			}

			tableMaterial = DataAnimation.TableMaterial;
			if(null == tableMaterial)
			{
				return(false);
			}
		}

		if(0 > idParts)
		{
			if(null == TableControlParts)
			{
				return(false);
			}
			int countParts = TableControlParts.Length;

			bool flagSuccess = true;
			for(int i=0; i<countParts; i++)
			{
				flagSuccess &= TableSetMaterialEffectMain(idParts, tableMaterial);
			}

			return(flagSuccess);
		}

		return(TableSetMaterialEffectMain(idParts, tableMaterial));
	}
	private bool TableSetMaterialEffectMain(int idParts, UnityEngine.Material[] tableMaterial)
	{
		Script_SpriteStudio6_RootEffect instanceRootEffect = null;
		bool flagSuccess = true;

		if((null == TableControlParts) || (0 > idParts) || (TableControlParts.Length <= idParts))
		{
			return(false);
		}

		if(Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE == DataAnimation.TableParts[idParts].Feature)
		{
			instanceRootEffect = TableControlParts[idParts].InstanceRootEffectUnderControl;
			if(null == instanceRootEffect)
			{
				return(true);
			}

			flagSuccess &= instanceRootEffect.TableSetMaterial(tableMaterial);
		}

		return(true);
	}
	#endregion Functions

	/* ----------------------------------------------- Classes, Structs & Interfaces */
	#region Classes, Structs & Interfaces
	public static partial class Material
	{
		/* ----------------------------------------------- Functions */
		#region Functions
		/* ********************************************************* */
		//! Get Material-Table's length
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
				return(-1);
			}

			return((countCellMap * ((int)Library_SpriteStudio6.KindMasking.TERMINATOR * (int)Library_SpriteStudio6.KindOperationBlend.TERMINATOR_TABLEMATERIAL)));
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
											Library_SpriteStudio6.KindOperationBlend operationBlend,
											Library_SpriteStudio6.KindMasking masking
										)
		{
			if(	(0 > indexCellMap)
				|| (Library_SpriteStudio6.KindOperationBlend.INITIATOR > operationBlend) || (Library_SpriteStudio6.KindOperationBlend.TERMINATOR <= operationBlend)
				|| (Library_SpriteStudio6.KindMasking.THROUGH > masking) || (Library_SpriteStudio6.KindMasking.TERMINATOR <= masking)
				)
			{
				return(-1);
			}

			return((((indexCellMap * (int)Library_SpriteStudio6.KindMasking.TERMINATOR) + (int)masking) * (int)Library_SpriteStudio6.KindOperationBlend.TERMINATOR_TABLEMATERIAL) + ((int)operationBlend - (int)Library_SpriteStudio6.KindOperationBlend.INITIATOR));
		}

		/* ********************************************************* */
		//! Get Texture-count in Material-Table
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
			return(tableMaterial.Length / ((int)Library_SpriteStudio6.KindMasking.TERMINATOR * (int)Library_SpriteStudio6.KindOperationBlend.TERMINATOR_TABLEMATERIAL));
		}

		/* ********************************************************* */
		//! Change Texture in Material-Table
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
			int indexTop = IndexGetTable(indexCellMap, Library_SpriteStudio6.KindOperationBlend.INITIATOR, Library_SpriteStudio6.KindMasking.THROUGH);
			if(0 > indexTop)
			{
				return(false);
			}

			UnityEngine.Material material;
			int index;
			for(int i=(int)Library_SpriteStudio6.KindMasking.THROUGH; i<(int)Library_SpriteStudio6.KindMasking.TERMINATOR; i++)
			{
				for(int j=(int)Library_SpriteStudio6.KindOperationBlend.INITIATOR; j<(int)Library_SpriteStudio6.KindOperationBlend.TERMINATOR; j++)
				{
					index = IndexGetTable(0, (Library_SpriteStudio6.KindOperationBlend)j, (Library_SpriteStudio6.KindMasking)i);
					index += indexTop;

					material = tableMaterial[index];
					if(true == flagMaterialNew)
					{	/* Create Material */
#if false
						/* MEMO: Before Ver.1.0.26 */
						if(null == material)
						{
							material = new UnityEngine.Material(Library_SpriteStudio6.Data.Shader.ShaderGetAnimation(	(Library_SpriteStudio6.KindOperationBlend)j,
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
							material = Library_SpriteStudio6.Data.Shader.MaterialCreateAnimation(	(Library_SpriteStudio6.KindOperationBlend)j,
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
							material = new UnityEngine.Material(Library_SpriteStudio6.Data.Shader.ShaderGetAnimation(	(Library_SpriteStudio6.KindOperationBlend)j,
																														(Library_SpriteStudio6.KindMasking)i
																													)
															);
						}
#else
						/* MEMO: After Ver.1.0.26 */
						if(null == material)
						{
							material = Library_SpriteStudio6.Data.Shader.MaterialCreateAnimation(	(Library_SpriteStudio6.KindOperationBlend)j,
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
