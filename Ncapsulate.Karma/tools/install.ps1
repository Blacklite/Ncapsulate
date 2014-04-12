param($installPath, $toolsPath, $package, $project)

<# Get the project location #>
$projectPath = $project.FullName;
$projectDirectory = (Get-Item $projectPath).DirectoryName

$projectDirectory

<# Find the relative location of this package #>
$tmp = Get-Location
Set-Location $projectDirectory
$karmaRelativePath = Resolve-Path -Relative $installPath
Set-Location $tmp

$karmaRelativePath

<# Find the relative location the Ncapsulate.Node #>
$nodePath = (Get-ChildItem "$installPath\..\Ncapsulate.Node.*" | Sort-Object Name -descending)[0].FullName;
Set-Location $projectDirectory
$nodeRelativePath = Resolve-Path -Relative $nodePath
Set-Location $tmp

$nodeRelativePath

<# Install karma.cmd #>
$karmaCmd = "@echo off
$nodeRelativePath\nodejs\node $karmaRelativePath\nodejs\node_modules\karma-cli\bin\karma --no-colors %*
@echo on";
$karmaLocation = ($projectDirectory + '\karma.cmd')
Set-Content $karmaLocation $karmaCmd -Encoding String


<# Install the build targets (so they can be configured beyond the defaults #>
[xml]$xml = Get-Content ($project.FullName);
$import = $xml.CreateElement('Import');
$import.SetAttribute("Project", "App_Build\karma.targets");
$xml.AppendChild($xml);
$xml.Save();
