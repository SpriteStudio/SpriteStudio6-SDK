#include "ssplayer_matrix.h"
#include <cstring>

namespace spritestudio6
{

void IdentityMatrix(float* matrix)
{
    SsMatrix4x4 m;
    std::memcpy(matrix, m.m, sizeof(float) * 16);
}

void ScaleMatrix(float* _matrix, const float x, const float y, const float z)
{
    SsMatrix4x4 m = SsMatrix4x4::CreateScale(x, y, z);
    std::memcpy(_matrix, m.m, sizeof(float) * 16);
}

void TranslationMatrix(float* _matrix, const float x, const float y, const float z)
{
    SsMatrix4x4 m = SsMatrix4x4::CreateTranslation(x, y, z);
    std::memcpy(_matrix, m.m, sizeof(float) * 16);
}

void MultiplyMatrix(const float* m0, const float* m1, float* matrix)
{
    SsMatrix4x4 res = SsMatrix4x4(m0) * SsMatrix4x4(m1);
    std::memcpy(matrix, res.m, sizeof(float) * 16);
}

void Matrix4RotationX(float* _matrix, const float radians)
{
    SsMatrix4x4 m = SsMatrix4x4::CreateRotationX(radians);
    std::memcpy(_matrix, m.m, sizeof(float) * 16);
}

void Matrix4RotationY(float* _matrix, const float radians)
{
    SsMatrix4x4 m = SsMatrix4x4::CreateRotationY(radians);
    std::memcpy(_matrix, m.m, sizeof(float) * 16);
}

void Matrix4RotationZ(float* _matrix, const float radians)
{
    SsMatrix4x4 m = SsMatrix4x4::CreateRotationZ(radians);
    std::memcpy(_matrix, m.m, sizeof(float) * 16);
}

void MatrixTransformVector3(float* _matrix, SsVector3& src, SsVector3& dst)
{
    SsMatrix4x4 m(_matrix);
    dst = m.Transform(src);
}

// SsOpenGLMatrix methods implementation
void SsOpenGLMatrix::setScaleMatrix(const float x, const float y, const float z)
{
    SsMatrix4x4 m = SsMatrix4x4::CreateScale(x, y, z);
    std::memcpy(_matrix, m.m, sizeof(float) * 16);
}

void SsOpenGLMatrix::setTranslationMatrix(const float x, const float y, const float z)
{
    SsMatrix4x4 m = SsMatrix4x4::CreateTranslation(x, y, z);
    std::memcpy(_matrix, m.m, sizeof(float) * 16);
}

void SsOpenGLMatrix::multiply(SsOpenGLMatrix& m1, SsOpenGLMatrix& m2)
{
    SsMatrix4x4 res = SsMatrix4x4(m1._matrix) * SsMatrix4x4(m2._matrix);
    std::memcpy(_matrix, res.m, sizeof(float) * 16);
}

void SsOpenGLMatrix::Matrix4RotationX(const float radians)
{
    SsMatrix4x4 m = SsMatrix4x4::CreateRotationX(radians);
    std::memcpy(_matrix, m.m, sizeof(float) * 16);
}

void SsOpenGLMatrix::Matrix4RotationY(const float radians)
{
    SsMatrix4x4 m = SsMatrix4x4::CreateRotationY(radians);
    std::memcpy(_matrix, m.m, sizeof(float) * 16);
}

void SsOpenGLMatrix::Matrix4RotationZ(const float radians)
{
    SsMatrix4x4 m = SsMatrix4x4::CreateRotationZ(radians);
    std::memcpy(_matrix, m.m, sizeof(float) * 16);
}

} // namespace spritestudio6
