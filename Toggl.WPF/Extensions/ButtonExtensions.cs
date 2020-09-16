using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Toggl.Core.UI.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.WPF.Extensions
{
    public static class ButtonExtensions
    {
        public static IObservable<Unit> Tap(this IReactive<Button> self) =>
            Observable.FromEvent<RoutedEventHandler, RoutedEvent>(
                e => self.Base.Click += e,
                e => self.Base.Click -= e
            ).SelectUnit();
    }
}
