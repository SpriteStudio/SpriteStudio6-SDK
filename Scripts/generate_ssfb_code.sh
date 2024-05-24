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

cmake -DCMAKE_BUILD_TYPE=Release -DENABLE_FLATC=ON ..
cmake --build . --target flatc --parallel
FLATC=$(pwd)/_deps/flatbuffers-build/flatc

popd > /dev/null # build

# generate ssfb code
${FLATC} -c fbs/ssfb.fbs --gen-compare --gen-object-api 
${FLATC} -n fbs/ssfb.fbs
${FLATC} -T fbs/ssfb.fbs

popd > /dev/null # Converter
popd > /dev/null # ${BUILDDIR}
