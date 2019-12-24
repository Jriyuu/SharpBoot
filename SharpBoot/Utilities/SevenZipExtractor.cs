﻿using System;
using System.Diagnostics;
using System.IO;
using SharpBoot.Properties;

namespace SharpBoot.Utilities
{
    public class SevenZipExtractor : IDisposable
    {
        public string SevenZipPath;

        public SevenZipExtractor()
        {
            var d = Utils.GetTemporaryDirectory();
            SevenZipPath = Path.Combine(d, "7za.exe");
            File.WriteAllBytes(SevenZipPath, Resources._7za);
        }

        public event EventHandler ExtractionFinished;

        protected void OnFinished()
        {
            ExtractionFinished?.Invoke(this, EventArgs.Empty);
        }

        public void Close()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Utils.SafeDel(Path.GetDirectoryName(SevenZipPath));
        }

        public void Extract(string arch, string output, bool wait = true, int maxDelay = 5)
        {
            var p = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = SevenZipPath,
                    Arguments = "x \"" + arch + "\" -o\"" + output + "\""
                }
            };

            p.Exited += delegate { OnFinished(); };
            var sp = new Stopwatch();
            sp.Start();
            p.Start();

            if (wait)
                while (!p.HasExited)
                    if (sp.Elapsed.Seconds > maxDelay)
                        break;
        }

        public void Dispose()
        {
            Close();
        }
    }
}