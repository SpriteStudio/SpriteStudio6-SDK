/**
	SpriteStudio6 Converter Ver.2

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
/* ========================================================================== */
/* Unityからソースを持ってくる上でのUnity独自の基礎データ型（ベクタなど）の互 */
/* 換定義関係です（完全に移植しているというわけではありません）。             */
/*                                                                            */
/* 差替用なので、（Typesが静的クラスでなく）using定義するために名前空間になっ */
/* ています。                                                                 */
/* ========================================================================== */
using System;
using System.Collections.Generic;
using System.Text;

namespace SS6ConverterVer2_DLL
{
namespace Types
{
	/* ----------------------------------------------- Enums & Constants */
	#region Enums & Constants
	#endregion Enums & Constants

	/* ----------------------------------------------- Classes, Structs & Interfaces */
	#region Classes, Structs & Interfaces
	/* 構造体: 2次元ベクトル */
	public partial struct Vector2
	{
		/* ----------------------------------------------- Variables & Properties */
		#region Variables & Properties
		public float x;
		public float y;

		public float this[int index]
		{
			get
			{
				/* MEMO: ちょっと暴力的だが、ランタイムとかでないので大丈夫かな？と。 */
				switch(index)
				{
					case 0:
						return(x);

					case 1:
						return(y);

					default:
						break;
				}

				return(float.NaN);	/* エラー */
			}
			set
			{
				/* MEMO: ちょっと暴力的だが、ランタイムとかでないので大丈夫かな？と。 */
				switch(index)
				{
					case 0:
						x = value;
						break;

					case 1:
						y = value;
						break;

					default:
						break;
				}
			}
		}
		public override int GetHashCode()
		{
			return(	x.GetHashCode()
					^ y.GetHashCode()
				);
		}
		#endregion Variables & Properties

		/* ----------------------------------------------- Functions */
		#region Functions
		public Vector2(float _x, float _y)
		{
			x = _x;
			y = _y;
		}

		public override bool Equals(object obj)
		{
			/* MEMO: 構造体なので省略判定しています。 */
			if(false == (obj is Vector2))
			{
				return(false);
			}

			Vector2 rhs = (Vector2)obj;
			return(	(x == rhs.x)
					&& (y == rhs.y)
				);	/* ? true : false */
		}
		#endregion Functions

		/* ----------------------------------------------- Operators & Casts */
		#region Operators & Casts
		public static bool operator ==(Vector2 lhs, Vector2 rhs)
		{
			/* MEMO: .NETは浮動小数の等号比較は（精度的に）大丈夫なはずだが……ダメなら範囲判定に変えること */
			return(	(lhs.x == rhs.x)
					&& (lhs.y == rhs.y)
				);	/* ? true : false */
		}
		public static bool operator !=(Vector2 lhs, Vector2 rhs)
		{
			return(!(lhs == rhs));	/* ? false : true */
		}

		public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
		{
			return(new Vector2(	(lhs.x + rhs.x),
								(lhs.y + rhs.y)
							)
				);
		}
		public static Vector2 operator -(Vector2 lhs, Vector2 rhs)
		{
			return(new Vector2(	(lhs.x - rhs.x),
								(lhs.y - rhs.y)
							)
				);
		}
		public static Vector2 operator *(Vector2 lhs, float rhs)
		{
			return(new Vector2(	(lhs.x * rhs),
								(lhs.y * rhs)
							)
				);
		}
		#endregion Operators & Casts

		/* ----------------------------------------------- Enums & Constants */
		#region Enums & Constants
		/* 定義済定数群 */
		public readonly static Vector2 zero = new Vector2(0.0f, 0.0f);
		public readonly static Vector2 one = new Vector2(1.0f, 1.0f);
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
		/* ----------------------------------------------- Variables & Properties */
		#region Variables & Properties
		public float x;
		public float y;
		public float z;

