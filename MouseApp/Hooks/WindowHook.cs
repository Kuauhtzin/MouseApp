﻿using MouseApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MouseApp.Hooks
{
    public static class WindowHook
    {
        internal static event EventHandler WindowActivatedChanged;
        internal static Timer TimerWatcher = new Timer();
        internal static InfoWindow WindowActive = new InfoWindow();
        internal static void DoStartWatcher()
        {
            TimerWatcher.Interval = 500;
            TimerWatcher.Tick += TimerWatcher_Tick;
            TimerWatcher.Start();
        }

        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        public static IDictionary<IntPtr, InfoWindow> GetOpenedWindows()
        {
            IntPtr shellWindow = GetShellWindow();
            Dictionary<IntPtr, InfoWindow> windows = new Dictionary<IntPtr, InfoWindow>();

            EnumWindows(new EnumWindowsProc(delegate (IntPtr hWnd, int lParam) {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;
                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;
                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);
                var info = new InfoWindow();
                info.Handle = hWnd;
                info.File = new FileInfo(GetProcessPath(hWnd));
                info.Title = builder.ToString();
                windows[hWnd] = info;
                return true;
            }), 0);
            return windows;
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        public static string GetProcessPath(IntPtr hwnd)
        {
            uint pid = 0;
            GetWindowThreadProcessId(hwnd, out pid);
            if (hwnd != IntPtr.Zero)
            {
                if (pid != 0)
                {
                    var process = Process.GetProcessById((int) pid);
                    if (process != null)
                    {
                        return process.Id.ToString();
                        ///return process.MainModule.FileName.ToString();
                    }
                }
            }
            return "";
        }

        [DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr handle, int nCmdShow);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("User32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("User32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("User32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("User32.dll")]
        private static extern IntPtr GetShellWindow();

        [DllImport("User32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out uint processId);


        static void TimerWatcher_Tick(object sender, EventArgs e)
        {
            var windowActive = new InfoWindow();
            windowActive.Handle = GetForegroundWindow();
            string path = GetProcessPath(windowActive.Handle);
            if (string.IsNullOrEmpty(path)) return;
            windowActive.File = new FileInfo(path);
            int length = GetWindowTextLength(windowActive.Handle);
            if (length == 0) return;
            StringBuilder builder = new StringBuilder(length);
            GetWindowText(windowActive.Handle, builder, length + 1);
            windowActive.Title = builder.ToString();
            if (windowActive.ToString() != WindowActive.ToString())
            {
                //fire:
                WindowActive = windowActive;
                if (WindowActivatedChanged != null) WindowActivatedChanged(sender, e);
                Console.WriteLine("Window: " + WindowActive.ToString());
            }
        }
    }
}