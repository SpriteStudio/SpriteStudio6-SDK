#include "ssloader_ssae.h"


SsAnimePack*	ssloader_ssae::Load(const std::string& filename )
{

	SsAnimePack* anime = new SsAnimePack();

	XMLDocument xml;
	if ( XML_SUCCESS == xml.LoadFile( filename.c_str() ) )
	{
		SsXmlIArchiver ar( xml.GetDocument() , "SpriteStudioAnimePack" );
		anime->__Serialize( &ar );
	}else{
		delete anime;
		anime = 0;
	}

	return anime;
}


SsAnimation*	SsAnimePack::findAnimation(SsString& name)
{

	for ( std::vector<SsAnimation*>::iterator itr = animeList.begin()
		; itr != animeList.end() ; ++itr )
	{
		if ( (*itr)->name == name )
		{
			return (*itr);
		}
	}
	return 0;
}




void	SsMeshBind::loader(ISsXmlArchiver* ar)
{

	const char* Text = ar->getxml()->GetText();

	//Meshのバインドリストはテキスト要素が空の場合がある（並び順も意味のあるデータ列なため）
	if (Text)
	{
		SsString str = Text;

		SsStringTokenizer tokenizer(str, ',');


		for (int i = 0; i < tokenizer.tokenNum(); i++)
		{
			std::string getstr;
			if (tokenizer.get(&getstr))
			{
				SsMeshBindInfo info;
				info.fromString(getstr);
				meshVerticesBindArray.push_back(info);
			}
		}
	}


}


void	SsMeshBindInfo::fromString(SsString str)
{

	memset(weight, 0, SSMESHPART_BONEMAX *  sizeof(float));
	memset(boneIndex, 0 , SSMESHPART_BONEMAX * sizeof(int));
	bindBoneNum = 0;


	SsStringTokenizer tokenizer(str, ' ');

	bool ret = true;
	int cnt = 0;

	tokenizer.get(&bindBoneNum);


	for (int i = 0; i < bindBoneNum; i++)
	{
		tokenizer.get(&boneIndex[i]);
		tokenizer.get(&weight[i]);
		tokenizer.get(&offset[i].x);
		tokenizer.get(&offset[i].y);
	}
}