		public float this[int index]
		{
			get
			{
				/* MEMO: ちょっと暴力的だが、ランタイムとかでないので大丈夫かな？と。 */
				switch(index)
				{
					case 0:
						return(x);

					case 1:
						return(y);

					case 2:
						return(z);

					default:
						break;
				}

				return(float.NaN);	/* エラー */
			}
			set
			{
				/* MEMO: ちょっと暴力的だが、ランタイムとかでないので大丈夫かな？と。 */
				switch(index)
				{
					case 0:
						x = value;
						break;

					case 1:
						y = value;
						break;

					case 2:
						z = value;
						break;

					default:
						break;
				}
			}
		}
		#endregion Variables & Properties

		/* ----------------------------------------------- Functions */
		#region Functions
		public Vector3(float _x, float _y, float _z)
		{
			x = _x;
			y = _y;
			z = _z;
		}

		public override bool Equals(object obj)
		{
			/* MEMO: 構造体なので省略判定しています。 */
			if(false == (obj is Vector3))	{
				return(false);
			}

			Vector3 rhs = (Vector3)obj;
			return(	(x == rhs.x)
					&& (y == rhs.y)
					&& (z == rhs.z)
				);	/* ? true : false */
		}
		public override int GetHashCode()
		{
			/* MEMO: Unityの実装は下記のはずだが……とりあえず暫定でXORだけしておく（Vector3だけ正しくても仕様がないし、エミュレーションライブラリ作ってるわけでなし） */
//			return(	x.GetHashCode()
//					^ y.GetHashCode() << 2
//					^ z.GetHashCode() >> 2
//				);
			return(	x.GetHashCode()
					^ y.GetHashCode()
					^ z.GetHashCode()
				);
		}
		#endregion Functions

		/* ----------------------------------------------- Operators & Casts */
		#region Operators & Casts
		public static bool operator ==(Vector3 lhs, Vector3 rhs)
		{
			/* MEMO: .NETは浮動小数の等号比較は（精度的に）大丈夫なはずだが……ダメなら範囲判定に変えること */
			return(	(lhs.x == rhs.x)
					&& (lhs.y == rhs.y)
					&& (lhs.z == rhs.z)
				);	/* ? true : false */
		}
		public static bool operator !=(Vector3 lhs, Vector3 rhs)
		{
			return(!(lhs == rhs));	/* ? false : true */
		}

		public static Vector3 operator +(Vector3 lhs, Vector3 rhs)
		{
			return(new Vector3(	(lhs.x + rhs.x),
								(lhs.y + rhs.y),
								(lhs.z + rhs.z)
							)
				);
		}
		public static Vector3 operator -(Vector3 lhs, Vector3 rhs)
		{
			return(new Vector3(	(lhs.x - rhs.x),
								(lhs.y - rhs.y),
								(lhs.z - rhs.z)
							)
				);
		}
		public static Vector3 operator *(Vector3 lhs, float rhs)
		{
			return(new Vector3(	(lhs.x * rhs),
								(lhs.y * rhs),
								(lhs.z * rhs)
							)
				);
		}
		#endregion Operators & Casts

		/* ----------------------------------------------- Enums & Constants */
		#region Enums & Constants
		/* 定義済定数群 */
		public readonly static Vector3 zero = new Vector3(0.0f, 0.0f, 0.0f);
		public readonly static Vector3 one = new Vector3(1.0f, 1.0f, 1.0f);
		#endregion Enums & Constants

		/* ----------------------------------------------- Classes, Structs & Interfaces */
		#region Classes, Structs & Interfaces
		#endregion Classes, Structs & Interfaces

		/* ----------------------------------------------- Delegate types */
		#region Delegates
		#endregion Delegates
	}

	/* 構造体: 4次元ベクトル */
	public partial struct Vector4
	{
		/* ----------------------------------------------- Variables & Properties */
		#region Variables & Properties
		public float x;
		public float y;
		public float z;
		public float w;

		public float this[int index]
		{
			get
			{
				/* MEMO: ちょっと暴力的だが、ランタイムとかでないので大丈夫かな？と。 */
				switch(index)
				{
					case 0:
						return(x);

					case 1:
						return(y);

					case 2:
						return(z);

					case 3:
						return(w);

					default:
						break;
				}

				return(float.NaN);	/* エラー */
			}
			set
			{
				/* MEMO: ちょっと暴力的だが、ランタイムとかでないので大丈夫かな？と。 */
				switch(index)
				{
					case 0:
						x = value;
						break;

					case 1:
						y = value;
						break;

					case 2:
						z = value;
						break;

					case 3:
						w = value;
						break;

					default:
						break;
				}
			}
		}
		#endregion Variables & Properties

