include(${CMAKE_CURRENT_SOURCE_DIR}/../../cmake/initialize.cmake)
cmake_minimum_required(VERSION 3.19.2)

project(ssHelper)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

OPTIONS()

CCACHE()

PREPARE()

set(ssHelper_SRCS
#    IsshTexture.cpp
    SsTextureFactory.cpp
    sshTextureBMP.cpp
    stb_image.c
    sshScene.cpp
    sshTask.cpp
    XPFileOpenDlg.cpp
    DebugPrint.cpp
    SsRawImage.cpp
    UnicodeConv.cpp
)

set(ssHelper_HEADERS
 #   IsshTexture.h
    SsTextureFactory.h
    sshTextureBMP.h
    stb_image.h
    sshObject.h
    sshTask.h
    XPFileOpenDlg.h
    DebugPrint.h
    SsRawImage.h
    UnicodeConv.h
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
