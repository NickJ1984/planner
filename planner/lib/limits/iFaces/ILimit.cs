using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.interfaces;
using lib.delegates;
using lib.limits.delegates;
using lib.limits.classes;

namespace lib.limits.iFaces
{
    public interface ILimit_check
    {
        DateTime checkDate(DateTime Date);

        event EventHandler event_update;
    }
    public interface ILimit_values
    {
        int direction { get; }
        e_dot_Limit limitType { get; set; }
        DateTime date { get; set; }
    }
    
    public interface ILim : ILimit_values,ILimit_check, IComparable, IEquatable<ILim>
    {
        getFunctionLimit getFunctionLim(e_dot_Limit Limit);
        KeyValuePair<double, double> getFreeSpace(DateTime cDate); //key - leftSpace, value - rightSpace
        bool checkDate(DateTime Date, out DateTime result);
        bool isAllowed(ILim limit);
        bool isAllowed(DateTime date);
    }
    public interface ILimit : ILimit_check
    {
        e_limDirection getDirection();
        Func<DateTime, DateTime> getCheckFunction();
        Func<DateTime, DateTime> getCheckIntersectionFunction(ILimit slaveLimit);
    }
    public interface ILimitVALS<T>
        where T : struct
    {
        T getValues();
        void setType<U>(U type);
    }
   
    
    
    
}
