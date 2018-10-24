@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set DEPENDDIR=%BUILDDIR%\Depends
set TOOLSDIR=%BASEDIR%\Tools
@echo on

call %CURDIR%\build_viewer_win.bat Release

pushd %BASEDIR%
rmdir /S/Q viewer_sample
mkdir viewer_sample
copy %BUILDDIR%\Viewer\build\Release\* viewer_sample\
zip -r viewer_sample.zip viewer_sample
move viewer_sample.zip %TOOLSDIR%\
rmdir /S/Q viewer_sample
popd