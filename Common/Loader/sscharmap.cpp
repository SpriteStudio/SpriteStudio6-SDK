#include "sscharmap.h"
#include <iostream>
#include <string>
#include <algorithm>
#include <iostream>
#include<fstream>

#include "ssloader_sspj.h"


namespace SpriteStudio
{


	const std::string WHITESPACE = " \n\r\t\f\v";

	std::string ltrim(const std::string& s)
	{
		size_t start = s.find_first_not_of(WHITESPACE);
		return (start == std::string::npos) ? "" : s.substr(start);
	}

	std::string rtrim(const std::string& s)
	{
		size_t end = s.find_last_not_of(WHITESPACE);
		return (end == std::string::npos) ? "" : s.substr(0, end + 1);
	}

	std::string trim(const std::string& s) {
		return rtrim(ltrim(s));
	}


	static int getInteger(const SsString& str)
	{
		//bool	bOk;
		int		iValue;


		std::string str2;

		//iValue = str.toInt(&bOk);
		iValue = std::stoi(str);


		//if (!bOk) {
		//	return	0;
		//}

		return	iValue;
	}

	static bool getBoolean(const SsString& str)
	{
		return	(getInteger(str) != 0);
	}

	static SsString getString(const SsString& str, const SsChar& cStart = '"', const SsChar& cEnd = '"')
	{
		int		iLeft;
		int		iRight;


		//iLeft = str.indexOf(cStart);
		//iRight = str.lastIndexOf(cEnd);

		iLeft = str.find_first_of(cStart);
		iRight = str.find_last_of(cEnd);

		if ((iLeft < 0) || (iRight < 0)) {
			return	"";
		}

		iLeft++;


		return str.substr(iLeft, iRight - iLeft);

		//return	str.mid(iLeft, iRight - iLeft);
	}

	static SsStringList splitString(const SsString& str, const SsChar& cSplit, const SsChar& cIgnoreStart = '"', const SsChar& cIgnoreEnd = '"')
	{
		SsStringList		list;
		SsString			strSplit;
		SsChar			c;
		int				iLength;
		int				iStart;
		int				iCount;
		bool			bIgnore;

		iLength = str.length();
		iStart = 0;
		iCount = 0;
		bIgnore = false;

		for (int i = 0; i < iLength; i++) {
			c = str.at(i);

			if (bIgnore) {
				if (c == cIgnoreEnd) {
					bIgnore = false;
				}
			}
			else {
				if (c == cIgnoreStart) {
					bIgnore = true;
				}
				else
					if (c == cSplit) {
//						strSplit = str.mid(iStart, iCount);
						strSplit = str.substr(iStart, iCount);

						if (strSplit.length() > 0) {
							//list.append(strSplit);
							list.push_back(strSplit);
						}

						iStart = i + 1;
						iCount = 0;
						continue;
					}
			}

			iCount++;
		}

//		strSplit = str.mid(iStart);
		strSplit = str.substr(iStart);

		if (strSplit.length() > 0) {
//			list.append(strSplit);
			list.push_back(strSplit);
		}

		return	list;
	}

	class ParamProc
	{
	public:
		ParamProc(SsCharMap* pCharMap)
		{
			m_pCharMap = pCharMap;

			m_mapParam.clear();
		}

		~ParamProc()
		{
			m_pCharMap = nullptr;

			m_mapParam.clear();
		}

		bool getNameAndValue(const SsString& strParam, SsString& strName, SsString& strValue)
		{
			SsStringList		nameAndValue;

			nameAndValue = splitString(strParam, '=');

			if (nameAndValue.size() != 2) {
				return	false;
			}


			//strName = nameAndValue[0].trimmed().toLower();
			//strValue = nameAndValue[1].trimmed();


			strName = trim(nameAndValue[0]);
			std::transform(strName.cbegin(), strName.cend(), strName.begin(), tolower);

			strValue = trim(nameAndValue[1]);

			
			return	true;
		}

		virtual bool init()
		{
			m_mapParam.clear();

			return	true;
		}

		virtual bool push(const SsString& strParam)
		{
			SsString		strName;
			SsString		strValue;

			if (!getNameAndValue(strParam, strName, strValue)) {
				return	false;
			}

			m_mapParam[strName] = strValue;

			return	true;
		}

