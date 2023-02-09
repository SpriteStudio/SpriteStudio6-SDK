/*!
 * \file	ssfonttexturemng.cpp
 * \author	CRI Middleware Co., Ltd.
*/
#include "ssfonttexturemng.h"

//! コンストラクタ
SsFontTextureMng::SsFontTextureMng()
{
	m_mapValue.clear();
}

//! デストラクタ
SsFontTextureMng::~SsFontTextureMng()
{
	m_mapValue.clear();
}

//! シングルトンインスタンスを取得
SsFontTextureMng& SsFontTextureMng::getInstance()
{
	static SsFontTextureMng		s_Self;

	return	s_Self;
}

//! 取得
SsFontTexture* SsFontTextureMng::get( const SsString& strText, const SsFontDesc& fontDesc, const SsSize& sizeGrid, const SsSize& sizeFixed )
{
	Key		key( strText, fontDesc, sizeGrid, sizeFixed );

	if ( m_mapValue.contains( key ) ) {
		return	m_mapValue[key].inc();
	}

	Value	value( strText, fontDesc, sizeGrid, sizeFixed );

	m_mapValue[key] = value;

	return	m_mapValue[key].inc();
}

//! 解放
void SsFontTextureMng::release( SsFontTexture* pFontTexture )
{
	if ( !pFontTexture ) {
		return;
	}

	Key		key( pFontTexture->getText(), pFontTexture->getFontDesc(), pFontTexture->getGridSize(), pFontTexture->getFixedSize() );

	if ( m_mapValue.contains( key ) ) {
		if ( m_mapValue[key].dec() ) {
			m_mapValue.remove( key );
		}
	}
}

//! 取得カウンタを取得
int SsFontTextureMng::count( SsFontTexture* pFontTexture )
{
	if ( !pFontTexture ) {
		return	0;
	}

	Key		key( pFontTexture->getText(), pFontTexture->getFontDesc(), pFontTexture->getGridSize(), pFontTexture->getFixedSize() );

	if ( m_mapValue.contains( key ) ) {
		return	m_mapValue[key].count();
	}

	return	0;
}

//! コンストラクタ
SsFontTextureMng::Key::Key( const SsString& strText, const SsFontDesc& fontDesc, const SsSize& sizeGrid, const SsSize& sizeFixed )
{
	m_strText = strText;
	m_FontDesc = fontDesc;
	m_GridSize = sizeGrid;
	m_FixedSize = sizeFixed;
}

//! デストラクタ
SsFontTextureMng::Key::~Key()
{
}

SsFontTextureMng::Key& SsFontTextureMng::Key::operator =( const Key& right )
{
	m_strText = right.m_strText;
	m_FontDesc = right.m_FontDesc;
	m_GridSize = right.m_GridSize;
	m_FixedSize = right.m_FixedSize;

	return	*this;
}

bool SsFontTextureMng::Key::operator ==( const Key& right ) const
{
	if (
		( m_strText == right.m_strText ) &&
		( m_FontDesc == right.m_FontDesc ) &&
		( m_GridSize == right.m_GridSize ) &&
		( m_FixedSize == right.m_FixedSize )
	) {
		return	true;
	}

	return	false;
}

bool SsFontTextureMng::Key::operator !=( const Key& right ) const
{
	return	!( *this == right );
}

bool SsFontTextureMng::Key::operator <( const Key& right ) const
{
	if ( m_strText < right.m_strText ) return true;
	if ( m_strText > right.m_strText ) return false;

	if ( m_FontDesc < right.m_FontDesc ) return true;
	if ( m_FontDesc > right.m_FontDesc ) return false;

	if ( m_GridSize.width() < right.m_GridSize.width() ) return true;
	if ( m_GridSize.width() > right.m_GridSize.width() ) return false;

	if ( m_GridSize.height() < right.m_GridSize.height() ) return true;
	if ( m_GridSize.height() > right.m_GridSize.height() ) return false;

	if ( m_FixedSize.width() < right.m_FixedSize.width() ) return true;
	if ( m_FixedSize.width() > right.m_FixedSize.width() ) return false;

	if ( m_FixedSize.height() < right.m_FixedSize.height() ) return true;
	if ( m_FixedSize.height() > right.m_FixedSize.height() ) return false;

	return	false;
}

bool SsFontTextureMng::Key::operator >( const Key& right ) const
{
	if ( m_strText > right.m_strText ) return true;
	if ( m_strText < right.m_strText ) return false;

	if ( m_FontDesc > right.m_FontDesc ) return true;
	if ( m_FontDesc < right.m_FontDesc ) return false;

	if ( m_GridSize.width() > right.m_GridSize.width() ) return true;
	if ( m_GridSize.width() < right.m_GridSize.width() ) return false;

	if ( m_GridSize.height() > right.m_GridSize.height() ) return true;
	if ( m_GridSize.height() < right.m_GridSize.height() ) return false;

	if ( m_FixedSize.width() > right.m_FixedSize.width() ) return true;
	if ( m_FixedSize.width() < right.m_FixedSize.width() ) return false;

	if ( m_FixedSize.height() > right.m_FixedSize.height() ) return true;
	if ( m_FixedSize.height() < right.m_FixedSize.height() ) return false;

	return	false;
}

//! コンストラクタ
SsFontTextureMng::Value::Value()
{
	m_iCnt = 0;
}

//! コンストラクタ
SsFontTextureMng::Value::Value( const SsString& strText, const SsFontDesc& fontDesc, const SsSize& sizeGrid, const SsSize& sizeFixed )
{
	m_iCnt = 0;
	m_FontTexture.setText( strText );
	m_FontTexture.setFontDesc( fontDesc );
	m_FontTexture.setGridSize( sizeGrid );
	m_FontTexture.setFixedSize( sizeFixed );
}

//! デストラクタ
SsFontTextureMng::Value::~Value()
{
}

SsFontTextureMng::Value& SsFontTextureMng::Value::operator =( const Value& right )
{
	m_FontTexture.setText( right.m_FontTexture.getText() );
	m_FontTexture.setFontDesc( right.m_FontTexture.getFontDesc() );
	m_FontTexture.setGridSize( right.m_FontTexture.getGridSize() );
	m_FontTexture.setFixedSize( right.m_FontTexture.getFixedSize() );

	return	*this;
}

bool SsFontTextureMng::Value::operator ==( const Value& right ) const
{
	if (
		( m_FontTexture.getText() == right.m_FontTexture.getText() ) &&
		( m_FontTexture.getFontDesc() == right.m_FontTexture.getFontDesc() ) &&
		( m_FontTexture.getGridSize() == right.m_FontTexture.getGridSize() ) &&
		( m_FontTexture.getFixedSize() == right.m_FontTexture.getFixedSize() )
	) {
		return	true;
	}

	return	false;
}

bool SsFontTextureMng::Value::operator !=( const Value& right ) const
{
	return	!( *this == right );
}

//! 取得カウンタを増やす
SsFontTexture* SsFontTextureMng::Value::inc()
{
	m_iCnt++;
	return	&m_FontTexture;
}

//! 取得カウンタを減らす
bool SsFontTextureMng::Value::dec()
{
	m_iCnt--;
	return	( m_iCnt <= 0 );
}

//! 取得カウンタを取得
int SsFontTextureMng::Value::count()
{
	return	m_iCnt;
}
