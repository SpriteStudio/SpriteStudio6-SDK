#include "ssnines.h"

//#include "ssmath.h"
#include "ssplayer_math.h"


namespace SpriteStudio
{

	//! コンストラクタ
	SsNines::SsNines()
	{
		setCoord(SsRectF(0, 0, 1, 1));
		setRegion(SsRectF(0, 0, 1, 1));
		setMargin(SsMargins(0, 0, 0, 0));
		setFillMode(0);
		setSize(SsSizeF(1, 1));

		m_bUpdate = true;

		m_Polygon.clear();
	}

	//! デストラクタ
	SsNines::~SsNines()
	{
	}

	//! 座標を設定
	void SsNines::setCoord(const SsRectF& coord)
	{
		if (m_Coord != coord) {
			m_Coord = coord;
			m_bUpdate = true;
		}
	}

	//! 範囲を設定
	void SsNines::setRegion(const SsRectF& region)
	{
		if (m_Region != region) {
			m_Region = region;
			m_bUpdate = true;
		}
	}

	//! マージンを設定
	void SsNines::setMargin(const SsMargins& margin)
	{
		if (m_Margin != margin) {
			m_Margin = margin;
			m_bUpdate = true;
		}
	}

	//! フィルモードを設定
	void SsNines::setFillMode(int eFillMode)
	{
		if (m_eFillMode != eFillMode) {
			m_eFillMode = eFillMode;
			m_bUpdate = true;
		}
	}

	//! サイズを設定
	void SsNines::setSize(const SsSizeF& size)
	{
		if (m_Size != size) {
			m_Size = size;
			m_bUpdate = true;
		}
	}

	//! 座標を取得
	SsRectF SsNines::getCoord() const
	{
		return	m_Coord;
	}

	//! 範囲を取得
	SsRectF SsNines::getRegion() const
	{
		return	m_Region;
	}

	//! マージンを取得
	SsMargins SsNines::getMargin() const
	{
		return	m_Margin;
	}

	//! フィルモードを取得
	int SsNines::getFillMode() const
	{
		return	m_eFillMode;
	}

	//! サイズを取得
	SsSizeF SsNines::getSize() const
	{
		return	m_Size;
	}

	bool SsNines::update()
	{
		bool	bUpdate = (m_bUpdate);

		if (!bUpdate) {
			return	false;
		}

		updateSafeMargin();
		updatePoints();

		createPolygon();

		m_bUpdate = false;

		return	true;
	}

	//! ポリゴンを取得
	const SsPolygon& SsNines::getPolygon() const
	{
		return	m_Polygon;
	}

	//! ポリゴンを生成
	bool SsNines::createPolygon()
	{
		bool	bResult = true;

		m_Polygon.clear();

		m_Polygon.setVertexOrder(SsPolygon::VO_TriangleStrip);

		switch (m_eFillMode) {
		case 0:
			bResult = createStretchPolygon();
			break;
		case 1:
			bResult = createTilePolygon();
			break;
		default:
			break;
		}

		return	bResult;
	}

	void SsNines::updateSafeMargin()
	{
		int		iDiff;
		int		iHalf;

		iDiff = m_Margin.left() + m_Margin.right() - m_Coord.width();

		if (iDiff > 0) {
			iHalf = iDiff / 2;

			m_SafeMargin.setLeft(m_Margin.left() - iHalf);
			m_SafeMargin.setRight(m_Margin.right() - (iDiff - iHalf));
		}
		else {
			m_SafeMargin.setLeft(m_Margin.left());
			m_SafeMargin.setRight(m_Margin.right());
		}

		iDiff = m_Margin.top() + m_Margin.bottom() - m_Coord.height();

		if (iDiff > 0) {
			iHalf = iDiff / 2;

			m_SafeMargin.setTop(m_Margin.top() - iHalf);
			m_SafeMargin.setBottom(m_Margin.bottom() - (iDiff - iHalf));
		}
		else {
			m_SafeMargin.setTop(m_Margin.top());
			m_SafeMargin.setBottom(m_Margin.bottom());
		}
	}

