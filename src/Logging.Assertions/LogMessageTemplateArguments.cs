//-----------------------------------------------------------------------
// <copyright file="LogMessageTemplateArguments.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions
{
    using System.Collections;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Contains the message template arguments of the <see cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/>
    /// which are accessible by the delegate specified on the <see cref="ILoggerMockSetupSequenceLog.WithArguments(int, Action{LogMessageTemplateArguments})"/>
    /// to assert the message template arguments.
    /// </summary>
    public sealed class LogMessageTemplateArguments : IEnumerable<object?>
    {
        private readonly IReadOnlyList<KeyValuePair<string, object?>> arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessageTemplateArguments"/> class.
        /// </summary>
        /// <param name="arguments">Arguments to assert.</param>
        internal LogMessageTemplateArguments(IEnumerable<KeyValuePair<string, object?>> arguments)
        {
            this.arguments = arguments.ToArray();
        }

        /// <summary>
        /// Gets the number of message template arguments.
        /// </summary>
        public int Count => this.arguments.Count;

        /// <summary>
        /// Gets the message template argument value by his key.
        /// </summary>
        /// <param name="key">Template argument name to retrieve the value.</param>
        /// <returns>The message template argument value by his key.</returns>
        /// <exception cref="KeyNotFoundException">If the <paramref name="key"/> argument name has not been found.</exception>
        public object? this[string key]
        {
            get
            {
                var valueFound = this.arguments.SingleOrDefault(kv => kv.Key == key);

                if (valueFound.Equals(default(KeyValuePair<string, object?>)))
                {
                    throw new KeyNotFoundException($"The given message template argument '{key}' was not present.");
                }

                return valueFound.Value;
            }
        }

        /// <summary>
        /// Gets the message template argument value by his index position.
        /// </summary>
        /// <param name="index">Template argument index position to retrieve the value.</param>
        /// <returns>The message template argument value at the specified <paramref name="index"/> position.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="index"/> is out of the range of the template message arguments list.</exception>
        public object? this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                if (index >= this.arguments.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return this.arguments[index].Value;
            }
        }

        /// <inheritdoc />
        public IEnumerator<object?> GetEnumerator()
        {
            return this.arguments.Select(kv => kv.Value).GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
