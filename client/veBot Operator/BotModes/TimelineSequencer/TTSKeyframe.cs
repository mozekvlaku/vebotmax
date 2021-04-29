using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veBot_Operator.BotModes.TimelineSequencer
{
    [Serializable()]
    class TTSKeyframe : Keyframe
    {
        public string ssmlString;
        public TTSKeyframe(TimeSpan time, string ssmlString) : base(time)
        {
            base.time = time;
            this.ssmlString = ssmlString;
        }

        public override void PlayKeyframe(SiphonaV2 siphona)
        {
            TextToSpeech tts = new TextToSpeech(siphona, true);
            tts.Speak(ssmlString, "cs-CZ");
        }
        public override string GetIdentification()
        {
            return "TTS";
        }
        public override string GetIcon()
        {
            return "";
        }
    }
}
