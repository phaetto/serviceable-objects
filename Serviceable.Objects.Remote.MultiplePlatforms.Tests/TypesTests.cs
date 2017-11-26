namespace Serviceable.Objects.Remote.MultiplePlatforms.Tests
{
    using Dependencies;
    using Objects.Composition;
    using Xunit;

    public class TypesTests
    {
        [Fact]
        public void FindType_WhenFindingTypeWithAssemblyQualifiedName_ThenTypeIsFound()
        {
            var typeAssemblyQualifiedName = typeof(ContextGraph).AssemblyQualifiedName;

            var type = Types.FindType(typeAssemblyQualifiedName);

            Assert.Equal(typeof(ContextGraph), type);
        }

        [Fact]
        public void FindType_WhenFindingTypeWithFullName_ThenTypeIsFound()
        {
            var typeFullName = typeof(ContextGraph).FullName;

            var type = Types.FindType(typeFullName);

            Assert.Equal(typeof(ContextGraph), type);
        }
    }
}