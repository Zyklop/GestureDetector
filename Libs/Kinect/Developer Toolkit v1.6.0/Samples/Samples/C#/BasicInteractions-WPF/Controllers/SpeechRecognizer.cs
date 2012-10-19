//------------------------------------------------------------------------------
// <copyright file="SpeechRecognizer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Kinect;
    using Microsoft.Speech.AudioFormat;
    using Microsoft.Speech.Recognition;

    internal enum NumberWords
    {
        One = 1,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Eleven,
        Twelve,
        Thirteen,
        Fourteen,
        Fifteen,
        Sixteen,
        Seventeen,
        Eighteen,
        Nineteen,
        Twenty,
    }

    public class SpeechRecognizer : IDisposable
    {
        private readonly RecognizerInfo recognizerInfo;
        private SpeechRecognitionEngine speechRecognitionEngine;
        private KinectSensor sensor;
        private bool sensorActive;

        public SpeechRecognizer()
        {
            this.recognizerInfo = GetKinectRecognizer();
            if (this.recognizerInfo != null)
            {
                this.speechRecognitionEngine = new SpeechRecognitionEngine(this.recognizerInfo.Id);
                this.speechRecognitionEngine.SpeechRecognized += this.SpeechRecognitionEngine_SpeechRecognized;
                this.speechRecognitionEngine.SpeechDetected += this.SpeechRecognitionEngine_SpeechDetected;
            }
        }

        public event EventHandler<SpeechRecognizedEventArgs> SpeechRecognized;

        public event EventHandler<SpeechDetectedEventArgs> SpeechDetected;

        public void SetSensor(KinectSensor newSensor)
        {
            if (this.speechRecognitionEngine == null)
            {
                return;
            }

            if (this.sensor != null)
            {
                this.sensorActive = false;
                this.speechRecognitionEngine.RecognizeAsyncStop();
            }

            this.sensor = newSensor;
            if (this.sensor != null)
            {
                this.sensor.AudioSource.NoiseSuppression = true;
                this.sensor.AudioSource.AutomaticGainControlEnabled = false;
                this.sensor.AudioSource.EchoCancellationMode = EchoCancellationMode.None;

                Stream s = this.sensor.AudioSource.Start();

                // Set the Kinect AudioSource stream as the audio stream for the speech recognition engine.
                this.speechRecognitionEngine.SetInputToAudioStream(
                    s,
                                                                   new SpeechAudioFormatInfo(
                                                                       EncodingFormat.Pcm,
                                                                       16000,
                                                                       16,
                                                                       1,
                                                                       32000,
                                                                       2,
                                                                       null));
                this.sensorActive = true;

                // Begin speech recognition.
                if (this.speechRecognitionEngine.Grammars.Count > 0)
                {
                    this.speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
                }
            }
        }



        public void SetSpeechGrammar(IEnumerable<string> speechChoices)
        {
            if (speechChoices == null)
            {
                throw new ArgumentNullException("speechChoices");
            }

            if (this.speechRecognitionEngine != null)
            {
                this.speechRecognitionEngine.RecognizeAsyncStop();
                this.speechRecognitionEngine.UnloadAllGrammars();

                // Create a Grammar for the speech recognizer, using the words or phrases from the passed in list.
                var gb = new GrammarBuilder();
                gb.Culture = this.recognizerInfo.Culture;

                var choices = new Choices();
                foreach (string choice in speechChoices)
                {
                    choices.Add(choice);
                }

                gb.Append(choices);

                var g = new Grammar(gb);

                // Load the created Grammar into the speech recognition engine, and start the Kinect AudioSource stream.
                this.speechRecognitionEngine.LoadGrammar(g);

                if (this.sensorActive)
                {
                    this.speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the recognizer info for Kinect.
        /// </summary>
        /// <returns>The recognizer info</returns>
        private static RecognizerInfo GetKinectRecognizer()
        {
            // Check for the Kinect language recognizer for a particular culture and return it.
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return
                    "True".Equals(value, StringComparison.OrdinalIgnoreCase) 
                    && "en-US".Equals(r.Culture.Name, StringComparison.OrdinalIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "speechRecognitionEngine",
            Justification = "In a full-fledged application, the SpeechRecognitionEngine object should be properly disposed. For the sake of simplicity, we're omitting that code in this sample.")]
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.sensor != null)
                {
                    this.sensor.AudioSource.Stop();
                    this.sensor = null;
                }

                if (this.speechRecognitionEngine != null)
                {
                    this.speechRecognitionEngine.RecognizeAsyncCancel();
                    this.speechRecognitionEngine = null;
                }
            }
        }

        private void SpeechRecognitionEngine_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            if (this.SpeechDetected != null)
            {
                this.SpeechDetected(this, e);
            }
        }

        /// <summary>
        /// Event handler for when speech has been recognized.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Speech Event Args</param>
        private void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Notify the KinectController that speech has been recognized.
            if (this.SpeechRecognized != null)
            {
                this.SpeechRecognized(this, e);
            }
        }
    }       
}