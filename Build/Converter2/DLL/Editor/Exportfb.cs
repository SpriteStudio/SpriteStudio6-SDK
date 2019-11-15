using System.Collections;
using System.Collections.Generic;
#if !_FOR_SS6CONVERTER_
using UnityEngine;
using UnityEditor;
#else
using SS6ConverterVer2_DLL;
using SS6ConverterVer2_DLL.Types;
using UnityEngine = SS6ConverterVer2_DLL.Assets;
#endif

//using ss.ssfb2;
using FlatBuffers;
using System.IO;
using System.Text;

public static partial class LibraryEditor_SpriteStudio6
{
    public static partial class Import
    {

        /// <summary>
        /// FlatBufferへの格納
        /// </summary>
        public static partial class ExportSSFB2
        {

            public static void fb_test(string outputfilename)
            {
                var fbb = new FlatBufferBuilder(16);

                ss.ssfb2.ProjectData.StartProjectData(fbb);
                ss.ssfb2.ProjectData.AddVersion(fbb, 1);
                var buf = ss.ssfb2.ProjectData.EndProjectData(fbb);

                ss.ssfb2.ProjectData.FinishProjectDataBuffer(fbb, buf);
                var data = fbb.DataBuffer.ToSizedArray();

                var writer = new BinaryWriter(new FileStream(outputfilename, FileMode.Create));
                writer.Write(data);
                writer.Close();
            }


            public static void output_ssfb2(Script_SpriteStudio6_DataProject outdata , string outputfilename)
            {
                var fbb = new FlatBufferBuilder(16);


                var  projdata = makeProjectData(fbb, outdata);
                ss.ssfb2.ProjectData.FinishProjectDataBuffer(fbb, projdata);
                var buf = fbb.DataBuffer.ToSizedArray();

                //ファイル出力
                var writer = new BinaryWriter(new FileStream( outputfilename , FileMode.Create));

                writer.Write(buf);
                writer.Close();

            }


            //プロジェクトデータの作成
            public static Offset<ss.ssfb2.ProjectData> makeProjectData(FlatBufferBuilder fbb , Script_SpriteStudio6_DataProject project )
            {

                var animelist = new Offset<ss.ssfb2.DataAnimation>[project.Animation.Length];
                var effectlist = new Offset<ss.ssfb2.DataEffect>[project.Effect.Length];

                for (int i =0; i < project.Animation.Length; i ++)
                {
                    animelist[i] = makeDataAnimation(fbb, project.Animation[i]);
                }
                for (int i = 0; i < project.Effect.Length; i++)
                {
                    effectlist[i] = makeDataEffect(fbb, project.Effect[i]);
                }

                return ss.ssfb2.ProjectData.CreateProjectData(fbb,
                    (uint)project.Version,
                    makeDataCellMap(fbb, project.CellMap),
                    ss.ssfb2.ProjectData.CreateAnimationVector(fbb, animelist),
                    ss.ssfb2.ProjectData.CreateEffectVector(fbb, effectlist)
                    );
            }

            public static Offset<ss.ssfb2.ColorLabel> makeColorLabel(FlatBufferBuilder fbb, Library_SpriteStudio6.Data.Parts.Animation.ColorLabel labelcolor)
            {
                return ss.ssfb2.ColorLabel.CreateColorLabel(
                    fbb,
                    (ss.ssfb2.ColorLabelKindForm)labelcolor.Form,
                    ss.ssfb2.Color32.CreateColor32( fbb , labelcolor.Color.r, labelcolor.Color.g , labelcolor.Color.b, labelcolor.Color.a )
                    );
            }


            public static Offset<ss.ssfb2.Bone> makeBone(FlatBufferBuilder fbb ,
                    Library_SpriteStudio6.Data.Parts.Animation.BindMesh.Vertex.Bone bone)
            {
                return ss.ssfb2.Bone.CreateBone(fbb,
                    (ushort)bone.Index,
                    bone.Weight,
                    ss.ssfb2.Vector3.CreateVector3(fbb,bone.CoordinateOffset.x,bone.CoordinateOffset.y,bone.CoordinateOffset.z)
                    );
            }


            public static VectorOffset makeVertex(FlatBufferBuilder fbb , Library_SpriteStudio6.Data.Parts.Animation.BindMesh.Vertex[] v)
            {
                if ( v == null )
                {
                    var tempv = new Offset<ss.ssfb2.Vertex>[0];
                    return ss.ssfb2.BindMesh.CreateTableVertexVector(fbb , tempv);
                }

                var _vertex = new Offset<ss.ssfb2.Vertex>[v.Length];

                for (int i = 0; i < v.Length; i++ )
                {
                    Offset<ss.ssfb2.Bone>[] _tablebone = new Offset<ss.ssfb2.Bone>[v[i].TableBone.Length];
                    int n = 0;
                    foreach ( var bone in v[i].TableBone )
                    {
                        _tablebone[n] = makeBone(fbb, bone);
                        n++;
                    }

                    _vertex[i] = ss.ssfb2.Vertex.CreateVertex(fbb, ss.ssfb2.Vertex.CreateTableBoneVector(fbb,_tablebone));
                }

                return ss.ssfb2.BindMesh.CreateTableVertexVector(fbb, _vertex);
            }

            public static Offset<ss.ssfb2.Vector2>[] convertArrayVector2(FlatBufferBuilder fbb , Vector2[] array_vec2)
            {
                 
                var ret = new Offset<ss.ssfb2.Vector2>[array_vec2.Length];
                for ( int i = 0; i < array_vec2.Length; i++)
                {
                    ret[i] = ss.ssfb2.Vector2.CreateVector2(fbb , array_vec2[i].x, array_vec2[i].y);
                }

                return ret;

            }

