/*!
 * \file	sspartvaluenines.h
 * \author	CRI Middleware Co., Ltd.
*/
#ifndef SSPARTVALUENINES_H
#define SSPARTVALUENINES_H

#include "sspartvalue.h"
#include "../nineslice/ssnines.h"



namespace SpriteStudio
{

	/*!
	 * \class	SsPartValueNines
	 * \brief	９スライスパーツパラメータ
	*/
	class SsPartValueNines : public SsPartValue
	{
	public:
		static const SsString	TAG;	//!< デフォルトタグ

		//! コンストラクタ
		SsPartValueNines();

		//! デストラクタ
		~SsPartValueNines() override;

		//! 複製
		/*!
		 * \return	パーツパラメータ
		*/
		SsPartValue* duplicate() override;

		//! 座標を設定
		/*!
		 * \param	coord		座標
		*/
		void setCoord(const SsRectF& coord);

		//! 範囲を設定
		/*!
		 * \param	coord		範囲
		*/
		void setRegion(const SsRectF& region);

		//! マージンを設定
		/*!
		 * \param	margin		マージン
		*/
		void setMargin(const SsMargins& margin);

		//! フィルモードを設定
		/*!
		 * \param	eFillMode	フィルモード
		*/
		void setFillMode(int eFillMode);

		//! マスクを設定
		/*!
		 * \param	bMask		マスク
		*/
		void setMask(bool bMask);

		//! セル？を設定
		/*!
		 * \param	cCell		セル？
		*/
		void setCell(char cCell);

		//! サイズを設定
		/*!
		 * \param	size		サイズ
		*/
		void setSize(const SsSizeF& size);

		//! マスクを取得
		/*!
		 * \return	マスク
		*/
		bool isMask() const;

		//! セル？を取得
		/*!
		 * \return	セル？
		*/
		char getCell() const;

		bool update();

		//! ９スライスを取得
		/*!
		 * \return	シェイプ
		*/
		const SsNines* getNines();

	private:
		bool				m_bMask;			//!< マスク
		char				m_cCell;			//!< セル？

		bool				m_bUpdate;			//!< 更新が必要かどうか

		SsNines* m_pNines;			//!< ９スライス
	};

};

#endif // SSPARTVALUENINES_H
