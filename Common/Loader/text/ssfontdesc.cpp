/*!
 * \file	ssfontdesc.cpp
 * \author	CRI Middleware Co., Ltd.
*/
#include "ssfontdesc.h"
namespace SpriteStudio
{
	const bool						SsFontDesc::DEFAULT_BITMAP = false;				//!< デフォルトビットマップ
	const SsString					SsFontDesc::DEFAULT_FAMILY = "Arial";				//!< デフォルトファミリ
	const SsString					SsFontDesc::DEFAULT_CHAR_MAP = "";					//!< デフォルトキャラマップ
	const int						SsFontDesc::DEFAULT_SIZE = 16;					//!< デフォルトサイズ
	const float						SsFontDesc::DEFAULT_SPACE = 0.0f;					//!< デフォルトスペース
	const SsAnchorButton::Anchor	SsFontDesc::DEFAULT_ANCHOR = SsAnchorButton::LT;	//!< デフォルトアンカー

	//! コンストラクタ
	SsFontDesc::SsFontDesc()
	{
		setBitmap(DEFAULT_BITMAP);
		setFamily(DEFAULT_FAMILY);
		setCharMap(DEFAULT_CHAR_MAP);
		setSize(DEFAULT_SIZE);
		setSpace(DEFAULT_SPACE);
		setAnchor(DEFAULT_ANCHOR);

		m_bUpdate = true;
	}

	//! デストラクタ
	SsFontDesc::~SsFontDesc()
	{
	}

	SsFontDesc& SsFontDesc::operator =(const SsFontDesc& right)
	{
		setBitmap(right.isBitmap());
		setFamily(right.getFamily());
		setCharMap(right.getCharMap());
		setSize(right.getSize());
		setSpace(right.getSpace());
		setAnchor(right.getAnchor());

		return	*this;
	}

	bool SsFontDesc::operator ==(const SsFontDesc& right) const
	{
		if (
			(isBitmap() == right.isBitmap()) &&
			(getFamily() == right.getFamily()) &&
			(getCharMap() == right.getCharMap()) &&
			(getSize() == right.getSize()) &&
			(getSpace() == right.getSpace()) &&
			(getAnchor() == right.getAnchor())
			) {
			return	true;
		}

		return	false;
	}

	bool SsFontDesc::operator !=(const SsFontDesc& right) const
	{
		return	!(*this == right);
	}

	bool SsFontDesc::operator <(const SsFontDesc& right) const
	{
		if (isBitmap() < right.isBitmap()) return true;
		if (isBitmap() > right.isBitmap()) return false;

		if (getFamily() < right.getFamily()) return true;
		if (getFamily() > right.getFamily()) return false;

		if (getCharMap() < right.getCharMap()) return true;
		if (getCharMap() > right.getCharMap()) return false;

		if (getSize() < right.getSize()) return true;
		if (getSize() > right.getSize()) return false;

		if (getSpace() < right.getSpace()) return true;
		if (getSpace() > right.getSpace()) return false;

		if (getAnchor() < right.getAnchor()) return true;
		if (getAnchor() > right.getAnchor()) return false;

		return	false;
	}

	bool SsFontDesc::operator >(const SsFontDesc& right) const
	{
		if (isBitmap() > right.isBitmap()) return true;
		if (isBitmap() < right.isBitmap()) return false;

		if (getFamily() > right.getFamily()) return true;
		if (getFamily() < right.getFamily()) return false;

		if (getCharMap() > right.getCharMap()) return true;
		if (getCharMap() < right.getCharMap()) return false;

		if (getSize() > right.getSize()) return true;
		if (getSize() < right.getSize()) return false;

		if (getSpace() > right.getSpace()) return true;
		if (getSpace() < right.getSpace()) return false;

		if (getAnchor() > right.getAnchor()) return true;
		if (getAnchor() < right.getAnchor()) return false;

		return	false;
	}

	//! ビットマップを設定
	void SsFontDesc::setBitmap(bool bBitmap)
	{
		if (m_bBitmap != bBitmap) {
			m_bBitmap = bBitmap;
			m_bUpdate = true;
		}
	}

	//! ファミリを設定
	void SsFontDesc::setFamily(const SsString& strFamily)
	{
		if (m_strFamily != strFamily) {
			m_strFamily = strFamily;
			m_bUpdate = true;
		}
	}

	//! キャラマップを設定
	void SsFontDesc::setCharMap(const SsString& strCharMap)
	{
		if (m_strCharMap != strCharMap) {
			m_strCharMap = strCharMap;
			m_bUpdate = true;
		}
	}

	//! サイズを設定
	void SsFontDesc::setSize(int iSize)
	{
		if (m_iSize != iSize) {
			m_iSize = iSize;
			m_bUpdate = true;
		}
	}

	//! スペースを設定
	void SsFontDesc::setSpace(float fSpace)
	{
		if (m_fSpace != fSpace) {
			m_fSpace = fSpace;
			m_bUpdate = true;
		}
	}

	//! アンカーを設定
	void SsFontDesc::setAnchor(SsAnchorButton::Anchor eAnchor)
	{
		if (m_eAnchor != eAnchor) {
			m_eAnchor = eAnchor;
			m_bUpdate = true;
		}
	}

	//! ビットマップを取得
	bool SsFontDesc::isBitmap() const
	{
		return	m_bBitmap;
	}

	//! ファミリを取得
	SsString SsFontDesc::getFamily() const
	{
		return	m_strFamily;
	}

	//! キャラマップを取得
	SsString SsFontDesc::getCharMap() const
	{
		return	m_strCharMap;
	}

	//! サイズを取得
	int SsFontDesc::getSize() const
	{
		return	m_iSize;
	}

	//! スペースを取得
	float SsFontDesc::getSpace() const
	{
		return	m_fSpace;
	}

	//! アンカーを取得
	SsAnchorButton::Anchor SsFontDesc::getAnchor() const
	{
		return	m_eAnchor;
	}

	//! 更新が必要かどうかを取得
	bool SsFontDesc::takeUpdate()
	{
		bool	bUpdate = m_bUpdate;

		m_bUpdate = false;

		return	bUpdate;
	}
};
