$installPath = $args[0];
$toolsPath   = $args[1];

<# Find the relative location the Ncapsulate.Node #>
$nodePath = (Get-ChildItem "$installPath\..\Ncapsulate.Node.*" | Sort-Object Name -descending).FullName;

$node = ($nodePath + "\nodejs\node");
$gulp = ($installPath + "\nodejs\node_modules\gulp\bin\gulp");

function gulp() {
    & $node $gulp $args;
}

Export-ModuleMember gulp