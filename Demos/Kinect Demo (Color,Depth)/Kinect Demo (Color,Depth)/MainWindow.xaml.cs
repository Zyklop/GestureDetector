using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Kinect;

namespace Kinect_Demo
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private state
        private KinectSensor _KinectDevice;
        #endregion Private state        

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            this.KinectDevice = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);

        }
        #endregion Constructor

        #region Methods
        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Initializing:
                case KinectStatus.Connected:
                    this.KinectDevice = e.Sensor;
                    break;
                case KinectStatus.Disconnected:
                    //TODO: Give the user feedback to plug-in a Kinect device.    
                    MessageBox.Show("This application requires a Kinect sensor.");
                    this.KinectDevice = null;
                    break;
                default:
                    //TODO: Show an error state
                    break;
            }
        }
        #endregion Methods

        #region Properties
        public KinectSensor KinectDevice
        {
            get { return this._KinectDevice; }
            set
            {
                if (this._KinectDevice != value)
                {
                    //Uninitialize
                    if (this._KinectDevice != null)
                    {
                        this._KinectDevice.Stop();

                        // ColorView
                        this._KinectDevice.ColorFrameReady -= KinectSensor_ColorFrameReady;
                        this._KinectDevice.ColorStream.Disable();

                        // DepthView
                        this._KinectDevice.DepthFrameReady -= KinectSensor_DepthFrameReady;
                        this._KinectDevice.DepthStream.Disable();
                    }

                    this._KinectDevice = value;

                    //Initialize
                    if (this._KinectDevice != null)
                    {
                        if (this._KinectDevice.Status == KinectStatus.Connected)
                        {
                            // ColorView
                            this._KinectDevice.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                            this._KinectDevice.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(KinectSensor_ColorFrameReady);

                            // DepthView
                            this._KinectDevice.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                            this._KinectDevice.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(KinectSensor_DepthFrameReady);

                            this._KinectDevice.Start();
                        }
                    }
                }
            }
        }
        #endregion Properties

        #region ColorView
        //*************** Event for ColorView *******************************************
        void KinectSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame frame = e.OpenColorImageFrame())
            {
                if (frame != null)
                {
                    byte[] pixelData = new byte[frame.PixelDataLength];
                    frame.CopyPixelDataTo(pixelData);

                    imageColorView.Source = BitmapImage.Create(frame.Width,
                        frame.Height,
                        96,
                        96,
                        PixelFormats.Bgr32,
                        null,
                        pixelData,
                        frame.Width * frame.BytesPerPixel);
                }
            }
        }
        #endregion ColorView

        #region DepthView
        //*************** Event for DepthView ********************************************
        void KinectSensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame frame = e.OpenDepthImageFrame())
            {
                if (frame != null)
                {
                    int depth;
                    int color;
                    int bytesPerPixel = 4;
                    byte[] enhPixelData = new byte[frame.Width * frame.Height * bytesPerPixel];
                    short[] pixelData = new short[frame.PixelDataLength];

                    frame.CopyPixelDataTo(pixelData);
                    int stride = frame.Width * frame.BytesPerPixel;

                    for (int i = 0, j = 0; i < pixelData.Length; i++, j += bytesPerPixel)
                    {
                        depth = pixelData[i] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                        if (depth < 0 || depth > 4000)
                        {
                            color = 0xFF;
                        }
                        else
                        {
                            color = (255 * depth / 0xFFF);
                        }

                        // Color for blue,green,red
                        enhPixelData[j] = 0;
                        enhPixelData[j + 1] = (byte)color;
                        enhPixelData[j + 2] = (byte)(255 - color);
                    }

                    imageDepthView.Source = BitmapSource.Create(frame.Width, frame.Height,
                                                  96, 96, PixelFormats.Bgr32, null,
                                                  enhPixelData, frame.Width * bytesPerPixel);
                     
                }
            }
        }
        #endregion DepthView
    }

}
