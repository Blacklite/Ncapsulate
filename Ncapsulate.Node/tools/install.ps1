param($installPath, $toolsPath, $package, $project)

<# Get the project location #>
$projectPath = $project.FullName;
$projectDirectory = (Get-Item $projectPath).DirectoryName

<# Find the relative location of this package #>
$tmp = Get-Location
Set-Location $projectDirectory
$relativePath = Resolve-Path -Relative $installPath
Set-Location $tmp

<# Install node.cmd #>
$nodeCmd = "@echo off
$relativePath\nodejs\node %*
@echo on";
$nodeLocation = ($projectDirectory + '\node.cmd')
Set-Content $nodeLocation $nodeCmd -Encoding String



<# Install npm.cmd #>
$npmCmd = "@echo off
$relativePath\nodejs\npm %*
@echo on";
$npmLocation = ($projectDirectory + '\npm.cmd')
Set-Content $npmLocation $npmCmd -Encoding String


<# Install the build targets (so they can be configured beyond the defaults #>
[xml]$xml = Get-Content ($project.FullName);
$import = $xml.CreateElement('Import');
$import.SetAttribute("Project", "App_Build\node.targets");
$xml.AppendChild($xml);
$xml.Save();
