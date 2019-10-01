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
	/* ******************************************************** */
	//! Get "initialization completion (valid)" status of animation object
	/*!
	@param	
		(none)
	@retval	Return-Value
		true == Complete<br>
		false == Error / Not complete

	Get state of whether animation object has completed "Start" processing.<br>
	Return-value is false also when "Start" processing failed and ended.<br>
	<br>
	Do not use property "StatusIsValid" directly. (For internal processing, Specification will be changed without notice)<br>
	*/
	public bool StatustGetValid()
	{
		return(StatusIsValid);
	}

	/* ******************************************************** */
	//! Create "AdditionalColor" Parameter
	/*!
	@param
		(None)
	@retval	Return-Value
		"AdditionalColor" Setting Buffer

	Get the Parameter-Buffer of "AdditionalColor" for instances of this class.<br>
	AdditionalColor" perform additional color processing to instances of this class's derived class.
	 ("Script_SpriteStudio6_Root", "Script_SpriteStudio6_RootEffect")<br>
	Caution that the effect differs for each derived class.<br>
	<br>
	- Script_SpriteStudio 6_Root (Animation)<br>
	Overwrite "Parts Color" attribute state for all sprite data used in animation. (No effect to "Mask")<br>
	<br>
	- Script_SpriteStudio6_RootEffect (Effect)<br>
	Execute processing equivalent to animation's "Parts Color" attribute for all particles' vertex-color used in "Effect".<br>
	(The result color is "particles' vertex-color processed with AdditionalColor" * "pixel of particle")<br>
	<br>
	When continue using AdditionalColor, need not to call this function more than once.<br>
	<br>
	The detail of how to set, refer to the commentary of "Library_SpriteStudio6.Control.AdditionalColor".<br>
	*/
	public Library_SpriteStudio6.Control.AdditionalColor AdditionalColorCreate()
	{
		/* Create Parameter-instance */
		if(null == AdditionalColor)
		{
			AdditionalColor = new Library_SpriteStudio6.Control.AdditionalColor();
			if(null == AdditionalColor)
			{	/* Error */
				return(null);
			}
			AdditionalColor.BootUp();
		}

		return(AdditionalColor);
	}

	/* ******************************************************** */
	//! Release "AdditionalColor" Parameter
	/*!
	@param
		(None)
	@retval	Return-Value
		(None)

	Release the Parameter-Buffer of "AdditionalColor".<br>
	And stop processing AdditionalColor.<br>
	(Animation and "Effect"s return to state not using AdditionalColor)<br>
	<br>
	To use AdditionalColor again after using this function, use "AdditionalColorCreate" function and re-get parameter-buffer.<br>
	*/
	public void AdditionalColorRelease()
	{
		AdditionalColor = null;
	}
	#endregion Functions

	/* ******************************************************** */
	//! Get "AdditionalColor" Parameter
	/*!
	@param	
	@retval	Return-Value
		"AdditionalColor" Setting Buffer<br>
		null == No-set(Not created)

	Get (now-set) Parameter-Buffer of "AdditionalColor" for instances of this class.<br>
	*/
	public Library_SpriteStudio6.Control.AdditionalColor AdditionalColorGet()
	{
		return(AdditionalColor);
	}
}
