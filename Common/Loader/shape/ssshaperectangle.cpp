#include "ssshaperectangle.h"


namespace SpriteStudio
{

	//! コンストラクタ
	SsShapeRectangle::SsShapeRectangle() : SsShape()
	{
	}

	//! デストラクタ
	SsShapeRectangle::~SsShapeRectangle()
	{
	}

	//! ポリゴンを生成
	bool SsShapeRectangle::createPolygon()
	{
		float	fX2 = m_Size.width() * 0.5f;
		float	fY2 = m_Size.height() * 0.5f;

		m_Polygon.clear();

		m_Polygon.setVertexOrder( SsPolygon::VO_TriangleFan );

		m_Polygon.addVertex( SsVertex( SsPoint2( 0.0f, 0.0f ) ) );
		m_Polygon.addVertex( SsVertex( SsPoint2( -fX2,  fY2 ) ) );
		m_Polygon.addVertex( SsVertex( SsPoint2(  fX2,  fY2 ) ) );
		m_Polygon.addVertex( SsVertex( SsPoint2(  fX2, -fY2 ) ) );
		m_Polygon.addVertex( SsVertex( SsPoint2( -fX2, -fY2 ) ) );
		m_Polygon.addVertex( SsVertex( SsPoint2( -fX2,  fY2 ) ) );

		return	true;
	}

};