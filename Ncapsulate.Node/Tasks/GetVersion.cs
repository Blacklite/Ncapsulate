using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

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
            string version = json.version;

            // Allow for subversions, it's possible we want to implement a new feature, task, or fix
            //    and the library (node, npm, bower, etc) has not be reved since the last push.
            // This allows us to keep our version consistent with the current release of the tool we're wrapping.
            var nugetVersion = Task.WhenAll(TaskHelpers.GetNugetVersion(Name)).Result.FirstOrDefault();
            version = TaskHelpers.GetNextVersion(version, nugetVersion);

            this.Log.LogMessage(MessageImportance.High, Name + @" version: ", version);

            this.Version = version;
            return true;
        }
    }
}
