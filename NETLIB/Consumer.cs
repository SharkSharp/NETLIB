using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace NETLIB
{
    /// <summary>
    /// Delegate used to encapsulate a method tha will be called when a package is consumed.
    /// </summary>
    /// <typeparam name="TPack">Type derived from BasePack that specify the pack to be used.</typeparam>
    /// <param name="consumer">Consumer that called the method.</param>
    /// <param name="receivedPack">Pack to be consumed.</param>
    public delegate void ThrowPackEventHandler<TPack>(Consumer<TPack> consumer, TPack receivedPack) where TPack : BasePack;

    /// <summary>
    /// Delegate used by the event <see cref="Consumer{TPack}.ConnectionClosed"/> to encapsulete a method that will be called when the connection close. 
    /// </summary>
    /// <typeparam name="Type">Type of the consumer that will be sent in case of closed connection.</typeparam>
    /// <param name="consumer">Consumer who had a close connection.</param>
    /// <seealso cref="Publisher"/>
    /// <seealso cref="Consumer{TPack}.ConnectionClosed"/>
    /// <see cref="Consumer{TPack}.OnConnectionClosedCall"/>
    public delegate void ConnectionClosedEventHandler<in Type>(Type consumer);

    /// <summary>
    /// Describes the class that will be responsible for consuming the packages, meaning it will build 
    /// packages with the buffers published by a publisher and will launch an event for every pack to be treated.
    /// </summary>
    /// <typeparam name="TPack">Pack class derived from <see cref="BasePack"/> that the Consumer will manage.</typeparam>
    /// <seealso cref="BasePack"/>
    /// <seealso cref="NETLIB.Publisher"/>
    /// <seealso cref="ThrowPackEventHandler{TPack}"/>
    public abstract class Consumer<TPack> : IDisposable where TPack : BasePack
    {
        #region Variables

        /// <summary>
        /// Event called for treatment and consumption of a incoming pack.
        /// </summary>
        /// <seealso cref="ThrowPackEventHandler{TPack}"/>
        /// <seealso cref="Consume"/>
        public event ThrowPackEventHandler<TPack> ReceivedPack;

        /// <summary>
        /// Event used to signal when the connection (input stream) was closed for any reason.
        /// </summary>
        /// <seealso cref="NETLIB.Publisher.ConnectionClosed"/>
        /// <seealso cref="OnConnectionClosedCall"/>
        /// <seealso cref="CloseConnection"/>
        public event ConnectionClosedEventHandler<Consumer<TPack>> ConnectionClosed;

        /// <summary>
        /// Publisher responsible for publishing the packages to be consumed.
        /// </summary>
        /// <seealso cref="NETLIB.Publisher"/>
        /// <seealso cref="NETLIB.Publisher.Publish"/>
        /// <seealso cref="Consume"/>
        protected Publisher publisher;

        /// <summary>
        /// Queue that stores incoming packets. Reference the queue created by <see cref="publisher"/>.
        /// </summary>
        /// <seealso cref="NETLIB.Publisher.PackQueue"/>
        /// <seealso cref="NETLIB.Publisher.Publish"/>
        /// <seealso cref="Consume"/>
        protected Queue<byte[]> packQueue;

        /// <summary>
        /// Event signaling that a packet arrived. Reference the event created by <see cref="publisher"/>.
        /// </summary>
        /// <seealso cref="NETLIB.Publisher"/>
        /// <seealso cref="NETLIB.Publisher.IncomingPackEvent"/>
        protected AutoResetEvent incomingPackEvent;

        /// <summary>
        /// Thread used to walk through the <see cref="packQueue"/> to the end, consuming up to
        /// the last pack and wait until another packet arrives to be consumed.
        /// </summary>
        /// <seealso cref="Consume"/>
        /// <seealso cref="Start"/>
        /// <seealso cref="StartConsume"/>
        protected Thread consumerThread;

        /// <summary>
        /// Boolean indicating when the consumption of packages is active.
        /// </summary>
        /// <seealso cref="Consume"/>
        protected bool isEnabled;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the consumer with a publisher and define the initial settings.
        /// </summary>
        /// <param name="publisher">Publisher to be consumed.</param>
        /// <seealso cref="NETLIB.Publisher"/>
        /// <seealso cref="packQueue"/>
        /// <seealso cref="OnConnectionClosedCall"/>
        public Consumer(Publisher publisher)
        {
            this.isEnabled = false;
            this.publisher = publisher;
            this.packQueue = publisher.PackQueue;
            this.incomingPackEvent = publisher.IncomingPackEvent;

            publisher.ConnectionClosed += OnConnectionClosedCall;
        }

        /// <summary>
        /// Ends the consumption of paks without changing the status of the publisher,
        /// in the death of the object, if the Dispose method was not used properly.
        /// </summary>
        /// <seealso cref="Dispose"/>
        /// <seealso cref="CloseConnection"/>
        ~Consumer()
        {
            if (isEnabled)
            {
                EndConsume();
            }
        }

        /// <summary>
        /// Ends the consumption of paks without changing the status of the publisher.
        /// </summary>
        /// <seealso cref="CloseConnection"/>
        public void Dispose()
        {
            if (isEnabled)
            {
                EndConsume();
            }
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Boolean indicating when the consumption of packages is active.
        /// </summary>
        /// <seealso cref="isEnabled"/>
        /// <seealso cref="Consume"/>
        /// <seealso cref="Start"/>
        /// <seealso cref="StartConsume"/>
        /// <seealso cref="Dispose"/>
        /// <seealso cref="EndConsume"/>
        /// <seealso cref="EndPublishConsume"/>
        /// <seealso cref="CloseConnection"/>
        public bool IsEnabled
        {
            get { return isEnabled; }
        }

        /// <summary>
        /// Boolean indicating whether the publisher is currently active, publishing packages.
        /// </summary>
        /// <seealso cref="NETLIB.Publisher.IsEnabled"/>
        public bool IsPublishEnabled
        {
            get { return publisher.IsEnabled; }
        }

        /// <summary>
        /// Returns the publisher being consumed.
        /// </summary>
        /// <seealso cref="publisher"/>
        /// <seealso cref="NETLIB.Publisher"/>
        public Publisher Publisher
        {
            get { return publisher; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends the packet through the <see cref="publisher"/>
        /// </summary>
        /// <param name="pack">Pack to be sent by the stream.</param>
        /// <param name="ip">Optional parameter used by protocols not based connection, such as UDP.</param>
        /// <seealso cref="BasePack"/>
        public virtual void SendPack(TPack pack, IPEndPoint ip = null)
        {
            publisher.SendPack(pack, ip);
        }

        /// <summary>
        /// Sends the packet through the <see cref="publisher"/>
        /// </summary>
        /// <param name="pack">Buffer to be sent as a pack by the stream.</param>
        /// <param name="ip">Optional parameter used by protocols not based connection, such as UDP.</param>
        public virtual void SendPack(byte[] pack, IPEndPoint ip = null)
        {
            publisher.SendPack(pack, ip);
        }

        /// <summary>
        /// Calls <see cref="ReceivedPack"/> for treatment and consumption of a incoming package.
        /// </summary>
        /// <param name="pack">Arrived pack to be consumed.</param>
        /// <seealso cref="ReceivedPack"/>
        /// <seealso cref="Consume"/>
        /// <seealso cref="NETLIB.Publisher.Publish"/>
        protected virtual void OnReceivedPackCall(TPack pack)
        {
            if (ReceivedPack != null)
            {
                ReceivedPack(this, pack);
            }
        }

        /// <summary>
        /// Starts the thread of consumption of packs.
        /// </summary>
        /// <seealso cref="consumerThread"/>
        /// <seealso cref="Start"/>
        /// <seealso cref="Consume"/>
        /// <seealso cref="EndConsume"/>
        /// <seealso cref="CloseConnection"/>
        public void StartConsume()
        {
            if (publisher.IsAlive)
            {
                if (isEnabled && consumerThread != null && consumerThread.IsAlive)
                {
                    throw new ConsumerRunningException("The consumer still running!");
                }

                isEnabled = true;
                consumerThread = new Thread(Consume);
                consumerThread.Start();
            }
            else
            {
                throw new ConnectionClosedException("Connection closed!");
            }
        }

        /// <summary>
        /// Starts the publisher and the thread of consumption of packs.
        /// </summary>
        /// <seealso cref="consumerThread"/>
        /// <seealso cref="NETLIB.Publisher.Start"/>
        /// <seealso cref="StartConsume"/>
        /// <seealso cref="Consume"/>
        /// <seealso cref="EndConsume"/>
        /// <seealso cref="CloseConnection"/>
        public void Start()
        {
            publisher.Start();
            StartConsume();
        }

        /// <summary>
        /// Closes the publisher connection.
        /// </summary>
        /// <seealso cref="NETLIB.Publisher.CloseConnection"/>
        public void CloseConnection()
        {
            publisher.CloseConnection();
        }

        /// <summary>
        /// Stops the pack consumption thread.
        /// </summary>
        /// <seealso cref="EndPublishConsume"/>
        /// <seealso cref="CloseConnection"/>
        public void EndConsume()
        {
            if (consumerThread != null)
            {
                isEnabled = false;
            }
        }

        /// <summary>
        /// Stops the publisher and the pack consumption thread.
        /// </summary>
        /// <seealso cref="NETLIB.Publisher.Stop"/>
        /// <seealso cref="EndConsume"/>
        public void EndPublishConsume()
        {
            publisher.Stop();
            EndConsume();
        }

        /// <summary>
        /// Responsible for waiting a sign of <see cref="NETLIB.Publisher.IncomingPackEvent"/>, packing the buffer data
        /// in a class derived from <see cref="BasePack"/>(<typeparamref name="TPack"/>) and call <see cref="ReceivedPack"/>
        /// for treatment and consumption of the incoming package.
        /// </summary>
        /// <seealso cref="NETLIB.Publisher.IncomingPackEvent"/>
        /// <seealso cref="PackFactory(BasePack)"/>
        /// <seealso cref="PackFactory(byte[])"/>
        /// <seealso cref="ReceivedPack"/>
        /// <seealso cref="BasePack"/>
        /// <seealso cref="NETLIB.Publisher.Publish"/>
        protected virtual void Consume()
        {
            while (publisher.IsEnabled && isEnabled)
            {
                while (packQueue.Count > 0)
                {
                    OnReceivedPackCall(PackFactory(packQueue.Dequeue()));
                }

                incomingPackEvent.WaitOne();
            }

            while (packQueue.Count > 0)
            {
                OnReceivedPackCall(PackFactory(packQueue.Dequeue()));
            }
        }

        /// <summary>
        /// Used by the <see cref="Consume"/> to obtain an instance of the <typeparamref name="TPack"/>
        /// through <see cref="BasePack(BasePack)"/>  constructor.
        /// </summary>
        /// <param name="pack">BasePack to be based on.</param>
        /// <returns>New <typeparamref name="TPack"/> based on <paramref name="pack"/></returns>
        public abstract TPack PackFactory(BasePack pack);

        /// <summary>
        /// Used by the <see cref="Consume"/> to obtain an instance of the <typeparamref name="TPack"/>
        /// through <see cref="BasePack(byte[])"/> constructor.
        /// </summary>
        /// <param name="pack">Buffer to be based on.</param>
        /// <returns>New <typeparamref name="TPack"/> based on <paramref name="pack"/></returns>
        public abstract TPack PackFactory(byte[] pack);

        /// <summary>
        /// Used by the <see cref="Consume"/> to obtain an instance of the <typeparamref name="TPack"/>
        /// through <see cref="BasePack()"/> constructor.
        /// </summary>
        /// <returns>New <typeparamref name="TPack"/>.</returns>
        public abstract TPack PackFactory();

        /// <summary>
        /// Calls <see cref="ConnectionClosed"/> sending this as a parameter.
        /// </summary>
        /// <seealso cref="ConnectionClosed"/>
        private void OnConnectionClosedCall()
        {
            if (ConnectionClosed != null)
            {
                ConnectionClosed(this);
            }
        }

        #endregion
    }
}
