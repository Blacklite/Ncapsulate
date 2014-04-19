using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

using Microsoft.Build.Framework;

namespace Ncapsulate.Node.Tasks
{
    class GetNpmVersion : NCapsulateTask
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [Output]
        public string Version { get; set; }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            var output =
                   Task.WhenAll(
                       ExecWithOutputResultAsync(
                           @"cmd",
                           String.Format(CultureInfo.InvariantCulture, @"/c {0}\npm -v", NodeDirectory)
                       )).Result.FirstOrDefault();

            if (output.StartsWith("ERROR"))
            {
                Log.LogError("npm version check - error: " + output);
                return false;
            }

            var version = output.TrimStart('v');
            var nugetVersion = Task.WhenAll(TaskHelpers.GetNugetVersion("npm")).Result.FirstOrDefault();
            version = TaskHelpers.GetNextVersion(version, nugetVersion);
            Log.LogMessage(MessageImportance.High, "Npm Version: ", output);

            Version = version;
            return true;
        }
    }
}
