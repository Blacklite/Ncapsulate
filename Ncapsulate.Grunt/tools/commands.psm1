$installPath = $args[0];
$toolsPath   = $args[1];

<# Find the relative location the Ncapsulate.Node #>
$nodePath = (Get-ChildItem "$installPath\..\Ncapsulate.Node.*" | Sort-Object Name -descending)[0].FullName;

$node = ($nodePath + "\nodejs\node");
$grunt = ($installPath + "\nodejs\node_modules\grunt-cli\bin\grunt");

function grunt() {
    & $node $grunt $args;
}

Export-ModuleMember grunt