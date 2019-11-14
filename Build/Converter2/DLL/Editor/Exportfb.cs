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

public static partial class LibraryEditor_SpriteStudio6
{
    public static partial class Import
    {

        /// <summary>
        /// FlatBufferへの格納
        /// </summary>
        public static partial class ExportSSFB2
        {
            public static bool output_ssfb2(Script_SpriteStudio6_DataProject outdata)
            {
                var fbb = new FlatBufferBuilder(1);

#if false
                //test
                Offset<ss.ssfb2.DataCellMap>  cellmaplist_offset = makeDataCellMap(fbb, outdata.CellMap);
                fbb.Finish(cellmaplist_offset.Value);
#else
                Offset<ss.ssfb2.ProjectData>  ProjectData = makeProjectData(fbb, outdata);
                fbb.Finish(ProjectData.Value);
#endif

                //ファイル出力
                var writer = new BinaryWriter(new FileStream("c:\\temp\\test.fb2", FileMode.Create));
                writer.Write(fbb.DataBuffer.ToFullArray());
                writer.Close();

                return true;
            }


            //プロジェクトデータの作成
            public static Offset<ss.ssfb2.ProjectData> makeProjectData(FlatBufferBuilder fbb , Script_SpriteStudio6_DataProject project )
            {

                //Offset<ss.ssfb2.DataCellMap> cellmaplist_offset = makeDataCellMap(fbb, project.CellMap);

                Offset<ss.ssfb2.DataAnimation>[] animelist = new Offset<ss.ssfb2.DataAnimation>[project.Animation.Length];
                for(int i =0; i < project.Animation.Length; i ++)
                {
                    animelist[i] = makeDataAnimation(fbb, project.Animation[i]);
                }

                return ss.ssfb2.ProjectData.CreateProjectData(fbb,
                    (uint)project.Version,
                    makeDataCellMap(fbb, project.CellMap),
                    ss.ssfb2.ProjectData.CreateAnimationVector(fbb, animelist)
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
                Offset<ss.ssfb2.Vertex>[] _vertex = new Offset<ss.ssfb2.Vertex>[v.Length];

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
                Offset<ss.ssfb2.Vector2>[] ret = new Offset<ss.ssfb2.Vector2>[array_vec2.Length];
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
                    ss.ssfb2.BindMesh.CreateTableRateUvVector(fbb, convertArrayVector2(fbb, bindmesh.TableRateUV)),
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

            static Offset<ss.ssfb2.DataAnimationParts> makeDataAnimationParts(FlatBufferBuilder fbb , Library_SpriteStudio6.Data.Animation.Parts _parts)
            {
                return ss.ssfb2.DataAnimationParts.CreateDataAnimationParts(fbb);
            }


            static Offset<ss.ssfb2.LibraryDataAnimation> makeLibraryDataAnimation(FlatBufferBuilder fbb , Library_SpriteStudio6.Data.Animation animeData)
            {


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
                    animeData.TableLabel,
                    animeData.TableParts
                    
                    );
            }


            //DataAnimation
            public static Offset<ss.ssfb2.DataAnimation> makeDataAnimation(FlatBufferBuilder fbb , Script_SpriteStudio6_DataAnimation anime)
            {

                Offset<ss.ssfb2.DataModelParts>[] modelparts = new Offset<ss.ssfb2.DataModelParts>[anime.TableParts.Length];

                Offset<ss.ssfb2.LibraryDataAnimation>[] table_animation = new Offset<ss.ssfb2.LibraryDataAnimation>[anime.TableAnimation.Length];
                for ( int i= 0; i < anime.TableAnimation.Length; i++ )
                {
                    table_animation[i] = makeLibraryDataAnimation(fbb, anime.TableAnimation[i]);
                }

                Offset<ss.ssfb2.DataSetup>[] table_animation_parts_setup = new Offset<ss.ssfb2.DataSetup>[anime.TableAnimationPartsSetup.Length];
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

                Offset<ss.ssfb2.Rect> _rectangle = ss.ssfb2.Rect.CreateRect(fbb, 0, 0, 0, 0);

                Offset<ss.ssfb2.Vector2> _pivot = ss.ssfb2.Vector2.CreateVector2(fbb, 0, 0);

                //メッシュの格納
                Offset<ss.ssfb2.DataMesh> _mesh = makeDataMesh(fbb,cell.Mesh);

                return ss.ssfb2.Cell.CreateCell(fbb, _name, _rectangle, _pivot, _mesh );
            }

            //セルマップ単体の格納
            public static Offset<ss.ssfb2.LibraryDataCellMap> makeLibraryDataCellMap(
                                                                                    FlatBufferBuilder fbb, 
                                                                                    Library_SpriteStudio6.Data.CellMap cellmap)
            {
                StringOffset _name = fbb.CreateString(cellmap.Name);
                Offset<ss.ssfb2.Vector2> _size_original = ss.ssfb2.Vector2.CreateVector2(fbb ,cellmap.SizeOriginal.x , cellmap.SizeOriginal.y);

                int cell_num = cellmap.TableCell.Length;
                Offset<ss.ssfb2.Cell>[] _cell_array = new Offset<ss.ssfb2.Cell>[cell_num];

                for (int i = 0; i < cell_num; i++)
                {
                    _cell_array[i] = makeCell(fbb, cellmap.TableCell[i]);
                }

                Offset<ss.ssfb2.LibraryDataCellMap> ret = ss.ssfb2.LibraryDataCellMap.CreateLibraryDataCellMap(
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
                FlatBuffers.Offset<ss.ssfb2.LibraryDataCellMap>[] listoffset = new FlatBuffers.Offset<ss.ssfb2.LibraryDataCellMap>[cellmap_size];

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

        }
    }
}
