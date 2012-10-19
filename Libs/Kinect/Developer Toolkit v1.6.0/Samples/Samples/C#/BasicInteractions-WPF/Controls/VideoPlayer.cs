//------------------------------------------------------------------------------
// <copyright file="VideoPlayer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    [TemplatePart(Name = Media, Type = typeof(MediaElement))]
    [TemplatePart(Name = PlayPauseButton, Type = typeof(HoverDwellButton))]
    public class VideoPlayer : Control
    {
        public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.Register("IsPlaying", typeof(bool), typeof(VideoPlayer), new UIPropertyMetadata(false, OnIsPlayingChanged));

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(VideoPlayer), new UIPropertyMetadata(null));

        private const string Media = "PART_Media";
        private const string PlayPauseButton = "PART_PlayPauseButton";

        private State currentState = State.Stopped;
        private MediaElement mediaElement;
        private HoverDwellButton playPauseButton;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Overriding metadata must occur within a static constructor")]
        static VideoPlayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VideoPlayer), new FrameworkPropertyMetadata(typeof(VideoPlayer)));
        }

        #region Nested type: State

        private enum State
        {
            Stopped,

            Paused,

            Playing
        }

        #endregion

        public bool IsPlaying
        {
            get { return (bool)this.GetValue(IsPlayingProperty); }
            set { this.SetValue(IsPlayingProperty, value); }
        }

        public Uri Source
        {
            get { return (Uri)this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            if (this.mediaElement != null)
            {
                this.mediaElement.Loaded -= this.Media_Loaded;
                this.mediaElement.MediaEnded -= this.MediaElement_MediaEnded;
            }

            if (this.playPauseButton != null)
            {
                this.playPauseButton.HoverClick -= this.PlayPauseButton_HoverClick;
            }

            base.OnApplyTemplate();

            this.mediaElement = this.Template.FindName(Media, this) as MediaElement;
            this.playPauseButton = this.Template.FindName(PlayPauseButton, this) as HoverDwellButton;

            if (this.mediaElement != null)
            {
                this.mediaElement.Loaded += this.Media_Loaded;
                this.mediaElement.MediaEnded += this.MediaElement_MediaEnded;
            }

            if (this.playPauseButton != null)
            {
                this.playPauseButton.HoverClick += this.PlayPauseButton_HoverClick;
            }
        }

        private static void OnIsPlayingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var instance = (VideoPlayer)o;
            if (e.NewValue is bool && (bool)e.NewValue)
            {
                if (instance.currentState != State.Playing)
                {
                    if (instance.mediaElement != null)
                    {
                        instance.mediaElement.Play();
                    }

                    instance.currentState = State.Playing;
                }
            }
            else
            {
                if (instance.currentState == State.Playing)
                {
                    if (instance.mediaElement != null)
                    {
                        instance.mediaElement.Pause();
                    }

                    instance.currentState = State.Paused;
                }
            }
        }

        private void PlayPauseButton_HoverClick(object sender, HandInputEventArgs e)
        {
            this.IsPlaying = !this.IsPlaying;
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.mediaElement.Position = TimeSpan.Zero;
            this.mediaElement.Play();
        }

        private void Media_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetMediaStart();
        }

        private void SetMediaStart()
        {
            this.mediaElement.Play();
            this.mediaElement.Position = TimeSpan.FromMilliseconds(100);
            if (false == this.IsPlaying)
            {
                this.mediaElement.Pause();
            }
        }
    }
}