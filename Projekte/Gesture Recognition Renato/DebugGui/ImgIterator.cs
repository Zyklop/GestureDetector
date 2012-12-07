using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DebugGui
{
    class ImgIterator:INotifyPropertyChanged
    {
        List<string> dir;
        int index = 0;

        public ImgIterator(string path, UriKind type)
        {
            //Uri uri = new Uri(path, type);
            dir = Directory.EnumerateFiles(path,"*.jpg").ToList();
        }

        public string Uri
        {
            get
            {
                return dir[index];
            }
        }

        public void Next()
        {
            if (index == dir.Count - 1)
            {
                index = 0;
            }
            index++;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Uri"));
            }
        }

        public void Previous()
        {
            if (index == 0)
            {
                index = dir.Count - 1;
            }
            index--;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Uri"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
