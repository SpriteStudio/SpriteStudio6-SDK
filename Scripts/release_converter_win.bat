@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set TOOLSDIR=%BASEDIR%\Tools
@echo on

call "%CURDIR%\build_converter_win.bat" Release || exit /b 1
call "%CURDIR%\build_convertergui_win.bat" Release || exit /b 1

pushd %BASEDIR%
rmdir /S /Q Ss6Converter
mkdir Ss6Converter
copy "%BUILDDIR%\Converter\build\Release\Ss6Converter.exe" Ss6Converter\
copy "%BUILDDIR%\Ss6ConverterGUI\Ss6ConverterGUI\build\Release\Ss6ConverterGUI.exe" Ss6Converter\
powershell compress-archive Ss6Converter Ss6Converter.zip
move /y Ss6Converter.zip "%TOOLSDIR%\"
rmdir /S /Q Ss6Converter
popd