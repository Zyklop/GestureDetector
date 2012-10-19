//------------------------------------------------------------------------------
// <copyright file="StorySelectionControl.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;
    using Microsoft.Samples.Kinect.BasicInteractions.Properties;

    /// <summary>
    /// Interaction logic for CategoryControl.xaml
    /// </summary>
    public partial class StorySelectionControl : UserControl
    {
        public static readonly DependencyProperty CategoryProperty =
            DependencyProperty.Register("Category", typeof(Category), typeof(StorySelectionControl), new UIPropertyMetadata(null, OnCategoryChanged));
        
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(ContentItem), typeof(StorySelectionControl), new UIPropertyMetadata(null, OnSelectionChanged));

        public static readonly DependencyProperty SubcategoryProperty =
            DependencyProperty.Register("Subcategory", typeof(string), typeof(StorySelectionControl), new UIPropertyMetadata(null));

        // Using a DependencyProperty as the backing store for CanScrollLeft.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanScrollLeftProperty =
            DependencyProperty.Register("CanScrollLeft", typeof(Visibility), typeof(StorySelectionControl), new UIPropertyMetadata(Visibility.Hidden));

        // Using a DependencyProperty as the backing store for CanScrollLeft.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanScrollRightProperty =
            DependencyProperty.Register("CanScrollRight", typeof(Visibility), typeof(StorySelectionControl), new UIPropertyMetadata(Visibility.Hidden));


        private readonly DispatcherTimer scrollTimer;
        private double scrollingOffset;
        private bool scrollingRight;

        public StorySelectionControl()
        {
            this.InitializeComponent();

            if (Settings.Default.ShowScrollRegions)
            {
                this.scrollTimer = new DispatcherTimer();
                double scrollingTick = 100 - Settings.Default.ScrollSpeed;
                this.scrollTimer.Interval = TimeSpan.FromMilliseconds(scrollingTick);
                this.scrollTimer.Tick += this.ScrollTimer_Tick;
                KinectController.AddPreviewHandEnterHandler(this.LeftScrollRegion, this.OnPreviewHandEnterLeft);
                KinectController.AddPreviewHandLeaveHandler(this.LeftScrollRegion, this.OnPreviewHandLeaveLeft);
                KinectController.AddPreviewHandEnterHandler(this.RightScrollRegion, this.OnPreviewHandEnterRight);
                KinectController.AddPreviewHandLeaveHandler(this.RightScrollRegion, this.OnPreviewHandLeaveRight);
                ContentScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                this.ContentScrollViewer.ScrollChanged += new ScrollChangedEventHandler(this.ContentScrollViewer_ScrollChanged);
            }
            else
            {
                this.MainGrid.Children.Remove(this.LeftScrollRegion);
                this.MainGrid.Children.Remove(this.RightScrollRegion);
            }
        }

        public Visibility CanScrollRight
        {
            get { return (Visibility)GetValue(CanScrollRightProperty); }
            set { this.SetValue(CanScrollRightProperty, value); }
        }

        public Visibility CanScrollLeft
        {
            get { return (Visibility)GetValue(CanScrollLeftProperty); }
            set { this.SetValue(CanScrollLeftProperty, value); }
        }

        public Category Category
        {
            get { return (Category)this.GetValue(CategoryProperty); }
            set { this.SetValue(CategoryProperty, value); }
        }

        public string Subcategory
        {
            get { return (string)this.GetValue(SubcategoryProperty); }
            set { this.SetValue(SubcategoryProperty, value); }
        }

        public ContentItem SelectedItem
        {
            get { return (ContentItem)this.GetValue(SelectedItemProperty); }
            set { this.SetValue(SelectedItemProperty, value); }
        }

        public void AnimateIn(Duration duration)
        {
            var parent = Utility.FindParent<CategorySelectionControl>(this);
            var centerPt = new Point();
            if (parent != null)
            {
                centerPt = parent.GetCenterOfSelectedItem(this.Category);
            }

            Point buttonCenterPt = this.PointFromScreen(centerPt);

            // scale this down around the button point
            this.RenderTransform = new ScaleTransform(1, 1, buttonCenterPt.X, buttonCenterPt.Y);

            // scale it back up to full size
            var sb = new Storyboard();

            var animateOpacity = new DoubleAnimation(0, 1, duration);
            Storyboard.SetTarget(animateOpacity, this);
            Storyboard.SetTargetProperty(animateOpacity, new PropertyPath("Opacity"));
            sb.Children.Add(animateOpacity);

            var animateY = new DoubleAnimation(0.05, 1, duration);
            Storyboard.SetTarget(animateY, this);
            Storyboard.SetTargetProperty(animateY, new PropertyPath("RenderTransform.ScaleY"));
            sb.Children.Add(animateY);

            var animateX = new DoubleAnimation(0.05, 1, duration);
            Storyboard.SetTarget(animateX, this);
            Storyboard.SetTargetProperty(animateX, new PropertyPath("RenderTransform.ScaleX"));
            sb.Children.Add(animateX);

            this.IsHitTestVisible = true;
            ((MainWindow)Application.Current.MainWindow).SetVoiceInstruction(Settings.Default.SelectStoryVui, Settings.Default.VuiDisplayDelay);
            sb.Begin();
        }

        public void ProcessSpeech(string speechText)
        {
            if (speechText.Equals(Settings.Default.SpeechHomeWord) ||
                speechText.Equals(Settings.Default.SpeechBackWord))
            {
                this.HomeButton.InvokeHoverClick(null);
                ((MainWindow)Application.Current.MainWindow).SetVoiceInstruction(speechText, 0);
            }
            else
            {
                string subcategory = this.Category.Subcategories.FirstOrDefault(sc => sc.Equals(speechText));
                if (null != subcategory)
                {
                    var element =
                        this.Subcategories.ItemContainerGenerator.ContainerFromItem(subcategory) as ContentPresenter;
                    if (null != element)
                    {
                        if (VisualTreeHelper.GetChildrenCount(element) > 0)
                        {
                            var selectedButton = VisualTreeHelper.GetChild(element, 0) as HoverDwellButton;
                            selectedButton.InvokeHoverClick(null);
                            ((MainWindow)Application.Current.MainWindow).SetVoiceInstruction(speechText, 0);
                        }
                    }
                }
                else
                {
                    int storyNumber = 0;
                    NumberWords numberWord;
                    if (Enum.TryParse(speechText, out numberWord))
                    {
                        storyNumber = (int)numberWord;
                        if (storyNumber <= this.Category.Content.Count)
                        {
                            foreach (ContentItem story in this.Category.Content)
                            {
                                if (story.ItemId == storyNumber)
                                {
                                    var element =
                                        this.ContentItems.ItemContainerGenerator.ContainerFromItem(story) as
                                        ContentPresenter;
                                    if (null != element)
                                    {
                                        if (VisualTreeHelper.GetChildrenCount(element) > 0)
                                        {
                                            var selectedButton = VisualTreeHelper.GetChild(element, 0) as HoverDwellButton;
                                            selectedButton.InvokeHoverClick(null);
                                        }
                                    }

                                    ((MainWindow)Application.Current.MainWindow).SetVoiceInstruction(storyNumber.ToString(CultureInfo.CurrentCulture), 0);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AnimateOut(Duration duration)
        {
            var sb = new Storyboard();

            var animateOpacity = new DoubleAnimation(0, duration);
            Storyboard.SetTarget(animateOpacity, this);
            Storyboard.SetTargetProperty(animateOpacity, new PropertyPath("Opacity"));
            sb.Children.Add(animateOpacity);
            sb.FillBehavior = FillBehavior.Stop;

            var animateY = new DoubleAnimation(1, 0.05, duration);
            Storyboard.SetTarget(animateY, this);
            Storyboard.SetTargetProperty(animateY, new PropertyPath("RenderTransform.ScaleY"));
            sb.Children.Add(animateY);

            var animateX = new DoubleAnimation(1, 0.05, duration);
            Storyboard.SetTarget(animateX, this);
            Storyboard.SetTargetProperty(animateX, new PropertyPath("RenderTransform.ScaleX"));
            sb.Children.Add(animateX);

            this.IsHitTestVisible = false;
            sb.Begin();
        }

        public void FadeOut(Duration duration)
        {
            this.MainGrid.IsHitTestVisible = false;
            var animateOpacity = new DoubleAnimation(0, duration);
            this.MainGrid.BeginAnimation(StorySelectionControl.OpacityProperty, animateOpacity);
        }

        public void FadeIn(Duration duration)
        {
            this.MainGrid.IsHitTestVisible = true;
            var animateOpacity = new DoubleAnimation(0, 1, duration);
            this.MainGrid.BeginAnimation(StorySelectionControl.OpacityProperty, animateOpacity);
            ((MainWindow)Application.Current.MainWindow).SetVoiceInstruction(Settings.Default.SelectStoryVui, Settings.Default.VuiDisplayDelay);
        }

        internal Point GetCenterOfSelectedItem(object selectedItem)
        {
            var pt = new Point();
            if (null != selectedItem)
            {
                var element =
                    this.ContentItems.ItemContainerGenerator.ContainerFromItem(selectedItem) as ContentPresenter;
                if (null != element)
                {
                    pt = element.PointToScreen(new Point(element.ActualWidth / 2, element.ActualHeight / 2));
                }
            }

            return pt;
        }

        private static void OnCategoryChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var self = o as StorySelectionControl;
            if (null != self)
            {
                self.Subcategory = Settings.Default.SubcategoryAll;
                var category = e.NewValue as Category;
                if (null != category)
                {
                    if (category.Subcategories.Count > 1)
                    {
                        self.SubCategoryRow.Height = GridLength.Auto;
                    }
                    else
                    {
                        self.SubCategoryRow.Height = new GridLength(0);
                    }

                    self.ContentScrollViewer.ScrollToLeftEnd();
                    self.AnimateIn(TimeSpan.FromSeconds(0.5));
                }
                else
                {
                    self.AnimateOut(TimeSpan.FromSeconds(0.5));
                }
            }
        }

        private static void OnSelectionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var self = o as StorySelectionControl;
            if (null != self)
            {
                if (null == e.NewValue)
                {
                    self.FadeIn(TimeSpan.FromSeconds(0.5));
                }
                else
                {
                    self.FadeOut(TimeSpan.FromSeconds(0.5));
                }
            }
        }

        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            this.scrollingOffset += this.scrollingRight ? 20 : -20;
            if (this.scrollingOffset < 0)
            {
                this.scrollingOffset = 0;
            }

            this.ContentScrollViewer.ScrollToHorizontalOffset(this.scrollingOffset);
        }

        private void ContentScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.CanScrollRight = (this.ContentScrollViewer.HorizontalOffset + this.ContentScrollViewer.ViewportWidth < this.ContentScrollViewer.ExtentWidth) ? Visibility.Visible : Visibility.Hidden;
            this.CanScrollLeft = (this.ContentScrollViewer.HorizontalOffset <= 0) ? Visibility.Hidden : Visibility.Visible;
        }

        private void OnPreviewHandEnterLeft(object sender, HandInputEventArgs args)
        {
            // start scrolling left
            this.LeftScrollRegion.Background = Application.Current.Resources["ActiveScrollRegionBrush"] as Brush;
            this.scrollingOffset = this.ContentScrollViewer.HorizontalOffset;
            this.scrollingRight = false;
            this.scrollTimer.Start();
            ContentScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void OnPreviewHandLeaveLeft(object sender, HandInputEventArgs args)
        {
            // stop scrolling left
            this.LeftScrollRegion.Background = Application.Current.Resources["InactiveScrollRegionBrush"] as Brush;
            this.scrollTimer.Stop();
            ContentScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private void OnPreviewHandEnterRight(object sender, HandInputEventArgs args)
        {
            // start scrolling left
            this.RightScrollRegion.Background = Application.Current.Resources["ActiveScrollRegionBrush"] as Brush;
            this.scrollingOffset = this.ContentScrollViewer.HorizontalOffset;
            this.scrollingRight = true;
            this.scrollTimer.Start();
            ContentScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void OnPreviewHandLeaveRight(object sender, HandInputEventArgs args)
        {
            // stop scrolling left
            this.RightScrollRegion.Background = Application.Current.Resources["InactiveScrollRegionBrush"] as Brush;
            this.scrollTimer.Stop();
            ContentScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private void StoryButtonClick(object sender, HandInputEventArgs e)
        {
            var currentItem = ((FrameworkElement)sender).DataContext as ContentItem;
            if (null != currentItem)
            {
                this.SelectedItem = currentItem;
            }
        }

        private void SubcategoryClick(object sender, HandInputEventArgs e)
        {
            var btn = sender as HoverDwellButton;
            if (null != btn)
            {
                var subcategory = btn.DataContext as string;
                if (null != subcategory)
                {
                    this.Subcategory = subcategory;
                    this.ContentScrollViewer.ScrollToLeftEnd();
                }
            }
        }

        private void HomeButton_HoverClick(object sender, HandInputEventArgs e)
        {
            this.Category = null;
            ((MainWindow)Application.Current.MainWindow).SetVoiceInstruction(Settings.Default.SelectCategoryVui, Settings.Default.VuiDisplayDelay);
        }
    }
}