﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using SharpBoot.Properties;
using W7R;
using Timer = System.Timers.Timer;

namespace SharpBoot
{
    public enum Bootloader
    {
        Syslinux = 0,
        Grub4Dos = 1
    }

    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public partial class MainWindow : Form
    {
        public void SetSize()
        {
            tbxSize.Text = Program.GetSizeString(CurImages.Sum(x => x.SizeB) + 988);

            menuStrip.Renderer = Windows7Renderer.Instance;

            cmsChecksum.Renderer = Windows7Renderer.Instance;
        }

        Dictionary<string, Tuple<CultureInfo, bool>> lngs = new Dictionary<string, Tuple<CultureInfo, bool>>(); 


        private void loadlng()
        {
            List<CultureInfo> result = fromresx(typeof (Strings));

            result.AddRange(fromresx(typeof (ISOCat)));

            var systemLng = CultureInfo.InstalledUICulture;
            if (!systemLng.IsNeutralCulture)
                systemLng = systemLng.Parent;

            if(result.All(x => x.ThreeLetterISOLanguageName != systemLng.ThreeLetterISOLanguageName))
            {
                result.Add(systemLng);
            }

            result = result.Distinct().ToList();
            result.Sort((x, y) => string.Compare(x.NativeName, y.NativeName, StringComparison.Ordinal));

            lngs.Clear();
            foreach (var x in result)
            {
                var mnit = new ToolStripMenuItem(x.NativeName, Utils.GetFlag(x.Name));
                mnit.Click += (sender, args) => LngItemClick(mnit);
                languageToolStripMenuItem.DropDownItems.Add(mnit);
                lngs.Add(x.NativeName, new Tuple<CultureInfo, bool>(x, x != systemLng));
            }
        }



        private void LngItemClick(ToolStripMenuItem it)
        {
            var tmp = lngs[it.Text];

            if (Program.GetCulture().Equals(tmp.Item1)) return;

            if(!tmp.Item2)
            {
                Process.Start("https://poeditor.com/join/project/GDNqzsHFSk");
                setlngitem(Program.GetCulture());
                return;
            }

            Program.SetAppLng(tmp.Item1);

            if (changing && FieldsEmpty())
            {
                Controls.Clear();
                InitializeComponent();
                SetSize();
                centerDragndrop();
                lngs.Clear();
                loadlng();
                cbxBootloader.SelectedIndex = 0;
                cbxRes.SelectedIndex = 0;
                cbxBackType.SelectedIndex = 0;
                updateAvailableToolStripMenuItem.Visible = update_available;
            }
            else if (!FieldsEmpty())
            {
                MessageBox.Show(Strings.ChangesNeedRestart);
            }

            changing = false;
            var c = Program.GetCulture();

            setlngitem(tmp.Item1);
            
            changing = true;
        }

        private static List<CultureInfo> fromresx(Type t)
        {
            List<CultureInfo> result = new List<CultureInfo>();
            ResourceManager rm = new ResourceManager(t);

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (CultureInfo culture in cultures)
            {
                try
                {
                    if (culture.Equals(CultureInfo.InvariantCulture)) continue; //do not use "==", won't work

                    ResourceSet rs = rm.GetResourceSet(culture, true, false);
                    if (rs != null)
                        result.Add(culture);
                }
                catch (CultureNotFoundException)
                {
                    //NOP
                }
            }
            return result;
        }

        public List<ImageLine> CurImages = new List<ImageLine>();

        public Bootloader SelectedBootloader => (Bootloader) cbxBootloader.SelectedIndex;


        public void AddImage(string filePath, ISOV ver = null)
        {
            if (CurImages.Count(x => x.FilePath == filePath) != 0)
                return;

            var name = Path.GetFileNameWithoutExtension(filePath);
            var desc = "";
            var cat = "";

            if (ver?.Hash == "nover")
            {
                name = ver.Parent.Name;
                desc = ver.Parent.Description;
                cat = ver.Parent.CategoryTxt;
            }
            else
            {
                if (automaticallyAddISOInfoToolStripMenuItem.Checked && ver?.Hash != "other")
                {
                    ver = ver ?? (ISOInfo.GetFromFile(filePath, new FileInfo(filePath).Length > 750000000));
                    if (ver == null)
                    {
                        MessageBox.Show(Path.GetFileName(filePath) + "\n\n" + Strings.CouldntDetect, "SharpBoot", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        name = ver.Name;
                        desc = ver.Parent.Description;
                        cat = ver.Parent.CategoryTxt;
                    }
                }
            }


            var im = new ImageLine(name, filePath, desc, cat);
            CurImages.Add(im);

            SetSize();


            lvIsos.Rows.Add(name, Program.GetFileSizeString(filePath), cat, desc, filePath);
        }

        public void setlngitem(CultureInfo ci)
        {
            bool found = false;

            foreach (ToolStripMenuItem mni in languageToolStripMenuItem.DropDownItems)
            {
                if (lngs[mni.Text].Item1.Equals(ci))
                {
                    found = true;
                    mni.Checked = true;
                    languageToolStripMenuItem.Image = mni.Image;
                    break;
                }
                else mni.Checked = false;
            }

            if(!found) setlngitem(new CultureInfo("en"));
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED       
                return handleParam;
            }
        }

        public Timer updTmr;

        public MainWindow()
        {
            DoubleBuffered = true;
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw,
                true);

            InitializeComponent();
            changing = true;
            loadlng();
            var c = Program.GetCulture();
            setlngitem(c);
            automaticallyAddISOInfoToolStripMenuItem.Checked = Settings.Default.AutoAddInfo;

            SetSize();
            if (Program.IsWin)
            {
                Utils.SetWindowTheme(lvIsos.Handle, "EXPLORER", null);
            }


            cbxBootloader.SelectedIndex = 0;
            cbxRes.SelectedIndex = 0;
            cbxBackType.SelectedIndex = 0;

            ISOInfo.UpdateFinished += (o, args) =>
            {
                try
                {
                    if (this.InvokeRequired)
                        this.Invoke((MethodInvoker) (() => mniUpdate.Visible = false));
                    else mniUpdate.Visible = false;
                }
                catch
                {

                }
            };
            mniUpdate.Visible = true;
            checkForUpdates();

            updTmr = new Timer(300000);
            updTmr.Elapsed += UpdTmr_Elapsed;
            updTmr.Enabled = true;

            
        }