            public static Offset<ss.ssfb2.BindMesh> makeBindMesh(FlatBufferBuilder fbb, Library_SpriteStudio6.Data.Parts.Animation.BindMesh bindmesh)
            {

                return ss.ssfb2.BindMesh.CreateBindMesh(
                    fbb,
                    bindmesh.CountVertex,
                    makeVertex(fbb, bindmesh.TableVertex),

                    bindmesh.TableRateUV == null ?
                            default(VectorOffset) :
                            ss.ssfb2.BindMesh.CreateTableRateUvVector(fbb, convertArrayVector2(fbb, bindmesh.TableRateUV)),

                    bindmesh.TableIndexVertex == null ? 
                            default(VectorOffset) : 
                            ss.ssfb2.BindMesh.CreateTableIndexVertexVector(fbb, bindmesh.TableIndexVertex),

                        bindmesh.CountVertexDeform
                    );
            }

            //DataModelParts
            public static Offset<ss.ssfb2.DataModelParts> makeDataModelParts(FlatBufferBuilder fbb , Library_SpriteStudio6.Data.Parts.Animation DataModelParts)
            {

                return ss.ssfb2.DataModelParts.CreateDataModelParts(fbb,
                    fbb.CreateString(DataModelParts.Name),
                    DataModelParts.ID,
                    DataModelParts.IDParent,
                    ss.ssfb2.DataModelParts.CreateTableIdChildrenVector(fbb, DataModelParts.TableIDChild),
                    (ss.ssfb2.DataModelPartsKindFeature)DataModelParts.Feature,
                    DataModelParts.CountMesh,
                    makeBindMesh(fbb,DataModelParts.Mesh),
                    makeColorLabel(fbb,DataModelParts.LabelColor),
                    (ss.ssfb2.KindOperationBlend)DataModelParts.OperationBlendTarget,
                    (ss.ssfb2.KindCollision)DataModelParts.ShapeCollision,
                    DataModelParts.SizeCollisionZ,
                    (ushort)DataModelParts.IndexAnimationPackUnderControl,
                    (ushort)DataModelParts.IndexEffectUnderControl,
                    fbb.CreateString(DataModelParts.NameAnimationUnderControl)
                    );
            }

            public static ushort[] convertArrayInt2ushort(FlatBufferBuilder fbb , int[] _in )
            {
                ushort[] ret = new ushort[_in.Length];
                for (int i = 0; i < _in.Length; i ++)
                {
                    ret[i] = (ushort)_in[i];
                }

                return ret;
            }

            //Catalog
            public static Offset<ss.ssfb2.Catalog> makeCatalog(FlatBufferBuilder fbb , Library_SpriteStudio6.Data.Parts.Animation.Catalog catalog)
            {
                
                return ss.ssfb2.Catalog.CreateCatalog(fbb,
                    ss.ssfb2.Catalog.CreateTableIdPartsNullVector(fbb, convertArrayInt2ushort(fbb, catalog.TableIDPartsNULL) ),
                    ss.ssfb2.Catalog.CreateTableIdPartsNormalVector(fbb,convertArrayInt2ushort(fbb,catalog.TableIDPartsNormal)),
                    ss.ssfb2.Catalog.CreateTableIdPartsInstanceVector(fbb, convertArrayInt2ushort(fbb, catalog.TableIDPartsInstance)),
                    ss.ssfb2.Catalog.CreateTableIdPartsEffectVector(fbb, convertArrayInt2ushort(fbb, catalog.TableIDPartsEffect)),
                    ss.ssfb2.Catalog.CreateTableIdPartsMaskVector(fbb, convertArrayInt2ushort(fbb, catalog.TableIDPartsMask)),
                    ss.ssfb2.Catalog.CreateTableIdPartsJointVector(fbb, convertArrayInt2ushort(fbb, catalog.TableIDPartsJoint)),
                    ss.ssfb2.Catalog.CreateTableIdPartsBoneVector(fbb, convertArrayInt2ushort(fbb, catalog.TableIDPartsBone)),
                    ss.ssfb2.Catalog.CreateTableIdPartsMoveNodeVector(fbb, convertArrayInt2ushort(fbb, catalog.TableIDPartsMoveNode)),
                    ss.ssfb2.Catalog.CreateTableIdPartsConstraintVector(fbb, convertArrayInt2ushort(fbb, catalog.TableIDPartsConstraint)),
                    ss.ssfb2.Catalog.CreateTableIdPartsBonePointVector(fbb, convertArrayInt2ushort(fbb, catalog.TableIDPartsBonePoint)),
                    ss.ssfb2.Catalog.CreateTableIdPartsMeshVector(fbb, convertArrayInt2ushort(fbb, catalog.TableIDPartsMesh))
                    );

            }


            static Offset<ss.ssfb2.UserData> makeUserData(FlatBufferBuilder fbb, Library_SpriteStudio6.Data.Animation.Attribute.UserData userdata)
            {
                return ss.ssfb2.UserData.CreateUserData(fbb,
                      (ss.ssfb2.UserDataFlagBit)userdata.Flags,
                      userdata.NumberInt,
                      ss.ssfb2.Rect.CreateRect( fbb , userdata.Rectangle.xMin , userdata.Rectangle.yMin , userdata.Rectangle.xMax , userdata.Rectangle.yMax ),
                      ss.ssfb2.Vector2.CreateVector2( fbb , userdata.Coordinate.x , userdata.Coordinate.y ),
                      fbb.CreateString( userdata.Text )
                    );
            }

            static Offset<ss.ssfb2.DataSetup> makeLibraryDataSetup(FlatBufferBuilder fbb, Script_SpriteStudio6_DataAnimation.DataSetup setup)
            {

                return ss.ssfb2.DataSetup.CreateDataSetup(fbb,
                    makeUserData(fbb,setup.UserData)
                    );
            }


