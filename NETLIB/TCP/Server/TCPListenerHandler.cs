using System;
using System.Net.Sockets;
using System.Threading;

namespace NETLIB.TCP.Server
{

    /// <summary>
    /// Delegate that encapsulates the methods of the event responsible for sending
    /// the new TCP client connected to the application responsible for use it.
    /// </summary>
    /// <param name="publisher"></param>
    public delegate void ReceivedConnectionEventHandler(Publisher publisher);

    /// <summary>
    /// Manages customer connection, remains listening TCP connections on a specific port,
    /// puts this connection in a <see cref="IOPackHandler{TPack}"/> and calls an event,
    /// sending the new client as a parameter
    /// </summary>
    /// <seealso cref="Publisher"/>
    /// <seealso cref="TCPPublisher"/>
    /// <example>
    /// The following example shows how to implement a chat server using <see cref="TCPListenerHandler"/> 
    /// to manage incoming connections.
    /// <code>
    ///using NETLIB;
    ///using NETLIB.TCP.Server;
    ///using System.Collections.Generic;
    ///
    ///namespace ChatExempleServer
    ///{
    ///    class Program
    ///    {
    ///        static TCPListenerHandler listenerHandler;
    ///        static List&lt;IOBasePackHandler&gt; clients;
    ///        static Protocol&lt;BasePack&gt; chatProtocol;
    ///
    ///        static void Main(string[] args)
    ///        {
    ///            clients = new List&lt;IOBasePackHandler&gt;();
    ///
    ///            chatProtocol = new Protocol&lt;BasePack&gt;("chatProtocol");
    ///            chatProtocol[0] += MessagePackHandle;
    ///
    ///            listenerHandler = new TCPListenerHandler();
    ///            listenerHandler.ReceivedConnection += ListenerHandlerReceivedConnection;
    ///            listenerHandler.BeginListen(1975);
    ///        }
    ///
    ///        private static void MessagePackHandle(Consumer&lt;BasePack&gt; consumer, BasePack receivedPack)
    ///        {
    ///            foreach (var client in clients)
    ///            {
    ///                if (client != consumer)
    ///                {
    ///                    client.SendPack(receivedPack);
    ///                }
    ///            }
    ///        }
    ///
    ///        private static void ListenerHandlerReceivedConnection(Publisher publisher)
    ///        {
    ///            var newClient = new IOBasePackHandler(publisher, chatProtocol);
    ///            clients.Add(newClient);
    ///            newClient.Start();
    ///        }
    ///    }
    ///}
    /// </code>
    /// </example>
    public class TCPListenerHandler : IDisposable
    {
        #region Variables

        /// <summary>
        /// Event to be called when entering a new TCP connection.
        /// </summary>
        public event ReceivedConnectionEventHandler ReceivedConnection;

        /// <summary>
        /// <see cref="TcpListener"/> that will listen TCP connections.
        /// </summary>
        TcpListener listener;

        /// <summary>
        /// Thread responsible for accepting and dealing with new connections.
        /// </summary>
        Thread listenThread;

        /// <summary>
        /// Boolean indicating when listening for new connections is active.
        /// </summary>
        bool isEnabled;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new listener without start the listening.
        /// </summary>
        public TCPListenerHandler()
        {
            isEnabled = false;
            listenThread = new Thread(ReceiveConnection);
        }

        /// <summary>
        /// Release resources in the death of the object if the <see cref="Dispose"/> method was not used properly.
        /// </summary>
        ~TCPListenerHandler()
        {
            Dispose();
        }

        /// <summary>
        /// Ends the listener thread and stop the <see cref="listener"/>.
        /// </summary>
        public void Dispose()
        {
            isEnabled = false;
            if (listener != null)
            {
                listener.Stop();
            }
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Boolean indicating when listening for new connections is active.
        /// </summary>
        public bool Enabled
        {
            get { return isEnabled; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Begins listening for connections on a particular port.
        /// </summary>
        /// <param name="port">Port to listen.</param>
        /// <exception cref="ListenerRunnigException">Throws when the Listener is already listening.</exception>
        public void BeginListen(int port)
        {
            if (!isEnabled)
            {
                listener = new TcpListener(port);

                listenThread.Start();
                isEnabled = true;
            }
            else
            {
                throw new ListenerRunnigException("Listener already listening.");
            }
        }

        /// <summary>
        /// Stop listening thread and <see cref="listener"/>
        /// </summary>
        public void StopListen()
        {
            if (isEnabled)
            {
                isEnabled = false;
                listener.Stop();
            }
        }

        /// <summary>
        /// Function that will accept and treat new connections.
        /// </summary>
        /// <seealso cref="Publisher"/>
        /// /// <seealso cref="TCPPublisher"/>
        private void ReceiveConnection()
        {
            listener.Start();

            try
            {
                while (isEnabled)
                {
                    OnReceivedConnectionCall(new TCPPublisher(listener.AcceptTcpClient().GetStream()));
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message + " - null input stream.");
            }
        }

        /// <summary>
        /// Calls the connections incoming event by sending a new connection encapsulated in a <see cref="TCPPublisher"/>.
        /// </summary>
        /// <param name="conexao">New connection encapsulated in a <see cref="TCPPublisher"/>.</param>
        private void OnReceivedConnectionCall(Publisher conexao)
        {
            if (ReceivedConnection != null)
            {
                ReceivedConnection(conexao);
            }
        }

        #endregion
    }
}
