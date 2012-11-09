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
using MF.Engineering.MF8910.GestureDetector.Gestures.Login;


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
            enableOutputBtn.IsChecked = stopped;
        }
        #endregion
        private void pseudoMain()
        {
            Device d = new Device();
            d.NewPerson += NewPerson;
            d.Start();
        }

        private void NewPerson(object src, NewPersonEventArgs e)
        {
            console.WriteLine(e.Person.ID);
            e.Person.OnWave += delegate(object o, GestureEventArgs ev) { console.WriteLine("gewinkt"); };
            WaveLeftCondition wlc = new WaveLeftCondition(e.Person);
            wlc.enable();
            wlc.Triggered += delegate(object o, GestureEventArgs ev)
            {
                WLTrigg.Visibility = System.Windows.Visibility.Visible;
                WLFail.Visibility = System.Windows.Visibility.Hidden;
            };
            wlc.Succeeded += delegate(object o, GestureEventArgs ev)
            {
                WLSucc.Visibility = System.Windows.Visibility.Visible;
                WLFail.Visibility = System.Windows.Visibility.Hidden;
            };
            wlc.Failed += delegate(object o, GestureEventArgs ev)
            {
                WLTrigg.Visibility = System.Windows.Visibility.Hidden;
                WLSucc.Visibility = System.Windows.Visibility.Hidden;
                WLFail.Visibility = System.Windows.Visibility.Visible;
            };
            WaveRightCondition wrc = new WaveRightCondition(e.Person);
            wlc.enable();
            wrc.Triggered += delegate(object o, GestureEventArgs ev)
            {
                WRTrigg.Visibility = System.Windows.Visibility.Visible;
                WRFail.Visibility = System.Windows.Visibility.Hidden;
            };
            wrc.Succeeded += delegate(object o, GestureEventArgs ev)
            {
                WRSucc.Visibility = System.Windows.Visibility.Visible;
                WRFail.Visibility = System.Windows.Visibility.Hidden;
            };
            wrc.Failed += delegate(object o, GestureEventArgs ev)
            {
                WRTrigg.Visibility = System.Windows.Visibility.Hidden;
                WRSucc.Visibility = System.Windows.Visibility.Hidden;
                WRFail.Visibility = System.Windows.Visibility.Visible;
            };
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
