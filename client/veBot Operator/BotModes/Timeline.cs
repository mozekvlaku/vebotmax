using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using veBot_Operator.BotModes.Models;
using veBot_Operator.BotParts;

namespace veBot_Operator.BotModes
{

    class Timeline
    {
        private List<EyesModel> eyes;
        private List<MouthModel> mouth;
        private List<TTSModel> tts;
        private List<PredefinedActionModel> prdy;

        private TimeSpan lastTimeLength;

        private Stopwatch aTimer;
        private System.Timers.Timer bTimer;
        private System.Timers.Timer cTimer;
        private Stopwatch dTimer;
        private Mouth mouthClass;
        private Eyes eyesClass;
        private SiphonaV2 siphona;
        private System.Windows.Controls.Image prevEyes;
        private System.Windows.Controls.Image prevMouth;
        private System.Windows.Controls.Image prevLids;
        private System.Windows.Controls.Label timerLbl;

        private veBot_Operator.Timeline viewTimeline;

        public Timeline(Mouth mouthClass, Eyes eyesClass, System.Windows.Controls.Image prevEyes, System.Windows.Controls.Image prevMouth, System.Windows.Controls.Image prevLids, System.Windows.Controls.Label timerLbl, veBot_Operator.Timeline viewTimeline, SiphonaV2 siphona)
        {
            eyes = new List<EyesModel>();
            mouth = new List<MouthModel>();
            tts = new List<TTSModel>();
            prdy = new List<PredefinedActionModel>();

            this.mouthClass = mouthClass;
            this.eyesClass = eyesClass;
            this.prevEyes = prevEyes;
            this.prevMouth = prevMouth;
            this.prevLids = prevLids;
            this.timerLbl = timerLbl;
            this.viewTimeline = viewTimeline;
        }
        List<int> sigmils;

        public void Play()
        {
            bTimer = new System.Timers.Timer(1000);
            dTimer = new Stopwatch();
            dTimer.Start();
            bTimer.Start();
            sigmils = this.GetSignificantMilliseconds();
            bTimer.Elapsed += BTimer_Elapsed;
            
        }

        public void Stop()
        {
            bTimer.Stop();
            dTimer.Stop();
            bTimer.Dispose();
            dTimer.Reset();
        }

