using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace NETLIB.UDP
{
    /// <summary>
    /// 
    /// </summary>
    public class UDPPublisher : Publisher
    {
        #region Variables

        Queue<IPEndPoint> hosts;

        UdpClient client;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public UDPPublisher()
            : base()
        {
            hosts = new Queue<IPEndPoint>();
            client = new UdpClient();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public UDPPublisher(string ip, int port)
            : base()
        {
            hosts = new Queue<IPEndPoint>();
            client = new UdpClient(ip, port);
        }

        #endregion

        #region Attributes

        /// <summary>
        /// 
        /// </summary>
        public Queue<IPEndPoint> Hosts
        {
            get { return hosts; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        public void Bind(int port)
        {
            client.Client.Bind(new IPEndPoint(IPAddress.Any, port));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="ip"></param>
        public override void SendPack(BasePack pack, IPEndPoint ip = null)
        {
            if (isEnabled)
            {
                try
                {
                    if (ip == null)
                    {
                        client.Send(pack.Buffer, pack.Buffer.Length);
                    }
                    else
                    {
                        client.Send(pack.Buffer, pack.Buffer.Length, ip);
                    }
                }
                catch (IOException e)
                {
                    OnConnectionClosedCall();
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="ip"></param>
        public override void SendPack(byte[] pack, IPEndPoint ip = null)
        {
            if (isEnabled)
            {
                try
                {
                    if (ip == null)
                    {
                        client.Send(pack, pack.Length);
                    }
                    else
                    {
                        client.Send(pack, pack.Length, ip);
                    }
                }
                catch (IOException e)
                {
                    OnConnectionClosedCall();
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Publish()
        {
            byte[] buffer;
            IPEndPoint receivedIP = null;

            try
            {
                while (isEnabled)
                {
                    buffer = client.Receive(ref receivedIP);
                    hosts.Enqueue(receivedIP);
                    packQueue.Enqueue(buffer);
                    incomingPackEvent.Set();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("Connection closed! Host do not answer.");
                CloseConnection();
            }

        }

        #endregion
    }
}
