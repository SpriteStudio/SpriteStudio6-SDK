#!/bin/sh -e

SCRIPTDIR=`dirname $0`
SCRIPTDIR=`cd $SCRIPTDIR && pwd -P`
BASEDIR=${SCRIPTDIR}/..
BASEDIR=`cd ${BASEDIR} && pwd -P`
BUILDDIR=${BASEDIR}/Build
BUILDDIR=`cd ${BUILDDIR} && pwd -P`
TOOLSDIR=${BASEDIR}/Tools
TOOLSDIR=`cd ${TOOLSDIR} && pwd -P`


${SCRIPTDIR}/build_viewer_macos.sh Release

pushd ${BASEDIR}
/bin/rm -f viewer_sample_Mac.zip
/bin/rm -rf viewer_sample_Mac
/bin/mkdir viewer_sample_Mac

/bin/cp -rp ${BUILDDIR}/Viewer/build/viewer_sample_gl.app ./viewer_sample_Mac
zip -r viewer_sample_Mac.zip viewer_sample_Mac

mv viewer_sample_Mac.zip ${TOOLSDIR}/
/bin/rm -rf viewer_sample_Mac/
popd > /dev/null # ${BASEDIR}
