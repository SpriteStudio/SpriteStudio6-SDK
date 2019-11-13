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

using ss.ssfb2;
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

#if true
                //test
                Offset<ss.ssfb2.DataCellMap>  cellmaplist_offset = makeDataCellMap(fbb, outdata.CellMap);
                fbb.Finish(cellmaplist_offset.Value);
#else
                Offset<ss.ssfb2.ProjectData>  ProjectData = makeProjectData(fbb, outdata);
                fbb.Finish(ProjectData.Value);
#endif

                //ファイル出力
                var writer = new BinaryWriter(new FileStream("c:\\temp\\test.fb2.cellmap", FileMode.Create));
                writer.Write(fbb.DataBuffer.ToFullArray());
                writer.Close();

                return true;
            }


            //プロジェクトデータの作成
            public static Offset<ss.ssfb2.ProjectData> makeProjectData(FlatBufferBuilder fbb , Script_SpriteStudio6_DataProject project )
            {

                Offset<ss.ssfb2.DataCellMap> cellmaplist_offset = makeDataCellMap(fbb, project.CellMap);

                return ss.ssfb2.ProjectData.CreateProjectData(fbb , 
                    (uint)project.Version,
                    cellmaplist_offset
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
                Offset<DataMesh> _mesh = makeDataMesh(fbb,cell.Mesh);

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
                FlatBuffers.Offset<LibraryDataCellMap>[] listoffset = new FlatBuffers.Offset<LibraryDataCellMap>[cellmap_size];

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
