//-----------------------------------------------------------------------
// <copyright file="ILoggerMockSetupSequence.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Allows to setup the sequence of the <see cref="ILogger"/> methods.
    /// </summary>
    public interface ILoggerMockSetupSequence
    {
        /// <summary>
        /// Expect the call to the <see cref="ILogger.BeginScope{TState}(TState)"/> method.
        /// </summary>
        /// <typeparam name="TState">Type of the state expected.</typeparam>
        /// <param name="state">Delegate called to assert the content of the state when the
        /// the <see cref="ILogger.BeginScope{TState}(TState)"/> is called.</param>
        /// <returns>The current <see cref="ILoggerMockSetupSequence"/> which allows to continue the setup of the <see cref="ILogger"/> method calls.</returns>
        ILoggerMockSetupSequence BeginScope<TState>(Action<TState> state);

        /// <summary>
        /// Expect the call to the <see cref="IDisposable.Dispose"/> method which represents the scope
        /// returned by the <see cref="ILogger.BeginScope{TState}(TState)"/> method..
        /// </summary>
        /// <returns>The current <see cref="ILoggerMockSetupSequence"/> which allows to continue the setup of the <see cref="ILogger"/> method calls.</returns>
        ILoggerMockSetupSequence EndScope();

        /// <summary>
        /// Expect the call to the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/> method.
        /// </summary>
        /// <param name="logLevel"><see cref="LogLevel"/> of the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/> call expected.</param>
        /// <param name="message">Message of the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/> call expected.</param>
        /// <returns>The current <see cref="ILoggerMockSetupSequence"/> which allows to continue the setup of the <see cref="ILogger"/> method calls.</returns>
        ILoggerMockSetupSequenceLog Log(LogLevel logLevel, string message);

        /// <summary>
        /// Expect the call to the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/> method
        /// with a <see cref="LogLevel.Error"/> log level.
        /// </summary>
        /// <param name="message">Message of the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/> call expected.</param>
        /// <returns>The current <see cref="ILoggerMockSetupSequenceError"/> which allows to continue the setup of the <see cref="ILogger"/> method calls
        /// and analyze also the <see cref="Exception"/> instance related to the log error message.</returns>
        ILoggerMockSetupSequenceError LogError(string message);
    }
}
