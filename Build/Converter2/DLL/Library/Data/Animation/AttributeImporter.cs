/**
	SpriteStudio6 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
// #define ATTRIBUTE_DUPLICATE_DEEP
#define CHANGE_DEFORM_DECODING

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
				/* ----------------------------------------------- Classes, Structs & Interfaces */
				#region Classes, Structs & Interfaces
				/* MEMO: Use this class only in Editor (only when "UNITY_EDITOR" is defined).                                               */
				/*       Written here to make simple "PackAttribute" section, but originally should be written in the assembly for Editor.  */
				/*       Therefore, presence of this class is not mentioned in "Libray_SpriteStudio.cs". (Intentionally hidden)             */
				public static partial class Importer
				{
					/* ----------------------------------------------- Enums & Constants */
					#region Enums & Constants
					public const string NameAttributeStatus = "Status";
					public const string NameAttributeCell = "Cell";
					public const string NameAttributePosition = "Position";
					public const string NameAttributeRotation = "Rotation";
					public const string NameAttributeScaling = "Scaling";
					public const string NameAttributeScalingLocal = "ScalingLocal";
					public const string NameAttributeRateOpacity = "RateOpacity";
					public const string NameAttributeRateOpacityLocal = "RateOpacityLocal";
					public const string NameAttributePowerMask = "PowerMask";
					public const string NameAttributePriority = "Priority";
					public const string NameAttributePartsColor = "PartsColor";
					public const string NameAttributeVertexCorrection = "VertexCorrection";
					public const string NameAttributeOffsetPivot = "OffsetPivot";
					public const string NameAttributePositionAnchor = "PositionAnchor";
					public const string NameAttributeSizeForce = "SizeForce";
					public const string NameAttributePositionTexture = "PositionTexture";
					public const string NameAttributeRotationTexture = "RotationTexture";
					public const string NameAttributeScalingTexture = "ScalingTexture";
					public const string NameAttributeRadiusCollision = "RadiusCollision";
					public const string NameAttributeUserData = "UserData";
					public const string NameAttributeInstance = "Instance";
					public const string NameAttributeEffect = "Effect";
					public const string NameAttributeDeform = "Deform";
					#endregion Enums & Constants

					/* ----------------------------------------------- Classes, Structs & Interfaces */
					#region Classes, Structs & Interfaces
					public class AttributeBool : Attribute<bool>
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public override bool ValueGet(out bool valueOutput, int frame)
						{
							int indexStart = IndexGetFramePrevious(frame);
							if(0 <= indexStart)
							{
								valueOutput = ListKey[indexStart].Value;
								return(true);
							}

							valueOutput = false;
							return(false);
						}
						#endregion Functions
					}
					public class AttributeInt : Attribute<int>
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public override bool ValueGet(out int valueOutput, int frame)
						{
							int indexStart = IndexGetFramePrevious(frame);
							int indexEnd = IndexGetFrameNext(frame);

							if(0 > frame)
							{
								goto ValueGet_ErrorEnd;
							}
							if(0 > indexStart)
							{	/* Front blank */
								if(0 > indexEnd)
								{	/* No Key */
									goto ValueGet_ErrorEnd;
								}
								valueOutput = ListKey[indexEnd].Value;
								return(true);
							}
							else
							{
								if(0 > indexEnd)
								{	/* End Blank */
									valueOutput = ListKey[indexStart].Value;
									return(true);
								}
							}

							float value = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																										frame,
																										ListKey[indexStart].Frame,
																										(float)ListKey[indexStart].Value,
																										ListKey[indexEnd].Frame,
																										(float)ListKey[indexEnd].Value,
																										ListKey[indexStart].FrameCurveStart,
																										ListKey[indexStart].ValueCurveStart,
																										ListKey[indexStart].FrameCurveEnd,
																										ListKey[indexStart].ValueCurveEnd
																									);
							valueOutput = (int)value;
							return(true);

						ValueGet_ErrorEnd:;
							valueOutput = 0;

							return(false);
						}
						#endregion Functions
					}
					public class AttributeFloat : Attribute<float>
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public override bool ValueGet(out float valueOutput, int frame)
						{
							int indexStart = IndexGetFramePrevious(frame);
							int indexEnd = IndexGetFrameNext(frame);

							if(0 > frame)
							{
								goto ValueGet_ErrorEnd;
							}
							if(0 > indexStart)
							{	/* Front blank */
								if(0 > indexEnd)
								{	/* No Key */
									goto ValueGet_ErrorEnd;
								}
								valueOutput = ListKey[indexEnd].Value;
								return(true);
							}
							else
							{
								if(0 > indexEnd)
								{	/* End Blank */
									valueOutput = ListKey[indexStart].Value;
									return(true);
								}
							}

							valueOutput = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																										frame,
																										ListKey[indexStart].Frame,
																										ListKey[indexStart].Value,
																										ListKey[indexEnd].Frame,
																										ListKey[indexEnd].Value,
																										ListKey[indexStart].FrameCurveStart,
																										ListKey[indexStart].ValueCurveStart,
																										ListKey[indexStart].FrameCurveEnd,
																										ListKey[indexStart].ValueCurveEnd
																									);
							return(true);

						ValueGet_ErrorEnd:;
							valueOutput = 0.0f;

							return(false);
						}
						#endregion Functions
					}
					public class AttributeVector2 : Attribute<Vector2>
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public override bool ValueGet(out Vector2 valueOutput, int frame)
						{
							int indexStart = IndexGetFramePrevious(frame);
							int indexEnd = IndexGetFrameNext(frame);

							if(0 > frame)
							{
								goto ValueGet_ErrorEnd;
							}
							if(0 > indexStart)
							{	/* Front blank */
								if(0 > indexEnd)
								{	/* No Key */
									goto ValueGet_ErrorEnd;
								}
								valueOutput = ListKey[indexEnd].Value;
								return(true);
							}
							else
							{
								if(0 > indexEnd)
								{	/* End Blank */
									valueOutput = ListKey[indexStart].Value;
									return(true);
								}
							}

							valueOutput.x = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																										frame,
																										ListKey[indexStart].Frame,
																										ListKey[indexStart].Value.x,
																										ListKey[indexEnd].Frame,
																										ListKey[indexEnd].Value.x,
																										ListKey[indexStart].FrameCurveStart,
																										ListKey[indexStart].ValueCurveStart,
																										ListKey[indexStart].FrameCurveEnd,
																										ListKey[indexStart].ValueCurveEnd
																									);
							valueOutput.y = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																										frame,
																										ListKey[indexStart].Frame,
																										ListKey[indexStart].Value.y,
																										ListKey[indexEnd].Frame,
																										ListKey[indexEnd].Value.y,
																										ListKey[indexStart].FrameCurveStart,
																										ListKey[indexStart].ValueCurveStart,
																										ListKey[indexStart].FrameCurveEnd,
																										ListKey[indexStart].ValueCurveEnd
																									);
							return(true);

						ValueGet_ErrorEnd:;
							valueOutput = Vector2.zero;
							return(false);
						}
						#endregion Functions
					}
					public class AttributeVector3 : Attribute<Vector3>
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public override bool ValueGet(out Vector3 valueOutput, int frame)
						{
							int indexStart = IndexGetFramePrevious(frame);
							int indexEnd = IndexGetFrameNext(frame);

							if(0 > frame)
							{
								goto ValueGet_ErrorEnd;
							}
							if(0 > indexStart)
							{	/* Front blank */
								if(0 > indexEnd)
								{	/* No Key */
									goto ValueGet_ErrorEnd;
								}
								valueOutput = ListKey[indexEnd].Value;
								return(true);
							}
							else
							{
								if(0 > indexEnd)
								{	/* End Blank */
									valueOutput = ListKey[indexStart].Value;
									return(true);
								}
							}

							valueOutput.x = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																										frame,
																										ListKey[indexStart].Frame,
																										ListKey[indexStart].Value.x,
																										ListKey[indexEnd].Frame,
																										ListKey[indexEnd].Value.x,
																										ListKey[indexStart].FrameCurveStart,
																										ListKey[indexStart].ValueCurveStart,
																										ListKey[indexStart].FrameCurveEnd,
																										ListKey[indexStart].ValueCurveEnd
																									);
							valueOutput.y = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																										frame,
																										ListKey[indexStart].Frame,
																										ListKey[indexStart].Value.y,
																										ListKey[indexEnd].Frame,
																										ListKey[indexEnd].Value.y,
																										ListKey[indexStart].FrameCurveStart,
																										ListKey[indexStart].ValueCurveStart,
																										ListKey[indexStart].FrameCurveEnd,
																										ListKey[indexStart].ValueCurveEnd
																									);
							valueOutput.z = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																										frame,
																										ListKey[indexStart].Frame,
																										ListKey[indexStart].Value.z,
																										ListKey[indexEnd].Frame,
																										ListKey[indexEnd].Value.z,
																										ListKey[indexStart].FrameCurveStart,
																										ListKey[indexStart].ValueCurveStart,
																										ListKey[indexStart].FrameCurveEnd,
																										ListKey[indexStart].ValueCurveEnd
																									);
							return(true);

						ValueGet_ErrorEnd:;
							valueOutput = Vector3.zero;
							return(false);
						}
						#endregion Functions
					}
					public class AttributeUserData : Attribute<Library_SpriteStudio6.Data.Animation.Attribute.UserData>
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public override bool ValueGet(out Library_SpriteStudio6.Data.Animation.Attribute.UserData valueOutput, int frame)
						{
							int count = (null != ListKey) ? ListKey.Count : 0;
							for(int i=0; i<count; i++)
							{
								if(ListKey[i].Frame == frame)
								{
									valueOutput = ListKey[i].Value;
									return(true);
								}
							}

							valueOutput = Library_SpriteStudio6.Data.Animation.Attribute.DefaultUseData;
							return(false);
						}
						#endregion Functions
					}
					public class AttributeCell : Attribute<Library_SpriteStudio6.Data.Animation.Attribute.Cell>
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public override bool ValueGet(out Library_SpriteStudio6.Data.Animation.Attribute.Cell valueOutput, int frame)
						{
							int indexStart = IndexGetFramePrevious(frame);
							int indexEnd = IndexGetFrameNext(frame);

							if(0 > frame)
							{
								goto ValueGet_ErrorEnd;
							}
							if(0 > indexStart)
							{	/* Front blank */
								if(0 > indexEnd)
								{	/* No Key */
									goto ValueGet_ErrorEnd;
								}
								valueOutput = ListKey[indexEnd].Value;
								return(true);
							}

							valueOutput = ListKey[indexStart].Value;
							return(true);

						ValueGet_ErrorEnd:;
							valueOutput = Library_SpriteStudio6.Data.Animation.Attribute.DefaultCell;
							return(false);
						}
						#endregion Functions
					}
					public class AttributePartsColor : Attribute<Library_SpriteStudio6.Data.Animation.Attribute.PartsColor>
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public override bool ValueGet(out Library_SpriteStudio6.Data.Animation.Attribute.PartsColor valueOutput, int frame)
						{
							int count = (int)Library_SpriteStudio6.KindVertex.TERMINATOR2;
							int indexStart = IndexGetFramePrevious(frame);
							int indexEnd = IndexGetFrameNext(frame);

							if(0 > frame)
							{
								goto ValueGet_ErrorEnd;
							}
							if(0 > indexStart)
							{	/* Front blank */
								if(0 > indexEnd)
								{	/* No Key */
									goto ValueGet_ErrorEnd;
								}
								valueOutput = ListKey[indexEnd].Value;
								return(false);
							}
							else
							{
								if(0 > indexEnd)
								{	/* End Blank */
									valueOutput = ListKey[indexStart].Value;
									return(false);
								}
							}

							valueOutput.Bound = ListKey[indexStart].Value.Bound;
							valueOutput.Operation = ListKey[indexStart].Value.Operation;
							valueOutput.VertexColor = new Color[count];
							valueOutput.RateAlpha = new float[count];
							for(int i=0; i<count; i++)
							{
#if false
								/* MEMO: SpriteStudio Ver.5.0-5.2 */
								valueOutput.VertexColor[i].r = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																															frame,
																															ListKey[indexStart].Frame,
																															ListKey[indexStart].Value.VertexColor[i].r,
																															ListKey[indexEnd].Frame,
																															ListKey[indexEnd].Value.VertexColor[i].r,
																															ListKey[indexStart].FrameCurveStart,
																															ListKey[indexStart].ValueCurveStart,
																															ListKey[indexStart].FrameCurveEnd,
																															ListKey[indexStart].ValueCurveEnd
																														);
								valueOutput.VertexColor[i].g = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																															frame,
																															ListKey[indexStart].Frame,
																															ListKey[indexStart].Value.VertexColor[i].g,
																															ListKey[indexEnd].Frame,
																															ListKey[indexEnd].Value.VertexColor[i].g,
																															ListKey[indexStart].FrameCurveStart,
																															ListKey[indexStart].ValueCurveStart,
																															ListKey[indexStart].FrameCurveEnd,
																															ListKey[indexStart].ValueCurveEnd
																														);
								valueOutput.VertexColor[i].b = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																															frame,
																															ListKey[indexStart].Frame,
																															ListKey[indexStart].Value.VertexColor[i].b,
																															ListKey[indexEnd].Frame,
																															ListKey[indexEnd].Value.VertexColor[i].b,
																															ListKey[indexStart].FrameCurveStart,
																															ListKey[indexStart].ValueCurveStart,
																															ListKey[indexStart].FrameCurveEnd,
																															ListKey[indexStart].ValueCurveEnd
																														);
								valueOutput.VertexColor[i].a = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																															frame,
																															ListKey[indexStart].Frame,
																															ListKey[indexStart].Value.VertexColor[i].a,
																															ListKey[indexEnd].Frame,
																															ListKey[indexEnd].Value.VertexColor[i].a,
																															ListKey[indexStart].FrameCurveStart,
																															ListKey[indexStart].ValueCurveStart,
																															ListKey[indexStart].FrameCurveEnd,
																															ListKey[indexStart].ValueCurveEnd
																														);

								valueOutput.RateAlpha[i] = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																														frame,
																														ListKey[indexStart].Frame,
																														ListKey[indexStart].Value.RateAlpha[i],
																														ListKey[indexEnd].Frame,
																														ListKey[indexEnd].Value.RateAlpha[i],
																														ListKey[indexStart].FrameCurveStart,
																														ListKey[indexStart].ValueCurveStart,
																														ListKey[indexStart].FrameCurveEnd,
																														ListKey[indexStart].ValueCurveEnd
																													);
