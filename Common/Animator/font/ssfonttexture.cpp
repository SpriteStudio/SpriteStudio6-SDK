/*!
 * \file	ssfonttexture.cpp
 * \author	CRI Middleware Co., Ltd.
*/
#include "ssfonttexture.h"

#include "sstypes.h"
#include "ssplayer_math.h"
#include "sscharmap.h"

#include"../Helper/SsTextureFactory.h"
#include "../Helper/SsRawImage.h"

#include "../Helper/UnicodeConv.h"

namespace SpriteStudio
{

#if 0
	//#include "SsController.h"

	//! 白を調整
	/*!
	 * \param	pBitmap		イメージ
	*/
	static void adjustWhite(QImage* pBitmap)
	{
		unsigned char* pData;
		int				iWidth;
		int				iHeight;
		int				iPixCount;
		int				iPixIdx;
		int				iA;

		pData = reinterpret_cast<unsigned char*>(pBitmap->bits());
		iWidth = pBitmap->width();
		iHeight = pBitmap->height();

		iPixCount = iWidth * iHeight;

		for (int i = 0; i < iPixCount; i++) {
			iPixIdx = i * 4;

#ifdef _WIN32
			iA = (pData[iPixIdx + 0] + pData[iPixIdx + 1] + pData[iPixIdx + 2]) / 3;
#else
			iA = (pData[iPixIdx + 3]);
#endif

			if (iA < 0x00) iA = 0x00;
			if (iA > 0xFF) iA = 0xFF;

			iA >>= 4;
			iA &= 0x0F;
			iA |= ((iA << 4) & 0xF0);

			pData[iPixIdx + 3] = static_cast<unsigned char>(iA);

			pData[iPixIdx + 0] = pData[iPixIdx + 1] = pData[iPixIdx + 2] = 0xFF;
		}
	}

#endif

	static SsRectI makeDstRect(const SsPoint2& point,  const SsRectI& rectSrc, double dScale)
	{
		SsRectI	rectDst;

		rectDst.setX(point.x);
		rectDst.setY(point.y);

		int width = rectSrc.width();

		rectDst.setWidth((int)((double)rectSrc.width() * dScale));
		rectDst.setHeight((int)((double)rectSrc.height() * dScale));

		return	rectDst;
	}

#if 0 // updateFromFamilyで使用
	static SsString toWrappedText(const SsString& strText, int iWidth, const QFont& font)
	{
		SsString		strLine = "";
		SsString		strResult = "";

		foreach(const QChar & e, strText) {
			if (e == '\n') {
				strLine.clear();
				strResult.push_back('\n');
				continue;
			}

			QStaticText		text(strLine.push_back(e));

			text.prepare(QTransform(), font);

			SsSizeF	size = text.size();

			if (size.width() > iWidth) {
				strLine.clear();
				strLine.push_back(e);
				strResult.push_back('\n');
			}

			strResult.push_back(e);
		}

		return	strResult;
	}
#endif

	//! コンストラクタ
	SsFontTexture::SsFontTexture()
	{
		//m_myProject = 0;
		m_pCharMap = nullptr;

		setText("");
		setGridSize(SsSize(0, 0));
		setFixedSize(SsSize(0, 0));

		m_iImageWidth = 0;
		m_iImageHeight = 0;
		m_iTextureWidth = 0;
		m_iTextureHeight = 0;

		m_bUpdate = true;

		m_pImage = nullptr;
		m_pTexture = nullptr;

#if 0
		m_pText = nullptr;
#endif
		m_pBitmap = nullptr;
	}

	//! デストラクタ
	SsFontTexture::~SsFontTexture()
	{
#if 0
		// SsOpenGLImage メンバの SsBitmap のデストラクタでは破棄されないようなので。
		if (m_pBitmap)
		{
			delete m_pBitmap;
			m_pBitmap = nullptr;
		}

		if (m_pImage) {
			delete m_pImage;
			m_pImage = nullptr;
		}
		if (m_pTexture) {
			SsOpenGLTextureManager::instance()->deleteTexture(m_pTexture);
			m_pTexture = nullptr;
		}

		if (m_pText) {
			delete m_pText;
			m_pText = nullptr;
		}
#endif
	}

