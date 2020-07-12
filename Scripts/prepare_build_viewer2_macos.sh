#!/bin/sh -e

SCRIPTDIR=`dirname $0`
SCRIPTDIR=`cd $SCRIPTDIR && pwd -P`
BASEDIR=${SCRIPTDIR}/..
BASEDIR=`cd ${BASEDIR} && pwd -P`
BUILDDIR=${BASEDIR}/Build
BUILDDIR=`cd ${BUILDDIR} && pwd -P`

pushd ${BUILDDIR}/Viewer2
/bin/rm -rf JUCE
/bin/rm -rf FRUT
git clone --branch=6.0.1 --depth=1 --single-branch https://github.com/WeAreROLI/JUCE.git
git clone https://github.com/McMartin/FRUT.git

# create Jucer2CMake command
pushd FRUT
mkdir build 
pushd build
cmake .. -DCMAKE_BUILD_TYPE=Release -DCMAKE_INSTALL_PREFIX=../prefix -DJUCE_ROOT=../../JUCE
cmake --build . --target install
popd > /dev/null # build
popd > /dev/null # FRUT


popd > /dev/null # ${BUILDDIR}/Viewer2
