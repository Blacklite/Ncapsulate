using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

using Microsoft.Build.Framework;

namespace Ncapsulate.Node.Tasks
{
    public class GetVersion : NCapsulateTask
    {
        /// <summary>
        /// Gets or sets the name to get the version for.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required]
        public string Name { get; set; }

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
            var json = Json.Decode(File.ReadAllText(@".\nodejs\node_modules\" + Name + @"\package.json"));
            var version = json.version;

            this.Log.LogMessage(MessageImportance.High, Name + @" version: ", version);

            this.Version = version;
            return true;
        }
    }
}
