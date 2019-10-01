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
	//! Get "Playing (animation)" status
	/*!
	@param	indexTrack
		Track index (0 origin)<br>
		-1 == Target the animation object
	@retval	Return-Value
		true == Now-playing<br>
		false == Error / Not playing

	Gets status as to whether animation is playing on track.<br>
	<br>
	Do not use property "StatusIsPlaying" directly. (For internal processing, Specification will be changed without notice)<br>
	*/
	public bool StatusGetPlay(int indexTrack)
	{
		if(null == TableControlTrack)
		{
			return(false);
		}

		if(0 > indexTrack)
		{
			return(StatusIsPlaying);
		}

		if(TableControlTrack.Length <= indexTrack)
		{
			return(false);
		}

		return(TableControlTrack[indexTrack].StatusIsPlaying);
	}

	/* ******************************************************** */
	//! Get "Pausing (animation)" status
	/*!
	@param	indexTrack
		Track index (0 origin)<br>
		-1 == All tracks
	@retval	Return-Value
		true == Pausing (Now-Suspended)<br>
		false == Error / Not playing / 

	Gets "Pause(suspended)" status as to whether animation is playing on track.<br>
	<br>
	When indexTrack is set "-1", return "true" if all playing tracks are paused.<br>
	<br>
	Do not use property "StatusIsPausing" directly. (For internal processing, Specification will be changed without notice)<br>
	*/
	public bool StatusGetPause(int indexTrack)
	{
		if(null == TableControlTrack)
		{
			return(false);
		}

		int countTrack = TableControlTrack.Length;

		if(0 > indexTrack)
		{
			for(int i=0; i<countTrack; i++)
			{
				if(true == TableControlTrack[i].StatusIsPlaying)
				{
					if(true == TableControlTrack[i].StatusIsPausing)
					{
						return(true);
					}
				}
			}
			return(false);
		}

		if(countTrack <= indexTrack)
		{
			return(false);
		}
		return(TableControlTrack[indexTrack].StatusIsPausing);
	}

	/* ******************************************************** */
	//! Get number of "Play-Track"
	/*!
	@param	
		(none)
	@retval	Return-Value
		Number of Play-Track<br>
		-1 == Error / "Name" is not-found.

	Get number of "Play-Track".<br>
	Up to this value, can play animations in parallel.<br>
	*/
	public int CountGetTrack()
	{
		return((null != TableControlTrack) ? TableControlTrack.Length : -1);
	}

	/* ******************************************************** */
	//! Change number of "Play-Track".
	/*!
	@param	count
		New number of tracks (Must be 1 or more)<br>
	@retval	Return-Value
		true == Success<br>
		false == Failure (Error)

	Change number of Play-Tracks.<br>
	If it is the same as current number of Play-Tracks, this function does not change anything.<br>
	<br>
	Playing-status of each track is preserved.<br>
	However, if becomes less than original number of tracks, playing-status of cut Play-Tracks will be lost.<br>
	Also, parts connected to cut Play-Tracks will be disconnected.<br>
	*/
	public bool TrackReboot(int count)
	{
		/* MEMO: Execute initialization to properly process play immediately after instantiating. */
		Start();

		if(0 >= count)
		{
			return(false);
		}

		int countTrack = ControlBootUpTrack(count);
		if(0 >= countTrack)
		{
			return(false);
		}

		/* Renew Play-Informations */
		if(TableInformationPlay.Length != countTrack)	/* (TableInformationPlay.Length < countTrack) */
		{
			InformationPlay[] tableInformationPlayNow = TableInformationPlay;
			TableInformationPlay = new InformationPlay[countTrack];
			if(null == TableInformationPlay)
			{
				return(false);
			}

			for(int i=0; i<countTrack; i++)
			{
				TableInformationPlay[i].CleanUp();
			}

			count = tableInformationPlayNow.Length;	/* Recycle */
			if(countTrack < count)
			{
				count = countTrack;
			}
			for(int i=0; i<count; i++)
			{
				TableInformationPlay[i] = tableInformationPlayNow[i];
			}

			tableInformationPlayNow = null;
		}

		/* Check conneced partst */
		if(null != TableControlParts)
		{
			int countParts = TableControlParts.Length;
			for(int i=0; i<countParts; i++)
			{
				if(count <= TableControlParts[i].IndexControlTrack)
				{
					TableControlParts[i].IndexControlTrack = -1;
				}
			}
		}

		return(true);
	}

	/* ******************************************************** */
	//! Connect parts to Play-Track
	/*!
	@param	idParts
		Parts-ID<br>
		-1 == all parts
	@param	indexTrack
		Play-Track index
	@param	flagChildParts
		true == Also connect child parts of "idParts"<br>
		false == only "idParts"
	@retval	Return-Value
		true == Success<br>
		false == Failure (Error)

	Connect (animation)parts to Play-Track.<br>
	The playing animation on connected Play-Track will be applied to specified parts.<br>
	<br>
	Using this function, it is possible to synthesize and play multiple animations to 1 animation object.<br>
	*/
	public bool TrackConnectParts(int idParts, int indexTrack, bool flagChildParts = false)
	{
		/* MEMO: Execute initialization to properly process play immediately after instantiating. */
		Start();

		if((null == TableControlParts) || (null == DataAnimation))
		{
			return(false);
		}

		int countParts = TableControlParts.Length;
		if(0 > idParts)
		{	/* All parts */
			/* MEMO: Ignore "flagChildParts" */
			for(int i=0; i<countParts; i++)
			{
				TableControlParts[i].IndexControlTrack = indexTrack;
			}
		}
		else
		{	/* Specific pats */
			if(countParts <= idParts)
			{
				return(false);
			}
			if(true == flagChildParts)
			{
				TrackConnectPartsInvolveChild(idParts, indexTrack);
			}
			else
			{
				TableControlParts[idParts].IndexControlTrack = indexTrack;
			}
		}

		return(true);
	}
	private void TrackConnectPartsInvolveChild(int idParts, int indexTrack)
	{
		TableControlParts[idParts].IndexControlTrack = indexTrack;

		int countPartsChild = DataAnimation.TableParts[idParts].TableIDChild.Length;
		for(int i=0; i<countPartsChild; i++)
		{
			TrackConnectPartsInvolveChild(DataAnimation.TableParts[idParts].TableIDChild[i], indexTrack);
		}
	}

	/* ********************************************************* */
	//! Transition the animation
	/*!
	@param	indexTrack
		Track index of now playing (Master track. 0 origin)
	@param	indexTrackSlave
		Track index to manage transition destination animation (Slave track. 0 origin)<br>
		-1 == Cancel transition
	@param	time
		Time to transition (1.0f = 1 second)
	@param	flagCancelPauseAfterTransition
		Cancel pause state after transition is completed<br>
		true == Cancel (Playing)<br>
		false == Leave master track's settings
	@retval	Return-Value
		true == Success<br>
		false == Failure (Error)

	Fades from the current playing state to first frame of the specified animation.<br>
	However, Transition is targeting only TRS(Position, Rotation and Scaling. Except "Local-Scaling").<br>
	Not apply to "Instance" animation too.<br>
	<br>
	Track 0 should not be used Slave side. (because Track 0 is master track of the entire animation)<br>
	<br>
	When transition is complete, destination-animation will be played on indexTrack and indexTrackSlave will be in stopped state.<br>
	(IndexTrackSlave is only used for managing fade destination animation)<br>
	<br>
	When master track is in transition, this function returns error.<br>
	<br>
	If transition is canceled in the middle, state of the transition is also canceled.<br>
	(Return to the same state as not being transitioned)<br>
	*/
	public bool TrackTransition(	int indexTrack,
									int indexTrackSlave,
									float time,
									bool flagCancelPauseAfterTransition
							)
	{
		/* MEMO: Execute initialization to properly process play immediately after instantiating. */
		Start();

		if(null == TableControlTrack)
		{
			return(false);
		}

		int countTrack = TableControlTrack.Length;
		if(0 > indexTrack)
		{
			if(0 <= indexTrackSlave)
			{
				return(false);
			}

			for(int i=0; i<countTrack; i++)
			{
				indexTrackSlave = TableControlTrack[i].IndexTrackSlave;
				if(0 <= indexTrackSlave)
				{
					TableControlTrack[indexTrackSlave].Stop();
					TableControlTrack[indexTrack].Transition(-1, 0.0f);
				}
			}

			return(true);
		}

		if(countTrack <= indexTrack)
		{
			return(false);
		}
		if(0 > indexTrackSlave)
		{	/* Cancel Transition */
			indexTrackSlave = TableControlTrack[indexTrack].IndexTrackSlave;
			TableControlTrack[indexTrackSlave].Stop();
			TableControlTrack[indexTrack].Transition(-1, 0.0f);

			return(true);
		}
		if(countTrack <= indexTrackSlave)
		{
			return(false);
		}

		if(false == TableControlTrack[indexTrackSlave].StatusIsPlaying)
		{	/* Slave, Playing Stop */
			return(false);
		}

		if(0 <= TableControlTrack[indexTrack].IndexTrackSlave)
		{	/* Master, Transitioning now */
			return(false);
		}
		if(0.0f >= time)
		{	/* time Invalid */
			return(false);
		}

		/* Set Master-Track to fade mode */
		TableControlTrack[indexTrack].StatusIsTransitionStart = true;
		TableControlTrack[indexTrack].StatusIsTransitionCancelPause = flagCancelPauseAfterTransition;
		return(TableControlTrack[indexTrack].Transition(indexTrackSlave, time));
	}

	/* ******************************************************** */
	//! Get Animation's index (Playing on track)
	/*!
	@param	indexTrack
		Track index (0 origin)
	@retval	Return-Value
		Animation's index<br>
		-1 == Error / Not playing

	Get animation's index being played on track.<br>
	*/
	public int IndexGetAnimation(int indexTrack)
	{
		if(null == TableControlTrack)
		{
			return(-1);
		}

		if((0 > indexTrack) || (TableControlTrack.Length <= indexTrack))
		{
			return(-1);
		}

		return(TableControlTrack[indexTrack].ArgumentContainer.IndexAnimation);
	}

	/* ******************************************************** */
	//! Get Track's index (Connected to Parts)
	/*!
	@param	idParts
		Parts-ID
	@retval	Return-Value
		Track's index<br>
		-1 == Error / Not connected

	Get track index connected to (animation's) part.<br>
	Default value is 0.<br>
	*/
	public int IndexGetTrack(int idParts)
	{
		if((null == TableControlTrack) || (null == TableControlParts))
		{
			return(-1);
		}

		if((0 > idParts) || (TableControlParts.Length <= idParts))
		{
			return(-1);
		}

		return(TableControlParts[idParts].IndexControlTrack);
	}

	/* ******************************************************** */
	//! Get Svale-Track's index
	/*!
	@param	indexTrack
		Track index (0 origin)
	@retval	Return-Value
		Slave-Track's index<br>
		-1 == Error / Not transitioned

	Get Slave-Track's index if the track is transitioning (has Slave-Track).<br>
	*/
	public int IndexGetTrackSlave(int indexTrack)
	{
		if(null == TableControlTrack)
		{
			return(-1);
		}

		if((0 > indexTrack) || (TableControlTrack.Length <= indexTrack))
		{
			return(-1);
		}

		return(TableControlTrack[indexTrack].IndexTrackSlave);
	}

	/* ********************************************************* */
	//! Changing animations' playing speed
	/*!
	@param	indexTrack
		Track index (0 origin)<br>
		-1 == All tracks
	@param	rateTime
		Coefficient of time-passage of animation.<br>
		Minus Value is given, Animation is played backwards.
	@retval	Return-Value
		true == Success<br>
		false == Failure (Error)

	Change speed of the animation during playing.<br>
	*/
	public bool RateTimeSet(int indexTrack, float rateTime)
	{
		if(null == TableControlTrack)
		{
			return(false);
		}

		int countTrack = TableControlTrack.Length;
		if(0 > indexTrack)
		{	/* All track */
			/* MEMO: Stop all current playback and play single animation at track 0. */
			for(int i=0; i<countTrack; i++)
			{
				if(true == TableControlTrack[i].StatusIsPlaying)
				{
					TableControlTrack[i].RateTime = rateTime;
				}
			}
		}
		else
		{	/* Specific track */
			if(countTrack <= indexTrack)
			{
				return(false);
			}
			if(false == TableControlTrack[indexTrack].StatusIsPlaying)
			{
				return(false);
			}
			TableControlTrack[indexTrack].RateTime = rateTime;
		}
		return(true);
	}

	/* ********************************************************* */
	//! Get animations' playing speed
	/*!
	@param	indexTrack
		Track index (0 origin)<br>
		-1 == All tracks
	@retval	Return-Value
		Coefficient of time-passage of animation.<br>
		float.NaN == Failure (Error)

	Change speed of the animation during playing.<br>
	For tracks that are not playing, playing speed up to previous time is returned.<br>
	*/
	public float RateTimeGet(int indexTrack)
	{
		if(null == TableControlTrack)
		{
			return(float.NaN);
		}

		if((0 > indexTrack) || (TableControlTrack.Length <= indexTrack))
		{
			return(float.NaN);
		}

//		if(true == TableControlTrack[indexTrack].StatusIsPlaying)
//		{
//			return(float.NaN);
//		}

		return(TableControlTrack[indexTrack].RateTime);
	}

	/* ******************************************************** */
	//! Get remaining play count
	/*!
	@param	indexTrack
		Track index (0 origin)
	@retval	Return-Value
		remaining play count<br>
		0 == Infinite loop (No finish)<br>
		-1 == Error / Not playing

	Get animation's index being played on track.<br>
	*/
	public int PlayTimesGetRemain(int indexTrack)
	{
		if(null == TableControlTrack)
		{
			return(-1);
		}

		if((0 > indexTrack) || (TableControlTrack.Length <= indexTrack))
		{
			return(-1);
		}

		if(false == TableControlTrack[indexTrack].StatusIsPlaying)
		{
			return(-1);
		}

		return(TableControlTrack[indexTrack].TimesPlay);
	}

	/* ******************************************************** */
	//! Get range of Track-Cursor
	/*!
	@param	indexTrack
		Track index (0 origin)
	@retval	Return-Value
		Track-Cursor range<br>
		float.Nan == Error / Not playing

	Gets Track-Cursor's maximum value in now-playing animation.<br>
	"Track-Cursor" is position of now-playing animation's timeline.<br>
	As a matter of fact, "Track-Cursor" is the value from "0.0" to
		"animation timeline length (seconds)".(The Absolute time of
		the position on timeline)<br>
	However, when the animation is played round-trip(PINGPONG),
		the range is doubled and "Track- Cursor" value also differs
		between forward and return.<br>
	*/
	public float RangeGetCursor(int indexTrack)
	{
		if(null == TableControlTrack)
		{
			return(float.NaN);
		}

		if((0 > indexTrack) || (TableControlTrack.Length <= indexTrack))
		{
			return(float.NaN);
		}

		if(false == TableControlTrack[indexTrack].StatusIsPlaying)
		{
			return(float.NaN);
		}

		float timeRange = TableControlTrack[indexTrack].TimeRange;
		if(true == TableControlTrack[indexTrack].StatusIsPlayStylePingpong)
		{	/* Play-Style: PingPong */
			timeRange *= 2.0f;
		}
		return(timeRange);
	}

	/* ******************************************************** */
	//! Get Track-Cursor
	/*!
	@param	indexTrack
		Track index (0 origin)
	@retval	Return-Value
		Track-Cursor (Seek point)<br>
		-1.0f == Waiting for stating animation. (in delay)<br>
		float.NaN == Error / Not playing

	Get Track-Cursor of now-playing animation.<br>
	When play-status is "waiting to start" due to conditions such
		as data, this function returns -1.0f.<br>
	*/
	public float CursorGet(int indexTrack)
	{
		if(null == TableControlTrack)
		{
			return(float.NaN);
		}

		if((0 > indexTrack) || (TableControlTrack.Length <= indexTrack))
		{
			return(float.NaN);
		}

		if(false == TableControlTrack[indexTrack].StatusIsPlaying)
		{
			return(float.NaN);
		}
		if(0.0f < TableControlTrack[indexTrack].TimeDelay)
		{	/* In delay */
			return(-1.0f);
		}

		float timeCursor = TableControlTrack[indexTrack].TimeElapsed;
		if(true == TableControlTrack[indexTrack].StatusIsPlayStylePingpong)
		{	/* Play-Style: PingPong */
			if(TableControlTrack[indexTrack].StatusIsPlayingReverse != TableControlTrack[indexTrack].StatusIsPlayStyleReverse)
			{	/* Return */
				timeCursor += TableControlTrack[indexTrack].TimeRange;
			}
		}
		return(timeCursor);
	}

	/* ******************************************************** */
	//! Set Track-Cursor
	/*!
	@param	indexTrack
		Track index (0 origin)
	@param	cursor
		Track-Cursor (Absolute time on animation's timeline)
	@retval	Return-Value
		true == Success<br>
		false == Failure (Error)

	Set Track-Cursor of now-playing animation.<br>
	This function is "CursorGet"'s paired function.<br>
	When update animation at directry-after this function,
		"User-Data"s between previous and new cursor is not
		decode (ignored).<br>
	<br>
	Note: This function may change the specification in near future.<br>
	*/
	public bool CursorSet(int indexTrack, float cursor)
	{
		if((true == float.IsNaN(cursor)) || (0.0f > cursor))
		{	/* Invalid value or In delay */
			return(false);
		}

		if(null == TableControlTrack)
		{
			return(false);
		}

		int countTrack = TableControlTrack.Length;
		if(0 > indexTrack)
		{	/* All Tracks */
			bool flagSucceed = false;
			for(int i=0; i<countTrack; i++)
			{
				flagSucceed |= CursorSetMain(i, cursor);
			}

			return(flagSucceed);
		}

		if(countTrack <= indexTrack)	{
			return(false);
		}
		CursorSetMain(indexTrack, cursor);

		return(true);
	}
	private bool CursorSetMain(int indexTrack, float cursor)
	{
		if(false == TableControlTrack[indexTrack].StatusIsPlaying)
		{
			return(false);
		}

		float rangeCursor = TableControlTrack[indexTrack].TimeRange;
		if(true == TableControlTrack[indexTrack].StatusIsPlayStylePingpong)
		{	/* Play-Style: PingPong */
			bool flagReverse = TableControlTrack[indexTrack].StatusIsPlayStyleReverse;
			if(rangeCursor <= cursor)
			{	/* Return */
				cursor -= rangeCursor;
				flagReverse = !flagReverse;
			}
			if(true == flagReverse)
			{
				TableControlTrack[indexTrack].Status |= Library_SpriteStudio6.Control.Animation.Track.FlagBitStatus.PLAYING_REVERSE;
			}
			else
			{
				TableControlTrack[indexTrack].Status &= ~Library_SpriteStudio6.Control.Animation.Track.FlagBitStatus.PLAYING_REVERSE;
			}
		}

		if(rangeCursor < cursor)
		{	/* Range Over */
			return(false);
		}

		/* MEMO: No decode User-Data as the elapsed section is abnormal. */
		TableControlTrack[indexTrack].Status |= Library_SpriteStudio6.Control.Animation.Track.FlagBitStatus.IGNORE_NEXTUPDATE_USERDATA;
		TableControlTrack[indexTrack].TimeElapsed = cursor;

		return(true);
	}

	/* ******************************************************** */
	//! Snap Track-Cursor to frame.
	/*!
	@param	indexTrack
		Track index (0 origin)
	@param	cursor
		Track-Cursor
	@retval	Return-Value
		Snapped value of Track-Cursor<br>
		float.NaN == Error / Not playing

	Snaps Track-Cursor to a frame.<br>
	Calculated according to FPS of now-playing animation.<br>
	<br>
	Note: This function may change the specification in near future.<br>
	*/
	public float CursorSnapFrame(int indexTrack, float cursor)
	{
		if((true == float.IsNaN(cursor)) || (0.0f > cursor))
		{	/* Invalid value or In delay */
			return(float.NaN);
		}

		if(null == TableControlTrack)
		{
			return(float.NaN);
		}

		if((0 > indexTrack) || (TableControlTrack.Length <= indexTrack))
		{
			return(float.NaN);
		}

		if(false == TableControlTrack[indexTrack].StatusIsPlaying)
		{
			return(float.NaN);
		}

		float frameFloat = cursor * (float)TableControlTrack[indexTrack].FramePerSecond;
		int frame = (int)((true == TableControlTrack[indexTrack].StatusIsPlayingReverse) ? Mathf.Ceil(frameFloat) : Mathf.Floor(frameFloat));

		return((float)frame * TableControlTrack[indexTrack].TimePerFrame);
	}
	#endregion Functions
}