	//! テキストを設定
	void SsFontTexture::setText(const SsString& strText)
	{
		if (m_strText != strText) {
			m_strText = strText;
			m_bUpdate = true;
		}
	}

	//! フォント設定を設定
	void SsFontTexture::setFontDesc(const SsFontDesc& fontDesc)
	{
		if (m_FontDesc != fontDesc) {
			m_FontDesc = fontDesc;
			m_bUpdate = true;
		}
	}

	//! グリッドサイズを設定
	void SsFontTexture::setGridSize(const SsSize& size)
	{
		if (m_GridSize != size) {
			m_GridSize = size;
			m_bUpdate = true;
		}
	}

	//! 固定サイズを設定
	void SsFontTexture::setFixedSize(const SsSize& size)
	{
		if (m_FixedSize != size) {
			m_FixedSize = size;
			m_bUpdate = true;
		}
	}

	//! テキストを取得
	SsString SsFontTexture::getText() const
	{
		return	m_strText;
	}

	//! フォント設定を取得
	SsFontDesc SsFontTexture::getFontDesc() const
	{
		return	m_FontDesc;
	}

	//! グリッドサイズを取得
	SsSize SsFontTexture::getGridSize() const
	{
		return	m_GridSize;
	}

	//! 固定サイズを取得
	SsSize SsFontTexture::getFixedSize() const
	{
		return	m_FixedSize;
	}

	//! 更新
	void SsFontTexture::update()
	{
		if (!m_bUpdate) {
			return;
		}

		if (m_FontDesc.isBitmap()) {
			updateFromCharMap();
		}
		else {
			updateFromFamily();
		}


		m_bUpdate = false;
	}

	//! イメージ幅を取得
	int SsFontTexture::getImageWidth() const
	{
		return	m_iImageWidth;
	}

	//! イメージ高さを取得
	int SsFontTexture::getImageHeight() const
	{
		return	m_iImageHeight;
	}

	//! テクスチャ幅を取得
	int SsFontTexture::getTextureWidth() const
	{
		return	m_iTextureWidth;
	}

	//! テクスチャ高さを取得
	int SsFontTexture::getTextureHeight() const
	{
		return	m_iTextureHeight;
	}

	//! ＧＬテクスチャを取得
	ISSTexture* SsFontTexture::getTexture() const
	{
		return	m_pTexture;
	}

#ifdef SSFONTTEXTURE_SET_BITMAP
	//! イメージを設定
	void SsFontTexture::setBitmap(QImage* pImage)
	{

		m_bUpdate = false;
	}
#endif // SSFONTTEXTURE_SET_BITMAP

#ifdef SSFONTTEXTURE_GET_BITMAP
	//! イメージを取得
	QImage* SsFontTexture::getBitmap() const
	{
		return	m_pBitmap;
	}
#endif // SSFONTTEXTURE_GET_BITMAP

	//! ファミリから更新
	void SsFontTexture::updateFromFamily()
	{

	}