	bool SsNines::updatePoints()
	{
		m_fX2 = m_Size.width() * 0.5f;
		m_fY2 = m_Size.height() * 0.5f;
		m_f0X = -m_fX2;
		m_f3X = m_fX2;
		m_f1X = m_f0X + m_SafeMargin.left();
		m_f2X = m_f3X - m_SafeMargin.right();
		m_f0Y = -m_fY2;
		m_f3Y = m_fY2;
		m_f1Y = m_f0Y + m_SafeMargin.bottom();
		m_f2Y = m_f3Y - m_SafeMargin.top();
		m_f0U = m_Coord.left() / m_Region.width();
		m_f3U = m_Coord.right() / m_Region.width();
		m_f1U = (m_Coord.left() + m_SafeMargin.left()) / m_Region.width();
		m_f2U = (m_Coord.right() - m_SafeMargin.right()) / m_Region.width();
		m_f0V = m_Coord.bottom() / m_Region.height();
		m_f3V = m_Coord.top() / m_Region.height();
		m_f1V = (m_Coord.bottom() - m_SafeMargin.bottom()) / m_Region.height();
		m_f2V = (m_Coord.top() + m_SafeMargin.top()) / m_Region.height();

		return	true;
	}

	bool SsNines::createStretchPolygon()
	{
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f0X, m_f0Y), SsPoint2(m_f0U, m_f0V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f0X, m_f1Y), SsPoint2(m_f0U, m_f1V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f1X, m_f0Y), SsPoint2(m_f1U, m_f0V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f1X, m_f1Y), SsPoint2(m_f1U, m_f1V)));

		m_Polygon.addVertex(SsVertex(SsPoint2(m_f2X, m_f0Y), SsPoint2(m_f2U, m_f0V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f2X, m_f1Y), SsPoint2(m_f2U, m_f1V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f3X, m_f0Y), SsPoint2(m_f3U, m_f0V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f3X, m_f1Y), SsPoint2(m_f3U, m_f1V)));

		m_Polygon.addVertex(SsVertex(SsPoint2(m_f0X, m_f2Y), SsPoint2(m_f0U, m_f2V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f0X, m_f3Y), SsPoint2(m_f0U, m_f3V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f1X, m_f2Y), SsPoint2(m_f1U, m_f2V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f1X, m_f3Y), SsPoint2(m_f1U, m_f3V)));

		m_Polygon.addVertex(SsVertex(SsPoint2(m_f2X, m_f2Y), SsPoint2(m_f2U, m_f2V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f2X, m_f3Y), SsPoint2(m_f2U, m_f3V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f3X, m_f2Y), SsPoint2(m_f3U, m_f2V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f3X, m_f3Y), SsPoint2(m_f3U, m_f3V)));

		m_Polygon.addVertexIndex(0);
		m_Polygon.addVertexIndex(1);
		m_Polygon.addVertexIndex(2);
		m_Polygon.addVertexIndex(3);
		m_Polygon.addVertexIndex(4);
		m_Polygon.addVertexIndex(5);
		m_Polygon.addVertexIndex(6);
		m_Polygon.addVertexIndex(7);
		m_Polygon.addStride(8);

		m_Polygon.addVertexIndex(8);
		m_Polygon.addVertexIndex(9);
		m_Polygon.addVertexIndex(10);
		m_Polygon.addVertexIndex(11);
		m_Polygon.addVertexIndex(12);
		m_Polygon.addVertexIndex(13);
		m_Polygon.addVertexIndex(14);
		m_Polygon.addVertexIndex(15);
		m_Polygon.addStride(8);

		m_Polygon.addVertexIndex(1);
		m_Polygon.addVertexIndex(8);
		m_Polygon.addVertexIndex(3);
		m_Polygon.addVertexIndex(10);
		m_Polygon.addVertexIndex(5);
		m_Polygon.addVertexIndex(12);
		m_Polygon.addVertexIndex(7);
		m_Polygon.addVertexIndex(14);
		m_Polygon.addStride(8);

		return	true;
	}

	bool SsNines::createTilePolygon()
	{
		bool	bResult = true;

		bResult = bResult && createTilePolygonCC();

		bResult = bResult && createTilePolygonLC();
		bResult = bResult && createTilePolygonRC();
		bResult = bResult && createTilePolygonCT();
		bResult = bResult && createTilePolygonCB();

		bResult = bResult && createTilePolygonLT();
		bResult = bResult && createTilePolygonRT();
		bResult = bResult && createTilePolygonLB();
		bResult = bResult && createTilePolygonRB();

		return	bResult;
	}

	bool SsNines::createTilePolygonCC()
	{
		float	fDstW = m_f2X - m_f1X;
		float	fDstH = m_f2Y - m_f1Y;
		float	fSrcW = m_Coord.width() - m_SafeMargin.left() - m_SafeMargin.right();
		float	fSrcH = m_Coord.height() - m_SafeMargin.top() - m_SafeMargin.bottom();
		float	fX0;
		float	fY0;
		float	fX1;
		float	fY1;
		float	fU0;
		float	fV0;
		float	fU1;
		float	fV1;
		int		iDivW = SsMath::Ceil(fDstW / fSrcW);
		int		iDivH = SsMath::Ceil(fDstH / fSrcH);
		int		iStride;
		int		iIndex = m_Polygon.getVertexCount();

		for (int iY = 0; iY < iDivH; iY++) {
			fY0 = m_f1Y + (fSrcH * iY);
			fY1 = fY0 + fSrcH;

			fV0 = m_f1V;
			fV1 = m_f2V;

			if (fY1 > m_f2Y) {
				fV1 = (m_Coord.top() + m_SafeMargin.top() + (fY1 - m_f2Y)) / m_Region.height();
				fY1 = m_f2Y;
			}

			for (int iX = 0; iX < iDivW; iX++) {
				fX0 = m_f1X + (fSrcW * iX);
				fX1 = fX0 + fSrcW;

				fU0 = m_f1U;
				fU1 = m_f2U;

				if (fX1 > m_f2X) {
					fU1 = (m_Coord.right() - m_SafeMargin.right() - (fX1 - m_f2X)) / m_Region.width();
					fX1 = m_f2X;
				}

				m_Polygon.addVertex(SsVertex(SsPoint2(fX0, fY0), SsPoint2(fU0, fV0)));
				m_Polygon.addVertex(SsVertex(SsPoint2(fX0, fY1), SsPoint2(fU0, fV1)));
				m_Polygon.addVertex(SsVertex(SsPoint2(fX1, fY0), SsPoint2(fU1, fV0)));
				m_Polygon.addVertex(SsVertex(SsPoint2(fX1, fY1), SsPoint2(fU1, fV1)));
			}

			iStride = iDivW * 4;

			for (int i = 0; i < iStride; i++) {
				m_Polygon.addVertexIndex(iIndex++);
			}

			m_Polygon.addStride(iStride);
		}

		return	true;
	}

	bool SsNines::createTilePolygonLC()
	{
		float	fDstH = m_f2Y - m_f1Y;
		float	fSrcH = m_Coord.height() - m_SafeMargin.top() - m_SafeMargin.bottom();
		float	fY0;
		float	fY1;
		float	fV0;
		float	fV1;
		int		iDivH = SsMath::Ceil(fDstH / fSrcH);
		int		iStride;
		int		iIndex = m_Polygon.getVertexCount();

		for (int iY = 0; iY < iDivH; iY++) {
			fY0 = m_f1Y + (fSrcH * iY);
			fY1 = fY0 + fSrcH;

			fV0 = m_f1V;
			fV1 = m_f2V;

			if (fY1 > m_f2Y) {
				fV1 = (m_Coord.top() + m_SafeMargin.top() + (fY1 - m_f2Y)) / m_Region.height();
				fY1 = m_f2Y;
			}

			m_Polygon.addVertex(SsVertex(SsPoint2(m_f0X, fY0), SsPoint2(m_f0U, fV0)));
			m_Polygon.addVertex(SsVertex(SsPoint2(m_f1X, fY0), SsPoint2(m_f1U, fV0)));
			m_Polygon.addVertex(SsVertex(SsPoint2(m_f0X, fY1), SsPoint2(m_f0U, fV1)));
			m_Polygon.addVertex(SsVertex(SsPoint2(m_f1X, fY1), SsPoint2(m_f1U, fV1)));
		}

		iStride = iDivH * 4;

		for (int i = 0; i < iStride; i++) {
			m_Polygon.addVertexIndex(iIndex++);
		}

		m_Polygon.addStride(iStride);

		return	true;
	}

	bool SsNines::createTilePolygonRC()
	{
		float	fDstH = m_f2Y - m_f1Y;
		float	fSrcH = m_Coord.height() - m_SafeMargin.top() - m_SafeMargin.bottom();
		float	fY0;
		float	fY1;
		float	fV0;
		float	fV1;
		int		iDivH = SsMath::Ceil(fDstH / fSrcH);
		int		iStride;
		int		iIndex = m_Polygon.getVertexCount();

		for (int iY = 0; iY < iDivH; iY++) {
			fY0 = m_f1Y + (fSrcH * iY);
			fY1 = fY0 + fSrcH;

			fV0 = m_f1V;
			fV1 = m_f2V;

			if (fY1 > m_f2Y) {
				fV1 = (m_Coord.top() + m_SafeMargin.top() + (fY1 - m_f2Y)) / m_Region.height();
				fY1 = m_f2Y;
			}

			m_Polygon.addVertex(SsVertex(SsPoint2(m_f2X, fY0), SsPoint2(m_f2U, fV0)));
			m_Polygon.addVertex(SsVertex(SsPoint2(m_f3X, fY0), SsPoint2(m_f3U, fV0)));
			m_Polygon.addVertex(SsVertex(SsPoint2(m_f2X, fY1), SsPoint2(m_f2U, fV1)));
			m_Polygon.addVertex(SsVertex(SsPoint2(m_f3X, fY1), SsPoint2(m_f3U, fV1)));
		}

		iStride = iDivH * 4;

		for (int i = 0; i < iStride; i++) {
			m_Polygon.addVertexIndex(iIndex++);
		}

		m_Polygon.addStride(iStride);

		return	true;
	}

	bool SsNines::createTilePolygonCT()
	{
		float	fDstW = m_f2X - m_f1X;
		float	fSrcW = m_Coord.width() - m_SafeMargin.left() - m_SafeMargin.right();
		float	fX0;
		float	fX1;
		float	fU0;
		float	fU1;
		int		iDivW = SsMath::Ceil(fDstW / fSrcW);
		int		iStride;
		int		iIndex = m_Polygon.getVertexCount();

		for (int iX = 0; iX < iDivW; iX++) {
			fX0 = m_f1X + (fSrcW * iX);
			fX1 = fX0 + fSrcW;

			fU0 = m_f1U;
			fU1 = m_f2U;

			if (fX1 > m_f2X) {
				fU1 = (m_Coord.right() - m_SafeMargin.right() - (fX1 - m_f2X)) / m_Region.width();
				fX1 = m_f2X;
			}

			m_Polygon.addVertex(SsVertex(SsPoint2(fX0, m_f0Y), SsPoint2(fU0, m_f0V)));
			m_Polygon.addVertex(SsVertex(SsPoint2(fX0, m_f1Y), SsPoint2(fU0, m_f1V)));
			m_Polygon.addVertex(SsVertex(SsPoint2(fX1, m_f0Y), SsPoint2(fU1, m_f0V)));
			m_Polygon.addVertex(SsVertex(SsPoint2(fX1, m_f1Y), SsPoint2(fU1, m_f1V)));
		}

		iStride = iDivW * 4;

		for (int i = 0; i < iStride; i++) {
			m_Polygon.addVertexIndex(iIndex++);
		}

		m_Polygon.addStride(iStride);

		return	true;
	}

	bool SsNines::createTilePolygonCB()
	{
		float	fDstW = m_f2X - m_f1X;
		float	fSrcW = m_Coord.width() - m_SafeMargin.left() - m_SafeMargin.right();
		float	fX0;
		float	fX1;
		float	fU0;
		float	fU1;
		int		iDivW = SsMath::Ceil(fDstW / fSrcW);
		int		iStride;
		int		iIndex = m_Polygon.getVertexCount();

		for (int iX = 0; iX < iDivW; iX++) {
			fX0 = m_f1X + (fSrcW * iX);
			fX1 = fX0 + fSrcW;

			fU0 = m_f1U;
			fU1 = m_f2U;

			if (fX1 > m_f2X) {
				fU1 = (m_Coord.right() - m_SafeMargin.right() - (fX1 - m_f2X)) / m_Region.width();
				fX1 = m_f2X;
			}

			m_Polygon.addVertex(SsVertex(SsPoint2(fX0, m_f2Y), SsPoint2(fU0, m_f2V)));
			m_Polygon.addVertex(SsVertex(SsPoint2(fX0, m_f3Y), SsPoint2(fU0, m_f3V)));
			m_Polygon.addVertex(SsVertex(SsPoint2(fX1, m_f2Y), SsPoint2(fU1, m_f2V)));
			m_Polygon.addVertex(SsVertex(SsPoint2(fX1, m_f3Y), SsPoint2(fU1, m_f3V)));
		}

		iStride = iDivW * 4;

		for (int i = 0; i < iStride; i++) {
			m_Polygon.addVertexIndex(iIndex++);
		}

		m_Polygon.addStride(iStride);

		return	true;
	}

	bool SsNines::createTilePolygonLT()
	{
		int		iStride;
		int		iIndex = m_Polygon.getVertexCount();

		m_Polygon.addVertex(SsVertex(SsPoint2(m_f0X, m_f0Y), SsPoint2(m_f0U, m_f0V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f0X, m_f1Y), SsPoint2(m_f0U, m_f1V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f1X, m_f0Y), SsPoint2(m_f1U, m_f0V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f1X, m_f1Y), SsPoint2(m_f1U, m_f1V)));

		iStride = 4;

		for (int i = 0; i < iStride; i++) {
			m_Polygon.addVertexIndex(iIndex++);
		}

		m_Polygon.addStride(iStride);

		return	true;
	}

	bool SsNines::createTilePolygonRT()
	{
		int		iStride;
		int		iIndex = m_Polygon.getVertexCount();

		m_Polygon.addVertex(SsVertex(SsPoint2(m_f2X, m_f0Y), SsPoint2(m_f2U, m_f0V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f2X, m_f1Y), SsPoint2(m_f2U, m_f1V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f3X, m_f0Y), SsPoint2(m_f3U, m_f0V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f3X, m_f1Y), SsPoint2(m_f3U, m_f1V)));

		iStride = 4;

		for (int i = 0; i < iStride; i++) {
			m_Polygon.addVertexIndex(iIndex++);
		}

		m_Polygon.addStride(iStride);

		return	true;
	}

	bool SsNines::createTilePolygonLB()
	{
		int		iStride;
		int		iIndex = m_Polygon.getVertexCount();

		m_Polygon.addVertex(SsVertex(SsPoint2(m_f0X, m_f2Y), SsPoint2(m_f0U, m_f2V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f0X, m_f3Y), SsPoint2(m_f0U, m_f3V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f1X, m_f2Y), SsPoint2(m_f1U, m_f2V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f1X, m_f3Y), SsPoint2(m_f1U, m_f3V)));

		iStride = 4;

		for (int i = 0; i < iStride; i++) {
			m_Polygon.addVertexIndex(iIndex++);
		}

		m_Polygon.addStride(iStride);

		return	true;
	}

	bool SsNines::createTilePolygonRB()
	{
		int		iStride;
		int		iIndex = m_Polygon.getVertexCount();

		m_Polygon.addVertex(SsVertex(SsPoint2(m_f2X, m_f2Y), SsPoint2(m_f2U, m_f2V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f2X, m_f3Y), SsPoint2(m_f2U, m_f3V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f3X, m_f2Y), SsPoint2(m_f3U, m_f2V)));
		m_Polygon.addVertex(SsVertex(SsPoint2(m_f3X, m_f3Y), SsPoint2(m_f3U, m_f3V)));

		iStride = 4;

		for (int i = 0; i < iStride; i++) {
			m_Polygon.addVertexIndex(iIndex++);
		}

		m_Polygon.addStride(iStride);

		return	true;
	}

};