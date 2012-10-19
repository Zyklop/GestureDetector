//------------------------------------------------------------------------------
// <copyright file="AttractControl.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for AttractControl.xaml
    /// </summary>
    public partial class AttractControl : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(AttractControl), new UIPropertyMetadata(null, OnItemsSourceChanged));

        public static readonly RoutedEvent SelectedItemChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedItemChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AttractControl));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(AttractControl), new UIPropertyMetadata(null, OnSelectedItemChanged));

        private readonly DispatcherTimer timer;

        public AttractControl()
        {
            this.InitializeComponent();
            
            // Creates a timer to cycle through the different categories of content.
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(5);
            this.timer.Tick += (o, s) =>
            {
                IEnumerator enumerator = this.ItemsSource.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current == this.SelectedItem)
                    {
                        if (enumerator.MoveNext())
                        {
                            this.SelectedItem = enumerator.Current;
                        }
                        else
                        {
                            enumerator.Reset();
                            if (enumerator.MoveNext())
                            {
                                this.SelectedItem = enumerator.Current;
                            }
                        }

                        break;
                    }
                }
            };
        }

        public event RoutedEventHandler SelectedItemChanged
        {
            add { this.AddHandler(SelectedItemChangedEvent, value); }
            remove { this.RemoveHandler(SelectedItemChangedEvent, value); }
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

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var cat = args.NewValue as Category;
            if (cat != null)
            {
                cat.ContentImage = null;
            }

            var sc = sender as AttractControl;
            if (sc != null)
            {
                sc.RaiseEvent(new RoutedEventArgs(SelectedItemChangedEvent));
            }
        }

        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var attract = sender as AttractControl;
            if (args.NewValue == null)
            {
                attract.timer.Stop();
            }
            else
            {
                attract.timer.Start();
            }
        }
    }
}