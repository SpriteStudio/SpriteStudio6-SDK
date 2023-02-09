#ifndef SSNINES_H
#define SSNINES_H

#include "../sstypes.h"
#include "../shape/sspolygon.h"

namespace SpriteStudio
{

	/*!
	 * \class	SsNines
	 * \brief	９スライス
	*/
	class SsNines
	{
	public:
		//! コンストラクタ
		SsNines();

		//! デストラクタ
		virtual ~SsNines();

		//! 座標を設定
		/*!
		 * \param	座標
		*/
		void setCoord(const SsRectF& coord);

		//! 範囲を設定
		/*!
		 * \param	範囲
		*/
		void setRegion(const SsRectF& region);

		//! マージンを設定
		/*!
		 * \margin	マージン
		*/
		void setMargin(const SsMargins& margin);

		//! フィルモードを設定
		/*!
		 * \eFillMode	フィルモード
		*/
		void setFillMode(int eFillMode);

		//! サイズを設定
		/*!
		 * \param	サイズ
		*/
		void setSize(const SsSizeF& size);

		//! 座標を取得
		/*!
		 * \return	座標
		*/
		SsRectF getCoord() const;

		//! 範囲を取得
		/*!
		 * \return	範囲
		*/
		SsRectF getRegion() const;

		//! マージンを取得
		/*!
		 * \return	マージン
		*/
		SsMargins getMargin() const;

		//! フィルモードを取得
		/*!
		 * \return	フィルモード
		*/
		int getFillMode() const;

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

	protected:
		SsRectF				m_Coord;			//!< 座標
		SsRectF				m_Region;			//!< 範囲
		SsMargins			m_Margin;			//!< マージン
		int					m_eFillMode;		//!< フィルモード
		SsSizeF				m_Size;				//!< サイズ
		bool				m_bUpdate;			//!< 更新が必要かどうか

		SsMargins			m_SafeMargin;		//!< マージン

		float				m_fX2;
		float				m_fY2;

		float				m_f0X;
		float				m_f1X;
		float				m_f2X;
		float				m_f3X;

		float				m_f0Y;
		float				m_f1Y;
		float				m_f2Y;
		float				m_f3Y;

		float				m_f0U;
		float				m_f1U;
		float				m_f2U;
		float				m_f3U;

		float				m_f0V;
		float				m_f1V;
		float				m_f2V;
		float				m_f3V;

		SsPolygon			m_Polygon;			//!< ポリゴン

		//! ポリゴンを生成
		/*!
		 * \return	成功したかどうか
		*/
		virtual bool createPolygon();

	private:
		void updateSafeMargin();

		bool updatePoints();

		bool createStretchPolygon();
		bool createTilePolygon();

		bool createTilePolygonCC();

		bool createTilePolygonLC();
		bool createTilePolygonRC();
		bool createTilePolygonCT();
		bool createTilePolygonCB();

		bool createTilePolygonLT();
		bool createTilePolygonRT();
		bool createTilePolygonLB();
		bool createTilePolygonRB();
	};

};

#endif // SSNINES_H
