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
                            @"/c {0}\gulp {1} {2}{3}{4}",
                            this.NodeDirectory,
                            this.Tasks ?? "default",
                            this.GulpFile != null ? "--gulpfile " + this.GulpFile : String.Empty
                        ))).Result.FirstOrDefault();

            if (output.StartsWith("ERROR"))
            {
                this.Log.LogError("gulp run - error: " + output);
                return false;
            }

            return true;
        }
    }
}
