using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using System.Media;

namespace Krank_Type
{
    partial class Program
    {
        static Keys[] whiteflagedKeys = new Keys[] { Keys.CapsLock };
        static Dictionary<Keys, char> KeysToChar = new Dictionary<Keys, char>()
        {
            {Keys.A, 'A' }, {Keys.B, 'B' }, { Keys.C, 'C' }, {Keys.D, 'D' }, {Keys.E, 'E' }, {Keys.F, 'F' }, {Keys.G, 'G' },
            { Keys.H, 'H' }, {Keys.I, 'I' }, {Keys.J, 'J' }, {Keys.K, 'K' }, {Keys.L, 'L' }, {Keys.M, 'M' }, {Keys.N, 'N' },
            { Keys.O, 'O' }, {Keys.P, 'P' }, {Keys.Q, 'Q' }, {Keys.R, 'R' }, {Keys.S, 'S' }, {Keys.T, 'T' }, {Keys.U, 'U' },
            { Keys.V, 'V' }, {Keys.W, 'W' }, {Keys.X, 'X' }, {Keys.Y, 'Y' }, {Keys.Z, 'Z' }
        };
        //Constants
        #region Super Secret Code
        static string unlock = "NOSCAMPLS"; //UNLOCK CODE MUST BE IN UPPERCASE
        #endregion
        static string target = "Testing 123. This is a Test 321."; 
        static bool mercy = false;

        //Variables
        static string cache = "--------------------"; //20 character long
        static string tempTarget = "";
        static IKeyboardMouseEvents ghk;
        static int block;
        static SoundPlayer player;
        static void Main(string[] args)
        {
            ApplicationContext msgLoop = new ApplicationContext();
            Console.WriteLine("High functioning edition of Krank Type.");
            ghk = Hook.GlobalEvents();
            ghk.KeyUp += KeyUp;
            ghk.KeyDown += KeyDown;
            MixerInfo mi = GetMixerControls();
            AdjustVolume(mi, 100);
            player = new SoundPlayer();
            player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\aduz.cab";
            player.Play();
            Application.Run(msgLoop);
            Console.ReadKey();
            Console.WriteLine("HIGH LEVEL CLOSURE");
            GHKEND();  
        }

        static void KeyUp(object sender, KeyEventArgs e)
        {
            if (block > 0)
            {
                block--;
                return;
            }
            if (KeysToChar.ContainsKey(e.KeyCode))
            {
                cache = cache.Substring(1) + KeysToChar[e.KeyCode];
                Console.WriteLine($"Cache: {cache}");
                if (cache.Contains(unlock))
                {
                    GHKEND();
                    Console.WriteLine("Service Ended");
                }
            }
            if (!whiteflagedKeys.Contains(e.KeyCode) || !mercy)
            {
                Console.WriteLine($"[UP] \"{e.KeyCode.ToString()}\" -> .handled = true");
                e.Handled = true;
            }
            else
            {
                Console.WriteLine($"[UP] \"{e.KeyCode.ToString()}\" -> .handled = false");
                e.Handled = false;
            }
        }

        static void KeyDown(object sender, KeyEventArgs e)
        {
            if (block > 0)
            {
                block--;
                return;
            }
            if (e.Alt && e.Control && e.Shift)
                return;
            if (!whiteflagedKeys.Contains(e.KeyCode) || !mercy)
            {
                Console.WriteLine($"[DOWN] \"{e.KeyCode.ToString()}\" -> .handled = true");
                e.Handled = true;
                if (tempTarget.Length == 0)
                {
                    tempTarget = target;
                }
                block = char.IsUpper(tempTarget[0]) ? 4 : 2;
                SendKeys.Send(tempTarget[0].ToString());
                tempTarget = tempTarget.Substring(1);
            }
            else
            {
                Console.WriteLine($"[DOWN] \"{e.KeyCode.ToString()}\" -> .handled = false");
                e.Handled = false;
            }
        }
        static void GHKEND()
        {
            player.Stop();
            ghk.KeyUp -= KeyUp;
            ghk.KeyDown -= KeyDown;
            ghk.Dispose();
        }
        public sealed class Wallpaper
        {
            Wallpaper() { }
            const int SPI_SETDESKWALL IMAGE AS BMP
        }
    }
}