		virtual bool exec()
		{
			return	true;
		}

	protected:
		SsCharMap* getCharMap()
		{
			return	m_pCharMap;
		}

		const std::map<SsString, SsString>& getParams()
		{
			return	m_mapParam;
		}

	private:
		SsCharMap* m_pCharMap;

		std::map<SsString, SsString>	m_mapParam;
	};

	class InfoParamProc : public ParamProc
	{
	public:
		InfoParamProc(SsCharMap* pCharMap) : ParamProc(pCharMap) {}

		virtual bool exec() override
		{
			const std::map<SsString, SsString>& mapParams = getParams();

			//foreach(const SsString & e, mapParams.keys()) {
			for( auto item : mapParams ){
				const SsString e = item.first;
				//getCharMap()->getInfo().set(e, mapParams[e]);
				getCharMap()->getInfo().set(e, item.second);
			}

			return	true;
		}
	};

	class CommonParamProc : public ParamProc
	{
	public:
		CommonParamProc(SsCharMap* pCharMap) : ParamProc(pCharMap) {}

		virtual bool exec() override
		{
			const std::map<SsString, SsString>& mapParams = getParams();

			//foreach(const SsString & e, mapParams.keys()) {
			for (auto item : mapParams) {
				const SsString e = item.first;
				getCharMap()->getCommon().set(e, item.second);
				//getCharMap()->getCommon().set(e, mapParams[e]);
			}

			return	true;
		}
	};

	class PagesParamProc : public ParamProc
	{
	public:
		PagesParamProc(SsCharMap* pCharMap) : ParamProc(pCharMap) {}

		virtual bool exec() override
		{
			const std::map<SsString, SsString>& mapParams = getParams();

			int		iId = -1;

//			foreach(const SsString & e, mapParams.keys()) {
			for (auto item : mapParams) {
				auto e = item.first;
				if (e == "id") {
//					iId = getInteger(mapParams[e]);
					iId = getInteger(item.second);
					break;
				}
			}

			if (iId < 0) {
				return	false;
			}

			for (auto item : mapParams) {
				auto e = item.first;
				//foreach(const SsString & e, mapParams.keys()) {
//				getCharMap()->getPages().set(iId, e, mapParams[e]);
				getCharMap()->getPages().set(iId, e, item.second);
			}

			getCharMap()->getPages().setDir(iId, getCharMap()->getDir());
			//getCharMap()->getPages().loadImage(iId, gController.project_);
			getCharMap()->getPages().loadImage(iId, 0 );

			return	true;
		}
	};

	class CharsParamProc : public ParamProc
	{
	public:
		CharsParamProc(SsCharMap* pCharMap) : ParamProc(pCharMap) {}

		virtual bool exec() override
		{
			const std::map<SsString, SsString>& mapParams = getParams();

			int		iId = -1;

//			foreach(const SsString & e, mapParams.keys()) {
			for (auto item : mapParams) {
				auto e = item.first;
				if (e == "id") {
//					iId = getInteger(mapParams[e]);
					iId = getInteger(item.second);
					break;
				}
			}

			if (iId < 0) {
				return	false;
			}

//			foreach(const SsString & e, mapParams.keys()) {
			for (const auto item : mapParams) {
				auto e = item.first;
//				getCharMap()->getChars().set(iId, e, mapParams[e]);
				getCharMap()->getChars().set(iId, e, item.second);
			}

			return	true;
		}
	};

	class KerningsParamProc : public ParamProc
	{
	public:
		KerningsParamProc(SsCharMap* pCharMap) : ParamProc(pCharMap) {}

		virtual bool exec() override
		{
			const std::map<SsString, SsString>& mapParams = getParams();

			int		iFirst = -1;

//			foreach(const SsString & e, mapParams.keys()) {
			for (auto item : mapParams) {
				auto e = item.first;
				if (e == "first") {
//					iFirst = getInteger(mapParams[e]);
					iFirst = getInteger(item.second);
					break;
				}
			}

			if (iFirst < 0) {
				return	false;
			}

			for (auto item : mapParams) {
				auto e = item.first;
				//foreach(const SsString & e, mapParams.keys()) {
//				getCharMap()->getKernings().set(iFirst, e, mapParams[e]);
				getCharMap()->getKernings().set(iFirst, e, item.second);
			}

			return	true;
		}
	};

