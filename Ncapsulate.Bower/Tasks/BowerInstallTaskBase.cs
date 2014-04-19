/***
 * These tasks are based on code used by the wonderful Web Essentials Extension.
 * No ownership is claimed, the relivante pieces of code are to be associated with the Web Essentials Extension.
 * https://raw.githubusercontent.com/madskristensen/WebEssentials2013/master/LICENSE.txt
 ***/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Build.Framework;

using Ncapsulate.Node.Tasks;

namespace Ncapsulate.Bower.Tasks
{
    /// <summary>
    /// This is not compiled (ItemType=None) but is invoked by the Inline Task (http://msdn.microsoft.com/en-us/library/dd722601) in the csproj file.
    /// </summary>
    public abstract class BowerInstallTaskBase : NcapsulateTask
    {
        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            var moduleResults = this.InstallPackages().Result.Where(r => r != ModuleInstallResult.AlreadyPresent);

            if (moduleResults.Contains(ModuleInstallResult.Error)) return false;

            if (!moduleResults.Any()) return true;

            this.Log.LogMessage(
                MessageImportance.High,
                String.Format(
                    CultureInfo.InvariantCulture,
                    "Installed {0} modules.  Flattening...",
                    moduleResults.Count()));

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
        public async Task<ModuleInstallResult> InstallModuleAsync(string moduleName)
        {
            this.Log.LogMessage(MessageImportance.High, "bower install " + moduleName + " ...");

            var nodeDirectory = this.NodeDirectory;

            this.Log.LogMessage(MessageImportance.High,  Directory.GetCurrentDirectory());

            var bowerCommand = String.Format(
                CultureInfo.InvariantCulture,
                @"/c {0}\bower install {1}{2}",
                this.NodeDirectory, // We drop a cmd file that finds the correct node.exe, and also in the case of bower, finds the correct .bin\bower
                moduleName);

            var output = await ExecWithOutputResultAsync(@"cmd", bowerCommand);

            if (output.StartsWith("ERROR"))
            {
                this.Log.LogError("bower install " + moduleName + " error: " + output);
                return ModuleInstallResult.Error;
            }

            Log.LogMessage(MessageImportance.High, output);
            return ModuleInstallResult.Installed;
        }

        /// <summary>
        /// Installs the modules asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<ModuleInstallResult> InstallModulesAsync()
        {
            this.Log.LogMessage(MessageImportance.High, "bower install ...");

            var nodeDirectory = this.NodeDirectory;

            this.Log.LogMessage(MessageImportance.High, Directory.GetCurrentDirectory());

            var bowerCommand = String.Format(
                CultureInfo.InvariantCulture,
                @"/c {0}\bower.cmd install",
                this.NodeDirectory);

            var output = await ExecWithOutputResultAsync(@"cmd", bowerCommand);

            if (output.StartsWith("ERROR"))
            {
                this.Log.LogError("bower install error: " + output);
                return ModuleInstallResult.Error;
            }

            Log.LogMessage(MessageImportance.High, output);
            return ModuleInstallResult.Installed;
        }

        /// <summary>
        /// Updates the modules asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<ModuleInstallResult> UpdateModulesAsync()
        {
            Log.LogMessage(MessageImportance.High, "bower update ...");

            var nodeDirectory = NodeDirectory;

            var bowerCommand = String.Format(
                CultureInfo.InvariantCulture,
                @"/c {0}\bower.cmd update",
                NodeDirectory);

            var output = await ExecWithOutputResultAsync(@"cmd", bowerCommand);

            if (output.StartsWith("ERROR"))
            {
                Log.LogError("bower update error: " + output);
                return ModuleInstallResult.Error;
            }

            Log.LogMessage(MessageImportance.High, output);
            return ModuleInstallResult.Installed;
        }
    }
}
