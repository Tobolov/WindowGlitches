﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using System.Media;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using System.Drawing;
using System.Diagnostics;
using System.Threading;

namespace Krank_Type
{
    partial class Program
    {
        static Dictionary<Keys, char> KeysToChar = new Dictionary<Keys, char>()
        {
            {Keys.A, 'A' }, {Keys.B, 'B' }, { Keys.C, 'C' }, {Keys.D, 'D' }, {Keys.E, 'E' }, {Keys.F, 'F' }, {Keys.G, 'G' },
            { Keys.H, 'H' }, {Keys.I, 'I' }, {Keys.J, 'J' }, {Keys.K, 'K' }, {Keys.L, 'L' }, {Keys.M, 'M' }, {Keys.N, 'N' },
            { Keys.O, 'O' }, {Keys.P, 'P' }, {Keys.Q, 'Q' }, {Keys.R, 'R' }, {Keys.S, 'S' }, {Keys.T, 'T' }, {Keys.U, 'U' },
            { Keys.V, 'V' }, {Keys.W, 'W' }, {Keys.X, 'X' }, {Keys.Y, 'Y' }, {Keys.Z, 'Z' }, {Keys.OemQuestion, '/' }
        };

        //Variables
        static string tempCache = "--------------------";
        static string cache = tempCache; //20 character long
        static IKeyboardMouseEvents ghk;
        static Process glk = null;
        static void Main(string[] args)
        {
            ApplicationContext msgLoop = new ApplicationContext();
            ghk = Hook.GlobalEvents();
            ghk.KeyUp += KeyUp;
            Application.Run(msgLoop);
            Console.ReadKey();
            END();
        }
        static void KeyUp(object sender, KeyEventArgs e)
        {
            if (KeysToChar.ContainsKey(e.KeyCode))
            {
                cache = cache.Substring(1) + KeysToChar[e.KeyCode];
                if (cache.Contains("///STOP"))
                {
                    cache = tempCache;
                    END();
                    RemoveChars(7);
                }
                if (cache.Contains("///GLK"))
                {
                    cache = tempCache;
                    if (glk == null)
                    {
                        glk = StartProc("glk.exe");
                    }
                    RemoveChars(6);
                }
                if (cache.Contains("///UNGLK"))
                {
                    cache = tempCache;
                    if (glk != null)
                    {
                        StopProc(glk);
                        glk = null;
                    }
                    RemoveChars(8);
                }
                if (cache.Contains("///BLUESCREEN///"))
                {
                    cache = tempCache;
                    Process.Start("nmf.exe", "crash 0x01 /accepteula");
                    Thread.Sleep(2000);
                    SendKeys.Send("{ENTER}");
                }
            }
        }


        static void END()
        {
            if (glk != null)
                StopProc(glk);
            ghk.KeyUp -= KeyUp;
            ghk.Dispose();
            Environment.Exit(0);
        }
        static void RemoveChars(int num)
        {
            SendKeys.Send(string.Concat(Enumerable.Repeat("{BS}", num)));
        }

        static Process StartProc(string procname)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = procname;
            proc.Start();
            return proc;
        }
        static void StopProc(Process proc)
        {
            proc.Kill(); //Kill()
        }
    }
}