		/* ----------------------------------------------- Functions */
		#region Functions
		public Vector4(float _x, float _y, float _z, float _w)
		{
			x = _x;
			y = _y;
			z = _z;
			w = _w;
		}

		public override bool Equals(object obj)
		{
			/* MEMO: 構造体なので省略判定しています。 */
			if(false == (obj is Vector4))	{
				return(false);
			}

			Vector4 rhs = (Vector4)obj;
			return(	(x == rhs.x)
					&& (y == rhs.y)
					&& (z == rhs.z)
					&& (w == rhs.w)
				);	/* ? true : false */
		}
		public override int GetHashCode()
		{
			return(	x.GetHashCode()
					^ y.GetHashCode()
					^ z.GetHashCode()
					^ w.GetHashCode()
				);
		}
		#endregion Functions

		/* ----------------------------------------------- Operators & Casts */
		#region Operators & Casts
		public static bool operator ==(Vector4 lhs, Vector4 rhs)
		{
			/* MEMO: .NETは浮動小数の等号比較は（精度的に）大丈夫なはずだが……ダメなら範囲判定に変えること */
			return(	(lhs.x == rhs.x)
					&& (lhs.y == rhs.y)
					&& (lhs.z == rhs.z)
					&& (lhs.w == rhs.w)
				);	/* ? true : false */
		}
		public static bool operator !=(Vector4 lhs, Vector4 rhs)
		{
			return(!(lhs == rhs));	/* ? false : true */
		}

		public static Vector4 operator +(Vector4 lhs, Vector4 rhs)
		{
			return(new Vector4(	(lhs.x + rhs.x),
								(lhs.y + rhs.y),
								(lhs.z + rhs.z),
								(lhs.w + rhs.w)
							)
				);
		}
		public static Vector4 operator -(Vector4 lhs, Vector4 rhs)
		{
			return(new Vector4(	(lhs.x - rhs.x),
								(lhs.y - rhs.y),
								(lhs.z - rhs.z),
								(lhs.w - rhs.w)
							)
				);
		}
		public static Vector4 operator *(Vector4 lhs, float rhs)
		{
			return(new Vector4(	(lhs.x * rhs),
								(lhs.y * rhs),
								(lhs.z * rhs),
								(lhs.w * rhs)
							)
				);
		}
		#endregion Operators & Casts

		/* ----------------------------------------------- Enums & Constants */
		#region Enums & Constants
		/* 定義済定数群 */
		public readonly static Vector4 zero = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
		public readonly static Vector4 one = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
		#endregion Enums & Constants

		/* ----------------------------------------------- Classes, Structs & Interfaces */
		#region Classes, Structs & Interfaces
		#endregion Classes, Structs & Interfaces

		/* ----------------------------------------------- Delegate types */
		#region Delegates
		#endregion Delegates
	}

	/* 構造体: 4元数 */
	public partial struct Quaternion
	{
		/* ----------------------------------------------- Variables & Properties */
		#region Variables & Properties
		public float x;
		public float y;
		public float z;
		public float w;

		public float this[int index]
		{
			get
			{
				/* MEMO: ちょっと暴力的だが、ランタイムとかでないので大丈夫かな？と。 */
				switch(index)
				{
					case 0:
						return(x);

					case 1:
						return(y);

					case 2:
						return(z);

					case 3:
						return(w);

					default:
						break;
				}

				return(float.NaN);	/* エラー */
			}
			set
			{
				/* MEMO: ちょっと暴力的だが、ランタイムとかでないので大丈夫かな？と。 */
				switch(index)
				{
					case 0:
						x = value;
						break;

					case 1:
						y = value;
						break;

					case 2:
						z = value;
						break;

					case 3:
						w = value;
						break;

					default:
						break;
				}
			}
		}
		#endregion Variables & Properties

		/* ----------------------------------------------- Functions */
		#region Functions
		public Quaternion(float _x, float _y, float _z, float _w)
		{
			x = _x;
			y = _y;
			z = _z;
			w = _w;
		}

