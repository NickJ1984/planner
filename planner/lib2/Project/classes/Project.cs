using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib2.classes;
using lib2.types;
using lib2.delegates;
using lib2.Task.classes;
using lib2.Link.classes;

namespace lib2.Project.classes
{
    public interface IProject : IID
    {
        string getName();
        void setName(string name);

        DateTime getStart();
        void setStart(DateTime date);

        DateTime getFinish();
        double getDuraton();

        void projectDelete();


        ILinkFactory getLinkFactory();
        ITaskFactory getTaskFactory();


        event EventHandler event_projectDelete;
        event EventHandler<EA_valueChange<DateTime>> event_startChanged;
        event EventHandler<EA_valueChange<DateTime>> event_finishChanged;
    }

    public class Project : IProject
    {
        #region Variables
        private identity _ID;
        private string _name;
        private DateTime _startDate;
        private DateTime _finishDate;

        public event EventHandler event_projectDelete;
        public event EventHandler<EA_valueChange<DateTime>> event_startChanged;
        public event EventHandler<EA_valueChange<DateTime>> event_finishChanged;

        public string getName()
        {
            throw new NotImplementedException();
        }

        public void setName(string name)
        {
            throw new NotImplementedException();
        }

        public DateTime getStart()
        {
            throw new NotImplementedException();
        }

        public void setStart(DateTime date)
        {
            throw new NotImplementedException();
        }

        public DateTime getFinish()
        {
            throw new NotImplementedException();
        }

        public double getDuraton()
        {
            throw new NotImplementedException();
        }

        public void projectDelete()
        {
            throw new NotImplementedException();
        }

        public ILinkFactory getLinkFactory()
        {
            throw new NotImplementedException();
        }

        public ITaskFactory getTaskFactory()
        {
            throw new NotImplementedException();
        }

        public identity getIDobject()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Properties

        #endregion
        #region Events

        #endregion
        #region Constructor

        #endregion
        #region Handlers

        #endregion
        #region Methods

        #endregion
    }
}
