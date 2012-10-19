//------------------------------------------------------------------------------
// <copyright file="HoverDwellButton.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Media;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using Microsoft.Samples.Kinect.BasicInteractions.Properties;

    public class HoverDwellButton : ContentControl, IDisposable
    {
        public static readonly RoutedEvent HoverClickEvent = EventManager.RegisterRoutedEvent("HoverClick", RoutingStrategy.Bubble, typeof(EventHandler<HandInputEventArgs>), typeof(HoverDwellButton));

        public static readonly DependencyProperty IsHoveredOverProperty =
            DependencyProperty.Register("IsHoveredOver", typeof(bool), typeof(HoverDwellButton), new UIPropertyMetadata(false));

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(HoverDwellButton), new UIPropertyMetadata(false));

        // Using a DependencyProperty as the backing store for Magnetic.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MagneticProperty =
            DependencyProperty.Register("Magnetic", typeof(bool), typeof(HoverDwellButton), new UIPropertyMetadata(false));

        // Using a DependencyProperty as the backing store for SoundOnEnter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SoundOnEnterProperty =
            DependencyProperty.Register("SoundOnEnter", typeof(string), typeof(HoverDwellButton), new UIPropertyMetadata(string.Empty, OnSoundOnEnterChanged));


        // Using a DependencyProperty as the backing store for SoundOnClick.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SoundOnClickProperty =
            DependencyProperty.Register("SoundOnClick", typeof(string), typeof(HoverDwellButton), new UIPropertyMetadata(string.Empty, OnSoundOnClickChanged));

        private readonly List<HandHoverTimer> trackedHandHovers = new List<HandHoverTimer>();
        private SoundPlayer soundPlayerOnClick;
        private SoundPlayer soundPlayerOnEnter;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Overriding metadata must occur within a static constructor")]
        static HoverDwellButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HoverDwellButton), new FrameworkPropertyMetadata(typeof(HoverDwellButton)));
        }

        public HoverDwellButton()
        {
            KinectController.AddPreviewHandEnterHandler(this, this.OnPreviewHandEnter);
            KinectController.AddPreviewHandLeaveHandler(this, this.OnPreviewHandLeave);
        }

        public event EventHandler<HandInputEventArgs> HoverClick
        {
            add { this.AddHandler(HoverClickEvent, value); }
            remove { this.RemoveHandler(HoverClickEvent, value); }
        }

        public bool IsHoveredOver
        {
            get { return (bool)this.GetValue(IsHoveredOverProperty); }
            set { this.SetValue(IsHoveredOverProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)this.GetValue(IsSelectedProperty); }
            set { this.SetValue(IsSelectedProperty, value); }
        }

        public bool Magnetic
        {
            get { return (bool)this.GetValue(MagneticProperty); }
            set { this.SetValue(MagneticProperty, value); }
        }

        public string SoundOnEnter
        {
            get { return (string)this.GetValue(SoundOnEnterProperty); }
            set { this.SetValue(SoundOnEnterProperty, value); }
        }

        public string SoundOnClick
        {
            get { return (string)this.GetValue(SoundOnClickProperty); }
            set { this.SetValue(SoundOnClickProperty, value); }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        

       

        public void InvokeHoverClick(HandPosition hand)
        {
            if (hand != null)
            {
                hand.IsInteracting = false;

                HandHoverTimer timer = this.trackedHandHovers.FirstOrDefault(h => h.Hand.Equals(hand));
                if (timer != null)
                {
                    timer.Stop();
                    this.trackedHandHovers.Remove(timer);
                }
            }

            this.IsSelected = true;

            if (this.soundPlayerOnClick != null)
            {
                this.soundPlayerOnClick.Play();
            }

            var t = new DispatcherTimer();
            t.Interval = TimeSpan.FromSeconds(0.6);

            t.Tick += (o, s) =>
                          {
                              t.Stop();
                              var clickArgs = new HandInputEventArgs(HoverClickEvent, this, hand);
                              this.RaiseEvent(clickArgs);
                              this.IsSelected = false;
                          };
            t.Start();
        }

        public TimeSpan GetTimeRemaining(HandPosition hand)
        {
            HandHoverTimer timer = this.trackedHandHovers.FirstOrDefault(h => h.Hand.Equals(hand));
            if (timer != null)
            {
                return timer.TimeRemaining;
            }

            return TimeSpan.MaxValue;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.soundPlayerOnEnter != null)
                {
                    this.soundPlayerOnEnter.Dispose();
                    this.soundPlayerOnEnter = null;
                }

                if (this.soundPlayerOnClick != null)
                {
                    this.soundPlayerOnClick.Dispose();
                    this.soundPlayerOnClick = null;
                }

                KinectController.RemovePreviewHandEnterHandler(this, this.OnPreviewHandEnter);
                KinectController.RemovePreviewHandLeaveHandler(this, this.OnPreviewHandLeave);
            }
        }

        private static void OnSoundOnEnterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var sound = e.NewValue as string;
            var self = o as HoverDwellButton;
            if (self.soundPlayerOnEnter != null)
            {
                self.soundPlayerOnEnter.Dispose();
                self.soundPlayerOnEnter = null;
            }

            if (!string.IsNullOrEmpty(sound))
            {
                Assembly a = Assembly.GetExecutingAssembly();
                Stream s = a.GetManifestResourceStream("Microsoft.Samples.Kinect.BasicInteractions.Resources.Sounds." + sound);
                self.soundPlayerOnEnter = new SoundPlayer(s);
            }
        }

        private static void OnSoundOnClickChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var sound = e.NewValue as string;
            var self = o as HoverDwellButton;
            if (self.soundPlayerOnClick != null)
            {
                self.soundPlayerOnClick.Dispose();
                self.soundPlayerOnClick = null;
            }

            if (!string.IsNullOrEmpty(sound))
            {
                Assembly a = Assembly.GetExecutingAssembly();
                Stream s = a.GetManifestResourceStream("Microsoft.Samples.Kinect.BasicInteractions.Resources.Sounds." + sound);
                self.soundPlayerOnClick = new SoundPlayer(s);
            }
        }

        private void OnPreviewHandEnter(object sender, HandInputEventArgs args)
        {
            if (this.trackedHandHovers.FirstOrDefault(t => t.Hand.Equals(args.Hand)) == null)
            {
                this.IsHoveredOver = true;
                if (this.SoundOnEnter.Length > 0)
                {
                    if (this.soundPlayerOnEnter == null)
                    {
                        Assembly a = Assembly.GetExecutingAssembly();

                        Stream s = a.GetManifestResourceStream("Microsoft.Samples.Kinect.BasicInteractions.Resources.Sounds." + this.SoundOnEnter);

                        this.soundPlayerOnEnter = new SoundPlayer(s);
                    }

                    this.soundPlayerOnEnter.Play();
                }

                args.Hand.IsInteracting = true;
                if (this.Magnetic)
                {
                    // set the X and Y of the hand so it is centered over the button
                    var element = this.Content as FrameworkElement;

                    if (element != null)
                    {
                        var pt = new Point(element.ActualWidth / 2, element.ActualHeight / 2);
                        Point lockedPoint = element.PointToScreen(pt);
                        args.Hand.X = (int)lockedPoint.X;
                        args.Hand.Y = (int)lockedPoint.Y;
                    }

                    args.Hand.Magnetized = true;
                }

                var timer = new HandHoverTimer(DispatcherPriority.Normal, this.Dispatcher);
                timer.Hand = args.Hand;

                timer.Interval = TimeSpan.FromMilliseconds(Settings.Default.SelectionTime);
                timer.Tick += (o, s) => { this.InvokeHoverClick(args.Hand); };
                this.trackedHandHovers.Add(timer);
                timer.Start();
            }

            args.Handled = true;
        }

        private void OnPreviewHandLeave(object sender, HandInputEventArgs args)
        {
            if (Application.Current != null
                && Application.Current.MainWindow != null)
            {
                IInputElement result = Application.Current.MainWindow.InputHitTest(new Point(args.Hand.X, args.Hand.Y));
                var parent = Utility.FindParent<HoverDwellButton>(result);
                if (parent != this)
                {
                    this.RemoveHand(args.Hand);
                    args.Handled = true;
                }
            }
        }

        private void RemoveHand(HandPosition hand)
        {
            this.IsHoveredOver = false;
            hand.IsInteracting = false;
            hand.Magnetized = false;

            // Stop active hover timer (if it exists) for this hand.
            HandHoverTimer timer = this.trackedHandHovers.FirstOrDefault(h => h.Hand.Equals(hand));
            if (timer != null)
            {
                timer.Stop();
                this.trackedHandHovers.Remove(timer);
            }
        }
    }
}