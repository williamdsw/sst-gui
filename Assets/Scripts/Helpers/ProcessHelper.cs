using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Utilities;

namespace Helpers
{
    public static class ProcessHelper
    {
        private static List<OutputItem> outputItems;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static List<OutputItem> Execute(List<string> arguments)
        {
            try
            {
                outputItems = new List<OutputItem>();

                using (Process process = new())
                {
#if UNITY_EDITOR
                    process.StartInfo.FileName = Properties.SstStreamingAssetsPath;
#else
                    process.StartInfo.FileName = Properties.SstPath;
#endif
                    process.StartInfo.Arguments = String.Join(" ", arguments);
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                    process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                }

                return outputItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OutputHandler(object sender, DataReceivedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Data) && !string.IsNullOrWhiteSpace(args.Data))
            {
                // TODO
                bool isValid = !args.Data.Equals("Faield to load save file!") && args.Data.IndexOf("Not a SSAVERAW file!") == -1;
                outputItems.Add(new OutputItem()
                {
                    IsValid = isValid,
                    Content = args.Data
                });
            }
        }
    }
}

