//-----------------------------------------------------------------------
// <copyright file="LoggerMockSetupSequenceErrorExtensions.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions
{
    using FluentAssertions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Extensions method of the <see cref="ILoggerMockSetupSequenceError"/> interface to setup the <see cref="LogLevel.Error"/> log level.
    /// </summary>
    public static class LoggerMockSetupSequenceErrorExtensions
    {
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
    }
}
