#ifndef SSSHAPETRIANGLE_H
#define SSSHAPETRIANGLE_H

#include "ssshape.h"

namespace SpriteStudio
{
	/*!
	* \class	SsShapeTriangle
	* \brief	三角形シェイプ
	*/
	class SsShapeTriangle : public SsShape
	{
	public :
		//! コンストラクタ
		SsShapeTriangle();

		//! デストラクタ
		~SsShapeTriangle() override;

	protected :
		//! ポリゴンを生成
		/*!
		* \return	成功したかどうか
		*/
		bool createPolygon() override;
	};
};

#endif // SSSHAPETRIANGLE_H
