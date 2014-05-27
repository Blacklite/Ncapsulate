param($installPath, $toolsPath, $package, $project)

<# Get the project location #>
$projectPath = $project.FullName;
$projectDirectory = (Get-Item $projectPath).DirectoryName

$projectDirectory

<# Find the relative location of this package #>
$tmp = Get-Location
Set-Location $projectDirectory
$gulpRelativePath = Resolve-Path -Relative $installPath
Set-Location $tmp

$gulpRelativePath

<# Find the relative location the Ncapsulate.Node #>
$nodePath = (Get-ChildItem "$installPath\..\Ncapsulate.Node.*" | Sort-Object Name -descending).FullName;
Set-Location $projectDirectory
$nodeRelativePath = Resolve-Path -Relative $nodePath
Set-Location $tmp

$nodeRelativePath

<# Install gulp.cmd #>
$gulpCmd = "@echo off
$nodeRelativePath\nodejs\node $gulpRelativePath\nodejs\node_modules\gulp\bin\gulp --no-color %*
@echo on";
$gulpLocation = ($projectDirectory + '\gulp.cmd')
Set-Content $gulpLocation $gulpCmd -Encoding String


<# Install the build targets (so they can be configured beyond the defaults #>
#$buildProject = @([Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($projectPath))[0]
#$buildProject.Xml.AddImport("App_Build\gulp.targets");
##$buildProject.Save();
