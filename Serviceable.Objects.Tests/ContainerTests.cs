﻿// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedField.Local

namespace Serviceable.Objects.Tests
{
    using System;
    using System.Collections.Generic;
    using Dependencies;
    using Exceptions;
    using Xunit;

    public sealed class ContainerTests
    {
        [Fact]
        public void CreateObject_WhenCreatingAPlainObject_ThenItIsSuccessfull()
        {
            var container = new Container();

            var result = container.Resolve(typeof(ContainerTestClass1));

            Assert.NotNull(result);
            Assert.IsType<ContainerTestClass1>(result);
        }

        [Fact]
        public void CreateObject_WhenCreatingAnObjectWithCyclicDependency_ThenItThrows()
        {
            var container = new Container();

            Assert.Throws<CyclicDependencyException>(
                () => 
                container.Resolve(typeof(ContainerTestClassCyclicDependency1)));
        }

        [Fact]
        public void CreateObject_WhenCreatingAnObjectWithCyclicDependency_ThenItThrowsEvenIfWeForceItNotTo()
        {
            var container = new Container();

            Assert.Throws<CyclicDependencyException>(
                () =>
                    container.Resolve(typeof(ContainerTestClassCyclicDependency1), throwOnError: false));
        }

        [Fact]
        public void CreateObject_WhenRegisteringWithInjectedInterface_ThenWeCanRetrieveTheObjectSuccessfully()
        {
            var container = new Container();

            container.RegisterWithDefaultInterface(typeof(ContainerTestClass1));

            var result = container.CreateObject(typeof(ContainerTestClassWithDepndentIinterface));

            Assert.NotNull(result);
            Assert.IsType<ContainerTestClassWithDepndentIinterface>(result);
        }

        [Fact]
        public void Resolve_WhenCreatingAnObject_ThenItIsSuccessfullyInjectingAllDependencies()
        {
            Dictionary<string, object> customObjectsCache = new Dictionary<string, object>();
            var container = new Container(customObjectsCache);

            var result = container.Resolve<ContainerTestClass2>();

            Assert.NotNull(result);
            Assert.IsType<ContainerTestClass2>(result);
            Assert.NotNull(result.o);
            Assert.IsType<ContainerTestClass1>(result.o);

            Assert.Equal(2, customObjectsCache.Count);
        }

        [Fact]
        public void Resolve_WhenCreatingObjectsThatShareDependencies_ThenItIsSuccessfullyInjectingAndSharingDependencies()
        {
            Dictionary<string, object> customObjectsCache = new Dictionary<string, object>();
            var container = new Container(customObjectsCache);

            var result2 = container.Resolve<ContainerTestClass2>();
            var result3 = container.Resolve<ContainerTestClass3>();

            Assert.NotNull(result2);
            Assert.IsType<ContainerTestClass2>(result2);
            Assert.NotNull(result2.o);
            Assert.IsType<ContainerTestClass1>(result2.o);
            Assert.NotNull(result3);
            Assert.IsType<ContainerTestClass3>(result3);
            Assert.NotNull(result3.o);
            Assert.IsType<ContainerTestClass1>(result3.o);

            Assert.Equal(3, customObjectsCache.Count); // Shares ContainerTestClass1
        }

        [Fact]
        public void Resolve_WhenRegisteringWithDefaultInterface_ThenWeCanRetrieveTheObjectSuccessfully()
        {
            var container = new Container();

            container.RegisterWithDefaultInterface(typeof(ContainerTestClass1));

            var result = container.Resolve<IContainerTestClass1>();

            Assert.NotNull(result);
            Assert.IsType<ContainerTestClass1>(result);
        }

        [Fact]
        public void Resolve_WhenTryingToRetrieveAnUnregisteredInterface_ThenItThrowsAnException()
        {
            var container = new Container();

            Assert.Throws<ArgumentException>(
                () =>
                    container.Resolve(typeof(IContainerTestClass1)));
        }

        [Fact]
        public void Resolve_WhenTryingToRetrieveAnUnregisteredInterfaceAndWeSetNoThrow_ThenItReturnsNull()
        {
            var container = new Container();

            var result = container.Resolve(typeof(IContainerTestClass1), throwOnError: false);

            Assert.Null(result);
        }

        [Fact]
        public void Resolve_WhenRegisteringWithInjectedInterface_ThenWeCanRetrieveTheObjectSuccessfully()
        {
            var container = new Container();

            container.RegisterWithDefaultInterface(typeof(ContainerTestClass1));

            var result = container.Resolve<ContainerTestClassWithDepndentIinterface>();

            Assert.NotNull(result);
            Assert.IsType<ContainerTestClassWithDepndentIinterface>(result);
        }

        [Fact]
        public void Resolve_WhenRegisteringOnAParentContainer_ThenWeCanRetrieveTheObjectSuccessfully()
        {
            var parentContainer = new Container();
            var container = new Container(parentContainer);

            parentContainer.RegisterWithDefaultInterface(typeof(ContainerTestClass1));

            var result = container.Resolve<IContainerTestClass1>();

            Assert.NotNull(result);
            Assert.IsType<ContainerTestClass1>(result);
        }

        private interface IContainerTestClass1
        {   
        }

        private class ContainerTestClass1 : IContainerTestClass1
        {
        }

        private class ContainerTestClass2
        {
            public readonly ContainerTestClass1 o;

            public ContainerTestClass2(ContainerTestClass1 o)
            {
                this.o = o;
            }
        }

        private class ContainerTestClass3
        {
            public readonly ContainerTestClass1 o;

            public ContainerTestClass3(ContainerTestClass1 o)
            {
                this.o = o;
            }
        }

        private class ContainerTestClassCyclicDependency1
        {
            public ContainerTestClassCyclicDependency1(ContainerTestClassCyclicDependency2 o)
            {
            }
        }

        private class ContainerTestClassCyclicDependency2
        {
            public ContainerTestClassCyclicDependency2(ContainerTestClassCyclicDependency1 o)
            {
            }
        }

        private class ContainerTestClassWithDepndentIinterface
        {
            public readonly IContainerTestClass1 o;

            public ContainerTestClassWithDepndentIinterface(IContainerTestClass1 o)
            {
                this.o = o;
            }
        }
    }
}
