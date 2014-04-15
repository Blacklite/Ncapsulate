param($installPath, $toolsPath, $package)

Import-Module (Join-Path $toolsPath commands.psm1) -ArgumentList $installPath, $toolsPath
