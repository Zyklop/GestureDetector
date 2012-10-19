//------------------------------------------------------------------------------
// <copyright file="Introduction.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;
    using Microsoft.Samples.Kinect.BasicInteractions.Properties;

    /// <summary>
    /// Interaction logic for Introduction.xaml
    /// </summary>
    public partial class Introduction : UserControl
    {
        public static readonly DependencyProperty SpeechHintTextProperty =
            DependencyProperty.Register("SpeechHintText", typeof(string), typeof(Introduction), new UIPropertyMetadata("Say 'start' or"));

        public static readonly DependencyProperty ButtonHintTextProperty =
            DependencyProperty.Register("ButtonHintText", typeof(string), typeof(Introduction), new UIPropertyMetadata("Hold your hand over the start button"));

        public static readonly DependencyProperty IsHandRaisedProperty =
            DependencyProperty.Register("IsHandRaised", typeof(bool), typeof(Introduction), new UIPropertyMetadata(false));

        public Introduction()
        {
            this.InitializeComponent();
            this.SpeechHintText = Settings.Default.IntroSpeechText;
            this.ButtonHintText = Settings.Default.IntroButtonText;
        }

        public string SpeechHintText
        {
            get { return (string)this.GetValue(SpeechHintTextProperty); }
            set { this.SetValue(SpeechHintTextProperty, value); }
        }
 
        public string ButtonHintText
        {
            get { return (string)this.GetValue(ButtonHintTextProperty); }
            set { this.SetValue(ButtonHintTextProperty, value); }
        }

        public bool IsHandRaised
        {
            get { return (bool)GetValue(IsHandRaisedProperty); }
            set { this.SetValue(IsHandRaisedProperty, value); }
        }


        public void ProcessSpeech(string speechText)
        {
            if (speechText == Settings.Default.SpeechStartWord)
            {
                ((MainWindow)Application.Current.MainWindow).SetVoiceInstruction(" " + speechText + " ", 0);
                this.IsHandRaised = true;
                this.ShowCategorySelection();
            }
        }

        internal void FadeIn()
        {
            var animateOpacity = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
            animateOpacity.Completed += (o, e) =>
                                {
                                    var mainWin = Application.Current.MainWindow as MainWindow;
                                    mainWin.VUI.AnimateMicIntro();
                                    KinectController.AddHandRaisedHandler(this, new EventHandler<HandInputEventArgs>(OnHandRaised));
                                    ((MainWindow)Application.Current.MainWindow).SetVoiceInstruction(Settings.Default.StartVui, Settings.Default.VuiDisplayDelay);
                                };

            this.Visibility = Visibility.Visible;

            this.BeginAnimation(Introduction.OpacityProperty, animateOpacity);
        }

        private void ShowSpeechText()
        {
            var animateSpeechOpacity = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(750));
            animateSpeechOpacity.FillBehavior = FillBehavior.Stop;
            animateSpeechOpacity.BeginTime = TimeSpan.FromMilliseconds(1000);
            animateSpeechOpacity.Completed += (o, s) => SpeechText.Opacity = 1;

            this.SpeechText.BeginAnimation(Introduction.OpacityProperty, animateSpeechOpacity);
        }

        private void OnHandRaised(object sender, HandInputEventArgs args)
        {
            this.IsHandRaised = true;
            KinectController.RemoveHandRaisedHandler(this, this.OnHandRaised);
        }

        private void StartButtonHoverClick(object sender, HandInputEventArgs e)
        {
            this.ShowCategorySelection();
        }

        private void ShowCategorySelection()
        {
            // hide this screen and show the category selection screen
            var animateOpacity = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));
            animateOpacity.Completed += (o, e) =>
            {
                var mainWin = Application.Current.MainWindow as MainWindow;
                mainWin.ShowCategorySelection();
                this.Visibility = Visibility.Collapsed;
            };
            this.BeginAnimation(Introduction.OpacityProperty, animateOpacity);
            KinectController.RemoveHandRaisedHandler(this, new EventHandler<HandInputEventArgs>(this.OnHandRaised));
        }
    }
}