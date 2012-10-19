//------------------------------------------------------------------------------
// <copyright file="CategorySelectionControl.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Microsoft.Samples.Kinect.BasicInteractions.Properties;

    /// <summary>
    /// Interaction logic for CategorySelectionControl.xaml
    /// </summary>
    public partial class CategorySelectionControl : UserControl
    {
        public static readonly DependencyProperty CategoriesProperty = 
            DependencyProperty.Register("Categories", typeof(IEnumerable<Category>), typeof(CategorySelectionControl), new UIPropertyMetadata(null));

        public static readonly DependencyProperty SelectedCategoryProperty =
            DependencyProperty.Register("SelectedCategory", typeof(Category), typeof(CategorySelectionControl), new UIPropertyMetadata(null));

        public CategorySelectionControl()
        {
            this.InitializeComponent();
        }

        public IEnumerable<Category> Categories
        {
            get { return (IEnumerable<Category>)this.GetValue(CategoriesProperty); }
            set { this.SetValue(CategoriesProperty, value); }
        }

        public Category SelectedCategory
        {
            get { return (Category)this.GetValue(SelectedCategoryProperty); }
            set { this.SetValue(SelectedCategoryProperty, value); }
        }

        public void TransitionToGrid(object selectedItem)
        {
            ((MainWindow)Application.Current.MainWindow).SetVoiceInstruction(string.Empty, 0);
            this.Visibility = Visibility.Visible;
            this.IsHitTestVisible = false;

            // move all grid items to position of selected item
            var pt = new Point();
            var li = this.CategoryListBox.ItemContainerGenerator.ContainerFromItem(selectedItem) as ListBoxItem;
            if (li != null)
            {
                pt = li.TranslatePoint(new Point(li.ActualWidth / 2, li.ActualHeight / 2), this);
            }

            var sb = new Storyboard();

            double speedFactor = 0.0005;
            foreach (Category item in this.CategoryListBox.Items)
            {
                li = this.CategoryListBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                if (li != null)
                {
                    Point itemPt = li.TranslatePoint(new Point(li.ActualWidth / 2, li.ActualHeight / 2), this);

                    double diffX = pt.X - itemPt.X;
                    double diffY = pt.Y - itemPt.Y;

                    li.RenderTransform = new TranslateTransform(diffX, diffY);

                    var d = new Duration(TimeSpan.FromSeconds(Math.Abs(diffY) * speedFactor));

                    var animateY = new DoubleAnimation(0, d);
                    animateY.BeginTime = TimeSpan.FromSeconds(0.5);

                    Storyboard.SetTarget(animateY, li);
                    Storyboard.SetTargetProperty(animateY, new PropertyPath("RenderTransform.Y"));
                    sb.Children.Add(animateY);

                    var d2 = new Duration(TimeSpan.FromSeconds(Math.Abs(diffX) * speedFactor));
                    var animateX = new DoubleAnimation(0, d2);
                    animateX.BeginTime = d.TimeSpan;
                    Storyboard.SetTarget(animateX, li);
                    Storyboard.SetTargetProperty(animateX, new PropertyPath("RenderTransform.X"));

                    sb.Children.Add(animateX);

                    var d3 = new Duration(d.TimeSpan + d2.TimeSpan);
                    var animateOpacity = new DoubleAnimation(1, d3);
                    animateOpacity.BeginTime = animateY.BeginTime;
                    Storyboard.SetTarget(animateOpacity, li);
                    Storyboard.SetTargetProperty(animateOpacity, new PropertyPath("Opacity"));

                    sb.Children.Add(animateOpacity);
                }
            }

            sb.Completed += this.TransitionToGridCompleted;
            sb.Begin();
        }

        internal Point GetCenterOfSelectedItem(object selectedItem)
        {
            var pt = new Point();
            if (selectedItem != null)
            {
                var li = this.CategoryListBox.ItemContainerGenerator.ContainerFromItem(selectedItem) as ListBoxItem;
                if (li != null)
                {
                    pt = li.PointToScreen(new Point(li.ActualWidth / 2, li.ActualHeight / 2));
                }
            }

            return pt;
        }

        internal void ProcessSpeech(string speechText)
        {
            Category category =
                this.Categories.FirstOrDefault(
                c => c.Title.Equals(speechText) || c.Title.Equals(speechText.Replace("and", "&")));
            if (category != null)
            {
                var li = this.CategoryListBox.ItemContainerGenerator.ContainerFromItem(category) as ListBoxItem;
                if (li != null)
                {
                    var selectedButton = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(li, 0), 0) as HoverDwellButton;
                    if (selectedButton != null)
                    {
                        selectedButton.InvokeHoverClick(null);
                        ((MainWindow)Application.Current.MainWindow).SetVoiceInstruction(speechText, 0);
                    }
                }
            }
        }

        private void TransitionToGridCompleted(object sender, EventArgs e)
        {
            this.IsHitTestVisible = true;
            ((MainWindow)Application.Current.MainWindow).SetVoiceInstruction(Settings.Default.SelectCategoryVui, Settings.Default.VuiDisplayDelay);
        }

        private void ImageDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var img = sender as Image;
            if (img != null)
            {
                var animateOpacity = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(300)));
                animateOpacity.AutoReverse = true;
                animateOpacity.FillBehavior = FillBehavior.Stop;
                img.BeginAnimation(CategorySelectionControl.OpacityProperty, animateOpacity);
            }
        }

        private void Button_HoverClick(object sender, HandInputEventArgs e)
        {
            var selected = ((HoverDwellButton)sender).DataContext as Category;
            if (selected != null)
            {
                this.SelectedCategory = selected;
            }
        }
    }
}