#include "ssplayer_matrix_v2.h"
#include <algorithm>

namespace spritestudio6
{

SsMatrix4x4 SsMatrix4x4::CreateIdentity() noexcept
{
    return SsMatrix4x4();
}

SsMatrix4x4 SsMatrix4x4::CreateScale(float x, float y, float z) noexcept
{
    SsMatrix4x4 res;
    res.m[0] = x;
    res.m[5] = y;
    res.m[10] = z;
    return res;
}

SsMatrix4x4 SsMatrix4x4::CreateTranslation(float x, float y, float z) noexcept
{
    SsMatrix4x4 res;
    res.m[12] = x;
    res.m[13] = y;
    res.m[14] = z;
    return res;
}

SsMatrix4x4 SsMatrix4x4::CreateRotationX(float radians) noexcept
{
    SsMatrix4x4 res;
    float s = std::sin(radians);
    float c = std::cos(radians);
    res.m[5] = c;  res.m[6] = s;
    res.m[9] = -s; res.m[10] = c;
    return res;
}

SsMatrix4x4 SsMatrix4x4::CreateRotationY(float radians) noexcept
{
    SsMatrix4x4 res;
    float s = std::sin(radians);
    float c = std::cos(radians);
    res.m[0] = c;  res.m[2] = -s;
    res.m[8] = s;  res.m[10] = c;
    return res;
}

SsMatrix4x4 SsMatrix4x4::CreateRotationZ(float radians) noexcept
{
    SsMatrix4x4 res;
    float s = std::sin(radians);
    float c = std::cos(radians);
    res.m[0] = c;  res.m[1] = s;
    res.m[4] = -s; res.m[5] = c;
    return res;
}

// Optimized Multiply: Column-based approach (maps 1:1 to Rust's wide::f32x4)
SsMatrix4x4 SsMatrix4x4::Multiply(const SsMatrix4x4& lhs, const SsMatrix4x4& rhs) noexcept
{
    SsMatrix4x4 res;
    const float* a = lhs.m;
    const float* b = rhs.m;
    float* out = res.m;

    for (int j = 0; j < 4; ++j) {
        // Right-hand side column j elements (Scalars in wide-SIMD terms)
        const float b0j = b[j * 4 + 0];
        const float b1j = b[j * 4 + 1];
        const float b2j = b[j * 4 + 2];
        const float b3j = b[j * 4 + 3];

        // Result column j = (A_col0 * b0j) + (A_col1 * b1j) + (A_col2 * b2j) + (A_col3 * b3j)
        // This block is exactly what f32x4 operations do in Rust wide crate.
        out[j * 4 + 0] = a[0] * b0j + a[4] * b1j + a[8] * b2j + a[12] * b3j;
        out[j * 4 + 1] = a[1] * b0j + a[5] * b1j + a[9] * b2j + a[13] * b3j;
        out[j * 4 + 2] = a[2] * b0j + a[6] * b1j + a[10] * b2j + a[14] * b3j;
        out[j * 4 + 3] = a[3] * b0j + a[7] * b1j + a[11] * b2j + a[15] * b3j;
    }
    return res;
}

SsVector3 SsMatrix4x4::Transform(const SsMatrix4x4& mat, const SsVector3& v) noexcept
{
    // Points are assumed to have w = 1.0f
    return SsVector3(
        v.x * mat.m[0] + v.y * mat.m[4] + v.z * mat.m[8] + mat.m[12],
        v.x * mat.m[1] + v.y * mat.m[5] + v.z * mat.m[9] + mat.m[13],
        v.x * mat.m[2] + v.y * mat.m[6] + v.z * mat.m[10] + mat.m[14]
    );
}

// Highly optimized Inverse using the Adjugate Matrix (Cofactor) method.
// This is branchless and unrolled, making it much faster than Gauss-Jordan for 4x4.
SsMatrix4x4 SsMatrix4x4::Inverse(const SsMatrix4x4& mat) noexcept
{
    const float* m = mat.m;
    
    float s0 = m[0] * m[5] - m[1] * m[4];
    float s1 = m[0] * m[6] - m[2] * m[4];
    float s2 = m[0] * m[7] - m[3] * m[4];
    float s3 = m[1] * m[6] - m[2] * m[5];
    float s4 = m[1] * m[7] - m[3] * m[5];
    float s5 = m[2] * m[7] - m[3] * m[6];

    float c5 = m[10] * m[15] - m[11] * m[14];
    float c4 = m[9] * m[15] - m[11] * m[13];
    float c3 = m[9] * m[14] - m[10] * m[13];
    float c2 = m[8] * m[15] - m[11] * m[12];
    float c1 = m[8] * m[14] - m[10] * m[12];
    float c0 = m[8] * m[13] - m[9] * m[12];

    float det = s0 * c5 - s1 * c4 + s2 * c3 + s3 * c2 - s4 * c1 + s5 * c0;

    if (std::abs(det) < 1e-12f) return CreateIdentity();

    float invDet = 1.0f / det;
    SsMatrix4x4 res;

    res.m[0] = ( m[5] * c5 - m[6] * c4 + m[7] * c3) * invDet;
    res.m[1] = (-m[1] * c5 + m[2] * c4 - m[3] * c3) * invDet;
    res.m[2] = ( m[13] * s5 - m[14] * s4 + m[15] * s3) * invDet;
    res.m[3] = (-m[9] * s5 + m[10] * s4 - m[11] * s3) * invDet;

    res.m[4] = (-m[4] * c5 + m[6] * c2 - m[7] * c1) * invDet;
    res.m[5] = ( m[0] * c5 - m[2] * c2 + m[3] * c1) * invDet;
    res.m[6] = (-m[12] * s5 + m[14] * s2 - m[15] * s1) * invDet;
    res.m[7] = ( m[8] * s5 - m[10] * s2 + m[11] * s1) * invDet;

    res.m[8] = ( m[4] * c4 - m[5] * c2 + m[7] * c0) * invDet;
    res.m[9] = (-m[0] * c4 + m[1] * c2 - m[3] * c0) * invDet;
    res.m[10] = ( m[12] * s4 - m[13] * s2 + m[15] * s0) * invDet;
    res.m[11] = (-m[8] * s4 + m[9] * s2 - m[11] * s0) * invDet;

    res.m[12] = (-m[4] * c3 + m[5] * c1 - m[6] * c0) * invDet;
    res.m[13] = ( m[0] * c3 - m[1] * c1 + m[2] * c0) * invDet;
    res.m[14] = (-m[12] * s3 + m[13] * s1 - m[14] * s0) * invDet;
    res.m[15] = ( m[8] * s3 - m[9] * s1 + m[10] * s0) * invDet;

    return res;
}

} // namespace spritestudio6
