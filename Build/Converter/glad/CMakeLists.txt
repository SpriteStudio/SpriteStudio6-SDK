cmake_minimum_required(VERSION 3.12)

# gladという名前でCMakeに認識させるライブラリを作成
# 静的ライブラリとしてコンパイル。含まれるソースを指定。
add_library(glad STATIC
    src/glad.c
)
