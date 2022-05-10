cmake_minimum_required(VERSION 3.19.2)

project(ssHelper)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

if(MSVC)
add_compile_options("$<$<C_COMPILER_ID:MSVC>:/utf-8>")
add_compile_options("$<$<CXX_COMPILER_ID:MSVC>:/utf-8>")
endif()

if(APPLE)
    if(("${CMAKE_BUILD_TYPE}" STREQUAL "Release") OR (NOT CMAKE_OSX_ARCHITECTURES))
        set(CMAKE_OSX_ARCHITECTURES "arm64;x86_64")
    endif()
endif()

set(ssHelper_SRCS
    IsshTexture.cpp
    sshTextureBMP.cpp
    stb_image.c
    sshScene.cpp
    sshTask.cpp
    XPFileOpenDlg.cpp
    DebugPrint.cpp
)

set(ssHelper_HEADERS
    IsshTexture.h
    sshTextureBMP.h
    stb_image.h
    sshObject.h
    sshTask.h
    XPFileOpenDlg.h
    DebugPrint.h
)

if(MSVC)
    set(ssHelper_SRCS ${ssHelper_SRCS}
        Win/WinMain.cpp
    )
elseif(APPLE AND (${CMAKE_SYSTEM_NAME} MATCHES "Darwin"))
    set(ssHelper_SRCS ${ssHelper_SRCS}
        MacOS/OSXFileOpen.mm
    )
    set(ssHelper_HEADERS ${ssHelper_HEADERS}
        MacOS/OSXFileOpen.h
    )
endif()

include_directories(${CMAKE_CURRENT_SOURCE_DIR})

ADD_LIBRARY(ssHelper ${ssHelper_SRCS} ${ssHelper_HEADERS})