	SsCharMap::Info::Info()
	{
		clear();
	}

	SsCharMap::Info::~Info()
	{
		clear();
	}

	void SsCharMap::Info::clear()
	{
		m_strFace.clear();
		m_bUnicode = false;
		m_bSmooth = false;
	}

	bool SsCharMap::Info::set(const SsString& strName, const SsString& strValue)
	{
		// face="Hiragino Sans"
		// unicode=0
		// smooth=1
		if (strName == "face") {
			m_strFace = getString(strValue);
		}
		else
			if (strName == "unicode") {
				m_bUnicode = getBoolean(strValue);
			}
			else
				if (strName == "smooth") {
					m_bSmooth = getBoolean(strValue);
				}
				else
				{
					return	false;
				}

		return	true;
	}

	const SsString& SsCharMap::Info::getFace()
	{
		return	m_strFace;
	}

	SsCharMap::Common::Common()
	{
		clear();
	}

	SsCharMap::Common::~Common()
	{
		clear();
	}

	void SsCharMap::Common::clear()
	{
		m_iLineHeight = 0;
		m_iBase = 0;
		m_Scale = SsSizeF(0, 0);
		m_iPages = 0;
	}

	bool SsCharMap::Common::set(const SsString& strName, const SsString& strValue)
	{
		// lineHeight=71
		// base=88
		// scaleW=1024 scaleH=1024
		// pages=1
		if (strName == "lineheight") {
			m_iLineHeight = getInteger(strValue);
		}
		else
			if (strName == "base") {
				m_iBase = getInteger(strValue);
			}
			else
				if (strName == "scalew") {
					m_Scale.setWidth(getInteger(strValue));
				}
				else
					if (strName == "scaleh") {
						m_Scale.setHeight(getInteger(strValue));
					}
					else
						if (strName == "pages") {
							m_iPages = getInteger(strValue);
						}
						else
						{
							return	false;
						}

		return	true;
	}

	int SsCharMap::Common::getLineHeight()
	{
		return	m_iLineHeight;
	}

	SsCharMap::Pages::Pages()
	{
		clear();
	}

	SsCharMap::Pages::~Pages()
	{
		clear();
	}

	void SsCharMap::Pages::clear()
	{
//		foreach(SsCharMapPage * e, m_mapPage) {
/*
		for (auto item : m_mapPage) {
			auto e = item.first;

			if (e->refCount() == 1) {
				gController.RemoveObjectFromSourceTree(e);
			}
			delete e;
		}
*/
		m_mapPage.clear();
	}

	void SsCharMap::Pages::dec(SsProject* pProject)
	{
		/*
		foreach(SsCharMapPage * e, m_mapPage) {
			if (e->decRef() <= 0) {
				pProject->removeResource(e);
			}
			delete e;
		}
*/
		m_mapPage.clear();
	}

	int SsCharMap::Pages::getCount() const
	{
//		return	m_mapPage.count();
		return	m_mapPage.size();
	}

	bool SsCharMap::Pages::set(int iId, const SsString& strName, const SsString& strValue)
	{
		SsCharMapPage* pPage = nullptr;


		if (!m_mapPage.count(iId)) {
//		if (!m_mapPage.contains(iId)) {
			pPage = new SsCharMapPage();
			m_mapPage[iId] = pPage;
		}

		pPage = m_mapPage[iId];

		// file="myFont.png"
		if (strName == "file") {
			pPage->setFile(getString(strValue));
		}
		else
		{
			return	false;
		}

		return	true;
	}

#if 1
	bool SsCharMap::Pages::setDir(int iId, const SsString& dir)
	{
		SsCharMapPage* pPage = nullptr;

		if (!m_mapPage.count(iId)) {
			pPage = new SsCharMapPage();
			m_mapPage[iId] = pPage;
		}

		pPage = m_mapPage[iId];

		pPage->setDir(dir);

		return	true;
	}
#endif


