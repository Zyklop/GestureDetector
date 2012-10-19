//------------------------------------------------------------------------------
// <copyright file="Rating.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    public class Rating : INotifyPropertyChanged
    {
        private int dislikes;
        private int likes;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public int Likes
        {
            get 
            { 
                return this.likes; 
            }

            set
            {
                this.likes = value;
                this.OnPropertyChanged(() => this.Likes);
            }
        }

        public int Dislikes
        {
            get 
            { 
                return this.dislikes; 
            }

            set
            {
                this.dislikes = value;
                this.OnPropertyChanged(() => this.Dislikes);
            }
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