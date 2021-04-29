using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace veBot_Operator.BotModes.TimelineSequencer
{
    class SequenceTimeline
    {
        private SiphonaV2 siphona;
        private List<Keyframe> currentSequence;
        private veBot_Operator.Timeline viewTimeline;
        private Timer playbackTimer;
        private Stopwatch playbackStopwatch;
        private Label timelabel;
        private TimeSpan lastTimeLength;
        public SequenceTimeline(SiphonaV2 siphonaConnection, veBot_Operator.Timeline viewTimeline, Label timelabel)
        {
            currentSequence = new List<Keyframe>();
            this.siphona = siphonaConnection;
            this.viewTimeline = viewTimeline;
            this.timelabel = timelabel;
            playbackTimer = new Timer(1000);
            playbackStopwatch = new Stopwatch();
            playbackTimer.Elapsed += PlaybackTimer_Elapsed;
        }

        private void PlaybackTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var keyframes = currentSequence.Where(x => (x.time.Seconds == playbackStopwatch.Elapsed.Seconds));
            if (keyframes.Count() > 0)
            {
                for(int i = 0; i < keyframes.Count(); i++)
                {
                    keyframes.ElementAt(i).PlayKeyframe(siphona);
                }
            }
            timelabel.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new System.Threading.ThreadStart(() =>
            {
                timelabel.Content = playbackStopwatch.Elapsed.Minutes.ToString("D2") + ":" + playbackStopwatch.Elapsed.Seconds.ToString("D2") + " Playing...";
            }));
            Application.Current.Dispatcher.Invoke((Action)delegate {
                viewTimeline.RefreshLine(playbackStopwatch.Elapsed.Seconds);
            });
            if (playbackStopwatch.Elapsed.Seconds >= lastTimeLength.Seconds)
            {
                StopSequence();
            }
        }

        public void CastKeyframe(Keyframe kf)
        {
            Trace.WriteLine(kf.time);
            Trace.WriteLine(kf.GetIdentification());
            currentSequence.Add(kf);
            viewTimeline.AddElement(kf.time.Seconds, kf.GetIdentification(), kf.GetIcon());
        }

        public void RemoveKeyframe(Keyframe kf)
        {
            currentSequence.Remove(kf);
        }

        public void PlaySequence()
        {

            lastTimeLength = currentSequence.Last().time;
            var keyframes = currentSequence.Where(x => (x.time.Seconds ==0));
            if (keyframes.Count() > 0)
            {
                for (int i = 0; i < keyframes.Count(); i++)
                {
                    keyframes.ElementAt(i).PlayKeyframe(siphona);
                }
            }
            timelabel.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new System.Threading.ThreadStart(() =>
            {
                timelabel.Content = "00:00" + " Playing...";
            }));
            Application.Current.Dispatcher.Invoke((Action)delegate {
                viewTimeline.RefreshLine(0);
            });
            playbackTimer.Start();
            playbackStopwatch.Start();
        }

        public void ResetSequence()
        {
            currentSequence.Clear();
            
        }
        public void PlaySequence(TimeSpan startingTime)
        {

        }

        public void ResetTimeline(veBot_Operator.Timeline tm)
        {
            viewTimeline = tm;
        }

        public void PauseSequence()
        {
            timelabel.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new System.Threading.ThreadStart(() =>
            {
                timelabel.Content = timelabel.Content + " paused";
            }));
            playbackTimer.Stop();
            playbackStopwatch.Stop();
        }
        public void StopSequence()
        {
            timelabel.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new System.Threading.ThreadStart(() =>
            {
                timelabel.Content = "00:00";
            }));
            playbackTimer.Stop();
            viewTimeline.RefreshLine(0);
            playbackStopwatch.Reset();
            playbackStopwatch.Stop();
        }
        public void ResumeSequence()
        {
            playbackTimer.Start();
            playbackStopwatch.Start();
        }

        public void SaveSequence()
        {
            Stream myStream;
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = "veBot sequences (*.bsq)|*.bsq";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.DefaultExt = ".bsq";
            saveFileDialog.Title = "Save a veBot sequence";
            if(saveFileDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                if ((myStream = saveFileDialog.OpenFile()) != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(myStream, currentSequence);
                    myStream.Close();
                }
            }
           
        }
        public void OpenSequence()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "veBot sequences (*.bsq)|*.bsq";
            openFileDialog.FilterIndex = 1;
            openFileDialog.DefaultExt = ".bsq";
            openFileDialog.Title = "Open a veBot sequence";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Get the path of specified file
                var filePath = openFileDialog.FileName;

                //Read the contents of the file into a stream
                var fileStream = openFileDialog.OpenFile();
                BinaryFormatter bf = new BinaryFormatter();

                currentSequence = (List<Keyframe>)bf.Deserialize(fileStream);

            }

            foreach(Keyframe kf in currentSequence)
            {
                viewTimeline.AddElement(kf.time.Seconds, kf.GetIdentification(), kf.GetIcon());
            }
        }
        public void OpenSequence(string filename)
        {
                FileStream fileStream = File.Open(filename, FileMode.Open);
      
                BinaryFormatter bf = new BinaryFormatter();

                currentSequence = (List<Keyframe>)bf.Deserialize(fileStream);

            foreach (Keyframe kf in currentSequence)
            {
                viewTimeline.AddElement(kf.time.Seconds, kf.GetIdentification(), kf.GetIcon());
            }
        }
    }
}
