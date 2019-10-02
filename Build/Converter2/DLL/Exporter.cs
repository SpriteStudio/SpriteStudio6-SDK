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

/* MEMO: 一旦メモ的に書きかけです。 */
#if false
namespace SS6ConverterVer2_DLL
{
	/* ----------------------------------------------- Enums & Constants */
	#region Enums & Constants
	#endregion Enums & Constants

	/* ----------------------------------------------- Classes, Structs & Interfaces */
	#region Classes, Structs & Interfaces
	/* エクスポート基底インタフェース
	/* MEMO: バイナリ出力でしか使わないのでは？……という微妙な予想もありつつ。 */
	public interface InterfaceExporter
	{
		/* ----------------------------------------------- Variables & Properties */
		#region Variables & Properties
		#endregion Variables & Properties

		/* ----------------------------------------------- Functions */
		#region Functions
//		bool BinaryGet(ref int sizeData, ChunkExport chunkData, ChunkExport chunkString);
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

	/* バイナリエクスポート時専用定義・処理関係 */
	public static class ExportBinary
	{
		/* ----------------------------------------------- Variables & Properties */
		#region Variables & Properties
		#endregion Variables & Properties

		/* ----------------------------------------------- Functions */
		#region Functions
		#endregion Functions

		/* ----------------------------------------------- Enums & Constants */
		#region Enums & Constants
//		public static bool BinaryGetInt(ref int sizeData, SS6ConverterVer2_DLL.ChunkExport chunkData, SS6ConverterVer2_DLL.ChunkExport chunkString, int value)
//		{
//			if(null != chunkData)
//			{
//				byte[] data = System.BitConverter.GetBytes(value);
//				chunkData.Data.AddRange(data);
//			}
//			sizeData += 4;
//
//			return(true);
//		}
		#endregion Enums & Constants

		/* ----------------------------------------------- Classes, Structs & Interfaces */
		#region Classes, Structs & Interfaces
		public class Work
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
			public class Chunk
			{
				/* ----------------------------------------------- Variables & Properties */
				#region Variables & Properties
				public int Offset;								/* データオフセット *//* ※原則先頭からのチャンクの開始絶対アドレス */
				public List<byte> Data;							/* チャンクのデータ列 *//* ※最終的にはこのデータをToArrayで静的配列化します。 */
				public int SizeData
				{
					get
					{
						return((null == Data) ? -1 : Data.Count);
					}
				}
				#endregion Variables & Properties

				/* ----------------------------------------------- Functions */
				#region Functions
				public Chunk()
				{
					CleanUp();
				}

				public void CleanUp()
				{
					Offset = 0;
					Data = null;
				}

				public bool BootUp()
				{
					CleanUp();
					Data = new List<byte>();
					if(null == Data)
					{
						return(false);
					}
					Data.Clear();

					return(true);
				}
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
		}
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
				if(false == SizeOriginal.BinaryGet(ref sizeData, chunkData, chunkString))
				{
					return(false);
				}

				/* セル数を出力 */
				/* MEMO: 必要があればこれもメンバ変数に持つこと。 */
				int countCell = TableCell.Length;
				if(false == BinaryGetInt(ref sizeData, chunkData, chunkString, countCell))
				{
					return(false);
				}

