#ifndef SSSHAPEARROW_H
#define SSSHAPEARROW_H

#include "ssshape.h"

namespace SpriteStudio
{
	/*!
	* \class	SsShapeArrow
	* \brief	矢印形シェイプ
	*/
	class SsShapeArrow : public SsShape
	{
	public :
		//! コンストラクタ
		SsShapeArrow();

		//! デストラクタ
		~SsShapeArrow() override;

	protected :
		//! ポリゴンを生成
		/*!
		* \return	成功したかどうか
		*/
		bool createPolygon() override;
	};
};

#endif // SSSHAPEARROW_H
