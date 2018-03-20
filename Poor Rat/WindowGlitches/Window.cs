
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Net;

namespace WindowGlitches
{
        #region stuff
    public partial class Window : Form
    {
        BackgroundWorker bw = new BackgroundWorker();
        Random rand = new Random(DateTime.Now.Millisecond);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);



        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, uint wMsg, UIntPtr wParam, IntPtr lParam); //used for maximizing the screen

        const int WM_SYSCOMMAND = 0x0112; //used for maximizing the screen.
        const int myWParam = 0xf120; //used for maximizing the screen.
        const int myLparam = 0x5073d; //used for maximizing the screen.


        int oldWindowLong;

        [Flags]
        enum WindowStyles : uint
        {
            WS_OVERLAPPED = 0x00000000,
            WS_POPUP = 0x80000000,
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_GROUP = 0x00020000,
            WS_TABSTOP = 0x00010000,

            WS_MINIMIZEBOX = 0x00020000,
            WS_MAXIMIZEBOX = 0x00010000,

            WS_CAPTION = WS_BORDER | WS_DLGFRAME,
            WS_TILED = WS_OVERLAPPED,
            WS_ICONIC = WS_MINIMIZE,
            WS_SIZEBOX = WS_THICKFRAME,
            WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_CHILDWINDOW = WS_CHILD,

            //Extended Window Styles

            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,

            //#if(WINVER >= 0x0400)

            WS_EX_MDICHILD = 0x00000040,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_WINDOWEDGE = 0x00000100,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_CONTEXTHELP = 0x00000400,

            WS_EX_RIGHT = 0x00001000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,

            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_APPWINDOW = 0x00040000,

            WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
            WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),
            //#endif /* WINVER >= 0x0400 */

            //#if(WIN32WINNT >= 0x0500)

            WS_EX_LAYERED = 0x00080000,
            //#endif /* WIN32WINNT >= 0x0500 */

            //#if(WINVER >= 0x0500)

            WS_EX_NOINHERITLAYOUT = 0x00100000, // Disable inheritence of mirroring by children
            WS_EX_LAYOUTRTL = 0x00400000, // Right to left mirroring
            //#endif /* WINVER >= 0x0500 */

            //#if(WIN32WINNT >= 0x0500)

            WS_EX_COMPOSITED = 0x02000000,
            WS_EX_NOACTIVATE = 0x08000000
            //#endif /* WIN32WINNT >= 0x0500 */

        }

        public enum GetWindowLongConst
        {
            GWL_WNDPROC = (-4),
            GWL_HINSTANCE = (-6),
            GWL_HWNDPARENT = (-8),
            GWL_STYLE = (-16),
            GWL_EXSTYLE = (-20),
            GWL_USERDATA = (-21),
            GWL_ID = (-12)
        }

        public enum LWA
        {
            ColorKey = 0x1,
            Alpha = 0x2,
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        /// <summary>
        /// Make the form (specified by its handle) a window that supports transparency.
        /// </summary>
        /// <param name="Handle">The window to make transparency supporting</param>
        public void SetFormTransparent(IntPtr Handle)
        {
            oldWindowLong = GetWindowLong(Handle, (int)GetWindowLongConst.GWL_EXSTYLE);
            SetWindowLong(Handle, (int)GetWindowLongConst.GWL_EXSTYLE, Convert.ToInt32(oldWindowLong | (uint)WindowStyles.WS_EX_LAYERED | (uint)WindowStyles.WS_EX_TRANSPARENT));
        }

        /// <summary>
        /// Make the form (specified by its handle) a normal type of window (doesn't support transparency).
        /// </summary>
        /// <param name="Handle">The Window to make normal</param>
        public void SetFormNormal(IntPtr Handle)
        {
            SetWindowLong(Handle, (int)GetWindowLongConst.GWL_EXSTYLE, Convert.ToInt32(oldWindowLong | (uint)WindowStyles.WS_EX_LAYERED));
        }

        /// <summary>
        /// Makes the form change White to Transparent and clickthrough-able
        /// Can be modified to make the form translucent (with different opacities) and change the Transparency Color.
        /// </summary>
        public void SetTheLayeredWindowAttribute()
        {
            uint transparentColor = 0xffffffff;

            SetLayeredWindowAttributes(this.Handle, transparentColor, 125, 0x2);

            this.TransparencyKey = Color.White;
        }

        /// <summary>
        /// Finds the Size of all computer screens combined (assumes screens are left to right, not above and below).
        /// </summary>
        /// <returns>The width and height of all screens combined</returns>
        public static Size getFullScreensSize()
        {
            int height = int.MinValue;
            int width = 0;

            foreach (Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                //take largest height
                height = Math.Max(screen.WorkingArea.Height, height);

                width += screen.Bounds.Width;
            }

            return new Size(width, height);
        }

        /// <summary>
        /// Finds the top left pixel position (with multiple screens this is often not 0,0)
        /// </summary>
        /// <returns>Position of top left pixel</returns>
        public static Point getTopLeft()
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;

            foreach (Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                minX = Math.Min(screen.WorkingArea.Left, minX);
                minY = Math.Min(screen.WorkingArea.Top, minY);
            }

            return new Point(minX, minY);
        }
        #endregion

        public Window()
        {
            InitializeComponent();

            MaximizeEverything();

            SetFormTransparent(this.Handle);

            SetTheLayeredWindowAttribute();

            this.ShowInTaskbar = false;
            Cursor.Hide();
            Bounds = SystemInformation.VirtualScreen;

            BackgroundWorker bwThread = new BackgroundWorker();
            bwThread.DoWork += new DoWorkEventHandler(backgroundThread);

            this.bw = bwThread;
            this.bw.RunWorkerAsync();
        }

        #region stuff
        private void MaximizeEverything()
        {
            this.Location = getTopLeft();
            this.Size = getFullScreensSize();

            SendMessage(this.Handle, WM_SYSCOMMAND, (UIntPtr)myWParam, (IntPtr)myLparam);
        }

        public delegate void d(Control c);
        public void AddControlToTheForm(Control controlToAdd)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new d(this.AddControlToTheForm), new object[] { controlToAdd });
            }
            else
            {
                this.Controls.Add(controlToAdd);
            }
        }
        public void RemoveControlFromForm(Control controlToRemove)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new d(this.RemoveControlFromForm), new object[] { controlToRemove });
            }
            else
            {
                this.Controls.Remove(controlToRemove);
            }
        }
        #endregion

        static bool dotsOnline = true;
        private void backgroundThread(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int burdID = 0;

            Size fullSize = getFullScreensSize();
            var vs = SystemInformation.VirtualScreen;
            Point topLeft = getTopLeft();

            using (Graphics formGraphics = this.CreateGraphics())
            {
                formGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low; // or NearestNeighbour
                formGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                formGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
                formGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                formGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
                //-----------------TIMING----------------
                int framesTillRekt = 15000;      //15  --
                int framesTillBurds = 1;     //20  --
                int framesTillSpaz = 1;       //35  --
                //-----------------ENABLED---------------
                bool HasPixel = false; //              --  
                bool HasBurd = true;  //              --    
                bool HasSpaz = false;  //               --
                //----------------DEBUGGING--------------
                bool ShowEvent = true; //             --  
                bool ShowBegin = true;  //            --    
                //---------------------------------------
                int nextPixel = 1;
                int nextBurd = 1;
                int nextSpaz = 1;

                if (HasBurd)
                    using (WebClient webClient = new WebClient())
                    {
                        byte[] data = webClient.DownloadData("https://68.media.tumblr.com/b21481bb5d888c0df2163016294b0d83/tumblr_olbmo3DVP01w1m5cqo1_500.gif");

                        using (MemoryStream mem = new MemoryStream(data))
                        {
                            Image burdImg = Image.FromStream(mem);
                            burdImg.Save(Path.Combine(Path.GetTempPath(), "Img.gif"));
                        }
                    }

                while (dotsOnline)
                {
                    if (framesTillRekt >= 0) { framesTillRekt--; }
                    nextPixel--;
                    if (framesTillBurds >= 0) { framesTillBurds--; }
                    else { nextBurd--; }
                    if (framesTillSpaz >= 0) { framesTillSpaz--; }
                    else { nextSpaz--; }

                    if(framesTillRekt == 0 && HasPixel)
                    {
                        if(ShowBegin) Debug.WriteLine("[Begin] Rekt");
                    }
                    if (framesTillBurds == 0 && HasBurd)
                    {
                        if (ShowBegin) Debug.WriteLine("[Begin] Burds");
                    }
                    if (framesTillSpaz == 0 && HasSpaz)
                    {
                        if (ShowBegin) Debug.WriteLine("[Begin] Spaz");
                        this.Invoke(new Action(() => TransparencyKey = Color.Transparent));
                        GraphicWhiteout(formGraphics, vs);
                    }

                    if (nextPixel == 0 && HasPixel)
                    {
                        if (ShowEvent) Debug.WriteLine("[Event] Pixel");
                        for (int i = 0; i < 12; i++)
                        {
                            formGraphics.FillRectangle(new SolidBrush(RandomColor()), rand.Next(0, (-1 * topLeft.X) + fullSize.Width), rand.Next(0, (-1 * topLeft.Y) + fullSize.Height), 1, 1);
                        }
                        if (framesTillRekt < 0)
                            nextPixel = 2;
                        else
                            nextPixel = 15;
                    }
                    if (nextBurd == 0 && (framesTillSpaz > 0 || !HasSpaz) && HasBurd)
                    {
                        if (ShowEvent) Debug.WriteLine("[Event] Burd");
                        nextBurd = rand.Next(50, 800);
                        WorkBurds(Image.FromFile(Path.Combine(Path.GetTempPath(), "Img.gif")), fullSize, topLeft, ++burdID);
                    }

                    if (nextSpaz == 0 && HasSpaz)
                    {
                        if (ShowEvent) Debug.WriteLine("[Event] Spaz");
                        nextSpaz = rand.Next(30, 50); //TIME
                        WorkGlitches(formGraphics, nextSpaz);
                    }

                    Thread.Sleep(1);
                }
            }
        }

        List<PictureBox> boxes = new List<PictureBox>();
        private void WorkBurds(Image i, Size fullSize, Point topLeft, int id)
        {
            PictureBox temp = new PictureBox();
            temp.Location = new Point(rand.Next(0, (-1 * topLeft.X) + fullSize.Width), rand.Next(0, (-1 * topLeft.Y) + fullSize.Height));
            temp.Size = new Size(500, 375); //IMAGE SIZE
            temp.Image = i;
            temp.Name = $"Burd:ID:{id}";
            temp.TabIndex = 0;
            temp.TabStop = false;
            this.AddControlToTheForm(temp);
            boxes.Add(temp);
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(rand.Next(3500, 9000));
                boxes.Remove(temp);
                this.RemoveControlFromForm(temp);
            });
        }

        Bitmap screen;
        Bitmap previous;
        private void WorkGlitches(Graphics g, int n)
        {
            var vs = SystemInformation.VirtualScreen;
            if (screen == null)
            {
                screen = new Bitmap(vs.Width, vs.Height);
                using (Graphics gtemp = Graphics.FromImage(screen))
                {
                    gtemp.CopyFromScreen(vs.Left, vs.Top, 0, 0, screen.Size);
                }
            }
            using (Bitmap b = new Bitmap(vs.Width, vs.Height, PixelFormat.Format32bppPArgb))
            {
                using (Graphics temp = Graphics.FromImage(b))
                {
                    temp.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                    temp.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    temp.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
                    temp.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                    temp.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
                    //----------------------------------------
                    //--------------- BEGIN DRAW -------------
                    //----------------------------------------

                    GraphicBitmapDisplacement(temp, vs, screen);

                    GraphicBars(temp, vs);

                    GraphicSquareGlitch(temp, vs, 100000);

                    //----------------------------------------
                    //--------------- END DRAW ---------------
                    //----------------------------------------
                    previous = b;
                    g.DrawImage(b, new Point(0, 0));
                }
            }
        }

        private void GraphicBars(Graphics temp, Rectangle vs)
        {
            for (int y = vs.Top; y < vs.Height; y += rand.Next(1, 10))
            {
                temp.FillRectangle(new SolidBrush(rand.Next(0, 2) == 0 ? RandomColorGray(200, 200, 255) : RandomColor(200)), 0, y, vs.Width, 1);
            }
        }

        private void GraphicCornerGlitch(Graphics temp, Rectangle vs)
        {
            int c = rand.Next(2, 6);
            for (int x = c / 4 == 0 ? vs.Left : vs.Left + vs.Width / 2; x < ((c / 4 == 0) ? vs.Left + vs.Width / 2 : vs.Width); x += rand.Next(1, 5))
            {
                for (int y = (c - 2) % 2 == 0 ? vs.Top : vs.Top + vs.Height / 2; y < ((c - 2) % 2 == 0 ? vs.Top + vs.Height / 2 : vs.Height); y += rand.Next(1, 5))
                {
                    temp.FillRectangle(new SolidBrush(RandomColorGray(255, 250, 255)), x, y, 1, 1);
                }
            }
        }
        private void GraphicSquareGlitch(Graphics temp, Rectangle vs, int pixels)
        {
            int ymin = rand.Next(0, vs.Height);
            int ymax = rand.Next(ymin+1, vs.Height);
            int xmin = rand.Next(0, vs.Width);
            int xtemp = pixels / (ymax - ymin) + xmin;
            int xmax = xtemp > vs.Width ? vs.Width : xtemp;
            for (int x = xmin; x < xmax; x += rand.Next(1, 5))
            {
                for (int y = ymin; y < ymax; y += rand.Next(1, 5))
                {
                    temp.FillRectangle(new SolidBrush(RandomColor(255)), x, y, 1, 1);
                }
            }
        }

        private void GraphicBitmapDisplacement(Graphics temp, Rectangle vs, Bitmap screen)
        {
            Bitmap b = (Bitmap)screen.Clone();
            LockBitmap bit = new LockBitmap(b);
            bit.LockBits();
            for (int x = 0; x < bit.Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var k = rand.Next(1, 15);
                    var c = bit.GetPixel((x + rand.Next(1, 10) * k) % vs.Width, y);
                    var d = bit.GetPixel((x + rand.Next(1, 10) * k) % vs.Width, y);
                    var e = bit.GetPixel(x, (y + rand.Next(1, 10)) % vs.Height);
                    bit.SetPixel(x, y, Color.FromArgb(255, ((c.R + d.R + e.R + rand.Next(0, 50)) / 3) % 255, ((c.G + d.G + e.G + rand.Next(0, 50)) / 3) % 255, ((c.B + d.B + e.B + rand.Next(0, 50)) / 3) % 255));
                }
            }
            bit.UnlockBits();
            temp.DrawImage(b, new Point(0, 0));
        }

        private void GraphicWhiteout(Graphics temp, Rectangle vs)
        {
            temp.FillRectangle(new SolidBrush(Color.FromArgb(0, 255, 255, 255)), 0, 0, vs.Width, vs.Height);
        }

        private Color RandomColor(int a = 0, int r = 0, int g = 0, int b = 0)
        {
            return Color.FromArgb(rand.Next(a, 255), rand.Next(r, 255), rand.Next(g, 255), rand.Next(b, 255));
        }
        private Color RandomColorGray(int a = 0, int min = 0, int max = 255)
        {
            byte b = (byte)rand.Next(min, max);
            return Color.FromArgb(rand.Next(a, 255), b, b, b);
        }

    }
}