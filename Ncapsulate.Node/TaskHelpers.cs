using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Microsoft.Build.Framework;

namespace Ncapsulate.Node
{
    public static class TaskHelpers
    {
        private const string _NugetVersionLocationFormat =
            "https://www.nuget.org/api/v2/FindPackagesById()?id='ncapsulate.{0}'&$filter=IsLatestVersion";

        public static async Task<string> GetNugetVersion(string name)
        {
            var url = String.Format(_NugetVersionLocationFormat, name);
            var request = WebRequest.CreateHttp(url);
            var response = await request.GetResponseAsync();
            var webResponse = response as HttpWebResponse;

            if (webResponse != null && webResponse.StatusCode == HttpStatusCode.OK)
            {
                var xml = XDocument.Load(webResponse.GetResponseStream());
                var version = xml.Descendants().SingleOrDefault(x => x.Name.LocalName == "Version");
                return version.Value;
            }

            return null;
        }

        public static string GetNextVersion(string version, string nugetVersion)
        {
            if (nugetVersion != null && nugetVersion.StartsWith(version))
            {
                if (nugetVersion.Trim() == version.Trim())
                {
                    version += ".1";
                }
                else
                {
                    var currentSubversion = nugetVersion.Substring(nugetVersion.LastIndexOf('.') + 1);
                    var intSubversion = int.Parse(currentSubversion) + 1;
                    version = version + "." + intSubversion;
                }
            }
            return version;
        }
    }
}
