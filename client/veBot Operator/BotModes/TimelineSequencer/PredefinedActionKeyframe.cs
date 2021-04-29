using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veBot_Operator.BotModes.TimelineSequencer
{
    [Serializable()]
    class PredefinedActionKeyframe : Keyframe
    {
        public PredefinedActions predefined;
        public int strength;
        public bool asynchronity;

        public PredefinedActionKeyframe(TimeSpan time, PredefinedActions predefined, int strength, bool asynchronity) : base(time)
        {
            base.time = time;
            this.predefined = predefined;
            this.strength = strength;
            this.asynchronity = asynchronity;
        }

        public override void PlayKeyframe(SiphonaV2 siphona)
        {
            siphona.SendAction(predefined, strength, asynchronity);
        }
        public override string GetIdentification()
        {
            return Enum.GetName(typeof(PredefinedActions), predefined);
        }
        public override string GetIcon()
        {
            return "";
        }
    }
}
