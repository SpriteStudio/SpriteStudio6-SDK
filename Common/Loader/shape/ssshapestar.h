#ifndef SSSHAPESTAR_H
#define SSSHAPESTAR_H

#include "ssshape.h"

namespace SpriteStudio
{

	/*!
	* \class	SsShapeStar
	* \brief	星形シェイプ
	*/
	class SsShapeStar : public SsShape
	{
	public :
		//! コンストラクタ
		SsShapeStar();

		//! デストラクタ
		~SsShapeStar() override;

	protected :
		//! ポリゴンを生成
		/*!
		* \return	成功したかどうか
		*/
		bool createPolygon() override;
	};

};
#endif // SSSHAPESTAR_H
