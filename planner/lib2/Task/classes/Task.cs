using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib2.classes;
using lib2.delegates;
using lib2.types;
using lib2.Project.classes;

namespace lib2.Task.classes
{
    

    public interface ITask : IID
    {
        string getName();
        void setName(string name);

        DateTime getStart();
        void setStart(DateTime date);

        DateTime getFinish();
        void setFinish(DateTime date);

        DateTime getDuration();
        void setDuration(DateTime date);

        eFLim getLimitType();
        void setLimitType(eFLim type);
        DateTime getLimitDate();
        void setLimitDate(DateTime date);

        void deleteTask();



        ILinkAdapter getLinkAdapter();



        event EventHandler<EA_valueChange<DateTime>> event_startChanged;
        event EventHandler<EA_valueChange<DateTime>> event_finishChanged;
        event EventHandler<EA_valueChange<DateTime>> event_taskDelete;
    }


    public class task
    {
        #region Variables
        private identity _ID;

        private Dot _start;
        private Dot _finish;

        private ILimit _lclLimit;

        private string _name;

        private double _duration;
        #endregion
    }
}
