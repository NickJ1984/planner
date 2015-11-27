using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.delegates;

namespace lib.dot.iFaces
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
