using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.delegates;
using lib.limits.iFaces;

namespace lib.dot.iFaces
{
    public interface IDot
    {
        DateTime date { get; set; }
        ILimit_check dotLimitCheck { get; set; }
        event EventHandler<eventArgs_valueChange<DateTime>> event_dateChanged;
    }
}
