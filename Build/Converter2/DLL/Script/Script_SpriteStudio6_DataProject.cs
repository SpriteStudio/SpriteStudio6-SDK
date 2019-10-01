/**
	SpriteStudio6 Converter Ver.2

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
/* ========================================================================== */
/* SS6Player for Unityには存在しなかった「プロジェクト」の最終データ部です。  */
/*                                                                            */
/* SSBP2的には、全てのデータを1ストリーム化したかったので、（静的データが全て */
/* バラバラになっていてインスタンスのプレハブからの参照で連結している）SS6PU  */
/* の構造のままでは不都合なのために、プロジェクトのデータ部を新造しました。   */
/* ========================================================================== */

using System.Collections;
using System.Collections.Generic;

using SS6ConverterVer2_DLL;
using SS6ConverterVer2_DLL.Types;
using UnityEngine = SS6ConverterVer2_DLL.Assets;

[System.Serializable]
public class Script_SpriteStudio6_DataProject : ScriptableObject
{
	/* ----------------------------------------------- Variables & Properties */
	#region Variables & Properties
	public KindVersion Version;

#if true
	/* MEMO: SS6PUのそのままのデータ */
	public Script_SpriteStudio6_DataCellMap CellMap;
#else
	/* MEMO: SS6ConverterVer2では、こちらの方が望ましい。 */
	/*       ※下記のようにダイレクトにいくか、Script_SpriteStudio6_DataCellMapの基底をLibrary_SpriteStudio6.Data.CellMapにするか。 */
	public Library_SpriteStudio6.Data.CellMap[] CellMap;
#endif
	public Script_SpriteStudio6_DataAnimation[] Animation;
	public Script_SpriteStudio6_DataEffect[] Effect;
	#endregion Variables & Properties

	/* ----------------------------------------------- Functions */
	#region Functions
	public void CleanUp()
	{
		Version = (KindVersion)(-1);
	}

	public bool VersionCheckRuntime()
	{
		return(((KindVersion.SUPPORT_EARLIEST <= Version) && (KindVersion.SUPPORT_LATEST >= Version)));	/* ? true : false */
	}
	#endregion Functions

	/* ----------------------------------------------- Enums & Constants */
	#region Enums & Constants
	public enum KindVersion
	{
		SUPPORT_EARLIEST = CODE_010000,
		SUPPORT_LATEST = CODE_010000,

		SSBP = 0,	/* Before SSBP2 *//* (Reserved) */
		CODE_010000,	/* = 0x00010000 */	/* SS6ConverterVer2 Ver.1.0.0 */
	}
	#endregion Enums & Constants

	/* ----------------------------------------------- Delegates */
	#region Delegates
	private delegate void FunctionSignatureBootUpFunction();
	#endregion Delegates
}
