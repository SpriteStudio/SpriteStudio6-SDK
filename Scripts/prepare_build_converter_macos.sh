#!/bin/sh -e

SCRIPTDIR=`dirname $0`
SCRIPTDIR=`cd $SCRIPTDIR && pwd -P`
BASEDIR=${SCRIPTDIR}/..
BASEDIR=`cd ${BASEDIR} && pwd -P`
BUILDDIR=${BASEDIR}/Build
BUILDDIR=`cd ${BUILDDIR} && pwd -P`

pushd ${BUILDDIR}/Converter
/bin/rm -rf glfw
git clone https://github.com/glfw/glfw.git

popd > /dev/null # ${BUILDDIR}/Converter

