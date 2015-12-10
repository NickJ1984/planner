using System;
using lib.types;
using lib.dot.archive.classes;
using lib.delegates;

namespace lib.dot.archive.iFaces
{
    public interface IDot_Limit
    {
        bool enabled { get; set; }
        e_dot_Limit limit { get; set; }
        
        bool isLimitMin { get; }
        bool isLimitMax { get; }
        DateTime estimatedDate { get; }
        DateTime limitMin { get; set; }
        DateTime limitMax { get; set; }
        double range { get; }

        event EventHandler<eventArgs_valueChange<DateTime>> event_dateChangeWarning;
        event EventHandler<eventArgs_valueChange<e_dot_Limit>> event_limitTypeChanged;
        event EventHandler<eventArgs_valuesChange<DateTime>> event_limitDateChanged;

        void dot_expectedDateChange(object sender, eventArgs_Date_expectedChange e);
        DateTime checkDate(DateTime date);
        void setDot(IDot_Value Dot);

        void reset();
    }
}
