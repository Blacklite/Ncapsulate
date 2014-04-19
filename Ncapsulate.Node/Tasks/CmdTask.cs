using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

using Microsoft.Build.Framework;

namespace Ncapsulate.Node.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CmdTask : Microsoft.Build.Utilities.AppDomainIsolatedTask
    {
        /// <summary>Invokes a command-line process asynchronously, capturing its output to a string.</summary>
        /// <returns>Null if the process exited successfully; the process' full output if it failed.</returns>
        public static async Task<string> ExecWithOutputResultAsync(string filename, string args, string workingDirectory = null)
        {
            var error = new StringWriter();
            var output = new StringWriter();

            int result = await ExecAsync(filename, args, workingDirectory, output, error);

            var errorString = error.ToString().Trim();
            var outputString = output.ToString().Trim();

            return result == 0 || String.IsNullOrWhiteSpace(errorString) ? outputString : ("ERROR:" + error.ToString().Trim());
        }

        /// <summary>Invokes a command-line process asynchronously, capturing its output to a string.</summary>
        /// <returns>Null if the process exited successfully; the process' full output if it failed.</returns>
        public static async Task<string> ExecWithOutputAsync(string filename, string args, string workingDirectory = null)
        {
            var error = new StringWriter();
            int result = await ExecAsync(filename, args, workingDirectory, null, error);

            return result == 0 ? null : error.ToString().Trim();
        }

        /// <summary>Invokes a command-line process asynchronously.</summary>
        public static Task<int> ExecAsync(string filename, string args, string workingDirectory = null, TextWriter stdout = null, TextWriter stderr = null)
        {
            stdout = stdout ?? TextWriter.Null;
            stderr = stderr ?? TextWriter.Null;

            var p = new Process
                        {
                            StartInfo = new ProcessStartInfo(filename, args)
                                            {
                                                UseShellExecute = false,
                                                CreateNoWindow = true,
                                                RedirectStandardError = true,
                                                RedirectStandardOutput = true,
                                                WorkingDirectory = workingDirectory == null ? null : Path.GetFullPath(workingDirectory),
                                            },
                            EnableRaisingEvents = true,
                        };

            p.OutputDataReceived += (sender, e) => stdout.WriteLine(e.Data);
            p.ErrorDataReceived += (sender, e) => stderr.WriteLine(e.Data);

            p.Start();
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
            var processTaskCompletionSource = new TaskCompletionSource<int>();

            p.EnableRaisingEvents = true;
            p.Exited += (s, e) =>
                {
                    p.WaitForExit();
                    processTaskCompletionSource.TrySetResult(p.ExitCode);
                };

            return processTaskCompletionSource.Task;
        }

        /// <summary>
        /// Due to the way node_modues work, the directory depth can get very deep and go beyond MAX_PATH (260 chars). 
        /// Therefore grab all node_modues directories and move them up to baseNodeModuleDir. Node's require() will then 
        /// traverse up and find them at the higher level. Should be fine as long as there are no versioning conflicts.
        /// </summary>
        protected void FlattenNodeModules(string baseNodeModuleDir)
        {
            var baseDir = new DirectoryInfo(baseNodeModuleDir);
            //var baseModulesDir = new DirectoryInfo(baseNodeModuleDir + "\node_modules");

            var nodeModulesDirs = from dir in baseDir.EnumerateDirectories("*", SearchOption.AllDirectories)
                                  where dir.Name.Equals("node_modules", StringComparison.OrdinalIgnoreCase)
                                  orderby dir.FullName.Count(c => c == Path.DirectorySeparatorChar) descending // Get deepest first
                                  select dir;

            foreach (var module in nodeModulesDirs.ToArray()
                .Where(x => Directory.Exists(x.FullName))
                .SelectMany(z => z.EnumerateDirectories().ToArray().Where(x => Directory.Exists(x.FullName))))
            {
                // If the package uses a non-default main file,
                // add a redirect in index.js so that require()
                // can find it without package.json.
                if (module.Name != ".bin" && !File.Exists(Path.Combine(module.FullName, "index.js")))
                {
                    dynamic package = Json.Decode(File.ReadAllText(Path.Combine(module.FullName, "package.json")));
                    string main = package.main;

                    if (!string.IsNullOrEmpty(main))
                    {
                        if (!main.StartsWith("."))
                            main = "./" + main;

                        File.WriteAllText(
                            Path.Combine(module.FullName, "index.js"),
                            "module.exports = require(" + Json.Encode(main) + ");"
                            );
                    }
                }

                string targetDir = Path.Combine(baseDir.FullName, "node_modules", module.Name);
                var targetInfo = new DirectoryInfo(targetDir);
                if (!Directory.Exists(targetDir))
                {
                    module.MoveTo(targetDir);
                }
                else if (module.Name != ".bin")
                {
                    var targetPackage = targetInfo.EnumerateFiles("package.json").FirstOrDefault();
                    var modulePackage = module.EnumerateFiles("package.json").FirstOrDefault();
                    if (targetPackage != null && modulePackage != null)
                    {
                        var targetPackageJson = Json.Decode(File.ReadAllText(targetPackage.FullName));
                        var targetVersionString = (string)targetPackageJson.version;

                        var modulePackageJson = Json.Decode(File.ReadAllText(modulePackage.FullName));
                        var moduleVersionString = (string)modulePackageJson.version;

                        if (targetVersionString != null && moduleVersionString != null)
                        {
                            if (String.Compare(targetVersionString, moduleVersionString, StringComparison.OrdinalIgnoreCase) > 0)
                            {
                                Directory.Delete(targetDir, true);
                                module.MoveTo(targetDir);
                                this.Log.LogMessage(
                                    MessageImportance.High,
                                    "Collapsing conflicting module " + module.Name + " v"
                                    + targetPackageJson.version + " \n\t(vs existing version v"
                                    + modulePackageJson.version + ")");
                                continue;
                            }
                            else if (!String.Equals(module.FullName.TrimEnd('\\'), targetInfo.FullName.TrimEnd('\\')) &&
                                     String.Compare(targetVersionString, moduleVersionString, StringComparison.OrdinalIgnoreCase) <= 0)
                            {
                                Directory.Delete(module.FullName, true);
                                this.Log.LogMessage(
                                    MessageImportance.High,
                                    "Deleting existing module " + module.Name + " v"
                                    + targetPackageJson.version + " \n\t(vs existing version v"
                                    + modulePackageJson.version + ")");
                                this.Log.LogMessage(
                                    MessageImportance.High,
                                    module.FullName);
                                this.Log.LogMessage(
                                    MessageImportance.High,
                                    targetInfo.FullName);
                                continue;
                            }
                        }
                    }

                    this.Log.LogMessage(
                        MessageImportance.High,
                        "Not collapsing conflicting module " + module.Name);
                }

                if (!String.Equals(module.FullName.TrimEnd('\\'), targetInfo.FullName.TrimEnd('\\')))
                {
                    if (string.Equals(module.Name, ".bin", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var binFile in module.EnumerateFiles())
                        {
                            var targetBin = Path.Combine(baseDir.FullName, "node_modules", ".bin", binFile.Name);
                            binFile.CopyTo(targetBin, true);
                        }
                    }
                    else
                    {
                        Directory.Delete(module.FullName, true);
                        this.Log.LogMessage(MessageImportance.High, "Deleting existing module " + module.Name);
                        this.Log.LogMessage(MessageImportance.High, module.FullName);
                        this.Log.LogMessage(MessageImportance.High, targetInfo.FullName);
                    }
                }
            }


            foreach (var nodeModules in nodeModulesDirs.ToArray().Where(x => Directory.Exists(x.FullName)))
            {
                if (!nodeModules.EnumerateFileSystemInfos().Any())
                    nodeModules.Delete();
            }
        }
    }
}