        private void UpdTmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            mniUpdate.Visible = true;
            
            ISOInfo.RefreshISOs();
            mniUpdate.Visible = true;
            checkForUpdates();
        }

        private bool update_available = false;

        private void checkForUpdates()
        {
            try
            {
                using (var wb = new WebClient())
                {
                    wb.Headers["User-Agent"] =
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.58 Safari/537.36";
                    var ct = wb.DownloadString("https://api.github.com/repos/zdimension/SharpBoot/releases/latest");

                    var lnid = ct.IndexOf("tag_name");
                    ct = ct.Substring(lnid + 13);
                    ct = ct.Substring(0, ct.IndexOf('"'));

                    var v = Version.Parse(ct);
                    //v = new Version(3, 7);
                    updateAvailableToolStripMenuItem.Visible = update_available = v > Assembly.GetEntryAssembly().GetName().Version;
                }
            }
            catch
            {

            }

            //mniUpdate.Visible = false;
        }

        private static void g_GenerationFinished(GenIsoFrm g)
        {
            Program.ClrTmp();

            Thread.CurrentThread.CurrentCulture = new CultureInfo(Settings.Default.Lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.Lang);

            if (
                MessageBox.Show(Strings.IsoCreated.Replace(@"\n", "\n"), Strings.IsoCreatedTitle,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                QEMUISO.LaunchQemu(g.OutputFilepath, g._usb);
            }
        }

        private void lvIsos_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = ((string[]) e.Data.GetData(DataFormats.FileDrop)).ToList();
                if (files.Count == 1)
                {
                    if (Path.GetExtension(files[0]).ToLower() != ".iso" &&
                        Path.GetExtension(files[0]).ToLower() != ".img")
                    {
                        e.Effect = DragDropEffects.None;
                        return;
                    }
                }
                else
                {
                    if (
                        !(files.Any(
                            x => Path.GetExtension(x).ToLower() == ".iso" || Path.GetExtension(x).ToLower() == ".img")))
                    {
                        e.Effect = DragDropEffects.None;
                        return;
                    }
                }

                e.Effect = DragDropEffects.Copy;
            }
        }

        private void lvIsos_DragDrop(object sender, DragEventArgs e)
        {
            ((string[]) e.Data.GetData(DataFormats.FileDrop)).All(x =>
            {
                AddImage(x);
                return true;
            });
        }

        private void btnGen_Click(object sender, EventArgs e)
        {
            launchgeniso(false);
        }

        private void launchgeniso(bool usb)
        {
            Form ask = null;
            if(usb) ask = new USBFrm(Strings.CreateMultibootUsb, Strings.Filesystem, Strings.OK, true, "FAT32 " + Strings.Recommended, "FAT16", "FAT12");
            else ask = new AskPath();
            if (ask.ShowDialog() == DialogResult.OK)
            {
                var fn = "";
                fn = usb ? ((USBFrm) ask).SelectedUSB.Name.ToUpper().Substring(0, 3) : ((AskPath) ask).FileName;
                var g = new GenIsoFrm(fn, usb);
                g.GenerationFinished += delegate { g_GenerationFinished(g); };
                
                g.Title = txtTitle.Text;
                if (usb) g.filesystem = ((USBFrm) ask).TheComboBox.SelectedItem.ToString().Split(' ')[0].ToUpper();
                switch (cbxBackType.SelectedIndex)
                {
                    case 0:
                        g.IsoBackgroundImage = "";
                        break;
                    case 1:
                        g.IsoBackgroundImage = txtBackFile.Text;
                        break;
                    default:
                        g.IsoBackgroundImage = "$$NONE$$";
                        break;
                }
                var selsize = cbxRes.SelectedItem.ToString();
                selsize = selsize.Replace("x", " ");
                var ssize = selsize.Split(' ');

                var selload = cbxBootloader.SelectedItem.ToString().ToLower().Trim();
                if (selload.StartsWith("syslinux")) selload = "syslinux";

                IBootloader bl = null;
                if (selload == "syslinux") bl = new Syslinux();
                if (selload == "grub4dos")
                {
                    bl = new Grub4DOS();
                }

                g.bloader = bl;
                Program.SupportAccent = bl.SupportAccent;
                g.Res = new Size(int.Parse(ssize[0]), int.Parse(ssize[1]));
                g.Images = CurImages.Select(x => new ImageLine(x.Name.RemoveAccent(), x.FilePath, x.Description.RemoveAccent(), x.Category.RemoveAccent())).ToList();
                g.ShowDialog(this);

                Program.ClrTmp();
                Program.SupportAccent = false;
            }
        }

        public void CheckFields()
        {
            lblDragHere.Visible = lvIsos.Rows.Count == 0;
            btnGen.Enabled = btnUSB.Enabled = !(lvIsos.Rows.Count == 0 ||
                               (cbxBackType.SelectedIndex == 1 && !File.Exists(txtBackFile.Text)));
        }

        private void btnRemISO_Click(object sender, EventArgs e)
        {
            var fp = lvIsos.SelectedRows[0].Cells[4].Value.ToString();
            CurImages.RemoveAll(x => x.FilePath == fp);
            lvIsos.Rows.Remove(lvIsos.Rows.OfType<DataGridViewRow>().Single(x => x.Cells[4].Value.ToString() == fp));

            SetSize();
        }

        private void gbxTest_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = ((string[]) e.Data.GetData(DataFormats.FileDrop)).ToList();
                if (files.Count == 1)
                {
                    if (Path.GetExtension(files[0]).ToLower() != ".iso" &&
                        Path.GetExtension(files[0]).ToLower() != ".img" && !files[0].EndsWith("\\"))
                        return;
                }
                else
                {
                    return;
                }

                e.Effect = DragDropEffects.Copy;
            }
        }

        private void gbxTest_DragDrop(object sender, DragEventArgs e)
        {
            var t = ((string[]) e.Data.GetData(DataFormats.FileDrop));
            var a = t[0];
            QEMUISO.LaunchQemu(a, a.EndsWith("\\"));
        }

        private void lvIsos_SelectionChanged(object sender, EventArgs e)
        {
            btnRemISO.Enabled = lvIsos.SelectedRows.Count == 1;
            btnChecksum.Enabled = lvIsos.SelectedRows.Count == 1;
        }

        private void lvIsos_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            btnRemISO_Click(this, EventArgs.Empty);
        }

        private void lvIsos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (lvIsos.SelectedRows.Count != 1)
                return;

            var newname = lvIsos.SelectedRows[0].Cells[0].Value?.ToString() ?? "";
            var newcat = lvIsos.SelectedRows[0].Cells[2].Value?.ToString() ?? "";
            var newdesc = lvIsos.SelectedRows[0].Cells[3].Value?.ToString() ?? "";

            var ind =
                CurImages.IndexOf(CurImages.Single(x => x.FilePath == lvIsos.SelectedRows[0].Cells[4].Value.ToString()));
            var nw = new ImageLine(newname, lvIsos.SelectedRows[0].Cells[4].Value.ToString(), newdesc, newcat);
            CurImages.RemoveAt(ind);
            CurImages.Insert(ind, nw);
        }

        private void btnBackBrowse_Click(object sender, EventArgs e)
        {
            var ofpI = new OpenFileDialog
            {
                Filter = Strings.PicFilter + " (*.png, *.jpg, *.jpeg, *.bmp)|*.png;*.jpg;*.jpeg;*.bmp"
            };

            if (ofpI.ShowDialog() == DialogResult.OK)
            {
                var img = Image.FromFile(ofpI.FileName);
                if (img.Width < 720)
                {
                    cbxRes.SelectedIndex = 0;
                }
                else if (img.Width >= 720 && img.Width < 912)
                {
                    cbxRes.SelectedIndex = 1;
                }
                else
                {
                    cbxRes.SelectedIndex = 2;
                }

                txtBackFile.Text = ofpI.FileName;

                CheckFields();
            }
        }

        private void btnChecksum_Click(object sender, EventArgs e)
        {
            btnChecksum.ShowContextMenuStrip();
        }

        private void chksum(string n, Func<string> f)
        {
            var d = DateTime.Now;
            Cursor = Cursors.WaitCursor;


            var sb = f();

            var a = DateTime.Now;
            var t = a - d;
            txImInfo.Text = string.Format(Strings.ChkOf, n, 
                            Path.GetFileName(lvIsos.SelectedRows[0].Cells[4].Value.ToString())) + "\r\n";
            txImInfo.Text += sb + "\r\n";
            /*txImInfo.Text += Strings.CalcIn + " " + t.Hours + "h " + t.Minutes + "m " + (t.TotalMilliseconds / 1000.0) +
                             "s";*/
            txImInfo.Text += string.Format(Strings.CalcIn, t);
            Cursor = Cursors.Default;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            UpdTmr_Elapsed(this, null);
        }

        private bool changing;

        public bool FieldsEmpty()
        {
            return lvIsos.Rows.Count == 0 && txtTitle.Text == "SharpBoot" && txtBackFile.Text.Length == 0;
        }

        private int lastIndex = -1;

        private bool temporary;

        private void centerDragndrop()
        {
            lblDragHere.Location = new Point(
                lvIsos.Width / 2 - lblDragHere.Width / 2 + lvIsos.Location.X,
                lvIsos.Height / 2 - lblDragHere.Height / 2 + lvIsos.Location.Y
                );
        }

        private void ReplaceFontRecursive(Control parent, Font f1, Font f2)
        {
            if (parent.Font == f1) parent.Font = f2;
            foreach(Control ct in parent.Controls)
            {
                if (ct.Font == f1) ct.Font = f2;
                ReplaceFontRecursive(ct, f1, f2);
            }
        }

        public void CheckGrub4Dos()
        {
            cbxRes.Enabled = !(cbxBackType.SelectedIndex == 2 && cbxBootloader.SelectedIndex == 1);
            if (cbxBackType.SelectedIndex == 1 && File.Exists(txtBackFile.Text) && cbxBootloader.SelectedIndex == 1)
            {
                var img = Image.FromFile(txtBackFile.Text);
                switch (img.Width)
                {
                    case 800:
                        cbxRes.SelectedIndex = 1;
                        break;
                    case 1024:
                        cbxRes.SelectedIndex = 2;
                        break;
                    default:
                        cbxRes.SelectedIndex = 0;
                        break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new About().ShowDialog(this);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void addISOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fr = new AddIso();
            if (fr.ShowDialog() == DialogResult.OK)
            {
                AddImage(fr.ISOPath, fr.IsoV);
            }
        }

        private void automaticallyAddISOInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            automaticallyAddISOInfoToolStripMenuItem.Checked = !automaticallyAddISOInfoToolStripMenuItem.Checked;
            Settings.Default.AutoAddInfo = automaticallyAddISOInfoToolStripMenuItem.Checked;
            Settings.Default.Save();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                lvIsos.Rows.Clear();
                txtTitle.Text = "";

                var d = XDocument.Load(new FileStream(openFileDialog.FileName, FileMode.Open));

                var c = d.Element("SharpBoot");

                txtTitle.Text = c.Element("Name").Value;
                cbxBootloader.SelectedIndex = Convert.ToInt32(c.Element("Bootloader").Value);
                cbxRes.SelectedIndex = Convert.ToInt32(c.Element("Resolution").Value);
                cbxBackType.SelectedIndex = Convert.ToInt32(c.Element("Backtype").Value);
                txtBackFile.Text = c.Element("Backpath").Value;

                foreach (XElement a in c.Elements("ISOs").Nodes())
                {
                    CurImages.Add(new ImageLine(a.Element("Nom").Value, a.Element("Path").Value, a.Element("Desc").Value,
                        a.Element("Cat").Value));
                    lvIsos.Rows.Add(a.Element("Nom").Value, Program.GetFileSizeString(a.Element("Path").Value),
                        a.Element("Cat").Value, a.Element("Desc").Value, a.Element("Path").Value);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var doc =
                    new XDocument(new XElement("SharpBoot",
                        new XElement("Name", txtTitle.Text),
                        new XElement("Bootloader", cbxBootloader.SelectedIndex),
                        new XElement("Resolution", cbxRes.SelectedIndex),
                        new XElement("Backtype", cbxBackType.SelectedIndex),
                        new XElement("Backpath", txtBackFile.Text),
                        new XElement("ISOs",
                            lvIsos.Rows.OfType<DataGridViewRow>().Select(x => new XElement("ISO",
                                new XElement("Nom", x.Cells[0].Value),
                                new XElement("Cat", x.Cells[2].Value),
                                new XElement("Desc", x.Cells[3].Value),
                                new XElement("Path", x.Cells[4].Value))))));

                doc.Save(saveFileDialog.FileName);
                /*if(File.Exists(saveFileDialog.FileName)) File.Delete(saveFileDialog.FileName);
                using (var ms = File.OpenWrite(saveFileDialog.FileName))
                {
                    var fmt = new BinaryFormatter();
                    fmt.Serialize(ms, doc);
                    ms.Flush();
                    ms.Close();
                }*/
            }
        }

        private void txtTitle_TextChanged(object sender, EventArgs e)
        {
            var pos = txtTitle.SelectionStart;
            txtTitle.Text = txtTitle.Text.RemoveAccent();
            txtTitle.SelectionStart = pos;
        }

        private void btnInstBoot_Click(object sender, EventArgs e)
        {
            //new InstallBoot().ShowDialog(this);
            var frm = new USBFrm(Strings.InstallABootLoader, Strings.ChooseBootloader, Strings.Install, false, "Syslinux " + Strings.Recommended, "Grub4Dos");
            frm.BtnClicked += (o, args) =>
            {
                frm.ProgressVisible = true;
                frm.SetProgress(5);
                BootloaderInst.Install(frm.SelectedUSB.Name, (Bootloaders) frm.TheComboBox.SelectedIndex);
                frm.SetProgress(100);
                MessageBox.Show(
                string.Format(Strings.BootloaderInstalled, (frm.TheComboBox.SelectedIndex == 1 ? "Grub4Dos" : "Syslinux"),
                    frm.SelectedUSB.Name), "SharpBoot", 0, MessageBoxIcon.Information);
            };
            frm.ShowDialog(this);
        }

        private void btnSha1_Click(object sender, EventArgs e)
        {
            chksum("SHA-1", () => Utils.FileSHA1(lvIsos.SelectedRows[0].Cells[4].Value.ToString()));
        }

        private void btnSha256_Click(object sender, EventArgs e)
        {
            chksum("SHA-256", () => Utils.FileSHA256(lvIsos.SelectedRows[0].Cells[4].Value.ToString()));
        }

        private void btnSha512_Click(object sender, EventArgs e)
        {
            chksum("SHA-512", () => Utils.FileSHA512(lvIsos.SelectedRows[0].Cells[4].Value.ToString()));
        }

        private void btnSha384_Click(object sender, EventArgs e)
        {
            chksum("SHA-384", () => Utils.FileSHA384(lvIsos.SelectedRows[0].Cells[4].Value.ToString()));
        }

        private void lvIsos_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CheckFields();
        }

        private void cbxBootloader_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckGrub4Dos();
        }

        private void cbxBackType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtBackFile.Enabled = btnBackBrowse.Enabled = cbxBackType.SelectedIndex == 1;
            if (cbxBackType.SelectedIndex != 1) txtBackFile.Text = "";
            CheckFields();
            CheckGrub4Dos();
        }

        private void lvIsos_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            CheckFields();
        }

        private void btnUSB_Click(object sender, EventArgs e)
        {
            launchgeniso(true);
        }

        private void mD5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chksum("MD5", () => Utils.FileMD5(lvIsos.SelectedRows[0].Cells[4].Value.ToString()));
        }

        private void updateAvailableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/zdimension/SharpBoot/releases/latest");
        }

        private void MainWindow_SizeChanged(object sender, EventArgs e)
        {
            centerDragndrop();
        }
    }
}