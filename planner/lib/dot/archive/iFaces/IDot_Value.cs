using System;

using lib.types;
using lib.dot.archive.classes;

namespace lib.dot.archive.iFaces
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
