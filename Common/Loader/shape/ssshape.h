#ifndef SSSHAPE_H
#define SSSHAPE_H

#include "../sstypes.h"
//#include "../partvalues/ssplayer_partValue.h"
#include "sspolygon.h"
namespace SpriteStudio
{

	/*!
	* \class	SsShape
	* \brief	シェイプ
	*/
	class SsShape 
	{
	public :
		//! コンストラクタ
		SsShape();

		//! デストラクタ
		virtual ~SsShape();

		//! サイズを設定
		/*!
		* \param	サイズ
		*/
		void setSize( const SsSizeF& size );

		//! サイズを取得
		/*!
		* \return	サイズ
		*/
		SsSizeF getSize() const;

		bool update();

		//! ポリゴンを取得
		/*!
		* \return	ポリゴン
		*/
		const SsPolygon& getPolygon() const;

	protected :
		SsSizeF				m_Size;				//!< サイズ
		bool				m_bUpdate;			//!< 更新が必要かどうか

		SsPolygon			m_Polygon;			//!< ポリゴン

		//! ポリゴンを生成
		/*!
		* \return	成功したかどうか
		*/
		virtual bool createPolygon();
	};
};

#endif // SSSHAPE_H
