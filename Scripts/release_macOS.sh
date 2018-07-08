#!/bin/sh -e

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
/bin/rm -f Ss6Converter_Mac.dmg
/bin/rm -rf Ss6Converter
/bin/mkdir Ss6Converter

/bin/cp ${BUILDDIR}/Converter/build/Ss6Converter ./Ss6Converter/
/bin/cp -rp ${BUILDDIR}/Ss6ConverterGUI/Ss6ConverterGUI/Ss6ConverterGUI.app ./Ss6Converter/
hdiutil create -fs HFS+ -format UDZO -srcfolder "Ss6Converter" -volname "Ss6Converter_Mac" Ss6Converter_Mac

mv Ss6Converter_Mac.dmg ${TOOLSDIR}/
/bin/rm -rf Ss6Converter/
popd > /dev/null # ${BASEDIR}
