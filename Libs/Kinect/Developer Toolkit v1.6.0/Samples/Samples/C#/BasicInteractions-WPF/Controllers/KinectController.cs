//------------------------------------------------------------------------------
// <copyright file="KinectController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq.Expressions;
    using System.Timers;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using Microsoft.Speech.Recognition;

    public class KinectController : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Routed Event for a hand entering a UI element.
        /// </summary>
        public static readonly RoutedEvent HandEnterEvent = EventManager.RegisterRoutedEvent(
            "HandEnter",
            RoutingStrategy.Bubble,
            typeof(EventHandler<HandInputEventArgs>),
            typeof(KinectController));

        /// <summary>
        /// Routed Event for a hand moving over a UI element.
        /// </summary>
        public static readonly RoutedEvent HandMoveEvent = EventManager.RegisterRoutedEvent(
            "HandMove", 
            RoutingStrategy.Bubble, 
            typeof(EventHandler<HandInputEventArgs>),
            typeof(KinectController));

        /// <summary>
        /// Routed Event for a hand leaving a UI element.
        /// </summary>
        public static readonly RoutedEvent HandLeaveEvent = EventManager.RegisterRoutedEvent(
            "HandLeave", 
            RoutingStrategy.Bubble, 
            typeof(EventHandler<HandInputEventArgs>),
            typeof(KinectController));

        /// <summary>
        /// Routed Event for a hand being raised above an elbow
        /// </summary>
        public static readonly RoutedEvent HandRaisedEvent = EventManager.RegisterRoutedEvent(
            "HandRaised", 
            RoutingStrategy.Tunnel, 
            typeof(EventHandler<HandInputEventArgs>),
            typeof(KinectController));

        private const int RedIndex = 2;
        private const int GreenIndex = 1;
        private const int BlueIndex = 0;
        private static readonly int Bgra32BytesPerPixel = (PixelFormats.Bgra32.BitsPerPixel + 7) / 8;
        private readonly Window parentWindow;
        private KinectSensor sensor;
        private HandPosition activeHand;
        private byte[] depthFrame32;
        private byte[] convertedDepthBits;
        private DepthImageFormat lastImageFormat;
        private short[] pixelData;
        private SpeechRecognizer speechRecognizer;
        private Skeleton trackedSkeleton;
        private Skeleton[] skeletons;
        private Timer trackedSkeletonTimer;
        private bool hasActiveSkeleton = false;
        private WriteableBitmap silhouette;

        public KinectController(Window parent)
        {
            this.parentWindow = parent;
            this.ActiveHand = new HandPosition();
            this.speechRecognizer = new SpeechRecognizer();
            this.speechRecognizer.SpeechRecognized += this.SpeechRecognizer_SpeechRecognized;
            this.speechRecognizer.SpeechDetected += this.SpeechRecognizer_SpeechDetected;
        }

        public static event EventHandler<SpeechRecognizedEventArgs> SpeechRecognized;

        public static event EventHandler<SpeechDetectedEventArgs> SpeechDetected;

        public event PropertyChangedEventHandler PropertyChanged;

        public double MinimumSpeechConfidence { get; set; }

        public HandPosition ActiveHand
        {
            get 
            { 
                return activeHand; 
            }

            set
            {
                activeHand = value;
                OnPropertyChanged(() => ActiveHand);
            }
        }

        public bool HasActiveSkeleton
        {
            get 
            { 
                return this.hasActiveSkeleton; 
            }

            private set
            {
                if (this.hasActiveSkeleton != value)
                {
                    this.hasActiveSkeleton = value;
                    this.OnPropertyChanged(() => this.HasActiveSkeleton);
                }
            }
        }

        

        public KinectSensor Sensor
        {
            get
            { 
                return this.sensor; 
            }

            private set
            {
                if (this.sensor != null)
                {
                    this.sensor.Stop();
                }

                this.sensor = value;
                if (this.sensor != null)
                {
                    try
                    {
                        this.sensor.Start();
                        this.SetupSensor();
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show(ex.Message, BasicInteractions.Properties.Resources.KinectInitializeError);
                        Application.Current.Shutdown();
                    }
                }
            }
        }

        

        public WriteableBitmap Silhouette
        {
            get 
            { 
                return this.silhouette; 
            }

            private set
            {
                if (this.silhouette != value)
                {
                    this.silhouette = value;
                    this.OnPropertyChanged(() => this.Silhouette);
                }
            }
        }


        public static void AddPreviewHandEnterHandler(DependencyObject sender, EventHandler<HandInputEventArgs> handler)
        {
            var element = sender as UIElement;
            if (element != null)
            {
                element.AddHandler(HandEnterEvent, handler);
            }
        }

        public static void RemovePreviewHandEnterHandler(DependencyObject sender, EventHandler<HandInputEventArgs> handler)
        {
            var element = sender as UIElement;
            if (element != null)
            {
                element.RemoveHandler(HandEnterEvent, handler);
            }
        }


        public static void AddPreviewHandMoveHandler(DependencyObject sender, EventHandler<HandInputEventArgs> handler)
        {
            var element = sender as UIElement;
            if (element != null)
            {
                element.AddHandler(HandMoveEvent, handler);
            }
        }

        public static void RemovePreviewHandMoveHandler(DependencyObject sender, EventHandler<HandInputEventArgs> handler)
        {
            var element = sender as UIElement;
            if (element != null)
            {
                element.RemoveHandler(HandMoveEvent, handler);
            }
        }

        public static void AddPreviewHandLeaveHandler(DependencyObject sender, EventHandler<HandInputEventArgs> handler)
        {
            var element = sender as UIElement;
            if (element != null)
            {
                element.AddHandler(HandLeaveEvent, handler);
            }
        }

        public static void RemovePreviewHandLeaveHandler(DependencyObject sender, EventHandler<HandInputEventArgs> handler)
        {
            var element = sender as UIElement;
            if (element != null)
            {
                element.RemoveHandler(HandLeaveEvent, handler);
            }
        }

        public static void AddHandRaisedHandler(DependencyObject sender, EventHandler<HandInputEventArgs> handler)
        {
            var element = sender as UIElement;
            if (element != null)
            {
                element.AddHandler(HandRaisedEvent, handler);
            }
        }

        public static void RemoveHandRaisedHandler(DependencyObject sender, EventHandler<HandInputEventArgs> handler)
        {
            var element = sender as UIElement;
            if (element != null)
            {
                element.RemoveHandler(HandRaisedEvent, handler);
            }
        }

        public void Initialize()
        {
            foreach (KinectSensor kinect in KinectSensor.KinectSensors)
            {
                if (kinect.Status == KinectStatus.Connected)
                {
                    this.Sensor = kinect;
                    break;
                }
            }

            if (this.Sensor == null)
            {
                MessageBox.Show("Unable to detect an available Kinect Sensor.\nPlease make sure you have a Kinect sensor plugged in.\nThis sample will now close.");
                Application.Current.Shutdown();
            }

            KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
        }

        public void SetSpeechGrammar(IEnumerable<string> speechChoices)
        {
            this.speechRecognizer.SetSpeechGrammar(speechChoices);
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.trackedSkeletonTimer != null)
                {
                    this.trackedSkeletonTimer.Dispose();
                    this.trackedSkeletonTimer = null;
                }

                if (this.speechRecognizer != null)
                {
                    this.speechRecognizer.Dispose();
                    this.speechRecognizer = null;
                }

                if (this.sensor != null)
                {
                    this.sensor.Stop();
                    this.sensor = null;
                }
            }
        }

        #endregion

        private static double ScaleY(Joint joint)
        {
            double y = ((SystemParameters.PrimaryScreenHeight / 0.4) * -joint.Position.Y) +
                       (SystemParameters.PrimaryScreenHeight / 2);
            return y;
        }

        private static void ScaleXY(Joint shoulderCenter, bool rightHand, Joint joint, out int scaledX, out int scaledY)
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;

            double x = 0;
            double y = ScaleY(joint);

            // if rightHand then place shouldCenter on left of screen
            // else place shouldCenter on right of screen
            if (rightHand) 
            {
                x = (joint.Position.X - shoulderCenter.Position.X) * screenWidth * 2;
            }
            else 
            {
                x = screenWidth - ((shoulderCenter.Position.X - joint.Position.X) * (screenWidth * 2));
            }


            if (x < 0)
            {
                x = 0;
            }
            else if (x > screenWidth - 5)
            {
                x = screenWidth - 5;
            }

            if (y < 0)
            {
                y = 0;
            }

            scaledX = (int)x;
            scaledY = (int)y;
        }

        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Sensor == this.sensor)
            {
                switch (e.Status)
                {
                    case KinectStatus.Connected:
                    case KinectStatus.DeviceNotGenuine:
                    case KinectStatus.Initializing:
                        break;
                    default:
                        MessageBox.Show("Unable to detect an available Kinect Sensor.\nPlease make sure you have a Kinect sensor plugged in.\nThis sample will now close.");
                        Application.Current.Shutdown();
                        break;
                }
            }
        }

        private void SpeechRecognizer_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            if (SpeechDetected != null)
            {
                SpeechDetected(this, e);
            }
        }

        private void SetupSensor()
        {
            var parameters = new TransformSmoothParameters
                                 {
                                     Correction = 0.5f,
                                     Prediction = 0.5f,
                                     Smoothing = 0.05f,
                                     JitterRadius = 0.8f,
                                     MaxDeviationRadius = 0.2f
                                 };

            Sensor.SkeletonStream.Enable(parameters);
            Sensor.DepthStream.Enable();
            Sensor.SkeletonFrameReady += this.Sensor_SkeletonFrameReady;


            this.speechRecognizer.SetSensor(Sensor);
        }

        private void Sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null && skeletonFrame.SkeletonArrayLength > 0)
                {
                    if (skeletons == null || skeletons.Length != skeletonFrame.SkeletonArrayLength)
                    {
                        skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }
                    else
                    {
                        for (int i = 0; i < skeletons.Length; i++)
                        {
                            skeletons[i] = null;
                        }
                    }

                    skeletonFrame.CopySkeletonDataTo(skeletons);

                    int playerIndex = 0;
                    Skeleton updatedSkeleton = null;
                    if (this.trackedSkeleton != null)
                    {
                        for (int i = 0; i < skeletons.Length; i++)
                        {
                            if (skeletons[i].TrackingId == this.trackedSkeleton.TrackingId)
                            {
                                playerIndex = i;
                                updatedSkeleton = skeletons[i];
                                break;
                            }
                        }
                    }

                    if (updatedSkeleton == null)
                    {
                        double closestX = 1;
                        for (int i = 0; i < skeletons.Length; i++)
                        {
                            Skeleton newSkeleton = skeletons[i];
                            if (newSkeleton.TrackingState != SkeletonTrackingState.NotTracked &&
                                Math.Abs(newSkeleton.Position.X) < closestX)
                            {
                                playerIndex = i;
                                updatedSkeleton = skeletons[i];
                            }
                        }
                    }

                    this.trackedSkeleton = updatedSkeleton;

                    if (updatedSkeleton != null && this.sensor != null)
                    {
                        // set the silhouette
                        using (DepthImageFrame depthFrame = this.sensor.DepthStream.OpenNextFrame(15))
                        {
                            this.GetPlayerSilhouette(depthFrame, playerIndex + 1);
                        }

                        if (this.trackedSkeletonTimer != null)
                        {
                            this.trackedSkeletonTimer.Stop();
                            this.trackedSkeletonTimer = null;
                        }

                        this.HasActiveSkeleton = true;
                        this.SkeletonUpdated(updatedSkeleton);
                        return;
                    }
                }

                this.trackedSkeleton = null;
                this.Silhouette = null;

                if (this.trackedSkeletonTimer == null)
                {
                    this.trackedSkeletonTimer = new Timer(2000);
                    this.trackedSkeletonTimer.Elapsed += (o, s) =>
                        {
                            this.HasActiveSkeleton = false;
                        };
                    this.trackedSkeletonTimer.Start();
                }
            }
        }

        

        private void SpeechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (SpeechRecognized != null && e.Result.Confidence >= this.MinimumSpeechConfidence)
            {
                SpeechRecognized(this, e);
            }
        }

        private void GetPlayerSilhouette(DepthImageFrame depthFrame, int playerIndex)
        {
            if (depthFrame != null)
            {
                bool haveNewFormat = this.lastImageFormat != depthFrame.Format;

                if (haveNewFormat)
                {
                    this.pixelData = new short[depthFrame.PixelDataLength];
                    this.depthFrame32 = new byte[depthFrame.Width * depthFrame.Height * Bgra32BytesPerPixel];
                    this.convertedDepthBits = new byte[this.depthFrame32.Length];
                }

                depthFrame.CopyPixelDataTo(this.pixelData);

                for (int i16 = 0, i32 = 0; i16 < pixelData.Length && i32 < this.depthFrame32.Length; i16++, i32 += 4)
                {
                    int player = pixelData[i16] & DepthImageFrame.PlayerIndexBitmask;
                    if (player == playerIndex)
                    {
                        convertedDepthBits[i32 + RedIndex] = 0x44;
                        convertedDepthBits[i32 + GreenIndex] = 0x23;
                        convertedDepthBits[i32 + BlueIndex] = 0x59;
                        convertedDepthBits[i32 + 3] = 0x66;
                    }
                    else if (player > 0)
                    {
                        convertedDepthBits[i32 + RedIndex] = 0xBC;
                        convertedDepthBits[i32 + GreenIndex] = 0xBE;
                        convertedDepthBits[i32 + BlueIndex] = 0xC0;
                        convertedDepthBits[i32 + 3] = 0x66;
                    }
                    else
                    {
                        convertedDepthBits[i32 + RedIndex] = 0x0;
                        convertedDepthBits[i32 + GreenIndex] = 0x0;
                        convertedDepthBits[i32 + BlueIndex] = 0x0;
                        convertedDepthBits[i32 + 3] = 0x0;
                    }
                }

                if (this.Silhouette == null || haveNewFormat)
                {
                    this.Silhouette = new WriteableBitmap(
                        depthFrame.Width,
                        depthFrame.Height,
                        96,
                        96,
                        PixelFormats.Bgra32,
                        null);
                }

                this.Silhouette.WritePixels(
                    new Int32Rect(0, 0, depthFrame.Width, depthFrame.Height),
                    convertedDepthBits,
                    depthFrame.Width * Bgra32BytesPerPixel,
                    0);

                this.lastImageFormat = depthFrame.Format;
            }
        }

        private void SkeletonUpdated(Skeleton skeleton)
        {
            if (this.ActiveHand == null)
            {
                this.ActiveHand = new HandPosition();
            }

            int leftX, leftY, rightX, rightY;
            Joint leftHandJoint = skeleton.Joints[JointType.HandLeft];
            Joint rightHandJoint = skeleton.Joints[JointType.HandRight];

            float leftZ = leftHandJoint.Position.Z;
            float rightZ = rightHandJoint.Position.Z;

            ScaleXY(skeleton.Joints[JointType.ShoulderCenter], false, leftHandJoint, out leftX, out leftY);
            ScaleXY(skeleton.Joints[JointType.ShoulderCenter], true, rightHandJoint, out rightX, out rightY);

            if (leftHandJoint.TrackingState == JointTrackingState.Tracked && leftZ < rightZ && leftY < SystemParameters.PrimaryScreenHeight)
            {
                this.ActiveHand.IsLeft = true;
                this.ActiveHand.X = leftX;
                this.ActiveHand.Y = leftY;
                this.HitTestHand(this.ActiveHand);
            }
            else if (rightHandJoint.TrackingState == JointTrackingState.Tracked && rightY < SystemParameters.PrimaryScreenHeight)
            {
                this.ActiveHand.IsLeft = false;
                this.ActiveHand.X = rightX;
                this.ActiveHand.Y = rightY;
                this.HitTestHand(this.ActiveHand);
            }
            else
            {
                if (this.activeHand != null && this.activeHand.CurrentElement != null)
                {
                    this.ActiveHand.X = -1;
                    this.ActiveHand.Y = -1;
                    HandInputEventArgs args = new HandInputEventArgs(HandLeaveEvent, this.ActiveHand.CurrentElement, this.ActiveHand);
                    this.activeHand.CurrentElement.RaiseEvent(args);
                    this.activeHand.CurrentElement = null;
                }

                this.ActiveHand = null;
            }

            if (this.ActiveHand == null)
            {
                return;
            }

            if (this.ActiveHand.CurrentElement != null)
            {
                if (ScaleY(skeleton.Joints[JointType.ElbowLeft]) > ScaleY(skeleton.Joints[JointType.HandLeft])
                    || ScaleY(skeleton.Joints[JointType.ElbowRight]) > ScaleY(skeleton.Joints[JointType.HandRight]))
                {
                    HandInputEventArgs args = new HandInputEventArgs(HandRaisedEvent, this.ActiveHand.CurrentElement, this.ActiveHand);
                    this.ActiveHand.CurrentElement.RaiseEvent(args);
                }
            }
        }

        private void HitTestHand(HandPosition hand)
        {
            var pt = new Point(hand.X, hand.Y);
            IInputElement input = this.parentWindow.InputHitTest(pt);
            if (hand.CurrentElement != input)
            {
                var inputObject = input as DependencyObject;
                var currentObject = hand.CurrentElement as DependencyObject;

                // If the new input is a child of the current element then don't fire the leave event.
                // It will be fired later when the current input moves to the parent of the current element.
                if (hand.CurrentElement != null && Utility.IsElementChild(currentObject, inputObject) == false)
                {
                    // Raise the HandLeaveEvent on the CurrentElement, which at this point is the previous element the hand was over.
                    hand.CurrentElement.RaiseEvent(new HandInputEventArgs(HandLeaveEvent, hand.CurrentElement, hand));
                }

                // If the current element is the parent of the new input element then don't
                // raise the entered event as it has already been fired.
                if (input != null && Utility.IsElementChild(inputObject, currentObject) == false)
                {
                    input.RaiseEvent(new HandInputEventArgs(HandEnterEvent, input, hand));
                }

                hand.CurrentElement = input;
            }
            else if (hand.CurrentElement != null)
            {
                hand.CurrentElement.RaiseEvent(new HandInputEventArgs(HandMoveEvent, hand.CurrentElement, hand));
            }
        }

        

        #region INotifyPropertyChanged Members

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

        #endregion
    }
}