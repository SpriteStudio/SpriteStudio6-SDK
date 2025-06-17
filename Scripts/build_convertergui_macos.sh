#!/bin/bash -e


if [ -z ${QT_PREFIX+x} ]; then
    QT_PREFIX=${HOME}/Qt/6.9.0/macos

    if [ ! -d $QT_PREFIX ]; then
        PREFIX=/usr/local
        if [ -e /opt/homebrew ]; then
            PREFIX=/opt/homebrew
        fi
        QT_PREFIX=${PREFIX}/opt/qt
    fi
fi

SCRIPTDIR=`dirname $0`
SCRIPTDIR=`cd $SCRIPTDIR && pwd -P`
BASEDIR=${SCRIPTDIR}/..
BASEDIR=`cd ${BASEDIR} && pwd -P`
BUILDDIR=${BASEDIR}/Build
BUILDDIR=`cd ${BUILDDIR} && pwd -P`

BUILDTYPE=Debug
if [ $# -ge 1 ]; then
    BUILDTYPE=$1
fi

pushd ${BUILDDIR}

pushd Ss6ConverterGUI/Ss6ConverterGUI

/bin/rm -rf build
/bin/mkdir build
pushd build > /dev/null
if type "ninja" > /dev/null; then
    cmake -G Ninja -DCMAKE_BUILD_TYPE=${BUILDTYPE} ..
else
    cmake -DCMAKE_BUILD_TYPE=${BUILDTYPE} ..
fi
cmake --build . --parallel

popd > /dev/null # build
popd > /dev/null # Ss6ConverterGUI/Ss6ConverterGUI/
popd > /dev/null # ${BUILDDIR}
