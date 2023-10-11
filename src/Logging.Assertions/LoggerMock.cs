//-----------------------------------------------------------------------
// <copyright file="LoggerMock.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions
{
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using FluentAssertions.Common;
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
    ///     <item>Gets the mocked instance of the <see cref="ILogger{TCategoryName}"/> using the <see cref="Object"/> property.</item>
    ///     <item>Do not forget to call the <see cref="VerifyLogs"/> method at the end of the unit test to check that there is no missing
    ///         calls to the <see cref="ILogger"/> methods.</item>
    /// </list>
    /// </summary>
    /// <typeparam name="TCategoryName">The category name of the <see cref="ILogger{TCategoryName}"/> to create.</typeparam>
    public sealed class LoggerMock<TCategoryName>
    {
        private readonly IList<ExpectedLogAction> expectedLogActions;

        private int expectedLogActionsIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerMock{TCategoryName}"/> class.
        /// </summary>
        public LoggerMock()
        {
            this.expectedLogActions = new List<ExpectedLogAction>();
            this.Object = new LoggerRecorder(this);
        }

        /// <summary>
        /// Gets the mocked instance of the <see cref="ILogger{TCategoryName}"/>.
        /// </summary>
        public ILogger<TCategoryName> Object { get; }

        /// <summary>
        /// Entry method which allows to setup the sequence of the expected <see cref="ILogger"/> method calls.
        /// </summary>
        /// <returns>An instance of <see cref="ILoggerMockSetupSequence"/> which allows to setup the expected <see cref="ILogger"/> method calls.</returns>
        public ILoggerMockSetupSequence SetupSequence()
        {
            return new LoggerMockSetupSequence(this);
        }

        /// <summary>
        /// To be call at the end of the unit test to check there is no missing expected calls of the <see cref="ILogger"/> methods.
        /// </summary>
        public void VerifyLogs()
        {
            if (this.expectedLogActions.Count != this.expectedLogActionsIndex)
            {
                var missingCalls = this.expectedLogActions.Skip(this.expectedLogActionsIndex).Select(m => "- " + m.ExceptionDisplayMessage);
                var missingCallsString = string.Join(Environment.NewLine, missingCalls);

                Services.ThrowException($"Logger has been called few times (Expected: {this.expectedLogActions.Count} calls, Actual: {this.expectedLogActionsIndex} calls).{Environment.NewLine}{missingCallsString}");
            }
        }

        private sealed class LoggerMockSetupSequence : ILoggerMockSetupSequence
        {
            private readonly LoggerMock<TCategoryName> mock;

            public LoggerMockSetupSequence(LoggerMock<TCategoryName> mock)
            {
                this.mock = mock;
            }

            public ILoggerMockSetupSequence BeginScope(object state)
            {
                this.mock.expectedLogActions.Add(new ExpectedLogBeginScope(state));

                return this;
            }

            public ILoggerMockSetupSequence EndScope()
            {
                this.mock.expectedLogActions.Add(new ExpectedLogEndScope());

                return this;
            }

            public ILoggerMockSetupSequence Log(LogLevel logLevel, string message)
            {
                this.mock.expectedLogActions.Add(new ExpectedLogMessage(logLevel, message));

                return this;
            }

            public ILoggerMockSetupSequenceError LogError(string message)
            {
                var logMessage = new ExpectedLogMessage(LogLevel.Error, message);

                this.mock.expectedLogActions.Add(logMessage);

                return new LoggerMockSetupSequenceError(this, logMessage);
            }

            private class LoggerMockSetupSequenceError : ILoggerMockSetupSequenceError
            {
                private readonly ILoggerMockSetupSequence sequence;

                private readonly ExpectedLogMessage logMessage;

                public LoggerMockSetupSequenceError(ILoggerMockSetupSequence sequence, ExpectedLogMessage logMessage)
                {
                    this.sequence = sequence;
                    this.logMessage = logMessage;
                }

                public ILoggerMockSetupSequence BeginScope(object state)
                {
                    return this.sequence.BeginScope(state);
                }

                [ExcludeFromCodeCoverage]
                public ILoggerMockSetupSequence EndScope()
                {
                    return this.sequence.EndScope();
                }

                public ILoggerMockSetupSequence Log(LogLevel logLevel, string message)
                {
                    return this.sequence.Log(logLevel, message);
                }

                public ILoggerMockSetupSequenceError LogError(string message)
                {
                    return this.sequence.LogError(message);
                }

                public ILoggerMockSetupSequence WithException(Action<Exception> exception)
                {
                    this.logMessage.Exception = exception;

                    return this;
                }
            }
        }

        private sealed class LoggerRecorder : ILogger<TCategoryName>
        {
            private readonly LoggerMock<TCategoryName> mock;

            public LoggerRecorder(LoggerMock<TCategoryName> mock)
            {
                this.mock = mock;
            }

            public IDisposable? BeginScope<TState>(TState state)
                where TState : notnull
            {
                var expectedLog = this.GetCurrentExpectedLogAction<ExpectedLogBeginScope>("BeginScope()");

                state.Should().BeEquivalentTo(expectedLog.State);

                this.mock.expectedLogActionsIndex++;

                return new LogScopeDisposable(this);
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                throw new NotSupportedException($"The mock of this method is not supported by the '{typeof(LoggerMock<>).Assembly.GetName().Name}' library.");
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                var expectedLog = this.GetCurrentExpectedLogAction<ExpectedLogMessage>("Log()");

                if (logLevel != expectedLog.LogLevel)
                {
                    Services.ThrowException($"Wrong log level for the Log() method call. (Expected: {expectedLog.LogLevel}, Actual: {logLevel})");
                }

                var message = formatter(state, exception);

                if (message != expectedLog.Message)
                {
                    Services.ThrowException($"Wrong log message for the Log({logLevel}) method call. (Expected: '{expectedLog.Message}', Actual: '{message}')");
                }

                if (expectedLog.Exception != null)
                {
                    if (exception is null)
                    {
                        Services.ThrowException($"Expected an exception but no exeception has been thrown.");
                    }
                    else
                    {
                        expectedLog.Exception(exception);
                    }
                }

                this.mock.expectedLogActionsIndex++;
            }

            private TLogAction GetCurrentExpectedLogAction<TLogAction>(string methodCall)
            {
                if (this.mock.expectedLogActionsIndex >= this.mock.expectedLogActions.Count)
                {
                    Services.ThrowException($"The ILogger has been called too many times (Expected: {this.mock.expectedLogActions.Count} calls)");
                }

                var expectedLogAction = this.mock.expectedLogActions[this.mock.expectedLogActionsIndex];

                if (expectedLogAction is not TLogAction expectedLogActionTyped)
                {
                    Services.ThrowException($"The '{methodCall}' method has been called but expected other action (Expected: {expectedLogAction.Name})");
                    throw new NotImplementedException();
                }

                return expectedLogActionTyped;
            }

            private sealed class LogScopeDisposable : IDisposable
            {
                private readonly LoggerRecorder recorder;

                public LogScopeDisposable(LoggerRecorder recorder)
                {
                    this.recorder = recorder;
                }

                public void Dispose()
                {
                    this.recorder.GetCurrentExpectedLogAction<ExpectedLogEndScope>("Dispose()");

                    this.recorder.mock.expectedLogActionsIndex++;
                }
            }
        }

        private sealed class ExpectedLogMessage : ExpectedLogAction
        {
            public ExpectedLogMessage(LogLevel logLevel, string message)
            {
                this.LogLevel = logLevel;
                this.Message = message;
            }

            public override string Name => "Message";

            public LogLevel LogLevel { get; }

            public string Message { get; }

            public Action<Exception>? Exception { get; set; }

            public override string ExceptionDisplayMessage => $"{this.Name}: ({this.Message})";
        }

        private sealed class ExpectedLogBeginScope : ExpectedLogAction
        {
            public ExpectedLogBeginScope(object state)
            {
                this.State = state;
            }

            public override string Name => "BeginScope";

            public object State { get; }
        }

        private sealed class ExpectedLogEndScope : ExpectedLogAction
        {
            public override string Name => "EndScope";
        }

        private abstract class ExpectedLogAction
        {
            public abstract string Name { get; }

            public virtual string ExceptionDisplayMessage => this.Name;
        }
    }
}
