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
	//! Start playing the animation
	/*!
	@param	timesPlay
		-1 == Apply previous setting.<br>
		0 == Infinite-looping<br>
		1 == Not looping<br>
		default: -1
	@param	RateTime
		Coefficient of time-passage of animation.<br>
		Minus Value is given, Animation is played backwards.<br>
		float.NaN is given, the now-setting is not changed) <br>
		default: float.NaN (Setting is not changed)
	@retval	Return-Value
		true == Success <br>
		false == Failure (Error)

	The playing of animation begins. <br>
	<br>
	The update speed of animation quickens when you give a value that is bigger than 1.0f to "RateTime".
	*/
	public bool AnimationPlay(	int timesPlay = -1,
								float rateTime = float.NaN,
								int framePerSecond = 0
							)
	{
		/* MEMO: Execute initialization to properly process play immediately after instantiating. */
		Start();

		if(0 == (Status & FlagBitStatus.VALID))
		{	/* Not Start */
			Start();
		}

		/* Check Fatal-Error */
		if((null == DataCellMap) || (null == DataEffect))
		{
			return(false);
		}

		/* MEMO: Do not specify 0 to framePerSecond, when control from animation. */
		RateTime = (true == float.IsNaN(rateTime)) ? RateTime : rateTime;
		if(0 == framePerSecond)
		{
			framePerSecond = DataEffect.CountFramePerSecond;
		}
		FramePerSecond = (float)framePerSecond;
		TimePerFrame = 1.0f / FramePerSecond;
//		Frame = 0.0f;
		TimeElapsed = (0.0f > RateTime) ? (FrameRange * TimePerFrame) : 0.0f; 

		/* Status Set */
		Status |= FlagBitStatus.PLAYING;
		Status &= ~(FlagBitStatus.CHANGE_TABLEMATERIAL | FlagBitStatus.CHANGE_CELLMAP);

		StatusPlaying |= Library_SpriteStudio6.Control.Animation.Track.FlagBitStatus.PLAYING;
		StatusPlaying |= Library_SpriteStudio6.Control.Animation.Track.FlagBitStatus.PLAYING_START;

		return(true);
	}

	/* ********************************************************* */
	//! Stop playing the animation
	/*!
	@param	flagReachEnd
		false == Animation is stopped with maintaining the current state.<br>
		true == Animation is stop and jump to last frame.<br>
		default: false
	@retval	Return-Value
		(None)

	The playing of animation stops.
	*/
	public void AnimationStop(bool flagJumpEnd = false)
	{
		/* Status Set */
		Status &= ~FlagBitStatus.PLAYING;

		StatusPlaying &= ~(Library_SpriteStudio6.Control.Animation.Track.FlagBitStatus.PLAYING | Library_SpriteStudio6.Control.Animation.Track.FlagBitStatus.PLAYING_START);
	}

	/* ********************************************************* */
	//! Set the pause-status of the animation
	/*!
	@param	flagSwitch
		true == Set pause (Suspend)<br>
		false == Rerease pause (Resume)
	@retval	Return-Value
		true == Success <br>
		false == Failure (Error)

	The playing of animation suspends or resumes.<br>
	if specific track, return false when the track is not playing.<br>
	*/
	public bool AnimationPause(bool flagSwitch)
	{
		if(0 == (StatusPlaying & Library_SpriteStudio6.Control.Animation.Track.FlagBitStatus.PLAYING))
		{
			return(false);
		}

		if(true == flagSwitch)
		{
			if(0 == (StatusPlaying & Library_SpriteStudio6.Control.Animation.Track.FlagBitStatus.PAUSING))
			{
				StatusPlaying |= Library_SpriteStudio6.Control.Animation.Track.FlagBitStatus.PAUSING;
			}
		}
		else
		{
			if(0 != (StatusPlaying & Library_SpriteStudio6.Control.Animation.Track.FlagBitStatus.PAUSING))
			{
				StatusPlaying &= ~Library_SpriteStudio6.Control.Animation.Track.FlagBitStatus.PAUSING;
			}
		}

		return(true);
	}
	#endregion Functions
}
