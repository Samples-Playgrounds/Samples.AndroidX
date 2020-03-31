namespace Toggl.Shared
{
    public struct New<T>
    {
        private readonly bool hasValue;
        private readonly T value;

        private New(T value)
        {
            hasValue = true;
            this.value = value;
        }

        public T ValueOr(T oldValue) => hasValue ? value : oldValue;


        public static New<T> None => default(New<T>);

        public static New<T> Value(T value) => new New<T>(value);

        public static implicit operator New<T>(T value) => new New<T>(value);
    }
}
