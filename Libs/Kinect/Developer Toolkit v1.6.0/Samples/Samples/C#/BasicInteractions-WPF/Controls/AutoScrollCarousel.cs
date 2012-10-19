//------------------------------------------------------------------------------
// <copyright file="AutoScrollCarousel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;

    public class AutoScrollCarousel : Control
    {
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectionChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedEventArgs),
            typeof(AutoScrollCarousel));

        public static readonly DependencyProperty SelectedItemProperty =
            Selector.SelectedItemProperty.AddOwner(typeof(AutoScrollCarousel), new PropertyMetadata(OnSelectedItemChanged));

        public static readonly DependencyProperty ItemsSourceProperty =
            ItemsControl.ItemsSourceProperty.AddOwner(typeof(AutoScrollCarousel), new PropertyMetadata(OnItemsSourceChanged));

        public static readonly DependencyProperty ItemsPanelProperty =
            ItemsControl.ItemsPanelProperty.AddOwner(typeof(AutoScrollCarousel));

        public static readonly DependencyProperty ItemTemplateProperty =
            ItemsControl.ItemTemplateProperty.AddOwner(typeof(AutoScrollCarousel));

        public static readonly DependencyProperty IsLoopingProperty =
            DependencyProperty.Register("IsLooping", typeof(bool), typeof(AutoScrollCarousel), new UIPropertyMetadata(false));

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(AutoScrollCarousel), new UIPropertyMetadata(0.0, OnHorizontalOffsetChanged));

        private ItemsControl itemsCurrent;
        private ItemsControl itemsPrevious;
        private StackPanel scroller;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Overriding metadata must occur within a static constructor")]
        static AutoScrollCarousel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoScrollCarousel), new FrameworkPropertyMetadata(typeof(AutoScrollCarousel)));
        }

        public AutoScrollCarousel()
        {
            this.Loaded += (o, s) => { this.CheckBounds(); };
        }

        public event RoutedEventHandler SelectionChanged
        {
            add { this.AddHandler(SelectionChangedEvent, value); }
            remove { this.RemoveHandler(SelectionChangedEvent, value); }
        }

        public object SelectedItem
        {
            get { return this.GetValue(SelectedItemProperty); }

            set { this.SetValue(SelectedItemProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }

            set { this.SetValue(ItemsSourceProperty, value); }
        }

        public ItemsPanelTemplate ItemsPanel
        {
            get { return (ItemsPanelTemplate)this.GetValue(ItemsPanelProperty); }

            set { this.SetValue(ItemsPanelProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)this.GetValue(ItemTemplateProperty); }
            set { this.SetValue(ItemTemplateProperty, value); }
        }

        public bool IsLooping
        {
            get { return (bool)this.GetValue(IsLoopingProperty); }
            set { this.SetValue(IsLoopingProperty, value); }
        }

        public double HorizontalOffset
        {
            get { return (double)this.GetValue(HorizontalOffsetProperty); }
            set { this.SetValue(HorizontalOffsetProperty, value); }
        }

        public void ScrollToItem(object item)
        {
            if (this.itemsCurrent == null)
            {
                return;
            }

            double position = 0 + this.itemsPrevious.ActualWidth;
            IEnumerator enumerator = this.ItemsSource.GetEnumerator();
            FrameworkElement elementContainer = null;
            while (enumerator.MoveNext())
            {
                elementContainer = this.itemsCurrent.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                if (enumerator.Current == item)
                {
                    break;
                }
                else
                {
                    if (elementContainer != null)
                    {
                        position += elementContainer.ActualWidth;
                    }
                }
            }

            double movement = position;
            this.ScrollToOffset(this.HorizontalOffset, movement);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.scroller = this.Template.FindName("PART_Scroller", this) as StackPanel;
            this.itemsPrevious = this.Template.FindName("PART_Previous", this) as ItemsControl;
            this.itemsCurrent = this.Template.FindName("PART_Current", this) as ItemsControl;
        }

        private static void OnHorizontalOffsetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var self = (AutoScrollCarousel)sender;
            if (self.scroller != null && args.NewValue is double)
            {
                self.scroller.Margin = new Thickness(-(double)args.NewValue + (self.ActualWidth / 2) - (self.itemsCurrent.ActualWidth / self.itemsCurrent.Items.Count / 2), 0, 0, 0);
            }
        }

        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var carousel = (AutoScrollCarousel)sender;
            if (args.NewValue is IEnumerable)
            {
                var list = args.NewValue as IEnumerable;
                IEnumerator enumerator = list.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    carousel.SelectedItem = enumerator.Current;
                }

                carousel.UpdateUI(list);
            }
        }

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var carousel = (AutoScrollCarousel)sender;
            if (carousel.itemsCurrent != null)
            {
                if (args.NewValue != null)
                {
                    var container =
                        carousel.itemsCurrent.ItemContainerGenerator.ContainerFromItem(args.NewValue) as
                        ContentPresenter;
                    if (container != null)
                    {
                        var autoScrollContainer = VisualTreeHelper.GetChild(container, 0) as AutoScrollCarouselContainer;
                        if (autoScrollContainer != null)
                        {
                            autoScrollContainer.IsSelected = true;
                        }
                    }
                }

                if (args.OldValue != null)
                {
                    var container =
                        carousel.itemsCurrent.ItemContainerGenerator.ContainerFromItem(args.OldValue) as
                        ContentPresenter;
                    if (container != null)
                    {
                        var autoScrollContainer = VisualTreeHelper.GetChild(container, 0) as AutoScrollCarouselContainer;
                        if (autoScrollContainer != null)
                        {
                            autoScrollContainer.IsSelected = false;
                        }
                    }
                }
            }

            carousel.ScrollToItem(args.NewValue);
            carousel.RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));
        }

        private void ScrollToOffset(double from, double to)
        {
            var animation = new DoubleAnimation();
            animation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
            if (from - to > this.itemsCurrent.ActualWidth / 2)
            {
                from -= this.itemsCurrent.ActualWidth;
            }

            animation.From = from;
            animation.To = to;
            animation.Duration = TimeSpan.FromMilliseconds(300);
            animation.FillBehavior = FillBehavior.Stop;
            this.HorizontalOffset = to;
            this.BeginAnimation(HorizontalOffsetProperty, animation);
        }

        private void CheckBounds()
        {
            if (this.IsLooping)
            {
                if (this.HorizontalOffset < this.itemsPrevious.ActualWidth)
                {
                    this.HorizontalOffset += this.itemsPrevious.ActualWidth;
                }

                if (this.HorizontalOffset > this.itemsPrevious.ActualWidth + this.itemsCurrent.ActualWidth)
                {
                    this.HorizontalOffset -= this.itemsCurrent.ActualWidth;
                }
            }
        }


        private void UpdateUI(IEnumerable nextItems)
        {
            if (nextItems.GetEnumerator().MoveNext())
            {
                var animation = new DoubleAnimation(this.Opacity, 1.0, TimeSpan.FromMilliseconds(300));
                this.Opacity = 1;
                animation.FillBehavior = FillBehavior.Stop;
                this.Dispatcher.BeginInvoke(new Action<DoubleAnimation>((a) => { this.BeginAnimation(AutoScrollCarousel.OpacityProperty, a); }), DispatcherPriority.Normal, animation);
            }
            else
            {
                var animation = new DoubleAnimation(this.Opacity, 0.0, TimeSpan.FromMilliseconds(300));
                this.Opacity = 0;
                animation.FillBehavior = FillBehavior.Stop;
                this.BeginAnimation(AutoScrollCarousel.OpacityProperty, animation);
            }
        }
    }

    public class AutoScrollCarouselContainer : ContentControl
    {
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(AutoScrollCarouselContainer), new UIPropertyMetadata(false));

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Overriding metadata must occur within a static constructor")]
        static AutoScrollCarouselContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoScrollCarouselContainer), new FrameworkPropertyMetadata(typeof(AutoScrollCarouselContainer)));
        }

        public bool IsSelected
        {
            get { return (bool)this.GetValue(IsSelectedProperty); }
            set { this.SetValue(IsSelectedProperty, value); }
        }
    }
}