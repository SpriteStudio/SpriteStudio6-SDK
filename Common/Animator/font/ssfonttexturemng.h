/*!
 * \file	ssfonttexturemng.h
 * \author	CRI Middleware Co., Ltd.
*/
#ifndef SSFONTTEXTUREMNG_H
#define SSFONTTEXTUREMNG_H

#include <QMap>

#include "ssfonttexture.h"

/*!
 * \class	SsFontTextureMng
 * \brief	フォントテクスチャマネージャ
*/
class SsFontTextureMng
{
private :
	//! コンストラクタ
	SsFontTextureMng();

public :
	//! デストラクタ
	~SsFontTextureMng();

	//! シングルトンインスタンスを取得
	/*!
	 * \return	フォントテクスチャマネージャ
	*/
	static SsFontTextureMng& getInstance();

	//! 取得
	/*!
	 * \param	strText		テキスト
	 * \param	fontDesc	フォント設定
	 * \param	sizeGrid	グリッドサイズ
	 * \param	sizeFixed	固定サイズ
	 * \return	フォントテクスチャ
	*/
	SsFontTexture* get( const SsString& strText, const SsFontDesc& fontDesc, const SsSize& sizeGrid, const SsSize& sizeFixed );

	//! 解放
	/*!
	 * \param	pFontTexture	フォントテクスチャ
	*/
	void release( SsFontTexture* pFontTexture );

	//! 取得カウンタを取得
	/*!
	 * \param	pFontTexture	フォントテクスチャ
	 * \return	取得カウンタ
	*/
	int count( SsFontTexture* pFontTexture );

private :
	/*!
	 * \class	SsFontTextureMng::Key
	 * \brief	キー
	*/
	class Key
	{
	public :
		//! コンストラクタ
		/*!
		 * \param	strText		テキスト
		 * \param	fontDesc	フォント設定
		 * \param	sizeGrid	グリッドサイズ
		 * \param	sizeFixed	固定サイズ
		*/
		Key( const SsString& strText, const SsFontDesc& fontDesc, const SsSize& sizeGrid, const SsSize& sizeFixed );

		//! デストラクタ
		~Key();

		Key& operator =( const Key& right );
		bool operator ==( const Key& right ) const;
		bool operator !=( const Key& right ) const;
		bool operator <( const Key& right ) const;
		bool operator >( const Key& right ) const;

	private :
		SsString		m_strText;		//!< テキスト
		SsFontDesc	m_FontDesc;		//!< フォント設定
		SsSize		m_GridSize;		//!< グリッドサイズ
		SsSize		m_FixedSize;	//!< 固定サイズ
	};

	/*!
	 * \class	SsFontTextureMng::Value
	 * \brief	値
	*/
	class Value
	{
	public :
		//! コンストラクタ
		Value();

		//! コンストラクタ
		/*!
		 * \param	strText		テキスト
		 * \param	fontDesc	フォント設定
		 * \param	sizeGrid	グリッドサイズ
		 * \param	sizeFixed	固定サイズ
		*/
		Value( const SsString& strText, const SsFontDesc& fontDesc, const SsSize& sizeGrid, const SsSize& sizeFixed );

		//! デストラクタ
		~Value();

		Value& operator =( const Value& right );
		bool operator ==( const Value& right ) const;
		bool operator !=( const Value& right ) const;

		//! 取得カウンタを増やす
		/*!
		 * \return	フォントテクスチャ
		*/
		SsFontTexture* inc();

		//! 取得カウンタを減らす
		/*!
		 * \return	取得カウンタが0以下になった場合に true
		*/
		bool dec();

		//! 取得カウンタを取得
		/*!
		 * \return	取得カウンタ
		*/
		int count();

	private :
		int				m_iCnt;			//!< 取得カウンタ
		SsFontTexture	m_FontTexture;	//!< フォントテクスチャ
	};

	QMap<Key, Value>	m_mapValue;		//!< 値マップ
};

#endif // SSFONTTEXTUREMNG_H
