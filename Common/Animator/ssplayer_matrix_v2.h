#ifndef __SSPLAYER_MATRIX_V2__
#define __SSPLAYER_MATRIX_V2__

#include "../Loader/sstypes.h"
#include <cmath>
#include <cstring>

namespace spritestudio6
{

/**
 * Modern Matrix 4x4 class
 * Memory Layout: Column-Major (OpenGL compatible)
 * Designed for easy porting to Rust:
 * - Fixed size array
 * - Aligned for potential SIMD (Rust: #[repr(C, align(16))])
 */
struct alignas(16) SsMatrix4x4
{
    float m[16];

    // Constructors
    SsMatrix4x4() noexcept { Identity(); }
    explicit SsMatrix4x4(const float* src) noexcept
    {
        for (int i = 0; i < 16; ++i) m[i] = src[i];
    }

    void Identity() noexcept
    {
        for (int i = 0; i < 16; ++i) m[i] = 0.0f;
        m[0] = m[5] = m[10] = m[15] = 1.0f;
    }

    // Accessors
    float& operator[](int index) { return m[index]; }
    const float& operator[](int index) const { return m[index]; }

    // Static Factories (Map directly to Rust associated functions)
    static SsMatrix4x4 CreateIdentity() noexcept;
    static SsMatrix4x4 CreateScale(float x, float y, float z) noexcept;
    static SsMatrix4x4 CreateTranslation(float x, float y, float z) noexcept;
    static SsMatrix4x4 CreateRotationX(float radians) noexcept;
    static SsMatrix4x4 CreateRotationY(float radians) noexcept;
    static SsMatrix4x4 CreateRotationZ(float radians) noexcept;

    // Pure Math Operations
    static SsMatrix4x4 Multiply(const SsMatrix4x4& lhs, const SsMatrix4x4& rhs) noexcept;
    static SsVector3 Transform(const SsMatrix4x4& mat, const SsVector3& v) noexcept;
    static SsMatrix4x4 Inverse(const SsMatrix4x4& mat) noexcept;

    // Helper operators for C++ usage
    SsMatrix4x4 operator*(const SsMatrix4x4& rhs) const noexcept
    {
        return Multiply(*this, rhs);
    }

    SsVector3 Transform(const SsVector3& v) const noexcept
    {
        return Transform(*this, v);
    }

    SsMatrix4x4 Inverse() const noexcept
    {
        return Inverse(*this);
    }
};

} // namespace spritestudio6

#endif // __SSPLAYER_MATRIX_V2__
