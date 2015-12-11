using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.delegates;
using lib.period.iFaces;
using lib.project.iFaces;

namespace lib.limits.iFaces
{
    public interface IPeriod_localLimit
    {
        e_tskLimit limitType { get; set; }
        DateTime limitDate { get; set; }
        ILimit_check outerLimit { get; set; }

        event EventHandler<eventArgs_valueChange<DateTime>> event_limitDateChanged;
        event EventHandler<eventArgs_valueChange<e_tskLimit>> event_limitTypeChanged;


        void connectDuration(IPeriod_duration duration);
        void connectProject(IProject_dates ipdProject);
    }
}
