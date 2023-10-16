//-----------------------------------------------------------------------
// <copyright file="LogMessageTemplateArgumentsTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions.Tests
{
    using System.Collections;
    using FluentAssertions;
    using Xunit;

    public class LogMessageTemplateArgumentsTest
    {
        [Fact]
        public void Count()
        {
            var list = new List<KeyValuePair<string, object?>>()
            {
                KeyValuePair.Create<string, object?>("A", "The A"),
                KeyValuePair.Create<string, object?>("B", 1234),
                KeyValuePair.Create<string, object?>("C", null),
            };

            var arguments = new LogMessageTemplateArguments(list);

            arguments.Count.Should().Be(3);
        }

        [Fact]
        public void Indexer_ByIndex()
        {
            var list = new List<KeyValuePair<string, object?>>()
            {
                KeyValuePair.Create<string, object?>("A", "The A"),
                KeyValuePair.Create<string, object?>("B", 1234),
                KeyValuePair.Create<string, object?>("C", null),
            };

            var arguments = new LogMessageTemplateArguments(list);

            arguments[0].Should().Be("The A");
            arguments[1].Should().Be(1234);
            arguments[2].Should().BeNull();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(3)]
        [InlineData(100)]
        public void Indexer_ByIndex_OutOfRange(int index)
        {
            var list = new List<KeyValuePair<string, object?>>()
            {
                KeyValuePair.Create<string, object?>("A", "The A"),
                KeyValuePair.Create<string, object?>("B", 1234),
                KeyValuePair.Create<string, object?>("C", null),
            };

            var arguments = new LogMessageTemplateArguments(list);

            var act = () =>
            {
                var unused = arguments[index];
            };

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("index");
        }

        [Fact]
        public void Indexer_ByKey()
        {
            var list = new List<KeyValuePair<string, object?>>()
            {
                KeyValuePair.Create<string, object?>("A", "The A"),
                KeyValuePair.Create<string, object?>("B", 1234),
                KeyValuePair.Create<string, object?>("C", null),
            };

            var arguments = new LogMessageTemplateArguments(list);

            arguments["A"].Should().Be("The A");
            arguments["B"].Should().Be(1234);
            arguments["C"].Should().BeNull();
        }

        [Fact]
        public void Indexer_ByKey_KeyNotFound()
        {
            var list = new List<KeyValuePair<string, object?>>()
            {
                KeyValuePair.Create<string, object?>("A", "The A"),
                KeyValuePair.Create<string, object?>("B", 1234),
                KeyValuePair.Create<string, object?>("C", null),
            };

            var arguments = new LogMessageTemplateArguments(list);

            var act = () =>
            {
                var unused = arguments["D"];
            };

            act.Should().Throw<KeyNotFoundException>()
                .WithMessage("The given message template argument 'D' was not present.");
        }

        [Fact]
        public void GetEnumerator()
        {
            var list = new List<KeyValuePair<string, object?>>()
            {
                KeyValuePair.Create<string, object?>("A", "The A"),
                KeyValuePair.Create<string, object?>("B", 1234),
                KeyValuePair.Create<string, object?>("C", null),
            };

            var arguments = new LogMessageTemplateArguments(list);

            var enumerator = arguments.GetEnumerator();

            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().Be("The A");

            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().Be(1234);

            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().BeNull();

            enumerator.MoveNext().Should().BeFalse();
            enumerator.MoveNext().Should().BeFalse();
        }

        [Fact]
        public void GetEnumerator_NoGeneric()
        {
            var list = new List<KeyValuePair<string, object?>>()
            {
                KeyValuePair.Create<string, object?>("A", "The A"),
                KeyValuePair.Create<string, object?>("B", 1234),
                KeyValuePair.Create<string, object?>("C", null),
            };

            var arguments = new LogMessageTemplateArguments(list);

            var enumerator = ((IEnumerable)arguments).GetEnumerator();

            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().Be("The A");

            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().Be(1234);

            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().BeNull();

            enumerator.MoveNext().Should().BeFalse();
            enumerator.MoveNext().Should().BeFalse();
        }
    }
}