		public override bool Equals(object obj)
		{
			/* MEMO: 構造体なので省略判定しています。 */
			if(false == (obj is Vector4))	{
				return(false);
			}

			Quaternion rhs = (Quaternion)obj;
			return(	(x == rhs.x)
					&& (y == rhs.y)
					&& (z == rhs.z)
					&& (w == rhs.w)
				);	/* ? true : false */
		}
		public override int GetHashCode()
		{
			return(	x.GetHashCode()
					^ y.GetHashCode()
					^ z.GetHashCode()
					^ w.GetHashCode()
				);
		}
		#endregion Functions

		/* ----------------------------------------------- Operators & Casts */
		#region Operators & Casts
		public static bool operator ==(Quaternion lhs, Quaternion rhs)
		{
			/* MEMO: .NETは浮動小数の等号比較は（精度的に）大丈夫なはずだが……ダメなら範囲判定に変えること */
			return(	(lhs.x == rhs.x)
					&& (lhs.y == rhs.y)
					&& (lhs.z == rhs.z)
					&& (lhs.w == rhs.w)
				);	/* ? true : false */
		}
		public static bool operator !=(Quaternion lhs, Quaternion rhs)
		{
			return(!(lhs == rhs));	/* ? false : true */
		}

		/* MEMO: 4元数の演算子は一旦オーバーライドしていません（元々+と==しかオーバーライドしていないはず）。 */
		#endregion Operators & Casts

		/* ----------------------------------------------- Enums & Constants */
		#region Enums & Constants
		/* 定義済定数群 */
		#endregion Enums & Constants

		/* ----------------------------------------------- Classes, Structs & Interfaces */
		#region Classes, Structs & Interfaces
		#endregion Classes, Structs & Interfaces

		/* ----------------------------------------------- Delegate types */
		#region Delegates
		#endregion Delegates
	}

	/* 構造体: 4x4行列 */
	public partial struct Matrix4x4
	{
		/* ----------------------------------------------- Variables & Properties */
		#region Variables & Properties
		/* MEMO: 配列でとるとコンストラクト時に確保できないので、バラで確保しています。 */
		private Vector4 column0;
		private Vector4 column1;
		private Vector4 column2;
		private Vector4 column3;

		public float this[int index]
		{
			get
			{
				int indexColumn = index / 4;
				int indexRow = index % 4;
				return(this[indexColumn, indexRow]);
			}
			set
			{
				int indexColumn = index / 4;
				int indexRow = index % 4;
				this[indexColumn, indexRow] = value;
			}
		}
		public float this[int indexColumn, int indexRow]
		{
			get
			{
				/* MEMO: ちょっと暴力的だが、ランタイムとかでないので大丈夫かな？と。 */
				switch(indexColumn)
				{
					case 0:
						return(column0[indexRow]);

					case 1:
						return(column1[indexRow]);

					case 2:
						return(column2[indexRow]);

					case 3:
						return(column3[indexRow]);

					default:
						break;
				}

				return(float.NaN);	/* エラー */
			}
			set
			{
				/* MEMO: ちょっと暴力的だが、ランタイムとかでないので大丈夫かな？と。 */
				switch(indexRow)
				{
					case 0:
						column0[indexRow] = value;
						break;

					case 1:
						column1[indexRow] = value;
						break;

					case 2:
						column2[indexRow] = value;
						break;

					case 3:
						column3[indexRow] = value;
						break;

					default:
						break;
				}
			}
		}

		public Matrix4x4 transpose
		{
			get
			{
				Matrix4x4 rv = new Matrix4x4();

				/* MEMO: 単なる転置なんでザラ書きしてます。 */
				rv.column0.x = column0.x;
				rv.column0.y = column1.x;
				rv.column0.z = column2.x;
				rv.column0.w = column3.x;

				rv.column1.x = column0.y;
				rv.column1.y = column1.y;
				rv.column1.z = column2.y;
				rv.column1.w = column3.y;

				rv.column2.x = column0.z;
				rv.column2.y = column1.z;
				rv.column2.z = column2.z;
				rv.column2.w = column3.z;

				rv.column3.x = column0.w;
				rv.column3.y = column1.w;
				rv.column3.z = column2.w;
				rv.column3.w = column3.w;

				return(rv);
			}
		}
		#endregion Variables & Properties

