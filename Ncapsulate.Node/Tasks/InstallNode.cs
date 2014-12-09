/***
 * These tasks are based on code used by the wonderful Web Essentials Extension.
 * No ownership is claimed, the relivante pieces of code are to be associated with the Web Essentials Extension.
 * https://raw.githubusercontent.com/madskristensen/WebEssentials2013/master/LICENSE.txt
 ***/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.Build.Framework;

namespace Ncapsulate.Node.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    public class InstallNode : CmdTask
    {
        public override bool Execute()
        {
            if (Directory.Exists(@"nodejs\node_modules"))
                Directory.Delete(@"nodejs\node_modules", true);
            Directory.CreateDirectory(@"nodejs\tools");
            // Force npm to install modules to the subdirectory
            // https://npmjs.org/doc/files/npm-folders.html#More-Information

            // We install our modules in this subdirectory so that
            // we can clean up their dependencies without catching
            // npm's modules, which we don't want.
            File.WriteAllText(@"nodejs\tools\package.json", @"{""name"":""Ncapsulate.Node""}");

            // Since this is a synchronous job, I have
            // no choice but to synchronously wait for
            // the tasks to finish. However, the async
            // still saves threads.

            Task.WaitAll(
                DownloadNodeAsync(),
                DownloadNpmAsync()
            );

            Task.WaitAll(UpdateNpm());

            return true;
        }

        private Task DownloadNodeAsync()
        {
            if (File.Exists(@"nodejs\node.exe"))
            {
                // Always want to package the latest version, the nupkg will be based on the current node version
                Log.LogMessage(MessageImportance.High, "Downloading nodejs ...");

                //return Task.FromResult<object>(null);
                return new WebClient().DownloadFileTaskAsync("http://nodejs.org/dist/latest/node.exe", @"nodejs\node.exe");
            }
            Log.LogMessage(MessageImportance.High, "Downloading nodejs ...");
            return new WebClient().DownloadFileTaskAsync("http://nodejs.org/dist/latest/node.exe", @"nodejs\node.exe");
        }

        private async Task DownloadNpmAsync()
        {
            if (File.Exists(@"nodejs\node_modules\npm\bin\npm.cmd"))
            {
                Log.LogMessage(MessageImportance.High, "Already have a version of npm ...");
                return;
            }

            Log.LogMessage(MessageImportance.High, "Downloading npm ...");

            var npmZip = await new WebClient().OpenReadTaskAsync("http://nodejs.org/dist/npm/npm-1.4.9.zip");

            try
            {
                ExtractZipWithOverwrite(npmZip, @"nodejs");
            }
            catch
            {
                // Make sure the next build doesn't see a half-installed npm
                Directory.Delete(@"nodejs\node_modules\npm", true);
                throw;
            }
        }

        private async Task UpdateNpm()
        {
            var output = await ExecWithOutputAsync(@"cmd", @"/c ..\npm.cmd install npm", @"nodejs\tools\");

            if (output != null)
            {
                Log.LogError("npm install npm -g error: " + output);
                throw new Exception("npm install npm -g error");
            }

            output = await ExecWithOutputAsync(@"cmd", @"/c ..\npm.cmd dedup", @"nodejs\tools\");

            if (output != null)
            {
                Log.LogError("npm dedup -g error: " + output);
            }

            FlattenNodeModules(@"nodejs\tools");

            foreach (var module in new DirectoryInfo(@"nodejs\node_modules").EnumerateDirectories())
            {
                module.Delete(true);
            }

            Directory.Delete(@"nodejs\node_modules", true);
            Directory.Move(@"nodejs\tools\node_modules", @"nodejs\node_modules");
        }

        private void ExtractZipWithOverwrite(Stream sourceZip, string destinationDirectoryName)
        {
            using (var source = new ZipArchive(sourceZip, ZipArchiveMode.Read))
            {
                foreach (var entry in source.Entries)
                {
                    const string prefix = "node_modules/npm/node_modules/";

                    // Collapse nested node_modules folders to avoid MAX_PATH issues from Path.GetFullPath
                    var targetSubPath = entry.FullName;
                    if (targetSubPath.StartsWith(prefix) && targetSubPath.Length > prefix.Length)
                    {
                        // If there is another node_modules folder after the prefix, collapse them
                        var lastModule = entry.FullName.LastIndexOf("node_modules/", System.StringComparison.Ordinal);
                        if (lastModule > prefix.Length)
                            targetSubPath = targetSubPath.Remove(prefix.Length, lastModule + "node_modules/".Length - prefix.Length);
                        Log.LogMessage(MessageImportance.Low, entry.FullName + "\t=> " + targetSubPath);
                    }

                    var targetPath = Path.GetFullPath(Path.Combine(destinationDirectoryName, targetSubPath));

                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                    if (!targetPath.EndsWith(@"\"))
                        entry.ExtractToFile(targetPath, overwrite: true);
                }
            }
        }
    }
}