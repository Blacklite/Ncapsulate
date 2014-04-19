$key = Get-Content api.key
$packages = Get-ChildItem -Recurse NCapsulate*\*.nupkg
foreach ($pkg in $packages) {
	.\.nuget\nuget push $pkg.FullName $key
}