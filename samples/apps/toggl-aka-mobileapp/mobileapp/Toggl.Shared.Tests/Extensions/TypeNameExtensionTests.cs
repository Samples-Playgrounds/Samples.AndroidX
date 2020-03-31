using FluentAssertions;
using System;
using System.Collections.Generic;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Shared.Tests.Extensions
{
    public sealed class TypeNameExtensionTests
    {
        private interface IFoo { }

        private interface IBar<TParam> { }

        private interface IFizz<TParam1, TParam2> { }

        private sealed class Foo : IFoo { }

        private sealed class Bar<TParam> : IBar<TParam> { }


        [Theory]
        [InlineData(typeof(string), "String")]
        [InlineData(typeof(IFoo), "IFoo")]
        [InlineData(typeof(IBar<string>), "IBar<String>")]
        [InlineData(typeof(IBar<string[]>), "IBar<String[]>")]
        [InlineData(typeof(IBar<IEnumerable<string>>), "IBar<IEnumerable<String>>")]
        [InlineData(typeof(IFizz<string, int>), "IFizz<String,Int32>")]
        [InlineData(typeof(IFizz<string, int[]>), "IFizz<String,Int32[]>")]
        [InlineData(typeof(IFizz<IEnumerable<IFoo>, IBar<Foo>>), "IFizz<IEnumerable<IFoo>,IBar<Foo>>")]
        [InlineData(typeof(Bar<Bar<Bar<Foo>>>), "Bar<Bar<Bar<Foo>>>")]
        public void ReturnsTheExpectedFriendlyName(Type type, string expectedName)
        {
            var friendlyname = type.GetFriendlyName();
            friendlyname.Should().Be(expectedName);
        }
    }
}
