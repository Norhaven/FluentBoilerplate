using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics;
    using System.IO;
    using System.Configuration;
    using Microsoft.Build.Utilities;

    [TestClass]
    public class BasePEVerifyTest
    {
        public static string PeVerifyPath { get; set; }

        [TestInitialize]
        [Conditional("PEVERIFY")]
        public void FindPeVerifySetUp()
        {
            var frameworkSdk = ToolLocationHelper.GetPathToDotNetFrameworkSdk(TargetDotNetFrameworkVersion.Version45);
            var file = Path.Combine(frameworkSdk, @"bin\NETFX 4.0 Tools\PEVerify.exe");
            if (!File.Exists(file))
                throw new FileNotFoundException("Could not locate PEVerify.exe, please check the path");

            PeVerifyPath = file;
        }

        protected void RunPEVerifyOnAssembly(string assemblyPath)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = PeVerifyPath,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    Arguments = "\"" + assemblyPath + "\" /VERBOSE",
                    CreateNoWindow = true
                }
            };
            process.Start();
            var processOutput = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var result = process.ExitCode + " code ";

            Console.WriteLine(GetType().FullName + ": " + result);

            if (process.ExitCode != 0)
            {
                Console.WriteLine(processOutput);
                Assert.Fail("PeVerify reported error(s): " + Environment.NewLine + processOutput, result);
            }
        }
    }
}
