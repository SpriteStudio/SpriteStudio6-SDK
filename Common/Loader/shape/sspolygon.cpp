#include "sspolygon.h"

namespace SpriteStudio
{


	//! コンストラクタ
	SsPolygon::SsPolygon()
	{
		clear();
	}

	//! デストラクタ
	SsPolygon::~SsPolygon()
	{
		clear();
	}

	//! クリア
	void SsPolygon::clear()
	{
		m_eVertexOrder = VO_Unknown;
		m_listVertex.clear();
		m_listVertexIndex.clear();
		m_listStride.clear();
	}

	//! 頂点順を設定
	void SsPolygon::setVertexOrder(VertexOrder eVertexOrder)
	{
		m_eVertexOrder = eVertexOrder;
	}

	//! 頂点順を取得
	SsPolygon::VertexOrder SsPolygon::getVertexOrder() const
	{
		return	m_eVertexOrder;
	}

	//! 頂点を追加
	void SsPolygon::addVertex(const SsVertex& vertex)
	{
//		m_listVertex.append(vertex);
		m_listVertex.push_back(vertex);
	}

	//! 頂点インデックスを追加
	void SsPolygon::addVertexIndex(uint32_t vertexIndex)
	{
		m_listVertexIndex.push_back(vertexIndex);
	}

	//! 区切りを追加
	void SsPolygon::addStride(uint32_t stride)
	{
		m_listStride.push_back(stride);
	}

	//! 頂点数を取得
	int SsPolygon::getVertexCount() const
	{
		return	m_listVertex.size();
	}

	//! 頂点インデックス数を取得
	int SsPolygon::getVertexIndexCount() const
	{
		return	m_listVertexIndex.size();
	}

	//! 区切り数を取得
	int SsPolygon::getStrideCount() const
	{
		return	m_listStride.size();
	}

	//! 頂点リストを取得
	const std::list<SsVertex>& SsPolygon::getVertices() const
	{
		return	m_listVertex;
	}

	//! 頂点インデックスリストを取得
	const std::list<uint32_t>& SsPolygon::getVertexIndices() const
	{
		return	m_listVertexIndex;
	}

	//! 区切りリストを取得
	const std::list<uint32_t>& SsPolygon::getStrides() const
	{
		return	m_listStride;
	}

};