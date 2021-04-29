using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veBot_Operator.BotModes.TimelineSequencer
{
    [Serializable()]
    class Keyframe
    {
        public TimeSpan time;
        public Keyframe(TimeSpan time)
        {
            this.time = time;
        }

        public virtual void PlayKeyframe(SiphonaV2 siphona)
        {
            throw new Exception("This keyframe could not be played as it is a generic, non-playable keyframe.");
        }

        public virtual string GetIdentification()
        {
            return "GENERIC";
        }
        public virtual string GetIcon()
        {
            return "";
        }
    }
}
