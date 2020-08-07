using System;
using System.Timers;

namespace Toggl.Shared
{
    public struct TimerView
    {
        private const string list = "list";
        private const string calendar = "calendar";

        public string Value { get; }

        public static TimerView List { get; } = FromString(list);
        public static TimerView Calendar { get; } = FromString(calendar);

        public bool IsList => Value == list;
        public bool IsCalendar => Value == calendar;

        private TimerView(string value)
        {
            Value = value;
        }

        public static TimerView FromString(string value)
        {
            if (value == calendar)
                return new TimerView(value);
            return new TimerView(list);
        }
    }
}
