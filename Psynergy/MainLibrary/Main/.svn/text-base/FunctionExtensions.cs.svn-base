using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Instrumentation;

using System.Threading;

namespace Psynergy
{
    public static class FunctionExtensions
    {
        public static void CallOn<T>(this object target, Action<T> action) where T : class
        {
            var subject = target as T;

            if (subject != null)
                action(subject);
        }

        public static void CallOnEach<T>(this IEnumerable enumerable, Action<T> action) where T : class
        {
            foreach (object o in enumerable)
                o.CallOn(action);
        }
    }
}
