@echo off
cd %~dp0
if not exist MakeMeAWorld.Client\Tychaia(
    mkdir MakeMeAWorld.Client\Tychaia
)
Libraries\JSIL\bin\JSILc.exe -o MakeMeAWorld.Client\Tychaia TychaiaWorldGenWebsite\bin\Debug\TychaiaWorldGenWebsite.dll
TychaiaWorldGenWebsite.ConfigConvert\bin\Debug\TychaiaWorldGenWebsite.ConfigConvert.exe Other\WorldConfig.xml MakeMeAWorld.Client\Tychaia\WorldConfig.js
