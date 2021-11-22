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
pushd flatbuffers

git clean -fdx .
cmake .
cmake --build .  --parallel
FLATC=$(pwd)/flatc

popd > /dev/null # flatbuffers

# generate ssfb code
${FLATC} -c fbs/ssfb.fbs --gen-compare --gen-object-api 
${FLATC} -s fbs/ssfb.fbs
${FLATC} -T fbs/ssfb.fbs --no-fb-import 

popd > /dev/null # Converter
popd > /dev/null # ${BUILDDIR}
