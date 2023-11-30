//-----------------------------------------------------------------------
// <copyright file="ExceptionHelperTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions.Tests
{
    using FluentAssertions;
    using Xunit;

    public class ExceptionHelperTest
    {
        [Fact]
        public void IsExceptionOnGoing()
        {
            ExceptionHelper.IsExceptionOnGoing().Should().BeFalse();

            try
            {
                ExceptionHelper.IsExceptionOnGoing().Should().BeFalse();

                throw new Exception("Oups...");
            }
            catch
            {
                ExceptionHelper.IsExceptionOnGoing().Should().BeTrue();
            }

            ExceptionHelper.IsExceptionOnGoing().Should().BeFalse();
        }
    }
}