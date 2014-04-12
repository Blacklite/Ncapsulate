using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Build.Framework;

namespace Ncapsulate.Node.Tasks
{
    public class GetNodeVersion : NCapsulateTask
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
                        String.Format(CultureInfo.InvariantCulture, @"/c {0}\node -v", NodeDirectory)
                    )).Result.FirstOrDefault();

            if (output.StartsWith("ERROR"))
            {
                Log.LogError("node version check - error: " + output);
                return false;
            }

            Log.LogMessage(MessageImportance.High, "Node Version: ", output);

            Version = output.TrimStart('v');
            return true;
        }
    }
}
