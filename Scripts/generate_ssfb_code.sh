#!/bin/sh -ex

SCRIPTDIR=`dirname $0`
SCRIPTDIR=`cd $SCRIPTDIR && pwd -P`
BASEDIR=${SCRIPTDIR}/..
BASEDIR=`cd ${BASEDIR} && pwd -P`
BUILDDIR=${BASEDIR}/Build
BUILDDIR=`cd ${BUILDDIR} && pwd -P`

pushd ${BUILDDIR}

pushd Converter

# Build flatc
/bin/rm -rf build
mkdir build
pushd build

cmake ..
cmake --build . --parallel
FLATC=$(pwd)/_deps/flatbuffers-build/flatc

popd > /dev/null # build

# generate ssfb code
${FLATC} -c fbs/ssfb.fbs --gen-compare --gen-object-api 
${FLATC} -s fbs/ssfb.fbs
${FLATC} -T fbs/ssfb.fbs --no-fb-import 

popd > /dev/null # Converter
popd > /dev/null # ${BUILDDIR}
