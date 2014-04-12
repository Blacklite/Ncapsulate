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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.Build.Framework;

namespace Ncapsulate.Node.Tasks
{
    /// <summary>
    /// This is not compiled (ItemType=None) but is invoked by the Inline Task (http://msdn.microsoft.com/en-us/library/dd722601) in the csproj file.
    /// </summary>
    public abstract class NpmInstallTaskBase : NCapsulateTask
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NpmInstall"/> is global.
        /// </summary>
        /// <value>
        ///   <c>true</c> if global; otherwise, <c>false</c>.
        /// </value>
        public bool Global { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NpmInstallTaskBase"/> is force.
        /// </summary>
        /// <value>
        ///   <c>true</c> if force; otherwise, <c>false</c>.
        /// </value>
        public bool Force { get; set; }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            var moduleResults = InstallPackages().Result.Where(r => r != ModuleInstallResult.AlreadyPresent);

            if (moduleResults.Contains(ModuleInstallResult.Error)) return false;

            if (!moduleResults.Any()) return true;

            Log.LogMessage(
                MessageImportance.High,
                String.Format(
                    CultureInfo.InvariantCulture,
                    "Installed {0} modules.  Flattening...",
                    moduleResults.Count()));

            if (!FlattenModulesAsync().Result) return false;

            return true;
        }

        /// <summary>
        /// Installs the packages.
        /// </summary>
        /// <returns></returns>
        public abstract Task<ModuleInstallResult[]> InstallPackages();

        /// <summary>
        /// The result of the module.
        /// </summary>
        public enum ModuleInstallResult
        {
            AlreadyPresent,

            Installed,

            Error
        }

        /// <summary>
        /// Installs the module asynchronous.
        /// </summary>
        /// <param name="cmdName">Name of the command.</param>
        /// <param name="moduleName">Name of the module.</param>
        /// <returns></returns>
        public async Task<ModuleInstallResult> InstallModuleAsync(string cmdName, string moduleName)
        {
            Log.LogMessage(MessageImportance.High, "npm install " + moduleName + " ...");

            var nodeDirectory = NodeDirectory;

            var npmCommand = String.Format(
                CultureInfo.InvariantCulture,
                @"/c {0}\npm.cmd install {1}{2}",
                NodeDirectory,
                moduleName,
                Global ? " -g" : String.Empty);

            var output = await ExecWithOutputAsync(@"cmd", npmCommand);

            if (output != null)
            {
                Log.LogError("npm install " + moduleName + " error: " + output);
                return ModuleInstallResult.Error;
            }

            return ModuleInstallResult.Installed;
        }

        /// <summary>
        /// Installs the modules asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<ModuleInstallResult> InstallModulesAsync()
        {
            Log.LogMessage(MessageImportance.High, "npm install ...");

            var nodeDirectory = NodeDirectory;

            var npmCommand = String.Format(
                CultureInfo.InvariantCulture,
                @"/c {0}\npm.cmd install",
                NodeDirectory);

            var output = await ExecWithOutputAsync(@"cmd", npmCommand);

            if (output != null)
            {
                Log.LogError("npm install error: " + output);
                return ModuleInstallResult.Error;
            }

            return ModuleInstallResult.Installed;
        }

        /// <summary>
        /// Updates the modules asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<ModuleInstallResult> UpdateModulesAsync()
        {
            Log.LogMessage(MessageImportance.High, "npm update ...");

            var nodeDirectory = NodeDirectory;

            var npmCommand = String.Format(
                CultureInfo.InvariantCulture,
                @"/c {0}\npm.cmd update",
                NodeDirectory);

            var output = await ExecWithOutputAsync(@"cmd", npmCommand);

            if (output != null)
            {
                Log.LogError("npm update error: " + output);
                return ModuleInstallResult.Error;
            }

            return ModuleInstallResult.Installed;
        }

        private async Task<bool> FlattenModulesAsync()
        {
            var nodeDirectory = NodeDirectory;

            var npmCommand = String.Format(
                CultureInfo.InvariantCulture,
                @"/c {0}\npm.cmd dedup{1}",
                nodeDirectory,
                Global ? " -g" : String.Empty);

            var output = await ExecWithOutputAsync(@"cmd", npmCommand);

            if (output != null)
            {
                Log.LogError("npm dedup error: " + output);

                return false;
            }

            if (!Global)
            {
                FlattenNodeModules(@"node_modules");
            }

            return true;
        }
    }
}
