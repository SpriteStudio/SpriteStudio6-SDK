/**
	SpriteStudio6 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
// #define ATTRIBUTE_DUPLICATE_DEEP

using System.Collections;
using System.Collections.Generic;
#if !_FOR_SS6CONVERTER_
using UnityEngine;
#else
using SS6ConverterVer2_DLL;
using SS6ConverterVer2_DLL.Types;
using UnityEngine = SS6ConverterVer2_DLL.Assets;
#endif

public static partial class Library_SpriteStudio6
{
	public static partial class Data
	{
		public partial class Animation
		{
			public static partial class Attribute
			{
				/* ----------------------------------------------- Enums & Constants */
				#region Enums & Constants
				public readonly static Color ColorClear = new Color(0.0f, 0.0f, 0.0f, 0.0f);

				/* Default values when no Key-Data exists */
				public const bool DefaultFlipX = false;
				public const bool DefaultFlipY = false;
				public const bool DefaultHide = false;
				public readonly static Status DefaultStatus = new Status(Status.FlagBit.CLEAR);

				public readonly static Cell DefaultCell = new Cell(-1, -1);

				public const float DefaultPositionX = 0.0f;
				public const float DefaultPositionY = 0.0f;
				public const float DefaultPositionZ = 0.0f;
				public readonly static Vector3 DefaultPosition = Vector3.zero;
				public const float DefaultRotationX = 0.0f;
				public const float DefaultRotationY = 0.0f;
				public const float DefaultRotationZ = 0.0f;
				public readonly static Vector3 DefaultRotation = Vector3.zero;
				public const float DefaultScalingX = 1.0f;
				public const float DefaultScalingY = 1.0f;
				public const float DefaultScalingZ = 1.0f;
				public readonly static Vector2 DefaultScaling = Vector2.one;

				public const float DefaultRateOpacity = 1.0f;
				public const float DefaultPriority = 0.0f;

				private readonly static Vector2[] TableCoordinateVertexCorrectionDefault = new Vector2[(int)Library_SpriteStudio6.KindVertex.TERMINATOR2]
				{
					Vector2.zero,
					Vector2.zero,
					Vector2.zero,
					Vector2.zero,
				};
				public readonly static VertexCorrection DefaultVertexCorrection = new VertexCorrection(TableCoordinateVertexCorrectionDefault);
				private readonly static Color[] TableVertexColorPartsColorDefault = new Color[(int)Library_SpriteStudio6.KindVertex.TERMINATOR2]
				{
					Library_SpriteStudio6.Data.Animation.Attribute.ColorClear,
					Library_SpriteStudio6.Data.Animation.Attribute.ColorClear,
					Library_SpriteStudio6.Data.Animation.Attribute.ColorClear,
					Library_SpriteStudio6.Data.Animation.Attribute.ColorClear,
				};
				internal readonly static float[] TableRateAlphaPartsColorDefault = new float[(int)Library_SpriteStudio6.KindVertex.TERMINATOR2]
				{
					1.0f,
					1.0f,
					1.0f,
					1.0f,
				};
				public readonly static PartsColor DefaultPartsColor = new PartsColor(	Library_SpriteStudio6.KindBoundBlend.NON,
																						Library_SpriteStudio6.KindOperationBlend.MIX,
																						TableVertexColorPartsColorDefault,
																						TableRateAlphaPartsColorDefault
																					);

				public const float DefaultPivotOffsetX = 0.0f;
				public const float DefaultPivotOffsetY = 0.0f;
				public readonly static Vector2 DefaultPivotOffset = Vector2.zero;

				public const float DefaultAnchorPositionX = 0.0f;
				public const float DefaultAnchorPositionY = 0.0f;
				public readonly static Vector2 DefaultAnchorPoint = Vector2.zero;

				public const float DefaultSizeForceX = 1.0f;
				public const float DefaultSizeForceY = 1.0f;
				public readonly static Vector2 DefaultSizeForce = Vector2.one;

				public const float DefaultTexturePositionX = 0.0f;
				public const float DefaultTexturePositionY = 0.0f;
				public readonly static Vector2 DefaultTexturePosition = Vector2.zero;

				public const float DefaultTextureRotation = 0.0f;
				public const float DefaultTextureScalingX = 1.0f;
				public const float DefaultTextureScalingY = 1.0f;
				public readonly static Vector2 DefaultTextureScaling = Vector2.one;
				public const bool DefaultTextureFlipX = false;
				public const bool DefaultTextureFlipY = false;

				public const float DefaultRadiusCollision = 0.0f;

				public readonly static UserData DefaultUseData = new UserData(UserData.FlagBit.CLEAR, 0, Rect.zero, Vector2.zero, "");

				public readonly static Instance DefaultInstance = new Instance(Instance.FlagBit.CLEAR, 0, 1.0f, 0, 0, "", "");

				public readonly static Effect DefaultEffect = new Effect(Effect.FlagBit.CLEAR, 0, 1.0f);

				public readonly static Deform DefaultDeform = new Deform(new Vector2[0]);
				#endregion Enums & Constants

				/* ----------------------------------------------- Classes, Structs & Interfaces */
				#region Classes, Structs & Interfaces
				[System.Serializable]
				public struct Status
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public FlagBit Flags;

					public bool IsValid
					{
						get
						{
							return(0 != (Flags & FlagBit.VALID));
						}
					}
					public bool IsHide
					{
						get
						{
							return(0 != (Flags & FlagBit.HIDE));
						}
					}
					public bool IsFlipX
					{
						get
						{
							return(0 != (Flags & FlagBit.FLIP_X));
						}
					}
					public bool IsFlipY
					{
						get
						{
							return(0 != (Flags & FlagBit.FLIP_Y));
						}
					}
					public bool IsTextureFlipX
					{
						get
						{
							return(0 != (Flags & FlagBit.FLIP_TEXTURE_X));
						}
					}
					public bool IsTextureFlipY
					{
						get
						{
							return(0 != (Flags & FlagBit.FLIP_TEXTURE_Y));
						}
					}
					public int IDPartsNextDraw
					{
						get
						{
							FlagBit data = Flags & FlagBit.ID_PARTS_NEXTDRAW;
//							return((FlagBit.ID_PARTS_DRAWNEXT == data) ? (-1) : (int)data >> (int)ShiftFlagBit.ID_PARTS_NEXTDRAW);
							return((FlagBit.ID_PARTS_NEXTDRAW == data) ? (-1) : (int)data);
						}
					}
					public int IDPartsNextPreDraw
					{
						get
						{
							FlagBit data = Flags & FlagBit.ID_PARTS_NEXTPREDRAW;
							return((FlagBit.ID_PARTS_NEXTPREDRAW == data) ? (-1) : (int)data >> (int)ShiftFlagBit.ID_PARTS_NEXTPREDRAW);
						}
					}
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public Status(FlagBit flags)
					{
						Flags = flags;
					}

					public void CleanUp()
					{
						Flags = FlagBit.CLEAR;
					}

					public void Duplicate(Status original)
					{
						Flags = original.Flags;
					}

					public override bool Equals(System.Object target)
					{
						if((null == target) || (GetType() != target.GetType()))
						{
							return(false);
						}

						Status targetData = (Status)target;
						return((Flags & FlagBit.MASK) == (targetData.Flags & FlagBit.MASK));
					}

					public override int GetHashCode()
					{
						return(base.GetHashCode());
					}
					#endregion Functions

					/* ----------------------------------------------- Enums & Constants */
					#region Enums & Constants
					[System.Flags]
					public enum FlagBit
					{
						VALID = 0x40000000,
						HIDE = 0x20000000,	/* Store as assistant data for skip useless processing. */

						FLIP_X = 0x08000000,
						FLIP_Y = 0x04000000,
						FLIP_TEXTURE_X = 0x02000000,
						FLIP_TEXTURE_Y = 0x01000000,

						ID_PARTS_NEXTDRAW = 0x00000fff,
						ID_PARTS_NEXTPREDRAW = 0x00fff000,

						CLEAR = 0x00000000,
						MASK = (VALID | HIDE | FLIP_X | FLIP_Y | FLIP_TEXTURE_X | FLIP_TEXTURE_Y | ID_PARTS_NEXTPREDRAW | ID_PARTS_NEXTDRAW),
						INITIAL = (VALID | ID_PARTS_NEXTDRAW | ID_PARTS_NEXTPREDRAW),
					}

					public enum ShiftFlagBit
					{
						ID_PARTS_NEXTDRAW = 0,
						ID_PARTS_NEXTPREDRAW = 12,
					}
					#endregion Enums & Constants
				}

				[System.Serializable]
				public struct Cell
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public int IndexCellMap;
					public int IndexCell;
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public Cell(int indexCellMap, int indexCell)
					{
						IndexCellMap = indexCellMap;
						IndexCell = indexCell;
					}

					public void CleanUp()
					{
						IndexCellMap = -1;
						IndexCell = -1;
					}

					public void Duplicate(Cell original)
					{
						IndexCellMap = original.IndexCellMap;
						IndexCell = original.IndexCell;
					}

					public override bool Equals(System.Object target)
					{
						if((null == target) || (GetType() != target.GetType()))
						{
							return(false);
						}

						Cell targetData = (Cell)target;
						return((IndexCellMap == targetData.IndexCellMap) && (IndexCell == targetData.IndexCell));
					}

					public override int GetHashCode()
					{
						return(base.GetHashCode());
					}
					#endregion Functions
				}

				[System.Serializable]
				public struct VertexCorrection
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public Vector2[] Coordinate;
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public VertexCorrection(Vector2[] coordinate)
					{
						Coordinate = coordinate;
					}

					public void CleanUp()
					{
						Coordinate = null;
					}

					public void BootUp()
					{
						int count = (int)Library_SpriteStudio6.KindVertex.TERMINATOR2;
						Coordinate = new Vector2[count];
						for(int i=0; i<count; i++)
						{
							Coordinate[i] = Vector2.zero;
						}
					}

					public void Duplicate(VertexCorrection original)
					{
#if ATTRIBUTE_DUPLICATE_DEEP
						/* MEMO: Deep copy */
						for(int i=0; i<Coordinate.Length; i++)
						{
							Coordinate[i] = original.Coordinate[i];
						}
#else
						/* MEMO: Shallow copy */
						Coordinate = original.Coordinate;
#endif
					}

					public override bool Equals(System.Object target)
					{
						if((null == target) || (GetType() != target.GetType()))
						{
							return(false);
						}

						VertexCorrection targetData = (VertexCorrection)target;
						int count = Coordinate.Length;
						if(count != targetData.Coordinate.Length)
						{
							return(false);
						}
						for(int i=0; i<count; i++)
						{
							if(Coordinate[i] != targetData.Coordinate[i])
							{
								return(false);
							}
						}
						return(true);
					}

					public override int GetHashCode()
					{
						return(base.GetHashCode());
					}
					#endregion Functions
				}

				[System.Serializable]
				public struct PartsColor
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public Library_SpriteStudio6.KindBoundBlend Bound;
					public Library_SpriteStudio6.KindOperationBlend Operation;
					public Color[] VertexColor;
					public float[] RateAlpha;
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public PartsColor(	Library_SpriteStudio6.KindBoundBlend bound,
										Library_SpriteStudio6.KindOperationBlend operation,
										Color[] vertexColor,
										float[] rateAlpha
									)
					{
						Bound = bound;
						Operation = operation;
						VertexColor = vertexColor;
						RateAlpha = rateAlpha;
					}

					public void CleanUp()
					{
						Bound = Library_SpriteStudio6.KindBoundBlend.NON;
						Operation = Library_SpriteStudio6.KindOperationBlend.MIX;
						VertexColor = null;
						RateAlpha = null;
					}

					public void BootUp(int countVertex)
					{
						Bound = Library_SpriteStudio6.KindBoundBlend.NON;
						Operation = Library_SpriteStudio6.KindOperationBlend.MIX;

						VertexColor = new Color[countVertex];
						for(int i=0; i<countVertex; i++)
						{
							VertexColor[i] = ColorClear;
						}

						RateAlpha = new float[countVertex];
						for(int i=0; i<countVertex; i++)
						{
							RateAlpha[i] = 1.0f;
						}
					}

					public void Duplicate(PartsColor original)
					{
						Bound = original.Bound;
						Operation = original.Operation;
#if ATTRIBUTE_DUPLICATE_DEEP
						/* MEMO: Deep copy */
						int countVertex = original.VertexColor.Length;
						if((null == VertexColor) || (VertexColor.Length != countVertex))
						{
							VertexColor = new Color[countVertex];
						}
						for(int i=0; i<countVertex; i++)
						{
							VertexColor[i] = original.VertexColor[i];
						}

						if((null == RateAlpha) || (RateAlpha.Length != countVertex))
						{
							RateAlpha = new float[countVertex];
						}
						for(int i=0; i<countVertex; i++)
						{
							RateAlpha[i] = original.RateAlpha[i];
						}
#else
						/* MEMO: Shallow copy */
						VertexColor = original.VertexColor;
						RateAlpha = original.RateAlpha;
#endif
					}

					public override bool Equals(System.Object target)
					{
						if((null == target) || (GetType() != target.GetType()))
						{
							return(false);
						}

						PartsColor targetData = (PartsColor)target;

						if((Bound != targetData.Bound) || (Operation != targetData.Operation))
						{
							return(false);
						}

						int countVertex = VertexColor.Length;
						if(VertexColor.Length != targetData.VertexColor.Length)
						{
							return(false);
						}
						for(int i=0; i<countVertex; i++)
						{
							if(VertexColor[i] != targetData.VertexColor[i])
							{
								return(false);
							}
						}

						countVertex = RateAlpha.Length;
						if(RateAlpha.Length != targetData.RateAlpha.Length)
						{
							return(false);
						}
						for(int i=0; i<countVertex; i++)
						{
							if(RateAlpha[i] != targetData.RateAlpha[i])
							{
								return(false);
							}
						}
						return(true);
					}

					public override int GetHashCode()
					{
						return(base.GetHashCode());
					}
					#endregion Functions
				}

				[System.Serializable]
				public struct UserData
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public FlagBit Flags;
					public int NumberInt;
					public Rect Rectangle;
					public Vector2 Coordinate;
					public string Text;

					public bool IsValid
					{
						get
						{
							return(0 != (Flags & FlagBit.VALID));
						}
					}
					public bool IsNumber
					{
						get
						{
							return(0 != (Flags & FlagBit.NUMBER));
						}
					}
					public bool IsRectangle
					{
						get
						{
							return(0 != (Flags & FlagBit.RECTANGLE));
						}
					}
					public bool IsCoordinate
					{
						get
						{
							return(0 != (Flags & FlagBit.COORDINATE));
						}
					}
					public bool IsText
					{
						get
						{
							return(0 != (Flags & FlagBit.TEXT));
						}
					}
					public uint Number
					{
						get
						{
							return((uint)NumberInt);
						}
					}
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public UserData(FlagBit flags, int numberInt, Rect rectangle, Vector2 coordinate, string text)
					{
						Flags = flags;
						NumberInt = numberInt;
						Rectangle = rectangle;
						Coordinate = coordinate;
						Text = text;
					}

					public void CleanUp()
					{
						Flags = FlagBit.CLEAR;
						NumberInt = 0;
						Rectangle = Rect.zero;
						Coordinate = Vector2.zero;
						Text = "";
					}

					public void Duplicate(UserData original)
					{
#if ATTRIBUTE_DUPLICATE_DEEP
						/* MEMO: Deep copy */
						Flags = original.Flags;
						NumberInt = original.NumberInt;
						Rectangle = original.Rectangle;
						Coordinate = original.Coordinate;
						Text = (true == string.IsNullOrEmpty(original.Text)) ? "" : string.Copy(original.Text);
#else
						/* MEMO: Shallow copy */
						Flags = original.Flags;
						NumberInt = original.NumberInt;
						Rectangle = original.Rectangle;
						Coordinate = original.Coordinate;
						Text = (true == string.IsNullOrEmpty(original.Text)) ? "" : original.Text;
#endif
					}

					public override bool Equals(System.Object target)
					{
						if((null == target) || (GetType() != target.GetType()))
						{
							return(false);
						}

						UserData targetData = (UserData)target;
						return(	(Flags == targetData.Flags)
								&& (NumberInt == targetData.NumberInt)
								&& (Rectangle == targetData.Rectangle)
								&& (Coordinate == targetData.Coordinate)
								&& (Text == targetData.Text)
							);
					}

					public override int GetHashCode()
					{
						return(base.GetHashCode());
					}
					#endregion Functions

					/* ----------------------------------------------- Enums & Constants */
					#region Enums & Constants
					[System.Flags]
					public enum FlagBit
					{
						VALID = 0x40000000,

						COORDINATE = 0x00000004,
						TEXT = 0x00000008,
						RECTANGLE = 0x00000002,
						NUMBER = 0x00000001,

						CLEAR = 0x00000000,
					}
					#endregion Enums & Constants
				}

				[System.Serializable]
				public struct Instance
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public FlagBit Flags;
					public int PlayCount;
					public float RateTime;
					public int OffsetStart;
					public int OffsetEnd;
					public string LabelStart;
					public string LabelEnd;

					public bool IsValid
					{
						get
						{
							return(0 != (Flags & FlagBit.VALID));
						}
					}
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public Instance(	FlagBit flags,
										int playCount,
										float rateTime,
										int offsetStart,
										int offsetEnd,
										string labelStart,
										string labelEnd
									)
					{
						Flags = flags;
						PlayCount = playCount;
						RateTime = rateTime;
						OffsetStart = offsetStart;
						OffsetEnd = offsetEnd;
						LabelStart = labelStart;
						LabelEnd = labelEnd;
					}

					public void CleanUp()
					{
						Flags = FlagBit.CLEAR;
						PlayCount = 1;
						RateTime = 1.0f;
						OffsetStart = 0;
						OffsetEnd = 0;
						LabelStart = "";
						LabelEnd = "";
					}

					public void Duplicate(Instance original)
					{
#if ATTRIBUTE_DUPLICATE_DEEP
						/* MEMO: Deep copy */
						Flags = original.Flags;
						PlayCount = original.PlayCount;
						RateTime = original.RateTime;
						OffsetStart = original.OffsetStart;
						OffsetEnd = original.OffsetEnd;
						LabelStart = string.Copy(original.LabelStart);
						LabelEnd = string.Copy(original.LabelEnd);
#else
						/* MEMO: Shallow copy */
						Flags = original.Flags;
						PlayCount = original.PlayCount;
						RateTime = original.RateTime;
						OffsetStart = original.OffsetStart;
						OffsetEnd = original.OffsetEnd;
						LabelStart = original.LabelStart;
						LabelEnd = original.LabelEnd;
#endif
					}

					public override bool Equals(System.Object target)
					{
						if((null == target) || (GetType() != target.GetType()))
						{
							return(false);
						}

						Instance targetData = (Instance)target;
						return(	(Flags == targetData.Flags)
									&& (PlayCount == targetData.PlayCount)
									&& (RateTime == targetData.RateTime)
									&& (OffsetStart == targetData.OffsetStart)
									&& (OffsetEnd == targetData.OffsetEnd)
									&& (LabelStart == targetData.LabelStart)
									&& (LabelEnd == targetData.LabelEnd)
							);
					}

					public override int GetHashCode()
					{
						return(base.GetHashCode());
					}
					#endregion Functions

					/* ----------------------------------------------- Enums & Constants */
					#region Enums & Constants
					[System.Flags]
					public enum FlagBit
					{
						VALID = 0x40000000,

						INDEPENDENT = 0x00000002,
						PINGPONG = 0x00000001,

						CLEAR = 0x00000000,
					}
					#endregion Enums & Constants
				}

				[System.Serializable]
				public struct Effect
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public FlagBit Flags;
					public int FrameStart;
					public float RateTime;

					public bool IsValid
					{
						get
						{
							return(0 != (Flags & FlagBit.VALID));
						}
					}
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public Effect(FlagBit flags, int frameStart, float rateTime)
					{
						Flags = flags;
						FrameStart = frameStart;
						RateTime = rateTime;
					}

					public void CleanUp()
					{
						Flags = FlagBit.CLEAR;
						FrameStart = 0;
						RateTime = 1.0f;
					}

					public void Duplicate(Effect original)
					{
						Flags = original.Flags;
						FrameStart = original.FrameStart;
						RateTime = original.RateTime;
					}

					public override bool Equals(System.Object target)
					{
						if((null == target) || (GetType() != target.GetType()))
						{
							return(false);
						}

						Effect targetData = (Effect)target;
						return(	(Flags == targetData.Flags)
								&& (FrameStart == targetData.FrameStart)
								&& (RateTime == targetData.RateTime)
							);
					}

					public override int GetHashCode()
					{
						return(base.GetHashCode());
					}
					#endregion Functions

					/* ----------------------------------------------- Enums & Constants */
					#region Enums & Constants
					[System.Flags]
					public enum FlagBit
					{
						VALID = 0x40000000,

						INDEPENDENT = 0x00000002,
						PINGPONG = 0x00000001,	/* (Reserved) */

						CLEAR = 0x00000000,
					}
					#endregion Enums & Constants
				}

				[System.Serializable]
				public struct Deform
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public Vector2[] TableCoordinate;

					public bool IsValid
					{
						get
						{
							return(!((null == TableCoordinate) || (0 >= TableCoordinate.Length)));	/* ((null == TableCoordinate) || (0 >= TableCoordinate.Length)) ? false : true */
						}
					}
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public Deform(Vector2[] tableCoordinate)
					{
						TableCoordinate = tableCoordinate;
					}

					public void CleanUp()
					{
						TableCoordinate = null;
					}

					public bool BootUp(int countVertex)
					{
						if(0 > countVertex)
						{
							CleanUp();
						}
						else
						{
							TableCoordinate = new Vector2[countVertex];
							if(null == TableCoordinate)
							{
								return(false);
							}

							for(int i=0; i<countVertex; i++)
							{
								TableCoordinate[i] = Vector2.zero;
							}
						}
						return(true);
					}

					public void Duplicate(Deform original)
					{
#if ATTRIBUTE_DUPLICATE_DEEP
						/* MEMO: Deep copy */
						if(null == original.TableCoordinate)
						{
							TableCoordinate = null;
						}
						else
						{
							int countVertex = original.TableCoordinate.Length;
							TableCoordinate = new Vector2[countVertex];
							for(int i=0; i<countVertex; i++)
							{
								TableCoordinate[i] = original.TableCoordinate[i];
							}
						}
#else
						/* MEMO: Shallow copy */
						TableCoordinate = original.TableCoordinate;
#endif
					}

					public override bool Equals(System.Object target)
					{
						if((null == target) || (GetType() != target.GetType()))
						{
							return(false);
						}

						Deform targetData = (Deform)target;
						if(null == TableCoordinate)
						{
							if((null == targetData.TableCoordinate) || (0 == targetData.TableCoordinate.Length))
							{
								return(true);
							}
							return(false);
						}
						if(null == targetData.TableCoordinate)
						{
							if(0 == TableCoordinate.Length)
							{
								return(true);
							}
						}

						int countVertex = TableCoordinate.Length;
						if(countVertex != targetData.TableCoordinate.Length)
						{
							return(false);
						}

						for(int i=0; i<countVertex; i++)
						{
							if(TableCoordinate[i] != targetData.TableCoordinate[i])
							{
								return(false);
							}
						}
						return(true);
					}

					public override int GetHashCode()
					{
						return(base.GetHashCode());
					}

					public void CoordinateReset()
					{
						if(null != TableCoordinate)
						{
							int count = TableCoordinate.Length;
							for(int i=0; i<count; i++)
							{
								TableCoordinate[i] = Vector2.zero;
							}
						}
					}
					#endregion Functions

					/* ----------------------------------------------- Enums & Constants */
					#region Enums & Constants
					#endregion Enums & Constants
				}
				#endregion Classes, Structs & Interfaces
			}
		}
	}
}
