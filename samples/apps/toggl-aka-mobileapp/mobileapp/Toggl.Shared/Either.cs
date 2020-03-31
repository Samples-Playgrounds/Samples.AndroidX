using System;

namespace Toggl.Shared
{
    public sealed class Either<TLeft, TRight>
    {
        private readonly bool useLeft;
        private readonly TLeft left;
        private readonly TRight right;

        public bool IsLeft => useLeft;

        public bool IsRight => !useLeft;

        public TLeft Left => useLeft ? left : throw new InvalidOperationException("Cannot use the Right property in an Either object created with a left value");

        public TRight Right => useLeft ? throw new InvalidOperationException("Cannot use the Left property in an Either object created with a right value") : right;

        private Either(TLeft left)
        {
            this.left = left;
            useLeft = true;
        }

        private Either(TRight right)
        {
            this.right = right;
            useLeft = false;
        }

        public void Match(Action<TLeft> leftAction, Action<TRight> rightAction)
        {
            Ensure.Argument.IsNotNull(leftAction, nameof(leftAction));
            Ensure.Argument.IsNotNull(rightAction, nameof(rightAction));

            if (useLeft)
            {
                leftAction(left);
                return;
            }

            rightAction(right);
        }

        public T Match<T>(Func<TLeft, T> leftAction, Func<TRight, T> rightFunc)
        {
            Ensure.Argument.IsNotNull(leftAction, nameof(leftAction));
            Ensure.Argument.IsNotNull(rightFunc, nameof(rightFunc));

            return useLeft ? leftAction(left) : rightFunc(right);
        }

        public static Either<TLeft, TRight> WithLeft(TLeft left)
            => new Either<TLeft, TRight>(left);

        public static Either<TLeft, TRight> WithRight(TRight right)
            => new Either<TLeft, TRight>(right);
    }
}
