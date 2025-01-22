//-----------------------------------------------------------------------
// <copyright file="LoggerMockSetupSequenceExtensions.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Extensions method of the <see cref="ILoggerMockSetupSequence"/> interface to setup the sequence of the expected logs.
    /// </summary>
    public static class LoggerMockSetupSequenceExtensions
    {
        /// <summary>
        /// Expect the call to the <see cref="ILogger.BeginScope{TState}(TState)"/> method with the specified <paramref name="state"/> object.
        /// </summary>
        /// <param name="sequence"><see cref="ILoggerMockSetupSequence"/> to setup the sequence.</param>
        /// <param name="state">Expected state of the <see cref="ILogger.BeginScope{TState}(TState)"/> call. The state object actual and expected
        /// are compared by equivalence (property by property) and not by instance.</param>
        /// <returns>The current <see cref="ILoggerMockSetupSequence"/> which allows to continue the setup of the <see cref="ILogger"/> method calls.</returns>
        public static ILoggerMockSetupSequence BeginScope(this ILoggerMockSetupSequence sequence, object state)
        {
            return sequence.BeginScope<object>(expectedState => AssertionHelper.BeEquivalentTo(ToDictionary(state), ToDictionary(expectedState)));
        }

        /// <summary>
        /// Expect the call to the <see cref="ILogger.BeginScope{TState}(TState)"/> method with a <see cref="Dictionary{TKey, TValue}"/>
        /// of <see cref="string"/>/<see cref="object"/> represents by the <paramref name="state"/> object instance.
        /// The dictionary is compared by all the public property of the specified <paramref name="state"/> object instance.
        /// </summary>
        /// <param name="sequence"><see cref="ILoggerMockSetupSequence"/> to setup the sequence.</param>
        /// <param name="state">Expected state of the <see cref="ILogger.BeginScope{TState}(TState)"/> call. The properties of the expected object <paramref name="state"/>
        /// is compared by a dictionary of <see cref="string"/>/<see cref="object"/> specified in the argument when calling the
        /// <see cref="ILogger.BeginScope{TState}(TState)"/> method.</param>
        /// <returns>The current <see cref="ILoggerMockSetupSequence"/> which allows to continue the setup of the <see cref="ILogger"/> method calls.</returns>
        public static ILoggerMockSetupSequence BeginScopeAsDictionary(this ILoggerMockSetupSequence sequence, object state)
        {
            return sequence.BeginScope<IDictionary<string, object>>(expectedState => AssertionHelper.BeEquivalentTo(ToDictionary(state), expectedState));
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
        public static ILoggerMockSetupSequenceLog WithException(this ILoggerMockSetupSequenceError sequence, Exception expectedException)
        {
            return sequence.WithException(actualException => AssertionHelper.BeSameAs(actualException, expectedException));
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
                    if (!Equals(actualArguments[i], expectedArguments[i]))
                    {
                        throw new LoggingAssertionFailedException(
                            $"Log message arguments at the index {i} is different.{Environment.NewLine}Expected: {AssertionHelper.ToString(expectedArguments[i])}{Environment.NewLine}Actual: {AssertionHelper.ToString(actualArguments[i])}");
                    }
                }
            });
        }

        private static IDictionary<string, object> ToDictionary(object @object)
        {
            var dictionary = new Dictionary<string, object>();

            foreach (var property in @object.GetType().GetProperties())
            {
                dictionary.Add(property.Name, property.GetValue(@object));
            }

            return dictionary;
        }
    }
}
