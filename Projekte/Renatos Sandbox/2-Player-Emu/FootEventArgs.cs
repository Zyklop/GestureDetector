using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Tools;

namespace _2_Player_Emu
{
    class FootEventArgs : GestureEventArgs
    {
        public Direction Foot { get; set; }

        public double Distance { get; set; }
    }
}
