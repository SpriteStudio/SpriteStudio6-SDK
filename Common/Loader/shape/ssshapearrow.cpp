#include "ssshapearrow.h"

namespace SpriteStudio
{

	//! コンストラクタ
	SsShapeArrow::SsShapeArrow() : SsShape()
	{
	}

	//! デストラクタ
	SsShapeArrow::~SsShapeArrow()
	{
	}

	//! ポリゴンを生成
	bool SsShapeArrow::createPolygon()
	{
		float	fX2 = m_Size.width() * 0.5f;
		float	fY2 = m_Size.height() * 0.5f;
		float	fX4 = m_Size.width() * 0.25f;
		float	fY4 = m_Size.height() * 0.25f;

		m_Polygon.clear();

		m_Polygon.setVertexOrder( SsPolygon::VO_TriangleFan );

		m_Polygon.addVertex( SsVertex( SsPoint2( 0.0f, 0.0f ) ) );
		m_Polygon.addVertex( SsVertex( SsPoint2( 0.0f,  fY2 ) ) );
		m_Polygon.addVertex( SsVertex( SsPoint2(  fX2, 0.0f ) ) );
		m_Polygon.addVertex( SsVertex( SsPoint2(  fX4, 0.0f ) ) );
		m_Polygon.addVertex( SsVertex( SsPoint2(  fX4, -fY2 ) ) );
		m_Polygon.addVertex( SsVertex( SsPoint2( -fX4, -fY2 ) ) );
		m_Polygon.addVertex( SsVertex( SsPoint2( -fX4, 0.0f ) ) );
		m_Polygon.addVertex( SsVertex( SsPoint2( -fX2, 0.0f ) ) );
		m_Polygon.addVertex( SsVertex( SsPoint2( 0.0f,  fY2 ) ) );

		return	true;
	}

};