#else
								/* MEMO: SpriteStudio Ver.5.2- or Ver -4.x */
								float rate = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																										frame,
																										ListKey[indexStart].Frame,
																										0.0f,
																										ListKey[indexEnd].Frame,
																										1.0f,
																										ListKey[indexStart].FrameCurveStart,
																										ListKey[indexStart].ValueCurveStart,
																										ListKey[indexStart].FrameCurveEnd,
																										ListKey[indexStart].ValueCurveEnd
																									);
								rate = Mathf.Clamp01(rate);

								valueOutput.VertexColor[i].r = Library_SpriteStudio6.Utility.Interpolation.Linear(ListKey[indexStart].Value.VertexColor[i].r, ListKey[indexEnd].Value.VertexColor[i].r, rate);
								valueOutput.VertexColor[i].g = Library_SpriteStudio6.Utility.Interpolation.Linear(ListKey[indexStart].Value.VertexColor[i].g, ListKey[indexEnd].Value.VertexColor[i].g, rate);
								valueOutput.VertexColor[i].b = Library_SpriteStudio6.Utility.Interpolation.Linear(ListKey[indexStart].Value.VertexColor[i].b, ListKey[indexEnd].Value.VertexColor[i].b, rate);
								valueOutput.VertexColor[i].a = Library_SpriteStudio6.Utility.Interpolation.Linear(ListKey[indexStart].Value.VertexColor[i].a, ListKey[indexEnd].Value.VertexColor[i].a, rate);
								valueOutput.RateAlpha[i] = Library_SpriteStudio6.Utility.Interpolation.Linear(ListKey[indexStart].Value.RateAlpha[i], ListKey[indexEnd].Value.RateAlpha[i], rate);
