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

        [Fact]
        public void FindType_WhenFindingTypeWithFullName_ThenAnExceptionIsThrown()
        {
            Types.TypeCache.Clear();

            var typeFullName = typeof(GraphContext).FullName;

            Assert.Throws<InvalidOperationException>(() => Types.FindType(typeFullName));
        }
    }
}