using System;

namespace NETLIB
{
    /// <summary>
    /// Throws when trying to start a <see cref="Publisher"/> that is already running.
    /// </summary>
    /// <seealso cref="Publisher"/>
    /// <seealso cref="Publisher.Start"/>
    public class PublisherRunnigException : Exception
    {
        #region Contructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherRunnigException"/> class.
        /// </summary>
        public PublisherRunnigException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherRunnigException"/> class with a specified error
        /// message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PublisherRunnigException(string message) : base(message) { }

        #endregion
    }
}