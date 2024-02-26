//-----------------------------------------------------------------------
// <copyright file="ILoggerMockSetupSequenceIsEnabled.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Allows to setup the sequence of <see cref="ILogger"/> method calls for the <see cref="ILogger.IsEnabled(LogLevel)"/>.
    /// </summary>
    public interface ILoggerMockSetupSequenceIsEnabled
    {
        /// <summary>
        /// Allows to return the specified <paramref name="value"/> when the <see cref="ILogger.IsEnabled(LogLevel)"/> is called.
        /// </summary>
        /// <param name="value">Boolean value to return when calling the <see cref="ILogger.IsEnabled(LogLevel)"/> method.</param>
        /// <returns>An instance of <see cref="ILoggerMockSetupSequence"/> which allows to continue the setup of the method calls for the <see cref="ILogger"/>.</returns>
        ILoggerMockSetupSequence Returns(bool value);
    }
}
