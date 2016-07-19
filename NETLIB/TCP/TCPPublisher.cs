using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace NETLIB.TCP
{
    /// <summary>
    /// <see cref="Publisher"/> that uses the TCP protocol for sending and receiving packs
    /// </summary>
    /// <seealso cref="Publisher"/>
    /// <seealso cref="BasePack"/>
    /// <seealso cref="Consumer{TPack}"/>
    public class TCPPublisher : Publisher
    {
        #region Variables

        /// <summary>
        /// TCP client stream.
        /// </summary>
        Stream client;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the publisher with a existing client TCP stream.
        /// </summary>
        /// <param name="imput">TCPClient's stream</param>
        public TCPPublisher(Stream imput)
        {
            this.client = imput;
        }

        /// <summary>
        /// Initializes the publisher and creates a TCP connection using the IP and port.
        /// </summary>
        /// <param name="ip">IP address that you want to start the TCP connection</param>
        /// <param name="port">Port that you want to make the connection</param>
        public TCPPublisher(string ip, int port)
        {
            TcpClient client = new TcpClient(ip, port);
            this.client = client.GetStream();
        }

        #endregion

        #region Attributes

        /// <summary>
        /// TCP client stream.
        /// </summary>
        public Stream InputOutput
        {
            get { return client; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends the packet through the TCP stream.
        /// </summary>
        /// <param name="pack">Pack to be sent by the stream.</param>
        /// <param name="ip">Optional parameter used by protocols not based connection, such as UDP.</param>
        /// <seealso cref="BasePack"/>
        public override void SendPack(BasePack pack, IPEndPoint ip = null)
        {
            if (isEnabled)
            {
                try
                {
                    client.Write(pack.Buffer, 0, pack.Buffer.Length);
                }
                catch (IOException e)
                {
                    OnConnectionClosedCall();
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Connection closed! Host do not answer.");
                }
            }
        }

        /// <summary>
        /// Sends the packet through the TCP stream.
        /// </summary>
        /// <param name="pack">Buffer to be sent as a pack by the stream.</param>
        /// <param name="ip">Optional parameter used by protocols not based connection, such as UDP.</param>
        public override void SendPack(byte[] pack, IPEndPoint ip = null)
        {
            if (isEnabled)
            {
                try
                {
                    client.Write(pack, 0, pack.Length);
                }
                catch (IOException e)
                {
                    OnConnectionClosedCall();
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Connection closed! Host do not answer.");
                }
            }
        }

        /// <summary>
        /// Method responsible for receiving packets of stream and puts them in the packet queue and signal the incoming pack event.
        /// </summary>
        /// <seealso cref="Publisher.Start"/>
        /// <seealso cref="Publisher.Stop"/>
        /// <seealso cref="Publisher.packQueue"/>
        /// <seealso cref="Publisher.incomingPackEvent"/>
        /// <seealso cref="Publisher.isEnabled"/>
        /// <seealso cref="Publisher.isAlive"/>
        protected override void Publish()
        {
            byte[] buffer;

            try
            {
                while (isEnabled)
                {
                    buffer = new byte[BasePack.packSize];
                    if (client.Read(buffer, 0, buffer.Length) == 0)
                        break;
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

        /// <summary>
        /// Stop listening packets, closes the stream and reset the properties of the publisher
        /// </summary>
        /// <seealso cref="Publisher.isAlive"/>
        /// <seealso cref="Publisher.isEnabled"/>
        /// <seealso cref="Publisher.Publish"/>
        public override void CloseConnection()
        {
            client.Close();
            base.CloseConnection();
        }

        #endregion
    }
}
