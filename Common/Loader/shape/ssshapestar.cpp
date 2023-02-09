#include "ssshapestar.h"

//#include "ssmath.h"
#include "ssplayer_math.h"

namespace SpriteStudio
{

	//! 水平位置に変換
	/*!
	* \param	fRad		ラジアン角
	* \return	水平位置
	*/
	static inline float toH( float fRad )
	{
		return	sinf( DegreeToRadian( fRad ) );
	}

	//! 垂直位置に変換
	/*!
	* \param	fRad		ラジアン角
	* \return	垂直位置
	*/
	static inline float toV( float fRad )
	{
		return	cosf( DegreeToRadian( fRad ) );
	}

	//! コンストラクタ
	SsShapeStar::SsShapeStar() : SsShape()
	{
	}

	//! デストラクタ
	SsShapeStar::~SsShapeStar()
	{
	}

	//! ポリゴンを生成
	bool SsShapeStar::createPolygon()
	{
		float	fX2 = m_Size.width() * 0.5f;
		float	fY2 = m_Size.height() * 0.5f;
		float	fX4 = m_Size.width() * 0.25f;
		float	fY4 = m_Size.height() * 0.25f;

		m_Polygon.clear();

		m_Polygon.setVertexOrder( SsPolygon::VO_TriangleFan );

		m_Polygon.addVertex( SsVertex( SsPoint2( 0.0f, 0.0f ) ) );

		m_Polygon.addVertex( SsVertex( SsPoint2( toH( 0.0f ) * fX2, toV( 0.0f ) * fY2 ) ) );

		for ( int i = 1; i < 10; i++ ) {
			m_Polygon.addVertex( SsVertex( SsPoint2( toH( 36 * i ) * ( ( i % 2 ) ? ( fX4 ) : ( fX2 ) ), toV( 36 * i ) * ( ( i % 2 ) ? ( fY4 ) : ( fY2 ) ) ) ) );
		}

		m_Polygon.addVertex( SsVertex( SsPoint2( toH( 0.0f ) * fX2, toV( 0.0f ) * fY2 ) ) );

		return	true;
	}


};