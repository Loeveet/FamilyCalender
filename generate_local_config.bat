@echo off
call ThirdParty\VoidPointer.Build\hMSBuild_minified.bat FamilyCalender.Web\FamilyCalender.msbuild /p:Env=local /t:GenerateConfig
pause
