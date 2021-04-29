using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace veBot_Operator.BotParts
{
    class Mouth
    {
        private string[] openMouthPhoneme;
        private string[] closeMouthPhoneme;
        private string[] slightlyOpenMouthPhoneme;
        private SerialConnector conn;
        private SiphonaV2 siphona;
        private bool useSiphona;
        private bool asyncrocity;
        public Mouth(SerialConnector sc, SiphonaV2 siphonaV2, bool useSiphona, bool asyncrocity)
        {
            this.conn = sc;
            siphona = siphonaV2;
            this.useSiphona = useSiphona;
            this.asyncrocity = asyncrocity;

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
                if (useSiphona)
                    siphona.Speak(dict[phoneme], asyncrocity);
                else
                    conn.Send(dict[phoneme].ToString());
            }
            catch
            {

            }
            

            if(openMouthPhoneme.Contains(phoneme))
            {
                OpenMouth();
                return "Opened";
            }
            else if(slightlyOpenMouthPhoneme.Contains(phoneme))
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
            if (useSiphona)
                siphona.SendAction(PredefinedActions.MOUTH, 100, asyncrocity);
            else
                conn.Send("openmouth");
        }

        public void SlightlyOpenMouth()
        {
            if (useSiphona)
                siphona.SendAction(PredefinedActions.MOUTH, 50, asyncrocity);
            else
                conn.Send("slightlymouth");
        }

        public void CloseMouth()
        {
            if (useSiphona)
                siphona.SendAction(PredefinedActions.MOUTH, 0, asyncrocity);
            else
                conn.Send("closemouth");
        }
    }
}
