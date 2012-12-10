using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace DebugGui
{
    class ImgIterator:INotifyPropertyChanged
    {
        List<string> dir;
        int _index;

        public ImgIterator(string path)
        {
            _index = 0;
            //Uri uri = new Uri(path, type);
            dir = Directory.EnumerateFiles(path,"*.jpg").ToList();
        }

        public string Uri
        {
            get
            {
                return dir[_index];
            }
        }

        public void Next()
        {
            if (_index == dir.Count - 1)
            {
                _index = 0;
            }
            _index++;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Uri"));
            }
        }

        public void Previous()
        {
            if (_index == 0)
            {
                _index = dir.Count - 1;
            }
            _index--;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Uri"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
