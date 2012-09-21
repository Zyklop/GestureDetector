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

namespace Kinect_Demo__Bild_
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private state
        private KinectSensor _KinectDevice;
        private Skeleton[] _FrameSkeletons;
        private bool IsLogoCatchedLeftHand = false;
        private bool IsLogoCatchedRightHand = false;
        #endregion Private state

        public MainWindow()
        {
            InitializeComponent();

            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            this.KinectDevice = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
        }
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

                        // Skeleton
                        this._KinectDevice.SkeletonFrameReady -= KinectDevice_SkeletonFrameReady;
                        this._KinectDevice.SkeletonStream.Disable();
                        this._FrameSkeletons = null;
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

                            // Skeleton
                            this._KinectDevice.SkeletonStream.Enable();
                            this._FrameSkeletons = new Skeleton[this._KinectDevice.SkeletonStream.FrameSkeletonArrayLength];
                            this.KinectDevice.SkeletonFrameReady += KinectDevice_SkeletonFrameReady;

                            this.KinectDevice.SkeletonStream.Enable(new TransformSmoothParameters()
                            {
                                Smoothing = 0.7f,               // Defualt :0.5
                                Correction = 0.5f,              // Defualt :0.5
                                JitterRadius = 0.10f,           // Defualt :0.05
                                MaxDeviationRadius = 0.04f      // Defualt :0.04
                            });                            

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


        #region SkeletonView
        private void KinectDevice_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    frame.CopySkeletonDataTo(this._FrameSkeletons);
                    Skeleton skeleton = GetPrimarySkeleton(this._FrameSkeletons);

                    if (skeleton == null)
                    {
                        IsLogoCatchedLeftHand = false;
                        IsLogoCatchedRightHand = false;

                        SetImagePosition(0, 0, ImageMFLogo.ActualWidth, 0);
                    }
                    else
                    {
                        TrackHand(skeleton);
                    }

                }
            }
        }

        private static Skeleton GetPrimarySkeleton(Skeleton[] skeletons)
        {
            Skeleton skeleton = null;

            if (skeletons != null)
            {
                //Find the closest skeleton
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (skeleton == null)
                        {
                            skeleton = skeletons[i];
                        }
                        else
                        {
                            if (skeleton.Position.Z > skeletons[i].Position.Z)
                            {
                                skeleton = skeletons[i];
                            }
                        }
                    }
                }
            }

            return skeleton;
        }

        private void SetImagePosition( double p_left, double p_lefttop, double p_right, double p_righttop)
        {
            double l_a, l_b, l_c, l_sin;

            l_a = (p_righttop - p_lefttop);
            l_c = (p_right - p_left);
            l_b = Math.Sqrt((l_a*l_a) + (l_c*l_c));

            l_sin = 0.0;
            if (l_c != 0.0)
            {
                l_sin = (l_a / l_c);
                if (l_sin > 1.0) l_sin = 1.0;
                if (l_sin < -1.0) l_sin = -1.0;
            }

            Canvas.SetLeft(ImageMFLogo, p_left);
            Canvas.SetTop(ImageMFLogo, p_lefttop);

            LogoRotate.Angle = (Math.Asin(l_sin) * 180 / Math.PI);

            double l_Scale;

            l_Scale = (l_b / ImageMFLogo.ActualWidth);

            LogoScale.ScaleX = l_Scale;
            LogoScale.ScaleY = l_Scale;
        }

        private void TrackHand(Skeleton skeleton)
        {
            Joint handLeft  = skeleton.Joints[JointType.HandLeft];
            Joint handRight = skeleton.Joints[JointType.HandRight];

            if ((handLeft.TrackingState == JointTrackingState.NotTracked) ||
                (handRight.TrackingState == JointTrackingState.NotTracked))
            {
                IsLogoCatchedLeftHand = false;
                IsLogoCatchedRightHand = false;
                SetImagePosition(0, 0, ImageMFLogo.ActualWidth, 0);
            }
            else
            {
                Point l_LogoTopLeft = new Point(0, 0);
                Point l_LogoBottomRight = new Point(0, 0);
                l_LogoTopLeft.X = Canvas.GetLeft(ImageMFLogo);
                l_LogoTopLeft.Y = Canvas.GetTop(ImageMFLogo);
                l_LogoBottomRight.X = l_LogoTopLeft.X + ImageMFLogo.ActualWidth;
                l_LogoBottomRight.Y = l_LogoTopLeft.Y + ImageMFLogo.ActualHeight;


                Point l_LeftHand = new Point(0,0);
                DepthImagePoint l_pointLeftHand = this.KinectDevice.MapSkeletonPointToDepth(handLeft.Position, DepthImageFormat.Resolution640x480Fps30);
                l_LeftHand.X = (l_pointLeftHand.X * GridMain.ActualWidth / this.KinectDevice.DepthStream.FrameWidth);
                l_LeftHand.Y = (l_pointLeftHand.Y * GridMain.ActualHeight / this.KinectDevice.DepthStream.FrameHeight);

                if (!IsLogoCatchedLeftHand)                     // mit linker Hand noch nicht gefangen
                {
                    if ((l_LeftHand.X >= l_LogoTopLeft.X) &&
                        (l_LeftHand.X <= l_LogoBottomRight.X) &&
                        (l_LeftHand.Y >= l_LogoTopLeft.Y) &&
                        (l_LeftHand.Y <= l_LogoBottomRight.Y))
                    {
                        IsLogoCatchedLeftHand = true;
                    }
                }

                Point l_RightHand = new Point(0, 0);
                DepthImagePoint l_pointRightHand = this.KinectDevice.MapSkeletonPointToDepth(handRight.Position, DepthImageFormat.Resolution640x480Fps30);
                l_RightHand.X = (l_pointRightHand.X * GridMain.ActualWidth / this.KinectDevice.DepthStream.FrameWidth);
                l_RightHand.Y = (l_pointRightHand.Y * GridMain.ActualHeight / this.KinectDevice.DepthStream.FrameHeight);

                if (!IsLogoCatchedRightHand)                     // mit rechte Hand noch nicht gefangen
                {
                    if ((l_RightHand.X >= l_LogoTopLeft.X) &&
                        (l_RightHand.X <= l_LogoBottomRight.X) &&
                        (l_RightHand.Y >= l_LogoTopLeft.Y) &&
                        (l_RightHand.Y <= l_LogoBottomRight.Y))
                    {
                        IsLogoCatchedRightHand = true;
                    }
                }

                if ((IsLogoCatchedLeftHand) && (IsLogoCatchedRightHand))
                {
                    SetImagePosition(l_LeftHand.X, l_LeftHand.Y, l_RightHand.X, l_RightHand.Y);
                }
                else
                {
                    if (IsLogoCatchedLeftHand)
                        SetImagePosition(l_LeftHand.X, l_LeftHand.Y, (l_LeftHand.X + ImageMFLogo.ActualWidth), l_LeftHand.Y);

                    if (IsLogoCatchedRightHand)
                        SetImagePosition((l_RightHand.X - ImageMFLogo.ActualWidth), l_RightHand.Y, l_RightHand.X, l_RightHand.Y);
                }
            }
        }
        
        #endregion SkeletonView

    }
}
