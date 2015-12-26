using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.function.temp.delegates;

namespace lib.function.temp.iFaces
{
    public interface IFunctionGet
    {
        e_limDirection getDirection();

        DateTime getMinDate();
        DateTime getMaxDate();

        bool inRange(IFunctionGet functionGet);
        bool inRange(DateTime Date);

        Func<DateTime, DateTime> getFunction();
        DateTime checkDate(DateTime Date);
    }
    public interface IFunctionSet
    {
        e_limDirection getDirection();
        void setDirection(e_limDirection direction);

        void setMinLimitDate(DateTime Date);
        DateTime getMinLimitDate();

        void setMaxLimitDate(DateTime Date);
        DateTime getMaxLimitDate();

        void setDate(DateTime Date);
    }
}
