using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Gestures.Zoom;
using MF.Engineering.MF8910.GestureDetector.Gestures.Swipe;
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
        private Person _active;
        private ImgIterator itr;
        private Device _d;

        public MainWindow()
        {
            try
            {
                itr = new ImgIterator(AppDomain.CurrentDomain.BaseDirectory + @"..\..\Images");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            DataContext = itr;
            InitializeComponent();
            Initialize();
            Show();
        }

        private void Initialize()
        {
            _d = new Device();
            _d.NewPerson += NewPerson;
            _d.PersonActive += ActivePerson;
            _d.PersonLost += Dispose;
            _d.Start();
            NrPersons.Text = "0";
        }

        #endregion

        private void ActivePerson(object sender, ActivePersonEventArgs e)
        {
            _active = e.Person;
            _active.OnZoom += Zoomed;
            _active.OnSwipe += Swiped;
            _active.OnWave += ActWaved;
            LoginText.Visibility = Visibility.Hidden;
            sv.Visibility = Visibility.Visible;
        }

        private void ActWaved(object sender, GestureEventArgs e)
        {
            sv.Visibility = Visibility.Hidden;
            LoginText.Visibility = Visibility.Visible;
            _active.OnWave += waved;
            _active.OnWave -= ActWaved;
            _active.OnZoom -= Zoomed;
            _active.OnSwipe -= Swiped;
            _active.Active = false;
            _active = null;
        }

        private void Swiped(object sender, GestureEventArgs e)
        {
            ExecuteSwipe(e);
        }

        private async void ExecuteSwipe(GestureEventArgs e)
        {
            SwipeGestureEventArgs args = (SwipeGestureEventArgs)e;
            switch (args.Direction)
            {
                case MF.Engineering.MF8910.GestureDetector.Tools.Direction.Forward:
                    break;
                case MF.Engineering.MF8910.GestureDetector.Tools.Direction.Upward:
                    break;
                case MF.Engineering.MF8910.GestureDetector.Tools.Direction.Downward:
                    break;
                case MF.Engineering.MF8910.GestureDetector.Tools.Direction.Left:
                    Storyboard sbl = this.FindResource("ImageLeftOut") as Storyboard;
                    if (sbl != null) sbl.Begin();
                    await Task.Delay(1000);
                    itr.Previous();
                    //Img.Height = MainGrid.RowDefinitions.ElementAt(1).ActualHeight;
                    Debug.WriteLine(sv.ActualHeight);
                    Img.Height = sv.ActualHeight;
                    Img.Width = sv.ActualWidth;
                    sbl = FindResource("ImageRightIn") as Storyboard;
                    if (sbl != null) sbl.Begin();
                    break;
                case MF.Engineering.MF8910.GestureDetector.Tools.Direction.Right:
                    Storyboard sbr = this.FindResource("ImageRightOut") as Storyboard;
                    if (sbr != null) sbr.Begin();
                    await Task.Delay(1000);
                    itr.Next();
                    //Img.Height = MainGrid.RowDefinitions.ElementAt(1).ActualHeight;
                    Img.Height = sv.ActualHeight;
                    Img.Width = sv.ActualWidth;
                    sbr = FindResource("ImageLeftIn") as Storyboard;
                    if (sbr != null) sbr.Begin();
                    break;
                case MF.Engineering.MF8910.GestureDetector.Tools.Direction.Backward:
                    break;
                case MF.Engineering.MF8910.GestureDetector.Tools.Direction.None:
                    break;
            }
        }

        private void Zoomed(object sender, GestureEventArgs e)
        {
            ZoomGestureEventArgs args = (ZoomGestureEventArgs)e;
            Img.Width *= args.ZoomFactorFromLast;
            Img.Height *= args.ZoomFactorFromLast;
        }

        private void NewPerson(object src, NewPersonEventArgs e)
        {
            if(_active == null)
                LoginText.Visibility = Visibility.Visible;
            UpdatePersonsCount();
            e.Person.OnWave += waved;
        }

        private void UpdatePersonsCount()
        {
            NrPersons.Text = _d.GetAllPersons().Count.ToString(CultureInfo.InvariantCulture);
        }

        private void Dispose(object sender, PersonDisposedEventArgs e)
        {
            if (e.Person == _active)
            {
                RemoveActive();
            }
            UpdatePersonsCount();
            if (_d.GetAllPersons().Count == 0)
            {
                LoginText.Visibility = Visibility.Hidden;
            }
        }

        private void RemoveActive()
        {
            sv.Visibility = Visibility.Hidden;
            _active.OnSwipe -= Swiped;
            _active.OnZoom -= Zoomed;
            _active = null;
        }

        /// <summary>
        /// Handled in Active Person
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void waved(object sender, GestureEventArgs e)
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //int i;
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
