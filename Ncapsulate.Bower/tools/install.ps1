param($installPath, $toolsPath, $package, $project)

<# Get the project location #>
$projectPath = $project.FullName;
$projectDirectory = (Get-Item $projectPath).DirectoryName

<# Find the relative location of this package #>
$tmp = Get-Location
Set-Location $projectDirectory
$bowerRelativePath = Resolve-Path -Relative $installPath
Set-Location $tmp

<# Find the relative location the Ncapsulate.Node #>
$nodePath = (Get-ChildItem "$installPath\..\Ncapsulate.Node.*" | Sort-Object Name -descending)[0].FullName;
Set-Location $projectDirectory
$nodeRelativePath = Resolve-Path -Relative $nodePath
Set-Location $tmp

<# Install bower.cmd #>
$bowerCmd = "@echo off
$nodeRelativePath\nodejs\node $bowerRelativePath\nodejs\node_modules\bower\bin\bower %*
@echo on";
$bowerLocation = ($projectDirectory + '\bower.cmd')
Set-Content $bowerLocation $bowerCmd -Encoding String

<# Install the build targets (so they can be configured beyond the defaults #>
$buildProject = @([Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($projectPath))[0]
$buildProject.Xml.AddImport("App_Build\bower.targets");
$buildProject.Save();

