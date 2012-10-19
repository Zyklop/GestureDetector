//------------------------------------------------------------------------------
// <copyright file="HandPosition.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Windows;
    using Microsoft.Samples.Kinect.BasicInteractions.Properties;

    public class HandPosition : INotifyPropertyChanged
    {
        private bool isInteracting;
        private bool isLeft;
        private int x;
        private int y;

        public HandPosition()
        {
            this.MagneticField = Settings.Default.MagneticField;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public double MagneticField { get; set; }

        /// <summary>
        /// The PlayerId that the hand belongs to.
        /// </summary>
        public int PlayerId { get; set; }

        /// <summary>
        /// The current element within the application that the hand is over.
        /// </summary>
        public IInputElement CurrentElement { get; set; }

        public bool Magnetized
        {
            get 
            { 
                return this.MagnetizedHorizontally || this.MagnetizedVertically; 
            }

            set
            {
                this.MagnetizedVertically = value;
                this.MagnetizedHorizontally = value;
                this.OnPropertyChanged(() => this.Magnetized);
            }
        }

        public bool MagnetizedHorizontally { get; set; }

        public bool MagnetizedVertically { get; set; }

        /// <summary>
        /// The X coordinate of the hand within the application, scaled to the screen size.
        /// </summary>
        public int X
        {
            get 
            { 
                return this.x; 
            }

            set
            {
                if (this.MagnetizedHorizontally)
                {
                    if (Math.Abs(value - this.x) < this.MagneticField)
                    {
                        return;
                    }
                }

                this.MagnetizedHorizontally = false;
                this.x = value;
                this.OnPropertyChanged(() => this.X);
            }
        }

        /// <summary>
        /// The Y coordinate of the hand within the application, scaled to the screen size.
        /// </summary>
        public int Y
        {
            get 
            { 
                return this.y; 
            }

            set
            {
                if (this.MagnetizedVertically)
                {
                    if (Math.Abs(value - this.y) < this.MagneticField)
                    {
                        return;
                    }
                }

                this.MagnetizedVertically = false;
                this.y = value;
                this.OnPropertyChanged(() => this.Y);
            }
        }

        /// <summary>
        /// Used to set whether the hand is the left or right hand. True = Left, False = Right.
        /// </summary>
        public bool IsLeft
        {
            get 
            { 
                return this.isLeft; 
            }

            set
            {
                this.isLeft = value;
                this.OnPropertyChanged(() => this.IsLeft);
            }
        }

        /// <summary>
        /// Used to set whether or not the hand is interacting with a UI element.
        /// </summary>
        public bool IsInteracting
        {
            get 
            { 
                return this.isInteracting; 
            }

            set
            {
                this.isInteracting = value;
                this.OnPropertyChanged(() => this.IsInteracting);
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as HandPosition;
            if (other != null)
            {
                return this.PlayerId.Equals(other.PlayerId) && this.IsLeft.Equals(other.IsLeft);
            }

            return false;
        }

        public override int GetHashCode()
        {
            string hash = this.PlayerId.ToString(CultureInfo.InvariantCulture) + this.IsLeft.ToString();
            return hash.GetHashCode();
        }

        private void OnPropertyChanged<T>(Expression<Func<T>> expression)
        {
            if (this.PropertyChanged == null)
            {
                return;
            }

            var body = (MemberExpression)expression.Body;
            string propertyName = body.Member.Name;
            var args = new PropertyChangedEventArgs(propertyName);
            this.PropertyChanged(this, args);
        }
    }
}