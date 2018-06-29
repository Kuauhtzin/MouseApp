using System;
using System.IO;
using System.Windows.Forms;

namespace MouseApp.Models
{
    public class InfoWindow
    {
        public IntPtr Handle = IntPtr.Zero;
        public FileInfo File = new FileInfo(Application.ExecutablePath);
        public string Title = Application.ProductName;

        public override string ToString()
        {
            return File.Name + "\t>\t" + Title;
        }
    }
}
