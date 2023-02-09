#ifndef SSSHAPERECTANGLE_H
#define SSSHAPERECTANGLE_H

#include "ssshape.h"

namespace SpriteStudio
{

	/*!
	* \class	SsShapeRectangle
	* \brief	矩形シェイプ
	*/
	class SsShapeRectangle : public SsShape
	{
	public :
		//! コンストラクタ
		SsShapeRectangle();

		//! デストラクタ
		~SsShapeRectangle() override;

	protected :
		//! ポリゴンを生成
		/*!
		* \return	成功したかどうか
		*/
		bool createPolygon() override;
	};

};

#endif // SSSHAPERECTANGLE_H
