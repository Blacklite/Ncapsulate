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
    public class NpmUpdate : NpmInstallTaskBase
    {
        /// <summary>
        /// Installs the packages.
        /// </summary>
        /// <returns></returns>
        public override Task<ModuleInstallResult[]> InstallPackages()
        {
            return Task.WhenAll(this.UpdateModulesAsync());
        }
    }
}