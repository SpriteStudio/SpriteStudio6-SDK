#!/bin/sh -xe
    QT_PREFIX=${HOME}/Qt/5.15.2/clang_64

if [ -z ${QT_PREFIX+x} ]; then
    QT_PREFIX=${HOME}/Qt/5.11.1/clang_64
    if [ ! -d $QT_PREFIX ]; then
        QT_PREFIX=/usr/local/opt/qt@5
    fi
fi

SCRIPTDIR=`dirname $0`
SCRIPTDIR=`cd $SCRIPTDIR && pwd -P`
BASEDIR=${SCRIPTDIR}/..
BASEDIR=`cd ${BASEDIR} && pwd -P`
BUILDDIR=${BASEDIR}/Build
BUILDDIR=`cd ${BUILDDIR} && pwd -P`

BUILDTYPE=Debug
if [ $# -ge 1 ]; then
    BUILDTYPE=$1
fi
BUILDTYPE_DOWNCASE=`echo $BUILDTYPE |  tr '[:upper:]' '[:lower:]'`

pushd ${BUILDDIR}

pushd Ss6ConverterGUI/Ss6ConverterGUI
if [ -f Makefile ]; then
    make distclean
fi
${QT_PREFIX}/bin/qmake "CONFIG+=${BUILDTYPE_DOWNCASE}" Ss6ConverterGUI.pro
make -j4
${QT_PREFIX}/bin/macdeployqt Ss6ConverterGUI.app
popd > /dev/null # Ss6ConverterGUI/Ss6ConverterGUI/

popd > /dev/null # ${BUILDDIR}
