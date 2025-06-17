#!/bin/bash -e

SCRIPTDIR=`dirname $0`
SCRIPTDIR=`cd $SCRIPTDIR && pwd -P`
BASEDIR=${SCRIPTDIR}/..
BASEDIR=`cd ${BASEDIR} && pwd -P`
BUILDDIR=${BASEDIR}/Build
BUILDDIR=`cd ${BUILDDIR} && pwd -P`
TOOLSDIR=${BASEDIR}/Tools
TOOLSDIR=`cd ${TOOLSDIR} && pwd -P`

${SCRIPTDIR}/build_viewer2_macos.sh Release

pushd ${BASEDIR}

/bin/rm -f SSViewer2_Mac.zip
ditto -c -k --sequesterRsrc --keepParent ${BUILDDIR}/Viewer2/cmakeBuild/SSView2_artefacts/Release/SSViewer2.app ./SSViewer2_Mac.zip
mv SSViewer2_Mac.zip ${TOOLSDIR}/

popd > /dev/null # ${BASEDIR}
