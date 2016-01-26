﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using SharpBoot.Properties;

namespace SharpBoot
{
    public abstract class IBootloader
    {
        public abstract string GetCode(BootMenu menu);
        public abstract string GetCode(BootMenuItem item);
        public abstract void SetImage(Image img, Size sz);
        public abstract string BinFile { get; set; }
        public abstract byte[] Archive { get; set; }
        public abstract string FolderName { get; set; }
        public abstract string DisplayName { get; set; }
        public abstract string FileExt { get; set; }
        public string WorkingDir { get; set; }
        public abstract string CmdArgs { get; set; }
        public Size Resolution { get; set; } = new Size(640, 480);
        public abstract bool SupportAccent { get; set; }
        public abstract long TotalSize { get; set; }
    }

    public class Syslinux : IBootloader
    {
        public override string CmdArgs { get; set; } = " -iso-level 4 ";
        public override string FolderName { get; set; } = "syslinux";
        public override string DisplayName { get; set; } = "Syslinux";

        public override string FileExt { get; set; } = ".cfg";

        public override bool SupportAccent { get; set; } = true;

        public override string GetCode(BootMenu menu)
        {
            var code = "";

            code += "INCLUDE /boot/syslinux/theme.cfg\n";
            code += "MENU title " + menu.Title.RemoveAccent() + "\n";


            if (menu.MainMenu)
                code += "TIMEOUT 100\n";
            else
                code += "### MENU START\n" +
                        "LABEL mainmenu\n" +
                        "MENU LABEL " + Strings.MainMenu.RemoveAccent() + "\n" +
                        "KERNEL /boot/syslinux/vesamenu.c32\n" +
                        "APPEND /boot/syslinux/syslinux.cfg\n" +
                        "### MENU END\n";

            menu.Items.ForEach(x => code += GetCode(x));

            return code;
        }

        public override string GetCode(BootMenuItem item)
        {
            if (item.CustomCode != "") return item.CustomCode;

            var code = "";

            code += "LABEL -\n";
            switch (item.Type)
            {
                case EntryType.BootHDD:
                    code += "localboot 0x80\n";
                    break;
                case EntryType.Category:
                    code += "KERNEL /boot/syslinux/vesamenu.c32\n";
                    code += "APPEND /boot/syslinux/" + item.IsoName + ".cfg\n";
                    break;
                case EntryType.ISO:
                    code += "LINUX /boot/syslinux/grub.exe\n";
                    code +=
                        string.Format(
                            "APPEND --config-file=\"ls /images/{0} || find --set-root /images/{0};map /images/{0} (0xff);map --hook;root (0xff);chainloader (0xff);boot\"\n",
                            item.IsoName);
                    break;
                case EntryType.IMG:
                    code += "LINUX /boot/syslinux/grub.exe\n";
                    code +=
                        string.Format(
                            "APPEND --config-file=\"ls /images/{0} || find --set-root /images/{0};map /images/{0} (fd0);map --hook;chainloader (fd0)+1;rootnoverify (fd0);boot\"\n",
                            item.IsoName);
                    break;
                case EntryType.NTLDR:
                    code += "COM32 /boot/syslinux/chain.c32\n";
                    code += "APPEND ntldr=/images/" + item.IsoName + "\n";
                    break;
                case EntryType.GRLDR:
                    code += "COM32 /boot/syslinux/chain.c32\n";
                    code += "APPEND grldr=/images/" + item.IsoName + "\n";
                    break;
                case EntryType.CMLDR:
                    code += "COM32 /boot/syslinux/chain.c32\n";
                    code += "APPEND cmldr=/images/" + item.IsoName + "\n";
                    break;
                case EntryType.FreeDOS:
                    code += "COM32 /boot/syslinux/chain.c32\n";
                    code += "APPEND freedos=/images/" + item.IsoName + "\n";
                    break;
                case EntryType.MS_DOS:
                    code += "COM32 /boot/syslinux/chain.c32\n";
                    code += "APPEND msdos=/images/" + item.IsoName + "\n";
                    break;
                case EntryType.MS_DOS_7:
                    code += "COM32 /boot/syslinux/chain.c32\n";
                    code += "APPEND msdos7=/images/" + item.IsoName + "\n";
                    break;
                case EntryType.PC_DOS:
                    code += "COM32 /boot/syslinux/chain.c32\n";
                    code += "APPEND pcdos=/images/" + item.IsoName + "\n";
                    break;
                case EntryType.DRMK:
                    code += "COM32 /boot/syslinux/chain.c32\n";
                    code += "APPEND drmk=/images/" + item.IsoName + "\n";
                    break;
                case EntryType.ReactOS:
                    code += "COM32 /boot/syslinux/chain.c32\n";
                    code += "APPEND reactos=/images/" + item.IsoName + "\n";
                    break;
            }

            code += "MENU LABEL " + item.Name.RemoveAccent() + "\n";

            if (item.Start)
            {
                code += "MENU START\n";
                code += "MENU DEFAULT\n";
            }

            code += "TEXT HELP\n";
            code += splitwidth(item.Description.RemoveAccent(), 78) + "\n";
            code += "ENDTEXT\n";

            return code;
        }

