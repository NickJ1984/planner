using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.limits.classes;

namespace lib.limits.iFaces
{
    public interface ILimit
    {
        e_dot_Limit2 limitType { get; set; }
        DateTime date { get; set; }
        bool checkDate(DateTime Date, out DateTime result);
        getFunctionLimit getFunctionLim(e_dot_Limit2 Limit);
        ILimit compare(ILimit outer);
    }


}
