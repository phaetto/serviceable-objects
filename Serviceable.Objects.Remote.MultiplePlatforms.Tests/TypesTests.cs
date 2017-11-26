namespace Serviceable.Objects.Remote.MultiplePlatforms.Tests
{
    using System;
    using Dependencies;
    using Objects.Composition.Graph;
    using Xunit;

    public class TypesTests
    {
        [Fact]
        public void FindType_WhenFindingTypeWithAssemblyQualifiedName_ThenTypeIsFound()
        {
            Types.TypeCache.Clear();

            var typeAssemblyQualifiedName = typeof(GraphContext).AssemblyQualifiedName;

            var type = Types.FindType(typeAssemblyQualifiedName);

            Assert.Equal(typeof(GraphContext), type);
        }

#if DOTNETSTANDARD_13
        [Fact]
        public void FindType_WhenFindingTypeWithFullName_ThenAnExceptionIsThrown()
        {
            Types.TypeCache.Clear();

            var typeFullName = typeof(GraphContext).FullName;

            Assert.Throws<InvalidOperationException>(() => Types.FindType(typeFullName));
        }
#else
        [Fact]
        public void FindType_WhenFindingTypeWithFullName_ThenTypeIsFound()
        {
            Types.TypeCache.Clear();

            var typeFullName = typeof(GraphContext).FullName;

            var type = Types.FindType(typeFullName);

            Assert.Equal(typeof(GraphContext), type);
        }
#endif
    }
}