            static Offset<ss.ssfb2.Label> makeLabel(FlatBufferBuilder fbb, Library_SpriteStudio6.Data.Animation.Label label )
            {
                return ss.ssfb2.Label.CreateLabel(
                    fbb,
                    fbb.CreateString(label.Name),
                    (ushort)label.Frame
                    );
            }


            static Offset<ss.ssfb2.CodeValueContainer> makeCodeValueContainer( FlatBufferBuilder fbb ,
                Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer codevalue
                )
            {
                return ss.ssfb2.CodeValueContainer.CreateCodeValueContainer(fbb,
                    ss.ssfb2.CodeValueContainer.CreateTableCodeVector(fbb, codevalue.TableCode)
                    );
            }


            static VectorOffset makeCodeValueContainerTable(FlatBufferBuilder fbb,
                Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[] codevalue)
            {
                var ret = new Offset<ss.ssfb2.CodeValueContainer>[codevalue.Length];
                for ( int i = 0; i < codevalue.Length; i++)
                {
                    ret[i] = makeCodeValueContainer(fbb, codevalue[i]);
                }
                return fbb.CreateVectorOfTables(ret);
            }

            //ContainerStatus
            static Offset<ss.ssfb2.ContainerStatus> makeContainerStatus( FlatBufferBuilder fbb , 
                Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerStatus status)
            {
                //ContainerStatusはTableValue　 = status を無視してそのままintで格納する
                int[] array = new int[status.TableValue.Length];
                for (int i = 0; i < status.TableValue.Length; i++ )
                {
                    array[i] = (int)status.TableValue[i].Flags;
                }

                return ss.ssfb2.ContainerStatus.CreateContainerStatus(fbb,
                        (ss.ssfb2.KindTypePack)status.TypePack,                     //定型
                        makeCodeValueContainerTable(fbb, status.TableCodeValue),    //定型
                        ss.ssfb2.ContainerStatus.CreateTableValueVector(fbb, array)
                    );
            }


            //AnimationAttributeCell
            static Offset<ss.ssfb2.AnimationAttributeCell>[] makeAnimationAttributeCellTable(
                    FlatBufferBuilder fbb,
                    Library_SpriteStudio6.Data.Animation.Attribute.Cell[] cells
                )
            {
                var ret = new Offset<ss.ssfb2.AnimationAttributeCell>[cells.Length];

                for (int i = 0;  i < cells.Length; i++ )
                {
                    ret[i] = ss.ssfb2.AnimationAttributeCell.CreateAnimationAttributeCell(
                            fbb ,
                            cells[i].IndexCellMap,
                            cells[i].IndexCell
                        );
                }

                return ret;
            }


            //ContainerCell
            static Offset<ss.ssfb2.ContainerCell> makeContainerCell(FlatBufferBuilder fbb,
                Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerCell cell)
            {
                //ContainerStatusはTableValue　 = status を無視してそのままintで格納する

                return ss.ssfb2.ContainerCell.CreateContainerCell(fbb,
                        (ss.ssfb2.KindTypePack)cell.TypePack,                     //定型
                        makeCodeValueContainerTable(fbb, cell.TableCodeValue),    //定型
                        ss.ssfb2.ContainerCell.CreateTableValueVector(fbb, makeAnimationAttributeCellTable(fbb,cell.TableValue))
                    );
            }

            //ContainerVector3
            static Offset<ss.ssfb2.ContainerVector3> makeContainerVector3(FlatBufferBuilder fbb,
                    Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3 vec3
                )
            {

                var table = new Offset<ss.ssfb2.Vector3>[vec3.TableValue.Length];
                for ( int i = 0; i < vec3.TableValue.Length; i ++)
                {
                    table[i] = ss.ssfb2.Vector3.CreateVector3( fbb,
                            vec3.TableValue[i].x,
                            vec3.TableValue[i].y,
                            vec3.TableValue[i].z
                        );
                }

                return ss.ssfb2.ContainerVector3.CreateContainerVector3(fbb,
                        (ss.ssfb2.KindTypePack)vec3.TypePack,                     //定型
                        makeCodeValueContainerTable(fbb, vec3.TableCodeValue),    //定型
                        ss.ssfb2.ContainerVector3.CreateTableValueVector(fbb, table )
                        );

            }


            //ContainerVector2
            static Offset<ss.ssfb2.ContainerVector2> makeContainerVector2(FlatBufferBuilder fbb,
                    Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2 vec2
                )
            {

                var table = new Offset<ss.ssfb2.Vector2>[vec2.TableValue.Length];
                for (int i = 0; i < vec2.TableValue.Length; i++)
                {
                    table[i] = ss.ssfb2.Vector2.CreateVector2(fbb,
                            vec2.TableValue[i].x,
                            vec2.TableValue[i].y
                        );
                }

                return ss.ssfb2.ContainerVector2.CreateContainerVector2(fbb,
                        (ss.ssfb2.KindTypePack)vec2.TypePack,                     //定型
                        makeCodeValueContainerTable(fbb, vec2.TableCodeValue),    //定型
                        ss.ssfb2.ContainerVector2.CreateTableValueVector(fbb, table)
                        );

            }

            //ContainerFloat
            static Offset<ss.ssfb2.ContainerFloat> makeContainerFloat(FlatBufferBuilder fbb,
                    Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerFloat fc
                )
            {

                if ( fc == null )
                {
                    return ss.ssfb2.ContainerFloat.CreateContainerFloat(fbb);
                    //fc = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerFloat();
                }

                return ss.ssfb2.ContainerFloat.CreateContainerFloat(fbb,
                        (ss.ssfb2.KindTypePack)fc.TypePack,                     //定型
                        makeCodeValueContainerTable(fbb, fc.TableCodeValue),    //定型
                        ss.ssfb2.ContainerFloat.CreateTableValueVector(fbb, fc.TableValue)
                        );
            }


