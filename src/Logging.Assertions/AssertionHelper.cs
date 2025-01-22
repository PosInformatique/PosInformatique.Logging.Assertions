//-----------------------------------------------------------------------
// <copyright file="AssertionHelper.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions
{
    internal static class AssertionHelper
    {
        public static string ToString(object? @object)
        {
            if (@object is null)
            {
                return "null";
            }

            return @object.ToString();
        }

        public static void BeEquivalentTo(object actual, object expected)
        {
            if (!Equals(actual, expected))
            {
                throw new LoggingAssertionFailedException($"Expected value: {ToString(expected)}{Environment.NewLine}Actual value: {ToString(actual)}");
            }
        }

        public static void BeEquivalentTo(IDictionary<string, object> actual, IDictionary<string, object> expected)
        {
            if (actual.Count != expected.Count)
            {
                if (expected.Count > actual.Count)
                {
                    var missingKeys = new List<string>();

                    foreach (var expectedKey in expected.Keys)
                    {
                        if (!actual.ContainsKey(expectedKey))
                        {
                            missingKeys.Add(expectedKey);
                        }
                    }

                    var missingKeysList = string.Join(", ", missingKeys.Select(k => "\"" + k + "\""));

                    throw new LoggingAssertionFailedException($"Expected state to be a dictionary with {expected.Count} item(s), but it misses key(s) {{{missingKeysList}}}");
                }
                else
                {
                    var additionalKeys = new List<string>();

                    foreach (var actualKey in actual.Keys)
                    {
                        if (!expected.ContainsKey(actualKey))
                        {
                            additionalKeys.Add(actualKey);
                        }
                    }

                    var additionalKeysList = string.Join(", ", additionalKeys.Select(k => "\"" + k + "\""));

                    throw new LoggingAssertionFailedException($"Expected state to be a dictionary with {expected.Count} item(s), but has additional key(s) {{{additionalKeysList}}}");
                }
            }
        }

        public static void BeSameAs(Exception actual, Exception expected)
        {
            if (actual.GetType() != expected.GetType())
            {
                throw new LoggingAssertionFailedException($"Expected exception to refer to {expected.GetType().FullName} with message \"{expected.Message}\", but found {actual.GetType().FullName} with message \"{actual.Message}\".");
            }

            if (!actual.Message.Equals(expected.Message, StringComparison.InvariantCulture))
            {
                throw new LoggingAssertionFailedException($"Expected exception to refer to {expected.GetType().FullName} with message \"{expected.Message}\", but found {actual.GetType().FullName} with message \"{actual.Message}\".");
            }
        }
    }
}
