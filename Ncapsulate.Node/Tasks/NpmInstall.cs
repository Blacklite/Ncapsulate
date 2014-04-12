/***
 * These tasks are based on code used by the wonderful Web Essentials Extension.
 * No ownership is claimed, the relivante pieces of code are to be associated with the Web Essentials Extension.
 * https://raw.githubusercontent.com/madskristensen/WebEssentials2013/master/LICENSE.txt
 ***/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Build.Framework;

namespace Ncapsulate.Node.Tasks
{
    /// <summary>
    /// Install a specific node module
    /// Requires a the cmd executable name (to see if it's already installed)
    /// And the module name to install from npm.
    /// 
    /// This is like install a dependency from the console "npm install {0}"
    /// </summary>
    public class NpmInstall : NpmInstallTaskBase
    {
        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value
        public string Cmd { get; set; }

        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        /// <value>
        /// The name of the module.
        /// </value>
        public string Module { get; set; }
    
        /// <summary>
        /// Installs the packages.
        /// </summary>
        /// <returns></returns>
        public override Task<ModuleInstallResult[]> InstallPackages()
        {
            if (Module == null)
                return Task.WhenAll(InstallModulesAsync());

            if (Cmd == null) Cmd = Module;
            return Task.WhenAll(this.InstallModuleAsync(Cmd, Module));
        }
    }
}