            //ContainerInt
            static Offset<ss.ssfb2.ContainerInt> makeContainerInt(FlatBufferBuilder fbb,
                    Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInt inttable
                )
            {
                return ss.ssfb2.ContainerInt.CreateContainerInt(fbb,
                        (ss.ssfb2.KindTypePack)inttable.TypePack,                     //定型
                        makeCodeValueContainerTable(fbb, inttable.TableCodeValue),    //定型
                        ss.ssfb2.ContainerInt.CreateTableValueVector(fbb, inttable.TableValue)
                        );
            }


            //AnimationAttributePartsColor
            static Offset<ss.ssfb2.AnimationAttributePartsColor>[] makeAnimationAttributePartsColorTable(
                    FlatBufferBuilder fbb,
                    Library_SpriteStudio6.Data.Animation.Attribute.PartsColor[] pcolor
                )
            {
                var ret = new Offset<ss.ssfb2.AnimationAttributePartsColor>[pcolor.Length];

                for (int i = 0; i < pcolor.Length; i++)
                {
                    var _colors = pcolor[i];

                    Offset<ss.ssfb2.Color>[] _color_offset = new Offset<ss.ssfb2.Color>[_colors.VertexColor.Length];
                    for ( int n = 0; n < _colors.VertexColor.Length; n++)
                    {
                        var vcolor = _colors.VertexColor[n];
                        _color_offset[i] = ss.ssfb2.Color.CreateColor(fbb, vcolor.r , vcolor.g , vcolor.b , vcolor.a );
                    } 

                    ret[i] = ss.ssfb2.AnimationAttributePartsColor.CreateAnimationAttributePartsColor(
                            fbb,
                            (short)pcolor[i].Bound,
                            (short)pcolor[i].Operation,
                            ss.ssfb2.AnimationAttributePartsColor.CreateVertexColorVector(fbb, _color_offset),
                            ss.ssfb2.AnimationAttributePartsColor.CreateRateAlphaVector(fbb, _colors.RateAlpha )
                        );
                }

                return ret;
            }


            //ContainerPartsColor
            static Offset<ss.ssfb2.ContainerPartsColor> makeContainerPartsColor(FlatBufferBuilder fbb,
                    Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerPartsColor partcolor
                )
            {
                return ss.ssfb2.ContainerPartsColor.CreateContainerPartsColor(fbb,
                        (ss.ssfb2.KindTypePack)partcolor.TypePack,                     //定型
                        makeCodeValueContainerTable(fbb, partcolor.TableCodeValue),    //定型
                            ss.ssfb2.ContainerPartsColor.CreateTableValueVector(
                                                            fbb,
                                                            makeAnimationAttributePartsColorTable(fbb , partcolor.TableValue) )
                        );
            }



            //VertexCorrection
            static Offset<ss.ssfb2.VertexCorrection>[] makeVertexCorrectionTable(FlatBufferBuilder fbb,
                    Library_SpriteStudio6.Data.Animation.Attribute.VertexCorrection[] _in
                )
            {

                var _rettable = new Offset<ss.ssfb2.VertexCorrection>[_in.Length];
                for (int i = 0; i < _in.Length; i ++)
                {

                    int vertex_num = _in[i].Coordinate.Length;
                    Offset<ss.ssfb2.Vector2>[] _vec2_table = new Offset<ss.ssfb2.Vector2>[vertex_num];
                    for (int n = 0; n < vertex_num; n ++ )
                    {
                        Vector2 v = _in[i].Coordinate[n];
                        _vec2_table[n] = ss.ssfb2.Vector2.CreateVector2(fbb, v.x, v.y);
                    }

                    _rettable[i] = ss.ssfb2.VertexCorrection.CreateVertexCorrection(
                            fbb ,
                            ss.ssfb2.VertexCorrection.CreateCoordinateVector(fbb, _vec2_table)
                        );
                }

                return _rettable;
            }


            //ContainerVertexCorrection
            static Offset<ss.ssfb2.ContainerVertexCorrection> makeContainerVertexCorrection(FlatBufferBuilder fbb,
                    Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVertexCorrection _in
                )
            {

                return ss.ssfb2.ContainerVertexCorrection.CreateContainerVertexCorrection(fbb,
                        (ss.ssfb2.KindTypePack)_in.TypePack,                     //定型
                        makeCodeValueContainerTable(fbb, _in.TableCodeValue),    //定型
                        ss.ssfb2.ContainerVertexCorrection.CreateTableValueVector(fbb, makeVertexCorrectionTable(fbb,_in.TableValue))
                        );
            }

            //ContainerUserData
            static Offset<ss.ssfb2.ContainerUserData> makeContainerUserData(FlatBufferBuilder fbb,
                    Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerUserData _in
                )
            {

                var _table = new Offset<ss.ssfb2.UserData>[_in.TableValue.Length];
                for ( int i = 0; i < _in.TableValue.Length; i ++ )
                {
                    _table[i] = makeUserData(fbb, _in.TableValue[i]);
                }

                return ss.ssfb2.ContainerUserData.CreateContainerUserData(fbb,
                        (ss.ssfb2.KindTypePack)_in.TypePack,                     //定型
                        makeCodeValueContainerTable(fbb, _in.TableCodeValue),    //定型
                        ss.ssfb2.ContainerUserData.CreateTableValueVector(fbb, _table )
                        );
            }

