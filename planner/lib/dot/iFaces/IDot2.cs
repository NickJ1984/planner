using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;

namespace lib.dot.iFaces
{
    public interface IDot2
    {
        DateTime date { get; set; }
        IDot_Limit2 dotLimit { get; set; }
        IDot_Value2 dValue { get; }
    }
}
