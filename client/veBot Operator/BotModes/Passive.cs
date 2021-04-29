using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using veBot_Operator.BotParts;
using System.Data;
using DlibDotNet;

namespace veBot_Operator.BotModes
{
    class Passive
    {
        private Timer timer;
        private SiphonaV2 siphona;
        Random rand4;
        Random rand5;
        Random blink;
        Random yawn;
        Random action;

        public Passive(SiphonaV2 siphona)
        {
            timer = new Timer(500);
            timer.Elapsed += Timer_Elapsed;
            this.siphona = siphona;
            rand4 = new Random();
            rand5 = new Random();
            blink = new Random();
            yawn = new Random();
            action = new Random();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {



            int act = action.Next(0, 20);
            switch (act)
            {
                case 0: //lookupdown
                    int deg0 = rand4.Next(0, 100);

                    siphona.SendAction(PredefinedActions.EYES_Y, deg0, true);
                    break;
                case 1: //lookleftright
                    int deg1 = rand5.Next(0, 100);
                    siphona.SendAction(PredefinedActions.EYES_X, deg1, true);

                    break;
                case 2:
                    siphona.SendAction(PredefinedActions.BLINK, 0, true);
                    break;
                case 10: //lookleftright
                    int deg3 = rand5.Next(0, 100);
                    siphona.SendAction(PredefinedActions.EYES_X, deg3, true);

                    break;
                case 20:
                    siphona.SendAction(PredefinedActions.BLINK, 0, true);
                    break;
                case 15: //lookleftright
                    int deg2 = rand5.Next(0, 100);
                    siphona.SendAction(PredefinedActions.EYES_X, deg2, true);

                    break;
                case 16:
                    siphona.SendAction(PredefinedActions.BLINK, 0, true);
                    break;
                case 3:
                    siphona.SendAction(PredefinedActions.FAKE_SMILE, 0, true);
                    break;
                case 4:
                    siphona.SendAction(PredefinedActions.SMILE, 0, true);
                    break;
                case 5:
                    siphona.SendAction(PredefinedActions.RESET_MOUTH, 0, true);
                    break;
                case 6:
                    siphona.SendAction(PredefinedActions.RESET_MOUTH, 0, true);
                    break;
                case 7:
                    siphona.SendAction(PredefinedActions.RESET_MOUTH, 0, true);
                    break;
                case 8:
                    siphona.SendAction(PredefinedActions.RESET_ALL, 0, true);
                    break;
                case 9:
                    siphona.SendAction(PredefinedActions.BROWS, 100, true);
                    break;
                case 11:
                    siphona.SendAction(PredefinedActions.BROWS, 0, true);
                    break;
                case 12:
                    int deg6 = rand5.Next(70, 130);
                    siphona.MoveServo(12, deg6, true);
                    break;
                   case 13:
                    TextToSpeech tts = new TextToSpeech(siphona, true);
                    tts.Speak(RandomWord(), "cs-CZ");
                    break;
                default:
                    break;
            }

        }

        public int RoundOff(int number, int interval)
        {
            int remainder = number % interval;
            number += (remainder < interval / 2) ? -remainder : (interval - remainder);
            return number;
        }

        public void PlayPassive()
        {
            timer.Enabled = true;
            timer.Start();
        }

        public void StopPassive()
        {
            timer.Stop();
            timer.Enabled = false;
        }

        public string RandomWord()
        {
            string[] slovověty = new string[]
            {
                "dnes je hezky",
"hm",
"akurátní ",
"mám hlad",
"týjo, vám to sluší",
"hehe",
"achjo",
"jsem robot",
"zavděč",
"sáhněme",
"basy",
"vyhledanými",
"hnědější",
"flešku",
"kanoucí",
"oplocený",
"vzdělávací",
"zpřesňuje",
"administrátora",
"avala",
"arkádovýma",
"atmosférickýma",
"astrolékařská",
"anarchickýma",
"anýzová",
"aranžérskýma",
"aretovaná",
"atakovala",
"titogradské",
"postgraduálně",
"zredigovaný",
"upgradovatelnému",
"kvadruplegik",
"radiotelegrafistka",
"magnetohydrodynamický",
"dvacetigramová",
"nedegenerovány",
"upgradovatelnou",
"jednovidé",
"jezde",
"jede",
"jednořadé",
"jehnědě",
"jezde",
"jede",
"jehnědě",
"jednořadé",
"jednovidé",
"jezdíval",
"jednal",
"tolerantnějších",
"rozpohybujete",
"pohrajte",
"produktivnějších",
"postrojených",
"roztahujeme",
"nejextrémnějšího",
"skřehotavější",
"trojského",
"trojspolkového",
"kastelánům",
"krystalickému",
"kardiostimulátorech",
"kapitolskému",
"koukatelnější",
"klaustrofobie",
"klášťovskému",
"kompostovatelnému",
"křišťálovému",
"kompostovatelnou",
"elektrotechnologie",
"konvergujícího",
"agrotechnická",
"integrovatelných",
"evangelizačních",
"germanistech",
"washingtonskýma",
"neorganizovaného",
"zgermanizovaných",
"agentových",
"přisluhovačském",
"ornamentalistickému",
"lichvářského",
"astroarcheologickému",
"restrukturalizačními",
"protimilitaristického",
"astrologické",
"neuropsycholožkami",
"přeškolovacího",
"malospotřebitelských",
"masturbačního",
"pětičárkovanou",
"tyčkařkou",
"rozsochatému",
"doburácet",
"čtvrťačkou",
"traumatickou",
"rasistickou",
"prozaistickou",
"romantizujícím",
"směl",
"nesplácel",
"úročitel",
"propouštěl",
"hlučel",
"popřemýšlel",
"odhlížel",
"nepouživatel"
            };
            Random věty = new Random();
            return slovověty[věty.Next(0, slovověty.Length)];
        }
    }
}
