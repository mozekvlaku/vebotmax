using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace veBot_Operator.BotModes
{
    class TextToSpeech
    {
        private string[] openMouthPhoneme;
        private string[] closeMouthPhoneme;
        private string[] slightlyOpenMouthPhoneme;
        private SiphonaV2 siphona;
        private bool asyncrocity;
        public TextToSpeech(SiphonaV2 siphona, bool asyncronity)
        {
            this.siphona = siphona;
            this.asyncrocity = asyncronity;

            openMouthPhoneme = new string[]
            {
                "a","ɛ","o","aː","ɛː","oː"
            };
            closeMouthPhoneme = new string[]
            {
                "n","t","d","t͡s","d͡z","s","z","r","l","r̝","t͡ʃ","d͡ʒ","ʃ","ʒ","ɲ","c","ɟ","j","ɪ","iː"
            };
            slightlyOpenMouthPhoneme = new string[]
            {
                "m","p","b","f","v","k","g","x","ɦ","u","uː","ou̯"
            };
        }

        public void Speak(string text, string lang)
        {
            var synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();
            synthesizer.SelectVoice("Microsoft Jakub"); //Speechtech Jan
            var builder = new PromptBuilder();
            builder.StartVoice(new CultureInfo(lang));
            builder.AppendText(text);
            builder.EndVoice();
            string ssmlString = "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"" + lang + "\">\n<voice name=\"en-US-AriaRUS\">\n";
            ssmlString += text;
            ssmlString += "\n</voice>\n</speak>";
            synthesizer.SpeakSsmlAsync(ssmlString);

            synthesizer.PhonemeReached += Synthesizer_PhonemeReached;
            synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
        }
        private void Synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
             siphona.SendAction(PredefinedActions.RESET_MOUTH, 100, asyncrocity);
        }

        private void Synthesizer_PhonemeReached(object sender, PhonemeReachedEventArgs e)
        {
            PronouncePhoneme(e.Phoneme);
        }

        public String PronouncePhoneme(string phoneme)
        {
            var dict = new Dictionary<string, int>();
            dict["a"] = 0;
            dict["aː"] = 0;
            dict["ɛ"] = 4;
            dict["ɛː"] = 4;
            dict["jɛ"] = 4;
            dict["ɪ"] = 9;
            dict["iː"] = 9;
            dict["o"] = 15;
            dict["oː"] = 15;
            dict["u"] = 22;
            dict["uː"] = 22;
            dict["uː"] = 22;
            dict["ɪ"] = 9;
            dict["iː"] = 9;
            dict["b"] = 1;
            dict["t͡s"] = 2;
            dict["t͡ʃ"] = 2;
            dict["d"] = 3;
            dict["ɟ"] = 3;
            dict["f"] = 5;
            dict["ɡ"] = 6;
            dict["ɦ"] = 7;
            dict["x"] = 8;
            dict["j"] = 10;
            dict["k"] = 11;
            dict["l"] = 12;
            dict["m"] = 13;
            dict["n"] = 14;
            dict["ɲ"] = 14;
            dict["p"] = 16;
            dict["r"] = 17;
            dict["r̝"] = 17;
            dict["s"] = 19;
            dict["ʃ"] = 20;
            dict["t"] = 21;
            dict["c"] = 21;
            dict["v"] = 23;
            dict["ks"] = 25;
            dict["gz"] = 25;
            dict["z"] = 27;
            dict["ʒ"] = 20;
            try
            {
                    siphona.Speak(dict[phoneme], asyncrocity);
            }
            catch
            {

            }


            if (openMouthPhoneme.Contains(phoneme))
            {
                OpenMouth();
                return "Opened";
            }
            else if (slightlyOpenMouthPhoneme.Contains(phoneme))
            {
                SlightlyOpenMouth();
                return "SlightlyOpened";
            }
            else
            {
                CloseMouth();
                return "Closed";
            }
        }

        public void OpenMouth()
        {
                siphona.SendAction(PredefinedActions.MOUTH, 100, asyncrocity);
        }

        public void SlightlyOpenMouth()
        {
                siphona.SendAction(PredefinedActions.MOUTH, 50, asyncrocity);
        }

        public void CloseMouth()
        {
                siphona.SendAction(PredefinedActions.MOUTH, 0, asyncrocity);
        }
    }
}
