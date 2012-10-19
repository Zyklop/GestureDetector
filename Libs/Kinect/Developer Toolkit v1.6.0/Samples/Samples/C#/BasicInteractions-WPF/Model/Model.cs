//------------------------------------------------------------------------------
// <copyright file="Model.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using System.Xml.Linq;
    using Microsoft.Samples.Kinect.BasicInteractions.Properties;

    public class Model
    {
        private readonly List<Category> categories;

        public Model()
        {
            this.categories = new List<Category>();

            // you can load your own feed here 
            this.LoadStories();
        }

        public ReadOnlyCollection<Category> Categories
        {
            get { return this.categories.AsReadOnly(); }
        }


        public ReadOnlyCollection<string> CreateSpeechGrammar()
        {
            var speechKeywords = new List<string>();
            int maxNumberOfStories = 0;
            foreach (Category cat in this.categories)
            {
                if (maxNumberOfStories < cat.Content.Count)
                {
                    maxNumberOfStories = cat.Content.Count;
                }

                // Add each of the categories to the list.
                if (!speechKeywords.Contains(cat.Title))
                {
                    if (cat.Title.Contains("&"))
                    {
                        speechKeywords.Add(cat.Title.Replace("&", "and"));
                    }

                    speechKeywords.Add(cat.Title);
                }

                foreach (ContentItem cont in cat.Content)
                {
                    if (cont.Subcategory.Length > 1)
                    {
                        if (!speechKeywords.Contains(cont.Subcategory))
                        {
                            if (cont.Subcategory.Contains("&"))
                            {
                                speechKeywords.Add(cont.Subcategory.Replace("&", "and"));
                            }

                            speechKeywords.Add(cont.Subcategory);
                        }
                    }
                }
            }

            speechKeywords.Add(Settings.Default.SubcategoryAll);

            // Add the control words for the application.
            speechKeywords.Add(Settings.Default.SpeechSelectWord);
            speechKeywords.Add(Settings.Default.SpeechBackWord);
            speechKeywords.Add(Settings.Default.SpeechHomeWord);
            speechKeywords.Add(Settings.Default.SpeechLikeWord);
            speechKeywords.Add(Settings.Default.SpeechDislikeWord);
            speechKeywords.Add(Settings.Default.SpeechPlayWord);
            speechKeywords.Add(Settings.Default.SpeechPauseWord);
            speechKeywords.Add(Settings.Default.SpeechStartWord);

            // add the story numbers
            for (var choice = NumberWords.One;
                 (int)choice <= maxNumberOfStories
                 && choice <= NumberWords.Twenty;
                 choice++)
            {
                speechKeywords.Add(choice.ToString());
            }

            return speechKeywords.AsReadOnly();
        }

        private void LoadStories()
        {
            XDocument doc = XDocument.Load("Content\\Stories.xml");
            foreach (XElement ele in doc.Descendants("story"))
            {
                string categoryTitle = ele.Element("category").Value;
                Category category = this.categories.FirstOrDefault(c => c.Title.Equals(categoryTitle));
                if (category == null)
                {
                    category = new Category { Title = categoryTitle };
                    this.categories.Add(category);
                }

                if (category != null)
                {
                    var content = new ContentItem
                                      {
                                          Title = ele.Element("title").Value,
                                          Category = category,
                                          Subcategory = ele.Element("subcategory").Value,
                                          Content = ele.Element("content").Value,
                                          ContentImage =
                                              string.IsNullOrEmpty(ele.Element("image").Value)
                                                  ? null : new BitmapImage(new Uri("pack://siteoforigin:,,,/Content/Images/" + ele.Element("image").Value)),
                                      };
                    category.AddContent(content);
                }
            }
        }
    }
}