﻿using System.ComponentModel;
using System.Windows.Forms;

namespace SharpBoot
{
    partial class About
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.btnOK = new System.Windows.Forms.Button();
            this.lblAbout = new System.Windows.Forms.Label();
            this.lblUseSoftware = new System.Windows.Forms.Label();
            this.ilTranslators = new System.Windows.Forms.ImageList(this.components);
            this.lblHelpTranslate = new SharpBoot.LinkLabelEx();
            this.lblWebsite = new SharpBoot.LinkLabelEx();
            this.lvTranslators = new SharpBoot.CustomListView();
            this.clmnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnURL = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.linkLabelEx6 = new SharpBoot.LinkLabelEx();
            this.linkLabelEx5 = new SharpBoot.LinkLabelEx();
            this.linkLabelEx4 = new SharpBoot.LinkLabelEx();
            this.linkLabelEx3 = new SharpBoot.LinkLabelEx();
            this.linkLabelEx1 = new SharpBoot.LinkLabelEx();
            this.lblQEMU = new SharpBoot.LinkLabelEx();
            this.linkLabelEx2 = new SharpBoot.LinkLabelEx();
            this.lbl7zip = new SharpBoot.LinkLabelEx();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // lblAbout
            // 
            resources.ApplyResources(this.lblAbout, "lblAbout");
            this.lblAbout.Name = "lblAbout";
            // 
            // lblUseSoftware
            // 
            resources.ApplyResources(this.lblUseSoftware, "lblUseSoftware");
            this.lblUseSoftware.Name = "lblUseSoftware";
            // 
            // ilTranslators
            // 
            this.ilTranslators.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilTranslators.ImageStream")));
            this.ilTranslators.TransparentColor = System.Drawing.Color.Transparent;
            this.ilTranslators.Images.SetKeyName(0, "flag_germany.png");
            this.ilTranslators.Images.SetKeyName(1, "flag_france.png");
            this.ilTranslators.Images.SetKeyName(2, "flag_romania.png");
            // 
            // lblHelpTranslate
            // 
            resources.ApplyResources(this.lblHelpTranslate, "lblHelpTranslate");
            this.lblHelpTranslate.Name = "lblHelpTranslate";
            this.lblHelpTranslate.TabStop = true;
            // 
            // lblWebsite
            // 
            resources.ApplyResources(this.lblWebsite, "lblWebsite");
            this.lblWebsite.Name = "lblWebsite";
            this.lblWebsite.TabStop = true;
            this.lblWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelClicked);
            // 
            // lvTranslators
            // 
            this.lvTranslators.BackColor = System.Drawing.SystemColors.Control;
            this.lvTranslators.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvTranslators.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmnName,
            this.clmnURL});
            this.lvTranslators.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvTranslators.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("lvTranslators.Items"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("lvTranslators.Items1"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("lvTranslators.Items2")))});
            this.lvTranslators.LargeImageList = this.ilTranslators;
            resources.ApplyResources(this.lvTranslators, "lvTranslators");
            this.lvTranslators.MultiSelect = false;
            this.lvTranslators.Name = "lvTranslators";
            this.lvTranslators.SmallImageList = this.ilTranslators;
            this.lvTranslators.StateImageList = this.ilTranslators;
            this.lvTranslators.UseCompatibleStateImageBehavior = false;
            this.lvTranslators.View = System.Windows.Forms.View.Details;
            this.lvTranslators.DoubleClick += new System.EventHandler(this.lvTranslators_DoubleClick);
            // 
            // clmnName
            // 
            resources.ApplyResources(this.clmnName, "clmnName");
            // 
            // clmnURL
            // 
            resources.ApplyResources(this.clmnURL, "clmnURL");
            // 
            // linkLabelEx6
            // 
            resources.ApplyResources(this.linkLabelEx6, "linkLabelEx6");
            this.linkLabelEx6.Name = "linkLabelEx6";
            this.linkLabelEx6.TabStop = true;
            this.linkLabelEx6.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelClicked);
            // 
            // linkLabelEx5
            // 
            resources.ApplyResources(this.linkLabelEx5, "linkLabelEx5");
            this.linkLabelEx5.Name = "linkLabelEx5";
            this.linkLabelEx5.TabStop = true;
            this.linkLabelEx5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelClicked);
            // 
            // linkLabelEx4
            // 
            resources.ApplyResources(this.linkLabelEx4, "linkLabelEx4");
            this.linkLabelEx4.Name = "linkLabelEx4";
            this.linkLabelEx4.TabStop = true;
            this.linkLabelEx4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelClicked);
            // 
            // linkLabelEx3
            // 
            resources.ApplyResources(this.linkLabelEx3, "linkLabelEx3");
            this.linkLabelEx3.Name = "linkLabelEx3";
            this.linkLabelEx3.TabStop = true;
            this.linkLabelEx3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelClicked);
            // 
            // linkLabelEx1
            // 
            resources.ApplyResources(this.linkLabelEx1, "linkLabelEx1");
            this.linkLabelEx1.Name = "linkLabelEx1";
            this.linkLabelEx1.TabStop = true;
            this.linkLabelEx1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelClicked);
            // 
            // lblQEMU
            // 
            resources.ApplyResources(this.lblQEMU, "lblQEMU");
            this.lblQEMU.Name = "lblQEMU";
            this.lblQEMU.TabStop = true;
            this.lblQEMU.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelClicked);
            // 
            // linkLabelEx2
            // 
            resources.ApplyResources(this.linkLabelEx2, "linkLabelEx2");
            this.linkLabelEx2.Name = "linkLabelEx2";
            this.linkLabelEx2.TabStop = true;
            this.linkLabelEx2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelClicked);
            // 
            // lbl7zip
            // 
            resources.ApplyResources(this.lbl7zip, "lbl7zip");
            this.lbl7zip.Name = "lbl7zip";
            this.lbl7zip.TabStop = true;
            this.lbl7zip.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelClicked);
            // 
            // About
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblHelpTranslate);
            this.Controls.Add(this.lblWebsite);
            this.Controls.Add(this.lvTranslators);
            this.Controls.Add(this.linkLabelEx6);
            this.Controls.Add(this.linkLabelEx5);
            this.Controls.Add(this.linkLabelEx4);
            this.Controls.Add(this.linkLabelEx3);
            this.Controls.Add(this.linkLabelEx1);
            this.Controls.Add(this.lblQEMU);
            this.Controls.Add(this.lblUseSoftware);
            this.Controls.Add(this.linkLabelEx2);
            this.Controls.Add(this.lbl7zip);
            this.Controls.Add(this.lblAbout);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnOK;
        private Label lblAbout;
        private LinkLabelEx lbl7zip;
        private LinkLabelEx linkLabelEx2;
        private Label lblUseSoftware;
        private LinkLabelEx lblQEMU;
        private LinkLabelEx linkLabelEx1;
        private LinkLabelEx linkLabelEx3;
        private LinkLabelEx linkLabelEx4;
        private LinkLabelEx linkLabelEx5;
        private LinkLabelEx linkLabelEx6;
        private CustomListView lvTranslators;
        private ColumnHeader clmnName;
        private ImageList ilTranslators;
        private ColumnHeader clmnURL;
        private LinkLabelEx lblWebsite;
        private LinkLabelEx lblHelpTranslate;
    }
}