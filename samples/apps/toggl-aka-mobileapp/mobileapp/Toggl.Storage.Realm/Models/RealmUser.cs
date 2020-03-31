using Realms;
using System;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Models;

namespace Toggl.Storage.Realm
{
    internal partial class RealmUser : RealmObject, IDatabaseUser
    {
        public string ApiToken { get; set; }

        public DateTimeOffset At { get; set; }

        public long? DefaultWorkspaceId { get; set; }

        //Realm doesn't support enums 
        [Ignored]
        public BeginningOfWeek BeginningOfWeek
        {
            get => (BeginningOfWeek)BeginningOfWeekInt;
            set => BeginningOfWeekInt = (int)value;
        }

        public int BeginningOfWeekInt { get; set; }

        [Ignored]
        public Email Email
        {
            get => EmailString.ToEmail();
            set => EmailString = value.ToString();
        }

        [MapTo("Email")]
        public string EmailString { get; set; }

        public string Fullname { get; set; }

        public string ImageUrl { get; set; }

        public string Language { get; set; }

        public string Timezone { get; set; }
    }
}
