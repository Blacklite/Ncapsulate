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

namespace Ncapsulate.Bower.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    public class InstallBower : CmdTask
    {
        public override bool Execute()
        {
            if (Directory.Exists(@"nodejs\node_modules"))
                Directory.Delete(@"nodejs\node_modules", true);
            Directory.CreateDirectory(@"nodejs");
            // Force bower to install modules to the subdirectory
            // https://bowerjs.org/doc/files/bower-folders.html#More-Information

            // We install our modules in this subdirectory so that
            // we can clean up their dependencies without catching
            // bower's modules, which we don't want.
            File.WriteAllText(@"nodejs\package.json", @"{""name"":""NCapsulate.Bower""}");

            // Since this is a synchronous job, I have
            // no choice but to synchronously wait for
            // the tasks to finish. However, the async
            // still saves threads.

            Task.WaitAll(
                this.DownloadBowerAsync()
            );

            return true;
        }

        private async Task DownloadBowerAsync()
        {
            var output = await ExecWithOutputAsync(@"cmd", @"/c ..\..\Ncapsulate.Node\nodejs\npm.cmd install bower", @"nodejs");

            if (output != null)
            {
                this.Log.LogError("npm install bower error: " + output);
                throw new Exception("npm install bower error");
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