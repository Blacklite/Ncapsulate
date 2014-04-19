using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Build.BuildEngine;

namespace Ncapsulate.Node.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class NCapsulateTask : CmdTask
    {
        /// <summary>
        /// Gets the node directory.
        /// </summary>
        /// <value>
        /// The node directory.
        /// </value>
        public string NodeDirectory
        {
            get
            {
                return _nodeDirectory ?? (_nodeDirectory = FindNodeDirectory());
            }
        }

        private string _nodeDirectory;

        private string FindNodeDirectory()
        {
            if (Directory.Exists("nodejs"))
            {
                if (File.Exists("nodejs\node.exe"))
                    return "nodejs";
                return @"..\Ncapsulate.Node\nodejs";
            }

            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            var nodeCmd = directory.EnumerateFiles("node.cmd", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (nodeCmd != null) return @".";

            while (directory != null)
            {
                var packagesDirectory =
                    directory.EnumerateDirectories("packages", SearchOption.TopDirectoryOnly).SingleOrDefault();

                if (packagesDirectory != null)
                {
                    var nodeExe =
                        packagesDirectory.EnumerateFiles("node.exe", SearchOption.AllDirectories)
                            .Reverse()
                            .FirstOrDefault();
                    if (nodeExe != null) return RelativePath(Directory.GetCurrentDirectory(), nodeExe.Directory.FullName);
                }

                directory = directory.Parent;
            }


            throw new FileNotFoundException("Could not find node.exe in any packages directory.");
        }

        private string RelativePath(string absolutePath, string relativeTo)
        {
            string[] absoluteDirectories = absolutePath.Split('\\');
            string[] relativeDirectories = relativeTo.Split('\\');

            //Get the shortest of the two paths
            int length = absoluteDirectories.Length < relativeDirectories.Length
                             ? absoluteDirectories.Length
                             : relativeDirectories.Length;

            //Use to determine where in the loop we exited
            int lastCommonRoot = -1;
            int index;

            //Find common root
            for (index = 0; index < length; index++)
                if (absoluteDirectories[index] == relativeDirectories[index]) lastCommonRoot = index;
                else break;

            //If we didn't find a common prefix then throw
            if (lastCommonRoot == -1) throw new ArgumentException("Paths do not have a common base");

            //Build up the relative path
            StringBuilder relativePath = new StringBuilder();

            //Add on the ..
            for (index = lastCommonRoot + 1; index < absoluteDirectories.Length; index++) if (absoluteDirectories[index].Length > 0) relativePath.Append("..\\");

            //Add on the folders
            for (index = lastCommonRoot + 1; index < relativeDirectories.Length - 1; index++) relativePath.Append(relativeDirectories[index] + "\\");
            relativePath.Append(relativeDirectories[relativeDirectories.Length - 1]);

            return relativePath.ToString();
        }

    }
}
