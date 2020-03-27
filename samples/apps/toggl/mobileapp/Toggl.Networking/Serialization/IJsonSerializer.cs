namespace Toggl.Networking.Serialization
{
    internal interface IJsonSerializer
    {
        T Deserialize<T>(string json);

        string Serialize<T>(T data, SerializationReason reason);
    }
}
