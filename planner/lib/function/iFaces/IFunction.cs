using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.function.delegates;

namespace lib.function.iFaces
{
    public interface IFunctionInfo
    {
        e_limDirection getDirection();
        bool isExist();

        Func<DateTime, DateTime> getCheckFunction();

        DateTime getMinDate();
        DateTime getMaxDate();
        
        double getLimitRange(); //-1 - бесконечно; 0 - точка
    }
    public interface IFunctionGet
    {
        e_limDirection getDirection();

        DateTime getMinDate();
        DateTime getMaxDate();
        DateTime checkDate(DateTime Date);
    }
    public interface IFunctionSet
    {
        void setDirection(e_limDirection direction);
        void setMinDate(DateTime Date);
        void setMaxDate(DateTime Date);
        void generateStatic();
    }
    
}
