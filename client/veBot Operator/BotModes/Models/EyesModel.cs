﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veBot_Operator.BotModes.Models
{
    class EyesModel
    {
        public TimeSpan time;
        public string action;

        public EyesModel(TimeSpan time, string action)
        {
            this.time = time;
            this.action = action;
        }
    }
}
