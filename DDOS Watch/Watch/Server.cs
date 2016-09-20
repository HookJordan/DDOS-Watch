using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace DDOS_Watch.Watch
{
    class Server
    {
        //IP Address or domain of server 
        public string IP { get; set; }
        //PORT server is running of 
        public int PORT { get; set; }
        //PROTOCOL server is using 
        public ProtocolType Protocol { get; private set; }

        //information about server 
        private bool status = true;
        private bool lastStatus = true; 

        /// <summary>
        /// Constructor, auto assign protocol (only TCP is support at this time)
        /// </summary>
        public Server()
        {
            //assign protocol 
            this.Protocol = ProtocolType.Tcp;
        }

        /// <summary>
        /// check if the status has changed since last update 
        /// </summary>
        /// <returns></returns>
        public bool hasChanged()
        {
            //if it has, return true 
            if(lastStatus != status)
                return true;
            else
                //else return false 
                return false;
        }

        //returns if the server is only 
        public bool isOnline
        {
            get
            {
                return status; 
            }
        }

        /// <summary>
        /// Connects to server and checks if a connection can be made 
        /// </summary>
        public void Check()
        {
            //the last check is updated 
            lastStatus = status;
            try
            {
                //create new socket based on protocol 
                Socket sck = null;
                if (Protocol == ProtocolType.Tcp)
                {
                    //TCP Stream socket 
                    sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                else if(Protocol == ProtocolType.Udp)
                {
                    //UDP DGram socket 
                    sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                }
                

                //Connect to the server 
                sck.Connect(new IPEndPoint(IPAddress.Parse(resolveIP()), PORT));

                //if connected 
                status = sck.Connected;

                //close the connection 
                sck.Close();
            }
            catch
            {
                //if no connection could be made.. update current status 
                status = false;
            }
        }

        /// <summary>
        /// call this function to resolve the ip address 
        /// </summary>
        /// <returns>Resolved ip address</returns>
        private string resolveIP()
        {
            try
            {
                //If the IP property is an ip address.. return 
                return IPAddress.Parse(IP).ToString();
            }
            catch
            {
                //Else if the ip is a domain, lookup the ip address and return 
                return Dns.GetHostAddresses(IP)[0].ToString();
            }
        }
    }
}
