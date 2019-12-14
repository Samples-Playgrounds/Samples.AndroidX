using Android.Widget;
using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.UI.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Extensions.Reactive
{
    public static class EditTextExtensions
    {
        public static IObservable<Unit> EditorActionSent(this IReactive<EditText> reactive)
            => Observable
                .FromEventPattern<TextView.EditorActionEventArgs>(e => reactive.Base.EditorAction += e, e => reactive.Base.EditorAction -= e)
                .SelectUnit();
    }
}
