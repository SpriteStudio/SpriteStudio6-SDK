#!/bin/sh -e

if [ -z ${QT_PREFIX+x} ]; then
    QT_PREFIX=${HOME}/Qt/5.11.1/clang_64
    if [ ! -d $QT_PREFIX ]; then
        QT_PREFIX=/usr/local/opt/qt
    fi
fi

SCRIPTDIR=`dirname $0`
SCRIPTDIR=`cd $SCRIPTDIR && pwd -P`
BASEDIR=${SCRIPTDIR}/..
BASEDIR=`cd ${BASEDIR} && pwd -P`
BUILDDIR=${BASEDIR}/Build
BUILDDIR=`cd ${BUILDDIR} && pwd -P`
TOOLSDIR=${BASEDIR}/Tools
TOOLSDIR=`cd ${TOOLSDIR} && pwd -P`

pushd ${BUILDDIR}

# Build Converter
pushd Converter
/bin/rm -rf build
/bin/mkdir build
pushd build
cmake -DENABLE_CCACHE=OFF -DCMAKE_BUILD_TYPE=Release -DCMAKE_OSX_ARCHITECTURES=x86_64 ..
make -j4
ctest .
popd > /dev/null # build
popd > /dev/null # Converter

pushd Ss6ConverterGUI/Ss6ConverterGUI
if [ -f Makefile ]; then
    make distclean
fi
${QT_PREFIX}/bin/qmake Release=1 Ss6ConverterGUI.pro
make -j4
${QT_PREFIX}/bin/macdeployqt Ss6ConverterGUI.app
popd > /dev/null # Ss6ConverterGUI/Ss6ConverterGUI/

popd > /dev/null # ${BUILDDIR}


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
