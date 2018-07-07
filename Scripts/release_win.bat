@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set TOOLSDIR=%BASEDIR%\Tools
set QT_PREFIX=C:\Qt\5.11.1\msvc2017_64
set VCVARSALL="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Auxiliary\Build\vcvarsall.bat"
@echo on

cd "%BUILDDIR%\Converter"
rmdir /S /Q build
mkdir build
cd build
cmake -G "Visual Studio 15 2017 Win64" -DCMAKE_BUILD_TYPE=Release .. || exit /b 1
cmake --build . --target ALL_BUILD -- /p:Configuration=Release || exit /b 1
ctest -C Release .

call %VCVARSALL% x64
cd "%BUILDDIR%\Ss6ConverterGUI\Ss6ConverterGUI"
if exist Makefile (
    nmake distclean
)
%QT_PREFIX%\bin\qmake Release=1 || exit /b 1
nmake || exit /b 1

cd %BASEDIR%
rmdir /S/Q Ss6Converter
mkdir Ss6Converter
copy %BUILDDIR%\Converter\build\Release\Ss6Converter.exe Ss6Converter\
copy %BUILDDIR%\Ss6ConverterGUI\Ss6ConverterGUI\Release\Ss6ConverterGUI.exe Ss6Converter\
%QT_PREFIX%\bin\windeployqt --release Ss6Converter\Ss6ConverterGUI.exe || exit /b 1
zip -r Ss6Converter.zip Ss6Converter
move Ss6Converter.zip %TOOLSDIR%\
rmdir /S/Q Ss6Converter
