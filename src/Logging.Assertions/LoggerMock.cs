//-----------------------------------------------------------------------
// <copyright file="LoggerMock.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions
{
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
    public class LoggerMock
    {
        private readonly IList<ExpectedLogAction> expectedLogActions;

        private int expectedLogActionsIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerMock"/> class.
        /// </summary>
        public LoggerMock()
        {
            this.expectedLogActions = new List<ExpectedLogAction>();
            this.Object = this.CreateRecorder<object>();
        }

        /// <summary>
        /// Gets the mocked instance of the <see cref="ILogger"/>.
        /// </summary>
        public ILogger Object { get; }

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

        /// <summary>
        /// Creates an instance of <see cref="LoggerRecorder{TCategoryName}"/> to record <see cref="ILogger"/> trace.
        /// </summary>
        /// <typeparam name="TCategoryName">Type of the category name to use for <see cref="ILogger{TCategoryName}"/>.</typeparam>
        /// <returns>An instance of <see cref="LoggerRecorder{TCategoryName}"/> to record <see cref="ILogger"/> trace.</returns>
        private protected virtual ILogger CreateRecorder<TCategoryName>()
        {
            return new LoggerRecorder<TCategoryName>(this);
        }

        private sealed class LoggerMockSetupSequence : ILoggerMockSetupSequence
        {
            private readonly LoggerMock mock;

            public LoggerMockSetupSequence(LoggerMock mock)
            {
                this.mock = mock;
            }

            public ILoggerMockSetupSequence BeginScope<TState>(Action<TState> state)
            {
                var logBeginScope = new ExpectedLogBeginScope<TState>(this, state);

                this.mock.expectedLogActions.Add(logBeginScope);

                return logBeginScope;
            }

            public ILoggerMockSetupSequence EndScope()
            {
                var logEndScope = new ExpectedLogEndScope(this);

                this.mock.expectedLogActions.Add(logEndScope);

                return logEndScope;
            }

            public ILoggerMockSetupSequenceLog Log(LogLevel logLevel, string message)
            {
                var logMessage = new ExpectedLogMessage(this, logLevel, message);

                this.mock.expectedLogActions.Add(logMessage);

                return logMessage;
            }

            public ILoggerMockSetupSequenceError LogError(string message)
            {
                var logMessage = new ExpectedLogMessage(this, LogLevel.Error, message);

                this.mock.expectedLogActions.Add(logMessage);

                return logMessage;
            }
        }

        private sealed class LoggerRecorder<TCategoryName> : ILogger, ILogger<TCategoryName>
        {
            private readonly LoggerMock mock;

            public LoggerRecorder(LoggerMock mock)
            {
                this.mock = mock;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                var expectedLog = this.GetCurrentExpectedLogAction<ExpectedLogBeginScope>("BeginScope()");

                expectedLog.Assert(state);

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

                expectedLog.Assert(logLevel, state, exception, formatter);

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
                    throw new NotImplementedException("Must not be called.");
                }

                return expectedLogActionTyped;
            }

            private sealed class LogScopeDisposable : IDisposable
            {
                private readonly LoggerRecorder<TCategoryName> recorder;

                public LogScopeDisposable(LoggerRecorder<TCategoryName> recorder)
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

        private sealed class ExpectedLogMessage : ExpectedLogAction, ILoggerMockSetupSequenceError
        {
            private readonly LogLevel logLevel;

            private readonly string message;

            private Action<Exception>? exception;

            private Arguments? arguments;

            public ExpectedLogMessage(LoggerMockSetupSequence sequence, LogLevel logLevel, string message)
                : base(sequence)
            {
                this.logLevel = logLevel;
                this.message = message;
            }

            public override string Name => "Message";

            public override string ExceptionDisplayMessage => $"{this.Name}: ({this.message})";

            public ILoggerMockSetupSequence WithArguments(int count, Action<LogMessageTemplateArguments> arguments)
            {
                this.arguments = new Arguments(count, arguments);

                return this;
            }

            public ILoggerMockSetupSequenceLog WithException(Action<Exception> exception)
            {
                this.exception = exception;

                return this;
            }

            public void Assert<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                if (logLevel != this.logLevel)
                {
                    Services.ThrowException($"Wrong log level for the Log() method call. (Expected: {this.logLevel}, Actual: {logLevel})");
                }

                if (this.arguments != null)
                {
                    var stateAsList = (IReadOnlyList<KeyValuePair<string, object?>>)state!;

                    var originalMessage = stateAsList.Single(kv => kv.Key == "{OriginalFormat}");

                    if (stateAsList.Count - 1 != this.arguments.Count)
                    {
                        Services.ThrowException($"Incorrect template message argument count for the '{originalMessage.Value}' template message. (Expected: '{this.arguments.Count}', Actual: '{stateAsList.Count - 1}')");
                    }

                    var messageArguments = new LogMessageTemplateArguments(
                        stateAsList.Where(kv => kv.Key != "{OriginalFormat}").ToDictionary(kv => kv.Key, kv => kv.Value));

                    this.arguments.Action(messageArguments);
                }
                else
                {
                    var message = formatter(state, exception);

                    if (message != this.message)
                    {
                        Services.ThrowException($"Wrong log message for the Log({logLevel}) method call. (Expected: '{this.message}', Actual: '{message}')");
                    }
                }

                if (this.exception != null)
                {
                    if (exception is null)
                    {
                        Services.ThrowException($"Expected an exception but no exeception has been thrown.");
                    }
                    else
                    {
                        this.exception(exception);
                    }
                }
            }

            private sealed class Arguments
            {
                public Arguments(int count, Action<LogMessageTemplateArguments> action)
                {
                    this.Count = count;
                    this.Action = action;
                }

                public int Count { get; }

                public Action<LogMessageTemplateArguments> Action { get; }
            }
        }

        private sealed class ExpectedLogBeginScope<TExpectedState> : ExpectedLogBeginScope
        {
            private readonly Action<TExpectedState> assert;

            public ExpectedLogBeginScope(LoggerMockSetupSequence sequence, Action<TExpectedState> state)
                : base(sequence)
            {
                this.assert = state;
            }

            public override void Assert<TState>(TState state)
            {
                if (state is TExpectedState expectedState)
                {
                    this.assert(expectedState);
                }
                else
                {
                    Services.ThrowException($"The 'BeginScope()' has been called with a wrong state argument type (Expected: {typeof(TExpectedState).Name}, Actual: {typeof(TState).Name}).");
                }
            }
        }

        private abstract class ExpectedLogBeginScope : ExpectedLogAction
        {
            protected ExpectedLogBeginScope(LoggerMockSetupSequence sequence)
                : base(sequence)
            {
            }

            public override string Name => "BeginScope";

            public abstract void Assert<TState>(TState state);
        }

        private sealed class ExpectedLogEndScope : ExpectedLogAction
        {
            public ExpectedLogEndScope(LoggerMockSetupSequence sequence)
                : base(sequence)
            {
            }

            public override string Name => "EndScope";
        }

        private abstract class ExpectedLogAction : ILoggerMockSetupSequence
        {
            private readonly LoggerMockSetupSequence sequence;

            protected ExpectedLogAction(LoggerMockSetupSequence sequence)
            {
                this.sequence = sequence;
            }

            public abstract string Name { get; }

            public virtual string ExceptionDisplayMessage => this.Name;

            public ILoggerMockSetupSequence BeginScope<TState>(Action<TState> state)
            {
                return this.sequence.BeginScope(state);
            }

            public ILoggerMockSetupSequence EndScope()
            {
                return this.sequence.EndScope();
            }

            public ILoggerMockSetupSequenceLog Log(LogLevel logLevel, string message)
            {
                return this.sequence.Log(logLevel, message);
            }

            public ILoggerMockSetupSequenceError LogError(string message)
            {
                return this.sequence.LogError(message);
            }
        }
    }
}