        public void Reset()
        {
            eyes.Clear();
            mouth.Clear();
            tts.Clear();
            timerLbl.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                timerLbl.Content = "00:00";
            }));
        }

        public ObservableCollection<DataObject> GetPreviewTable()
        {
            var list = new ObservableCollection<DataObject>();
            foreach (MouthModel mouthModel in mouth)
            {
                list.Add(new DataObject() { action = mouthModel.action, language = "", text = "", time = mouthModel.time.ToString(@"mm\:ss"), type = "Mouth" });
            }
            foreach (EyesModel eyesModel in eyes)
            {
                list.Add(new DataObject() { action = eyesModel.action, language = "", text = "", time = eyesModel.time.ToString(@"mm\:ss"), type = "Eyes" });
            }
            foreach (TTSModel ttsModel in tts)
            {
                list.Add(new DataObject() { action = "speech", language = ttsModel.synthesisLanguage, text = ttsModel.ssmlString, time = ttsModel.time.ToString(@"mm\:ss"), type = "TTS" });
            }
            foreach (PredefinedActionModel prdModel in prdy)
            {
                list.Add(new DataObject() { action = prdModel.predefined.ToString(), language = "", text = prdModel.strength.ToString(), time = prdModel.time.ToString(@"mm\:ss"), type = "SiphonaPredef" });
            }

            return list;
        }
        
        private async void BTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            await timerLbl.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                timerLbl.Content = dTimer.Elapsed.Minutes.ToString("D2") + ":" + dTimer.Elapsed.Seconds.ToString("D2");
            }));
            if(sigmils.Contains(dTimer.Elapsed.Seconds))
            {
                var mouths = mouth.Where(x => (x.time.Seconds == dTimer.Elapsed.Seconds));
                var eyesi = eyes.Where(x => (x.time.Seconds == dTimer.Elapsed.Seconds));
                var ttsi = tts.Where(x => (x.time.Seconds == dTimer.Elapsed.Seconds));
                var prdyi = prdy.Where(x => (x.time.Seconds == dTimer.Elapsed.Seconds));
                if (mouths.Count() > 0)
                {
                    //nalezeno v ustech
                    DoMouthAction(mouths.First());
                }
                if (eyesi.Count() > 0)
                {
                    //nalezeno v ocich
                    await DoEyesActionAsync(eyesi.First());
                }
                if (ttsi.Count() > 0)
                {
                    //nalezeno v tts
                    DoTTSAction(ttsi.First());
                }
                if (prdyi.Count() > 0)
                {
                    //nalezeno v tts
                    DoPredefinedAction(prdyi.First());
                }
            }

            if(dTimer.Elapsed.Seconds >= lastTimeLength.Seconds)
            {
                Stop();
            }
        }

        private void DoPredefinedAction(PredefinedActionModel prdModel)
        {
            siphona.SendAction(prdModel.predefined, prdModel.strength, prdModel.asynchronity);
        }

        private void DoMouthAction(MouthModel mouthModel)
        {
            string action = mouthModel.action;
            switch (action)
            {
                case "openmouth":
                    mouthClass.OpenMouth();
                    prevMouth.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                        prevMouth.Source = new BitmapImage(Get("Resources/MouthPrev/MouthOpened.png"));
                    }));
                    break;
                case "closemouth":
                    mouthClass.CloseMouth();
                    prevMouth.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                        prevMouth.Source = new BitmapImage(Get("Resources/MouthPrev/MouthClosed.png"));
                    }));
                    break;
                default:
                    break;
            }
        }

        private async Task DoEyesActionAsync(EyesModel eyesModel)
        {
            string action = eyesModel.action;
            switch (action)
            {
                case "openeyes":
                    eyesClass.OpenLids();
                    await prevLids.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevLids.Source = new BitmapImage(Get("Resources/EyesPrev/LidsOpen.png"));
                    }));
                    break;
                case "closeeyes":
                    eyesClass.CloseLids();
                    await prevLids.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevLids.Source = new BitmapImage(Get("Resources/EyesPrev/LidsClosed.png"));
                    }));
                    break;
                case "blink":
                    eyesClass.Blink();
                    await prevLids.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevLids.Source = new BitmapImage(Get("Resources/EyesPrev/LidsClosed.png"));
                    }));
                    await Task.Delay(400);
                    await prevLids.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevLids.Source = new BitmapImage(Get("Resources/EyesPrev/LidsOpen.png"));
                    }));
                    break;
                case "sonnyblink":
                    eyesClass.SonnyBlink();
                    await prevLids.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevLids.Source = new BitmapImage(Get("Resources/EyesPrev/LidsMonoBlink.png"));
                    }));
                    await Task.Delay(400);
                    await prevLids.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevLids.Source = new BitmapImage(Get("Resources/EyesPrev/LidsOpen.png"));
                    }));
                    break;
                case "reset":
                    eyesClass.Reset();
                    await prevLids.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevLids.Source = new BitmapImage(Get("Resources/EyesPrev/LidsResting.png"));
                    }));
                    await prevEyes.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevEyes.Source = new BitmapImage(Get("Resources/EyesPrev/EyesCenter.png"));
                    }));
                    break;
                case "vykuloci":
                    eyesClass.VykulOci();
                    await prevLids.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevLids.Source = new BitmapImage(Get("Resources/EyesPrev/LidsSurprised.png"));
                    }));
                    break;
                case "envy":
                    eyesClass.Envy();
                    await prevLids.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevLids.Source = new BitmapImage(Get("Resources/EyesPrev/LidsEnvy.png"));
                    }));
                    break;
                case "dodgeface":
                    eyesClass.DodgeFace();
                    await prevLids.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevLids.Source = new BitmapImage(Get("Resources/EyesPrev/LidsDisgusted.png"));
                    }));
                    break;
                case "dement":
                    eyesClass.Dement();
                    await prevLids.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevLids.Source = new BitmapImage(Get("Resources/EyesPrev/LidsLobotomy.png"));
                    }));
                    break;
                case "center":
                    eyesClass.Center();
                    await prevEyes.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevEyes.Source = new BitmapImage(Get("Resources/EyesPrev/EyesCenter.png"));
                    }));
                    break;
                case "movecenter":
                    eyesClass.MoveCenter();
                    await prevEyes.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevEyes.Source = new BitmapImage(Get("Resources/EyesPrev/EyesCenter.png"));
                    }));
                    break;
                case "movedown":
                    eyesClass.MoveDown();
                    await prevEyes.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevEyes.Source = new BitmapImage(Get("Resources/EyesPrev/EyesDown.png"));
                    }));
                    break;
                case "moveup":
                    eyesClass.MoveUp();
                    await prevEyes.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevEyes.Source = new BitmapImage(Get("Resources/EyesPrev/EyesUp.png"));
                    }));
                    break;
                case "moveleft":
                    eyesClass.MoveToLeft();
                    await prevEyes.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevEyes.Source = new BitmapImage(Get("Resources/EyesPrev/EyesLeft.png"));
                    }));
                    break;
                case "moveright":
                    eyesClass.MoveToRight();
                    await prevEyes.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevEyes.Source = new BitmapImage(Get("Resources/EyesPrev/EyesRight.png"));
                    }));
                    break;
                case "moveleftup":
                    eyesClass.MoveLeftUp();
                    await prevEyes.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevEyes.Source = new BitmapImage(Get("Resources/EyesPrev/EyesUpLeft.png"));
                    }));
                    break;
                case "moveleftdown":
                    eyesClass.MoveLeftDown();
                    await prevEyes.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevEyes.Source = new BitmapImage(Get("Resources/EyesPrev/EyesDownLeft.png"));
                    }));
                    break;
                case "moverightup":
                    eyesClass.MoveRightUp();
                    await prevEyes.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevEyes.Source = new BitmapImage(Get("Resources/EyesPrev/EyesUpRight.png"));
                    }));
                    break;
                case "moverightdown":
                    eyesClass.MoveRightDown();
                    await prevEyes.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                    prevEyes.Source = new BitmapImage(Get("Resources/EyesPrev/EyesDownRight.png"));
                    }));
                    break;
                case "vain":
                    eyesClass.MoveToLeft();
                    await prevEyes.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                        prevEyes.Source = new BitmapImage(Get("Resources/EyesPrev/EyesLeft.png"));
                    }));
                    await Task.Delay(300);
                    eyesClass.MoveUp();
                    await prevEyes.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                        prevEyes.Source = new BitmapImage(Get("Resources/EyesPrev/EyesUp.png"));
                    }));
                    await Task.Delay(300);
                    eyesClass.MoveToRight();
                    await prevEyes.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                        {
                            prevEyes.Source = new BitmapImage(Get("Resources/EyesPrev/EyesRight.png"));
                        }));
                    await Task.Delay(400);
                    eyesClass.MoveCenter();
                    await prevEyes.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                            {
                                prevEyes.Source = new BitmapImage(Get("Resources/EyesPrev/EyesCenter.png"));
                            }));
                    break;
                default:
                    break;
            }
        }

        private void DoTTSAction(TTSModel ttsModel)
        {
            string text = ttsModel.ssmlString;
            string language = ttsModel.synthesisLanguage;
            var synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();
            synthesizer.SelectVoice("Microsoft Jakub"); //Speechtech Jan
            synthesizer.SpeakSsmlAsync(text);
            synthesizer.PhonemeReached += Synthesizer_PhonemeReached;
        }
        private void Synthesizer_PhonemeReached(object sender, PhonemeReachedEventArgs e)
        {
            string state = mouthClass.PronouncePhoneme(e.NextPhoneme);
            prevMouth.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
               // prevMouth.Source = new BitmapImage(Get("Resources/MouthPrev/Mouth" + state + ".png"));
            }));
        }

        private static Uri Get(string resourcePath)
        {
            var uri = string.Format(
                "pack://application:,,,/{0};component/{1}"
                , Assembly.GetExecutingAssembly().GetName().Name
                , resourcePath
            );

            return new Uri(uri);
        }
        private List<int> GetSignificantMilliseconds()
        {
            List<int> sigmil = new List<int>();
            foreach (MouthModel mouthModel in mouth)
            {
                sigmil.Add(mouthModel.time.Seconds);
            }
            foreach (EyesModel eyesModel in eyes)
            {
                sigmil.Add(eyesModel.time.Seconds);
            }
            foreach (TTSModel ttsModel in tts)
            {
                sigmil.Add(ttsModel.time.Seconds);
            }
            return sigmil;
        }

        public void StartTimer()
        {
            aTimer = new Stopwatch();
            cTimer = new System.Timers.Timer(1000);
            aTimer.Start();
            
            cTimer.Start();
            cTimer.Elapsed += CTimer_Elapsed;
            
        }

        private void CTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerLbl.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                timerLbl.Content = aTimer.Elapsed.Minutes.ToString("D2") + ":" + aTimer.Elapsed.Seconds.ToString("D2");
            }));
        }

        public void PauseTimer()
        {
            aTimer.Stop();
            cTimer.Stop();
            cTimer.Close();
            cTimer.Dispose();
        }

        public void StopTimer()
        {
            lastTimeLength = aTimer.Elapsed;
            aTimer.Stop();
            aTimer.Reset();
            cTimer.Stop();
            cTimer.Dispose();
            timerLbl.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                    timerLbl.Content = "00:00, stopped at " + lastTimeLength.ToString();
            }));
        }

        public void ResetTimeline()
        {
            if(aTimer.IsRunning)
            {
                StopTimer();
            }
            tts.Clear();
            mouth.Clear();
            eyes.Clear();
            timerLbl.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                timerLbl.Content = "00:00";
            }));
        }

        public void CastSnapshotTTS(string text, string language)
        {
            tts.Add(new TTSModel(aTimer.Elapsed, text, language));
            viewTimeline.AddElement(aTimer.Elapsed.Seconds, "speak", "");
        }
        public void CastSnapshotMouth(string action)
        {
            mouth.Add(new MouthModel(aTimer.Elapsed,action));
            viewTimeline.AddElement(aTimer.Elapsed.Seconds, action, "");
        }
        public void CastSnapshotEyes(string action)
        {
            eyes.Add(new EyesModel(aTimer.Elapsed, action));
            viewTimeline.AddElement(aTimer.Elapsed.Seconds, action, "");
        }

        public void CastSnapshotPredefinedAction(PredefinedActions predefined, int strength, bool asynchronity)
        {
            prdy.Add(new PredefinedActionModel(aTimer.Elapsed,predefined, strength,asynchronity));
            viewTimeline.AddElement(aTimer.Elapsed.Seconds, predefined.ToString(), "?");
        }
    }
    public class DataObject
    {
        public string time { get; set; }
        public string type { get; set; }
        public string action { get; set; }
        public string language { get; set; }
        public string text { get; set; }
    }

}
