using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Gestures.Wave;
using MF.Engineering.MF8910.GestureDetector.Gestures.Zoom;
using MF.Engineering.MF8910.GestureDetector.Gestures.Swipe;
using System.Threading;
using System.Windows.Media.Animation;
using System.Diagnostics;


namespace DebugGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region initializing
        private List<Person> persons = new List<Person>();
        private Person active;
        private ImgIterator itr;

        public MainWindow()
        {
            try
            {
                itr = new ImgIterator(@"C:\Users\bor\Documents\Git\SA\Projekte\Gesture Recognition Renato\DebugGui\Images", UriKind.Relative);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            DataContext = itr;
            InitializeComponent();
            Initialize();
            this.Show();
        }

        private void Initialize()
        {
            Device d = new Device();
            d.NewPerson += NewPerson;
            d.PersonActive += ActivePerson;
            d.Start();
            NrPersons.Text = "0";
        }

        #endregion

        private void ActivePerson(object sender, ActivePersonEventArgs e)
        {
            active = e.Person;
            active.OnZoom += Zoomed;
            active.OnSwipe += Swiped;
            active.OnWave += actWaved;
            LoginText.Visibility = System.Windows.Visibility.Hidden;
            sv.Visibility = System.Windows.Visibility.Visible;
        }

        private void actWaved(object sender, GestureEventArgs e)
        {
            sv.Visibility = System.Windows.Visibility.Hidden;
            LoginText.Visibility = System.Windows.Visibility.Visible;
            active.OnWave += waved;
            active.OnWave -= actWaved;
            active.OnZoom -= Zoomed;
            active.OnSwipe -= Swiped;
            active.Active = false;
            active = null;
        }

        private async void Swiped(object sender, GestureEventArgs e)
        {
            SwipeGestureEventArgs args = (SwipeGestureEventArgs)e;
            switch (args.Direction)
            {
                case MF.Engineering.MF8910.GestureDetector.Tools.Direction.forward:
                    break;
                case MF.Engineering.MF8910.GestureDetector.Tools.Direction.upward:
                    break;
                case MF.Engineering.MF8910.GestureDetector.Tools.Direction.downward:
                    break;
                case MF.Engineering.MF8910.GestureDetector.Tools.Direction.left:
                    Storyboard sbl = this.FindResource("ImageLeftOut") as Storyboard;
                    sbl.Begin();
                    await Task.Delay(1000);
                    itr.Previous();
                    Img.Width = 800;
                    Img.Height = 600;
                    sbl = this.FindResource("ImageRightIn") as Storyboard;
                    sbl.Begin();
                    break;
                case MF.Engineering.MF8910.GestureDetector.Tools.Direction.right:
                    Storyboard sbr = this.FindResource("ImageRightOut") as Storyboard;
                    sbr.Begin();
                    await Task.Delay(1000);
                    itr.Next();
                    Img.Width = 800;
                    Img.Height = 600;
                    sbr = this.FindResource("ImageLeftOut") as Storyboard;
                    sbr.Begin();
                    break;
                case MF.Engineering.MF8910.GestureDetector.Tools.Direction.backward:
                    break;
                case MF.Engineering.MF8910.GestureDetector.Tools.Direction.none:
                    break;
                default:
                    break;
            }
            //Image i = new Image();
            //i.Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/OK.png",
            //                           UriKind.RelativeOrAbsolute));
        }

        private void Zoomed(object sender, GestureEventArgs e)
        {
            ZoomGestureEventArgs args = (ZoomGestureEventArgs)e;
            Img.Width *= args.ZoomFactorFromLast;
            Img.Height *= args.ZoomFactorFromLast;
        }

        private void NewPerson(object src, NewPersonEventArgs e)
        {
            if(active == null)
                LoginText.Visibility = System.Windows.Visibility.Visible;
            persons.Add(e.Person);
            UpdatePersonsCount();
            e.Person.OnWave += waved;
            e.Person.OnDispose += Dispose;
        }

        private void UpdatePersonsCount()
        {
            NrPersons.Text = persons.Count.ToString();
        }

        private void Dispose(object sender, PersonDisposedEventArgs e)
        {
            if (e.Person == active)
            {
                RemoveActive();
            }
            persons.Remove(e.Person);
            UpdatePersonsCount();
            if (persons.Count == 0)
            {
                LoginText.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void RemoveActive()
        {
            sv.Visibility = System.Windows.Visibility.Hidden;
            active.OnSwipe -= Swiped;
            active.OnZoom -= Zoomed;
            active = null;
        }

        private void waved(object sender, GestureEventArgs e)
        {
        }
        
    }
}
