using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Build.Framework;

using Ncapsulate.Node.Tasks;

namespace Ncapsulate.Grunt.Tasks
{
    public class Grunt : NCapsulateTask
    {
        /// <summary>
        /// Gets or sets the grunt file.
        /// </summary>
        /// <value>
        /// The grunt file.
        /// </value>
        public string GruntFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Grunt"/> is force.
        /// </summary>
        /// <value>
        ///   <c>true</c> if force; otherwise, <c>false</c>.
        /// </value>
        public bool Force { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Grunt"/> is verbose.
        /// </summary>
        /// <value>
        ///   <c>true</c> if verbose; otherwise, <c>false</c>.
        /// </value>
        public bool Verbose { get; set; }

        /// <summary>
        /// Gets or sets the tasks.
        /// </summary>
        /// <value>
        /// The tasks.
        /// </value>
        public string Tasks { get; set; }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            var output = Task.WhenAll(
                       ExecWithOutputResultAsync(@"cmd", String.Format(
                            CultureInfo.InvariantCulture,
                            @"/c {0}\grunt {1} {2}{3}{4}",
                            this.NodeDirectory,
                            this.Tasks ?? "default",
                            this.GruntFile != null ? "--gruntfile " + this.GruntFile : String.Empty,
                            this.Verbose ? "--verbose " : String.Empty,
                            this.Force ? "--force" : String.Empty
                        ))).Result.FirstOrDefault();

            if (output.StartsWith("ERROR"))
            {
                this.Log.LogError("grunt run - error: " + output);
                return false;
            }

            Log.LogMessage(MessageImportance.High, output);
            return true;
        }
    }
}
