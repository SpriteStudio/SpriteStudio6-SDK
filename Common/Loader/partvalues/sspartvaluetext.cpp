/*!
 * \file	sspartvaluetext.cpp
 * \author	CRI Middleware Co., Ltd.
*/
#include "sspartvaluetext.h"

#include "text/ssfontdesc.h"
//#include "ssfonttexturemng.h"
//#include "sscompmap.h"

namespace SpriteStudio
{
	const SsString	SsPartValueText::TAG	= "text";	//!< デフォルトタグ

	//! コンストラクタ
	SsPartValueText::SsPartValueText() : SsPartValue( TAG )
	{
		setText( "" );

		//m_pFontTexture = nullptr;

		setSmooth( true );
		setMask( false );
		setWidth( 0 );
		setHeight( 0 );
	#ifdef SSPARTVALUETEXT_SET_CMP
		setCmp( "" );
		setCmpSize( SsVector2( 0, 0 ) );
	#endif // SSPARTVALUETEXT_SET_CMP

		m_bUpdate = true;
	}

	//! デストラクタ
	SsPartValueText::~SsPartValueText()
	{
#if IS_EDITOR
		m_Image.texture = nullptr;

		SsFontTextureMng::getInstance().release( m_pFontTexture );
		m_pFontTexture = nullptr;
#endif
	}

	//! 複製
	SsPartValue* SsPartValueText::duplicate()
	{
		SsPartValueText*	pDup = new SsPartValueText();

		pDup->setText( m_strText );

		pDup->m_FontDesc = m_FontDesc;


		pDup->setSmooth( m_bSmooth );
		pDup->setMask( m_bMask );
		pDup->setWidth( m_iWidth );
		pDup->setHeight( m_iHeight );
	#ifdef SSPARTVALUETEXT_SET_CMP
		pDup->setCmp( "" );
		pDup->setCmpSize( SsVector2( 0, 0 ) );
	#endif // SSPARTVALUETEXT_SET_CMP

#if IS_EDITOR
		pDup->m_pFontTexture = nullptr;
		m_Image.texture = nullptr;

		SsFontTextureMng::getInstance().release( m_pFontTexture );
		m_pFontTexture = nullptr;
#endif

		return	pDup;
	}

	//! テキストを設定
	void SsPartValueText::setText( const SsString& strText )
	{
		if ( m_strText != strText ) {
			m_strText = strText;
			m_bUpdate = true;
		}
	}

	//! ビットマップを設定
	void SsPartValueText::setBitmap( bool bBitmap )
	{
		m_FontDesc.setBitmap( bBitmap );
	}

	//! ファミリを設定
	void SsPartValueText::setFamily( const SsString& strFamily )
	{
		m_FontDesc.setFamily( strFamily );
	}

	//! キャラマップを設定
	void SsPartValueText::setCharMap( const SsString& strCharMap )
	{
		m_FontDesc.setCharMap( strCharMap );
	}

	//! サイズを設定
	void SsPartValueText::setSize( int iSize )
	{
		m_FontDesc.setSize( iSize );
	}

	//! スペースを設定
	void SsPartValueText::setSpace( float fSpace )
	{
		m_FontDesc.setSpace( fSpace );
	}

	//! アンカーを設定
	void SsPartValueText::setAnchor( SsAnchorButton::Anchor eAnchor )
	{
		m_FontDesc.setAnchor( eAnchor );
	}

	//! スムースを設定
	void SsPartValueText::setSmooth( bool bSmooth )
	{
		if ( m_bSmooth != bSmooth ) {
			m_bSmooth = bSmooth;
		}
	}

	//! マスクを設定
	void SsPartValueText::setMask( bool bMask )
	{
		if ( m_bMask != bMask ) {
			m_bMask = bMask;
		}
	}

	//! 幅を設定
	void SsPartValueText::setWidth( int iWidth )
	{
		if ( iWidth < 0 ) iWidth = 0;
		if ( iWidth > 8192 ) iWidth = 8192;

		if ( m_iWidth != iWidth ) {
			m_iWidth = iWidth;
		}
	}

	//! 高さを設定
	void SsPartValueText::setHeight( int iHeight )
	{
		if ( iHeight < 0 ) iHeight = 0;
		if ( iHeight > 8192 ) iHeight = 8192;

		if ( m_iHeight != iHeight ) {
			m_iHeight = iHeight;
		}
	}

	#ifdef SSPARTVALUETEXT_SET_CMP
	//! 圧縮ビットマップ文字列を設定
	void SsPartValueText::setCmp( const SsString& strCmp )
	{
		m_strCmp = strCmp;
	}

	//! 圧縮ビットマップサイズを設定
	void SsPartValueText::setCmpSize( const SsVector2& cmpSize )
	{
		m_CmpSize = cmpSize;
	}
	#endif // SSPARTVALUETEXT_SET_CMP

	//! テキストを取得
	SsString SsPartValueText::getText() const
	{
		return	m_strText;
	}

	//! ビットマップを取得
	bool SsPartValueText::isBitmap() const
	{
		return	m_FontDesc.isBitmap();
	}

	//! ファミリを取得
	SsString SsPartValueText::getFamily() const
	{
		return	m_FontDesc.getFamily();
	}

