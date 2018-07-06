function(Depends_Lib)
    if(APPLE AND (${CMAKE_SYSTEM_NAME} MATCHES "Darwin"))
        set(DEPSDIR "/usr/local/opt") # homebrew prefix
    elseif(MSVC)
        set(DEPSDIR "${CMAKE_SOURCE_DIR}/")
    endif()

    # boost include directories
    if (DEFINED $ENV{BOOST_INCLUDE_PATH})
        include_directories($ENV{BOOST_INCLUDE_PATH})
    else()
        if(APPLE AND (${CMAKE_SYSTEM_NAME} MATCHES "Darwin"))
            include_directories(${DEPSDIR}/boost/include)
        elseif(MSVC)
            include_directories(${DEPSDIR}/boost_1_66_0)
        endif()
    endif()

    ## boost library directories
    if (DEFINED $ENV{BOOST_LIB_PATH})
        set(BOOST_LIB_PATH $ENV{BOOST_LIB_PATH})
    else()
        if(APPLE AND (${CMAKE_SYSTEM_NAME} MATCHES "Darwin"))
            set(BOOST_LIB_PATH ${DEPSDIR}/boost/lib)
        else(MSVC)
            # TODO: change msvc version
            if(WIN32)
                set(BOOST_LIB_PATH ${DEPSDIR}/boost_1_66_0/lib32-msvc-14.1)
                set(BOOST_LIB_ARCH x32)
            else()
                # don't consider ARM yet.
                set(BOOST_LIB_PATH ${DEPSDIR}/boost_1_66_0/lib64-msvc-14.1)
                set(BOOST_LIB_ARCH x64)
            endif()
        endif()
    endif()
    link_directories(${BOOST_LIB_PATH})
    if(MSVC AND ENABLE_STATIC)
        # TODO: change msvc version
        if(CMAKE_BUILD_TYPE MATCHES Release)
            set(BOOST_LIB_SUFFIX vc141-mt-${BOOST_LIB_ARCH}-1_66)
        else()
            set(BOOST_LIB_SUFFIX vc141-mt-gd-${BOOST_LIB_ARCH}-1_66)
        endif()
    else()
    # TODO:
    endif()

    # python include directories
    if (DEFINED $ENV{PYTHON_INCLUDE_PATH})
        include_directories($ENV{PYTHON_INCLUDE_PATH})
    else()
        if(APPLE AND (${CMAKE_SYSTEM_NAME} MATCHES "Darwin"))
            # TODO:
            include_directories(${DEPSDIR}/python/Frameworks/Python.framework/Versions/3.7/Headers)
        elseif(MSVC)
            include_directories(${DEPSDIR}/boost_1_66_0)
        endif()
    endif()
endfunction()