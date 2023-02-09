/*!
 * \file	sspartvalue.cpp
 * \author	CRI Middleware Co., Ltd.
*/
#include "sspartvalue.h"

namespace SpriteStudio
{

	//! コンストラクタ
	SsPartValue::SsPartValue(const SsString& strTag)
	{
		m_strTag = strTag;
	}

	//! デストラクタ
	SsPartValue::~SsPartValue()
	{
	}

	//! タグを取得
	SsString SsPartValue::getTag()
	{
		return	m_strTag;
	}

	//! 複製
	SsPartValue* SsPartValue::duplicate()
	{
		return	new SsPartValue(m_strTag);
	}
};