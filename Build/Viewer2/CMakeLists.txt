include(${CMAKE_CURRENT_SOURCE_DIR}/../../cmake/initialize.cmake)
CMAKE_MINIMUM_REQUIRED(VERSION 3.25)
project(SSView2)

# command options
OPTIONS()

CCACHE()

PREPARE()

message("start fetching dependency libraries")
include(FetchContent)

message("fetch JUCE")
FetchContent_Declare(
    JUCE
    GIT_REPOSITORY https://github.com/juce-framework/JUCE.git
    GIT_TAG        7.0.10)
FetchContent_MakeAvailable(JUCE)

message("finish fetching dependency libraries")

add_definitions(-DUSE_GLEW)
add_definitions(-DJUCE_MODAL_LOOPS_PERMITTED=1)
if(APPLE AND (${CMAKE_SYSTEM_NAME} MATCHES "Darwin"))
    add_definitions(-DJUCE_SILENCE_XCODE_15_LINKER_WARNING)
endif()

if(NOT TARGET ssHelper)
    add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/../../Common ${PROJECT_NAME}_Common)
endif()
include_directories(${CMAKE_CURRENT_SOURCE_DIR})
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Loader)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Animator)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper)

if(NOT TARGET glad)
    add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Libs/glad ${PROJECT_NAME}_glad)
endif()
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Drawer)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Libs/glad/include)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/lib/glfw-master/include)

set(CMAKE_OSX_DEPLOYMENT_TARGET "10.15")

juce_add_gui_app(SSView2
    VERSION 1.0.0                       # Set this if the app version is different to the project version
    # ICON_BIG ...                      # ICON_* arguments specify a path to an image file to use as an icon
    # ICON_SMALL ...
    # DOCUMENT_EXTENSIONS ...           # Specify file extensions that should be associated with this app
    # COMPANY_NAME ...                  # Specify the name of the app's author
    PRODUCT_NAME "SSViewer2"
    REPORT_JUCE_APP_USAGE ON # Required for closed source applications without an Indie or Pro JUCE license
    DISPLAY_THE_JUCE_SPLASH_SCREEN ON # Required for closed source applications without an Indie or Pro JUCE license
    SPLASH_SCREEN_COLOUR "Dark"
    PROJECT_TYPE "GUI Application"
    INCLUDE_BINARYDATA ON
    CXX_LANGUAGE_STANDARD "C++11")

# lib
set(glew lib/glew-2.1.0/src/glew.c)
include_directories(lib/glew-2.1.0/include)

# Controller
set(Controller
        Source/Controller/Main.cpp
        Source/Controller/MainComponent.cpp
        Source/Controller/MainComponent.h)

# View
set(View
        Source/View/DocumentView3D.cpp
        Source/View/DocumentView3D.h
        Source/View/GPL_v3.cpp
        Source/View/GPL_v3.h
        Source/View/MainWindow.cpp
        Source/View/MainWindow.h
        Source/View/initgrew.cpp
        Source/View/initgrew.h
        )
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper)

# Model
set(Model
        Source/Model/Loader.cpp
        Source/Model/Loader.h
        Source/Model/Player.cpp
        Source/Model/Player.h)
## Helper
set(SDKs_Helper
        ../../Common/Helper/OpenGL/SSTextureGL.cpp)

## Drawer
set(SDKs_Drawer
        ../../Common/Drawer/GLSL/sprite.fs
        ../../Common/Drawer/GLSL/sprite.vs
        ../../Common/Drawer/GLSL/sprite_pot.fs
        ../../Common/Drawer/ssOpenGLSetting.h
        ../../Common/Drawer/ssplayer_render_gl.cpp
        ../../Common/Drawer/ssplayer_render_gl.h
        ../../Common/Drawer/ssplayer_shader_gl.cpp
        ../../Common/Drawer/ssplayer_shader_gl.h)
include_directories(../../Common/Drawer)

# generate JuceHeader.h
juce_generate_juce_header(SSView2)

include_directories(Source)

target_sources(SSView2
    PRIVATE
        ${glew}
        ${Controller}
        ${View}
        ${Model}
        ${SDKs_Helper}
        ${SDKs_Drawer}
        )

target_link_libraries(SSView2
    PRIVATE
        # GuiAppData            # If we'd created a binary data target, we'd link to it here
        juce::juce_gui_extra
    PUBLIC
        ssLoader
        ssAnimator
        ssHelper
        glad

        juce::juce_core
        juce::juce_data_structures
        juce::juce_events
        juce::juce_graphics
        juce::juce_gui_basics
        juce::juce_gui_extra
        juce::juce_opengl
        juce::juce_recommended_config_flags
        juce::juce_recommended_lto_flags
        juce::juce_recommended_warning_flags)
