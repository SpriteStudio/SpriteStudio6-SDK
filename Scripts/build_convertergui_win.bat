@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set QT_PREFIX=C:\Qt\5.11.1\msvc2017_64
set VCDIR=C:\Program Files (x86)\Microsoft Visual Studio\2017
@echo on

set BUILD_TYPE=Debug
if not "%1" == "" (
    set BUILD_TYPE=%1
)

@echo off
set BUILD_TYPE=%BUILD_TYPE:A=a%
set BUILD_TYPE=%BUILD_TYPE:B=b%
set BUILD_TYPE=%BUILD_TYPE:C=c%
set BUILD_TYPE=%BUILD_TYPE:D=d%
set BUILD_TYPE=%BUILD_TYPE:E=e%
set BUILD_TYPE=%BUILD_TYPE:F=f%
set BUILD_TYPE=%BUILD_TYPE:G=g%
set BUILD_TYPE=%BUILD_TYPE:H=h%
set BUILD_TYPE=%BUILD_TYPE:I=i%
set BUILD_TYPE=%BUILD_TYPE:J=j%
set BUILD_TYPE=%BUILD_TYPE:K=K%
set BUILD_TYPE=%BUILD_TYPE:L=l%
set BUILD_TYPE=%BUILD_TYPE:M=m%
set BUILD_TYPE=%BUILD_TYPE:N=n%
set BUILD_TYPE=%BUILD_TYPE:O=o%
set BUILD_TYPE=%BUILD_TYPE:P=p%
set BUILD_TYPE=%BUILD_TYPE:Q=q%
set BUILD_TYPE=%BUILD_TYPE:R=r%
set BUILD_TYPE=%BUILD_TYPE:S=s%
set BUILD_TYPE=%BUILD_TYPE:T=t%
set BUILD_TYPE=%BUILD_TYPE:U=u%
set BUILD_TYPE=%BUILD_TYPE:V=v%
set BUILD_TYPE=%BUILD_TYPE:W=w%
set BUILD_TYPE=%BUILD_TYPE:X=x%
set BUILD_TYPE=%BUILD_TYPE:Y=y%
set BUILD_TYPE=%BUILD_TYPE:Z=z%
@echo on

if exist "%VCDIR%\Enterprise" (
    set VCVARSALL="%VCDIR%\Enterprise\VC\Auxiliary\Build\vcvarsall.bat"
) else if exist "%VCDIR%\Professional" (
    set VCVARSALL="%VCDIR%\Professional\VC\Auxiliary\Build\vcvarsall.bat"
) else (
    set VCVARSALL="%VCDIR%\Community\VC\Auxiliary\Build\vcvarsall.bat"
)

call %VCVARSALL% x64 || exit /b 1
pushd "%BUILDDIR%\Ss6ConverterGUI\Ss6ConverterGUI"
if exist Makefile (
    nmake distclean
)
%QT_PREFIX%\bin\qmake "CONFIG+=%BUILD_TYPE%" || exit /b 1
nmake || exit /b 1
popd
