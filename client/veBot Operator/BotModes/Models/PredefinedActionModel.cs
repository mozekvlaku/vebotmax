using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veBot_Operator.BotModes.Models
{
    class PredefinedActionModel
    {
        public TimeSpan time;
        public PredefinedActions predefined;
        public int strength;
        public bool asynchronity;

   
        public PredefinedActionModel(TimeSpan time, PredefinedActions predefined, int strength, bool asynchronity)
        {
            this.time = time;
            this.predefined = predefined;
            this.strength = strength;
            this.asynchronity = asynchronity;
        }
    }
}
