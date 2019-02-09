#!/bin/sh -e

SCRIPTDIR=`dirname $0`
SCRIPTDIR=`cd $SCRIPTDIR && pwd -P`
BASEDIR=${SCRIPTDIR}/..
BASEDIR=`cd ${BASEDIR} && pwd -P`
BUILDDIR=${BASEDIR}/Build
BUILDDIR=`cd ${BUILDDIR} && pwd -P`

Jucer2Reprojucer=${BUILDDIR}/Viewer2/FRUT/prefix/FRUT/bin/Jucer2Reprojucer

# generate CMakeLists.txt
pushd ${BUILDDIR}/Viewer2
${Jucer2Reprojucer} ./Viewer2.jucer ./FRUT/prefix/FRUT/cmake/Reprojucer.cmake  --juce-modules ./JUCE/modules

/bin/rm -rf cmakeBuild
mkdir cmakeBuild
pushd cmakeBuild
cmake .. -DCMAKE_OSX_ARCHITECTURES=x86_64
cmake --build .
/bin/cp ../misc/Info.plist ./Viewer2.app/Contents/Info.plist
popd > /dev/null # cmakeBuild
popd > /dev/null # ${BUILDDIR}/Viewer2
