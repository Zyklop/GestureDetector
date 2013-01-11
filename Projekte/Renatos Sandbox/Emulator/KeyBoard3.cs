using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emulator
{
    class KeyBoard3
    {

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        public static void KeyDown(Keys key)
        {
            keybd_event((byte)key, 0, 0, 0);
        }

        public static void KeyUp(Keys key)
        {
            keybd_event((byte)key, 0, 0x7F, 0);
        }
    }
}
