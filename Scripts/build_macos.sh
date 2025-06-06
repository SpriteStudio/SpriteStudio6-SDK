#!/bin/bash -e

SCRIPTDIR=`dirname $0`
SCRIPTDIR=`cd $SCRIPTDIR && pwd -P`
BASEDIR=${SCRIPTDIR}/..
BASEDIR=`cd ${BASEDIR} && pwd -P`
BUILDDIR=${BASEDIR}/Build
BUILDDIR=`cd ${BUILDDIR} && pwd -P`
TOOLSDIR=${BASEDIR}/Tools
TOOLSDIR=`cd ${TOOLSDIR} && pwd -P`

${SCRIPTDIR}/build_converter_macos.sh Release
${SCRIPTDIR}/build_convertergui_macos.sh Release
/bin/cp ${BUILDDIR}/Converter/build/Ss6Converter ${BUILDDIR}/Ss6ConverterGUI/Ss6ConverterGUI/build/Ss6ConverterGUI.app/Contents/MacOS/

${SCRIPTDIR}/build_viewer2_macos.sh Release