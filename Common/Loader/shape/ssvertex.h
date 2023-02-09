#ifndef SSVERTEX_H
#define SSVERTEX_H

#include "sstypes.h"
namespace SpriteStudio
{
	/*!
	 * \class	SsVertex
	 * \brief	頂点
	*/
	class SsVertex
	{
	public:
		//! コンストラクタ
		SsVertex();

		//! コンストラクタ
		/*!
		 * \param	position	位置
		*/
		SsVertex(const SsPoint2& position);

		//! コンストラクタ
		/*!
		 * \param	position	位置
		 * \param	coord		座標
		*/
		SsVertex(const SsPoint2& position, const SsPoint2& coord);

		//! デストラクタ
		~SsVertex();

		//! 位置を取得
		/*!
		 * \return	位置
		*/
		const SsPoint2& getPosition() const;

		//! 座標を取得
		/*!
		 * \return	座標
		*/
		const SsPoint2& getCoord() const;

	private:
		SsPoint2		m_Position;		//!< 位置
		SsPoint2		m_Coord;		//!< 座標
	};

};

#endif // SSVERTEX_H
