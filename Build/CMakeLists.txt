include(${CMAKE_CURRENT_SOURCE_DIR}/../cmake/initialize.cmake)
CMAKE_MINIMUM_REQUIRED(VERSION 3.19.2)
project(sssdk)

# command options
OPTIONS()

CCACHE()

PREPARE()

enable_testing()

# Ss6Converter
add_subdirectory(Converter)

# SSViewer2
add_subdirectory(Viewer2)
