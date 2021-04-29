using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using veBot_Operator.BotParts;
using System.Collections.ObjectModel;
using MjpegProcessor;
using Renci.SshNet;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Speech.Synthesis;
using System.Globalization;
using RichTextBox = System.Windows.Controls.RichTextBox;
using MessageBox = System.Windows.MessageBox;
using FaceRecognitionDotNet;
using Image = FaceRecognitionDotNet.Image;
using System.IO;
using Emgu.CV.Face;

using veBot_Operator.BotModes;
using System.Windows.Threading;
using System.Diagnostics;
using veBot_Operator.BotModes.TimelineSequencer;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace veBot_Operator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Eyes ex;
        private Mouth mt;
        MjpegDecoder _mjpeg;
        SshClient cSSH;
        BitmapImage lastImage;

        private bool isSiphona = true;
        private bool isEasing = true;

        private bool _recordingMode = false;
        private bool _passiveMode = false;
        private bool _paused = false;
        private SerialConnector sc;
        private SiphonaV2 sip2;
        private ActionInterop action;
        private SequenceTimeline sequence;
        private BotModes.Timeline timelineClass;
        private BotModes.Passive ps;
        private bool asynchronity = true;

        private string eyever = "EyesSkin";


        private veBot_Operator.Timeline viewTimeline;

        private int XscaleVar = 50;
        private int YscaleVar = 50;

        private FaceExpression fex;

        public MainWindow()
        {
            sc = new SerialConnector(false);
            InitializeComponent();

            ex = new Eyes(sc);
            sip2 = new SiphonaV2(15);
            mt = new Mouth(sc, sip2, isSiphona, asynchronity);
            timelineClass = new BotModes.Timeline(mt, ex, prevEyes, prevMouth, prevLids, lengthLbl, viewTimeline, sip2);
            _mjpeg = new MjpegDecoder();
            _mjpeg.FrameReady += mjpeg_FrameReady;
            ps = new Passive(sip2);
            sequence = new SequenceTimeline(sip2, viewTimeline, lengthLbl);
            action = new ActionInterop(sip2, sequence, viewTimeline, lengthLbl);

            // fex = new FaceExpression(imaged);

            sc.isEasing = isEasing;
            sc.isSiphona = isSiphona;
            SiphonaSetting.IsChecked = isSiphona;
            EasingSetting.IsChecked = isEasing;

            Xscale.Value = XscaleVar;
            Yscale.Value = YscaleVar;

            string[] arguments = Environment.GetCommandLineArgs();
            try
            {
                if (arguments[1] != null)
                {
                    sequence.OpenSequence(arguments[1]);
                    this.Title = System.IO.Path.GetFileName(arguments[1]) + " | veBot Operator";
                }
            }
            catch { }



        }
        private void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
        {
            BitmapImage bm = e.BitmapImage;

            image.Source = bm;
            lastImage = bm;
        }



        private void sshEst_btn_Click(object sender, RoutedEventArgs e)
        {
            sshEst();
        }

        public void sshEst()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                cmdinn.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                {

                    cmdinn.StartProcess("plink.exe", "pi@" + comport.Text + " -pw 868686");
                }));
            }).Start();



            sshStatus_txt.Foreground = new SolidColorBrush(Colors.LightGreen);
        }

        private void sshClose_btn_Click(object sender, RoutedEventArgs e)
        {
            cmdinn.StopProcess();
            cmdinn.ClearOutput();
            sshStatus_txt.Foreground = new SolidColorBrush(Colors.Red);
        }

        private void sshStartScript_btn_Click(object sender, RoutedEventArgs e)
        {
            sshStart();
        }

        public void sshStart()
        {
            cmdinn.ClearOutput();
            string comnd;
            if (isSiphona)
                comnd = "python3 siphona2.py";
            else
                comnd = "python3 vebotctrl.py";

            cmdinn.WriteInput(comnd, Colors.Green, true);
        }
        private void sshStopScript_btn_Click(object sender, RoutedEventArgs e)
        {
            String comnd = "\x03";
            cmdinn.WriteInput(comnd, Colors.Red, true);
        }

        private async void sshResetScript_btn_Click(object sender, RoutedEventArgs e)
        {
            cmdinn.ClearOutput();
            String comnd = "sudo shutdown now";
            cmdinn.WriteInput(comnd, Colors.Green, true);
            await Task.Delay(1500);
            String comnda = "868686";
            cmdinn.WriteInput(comnda, Colors.Green, true);
        }

        private void cameraLoad_btn_Click(object sender, RoutedEventArgs e)
        {
            loadcam();
        }

        public void loadcam()
        {
            try
            {
                _mjpeg.ParseStream(new Uri("http://" + comport.Text + ":8080/?action=stream"));

                cameraStatus_txt.Foreground = new SolidColorBrush(Colors.LightGreen);
                _mjpeg.Error += _mjpeg_Error;
            }
            catch
            {
                image.Source = null;
            }
        }

        private void _mjpeg_Error(object sender, MjpegProcessor.ErrorEventArgs e)
        {
            cameraStatus_txt.Foreground = new SolidColorBrush(Colors.Red);
            image.Source = null;
        }

        private void netConnect_btn_Click(object sender, RoutedEventArgs e)
        {
            socketconnect();
        }

        private void socketconnect()
        {
            if (isSiphona)
            {
                sip2.Open(comport.Text, System.Net.Sockets.ProtocolType.Tcp);
            }
            else
            {
                ex.OpenConnection(comport.Text);
            }

            socketStatus_txt.Foreground = new SolidColorBrush(Colors.LightGreen);
            sc.connected = true;
          
        }

        private void netDisconnect_btn_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
            {
                sip2.Close();
            }
            else
            {
                ex.CloseConnection();
            }

            sc.connected = false;
            socketStatus_txt.Foreground = new SolidColorBrush(Colors.Red);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isSiphona)
            {
                sip2.Close();
            }
            else
            {
                ex.CloseConnection();
            }

            cmdinn.StopProcess();
        }

        private void manCenter_Click(object sender, RoutedEventArgs e)
        {

            if (isSiphona)
            {
                
                action.SendAction(PredefinedActions.RESET_EYES, 100, asynchronity);
            }
            else
            {
                if (_recordingMode)
                {
                    timelineClass.CastSnapshotEyes("center");
                    //TimelineProzatim.ItemsSource = timelineClass.GetPreviewTable();
                }
                ex.MoveCenter();
            }


            prevEyes.Source = new BitmapImage(Get("Resources/" + eyever + "/EyesCenter.png"));
        }

        private void manLeft_Click(object sender, RoutedEventArgs e)
        {

            if (isSiphona)
            {
                action.SendAction(PredefinedActions.EYES_X, 0, asynchronity);
            }
            else
            {
                if (_recordingMode)
                {
                    timelineClass.CastSnapshotEyes("moveleft");
                    //TimelineProzatim.ItemsSource = timelineClass.GetPreviewTable();
                }
                ex.MoveToLeft();
            }
            prevEyes.Source = new BitmapImage(Get("Resources/" + eyever + "/EyesLeft.png"));
        }
        public static Uri Get(string resourcePath)
        {
            var uri = string.Format(
                "pack://application:,,,/{0};component/{1}"
                , Assembly.GetExecutingAssembly().GetName().Name
                , resourcePath
            );

            return new Uri(uri);
        }
        private void manRight_Click(object sender, RoutedEventArgs e)
        {

            if (isSiphona)
            {
                action.SendAction(PredefinedActions.EYES_X, 100, asynchronity);
            }
            else
            {
                if (_recordingMode)
                {
                    timelineClass.CastSnapshotEyes("moveright");
                    //TimelineProzatim.ItemsSource = timelineClass.GetPreviewTable();
                }
                ex.MoveToRight();
            }

            prevEyes.Source = new BitmapImage(Get("Resources/" + eyever + "/EyesRight.png"));
        }

        private void manUp_Click(object sender, RoutedEventArgs e)
        {

            if (isSiphona)
            {
                
                action.SendAction(PredefinedActions.EYES_Y, 0, asynchronity);
            }

            else
            {
                if (_recordingMode)
                {
                    timelineClass.CastSnapshotEyes("moveup");
                    //TimelineProzatim.ItemsSource = timelineClass.GetPreviewTable();
                }
                ex.MoveUp();
            }


            prevEyes.Source = new BitmapImage(Get("Resources/" + eyever + "/EyesUp.png"));
        }

        private void manDown_Click(object sender, RoutedEventArgs e)
        {

            if (isSiphona)
            {
                action.SendAction(PredefinedActions.EYES_Y, 100, asynchronity); }
            else
            {
                if (_recordingMode)
                {
                    timelineClass.CastSnapshotEyes("movedown");
                    //TimelineProzatim.ItemsSource = timelineClass.GetPreviewTable();
                }
                ex.MoveDown(); }

            prevEyes.Source = new BitmapImage(Get("Resources/" + eyever + "/EyesDown.png"));
        }

        private void manUpLeft_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
            {
                action.SendAction(PredefinedActions.EYES_DIAG_LEFT, 0, asynchronity);
            }
            else
            {
                if (_recordingMode)
                {
                    timelineClass.CastSnapshotEyes("moveleftup");
                    //TimelineProzatim.ItemsSource = timelineClass.GetPreviewTable();
                }
                ex.MoveLeftUp();
            }
            prevEyes.Source = new BitmapImage(Get("Resources/" + eyever + "/EyesUpLeft.png"));
        }

        private void manUpRight_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
            {
                action.SendAction(PredefinedActions.EYES_DIAG_RIGHT, 0, asynchronity);
            }
            else
            {
                if (_recordingMode)
                {
                    timelineClass.CastSnapshotEyes("moverightup");
                    //TimelineProzatim.ItemsSource = timelineClass.GetPreviewTable();
                }
                ex.MoveRightUp();
            }
            prevEyes.Source = new BitmapImage(Get("Resources/" + eyever + "/EyesUpRight.png"));
        }

        private void manDownLeft_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
            {
                action.SendAction(PredefinedActions.EYES_DIAG_LEFT, 100, asynchronity);
            }
            else
            {
                if (_recordingMode)
                {
                    timelineClass.CastSnapshotEyes("moveleftdown");
                    //TimelineProzatim.ItemsSource = timelineClass.GetPreviewTable();
                }
                ex.MoveLeftDown();
            }
            prevEyes.Source = new BitmapImage(Get("Resources/" + eyever + "/EyesDownLeft.png"));
        }

        private void manDownRight_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
            {
                action.SendAction(PredefinedActions.EYES_DIAG_RIGHT, 100, asynchronity);
            }
            else
            {
                if (_recordingMode)
                {
                    timelineClass.CastSnapshotEyes("moverightdown");
                    //TimelineProzatim.ItemsSource = timelineClass.GetPreviewTable();
                }
                ex.MoveRightDown();
            }
            prevEyes.Source = new BitmapImage(Get("Resources/" + eyever + "/EyesDownRight.png"));
        }

        private void manClose_Click(object sender, RoutedEventArgs e)
        {

            if (isSiphona)
            {
                action.SendAction(PredefinedActions.CLOSE_EYES, 100, asynchronity);
            }

            else
            {
                if (_recordingMode)
                {
                    timelineClass.CastSnapshotEyes("closeeyes");
                    //TimelineProzatim.ItemsSource = timelineClass.GetPreviewTable();
                }
                ex.CloseLids();
            }


            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsClosed.png"));
        }

        private void manReset_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
            {
                action.SendAction(PredefinedActions.RESET_EYES, 100, asynchronity);
            }

            else
            {
                if (_recordingMode)
                {
                    timelineClass.CastSnapshotEyes("reset");
                    //TimelineProzatim.ItemsSource = timelineClass.GetPreviewTable();
                }
                ex.CloseLids();
            }


            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsResting.png"));
            prevEyes.Source = new BitmapImage(Get("Resources/" + eyever + "/EyesCenter.png"));
        }
        //CONTINUE
        private void manOpen_Click(object sender, RoutedEventArgs e)
        {
            
            if (isSiphona)
                action.SendAction(PredefinedActions.OPEN_EYES, 100, asynchronity);
            else
                ex.OpenLids();
            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsOpen.png"));
        }

        private async void actBlink_Click(object sender, RoutedEventArgs e)
        {
            
            if (isSiphona)
                action.SendAction(PredefinedActions.BLINK, 100, asynchronity);
            else
                ex.Blink();
            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsClosed.png"));
            await Task.Delay(100);
            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsOpen.png"));
        }

        private async void actBlinkOne_Click(object sender, RoutedEventArgs e)
        {
            
            if (isSiphona)
                action.SendAction(PredefinedActions.SINGLE_BLINK_LEFT, 100, asynchronity);
            else
                ex.SonnyBlink();
            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsMonoBlink.png"));
            await Task.Delay(400);
            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsOpen.png"));
        }



        private void actEnvy_Click(object sender, RoutedEventArgs e)
        {
            
            if (isSiphona)
                action.SendAction(PredefinedActions.DISGUSTED, 100, asynchronity);
            else
                ex.Envy();
            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsEnvy.png"));
        }

        private void basClose_Click(object sender, RoutedEventArgs e)
        {
           
            if (isSiphona)
                action.SendAction(PredefinedActions.MOUTH, 0, asynchronity);
            else
                ex.CloseMouth();
        }

        private void basOpen_Click(object sender, RoutedEventArgs e)
        {
            
            if (isSiphona)
                action.SendAction(PredefinedActions.MOUTH, 100, asynchronity);
            else
                ex.OpenMouth();
        }

        private void synthButt_Click(object sender, RoutedEventArgs e)
        {
            if(isSiphona)
            {
                action.Speak(StringFromRichTextBox(synthText));
            }
            else
            {
                var synthesizer = new SpeechSynthesizer();
                synthesizer.SetOutputToDefaultAudioDevice();
                synthesizer.SelectVoice("Microsoft Jakub"); //Speechtech Jan
                var builder = new PromptBuilder();
                builder.StartVoice(new CultureInfo("cs-CZ"));
                builder.AppendText(StringFromRichTextBox(synthText));
                builder.EndVoice();
                string ssmlString = "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"" + "cs-CZ" + "\">\n<voice name=\"en-US-AriaRUS\">\n";
                ssmlString += StringFromRichTextBox(synthText);
                ssmlString += "\n</voice>\n</speak>";
                synthesizer.SpeakSsmlAsync(ssmlString);

                synthesizer.SpeakProgress += Synthesizer_SpeakProgress;
                synthesizer.PhonemeReached += Synthesizer_PhonemeReached;
                synthesizer.VisemeReached += Synthesizer_VisemeReached;
                synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
            }
            
        }

        private void Synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            if (isSiphona)
                sip2.SendAction(PredefinedActions.RESET_MOUTH, 100, asynchronity);
            else
                sc.Send("resetMouth");
        }

        private void Synthesizer_VisemeReached(object sender, VisemeReachedEventArgs e)
        {
            //ttsDebug.Text += e.Viseme;
        }

        private void Synthesizer_PhonemeReached(object sender, PhonemeReachedEventArgs e)
        {

            // ttsDebug.Text += e.Phoneme;
            string state = mt.PronouncePhoneme(e.Phoneme);

            prevMouth.Source = new BitmapImage(Get("Resources/MouthPrev/Mouth" + state + ".png"));
        }

        private void Synthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {


        }

        string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                // TextPointer to the start of content in the RichTextBox.
                rtb.Document.ContentStart,
                // TextPointer to the end of content in the RichTextBox.
                rtb.Document.ContentEnd
            );

            // The Text property on a TextRange object returns a string
            // representing the plain text content of the TextRange.
            return textRange.Text;
        }

        private void ttsBr12_Click(object sender, RoutedEventArgs e)
        {
            synthText.AppendText("<break time=\"500ms\"/>\n");
        }

        private void ttsBr1_Click(object sender, RoutedEventArgs e)
        {
            synthText.AppendText("<break time=\"1s\"/>\n");
        }

        private void ttsBr3_Click(object sender, RoutedEventArgs e)
        {
            synthText.AppendText("<break time=\"3s\"/>\n");
        }

        private void ttsEm1_Click(object sender, RoutedEventArgs e)
        {
            synthText.AppendText("<emphasis level=\"reduced\"></emphasis>\n");
            synthText.Focus();
            synthText.CaretPosition = synthText.CaretPosition.DocumentEnd;
            synthText.CaretPosition = synthText.CaretPosition.GetPositionAtOffset(-15);
        }

        private void ttsEm2_Click(object sender, RoutedEventArgs e)
        {
            synthText.AppendText("<emphasis level=\"moderate\"></emphasis>\n");
            synthText.Focus();
            synthText.CaretPosition = synthText.CaretPosition.DocumentEnd;
            synthText.CaretPosition = synthText.CaretPosition.GetPositionAtOffset(-15);
        }

        private void ttsEm3_Click(object sender, RoutedEventArgs e)
        {
            synthText.AppendText("<emphasis level=\"strong\"></emphasis>\n");
            synthText.Focus();
            synthText.CaretPosition = synthText.CaretPosition.DocumentEnd;
            synthText.CaretPosition = synthText.CaretPosition.GetPositionAtOffset(-15);
        }

        private void ttsVo1_Click(object sender, RoutedEventArgs e)
        {
            synthText.AppendText("<prosody volume=\"x-soft\"></prosody>\n");
            synthText.Focus();
            synthText.CaretPosition = synthText.CaretPosition.DocumentEnd;
            synthText.CaretPosition = synthText.CaretPosition.GetPositionAtOffset(-14);
        }

        private void ttsVo2_Click(object sender, RoutedEventArgs e)
        {
            synthText.AppendText("<prosody volume=\"medium\"></prosody>\n");
            synthText.Focus();
            synthText.CaretPosition = synthText.CaretPosition.DocumentEnd;
            synthText.CaretPosition = synthText.CaretPosition.GetPositionAtOffset(-14);
        }

        private void ttsVo3_Click(object sender, RoutedEventArgs e)
        {
            synthText.AppendText("<prosody volume=\"x-loud\"></prosody>\n");
            synthText.Focus();
            synthText.CaretPosition = synthText.CaretPosition.DocumentEnd;
            synthText.CaretPosition = synthText.CaretPosition.GetPositionAtOffset(-14);
        }

        private void ttsSp1_Click(object sender, RoutedEventArgs e)
        {
            synthText.AppendText("<prosody rate=\"x-slow\"></prosody>\n");
            synthText.Focus();
            synthText.CaretPosition = synthText.CaretPosition.DocumentEnd;
            synthText.CaretPosition = synthText.CaretPosition.GetPositionAtOffset(-14);
        }

        private void ttsSp2_Click(object sender, RoutedEventArgs e)
        {
            synthText.AppendText("<prosody rate=\"medium\"></prosody>\n");
            synthText.Focus();
            synthText.CaretPosition = synthText.CaretPosition.DocumentEnd;
            synthText.CaretPosition = synthText.CaretPosition.GetPositionAtOffset(-14);
        }

        private void ttsSp3_Click(object sender, RoutedEventArgs e)
        {
            synthText.AppendText("<prosody rate=\"x-fast\"></prosody>\n");
            synthText.Focus();
            synthText.CaretPosition = synthText.CaretPosition.DocumentEnd;
            synthText.CaretPosition = synthText.CaretPosition.GetPositionAtOffset(-14);
        }

        private void ttsPar_Click(object sender, RoutedEventArgs e)
        {
            synthText.AppendText("<p></p>\n");
            synthText.Focus();
            synthText.CaretPosition = synthText.CaretPosition.DocumentEnd;
            synthText.CaretPosition = synthText.CaretPosition.GetPositionAtOffset(-8);
        }

        private void ttsEhm_Click(object sender, RoutedEventArgs e)
        {
            synthText.AppendText("<prosody rate=\"x-slow\">ehm</prosody>\n");
        }

        private void ttsSen_Click(object sender, RoutedEventArgs e)
        {
            synthText.AppendText("<s></s>\n");
            synthText.Focus();
            synthText.CaretPosition = synthText.CaretPosition.DocumentEnd;
            synthText.CaretPosition = synthText.CaretPosition.GetPositionAtOffset(-8);
        }

        private void ttsPi1_Click(object sender, RoutedEventArgs e)
        {
            synthText.AppendText("<prosody pitch=\"x-low\"></prosody>\n");
            synthText.Focus();
            synthText.CaretPosition = synthText.CaretPosition.DocumentEnd;
            synthText.CaretPosition = synthText.CaretPosition.GetPositionAtOffset(-14);
        }

        private void ttsPi2_Click(object sender, RoutedEventArgs e)
        {
            synthText.AppendText("<prosody pitch=\"x-high\"></prosody>\n");
            synthText.Focus();
            synthText.CaretPosition = synthText.CaretPosition.DocumentEnd;
            synthText.CaretPosition = synthText.CaretPosition.GetPositionAtOffset(-14);
        }

        private void recordingMode_Click(object sender, RoutedEventArgs e)
        {

            if (!_recordingMode)
            {
                if(isSiphona)
                {
                    action.Record();
                }
                else
                {
                    timelineClass.StartTimer();
                }
                this._recordingMode = true;
                recordingMode.Foreground = new SolidColorBrush(Colors.Red);
             
            }
            else
            {
                if (isSiphona)
                {

                    action.StopRecording();
                }
                else
                {
                    timelineClass.StopTimer();
                }
                this._recordingMode = false;
                recordingMode.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(200, 200, 200));
              
            }
        }

        private void startFac_Click(object sender, RoutedEventArgs e)
        {
            FaceRecognition.InternalEncoding = System.Text.Encoding.GetEncoding("shift_jis");

            string directory = @"C:\models";
            using (FaceRecognition fr = FaceRecognition.Create(directory))
            {
                Image imageA = FaceRecognition.LoadImage(BitmapImage2Bitmap(lastImage));
                IEnumerable<Location> locationsA = fr.FaceLocations(imageA);
                Location[] arr = locationsA.Cast<Location>().ToArray();
                try
                {
                    Rect rect = new Rect();
                    rect.X = locationsA.First().Left;
                    rect.Y = locationsA.First().Top;
                    rect.Width = locationsA.First().Right - locationsA.First().Left;
                    rect.Height = locationsA.First().Bottom - locationsA.First().Top;
                    System.Windows.Shapes.Rectangle recta = new System.Windows.Shapes.Rectangle();
                    recta.StrokeThickness = 2;
                    recta.Stroke = new SolidColorBrush(Colors.Red);
                    recta.Width = locationsA.First().Right - locationsA.First().Left;
                    recta.Height = locationsA.First().Bottom - locationsA.First().Top;
                    Canvas.SetLeft(recta, locationsA.First().Left);
                    Canvas.SetTop(recta, locationsA.First().Top);
                    facerec.Children.Add(recta);
                    //MessageBox.Show(locationsA.First().Right.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                for (int i = 0; i < arr.Length; i++)
                {
                    edebug.Text += arr[i].ToString() + "\n Left: ";
                    edebug.Text += locationsA.First().Left + "\n Right: ";
                    edebug.Text += locationsA.First().Right + "\n Top: ";
                    edebug.Text += locationsA.First().Top + "\n Bottom: ";
                    edebug.Text += locationsA.First().Bottom + "\n";

                }
            }

        }
        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        private void playRecording_Click(object sender, RoutedEventArgs e)
        {
            if(isSiphona)
            {
                sequence.PlaySequence();
            }
            else
            {
                timelineClass.Play();
            }
        }

        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
            {
                sequence.ResetSequence();
            }
            else
            {
                timelineClass.Reset();
            }
            
            viewTimeline = new veBot_Operator.Timeline(2500, 150);
            viewTimeline.Setup(0, 160, 1, 90);
            host1.Children.Clear();
            host1.Children.Add(viewTimeline);
            action.ResetTimeline(viewTimeline);
            sequence.ResetTimeline(viewTimeline);
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
            {
                if (_recordingMode)
                {

                        if (isSiphona)
                        {

                            action.StopRecording();
                        }
                        else
                        {
                            timelineClass.StopTimer();
                        }
                        this._recordingMode = false;
                        recordingMode.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(200, 200, 200));

                }
                else
                {
                    sequence.StopSequence();
                }
            }
            else
            {
                timelineClass.Stop();

            }
        }

        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
            {
                sequence.PlaySequence();
            }
            else
            {
                timelineClass.Play();
            }
        }

        private void hostHost_Initialized(object sender, EventArgs e)
        {
            viewTimeline = new veBot_Operator.Timeline(2500, 150);
            viewTimeline.Setup(0, 160, 1, 90);
            host1.Children.Add(viewTimeline);
        }

        private void s0set_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.MoveServo(0, Convert.ToInt32(s0.Text), asynchronity);
            else
                sc.Send("0:" + s0.Text);
        }

        private void s1set_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.MoveServo(1, Convert.ToInt32(s1.Text), asynchronity);
            else
                sc.Send("1:" + s1.Text);
        }

        private void s3set_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.MoveServo(3, Convert.ToInt32(s3.Text), asynchronity);
            else
                sc.Send("3:" + s3.Text);
        }

        private void s2set_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.MoveServo(2, Convert.ToInt32(s2.Text), asynchronity);
            else
                sc.Send("2:" + s2.Text);
        }

        private void s5set_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.MoveServo(5, Convert.ToInt32(s5.Text), asynchronity);
            else
                sc.Send("5:" + s5.Text);
        }

        private void s4set_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.MoveServo(4, Convert.ToInt32(s4.Text), asynchronity);
            else
                sc.Send("4:" + s4.Text);
        }

        private void s6set_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.MoveServo(6, Convert.ToInt32(s6.Text), asynchronity);
            else
                sc.Send("6:" + s6.Text);
        }

        private void clearFac_Click(object sender, RoutedEventArgs e)
        {
            facerec.Children.Clear();
        }

        private void SiphonaSetting_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
            {
                isSiphona = false;
            }
            else
            {
                isSiphona = true;
            }
            SiphonaSetting.IsChecked = isSiphona;
            sc.isSiphona = isSiphona;
        }

        private void EasingSetting_Click(object sender, RoutedEventArgs e)
        {
            if (isEasing)
            {
                isEasing = false;
                if (isSiphona)
                    sip2.SendAction(PredefinedActions.SET_EASING_OFF, 100, asynchronity);
            }
            else
            {
                isEasing = true;
                if (isSiphona)
                    sip2.SendAction(PredefinedActions.SET_EASING_ON, 100, asynchronity);
            }
            EasingSetting.IsChecked = isEasing;
            sc.isEasing = isEasing;


        }

        private void passiveMode_Click(object sender, RoutedEventArgs e)
        {
            if (_passiveMode)
            {
                _passiveMode = false;
                ps.StopPassive();
                passiveMode.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 41, 41, 41));
            }
            else
            {
                _passiveMode = true;
                passiveMode.BorderBrush = new SolidColorBrush(Colors.Red);
                ps.PlayPassive();
            }
        }

        private void s10set_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.MoveServo(10, Convert.ToInt32(s10.Text), asynchronity);
            else
                sc.Send("10:" + s10.Text);
        }

        private void s9set_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.MoveServo(9, Convert.ToInt32(s9.Text), asynchronity);
            else
                sc.Send("9:" + s9.Text);
        }

        private void s8set_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.MoveServo(8, Convert.ToInt32(s8.Text), asynchronity);
            else
                sc.Send("8:" + s8.Text);
        }

        private void s7set_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.MoveServo(7, Convert.ToInt32(s7.Text), asynchronity);
            else
                sc.Send("7:" + s7.Text);
        }

        private void s11set_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.MoveServo(11, Convert.ToInt32(s11.Text), asynchronity);
            else
                sc.Send("11:" + s11.Text);
        }

        private void smileButt_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.SendAction(PredefinedActions.FAKE_SMILE, 100, asynchronity);
            else
                sc.Send("smile");

            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsResting.png"));

        }

        private void disgustButt_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.SendAction(PredefinedActions.DISGUSTED, 100, asynchronity);
            else
                sc.Send("disgust");

            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsDisgusted.png"));
        }

        private void hiGeorgieButt_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.SendAction(PredefinedActions.FAKE_SMILE, 100, asynchronity);
            else
                sc.Send("hiGeorgie");
        }

        private void resetMouth_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.SendAction(PredefinedActions.RESET_MOUTH, 100, asynchronity);
            else
                sc.Send("resetMouth");
        }

        private void s12set_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.MoveServo(12, Convert.ToInt32(s12.Text), asynchronity);
            else
                sc.Send("12:" + s12.Text);
        }

        private void s13set_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.MoveServo(13, Convert.ToInt32(s13.Text), asynchronity);
            else
                sc.Send("13:" + s13.Text);
        }

        private void s14set_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.MoveServo(14, Convert.ToInt32(s14.Text), asynchronity);
            else
                sc.Send("14:" + s14.Text);
        }

        private void csSpanek_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.SendAction(PredefinedActions.SLEEP, 100, asynchronity);
            else
                sc.Send("csSpanek");

            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsClosed.png"));
        }

        private void csSmutek_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.SendAction(PredefinedActions.FROWN, 100, asynchronity);
            else
                sc.Send("csSmutek");

            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsResting.png"));
        }

        private void csZnechuceni_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.SendAction(PredefinedActions.DISGUSTED, 100, asynchronity);
            else
                sc.Send("csZnechuceni");

            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsDisgusted.png"));
        }

        private async void csSexy_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.SendAction(PredefinedActions.SEXY, 100, asynchronity);
            else
                sc.Send("csSexy");

            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsMonoBlink.png"));
            await Task.Delay(400);
            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsOpen.png"));
        }

        private void csStesti_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.SendAction(PredefinedActions.HAPPINESS, 100, asynchronity);
            else
                sc.Send("csStesti");

            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsResting.png"));
        }

        private void csPrekvapeni_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.SendAction(PredefinedActions.SURPRISE, 100, asynchronity);
            else
                sc.Send("csPrekvapeni");

            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsSurprised.png"));
        }

        private void csNasranost_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.SendAction(PredefinedActions.MADNESS, 100, asynchronity);
            else
                sc.Send("csNasranost");

            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsDisgusted.png"));
        }

        private void csUsmev_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.SendAction(PredefinedActions.SMILE, 100, asynchronity);
            else
                sc.Send("csUsmev");

            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsSurprised.png"));
        }

        private void csReset_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.SendAction(PredefinedActions.RESET_ALL, 100, asynchronity);
            else
                sc.Send("csReset");

            prevLids.Source = new BitmapImage(Get("Resources/" + eyever + "/LidsResting.png"));
        }

        private void s15set_Click(object sender, RoutedEventArgs e)
        {
            if (isSiphona)
                action.MoveServo(15, Convert.ToInt32(s15.Text), asynchronity);
            else
                sc.Send("15:" + s15.Text);
        }

        private void XLower_Click(object sender, RoutedEventArgs e)
        {

            XscaleVar -= 5;
            XscaleVar = checkvar(XscaleVar);
            Xscale.Value = XscaleVar;
            sip2.SendAction(PredefinedActions.EYES_X, XscaleVar, asynchronity);
        }

        private void YLower_Click(object sender, RoutedEventArgs e)
        {

            YscaleVar -= 5;
            YscaleVar = checkvar(YscaleVar);
            Yscale.Value = YscaleVar;
            sip2.SendAction(PredefinedActions.EYES_Y, YscaleVar, asynchronity);
        }

        private void XHigher_Click(object sender, RoutedEventArgs e)
        {

            XscaleVar += 5;
            XscaleVar = checkvar(XscaleVar);
            Xscale.Value = XscaleVar;
            sip2.SendAction(PredefinedActions.EYES_X, XscaleVar, asynchronity);
        }

        private void YHigher_Click(object sender, RoutedEventArgs e)
        {
            YscaleVar -= 5;
            YscaleVar = checkvar(YscaleVar);
            Yscale.Value = YscaleVar;
            sip2.SendAction(PredefinedActions.EYES_Y, YscaleVar, asynchronity);
        }

        public void setNeck(object sender, RoutedEventArgs e)
        {
            int deg = Convert.ToInt32(((System.Windows.Controls.Button)sender).Tag);
            action.MoveServo(12, deg, asynchronity);
        }

        public void setMouth(object sender, RoutedEventArgs e)
        {
            int deg = Convert.ToInt32((sender as Slider).Value);
            int servo = Convert.ToInt32((sender as Slider).Tag);
            if (action != null)
            {
                action.MoveServo(servo, deg, asynchronity);
            }
        }

        private int checkvar(int varb)
        {
            if(varb < 0)
            {
                varb = 0;
            }
            if (varb > 100)
            {
                varb = 100;
            }

            return varb;
        }

        private void startdFac_Click(object sender, RoutedEventArgs e)
        {
            fex.start_cam_stream();
        }

        private void exitApp_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void saveSeq_Click(object sender, RoutedEventArgs e)
        {
            sequence.SaveSequence();
        }

        private void openSeq_Click(object sender, RoutedEventArgs e)
        {
            sequence.OpenSequence();
        }

       

        private void neckSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (action != null)
            {
                action.MoveServo(12, Convert.ToInt32(neckSlider.Value), asynchronity);
            }
          
        }

        private void Xscale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(action != null)
            { 
                action.SendAction(PredefinedActions.EYES_X,Convert.ToInt32(Xscale.Value), asynchronity);
            }
           
        }

        private void Yscale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (action != null)
            {
                action.SendAction(PredefinedActions.EYES_Y, Convert.ToInt32(Yscale.Value), asynchronity);
            }
           
        }

        private void pauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if(_recordingMode)
            {
                if(_paused)
                {
                    _paused = false;
                    action.ResumeRecording();
                }
                else
                {
                    _paused = true;
                    action.PauseRecording();
                }
            }
            else
            {
                if (_paused)
                {
                    _paused = false;
                    sequence.ResumeSequence();
                }
                else
                {
                    _paused = true;
                    sequence.PauseSequence();
                }
                
            }
        }

        private async void quickconnect_Click(object sender, RoutedEventArgs e)
        {
            sshEst();
            await Task.Delay(500);
            sshStart();
            loadcam();
            await Task.Delay(500);
            socketconnect();
        }

        private void eyelidsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (action != null)
            {
                action.SendAction(PredefinedActions.EYES_LIDS, Convert.ToInt32(eyelidsSlider.Value), asynchronity);
            }
        }

        private void leftBrowSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (action != null)
            {
                action.SendAction(PredefinedActions.BROW_LEFT, Convert.ToInt32(leftBrowSlider.Value), asynchronity);
            }
        }

        private void bothBrowSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            leftBrowSlider.Value = bothBrowSlider.Value;
            rightBrowSlider.Value = bothBrowSlider.Value;
        }

        private void rightBrowSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (action != null)
            {
                action.SendAction(PredefinedActions.BROW_RIGHT, Convert.ToInt32(rightBrowSlider.Value), asynchronity);
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            About ab = new About();
            ab.Show();
        }
    }
}
