#!/bin/bash -e

SCRIPTDIR=$(dirname $0)
SCRIPTDIR=$(cd $SCRIPTDIR && pwd -P)
BASEDIR=${SCRIPTDIR}/..
BASEDIR=$(cd ${BASEDIR} && pwd -P)
BUILDDIR=${BASEDIR}/Build
BUILDDIR=$(cd ${BUILDDIR} && pwd -P)
TOOLSDIR=${BASEDIR}/Tools
TOOLSDIR=$(cd ${TOOLSDIR} && pwd -P)

pushd ${BASEDIR}

# clean up
/bin/rm -rf ./Tools/*.zip

# pack and delopy
## Converter GUI a(nd CLI)
/bin/rm -f Ss6Converter_Mac.zip
ditto -c -k --sequesterRsrc --keepParent ${BUILDDIR}/Ss6ConverterGUI/Ss6ConverterGUI/build/Ss6ConverterGUI.app ./Ss6Converter_Mac.zip
mv Ss6Converter_Mac.zip ${TOOLSDIR}/

## Viewer2
/bin/rm -f SSViewer2_Mac.zip
ditto -c -k --sequesterRsrc --keepParent ${BUILDDIR}/Viewer2/cmakeBuild/SSView2_artefacts/Release/SSViewer2.app ./SSViewer2_Mac.zip
mv SSViewer2_Mac.zip ${TOOLSDIR}/

popd > /dev/null # ${BASEDIR}

