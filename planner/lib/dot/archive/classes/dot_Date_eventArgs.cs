using System;

using lib.dot.archive.iFaces;
using lib.delegates;

namespace lib.dot.archive.classes
{
    public class eventArgs_Date_expectedChange : System.EventArgs
    {
        public readonly DateTime oldValue;
        public readonly DateTime newValue;

        public readonly d_valueChange_eventArgs<DateTime> expectedDate;
        public readonly IDot_Value instance;

        public eventArgs_Date_expectedChange(IDot_Value sender, 
            d_valueChange_eventArgs<DateTime> currentValueSetHandler, 
            DateTime Old, DateTime New)
        {
            expectedDate = currentValueSetHandler;
            instance = sender;
            newValue = New;
            oldValue = Old;
        }
    }
    public class eventArgs_dateChanged : System.EventArgs
    {
        public readonly DateTime oldValue;
        public readonly DateTime newValue;
        public readonly IDot_Value instance;

        public eventArgs_dateChanged(IDot_Value sender,
                                            DateTime Old, DateTime New)
        {
            instance = sender;
            newValue = New;
            oldValue = Old;
        }
    }

    public enum e_PropsEArgs { None = 0, Enable = 1, Fixed = 2, Limit = 4, Date = 8 }
}
