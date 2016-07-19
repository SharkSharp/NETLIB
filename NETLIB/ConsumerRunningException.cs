using System;

namespace NETLIB
{
    /// <summary> 
    /// Throws when trying to start a <see cref="Consumer{TPack}"/> that is already running.
    /// </summary>
    /// <seealso cref="Consumer{TPack}"/>
    /// <seealso cref="Consumer{TPack}.Start"/>
    /// <seealso cref="Consumer{TPack}.StartConsume"/>
    public class ConsumerRunningException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerRunningException"/> class.
        /// </summary>
        public ConsumerRunningException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerRunningException"/> class with a specified error
        /// message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConsumerRunningException(string message) : base(message) { }
    }
}
