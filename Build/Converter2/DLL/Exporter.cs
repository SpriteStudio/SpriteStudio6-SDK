/**
	SpriteStudio6 Converter Ver.2

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
/* ========================================================================== */
/* UnityでいうScriptableObjectのようなものでデータをバイナリ保存するための機  */
/* 能を持ったクラスです。                                                     */
/* 原則として、出力対象になっているクラスは本クラスを継承して・機能関数を実装 */
/* してください。                                                             */
/*                                                                            */
/* ※元のソースに直接継承を加えるとUnityとのソース共有なども面倒になるため、  */
/*   対象となるクラス群を本ソース内で部分実装（partialの一部）をしています。  */
/* ========================================================================== */
using System;
using System.Collections.Generic;
using System.Text;

namespace SS6ConverterVer2_DLL
{
	/* ----------------------------------------------- Enums & Constants */
	#region Enums & Constants
	#endregion Enums & Constants

	/* ----------------------------------------------- Classes, Structs & Interfaces */
	#region Classes, Structs & Interfaces
	public class ChunkExport
	{
		/* ----------------------------------------------- Variables & Properties */
		#region Variables & Properties
		public int Offset;								/* データオフセット *//* ※原則先頭からのチャンクの開始絶対アドレス */
		public List<byte> Data;							/* チャンクのデータ列 *//* ※最終的にはこのデータをToArrayで静的配列化します。 */
		#endregion Variables & Properties

		/* ----------------------------------------------- Functions */
		#region Functions
		#endregion Functions

		/* ----------------------------------------------- Operators & Casts */
		#region Operators & Casts
		#endregion Operators & Casts

		/* ----------------------------------------------- Enums & Constants */
		#region Enums & Constants
		#endregion Enums & Constants

		/* ----------------------------------------------- Classes, Structs & Interfaces */
		#region Classes, Structs & Interfaces
		#endregion Classes, Structs & Interfaces

		/* ----------------------------------------------- Delegate types */
		#region Delegates
		#endregion Delegates
	}

	public interface InterfaceExporter
	{
		/* ----------------------------------------------- Variables & Properties */
		#region Variables & Properties
		#endregion Variables & Properties

		/* ----------------------------------------------- Functions */
		#region Functions
		bool BinaryGet(ref int sizeData, ChunkExport chunkData, ChunkExport chunkString);
		#endregion Functions

		/* ----------------------------------------------- Operators & Casts */
		#region Operators & Casts
		#endregion Operators & Casts

		/* ----------------------------------------------- Enums & Constants */
		#region Enums & Constants
		#endregion Enums & Constants

		/* ----------------------------------------------- Classes, Structs & Interfaces */
		#region Classes, Structs & Interfaces
		#endregion Classes, Structs & Interfaces

		/* ----------------------------------------------- Delegate types */
		#region Delegates
		#endregion Delegates
	}
	#endregion Classes, Structs & Interfaces

	/* ----------------------------------------------- Delegate types */
	#region Delegates
	#endregion Delegates
}	/* SS6ConverterVer2_DLL */

public static partial class Library_SpriteStudio6
{
	/* ----------------------------------------------- Variables & Properties */
	#region Variables & Properties
	#endregion Variables & Properties

	/* ----------------------------------------------- Functions */
	#region Functions
	#endregion Functions

	/* ----------------------------------------------- Operators & Casts */
	#region Operators & Casts
	#endregion Operators & Casts

	/* ----------------------------------------------- Enums & Constants */
	#region Enums & Constants
	#endregion Enums & Constants

	/* ----------------------------------------------- Classes, Structs & Interfaces */
	#region Classes, Structs & Interfaces
	public static partial class Data
	{
		/* ----------------------------------------------- Classes, Structs & Interfaces */
		#region Classes, Structs & Interfaces
		public partial class Animation
		{
			/* ----------------------------------------------- Variables & Properties */
			#region Variables & Properties
			#endregion Variables & Properties

			/* ----------------------------------------------- Functions */
			#region Functions
			#endregion Functions

			/* ----------------------------------------------- Enums & Constants */
			#region Enums & Constants
			#endregion Enums & Constants

			/* ----------------------------------------------- Classes, Structs & Interfaces */
			#region Classes, Structs & Interfaces
			#endregion Classes, Structs & Interfaces
		}

		public static partial class Effect
		{
			/* ----------------------------------------------- Classes, Structs & Interfaces */
			#region Classes, Structs & Interfaces
			public partial struct Emitter
			{
				/* ----------------------------------------------- Variables & Properties */
				#region Variables & Properties
				#endregion Variables & Properties

				/* ----------------------------------------------- Functions */
				#region Functions
				#endregion Functions

				/* ----------------------------------------------- Enums & Constants */
				#region Enums & Constants
				#endregion Enums & Constants

				/* ----------------------------------------------- Classes, Structs & Interfaces */
				#region Classes, Structs & Interfaces
				#endregion Classes, Structs & Interfaces
			}
			#endregion Classes, Structs & Interfaces
		}

#if false
		public partial class CellMap : SS6ConverterVer2_DLL.InterfaceExporter
		{
			/* ----------------------------------------------- Variables & Properties */
			#region Variables & Properties
			#endregion Variables & Properties

			/* ----------------------------------------------- Functions */
			#region Functions
			public bool BinaryGet(ref int sizeData, SS6ConverterVer2_DLL.ChunkExport chunkData, SS6ConverterVer2_DLL.ChunkExport chunkString)
			{
				if(null != chunkData)
				{
				}
//			public Vector2 SizeOriginal;
//			public Cell[] TableCell;
				
			}
			private int BinaryGetCell(ref int sizeData, SS6ConverterVer2_DLL.ChunkExport chunkData, SS6ConverterVer2_DLL.ChunkExport chunkString)
			{
//			public Cell[] TableCell;
			}
			#endregion Functions

			/* ----------------------------------------------- Classes, Structs & Interfaces */
			#region Classes, Structs & Interfaces
			#endregion Classes, Structs & Interfaces
		}
#endif
		#endregion Classes, Structs & Interfaces
	}
	#endregion Classes, Structs & Interfaces

	/* ----------------------------------------------- Delegate types */
	#region Delegates
	#endregion Delegates
}