#endif
							}
							return(true);

						ValueGet_ErrorEnd:;
							valueOutput = Library_SpriteStudio6.Data.Animation.Attribute.DefaultPartsColor;
							return(false);
						}
						#endregion Functions
					}
					public class AttributeVertexCorrection : Attribute<Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection>
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public override bool ValueGet(out Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection valueOutput, int frame)
						{	/* MEMO: This attribute does not consider inheritance. */
							int count = (int)Library_SpriteStudio6.KindVertex.TERMINATOR2;
							int indexStart = IndexGetFramePrevious(frame);
							int indexEnd = IndexGetFrameNext(frame);

							if(0 > frame)
							{
								goto ValueGet_ErrorEnd;
							}
							if(0 > indexStart)
							{	/* Front blank */
								if(0 > indexEnd)
								{	/* No Key */
									goto ValueGet_ErrorEnd;
								}
								valueOutput = ListKey[indexEnd].Value;
								return(false);
							}
							else
							{
								if(0 > indexEnd)
								{	/* End Blank */
									valueOutput = ListKey[indexStart].Value;
									return(false);
								}
							}

							valueOutput.Coordinate = new Vector2[count];
							for(int i=0; i<count; i++)
							{
								valueOutput.Coordinate[i].x = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																															frame,
																															ListKey[indexStart].Frame,
																															ListKey[indexStart].Value.Coordinate[i].x,
																															ListKey[indexEnd].Frame,
																															ListKey[indexEnd].Value.Coordinate[i].x,
																															ListKey[indexStart].FrameCurveStart,
																															ListKey[indexStart].ValueCurveStart,
																															ListKey[indexStart].FrameCurveEnd,
																															ListKey[indexStart].ValueCurveEnd
																														);
								valueOutput.Coordinate[i].y = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																															frame,
																															ListKey[indexStart].Frame,
																															ListKey[indexStart].Value.Coordinate[i].y,
																															ListKey[indexEnd].Frame,
																															ListKey[indexEnd].Value.Coordinate[i].y,
																															ListKey[indexStart].FrameCurveStart,
																															ListKey[indexStart].ValueCurveStart,
																															ListKey[indexStart].FrameCurveEnd,
																															ListKey[indexStart].ValueCurveEnd
																														);
							}
							return(true);

						ValueGet_ErrorEnd:;
							valueOutput = Library_SpriteStudio6.Data.Animation.Attribute.DefaultVertexCorrection;
							return(false);
						}
						#endregion Functions
					}
					public class AttributeInstance : Attribute<Library_SpriteStudio6.Data.Animation.Attribute.Instance>
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public override bool ValueGet(out Library_SpriteStudio6.Data.Animation.Attribute.Instance valueOutput, int frame)
						{
							int count = (null != ListKey) ? ListKey.Count : 0;
							for(int i=0; i<count; i++)
							{
								if(ListKey[i].Frame == frame)
								{
									valueOutput = ListKey[i].Value;
									return(true);
								}
							}

							valueOutput = Library_SpriteStudio6.Data.Animation.Attribute.DefaultInstance;
							return(false);
						}
						#endregion Functions
					}
					public class AttributeEffect : Attribute<Library_SpriteStudio6.Data.Animation.Attribute.Effect>
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public override bool ValueGet(out Library_SpriteStudio6.Data.Animation.Attribute.Effect valueOutput, int frame)
						{
							int count = (null != ListKey) ? ListKey.Count : 0;
							for(int i=0; i<count; i++)
							{
								if(ListKey[i].Frame == frame)
								{
									valueOutput = ListKey[i].Value;
									return(true);
								}
							}

							valueOutput = Library_SpriteStudio6.Data.Animation.Attribute.DefaultEffect;
							return(false);
						}
						#endregion Functions
					}

					public class AttributeDeform : Attribute<DataDeform>
					{
						/* MEMO: Attribute "Deform" does not use struct-for-runtime like other "Attribute" derived classes.   */
						/*       (Because how-to-store-data is different for importers and for runtime. This struct converted */
						/*        to "Library_SpriteStudio6.Data.Animation.Attribute.Deform" in "PackAttribute_Codec/*.cs".)  */
						/* ----------------------------------------------- Functions */
						#region Functions
						public override bool ValueGet(out DataDeform valueOutput, int frame)
						{
							int indexStart = IndexGetFramePrevious(frame);
							int indexEnd = IndexGetFrameNext(frame);

							if(0 > frame)
							{
								goto ValueGet_ErrorEnd;
							}
							if(0 > indexStart)
							{	/* Front blank */
								if(0 > indexEnd)
								{	/* No Key */
									goto ValueGet_ErrorEnd;
								}
								valueOutput = ListKey[indexEnd].Value;
								return(false);
							}
							else
							{
								if(0 > indexEnd)
								{	/* End Blank */
									valueOutput = ListKey[indexStart].Value;
									return(false);
								}
							}

							/* MEMO: "countVertexMesh" is the same for any key data. (The number of Mesh-Cell's vertices) */
							int countVertexMesh = ListKey[indexStart].Value.CountVertexMesh;
							valueOutput.CountVertexMesh = countVertexMesh;
							valueOutput.TableVertex = new DataDeform.Vertex[countVertexMesh];
							Vector2 coordinateStart;
							Vector2 coordinateEnd;
#if CHANGE_DEFORM_DECODING
#else
							int indexVertexStart;
							int indexVertexEnd;
							bool flagNotShift;
#endif
							for(int i=0; i<countVertexMesh; i++)
							{
#if CHANGE_DEFORM_DECODING
								valueOutput.TableVertex[i].Index = ListKey[indexStart].Value.TableVertex[i].Index;
								coordinateStart = ListKey[indexStart].Value.TableVertex[i].Coordinate;
								coordinateEnd = ListKey[indexEnd].Value.TableVertex[i].Coordinate;

								valueOutput.TableVertex[i].Coordinate.x = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																																		frame,
																																		ListKey[indexStart].Frame,
																																		coordinateStart.x,
																																		ListKey[indexEnd].Frame,
																																		coordinateEnd.x,
																																		ListKey[indexStart].FrameCurveStart,
																																		ListKey[indexStart].ValueCurveStart,
																																		ListKey[indexStart].FrameCurveEnd,
																																		ListKey[indexStart].ValueCurveEnd
																																	);
								valueOutput.TableVertex[i].Coordinate.y = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																																		frame,
																																		ListKey[indexStart].Frame,
																																		coordinateStart.y,
																																		ListKey[indexEnd].Frame,
																																		coordinateEnd.y,
																																		ListKey[indexStart].FrameCurveStart,
																																		ListKey[indexStart].ValueCurveStart,
																																		ListKey[indexStart].FrameCurveEnd,
																																		ListKey[indexStart].ValueCurveEnd
																																	);
#else
								coordinateStart = Vector2.zero;
								coordinateEnd = Vector2.zero;
								flagNotShift = false;

								valueOutput.TableVertex[i].Index = i;
								indexVertexStart = IndexGetTableVertex(ListKey[indexStart].Value.TableVertex, i);
								indexVertexEnd = IndexGetTableVertex(ListKey[indexEnd].Value.TableVertex, i);
								if(0 <= indexVertexStart)
								{
									coordinateStart = ListKey[indexStart].Value.TableVertex[indexVertexStart].Coordinate;
									flagNotShift |= true;
								}
								if(0 <= indexVertexEnd)
								{
									coordinateEnd = ListKey[indexEnd].Value.TableVertex[indexVertexEnd].Coordinate;
									flagNotShift |= true;
								}
								if(false == flagNotShift)
								{
									valueOutput.TableVertex[i].Coordinate = Vector2.zero;
								}
								else
								{
									valueOutput.TableVertex[i].Coordinate.x = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																																			frame,
																																			ListKey[indexStart].Frame,
																																			coordinateStart.x,
																																			ListKey[indexEnd].Frame,
																																			coordinateEnd.x,
																																			ListKey[indexStart].FrameCurveStart,
																																			ListKey[indexStart].ValueCurveStart,
																																			ListKey[indexStart].FrameCurveEnd,
																																			ListKey[indexStart].ValueCurveEnd
																																		);
									valueOutput.TableVertex[i].Coordinate.y = Library_SpriteStudio6.Utility.Interpolation.ValueGetFloat(	ListKey[indexStart].Formula,
																																			frame,
																																			ListKey[indexStart].Frame,
																																			coordinateStart.y,
																																			ListKey[indexEnd].Frame,
																																			coordinateEnd.y,
																																			ListKey[indexStart].FrameCurveStart,
																																			ListKey[indexStart].ValueCurveStart,
																																			ListKey[indexStart].FrameCurveEnd,
																																			ListKey[indexStart].ValueCurveEnd
																																		);
								}
