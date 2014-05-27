$installPath = $args[0];
$toolsPath   = $args[1];

<# Find the relative location the Ncapsulate.Node #>
$nodePath = (Get-ChildItem "$installPath\..\Ncapsulate.Node.*" | Sort-Object Name -descending).FullName;

$node = ($nodePath + "\nodejs\node");
$bower = ($installPath + "\nodejs\node_modules\bower\bin\bower");

function bower() {
    & $node $bower $args;
}

Export-ModuleMember bower