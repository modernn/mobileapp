using System;
using System.Reactive.Linq;
using System.Windows.Controls;
using Toggl.Core.UI.Reactive;

namespace Toggl.WPF.Extensions.Reactive
{
    public static class TextBoxExtensions
    {
        public static IObservable<string> Text(this IReactive<TextBox> reactive) =>
            Observable.FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(
                e => reactive.Base.TextChanged += e,
                e => reactive.Base.TextChanged -= e
            ).Select(_ => reactive.Base.Text);

        public static Action<string> TextObserver(this IReactive<TextBox> reactive) =>
            text =>
            {
                reactive.Base.Text = text;
            };
    }
}
