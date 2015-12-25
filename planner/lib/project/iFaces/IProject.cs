using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.delegates;
using lib.types;

namespace lib.project.iFaces
{
    public interface IProject_info
    {
        string name { get; }
        Guid ID { get; }
    }
    public interface IProject_dates
    {
        e_prjType projectType { get; }
        DateTime start { get; }
        DateTime finish { get; }

        event EventHandler<eventArgs_valueChange<DateTime>> event_startChanged;
        event EventHandler<eventArgs_valueChange<DateTime>> event_finishChanged;

        
    }
}
