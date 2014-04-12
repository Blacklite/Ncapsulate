using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

using Microsoft.Build.Framework;

using Ncapsulate.Node.Tasks;

namespace Ncapsulate.Karma.Tasks
{
    public class GetKarmaVersion : NCapsulateTask
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
            var json = Json.Decode(File.ReadAllText(@".\nodejs\node_modules\karma\package.json"));
            var version = json.version;

            this.Log.LogMessage(MessageImportance.High, "karma Version: ", version);

            this.Version = version;
            return true;
        }
    }
}
