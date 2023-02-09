#include "ssloader_ssae.h"

//#include "partvalues/ssshape.h"
//#include "partvalues/sspartvalueshape.h"
//#include "partvalues/sspartvaluetext.h"

namespace SpriteStudio
{
	//const SsString 	SsPartValueTextInfo::TAG = "text";
	//const SsString 	SsPartValueShapeInfo::TAG = "shape";
	//const SsString 	SsPartValueNineSliceInfo::TAG = "nines";


	///シリアライズのための宣言です。
//	SPRITESTUDIO6SDK_SERIALIZE_BLOCK
	void SsPart::__Serialize(ISsXmlArchiver* ar)
	{
		SPRITESTUDIO6SDK_SSAR_DECLARE(name);
		SPRITESTUDIO6SDK_SSAR_DECLARE(arrayIndex);
		SPRITESTUDIO6SDK_SSAR_DECLARE(parentIndex);

		SPRITESTUDIO6SDK_SSAR_DECLARE_ENUM(type);
		SPRITESTUDIO6SDK_SSAR_DECLARE_ENUM(boundsType);
		SPRITESTUDIO6SDK_SSAR_DECLARE_ENUM(inheritType);
		SPRITESTUDIO6SDK_SSAR_DECLARE_ENUM(alphaBlendType);
		SPRITESTUDIO6SDK_SSAR_DECLARE(show);
		SPRITESTUDIO6SDK_SSAR_DECLARE(locked);
		SPRITESTUDIO6SDK_SSAR_DECLARE(colorLabel);
		SPRITESTUDIO6SDK_SSAR_DECLARE(maskInfluence);

		SPRITESTUDIO6SDK_SSAR_DECLARE(refAnimePack);
		SPRITESTUDIO6SDK_SSAR_DECLARE(refAnime);

		SPRITESTUDIO6SDK_SSAR_DECLARE(refEffectName);

		SPRITESTUDIO6SDK_SSAR_DECLARE(boneLength);
		SPRITESTUDIO6SDK_SSAR_DECLARE(bonePosition);
		SPRITESTUDIO6SDK_SSAR_DECLARE(boneRotation);
		SPRITESTUDIO6SDK_SSAR_DECLARE(weightPosition);
		SPRITESTUDIO6SDK_SSAR_DECLARE(weightImpact);
		SPRITESTUDIO6SDK_SSAR_DECLARE(meshWeightType);
		SPRITESTUDIO6SDK_SSAR_DECLARE(meshWeightStrong);
		SPRITESTUDIO6SDK_SSAR_DECLARE(IKDepth);
		SPRITESTUDIO6SDK_SSAR_DECLARE_ENUM(IKRotationArrow);

		//継承率後に改良を実施
		if (ar->getType() == EnumSsArchiver::in)
		{
			libXML::XMLElement* e = ar->getxml()->FirstChildElement("ineheritRates");
			if (e)
			{
				libXML::XMLElement* ec = e->FirstChildElement();
				while (ec)
				{
					//継承設定の取得
					const char* tag = ec->Value();
					SsAttributeKind::_enum enumattr;

					__StringToEnum_(tag , enumattr);
					inheritRates[(int)enumattr] = (float)atof(ec->GetText());
					ec = ec->NextSiblingElement();
				}
			}
		}

		if (type == SsPartType::shape)
		{
			SsPartValueShape* value = new SsPartValueShape();
			SsString	shapeType;
			bool		shapeMask;
			SPRITESTUDIO6SDK_SSAR_DECLARE_EX("shapeType" , shapeType);
			SPRITESTUDIO6SDK_SSAR_DECLARE_EX("shapeMask" , shapeMask);

			value->setType(SsPartValueShape::toType(shapeType.c_str()));
			value->setMask(shapeMask);

			m_pPartValueInfo.reset(value);
		}

		//failed |= !SS_MAKE_NVP(ar, text); TODO when テキストパーツ実装時
		if (type == SsPartType::text)
		{
			bool		textBitmap;
			SsString	textFamily;
			SsString	textCharMap;
			int			textSize;
			float		textSpace;
			bool		textSmooth;
			bool		textMask;
			int			textWidth;
			int			textHeight;
			int			eAnchor;
			SsString	text;
			SsPartValueText* value = new SsPartValueText();

			//value->text = text;
			SPRITESTUDIO6SDK_SSAR_DECLARE_EX("text", text);

			SPRITESTUDIO6SDK_SSAR_DECLARE_EX( "textBitmap", textBitmap);
			SPRITESTUDIO6SDK_SSAR_DECLARE_EX( "textFamily" , textFamily);
			SPRITESTUDIO6SDK_SSAR_DECLARE_EX( "textCharMap" , textCharMap);
			SPRITESTUDIO6SDK_SSAR_DECLARE_EX( "textSize" , textSize);
			SPRITESTUDIO6SDK_SSAR_DECLARE_EX( "textSpace" , textSpace);
			SPRITESTUDIO6SDK_SSAR_DECLARE_EX( "textSmooth" , textSmooth);
			SPRITESTUDIO6SDK_SSAR_DECLARE_EX( "textMask" , textMask);
			SPRITESTUDIO6SDK_SSAR_DECLARE_EX( "textWidth" , textWidth);
			SPRITESTUDIO6SDK_SSAR_DECLARE_EX( "textHeight" , textHeight);
			SPRITESTUDIO6SDK_SSAR_DECLARE_EX( "eAnchor" , eAnchor);

			value->setBitmap(textBitmap);
			value->setFamily(textFamily.c_str());
			value->setCharMap(textCharMap.c_str());
			value->setSize(textSize);
			value->setSpace(textSpace);
			value->setSmooth(textSmooth);
			value->setMask(textMask);
			value->setWidth(textWidth);
			value->setHeight(textHeight);
			value->setAnchor((SsAnchorButton::Anchor)eAnchor);
			value->setText(text);
			//this->text = text;

			m_pPartValueInfo.reset(value);

		}


		if (type == SsPartType::nines)
		{
			SsPartValueNines* value = new SsPartValueNines();
			int			ninesMarginL;
			int			ninesMarginR;
			int			ninesMarginT;
			int			ninesMarginB;
			int			ninesFillMode;
			bool		ninesMask;

			SPRITESTUDIO6SDK_SSAR_DECLARE_EX( "ninesMarginL" , ninesMarginL);
			SPRITESTUDIO6SDK_SSAR_DECLARE_EX( "ninesMarginR" , ninesMarginR);
			SPRITESTUDIO6SDK_SSAR_DECLARE_EX( "ninesMarginT" , ninesMarginT);
			SPRITESTUDIO6SDK_SSAR_DECLARE_EX( "ninesMarginB" , ninesMarginB);
			SPRITESTUDIO6SDK_SSAR_DECLARE_EX( "ninesFillMode" , ninesFillMode);
			SPRITESTUDIO6SDK_SSAR_DECLARE_EX( "ninesMask" , ninesMask);

			value->setMargin(SsMargins(ninesMarginL, ninesMarginT, ninesMarginR, ninesMarginB));
			value->setFillMode(ninesFillMode);
			value->setMask(ninesMask);

			m_pPartValueInfo.reset(value);
		}


}



SsAnimePack*	ssloader_ssae::Load(const std::string& filename )
{

	SsAnimePack* anime = new SsAnimePack();

	libXML::XMLDocument xml;
	if ( libXML::XML_SUCCESS == xml.LoadFile( filename.c_str() ) )
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

	memset(weight, 0, SSMESHBIND_BONEMAX *  sizeof(float));
	memset(boneIndex, 0 , SSMESHBIND_BONEMAX * sizeof(int));
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


}	// namespace SpriteStudio
