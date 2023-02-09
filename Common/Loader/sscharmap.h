#ifndef __SSCHARMAP__
#define __SSCHARMAP__

#include "sstypes.h"
#include "../Helper/SsTextureFactory.h"


#define DEBUG_OUT

#include <map>
#include <list>
#include <vector>

namespace SpriteStudio
{
	class SsProject;
	class SsRawImage;

	typedef uint16_t SsChar ;
//	typedef SsString SsChar;
	//typedef std::basic_string<SsChar>  SsString;


	class SsCharMapPage	//ただのイメージファイル情報とした。別途FontTextureManagerで読み込みを行う
	{
	private:
		SsString _dir;
		SsString _file;

	public:
		SsCharMapPage() : imagePtr(0) {}
		virtual ~SsCharMapPage() {}

		void setFile(SsString& str) { _file = str; }
		void setDir(const SsString& str) { _dir = str; }

		bool loadImage(SsProject* proj);

		SsString getFilenamePath() {
			return _dir + _file;
		}

		std::string dump();

		//SSTextureLoader::DataHandle imagePtr;
		SsRawImage* imagePtr;
	};


	class SsCharMap
	{
	private:
		class Info
		{
		public:
			Info();
			~Info();

			void clear();

			bool set(const SsString& strName, const SsString& strValue);

			const SsString& getFace();

			std::string dump();

		private:
			SsString	m_strFace;
			bool		m_bUnicode;
			bool		m_bSmooth;
		};

		class Common
		{
		public:
			Common();
			~Common();

			void clear();

			bool set(const SsString& strName, const SsString& strValue);

			int getLineHeight();

			std::string dump();

		private:
			int			m_iLineHeight;
			int			m_iBase;
			SsSizeF		m_Scale;
			int			m_iPages;
		};

		class Pages
		{
		public:
			Pages();
			~Pages();

			void clear();
			void dec(SsProject* pProject);

			int getCount() const;

			bool set(int iId, const SsString& strName, const SsString& strValue);
			bool setDir(int iId, const SsString& dir);

			bool loadImage(int iId, SsProject* pProject);
			//bool saveImage(int iId, const QDir& dir);
			//bool reload();

			SsRawImage* getImage(int iId);
			//SsOpenGLTexture* getTexture(int iId);

			std::string dump();

			const std::map<int, SsCharMapPage*> getDic() 
			{
				return m_mapPage;
			}

		private:
			std::map<int, SsCharMapPage*>	m_mapPage;
		};

		class Chars
		{
		public:
			Chars();
			~Chars();

			void clear();

			void updateOffset();


			const SsPoint2* getOffset();

			int getCount();
			void getKeys(std::list<SsChar>& list);

			bool set(int iId, const SsString& strName, const SsString& strValue);

			bool hasKey(SsChar cChar);
			const SsIRect* getSrcRect(SsChar cChar);
			const SsPoint2* getOffset(SsChar cChar);
			int getAdvance(SsChar cChar);
			int getPage(SsChar cChar);

			std::string dump();

		private:
			class Elem
			{
			public:
				Elem();
				~Elem();

				void clear();

				void setSrcX(int iX);
				void setSrcY(int iY);
				void setSrcWidth(int iWidth);
				void setSrcHeight(int iHeight);
				void setOffsetX(int iX);
				void setOffsetY(int iY);
				void setAdvance(int iAdvance);
				void setPage(int iPage);

				const SsIRect* getSrcRect();
				const SsPoint2* getOffset();
				int getAdvance();
				int getPage();

				std::string dump();

			private:
				SsIRect		m_SrcRect;
				SsPoint2		m_Offset;
				int			m_iAdvance;
				int			m_iPage;
			};

			SsPoint2				m_Offset;

			//<utf16 code , data>
			std::map<SsChar, Elem*>	m_mapElem;
		};

		class Kernings
		{
		public:
			Kernings();
			~Kernings();

			void clear();

			int getCount();

			bool set(int iFirst, const SsString& strName, const SsString& strValue);
			std::string dump();

		private:
			class Elem
			{
			public:
				Elem();
				~Elem();

				void clear();

				void setNext(SsChar cNext);
				void setAmount(int iAmount);

				std::string dump();

			private:
				SsChar		m_cNext;
				int			m_iAmount;
			};

			std::map<SsChar, Elem*>	m_mapElem;
		};

	public:
		///typedef std::list<SsCharMapPage*>				CharMapPageList;
		typedef std::map<SsString, SsCharMapPage*>		CharMapPageDic;
		typedef std::pair<SsString, SsCharMapPage*>	CharMapPageDicItem;


		//typedef	bool (*OnLoadFunc)(SsXmlIArchive& arc, const unsigned int version, bool* oContinue);
		//SsString	projRelPath() const;
		//static SsString defaultFileExtension();
		//static SsString fileFilter();

		/// ファイルを読み込んで生成する
		//static SsCharMap* createFromFile(const SsString& path, const SsFileIoOption& option = s_defaultFileIoOption, OnLoadFunc = NULL);

		//SsCharMap(SsFileObject* pParent = nullptr);
		SsCharMap();

		~SsCharMap();

		const SsString& getDir();

		Info& getInfo();
		Common& getCommon();
		Pages& getPages();
		Chars& getChars();
		Kernings& getKernings();

		void clear();
		void clearPages();
		void decPages(SsProject* pProject);

		bool read( SsProject* proj , SsString filename );


		const std::list<SsCharMapPage*> getCharMapPages() ;
		std::string dump();

#if 0
		bool read(QIODevice& input);

		/**
			ファイルを読み込み、各種補正処理、参照ページの読み込みも行う。
		*/
		bool	loadFromFile(const SsString& path = s_defaultFilePath, const SsFileIoOption& option = s_defaultFileIoOption) override;


		/// リロードする。
		bool	reload(const SsFileIoOption* option = NULL) override;
#endif


	private:
		SsString		m_Dir;
		SsString		m_strFile;

		Info		m_Info;
		Common		m_Common;
		Pages		m_Pages;
		Chars		m_Chars;
		Kernings	m_Kernings;
	};

	typedef std::pair<SsString, SsCharMap*>	SsCharMapDicItem;



}	// namespace SpriteStudio
#endif

