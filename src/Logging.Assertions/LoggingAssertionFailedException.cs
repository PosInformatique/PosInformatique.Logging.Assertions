//-----------------------------------------------------------------------
// <copyright file="LoggingAssertionFailedException.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions
{
    /// <summary>
    /// Occurs when a logging assertion has been failed.
    /// </summary>
    public class LoggingAssertionFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingAssertionFailedException"/> class.
        /// </summary>
        public LoggingAssertionFailedException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingAssertionFailedException"/> class
        /// with the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        public LoggingAssertionFailedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingAssertionFailedException"/> class
        /// with the specified <paramref name="message"/> and the <paramref name="innerException"/>.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Inner exception related to the <see cref="LoggingAssertionFailedException"/> to create.</param>
        public LoggingAssertionFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
