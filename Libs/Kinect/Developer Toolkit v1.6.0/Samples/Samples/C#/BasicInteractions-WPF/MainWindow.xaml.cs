//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;
    using Microsoft.Samples.Kinect.BasicInteractions.Properties;
    using Microsoft.Speech.Recognition;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer vuiHideTimer;
        private DispatcherTimer vuiShowTimer;

        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
            App.Controller.PropertyChanged += this.KinectController_PropertyChanged;
            KinectController.SpeechRecognized += this.KinectController_SpeechRecognized;
            this.KeyUp += new System.Windows.Input.KeyEventHandler(this.MainWindow_KeyUp);
            this.Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
        }

        public void SetVoiceInstruction(string text, double delay)
        {
            if (this.vuiShowTimer != null)
            {
                this.vuiShowTimer.Stop();
                this.vuiShowTimer = null;
            }

            this.vuiShowTimer = new DispatcherTimer();
            this.vuiShowTimer.Interval = TimeSpan.FromMilliseconds(delay);
            this.vuiShowTimer.Tick += (o, s) =>
                                          {
                                              this.vuiShowTimer.Stop();
                                              this.VUI.Text = text;

                                              if (this.vuiHideTimer != null)
                                              {
                                                  this.vuiHideTimer.Stop();
                                                  this.vuiHideTimer = null;
                                              }

                                              this.vuiHideTimer = new DispatcherTimer();
                                              this.vuiHideTimer.Interval =
                                                  TimeSpan.FromMilliseconds(Settings.Default.VuiTextTimeout);
                                              this.vuiHideTimer.Tick += (o2, s2) =>
                                                                            {
                                                                                this.vuiHideTimer.Stop();
                                                                                this.vuiHideTimer = null;
                                                                                this.VUI.Text = string.Empty;
                                                                            };
                                              this.vuiHideTimer.Start();
                                          };
            this.vuiShowTimer.Start();
        }

        public void ShowCategorySelection()
        {
            this.CategorySelection.TransitionToGrid(this.AttractContent.SelectedItem);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // get the main screen size
            double height = System.Windows.SystemParameters.PrimaryScreenHeight;
            double width = System.Windows.SystemParameters.PrimaryScreenWidth;

            // if the main screen is not 1920 x 1080 then warn the user it is not the optimal experience 
            if (width != 1920 || height != 1080)
            {
                MessageBoxResult continueResult = MessageBox.Show("This screen is not 1920 x 1080.\nThis sample has been optimized for a screen resolution of 1920 x 1080.\nDo you wish to continue?", "Suboptimal Screen Resolution", MessageBoxButton.YesNo);
                if (continueResult == MessageBoxResult.No)
                {
                    this.Close();
                }
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            App.Controller.Dispose();
        }   

        private void KinectController_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HasActiveSkeleton")
            {
                if (App.Controller.HasActiveSkeleton)
                {
                    this.VUI.HideMic();
                    var t = new DispatcherTimer();
                    t.Interval = TimeSpan.FromMilliseconds(Settings.Default.ShowSilhouetteTime);
                    t.Tick += (o, s) =>
                                  {
                                      t.Stop();
                                      var hideStoryboard = this.Resources["HideContent"] as Storyboard;
                                      hideStoryboard.Begin(this.AttractContent);
                                      this.CategorySelection.SelectedCategory = null;
                                      this.CategorySelection.CategoryContent.SelectedItem = null;

                                      if (Settings.Default.ShowIntro)
                                      {
                                          this.AttractContent.Scale.CenterX = this.ActualWidth / 2;
                                          this.AttractContent.Scale.CenterY = this.ActualHeight / 2;
                                          this.SetVoiceInstruction(string.Empty, 0);
                                          this.IntroScreen.FadeIn();
                                      }
                                      else
                                      {
                                          this.IntroScreen.IsHandRaised = true;
                                          this.ShowCategorySelection();
                                          this.VUI.AnimateMicIntro();
                                      }
                                  };
                    t.Start();
                }
                else
                {
                    // as this property might be changed from a different thread
                    // it is necessary to use dispatcher invoke to ensure the animation 
                    // occurs on the render thread, otherwise it will fail
                    this.Dispatcher.Invoke(new Action(() => 
                        {
                                var showStoryboard =
                                    this.Resources["ShowContent"] as Storyboard;
                                showStoryboard.Begin(this.AttractContent);
                                this.CategorySelection.Visibility = Visibility.Hidden;
                                this.CategorySelection.SelectedCategory = null;
                                this.CategorySelection.CategoryContent.SelectedItem = null;
                                this.IntroScreen.IsHandRaised = false;
                          }));
                }
            }
        }

        private void KinectController_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string speechText = e.Result.Text;

            if (speechText.Equals(Settings.Default.SpeechSelectWord))
            {
                if (App.Controller.ActiveHand != null && App.Controller.ActiveHand.IsInteracting)
                {
                    var btn = App.Controller.ActiveHand.CurrentElement as HoverDwellButton;
                    if (btn != null)
                    {
                        btn.InvokeHoverClick(App.Controller.ActiveHand);
                    }
                }
            }

            if (this.IntroScreen.Visibility == System.Windows.Visibility.Visible)
            {
                // Expect start command
                this.IntroScreen.ProcessSpeech(speechText);
            }
            else if (this.CategorySelection.SelectedCategory == null)
            {
                // Expect a category
                this.CategorySelection.ProcessSpeech(speechText);
            }
            else if (this.CategorySelection.CategoryContent.SelectedItem == null)
            {
                // Expect a story
                this.CategorySelection.CategoryContent.ProcessSpeech(speechText);
            }
            else
            {
                // Expect a story related command
                this.CategorySelection.CategoryContent.Story.ProcessSpeech(speechText);
            }
        }

        private void MainWindow_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called from XAML not from CS")]
        private void OnVuiPreviewHandEnter(object sender, HandInputEventArgs e)
        {
            if (string.IsNullOrEmpty(this.VUI.Text))
            {
                if (this.IntroScreen.Visibility == Visibility.Visible)
                {
                    this.SetVoiceInstruction(Settings.Default.StartVui, 0);
                }
                else if (this.CategorySelection.SelectedCategory == null)
                {
                    this.SetVoiceInstruction(Settings.Default.SelectCategoryVui, 0);
                }
                else if (this.CategorySelection.CategoryContent.SelectedItem == null)
                {
                    this.SetVoiceInstruction(Settings.Default.SelectStoryVui, 0);
                }
                else
                {
                    this.SetVoiceInstruction(Settings.Default.ReadStoryVui, 0);
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called from XAML not from CS")]
        private void AttractControl_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            if (!Settings.Default.ShowIntro)
            {
                Point pt =
                    this.AttractContent.PointFromScreen(
                        this.CategorySelection.GetCenterOfSelectedItem(this.AttractContent.SelectedItem));

                this.AttractContent.Scale.CenterX = pt.X;
                this.AttractContent.Scale.CenterY = pt.Y;
            }
        }
    }
}