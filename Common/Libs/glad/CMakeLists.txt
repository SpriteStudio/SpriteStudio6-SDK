include(${CMAKE_CURRENT_SOURCE_DIR}/../../../cmake/initialize.cmake)
cmake_minimum_required(VERSION 3.25)

project(glad)

OPTIONS()

CCACHE()

PREPARE()

include_directories(include)

# gladという名前でCMakeに認識させるライブラリを作成
# 静的ライブラリとしてコンパイル。含まれるソースを指定。
add_library(glad STATIC
    ${CMAKE_CURRENT_SOURCE_DIR}/src/glad.c
)
