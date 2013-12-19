@echo off
cd %~dp0
if exist compiled goto cleanup
goto compile
:cleanup
echo Removing existing compiled assets...
del /S /Q compiled
:compile
echo Compiling assets...
cd assets
..\..\Protogame\ProtogameAssetTool\bin\Debug\ProtogameAssetTool.exe ^
	-a ..\..\Tychaia.Asset\bin\Debug\Tychaia.Asset.dll ^
	-o ..\compiled ^
	-p Windows ^
	-p Linux 
cd ..

rem -p MacOSX ^
rem -p Xbox360 ^
rem -p WindowsPhone ^
rem -p iOS ^
rem -p Android ^
rem -p WindowsStoreApp ^
rem -p NativeClient ^
rem -p Ouya ^
rem -p PlayStationMobile ^
rem -p WindowsPhone8 ^
rem -p RaspberryPi ^