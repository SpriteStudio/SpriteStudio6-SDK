#include "ssopenglninesdrawer.h"

#include "ssOpenGLSetting.h"



namespace SpriteStudio
{
	class QuadLeap
	{
	public:
		class Rate
		{
		public:
			Rate()
			{
				set(0.0f, 0.0f, 0.0f, 0.0f);
			}

			~Rate()
			{
			}

			void set(float fLT, float fRT, float fLB, float fRB)
			{
				m_fLT = fLT;
				m_fRT = fRT;
				m_fLB = fLB;
				m_fRB = fRB;
			}

			float getLT() const
			{
				return	m_fLT;
			}

			float getRT() const
			{
				return	m_fRT;
			}

			float getLB() const
			{
				return	m_fLB;
			}

			float getRB() const
			{
				return	m_fRB;
			}

		private:
			float		m_fLT;
			float		m_fRT;
			float		m_fLB;
			float		m_fRB;
		};

		QuadLeap()
		{
			set(-0.5f, 0.5f, 0.5f, -0.5);
		}

		~QuadLeap()
		{
		}

		void set(float fL, float fR, float fT, float fB)
		{
			m_fL = fL;
			m_fR = fR;
			m_fT = fT;
			m_fB = fB;
		}

		void updateRate(const SsPoint2& point)
		{
			float	fL = 0.0f;
			float	fR = 0.0f;
			float	fT = 0.0f;
			float	fB = 0.0f;
			float	fH = 0.0f;
			float	fV = 0.0f;
			float	fX = 0.0f;
			float	fY = 0.0f;

			if (m_fL < m_fR) {
				fL = m_fL;
				fR = m_fR;
			}
			else {
				fL = m_fR;
				fR = m_fL;
			}

			if (m_fT < m_fB) {
				fT = m_fT;
				fB = m_fB;
			}
			else {
				fT = m_fB;
				fB = m_fT;
			}

			fH = fR - fL;
			fV = fB - fT;

			fX = (point.x - fL) / fH;
			fY = (point.y - fT) / fV;

			if (m_fL < m_fR) {
				fX = 1.0f - fX;
			}

			if (m_fT < m_fB) {
				fY = 1.0f - fY;
			}

			m_Rate.set(
				fX * fY,
				(1.0f - fX) * fY,
				fX * (1.0f - fY),
				(1.0f - fX) * (1.0f - fY)
			);
		}

		template<typename T>
		T getLeapValue(const T& vLT, const T& vRT, const T& vLB, const T& vRB)
		{
			return	(vLT * m_Rate.getLT()) + (vRT * m_Rate.getRT()) + (vLB * m_Rate.getLB()) + (vRB * m_Rate.getRB());
		}

	private:
		float		m_fL;
		float		m_fR;
		float		m_fT;
		float		m_fB;

		Rate		m_Rate;
	};

	SsOpenGLNinesDrawer::SsOpenGLNinesDrawer()
	{
	}

	bool SsOpenGLNinesDrawer::execute(const SsNines& nines, const float* vertices, const float* colors)
	{
		const SsPolygon& polygon = nines.getPolygon();
		int						iVertexCount = polygon.getVertexCount();
		int						iVertexIndexCount = polygon.getVertexIndexCount();

		if (iVertexCount >= 3) {
			const std::list<SsVertex>& listVertex = polygon.getVertices();
			const std::list<uint32_t>& listVertexIndex = polygon.getVertexIndices();
			const std::list<uint32_t>& listStride = polygon.getStrides();

			int			iVertexSize = sizeof(float) * 3;
			int			iCoordSize = sizeof(float) * 2;
			int			iColorSize = sizeof(float) * 4;
			int			iIndexSize = sizeof(uint16_t) * 1;
			float* pVertices = (float*)malloc(iVertexSize * iVertexCount);
			float* pCoords = (float*)malloc(iCoordSize * iVertexCount);
			float* pColors = (float*)malloc(iColorSize * iVertexCount);
			//ushort* pIndices = (ushort*)malloc(iIndexSize * iVertexIndexCount);
			uint16_t* pIndices = (uint16_t*)malloc(iIndexSize * iVertexIndexCount);
			

			float* pVertexSeek = pVertices;
			float* pCoordSeek = pCoords;
			float* pColorSeek = pColors;
			uint16_t* pIndexSeek = pIndices;
			int			iSeek = 0;

			QuadLeap	quadLeap;
			SsVector4	cLT(colors[0 * 4 + 0], colors[0 * 4 + 1], colors[0 * 4 + 2], colors[0 * 4 + 3]);
			SsVector4	cRT(colors[1 * 4 + 0], colors[1 * 4 + 1], colors[1 * 4 + 2], colors[1 * 4 + 3]);
			SsVector4	cLB(colors[2 * 4 + 0], colors[2 * 4 + 1], colors[2 * 4 + 2], colors[2 * 4 + 3]);
			SsVector4	cRB(colors[3 * 4 + 0], colors[3 * 4 + 1], colors[3 * 4 + 2], colors[3 * 4 + 3]);
			SsVector4	cCC;
			SsSizeF		size = nines.getSize();
			float		fX2 = size.width() * 0.5f;
			float		fY2 = size.height() * 0.5f;

			quadLeap.set(-fX2, fX2, fY2, -fY2);

			for(const SsVertex & e : listVertex) {
				//foreach(const SsVertex & e, listVertex) {
				const SsPoint2& position = e.getPosition();
				const SsPoint2& coord = e.getCoord();

				quadLeap.updateRate(position);

				cCC = quadLeap.getLeapValue(cLT, cRT, cLB, cRB);

				*pVertexSeek++ = position.x;
				*pVertexSeek++ = position.y;
				*pVertexSeek++ = 0.0f;

				*pCoordSeek++ = coord.x;
				*pCoordSeek++ = coord.y;

				*pColorSeek++ = cCC.x;
				*pColorSeek++ = cCC.y;
				*pColorSeek++ = cCC.z;
				*pColorSeek++ = cCC.w;

			}

			for(const uint32_t& e: listVertexIndex) {
			//	foreach(const quint32 & e, listVertexIndex) {
				*pIndexSeek++ = e;
			}

			glVertexPointer(3, GL_FLOAT, 0, (GLvoid*)pVertices);
			glTexCoordPointer(2, GL_FLOAT, 0, (GLvoid*)pCoords);
			glColorPointer(4, GL_FLOAT, 0, (GLvoid*)pColors);

			iSeek = 0;
			//foreach(const quint32 & e, listStride) {
			for( auto & e : listStride) {
				glDrawElements(GL_TRIANGLE_STRIP, e, GL_UNSIGNED_SHORT, (GLvoid*)&pIndices[iSeek]);
				iSeek += e;
			}

			free(pVertices);
			free(pCoords);
			free(pColors);
			free(pIndices);
		}

		return	true;
	}
}