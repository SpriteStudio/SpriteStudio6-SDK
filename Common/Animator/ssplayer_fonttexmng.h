#ifndef __SSPLAYER_FONTMNG__
#define __SSPLAYER_FONTMNG__

#include "../Helper/SsTextureFactory.h"
#include "font/ssfonttexture.h"
#include "sstypes.h"
#include <map>


namespace SpriteStudio
{
	class SsProject;

	class FontTextureManager
	{
	private :
		class ShareTextureDesc
		{
		public:
			SsString text;
			SsFontDesc desc;
			SsSize sizeGrid;
			SsSize sizeFixed;
			SsRawImage* image;
			SsFontTexture* fontTexture;

			bool compare(const SsString text,
				const SsFontDesc& desc,
				const SsSize& sizeGrid,
				const SsSize& sizeFixed
			) {
				return 
					(text == this->text &&
					desc == this->desc &&
					sizeGrid == this->sizeGrid &&
					sizeFixed == this->sizeFixed
					);
			}

			void release()
			{
				delete fontTexture;
				fontTexture = 0;
				delete image;
				image = 0;
			}
		};

	public:
		struct CharaMapImage {
			int width;
			int height;
			int bpp;
			SsRawImage* imgptr;
		};

	private:
		std::map<SsString, CharaMapImage> m_charamap_images;
		std::vector<ShareTextureDesc> m_part_textures;

	public:

		std::pair<SsRawImage*, SsFontTexture*> findShareTexture(const SsString& text, const SsFontDesc& desc, const SsSize& sizeGrid, const SsSize& sizeFixed);

		FontTextureManager();
		FontTextureManager(SsProject* myproject);

		virtual ~FontTextureManager();

		//SsProjectを起点としてフォントファイルをロードします。
		bool loadTextureFromProject(SsProject* pj);

		void releaseRawImageAll();
		void releasePartsTextureAll();


	};


};

#endif

