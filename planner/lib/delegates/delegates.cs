using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.delegates
{
    public delegate void d_value<T>(T Value);
    public delegate T d_valueGet<T>();

    public delegate void d_singleValue<T>(object sender, T Value);
    public delegate void d_valueChange<T>(object sender, T oldValue, T newValue);

    public delegate T d_valueReference<T>();

    public delegate void d_valueChange_eventArgs<T>(object sender, T oldValue, T newValue, bool allowed = false);

    public delegate void d_returnValueChange_eventArgs<T>(object sender, T Value, bool allowed = false);

    public delegate bool d_valueComparator<T>(object sender, T oldValue, T newValue, out T result);
}
