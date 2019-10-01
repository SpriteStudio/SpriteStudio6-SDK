/**
	SpriteStudio6 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
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
			public static partial class PackAttribute
			{
				/* ----------------------------------------------- Enums & Constants */
				#region Enums & Constants
#if false
				private readonly static CapacityContainer CapacityContainerDummy = new CapacityContainer(
					false,		/* Status */
					false,		/* Cell */
					false,		/* Position */
					false,		/* Rotation */
					false,		/* Scaling */
					false,		/* ScalingLocal */
					false,		/* RateOpacity */
					false,		/* Priority */
					false,		/* PartsColor */
					false,		/* VertexCorrection */
					false,		/* OffsetPivot */
					false,		/* PositionAnchor */
					false,		/* SizeForce */
					false,		/* PositionTexture */
					false,		/* RotationTexture */
					false,		/* ScalingTexture */
					false,		/* RadiusCollision */
					false,		/* UserData (Trigger) */
					false,		/* Instance (Trigger) */
					false,		/* Effect (Trigger) */
					false		/* Deform */
				);
#endif
				#endregion Enums & Constants

				/* ----------------------------------------------- Classes, Structs & Interfaces */
				#region Classes, Structs & Interfaces
				[System.Serializable]
				public class Container<_TypeValue, _TypeInterface>
					where _TypeValue : struct
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack TypePack;
					public CodeValueContainer[] TableCodeValue;
					public _TypeValue[] TableValue;

					public _TypeInterface Function;	/* NonSerialized */
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					/* MEMO: Be sure to override virtual-functions with implementation class. */
					#region Functions
					public void CleanUp()
					{
						TypePack = (Library_SpriteStudio6.Data.Animation.PackAttribute.KindTypePack)(-1);
						TableCodeValue = null;
						TableValue = null;

						Function = default(_TypeInterface);
					}
					#endregion Functions
				}

				public interface InterfaceContainerInt : InterfaceContainer<	int,
																				ContainerInt,
																				Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeInt
																			> {}
				public interface InterfaceContainerFloat : InterfaceContainer<	float,
																				ContainerFloat,
																				Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat
																			> {}
				public interface InterfaceContainerVector2 : InterfaceContainer<	Vector2,
																					ContainerVector2,
																					Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat
																				> {}
				public interface InterfaceContainerVector3 : InterfaceContainer<	Vector3,
																					ContainerVector3,
																					Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeFloat
																				> {}
				public interface InterfaceContainerStatus : InterfaceContainer<	Library_SpriteStudio6.Data.Animation.Attribute.Status,
																					ContainerStatus,
																					Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeBool
																			> {}
				public interface InterfaceContainerCell : InterfaceContainer<	Library_SpriteStudio6.Data.Animation.Attribute.Cell,
																				ContainerCell,
																				Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeCell
																			> {}
				public interface InterfaceContainerPartsColor: InterfaceContainer<	Library_SpriteStudio6.Data.Animation.Attribute.PartsColor,
																						ContainerPartsColor,
																						Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributePartsColor
																				> {}
				public interface InterfaceContainerVertexCorrection : InterfaceContainer<	Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection,
																							ContainerVertexCorrection,
																							Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeVertexCorrection
																						> {}
				public interface InterfaceContainerUserData : InterfaceContainer<	Library_SpriteStudio6.Data.Animation.Attribute.UserData,
																					ContainerUserData,
																					Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeUserData
																				> {}
				public interface InterfaceContainerInstance : InterfaceContainer<	Library_SpriteStudio6.Data.Animation.Attribute.Instance,
																					ContainerInstance,
																					Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeInstance
																				> {}
				public interface InterfaceContainerEffect : InterfaceContainer<	Library_SpriteStudio6.Data.Animation.Attribute.Effect,
																				ContainerEffect,
																				Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeEffect
																			> {}
				public interface InterfaceContainerDeform : InterfaceContainer<	Library_SpriteStudio6.Data.Animation.Attribute.Deform,
																				ContainerDeform,
																				Library_SpriteStudio6.Data.Animation.Attribute.Importer.AttributeDeform
																			> {}

				public interface InterfaceContainer<_TypeValue, _TypeContainer, _TypeSource>
					where _TypeValue : struct
				{
					/* ----------------------------------------------- Functions */
					#region Functions
					bool ValueGet(	ref _TypeValue outValue,
									ref int outFrameKey,
									_TypeContainer container,
									ref Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer argument
								);
					bool ValueGetIndex(	ref _TypeValue outValue,
										ref int outFrameKey,
										int index,
										_TypeContainer container,
										ref Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer argument
									);
					int CountGetValue(_TypeContainer container);

					bool Pack(	_TypeContainer container,
								string nameAttribute,
								int countFrame,
								Library_SpriteStudio6.Data.Animation.Parts.FlagBitStatus flagStatusParts,
								int[] tableOrderDraw,
								int[] tableOrderPreDraw,
								params _TypeSource[] listKeyData
							);
					#endregion Functions
				}

				[System.Serializable]
				public struct CodeValueContainer
				{	/* MEMO: Since Jagged-Array can not be serialized...  */
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public int[] TableCode;
					#endregion Variables & Properties
				}

				public struct ArgumentContainer
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					public Script_SpriteStudio6_DataAnimation DataAnimation;
					public int IndexAnimation;
					public int IDParts;
					public int Frame;
					public int FramePrevious;
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public ArgumentContainer(Script_SpriteStudio6_DataAnimation dataAnimation, int indexAnimation, int idParts, int frame, int framePrevious)
					{
						DataAnimation = dataAnimation;
						IndexAnimation = indexAnimation;
						IDParts = idParts;
						Frame = frame;
						FramePrevious = framePrevious;
					}

					public void CleanUp()
					{
						DataAnimation = null;
						IndexAnimation = -1;
						IDParts = -1;
						Frame = -1;
						FramePrevious = -1;
					}
					#endregion Functions
				}

				[System.Serializable]
				public class CapacityContainer
				{
					/* ----------------------------------------------- Variables & Properties */
					#region Variables & Properties
					private FlagBit Flags;

					public bool Status
					{
						get
						{
							return(0 != (Flags & FlagBit.STATUS));
						}
					}
					public bool Cell
					{
						get
						{
							return(0 != (Flags & FlagBit.CELL));
						}
					}
					public bool Position
					{
						get
						{
							return(0 != (Flags & FlagBit.POSITION));
						}
					}
					public bool Rotation
					{
						get
						{
							return(0 != (Flags & FlagBit.ROTATION));
						}
					}
					public bool Scaling
					{
						get
						{
							return(0 != (Flags & FlagBit.SCALING));
						}
					}
					public bool ScalingLocal
					{
						get
						{
							return(0 != (Flags & FlagBit.SCALING_LOCAL));
						}
					}
					public bool RateOpacity
					{
						get
						{
							return(0 != (Flags & FlagBit.RATE_OPACITY));
						}
					}
					public bool Priority
					{
						get
						{
							return(0 != (Flags & FlagBit.PRIORITY));
						}
					}
					public bool PartsColor
					{
						get
						{
							return(0 != (Flags & FlagBit.PARTS_COLOR));
						}
					}
					public bool VertexCorrection
					{
						get
						{
							return(0 != (Flags & FlagBit.VERTEX_CORRECTION));
						}
					}
					public bool OffsetPivot
					{
						get
						{
							return(0 != (Flags & FlagBit.OFFSET_PIVOT));
						}
					}
					public bool PositionAnchor
					{
						get
						{
							return(0 != (Flags & FlagBit.POSITION_ANCHOR));
						}
					}
					public bool SizeForce
					{
						get
						{
							return(0 != (Flags & FlagBit.SIZE_FORCE));
						}
					}
					public bool PositionTexture
					{
						get
						{
							return(0 != (Flags & FlagBit.POSITION_TEXTURE));
						}
					}
					public bool RotationTexture
					{
						get
						{
							return(0 != (Flags & FlagBit.ROTATION_TEXTURE));
						}
					}
					public bool ScalingTexture
					{
						get
						{
							return(0 != (Flags & FlagBit.SCALING_TEXTURE));
						}
					}
					public bool RadiusCollision
					{
						get
						{
							return(0 != (Flags & FlagBit.RADIUS_COLLISION));
						}
					}
					public bool UserData
					{
						get
						{
							return(0 != (Flags & FlagBit.USER_DATA));
						}
					}
					public bool Instance
					{
						get
						{
							return(0 != (Flags & FlagBit.INSTANCE));
						}
					}
					public bool Effect
					{
						get
						{
							return(0 != (Flags & FlagBit.EFFECT));
						}
					}
					public bool Deform
					{
						get
						{
							return(0 != (Flags & FlagBit.DEFORM));
						}
					}
					#endregion Variables & Properties

					/* ----------------------------------------------- Functions */
					#region Functions
					public CapacityContainer(	bool status,
												bool cell,
												bool position,
												bool rotation,
												bool scaling,
												bool scalingLocal,
												bool rateOpacity,
												bool priority,
												bool partsColor,
												bool vertexCorrection,
												bool offsetPivot,
												bool positionAnchor,
												bool sizeForce,
												bool positionTexture,
												bool rotationTexture,
												bool scalingTexture,
												bool radiusCollision,
												bool userData,
												bool instance,
												bool effect,
												bool deform
											)
					{
						Flags = 0;
						Flags |= (true == status) ? FlagBit.STATUS : (FlagBit)0;

						Flags |= (true == cell) ? FlagBit.CELL : (FlagBit)0;

						Flags |= (true == position) ? FlagBit.POSITION : (FlagBit)0;
						Flags |= (true == rotation) ? FlagBit.ROTATION : (FlagBit)0;
						Flags |= (true == scaling) ? FlagBit.SCALING : (FlagBit)0;
						Flags |= (true == scalingLocal) ? FlagBit.SCALING_LOCAL : (FlagBit)0;

						Flags |= (true == rateOpacity) ? FlagBit.RATE_OPACITY : (FlagBit)0;
						Flags |= (true == priority) ? FlagBit.PRIORITY : (FlagBit)0;
						Flags |= (true == partsColor) ? FlagBit.PARTS_COLOR : (FlagBit)0;
						Flags |= (true == vertexCorrection) ? FlagBit.VERTEX_CORRECTION : (FlagBit)0;

						Flags |= (true == offsetPivot) ? FlagBit.OFFSET_PIVOT : (FlagBit)0;
						Flags |= (true == positionAnchor) ? FlagBit.POSITION_ANCHOR : (FlagBit)0;
						Flags |= (true == sizeForce) ? FlagBit.SIZE_FORCE : (FlagBit)0;
						Flags |= (true == positionTexture) ? FlagBit.POSITION_TEXTURE : (FlagBit)0;
						Flags |= (true == rotationTexture) ? FlagBit.ROTATION_TEXTURE : (FlagBit)0;
						Flags |= (true == scalingTexture) ? FlagBit.SCALING_TEXTURE : (FlagBit)0;

						Flags |= (true == radiusCollision) ? FlagBit.RADIUS_COLLISION : (FlagBit)0;

						Flags |= (true == userData) ? FlagBit.USER_DATA : (FlagBit)0;
						Flags |= (true == instance) ? FlagBit.INSTANCE : (FlagBit)0;
						Flags |= (true == effect) ? FlagBit.EFFECT : (FlagBit)0;
						Flags |= (true == deform) ? FlagBit.DEFORM : (FlagBit)0;
					}
					#endregion Functions

					/* ----------------------------------------------- Enums & Constants */
					#region Enums & Constants
					[System.Flags]
					private enum FlagBit
					{
						STATUS = 0x00000001,
						CELL = 0x00000002,
						POSITION = 0x00000004,
						ROTATION = 0x00000008,
						SCALING = 0x00000010,
						SCALING_LOCAL = 0x00000020,
						RATE_OPACITY = 0x00000040,
						PRIORITY = 0x00000080,
						PARTS_COLOR = 0x00000100,
						VERTEX_CORRECTION = 0x00000200,
						OFFSET_PIVOT = 0x00000400,
						POSITION_ANCHOR = 0x00000800,
						SIZE_FORCE = 0x00001000,
						POSITION_TEXTURE = 0x00002000,
						ROTATION_TEXTURE = 0x00004000,
						SCALING_TEXTURE = 0x00008000,
						RADIUS_COLLISION = 0x00010000,
						USER_DATA = 0x00020000,
						INSTANCE = 0x00040000,
						EFFECT = 0x00080000,
						DEFORM = 0x00100000,
					}
					#endregion Enums & Constants
				}
				#endregion Classes, Structs & Interfaces
			}
		}
	}
}
