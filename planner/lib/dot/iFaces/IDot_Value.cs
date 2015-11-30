using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.dot.classes;

namespace lib.dot.iFaces
{
    public interface IDot_Value
    {
        e_dot_stateValue state { get; }
        DateTime source { get; }
        DateTime current { get; set; }

        event EventHandler<eventArgs_dateChanged> dateChanged;
        event EventHandler<eventArgs_Date_expectedChange> expectedDateChange;

        bool declareDate(DateTime date);

        void reset();
        void reset(DateTime startDate, e_dot_stateValue startState = e_dot_stateValue.Source);
    }
}
