/**
	SpriteStudio6 Converter Ver.2

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
/* ========================================================================== */
/* Unityからソースを持ってくる上でのUnity独自のMath関係の互換関数です（完全に */
/* 移植しているというわけではありません）。                                   */
/*                                                                            */
/* Mathが元々Mathfを静的クラス（実は構造体）実装しているために、Mathf.～のア  */
/* クセスが殆どのために、同じ定義方法にしてありますが……不都合なら、後に名前 */
/* 空間に定義を直すかもしれません。                                           */
/*                                                                            */
/* ※現状コンバート処理のための作成なので、ランタイムコストを若干度外視した実 */
/*   装になっています。                                                       */
/* ========================================================================== */
using System;
using System.Collections.Generic;
using System.Text;

namespace SS6ConverterVer2_DLL
{
/* MEMO: 都合が悪かったらnamespaceにすること。 */
public static class Mathf
{
	/* ----------------------------------------------- Variables & Properties */
	#region Variables & Properties
	#endregion Variables & Properties

	/* ----------------------------------------------- Functions */
	#region Functions
	public static float Floor(float value)
	{
		return((float)(Math.Floor((double)value)));
	}

	public static float Sin(float radian)
	{
		return((float)(Math.Sin((double)radian)));
	}
	public static float Cos(float radian)
	{
		return((float)(Math.Cos((double)radian)));
	}
	public static float Tan(float radian)
	{
		return((float)(Math.Tan((double)radian)));
	}

	public static float Clamp(float value, float min, float max)
	{
		/* MEMO: 今はランタイムコスト考えてない実装です。 */
		if(min >= value)	{
			return(min);
		}
		if(max <= value)	{
			return(max);
		}

		return(value);
	}

	public static float Clamp01(float value)
	{
		/* MEMO: 今はランタイムコスト考えてない実装です。 */
		return(Clamp(value, 0.0f, 1.0f));
	}
	#endregion Functions

	/* ----------------------------------------------- Enums & Constants */
	#region Enums & Constants
	public const float PI = 3.14159265358979f;			/* 円周率 */
	public const float Deg2Rad = (2.0f * PI) / 360.0f;	/* 角度→ラジアンの変換定数 */
	public const float Rad2Deg = 360.0f / (2.0f * PI);	/* ラジアン→角度の変換定数 */
	#endregion Enums & Constants

	/* ----------------------------------------------- Classes, Structs & Interfaces */
	#region Classes, Structs & Interfaces
	#endregion Classes, Structs & Interfaces

	/* ----------------------------------------------- Delegate types */
	#region Delegates
	#endregion Delegates
}	/* Math */
}	/* SS6ConverterVer2_DLL */
