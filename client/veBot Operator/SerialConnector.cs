using Renci.SshNet.Messages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace veBot_Operator
{
    class SerialConnector
    {
        private const int port = 13000;
        private Socket client;
        private static ManualResetEvent connectDone =
          new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        public bool connected;

        public bool isSiphona;
        public bool isEasing;

        // The response from the remote device.  
        private static String response = String.Empty;
        public SerialConnector(bool connected)
        {
            this.connected = connected;
        }
        public void Open(String ipaddress)
        {
            IPAddress ipAddress = System.Net.IPAddress.Parse(ipaddress);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
            connectDone.WaitOne();
        }

        public void Send(string command)
        {
            if(connected)
            {
                if (isSiphona)
                {
                    Siphona sp = new Siphona();
                    if (isEasing)
                    {
                        //SendEasing(command);
                        SendBinary(client, sp.send(command,true));
                    }
                    else
                    {
                        SendBinary(client, sp.send(command));
                    }
                }
                else
                {
                    Send(client, command);
                }
                try
                {
                    
                    
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        public void SendDirect(int command)
        {
            if (connected)
            {
                if (isSiphona)
                {
                    Siphona sp = new Siphona();
                    SendBinary(client, sp.sendCommand(command,0));
                }
            }
        }

        public void SendEasing(string command)
        {
            Siphona sp = new Siphona();
            string[] ind = command.Split(':');
            int servoNum = Convert.ToInt32(ind[0]);
            int destinationDeg = Convert.ToInt32(ind[1]);
            SendBinary(client, sp.sendCommand(1,servoNum));
            sendDone.WaitOne();
            Thread.Sleep(100);
            byte[] buffer = new byte[256];
            client.ReceiveTimeout = 5000;
            client.Receive(buffer, 0, client.Available, SocketFlags.None);

            //Receive(client);
            //receiveDone.WaitOne(5000);
            int sourceDeg = 0;
            string mess = Encoding.ASCII.GetString(buffer).TrimEnd('\0');

            try
            {
                double dd = Convert.ToDouble(mess,CultureInfo.InvariantCulture);
                sourceDeg = Convert.ToInt32(Math.Ceiling(dd));
            }
            catch
            { }
            int samplingRate = 30;
            int easingFrom = 20;
            if(sourceDeg > destinationDeg)
            {
                for (int i = sourceDeg; i > destinationDeg; i--)
                {
                    if(i > destinationDeg - easingFrom)
                    {
                        SendBinary(client, sp.send(i,servoNum));
                        Thread.Sleep(50);
                    }
                    else
                    {
                        SendBinary(client, sp.send(i, servoNum));
                        Thread.Sleep(10);
                    }
                }
            }
            else 
            if(sourceDeg < destinationDeg)
            {
                for (int i = sourceDeg; i < destinationDeg; i++)
                {
                    if (i > destinationDeg - easingFrom)
                    {
                        SendBinary(client, sp.send(i, servoNum));
                        Thread.Sleep(50);
                    }
                    else
                    {
                        SendBinary(client, sp.send(i, servoNum));
                        Thread.Sleep(10);
                    }
                }
            }
            else
            if (destinationDeg == sourceDeg)
            {

            }
        }


        public void Close()
        {
            try
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch { }
            
        }

        private static void StartClient(string ipaaddress)
        {
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // The name of the    

                IPAddress ipAddress = System.Net.IPAddress.Parse(ipaaddress);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.  
                Send(client, "This is a test<EOF>");
                sendDone.WaitOne();

                // Receive the response from the remote device.  
                Receive(client);
                receiveDone.WaitOne();

                // Write the response to the console.  
                Console.WriteLine("Response received : {0}", response);

                // Release the socket.  
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
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

        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            try
            {
                client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
            }
            catch
            { }
        }

        private static void SendBinary(Socket client, byte[] data)
        {
            

            // Begin sending the data to the remote device.  
            try
            {
                client.BeginSend(data, 0, 2, 0,
                new AsyncCallback(SendCallback), client);
                
            }
            catch
            { }
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
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }
}