	//! キャラマップを取得
	SsString SsPartValueText::getCharMap() const
	{
		return	m_FontDesc.getCharMap();
	}

	//! サイズを取得
	int SsPartValueText::getSize() const
	{
		return	m_FontDesc.getSize();
	}

	//! スペースを取得
	float SsPartValueText::getSpace() const
	{
		return	m_FontDesc.getSpace();
	}

	//! アンカーを取得
	SsAnchorButton::Anchor SsPartValueText::getAnchor() const
	{
		return	m_FontDesc.getAnchor();
	}

	//! スムースを取得
	bool SsPartValueText::isSmooth() const
	{
		return	m_bSmooth;
	}

	//! マスクを取得
	bool SsPartValueText::isMask() const
	{
		return	m_bMask;
	}

	//! 幅を取得
	int SsPartValueText::getWidth() const
	{
		return	m_iWidth;
	}

	//! 高さを取得
	int SsPartValueText::getHeight() const
	{
		return	m_iHeight;
	}

	#ifdef SSPARTVALUETEXT_GET_CMP
	//! 圧縮ビットマップ文字列を取得
	SsString SsPartValueText::getCmp()
	{
		SsCompMap	cmp;

		if ( !m_pFontTexture ) {
			return	"";
		}

	#ifdef SSFONTTEXTURE_GET_BITMAP
		cmp.fromImage( m_pFontTexture->getBitmap(), SsCompMap::FM_L4, m_pFontTexture->getImageWidth(), m_pFontTexture->getImageHeight() );
	#endif // SSFONTTEXTURE_GET_BITMAP

		return	cmp.encode().toStdString();
	}

	//! 圧縮ビットマップサイズを取得
	SsVector2 SsPartValueText::getCmpSize()
	{
		if ( !m_pFontTexture ) {
			return	SsVector2( 0, 0 );
		}

		return	SsVector2( m_pFontTexture->getImageWidth(), m_pFontTexture->getImageHeight() );
	}
	#endif // SSPARTVALUETEXT_GET_CMP

	bool SsPartValueText::needUpdate()
	{
//		bool	bUpdate = (m_FontDesc.takeUpdate() || m_bUpdate || !m_pFontTexture);
		bool	bUpdate = (m_FontDesc.takeUpdate() || m_bUpdate );

		return bUpdate;

	}


	bool SsPartValueText::update()
	{

#if IS_EDITOR
		bool	bUpdate = ( m_FontDesc.takeUpdate() || m_bUpdate || !m_pFontTexture );

		if ( !bUpdate ) {
			return	false;
		}

		SsFontTexture*	pFontTexture = m_pFontTexture;

		m_Image.texture = nullptr;

		m_pFontTexture = SsFontTextureMng::getInstance().get( m_strText, m_FontDesc, SsSize( 0, 0 ), SsSize( getWidth(), getHeight() ) );
		SsFontTextureMng::getInstance().release( pFontTexture );

		m_pFontTexture->setText( m_strText );
		m_pFontTexture->setFontDesc( m_FontDesc );
		m_pFontTexture->setGridSize( SsSize( 0, 0 ) );
		m_pFontTexture->setFixedSize( SsSize( getWidth(), getHeight() ) );

	#ifdef SSPARTVALUETEXT_SET_CMP
		if ( !m_strCmp.empty() ) {
			if ( SsFontTextureMng::getInstance().count( m_pFontTexture ) <= 1 ) {
				SsCompMap	cmp;

				cmp.decode( m_strCmp.c_str(), SsCompMap::FM_L4, m_CmpSize.x, m_CmpSize.y );

	#ifdef SSFONTTEXTURE_SET_BITMAP
				m_pFontTexture->setBitmap( cmp.toImage() );
	#endif // SSFONTTEXTURE_SET_BITMAP
			}

			m_strCmp.clear();
		}
	#endif // SSPARTVALUETEXT_SET_CMP

		m_pFontTexture->update();

		m_bUpdate = false;
#endif

		return	true;
	}

	//! セルを取得
	SsCell* SsPartValueText::getCell()
	{
#if 0
		if ( !update() ) {
			return	&m_Cell;
		}
#if IS_EDITOR
		m_Image.texture = m_pFontTexture->getTexture();
#endif

		m_CellMap.pixelSize = SsPoint2( m_pFontTexture->getTextureWidth(), m_pFontTexture->getTextureHeight() );
		//m_CellMap.parent = nullptr;

		m_CellMap.overrideTexSettings = true;
		m_CellMap.filterMode = m_bSmooth ? SsTexFilterMode::linear : SsTexFilterMode::nearest;
		m_CellMap.wrapMode = SsTexWrapMode::clamp;

		//m_CellMap.image = &m_Image;

		//m_Cell.map = &m_CellMap;
		m_Cell.pos = SsVector2( 0, 0 );
		m_Cell.size = SsVector2( m_pFontTexture->getImageWidth(), m_pFontTexture->getImageHeight() );
		m_Cell.pivot = SsVector2( 0, 0 );
		
		//m_Cell.calcUvs();
		return	&m_Cell;
#endif
		return 0;
	}

};