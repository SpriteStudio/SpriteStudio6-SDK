CMAKE_MINIMUM_REQUIRED(VERSION 3.11)
project(sssdk)

# command options
option(ENABLE_CCACHE "Enable ccache?" ON)
if(MSVC)
    set(DEFAULT_STATIC_OPTION YES)
else()
    set(DEFAULT_STATIC_OPTION NO)
endif()
option(ENABLE_PYTHON "Enable Python binding" OFF)
option(ENABLE_STATIC "static link" ${DEFAULT_STATIC_OPTION})

# enable ccache
find_program(CCACHE_EXE ccache)
if(CCACHE_EXE)
    if(ENABLE_CCACHE)
        message(STATUS "Enable ccache")
        if(CMAKE_C_COMPILER_LAUNCHER)
            set(CMAKE_C_COMPILER_LAUNCHER "${CMAKE_C_COMPILER_LAUNCHER}" "${CCACHE_EXE}")
        else()
            set(CMAKE_C_COMPILER_LAUNCHER "${CCACHE_EXE}")
        endif()
        if(CMAKE_CXX_COMPILER_LAUNCHER)
            set(CMAKE_CXX_COMPILER_LAUNCHER "${CMAKE_CXX_COMPILER_LAUNCHER}" "${CCACHE_EXE}")
        else()
            set(CMAKE_CXX_COMPILER_LAUNCHER "${CCACHE_EXE}")
        endif()
    endif()
endif()

if(NOT CMAKE_BUILD_TYPE)
    set(CMAKE_BUILD_TYPE Debug CACHE STRING "" FORCE)
endif()

if(CMAKE_COMPILER_IS_GNUCXX)
    add_definitions(-std=gnu++14)
endif()

IF(WIN32)
    #環境により変更してください
    #SET(BOOST_ROOT "C:/boost-1_64_0")
    #SET(BOOST_ROOT "C:/Boost/include/boost-1_64/")
    #SET(BOOST_PYTHON_LIB "C:/Boost/lib" )
    #SET(PYTHON_IHNCLUDE "C:/Python27/include")
    #SET(PYTHON_LIB "C:/Python27/libs")
ELSEIF(APPLE)
    SET(PYTHON_LIB "/usr/local/lib")
ELSEIF(CMAKE_COMPILER_IS_GNUCXX)
    ADD_DEFINITIONS(-std=gnu++14)
ENDIF()

# viewer_sample_gl and viewer_sample_dx9
add_subdirectory(Viewer)

# scriptbase_converter
if(ENABLE_PYTHON)
    add_subdirectory(PythonBinder)
endif()

# Ss6Converter
add_subdirectory(Converter)
