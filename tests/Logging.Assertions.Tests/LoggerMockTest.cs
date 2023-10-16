﻿//-----------------------------------------------------------------------
// <copyright file="LoggerMockTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions.Tests
{
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Xunit;
    using Xunit.Sdk;

    public class LoggerMockTest
    {
        [Fact]
        public void VerifyAllLogs()
        {
            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogTrace("Log Trace 1")
                .LogDebug("Log Debug 2")
                .LogInformation("Log Information 3")
                .LogWarning("Log Warning 4")
                .LogError("Log Error 5");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.Invoke();

            logger.VerifyLogs();
        }

        [Fact]
        public void VerifyAllLogs_MissingLogs()
        {
            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogTrace("Log Trace 1")
                .LogDebug("Log Debug 2")
                .LogInformation("Log Information 3")
                .LogWarning("Log Warning 4")
                .LogError("Log Error 5")
                .LogInformation("Missing log Information")
                .LogError("Missing log Error")
                .BeginScope(new { })
                .EndScope();

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.Invoke();

            logger.Invoking(l => l.VerifyLogs())
                .Should().ThrowExactly<XunitException>()
                .WithMessage(@"Logger has been called few times (Expected: 9 calls, Actual: 5 calls).
- Message: (Missing log Information)
- Message: (Missing log Error)
- BeginScope
- EndScope");
        }

        [Fact]
        public void LogTraceFailed()
        {
            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogTrace("Log Trace expected")
                .LogDebug("Log Debug 2")
                .LogInformation("Log Information 3")
                .LogWarning("Log Warning 4")
                .LogError("Log Error 5");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.Invoking(o => o.Invoke())
                .Should().ThrowExactly<XunitException>()
                .WithMessage("Wrong log message for the Log(Trace) method call. (Expected: 'Log Trace expected', Actual: 'Log Trace 1')");
        }

        [Fact]
        public void LogDebugFailed()
        {
            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogTrace("Log Trace 1")
                .LogDebug("Log Debug expected")
                .LogInformation("Log Information 3")
                .LogWarning("Log Warning 4")
                .LogError("Log Error 5");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.Invoking(o => o.Invoke())
                .Should().ThrowExactly<XunitException>()
                .WithMessage("Wrong log message for the Log(Debug) method call. (Expected: 'Log Debug expected', Actual: 'Log Debug 2')");
        }

        [Fact]
        public void LogInformationFailed()
        {
            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogTrace("Log Trace 1")
                .LogDebug("Log Debug 2")
                .LogInformation("Log Information expected")
                .LogWarning("Log Warning 3")
                .LogError("Log Error 4");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.Invoking(o => o.Invoke())
                .Should().ThrowExactly<XunitException>()
                .WithMessage("Wrong log message for the Log(Information) method call. (Expected: 'Log Information expected', Actual: 'Log Information 3')");
        }

        [Fact]
        public void LogWarningFailed()
        {
            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogTrace("Log Trace 1")
                .LogDebug("Log Debug 2")
                .LogInformation("Log Information 3")
                .LogWarning("Log Warning expected")
                .LogError("Log Error 5");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.Invoking(o => o.Invoke())
                .Should().ThrowExactly<XunitException>()
                .WithMessage("Wrong log message for the Log(Warning) method call. (Expected: 'Log Warning expected', Actual: 'Log Warning 4')");
        }

        [Fact]
        public void LogErrorFailed()
        {
            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogTrace("Log Trace 1")
                .LogDebug("Log Debug 2")
                .LogInformation("Log Information 3")
                .LogWarning("Log Warning 4")
                .LogError("Log Error expected");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.Invoking(o => o.Invoke())
                .Should().ThrowExactly<XunitException>()
                .WithMessage("Wrong log message for the Log(Error) method call. (Expected: 'Log Error expected', Actual: 'Log Error 5')");
        }

        [Fact]
        public void LogError_WithException_SameObjectReference()
        {
            var exception = new FormatException("The exception");

            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogInformation("Log before error")
                .LogError("Log Error")
                    .WithException(exception)
                .LogTrace("Log Trace after error")
                .LogDebug("Log Debug after error")
                .LogInformation("Log Information after error")
                .LogWarning("Log Warning after error")
                .LogError("Log Error after error");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.InvokeWithException(exception);

            logger.VerifyLogs();
        }

        [Fact]
        public void LogError_WithException_WithActionDelegate()
        {
            var exception = new FormatException("The exception");

            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogInformation("Log before error")
                .LogError("Log Error")
                    .WithException(e => e.Should().BeSameAs(exception))
                .LogTrace("Log Trace after error")
                .LogDebug("Log Debug after error")
                .LogInformation("Log Information after error")
                .LogWarning("Log Warning after error")
                .LogError("Log Error after error");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.InvokeWithException(exception);

            logger.VerifyLogs();
        }

        [Fact]
        public void LogError_WithException_ExceptionDifferent()
        {
            var exception = new FormatException("The exception");

            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogInformation("Log before error")
                .LogError("Log Error")
                    .WithException(new DivideByZeroException("Other exception"))
                .LogTrace("Log Trace after error")
                .LogDebug("Log Debug after error")
                .LogInformation("Log Information after error")
                .LogWarning("Log Warning after error")
                .LogError("Log Error expected after error");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.Invoking(o => o.InvokeWithException(exception))
                .Should().ThrowExactly<XunitException>()
                .WithMessage("Expected actualException to refer to System.DivideByZeroException with message \"Other exception\", but found System.FormatException with message \"The exception\".");
        }

        [Fact]
        public void LogError_ExpectedExceptionButNoExceptionRaised()
        {
            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogTrace("Log Trace 1")
                .LogDebug("Log Debug 2")
                .LogInformation("Log Information 3")
                .LogWarning("Log Warning 4")
                .LogError("Log Error 5")
                    .WithException(new DivideByZeroException("Other exception"));

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.Invoking(o => o.Invoke())
                .Should().ThrowExactly<XunitException>()
                .WithMessage("Expected an exception but no exeception has been thrown.");
        }

        [Fact]
        public void LogWrongLevel()
        {
            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogWarning("Log Trace 1");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.Invoking(o => o.Invoke())
                .Should().ThrowExactly<XunitException>()
                .WithMessage("Wrong log level for the Log() method call. (Expected: Warning, Actual: Trace)");
        }

        [Fact]
        public void LogWithMessageTemplate_DelegateAssertion()
        {
            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogInformation("Log information with parameters {Id}, {Name} and {Object}")
                    .WithArguments(3, args =>
                    {
                        args["Id"].Should().Be(1234);
                        args["Name"].Should().Be("The name");
                        args["Object"].Should().BeEquivalentTo(new { Property = "I am object" });
                    })
                .LogError("Log error after message template");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.InvokeWithMessageTemplate();

            logger.VerifyLogs();
        }

        [Fact]
        public void LogWithMessageTemplate_DelegateAssertion_WrongExpectedArgumentCount()
        {
            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogInformation("Log information with parameters {Id}, {Name} and {Object}")
                    .WithArguments(100, args =>
                    {
                        args["Id"].Should().Be(1234);
                        args["Name"].Should().Be("The name");
                        args["Object"].Should().BeEquivalentTo(new { Property = "I am object" });
                    })
                .LogError("Log error after message template");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.Invoking(o => o.InvokeWithMessageTemplate())
                .Should().ThrowExactly<XunitException>()
                .WithMessage("Incorrect template message argument count for the 'Log information with parameters {Id}, {Name} and {Object}' template message. (Expected: '100', Actual: '4')");
        }

        [Fact]
        public void LogWithMessageTemplate_ParametersAssertion()
        {
            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogInformation("Log information with parameters {Id}, {Name} and {Object}")
                    .WithArguments(1234, "The name", new { Property = "I am object" })
                .LogError("Log error after message template");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.InvokeWithMessageTemplate();

            logger.VerifyLogs();
        }

        [Fact]
        public void LogWithMessageTemplate_ParametersAssertion_WrongExpectedArgumentCount()
        {
            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogInformation("Log information with parameters {Id}, {Name} and {Object}")
                    .WithArguments(1, 2, 3, 4, 5, 6, 7, "The name", new { Property = "I am object" })
                .LogError("Log error after message template");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.Invoking(o => o.InvokeWithMessageTemplate())
                .Should().ThrowExactly<XunitException>()
                .WithMessage("Incorrect template message argument count for the 'Log information with parameters {Id}, {Name} and {Object}' template message. (Expected: '9', Actual: '4')");
        }

        [Fact]
        public void LoggerCalledToManyTimes()
        {
            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogTrace("Log Trace 1")
                .LogDebug("Log Debug 2")
                .LogInformation("Log Information 3");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.Invoking(o => o.Invoke())
                .Should().ThrowExactly<XunitException>()
                .WithMessage("The ILogger has been called too many times (Expected: 3 calls)");
        }

        [Fact]
        public void VerifyAllLogs_WithScopes()
        {
            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogTrace("Log Trace 1")
                .BeginScope(new { ScopeLevel = 1, ScopeName = "Scope level 1" })
                    .LogDebug("Log Debug 2")
                    .BeginScope(new { ScopeLevel = 2, ScopeName = "Scope level 2" })
                        .LogInformation("Log Information 3")
                    .EndScope()
                    .LogWarning("Log Warning 4")
                .EndScope()
                .LogError("Log Error 5");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.InvokeWithScope();

            logger.VerifyLogs();
        }

        [Fact]
        public void VerifyAllLogs_WithScopes_BeginScopeExpected()
        {
            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogTrace("Log Trace 1")
                .LogDebug("Log Debug 2");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.Invoking(o => o.InvokeWithScope())
                .Should().ThrowExactly<XunitException>()
                .WithMessage("The 'BeginScope()' method has been called but expected other action (Expected: Message)");
        }

        [Fact]
        public void VerifyAllLogs_WithScopes_EndScopeExpected()
        {
            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogTrace("Log Trace 1")
                .BeginScope(new { ScopeLevel = 1, ScopeName = "Scope level 1" })
                    .LogDebug("Log Debug 2")
                    .BeginScope(new { ScopeLevel = 2, ScopeName = "Scope level 2" })
                        .LogInformation("Log Information 3")
                    .EndScope()
                    .LogWarning("Log Warning 4")
                .LogError("Log Error 5");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.Invoking(o => o.InvokeWithScope())
                .Should().ThrowExactly<XunitException>()
                .WithMessage("The 'Dispose()' method has been called but expected other action (Expected: Message)");
        }

        [Fact]
        public void IsEnabled_NotSupported()
        {
            var logger = new LoggerMock<ObjectToLog>();

            logger.Object.Invoking(l => l.IsEnabled(LogLevel.Debug))
                .Should().ThrowExactly<NotSupportedException>()
                .WithMessage("The mock of this method is not supported by the 'PosInformatique.Logging.Assertions' library.");
        }

        [Fact]
        public void LogError_WithException_ChainedWithOtherSequence()
        {
            var exception = new FormatException("The exception");

            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogError("Log Error")
                    .WithException(exception)
                .BeginScope(new { Scope = "Scope 1" })
                    .LogTrace("Log Trace after error")
                    .LogDebug("Log Debug after error")
                    .LogInformation("Log Information after error")
                .EndScope()
                .LogWarning("Log Warning after error")
                .LogError("Log Error after error");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.InvokeWithExceptionChainedWithOtherSequence(exception);

            logger.VerifyLogs();
        }

        [Fact]
        public void LogError_WithException_ChainedWithLogError()
        {
            var exception = new FormatException("The exception");

            var logger = new LoggerMock<ObjectToLog>();
            logger.SetupSequence()
                .LogError("Log Error")
                    .WithException(exception)
                .LogError("Log Error after error");

            var objectToLog = new ObjectToLog(logger.Object);

            objectToLog.InvokeWithExceptionChainedWithLogError(exception);

            logger.VerifyLogs();
        }

        private class ObjectToLog
        {
            private readonly ILogger<ObjectToLog> logger;

            public ObjectToLog(ILogger<ObjectToLog> logger)
            {
                this.logger = logger;
            }

            public void Invoke()
            {
                this.logger.LogTrace("Log Trace {0}", 1);
                this.logger.LogDebug("Log Debug {0}", 2);
                this.logger.LogInformation("Log Information {0}", 3);
                this.logger.LogWarning("Log Warning {0}", 4);
                this.logger.LogError("Log Error {0}", 5);
            }

            public void InvokeWithException(Exception exception)
            {
                this.logger.LogInformation("Log before error");
                this.logger.LogError(exception, "Log Error");
                this.logger.LogTrace("Log Trace after error");
                this.logger.LogDebug("Log Debug after error");
                this.logger.LogInformation("Log Information after error");
                this.logger.LogWarning("Log Warning after error");
                this.logger.LogError("Log Error after error");
            }

            public void InvokeWithExceptionChainedWithOtherSequence(Exception exception)
            {
                this.logger.LogError(exception, "Log Error");

                using (var scope = this.logger.BeginScope(new { Scope = "Scope 1" }))
                {
                    this.logger.LogTrace("Log Trace after error");
                    this.logger.LogDebug("Log Debug after error");
                    this.logger.LogInformation("Log Information after error");
                }

                this.logger.LogWarning("Log Warning after error");
                this.logger.LogError("Log Error after error");
            }

            public void InvokeWithExceptionChainedWithLogError(Exception exception)
            {
                this.logger.LogError(exception, "Log Error");
                this.logger.LogError("Log Error after error");
            }

            public void InvokeWithScope()
            {
                this.logger.LogTrace("Log Trace {0}", 1);

                using (var scope1 = this.logger.BeginScope(new { ScopeLevel = 1, ScopeName = "Scope level 1" }))
                {
                    this.logger.LogDebug("Log Debug {0}", 2);

                    using (var scope2 = this.logger.BeginScope(new { ScopeLevel = 2, ScopeName = "Scope level 2" }))
                    {
                        this.logger.LogInformation("Log Information {0}", 3);
                    }

                    this.logger.LogWarning("Log Warning {0}", 4);
                }

                this.logger.LogError("Log Error {0}", 5);
            }

            public void InvokeWithMessageTemplate()
            {
                this.logger.LogInformation("Log information with parameters {Id}, {Name} and {Object}", 1234, "The name", new { Property = "I am object" });
                this.logger.LogError("Log error after message template");
            }
        }
    }
}