		/* ----------------------------------------------- Functions */
		#region Functions
		public Matrix4x4(Vector4 _column0, Vector4 _column1, Vector4 _column2, Vector4 _column3)
		{
			column0 = _column0;
			column1 = _column1;
			column2 = _column2;
			column3 = _column3;
		}

		public override bool Equals(object obj)
		{
			/* MEMO: 構造体なので省略判定しています。 */
			if(false == (obj is Matrix4x4))	{
				return(false);
			}

			Matrix4x4 rhs = (Matrix4x4)obj;
			return(	(column0 == rhs.column0)
					&& (column1 == rhs.column1)
					&& (column2 == rhs.column2)
					&& (column3 == rhs.column3)
				);	/* ? true : false */
		}
		public override int GetHashCode()
		{
			return(	column0.GetHashCode()
					^ column0.GetHashCode()
					^ column0.GetHashCode()
					^ column0.GetHashCode()
				);
		}

		public void SetTRS(Vector3 pos, Quaternion q, Vector3 s)
		{
			this  = identity;	/*	単位行列化 */

			/* MEMO: この順序で良いのかは考えること。 */
			QuaternionSetRotate(ref q);		/* 回転 */
			column0.x *= s.x;				/* スケール */
			column1.y *= s.y;
			column2.z *= s.z;
			VectorSetTranslate(ref pos);	/* 平行移動 */
		}
		private void QuaternionSetRotate(ref Quaternion q)
		{
			/* MEMO: 転置しているかもしれない。 */
			column0.x = 1.0f - (2.0f * q.y * q.y) - (2.0f * q.z * q.z);
			column0.y = (2.0f * q.x * q.y) + (2.0f * q.w * q.z);
			column0.z = (2.0f * q.x * q.z) - (2.0f * q.w * q.y);

			column1.x = (2.0f * q.x * q.y) - (2.0f * q.w * q.z);
			column1.y = 1.0f - (2.0f * q.x * q.x) - (2.0f * q.z * q.z);
			column1.z = (2.0f * q.y * q.z) + (2.0f * q.w * q.x);

			column2.x = (2.0f * q.x * q.z) + (2.0f * q.w * q.y);
			column2.y = (2.0f * q.y * q.z) - (2.0f * q.w * q.x);
			column2.z = 1.0f - (2.0f * q.x * q.x) - (2.0f * q.y * q.y);
		}
		private void VectorSetTranslate(ref Vector3 pos)
		{
			/* MEMO: 入れている位置が間違えてるかもしれない（column3.xyz）に入れなきゃいけないかも。 */
			column0.w = pos.x;
			column1.w = pos.y;
			column2.w = pos.z;
		}
		#endregion Functions

		/* ----------------------------------------------- Operators & Casts */
		#region Operators & Casts
		public readonly static Matrix4x4 zero = new Matrix4x4(Vector4.zero, Vector4.zero, Vector4.zero, Vector4.zero);
		public readonly static Matrix4x4 identity = new Matrix4x4(	new Vector4(1.0f, 0.0f, 0.0f, 0.0f),
																	new Vector4(0.0f, 1.0f, 0.0f, 0.0f),
																	new Vector4(0.0f, 0.0f, 1.0f, 0.0f),
																	new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
																);
		#endregion Operators & Casts

		/* ----------------------------------------------- Enums & Constants */
		#region Enums & Constants
		/* 定義済定数群 */
		#endregion Enums & Constants

		/* ----------------------------------------------- Classes, Structs & Interfaces */
		#region Classes, Structs & Interfaces
		#endregion Classes, Structs & Interfaces

		/* ----------------------------------------------- Delegate types */
		#region Delegates
		#endregion Delegates
	}

	/* 構造体: 矩形 */
	public partial struct Rect
	{
		/* ----------------------------------------------- Variables & Properties */
		#region Variables & Properties
		public float xMin;
		public float yMin;
		public float xMax;
		public float yMax;

