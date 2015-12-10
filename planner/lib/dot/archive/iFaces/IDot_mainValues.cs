using System;

using lib.types;

namespace lib.dot.archive.iFaces
{
    public interface IDot_mainValues
    {
        e_dot_aspect aspect { get; }
        e_dot_stateValue state { get; }
        DateTime source { get; }
        DateTime current { get; }



        DateTime getCurrentDate();
        void setCurrentDate(DateTime date);

    }
}
