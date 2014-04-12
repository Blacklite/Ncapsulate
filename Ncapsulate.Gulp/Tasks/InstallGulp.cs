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

namespace Ncapsulate.Gulp.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    public class InstallGulp : CmdTask
    {
        public override bool Execute()
        {
            if (Directory.Exists(@"nodejs\node_modules"))
                Directory.Delete(@"nodejs\node_modules", true);
            Directory.CreateDirectory(@"nodejs");
            // Force gulp to install modules to the subdirectory
            // https://gulpjs.org/doc/files/gulp-folders.html#More-Information

            // We install our modules in this subdirectory so that
            // we can clean up their dependencies without catching
            // gulp's modules, which we don't want.
            File.WriteAllText(@"nodejs\package.json", @"{""name"":""NCapsulate.Gulp""}");  

            // Since this is a synchronous job, I have
            // no choice but to synchronously wait for
            // the tasks to finish. However, the async
            // still saves threads.

            Task.WaitAll(
                this.DownloadGulpAsync()
            );

            return true;
        }

        private async Task DownloadGulpAsync()
        {
            var output = await ExecWithOutputAsync(@"cmd", @"/c ..\..\Ncapsulate.Node\nodejs\npm.cmd install gulp", @"nodejs");

            if (output != null)
            {
                this.Log.LogError("npm install gulp error: " + output);
                throw new Exception("npm install gulp error");
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