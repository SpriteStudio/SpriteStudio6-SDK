/*!
 * \file	sspartvaluenines.cpp
 * \author	CRI Middleware Co., Ltd.
*/
#include "sstypes.h"
#include "sspartvaluenines.h"

//#include "SsGUI.h"


namespace SpriteStudio
{


	const SsString	SsPartValueNines::TAG = "nines";	//!< デフォルトタグ

	//! コンストラクタ
	SsPartValueNines::SsPartValueNines() : SsPartValue(TAG)
		, m_pNines(new SsNines())
	{
		setMask(false);
		setCell('-');

		m_bUpdate = true;
	}

	//! デストラクタ
	SsPartValueNines::~SsPartValueNines()
	{
		if (m_pNines) {
			delete m_pNines;
			m_pNines = nullptr;
		}
	}

	//! 複製
	SsPartValue* SsPartValueNines::duplicate()
	{
		SsPartValueNines* pDup = new SsPartValueNines();

		*pDup->m_pNines = *m_pNines;

		pDup->setMask(m_bMask);
		pDup->setCell(m_cCell);

		return	pDup;
	}

	//! 座標を設定
	void SsPartValueNines::setCoord(const SsRectF& coord)
	{
		if (m_pNines) {
			m_pNines->setCoord(coord);

			m_bUpdate = true;
		}
	}

	//! 範囲を設定
	void SsPartValueNines::setRegion(const SsRectF& region)
	{
		if (m_pNines) {
			m_pNines->setRegion(region);

			m_bUpdate = true;
		}
	}

	//! マージンを設定
	void SsPartValueNines::setMargin(const SsMargins& margin)
	{
		if (m_pNines) {
			m_pNines->setMargin(margin);

			m_bUpdate = true;
		}
	}

	//! フィルモードを設定
	void SsPartValueNines::setFillMode(int eFillMode)
	{
		if (m_pNines) {
			m_pNines->setFillMode(eFillMode);

			m_bUpdate = true;
		}
	}

	//! マスクを設定
	void SsPartValueNines::setMask(bool bMask)
	{
		if (m_bMask != bMask) {
			m_bMask = bMask;
		}
	}

	//! セル？を設定
	void SsPartValueNines::setCell(char cCell)
	{
		if (m_cCell != cCell) {
			m_cCell = cCell;
		}
	}

	//! サイズを設定
	void SsPartValueNines::setSize(const SsSizeF& size)
	{
		if (m_pNines) {
			m_pNines->setSize(size);

			m_bUpdate = true;
		}
	}

	//! マスクを取得
	bool SsPartValueNines::isMask() const
	{
		return	m_bMask;
	}

	//! セル？を取得
	char SsPartValueNines::getCell() const
	{
		return	m_cCell;
	}

	bool SsPartValueNines::update()
	{
		bool	bUpdate = (m_bUpdate);

		if (!bUpdate) {
			return	false;
		}

		if (m_pNines) {
			m_pNines->update();
		}

		m_bUpdate = false;

		return	true;
	}

	//! ９スライスを取得
	const SsNines* SsPartValueNines::getNines()
	{
		if (!update()) {
			return	m_pNines;
		}

		return	m_pNines;
	}

};