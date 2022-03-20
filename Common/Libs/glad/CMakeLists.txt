cmake_minimum_required(VERSION 3.19.2)

project(glad)

if(APPLE)
    if(("${CMAKE_BUILD_TYPE}" STREQUAL "Release") OR (NOT CMAKE_OSX_ARCHITECTURES))
        set(CMAKE_OSX_ARCHITECTURES "arm64;x86_64")
    endif()
endif()

include_directories(include)

# gladという名前でCMakeに認識させるライブラリを作成
# 静的ライブラリとしてコンパイル。含まれるソースを指定。
add_library(glad STATIC
    ${CMAKE_CURRENT_SOURCE_DIR}/src/glad.c
)
