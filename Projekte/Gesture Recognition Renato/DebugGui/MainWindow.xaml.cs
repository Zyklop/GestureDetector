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
        private Person active;
        private ImgIterator itr;
        private Device d;

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
            d = new Device();
            d.NewPerson += NewPerson;
            d.PersonActive += ActivePerson;
            d.PersonLost += Dispose;
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
                    Img.Height = MainGrid.RowDefinitions.ElementAt(1).ActualHeight;
                    sbl = this.FindResource("ImageRightIn") as Storyboard;
                    sbl.Begin();
                    break;
                case MF.Engineering.MF8910.GestureDetector.Tools.Direction.right:
                    Storyboard sbr = this.FindResource("ImageRightOut") as Storyboard;
                    sbr.Begin();
                    await Task.Delay(1000);
                    itr.Next();
                    Img.Height = MainGrid.RowDefinitions.ElementAt(1).ActualHeight;
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
            Debug.WriteLine("zoomed");
            ZoomGestureEventArgs args = (ZoomGestureEventArgs)e;
            //Img.Width *= args.ZoomFactorFromLast;
            Img.Height *= args.ZoomFactorFromLast;
        }

        private void NewPerson(object src, NewPersonEventArgs e)
        {
            if(active == null)
                LoginText.Visibility = System.Windows.Visibility.Visible;
            UpdatePersonsCount();
            e.Person.OnWave += waved;
        }

        private void UpdatePersonsCount()
        {
            NrPersons.Text = d.GetAll().Count.ToString();
        }

        private void Dispose(object sender, PersonDisposedEventArgs e)
        {
            if (e.Person == active)
            {
                RemoveActive();
            }
            UpdatePersonsCount();
            if (d.GetAll().Count == 0)
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int i;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Grid_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
            }
        }
        
    }
}
