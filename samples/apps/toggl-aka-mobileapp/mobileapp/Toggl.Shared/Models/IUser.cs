namespace Toggl.Shared.Models
{
    public interface IUser : IIdentifiable, ILastChangedDatable
    {
        string ApiToken { get; }

        long? DefaultWorkspaceId { get; }

        Email Email { get; }

        string Fullname { get; }

        BeginningOfWeek BeginningOfWeek { get; }

        string Language { get; }

        string ImageUrl { get; }

        string Timezone { get; }
    }
}
