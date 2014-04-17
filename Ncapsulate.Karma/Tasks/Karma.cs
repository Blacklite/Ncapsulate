using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Build.Framework;

using Ncapsulate.Node.Tasks;

namespace Ncapsulate.Karma.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    public class Karma : NCapsulateTask
    {
        public string ConfigFile { get; set; }

        public int Port { get; set; }

        public string LogLevel { get; set; }

        public string Reporters { get; set; }

        public string Browsers { get; set; }

        public int CaptureTimeout { get; set; }

        public int ReportSlowerThan { get; set; }

        /// <summary>
        /// Runs the task.
        /// </summary>
        /// <returns>
        /// true if successful; otherwise, false.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool Execute()
        {
            var cmd = String.Format(CultureInfo.InvariantCulture, @"/c {0}\karma start --single-run", this.NodeDirectory);

            if (this.ConfigFile != null) cmd += " " + this.ConfigFile;
            if (this.Port > 0) cmd += " --port " + this.Port;
            if (this.CaptureTimeout > 0) cmd += " --capture-timeout " + this.CaptureTimeout;
            if (this.ReportSlowerThan > 0) cmd += " --report-slower-than " + this.ReportSlowerThan;
            if (this.LogLevel != null) cmd += " --log-level " + this.LogLevel;
            if (this.Browsers != null) cmd += " --browsers " + this.Browsers;
            if (this.Reporters != null) cmd += " --reporters " + this.Reporters;

            var output = Task.WhenAll(ExecWithOutputResultAsync(@"cmd", cmd)).Result.FirstOrDefault();

            if (output.StartsWith("ERROR"))
            {
                this.Log.LogError("karma start - error: " + output);
                return false;
            }

            this.Log.LogMessage(MessageImportance.High, output);

            return true;
        }
    }
}
