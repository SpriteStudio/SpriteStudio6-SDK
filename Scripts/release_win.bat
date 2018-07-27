@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set TOOLSDIR=%BASEDIR%\Tools
set QT_PREFIX=C:\Qt\5.11.1\msvc2017_64
@echo on

call %CURDIR%\build_converter_win.bat Release
call %CURDIR%\build_convertergui_win.bat Release

pushd %BASEDIR%
rmdir /S/Q Ss6Converter
mkdir Ss6Converter
copy %BUILDDIR%\Converter\build\Release\Ss6Converter.exe Ss6Converter\
copy %BUILDDIR%\Ss6ConverterGUI\Ss6ConverterGUI\Release\Ss6ConverterGUI.exe Ss6Converter\
%QT_PREFIX%\bin\windeployqt --release Ss6Converter\Ss6ConverterGUI.exe || exit /b 1
zip -r Ss6Converter.zip Ss6Converter
move Ss6Converter.zip %TOOLSDIR%\
rmdir /S/Q Ss6Converter
popd