using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ncapsulate.Node.Tasks;

namespace Ncapsulate.Gulp.Tasks
{
    public class Gulp : NCapsulateTask
    {
        /// <summary>
        /// Gets or sets the gulp file.
        /// </summary>
        /// <value>
        /// The gulp file.
        /// </value>
        public string GulpFile { get; set; }

        /// <summary>
        /// Gets or sets the tasks.
        /// </summary>
        /// <value>
        /// The tasks.
        /// </value>
        public string Tasks { get; set; }

        /// <summary>
        /// Gets or sets the tasks.
        /// </summary>
        /// <value>
        /// The tasks.
        /// </value>
        public string Params { get; set; }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            var cmd = String.Format(CultureInfo.InvariantCulture, @"/c {0}\gulp ", this.NodeDirectory);

            cmd += this.Tasks ?? "default";
            if (this.GulpFile != null) cmd += " --gulpfile " + this.GulpFile;
            if (this.Params != null) cmd += " " + this.Params;

            var output = Task.WhenAll(ExecWithOutputResultAsync(@"cmd", cmd)).Result.FirstOrDefault();

            if (output.StartsWith("ERROR"))
            {
                this.Log.LogError("gulp run - error: " + output);
                return false;
            }

            Log.LogMessage(MessageImportance.High, output);
            return true;
        }
    }
}
