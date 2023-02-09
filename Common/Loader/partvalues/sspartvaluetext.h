/*!
 * \file	sspartvaluetext.h
 * \author	CRI Middleware Co., Ltd.
*/
#ifndef SSPARTVALUETEXT_H
#define SSPARTVALUETEXT_H

//#define SSPARTVALUETEXT_SET_CMP
//#define SSPARTVALUETEXT_GET_CMP

#include "../sstypes.h"
#include "sspartvalue.h"
#include "../ssloader_ssce.h"
#include "../text/ssfontdesc.h"

#define IS_EDITOR (0)

//#include "ssfonttexture.h"
//#include "SsCellMap.h"
//#include "SsImage.h"

namespace SpriteStudio
{



	/*!
	 * \class	SsPartValueText
	 * \brief	テキストパーツパラメータ
	*/
	class SsPartValueText : public SsPartValue
	{
	public:
		static const SsString	TAG;	//!< デフォルトタグ

		//! コンストラクタ
		SsPartValueText();

		//! デストラクタ
		~SsPartValueText() override;

		//! 複製
		/*!
		 * \return	パーツパラメータ
		*/
		SsPartValue* duplicate() override;

		//! テキストを設定
		/*!
		 * \param	strText		テキスト
		*/
		void setText(const SsString& strText);

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

		//! スムースを設定
		/*!
		 * \param	bSmooth		スムース
		*/
		void setSmooth(bool bSmooth);

		//! マスクを設定
		/*!
		 * \param	bMask		マスク
		*/
		void setMask(bool bMask);

		//! 幅を設定
		/*!
		 * \param	iWidth		幅
		*/
		void setWidth(int iWidth);

		//! 高さを設定
		/*!
		 * \param	iHeight		高さ
		*/
		void setHeight(int iHeight);

#ifdef SSPARTVALUETEXT_SET_CMP
		//! 圧縮ビットマップ文字列を設定
		/*!
		 * \param	strCmp		圧縮ビットマップ文字列
		*/
		void setCmp(const SsString& strCmp);

		//! 圧縮ビットマップサイズを設定
		/*!
		 * \param	cmpSize		圧縮ビットマップサイズ
		*/
		void setCmpSize(const SsVector2& cmpSize);
#endif // SSPARTVALUETEXT_SET_CMP

		//! テキストを取得
		/*!
		 * \return	テキスト
		*/
		SsString getText() const;

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

		//! スムースを取得
		/*!
		 * \return	スムース
		*/
		bool isSmooth() const;

		//! マスクを取得
		/*!
		 * \return	マスク
		*/
		bool isMask() const;

		//! 幅を取得
		/*!
		 * \return	幅
		*/
		int getWidth() const;

		//! 高さを取得
		/*!
		 * \return	高さ
		*/
		int getHeight() const;

#ifdef SSPARTVALUETEXT_GET_CMP
		//! 圧縮ビットマップ文字列を取得
		/*!
		 * \return	圧縮ビットマップ文字列
		*/
		SsString getCmp();

		//! 圧縮ビットマップサイズを取得
		/*!
		 * \return	圧縮ビットマップサイズ
		*/
		SsVector2 getCmpSize();
#endif // SSPARTVALUETEXT_GET_CMP


		virtual bool needUpdate();

		virtual bool update();

		//! セルを取得
		/*!
		 * \return	セル
		*/
		virtual SsCell* getCell();


		const SsFontDesc& getFontDesc() {
			return m_FontDesc
				;
		}


	protected:
		SsString				m_strText;			//!< テキスト
		SsFontDesc				m_FontDesc;			//!< フォント設定
		bool					m_bSmooth;			//!< スムース
		bool					m_bMask;			//!< マスク
		int						m_iWidth;			//!< 幅
		int						m_iHeight;			//!< 高さ
#ifdef SSPARTVALUETEXT_SET_CMP
		SsString				m_strCmp;			//!< 圧縮ビットマップ文字列
		SsVector2				m_CmpSize;			//!< 圧縮ビットマップサイズ
#endif // SSPARTVALUETEXT_SET_CMP

		bool					m_bUpdate;			//!< 更新が必要かどうか

#if IS_EDITOR
		SsFontTexture* m_pFontTexture;		//!< フォントテクスチャ
		SsImage					m_Image;			//!< イメージ
#else
		//ISSTexture* m_pFontTexture;

#endif

#if 0
	public:

		SsCellMap				m_CellMap;			//!< セルマップ
		SsCell					m_Cell;				//!< セル
#endif
	};


};



#endif // SSPARTVALUETEXT_H
