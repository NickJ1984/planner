using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.delegates;

namespace lib.period.iFaces
{
    public interface IPeriod_duration : IComparable, IComparable<double>, IComparable<IPeriod_duration>, IEquatable<IPeriod_duration>, IEquatable<double>
    {
        double duration { get; set; }

        Func<double> getDuration { get; }

        event EventHandler<eventArgs_valueChange<double>> event_durationChanged;

        void increment(double inc);
        void decrement(double dec);
    }
}
