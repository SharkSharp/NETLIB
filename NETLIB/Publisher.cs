using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace NETLIB
{
    /// <summary>
    /// Delegate used by the event <see cref="Publisher.ConnectionClosed"/> to encapsulete a method that will be called when the connection close. 
    /// </summary>
    /// <seealso cref="Publisher"/>
    /// <seealso cref="Publisher.ConnectionClosed"/>
    /// <seealso cref="Publisher.Dispose"/>
    /// <seealso cref="Publisher.Publish"/>
    public delegate void ConnectionClosedEventHandler();

    /// <summary>
    /// Describes a pack publisher, which will be responsible for managing the incoming packs, adding them in a <see cref="packQueue"/>
    /// and by setting an <see cref="incomingPackEvent"/> to signal to the <see cref="Consumer{TPack}.Consume"/> that there is a packet in the queue.
    /// </summary>
    /// <seealso cref="BasePack"/>
    /// <seealso cref="Consumer{TPack}"/>
    public abstract class Publisher : IDisposable
    {
        #region Variables

        /// <summary>
        /// Event used to signal when the connection (input stream) was closed for any reason.
        /// </summary>
        /// <seealso cref="Publish"/>
        /// <seealso cref="Consumer{TPack}"/>
        /// <seealso cref="Consumer{TPack}.Consume"/>
        public event ConnectionClosedEventHandler ConnectionClosed;

        /// <summary>
        /// Queue that stores incoming packets.
        /// </summary>
        /// <seealso cref="Publish"/>
        /// <seealso cref="Consumer{TPack}"/>
        /// <seealso cref="Consumer{TPack}.Consume"/>
        protected Queue<byte[]> packQueue;

        /// <summary>
        /// Event signaling that a packet arrived.
        /// </summary>
        /// <seealso cref="Publish"/>
        /// <seealso cref="Consumer{TPack}"/>
        /// <seealso cref="Consumer{TPack}.Consume"/>
        protected AutoResetEvent incomingPackEvent;

        /// <summary>
        /// Boolean indicating whether the stream is still alive and able to receive new packages.
        /// </summary>
        /// <seealso cref="Dispose"/>
        /// <seealso cref="CloseConnection"/>
        protected bool isAlive;

        /// <summary>
        /// Boolean indicating whether the publisher is currently active, publishing packages.
        /// </summary>
        /// <seealso cref="Stop"/>
        /// <seealso cref="Dispose"/>
        /// <see cref="CloseConnection"/>
        protected bool isEnabled;

        /// <summary>
        /// Thread that will run the reception and publication of packs.
        /// </summary>
        /// <seealso cref="Start"/>
        /// <seealso cref="Publish"/>
        protected Thread publisherThread;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the input event, the queue of packages and the states of the publisher.
        /// </summary>
        /// <seealso cref="incomingPackEvent"/>
        /// <seealso cref="packQueue"/>
        /// <seealso cref="isAlive"/>
        /// <see cref="isEnabled"/>
        public Publisher()
        {
            this.incomingPackEvent = new AutoResetEvent(false);
            this.packQueue = new Queue<byte[]>();
            isAlive = true;
            isEnabled = false;
        }

        /// <summary>
        /// Release resources in the death of the object if the <see cref="Dispose"/> method was not used properly.
        /// </summary>
        /// <seealso cref="Dispose"/>
        /// <seealso cref="CloseConnection"/>
        ~Publisher()
        {
            if (isAlive != false)
            {
                CloseConnection();   
            }
        }

        /// <summary>
        /// Closes the connection, and resets the states of the publisher.
        /// </summary>
        /// <seealso cref="CloseConnection"/>
        public void Dispose()
        {
            if (isAlive != false)
            {
                CloseConnection();
            }
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Boolean indicating whether the publisher is currently active, publishing packages.
        /// </summary>
        /// <seealso cref="Stop"/>
        /// <seealso cref="Dispose"/>
        /// <see cref="CloseConnection"/>
        public bool IsEnabled
        {
            get { return isEnabled; }
        }

        /// <summary>
        /// Boolean indicating whether the stream is still alive and able to receive new packages.
        /// </summary>
        /// <seealso cref="Dispose"/>
        /// <seealso cref="CloseConnection"/>
        public bool IsAlive
        {
            get { return isAlive; }
        }

        /// <summary>
        /// Gets the pack queue
        /// </summary>
        /// <seealso cref="Publish"/>
        /// <seealso cref="Consumer{TPack}"/>
        /// <seealso cref="Consumer{TPack}.Consume"/>
        public Queue<byte[]> PackQueue
        {
            get { return packQueue; }
        }

        /// <summary>
        /// Event signaling that a packet arrived.
        /// </summary>
        /// <seealso cref="Publish"/>
        /// <seealso cref="Consumer{TPack}"/>
        /// <seealso cref="Consumer{TPack}.Consume"/>
        public AutoResetEvent IncomingPackEvent
        {
            get { return incomingPackEvent; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends the packet through the same stream that is receiving packets.
        /// </summary>
        /// <param name="pack">Pack to be sent by the stream.</param>
        /// <param name="ip">Optional parameter used by protocols not based connection, such as UDP.</param>
        /// <seealso cref="BasePack"/>
        public abstract void SendPack(BasePack pack, IPEndPoint ip = null);

        /// <summary>
        /// Sends the packet through the same stream that is receiving packets.
        /// </summary>
        /// <param name="pack">Buffer to be sent as a pack by the stream.</param>
        /// <param name="ip">Optional parameter used by protocols not based connection, such as UDP.</param>
        public abstract void SendPack(byte[] pack, IPEndPoint ip = null);

        /// <summary>
        /// Method responsible for receiving packets of stream and puts them in the packet queue and signal the incoming pack event.
        /// </summary>
        /// <seealso cref="Start"/>
        /// <seealso cref="Stop"/>
        /// <seealso cref="packQueue"/>
        /// <seealso cref="incomingPackEvent"/>
        /// <seealso cref="isEnabled"/>
        /// <seealso cref="isAlive"/>
        protected abstract void Publish();

        /// <summary>
        /// Starts the packages listening/receive thread.
        /// </summary>
        /// <seealso cref="Stop"/>
        /// <see cref="Publish"/>
        /// <seealso cref="isEnabled"/>
        /// <see cref="isAlive"/>
        public void Start()
        {
            if (isAlive)
            {
                if (isEnabled && publisherThread != null && publisherThread.IsAlive)
                {
                    throw new PublisherRunnigException();
                }

                isEnabled = true;
                publisherThread = new Thread(Publish);
                publisherThread.Start();
            }
            else
            {
                throw new ConnectionClosedException("The publisher isn't alive!");
            }
        }

        /// <summary>
        /// Stops the packages listening/receive thread.
        /// </summary>
        /// <seealso cref="Start"/>
        /// <see cref="Publish"/>
        /// <seealso cref="isEnabled"/>
        /// <see cref="isAlive"/>
        public void Stop()
        {
            isEnabled = false;
        }

        /// <summary>
        /// Calls ConnectionClosedEvent
        /// </summary>
        /// <see cref="CloseConnection"/>
        protected void OnConnectionClosedCall()
        {
            if (ConnectionClosed != null)
            {
                ConnectionClosed();
            }
        }

        /// <summary>
        /// Stop listening packets, closes the stream and reset the properties of the publisher
        /// </summary>
        /// <seealso cref="isAlive"/>
        /// <seealso cref="isEnabled"/>
        /// <seealso cref="Publish"/>
        public virtual void CloseConnection()
        {
            isEnabled = false;
            isAlive = false;
            incomingPackEvent.Set();
            OnConnectionClosedCall();
        }

        #endregion
    }
}
