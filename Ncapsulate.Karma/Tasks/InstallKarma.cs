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

namespace Ncapsulate.Karma.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    public class InstallKarma : CmdTask
    {
        public override bool Execute()
        {
            if (Directory.Exists(@"nodejs\node_modules"))
                Directory.Delete(@"nodejs\node_modules", true);
            Directory.CreateDirectory(@"nodejs");
            // Force karma to install modules to the subdirectory
            // https://karmajs.org/doc/files/karma-folders.html#More-Information

            // We install our modules in this subdirectory so that
            // we can clean up their dependencies without catching
            // karma's modules, which we don't want.
            File.WriteAllText(@"nodejs\package.json", @"{""name"":""NCapsulate.Karma""}");

            // Since this is a synchronous job, I have
            // no choice but to synchronously wait for
            // the tasks to finish. However, the async
            // still saves threads.

            Task.WaitAll(
                this.DownloadKarmaAsync()
            );

            return true;
        }

        private async Task DownloadKarmaAsync()
        {
            var output = await ExecWithOutputAsync(@"cmd", @"/c ..\..\Ncapsulate.Node\nodejs\npm.cmd install karma-cli", @"nodejs");

            if (output != null)
            {
                this.Log.LogError("npm install karma error: " + output);
                throw new Exception("npm install karma error");
            }

            output = await ExecWithOutputAsync(@"cmd", @"/c ..\..\Ncapsulate.Node\nodejs\npm.cmd install karma", @"nodejs");

            if (output != null)
            {
                this.Log.LogError("npm install karma error: " + output);
                throw new Exception("npm install karma error");
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