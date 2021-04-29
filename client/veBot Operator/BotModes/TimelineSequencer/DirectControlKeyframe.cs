using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veBot_Operator.BotModes.TimelineSequencer
{
    [Serializable()]
    class DirectControlKeyframe : Keyframe
    {
        public int servoNum;
        public int servoDegree;
        public bool asnc;
        public DirectControlKeyframe(TimeSpan time, int servoNum, int servoDegree, bool asnc) : base(time)
        {
            base.time = time;
            this.servoDegree = servoDegree;
            this.servoNum = servoNum;
            this.asnc = asnc;
        }

        public override void PlayKeyframe(SiphonaV2 siphona)
        {
            siphona.MoveServo(servoNum, servoDegree, asnc);
        }
        public override string GetIdentification()
        {
            return servoNum.ToString()+":"+servoDegree.ToString();
        }
        public override string GetIcon()
        {
            return "";
        }
    }
}
