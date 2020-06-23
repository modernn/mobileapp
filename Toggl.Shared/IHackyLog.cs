using System;
using System.Collections.Generic;
using System.Text;

namespace Toggl.Shared
{
    public static class HackyLog
    {
        private static Action<string, string> output = (tag, text) => { };

        public static void Initialize(Action<string, string> output)
        {
            HackyLog.output = output;
        }

        public static void Out(string tag, string text)
        {
            output?.Invoke(tag, text);
        }
    }
}
