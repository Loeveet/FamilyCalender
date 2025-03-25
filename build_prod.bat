@echo off


cd FamilyCalender.Web
call ..\ThirdParty\VoidPointer.Build\hMSBuild_minified.bat FamilyCalender.msbuild /t:FamilyCalander /p:Env=prod /p:NoWarn=1591
cd ..
pause
goto :end

:end