	bool SsCharMap::Pages::loadImage(int iId, SsProject* pProject)
	{
		SsCharMapPage* pPage = nullptr;

//		if (!m_mapPage.contains(iId)) {
		if (!m_mapPage.count(iId)) {
				pPage = new SsCharMapPage();
			m_mapPage[iId] = pPage;
		}

		pPage = m_mapPage[iId];

		return	pPage->loadImage(pProject);
	}

#if 0
	bool SsCharMap::Pages::saveImage(int iId, const QDir& dir)
	{
		SsCharMapPage* pPage = nullptr;

		if (!m_mapPage.contains(iId)) {
			pPage = new SsCharMapPage();
			m_mapPage[iId] = pPage;
		}

		pPage = m_mapPage[iId];

		return	pPage->saveImage(dir);
	}
#endif

#if 0
	bool SsCharMap::Pages::reload()
	{
		//foreach(int e, m_mapPage.keys()) {
		for( auto item : m_mapPage ){
			SsCharMapPage* pPage = m_mapPage[item.first];
			if (pPage)
			{
				pPage->reload();
			}
		}

		return	true;
	}
#endif

	SsRawImage* SsCharMap::Pages::getImage(int iId)
	{
		SsCharMapPage* pPage = nullptr;

//		if (!m_mapPage.contains(iId)) {
		if (!m_mapPage.count(iId)) {
				pPage = new SsCharMapPage();
			m_mapPage[iId] = pPage;
		}

		pPage = m_mapPage[iId];

		return	pPage->imagePtr;
	}

#if 0	//Animeterの方へ
	SsOpenGLTexture* SsCharMap::Pages::getTexture(int iId)
	{
		SsCharMapPage* pPage = nullptr;

		if (!m_mapPage.contains(iId)) {
			pPage = new SsCharMapPage();
			m_mapPage[iId] = pPage;
		}

		pPage = m_mapPage[iId];

		return	pPage->getTexture();
	}
#endif


	SsCharMap::Chars::Elem::Elem()
	{
		clear();
	}

	SsCharMap::Chars::Elem::~Elem()
	{
		clear();
	}

	void SsCharMap::Chars::Elem::clear()
	{
		m_SrcRect = SsIRect(0, 0, 0, 0);
		m_Offset = SsPoint2(0, 0);
		m_iAdvance = 0;
		m_iPage = 0;
	}

	void SsCharMap::Chars::Elem::setSrcX(int iX)
	{
		m_SrcRect.moveLeft(iX);
	}

	void SsCharMap::Chars::Elem::setSrcY(int iY)
	{
		m_SrcRect.moveTop(iY);
	}

	void SsCharMap::Chars::Elem::setSrcWidth(int iWidth)
	{
		m_SrcRect.setWidth(iWidth);
	}

	void SsCharMap::Chars::Elem::setSrcHeight(int iHeight)
	{
		m_SrcRect.setHeight(iHeight);
	}

	void SsCharMap::Chars::Elem::setOffsetX(int iX)
	{
		m_Offset.setX(iX);
	}

	void SsCharMap::Chars::Elem::setOffsetY(int iY)
	{
		m_Offset.setY(iY);
	}

	void SsCharMap::Chars::Elem::setAdvance(int iAdvance)
	{
		m_iAdvance = iAdvance;
	}

	void SsCharMap::Chars::Elem::setPage(int iPage)
	{
		m_iPage = iPage;
	}

	const SsIRect* SsCharMap::Chars::Elem::getSrcRect()
	{
		return	&m_SrcRect;
	}

	const SsPoint2* SsCharMap::Chars::Elem::getOffset()
	{
		return	&m_Offset;
	}

	int SsCharMap::Chars::Elem::getAdvance()
	{
		return	m_iAdvance;
	}

	int SsCharMap::Chars::Elem::getPage()
	{
		return	m_iPage;
	}

	SsCharMap::Chars::Chars()
	{
		clear();
	}

	SsCharMap::Chars::~Chars()
	{
		clear();
	}

	void SsCharMap::Chars::clear()
	{
		m_Offset = SsPoint2();

//		foreach(Elem * e, m_mapElem) {
		for(auto item : m_mapElem) {
			Elem* e = item.second;
			delete e;
		}
		m_mapElem.clear();
	}

	void SsCharMap::Chars::updateOffset()
	{
		bool	bInit = true;

		m_Offset = SsPoint2();

//		foreach(Elem * e, m_mapElem) {
		for(auto item :  m_mapElem) {
			Elem* e = item.second;

				const SsPoint2* pOffset = e->getOffset();

			if (bInit || m_Offset.x > pOffset->x) {
				m_Offset.setX(pOffset->x);
			}
			if (bInit || m_Offset.y > pOffset->y) {
				m_Offset.setY(pOffset->y);
			}

			bInit = false;
		}
	}

