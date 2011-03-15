@call "%VS100COMNTOOLS%\vsvars32.bat"

@msbuild.exe deploy\build.xml
