//------------------------------------------------------------------------------
// <copyright file="HandVisualizer.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;

    /// <summary>
    /// Interaction logic for HandVisualizer.xaml
    /// </summary>
    public partial class HandVisualizer : UserControl
    {
        public static readonly DependencyProperty HandProperty =
    DependencyProperty.Register("Hand", typeof(HandPosition), typeof(HandVisualizer), new UIPropertyMetadata(null, OnHandChanged));

        private Storyboard expandStoryboard;
        private Storyboard ringStoryboard;

        public HandVisualizer()
        {
            this.InitializeComponent();
        }

        public HandPosition Hand
        {
            get { return (HandPosition)GetValue(HandProperty); }
            set { this.SetValue(HandVisualizer.HandProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.ringStoryboard = this.Resources["CompleteRing"] as Storyboard;
            this.expandStoryboard = this.Resources["ExpandRing"] as Storyboard;
        }

        private static void OnHandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                ((HandPosition)e.OldValue).PropertyChanged -= ((HandVisualizer)o).ActiveHand_PropertyChanged;
            }

            if (e.NewValue != null)
            {
                ((HandPosition)e.NewValue).PropertyChanged += ((HandVisualizer)o).ActiveHand_PropertyChanged;
            }
        }

        private void ActiveHand_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsInteracting")
            {
                if (App.Controller.ActiveHand.IsInteracting)
                {
                    if (this.ringStoryboard != null)
                    {
                        this.ringStoryboard.Begin();
                    }
                }
                else
                {
                    if (this.ringStoryboard != null)
                    {
                        this.ringStoryboard.Stop();
                    }
                }
            }

            if (e.PropertyName == "Magnetized")
            {
                if (App.Controller.ActiveHand.Magnetized
                    && App.Controller.ActiveHand.IsInteracting)
                {
                    if (this.expandStoryboard != null)
                    {
                        this.expandStoryboard.Begin();
                    }
                }
                else
                {
                    if (this.expandStoryboard != null)
                    {
                        this.expandStoryboard.Stop();
                    }
                }
            }
        }
    }
}