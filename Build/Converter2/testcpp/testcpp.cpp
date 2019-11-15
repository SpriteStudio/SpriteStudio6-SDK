// testcpp.cpp : このファイルには 'main' 関数が含まれています。プログラム実行の開始と終了がそこで行われます。
//

#include "pch.h"
#include <iostream>
#include "../ssfb2_generated.h"
#include <iostream>
#include <fstream>
#include <iterator>




void Catalog(const ss::ssfb2::Catalog* catalog)
{

}

void ColorLabel(const ss::ssfb2::ColorLabel* label)
{

}

void Label(const ss::ssfb2::Label* label)
{

}

void BindMesh(const ss::ssfb2::BindMesh* Mesh)
{

}

void DataModelParts(const ss::ssfb2::DataModelParts* Model)
{
	std::cout << "--------Model Parts--------" << std::endl;

	std::cout << std::dec;
	std::cout << "name :" << Model->name()->str() << std::endl;
	std::cout << "id :" << Model->id() << std::endl;
	std::cout << "id_parent :" << Model->id_parent() << std::endl;

	std::cout << "+table_id_children" << std::endl;
	for (auto i = 0; i < Model->table_id_children()->Length(); i++)
	{
		auto childid = (*Model->table_id_children())[i];
		std::cout << "--children id : " << childid  << std::endl;
	}

	std::cout << "feature :" << (int)Model->feature() << std::endl;
	std::cout << "count_mesh :" << Model->count_mesh() << std::endl;	
	
	BindMesh(Model->mesh());
	ColorLabel(Model->label_color());

	std::cout << "operation_blend_target :" << (int)Model->operation_blend_target() << std::endl;
	std::cout << "shape_collision :" << (int)Model->shape_collision() << std::endl;
	std::cout << "size_collision :" << Model->size_collision() << std::endl;

	std::cout << std::hex;
	std::cout << "index_animation_pack_under_control :" << Model->index_animation_pack_under_control() << std::endl;
	std::cout << "index_effect_under_control :" << Model->index_effect_under_control() << std::endl;
	std::cout << std::dec;

	std::cout << "name_animation_under_control :" << Model->name_animation_under_control()->str() << std::endl;


}

void DataAnimationParts(const ss::ssfb2::DataAnimationParts* parts)
{

}



void LibraryDataAnimation(const ss::ssfb2::LibraryDataAnimation* dataanime)
{
	std::cout << "--------LibraryDataAnimation--------" << std::endl;

	std::cout << std::dec;
	std::cout << "name :" << dataanime->name()->str() << std::endl;
	std::cout << "frame_per_second :" << dataanime->frame_per_second() << std::endl;
	std::cout << "count_frame :" << dataanime->count_frame() << std::endl;
	std::cout << "size_canvas_x :" << dataanime->size_canvas_x() << std::endl;
	std::cout << "size_canvas_y :" << dataanime->size_canvas_y() << std::endl;

	std::cout << "frame_valid_start :" << dataanime->frame_valid_start() << std::endl;
	std::cout << "frame_valid_end :" << dataanime->frame_valid_end() << std::endl;
	std::cout << "count_frame_valid :" << dataanime->count_frame_valid() << std::endl;

	std::cout << "depth_ik :" << dataanime->depth_ik() << std::endl;
	std::cout << "mode_sort :" << (int)dataanime->mode_sort() << std::endl;

	for (auto i = 0 ; i < dataanime->table_label()->Length() ; i ++ )
		Label( (*dataanime->table_label())[i] );

	for (auto i = 0; i < dataanime->table_parts()->Length(); i++)
		DataAnimationParts((*dataanime->table_parts())[i]);


}

std::string ssfbVector2(const ss::ssfb2::Vector2* vector)
{
	std::string ret = "vector2(";

	ret += std::to_string(vector->x()) + ",";
	ret += std::to_string(vector->y()) + ")";

	return ret;
}

