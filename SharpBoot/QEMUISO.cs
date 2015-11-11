﻿using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading;
using SharpBoot.Properties;

namespace SharpBoot
{
    public class QEMUISO
    {
        public static void LaunchQemu(string iso, bool usb = false)
        {
            var f = Program.GetTemporaryDirectory();

            var floppy = Path.GetExtension(iso).ToLower() == ".img";

            var ext = new SevenZipExtractor();

            File.WriteAllBytes(Path.Combine(f, "qemutmp.7z"), Resources.qemu);

            ext.Extract(Path.Combine(f, "qemutmp.7z"), f);


            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };
            p.StartInfo.FileName = Path.Combine(f, "qemu.exe");
            p.StartInfo.WorkingDirectory = f;
            if (usb)
            {
                var logicalDiskId = iso.Substring(0, 2);
                var deviceId = string.Empty;
                var queryResults = new ManagementObjectSearcher(
                    $"ASSOCIATORS OF {{Win32_LogicalDisk.DeviceID='{logicalDiskId}'}} WHERE AssocClass = Win32_LogicalDiskToPartition");
                var partitions = queryResults.Get();
                foreach (var partition in partitions)
                {
                    queryResults = new ManagementObjectSearcher(
                        $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partition["DeviceID"]}'}} WHERE AssocClass = Win32_DiskDriveToDiskPartition");
                    var drives = queryResults.Get();
                    foreach (var drive in drives)
                        deviceId = drive["DeviceID"].ToString();
                }
                p.StartInfo.Arguments = " -L . -boot c  -drive file=" + deviceId +
                                        ",if=ide,index=0,media=disk -m 512 -localtime";
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.Verb = "runas";
            }
            else
            {
                p.StartInfo.Arguments = "-m 512 -localtime -M pc " + (floppy ? "-fda" : "-cdrom") + " \"" + iso + "\"";
            }
            Thread.Sleep(300);
            p.Start();
        }
    }
}