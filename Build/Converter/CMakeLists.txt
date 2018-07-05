cmake_minimum_required(VERSION 3.11)

project(Ss6Converter)

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
target_link_libraries(Ss6Converter ssAnimater)
target_link_libraries(Ss6Converter ssHelper)