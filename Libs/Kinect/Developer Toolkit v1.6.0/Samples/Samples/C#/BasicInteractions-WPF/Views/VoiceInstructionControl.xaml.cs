//------------------------------------------------------------------------------
// <copyright file="VoiceInstructionControl.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Microsoft.Speech.Recognition;

    /// <summary>
    /// Interaction logic for VoiceInstructionControl.xaml
    /// </summary>
    public partial class VoiceInstructionControl : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(VoiceInstructionControl), new UIPropertyMetadata(null, OnTextChanged));

        private Storyboard animateMicrophoneSelected;
        private Storyboard animateSpeechDetected;
        private bool isTextVisible;

        public VoiceInstructionControl()
        {
            this.InitializeComponent();
        }

        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        public void AnimateMicIntro()
        {
            var sb = this.Resources["EnableMic"] as Storyboard;

            if (sb != null)
            {
                this.Dispatcher.Invoke(new Action(() => sb.Begin()));
            }
        }

        internal void HideMic()
        {
            var sb = this.Resources["HideMic"] as Storyboard;

            if (sb != null)
            {
                this.Dispatcher.Invoke(new Action(() => sb.Begin()));
            }
        }

        private static void OnTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var self = o as VoiceInstructionControl;
            if (self != null)
            {
                self.UpdateText(e.NewValue as string);
            }
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            this.animateMicrophoneSelected = this.Resources["SelectMic"] as Storyboard;
            this.animateSpeechDetected = this.Resources["SpeechReco"] as Storyboard;

            KinectController.SpeechDetected += this.OnSpeechDetected;
        }


        private void OnSpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            if (this.animateSpeechDetected != null)
            {
                this.Dispatcher.Invoke(new Action(() => this.animateSpeechDetected.Begin()));
            }
        }

        private void UpdateText(string text)
        {
            Console.WriteLine(text);
            if (string.IsNullOrEmpty(text))
            {
                if (this.isTextVisible)
                {
                    var animateText = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5));
                    var animateMargin = new ThicknessAnimation(new Thickness(90, 0, 0, 0), TimeSpan.FromSeconds(0.5));
                    this.InstructionText.BeginAnimation(TextBlock.WidthProperty, animateText);
                    this.VuiBorder.BeginAnimation(Border.MarginProperty, animateMargin);
                    this.isTextVisible = false;
                }
            }
            else
            {
                var ft = new FormattedText(text + "    ", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(this.InstructionText.FontFamily, this.InstructionText.FontStyle, this.InstructionText.FontWeight, this.InstructionText.FontStretch), this.InstructionText.FontSize, this.InstructionText.Foreground);

                double width = ft.WidthIncludingTrailingWhitespace;

                if (this.isTextVisible == false)
                {
                    this.InstructionText.Text = text;
                    var animateText = new DoubleAnimation(width, TimeSpan.FromSeconds(0.5));
                    var animateMargin = new ThicknessAnimation(new Thickness(width + 90, 0, 0, 0), TimeSpan.FromSeconds(0.5));
                    this.InstructionText.BeginAnimation(TextBlock.WidthProperty, animateText);
                    this.VuiBorder.BeginAnimation(Border.MarginProperty, animateMargin);
                    this.isTextVisible = true;
                }
                else
                {
                    var animateOut = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5));
                    var animateMarginOut = new ThicknessAnimation(new Thickness(90, 0, 0, 0), TimeSpan.FromSeconds(0.5));
                    animateOut.Completed += (o, s) =>
                                                {
                                                    this.InstructionText.Text = text;
                                                    var animateIn = new DoubleAnimation(width, TimeSpan.FromSeconds(0.5));
                                                    var animateMargin = new ThicknessAnimation(new Thickness(width + 90, 0, 0, 0), TimeSpan.FromSeconds(0.5));
                                                    this.InstructionText.BeginAnimation(WidthProperty, animateIn);
                                                    this.VuiBorder.BeginAnimation(Border.MarginProperty, animateMargin);
                                                };
                    this.InstructionText.BeginAnimation(TextBlock.WidthProperty, animateOut);
                    this.VuiBorder.BeginAnimation(Border.MarginProperty, animateMarginOut);
                }

                if (this.animateMicrophoneSelected != null)
                {
                    this.animateMicrophoneSelected.Begin();
                }

                this.InstructionText.TextAlignment = TextAlignment.Center;
            }
        }
    }
}