            static Offset<ss.ssfb2.AnimationAttributeInstance>[] makeAnimationAttributeInstance(FlatBufferBuilder fbb,
                    Library_SpriteStudio6.Data.Animation.Attribute.Instance[] _in                
                )
            {
                var _table = new Offset<ss.ssfb2.AnimationAttributeInstance>[_in.Length];
                for ( int i = 0; i < _in.Length; i ++ )
                {
                    Library_SpriteStudio6.Data.Animation.Attribute.Instance inst = _in[i];
                    _table[i] = ss.ssfb2.AnimationAttributeInstance.CreateAnimationAttributeInstance(
                            fbb,
                            (int)inst.Flags,
                            inst.PlayCount,
                            inst.RateTime,
                            inst.OffsetStart,
                            inst.OffsetEnd,
                            fbb.CreateString( inst.LabelStart ),
                            fbb.CreateString( inst.LabelEnd )
                            );
                }
                return _table;
            }


            //ContainerInstance
            static Offset<ss.ssfb2.ContainerInstance> makeContainerInstance(FlatBufferBuilder fbb,
                    Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerInstance _in
                )
            {

                return ss.ssfb2.ContainerInstance.CreateContainerInstance(fbb,
                        (ss.ssfb2.KindTypePack)_in.TypePack,                     //定型
                        makeCodeValueContainerTable(fbb, _in.TableCodeValue),    //定型
                        ss.ssfb2.ContainerInstance.CreateTableValueVector( 
                            fbb ,
                            makeAnimationAttributeInstance( fbb , _in.TableValue ) )
                        );
            }


            static Offset<ss.ssfb2.AnimationAttributeEffect>[] makeAnimationAttributeEffect(FlatBufferBuilder fbb,
                    Library_SpriteStudio6.Data.Animation.Attribute.Effect[] _in
                )
            {
                var _table = new Offset<ss.ssfb2.AnimationAttributeEffect>[_in.Length];
                for (int i = 0; i < _in.Length; i++)
                {
                    Library_SpriteStudio6.Data.Animation.Attribute.Effect inst = _in[i];
                    _table[i] = ss.ssfb2.AnimationAttributeEffect.CreateAnimationAttributeEffect(
                            fbb,
                            (int)inst.Flags,
                            inst.FrameStart,
                            inst.RateTime
                            );
                }
                return _table;
            }


            //ContainerEffect
            static Offset<ss.ssfb2.ContainerEffect> makeContainerEffect(FlatBufferBuilder fbb,
                    Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerEffect _in
                )
            {
                return ss.ssfb2.ContainerEffect.CreateContainerEffect(fbb,
                        (ss.ssfb2.KindTypePack)_in.TypePack,                     //定型
                        makeCodeValueContainerTable(fbb, _in.TableCodeValue),    //定型
                        ss.ssfb2.ContainerEffect.CreateTableValueVector(
                            fbb,
                            makeAnimationAttributeEffect(fbb, _in.TableValue))
                        );
            }




            static Offset<ss.ssfb2.AnimationAttributeDeform>[] makeAnimationAttributeDeform(FlatBufferBuilder fbb,
                    Library_SpriteStudio6.Data.Animation.Attribute.Deform[] _in
                )
            {
                var _table = new Offset<ss.ssfb2.AnimationAttributeDeform>[_in.Length];
                for (int i = 0; i < _in.Length; i++)
                {
                    Library_SpriteStudio6.Data.Animation.Attribute.Deform inst = _in[i];

                    var vector_offset = new Offset<ss.ssfb2.Vector2>[inst.TableCoordinate.Length];
                    for ( int n = 0; n < inst.TableCoordinate.Length; n ++ )
                    {
                        Vector2 v = inst.TableCoordinate[n];
                        vector_offset[n] = ss.ssfb2.Vector2.CreateVector2(fbb, v.x, v.y);
                    }

                    _table[i] = ss.ssfb2.AnimationAttributeDeform.CreateAnimationAttributeDeform(
                            fbb,
                            ss.ssfb2.AnimationAttributeDeform.CreateTableCoordinateVector( fbb , vector_offset )
                            );
                }
                return _table;
            }

            //ContainerDeform
            static Offset<ss.ssfb2.ContainerDeform> makeContainerDeform(FlatBufferBuilder fbb,
                    Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerDeform _in
                )
            {
                return ss.ssfb2.ContainerDeform.CreateContainerDeform(fbb,
                        (ss.ssfb2.KindTypePack)_in.TypePack,                     //定型
                        makeCodeValueContainerTable(fbb, _in.TableCodeValue),    //定型
                        ss.ssfb2.ContainerDeform.CreateTableValueVector( fbb ,
                            makeAnimationAttributeDeform( fbb , _in.TableValue ) )
                        );
            }


            //パーツ情報 
            static Offset<ss.ssfb2.DataAnimationParts> makeDataAnimationParts(FlatBufferBuilder fbb, Library_SpriteStudio6.Data.Animation.Parts part)
            {

                return ss.ssfb2.DataAnimationParts.CreateDataAnimationParts
                    (
                        fbb,
                        (ss.ssfb2.DataAnimationPartsFlagBitStatus)part.StatusParts,
                        makeContainerStatus(fbb, part.Status),
                        makeContainerCell(fbb, part.Cell),
                        makeContainerVector3(fbb, part.Position),
                        makeContainerVector3(fbb, part.Rotation),
                        makeContainerVector2(fbb, part.Scaling),
                        makeContainerVector2(fbb, part.ScalingLocal),
                        makeContainerFloat(fbb, part.RateOpacity),
                        makeContainerInt(fbb, part.Priority),
                        makeContainerPartsColor(fbb, part.PartsColor),
                        makeContainerVertexCorrection(fbb, part.VertexCorrection),
                        makeContainerVector2(fbb, part.OffsetPivot),
                        makeContainerVector2(fbb, part.PositionAnchor),
                        makeContainerVector2(fbb, part.SizeForce),
                        makeContainerVector2(fbb, part.PositionTexture),
                        makeContainerFloat(fbb, part.RotationTexture),
                        makeContainerVector2(fbb, part.ScalingTexture),
                        makeContainerFloat(fbb, part.RadiusCollision),
                        makeContainerUserData(fbb, part.UserData),
                        makeContainerInstance( fbb ,part.Instance ),
                        makeContainerEffect( fbb , part.Effect ),
                        makeContainerDeform( fbb , part.Deform )
                    );
            }


