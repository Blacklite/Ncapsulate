$installPath = $args[0];
$toolsPath   = $args[1];

<# Find the relative location the Ncapsulate.Node #>
$nodePath = (Get-ChildItem "$installPath\..\Ncapsulate.Node.*" | Sort-Object Name -descending)[0].FullName;

$node = ($nodePath + "\nodejs\node");
$karma = ($installPath + "\nodejs\node_modules\karma-cli\bin\karma");

function karma() {
    & $node $karma $args;
}

Export-ModuleMember karma