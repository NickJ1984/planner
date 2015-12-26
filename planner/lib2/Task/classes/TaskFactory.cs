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
    public interface ITaskFactory : IID
    {
        IProject getProject();
        int getCountTasks();
        ITask getLastCreated();



        ITask getMinTask();
        ITask getMaxTask();

        event EventHandler<EA_valueChange<DateTime>> event_minDateChanged;
        event EventHandler<EA_valueChange<DateTime>> event_maxDateChanged;



        ITask createTask(string name, DateTime start, double duration, eTskLLim localLimit, DateTime limitDate);
        ITask createTask(string name, eTskLLim localLimit, DateTime limitDate, double duration);
        ITask createTask(string name, DateTime start, double duration);

        ITask getTask(identity ID);
        ITask getTask(string ID);

        ITask deleteTask(identity ID);
        ITask deleteTask(string ID);
    }
}
