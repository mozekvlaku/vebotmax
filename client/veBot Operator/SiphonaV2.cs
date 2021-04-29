using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace veBot_Operator
{
    public enum PredefinedActions : byte
    {
        SMILE               = 0b_0000_0001,
        FROWN               = 0b_0000_0010,
        SURPRISE            = 0b_0000_0011,
        BLINK               = 0b_0000_0100,
        CLOSE_EYES          = 0b_0000_0101,
        OPEN_EYES           = 0b_0000_0110,
        SET_EASING_ON       = 0b_0000_0111,
        SET_EASING_OFF      = 0b_0000_1000,
        EYES_X              = 0b_0000_1001,
        EYES_Y              = 0b_0000_1010,
        SINGLE_BLINK_RIGHT  = 0b_0000_1011,
        SINGLE_BLINK_LEFT   = 0b_0000_1100,
        EYES_LIDS           = 0b_0000_1101,
        FAKE_SMILE          = 0b_0000_1110,
        WAKE_UP             = 0b_0000_1111,
        SLEEP               = 0b_0001_0000,
        RESET_MOUTH         = 0b_0001_0001,
        RESET_EYES          = 0b_0001_0010,
        RESET_ALL           = 0b_0001_0011,
        HAPPINESS           = 0b_0001_0100,
        SEXY                = 0b_0001_0101,
        DISGUSTED           = 0b_0001_0110,
        MADNESS             = 0b_0001_0111,
        MOUTH               = 0b_0001_1000,
        ENVY                = 0b_0001_1001,
        EYES_DIAG_LEFT      = 0b_0001_1010,
        EYES_DIAG_RIGHT     = 0b_0001_1011,
        BROWS               = 0b_0001_1100,
        BROW_LEFT           = 0b_0001_1101,
        BROW_RIGHT          = 0b_0001_1110,

    }
    class SiphonaV2
    {
        //constants
        const byte DIRECT_CTRL = 0b_01;
        const byte PREDEF_ACTS = 0b_00;
        const byte SPEECH_SYNT = 0b_10;
        const byte COMMUN_SETT = 0b_11;
        //protocol
        private bool asynchronous_comm;
        private int  maxServos;
        //socket
        private const int port = 13000;
        private Socket client;
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        public SiphonaV2(int maxServos)
        {
            this.maxServos = maxServos;
        }

        public void SendAction(PredefinedActions action, int strength, bool asnc)
        {
            if (strength < 0 || strength > 100)
            {
                throw new Exception("The strength (" + strength + ") of this action is between 0 and 100!");
            }

            int strengthInt = (int)Math.Round(0.15 * strength);
            byte strengthByte = Convert.ToByte(strengthInt);
            byte headerByte;

            if (asnc)
                headerByte = 0b_0100_0000;
            else
                headerByte = 0b_0000_0000;

            byte upperByte = (byte)(headerByte | strengthByte);
            byte lowerByte = (byte)action;

            ushort upperWord = (ushort)(upperByte << 8);
            ushort lowerWord = lowerByte;

            ushort completedPacket = (ushort)(upperWord | lowerWord);

            bool evenParity = this.CheckParity(completedPacket);
            ushort parityWord;

            if (evenParity)
                parityWord = 0b_1000_0000_0000_0000;
            else
                parityWord = 0b_0000_0000_0000_0000;

            completedPacket = (ushort)(completedPacket | parityWord);

            SendToPacket(completedPacket);
        }
        public void Speak(int tts, bool asnc)
        {
            byte headerByte;
            byte lowerByte = Convert.ToByte(tts);
            if (asnc)
                headerByte = 0b_0110_0000;
            else
                headerByte = 0b_0010_0000;

            byte upperByte = (byte)(headerByte);

            ushort upperWord = (ushort)(upperByte << 8);
            ushort lowerWord = lowerByte;

            ushort completedPacket = (ushort)(upperWord | lowerWord);

            bool evenParity = this.CheckParity(completedPacket);
            ushort parityWord;

            if (evenParity)
                parityWord = 0b_1000_0000_0000_0000;
            else
                parityWord = 0b_0000_0000_0000_0000;

            completedPacket = (ushort)(completedPacket | parityWord);

            SendToPacket(completedPacket);
        }
        public void MoveServo(int servoNum, int servoDegree, bool asnc)
        {
            asynchronous_comm = asnc;
            //Check if servo number exists
            if (servoNum > maxServos)
            {
                throw new Exception("This servo (" + servoNum + ") is not connected to the APUROID robotic system. The maximum servo number is " + maxServos);
            }
            //Check if the servo degree is between 0 and 180
            if (servoDegree > 180 || servoDegree < 0)
            {
                throw new Exception("This is an invalid degree. Please try a degree between 0 and 180.");
            }
            //Convert integers to binary data
            byte servoNumberByte = Convert.ToByte(servoNum);
            byte servoDegreeByte = Convert.ToByte(servoDegree);
            byte headerByte;

            if (asnc)
                headerByte = 0b_0101_0000;
            else
                headerByte = 0b_0001_0000;

            byte upperByte = (byte)(headerByte | servoNumberByte);
            byte lowerByte = servoDegreeByte;

            ushort upperWord = (ushort)(upperByte << 8);
            ushort lowerWord = lowerByte;

            ushort completedPacket = (ushort)(upperWord | lowerWord);

            bool evenParity = this.CheckParity(completedPacket);
            ushort parityWord;

            if (evenParity)
                parityWord = 0b_1000_0000_0000_0000;
            else
                parityWord = 0b_0000_0000_0000_0000;

            completedPacket = (ushort)(completedPacket | parityWord);

            SendToPacket(completedPacket);
        }
        private bool CheckParity(int num)
        {
            return (num % 2 == 0);
        }

        private void SendToPacket(ushort data)
        {
            try
            {
                client.BeginSend(BitConverter.GetBytes(data), 0, 2, 0, new AsyncCallback(SendCallback), client);
            }
            catch
            { }
        }
        private void SendToPacket(string data)
        {

        }

        public void Open(string ipaddress, ProtocolType protocol)
        {
            IPAddress ipAddress = System.Net.IPAddress.Parse(ipaddress);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            client = new Socket(ipAddress.AddressFamily, SocketType.Stream, protocol);
            client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
            connectDone.WaitOne();
        }
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void Close()
        {
            try
            {
                if(client != null)
                {
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
              
            }
            catch { }

        }
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
