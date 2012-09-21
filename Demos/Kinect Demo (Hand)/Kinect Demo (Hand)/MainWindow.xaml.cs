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


namespace Kinect_Demo__Hand_
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Member Variables
        private KinectSensor _KinectDevice;
        private Skeleton[] _FrameSkeletons;
        #endregion Member Variables


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
                        ImageHand.Visibility = Visibility.Collapsed;
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

        private void TrackHand(Skeleton skeleton)
        {
            Joint hand = skeleton.Joints[JointType.HandRight];

            Joint leftMax = skeleton.Joints[JointType.ShoulderCenter];
            Joint rightMax = skeleton.Joints[JointType.ShoulderRight];

            Joint topMax = skeleton.Joints[JointType.ShoulderCenter];
            Joint bottomMax = skeleton.Joints[JointType.Spine];

            if (hand.TrackingState == JointTrackingState.NotTracked)
            {
                ImageHand.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                ImageHand.Visibility = System.Windows.Visibility.Visible;

                float VerticalRange = (rightMax.Position.X - leftMax.Position.X) * 2.0f;
                float HorizontalRange = topMax.Position.Y - bottomMax.Position.Y;
                float ScreenWidth = (float)(GridHand.ActualWidth - ImageHand.ActualWidth);
                float ScreenHeight = (float)(GridHand.ActualHeight - (ImageHand.ActualWidth/2));

                float HandPosX = hand.Position.X - leftMax.Position.X;
                float HandPosY = topMax.Position.Y - hand.Position.Y;

                float x = (HandPosX * ScreenWidth / VerticalRange);
                if (x < 0) x = 0;
                if (x > ScreenWidth) x = ScreenWidth;
                float y = (HandPosY * ScreenHeight / HorizontalRange);
                if (y < 0) y = 0;
                if (y > ScreenHeight) y = ScreenHeight;

                Canvas.SetLeft(ImageHand, x);
                Canvas.SetTop(ImageHand, y);
            }
        }

    }
}
