//-----------------------------------------------------------------------
// <copyright file="LoggerMockSetupSequenceExtensions.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Extensions method of the <see cref="ILoggerMockSetupSequence"/> interface to setup the sequence of the expected logs.
    /// </summary>
    public static class LoggerMockSetupSequenceExtensions
    {
        /// <summary>
        /// Expect the call to the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/> method
        /// with a <see cref="LogLevel.Debug"/> log level.
        /// </summary>
        /// <param name="sequence"><see cref="ILoggerMockSetupSequence"/> to setup the sequence.</param>
        /// <param name="message">Message of the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/> call expected.</param>
        /// <returns>The current <paramref name="sequence"/> which allows to continue the setup of the <see cref="ILogger"/> method calls.</returns>
        public static ILoggerMockSetupSequence LogDebug(this ILoggerMockSetupSequence sequence, string message)
        {
            return sequence.Log(LogLevel.Debug, message);
        }

        /// <summary>
        /// Expect the call to the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/> method
        /// with a <see cref="LogLevel.Information"/> log level.
        /// </summary>
        /// <param name="sequence"><see cref="ILoggerMockSetupSequence"/> to setup the sequence.</param>
        /// <param name="message">Message of the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/> call expected.</param>
        /// <returns>The current <paramref name="sequence"/> which allows to continue the setup of the <see cref="ILogger"/> method calls.</returns>
        public static ILoggerMockSetupSequence LogInformation(this ILoggerMockSetupSequence sequence, string message)
        {
            return sequence.Log(LogLevel.Information, message);
        }

        /// <summary>
        /// Expect the call to the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/> method
        /// with a <see cref="LogLevel.Trace"/> log level.
        /// </summary>
        /// <param name="sequence"><see cref="ILoggerMockSetupSequence"/> to setup the sequence.</param>
        /// <param name="message">Message of the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/> call expected.</param>
        /// <returns>The current <paramref name="sequence"/> which allows to continue the setup of the <see cref="ILogger"/> method calls.</returns>
        public static ILoggerMockSetupSequence LogTrace(this ILoggerMockSetupSequence sequence, string message)
        {
            return sequence.Log(LogLevel.Trace, message);
        }

        /// <summary>
        /// Expect the call to the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/> method
        /// with a <see cref="LogLevel.Warning"/> log level.
        /// </summary>
        /// <param name="sequence"><see cref="ILoggerMockSetupSequence"/> to setup the sequence.</param>
        /// <param name="message">Message of the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/> call expected.</param>
        /// <returns>The current <paramref name="sequence"/> which allows to continue the setup of the <see cref="ILogger"/> method calls.</returns>
        public static ILoggerMockSetupSequence LogWarning(this ILoggerMockSetupSequence sequence, string message)
        {
            return sequence.Log(LogLevel.Warning, message);
        }
    }
}
