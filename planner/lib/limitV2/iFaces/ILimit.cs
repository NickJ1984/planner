using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.function.temp.iFaces;
using lib.types;
using lib.delegates;

namespace lib.limitV2.iFaces
{
    public interface ILimit
    {
        e_dot_Limit getType();
        void setType(e_dot_Limit Type);

        event EventHandler<eventArgs_valueChange<DateTime>> event_dateChanged;
        event EventHandler<eventArgs_valueChange<e_dot_Limit>> event_typeChanged;

        DateTime getLimitDate();
        void setLimitDate(DateTime date);
    }
    public interface ILimit_data
    {
        e_dot_Limit getType();
        DateTime getLimitDate();

        event EventHandler<eventArgs_valueChange<DateTime>> event_dateChanged;
        event EventHandler<eventArgs_valueChange<e_dot_Limit>> event_typeChanged;
    }
    public interface ILimit_check
    {
        DateTime checkDate(DateTime Date);

        event EventHandler event_update;
    }
}
