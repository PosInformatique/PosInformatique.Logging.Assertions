//-----------------------------------------------------------------------
// <copyright file="ILoggerMockSetupSequenceLog.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Allows to setup the sequence of <see cref="ILogger"/> method calls for the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/>.
    /// </summary>
    public interface ILoggerMockSetupSequenceLog : ILoggerMockSetupSequence
    {
        /// <summary>
        /// Allows to assert the template message arguments.
        /// </summary>
        /// <param name="expectedCount">Number of template message arguments expected.</param>
        /// <param name="expectedArguments">A delegate which allows to check the template message arguments. All the arguments can be asserted using the <see cref="LogMessageTemplateArguments"/>
        /// parameter of the delegate.</param>
        /// <returns>An instance of <see cref="ILoggerMockSetupSequence"/> which allows to continue the setup of the method calls for the <see cref="ILogger"/>.</returns>
        ILoggerMockSetupSequence WithArguments(int expectedCount, Action<LogMessageTemplateArguments> expectedArguments);
    }
}
