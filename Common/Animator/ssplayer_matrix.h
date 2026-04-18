#ifndef __SSPLAYER_MATRIX__
#define __SSPLAYER_MATRIX__

#include "ssplayer_matrix_v2.h"

namespace spritestudio6
{

// Legacy C-style functions re-implemented using v2
void IdentityMatrix(float* matrix);
void ScaleMatrix(float* _matrix, const float x, const float y, const float z);
void TranslationMatrix(float* _matrix, const float x, const float y, const float z);
void MultiplyMatrix(const float* m0, const float* m1, float* matrix);
void Matrix4RotationX(float* _matrix, const float radians);
void Matrix4RotationY(float* _matrix, const float radians);
void Matrix4RotationZ(float* _matrix, const float radians);

void MatrixTransformVector3(float* _matrix, SsVector3& src, SsVector3& dst);

inline void TranslationMatrixM(float* _matrix, const float x, const float y, const float z)
{
    SsMatrix4x4 m = SsMatrix4x4(_matrix) * SsMatrix4x4::CreateTranslation(x, y, z);
    std::memcpy(_matrix, m.m, sizeof(float) * 16);
}

inline void ScaleMatrixM(float* _matrix, const float x, const float y, const float z)
{
    SsMatrix4x4 m = SsMatrix4x4(_matrix) * SsMatrix4x4::CreateScale(x, y, z);
    std::memcpy(_matrix, m.m, sizeof(float) * 16);
}

inline void RotationXYZMatrixM(float* _matrix, const float x, const float y, const float z)
{
    SsMatrix4x4 m(_matrix);
    if (x != 0.0f) m = m * SsMatrix4x4::CreateRotationX(x);
    if (y != 0.0f) m = m * SsMatrix4x4::CreateRotationY(y);
    if (z != 0.0f) m = m * SsMatrix4x4::CreateRotationZ(z);
    std::memcpy(_matrix, m.m, sizeof(float) * 16);
}

/**
 * Legacy SsOpenGLMatrix class (Now a wrapper for SsMatrix4x4)
 * Renamed to imply it's for compatibility, but keeping the original name for source compatibility.
 */
class SsOpenGLMatrix
{
public:
    float _matrix[16];

public:
    SsOpenGLMatrix() { identityMatrix(); }
    SsOpenGLMatrix(float* src) { pushMatrix(src); }
    virtual ~SsOpenGLMatrix() {}

    void identityMatrix()
    {
        SsMatrix4x4 m;
        std::memcpy(_matrix, m.m, sizeof(float) * 16);
    }

    void popMatrix(float* dst) { std::memcpy(dst, _matrix, sizeof(float) * 16); }
    void pushMatrix(float* src) { std::memcpy(_matrix, src, sizeof(float) * 16); }

    inline void Scaling(const float x, const float y, const float z)
    {
        SsMatrix4x4 m = SsMatrix4x4(_matrix) * SsMatrix4x4::CreateScale(x, y, z);
        std::memcpy(_matrix, m.m, sizeof(float) * 16);
    }

    inline void Translation(const float x, const float y, const float z)
    {
        SsMatrix4x4 m = SsMatrix4x4(_matrix) * SsMatrix4x4::CreateTranslation(x, y, z);
        std::memcpy(_matrix, m.m, sizeof(float) * 16);
    }

    void setScaleMatrix(const float x, const float y, const float z);
    void setTranslationMatrix(const float x, const float y, const float z);
    void multiply(SsOpenGLMatrix& m1, SsOpenGLMatrix& m2);

    void inverseMatrix()
    {
        SsMatrix4x4 m(_matrix);
        m = m.Inverse();
        std::memcpy(_matrix, m.m, sizeof(float) * 16);
    }

    float getPositionX() const { return _matrix[12]; }
    float getPositionY() const { return _matrix[13]; }

    void TransformVector3(SsVector3& src, SsVector3& dst)
    {
        SsMatrix4x4 m(_matrix);
        dst = m.Transform(src);
    }

    inline void RotationXYZ(const float x, const float y, const float z)
    {
        SsMatrix4x4 m(_matrix);
        if (x != 0.0f) m = m * SsMatrix4x4::CreateRotationX(x);
        if (y != 0.0f) m = m * SsMatrix4x4::CreateRotationY(y);
        if (z != 0.0f) m = m * SsMatrix4x4::CreateRotationZ(z);
        std::memcpy(_matrix, m.m, sizeof(float) * 16);
    }

    void Matrix4RotationX(const float radians);
    void Matrix4RotationY(const float radians);
    void Matrix4RotationZ(const float radians);
};

} // namespace spritestudio6

#endif // __SSPLAYER_MATRIX__