	const SsPoint2* SsCharMap::Chars::getOffset()
	{
		return	&m_Offset;
	}

	int SsCharMap::Chars::getCount()
	{
		return	m_mapElem.size();
	}

	void SsCharMap::Chars::getKeys(std::list<SsChar>& list)
	{
		list.clear();
//		list.append(m_mapElem.keys());

		for (auto i : m_mapElem)
			list.push_back(i.first);

	}

	bool SsCharMap::Chars::set(int iId, const SsString& strName, const SsString& strValue)
	{
		Elem* pElem = nullptr;

//		if (!m_mapElem.contains(iId)) {
		if (!m_mapElem.count(iId)) {
				pElem = new Elem();
			m_mapElem[iId] = pElem;
		}

		pElem = m_mapElem[iId];

		// x=918 y=866 width=0 height=0
		// xoffset=0 yoffset=67
		// xadvance=21
		// page=0
		if (strName == "x") {
			pElem->setSrcX(getInteger(strValue));
		}
		else
			if (strName == "y") {
				pElem->setSrcY(getInteger(strValue));
			}
			else
				if (strName == "width") {
					pElem->setSrcWidth(getInteger(strValue));
				}
				else
					if (strName == "height") {
						pElem->setSrcHeight(getInteger(strValue));
					}
					else
						if (strName == "xoffset") {
							pElem->setOffsetX(getInteger(strValue));
						}
						else
							if (strName == "yoffset") {
								pElem->setOffsetY(getInteger(strValue));
							}
							else
								if (strName == "xadvance") {
									pElem->setAdvance(getInteger(strValue));
								}
								else
									if (strName == "page") {
										pElem->setPage(getInteger(strValue));
									}
									else
									{
										return	false;
									}

		return	true;
	}

	bool SsCharMap::Chars::hasKey(SsChar cChar)
	{
//		return	m_mapElem.contains(cChar);
		bool ret = m_mapElem.count(cChar);
		return	ret;
	}

	const SsIRect* SsCharMap::Chars::getSrcRect(SsChar cChar)
	{
		if (!hasKey(cChar)) {
			return	nullptr;
		}

		return	m_mapElem[cChar]->getSrcRect();
	}

	const SsPoint2* SsCharMap::Chars::getOffset(SsChar cChar)
	{
		if (!hasKey(cChar)) {
			return	nullptr;
		}

		return	m_mapElem[cChar]->getOffset();
	}

	int SsCharMap::Chars::getAdvance(SsChar cChar)
	{
		if (!hasKey(cChar)) {
			return	-1;
		}

		return	m_mapElem[cChar]->getAdvance();
	}

	int SsCharMap::Chars::getPage(SsChar cChar)
	{
		if (!hasKey(cChar)) {
			return	-1;
		}

		return	m_mapElem[cChar]->getPage();
	}

	SsCharMap::Kernings::Elem::Elem()
	{
		clear();
	}

	SsCharMap::Kernings::Elem::~Elem()
	{
		clear();
	}

	void SsCharMap::Kernings::Elem::clear()
	{
		m_cNext = '\0';
		m_iAmount = 0;
	}

	void SsCharMap::Kernings::Elem::setNext(SsChar cNext)
	{
		m_cNext = cNext;
	}

	void SsCharMap::Kernings::Elem::setAmount(int iAmount)
	{
		m_iAmount = iAmount;
	}

	SsCharMap::Kernings::Kernings()
	{
		clear();
	}

	SsCharMap::Kernings::~Kernings()
	{
		clear();
	}

	void SsCharMap::Kernings::clear()
	{
//		foreach(Elem * e, m_mapElem) {
		for(auto item : m_mapElem) {
			Elem* e = item.second;
				delete e;
		}
		m_mapElem.clear();
	}

	int SsCharMap::Kernings::getCount()
	{
		return	m_mapElem.size();
	}