	//! キャラマップから更新
	void SsFontTexture::updateFromCharMap()
	{


		SsString chamapName = m_FontDesc.getCharMap();
		SsCharMap* pCharMap = m_pCharMap;


		if (!pCharMap) {
			return;
		}
		

		typedef std::pair<int, std::pair<SsRectI, SsRectI>>		Shape;

		SsRectI	bound;
		int		iLineHeight;
		int		iPage;
		SsPoint2	point;
		SsRectI	rectSrc;
		SsRectI	rectDst;
		int		iAdvance;
		SsPoint2	offset;
		bool	bInit;
		int		iL, iR, iT, iB;
		iL = iR = iT = iB = 0;
		double	dScale;
		float	fSpace;
		bool	bFixed = (m_FixedSize.width() > 0) || (m_FixedSize.height() > 0);

		std::map<int, std::list<Shape>*>	map;
		std::list<Shape>* pListShape;
		Shape						shape;
		std::vector<int>					listL;
		std::vector<int>					listR;
		std::vector<int>					listW;
		int							iLine;
		bool						bLine;

		bInit = true;
		listL.clear();
		listR.clear();
		listW.clear();
		listL.push_back(0);
		listR.push_back(0);
		iLine = 0;
		bLine = true;

		iLineHeight = pCharMap->getCommon().getLineHeight();

		dScale = (double)m_FontDesc.getSize() / (double)iLineHeight;
		fSpace = m_FontDesc.getSpace();


		SsString str = getText();
		int char_size = 0;
		unsigned char lead;;

		///u8 -> u16文字列へ
		SsString temp = getText();
		std::u16string text_list;
		ConvU8ToU16(temp, text_list);

		for(SsChar e : text_list ){

			if (pCharMap->getChars().hasKey(e)) {
				iPage = pCharMap->getChars().getPage(e);
				point = offset + ((*pCharMap->getChars().getOffset(e) - *pCharMap->getChars().getOffset()) * dScale);
				rectSrc = *pCharMap->getChars().getSrcRect(e);
				rectDst = makeDstRect(point, rectSrc, dScale);
				iAdvance = (pCharMap->getChars().getAdvance(e) * dScale);

				if (bFixed) {
					if (point.x + rectDst.width() > m_FixedSize.width()) {
						listL.push_back(0);
						listR.push_back(0);
						iLine++;
						bLine = true;
						offset.setX(0);
						offset.setY(offset.y + m_FontDesc.getSize());

						point = offset + ((*pCharMap->getChars().getOffset(e) - *pCharMap->getChars().getOffset()) * dScale);
						rectDst = makeDstRect(point, rectSrc, dScale);
					}
				}

				if (!map.count(iPage)>0) {
					map[iPage] = new std::list<Shape>();
				}
				pListShape = map[iPage];

				shape.first = iLine;
				shape.second = std::make_pair(rectSrc, rectDst);
				pListShape->push_back(shape);

				if (bInit) {
					iL = point.x;
					iT = point.y;
					iR = iL + rectDst.width();
					iB = iT + rectDst.height();
					bInit = false;
				}
				else {
					if (iL > point.x) iL = point.x;
					if (iT > point.y) iT = point.y;
					if (iR < point.x + rectDst.width()) iR = point.x + rectDst.width();
					if (iB < point.y + rectDst.height()) iB = point.y + rectDst.height();
				}

				if (bLine || listL[iLine] > point.x) {
					listL[iLine] = point.x;
				}
				if (bLine || listR[iLine] < point.x + rectDst.width()) {
					listR[iLine] = point.x + rectDst.width();
				}
				bLine = false;

				offset.setX(offset.x + iAdvance + fSpace);
			}
			else
				if (e == '\n') {
					listL.push_back(0);
					listR.push_back(0);
					iLine++;
					bLine = true;
					offset.setX(0);
					offset.setY(offset.y + m_FontDesc.getSize());
				}
		}

		bound.setCoords(iL, iT, iR, iB);

		m_iImageWidth = bound.left() * 2 + bound.width();
		m_iImageHeight = bound.top() * 2 + bound.height();

		if (bFixed) {
			if (m_FixedSize.width() > 0) m_iImageWidth = m_FixedSize.width();
			if (m_FixedSize.height() > 0) m_iImageHeight = m_FixedSize.height();
		}

		if (m_iImageWidth < 8) m_iImageWidth = 8;
		if (m_iImageHeight < 8) m_iImageHeight = 8;
		if (m_iImageWidth > 8192) m_iImageWidth = 8192;
		if (m_iImageHeight > 8192) m_iImageHeight = 8192;

		m_iTextureWidth = SsMath::convPow2(m_iImageWidth);
		m_iTextureHeight = SsMath::convPow2(m_iImageHeight);


#if 1
		//Bitmapイメージを作成する
		if (!m_pBitmap) {

			m_pBitmap = new SsRawImage(m_iTextureWidth, m_iTextureHeight);
		}
		else {
			//サイズが異なっている
			if ((m_pBitmap->getWidth() != m_iTextureWidth) || (m_pBitmap->getHeight() != m_iTextureHeight)) {
				if (m_pImage) {
					delete m_pImage;
					m_pImage = nullptr;
				}

				//if (m_pTexture) {
				//	SsOpenGLTextureManager::instance()->deleteTexture(m_pTexture);
				//	m_pTexture = nullptr;
				//}

				m_pBitmap = new SsRawImage(m_iTextureWidth, m_iTextureHeight);
			}
		}

#endif
		//m_pBitmap->fill(SsColor(0xffffffff));

		int	a = SsAnchorButton::toAlignment(m_FontDesc.getAnchor());

		for (int i = 0; i < listL.size(); i++) {
			listW.push_back(m_iImageWidth - (listR[i]));
		}

		if (a & SsAnchorButton::AlignLeft) {
			for (int i = 0; i < listW.size(); i++) {
				listW[i] *= 0;
			}
		}
		else
			if (a & SsAnchorButton::AlignHCenter) {
				for (int i = 0; i < listW.size(); i++) {
					listW[i] /= 2;
				}
			}
			else
				if (a & SsAnchorButton::AlignRight) {
					for (int i = 0; i < listW.size(); i++) {
						listW[i] *= 1;
					}
				}

		iLine = m_iImageHeight - (iB);

		if (a & SsAnchorButton::AlignTop) {
			iLine *= 0;
		}
		else {

			if (a & SsAnchorButton::AlignVCenter) {
				iLine /= 2;
			}
			else if (a & SsAnchorButton::AlignBottom) 
			{
				iLine *= 1;
			}
		}


		//Bitmapfontから転送
		for(auto item: map) {
			int e = item.first;

			SsRawImage* pImage = pCharMap->getPages().getImage(e);

			for (auto item2 : *map[e]) //Shape
			{
				SsRectI srcRect = item2.second.first;
				SsRectI dstRect = item2.second.second;

				int offsetx = listW[item2.first];
				int offsety = iLine;

				m_pBitmap->Bitblt(pImage, srcRect, dstRect);

			}

		}

//		foreach(int e, map.keys()) {
		for(auto e :  map ) {
			delete map[e.first];
		}
		map.clear();

		//	adjustWhite( m_pBitmap );

		if (m_pTexture)
		{
			delete m_pTexture;
		}

		//表示用のテクスチャを作成する
		m_pTexture = SSTextureFactory::create(m_pBitmap);

		calcCell();

#if 0
		if (!m_pImage) {
			m_pImage = new SsOpenGLImage(m_pBitmap);
		}

		bool	bResend = false;

		if (m_pTexture == nullptr) {
			m_pTexture = SsOpenGLTextureManager::instance()->newTexture(m_pImage);
			if (m_pTexture) m_pTexture->setGroup(1);
		}
		else {
			bResend = true;
		}

		if (m_pTexture != nullptr) {
			if (bResend) {
				SsOpenGLTextureManager::instance()->resend(m_pTexture);
			}
			else {
				SsOpenGLTextureManager::instance()->send(m_pTexture);
			}
		}
#endif
	}
	void SsFontTexture::calcCell()
	{

		m_CellMap.pixelSize = SsPoint2(getTextureWidth(), getTextureHeight());
		//m_CellMap.parent = nullptr;

		m_CellMap.overrideTexSettings = true;
		bool m_bSmooth = m_valueText->isSmooth();

		m_CellMap.filterMode = m_bSmooth ? SsTexFilterMode::linear : SsTexFilterMode::nearlest;
		m_CellMap.wrapMode = SsTexWrapMode::clamp;

		//m_CellMap.image = &m_Image;

		//m_Cell.map = &m_CellMap;
		m_Cell.pos = SsVector2(0, 0);
		m_Cell.size = SsVector2(this->getImageWidth(), this->getImageHeight());
		m_Cell.pivot = SsVector2(0, 0);

		m_Cell.parentSize = SsPoint2(this->getTextureWidth(), this->getTextureHeight());
	}
};