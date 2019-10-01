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
	@param	flagInvolveChildren
		true == Children are set same buffer.<br>
		false == only oneself.<br>
		default: true
	@retval	Return-Value
		"AdditionalColor" Setting Buffer

	Get(Create) the Parameter-Buffer of "AdditionalColor" for instances of this class.<br>
	<br>
	Caution that "Additional-Color" affects differs for each animation-object.<br>
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
	public Library_SpriteStudio6.Control.AdditionalColor AdditionalColorCreate(bool flagInvolveChildren=true)
	{
		Library_SpriteStudio6.Control.AdditionalColor additionalColor = AdditionalColor;
		if(null == additionalColor)
		{
			/* Create Parameter-instance */
			additionalColor = new Library_SpriteStudio6.Control.AdditionalColor();
			if(null == additionalColor)
			{	/* Error */
				return(null);
			}
			additionalColor.BootUp();

			if(true == flagInvolveChildren)
			{
				AdditionalColorSet(additionalColor);
			}
			else
			{
				AdditionalColor = additionalColor;
			}
		}
		return(additionalColor);
	}
	private void AdditionalColorSet(Library_SpriteStudio6.Control.AdditionalColor additionalColor)
	{
		AdditionalColor = additionalColor;

		if((null != DataAnimation) && (null != TableControlParts))
		{
			int count;
			int indexParts;
			int[] tableIndexParts = null;

			/* Set child-"Instance"s */
			tableIndexParts = DataAnimation.CatalogParts.TableIDPartsInstance;
			if(null != tableIndexParts)
			{
				count = tableIndexParts.Length;
				for(int i=0; i<count; i++)
				{
					indexParts = tableIndexParts[i];
					if(null != TableControlParts[indexParts].InstanceRootUnderControl)
					{
						TableControlParts[indexParts].InstanceRootUnderControl.AdditionalColorSet(additionalColor);
					}
				}
			}

			/* Set child-"Effect"s */
			tableIndexParts = DataAnimation.CatalogParts.TableIDPartsEffect;
			if(null != tableIndexParts)
			{
				count = tableIndexParts.Length;
				for(int i=0; i<count; i++)
				{
					indexParts = tableIndexParts[i];
					if(null != TableControlParts[indexParts].InstanceRootEffectUnderControl)
					{
						TableControlParts[indexParts].InstanceRootEffectUnderControl.AdditionalColor = additionalColor;
					}
				}
			}
		}
	}

	/* ******************************************************** */
	//! Release "AdditionalColor" Parameter
	/*!
	@param	flagInvolveChildren
		true == Children are set same buffer.<br>
		false == only oneself.<br>
		default: true
	@retval	Return-Value
		(None)

	Release the Parameter-Buffer of "AdditionalColor".<br>
	And stop processing AdditionalColor.<br>
	(Animation and "Effect"s return to state not using AdditionalColor)<br>
	<br>
	To use AdditionalColor again after using this function, use "AdditionalColorCreate" function and re-get parameter-buffer.<br>
	*/
	public void AdditionalColorRelease(bool flagInvolveChildren=true)
	{
		if(true == flagInvolveChildren)
		{
			AdditionalColorSet(null);
		}
		else
		{
			AdditionalColor = null;
		}
	}

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

	/* ******************************************************** */
	//! Cancel Transform refresh
	/*!
	@param	idParts
		Parts-ID<br>
		-1 == Set to all parts
	@param	flagPosition
		true == Cancel Position refresh<br>
		false == Behave as usual
	@param	flagRotation
		true == Cancel Rotation refresh<br>
		false == Behave as usual
	@param	flagScaling
		true == Cancel Scaling refresh<br>
		false == Behave as usual
	@retval	Return-Value
		true == Success<br>
		false == Failure (Error)

	Cancel refresh of parts' transform(position, rotation or scaling) after change animation.<br>
	<br>
	If transform has been changed in previous animation, transform will be reset when change animation.<br>
	The same behavior is also execute in first animation process (in LateUpdate) that animation object initialized.<br>
	This action is necessary for stable animation playing.<br>
	<br>
	However, in rare cases, this "Refreshing" may be obstructive.<br>
	In particular, "SS6 Player for Unity" is a specification that does not intentionally change transform
	 when "Position","Rotation" or "Scaling" are not manipulated with animation data.<br>
	(In that case, you can change animation parts' transform from script)<br>
	<br>
	Use this function if you want to maintain transform without refresh even when you change the animation.<br>
	<br>
	However, if animation after switching has data in "position","rotation" or "scaling", this function do not have  meaning.<br>
	(Because transform is overwritten with animation data)<br>
	*/
	public bool RefreshCancelTransform(int idParts, bool flagPosition, bool flagRotation, bool flagScaling)
	{
		if(null == TableControlParts)
		{
			return(false);
		}
		int countParts = TableControlParts.Length;

		if(0 > idParts)
		{
			bool flagSuccess = true;
			for(int i=0; i<countParts; i++)
			{
				flagSuccess &= RefreshCancelTransformMain(i, flagPosition, flagRotation, flagScaling);
			}

			return(flagSuccess);
		}

		if(countParts <= idParts)
		{
			return(false);
		}

		return(RefreshCancelTransformMain(idParts, flagPosition, flagRotation, flagScaling));
	}
	private bool RefreshCancelTransformMain(int idParts, bool flagPosition, bool flagRotation, bool flagScaling)
	{
		Library_SpriteStudio6.Control.Animation.Parts.FlagBitStatus statusMask = ~Library_SpriteStudio6.Control.Animation.Parts.FlagBitStatus.CLEAR;	/* All bit on */

		if(true == flagPosition)
		{
			statusMask &= ~Library_SpriteStudio6.Control.Animation.Parts.FlagBitStatus.CHANGE_TRANSFORM_POSITION;
		}
		if(true == flagRotation)
		{
			statusMask &= ~Library_SpriteStudio6.Control.Animation.Parts.FlagBitStatus.CHANGE_TRANSFORM_ROTATION;
		}
		if(true == flagScaling)
		{
			statusMask &= ~Library_SpriteStudio6.Control.Animation.Parts.FlagBitStatus.CHANGE_TRANSFORM_SCALING;
		}

		TableControlParts[idParts].Status &= statusMask;

		return(true);
	}

	/* ******************************************************** */
	//! Get instance of "Instance"
	/*!
	@param	idParts
		Parts-ID ("Instance"-part)
	@retval	Return-Value
		"Instance" animation's instance<br>
		null == Error / Invalid instance animation

	Get "Instance" controled by part.<br>
	<br>
	The reason why this function returns null is as follows.
	<br>
	- "idParts" is not "Instance" part<br>
	- "Instance"'s instance does not exist<br>
	- Before executing "Start()"<br>
	*/
	public Script_SpriteStudio6_Root InstanceGet(int idParts)
	{
		if((null == DataAnimation) || (null == TableControlParts))
		{
			return(null);
		}
		if((0 > idParts) || (TableControlParts.Length <= idParts))
		{
			return(null);
		}
		if(Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE != DataAnimation.TableParts[idParts].Feature)
		{
			return(null);
		}

		return(TableControlParts[idParts].InstanceRootUnderControl);
	}

	/* ******************************************************** */
	//! Change "Instance"
	/*!
	@param	idParts
		Parts-ID ("Instance"-part)
	@param	source
		Source of new "Instance" (mainly "Prefab")<br>
		null == Revert to initial data
	@retval	Return-Value
		true == Success<br>
		false == Failure (Error)

	Set(Change) "Instance" controled by part.<br>
	<br>
	Be careful when using this function.<br>
	<br>
	This function is heavey load and consumes "Managed-Heap".<br>
	Because execute following process, basically.<br>
	<br>
	- Destroy old "Instance"<br>
	- Instantiate "source"(New "Instance")<br>
	- Rebuild parent-animation's drawing-meshes buffer<br>
	<br>
	Also, when set new "Instance", animation to be played is set animation of index 0.<br>
	(When revert, set animation-index that original-data has)<br>
	<br>
	Depending on animation data of the newly set "Instance",  may not playing-result as expected.<br>
	(Pay attention that how to be controlled from the "Instance"-part)<br>
	<br>
	Caution that not to cause infinite-nesting by replacing.<br>
	*/
	public bool InstanceChange(int idParts, GameObject source)
	{
		if((null == DataAnimation) || (null == TableControlParts))
		{
			return(false);
		}
		if((0 > idParts) || (TableControlParts.Length <= idParts))
		{
			return(false);
		}
		if(Library_SpriteStudio6.Data.Parts.Animation.KindFeature.INSTANCE != DataAnimation.TableParts[idParts].Feature)
		{
			return(false);
		}

		Script_SpriteStudio6_Root scriptRootHighest = RootGetHighest();
		if(null == scriptRootHighest)
		{
			scriptRootHighest = this;
		}

		/* Renew "Instance" */
		if(false == TableControlParts[idParts].BootUpInstance(this, idParts, true, source))
		{
			return(false);
		}

		/* Rebuild Hiest-Root's Draw buffes */
		if(false == scriptRootHighest.ClusterBootUpDraw())
		{
			return(false);
		}

		return(true);
	}

	/* ******************************************************** */
	//! Change "Instance"'s Animation (by name)
	/*!
	@param	idParts
		Parts-ID ("Instance"-part)
	@param	nameAnimation
		New "Instance"'s animation-name<br>
		"" or null == Change only "ignoreAttribute"
	@param	ignoreAttribute
		NON == Restart animation when new "Instance" attribute deecoded<br>
		NOW_ANIMATION == Ignore "Instance" attribute until new animation starts playing<br>
		PERMANENT == Continue Ignoring "Instance" attribute even if new animation starts playing
	@param	flagStartImmediate
		true == Animation is started immediate<br>
		false == Start playing according to "Instance" attribute
	@param	timesPlay
		0 == Infinite-looping<br>
		1 == Not looping<br>
		2 <= Number of Plays
	@param	rateTime
		Coefficient of time-passage of new animation.<br>
		Minus Value is given, Animation is played backwards.
	@param	style
		Library_SpriteStudio6.KindStylePlay.NOMAL == Animation is played One-Way.<br>
		Library_SpriteStudio6.KindStylePlay.PINGPONG == Animation is played round-trip.
	@param	labelRangeStart
		Label name to start playing animation.<br>
		"" or "_start" == Top frame of Animation ("_start" is reserved label-name)
	@param	frameRangeOffsetStart
		Offset frame from labelRangeStart<br>
		Start frame of animation play range is "labelRangeStart + frameRangeOffsetStart".
	@param	labelRangeEnd
		Label-name of the terminal in animation.<br>
		"" or "_end" == Last frame of Animation ("_end" is reserved label-name)
	@param	frameRangeOffsetEnd
		Offset frame from labelRangeStart<br>
		End frame of animation play range is "labelRangeEnd + frameRangeOffsetEnd".
	@retval	Return-Value
		true == Success<br>
		false == Failure (Error)

	Change "Instance"'s animation controled by part.<br>
	<br>
	To change "Instance"'s animation, use this function without calling "Instance"'s "Script_SpriteStudio6_Root.AnimationPlay".<br>
	(Cause inconsistency with control from "Instance" part)<br>
	<br>
	As a general rule, no designation to "Do not change playing  parameters" like "AnimationPlay".<br>
	The only exception is changing attribute-ignore setting(ignoreAttribute)  without changing the animation.<br>
	When set null or "" to "nameAnimation", change only attribute-ignore setting.<br>
	(The argument omission after "flagStartImmediate" is written to change only attribute-ignore setting)<br>
	<br>
	When "nameAnimation" is set normally animation name...<br>
	If "flagStartImmediate" is set to true, "Instance"'s animation will be played immediately.<br>
	In the case, "Instance" play behaves the same as when "Independent of time" is checked in the "Instance" attribute on SpriteStudio6.<br>
	Conversely, If "flagStartImmediate" is set to false, forcefully wait for next data of "Instance" attribute and start playing at decoding new data.
	*/
	public bool AnimationChangeInstance(	int idParts,
											string nameAnimation,
											Library_SpriteStudio6.KindIgnoreAttribute ignoreAttribute,
											bool flagStartImmediate = false,
											int timesPlay = 1,
											float rateTime = 1.0f,
											Library_SpriteStudio6.KindStylePlay style = Library_SpriteStudio6.KindStylePlay.NORMAL,
											string labelRangeStart = null,
											int frameRangeOffsetStart = 0,
											string labelRangeEnd = null,
											int frameRangeOffsetEnd = 0
										)
	{
		Script_SpriteStudio6_Root scriptRootInstance = InstanceGet(idParts);
		if(null == scriptRootInstance)
		{
			return(false);
		}

		int indexAnimation = -1;
		if(false == string.IsNullOrEmpty(nameAnimation))
		{
			indexAnimation = scriptRootInstance.IndexGetAnimation(nameAnimation);
			if(0 > indexAnimation)
			{
				return(false);
			}
		}

		return(AnimationChangeInstanceMain(	ref TableControlParts[idParts],
											scriptRootInstance,
											indexAnimation,
											ignoreAttribute,
											flagStartImmediate,
											timesPlay,
											rateTime,
											style,
											labelRangeStart,
											frameRangeOffsetStart,
											labelRangeEnd,
											frameRangeOffsetEnd
										)
			);
	}
	private bool AnimationChangeInstanceMain(	ref Library_SpriteStudio6.Control.Animation.Parts controlParts,
												Script_SpriteStudio6_Root scriptRootInstance,
												int indexAnimation,
												Library_SpriteStudio6.KindIgnoreAttribute ignoreAttribute,
												bool flagStartImmediate,
												int timesPlay,
												float rateTime,
												Library_SpriteStudio6.KindStylePlay style,
												string labelRangeStart,
												int frameRangeOffsetStart,
												string labelRangeEnd,
												int frameRangeOffsetEnd
											)
	{
		if(null == TableControlTrack)
		{
			return(false);
		}
		if(0 > timesPlay)
		{
			return(false);
		}
		if(true == float.IsNaN(rateTime))
		{
			return(false);
		}

		Library_SpriteStudio6.Data.Animation.Attribute.Instance dataInstance = new Library_SpriteStudio6.Data.Animation.Attribute.Instance();
		dataInstance.Flags = Library_SpriteStudio6.Data.Animation.Attribute.Instance.FlagBit.CLEAR;
		switch(style)
		{
			case Library_SpriteStudio6.KindStylePlay.NO_CHANGE:
				return(false);

			case Library_SpriteStudio6.KindStylePlay.NORMAL:
				break;

			case Library_SpriteStudio6.KindStylePlay.PINGPONG:
				dataInstance.Flags |= Library_SpriteStudio6.Data.Animation.Attribute.Instance.FlagBit.PINGPONG;
				break;
		}

		controlParts.Status &= ~Library_SpriteStudio6.Control.Animation.Parts.FlagBitStatus.INSTANCE_IGNORE_EXCEPT_NEXTDATA;
		switch(ignoreAttribute)
		{
			case Library_SpriteStudio6.KindIgnoreAttribute.NON:
				controlParts.Status &= ~Library_SpriteStudio6.Control.Animation.Parts.FlagBitStatus.INSTANCE_IGNORE_ATTRIBUTE;
				controlParts.Status &= ~Library_SpriteStudio6.Control.Animation.Parts.FlagBitStatus.INSTANCE_IGNORE_NEWANIMATION;

				dataInstance.Flags &= ~Library_SpriteStudio6.Data.Animation.Attribute.Instance.FlagBit.INDEPENDENT;
				break;

			case Library_SpriteStudio6.KindIgnoreAttribute.NOW_ANIMATION:
				controlParts.Status |= Library_SpriteStudio6.Control.Animation.Parts.FlagBitStatus.INSTANCE_IGNORE_ATTRIBUTE;
				controlParts.Status &= ~Library_SpriteStudio6.Control.Animation.Parts.FlagBitStatus.INSTANCE_IGNORE_NEWANIMATION;

				dataInstance.Flags |= Library_SpriteStudio6.Data.Animation.Attribute.Instance.FlagBit.INDEPENDENT;
				break;

			case Library_SpriteStudio6.KindIgnoreAttribute.PERMANENT:
				controlParts.Status |= Library_SpriteStudio6.Control.Animation.Parts.FlagBitStatus.INSTANCE_IGNORE_ATTRIBUTE;
				controlParts.Status |= Library_SpriteStudio6.Control.Animation.Parts.FlagBitStatus.INSTANCE_IGNORE_NEWANIMATION;

				dataInstance.Flags |= Library_SpriteStudio6.Data.Animation.Attribute.Instance.FlagBit.INDEPENDENT;
				break;
		}

		dataInstance.PlayCount = timesPlay;
		dataInstance.RateTime = rateTime;
		dataInstance.LabelStart = labelRangeStart;
		dataInstance.OffsetStart = frameRangeOffsetStart;
		dataInstance.LabelEnd = labelRangeEnd;
		dataInstance.OffsetEnd = frameRangeOffsetEnd;

		if(false == flagStartImmediate)
		{
			controlParts.Status &= ~Library_SpriteStudio6.Control.Animation.Parts.FlagBitStatus.INSTANCE_IGNORE_EXCEPT_NEXTDATA;
		}
		else
		{
			controlParts.Status |= Library_SpriteStudio6.Control.Animation.Parts.FlagBitStatus.INSTANCE_IGNORE_EXCEPT_NEXTDATA;

			/* MEMO: When does not start immediately, necessary to always decode next data, so turn off "Independent time" status. */
			controlParts.Status &= ~Library_SpriteStudio6.Control.Animation.Parts.FlagBitStatus.INSTANCE_PLAY_INDEPENDENT;

			controlParts.IndexAnimationUnderControl = indexAnimation;
			controlParts.DataInstance = dataInstance;

			int indexTrack = controlParts.IndexControlTrack;
			if(0 <= indexTrack)
			{
				float rateTimeControlTrack = TableControlTrack[indexTrack].RateTime * ((true == TableControlTrack[indexTrack].StatusIsPlayingReverse) ? -1.0f : 1.0f);
				return(controlParts.InstancePlayStart(this, rateTimeControlTrack));
			}
		}

		return(true);
	}

	/* ******************************************************** */
	//! Change "Instance"'s Animation (by index)
	/*!
	@param	idParts
		Parts-ID ("Instance"-part)
	@param	indexAnimation
		New "Instance"'s animation-index<br>
		-1 == Change only "ignoreAttribute"
	@param	flagStartImmediate
		true == Animation is started immediate<br>
		false == Start playing according to "Instance" attribute
	@param	ignoreAttribute
		NON == Restart animation when new "Instance" attribute deecoded<br>
		NOW_ANIMATION == Ignore "Instance" attribute until new animation starts playing<br>
		PERMANENT == Continue Ignoring "Instance" attribute even if new animation starts playing
	@param	rateTime
		Coefficient of time-passage of new animation.<br>
		Minus Value is given, Animation is played backwards.
	@param	style
		Library_SpriteStudio6.KindStylePlay.NOMAL == Animation is played One-Way.<br>
		Library_SpriteStudio6.KindStylePlay.PINGPONG == Animation is played round-trip.
	@param	labelRangeStart
		Label name to start playing animation.<br>
		"" or "_start" == Top frame of Animation ("_start" is reserved label-name)
	@param	frameRangeOffsetStart
		Offset frame from labelRangeStart<br>
		Start frame of animation play range is "labelRangeStart + frameRangeOffsetStart".
	@param	labelRangeEnd
		Label-name of the terminal in animation.<br>
		"" or "_end" == Last frame of Animation ("_end" is reserved label-name)
	@param	frameRangeOffsetEnd
		Offset frame from labelRangeStart<br>
		End frame of animation play range is "labelRangeEnd + frameRangeOffsetEnd".
	@retval	Return-Value
		"Instance" animation's instance<br>
		null == Error / Invalid instance animation

	Change "Instance"'s animation controled by part.<br>
	<br>
	This function is the same as "name designation" except that animation is "index designation".<br>
	(This function is a little bit faster than "name designation" by not searching animation names)<br>
	Can use if "Instance"'s animation indexes have been gotten in advance.<br>
	*/
	public bool AnimationChangeInstance(	int idParts,
											int indexAnimation,
											Library_SpriteStudio6.KindIgnoreAttribute ignoreAttribute,
											bool flagStartImmediate = false,
											int timesPlay = 1,
											float rateTime = 1.0f,
											Library_SpriteStudio6.KindStylePlay style = Library_SpriteStudio6.KindStylePlay.NORMAL,
											string labelRangeStart = null,
											int frameRangeOffsetStart = 0,
											string labelRangeEnd = null,
											int frameRangeOffsetEnd = 0
										)
	{
		Script_SpriteStudio6_Root scriptRootInstance = InstanceGet(idParts);
		if(null == scriptRootInstance)
		{
			return(false);
		}

		if(0 <= indexAnimation)
		{
			if(scriptRootInstance.CountGetAnimation() <= indexAnimation)
			{
				return(false);
			}
		}

		return(AnimationChangeInstanceMain(	ref TableControlParts[idParts],
											scriptRootInstance,
											indexAnimation,
											ignoreAttribute,
											flagStartImmediate,
											timesPlay,
											rateTime,
											style,
											labelRangeStart,
											frameRangeOffsetStart,
											labelRangeEnd,
											frameRangeOffsetEnd
										)
			);
	}

	/* ******************************************************** */
	//! Get instance of "Effect"
	/*!
	@param	idParts
		Parts-ID
	@retval	Return-Value
		"Effect" animation's instance<br>
		null == Error / Invalid instance animation

	Get "Effect" animation controlled by part.<br>
	<br>
	The reason why this function returns null is as follows.
	<br>
	- "idParts" is not "Effect" part<br>
	- "Effect"'s instance does not exist<br>
	- Before executing "Start()"<br>
	*/
	public Script_SpriteStudio6_RootEffect EffectGet(int idParts)
	{
		if((null == DataAnimation) || (null == TableControlParts))
		{
			return(null);
		}

		if((0 > idParts) || (TableControlParts.Length <= idParts))
		{
			return(null);
		}

		if(Library_SpriteStudio6.Data.Parts.Animation.KindFeature.EFFECT != DataAnimation.TableParts[idParts].Feature)
		{
			return(null);
		}

		return(TableControlParts[idParts].InstanceRootEffectUnderControl);
	}

//	EffectChange
	#endregion Functions
}