	bool SsCharMap::Kernings::set(int iFirst, const SsString& strName, const SsString& strValue)
	{
		Elem* pElem = nullptr;

//		if (!m_mapElem.contains(iFirst)) {
		if (!m_mapElem.count(iFirst)) {
				pElem = new Elem();
			m_mapElem[iFirst] = pElem;
		}

		pElem = m_mapElem[iFirst];

		// second?
		// amount?
		if (strName == "second") {
			pElem->setNext(getInteger(strValue));
		}
		else
			if (strName == "amount") {
				pElem->setAmount(getInteger(strValue));
			}
			else
			{
				return	false;
			}

		return	true;
	}

#if 0
	SsString
		SsCharMap::projRelPath() const
	{
		SsProject* project = (SsProject*)parent;
		SsString p(filePath);
		// フルパスでない時のみ基準ディレクトリ＋相対ファイルパスにする。
		if (!SsPath::isAbsolute(filePath))
			p = SsPath::makeAbsolute(project->getAbsDir(SsBaseDirType::none), filePath);

		return project->makeRelPath(p, ""); //この関数SsFileObjectでよくない？

	}

	SsString SsCharMap::defaultFileExtension()
	{
		return	_D("fnt");
	}

	SsString SsCharMap::fileFilter()
	{
		return	_D("BMFont files (*.fnt)");
	}

	SsCharMap*
		SsCharMap::createFromFile(const SsString& path, const SsFileIoOption& option, OnLoadFunc onLoadFunc)
	{
		std::unique_ptr<SsCharMap> p(new SsCharMap);

		// パスは追加時のキーになるので必須
		p->filePath = path;

		// ファイルを読み込む
		if (!p->loadFromFile(p->filePath, option))
		{
			if (p->isMissing)
			{
				// プロジェクト上に赤字で表示するためファイルが存在しない場合でもオブジェクトを返す。
				return p.release();
			}
#if 1 // プロジェクト上に赤字で表示するため読み込みエラーの場合でもオブジェクトを返す。 2013.04.03 水 14:53
			p->isMissing = true;
			//p->dirty = false;
			p->setDirty_(false);

			return p.release();
#else
			return NULL;
#endif
		}

		return p.release();
	}

#endif

#if 0
	SsCharMap::SsCharMap(SsFileObject* pParent)
	{
		// ?
		dirty_ = false;
		parent = pParent;

		clear();
	}
#endif
	SsCharMap::SsCharMap()
	{
		clear();
	}


	SsCharMap::~SsCharMap()
	{
		clear();
	}

	const SsString& SsCharMap::getDir()
	{
		return	m_Dir;
	}

	SsCharMap::Info& SsCharMap::getInfo()
	{
		return	m_Info;
	}

	SsCharMap::Common& SsCharMap::getCommon()
	{
		return	m_Common;
	}

	SsCharMap::Pages& SsCharMap::getPages()
	{
		return	m_Pages;
	}

	SsCharMap::Chars& SsCharMap::getChars()
	{
		return	m_Chars;
	}

	SsCharMap::Kernings& SsCharMap::getKernings()
	{
		return	m_Kernings;
	}

	void SsCharMap::clear()
	{
		m_Dir = "";
		m_strFile.clear();

		m_Info.clear();
		m_Common.clear();
		clearPages();
		m_Chars.clear();
		m_Kernings.clear();
	}

	void SsCharMap::clearPages()
	{
		m_Pages.clear();
	}

