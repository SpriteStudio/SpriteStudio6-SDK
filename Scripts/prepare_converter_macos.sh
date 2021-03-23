#!/bin/bash -ex

SCRIPTDIR=`dirname $0`
SCRIPTDIR=`cd $SCRIPTDIR && pwd -P`
BASEDIR=${SCRIPTDIR}/..
BASEDIR=`cd ${BASEDIR} && pwd -P`
BUILDDIR=${BASEDIR}/Build
BUILDDIR=`cd ${BUILDDIR} && pwd -P`

pushd ${BASEDIR}
pushd ${BUILDDIR}

#/bin/rm -rf vcpkg
#git clone https://github.com/Microsoft/vcpkg.git
./vcpkg/bootstrap-vcpkg.sh

if [ -z ${VCPKG_DEFAULT_TRIPLET+x} ]; then
   export VCPKG_DEFAULT_TRIPLET=x64-osx
fi

pushd vcpkg > /dev/null
./vcpkg install zlib
./vcpkg install minizip
popd > /dev/null

popd > /dev/null # 