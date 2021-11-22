#!/bin/sh -e

SCRIPTDIR=`dirname $0`
SCRIPTDIR=`cd $SCRIPTDIR && pwd -P`
BASEDIR=${SCRIPTDIR}/..
BASEDIR=`cd ${BASEDIR} && pwd -P`
BUILDDIR=${BASEDIR}/Build
BUILDDIR=`cd ${BUILDDIR} && pwd -P`

Jucer2CMake=${BUILDDIR}/Viewer2/FRUT/prefix/FRUT/bin/Jucer2CMake

BUILDTYPE=Debug
if [ $# -ge 1 ]; then
    BUILDTYPE=$1
fi

# generate CMakeLists.txt
pushd ${BUILDDIR}/Viewer2
${Jucer2CMake} reprojucer ./Viewer2.jucer ./FRUT/prefix/FRUT/cmake/Reprojucer.cmake  --juce-modules $(pwd)/JUCE/modules

/bin/rm -rf cmakeBuild
mkdir cmakeBuild
pushd cmakeBuild
cmake .. -DCMAKE_OSX_ARCHITECTURES=x86_64  -DCMAKE_BUILD_TYPE=${BUILDTYPE}
cmake --build . --parallel
/bin/cp ../misc/Info.plist ./Viewer2.app/Contents/Info.plist
popd > /dev/null # cmakeBuild
popd > /dev/null # ${BUILDDIR}/Viewer2
