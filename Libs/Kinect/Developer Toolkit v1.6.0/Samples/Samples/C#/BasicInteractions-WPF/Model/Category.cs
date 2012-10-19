//------------------------------------------------------------------------------
// <copyright file="Category.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Windows.Media;
    using Microsoft.Samples.Kinect.BasicInteractions.Properties;

    public class Category : INotifyPropertyChanged
    {
        private readonly List<ContentItem> content;

        private readonly List<string> subcategories;

        private string contentHeadline;

        private ImageSource contentImage;

        public Category()
        {
            this.content = new List<ContentItem>();
            this.subcategories = new List<string>();
            this.subcategories.Add(Settings.Default.SubcategoryAll);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public string Title { get; set; }

        public ReadOnlyCollection<ContentItem> Content
        {
            get { return this.content.AsReadOnly(); }
        }

        public ReadOnlyCollection<string> Subcategories
        {
            get { return this.subcategories.AsReadOnly(); }
        }

        public string ContentHeadline
        {
            get 
            { 
                return this.contentHeadline; 
            }

            private set
            {
                this.contentHeadline = value;
                this.OnPropertyChanged(() => this.ContentHeadline);
            }
        }

        public ImageSource ContentImage
        {
            get
            {
                ContentItem currentContent = null;
                if (this.content != null && this.content.Count > 0)
                {
                    ImageSource img = null;
                    while (img == null)
                    {
                        var rnd = new Random();
                        int index = rnd.Next(this.content.Count);

                        currentContent = this.content[index];
                        img = currentContent.ContentImage;
                    }

                    this.contentImage = currentContent.ContentImage;
                    this.contentHeadline = currentContent.Title;
                    this.OnPropertyChanged(() => this.ContentHeadline);
                }

                return this.contentImage;
            }

            set
            {
                this.contentImage = value;
                this.OnPropertyChanged(() => this.ContentImage);
            }
        }


        public void AddContent(ContentItem newContent)
        {
            if (newContent == null)
            {
                throw new ArgumentNullException("newContent");
            }


            // for this sample limit the max number of stories to 20
            if (this.content.Count >= 20)
            {
                return;
            }

            this.content.Add(newContent);
            newContent.ItemId = this.content.Count;
            if (newContent.Subcategory.Length > 1)
            {
                if (!this.subcategories.Contains(newContent.Subcategory))
                {
                    this.subcategories.Add(newContent.Subcategory);
                }
            }

            if (this.contentImage == null)
            {
                this.contentImage = newContent.ContentImage;
                this.contentHeadline = newContent.Title;
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