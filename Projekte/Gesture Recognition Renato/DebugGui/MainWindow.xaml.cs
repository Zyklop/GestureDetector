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


namespace DebugGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region initializing
        private bool stopped = false;
        private OwnConsole console;
        private List<int> IDs = new List<int>();
        private Dictionary<Person, Image> persons = new Dictionary<Person, Image>();
        private Person active;

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
            this.Show();
            pseudoMain();
        }

        private void Initialize()
        {
            console = new OwnConsole(ConsoleOutput, sv);
        }

        private void enableOutputBtn_Checked_1(object sender, RoutedEventArgs e)
        {
            stopped = !stopped;
        }

        #endregion
        private void pseudoMain()
        {
            Device d = new Device();
            d.NewPerson += NewPerson;
            d.PersonActive += ActivePerson;
            d.Start();
        }

        private void ActivePerson(object sender, ActivePersonEventArgs e)
        {
            active = e.Person;
            active.OnZoom += Zoomed;
            active.OnSwipe += Swiped;
            active.OnWave += actWaved;
            ActPersonTxt.Text = e.Person.ID.ToString();
        }

        private async void actWaved(object sender, GestureEventArgs e)
        {
            console.WriteLine(((Person)sender).ID + "waved");
            WaveActiveOk.Visibility = System.Windows.Visibility.Visible;
            Task tim = Task.Factory.StartNew(() => timer(2500));
            await tim;
            WaveActiveOk.Visibility = System.Windows.Visibility.Hidden;
        }

        private async void Swiped(object sender, GestureEventArgs e)
        {
            console.WriteLine("swiped");
            SwipeGestureEventArgs args = (SwipeGestureEventArgs)e;
            SwipeActiveOk.Visibility = System.Windows.Visibility.Visible;
            SwipeDirection.Text = (args.Direction.ToString());
            Task tim = Task.Factory.StartNew(() => timer(2500));
            await tim;
            SwipeActiveOk.Visibility = System.Windows.Visibility.Hidden;
        }

        private async void Zoomed(object sender, GestureEventArgs e)
        {
            console.WriteLine("zoomed");
            ZoomGestureEventArgs args = (ZoomGestureEventArgs)e;
            ZoomActiveOk.Visibility = System.Windows.Visibility.Visible;
            ActZoomFactor.Text = (args.ZoomFactorFromLast.ToString());
            StaticZoomFactor.Text = args.ZoomFactorFromBegin.ToString();
            Task tim = Task.Factory.StartNew(() => timer(2500));
            await tim;
            ZoomActiveOk.Visibility = System.Windows.Visibility.Hidden;
        }

        private void NewPerson(object src, NewPersonEventArgs e)
        {
            if (IDs.Contains(e.Person.ID))
            {
                console.WriteLine(e.Person.ID + " back");
            }
            else
            {
                console.WriteLine(e.Person.ID + " new");
                IDs.Add(e.Person.ID);
            }
            Image i = new Image();
            i.Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/OK.png",
                                       UriKind.RelativeOrAbsolute));
            i.Visibility = System.Windows.Visibility.Hidden;
            persons.Add(e.Person, i);
            TextBlock tb = new TextBlock();
            tb.Height = 50;
            tb.Text = e.Person.ID.ToString();
            PersonList.Children.Add(tb);
            WavePanel.Children.Add(i);
            e.Person.OnWave += waved;
            e.Person.OnDispose += Dispose;
        }

        private void Dispose(object sender, PersonDisposedEventArgs e)
        {
            console.WriteLine("disposing: " + e.Person.ID);
            e.Person.OnWave -= waved;
            if (e.Person == active)
            {
                RemoveActive();
            }
            WavePanel.Children.Remove(persons[e.Person]);
            TextBlock tb = null;
            foreach (TextBlock item in PersonList.Children)
            {
                try
                {
                    int i = Convert.ToInt32(item.Text);
                    if (i == e.Person.ID)
                    {
                        tb = item;
                    }
                }
                catch (System.Exception ex)
                {
                }
            }
            console.WriteLine("disposing: " + tb.Text);
            PersonList.Children.Remove(tb);
            persons.Remove(e.Person);

        }

        private void RemoveActive()
        {
            active = null;
            ActPersonTxt.Text = "";
            active.OnSwipe -= Swiped;
            active.OnZoom -= Zoomed;
        }

        private async void waved(object sender, GestureEventArgs e)
        {
            console.WriteLine(((Person)sender).ID + "waved");
            persons[(Person)sender].Visibility = System.Windows.Visibility.Visible;
            Task tim = Task.Factory.StartNew(() => timer(2500));
            await tim;
            persons[(Person)sender].Visibility = System.Windows.Visibility.Hidden;
        }

        private async Task timer(int i)
        {
            Thread.Sleep(i);
        }
        #region OwnConsole
        private class OwnConsole
        {
            private TextBlock output;
            private ScrollViewer sv;

            public OwnConsole(TextBlock tb, ScrollViewer scrV)
            {
                output=tb;
                sv = scrV;
            }
            #region WriteLine
            public void WriteLine(int s)
            {
                output.Text += s.ToString();
                output.Text += "\n";
                sv.ScrollToEnd();
            }

            public void WriteLine(double d)
            {
                output.Text += d.ToString();
                output.Text += "\n";
                sv.ScrollToEnd();
            }

            public void WriteLine(bool b)
            {
                output.Text += b.ToString();
                output.Text += "\n";
                sv.ScrollToEnd();
            }

            public void WriteLine(string s)
            {
                output.Text += s;
                output.Text += "\n";
                sv.ScrollToEnd();
            }
            #endregion
            #region Write
            public void WriteLine(object s)
            {
                output.Text += s.ToString();
                output.Text += "\n";
                sv.ScrollToEnd();
            }

            public void Write(int s)
            {
                output.Text += s.ToString();
            }

            public void Write(double d)
            {
                output.Text += d.ToString();
            }

            public void Write(bool b)
            {
                output.Text += b.ToString();
            }

            public void Write(string s)
            {
                output.Text += s;
            }

            public void Write(object s)
            {
                output.Text += s.ToString();
            }
            #endregion
        }
        #endregion
    }
}
