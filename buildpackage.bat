@echo off

rem Example Execution: buildpackage.bat "C:\Program Files\Unity\Editor\Unity.exe"

rem Build package takes in the path to our unity installation
set unityPath=%1

rem shortcut to our shared command line arguments
set general=-batchmode -quit -projectPath "%cd%"

rem First we export our tests and samples folder into our main preview tween folder
echo Building Test and Sample Packages
%unityPath% %general% -exportPackage Assets\Tests Assets\PreviewTween\Tests.unitypackage
%unityPath% %general% -exportPackage Assets\Samples Assets\PreviewTween\Samples.unitypackage

rem then we package the main folder as a new package ready for upload
echo Building PreviewTween Package
%unityPath% %general% -exportPackage Assets\PreviewTween PreviewTween.unitypackage

rem then we delete the intermediate files
echo Deleting intermediate files
del Assets\PreviewTween\Tests.unitypackage
del Assets\PreviewTween\Tests.unitypackage.meta
del Assets\PreviewTween\Samples.unitypackage
del Assets\PreviewTween\Samples.unitypackage.meta

rem we are now done
echo Complete