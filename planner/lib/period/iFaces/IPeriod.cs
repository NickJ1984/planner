using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.project.iFaces;

namespace lib.period.iFaces
{
    public interface IPeriod
    {
        IProjectInfo projectInfo { get; }

        event EventHandler event_startChanged;
        event EventHandler event_finishChanged;

        DateTime getStart();
        void setStart(object sender, EventArgs e);
        DateTime getFinish();
        void setFinish(object sender, EventArgs e);
        double getDuration();
        void setDuration(double dStart);
    }
}
