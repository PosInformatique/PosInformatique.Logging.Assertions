//-----------------------------------------------------------------------
// <copyright file="LoggerMockSetupSequenceExtensions.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions
{
    using FluentAssertions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Extensions method of the <see cref="ILoggerMockSetupSequence"/> interface to setup the sequence of the expected logs.
    /// </summary>
    public static class LoggerMockSetupSequenceExtensions
    {
        /// <summary>
        /// Expect the call to the <see cref="ILogger.BeginScope{TState}(TState)"/> method.
        /// </summary>
        /// <param name="sequence"><see cref="ILoggerMockSetupSequence"/> to setup the sequence.</param>
        /// <param name="state">Expected state of the <see cref="ILogger.BeginScope{TState}(TState)"/> call. The state object actual and expected
        /// are compared by equivalence (property by property) and not by instance.</param>
        /// <returns>The current <see cref="ILoggerMockSetupSequence"/> which allows to continue the setup of the <see cref="ILogger"/> method calls.</returns>
        public static ILoggerMockSetupSequence BeginScope(this ILoggerMockSetupSequence sequence, object state)
        {
            return sequence.BeginScope<object>(expectedState => state.Should().BeEquivalentTo(state));
        }

        /// <summary>
        /// Expect the call to the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/> method
        /// with a <see cref="LogLevel.Debug"/> log level.
        /// </summary>
        /// <param name="sequence"><see cref="ILoggerMockSetupSequence"/> to setup the sequence.</param>
        /// <param name="message">Message of the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/> call expected.</param>
        /// <returns>The current <paramref name="sequence"/> which allows to continue the setup of the <see cref="ILogger"/> method calls.</returns>
        public static ILoggerMockSetupSequenceLog LogDebug(this ILoggerMockSetupSequence sequence, string message)
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
        public static ILoggerMockSetupSequenceLog LogInformation(this ILoggerMockSetupSequence sequence, string message)
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
        public static ILoggerMockSetupSequenceLog LogTrace(this ILoggerMockSetupSequence sequence, string message)
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
        public static ILoggerMockSetupSequenceLog LogWarning(this ILoggerMockSetupSequence sequence, string message)
        {
            return sequence.Log(LogLevel.Warning, message);
        }

        /// <summary>
        /// Allows to check the <see cref="Exception"/> passed in the argument of the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/>.
        /// </summary>
        /// <param name="sequence"><see cref="ILoggerMockSetupSequence"/> to setup the sequence.</param>
        /// <param name="expectedException"><see cref="Exception"/> instance expected.</param>
        /// <returns>An instance of <see cref="ILoggerMockSetupSequence"/> which allows to continue the setup of the method calls.</returns>
        public static ILoggerMockSetupSequence WithException(this ILoggerMockSetupSequenceError sequence, Exception expectedException)
        {
            return sequence.WithException(actualException => actualException.Should().BeSameAs(expectedException));
        }

        /// <summary>
        /// Allows to assert the template message arguments using a list of parameters.
        /// </summary>
        /// <param name="sequence"><see cref="ILoggerMockSetupSequence"/> to setup the sequence.</param>
        /// <param name="expectedArguments">List of the expected arguments in the template message. The arguments have to be specified in the right expected order.</param>
        /// <returns>An instance of <see cref="ILoggerMockSetupSequence"/> which allows to continue the setup of the method calls for the <see cref="ILogger"/>.</returns>
        public static ILoggerMockSetupSequence WithArguments(this ILoggerMockSetupSequenceLog sequence, params object[] expectedArguments)
        {
            return sequence.WithArguments(expectedArguments.Length, actualArguments =>
            {
                for (int i = 0; i < expectedArguments.Length; i++)
                {
                    actualArguments[i].Should().Be(expectedArguments[i]);
                }
            });
        }
    }
}