        public override void SetImage(Image image, Size sz)
        {
            if (image == null) return;

            var width = sz.Width;
            var height = sz.Height;

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height, PixelFormat.Format16bppRgb555);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            destImage.Save(Path.Combine(WorkingDir, "sharpboot.jpg"), ImageFormat.Jpeg);
        }

        public override string BinFile { get; set; } = "boot/syslinux/isolinux.bin";
        public override byte[] Archive { get; set; } = Resources.syslinux1;
        public override long TotalSize { get; set; } = 1064874;

        private static string splitwidth(string s, int w)
        {
            return string.Join("\n", s.Wrap(w));
        }
    }

    public class Grub4DOS : IBootloader
    {
        public override string DisplayName { get; set; } = "Grub4DOS";
        public override string FileExt { get; set; } = ".lst";
        public override string CmdArgs { get; set; } = "";
        public override string FolderName { get; set; } = "grub4dos";
        public override bool SupportAccent { get; set; } = false;

        public override string GetCode(BootMenu menu)
        {
            var code = "";

            //code += "color magenta/white white/magenta black/white black/white\n";

            if (noback) code += $"graphicsmode -1 {Resolution.Width} {Resolution.Height} 32\n";
            else code += "splashimage /boot/grub4dos/sharpboot.xpm.lzma\n";

            if (menu.MainMenu)
            {
                code += "timeout 100\n\n";
            }
            else code += "title " + Strings.MainMenu.RemoveAccent().Trim() + "\nconfigfile /menu.lst\n\n";


            menu.Items.ForEach(x => code += GetCode(x));

            return code;
        }

        public override string GetCode(BootMenuItem item)
        {
            if (item.CustomCode != "") return item.CustomCode;

            var code = "";

            code += "title " + item.Name.RemoveAccent() + "\\n";

            code += item.Description.RemoveAccent().Trim().Replace("\r\n", "\n").Replace("\n", "\\n") + "\n";

            switch (item.Type)
            {
                case EntryType.BootHDD:
                    code += "map (hd0) (hd1)\n";
                    code += "map (hd1) (hd0)\n";
                    code += "map --hook\n";
                    code += "chainloader (hd0,0)\n";
                    break;
                case EntryType.Category:
                    code += "configfile /boot/grub4dos/" + item.IsoName + ".lst\n";
                    break;
                case EntryType.ISO:
                    code +=
                        string.Format(
                            "ls /images/{0} || find --set-root /images/{0}\nmap /images/{0} (0xff)\nmap --hook\nroot (0xff)\nchainloader (0xff)\n",
                            item.IsoName);
                    break;
                case EntryType.IMG:
                    code +=
                        string.Format(
                            "ls /images/{0} || find --set-root /images/{0}\nmap /images/{0} (fd0)\nmap --hook\nchainloader (fd0)+1\nrootnoverify (fd0)\n",
                            item.IsoName);
                    break;
                case EntryType.NTLDR:
                case EntryType.GRLDR:
                case EntryType.CMLDR:
                case EntryType.FreeDOS:
                case EntryType.MS_DOS:
                case EntryType.MS_DOS_7:
                case EntryType.PC_DOS:
                case EntryType.DRMK:
                case EntryType.ReactOS:
                    code += string.Format(
                        "ls /images/{0} || find --set-root /Images/{0}\nchainloader /images/{0}\n",
                        item.IsoName);
                    break;
            }

            code += "\n";

            return code;
        }

        private bool noback;

        public override void SetImage(Image image, Size sz)
        {
            if (image == null)
            {
                noback = true;
                return;
            }

            var width = sz.Width;
            var height = sz.Height;

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height, PixelFormat.Format16bppRgb555);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            var imgf = Path.Combine(WorkingDir, "sharpboot.bmp");

            destImage.Save(imgf, ImageFormat.Bmp);

            var convertdir = Path.Combine(WorkingDir, "imagemagick");
            Directory.CreateDirectory(convertdir);
            File.WriteAllBytes(Path.Combine(convertdir, "convert.7z"), Resources.imagemagick);

            var ext = new SevenZipExtractor();
            ext.Extract(Path.Combine(convertdir, "convert.7z"), convertdir);


            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    FileName = Path.Combine(convertdir, "convert.exe"),
                    CreateNoWindow = true,
                    WorkingDirectory = convertdir
                }
            };
            p.StartInfo.Arguments += " ../sharpboot.bmp ../sharpboot.xpm.lzma";
            Thread.Sleep(300);
            var begin = DateTime.Now;
            while(!File.Exists(p.StartInfo.FileName))
            {
                if ((DateTime.Now - begin).TotalSeconds > 15)
                {
                    break;
                }
            }
            begin = DateTime.Now;
            p.Start();
            while (!File.Exists(Path.Combine(WorkingDir, "sharpboot.xpm.lzma")))
            {
                if (p.HasExited)
                {
                    p.Start();
                    continue;
                }
                if ((DateTime.Now - begin).TotalSeconds > 15)
                {
                    break;
                }
            }
            Thread.Sleep(1000);
            File.Delete(imgf);
            ext.Close();
            while (Directory.Exists(convertdir))
            {
                try
                {
                    Directory.Delete(convertdir, true);
                }
                catch
                {
                }
            }
        }

        public override string BinFile { get; set; } = "grldr";
        public override byte[] Archive { get; set; } = Resources.grub4dos;

        public override long TotalSize { get; set; } = 280911;
    }

    public interface IBootloaderTheme
    {
        Size Resolution { get; set; }

        string GetCode();
    }

    public class SyslinuxTheme
    {
        public int Margin { get; set; } = 4;
        public Size Resolution { get; set; } = new Size(640, 480);

        public bool noback = false;

        public enum ShadowType
        {
            /// <summary>
            ///     No shadowing
            /// </summary>
            none,

            /// <summary>
            ///     Standard shadowing -- foreground pixels are raised
            /// </summary>
            std,

            /// <summary>
            ///     Both background and foreground raised
            /// </summary>
            all,

            /// <summary>
            ///     Background pixels are raised
            /// </summary>
            rev
        }

        public string GetCode()
        {
            return "prompt 0\n" +
                   "UI /boot/syslinux/vesamenu.c32\n" +
                   "menu resolution " + Resolution.Width + " " + Resolution.Height + "\n" +
                   "menu margin " + Margin + "\n" +
                   (noback ? "" : "menu background /boot/syslinux/sharpboot.jpg\n") +
                   string.Concat(Entries.Select(x => x.GetCode())) +
                   "menu helpmsgrow 19\n" +
                   "menu helpmsendrow -1\n" +
                   (Program.UseCyrillicFont ? "FONT /boot/syslinux/cyrillic_cp866.psf\n" : "");
        }

        public List<SLTEntry> Entries = new List<SLTEntry>
        {
            new SLTEntry("screen", "#80ffffff", "#00000000", ShadowType.std),
            new SLTEntry("border", "#ffffffff", "#ee000000", ShadowType.std),
            new SLTEntry("title", "#ffffffff", "#ee000000", ShadowType.std),
            new SLTEntry("unsel", "#ffffffff", "#ee000000", ShadowType.std),
            new SLTEntry("hotkey", "#ff00ff00", "#ee000000", ShadowType.std),
            new SLTEntry("sel", "#ffffffff", "#85000000", ShadowType.std),
            new SLTEntry("hotsel", "#ffffffff", "#85000000", ShadowType.std),
            new SLTEntry("disabled", "#60cccccc", "#00000000", ShadowType.std),
            new SLTEntry("scrollbar", "#40000000", "#00000000", ShadowType.std),
            new SLTEntry("tabmsg", "#90ffff00", "#00000000", ShadowType.std),
            new SLTEntry("cmdmark", "#c000ffff", "#00000000", ShadowType.std),
            new SLTEntry("cmdline", "#c0ffffff", "#00000000", ShadowType.std),
            new SLTEntry("pwdborder", "#80ffffff", "#20ffffff", ShadowType.rev),
            new SLTEntry("pwdheader", "#80ff8080", "#20ffffff", ShadowType.rev),
            new SLTEntry("pwdentry", "#80ffffff", "#20ffffff", ShadowType.rev),
            new SLTEntry("timeout_msg", "#80ffffff", "#00000000", ShadowType.std),
            new SLTEntry("timeout", "#c0ffffff", "#00000000", ShadowType.std),
            new SLTEntry("help", "#c0ffffff", "#00000000", ShadowType.std)
        };

        // SLTEntry = SysLinux Theme Entry
        public class SLTEntry
        {
            public SLTEntry(string name, Color foreground, Color background, ShadowType shadowType = ShadowType.none)
            {
                Name = name;
                Foreground = foreground;
                Background = background;
                ShadowType = shadowType;
            }

            public SLTEntry(string name, string foreground, string background, ShadowType shadowType = ShadowType.none)
                : this(name, ColorTranslator.FromHtml(foreground), ColorTranslator.FromHtml(background), shadowType)
            {
            }

            public string Name { get; set; }
            public Color Foreground { get; set; }
            public Color Background { get; set; }

            public ShadowType ShadowType { get; set; }

            public string GetCode()
            {
                return $"menu color {Name} 0 {Foreground.ToHexArgb()} {Background.ToHexArgb()} {ShadowType}\n";
            }
        }
    }

    public enum Bootloaders
    {
        Syslinux = 0,
        Grub4DOS = 1,
        Grub2 = 2
    }

    public class BootloaderInst
    {
        public static void Install(string l, int bl)
        {
            Install(l, (Bootloaders) bl);
        }

        public static void Install(string l, string bl)
        {
            if (bl == "grub4dos") Install(l, Bootloaders.Grub4DOS);
            if (bl == "syslinux") Install(l, Bootloaders.Syslinux);
        }

        public static void Install(string l, Bootloaders bl)
        {
            if (bl == Bootloaders.Grub2)
            {
                var grub2_mbr = new byte[]
                {
                    0xEB, 0x63, 0x90, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x01, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0xFF, 0xFA, 0x90, 0x90, 0xF6, 0xC2, 0x80, 0x74,
                    0x05, 0xF6, 0xC2, 0x70, 0x74, 0x02, 0xB2, 0x80, 0xEA, 0x79, 0x7C, 0x00,
                    0x00, 0x31, 0xC0, 0x8E, 0xD8, 0x8E, 0xD0, 0xBC, 0x00, 0x20, 0xFB, 0xA0,
                    0x64, 0x7C, 0x3C, 0xFF, 0x74, 0x02, 0x88, 0xC2, 0x52, 0xBE, 0x80, 0x7D,
                    0xE8, 0x17, 0x01, 0xBE, 0x05, 0x7C, 0xB4, 0x41, 0xBB, 0xAA, 0x55, 0xCD,
                    0x13, 0x5A, 0x52, 0x72, 0x3D, 0x81, 0xFB, 0x55, 0xAA, 0x75, 0x37, 0x83,
                    0xE1, 0x01, 0x74, 0x32, 0x31, 0xC0, 0x89, 0x44, 0x04, 0x40, 0x88, 0x44,
                    0xFF, 0x89, 0x44, 0x02, 0xC7, 0x04, 0x10, 0x00, 0x66, 0x8B, 0x1E, 0x5C,
                    0x7C, 0x66, 0x89, 0x5C, 0x08, 0x66, 0x8B, 0x1E, 0x60, 0x7C, 0x66, 0x89,
                    0x5C, 0x0C, 0xC7, 0x44, 0x06, 0x00, 0x70, 0xB4, 0x42, 0xCD, 0x13, 0x72,
                    0x05, 0xBB, 0x00, 0x70, 0xEB, 0x76, 0xB4, 0x08, 0xCD, 0x13, 0x73, 0x0D,
                    0x5A, 0x84, 0xD2, 0x0F, 0x83, 0xD8, 0x00, 0xBE, 0x8B, 0x7D, 0xE9, 0x82,
                    0x00, 0x66, 0x0F, 0xB6, 0xC6, 0x88, 0x64, 0xFF, 0x40, 0x66, 0x89, 0x44,
                    0x04, 0x0F, 0xB6, 0xD1, 0xC1, 0xE2, 0x02, 0x88, 0xE8, 0x88, 0xF4, 0x40,
                    0x89, 0x44, 0x08, 0x0F, 0xB6, 0xC2, 0xC0, 0xE8, 0x02, 0x66, 0x89, 0x04,
                    0x66, 0xA1, 0x60, 0x7C, 0x66, 0x09, 0xC0, 0x75, 0x4E, 0x66, 0xA1, 0x5C,
                    0x7C, 0x66, 0x31, 0xD2, 0x66, 0xF7, 0x34, 0x88, 0xD1, 0x31, 0xD2, 0x66,
                    0xF7, 0x74, 0x04, 0x3B, 0x44, 0x08, 0x7D, 0x37, 0xFE, 0xC1, 0x88, 0xC5,
                    0x30, 0xC0, 0xC1, 0xE8, 0x02, 0x08, 0xC1, 0x88, 0xD0, 0x5A, 0x88, 0xC6,
                    0xBB, 0x00, 0x70, 0x8E, 0xC3, 0x31, 0xDB, 0xB8, 0x01, 0x02, 0xCD, 0x13,
                    0x72, 0x1E, 0x8C, 0xC3, 0x60, 0x1E, 0xB9, 0x00, 0x01, 0x8E, 0xDB, 0x31,
                    0xF6, 0xBF, 0x00, 0x80, 0x8E, 0xC6, 0xFC, 0xF3, 0xA5, 0x1F, 0x61, 0xFF,
                    0x26, 0x5A, 0x7C, 0xBE, 0x86, 0x7D, 0xEB, 0x03, 0xBE, 0x95, 0x7D, 0xE8,
                    0x34, 0x00, 0xBE, 0x9A, 0x7D, 0xE8, 0x2E, 0x00, 0xCD, 0x18, 0xEB, 0xFE,
                    0x47, 0x52, 0x55, 0x42, 0x20, 0x00, 0x47, 0x65, 0x6F, 0x6D, 0x00, 0x48,
                    0x61, 0x72, 0x64, 0x20, 0x44, 0x69, 0x73, 0x6B, 0x00, 0x52, 0x65, 0x61,
                    0x64, 0x00, 0x20, 0x45, 0x72, 0x72, 0x6F, 0x72, 0x0D, 0x0A, 0x00, 0xBB,
                    0x01, 0x00, 0xB4, 0x0E, 0xCD, 0x10, 0xAC, 0x3C, 0x00, 0x75, 0xF4, 0xC3
                };

                var dp = @"\\.\" + l.Substring(0, 2);
                /*var dh = Utils.CreateFile(dp, 0xC0000000, 0x03, IntPtr.Zero, 0x03, DeviceIO.FILE_FLAG_WRITE_THROUGH | DeviceIO.FILE_FLAG_NO_BUFFERING, IntPtr.Zero);
                if (dh.IsInvalid)
                {
                    dh.Close();
                    dh.Dispose();
                    dh = null;
                    throw new Exception("Win32 Exception : 0x" +
                                        Convert.ToString(Marshal.GetHRForLastWin32Error(), 16).PadLeft(8, '0'));
                }
                var ds = new FileStream(dh, FileAccess.ReadWrite);

                var buf = new byte[512];
                ds.Read(buf, 0, 512);
                //Array.Copy(grub2_mbr, buf, grub2_mbr.Length);
                ds.Position = 0;

                //ds.Write(buf, 0, 512);
                

                for (var i = 0; i < grub2_mbr.Length; i++)
                {
                    ds.WriteByte(grub2_mbr[i]);
                }


                //ds.Close();
                //ds.Dispose();
                if (!dh.IsClosed) dh.Close();
                dh.Dispose();
                dh = null;*/


                using (var da = new DriveAccess(dp))
                {
                    foreach (byte t in grub2_mbr)
                    {
                        da.driveStream.WriteByte(t);
                    }
                }


                return;
            }

            var exename = bl == Bootloaders.Grub4DOS ? "grubinst.exe" : "syslinux.exe";

            var d = Program.GetTemporaryDirectory();
            var exepath = Path.Combine(d, exename);
            File.WriteAllBytes(exepath, bl == Bootloaders.Grub4DOS ? Resources.grubinst : Resources.syslinux);

            var p = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    FileName = exepath,
                    Verb = "runas"
                }
            };
            var driveletter = l.ToLower().Substring(0, 2);
            if (bl == Bootloaders.Grub4DOS)
            {
                var deviceId = Utils.GetPhysicalPath(driveletter);
                
                p.StartInfo.Arguments = " --skip-mbr-test --no-backup-mbr -t=0 (hd" + string.Concat(deviceId.Where(char.IsDigit)) + ")";
            }
            else
            {
                p.StartInfo.Arguments = " -m -a " + driveletter;
            }
            p.Start();
            p.WaitForExit();

            Program.SafeDel(d);
        }
    }

    public class driveitem
    {
        public string Disp { get; set; }
        public DriveInfo Value { get; set; }
    }
}