std::string ssfbRect(const ss::ssfb2::Rect* rect)
{
	std::string ret = "rect(";

	ret += std::to_string(rect->x_min()) + ",";
	ret += std::to_string(rect->y_min()) + ",";
	ret += std::to_string(rect->x_max()) + ",";
	ret += std::to_string(rect->y_max()) + ")";

	return ret;
}

void UserData(const ss::ssfb2::UserData* userdata)
{

	std::cout << "--------UserData--------" << std::endl;
	std::cout << "flags :" << (int)userdata->flags() << std::endl;
	std::cout << "number_int :" << userdata->number_int() << std::endl;
	std::cout << "rectangle :" << ssfbRect(userdata->rectangle()) << std::endl;
	std::cout << "coordinate :" << ssfbVector2(userdata->coordinate()) << std::endl;
	std::cout << "text :" << userdata->text() << std::endl;


}


void DataSetup(const ss::ssfb2::DataSetup* setup)
{

	UserData(setup->user_data());

}



//スキーマ　DataAnimation
void DataAnimation(const ss::ssfb2::DataAnimation* animepack)
{

	std::cout << "------DataAnimation--------" << std::endl;

	std::cout << std::hex;
	std::cout << "ssae version : " << animepack->version() << std::endl;
	std::cout << std::dec;

	//table_parts:[DataModelParts]; 
	for (auto i = 0; i < animepack->table_parts()->Length(); i++)
	{
		auto table = animepack->table_parts();
		DataModelParts( (*table)[i] ) ;
	}

	Catalog(animepack->catalog_parts());

	for (auto i = 0; i < animepack->table_animation()->Length(); i++)
	{
		LibraryDataAnimation(  (*animepack->table_animation())[i] );
	}

	for (auto i = 0; i < animepack->table_animation_parts_setup()->Length(); i++)
	{
		DataSetup((*animepack->table_animation_parts_setup())[i]);
	}


}



void DataAnimationList(const ss::ssfb2::ProjectData* proj)
{

	//animation:[DataAnimation]; == ssae file
	//なので DataAnimationのVectorが取れる
	// flatbuffer::Vector<>

	//Projectに含まれるアニメーションパックのリストとなる

	auto list_of_animepack = proj->animation();

	//Vectorの長さ取得
	int size = list_of_animepack->Length();

	std::cout << "Anime Pack List Size = " << size << std::endl;

	//要素取得
	for (auto i = 0; i < list_of_animepack->Length(); i++)
	{
		//DataAnimation
		auto dataAnimation = (*list_of_animepack)[i];
		DataAnimation(dataAnimation);
	}


}


int main()
{
	//ssfbファイルの読み込み
	std::string filename = "box_00_00.ssfb.ssbp2";

	std::ifstream ifs(filename, std::ios::in | std::ios::binary);
	std::istreambuf_iterator<char> its(ifs) ;
	std::istreambuf_iterator<char> ite;
	std::vector<char> buf( its,ite);
	
	//project読み込み
	const ss::ssfb2::ProjectData* proj = ss::ssfb2::GetProjectData(buf.data());

	DataAnimationList(proj);



	return 0;

}

// プログラムの実行: Ctrl + F5 または [デバッグ] > [デバッグなしで開始] メニュー
// プログラムのデバッグ: F5 または [デバッグ] > [デバッグの開始] メニュー

// 作業を開始するためのヒント: 
//    1. ソリューション エクスプローラー ウィンドウを使用してファイルを追加/管理します 
//   2. チーム エクスプローラー ウィンドウを使用してソース管理に接続します
//   3. 出力ウィンドウを使用して、ビルド出力とその他のメッセージを表示します
//   4. エラー一覧ウィンドウを使用してエラーを表示します
//   5. [プロジェクト] > [新しい項目の追加] と移動して新しいコード ファイルを作成するか、[プロジェクト] > [既存の項目の追加] と移動して既存のコード ファイルをプロジェクトに追加します
//   6. 後ほどこのプロジェクトを再び開く場合、[ファイル] > [開く] > [プロジェクト] と移動して .sln ファイルを選択します
