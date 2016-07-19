using System.IO;

namespace NETLIB
{
    /// <summary>
    /// Throws when trying to start a <see cref="Publisher"/> with a closed stream.
    /// </summary>
    /// <seealso cref="Publisher"/>
    /// <seealso cref="Publisher.Start"/>
    /// <seealso cref="Consumer{TPack}"/>
    /// <seealso cref="Consumer{TPack}.Start"/>
    public class ConnectionClosedException : IOException
    {
        #region Contructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionClosedException"/> class.
        /// </summary>
        public ConnectionClosedException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionClosedException"/> class with a specified error
        /// message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConnectionClosedException(string message)
            : base(message)
        { }

        #endregion
    }
}