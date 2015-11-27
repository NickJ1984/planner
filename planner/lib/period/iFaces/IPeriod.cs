using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.period.iFaces
{
    public interface IPeriod
    {

        event EventHandler event_startChanged;
        event EventHandler event_finishChanged;

        DateTime getStart();
        void setStart(DateTime dStart);
        DateTime getFinish();
        void setFinish(DateTime dFinish);
        double getDuration();
        void setDuration(double dStart);
    }
}