            static Offset<ss.ssfb2.LibraryDataAnimation> makeLibraryDataAnimation(FlatBufferBuilder fbb , Library_SpriteStudio6.Data.Animation animeData)
            {
                var label_offsets = new Offset<ss.ssfb2.Label>[animeData.TableLabel.Length];
                for ( int i = 0 ; i < animeData.TableLabel.Length; i++ )
                    label_offsets[i] = makeLabel(fbb, animeData.TableLabel[i]);

                var parts_offsets = new Offset<ss.ssfb2.DataAnimationParts>[animeData.TableParts.Length];
                for (int i = 0; i < animeData.TableParts.Length; i++)
                    parts_offsets[i] = makeDataAnimationParts(fbb, animeData.TableParts[i]);

                return ss.ssfb2.LibraryDataAnimation.CreateLibraryDataAnimation(
                    fbb,
                    fbb.CreateString(animeData.Name),
                    animeData.FramePerSecond,
                    animeData.CountFrame,
                    animeData.SizeCanvasX,
                    animeData.SizeCanvasY,
                    animeData.FrameValidStart,
                    animeData.FrameValidEnd,
                    animeData.CountFrameValid,
                    animeData.DepthIK,
                    (ss.ssfb2.KindModeSort)animeData.ModeSort,
                    ss.ssfb2.LibraryDataAnimation.CreateTableLabelVector(fbb, label_offsets),
                    ss.ssfb2.LibraryDataAnimation.CreateTablePartsVector(fbb, parts_offsets)
                    );
            }


            //DataAnimation
            public static Offset<ss.ssfb2.DataAnimation> makeDataAnimation(FlatBufferBuilder fbb , Script_SpriteStudio6_DataAnimation anime)
            {

                var modelparts = new Offset<ss.ssfb2.DataModelParts>[anime.TableParts.Length];
                for ( var i = 0; i < anime.TableParts.Length; i ++ )
                    modelparts[i] = makeDataModelParts(fbb , anime.TableParts[i]);

                var table_animation = new Offset<ss.ssfb2.LibraryDataAnimation>[anime.TableAnimation.Length];
                for ( int i= 0; i < anime.TableAnimation.Length; i++ )
                {
                    table_animation[i] = makeLibraryDataAnimation(fbb, anime.TableAnimation[i]);
                }

                var table_animation_parts_setup = new Offset<ss.ssfb2.DataSetup>[anime.TableAnimationPartsSetup.Length];
                for (int i = 0; i < anime.TableAnimationPartsSetup.Length; i++)
                {
                    table_animation_parts_setup[i] = makeLibraryDataSetup(fbb, anime.TableAnimationPartsSetup[i]);
                }

                return ss.ssfb2.DataAnimation.CreateDataAnimation(
                    fbb,
                    (uint)anime.Version,
                    ss.ssfb2.DataAnimation.CreateTablePartsVector(fbb, modelparts),
                    makeCatalog(fbb, anime.CatalogParts),
                    ss.ssfb2.DataAnimation.CreateTableAnimationVector(fbb, table_animation),
                    ss.ssfb2.DataAnimation.CreateTableAnimationPartsSetupVector(fbb, table_animation_parts_setup)
                    );
            }

            //メッシュデータの格納
            public static Offset<ss.ssfb2.DataMesh> makeDataMesh(FlatBufferBuilder fbb, Library_SpriteStudio6.Data.CellMap.Cell.DataMesh mesh)
            {

                if (mesh.TableCoordinate != null)
                {
                    //mesh.TableCoordinate
                    Offset<ss.ssfb2.Vector2>[] table_coordinate = new Offset<ss.ssfb2.Vector2>[mesh.TableCoordinate.Length];
                    for (int i = 0; i < mesh.TableCoordinate.Length; i++)
                    {
                        table_coordinate[i] = ss.ssfb2.Vector2.CreateVector2(fbb, mesh.TableCoordinate[i].x, mesh.TableCoordinate[i].y);
                    }

                    Offset<ushort>[] table_index_vertex = new Offset<ushort>[mesh.TableIndexVertex.Length];
                    for (int i = 0; i < mesh.TableIndexVertex.Length; i++)
                    {
                        table_index_vertex[i] = new Offset<ushort>(mesh.TableIndexVertex[i]);
                    }

                    return ss.ssfb2.DataMesh.CreateDataMesh(fbb,
                        fbb.CreateVectorOfTables(table_coordinate),
                        fbb.CreateVectorOfTables(table_index_vertex));
                }
                else
                {
                    return ss.ssfb2.DataMesh.CreateDataMesh(fbb);
                }

            }

            //セルの格納
            public static Offset<ss.ssfb2.Cell> makeCell(FlatBufferBuilder fbb, Library_SpriteStudio6.Data.CellMap.Cell cell)
            {        
                //Cell単品の格納
                StringOffset _name = fbb.CreateString(cell.Name);

                var _rectangle = ss.ssfb2.Rect.CreateRect(fbb, 0, 0, 0, 0);

                var _pivot = ss.ssfb2.Vector2.CreateVector2(fbb, 0, 0);

                //メッシュの格納
                var _mesh = makeDataMesh(fbb,cell.Mesh);

                return ss.ssfb2.Cell.CreateCell(fbb, _name, _rectangle, _pivot, _mesh );
            }

