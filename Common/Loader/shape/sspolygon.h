#ifndef SSPOLYGON_H
#define SSPOLYGON_H

#include "../sstypes.h"
#include "ssvertex.h"
#include <list>


namespace SpriteStudio
{

	/*!
	 * \class	SsPolygon
	 * \brief	ポリゴン
	*/
	class SsPolygon
	{
	public:
		/*!
		 * \enum	Polygon::VertexOrder
		 * \brief	頂点順
		*/
		enum VertexOrder
		{
			VO_Unknown = 0,	//!< 不明
			VO_TriangleList,			//!< 三角リスト
			VO_TriangleFan,				//!< 三角ファン
			VO_TriangleStrip,			//!< 三角ストリップ
		};

		//! コンストラクタ
		SsPolygon();

		//! デストラクタ
		~SsPolygon();

		//! クリア
		void clear();

		//! 頂点順を設定
		/*!
		 * \param	eVertexOrder	頂点順
		*/
		void setVertexOrder(VertexOrder eVertexOrder);

		//! 頂点順を取得
		/*!
		 * \return	頂点順
		*/
		VertexOrder getVertexOrder() const;

		//! 頂点を追加
		/*!
		 * \param	vertex	頂点
		*/
		void addVertex(const SsVertex& vertex);

		//! 頂点インデックスを追加
		/*!
		 * \param	vertexIndex	頂点インデックス
		*/
		void addVertexIndex(uint32_t vertexIndex);

		//! 区切りを追加
		/*!
		 * \param	stride	区切り
		*/
		void addStride(uint32_t stride);

		//! 頂点数を取得
		/*!
		 * \return	頂点数
		*/
		int getVertexCount() const;

		//! 頂点インデックス数を取得
		/*!
		 * \return	頂点インデックス数
		*/
		int getVertexIndexCount() const;

		//! 区切り数を取得
		/*!
		 * \return	区切り数
		*/
		int getStrideCount() const;

		//! 頂点リストを取得
		/*!
		 * \return	頂点リスト
		*/
		const std::list<SsVertex>& getVertices() const;

		//! 頂点インデックスリストを取得
		/*!
		 * \return	頂点インデックスリスト
		*/
		const std::list<uint32_t>& getVertexIndices() const;

		//! 区切りリストを取得
		/*!
		 * \return	区切りリスト
		*/
		const std::list<uint32_t>& getStrides() const;

	private:
		VertexOrder		m_eVertexOrder;			//!< 頂点順
		std::list<SsVertex>	m_listVertex;			//!< 頂点リスト
		std::list<uint32_t>	m_listVertexIndex;		//!< 頂点インデックスリスト
		std::list<uint32_t>	m_listStride;			//!< 区切りリスト
	};

};

#endif // SSPOLYGON_H
