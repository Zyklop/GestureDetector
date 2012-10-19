//------------------------------------------------------------------------------
// <copyright file="StoryControl.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;
    using Microsoft.Samples.Kinect.BasicInteractions.Properties;

    /// <summary>
    /// Interaction logic for StoryControl.xaml
    /// </summary>
    public partial class StoryControl : UserControl
    {
        public static readonly DependencyProperty StoryProperty = 
            DependencyProperty.Register("Story", typeof(ContentItem), typeof(StoryControl), new UIPropertyMetadata(null, OnStoryChanged));

        // Using a DependencyProperty as the backing store for ScrollUpVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollUpVisibilityProperty =
            DependencyProperty.Register("ScrollUpVisibility", typeof(Visibility), typeof(StoryControl), new UIPropertyMetadata(Visibility.Hidden));

        // Using a DependencyProperty as the backing store for ScrollUpVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollDownVisibilityProperty =
            DependencyProperty.Register("ScrollDownVisibility", typeof(Visibility), typeof(StoryControl), new UIPropertyMetadata(Visibility.Hidden));


        private readonly DispatcherTimer scrollTimer;
        private Storyboard displayStoryboard;
        private double scrollingOffset;
        private bool scrollingUp;

        public StoryControl()
        {
            this.InitializeComponent();

            if (Settings.Default.ShowScrollRegions)
            {
                this.scrollTimer = new DispatcherTimer();
                double scrollingTick = 100 - Settings.Default.ScrollSpeed;
                this.scrollTimer.Interval = TimeSpan.FromMilliseconds(scrollingTick);
                this.scrollTimer.Tick += this.ScrollTimer_Tick;
                KinectController.AddPreviewHandEnterHandler(this.UpScrollRegion, this.OnPreviewHandEnterUp);
                KinectController.AddPreviewHandLeaveHandler(this.UpScrollRegion, this.OnPreviewHandLeaveUp);
                KinectController.AddPreviewHandEnterHandler(this.DownScrollRegion, this.OnPreviewHandEnterDown);
                KinectController.AddPreviewHandLeaveHandler(this.DownScrollRegion, this.OnPreviewHandLeaveDown);
                ContentScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                this.ContentScrollViewer.ScrollChanged += new ScrollChangedEventHandler(this.ContentScrollViewer_ScrollChanged);
            }
            else
            {
                this.MainGrid.Children.Remove(this.UpScrollRegion);
                this.MainGrid.Children.Remove(this.DownScrollRegion);
            }
        }

        public ContentItem Story
        {
            get { return (ContentItem)this.GetValue(StoryProperty); }
            set { this.SetValue(StoryProperty, value); }
        }

        public Visibility ScrollDownVisibility
        {
            get { return (Visibility)GetValue(ScrollDownVisibilityProperty); }
            set { this.SetValue(ScrollDownVisibilityProperty, value); }
        }

        public Visibility ScrollUpVisibility
        {
            get { return (Visibility)GetValue(ScrollUpVisibilityProperty); }
            set { this.SetValue(ScrollUpVisibilityProperty, value); }
        }

        
        public void AnimateIn(Duration duration)
        {
            var parent = Utility.FindParent<StorySelectionControl>(this);
            var centerPt = new Point();
            if (parent != null)
            {
                centerPt = parent.GetCenterOfSelectedItem(this.Story);
            }

            Point buttonCenterPt = this.PointFromScreen(centerPt);

            this.ContentScrollViewer.ScrollToTop();

            // scale this down around the button point
            var st = new ScaleTransform(1, 1, buttonCenterPt.X, buttonCenterPt.Y);

            this.RenderTransform = st;

            if (this.displayStoryboard != null)
            {
                this.displayStoryboard.Stop();
                this.displayStoryboard = null;
            }

            // scale it back up to full size
            this.displayStoryboard = new Storyboard();

            var animateY = new DoubleAnimation(0.05, 1, duration);
            Storyboard.SetTarget(animateY, this);
            Storyboard.SetTargetProperty(animateY, new PropertyPath("RenderTransform.ScaleY"));
            this.displayStoryboard.Children.Add(animateY);

            var animateX = new DoubleAnimation(0.05, 1, duration);
            Storyboard.SetTarget(animateX, this);
            Storyboard.SetTargetProperty(animateX, new PropertyPath("RenderTransform.ScaleX"));
            this.displayStoryboard.Children.Add(animateX);

            this.Visibility = Visibility.Visible;
            this.displayStoryboard.Begin();

            this.IsHitTestVisible = true;
            ((MainWindow)Application.Current.MainWindow).SetVoiceInstruction(Settings.Default.ReadStoryVui, Settings.Default.VuiDisplayDelay);
        }

        public void ProcessSpeech(string speechText)
        {
            if (speechText.Equals(Settings.Default.SpeechLikeWord))
            {
                this.Story.Rating.Likes++;
                this.ContentScrollViewer.ScrollToBottom();
                ((MainWindow)Application.Current.MainWindow).SetVoiceInstruction(speechText, 0);
            }
            else if (speechText.Equals(Settings.Default.SpeechDislikeWord))
            {
                this.Story.Rating.Dislikes++;
                this.ContentScrollViewer.ScrollToBottom();
                ((MainWindow)Application.Current.MainWindow).SetVoiceInstruction(speechText, 0);
            }
            else if (speechText.Equals(Settings.Default.SpeechHomeWord))
            {
                this.HomeButton.InvokeHoverClick(null);
                ((MainWindow)Application.Current.MainWindow).SetVoiceInstruction(speechText, 0);
            }
            else if (speechText.Equals(Settings.Default.SpeechBackWord))
            {
                this.BackButton.InvokeHoverClick(null);
                ((MainWindow)Application.Current.MainWindow).SetVoiceInstruction(speechText, 0);
            }
        }

        public void AnimateOut(Duration duration)
        {
            if (this.displayStoryboard != null)
            {
                this.displayStoryboard.Stop();
                this.displayStoryboard = null;
            }

            if (this.Visibility != Visibility.Visible)
            {
                return;
            }

            this.displayStoryboard = new Storyboard();

            var animateOpacity = new DoubleAnimation(0, duration);
            Storyboard.SetTarget(animateOpacity, this);
            Storyboard.SetTargetProperty(animateOpacity, new PropertyPath("Opacity"));
            this.displayStoryboard.Children.Add(animateOpacity);
            this.displayStoryboard.FillBehavior = FillBehavior.Stop;

            var animateY = new DoubleAnimation(1, 0.05, duration);
            Storyboard.SetTarget(animateY, this);
            Storyboard.SetTargetProperty(animateY, new PropertyPath("RenderTransform.ScaleY"));
            this.displayStoryboard.Children.Add(animateY);

            var animateX = new DoubleAnimation(1, 0.05, duration);
            Storyboard.SetTarget(animateX, this);
            Storyboard.SetTargetProperty(animateX, new PropertyPath("RenderTransform.ScaleX"));
            this.displayStoryboard.Children.Add(animateX);

            this.displayStoryboard.Completed += this.AnimateOutCompleted;

            this.IsHitTestVisible = false;
            this.displayStoryboard.Begin();
        }

        private static void OnStoryChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var self = o as StoryControl;
            if (self != null)
            {
                var story = e.NewValue as ContentItem;
                if (story != null)
                {
                    self.ContentScrollViewer.ScrollToTop();
                    self.AnimateIn(TimeSpan.FromSeconds(0.5));
                }
                else
                {
                    self.AnimateOut(TimeSpan.FromSeconds(0.5));
                }
            }
        }

        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            this.scrollingOffset += this.scrollingUp ? -20 : 20;
            if (this.scrollingOffset < 0)
            {
                this.scrollingOffset = 0;
            }

            this.ContentScrollViewer.ScrollToVerticalOffset(this.scrollingOffset);
        }

        private void ContentScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.ScrollUpVisibility = (this.ContentScrollViewer.VerticalOffset <= 0) ? Visibility.Hidden : Visibility.Visible;
            this.ScrollDownVisibility = (this.ContentScrollViewer.VerticalOffset + this.ContentScrollViewer.ViewportHeight < this.ContentScrollViewer.ExtentHeight) ? Visibility.Visible : Visibility.Hidden;
        }

        private void OnPreviewHandEnterUp(object sender, HandInputEventArgs args)
        {
            // start scrolling left
            this.UpScrollRegion.Background = Application.Current.Resources["ActiveScrollRegionBrush"] as Brush;
            this.scrollingOffset = this.ContentScrollViewer.VerticalOffset;
            this.scrollingUp = true;
            this.scrollTimer.Start();
            ContentScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void OnPreviewHandLeaveUp(object sender, HandInputEventArgs args)
        {
            // stop scrolling left
            this.UpScrollRegion.Background = Application.Current.Resources["InactiveScrollRegionBrush"] as Brush; 
            this.scrollTimer.Stop();
            ContentScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private void OnPreviewHandEnterDown(object sender, HandInputEventArgs args)
        {
            // start scrolling left
            this.DownScrollRegion.Background = Application.Current.Resources["ActiveScrollRegionBrush"] as Brush;
            this.scrollingOffset = this.ContentScrollViewer.VerticalOffset;
            this.scrollingUp = false;
            this.scrollTimer.Start();
            ContentScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void OnPreviewHandLeaveDown(object sender, HandInputEventArgs args)
        {
            // stop scrolling left
            this.DownScrollRegion.Background = Application.Current.Resources["InactiveScrollRegionBrush"] as Brush;
            this.scrollTimer.Stop();
            ContentScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }
        
        private void BackButtonHoverClick(object sender, HandInputEventArgs e)
        {
            this.Story = null;
        }

        private void HomeButtonHoverClick(object sender, HandInputEventArgs e)
        {
            this.Story = null;
            var categorySelection = Utility.FindParent<CategorySelectionControl>(this);
            if (categorySelection != null)
            {
                categorySelection.SelectedCategory = null;
                ((MainWindow)Application.Current.MainWindow).SetVoiceInstruction(Settings.Default.SelectCategoryVui, Settings.Default.VuiDisplayDelay);
            }
        }

        private void ThumbUpClick(object sender, HandInputEventArgs e)
        {
            this.Story.Rating.Likes += 1;
        }

        private void ThumbDownClick(object sender, HandInputEventArgs e)
        {
            this.Story.Rating.Dislikes += 1;
        }

        private void AnimateOutCompleted(object sender, EventArgs e)
        {
            if (this.displayStoryboard != null)
            {
                this.displayStoryboard.Stop();
                this.displayStoryboard = null;
            }

            this.Visibility = Visibility.Collapsed;
            this.Opacity = 1;
        }
    }
}