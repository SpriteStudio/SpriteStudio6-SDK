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

pushd ${BASEDIR}
/bin/rm -f Ss6Converter_Mac.zip
/bin/rm -rf Ss6Converter
/bin/mkdir Ss6Converter

/bin/cp ${BUILDDIR}/Converter/build/Ss6Converter ./Ss6Converter/
/bin/cp -rp ${BUILDDIR}/Ss6ConverterGUI/Ss6ConverterGUI/build/Ss6ConverterGUI.app ./Ss6Converter/
zip -r Ss6Converter_Mac.zip Ss6Converter/

mv Ss6Converter_Mac.zip ${TOOLSDIR}/
/bin/rm -rf Ss6Converter/
popd > /dev/null # ${BASEDIR}
