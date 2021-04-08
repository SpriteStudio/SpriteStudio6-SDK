set BASEDIR=%~dp0
set DSTDIR=%BASEDIR%..\Build

if not ""%1""=="""" (
    set DSTDIR=%1
)

pushd %DSTDIR%
pushd vcpkg
call bootstrap-vcpkg.bat

set VCPKG_DEFAULT_TRIPLET=x64-windows-static
vcpkg install zlib
vcpkg install minizip

vcpkg integrate install
popd

popd

