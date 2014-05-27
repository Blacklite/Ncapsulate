param($installPath, $toolsPath, $package, $project)

<# Get the project location #>
$projectPath = $project.FullName;
$projectDirectory = (Get-Item $projectPath).DirectoryName

$projectDirectory

<# Find the relative location of this package #>
$tmp = Get-Location
Set-Location $projectDirectory
$gruntRelativePath = Resolve-Path -Relative $installPath
Set-Location $tmp

$gruntRelativePath

<# Find the relative location the Ncapsulate.Node #>
$nodePath = (Get-ChildItem "$installPath\..\Ncapsulate.Node.*" | Sort-Object Name -descending).FullName;
Set-Location $projectDirectory
$nodeRelativePath = Resolve-Path -Relative $nodePath
Set-Location $tmp

$nodeRelativePath

<# Install grunt.cmd #>
$gruntCmd = "@echo off
$nodeRelativePath\nodejs\node $gruntRelativePath\nodejs\node_modules\grunt-cli\bin\grunt --no-color %*
@echo on";
$gruntLocation = ($projectDirectory + '\grunt.cmd')
Set-Content $gruntLocation $gruntCmd -Encoding String


<# Install the build targets (so they can be configured beyond the defaults #>
#$buildProject = @([Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($projectPath))[0]
#$buildProject.Xml.AddImport("App_Build\grunt.targets");
##$buildProject.Save();
