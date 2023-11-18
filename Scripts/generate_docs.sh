#!/bin/bash -e

SCRIPTDIR=`dirname $0`
SCRIPTDIR=`cd $SCRIPTDIR && pwd -P`
BASEDIR=${SCRIPTDIR}/..
BASEDIR=`cd ${BASEDIR} && pwd -P`
DOCSDIR=${BASEDIR}/docs
DOCSDIR=`cd ${DOCSDIR} && pwd -P`

pushd ${BASEDIR}

/bin/rm -rf ${DOCSDIR}
doxygen Doxyfile

popd > /dev/null # ${BUILDDIR}
