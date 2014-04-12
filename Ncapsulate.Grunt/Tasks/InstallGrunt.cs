/***
 * These tasks are based on code used by the wonderful Web Essentials Extension.
 * No ownership is claimed, the relivante pieces of code are to be associated with the Web Essentials Extension.
 * https://raw.githubusercontent.com/madskristensen/WebEssentials2013/master/LICENSE.txt
 ***/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ncapsulate.Node.Tasks;

namespace Ncapsulate.Grunt.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    public class InstallGrunt : CmdTask
    {
        public override bool Execute()
        {
            if (Directory.Exists(@"nodejs\node_modules"))
                Directory.Delete(@"nodejs\node_modules", true);
            Directory.CreateDirectory(@"nodejs");
            // Force grunt to install modules to the subdirectory
            // https://gruntjs.org/doc/files/grunt-folders.html#More-Information

            // We install our modules in this subdirectory so that
            // we can clean up their dependencies without catching
            // grunt's modules, which we don't want.
            File.WriteAllText(@"nodejs\package.json", @"{""name"":""NCapsulate.Grunt""}");

            // Since this is a synchronous job, I have
            // no choice but to synchronously wait for
            // the tasks to finish. However, the async
            // still saves threads.

            Task.WaitAll(
                this.DownloadGruntAsync()
            );

            return true;
        }

        private async Task DownloadGruntAsync()
        {
            var output = await ExecWithOutputAsync(@"cmd", @"/c ..\..\Ncapsulate.Node\nodejs\npm.cmd install grunt-cli", @"nodejs");

            if (output != null)
            {
                this.Log.LogError("npm install grunt-cli error: " + output);
                throw new Exception("npm install grunt-cli error");
            }

            output = await ExecWithOutputAsync(@"cmd", @"/c ..\..\Ncapsulate.Node\nodejs\npm.cmd install grunt", @"nodejs");

            if (output != null)
            {
                this.Log.LogError("npm install grunt error: " + output);
                throw new Exception("npm install grunt error");
            }

            output = await ExecWithOutputAsync(@"cmd", @"/c ..\..\Ncapsulate.Node\nodejs\npm.cmd dedup", @"nodejs");

            if (output != null)
            {
                this.Log.LogError("npm dedup error: " + output);
            }

            this.FlattenNodeModules(@"nodejs");
        }
    }
}