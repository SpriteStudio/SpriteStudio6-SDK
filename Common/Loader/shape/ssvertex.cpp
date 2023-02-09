#include "ssvertex.h"

namespace SpriteStudio
{
	//! コンストラクタ
	SsVertex::SsVertex()
	{
		m_Position = SsPoint2();
		m_Coord = SsPoint2();
	}

	//! コンストラクタ
	SsVertex::SsVertex(const SsPoint2& position)
	{
		m_Position = position;
		m_Coord = SsPoint2();
	}

	//! コンストラクタ
	SsVertex::SsVertex(const SsPoint2& position, const SsPoint2& coord)
	{
		m_Position = position;
		m_Coord = coord;
	}

	//! デストラクタ
	SsVertex::~SsVertex()
	{
	}

	//! 位置を取得
	const SsPoint2& SsVertex::getPosition() const
	{
		return	m_Position;
	}

	//! 座標を取得
	const SsPoint2& SsVertex::getCoord() const
	{
		return	m_Coord;
	}


};