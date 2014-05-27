$installPath = $args[0];
$toolsPath   = $args[1];

<# Find the relative location the Ncapsulate.Node #>
$nodePath = (Get-ChildItem "$installPath\..\Ncapsulate.Node.*" | Sort-Object Name -descending).FullName;

$node = ($nodePath + "\nodejs\node");
$npm = ($nodePath + "\nodejs\npm");

function node() {
    & $node $args;
}

function npm() {
    & $npm $args;
}

Export-ModuleMember node, npm