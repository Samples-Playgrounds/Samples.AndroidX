namespace Toggl.Networking.Network
{
    internal struct HttpHeader
    {
        public enum HeaderType
        {
            None = 0,
            Auth,
            Other
        }

        public HeaderType Type { get; }

        public string Name { get; }

        public string Value { get; }

        public HttpHeader(string name, string value, HeaderType type = HeaderType.Other)
        {
            Type = type;
            Name = name;
            Value = value;
        }

        public static HttpHeader None => new HttpHeader(null, null, HeaderType.None);
    }
}
