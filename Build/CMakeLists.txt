CMAKE_MINIMUM_REQUIRED(VERSION 3.14)
project(sssdk)

# command options
option(ENABLE_CCACHE "Enable ccache?" ON)
if(MSVC)
    set(DEFAULT_STATIC_OPTION YES)
else()
    set(DEFAULT_STATIC_OPTION NO)
endif()
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
ELSEIF(APPLE)
ELSEIF(CMAKE_COMPILER_IS_GNUCXX)
    ADD_DEFINITIONS(-std=gnu++14)
ENDIF()


enable_testing()

# Ss6Converter
add_subdirectory(Converter)