            //セルマップ単体の格納
            public static Offset<ss.ssfb2.LibraryDataCellMap> makeLibraryDataCellMap(
                                                                                    FlatBufferBuilder fbb, 
                                                                                    Library_SpriteStudio6.Data.CellMap cellmap)
            {
                StringOffset _name = fbb.CreateString(cellmap.Name);
                var _size_original = ss.ssfb2.Vector2.CreateVector2(fbb ,cellmap.SizeOriginal.x , cellmap.SizeOriginal.y);

                int cell_num = cellmap.TableCell.Length;
                var _cell_array = new Offset<ss.ssfb2.Cell>[cell_num];

                for (int i = 0; i < cell_num; i++)
                {
                    _cell_array[i] = makeCell(fbb, cellmap.TableCell[i]);
                }

                var ret = ss.ssfb2.LibraryDataCellMap.CreateLibraryDataCellMap(
                    fbb,
                    _name,
                    _size_original,
                    fbb.CreateVectorOfTables(_cell_array)
                    );

                return ret;
            }

            //Cellmapリスト情報の格納
            public static Offset<ss.ssfb2.DataCellMap> makeDataCellMap(FlatBufferBuilder fbb , Script_SpriteStudio6_DataCellMap cellmap )
            {

                //Cellmapの格納元array
                //LibraryDataCellMap Cellmapの本体データ
                int cellmap_size = cellmap.TableCellMap.Length;
                var listoffset = new FlatBuffers.Offset<ss.ssfb2.LibraryDataCellMap>[cellmap_size];

                //セルマップリストの作成
                for (int i = 0; i < cellmap_size; i++)
                {
                    Offset<ss.ssfb2.LibraryDataCellMap> _offset = makeLibraryDataCellMap(fbb, cellmap.TableCellMap[i]);
                    listoffset[i] = _offset;
                }

                return ss.ssfb2.DataCellMap.CreateDataCellMap(fbb, 
                                                            (uint)cellmap.Version,
                                                            fbb.CreateVectorOfTables(listoffset));

            }


            public static Offset<ss.ssfb2.DataEffectParts> makeDataEffectParts(FlatBufferBuilder fbb , Library_SpriteStudio6.Data.Parts.Effect e)
            {
                return ss.ssfb2.DataEffectParts.CreateDataEffectParts(
                        fbb,
                        fbb.CreateString(e.Name),
                        (ushort)e.ID,
                        (ushort)e.IDParent,
                        ss.ssfb2.DataEffectParts.CreateTableIdChildVector(fbb, e.TableIDChild),
                        (ushort)e.Feature,
                        (ushort)e.IndexEmitter
                    );
            }

            public static VectorOffset makeDataEffectPartsTable(FlatBufferBuilder fbb , Library_SpriteStudio6.Data.Parts.Effect[] effectPart)
            {

                var table = new Offset<ss.ssfb2.DataEffectParts>[effectPart.Length];

                for (int i = 0; i < effectPart.Length; i ++ )
                {
                    Library_SpriteStudio6.Data.Parts.Effect e = effectPart[i];
                    table[i] = makeDataEffectParts(fbb, e);
                }

                return ss.ssfb2.DataEffect.CreateTablePartsVector(fbb, table);
            }

            public static Offset<ss.ssfb2.PatternEmit> makePatternEmit(FlatBufferBuilder fbb, Library_SpriteStudio6.Data.Effect.Emitter.PatternEmit p)
            {
                return ss.ssfb2.PatternEmit.CreatePatternEmit(
                        fbb,
                        p.IndexGenerate,
                        p.Duration,
                        p.Cycle
                    );
            }

