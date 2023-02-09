/*!
 * \file	sspartvalueshape.cpp
 * \author	CRI Middleware Co., Ltd.
*/
#include "sspartvalueshape.h"

#include "../shape/ssshapetriangle.h"
#include "../shape/ssshaperectangle.h"
#include "../shape/ssshapearrow.h"
#include "../shape/ssshapestar.h"


namespace SpriteStudio
{


	const SsString	SsPartValueShape::TAG = "shape";	//!< デフォルトタグ

	std::map<SsPartValueShape::Type, SsString>	SsPartValueShape::s_mapFromType;		//!< タイプから文字列への変換マップ
	std::map<SsPartValueShape::Type, SsString>	SsPartValueShape::s_mapFromTypeName;	//!< タイプ名から文字列への変換マップ



	//! 有効なタイプのリストを取得
	SsStringList SsPartValueShape::getAvailableTypeList()
	{
		SsStringList		list;

		if (s_mapFromType.empty()) {
			initMap();
		}

		//list = s_mapFromType.values();

		for (auto item : s_mapFromType)
		{
			list.push_back( item.second );
		}

		return	list;
	}

	//! 有効なタイプ名のリストを取得
	SsStringList SsPartValueShape::getAvailableTypeNameList()
	{
		SsStringList		list;

		if (s_mapFromTypeName.empty()) {
			initMap();
		}

		//list = s_mapFromTypeName.values();
		for (auto item : s_mapFromTypeName)
		{
			list.push_back(item.second);
		}

		return	list;
	}

	//! 文字列をタイプに変換
	SsPartValueShape::Type SsPartValueShape::toType(const SsString& strType)
	{
		if (s_mapFromType.empty()) {
			initMap();
		}

		//return	s_mapFromType.key(strType, T_Unknown);

		auto result = std::find_if(
			s_mapFromType.begin(),
			s_mapFromType.end(),
			[strType](const auto& mo) {return mo.second == strType; });

		if (result != s_mapFromType.end())
		{
			SsPartValueShape::Type foundkey = result->first;
			return foundkey;
		}
		return T_Unknown;
	}

	//! タイプを文字列に変換
	SsString SsPartValueShape::fromType(Type eType)
	{
		if (s_mapFromType.empty()) {
			initMap();
		}
		if (s_mapFromType.count(eType) > 0)
		{
			return s_mapFromType[eType];
		}
		return "unknown";
//		return	s_mapFromType.value(eType, "unknown");
	}

	//! コンストラクタ
	SsPartValueShape::SsPartValueShape() : SsPartValue(TAG)
		, m_pShape(nullptr)
	{
		setType(T_Rectangle);

		setMask(false);

		m_bUpdate = true;
	}

	//! デストラクタ
	SsPartValueShape::~SsPartValueShape()
	{
		if (m_pShape) {
			delete m_pShape;
			m_pShape = nullptr;
		}
	}

	//! 複製
	SsPartValue* SsPartValueShape::duplicate()
	{
		SsPartValueShape* pDup = new SsPartValueShape();

		pDup->setType(m_eType);

		pDup->setMask(m_bMask);

		return	pDup;
	}

	//! タイプを設定
	void SsPartValueShape::setType(Type eType)
	{
		if (m_eType != eType) {
			m_eType = eType;

			changeShape(eType);

			m_bUpdate = true;
		}
	}

	//! マスクを設定
	void SsPartValueShape::setMask(bool bMask)
	{
		if (m_bMask != bMask) {
			m_bMask = bMask;
		}
	}

	//! サイズを設定
	void SsPartValueShape::setSize(const SsSizeF& size)
	{
		if (m_pShape) {
			m_pShape->setSize(size);

			m_bUpdate = true;
		}
	}

	//! タイプを取得
	SsPartValueShape::Type SsPartValueShape::getType() const
	{
		return	m_eType;
	}

	//! マスクを取得
	bool SsPartValueShape::isMask() const
	{
		return	m_bMask;
	}

	bool SsPartValueShape::update()
	{
		bool	bUpdate = (m_bUpdate);

		if (!bUpdate) {
			return	false;
		}

		if (m_pShape) {
			m_pShape->update();
		}

		m_bUpdate = false;

		return	true;
	}

	//! シェイプを取得
	const SsShape* SsPartValueShape::getShape()
	{
		if (!update()) {
			return	m_pShape;
		}

		return	m_pShape;
	}

	//! マップを初期化
	void SsPartValueShape::initMap()
	{
		s_mapFromType.clear();

		//	s_mapFromType[T_Unknown] = "unknown";
		s_mapFromType[T_Triangle] = "triangle";
		s_mapFromType[T_Rectangle] = "rectangle";
		s_mapFromType[T_Arrow] = "arrow";
		s_mapFromType[T_Star] = "star";

		s_mapFromTypeName.clear();

#if 0	//外側で定義してもらう
		//	s_mapFromTypeName[T_Unknown] = SsGUIString( "ShapeTypeUnknown" ).c_str();
		s_mapFromTypeName[T_Triangle] = SsGUIString("ShapeTypeTriangle").c_str();
		s_mapFromTypeName[T_Rectangle] = SsGUIString("ShapeTypeRectangle").c_str();
		s_mapFromTypeName[T_Arrow] = SsGUIString("ShapeTypeArrow").c_str();
		s_mapFromTypeName[T_Star] = SsGUIString("ShapeTypeStar").c_str();
#endif
	}


	//! シェイプを切替
	void SsPartValueShape::changeShape(Type eType)
	{
		if (m_pShape) {
			delete m_pShape;
			m_pShape = nullptr;
		}

		switch (eType) {
		case T_Triangle:
			m_pShape = new SsShapeTriangle();
			break;
		case T_Rectangle:
			m_pShape = new SsShapeRectangle();
			break;
		case T_Arrow:
			m_pShape = new SsShapeArrow();
			break;
		case T_Star:
			m_pShape = new SsShapeStar();
			break;
		default:
			break;
		}
	}


};