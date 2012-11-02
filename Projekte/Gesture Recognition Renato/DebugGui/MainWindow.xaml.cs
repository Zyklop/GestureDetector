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
using DataSources;
using GestureEvents;
using Conditions;
using Conditions.Login;


namespace DebugGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

        private void pseudoMain()
        {
            Device d = new Device();
            d.NewPerson += NewPerson;
            d.Start();
            //while (true)
            //{
            //    if (pers != d.GetAll().Count)
            //    {
            //        console.WriteLine(d.GetAll().Count);
            //        pers = d.GetAll().Count;
            //    }
            //}
        }

        private void NewPerson(object src, NewPersonEventArgs e)
        {
            console.WriteLine(e.Person.ID);
            e.Person.OnWave += delegate(object o, EventArgs ev) { console.WriteLine("gewinkt"); };
            WaveLeftCondition wlc = new WaveLeftCondition(e.Person);
            wlc.enable();
            wlc.Triggered += delegate(object o, EventArgs ev)
            {
                WLTrigg.Visibility = System.Windows.Visibility.Visible;
                WLFail.Visibility = System.Windows.Visibility.Hidden;
            };
            wlc.Succeeded += delegate(object o, EventArgs ev)
            {
                WLSucc.Visibility = System.Windows.Visibility.Visible;
                WLFail.Visibility = System.Windows.Visibility.Hidden;
            };
            wlc.Failed += delegate(object o, EventArgs ev)
            {
                console.WriteLine("failed");
                WLTrigg.Visibility = System.Windows.Visibility.Hidden;
                WLSucc.Visibility = System.Windows.Visibility.Hidden;
                WLFail.Visibility = System.Windows.Visibility.Visible;
            };
            WaveRightCondition wrc = new WaveRightCondition(e.Person);
            wlc.enable();
            wrc.Triggered += delegate(object o, EventArgs ev)
            {
                WRTrigg.Visibility = System.Windows.Visibility.Visible;
                WRFail.Visibility = System.Windows.Visibility.Hidden;
            };
            wrc.Succeeded += delegate(object o, EventArgs ev)
            {
                WRSucc.Visibility = System.Windows.Visibility.Visible;
                WRFail.Visibility = System.Windows.Visibility.Hidden;
            };
            wrc.Failed += delegate(object o, EventArgs ev)
            {
                WRTrigg.Visibility = System.Windows.Visibility.Hidden;
                WRSucc.Visibility = System.Windows.Visibility.Hidden;
                WRFail.Visibility = System.Windows.Visibility.Visible;
            };
        }

        private class OwnConsole
        {
            private TextBlock output;
            private ScrollViewer sv;

            public OwnConsole(TextBlock tb, ScrollViewer scrV)
            {
                output=tb;
                sv = scrV;
            }

            public void WriteLine(int s)
            {
                //StringBuilder sb = new StringBuilder();
                //sb.Append(output.Text);
                //sb.Append(s);
                //sb.Append("/n");
                //output.Text = sb.ToString();
                output.Text += s.ToString();
                output.Text += "\n";
            }

            public void WriteLine(string s)
            {
                //StringBuilder sb = new StringBuilder();
                //sb.Append(output.Text);
                //sb.Append(s);
                //sb.Append("/n");
                //output.Text = sb.ToString();
                output.Text += s;
                output.Text += "\n";
                sv.ScrollToEnd();
            }

            public void Write(object s)
            {
                output.Text += s.ToString();
                //StringBuilder sb = new StringBuilder();
                //sb.Append(output.Text);
                //sb.Append(s);
                //output.Text = sb.ToString();
            }
        }
    }
}
