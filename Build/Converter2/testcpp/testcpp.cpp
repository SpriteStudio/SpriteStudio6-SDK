// testcpp.cpp : このファイルには 'main' 関数が含まれています。プログラム実行の開始と終了がそこで行われます。
//

#include "pch.h"
#include <iostream>
#include "../ssfb2_generated.h"
#include <iostream>
#include <fstream>
#include <iterator>


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
	
	auto anime = proj->animation();

	int size = anime->Length();
	const ss::ssfb2::DataAnimation* a = anime->Get(0);

	{
		const int& v = a->version();
		const auto& part = a->table_parts();

	}
	

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
