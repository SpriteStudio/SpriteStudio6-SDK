@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set DEPENDDIR=%BUILDDIR%\Depends
set TOOLSDIR=%BASEDIR%\Tools
@echo on

call %CURDIR%\build_viewer_win.bat Release || exit /b 1

pushd %BASEDIR%
rmdir /S /Q viewer_sample
mkdir viewer_sample
copy %BUILDDIR%\Viewer\build\Release\* viewer_sample\
zip -r viewer_sample.zip viewer_sample
move /y viewer_sample.zip %TOOLSDIR%\
rmdir /S /Q viewer_sample
popd