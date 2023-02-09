/*!
 * \file	ssfonttexture.h
 * \author	CRI Middleware Co., Ltd.
*/
#ifndef SSFONTTEXTURE_H
#define SSFONTTEXTURE_H

//#define SSFONTTEXTURE_SET_BITMAP
//#define SSFONTTEXTURE_GET_BITMAP

//#include <QLabel>
#include "sstypes.h"

#include "../Loader/text/ssfontdesc.h"
#include "../Helper/SsTextureFactory.h"
#include "../Loader/ssloader_sspj.h"

//#include "SsOpenGLTexture.h"
//#include "SsOpenGLImage.h"


namespace SpriteStudio
{
	/*!
	 * \class	SsFontTexture
	 * \brief	フォントテクスチャ
	*/
	class SsFontTexture
	{
	public:
		//! コンストラクタ
		SsFontTexture();

		//! デストラクタ
		~SsFontTexture();

		//! テキストを設定
		/*!
		 * \param	strText		テキスト
		*/
		void setText(const SsString& strText);

		//! フォント設定を設定
		/*!
		 * \param	fontDesc	フォント設定
		*/
		void setFontDesc(const SsFontDesc& fontDesc);

		//! グリッドサイズを設定
		/*!
		 * \param	size		グリッドサイズ
		*/
		void setGridSize(const SsSize& size);

		//! 固定サイズを設定
		/*!
		 * \param	size		固定サイズ
		*/
		void setFixedSize(const SsSize& size);

		//! テキストを取得
		/*!
		 * \return	テキスト
		*/
		SsString getText() const;

		//! フォント設定を取得
		/*!
		 * \return	フォント設定
		*/
		SsFontDesc getFontDesc() const;

		//! グリッドサイズを取得
		/*!
		 * \return	グリッドサイズ
		*/
		SsSize getGridSize() const;

		//! 固定サイズを取得
		/*!
		 * \return	固定サイズ
		*/
		SsSize getFixedSize() const;

		//! 更新
		void update();

		//! イメージ幅を取得
		/*!
		 * \return	イメージ幅
		*/
		int getImageWidth() const;

		//! イメージ高さを取得
		/*!
		 * \return	イメージ高さ
		*/
		int getImageHeight() const;

		//! テクスチャ幅を取得
		/*!
		 * \return	テクスチャ幅
		*/
		int getTextureWidth() const;

		//! テクスチャ高さを取得
		/*!
		 * \return	テクスチャ高さ
		*/
		int getTextureHeight() const;

		//! ＧＬテクスチャを取得
		/*!
		 * \return	ＧＬテクスチャ
		*/
		ISSTexture* getTexture() const;


		//void setProject(SsProject* proj) {
		//	m_myProject = proj;
		//}

		//void setCharMap(SsCharMap* charmap, SsRawImage* charamapIma) {
		void setCharMap(SsCharMap* charmap ) {
			m_pCharMap = charmap;
		}

		void setPartValue(SsPartValueText* v)
		{
			m_valueText = v;
		}

		SsCell* getCell() {
			return &m_Cell;
		}

		SsCellMap* getCellMap() {
			return &m_CellMap;
		}

#ifdef SSFONTTEXTURE_SET_BITMAP
		//! イメージを設定
		/*!
		 * \param	pImage	イメージ
		*/
		void setBitmap(QImage* pImage);
#endif // SSFONTTEXTURE_SET_BITMAP

#ifdef SSFONTTEXTURE_GET_BITMAP
		//! イメージを取得
		/*!
		 * \return	イメージ
		*/
		SsImage* getBitmap() const;
#endif // SSFONTTEXTURE_GET_BITMAP

	private:
		SsString			m_strText;			//!< テキスト
		SsFontDesc			m_FontDesc;			//!< フォント設定
		SsSize				m_GridSize;			//!< グリッドサイズ
		SsSize				m_FixedSize;		//!< 固定サイズ

		int					m_iImageWidth;		//!< イメージ幅
		int					m_iImageHeight;		//!< イメージ高さ
		int					m_iTextureWidth;	//!< テクスチャ幅
		int					m_iTextureHeight;	//!< テクスチャ高さ

		bool				m_bUpdate;			//!< 更新が必要かどうか

//		SsProject* m_myProject;					//!< 自身が所属しているプロジェクト

		SsCharMap* m_pCharMap;
		SsRawImage* m_pBitmap;			//!< イメージ


		ISSTexture* m_pTexture;			//!< ＧＬテクスチャ

		SsCellMap				m_CellMap;			//!< セルマップ
		SsCell					m_Cell;				//!< セル
		SsPartValueText*		m_valueText;

#if 0
		SsOpenGLImage* m_pImage;			//!< ＧＬイメージ
		SsOpenGLTexture* m_pTexture;			//!< ＧＬテクスチャ

#endif
		SsRawImage* m_pImage;			//!< ＧＬイメージ
		//SsCellValue				m_CellValue;

		//! ファミリから更新
		void updateFromFamily();


		//! キャラマップから更新
		void updateFromCharMap();

		//cellmap計算
		void calcCell();

	public:


	};

};

#endif // SSFONTTEXTURE_H
