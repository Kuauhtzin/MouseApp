using MouseApp.Hooks;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace MouseApp
{
    public partial class Form1 : Form
    {
        MouseHook.POINT p;
        Bitmap bmp = new Bitmap(1, 1);
        Bitmap myBitmap;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            p = new MouseHook.POINT();

            /*
            MouseHook mouseHook = new MouseHook();
            mouseHook.LeftButtonDown += new MouseHook.MouseHookCallback(MouseHook_MouseClick);
            mouseHook.Install();
            */

            KeyboardHook keyboardHook = new KeyboardHook();
            keyboardHook.KeyDown += new KeyboardHook.KeyboardHookCallback(KeyboardHook_KeyDown);
            keyboardHook.KeyUp += new KeyboardHook.KeyboardHookCallback(KeyboardHook_KeyUp);
            keyboardHook.Install();

            /*
            myBitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics gr = Graphics.FromImage(myBitmap);
            gr.CopyFromScreen(0, 0, 0, 0, myBitmap.Size);
            myBitmap.Save("img.png", System.Drawing.Imaging.ImageFormat.Png);
            */

            //Process[] processlist = Process.GetProcesses();
            /*Process[] processlist = Process.GetProcessesByName("notepad");
            foreach (Process process in processlist)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    proc = (IntPtr) process.Id;
                    Console.WriteLine("Process: {0} ID: {1} Window title: {2}", process.ProcessName, process.Id, process.MainWindowTitle);
                }
            }*/

            //WindowHook.DoStartWatcher();
        }

        private void KeyboardHook_KeyUp(KeyboardHook.VKeys key)
        {
            p.x = Cursor.Position.X;
            p.y = Cursor.Position.Y;
            switch (key)
            {
                case KeyboardHook.VKeys.SPACE:
                    ClickOnPointTool.ClickOff(this.Handle, new Point(p.x, p.y));
                    break;
                default:
                    break;
            }
        }

        private void KeyboardHook_KeyDown(KeyboardHook.VKeys key)
        {
            ///Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] KeyDown Event {" + key.ToString() + "}");
            p.x = Cursor.Position.X;
            p.y = Cursor.Position.Y;
            int step = 15;

            switch (key)
            {
                case KeyboardHook.VKeys.UP:
                    p.y -= step;
                    break;
                case KeyboardHook.VKeys.DOWN:
                    p.y += step;
                    break;
                case KeyboardHook.VKeys.LEFT:
                    p.x -= step;
                    break;
                case KeyboardHook.VKeys.RIGHT:
                    p.x += step;
                    break;
                case KeyboardHook.VKeys.SPACE:
                    //ClickOnPointTool.ClickOnPoint(this.Handle,new Point(p.x , p.y));
                    ClickOnPointTool.ClickOn(this.Handle,new Point(p.x , p.y));
                    break;
                default:
                    break;
            }
            
            Console.WriteLine($"{p.x} , {p.y} , {key}");
            Cursor.Position = new Point(p.x, p.y);
            //Cursor.Clip = new Rectangle(this.Location, this.Size); //block cursor exit from form dimensions
        }

        private void MouseHook_MouseClick(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            Color color = GetColorAt(mouseStruct.pt.x, mouseStruct.pt.y);
            Console.WriteLine( $"{mouseStruct.pt.x} , {mouseStruct.pt.y} , {mouseStruct.dwExtraInfo.ToString()} , {color}" );
            
            ///Color pixelColor = myBitmap.GetPixel(50, 50);
        }

        private Color GetColorAt(int x, int y)
        {
            Rectangle bounds = new Rectangle(x, y, 1, 1);
            using (Graphics g = Graphics.FromImage(bmp))
                g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
            return bmp.GetPixel(0, 0);
        }

        public static void Mouse_Click(int dx, int dy)
        {
            MouseHook.mouse_event((int)(MouseHook.MouseMessages.WM_LBUTTONDOWN), dx, dy, 0, 0);
            MouseHook.mouse_event((int)(MouseHook.MouseMessages.WM_LBUTTONUP), dx, dy, 0, 0);
            Console.WriteLine($" Clicked: {dx} , {dy}");
        }

    }
}

