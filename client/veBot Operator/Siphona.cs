using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veBot_Operator
{
    class Siphona
    {
        public int CMD_READ = 1;

        public Siphona()
        {

        }

        public byte[] send(string data)
        {
            string[] ind = data.Split(':');
            byte[] siphonaSeed = new byte[2];
            string input = moveServo(Convert.ToInt32(ind[1]), Convert.ToInt32(ind[0]),false);
            siphonaSeed[0] = Convert.ToByte(input.Substring(0, 8), 2);
            siphonaSeed[1] = Convert.ToByte(input.Substring(8), 2);
            return siphonaSeed;
        }
        public byte[] send(string data,bool easing)
        {
            string[] ind = data.Split(':');
            byte[] siphonaSeed = new byte[2];
            string input = moveServo(Convert.ToInt32(ind[1]), Convert.ToInt32(ind[0]),easing);
            siphonaSeed[0] = Convert.ToByte(input.Substring(0, 8), 2);
            siphonaSeed[1] = Convert.ToByte(input.Substring(8), 2);
            return siphonaSeed;
        }
        public byte[] send(int servo_num, int servo_deg)
        {
            byte[] siphonaSeed = new byte[2];
            string input = moveServo(servo_num, servo_deg, false);
            siphonaSeed[0] = Convert.ToByte(input.Substring(0, 8), 2);
            siphonaSeed[1] = Convert.ToByte(input.Substring(8), 2);
            return siphonaSeed;
        }
        public string moveServo(int servo_num, int servo_deg,bool ease)
        {
            string number = Convert.ToString(servo_num, 2).PadLeft(8, '0');
            string degree = Convert.ToString(servo_deg, 2).PadLeft(8, '0');
            if(number.Length==4)
            {
                number = "0000" + number;
            }
            if (degree.Length == 4)
            {
                degree = "0000" + degree;
            }
            degree = degree.Substring(3);

            return addHeader(number + degree,ease);
        }

        public byte[] sendCommand(int command, int degreeOpt)
        {
            byte[] siphonaSeed = new byte[2];
            string degree = Convert.ToString(degreeOpt, 2).PadLeft(8, '0');
            degree = degree.Substring(3);
            string input = "";
            switch (command)
            {
                case 1:
                    input = addHeaderCommand("10000000" + degree);
                    break;
                case 2: //close
                    input = addHeaderCommand("10000001" + degree);
                    break;
                case 3: //open
                    input = addHeaderCommand("10000010" + degree);
                    break;
                default:
               
                    break;
            }
            siphonaSeed[0] = Convert.ToByte(input.Substring(0, 8), 2);
            siphonaSeed[1] = Convert.ToByte(input.Substring(8), 2);
            return siphonaSeed;
        }

        public string addHeader(string trailer,bool ease)
        {
            string header;
            if (ease)
                header = "101";
            else
                header = "100";
            return header + trailer;
        }

        public string addHeaderCommand(string trailer)
        {
            string header = "110";
            return header + trailer;
        }

    }
}
