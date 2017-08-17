namespace Serviceable.Objects.Tests
{
    using System;
    using Serviceable.Objects.Exceptions;
    using Xunit;

    public class CheckTest
    {
        private const string ExceptionPrefix = "exception: ";

        private const string ExceptionMessage = "Exception string triggered";

        [Fact]
        public void ArgumentNull_WhenValidatingNull_ThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() => CallMethodWithCheck(null));
        }

        [Fact]
        public void ArgumentNull_WhenValidationPasses_ThenDoesNotThrows()
        {
            CallMethodWithCheck("do not throw man.");
        }

        [Fact]
        public void ArgumentNull_WhenValidatingConditionMet_ThenItThrows()
        {
            Assert.Throws<ArgumentException>(() => CallMethodWithCheck(ExceptionPrefix + "something here"));
        }

        private void CallMethodWithCheck(string arg)
        {
            Check.ArgumentNull(arg, nameof(arg));
            Check.Argument(arg.StartsWith(ExceptionPrefix), nameof(arg), ExceptionMessage);
        }
    }
}
