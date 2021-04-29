using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veBot_Operator.BotModes.Models
{
    class TTSModel
    {
        public TimeSpan time;
        public string ssmlString;
        public string synthesisLanguage;

        public TTSModel(TimeSpan time, string ssmlString, string synthesisLanguage)
        {
            this.time = time;
            this.ssmlString = ssmlString;
            this.synthesisLanguage = synthesisLanguage;
        }
    }
}
