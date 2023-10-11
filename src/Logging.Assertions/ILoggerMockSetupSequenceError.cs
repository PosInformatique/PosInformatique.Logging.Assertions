//-----------------------------------------------------------------------
// <copyright file="ILoggerMockSetupSequenceError.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions
{
    using FluentAssertions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Allows to setup the sequence of <see cref="ILogger"/> method calls for the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/>
    /// with the <see cref="LogLevel.Error"/> log level when an <see cref="Exception"/> is occured.
    /// </summary>
    public interface ILoggerMockSetupSequenceError : ILoggerMockSetupSequence
    {
        /// <summary>
        /// Allows to check the <see cref="Exception"/> passed in the argument of the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/>.
        /// </summary>
        /// <param name="exception">Delegate which allows to analyze the content of the <see cref="Exception"/>.</param>
        /// <returns>An instance of <see cref="ILoggerMockSetupSequence"/> which allows to continue the setup of the method calls.</returns>
        ILoggerMockSetupSequence WithException(Action<Exception> exception);

        /// <summary>
        /// Allows to check the <see cref="Exception"/> passed in the argument of the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/>.
        /// </summary>
        /// <param name="expectedException"><see cref="Exception"/> instance expected.</param>
        /// <returns>An instance of <see cref="ILoggerMockSetupSequence"/> which allows to continue the setup of the method calls.</returns>
        ILoggerMockSetupSequence WithException(Exception expectedException)
            => this.WithException(actualException => actualException.Should().BeSameAs(expectedException));
    }
}
