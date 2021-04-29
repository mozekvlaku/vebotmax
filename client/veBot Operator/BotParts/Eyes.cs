using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace veBot_Operator.BotParts
{
    class Eyes
    {
        private SerialConnector conn;
        public int xAxis = 90;
        public int yAxis = 90;
        public String lidState;
        public String status;

        public bool isSiphona;

        public Eyes(SerialConnector sc)
        {
            this.conn = sc;
            isSiphona = sc.isSiphona;
        }

        public void OpenConnection(String comport)
        {
            this.conn.Open(comport);
        }
        public void CloseConnection()
        {
            this.conn.Close();
        }
        public void MoveToLeft()
        {
            status = "Moving to left";
            if (conn.isSiphona)
            {
                conn.Send("5:140");
            }
            else
                conn.Send("lookleft");
        }

        public void MoveCenter()
        {
            status = "Moving to cehter";
            if (conn.isSiphona)
            {
                conn.Send("5:90");
            }
            else
                conn.Send("lookcenter");
        }

        public void MoveToRight()
        {
            status = "Moving to right";
            if (conn.isSiphona)
            {
                conn.Send("5:40");
            }
            else
                conn.Send("lookright");
        }

        public void Dement()
        {
            status = "dement";
            if (conn.isSiphona)
            {
                conn.Send("2:110");
                conn.Send("0:75");
                conn.Send("1:105");
                conn.Send("3:50");
            }
            else
                conn.Send("dementlook");
        }

        public void Envy()
        {
            status = "envy";
            if (conn.isSiphona)
            {
                conn.Send("2:110");
                conn.Send("1:60");
                conn.Send("3:50");
                conn.Send("0:110");
            }
            else
                conn.Send("envylook");
        }

        public void MoveUp()
        {
            status = "Moving up";
            if (conn.isSiphona)
            {
                conn.Send("4:110");
            }
            else
                conn.Send("lookup");
        }

        public void MoveDown()
        {
            status = "Moving down";
            if (conn.isSiphona)
            {
                conn.Send("4:170");
            }
            else
                conn.Send("lookdown");
        }

        public void MoveRightUp()
        {
            status = "Moving right up";
            if (conn.isSiphona)
            {
                conn.Send("4:110");
                conn.Send("5:40");
            }
            else
                conn.Send("lookupright");
        }

        public void MoveRightDown()
        {
            status = "Moving right down";
            if (conn.isSiphona)
            {
                conn.Send("4:170");
                conn.Send("5:40");
            }
            else
                conn.Send("lookdownright");
        }
        public void MoveLeftUp()
        {
            status = "Moving left up";
            if (conn.isSiphona)
            {
                conn.Send("4:110");
                conn.Send("5:140");
            }
            else
                conn.Send("lookupleft");
        }
        public void MoveLeftDown()
        {
            status = "Moving left down";
            if (conn.isSiphona)
            {
                conn.Send("4:170");
                conn.Send("5:140");
            }
            else
                conn.Send("lookdownleft");
        }

        public void Center()
        {
            status = "Centering";
            MoveX(90);
            MoveY(90);
        }


        public void MoveX(int degrees)
        {
            if (degrees > 180)
                degrees = 180;
            if (degrees < 0)
                degrees = 0;

            xAxis = degrees;
            status = "Moving X to " + degrees.ToString();
            conn.Send("5:"+degrees.ToString());
        }

        public void MoveY(int degrees)
        {
            if (degrees > 180)
                degrees = 180;
            if (degrees < 0)
                degrees = 0;

            yAxis = degrees;
            status = "Moving Y to " + degrees.ToString();
            conn.Send("4:" + degrees.ToString());
        }

        public void Blink()
        {
            lidState = "blinking";
            status = lidState;
            if (conn.isSiphona)
            {
                conn.SendDirect(2);
                Thread.Sleep(200);
                conn.SendDirect(3);
            }
            else
                conn.Send("blink");
        }

        public void VykulOci()
        {
            lidState = "kuleni";
            status = lidState;
            if (conn.isSiphona)
            {
                conn.Send(":");
            }
            else
                conn.Send("vykuloci");
        }

        public void DodgeFace()
        {
            lidState = "dodge";
            status = lidState;
            if (conn.isSiphona)
            {
                conn.Send(":");
            }
            else
                conn.Send("dodgeface");
        }

        public void SonnyBlink()
        {
            lidState = "sonny";
            status = lidState;
            if (conn.isSiphona)
            {
                conn.Send(":");
            }
            else
                conn.Send("sonnyblink");
        }

        public void Reset()
        {
            lidState = "reseting";
            status = lidState;
            if (conn.isSiphona)
            {
                conn.Send(":");
            }
            else
                conn.Send("reset");
        }

        public void CloseLids()
        {
            lidState = "closed";
            status = lidState;
            if (conn.isSiphona)
            {
                conn.SendDirect(2);
            }
            else
                conn.Send("closelids");
        }

        public void OpenLids()
        {
            lidState = "open";
            status = lidState;
            if (conn.isSiphona)
            {
                conn.SendDirect(3);
            }
            else
                conn.Send("openlids");
        }

        public void OpenMouth()
        {
            lidState = "openmouth";
            status = lidState;
            if (conn.isSiphona)
            {
                conn.Send("6:20");
            }
            else
                conn.Send("openmouth");
        }

        public void CloseMouth()
        {
            lidState = "closemouth";
            status = lidState;
            if (conn.isSiphona)
            {
                conn.Send("6:90");
            }
            else
                conn.Send("closemouth");
        }
    }
}
