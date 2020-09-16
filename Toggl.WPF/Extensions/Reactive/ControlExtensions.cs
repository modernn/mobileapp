using System;
using System.Windows;
using System.Windows.Controls;
using Toggl.Core.UI.Reactive;

namespace Toggl.WPF.Extensions.Reactive
{
    public static class ControlExtensions
    {
        public static IReactive<T> Rx<T>(this T type) where T : Control
            => new ReactiveControl<T>(type);

        private class ReactiveControl<T> : IReactive<T>
            where T : Control
        {
            public T Base { get; }

            public ReactiveControl(T @base)
            {
                Base = @base;
            }
        }

        public static Action<bool> IsVisible(this IReactive<Control> self) =>
            isVisible => self.Base.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
    }
}