				/* セルを出力 */
				for(int i=0; i<countCell; i++)
				{
					if(false == BinaryGetCell(ref sizeData, chunkData, chunkString, ))
				}

//			public Vector2 SizeOriginal;
//			public Cell[] TableCell;
				
			}
			private bool BinaryGetCell(ref int sizeData, SS6ConverterVer2_DLL.ChunkExport chunkData, SS6ConverterVer2_DLL.ChunkExport chunkString)
			{
//			public Cell[] TableCell;
				public string Name;
				public Rect Rectangle;
				public Vector2 Pivot;
				public DataMesh Mesh;
					public Vector2[] TableCoordinate;
					public int[] TableIndexVertex;
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

namespace SS6ConverterVer2_DLL
{
namespace Types
{
	/* MEMO: ここに記述するのは、エクスポート関係だけにして、本体機能は書かない。 */
	public partial struct Vector2
	{
		/* ----------------------------------------------- Functions */
		#region Functions
		public bool BinaryGet(ref int sizeData, SS6ConverterVer2_DLL.ChunkExport chunkData, SS6ConverterVer2_DLL.ChunkExport chunkString)
		{
			if(null != chunkData)
			{
				byte[] data = System.BitConverter.GetBytes(x);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			if(null != chunkData)
			{
				byte[] data = System.BitConverter.GetBytes(y);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			return(true);
		}
		#endregion Functions

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

	/* 構造体: 3次元ベクトル */
	public partial struct Vector3
	{
		/* ----------------------------------------------- Functions */
		#region Functions
		public bool BinaryGet(ref int sizeData, SS6ConverterVer2_DLL.ChunkExport chunkData, SS6ConverterVer2_DLL.ChunkExport chunkString)
		{
			if(null != chunkData)
			{
				byte[] data = System.BitConverter.GetBytes(x);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			if(null != chunkData)
			{
				byte[] data = System.BitConverter.GetBytes(y);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			if(null != chunkData)
			{
				byte[] data = System.BitConverter.GetBytes(z);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			return(true);
		}
		#endregion Functions
	}

	/* 構造体: 4次元ベクトル */
	public partial struct Vector4
	{
		/* ----------------------------------------------- Functions */
		#region Functions
		public bool BinaryGet(ref int sizeData, SS6ConverterVer2_DLL.ChunkExport chunkData, SS6ConverterVer2_DLL.ChunkExport chunkString)
		{
			if(null != chunkData)
			{
				byte[] data = System.BitConverter.GetBytes(x);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			if(null != chunkData)
			{
				byte[] data = System.BitConverter.GetBytes(y);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			if(null != chunkData)
			{
				byte[] data = System.BitConverter.GetBytes(z);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			if(null != chunkData)
			{
				byte[] data = System.BitConverter.GetBytes(w);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			return(true);
		}
		#endregion Functions
	}

	/* 構造体: 4元数 */
	public partial struct Quaternion
	{
		/* ----------------------------------------------- Functions */
		#region Functions
		public bool BinaryGet(ref int sizeData, SS6ConverterVer2_DLL.ChunkExport chunkData, SS6ConverterVer2_DLL.ChunkExport chunkString)
		{
			if(null != chunkData)
			{
				byte[] data = System.BitConverter.GetBytes(x);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			if(null != chunkData)
			{
				byte[] data = System.BitConverter.GetBytes(y);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			if(null != chunkData)
			{
				byte[] data = System.BitConverter.GetBytes(z);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			if(null != chunkData)
			{
				byte[] data = System.BitConverter.GetBytes(w);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			return(true);
		}
		#endregion Functions
	}

	/* 構造体: 4x4行列 */
	public partial struct Matrix4x4
	{

		/* ----------------------------------------------- Functions */
		#region Functions
		/* MEMO: 今のところ出力する予定がないので、BinaryGet関数は積んでいません。 */
		#endregion Functions
	}

	/* 構造体: 矩形 */
	public partial struct Rect
	{
		/* ----------------------------------------------- Functions */
		#region Functions
		public bool BinaryGet(ref int sizeData, SS6ConverterVer2_DLL.ChunkExport chunkData, SS6ConverterVer2_DLL.ChunkExport chunkString)
		{
			/* MEMO: 出力時はx/y/width/heightで出しています。 */

			if(null != chunkData)
			{
//				byte[] data = System.BitConverter.GetBytes(xMin);
				byte[] data = System.BitConverter.GetBytes(x);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			if(null != chunkData)
			{
//				byte[] data = System.BitConverter.GetBytes(yMin);
				byte[] data = System.BitConverter.GetBytes(y);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			if(null != chunkData)
			{
//				byte[] data = System.BitConverter.GetBytes(xMax);
				byte[] data = System.BitConverter.GetBytes(width);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			if(null != chunkData)
			{
//				byte[] data = System.BitConverter.GetBytes(yMax);
				byte[] data = System.BitConverter.GetBytes(height);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			return(true);
		}
		#endregion Functions
	}

	/* 構造体: 正規化色 */
	public partial struct Color
	{
		/* ----------------------------------------------- Functions */
		#region Functions
		public bool BinaryGet(ref int sizeData, SS6ConverterVer2_DLL.ChunkExport chunkData, SS6ConverterVer2_DLL.ChunkExport chunkString)
		{
			if(null != chunkData)
			{
				byte[] data = System.BitConverter.GetBytes(r);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			if(null != chunkData)
			{
				byte[] data = System.BitConverter.GetBytes(g);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			if(null != chunkData)
			{
				byte[] data = System.BitConverter.GetBytes(b);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			if(null != chunkData)
			{
				byte[] data = System.BitConverter.GetBytes(a);
				chunkData.Data.AddRange(data);
			}
			sizeData += 4;

			return(true);
		}
		#endregion Functions
	}

	/* 構造体: A8R8G8B8色 */
	public partial struct Color32
	{
		/* ----------------------------------------------- Functions */
		#region Functions
		/* MEMO: argbの32bitで出そうにもuintになるしなぁ……。 */
		#endregion Functions
	}
}	/* Types */
}	/* SS6ConverterVer2_DLL */
#endif