            public static Offset<ss.ssfb2.DataEffectEmitter> makeDataEffectEmitter(FlatBufferBuilder fbb , Library_SpriteStudio6.Data.Effect.Emitter e)
            {

                var table_pattern_emit = new Offset<ss.ssfb2.PatternEmit>[e.TablePatternEmit.Length];
                for ( int i = 0; i < e.TablePatternEmit.Length; i ++ )
                {
                    table_pattern_emit[i] = makePatternEmit(fbb, e.TablePatternEmit[i]);
                }
                
                ss.ssfb2.DataEffectEmitter.CreateDataEffectEmitter(fbb,
                    (uint)e.FlagData,
                    (uint)e.OperationBlendTarget,
                    (ushort)e.IndexCellMap,
                    (ushort)e.IndexCell,
                    ss.ssfb2.RangeFloat.CreateRangeFloat( fbb , e.Angle.Main , e.Angle.Sub ),
                    ss.ssfb2.Vector2.CreateVector2(fbb ,e.GravityDirectional.x , e.GravityDirectional.y ),
                    ss.ssfb2.Vector2.CreateVector2(fbb, e.GravityPointPosition.x, e.GravityPointPosition.y),
                    e.GravityPointPower,
                    ss.ssfb2.RangeVector2.CreateRangeVector2( fbb , 
                                ss.ssfb2.Vector2.CreateVector2( fbb , e.Position.Main.x , e.Position.Main.y ),
                                ss.ssfb2.Vector2.CreateVector2(fbb, e.Position.Sub.x, e.Position.Sub.y) ),
                    //rotation
                    ss.ssfb2.RangeFloat.CreateRangeFloat( fbb , e.Rotation.Main , e.Rotation.Sub ),
                    //rotation_fluctuation
                    ss.ssfb2.RangeFloat.CreateRangeFloat(fbb, e.RotationFluctuation.Main, e.RotationFluctuation.Sub),
                    //rotation_fluctuation_rate
                    e.RotationFluctuationRate,
                    //rotation_fluctuation_rate
                    e.RotationFluctuationRateTime,

                    //rate_tangential_acceleration
                    ss.ssfb2.RangeFloat.CreateRangeFloat(fbb, e.RateTangentialAcceleration.Main, e.RateTangentialAcceleration.Sub),

                    //scale_start
                    ss.ssfb2.RangeVector2.CreateRangeVector2(fbb,
                                ss.ssfb2.Vector2.CreateVector2(fbb, e.ScaleStart.Main.x, e.ScaleStart.Main.y),
                                ss.ssfb2.Vector2.CreateVector2(fbb, e.ScaleStart.Sub.x, e.ScaleStart.Sub.y)),
                    //scale_rate_start
                    ss.ssfb2.RangeFloat.CreateRangeFloat(fbb, e.ScaleRateStart.Main, e.ScaleRateStart.Sub),

                    //scale_end
                    ss.ssfb2.RangeVector2.CreateRangeVector2(fbb,
                                ss.ssfb2.Vector2.CreateVector2(fbb, e.ScaleEnd.Main.x, e.ScaleEnd.Main.y),
                                ss.ssfb2.Vector2.CreateVector2(fbb, e.ScaleEnd.Sub.x, e.ScaleEnd.Sub.y)),
                    //scale_rate_end
                    ss.ssfb2.RangeFloat.CreateRangeFloat(fbb, e.ScaleRateEnd.Main, e.ScaleRateEnd.Sub),

                    //delay
                    e.Delay,

                    //color_vertex
                    ss.ssfb2.RangeColor.CreateRangeColor( fbb,
                                ss.ssfb2.Color.CreateColor( fbb ,   
                                                            e.ColorVertex.Main.r ,
                                                            e.ColorVertex.Main.g , 
                                                            e.ColorVertex.Main.b ,
                                                            e.ColorVertex.Main.a ),
                                ss.ssfb2.Color.CreateColor(fbb,     
                                                            e.ColorVertex.Sub.r ,
                                                            e.ColorVertex.Sub.g, 
                                                            e.ColorVertex.Sub.b, 
                                                            e.ColorVertex.Sub.a) ),

                    //color_vertex_fluctuation
                    ss.ssfb2.RangeColor.CreateRangeColor(fbb,
                                ss.ssfb2.Color.CreateColor(fbb, 
                                                            e.ColorVertexFluctuation.Main.r ,
                                                            e.ColorVertexFluctuation.Main.g, 
                                                            e.ColorVertexFluctuation.Main.b, 
                                                            e.ColorVertexFluctuation.Main.a),
                                ss.ssfb2.Color.CreateColor(fbb, 
                                                            e.ColorVertexFluctuation.Sub.r ,
                                                            e.ColorVertexFluctuation.Sub.g, 
                                                            e.ColorVertexFluctuation.Sub.b, 
                                                            e.ColorVertexFluctuation.Sub.a)),
                    //alpha_fade_start
                    e.AlphaFadeStart,

                    //alpha_fade_end
                    e.AlphaFadeEnd,

                    //speed
                    ss.ssfb2.RangeFloat.CreateRangeFloat(fbb, e.Speed.Main, e.Speed.Sub),

                    //speed_fluctuation
                    ss.ssfb2.RangeFloat.CreateRangeFloat(fbb, e.SpeedFluctuation.Main, e.SpeedFluctuation.Sub),

                    //turn_direction_fluctuation
                    e.TurnDirectionFluctuation,

                    e.SeedRandom,

                    e.DurationEmitter,
                    e.Interval,

                    //duration_particle
                    ss.ssfb2.RangeFloat.CreateRangeFloat(fbb, e.DurationParticle.Main, e.DurationParticle.Sub),

                    //count_particle_max
                    e.CountParticleMax,
                    //count_particle_emit
                    e.CountParticleEmit,

                    //count_parts_maximum
                    e.CountPartsMaximum,

                    //table_pattern_emit
                    ss.ssfb2.DataEffectEmitter.CreateTablePatternEmitVector(fbb , table_pattern_emit ),
                    ss.ssfb2.DataEffectEmitter.CreateTablePatternOffsetVector(fbb , e.TablePatternOffset ),
                    ss.ssfb2.DataEffectEmitter.CreateTableSeedParticleVector(fbb , e.TableSeedParticle )
                    );


                return ss.ssfb2.DataEffectEmitter.CreateDataEffectEmitter(fbb);
            }


            public static VectorOffset makeDataEffectEmitterTable(FlatBufferBuilder fbb, Library_SpriteStudio6.Data.Effect.Emitter[] emmiterPart)
            {
                var table = new Offset<ss.ssfb2.DataEffectEmitter>[emmiterPart.Length];

                for ( int i = 0; i < emmiterPart.Length; i ++ )
                {
                    table[i] = makeDataEffectEmitter(fbb, emmiterPart[i]);
                }

                return ss.ssfb2.DataEffect.CreateTableEmitterVector(fbb, table);
            }


            //DataEffect
            public static Offset<ss.ssfb2.DataEffect> makeDataEffect(FlatBufferBuilder fbb, Script_SpriteStudio6_DataEffect effect)
            {
                return ss.ssfb2.DataEffect.CreateDataEffect(
                        fbb,
                        (uint)effect.Version,
                        (int)effect.FlagData,
                        effect.SeedRandom,
                        effect.VersionRenderer,
                        effect.CountMaxParticle,
                        effect.CountFramePerSecond,
                        ss.ssfb2.Vector2.CreateVector2( fbb , effect.ScaleLayout.x , effect.ScaleLayout.y ),
                        makeDataEffectPartsTable( fbb , effect.TableParts ),
                        makeDataEffectEmitterTable( fbb , effect.TableEmitter ),
                        ss.ssfb2.DataEffect.CreateTableIndexEmitterOrderDrawVector(fbb, effect.TableIndexEmitterOrderDraw )
                    );
            }

        }
    }
}
