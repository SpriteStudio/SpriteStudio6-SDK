#ifndef __SSPLAYER_MESH_V2__
#define __SSPLAYER_MESH_V2__

#include <cstdint>
#include <cstddef>
#include <vector>

namespace spritestudio6
{
namespace mesh_v2 {

// --------------------------------------------------------------------------
// SS7-SDK への移植性を考慮した独立数学型
// --------------------------------------------------------------------------
#if defined(_MSC_VER)
#define SS_ALIGN(x) __declspec(align(x))
#else
#define SS_ALIGN(x) __attribute__((aligned(x)))
#endif

struct Vector2 { float x, y; };
struct SS_ALIGN(16) Vector3 { float x, y, z, w; };
struct Color { float r, g, b, a; };
struct SS_ALIGN(16) Matrix44 { float m[16]; };

// 1頂点あたりの最大ボーンバインド数
static constexpr int MAX_BINDS = 8;

// --------------------------------------------------------------------------
// Mesh Processor (計算式・ロジック本体)
// --------------------------------------------------------------------------
class MeshProcessor {
public:
    // 【移植用：純粋計算インターフェース】
    // SDK固有のコンテナ(std::vector等)を使わず、ポインタでデータを受け渡す。
    // これにより、SS7側のデータ構造が何であっても呼び出しが可能。
    static void compute(
        int vertexCount,
        const float* baseVertices,              // float x 3 * vertexCount
        const int* unboundIndices,              // バインドなし頂点のインデックス配列
        int unboundCount,
        const int* boundIndices,                // バインドあり頂点のインデックス配列
        int boundCount,
        const int* bindCounts,                  // 各頂点のボーン数 (boundCount分)
        const int* boneIndices,                 // ボーンインデックス (boundCount * MAX_BINDS)
        const float* weights,                   // ウェイト (boundCount * MAX_BINDS)
        const Vector3* bindOffsets,             // オフセット (boundCount * MAX_BINDS)
        const Matrix44* activeBoneMatrices,     // ボーン行列配列
        const Vector3* localDeformOffsets,      // ローカルデフォーム変位 (vertexCount分)
        bool isDeform,
        const Matrix44& partMatrix,             // パーツ行列
        const Matrix44& invPartMatrix,          // パーツ逆行列 (isDeform時のみ参照)
        float* outVertices                      // 出力：変形後座標 (float x 3 * vertexCount)
    );

    // ユーティリティ
    static bool InverseMatrix(const Matrix44& src, Matrix44& dst);
};

} // namespace mesh_v2

// --------------------------------------------------------------------------
// SS6-SDK ブリッジ（この下は SS6 固有の実装）
// --------------------------------------------------------------------------

struct SsPartState;
class SsCell;
class ISSTexture;

struct MeshDrawData {
    std::vector<float> vertices;
    std::vector<float> uvs;
    std::vector<float> colors;
    std::vector<uint16_t> indices;
    uint32_t textureHandle;
    int32_t blendType;
};

struct SkinningDataSoA {
    std::vector<int> boundVertexIndices;
    std::vector<int> unboundVertexIndices;
    std::vector<int> bindCounts;
    std::vector<int> boneIndices;
    std::vector<float> weights;
    std::vector<mesh_v2::Vector3> offsets;
};

class IMeshRendererDelegate {
public:
    virtual ~IMeshRendererDelegate() = default;
    virtual void renderMesh(const MeshDrawData& drawData) = 0;
};

class SsMeshPartV2 {
public:
    std::vector<float> baseVertices;
    std::vector<float> uvs;
    std::vector<uint16_t> indices;
    SkinningDataSoA skinningData;
    std::vector<mesh_v2::Matrix44> activeBoneMatrices;
    std::vector<mesh_v2::Vector3> localDeformOffsets;
    SsPartState* myPartState;
    ISSTexture* targetTexture;
    IMeshRendererDelegate* rendererDelegate;

    SsMeshPartV2(SsPartState* s) : myPartState(s), targetTexture(nullptr), rendererDelegate(nullptr) {}
    void update();
    void draw(float alpha);
    void setupFromCell(SsCell* cell, ISSTexture* tex);
private:
    MeshDrawData _cachedDrawData;
};

} // namespace spritestudio6

#endif
