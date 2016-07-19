using System;

namespace NETLIB
{
    /// <summary>
    /// Throws when trying to start a <see cref="TCP.Server.TCPListenerHandler"/> that is already running.
    /// </summary>
    /// <seealso cref = "TCP.Server.TCPListenerHandler" />
    /// <seealso cref="TCP.Server.TCPListenerHandler.BeginListen(int)"/>
    public class ListenerRunnigException : Exception
    {
        #region Contructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ListenerRunnigException"/> class.
        /// </summary>
        public ListenerRunnigException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListenerRunnigException"/> class with a specified error
        /// message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ListenerRunnigException(string message) : base(message) { }

        #endregion
    }
}