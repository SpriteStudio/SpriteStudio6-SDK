#!/bin/sh -e

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
DIR=viewer_sample_2_Mac
/bin/rm -f ${DIR}.zip
/bin/rm -rf ${DIR}
/bin/mkdir ${DIR}

/bin/cp -rp ${BUILDDIR}/Viewer2/cmakeBuild/SSView2_artefacts/Release/SSViewer2.app ./${DIR}
zip -r ${DIR}.zip ${DIR}

mv ${DIR}.zip ${TOOLSDIR}/
/bin/rm -rf ${DIR}/
popd > /dev/null # ${BASEDIR}
