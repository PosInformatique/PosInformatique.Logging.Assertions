//-----------------------------------------------------------------------
// <copyright file="LoggerMock{TCategoryName}.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Allows to mock and retrieve an instance of <see cref="ILogger{TCategoryName}"/>. To mock a <see cref="ILogger{TCategoryName}"/>:
    /// <list type="bullet">
    ///     <item>Creates an instance of the <see cref="LoggerMock{TCategoryName}"/>.</item>
    ///     <item>
    ///         Setup the expected the calls to the
    ///         <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/> and
    ///         <see cref="ILogger.BeginScope{TState}(TState)"/> methods.
    ///     </item>
    ///     <item>Gets the mocked instance of the <see cref="ILogger{TCategoryName}"/> using the <see cref="object"/> property.</item>
    ///     <item>Do not forget to call the <see cref="LoggerMock.VerifyLogs"/> method at the end of the unit test to check that there is no missing
    ///         calls to the <see cref="ILogger"/> methods.</item>
    /// </list>
    /// </summary>
    /// <typeparam name="TCategoryName">The category name of the <see cref="ILogger{TCategoryName}"/> to create.</typeparam>
    public class LoggerMock<TCategoryName> : LoggerMock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerMock{TCategoryName}"/> class.
        /// </summary>
        public LoggerMock()
        {
        }

        /// <summary>
        /// Gets the mocked instance of the <see cref="ILogger{TCategoryName}"/>.
        /// </summary>
        public new ILogger<TCategoryName> Object => (ILogger<TCategoryName>)base.Object;

        /// <inheritdoc/>
        private protected override ILogger CreateRecorder<TIgnored>()
        {
            return base.CreateRecorder<TCategoryName>();
        }
    }
}
