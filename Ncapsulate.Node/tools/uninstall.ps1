param($installPath, $toolsPath, $package, $project)

<# Get the project location #>
$projectPath = $project.FullName;
$projectDirectory = (Get-Item $projectPath).DirectoryName

<# Remove node.cmd #>
Remove-Item ($projectDirectory + '\node.cmd')

<# Remove npm.cmd #>
Remove-Item ($projectDirectory + '\npm.cmd')

<# Remove the build target #>
#$buildProject = @([Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($projectPath))[0]
<#foreach ($import in $buildProject.Xml.Imports) {
	if ($import.Project -eq "App_Build\node.targets") {
		$buildProject.Xml.RemoveChild($import);
		break;
	}
}#>
##$buildProject.Save();

