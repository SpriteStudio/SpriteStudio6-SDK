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
	/* ******************************************************** */
	//! Get number of parts in animation
	/*!
	@param	
		(none)
	@retval	Return-Value
		Number of parts in animation
		-1 == Error / "Name" is not-found.

	Get number of parts in animation.<br>
	The range of Parts-ID is "0" to "ReturnValue-1".<br>
	*/
	public int CountGetParts()
	{
		if(null == DataAnimation)
		{
			return(-1);
		}

		return(DataAnimation.CountGetParts());
	}

	/* ******************************************************** */
	//! Get part's index(Parts-ID) from the part's-name
	/*!
	@param	Name
		Part's name
	@retval	Return-Value
		Parts-ID
		-1 == Error / "Name" is not-found.

	Get the part's-Index(Parts-ID) from the name.<br>
	The Index is the serial-number (0 origins) in the Animation-data.
	*/
	public int IDGetParts(string name)
	{
		if(null == DataAnimation)
		{
			return(-1);
		}

		return(DataAnimation.IndexGetParts(name));
	}

	/* ******************************************************** */
	//! Force-Hide Set
	/*!
	@param	idParts
		Parts-ID<br>
		0 == Hide the entire animation<br>
		-1 == Set Hide to all parts
	@param	flagSwitch
		true == Set Hide (Hide, force)<br>
		false == Hide Reset (Show. State of animation is followed.)
	@param	flagInvolveChildren
		true == Children are set same state.<br>
		false == only oneself.<br>
		default: false
	@retval	Return-Value
		true == Success<br>
		false == Failure (Error)

	State of "Hide" is set to parts, ignore with state of animation.<br>
	This setting is ignored when set to parts that does not have display capability such as "NULL"-parts.<br>
	Moreover, when set to "Mask"-parts, affect of mask can be erased.<br>
	This setting also affects the "Instance"-parts and "Effect"-parts, but not set to each (subordinate) animation objects.<br>
	<br>
	If set 0 or -1 to "idParts", hide entire animation.<br>
	However, behaviors differ clearly between 0 and -1.<br>
	Do not confuse both.<br>
	<br>
	idParts == 0:<br>
	Set hide state to whole animation.<br>
	(It is same behavior as checking or unchecking "Hide" on inspector)<br>
	Recommend that use this when normally set hide state of the whole animation.<br>
	This setting is separately from setting to each parts, and has priority.<br>
	<br>
	idParts == -1:<br>
	Set hide state to each all parts.<br>
	*/
	public bool HideSet(int idParts, bool flagSwitch, bool flagInvolveChildren=false)
	{
		if((null == DataAnimation) || (null == TableControlParts))
		{
			return(false);
		}

		int countParts = TableControlParts.Length;
		if(0 > idParts)
		{	/* All parts */
			for(int i=1; i<countParts; i++)
			{
				HideSetMain(i, flagSwitch, false);
			}
			return(true);
		}

		if(0 == idParts)
		{	/* "Root"-Parts */
			FlagHideForce = flagSwitch;
			return(true);
		}

		if(countParts <= idParts)
		{	/* Invalid ID */
			return(false);
		}

		HideSetMain(idParts, flagSwitch, flagInvolveChildren);

		return(true);
	}
	private void HideSetMain(int idParts, bool flagSwitch, bool flagInvolveChildren=false)
	{
		if(true == flagSwitch)
		{
			TableControlParts[idParts].Status |= Library_SpriteStudio6.Control.Animation.Parts.FlagBitStatus.HIDE_FORCE;
		}
		else
		{
			TableControlParts[idParts].Status &= ~Library_SpriteStudio6.Control.Animation.Parts.FlagBitStatus.HIDE_FORCE;
		}

		if(true == flagInvolveChildren)
		{
			int[] tableIDPartsChild = DataAnimation.TableParts[idParts].TableIDChild;
			int countPartsChild = tableIDPartsChild.Length;
			for(int i=0; i<countPartsChild; i++)
			{
				HideSetMain(tableIDPartsChild[i], flagSwitch, true);
			}
		}
	}

	/* ******************************************************** */
	//! Get part's ColorLabel-form
	/*!
	@param	idParts
		Parts-ID
	@retval	Return-Value
		ColorLabel's form<br>
		-1 == Error

	Get part's ColorLabel-form.<br>
	When "ColorLabel" is set to "Custom Color" on "SpriteStudio6",
	 this function returns "Library_SpriteStudio6.Data.Parts.Animation.ColorLabel.KindForm.CUSTOM".(Irrespective of the actual color)<br>
	*/
	public Library_SpriteStudio6.Data.Parts.Animation.ColorLabel.KindForm FormGetColorLabel(int idParts)
	{
		if(null == DataAnimation)
		{
			return((Library_SpriteStudio6.Data.Parts.Animation.ColorLabel.KindForm)(-1));
		}

		if((0 > idParts) || (DataAnimation.CountGetParts() <= idParts))
		{
			return((Library_SpriteStudio6.Data.Parts.Animation.ColorLabel.KindForm)(-1));
		}

		return(DataAnimation.TableParts[idParts].LabelColor.Form);
	}

	/* ******************************************************** */
	//! Get part's ColorLabel-color
	/*!
	@param	idParts
		Parts-ID
	@retval	Return-Value
		ColorLabel's actual color<br>
		"A/R/G/B all 0" == Error

	Regardless (Color-Label's) form, this function returns actual color of the color label.<br>
	Use to get color when form is "Custom Color".<br>
	*/
	public Color ColorGetColorLabel(int idParts)
	{
		if(null == DataAnimation)
		{
			return(Library_SpriteStudio6.Data.Parts.Animation.ColorLabel.TableDefault[(int)Library_SpriteStudio6.Data.Parts.Animation.ColorLabel.KindForm.NON].Color);
		}

		if((0 > idParts) || (DataAnimation.CountGetParts() <= idParts))
		{
			return(Library_SpriteStudio6.Data.Parts.Animation.ColorLabel.TableDefault[(int)Library_SpriteStudio6.Data.Parts.Animation.ColorLabel.KindForm.NON].Color);
		}

		return(DataAnimation.TableParts[idParts].LabelColor.Color);
	}

	/* ******************************************************** */
	//! Set coefficient for parts' flipping
	/*!
	@param	idParts
		Parts-ID
	@param	scaleLocalX
		Coefficient to attribute "Holizontal Flip"<br>
		true == Flip<br>
		false == Not Flip
	@param	scaleLocalY
		Coefficient to attribute "Vertical Flip"<br>
		true == Flip<br>
		false == Not Flip
	@param	flagFlipImageX
		Coefficient to attribute "Image H Flip"<br>
		true == Flip<br>
		false == Not Flip
	@param	flagFlipImageY
		Coefficient to attribute "Image V Flip"<br>
		true == Flip<br>
		false == Not Flip
	@param	flagInvolveChild
		true == Also set to child-parts<br>
		false == Set to Only specified part
	@retval	Return-Value
		true == Success<br>
		false == Failure (Error)

	Set coefficient for parts' flipping.<br>
	Values affects additional to animation data. (not "Overwriting")<br>
	Each corresponds to the following SpriteStudio6's attribute.<br>
	- flagFlipX : Holizontal Flip (Same as "Local Scale X *= -1.0f")<br>
	- flagFlipY : Vertical Flip (Same as "Local Scale Y *= -1.0f")<br>
	- flagFlipImageX : Image H Flip<br>
	- flagFlipImageY : Image V Flip<br>
	<br>
	This function can use to parts type "Normal" or "Mask".<br>
	Otherwise error(false) is returned.<br>
	When "flagInvolveChild" is true, same values are set to settable parts in all child-parts.<br>
	And always return success(true).<br>
	*/
	public bool FlipSetParts(int idParts, bool flagFlipX, bool flagFlipY, bool flagFlipImageX, bool flagFlipImageY, bool flagInvolveChild=false)
	{
		int countParts = CountGetParts();
		if(0 >= countParts)
		{	/* Has no parts */
			return(false);
		}
		if((0 > idParts) || (countParts <= idParts))
		{	/* Invalid ID */
			return(false);
		}

		Library_SpriteStudio6.Control.Animation.Parts.BufferParameterSprite.FlagBitStatus flags = Library_SpriteStudio6.Control.Animation.Parts.BufferParameterSprite.FlagBitStatus.CLEAR;
		if(true == flagFlipX)
		{
			flags |= Library_SpriteStudio6.Control.Animation.Parts.BufferParameterSprite.FlagBitStatus.FLIP_COEFFICIENT_X;
		}
		if(true == flagFlipY)
		{
			flags |= Library_SpriteStudio6.Control.Animation.Parts.BufferParameterSprite.FlagBitStatus.FLIP_COEFFICIENT_Y;
		}
		if(true == flagFlipImageX)
		{
			flags |= Library_SpriteStudio6.Control.Animation.Parts.BufferParameterSprite.FlagBitStatus.FLIP_COEFFICIENT_TEXTURE_X;
		}
		if(true == flagFlipImageY)
		{
			flags |= Library_SpriteStudio6.Control.Animation.Parts.BufferParameterSprite.FlagBitStatus.FLIP_COEFFICIENT_TEXTURE_Y;
		}

		return(CoefficientSetScalePartsMain(idParts, flags, flagInvolveChild));
	}
	private bool CoefficientSetScalePartsMain(int idParts, Library_SpriteStudio6.Control.Animation.Parts.BufferParameterSprite.FlagBitStatus flags, bool flagInvolveChild)
	{
		bool flagSettable = false;
		switch(DataAnimation.TableParts[idParts].Feature)
		{
			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.ROOT:
			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NULL:
//				flagSettable = false;
				break;

			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL:
				flagSettable = true;
				break;

			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE:
			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT:
//				flagSettable = false;
				break;

			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK:
				flagSettable = true;
				break;

			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.JOINT:
			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONE:
			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MOVENODE:
			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.CONSTRAINT:
			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONEPOINT:
			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH:
			default:
//				flagSettable = false;
				break;
		}

		if(true == flagSettable)
		{
			TableControlParts[idParts].ParameterSprite.Status &= ~(	Library_SpriteStudio6.Control.Animation.Parts.BufferParameterSprite.FlagBitStatus.FLIP_COEFFICIENT_X
																	| Library_SpriteStudio6.Control.Animation.Parts.BufferParameterSprite.FlagBitStatus.FLIP_COEFFICIENT_Y
																	| Library_SpriteStudio6.Control.Animation.Parts.BufferParameterSprite.FlagBitStatus.FLIP_COEFFICIENT_TEXTURE_X
																	| Library_SpriteStudio6.Control.Animation.Parts.BufferParameterSprite.FlagBitStatus.FLIP_COEFFICIENT_TEXTURE_Y
																);
//			TableControlParts[idParts].ParameterSprite.Status |= flags | Library_SpriteStudio6.Control.Animation.Parts.BufferParameterSprite.FlagBitStatus.UPDATE_FLIP_COEFFICIENT;
			TableControlParts[idParts].ParameterSprite.Status |= flags;
		}

		if(true == flagInvolveChild)
		{
			int[] tableIDChild = DataAnimation.TableParts[idParts].TableIDChild;
			int countChild = tableIDChild.Length;
			for(int i=0; i<countChild; i++)
			{
				/* MEMO: Ignore errors. Because child-parts are not always settable. */
				CoefficientSetScalePartsMain(tableIDChild[i], flags, true);
			}
			return(true);	/* Always true */
		}

		return(flagSettable);
	}

	/* ******************************************************** */
	//! Get coefficient for parts' flipping.<br>
	/*!
	@param	flagFlipX
		(Output) Coefficient to attribute "Holizontal Flip"
	@param	flagFlipY
		(Output) Coefficient to attribute "Vertical Flip"
	@param	flagFlipImageX
		(Output) Coefficient to attribute "Image H Flip"
	@param	flagFlipImageY
		(Output) Coefficient to attribute "Image V Flip"
	@param	idParts
		Parts-ID
	@retval	Return-Value
		true == Success<br>
		false == Failure (Error)

	Get coefficient for parts' flipping.<br>
	*/
	public bool FlipGetParts(out bool flagFlipX, out bool flagFlipY, out bool flagFlipImageX, out bool flagFlipImageY, int idParts)
	{
		int countParts = CountGetParts();
		if(0 >= countParts)
		{	/* Has no parts */
			goto CoefficientGetScaleParts_ErrorEnd;
		}
		if((0 > idParts) || (countParts <= idParts))
		{	/* Invalid ID */
			goto CoefficientGetScaleParts_ErrorEnd;
		}

		switch(DataAnimation.TableParts[idParts].Feature)
		{
			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.ROOT:
			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NULL:
				goto CoefficientGetScaleParts_ErrorEnd;

			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.NORMAL:
				break;

			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE:
			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT:
				goto CoefficientGetScaleParts_ErrorEnd;

			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MASK:
				break;

			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.JOINT:
			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONE:
			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MOVENODE:
			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.CONSTRAINT:
			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.BONEPOINT:
			case Library_SpriteStudio6.Data.Parts.Animation.KindFeature.MESH:
			default:
				goto CoefficientGetScaleParts_ErrorEnd;
		}

		Library_SpriteStudio6.Control.Animation.Parts.BufferParameterSprite.FlagBitStatus flags = TableControlParts[idParts].ParameterSprite.Status;
		flagFlipX = (0 != (flags & Library_SpriteStudio6.Control.Animation.Parts.BufferParameterSprite.FlagBitStatus.FLIP_COEFFICIENT_X));	/* ? true : false */
		flagFlipY = (0 != (flags & Library_SpriteStudio6.Control.Animation.Parts.BufferParameterSprite.FlagBitStatus.FLIP_COEFFICIENT_Y));	/* ? true : false */
		flagFlipImageX = (0 != (flags & Library_SpriteStudio6.Control.Animation.Parts.BufferParameterSprite.FlagBitStatus.FLIP_COEFFICIENT_TEXTURE_X));	/* ? true : false */
		flagFlipImageY = (0 != (flags & Library_SpriteStudio6.Control.Animation.Parts.BufferParameterSprite.FlagBitStatus.FLIP_COEFFICIENT_TEXTURE_Y));	/* ? true : false */

		return(true);

	CoefficientGetScaleParts_ErrorEnd:;
		flagFlipX = false;
		flagFlipY = false;
		flagFlipImageX = false;
		flagFlipImageY = false;

		return(false);
	}
	#endregion Functions

	/* ----------------------------------------------- Classes, Structs & Interfaces */
	#region Classes, Structs & Interfaces
	public static partial class Parts
	{
		/* ----------------------------------------------- Functions */
		#region Functions
		/* ******************************************************** */
		//! Get Root-Parts
		/*!
		@param	gameObject
			GameObject of starting search
		@param	flagApplySelf
			true == Include "gameObject" as check target<br>
			false == exclude "gameObject"<br>
			default: true
		@retval	Return-Value
			Instance of "Script_SpriteStudio6_Root"<br>
			null == Not-Found / Failure	

		Get component "Script_SpriteStudio6_Root" by examining "gameObject" and direct-children.<br>
		<br>
		This function returns "Script_SpriteStudio6_Root" first found.<br>
		However, it is not necessarily in shallowest GameObject-hierarchy.<br>
		(Although guarantee up to direct-children, can not guarantee if farther than direct-children)<br>
		*/
		public static Script_SpriteStudio6_Root RootGet(GameObject gameObject, bool flagApplySelf=true)
		{
			Script_SpriteStudio6_Root scriptRoot = null;

			/* Check Origin */
			if(true == flagApplySelf)
			{
				scriptRoot = RootGetMain(gameObject);
				if(null != scriptRoot)
				{
					return(scriptRoot);
				}
			}

			/* Check Direct-Children */
			/* MEMO: Processing is wastefull, but check direct-children first so that make to find in closely-relation as much as possible. */
			int countChild = gameObject.transform.childCount;
			Transform transformChild = null;

			for(int i=0; i<countChild; i++)
			{
				transformChild = gameObject.transform.GetChild(i);
				scriptRoot = RootGetMain(transformChild.gameObject);
				if(null != scriptRoot)
				{
					return(scriptRoot);
				}
			}

			/* Check Children */
			for(int i=0; i<countChild; i++)
			{
				transformChild = gameObject.transform.GetChild(i);
				scriptRoot = RootGet(transformChild.gameObject, false);
				if(null != scriptRoot)
				{	/* Has Root-Parts */
					return(scriptRoot);
				}
			}

			return(null);
		}
		private static Script_SpriteStudio6_Root RootGetMain(GameObject gameObject)
		{
			Script_SpriteStudio6_Root scriptRoot = null;
			scriptRoot = gameObject.GetComponent<Script_SpriteStudio6_Root>();
			if(null != scriptRoot)
			{	/* Has Root-Parts */
				if(null == scriptRoot.InstanceRootParent)
				{	/* has no Parent */
					return(scriptRoot);
				}
			}

			return(null);
		}
		#endregion Functions
	}
	#endregion Classes, Structs & Interfaces
}
