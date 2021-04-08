@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set TOOLSDIR=%BASEDIR%\Tools
set DEFAULT_QT_PREFIX=C:\Qt\5.11.2\msvc2017_64
if "%QT_PREFIX%" == "" (
    set QT_PREFIX=%DEFAULT_QT_PREFIX%
)
if exist "%QT_PREFIX%\bin\windeployqt.exe" (
    set WINDEPLOYQT="%QT_PREFIX%\bin\windeployqt.exe"
) else (
    set WINDEPLOYQT=windeployqt.exe
)
@echo on

call %CURDIR%\build_converter_win.bat Release || exit /b 1
call %CURDIR%\build_convertergui_win.bat Release || exit /b 1

pushd %BASEDIR%
rmdir /S /Q Ss6Converter
mkdir Ss6Converter
copy %BUILDDIR%\Converter\build\Release\Ss6Converter.exe Ss6Converter\
copy %BUILDDIR%\Ss6ConverterGUI\Ss6ConverterGUI\Release\Ss6ConverterGUI.exe Ss6Converter\
%WINDEPLOYQT% --release Ss6Converter\Ss6ConverterGUI.exe || exit /b 1
zip -r Ss6Converter.zip Ss6Converter
move /y Ss6Converter.zip %TOOLSDIR%\
rmdir /S /Q Ss6Converter
popd