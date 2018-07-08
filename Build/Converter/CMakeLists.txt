cmake_minimum_required(VERSION 3.11)

project(Ss6Converter)

set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# command options
option(ENABLE_CCACHE "Enable ccache?" ON)
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

if(NOT TARGET ssHelper)
    add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/../../Common Ss6Converter_Common)
endif()
add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/../Depends Ss6Converter_Depends)

set(Ss6Converter_SRCS
    main.cpp
    BinaryDataWriter.cpp
    FileUtil.cpp
    Lump.cpp
    LumpExporter.cpp
)

set(Ss6Converter_HEADERS
    BinaryDataWriter.h
    FileUtil.h
    Lump.h
    LumpExporter.h
    SsPlayerConverter.h
)

include_directories(${CMAKE_CURRENT_SOURCE_DIR})
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Loader)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Animator)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper)

add_executable(Ss6Converter ${Ss6Converter_SRCS} ${Ss6Converter_HEADERS})
target_link_libraries(Ss6Converter ssLoader)
target_link_libraries(Ss6Converter ssAnimator)
target_link_libraries(Ss6Converter ssHelper)
if(NOT (WIN32 OR APPLE))
    # For Linux
    target_link_libraries(Ss6Converter ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter_Common/Loader/libssLoader.a)
    target_link_libraries(Ss6Converter ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter_Common/Animator/libssAnimator.a)
    target_link_libraries(Ss6Converter ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter_Common/Helper/libssHelper.a)
endif()

enable_testing()
add_test(NAME allAttributeV6 COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/allAttributeV6/allAttributeV6.sspj)
add_test(NAME allPartsV6 COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/allPartsV6/allPartsV6.sspj)
add_test(NAME effect_allcheck COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/effect_allcheck/effect_allcheck.sspj)