		public float x
		{
			get
			{
				return(xMin);
			}
			set
			{
				xMin = value;
			}
		}
		public float y
		{
			get
			{
				return(yMin);
			}
			set
			{
				yMin = value;
			}
		}
		public float width
		{
			get
			{
				/* MEMO: 確か絶対値でよかったよな…… */
				return(Math.Abs(xMax - xMin));
			}
			set
			{
				xMax = value - xMin;
			}
		}
		public float height
		{
			get
			{
				/* MEMO: 確か絶対値でよかったよな…… */
				return(Math.Abs(yMax - yMin));
			}
			set
			{
				yMax = value - yMin;
			}
		}
		#endregion Variables & Properties

		/* ----------------------------------------------- Functions */
		#region Functions
		public Rect(float _x, float _y, float _width, float _height)
		{
			xMin = _x;
			yMin = _y;
			xMax = Math.Abs(_width - _x);
			yMax = Math.Abs(_height - _y);
		}

		public override bool Equals(object obj)
		{
			/* MEMO: 構造体なので省略判定しています。 */
			if(false == (obj is Rect))	{
				return(false);
			}

			Rect rhs = (Rect)obj;
			return(	(xMin == rhs.xMin)
					&& (yMin == rhs.yMin)
					&& (xMax == rhs.xMax)
					&& (yMax == rhs.yMax)
				);	/* ? true : false */
		}
		public override int GetHashCode()
		{
			return(	xMin.GetHashCode()
					^ yMin.GetHashCode()
					^ xMax.GetHashCode()
					^ yMax.GetHashCode()
				);
		}
		#endregion Functions

		/* ----------------------------------------------- Operators & Casts */
		#region Operators & Casts
		public static bool operator ==(Rect lhs, Rect rhs)
		{
			/* MEMO: .NETは浮動小数の等号比較は（精度的に）大丈夫なはずだが……ダメなら範囲判定に変えること */
			return(	(lhs.xMin == rhs.xMin)
					&& (lhs.xMax == rhs.xMax)
					&& (lhs.yMin == rhs.yMin)
					&& (lhs.yMax == rhs.yMax)
				);	/* ? true : false */
		}
		public static bool operator !=(Rect lhs, Rect rhs)
		{
			return(!(lhs == rhs));	/* ? false : true */
		}
		#endregion Operators & Casts

		/* ----------------------------------------------- Enums & Constants */
		#region Enums & Constants
		/* 定義済定数群 */
		public readonly static Rect zero = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
		#endregion Enums & Constants

		/* ----------------------------------------------- Classes, Structs & Interfaces */
		#region Classes, Structs & Interfaces
		#endregion Classes, Structs & Interfaces

		/* ----------------------------------------------- Delegate types */
		#region Delegates
		#endregion Delegates
	}

	/* 構造体: 正規化色 */
	public partial struct Color
	{
		/* ----------------------------------------------- Variables & Properties */
		#region Variables & Properties
		public float r;
		public float g;
		public float b;
		public float a;
		#endregion Variables & Properties

		/* ----------------------------------------------- Functions */
		#region Functions
		public Color(float _r, float _g, float _b, float _a)
		{
			r = _r;
			g = _g;
			b = _b;
			a = _a;
		}

		public override bool Equals(object obj)
		{
			/* MEMO: 構造体なので省略判定しています。 */
			if(false == (obj is Color))	{
				return(false);
			}

			Color rhs = (Color)obj;
			return(	(r == rhs.r)
					&& (g == rhs.g)
					&& (b == rhs.b)
					&& (a == rhs.a)
				);	/* ? true : false */
		}
		public override int GetHashCode()
		{
			return(	r.GetHashCode()
					^ g.GetHashCode()
					^ b.GetHashCode()
					^ a.GetHashCode()
				);
		}
		#endregion Functions

		/* ----------------------------------------------- Operators & Casts */
		#region Operators & Casts
		public static implicit operator Color32(Color value)
		{
			/* MEMO: 現状ちょっと投げやりな変換式。          */
			/*       Color32.Color()側と併せて変更すること。 */
			return(new Color32(	(byte)(value.r * 255.0f),
								(byte)(value.g * 255.0f),
								(byte)(value.b * 255.0f),
								(byte)(value.a * 255.0f)
							)
				);
		}

