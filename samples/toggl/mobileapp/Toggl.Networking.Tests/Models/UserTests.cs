using FluentAssertions;
using System;
using Toggl.Networking.Models;
using Xunit;
using static Toggl.Shared.Extensions.EmailExtensions;

namespace Toggl.Networking.Tests.Models
{
    public sealed class UserTests
    {
        public sealed class TheUserModel
        {
            private string validJson =>
                "{\"id\":9000,\"api_token\":\"1971800d4d82861d8f2c1651fea4d212\",\"default_workspace_id\":777,\"email\":\"johnt@swift.com\",\"fullname\":\"John Swift\",\"beginning_of_week\":0,\"language\":\"en_US\",\"image_url\":\"https://www.toggl.com/system/avatars/9000/small/open-uri20121116-2767-b1qr8l.png\",\"timezone\":\"Europe/Zagreb\",\"at\":\"2013-03-06T12:18:42+00:00\"}";

            private string validJsonWithNullDefaultWorkspaceId =>
                "{\"id\":9000,\"api_token\":\"1971800d4d82861d8f2c1651fea4d212\",\"default_workspace_id\":null,\"email\":\"johnt@swift.com\",\"fullname\":\"John Swift\",\"beginning_of_week\":0,\"language\":\"en_US\",\"image_url\":\"https://www.toggl.com/system/avatars/9000/small/open-uri20121116-2767-b1qr8l.png\",\"timezone\":\"Europe/Zagreb\",\"at\":\"2013-03-06T12:18:42+00:00\"}";

            private User validUser => new User
            {
                Id = 9000,
                ApiToken = "1971800d4d82861d8f2c1651fea4d212",
                DefaultWorkspaceId = 777,
                Email = "johnt@swift.com".ToEmail(),
                Fullname = "John Swift",
                BeginningOfWeek = 0,
                Language = "en_US",
                ImageUrl = "https://www.toggl.com/system/avatars/9000/small/open-uri20121116-2767-b1qr8l.png",
                Timezone = "Europe/Zagreb",
                At = new DateTimeOffset(2013, 3, 6, 12, 18, 42, TimeSpan.Zero)
            };

            private User validUserWithNullDefaultWorkspaceId => new User
            {
                Id = 9000,
                ApiToken = "1971800d4d82861d8f2c1651fea4d212",
                DefaultWorkspaceId = null,
                Email = "johnt@swift.com".ToEmail(),
                Fullname = "John Swift",
                BeginningOfWeek = 0,
                Language = "en_US",
                ImageUrl = "https://www.toggl.com/system/avatars/9000/small/open-uri20121116-2767-b1qr8l.png",
                Timezone = "Europe/Zagreb",
                At = new DateTimeOffset(2013, 3, 6, 12, 18, 42, TimeSpan.Zero)
            };

            [Fact, LogIfTooSlow]
            public void HasConstructorWhichCopiesValuesFromInterfaceToTheNewInstance()
            {
                var clonedObject = new User(validUser);

                clonedObject.Should().NotBeSameAs(validUser);
                clonedObject.Should().BeEquivalentTo(validUser, options => options.IncludingProperties());
            }

            [Fact, LogIfTooSlow]
            public void CanBeDeserialized()
            {
                SerializationHelper.CanBeDeserialized(validJson, validUser);
            }

            [Fact, LogIfTooSlow]
            public void CanBeSerialized()
            {
                SerializationHelper.CanBeSerialized(validJson, validUser);
            }

            [Fact, LogIfTooSlow]
            public void CanBeDeserializedWithNullDefaultWorkspaceId()
            {
                SerializationHelper.CanBeDeserialized(validJsonWithNullDefaultWorkspaceId, validUserWithNullDefaultWorkspaceId);
            }

            [Fact, LogIfTooSlow]
            public void CanBeSerializedWithNullDefaultWorkspaceId()
            {
                SerializationHelper.CanBeSerialized(validJsonWithNullDefaultWorkspaceId, validUserWithNullDefaultWorkspaceId);
            }
        }
    }
}
