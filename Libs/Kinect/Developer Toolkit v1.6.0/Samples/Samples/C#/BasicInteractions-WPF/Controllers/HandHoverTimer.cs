//------------------------------------------------------------------------------
// <copyright file="HandHoverTimer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Windows.Threading;

    public class HandHoverTimer
    {
        private readonly DispatcherTimer timer;
        private DateTime startTime;
        private bool startTimeValid;

        public HandHoverTimer(DispatcherPriority priority, Dispatcher dispatcher)
        {
            this.timer = new DispatcherTimer(priority, dispatcher);
        }

        public event EventHandler Tick
        {
            add { this.timer.Tick += value; }
            remove { this.timer.Tick -= value; }
        }

        public HandPosition Hand { get; set; }

        public TimeSpan Interval
        {
            get { return this.timer.Interval; }
            set { this.timer.Interval = value; }
        }

        public TimeSpan TimeRemaining
        {
            get { return this.startTimeValid ? this.Interval - (DateTime.Now - this.startTime) : TimeSpan.MaxValue; }
        }

        public void Start()
        {
            this.startTime = DateTime.Now;
            this.startTimeValid = true;
            this.timer.Start();
        }

        public void Stop()
        {
            this.startTimeValid = false;
            this.timer.Stop();
        }
    }
}