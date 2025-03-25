1. Copy the directory to a third party folder. In this example the folder name is "VoidPointer.Build"
2. Put a include like this in top of your msbuild-file:

	<PropertyGroup>
	  <RelativePathToVoidPointerBuild>ThirdParty\VoidPointer.Build</RelativePathToVoidPointerBuild>
	</PropertyGroup>
	<Import Project="$(RelativePathToVoidPointerBuild)\vpbuild.msbuild" />

3. Enjoy VoidPointer.Build! :)