		public static bool operator ==(Color lhs, Color rhs)
		{
			/* MEMO: .NETは浮動小数の等号比較は（精度的に）大丈夫なはずだが……ダメなら範囲判定に変えること */
			return(	(lhs.r == rhs.r)
					&& (lhs.g == rhs.g)
					&& (lhs.b == rhs.b)
					&& (lhs.a == rhs.a)
				);	/* ? true : false */
		}
		public static bool operator !=(Color lhs, Color rhs)
		{
			return(!(lhs == rhs));	/* ? false : true */
		}

		public static Color operator +(Color lhs, Color rhs)
		{
			return(new Color(	(lhs.r + rhs.r),
								(lhs.g + rhs.g),
								(lhs.b + rhs.b),
								(lhs.a + rhs.a)
							)
				);
		}
		public static Color operator -(Color lhs, Color rhs)
		{
			return(new Color(	(lhs.r - rhs.r),
								(lhs.g - rhs.g),
								(lhs.b - rhs.b),
								(lhs.a - rhs.a)
							)
				);
		}
		public static Color operator *(Color lhs, float rhs)
		{
			return(new Color(	(lhs.r * rhs),
								(lhs.g * rhs),
								(lhs.b * rhs),
								(lhs.a * rhs)
							)
				);
		}
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

	/* 構造体: A8R8G8B8色 */
	public partial struct Color32
	{
		/* ----------------------------------------------- Variables & Properties */
		#region Variables & Properties
		public byte r;
		public byte g;
		public byte b;
		public byte a;
		#endregion Variables & Properties

		/* ----------------------------------------------- Functions */
		#region Functions
		public Color32(byte _r, byte _g, byte _b, byte _a)
		{
			r = _r;
			g = _g;
			b = _b;
			a = _a;
		}

		public override bool Equals(object obj)
		{
			/* MEMO: 構造体なので省略判定しています。 */
			if(false == (obj is Color))	{
				return(false);
			}

			Color rhs = (Color)obj;
			return(	(r == rhs.r)
					&& (g == rhs.g)
					&& (b == rhs.b)
					&& (a == rhs.a)
				);	/* ? true : false */
		}
		public override int GetHashCode()
		{
			return(	r.GetHashCode()
					^ g.GetHashCode()
					^ b.GetHashCode()
					^ a.GetHashCode()
				);
		}

		/* MEMO: argbの32bitで出そうにもuintになるしなぁ……。 */
		#endregion Functions

		/* ----------------------------------------------- Operators & Casts */
		#region Operators & Casts
		public static implicit operator Color(Color32 value)
		{
			/* MEMO: 現状ちょっと投げやりな変換式。          */
			/*       Color.Color32()側と併せて変更すること。 */
			return(new Color(	((float)value.r / 255.0f),
								((float)value.g / 255.0f),
								((float)value.b / 255.0f),
								((float)value.a / 255.0f)
							)
				);
		}

		public static bool operator ==(Color32 lhs, Color32 rhs)
		{
			return(	(lhs.r == rhs.r)
					&& (lhs.g == rhs.g)
					&& (lhs.b == rhs.b)
					&& (lhs.a == rhs.a)
				);	/* ? true : false */
		}
		public static bool operator !=(Color32 lhs, Color32 rhs)
		{
			return(!(lhs == rhs));	/* ? false : true */
		}

		/* MEMO: Color32は中身がbyteなので演算子オーバーライドしていません（元々やれなかったはず） */
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

	/* クラス: スクリプタブルオブジェクト（シリアライズするデータの基底） */
	/* MEMO: 実質、これは未使用と考えてよい（単に継承記述維持のためだけに存在）。 */
	public class ScriptableObject
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

		/* ----------------------------------------------- Delegate types */
		#region Delegates
		#endregion Delegates
	}
	#endregion Classes, Structs & Interfaces

	/* ----------------------------------------------- Delegate types */
	#region Delegates
	#endregion Delegates
}	/* Types */
}   /* SS6ConverterVer2_DLL */
