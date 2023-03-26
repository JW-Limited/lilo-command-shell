using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LILO.Shell
{
    internal interface CommunicationInterface
    {

        /* Connects to the server 
        // Create a CommunicationInterface object
            CommunicationInterface ci = new CommunicationInterface(); 

        // Connect to two application using their hostname/IP addresses and ports
            ci.Connect("host1", port1, "host2", port2);

        // Send data from our application to the other applications
            byte[] data1 = Encoding.ASCII.GetBytes("Hello Application 1!"); 
            byte[] data2 = Encoding.ASCII.GetBytes("Hello Application 2!"); 
            ci.SendData(data1, data2); 

        // Receive data from the other applications
            ci.ReceiveDataFromAnotherApplication(); 

        // Finally, disconnect from the applications
            ci.Disconnect();
        End*/


        Socket Socket1 { get; set; }
        Socket Socket2 { get; set; }

        public void Connect(string host1, int port1, string host2, int port2)  
        { 
            // Initialize the socket objects
            Socket1 = new Socket(SocketType.Stream, ProtocolType.Tcp);
            Socket2 = new Socket(SocketType.Stream, ProtocolType.Tcp); 
            
            // Connect to each host using the given ports
            Socket1.Connect(host1, port1); 
            Socket2.Connect(host2, port2);
            Socket1.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            Socket2.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        }

        public void SendData(byte[] data1, byte[] data2) 
        { 
            // Send the data over each socket
            Socket1.Send(data1); 
            Socket2.Send(data2); 
        }

        public void ReceiveData() 
        { 
            // Receive data from each socket
            byte[] data1 = new byte[1024]; 
            Socket1.Receive(data1); 
            
            byte[] data2 = new byte[1024]; 
            Socket2.Receive(data2); 
        }

        public void Disconnect() 
        { 
            // Close each socket connection 
            try 
            { 
                if (Socket1 != null)
                    Socket1.Close(); 
            } finally 
            { 
                Socket2.Close(); 
            } 
        }

        public bool IsConnected(Socket s)
        {
            return s.Connected;
        }

        public void SendToDefaultBuffer(byte[] data1, byte[] data2) 
        { 
            // Send the data over each socket
            Socket1.SendBufferSize = 10024; 
            Socket1.Send(data1); 
            Socket2.SendBufferSize = 10024; 
            Socket2.Send(data2); 
        }

        public void ReceiveFromDefaultBuffer() 
        { 
            // Receive data from each socket
            byte[] data1 = new byte[1024]; 
            Socket1.ReceiveBufferSize = 1024; 
            Socket1.Receive(data1); 
            
            byte[] data2 = new byte[1024]; 
            Socket2.ReceiveBufferSize = 1024; 
            Socket2.Receive(data2); 
        }

        public void ReceiveDataFromAnotherApplication() 
        { 
            // Receive data from each socket
            byte[] data1 = new byte[1024]; 
            Socket1.Receive(data1); 
            
            byte[] data2 = new byte[1024]; 
            Socket2.Receive(data2); 
        }

        public void SendDataFromAnotherApplication(byte[] data1, byte[] data2)
        {
            Socket1.Send(data1);
            Socket2.Send(data2);
        }
    }
}