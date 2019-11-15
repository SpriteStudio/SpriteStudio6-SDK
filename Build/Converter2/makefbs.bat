cd ..\Converter\flatbuffers
mkdir _build
cd _build
cmake ..
cmake --build .
cd ..
cd ..
cd ..
cd Converter2
copy ..\Converter\flatbuffers\_build\Debug\flatc.exe .
flatc --gen-onefile -n fbs\ssfb2.fbs 
flatc --gen-onefile -c fbs\ssfb2.fbs 
copy ssfb2.cs DLL\

