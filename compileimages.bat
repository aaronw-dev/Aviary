@echo off
cd bin/Debug/net6.0
echo Compiling 16x16...
inkscape -w 16 -h 16 -o 16.png icon.svg
echo Compiling 32x32...
inkscape -w 32 -h 32 -o 32.png icon.svg
echo Compiling 48x48...
inkscape -w 48 -h 48 -o 48.png icon.svg
echo Compiling 64x64...
inkscape -w 64 -h 64 -o 64.png icon.svg
echo Done!
cd ..
cd ..
cd ..
@echo on