namespace Serviceable.Objects.Remote.Tests
{
    using Dependencies;
    using Xunit;

    public class CreateObjectTests
    {
        [Fact]
        public void CreateObjectWithParameters_WhenInjectionIsUsed_ThenItemIsBeenSetCorrectly()
        {
            var createdObject = Types.CreateObjectWithParametersAndInjection(
                typeof(WantedClass).AssemblyQualifiedName,
                new object[]
                {
                    1, "one"
                },
                new object[]
                {
                    new InjectedClass()
                }) as WantedClass;

            Assert.NotNull(createdObject);
            Assert.IsType<WantedClass>(createdObject);
            Assert.IsType<InjectedClass>(createdObject.InjectedClass);
        }

        [Fact]
        public void CreateObjectWithParameters_WhenInjectionIsUsedWithWrongOrder_ThenItemIsBeenSetCorrectly()
        {
            var createdObject = Types.CreateObjectWithParametersAndInjection(
                typeof(WantedClass).AssemblyQualifiedName,
                new object[]
                {
                    1, "one"
                },
                new object[]
                {
                    new InjectedClass2(),
                    new InjectedClass()
                }) as WantedClass;

            Assert.NotNull(createdObject);
            Assert.IsType<WantedClass>(createdObject);
            Assert.IsType<InjectedClass>(createdObject.InjectedClass);
            Assert.IsType<InjectedClass2>(createdObject.InjectedClass2);
        }

        [Fact]
        public void CreateObjectWithParameters_WhenArrayObjectIsUsed_ThenItemIsBeenSetCorrectly()
        {
            var createdObject = Types.CreateObjectWithParametersAndInjection(
                typeof(WantedClass).AssemblyQualifiedName,
                new object[]
                {
                    1, new[] { "one" }
                },
                new object[]
                {
                    new InjectedClass2(),
                    new InjectedClass()
                }) as WantedClass;

            Assert.NotNull(createdObject);
            Assert.IsType<WantedClass>(createdObject);
            Assert.IsType<InjectedClass>(createdObject.InjectedClass);
        }

        [Fact]
        public void CreateObjectWithParameters_WhenDefaultOptionIsUsed_ThenParameterIsBeenSetCorrectly()
        {
            var createdObject = Types.CreateObjectWithParametersAndInjection(
                typeof(WantedClass).AssemblyQualifiedName,
                new object[]
                {
                    1
                },
                new object[]
                {
                    new InjectedClass2(),
                    new InjectedClass()
                }) as WantedClass;

            Assert.NotNull(createdObject);
            Assert.Equal("default", createdObject.Astring);
            Assert.IsType<WantedClass>(createdObject);
            Assert.Null(createdObject.InjectedClass);
        }

        public class InjectedClass
        {
        }

        public class InjectedClass2
        {
        }

        public class WantedClass
        {
            public readonly bool Abool;
            public readonly string Astring;
            public readonly string[] Astrings;
            public readonly InjectedClass InjectedClass;
            public readonly InjectedClass2 InjectedClass2;

            public WantedClass(bool abool, string[] astrings, InjectedClass injectedClass)
            {
                Abool = abool;
                Astrings = astrings;
                InjectedClass = injectedClass;
            }

            public WantedClass(bool abool, string astring = "default")
            {
                Abool = abool;
                Astring = astring;
            }

            public WantedClass(bool abool, string astring, InjectedClass injectedClass)
            {
                Abool = abool;
                Astring = astring;
                InjectedClass = injectedClass;
            }

            public WantedClass(bool abool, string astring, InjectedClass injectedClass = null, InjectedClass2 injectedClass2 = null)
            {
                Abool = abool;
                Astring = astring;
                InjectedClass = injectedClass;
                InjectedClass2 = injectedClass2;
            }
        }
    }
}
