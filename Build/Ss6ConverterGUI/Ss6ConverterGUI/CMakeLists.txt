include(${CMAKE_CURRENT_SOURCE_DIR}/../../../cmake/initialize.cmake)
cmake_minimum_required(VERSION 3.19.2)

project(Ss6ConverterGUI)

OPTIONS()

CCACHE()

PREPARE()

set(CMAKE_OSX_DEPLOYMENT_TARGET "10.15")

if((NOT CMAKE_PROJECT_NAME))
    set(CMAKE_PROJECT_NAME Ss6ConverterGUI)
endif()

# find Qt6
set(QT_DEFAULT_VERSION "6.5.1")
if(MSVC)
    if(NOT $ENV{QT_PREFIX})
        set(QT_PREFIX "$ENV{QT_PREFIX}")
    else()
        message(status ${CMAKE_SYSTEM_PROCESSOR})
        if(${CMAKE_SYSTEM_PROCESSOR} MATCHES "aarch64|ARM64")
            set(QT_PREFIX "C:\\Qt\\${QT_DEFAULT_VERSION}\\msvc2019_arm64")
        else()
            set(QT_PREFIX "C:\\Qt\\${QT_DEFAULT_VERSION}\\msvc2019_64")
        endif()
    endif()
    message(status ${QT_PREFIX})
    set(CMAKE_PREFIX_PATH "${QT_PREFIX}")
elseif(APPLE)
    if(NOT $ENV{QT_PREFIX})
      set(QT_PREFIX "$ENV{QT_PREFIX}")
    else()
        set(QT_PREFIX "$ENV{HOME}/Qt/${QT_DEFAULT_VERSION}/macos")
        if (NOT EXISTS ${QT_PREFIX})
            set(PREFIX "/usr/local")
            if (EXISTS "/opt/homebrew")
                set(PREFIX "/opt/homebrew")
            endif()
            set(QT_PREFIX "${PREFIX}/opt/qt")
        endif()
    endif()
    set(CMAKE_PREFIX_PATH "${QT_PREFIX}")
else()
# TODO: another operation systems
endif()

set(src
    main.cpp
    mainwindow.cpp 
    mainwindow.h
    mainwindow.ui
    resource.qrc
    Application-000.icns
)

set(CMAKE_INCLUDE_CURRENT_DIR ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTOUIC ON)
set(CMAKE_AUTORCC ON)
find_package(Qt6 COMPONENTS Widgets REQUIRED)
find_package(Qt6 COMPONENTS Core REQUIRED)
add_definitions(-DQT_DEPRECATED_WARNINGS)


get_target_property(_qmake_executable Qt6::qmake IMPORTED_LOCATION)
get_filename_component(_qt_bin_dir "${_qmake_executable}" DIRECTORY)

if(WIN32 OR WIN64)
    set(APP_ICON_RESOURCE_WINDOWS Ss6ConverterGUI_resource.rc)
    add_executable(${CMAKE_PROJECT_NAME} WIN32 ${src})
    if(("${CMAKE_BUILD_TYPE}" STREQUAL "Release"))
        find_program(DEPLOYQT_EXECUTABLE windeployqt HINTS "${_qt_bin_dir}")
        add_custom_command(TARGET ${CMAKE_PROJECT_NAME} POST_BUILD COMMAND "${CMAKE_COMMAND}" -E env PATH="${_qt_bin_dir}" "${DEPLOYQT_EXECUTABLE}" "$<TARGET_FILE:${CMAKE_PROJECT_NAME}>" COMMENT "Running windeployqt...")
    endif()
elseif(APPLE)
    set(MACOSX_BUNDLE_ICON_FILE Application-000.icns)
    set_source_files_properties(Application-000.icns PROPERTIES MACOSX_PACKAGE_LOCATION "Resources")
    add_executable(${CMAKE_PROJECT_NAME} MACOSX_BUNDLE ${src})
    if(("${CMAKE_BUILD_TYPE}" STREQUAL "Release"))
        find_program(DEPLOYQT macdeployqt HINTS "${_qt_bin_dir}")
        add_custom_command(TARGET ${CMAKE_PROJECT_NAME} POST_BUILD COMMAND "${DEPLOYQT}" "$<TARGET_FILE_DIR:${CMAKE_PROJECT_NAME}>/../.." -always-overwrite COMMENT "Running macdeployqt...")
    endif()
endif()

target_link_libraries(${CMAKE_PROJECT_NAME} PRIVATE Qt6::Core Qt6::Widgets)
