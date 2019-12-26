﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpBoot.Utilities;

namespace SharpBoot.Controls
{
    // Thank you Cody Gray http://stackoverflow.com/a/6017174/2196124

    public class LinkLabelEx : LinkLabel
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetCursor(IntPtr hCursor);

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WindowMessage.WM_SETCURSOR)
            {
                // Set the cursor to use the system hand cursor
                SetCursor(LoadCursor(IntPtr.Zero, WinApi.IDC_HAND));

                // Indicate that the message has been handled
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }
    }
}