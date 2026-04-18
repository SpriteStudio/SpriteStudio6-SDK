#include "ssplayer_mesh_v2.h"
#include <cmath>
#include <cstring>
#include <algorithm>

// SDK本体への依存は SsMeshPartV2 のブリッジ部分にのみ残す
#include "ssplayer_matrix.h"
#include "../Loader/ssloader.h"
#include "ssplayer_animedecode.h"

namespace spritestudio6
{

namespace mesh_v2 {

// 【移植用：独立数学ロジック】
bool MeshProcessor::InverseMatrix(const Matrix44& src, Matrix44& dst)
{
    const float* m = src.m;
    float inv[16];
    float det;
    inv[0] = m[5] * m[10] * m[15] - m[5] * m[11] * m[14] - m[9] * m[6] * m[15] + m[9] * m[7] * m[14] + m[13] * m[6] * m[11] - m[13] * m[7] * m[10];
    inv[4] = -m[4] * m[10] * m[15] + m[4] * m[11] * m[14] + m[8] * m[6] * m[15] - m[8] * m[7] * m[14] - m[12] * m[6] * m[11] + m[12] * m[7] * m[10];
    inv[8] = m[4] * m[9] * m[15] - m[4] * m[11] * m[13] - m[8] * m[5] * m[15] + m[8] * m[7] * m[13] + m[12] * m[5] * m[11] - m[12] * m[7] * m[9];
    inv[12] = -m[4] * m[9] * m[14] + m[4] * m[10] * m[13] + m[8] * m[5] * m[14] - m[8] * m[6] * m[13] - m[12] * m[5] * m[10] + m[12] * m[6] * m[9];
    det = m[0] * inv[0] + m[1] * inv[4] + m[2] * inv[8] + m[3] * inv[12];
    if (det == 0) return false;
    det = 1.0f / det;
    inv[1] = -m[1] * m[10] * m[15] + m[1] * m[11] * m[14] + m[9] * m[2] * m[15] - m[9] * m[3] * m[14] - m[13] * m[2] * m[11] + m[13] * m[3] * m[10];
    inv[5] = m[0] * m[10] * m[15] - m[0] * m[11] * m[14] - m[8] * m[2] * m[15] + m[8] * m[3] * m[14] + m[12] * m[2] * m[11] - m[12] * m[3] * m[10];
    inv[9] = -m[0] * m[9] * m[15] + m[0] * m[11] * m[13] + m[8] * m[1] * m[15] - m[8] * m[3] * m[13] - m[12] * m[1] * m[11] + m[12] * m[3] * m[9];
    inv[13] = m[0] * m[9] * m[14] - m[0] * m[10] * m[13] - m[8] * m[1] * m[14] + m[8] * m[2] * m[13] + m[12] * m[1] * m[10] - m[12] * m[2] * m[9];
    inv[2] = m[1] * m[6] * m[15] - m[1] * m[7] * m[14] - m[5] * m[2] * m[15] + m[5] * m[3] * m[14] + m[13] * m[2] * m[7] - m[13] * m[3] * m[6];
    inv[6] = -m[0] * m[6] * m[15] + m[0] * m[7] * m[14] + m[4] * m[2] * m[15] - m[4] * m[3] * m[14] - m[12] * m[2] * m[7] + m[12] * m[3] * m[6];
    inv[10] = m[0] * m[5] * m[15] - m[0] * m[7] * m[13] - m[4] * m[1] * m[15] + m[4] * m[3] * m[13] + m[12] * m[1] * m[7] - m[12] * m[3] * m[5];
    inv[14] = -m[0] * m[5] * m[14] + m[0] * m[6] * m[13] + m[4] * m[1] * m[14] - m[4] * m[2] * m[13] - m[12] * m[1] * m[6] + m[12] * m[2] * m[5];
    inv[3] = -m[1] * m[6] * m[11] + m[1] * m[7] * m[10] + m[5] * m[2] * m[11] - m[5] * m[3] * m[10] - m[9] * m[2] * m[7] + m[9] * m[3] * m[6];
    inv[7] = m[0] * m[6] * m[11] - m[0] * m[7] * m[10] - m[4] * m[2] * m[11] + m[4] * m[3] * m[10] + m[8] * m[2] * m[7] - m[8] * m[3] * m[6];
    inv[11] = -m[0] * m[5] * m[11] + m[0] * m[7] * m[9] + m[4] * m[1] * m[11] - m[4] * m[3] * m[9] - m[8] * m[1] * m[7] + m[8] * m[3] * m[5];
    inv[15] = m[0] * m[5] * m[10] - m[0] * m[6] * m[9] - m[4] * m[1] * m[10] + m[4] * m[2] * m[9] + m[8] * m[1] * m[6] - m[8] * m[2] * m[5];
    for (int i = 0; i < 16; i++) dst.m[i] = inv[i] * det;
    return true;
}

// 【移植用：独立計算エンジン】
// すべてポインタ・スキャン形式で記述し、特定のコンテナクラスへの依存を完全に断つ
void MeshProcessor::compute(
    int vertexCount,
    const float* baseVertices,
    const int* unboundIndices, int unboundCount,
    const int* boundIndices, int boundCount,
    const int* bindCounts,
    const int* boneIndices,
    const float* weights,
    const Vector3* bindOffsets,
    const Matrix44* activeBoneMatrices,
    const Vector3* localDeformOffsets,
    bool isDeform,
    const Matrix44& partMatrix,
    const Matrix44& invPartMatrix,
    float* outVertices
)
{
    const float* pm = partMatrix.m;
    const float* inv = invPartMatrix.m;

    // Unbound: バインドなし頂点の計算
    for (int i = 0; i < unboundCount; i++)
    {
        int idx = unboundIndices[i];
        float lx = baseVertices[idx * 3 + 0];
        float ly = baseVertices[idx * 3 + 1];

        if (isDeform) { 
            lx += localDeformOffsets[idx].x; 
            ly += localDeformOffsets[idx].y; 
        }

        outVertices[idx * 3 + 0] = lx * pm[0] + ly * pm[4] + pm[12];
        outVertices[idx * 3 + 1] = lx * pm[1] + ly * pm[5] + pm[13];
        outVertices[idx * 3 + 2] = 0.0f;
    }

    // Bound: バインドあり頂点（スキニング）の計算
    for (int i = 0; i < boundCount; i++)
    {
        int idx = boundIndices[i];
        float outX = 0.0f;
        float outY = 0.0f;
        
        int binds = bindCounts[i];
        int dataOffset = i * MAX_BINDS;
        const int* bIndices = &boneIndices[dataOffset];
        const float* bWeights = &weights[dataOffset];
        const Vector3* bOffsets = &bindOffsets[dataOffset];

        for (int n = 0; n < binds; n++)
        {
            const float* bm = activeBoneMatrices[bIndices[n]].m;
            const Vector3& offset = bOffsets[n];
            float w = bWeights[n];

            float tx = offset.x * bm[0] + offset.y * bm[4] + offset.z * bm[8] + bm[12];
            float ty = offset.x * bm[1] + offset.y * bm[5] + offset.z * bm[9] + bm[13];

            outX += tx * w;
            outY += ty * w;
        }

        if (isDeform)
        {
            float lx = outX * inv[0] + outY * inv[4] + inv[12] + localDeformOffsets[idx].x;
            float ly = outX * inv[1] + outY * inv[5] + inv[13] + localDeformOffsets[idx].y;
            float lz = outX * inv[2] + outY * inv[6] + inv[14];

            outX = lx * pm[0] + ly * pm[4] + lz * pm[8] + pm[12];
            outY = lx * pm[1] + ly * pm[5] + lz * pm[9] + pm[13];
        }

        outVertices[idx * 3 + 0] = outX;
        outVertices[idx * 3 + 1] = outY;
        outVertices[idx * 3 + 2] = 0.0f;
    }
}

} // namespace mesh_v2

// --- SsMeshPartV2 (SDKブリッジ部分) ---

void SsMeshPartV2::setupFromCell(SsCell* cell, ISSTexture* tex)
{
    if (cell == nullptr || tex == nullptr) return;
    targetTexture = tex;
    size_t psize = cell->meshPointList.size();
    baseVertices.assign(psize * 3, 0.0f);
    uvs.assign(psize * 2, 0.0f);
    localDeformOffsets.assign(psize, mesh_v2::Vector3{0.0f, 0.0f, 0.0f, 0.0f});
    
    _cachedDrawData.vertices.assign(psize * 3, 0.0f);
    _cachedDrawData.uvs.assign(psize * 2, 0.0f);
    _cachedDrawData.colors.assign(psize * 4, 1.0f);
    
    SsVector2 offs;
    offs.x = (-cell->size.x / 2.0f) - (cell->pivot.x * cell->size.x);
    offs.y = (cell->size.y / 2.0f) - (cell->pivot.y * cell->size.y);

    float txsizew = static_cast<float>(tex->getWidth());
    float txsizeh = static_cast<float>(tex->getHeight());
    float uvpixel_x = tex->isPow2() ? (1.0f / txsizew) : 1.0f;
    float uvpixel_y = tex->isPow2() ? (1.0f / txsizeh) : 1.0f;

    skinningData.unboundVertexIndices.clear();
    skinningData.boundVertexIndices.clear();
    for (size_t i = 0; i < psize; i++)
    {
        SsVector2& v = cell->meshPointList[i];
        baseVertices[i * 3 + 0] = v.x + offs.x;
        baseVertices[i * 3 + 1] = -v.y + offs.y;
        _cachedDrawData.uvs[i * 2 + 0] = (cell->pos.x + v.x) * uvpixel_x;
        _cachedDrawData.uvs[i * 2 + 1] = (cell->pos.y + v.y) * uvpixel_y;
        skinningData.unboundVertexIndices.push_back(static_cast<int>(i));
    }

    size_t triSize = cell->meshTriList.size();
    _cachedDrawData.indices.resize(triSize * 3);
    for (size_t i = 0; i < triSize; i++)
    {
        SsTriangle& t = cell->meshTriList[i];
        _cachedDrawData.indices[i * 3 + 0] = t.idxPo1;
        _cachedDrawData.indices[i * 3 + 1] = t.idxPo2;
        _cachedDrawData.indices[i * 3 + 2] = t.idxPo3;
    }
}

void SsMeshPartV2::update()
{
    if (myPartState == nullptr) return;

    mesh_v2::Matrix44 partMatrix, invPartMatrix;
    std::memcpy(partMatrix.m, myPartState->matrix, sizeof(float) * 16);
    
    bool isDeform = myPartState->is_defrom;
    if (isDeform) {
        mesh_v2::MeshProcessor::InverseMatrix(partMatrix, invPartMatrix);
        int vnum = static_cast<int>(baseVertices.size() / 3);
        if (localDeformOffsets.size() != static_cast<size_t>(vnum)) {
            localDeformOffsets.resize(vnum, mesh_v2::Vector3{0.0f, 0.0f, 0.0f, 0.0f});
        }
        for (int i = 0; i < vnum; i++) {
            localDeformOffsets[i].x = myPartState->deformValue.verticeChgList[i].x;
            localDeformOffsets[i].y = myPartState->deformValue.verticeChgList[i].y;
            localDeformOffsets[i].z = 0.0f;
        }
    }

    mesh_v2::MeshProcessor::compute(
        static_cast<int>(baseVertices.size() / 3),
        baseVertices.data(),
        skinningData.unboundVertexIndices.data(), static_cast<int>(skinningData.unboundVertexIndices.size()),
        skinningData.boundVertexIndices.data(), static_cast<int>(skinningData.boundVertexIndices.size()),
        skinningData.bindCounts.data(),
        skinningData.boneIndices.data(),
        skinningData.weights.data(),
        skinningData.offsets.data(),
        activeBoneMatrices.data(),
        localDeformOffsets.data(),
        isDeform,
        partMatrix,
        invPartMatrix,
        _cachedDrawData.vertices.data()
    );

    _cachedDrawData.blendType = static_cast<int>(myPartState->alphaBlendType);
    _cachedDrawData.textureHandle = 0;
}

void SsMeshPartV2::draw(float alpha)
{
    if (rendererDelegate)
    {
        for(size_t i=0; i < _cachedDrawData.colors.size() / 4; ++i) {
            _cachedDrawData.colors[i * 4 + 3] = alpha;
        }
        rendererDelegate->renderMesh(_cachedDrawData);
    }
}

} // namespace spritestudio6