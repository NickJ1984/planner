using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.delegates;
using lib.period.iFaces;

namespace lib.limits.iFaces
{
    public interface IPeriod_localLimit
    {
        e_tskLimit limitType { get; set; }
        DateTime limitDate { get; set; }
        ILimit_check outerLimit { get; set; }
        Func<double> getDuration { get; set; }

        event EventHandler<eventArgs_valueChange<DateTime>> event_limitDateChanged;
        event EventHandler<eventArgs_valueChange<e_tskLimit>> event_limitTypeChanged;

        void handler_durationChanged(object sender, eventArgs_valueChange<double> e);

        void connectDuration(IPeriod_duration duration);
    }
}
