//-----------------------------------------------------------------------
// <copyright file="LoggingAssertionFailedExceptionTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions.Tests
{
    using FluentAssertions;
    using Xunit;

    public class LoggingAssertionFailedExceptionTest
    {
        [Fact]
        public void Constructor()
        {
            var exception = new LoggingAssertionFailedException();

            exception.Message.Should().Be("Exception of type 'PosInformatique.Logging.Assertions.LoggingAssertionFailedException' was thrown.");
            exception.InnerException.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithMessage()
        {
            var exception = new LoggingAssertionFailedException("The message");

            exception.Message.Should().Be("The message");
            exception.InnerException.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithMessageAndInnerException()
        {
            var innerException = new FormatException("The inner exception");
            var exception = new LoggingAssertionFailedException("The message", innerException);

            exception.Message.Should().Be("The message");
            exception.InnerException.Should().BeSameAs(innerException);
        }
    }
}