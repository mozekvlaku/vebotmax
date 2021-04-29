using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using veBot_Operator.BotModes.TimelineSequencer;

namespace veBot_Operator.BotModes
{
    class ActionInterop
    {
        private SequenceTimeline sequence;
        public bool recording;
        private SiphonaV2 siphona;
        private Stopwatch recordingStopwatch;
        private System.Timers.Timer recordingTimer;
        private veBot_Operator.Timeline viewTimeline;
        private Label timelabel;
        public ActionInterop(SiphonaV2 siphona, SequenceTimeline sequence, veBot_Operator.Timeline viewTimeline, Label timelabel)
        {
            this.sequence = sequence;
            this.timelabel = timelabel;
            this.viewTimeline = viewTimeline;
            this.siphona = siphona;
            recordingStopwatch = new Stopwatch();
            recordingTimer = new System.Timers.Timer(1000);
            recordingTimer.Elapsed += RecordingTimer_Elapsed;
        }

        private void RecordingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timelabel.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                timelabel.Content = recordingStopwatch.Elapsed.Minutes.ToString("D2") + ":" + recordingStopwatch.Elapsed.Seconds.ToString("D2") + " Recording now";
            }));
            Application.Current.Dispatcher.Invoke((Action)delegate {
                viewTimeline.RefreshLine(recordingStopwatch.Elapsed.Seconds);
            });
        }

        public void SendAction(PredefinedActions action, int strength, bool asnc)
        {
            if (recording)
            {
                TimeSpan time = recordingStopwatch.Elapsed;
                sequence.CastKeyframe(new PredefinedActionKeyframe(time, action, strength, asnc));
            }
            siphona.SendAction(action, strength, asnc);
        }
        public void ResetTimeline(veBot_Operator.Timeline tm)
        {
            viewTimeline = tm;
        }
        public void MoveServo(int servoNum, int servoDegree, bool asnc)
        {
            if (recording)
            {
                TimeSpan time = recordingStopwatch.Elapsed;
                sequence.CastKeyframe(new DirectControlKeyframe(time, servoNum, servoDegree, asnc));
            }
            siphona.MoveServo(servoNum, servoDegree, asnc);
        }

        public void Speak(string ssmlString)
        {
            if (recording)
            {
                TimeSpan time = recordingStopwatch.Elapsed;
                sequence.CastKeyframe(new TTSKeyframe(time,ssmlString));
            }
            TextToSpeech tts = new TextToSpeech(siphona, true);
            tts.Speak(ssmlString, "cs-CZ");
        }

        public void Record()
        {
            timelabel.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                timelabel.Content =  "00:00"  + " Recording now";
            }));
            viewTimeline.RefreshLine(0);
            recording = true;
            recordingStopwatch.Start();
            recordingTimer.Start();
        }

        public void PauseRecording()
        {
            recording = false;
            timelabel.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                timelabel.Content = timelabel.Content + " Paused recording ";
            }));
            recordingStopwatch.Stop();
            recordingTimer.Stop();
        }

        public void ResumeRecording()
        {
            recording = true;
            recordingStopwatch.Start();
            recordingTimer.Start();
        }

        public void StopRecording()
        {
            recording = false;
            timelabel.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                timelabel.Content = "00:00 Stopped recording ";
            }));
            viewTimeline.RefreshLine(0);
            recordingStopwatch.Reset();
            recordingTimer.Stop();
        }
    }
}
