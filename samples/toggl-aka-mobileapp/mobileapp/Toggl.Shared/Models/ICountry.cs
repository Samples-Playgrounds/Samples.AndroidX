namespace Toggl.Shared.Models
{
    public interface ICountry : IIdentifiable
    {
        string Name { get; }
        string CountryCode { get; }
    }
}