#endif
							}
							return(true);

						ValueGet_ErrorEnd:;
							valueOutput = Default;
							return(false);
						}
#if CHANGE_DEFORM_DECODING
#else
						private static int IndexGetTableVertex(DataDeform.Vertex[] tableVertex, int indexVertex)
						{
							int count = tableVertex.Length;
							for(int i=0; i<count; i++)
							{
								if(indexVertex == tableVertex[i].Index)
								{
									return(i);
								}
							}
							return(-1);
						}
#endif

#if CHANGE_DEFORM_DECODING
						public void Normalize(int countVertexValid)
						{
							/* MEMO: Attribute "Deform" requires special correcting processing.                                                           */
							/*       Reasons are as follows.                                                                                              */
							/*       - The number of vertex-datas in "Deform" may over or under the nunber of Cell-Mesh's vertices in some cases.         */
							/*         e.g.) When the Cell-Mesh at editing the "Deform" is different from actually used, datas may not be consistent.     */
							/*               Originally, under(flow) occurs with data of the omitted vertices. This behavior is correct as specification. */
							/*               But occurs even when "Deform" data of different number of vertices is applied.                               */
							/* Re-Collect indexes of shifting vertices */
							Library_SpriteStudio6.Data.Animation.Attribute.Importer.DataDeform.Vertex[] tableVertex;
							int countVertex;
							int indexVertex;

							List<int> listIndexVertex = new List<int>();
							listIndexVertex.Clear();

							List<int> listKeyDelete = new List<int>();
							listKeyDelete.Clear();

							int countKey = CountGetKey();
							if(0 >= countKey)
							{
								return;
							}

							bool flagDeleteKey;
							bool flagHasVertex;
							for(int i=0; i<countKey; i++)
							{
								/* MEMO: Delete key-data when has only invalid-range vertex data. */
								flagDeleteKey = true;
								flagHasVertex = false;

								tableVertex = ListKey[i].Value.TableVertex;
								if(null != tableVertex)
								{
									countVertex = tableVertex.Length;	/* CountVertexMesh */
									for(int j=0; j<countVertex; j++)
									{
										flagHasVertex = true;

										indexVertex = tableVertex[j].Index;
										if((0 > countVertexValid) || (indexVertex < countVertexValid))
										{
											flagDeleteKey = false;

											if(false == listIndexVertex.Contains(indexVertex))
											{
												listIndexVertex.Add(indexVertex);
											}
										}
									}
								}

								if(true == (flagHasVertex & flagDeleteKey))
								{	/* Has no valid-data */
									listKeyDelete.Add(i);
								}
							}
							listIndexVertex.Sort();
							listKeyDelete.Sort();

							/* Delete no-data key */
							countVertex = listKeyDelete.Count; /* Recycle */
							for(int i=(countVertex-1); i>=0; i--)
							{
								ListKey.RemoveAt(listKeyDelete[i]);
							}
							listKeyDelete.Clear();
							listKeyDelete = null;

							/* Re-Construct all key-datas */
							countKey = CountGetKey();	/* Re-Get */
							countVertex = listIndexVertex.Count;
							int indexTableVertex;
							Library_SpriteStudio6.Data.Animation.Attribute.Importer.DataDeform dataDeform;
							for(int i=0; i<countKey; i++)
							{
								tableVertex = new DataDeform.Vertex[countVertex];

								/* Clear */
								for(int j=0; j<countVertex; j++)
								{
									tableVertex[j].Index = listIndexVertex[j];
									tableVertex[j].Coordinate = Vector2.zero;
								}

								/* Re-Set Coordinate */
								int countVertexSource = ListKey[i].Value.TableVertex.Length;
								for(int j=0; j<countVertexSource; j++)
								{
									indexVertex = ListKey[i].Value.TableVertex[j].Index;
									indexTableVertex = listIndexVertex.IndexOf(indexVertex);
									if(0 <= indexTableVertex)
									{
										tableVertex[indexTableVertex].Index = indexVertex;
										tableVertex[indexTableVertex].Coordinate = ListKey[i].Value.TableVertex[j].Coordinate;
									}
								}

								/* Set new data */
								dataDeform = new Library_SpriteStudio6.Data.Animation.Attribute.Importer.DataDeform();
								dataDeform.CountVertexMesh = countVertex;
								dataDeform.TableVertex = tableVertex;
								ListKey[i].Value = dataDeform;
							}

							listIndexVertex.Clear();
							listIndexVertex = null;
						}
