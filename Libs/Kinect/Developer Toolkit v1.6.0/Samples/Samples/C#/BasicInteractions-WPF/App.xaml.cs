//------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System.ComponentModel;
    using System.Windows;
    using Microsoft.Samples.Kinect.BasicInteractions.Properties;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static KinectController controller;

        public App()
        {
            Model = new Model();
        }

        public static Model Model { get; private set; }

        public static KinectController Controller
        {
            get
            {
                if (controller == null)
                {
                    if (DesignerProperties.GetIsInDesignMode(Current.MainWindow) == false)
                    {
                        controller = new KinectController(Current.MainWindow);
                        controller.Initialize();
                        controller.SetSpeechGrammar(Model.CreateSpeechGrammar());
                        controller.MinimumSpeechConfidence = Settings.Default.SpeechMinimumConfidence;
                    }
                }

                return controller;
            }
        }
    }
}