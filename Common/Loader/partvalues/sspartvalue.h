/*!
 * \file	sspartvalue.h
 * \author	CRI Middleware Co., Ltd.
*/
#ifndef SSPARTVALUE_H
#define SSPARTVALUE_H

//#include "wtlibstring.h"
//#include "sstypes.h"
#include "../sstypes.h"

namespace SpriteStudio
{
	/*!
	 * \class	SsPartValue
	 * \brief	パーツパラメータ
	*/
	class SsPartValue
	{
	public:
		//! コンストラクタ
		/*!
		 * \param	strTag		タグ
		*/
		SsPartValue(const SsString& strTag = "unknown");

		//! デストラクタ
		virtual ~SsPartValue();

		//! タグを取得
		/*!
		 * \return	タグ
		*/
		SsString getTag();

		//! 複製
		/*!
		 * \return	パーツパラメータ
		*/
		virtual SsPartValue* duplicate();

	private:
		SsString	m_strTag;	//!< タグ
	};
};

#endif // SSPARTVALUE_H
