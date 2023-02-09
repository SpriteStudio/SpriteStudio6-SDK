/*!
 * \file	ssfontdesc.h
 * \author	CRI Middleware Co., Ltd.
*/
#ifndef SSFONTDESC_H
#define SSFONTDESC_H

#include "../sstypes.h"

namespace SpriteStudio
{


	/*!
	 * \class	SsFontDesc
	 * \brief	フォント設定
	*/
	class SsFontDesc
	{
	public:
		static const bool						DEFAULT_BITMAP;		//!< デフォルトビットマップ
		static const SsString					DEFAULT_FAMILY;		//!< デフォルトファミリ
		static const SsString					DEFAULT_CHAR_MAP;	//!< デフォルトキャラマップ
		static const int						DEFAULT_SIZE;		//!< デフォルトサイズ
		static const float						DEFAULT_SPACE;		//!< デフォルトスペース
		static const SsAnchorButton::Anchor		DEFAULT_ANCHOR;		//!< デフォルトアンカー

		//! コンストラクタ
		SsFontDesc();

		//! デストラクタ
		~SsFontDesc();

		SsFontDesc& operator =(const SsFontDesc& right);
		bool operator ==(const SsFontDesc& right) const;
		bool operator !=(const SsFontDesc& right) const;
		bool operator <(const SsFontDesc& right) const;
		bool operator >(const SsFontDesc& right) const;

		//! ビットマップを設定
		/*!
		 * \param	bBitmap		ビットマップ
		*/
		void setBitmap(bool bBitmap);

		//! ファミリを設定
		/*!
		 * \param	strFamily	ファミリ
		*/
		void setFamily(const SsString& strFamily);

		//! キャラマップを設定
		/*!
		 * \param	strCharMap	キャラマップ
		*/
		void setCharMap(const SsString& strCharMap);

		//! サイズを設定
		/*!
		 * \param	iSize		サイズ
		*/
		void setSize(int iSize);

		//! スペースを設定
		/*!
		 * \param	fSpace		スペース
		*/
		void setSpace(float fSpace);

		//! アンカーを設定
		/*!
		 * \param	eAnchor		アンカー
		*/
		void setAnchor(SsAnchorButton::Anchor eAnchor);

		//! ビットマップを取得
		/*!
		 * \return	ビットマップ
		*/
		bool isBitmap() const;

		//! ファミリを取得
		/*!
		 * \return	ファミリ
		*/
		SsString getFamily() const;

		//! キャラマップを取得
		/*!
		 * \return	キャラマップ
		*/
		SsString getCharMap() const;

		//! サイズを取得
		/*!
		 * \return	サイズ
		*/
		int getSize() const;

		//! スペースを取得
		/*!
		 * \return	スペース
		*/
		float getSpace() const;

		//! アンカーを取得
		/*!
		 * \return	アンカー
		*/
		SsAnchorButton::Anchor getAnchor() const;

		//! 更新が必要かどうかを取得
		/*!
		 * \return	更新が必要な場合に true
		*/
		bool takeUpdate();

	private:
		bool					m_bBitmap;		///< ビットマップ
		SsString					m_strFamily;	///< ファミリ
		SsString					m_strCharMap;	///< キャラマップ
		int						m_iSize;		///< サイズ
		float					m_fSpace;		///< スペース
		SsAnchorButton::Anchor	m_eAnchor;		///< アンカー

		bool					m_bUpdate;		///< 更新が必要かどうか
	};

};
#endif // SSFONTDESC_H
