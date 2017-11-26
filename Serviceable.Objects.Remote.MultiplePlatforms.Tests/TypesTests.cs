namespace Serviceable.Objects.Remote.MultiplePlatforms.Tests
{
    using System;
    using Dependencies;
    using Objects.Composition;
    using Xunit;

    public class TypesTests
    {
        [Fact]
        public void FindType_WhenFindingTypeWithAssemblyQualifiedName_ThenTypeIsFound()
        {
            Types.TypeCache.Clear();

            var typeAssemblyQualifiedName = typeof(ContextGraph).AssemblyQualifiedName;

            var type = Types.FindType(typeAssemblyQualifiedName);

            Assert.Equal(typeof(ContextGraph), type);
        }

#if DOTNETSTANDARD_13
        [Fact]
        public void FindType_WhenFindingTypeWithFullName_ThenAnExceptionIsThrown()
        {
            Types.TypeCache.Clear();

            var typeFullName = typeof(ContextGraph).FullName;

            Assert.Throws<InvalidOperationException>(() => Types.FindType(typeFullName));
        }
#else
        [Fact]
        public void FindType_WhenFindingTypeWithFullName_ThenTypeIsFound()
        {
            Types.TypeCache.Clear();

            var typeFullName = typeof(ContextGraph).FullName;

            var type = Types.FindType(typeFullName);

            Assert.Equal(typeof(ContextGraph), type);
        }
#endif
    }
}