	void SsCharMap::decPages(SsProject* pProject)
	{
		m_Pages.dec(pProject);
	}

#if 1
	bool SsCharMap::read(SsProject* proj, SsString basefilename)
	{

		SsString filename = proj->m_proj_filepath + basefilename;
		m_Dir = path2dir(filename);
		m_strFile = path2file(basefilename);

		std::fstream fs;
		fs.open(filename, std::ios::in);//read
		if (!fs.is_open())
		{
			return false;
		}

//		if (!input.open(QIODevice::ReadOnly)) {
//			return	false;
//		}

		//QByteArray					bytes;
		SsString						strLine;
		SsString						strColumn;
		SsStringList					listColumn;
		SsStringList					listParam;

		//QRegExp						regExpOneSpace;
		//QRegExp						regExpZeroSpace;

		InfoParamProc				infoParamProc(this);
		CommonParamProc				commonParamProc(this);
		PagesParamProc				pagesParamProc(this);
		CharsParamProc				charsParamProc(this);
		KerningsParamProc			kerningsParamProc(this);
		std::map<SsString, ParamProc*>	mapParamProc;
		ParamProc* pParamProc;

		mapParamProc["info"] = &infoParamProc;
		mapParamProc["common"] = &commonParamProc;
		mapParamProc["page"] = &pagesParamProc;
		mapParamProc["chars"] = &charsParamProc;
		mapParamProc["char"] = &charsParamProc;
		mapParamProc["kernings"] = &kerningsParamProc;
		mapParamProc["kerning"] = &kerningsParamProc;
		pParamProc = nullptr;

		//regExpOneSpace.setPattern("[ \\t]+");
		//regExpZeroSpace.setPattern("[ \\t]+[=,][ \\t]+");

		std::string linestr;
		while(std::getline(fs,linestr)){

		//while (!input.atEnd()) {
		//	bytes = input.readLine();

			//if (bytes.size() <= 0) {
			//	break;
			//}

			if (linestr.size() <= 0)
			{
				break;
			}

			//strLine = SsString::fromUtf8(bytes).trimmed();
			strLine = trim(linestr);

			//tab等をスペースへ 行を成型
			//strLine = strLine.replace(regExpOneSpace, " ");
			//strLine = strLine.replace(regExpZeroSpace, "");

			listColumn = splitString(strLine, ' ', '"', '"');

			for (int i = 0; i < listColumn.size(); i++) {
//				strColumn = listColumn[i].trimmed().toLower();
				strColumn = trim(listColumn[i]);
				std::transform(strColumn.cbegin(), strColumn.cend(), strColumn.begin(), tolower);

				if (mapParamProc.count(strColumn)) {
					if (pParamProc) {
						pParamProc->exec();
					}

					pParamProc = mapParamProc[strColumn];

					if (pParamProc) {
						pParamProc->init();
					}
				}
				else {
					if (pParamProc) {
						pParamProc->push(listColumn[i]);
					}
				}
			}
		}

		if (pParamProc) {
			pParamProc->exec();
		}

//		input.close();
		fs.close();

		m_Chars.updateOffset();

#if 0
		std::string ret = this->dump();
		FILE* f = fopen( (filename + ".log").c_str(), "wt");

		fprintf(f, "%s", ret.c_str());

		fclose(f);
#endif


		return	true;
	}
#endif


#if 0
	bool SsCharMap::loadFromFile(const SsString& path, const SsFileIoOption& option)
	{
		QFile	file(path.c_str());
		bool	bResult = false;

		if (file.exists()) {
			m_Dir = QFileInfo(path.c_str()).absoluteDir();
			m_strFile = file.fileName();

			bResult = read(file);
		}

		return	bResult;
	}

#ifndef SS_DISABLE_FILE_SAVE
	// 保存可能
	bool SsCharMap::saveToFile(const SsString& path, const SsFileIoOption& option, bool cleanDirty)
	{
		SsSemaphore		sem;

		if (sem.HasError()) {
			return	false;
		}

		SsString* pPath = const_cast<SsString*>(&path);
		// SsProject::saveToFileProc_ 経由だと値が渡ってくるため内容比較に変更した。
		if (path == s_defaultFilePath)
			pPath = &filePath;

		QFileInfo	info(m_strFile);
		QFileInfo	infoSrc(m_Dir, info.fileName());
		QFileInfo	infoDst(pPath->c_str());

		SsFile::copy(infoSrc.absoluteFilePath().toStdString(), infoDst.absoluteFilePath().toStdString(), false);

		QDir		dir = infoDst.absoluteDir();

		for (int i = 0; i < m_Pages.getCount(); i++) {
			m_Pages.saveImage(i, dir);
		}

		if (SsFileObject::setCleanDirtyCallbac_)
		{
			(SsFileObject::setCleanDirtyCallbac_)(this, cleanDirty);
		}

		return true;
	}
#endif

	bool
		SsCharMap::reload(const SsFileIoOption* option)
	{
		SsAutoReverter<bool> rev(&isReloading_, true);

		SsProject* project = (SsProject*)parent;

		// カレントをセルマップ基準ディレクトリにする。
		SsCurrentDirectoryRestorer cdr;
		project->setCurrentToBaseDir(SsBaseDirType::none);

		// テンポラリとしてロードする
		std::unique_ptr<SsCharMap> cm(new SsCharMap());
		cm->filePath = filePath;

		// ファイルを読み込む
		if (!cm->loadFromFile(filePath, *option))
		{
			// 失敗
			// TODO? dialog
			return false;
		}

		return true;
	}
#endif