#else
#endif
						#endregion Functions

						/* ----------------------------------------------- Enums & Constants */
						#region Enums & Constants
						private readonly static DataDeform.Vertex[] DefaultDeformVertex = new DataDeform.Vertex[0];
						public readonly static DataDeform Default = new DataDeform(0, DefaultDeformVertex);
						#endregion Enums & Constants
					}

					public abstract class Attribute<_Type>
						where _Type : struct
					{
						/* ----------------------------------------------- Variables & Properties */
						#region Variables & Properties
						public List<KeyData> ListKey;
						public Attribute<_Type> Parent;
						#endregion Variables & Properties

						/* ----------------------------------------------- Functions */
						#region Functions
						public abstract bool ValueGet(out _Type valueOutput, int frame);

						public void CleanUp()
						{
							ListKey = null;
							Parent = null;
						}

						public bool CleanUpKey()
						{	/* MEMO: Clear key without breaking parent-child relation. */
							if(null == ListKey)
							{
								ListKey = new List<KeyData>();
							}
							ListKey.Clear();

							return(true);
						}

						public bool BootUp()
						{
							CleanUpKey();

							Parent = null;

							return(true);
						}

						public void ShutDown()
						{
							if(null != ListKey)
							{
								ListKey.Clear();
							}
							ListKey = null;
							Parent = null;
						}

						public int CountGetKey()
						{
							return((null == ListKey) ? 0 : ListKey.Count);
						}

						public int IndexGetFramePrevious(int frame)
						{
							if((null != ListKey) && (0 <= frame))
							{
								int indexPrevious = -1;
								int count = ListKey.Count;
								for(int i=0; i<count; i++)
								{
									int frameNow = ListKey[i].Frame;
									if(frameNow == frame)
									{
										return(i);
									}
									if(frameNow > frame)
									{
										return(indexPrevious);
									}
									indexPrevious = i;
								}
								return(indexPrevious);
							}
							return(-1);
						}

						public int IndexGetFrameNext(int frame)
						{
							if((null != ListKey) && (0 <= frame))
							{
								int count = ListKey.Count;
								for(int i=0; i<count; i++)
								{
									if(ListKey[i].Frame > frame)
									{
										return(i);
									}
								}
								return(count - 1);
							}
							return(-1);
						}

						public bool KeyDataAdjustTopFrame(Attribute<_Type> setup)
						{	/* MEMO: Usually, use this to adjust top frame's key data.                      */
							/*       When no key data, attribute is unused.                                 */
							/*       When there are key datas, if extra frames at the top ...               */
							/*        - When has "Setup" value, same value as "Setup" value.                */
							/*        - When has no "Setup" value, same value as closest key datas are set. */
							/*       However, interpolation is not performed.                               */
							bool flagHasSetup = false;
							if(null != setup)
							{
								flagHasSetup = (0 < setup.CountGetKey()) ? true : false;
							}

							if(0 >= ListKey.Count)
							{	/* No Keys */
								if(false == flagHasSetup)
								{	/* No Setup-Key */
									return(false);	/* Has no Keys */
								}
							}
							else
							{	/* Has Keys */
								if(0 == ListKey[0].Frame)
								{	/* Has data at frame 0 */
									return(true);	/* Has Keys */
								}
							}

							/* Create Top Key-Data */
							/* MEMO: Same value. However, "frame = 0" and "no interpolation". */
							KeyData KeyDataTopFrame = new KeyData();
							KeyDataTopFrame.Frame = 0;
							KeyDataTopFrame.Formula = Utility.Interpolation.KindFormula.NON;
							KeyDataTopFrame.FrameCurveStart = 0.0f;
							KeyDataTopFrame.ValueCurveStart = 0.0f;
							KeyDataTopFrame.FrameCurveEnd = 0.0f;
							KeyDataTopFrame.ValueCurveEnd = 0.0f;
							if(false == flagHasSetup)
							{	/* No Setup-Key */
								KeyDataTopFrame.Value = ListKey[0].Value;
							}
							else
							{	/* Setup-Key */
								KeyDataTopFrame.Value = setup.ListKey[0].Value;
							}

							ListKey.Insert(0, KeyDataTopFrame);
							return(true);	/* Has Keys */
						}

						public bool KeyDataAdjustTopFrame(Attribute<_Type> setup, _Type valueDefault)
						{	/* MEMO: This function is for attributes with special specifications to adjust top frame's key data. */
							/*       - When no key data, interpret as default value is set                                       */
							/*       - Otherwise, performs the same as "KeyDataAdjustTopFrame" for normal.                       */
							/*       However, interpolation is not performed.                                                    */
							bool flagHasSetup = false;
							if(null != setup)
							{
								flagHasSetup = (0 < setup.CountGetKey()) ? true : false;
							}

							if(0 < ListKey.Count)
							{	/* Has Keys */
								if(0 == ListKey[0].Frame)
								{	/* Has data at frame 0 */
									return(true);	/* Has Keys */
								}
							}

							/* Create Top Key-Data */
							/* MEMO: Same value. However, "frame = 0" and "no interpolation". */
							KeyData KeyDataTopFrame = new KeyData();
							KeyDataTopFrame.Frame = 0;
							KeyDataTopFrame.Formula = Utility.Interpolation.KindFormula.NON;
							KeyDataTopFrame.FrameCurveStart = 0.0f;
							KeyDataTopFrame.ValueCurveStart = 0.0f;
							KeyDataTopFrame.FrameCurveEnd = 0.0f;
							KeyDataTopFrame.ValueCurveEnd = 0.0f;
							if(false == flagHasSetup)
							{	/* No Setup-Key */
								if(0 >= ListKey.Count)
								{	/* Has no keys */
									KeyDataTopFrame.Value = valueDefault;
								}
								else
								{	/* Has Keys */
									KeyDataTopFrame.Value = ListKey[0].Value;
								}
							}
							else
							{	/* Setup-Key */
								KeyDataTopFrame.Value = setup.ListKey[0].Value;
							}

							ListKey.Insert(0, KeyDataTopFrame);
							return(true);	/* Has Keys */
						}

						public bool KeyDataAdjustTopFrame(Attribute<_Type> setup, _Type valueDefault, bool flagNoKeyIsNoData, bool flagNotRetroactivelyComplement)
						{	/* MEMO: This function is for attributes with special specifications to adjust top frame's key data. */
							/*       - When no key data, interpret as default value is set                                       */
							/*       - When no key data in frame 0 ...                                                           */
							/*         - When has "Setup" value, same value as "Setup" value.                                    */
							/*         - When has no "Setup" value, default value is forcibly set.                               */
							/*       However, interpolation is not performed.                                                    */
							bool flagHasSetup = false;
							if(null != setup)
							{
								flagHasSetup = (0 < setup.CountGetKey()) ? true : false;
							}

							if(0 >= ListKey.Count)
							{	/* No Keys */
								if(false == flagHasSetup)
								{	/* No Setup-Key */
									if(true == flagNoKeyIsNoData)
									{	/* Interpret as no data */
										return(false);	/* Has no Keys */
									}
								}
							}
							else
							{	/* Has Keys */
								if(0 == ListKey[0].Frame)
								{	/* Has data at frame 0 */
									return(true);	/* Has Keys */
								}
							}

							if((false == flagHasSetup) && (true == flagNotRetroactivelyComplement))
							{	/* Has no Setup-Key & Not Retroactively Complement */
								return(true);
							}

							/* Create Top Key-Data */
							KeyData KeyDataTopFrame = new KeyData();
							KeyDataTopFrame.Frame = 0;
							KeyDataTopFrame.Formula = Utility.Interpolation.KindFormula.NON;
							KeyDataTopFrame.FrameCurveStart = 0.0f;
							KeyDataTopFrame.ValueCurveStart = 0.0f;
							KeyDataTopFrame.FrameCurveEnd = 0.0f;
							KeyDataTopFrame.ValueCurveEnd = 0.0f;
							if(false == flagHasSetup)
							{	/* No Setup-Key */
								KeyDataTopFrame.Value = valueDefault;
							}
							else
							{	/* Setup-Key */
								KeyDataTopFrame.Value = setup.ListKey[0].Value;
							}

							ListKey.Insert(0, KeyDataTopFrame);
							return(true);	/* Has Keys */
						}
						#endregion Functions

						/* ----------------------------------------------- Classes, Structs & Interfaces */
						#region Classes, Structs & Interfaces
						public class KeyData
						{
							/* ----------------------------------------------- Variables & Properties */
							#region Variables & Properties
							public int Frame;
							public _Type Value;

							public Library_SpriteStudio6.Utility.Interpolation.KindFormula Formula;
							public float FrameCurveStart;
							public float ValueCurveStart;
							public float FrameCurveEnd;
							public float ValueCurveEnd;
							#endregion Variables & Properties

							/* ----------------------------------------------- Functions */
							#region Functions
							public void CleanUp()
							{
								Frame = -1;	/* Frame-Value Invalid */
								Value = default(_Type);

								Formula = Library_SpriteStudio6.Utility.Interpolation.KindFormula.NON;
								FrameCurveStart = 0.0f;
								ValueCurveStart = 0.0f;
								FrameCurveEnd = 0.0f;
								ValueCurveEnd = 0.0f;
							}
							#endregion Functions
						}
						#endregion Classes, Structs & Interfaces
					}

					public struct DataDeform
					{
						/* MEMO: Alternative to "Library_SpriteStudio6.Data.Animation.Attribute.Deform" used only in importer. */

						/* ----------------------------------------------- Variables & Properties */
						#region Variables & Properties
						public int CountVertexMesh;
						public Vertex[] TableVertex;
						#endregion Variables & Properties

						/* ----------------------------------------------- Functions */
						#region Functions
						public DataDeform(int countVertexMesh, Vertex[] tableVertex)
						{
							CountVertexMesh = countVertexMesh;
							TableVertex = tableVertex;
						}

						public void CleanUp()
						{
							CountVertexMesh = 0;
							TableVertex = null;
						}

						public bool BootUp(int countVertexMesh)
						{
							if(0 > countVertexMesh)
							{
								CleanUp();
							}
							else
							{
								CountVertexMesh = countVertexMesh;
								TableVertex = new Vertex[countVertexMesh];
								if(null == TableVertex)
								{
									return(false);
								}

								for(int i=0; i<countVertexMesh; i++)
								{
									TableVertex[i].Index = i;
									TableVertex[i].Coordinate = Vector2.zero;
								}
							}
							return(true);
						}

						public void Duplicate(DataDeform original)
						{
#if ATTRIBUTE_DUPLICATE_DEEP
							/* MEMO: Deep copy */
							CountVertexMesh = original.CountVertexMesh;
							if(null == original.TableVertex)
							{
								TableVertex = null;
							}
							else
							{
								int countVertex = original.TableVertex.Length;
								TableVertex = new Vertex[countVertex];
								for(int i=0; i<countVertex; i++)
								{
									TableVertex[i].Duplicate(original.TableVertex[i]);
								}
							}
#else
							/* MEMO: Shallow copy */
							CountVertexMesh = original.CountVertexMesh;
							TableVertex = original.TableVertex;
#endif
						}

						public override bool Equals(System.Object target)
						{
							if((null == target) || (GetType() != target.GetType()))
							{
								return(false);
							}

							DataDeform targetData = (DataDeform)target;
							if(CountVertexMesh != targetData.CountVertexMesh)
							{
								return(false);
							}
							if(null == TableVertex)
							{
								if(null != targetData.TableVertex)
								{
									return(false);
								}
							}
							else
							{
								int countVertex = TableVertex.Length;
								for(int i=0; i<countVertex; i++)
								{
									if(false == TableVertex[i].Equals(targetData.TableVertex[i]))
									{
										return(false);
									}
								}
							}

							return(true);
						}

						public override int GetHashCode()
						{
							return(base.GetHashCode());
						}
						#endregion Functions

						/* ----------------------------------------------- Enums & Constants */
						#region Enums & Constants
						#endregion Enums & Constants

						/* ----------------------------------------------- Classes, Structs & Interfaces */
						#region Classes, Structs & Interfaces
						public struct Vertex
						{
							/* ----------------------------------------------- Variables & Properties */
							#region Variables & Properties
							public int Index;
							public Vector2 Coordinate;
							#endregion Variables & Properties

							/* ----------------------------------------------- Functions */
							#region Functions
							public Vertex(int index, ref Vector2 coordinate)
							{
								Index = index;
								Coordinate = coordinate;
							}

							public void CleanUp()
							{
								Index = -1;
								Coordinate = Vector2.zero;
							}

							public void Duplicate(Vertex original)
							{
								Index = original.Index;
								Coordinate = original.Coordinate;
							}

							public override bool Equals(System.Object target)
							{
								if((null == target) || (GetType() != target.GetType()))
								{
									return(false);
								}

								Vertex targetData = (Vertex)target;
								return((Index == targetData.Index) && (Coordinate == targetData.Coordinate));
							}

							public override int GetHashCode()
							{
								return(base.GetHashCode());
							}
							#endregion Functions

							/* ----------------------------------------------- Enums & Constants */
							#region Enums & Constants
							#endregion Enums & Constants
						}
						#endregion Classes, Structs & Interfaces
					}

					public static class Inheritance
					{
						/* ----------------------------------------------- Functions */
						#region Functions
						public static bool ValueGetBoolToggle(out bool valueOutput, Library_SpriteStudio6.Data.Animation.Attribute.Importer.Attribute<bool> attribute, int frame)
						{	/* MEMO: Mainly used for acquiring inheritance value of "Flip". */
							bool valueParent = false;
							if(null != attribute.Parent)
							{
								ValueGetBoolToggle(out valueParent, attribute.Parent, frame);
							}

							bool value;
							attribute.ValueGet(out value, frame);	/* "value" will always be false in case of error. */

							valueOutput = (true == value) ? !valueParent : valueParent;
							return(true);

//						ValueGetBoolToggle_ErrorEnd:;
//							valueOutput = false;
//							return(false);
						}

						public static bool ValueGetBoolOR(out bool valueOutput, Library_SpriteStudio6.Data.Animation.Attribute.Importer.Attribute<bool> attribute, int frame, bool valueDefault)
						{	/* MEMO: Mainly used for acquiring inheritance value of "Hide". */
							bool value = valueDefault;
							if(false == attribute.ValueGet(out value, frame))
							{
								value = valueDefault;
							}
							if(true == value)
							{
								valueOutput = true;
								return(true);
							}

							bool valueParent = false;
							if(null != attribute.Parent)
							{
								ValueGetBoolOR(out valueParent, attribute.Parent, frame, valueDefault);
							}

							valueOutput = valueParent;	/* valueParent | value *//* "value" is always false. */
							return(true);

//						ValueGetBoolOR_ErrorEnd:;
//							valueOutput = valueDefault;
//							return(false);
						}

						public static bool ValueGetFloatMultiple(out float valueOutput, Library_SpriteStudio6.Data.Animation.Attribute.Importer.Attribute<float> attribute, int frame, float valueDefault)
						{
							float valueParent = 1.0f;
							if(null != attribute.Parent)
							{
								ValueGetFloatMultiple(out valueParent, attribute.Parent, frame, valueDefault);
							}

							float value = valueDefault;
							if(false == attribute.ValueGet(out value, frame))
							{
								value = valueDefault;
							}

							valueOutput = valueParent * value;
							return(true);

//						ValueGetFloatMultiple_ErrorEnd:;
//							valueOutput = valueDefault;
//							return(false);
						}
						#endregion Functions
					}
					#endregion Classes, Structs & Interfaces
				}
				#endregion Classes, Structs & Interfaces
			}
		}
	}
}
