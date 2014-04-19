using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Build.Framework;

namespace Ncapsulate.Node.Tasks
{
    public class InstallModule : CmdTask
    {
        /// <summary>
        /// Gets or sets the modules.
        /// Each module is seperated by a space (much like npm install)
        /// </summary>
        /// <value>
        /// The modules.
        /// </value>
        [Required]
        public string Modules { get; set; }

        public override bool Execute()
        {
            if (Directory.Exists(@"nodejs\node_modules"))
                Directory.Delete(@"nodejs\node_modules", true);
            Directory.CreateDirectory(@"nodejs");

            // We install our modules in this subdirectory so that
            // we can clean up their dependencies without catching
            // bower's modules, which we don't want.
            File.WriteAllText(@"nodejs\package.json", @"{}");

            // Since this is a synchronous job, I have
            // no choice but to synchronously wait for
            // the tasks to finish. However, the async
            // still saves threads.

            Task.WaitAll(
                this.InstallModulesAsync()
            );

            return true;
        }

        private async Task InstallModulesAsync()
        {
            var output = await ExecWithOutputAsync(@"cmd", @"/c ..\..\Ncapsulate.Node\nodejs\npm.cmd install " + this.Modules, @"nodejs");

            if (output != null)
            {
                this.Log.LogError("npm install " + this.Modules + " error: " + output);
                throw new Exception("npm install " + this.Modules + " error");
            }

            output = await ExecWithOutputAsync(@"cmd", @"/c ..\..\Ncapsulate.Node\nodejs\npm.cmd dedup", @"nodejs");

            if (output != null)
            {
                this.Log.LogError("npm dedup error: " + output);
            }

            FlattenNodeModules(@"nodejs");
        }
    }
}
