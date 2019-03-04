@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set DEPENDDIR=%BUILDDIR%\Depends
set TOOLSDIR=%BASEDIR%\Tools
@echo on

call %CURDIR%\prepare_build_viewer2_win.bat || exit /b 1
call %CURDIR%\build_viewer2_win.bat Release || exit /b 1

pushd %BASEDIR%
rmdir /S /Q viewer_sample_2
mkdir viewer_sample_2
copy %BUILDDIR%\Viewer2\cmakeBuild\Release\App\Viewer2.exe viewer_sample_2\
zip -r viewer_sample_2.zip viewer_sample_2
move /y viewer_sample_2.zip %TOOLSDIR%\
rmdir /S /Q viewer_sample_2
popd