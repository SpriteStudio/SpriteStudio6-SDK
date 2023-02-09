#include "ssshape.h"


namespace SpriteStudio
{

	//! コンストラクタ
	SsShape::SsShape()
	{
		setSize( SsSizeF( 1, 1 ) );

		m_bUpdate = true;

		m_Polygon.clear();
	}

	//! デストラクタ
	SsShape::~SsShape()
	{
	}

	//! サイズを設定
	void SsShape::setSize( const SsSizeF& size )
	{
		if ( m_Size != size ) {
			m_Size = size;
			m_bUpdate = true;
		}
	}

	//! サイズを取得
	/*!
	* \return	サイズ
	*/
	SsSizeF SsShape::getSize() const
	{
		return	m_Size;
	}

	bool SsShape::update()
	{
		bool	bUpdate = ( m_bUpdate );

		if ( !bUpdate ) {
			return	false;
		}

		createPolygon();

		m_bUpdate = false;

		return	true;
	}

	//! ポリゴンを取得
	/*!
	* \return	ポリゴン
	*/
	const SsPolygon& SsShape::getPolygon() const
	{
		return	m_Polygon;
	}

	//! ポリゴンを生成
	bool SsShape::createPolygon()
	{
		m_Polygon.clear();

		return	true;
	}

};