	//add 7.1 SDK 
	const std::list<SsCharMapPage*> SsCharMap::getCharMapPages() 
	{
		std::list<SsCharMapPage*> outlist;

		for (auto& i : m_Pages.getDic())
		{
			outlist.push_back( i.second );
		}

		return outlist;
	}

	#include <string>
	#include <cstdio>
	#include <vector>
	template <typename ... Args>
	std::string format(const std::string& fmt, Args ... args)
	{
		size_t len = std::snprintf(nullptr, 0, fmt.c_str(), args ...);
		std::vector<char> buf(len + 1);
		std::snprintf(&buf[0], len + 1, fmt.c_str(), args ...);
		return std::string(&buf[0], &buf[0] + len);
	}


	std::string SsCharMapPage::dump()
	{
		std::string ret = "";

		ret += format("SsCharMapPage \n");
		ret += format( "dir = %s \n" ,_dir.c_str() );


		ret +=("%s", _dir.c_str());

		return ret;
	}


	std::string SsCharMap::Info::dump()
	{
		std::string ret = "";

		ret += format("SsCharMap::Info  \n");
		ret += format("Face = %s unicode = %d smooth =%d  \n", m_strFace.c_str(), m_bUnicode , (m_bSmooth ? 0: 1) );

		return ret;
	}

	std::string SsCharMap::Common::dump()
	{
		std::string ret = "";

		ret += format("SsCharMap::Common  \n");
		ret += format("%d %d (%f,%f) %d  \n", m_iLineHeight , m_iBase, m_Scale.width(), m_Scale.height(), m_iPages);

		return ret;
	}

	std::string SsCharMap::Pages::dump()
	{
		std::string ret = "";

		ret += format("SsCharMap::Pages  \n");

		for (auto item : m_mapPage)
		{
			int id = item.first;
			ret += format("id = %d  \n", id);
			ret += item.second->dump();
		}

		return ret;
	}

	std::string SsCharMap::Chars::dump()
	{
		std::string ret = "";

		ret += format("SsCharMap::Chars  \n");
		ret += format("(%f %f)  \n" , m_Offset.x , m_Offset.y );

		for (auto item : m_mapElem)
		{
			SsChar code = item.first;
			ret += format("code = %d \n", code);
			ret += item.second->dump();
		}

		return ret;
	}

	std::string SsCharMap::Chars::Elem::dump()
	{
		std::string ret = "";

		ret += format("SsCharMap::Chars::Elem \n");
//		ret += format("xy(%d,%d) wh(%d,%d) \n" , m_SrcRect.x , m_SrcRect.y , m_SrcRect.w , m_SrcRect.h);
		ret += format("xy(%d,%d) wh(%d,%d) \n", m_SrcRect.x(), m_SrcRect.y(), m_SrcRect.width(), m_SrcRect.height());

		return ret;
	}

	std::string SsCharMap::Kernings::dump()
	{
		std::string ret = "";

		ret += format("SsCharMap::Kernings \n");
		for (auto item : m_mapElem)
		{
			SsChar c = item.first;

			ret += format("Char = %d \n", c);
			ret += item.second->dump();
		}
		return ret;
	}

	std::string SsCharMap::Kernings::Elem::dump()
	{
		std::string ret = "";

		ret += format("SsCharMap::Kernings::Elem \n");
		ret += format("Next = %d Amount = %d \n", m_cNext, m_iAmount);

		return ret;
	}


	std::string SsCharMap::dump()
	{
		std::string ret = "";

		ret +=("SsCharMap \n");
		ret +=("m_Dir = %s \n",m_Dir.c_str());
		ret +=("m_strFile = %s \n", m_strFile.c_str());

		ret +=m_Info.dump();
		ret += m_Common.dump();
		ret += m_Pages.dump();
		ret += m_Chars.dump();
		ret += m_Kernings.dump();


		return ret;
	}


	bool SsCharMapPage::loadImage(SsProject* proj) {
		return true;
	}

}	// namespace SpriteStudio
