using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib2.classes;
using lib2.types;
using lib2.Task.classes;
using lib2.Project.classes;

namespace lib2.Link.classes
{
    public interface ILinkFactory  : IID
    {
        IProject getProject();
        int getCountLinks();
        ILink getLastCreated();



        ILink createLink(identity PrecursorID, identity FollowerID, eLnkType type, double delay);
        ILink createLink(identity PrecursorID, identity FollowerID, eLnkType type);
        ILink createLink(string PrecursorID, string FollowerID, eLnkType type, double delay);
        ILink createLink(string PrecursorID, string FollowerID, eLnkType type);

        ILink getLink(identity ID);
        ILink getLink(string ID);

        void deleteLink(identity ID);
        void deleteLink(string ID);


        event EventHandler event_linkFactoryDelete;
    }

    public class linkFactory : ILinkFactory
    {
        #region Variables

        private identity _ID;

        private IProject _project;

        private Dictionary<string, link> dctLinks;

        #endregion
        #region Properties

        public identity ID { get { return _ID; } }

        public ILink lastCreated { get; private set; }

        public int count { get { return dctLinks.Count; } }

        #endregion
        #region Events
        public event EventHandler event_linkFactoryDelete;
        #endregion
        #region Constructor
        public linkFactory(IProject project, Action destroyMethod)
        {
            _ID = new identity(eEntity.factory);
            dctLinks = new Dictionary<string, link>();

            _project = project;
            project.event_projectDelete += handler_projectDelete;

            destroyMethod = deleteThis;
        }
        ~linkFactory()
        {
            if (dctLinks != null)
            {
                onFactoryDestroy();
                dctLinks.Clear();
                dctLinks = null;
            }

            if (_project != null)
            {
                _project.event_projectDelete -= handler_projectDelete;
                _project = null;
            }

            _ID = null;
        }
        #endregion

        #region Handlers
        private void handler_projectDelete(object sender, EventArgs e)
        {
            deleteThis();
        }
        private void handler_linkDelete(object sender, EventArgs e)
        {
            ILink lnk = (ILink)sender;
            lnk.event_linkDeleted -= handler_linkDelete;
            delElement(lnk.getIDobject().ID);
        }
        private void onFactoryDestroy()
        {
            EventHandler handler = event_linkFactoryDelete;
            if (handler != null) handler(this, new EventArgs());
        }
        #endregion
        
        #region Methods

        public ILink createLink(identity PrecursorID, identity FollowerID, eLnkType type, double delay)
        {
            link newLink = new link(_project, 
                _project.getTaskFactory().getTask(PrecursorID),
                _project.getTaskFactory().getTask(FollowerID), 
                type, delay);

            newLink.event_linkDeleted += handler_linkDelete;
            addElement(newLink);

            return newLink;
        }
        public ILink createLink(identity PrecursorID, identity FollowerID, eLnkType type)
        { return createLink(PrecursorID, FollowerID, type, 0); }
        public ILink createLink(string PrecursorID, string FollowerID, eLnkType type, double delay)
        {
            identity prec = _project.getTaskFactory().getTask(PrecursorID).getIDobject();
            identity foll = _project.getTaskFactory().getTask(FollowerID).getIDobject();
            return createLink(prec, foll, type, 0);
        }
        public ILink createLink(string PrecursorID, string FollowerID, eLnkType type)
        { return createLink(PrecursorID, FollowerID, type, 0); }



        public ILink getLink(identity ID)
        { return getLink(ID.ID); }
        public ILink getLink(string ID)
        { return getElement(ID); }



        public void deleteLink(identity ID)
        { deleteLink(ID.ID);  }
        public void deleteLink(string ID)
        { getElement(ID).deleteLink(); }

        #endregion
        #region Service
        private string excp_notFoundInCollection(string ID)
        { return string.Format("not found link with such ID: {0}", ID); }
        private bool isExist(string ID)
        { return dctLinks.Keys.Contains(ID); }
        private void addElement(link Object)
        {
            dctLinks.Add(Object.getIDobject().ID, Object);
            lastCreated = Object;
        }
        private bool delElement(string ID)
        {
            if (ID == lastCreated.getIDobject().ID) lastCreated = null;
            return dctLinks.Remove(ID);
        }
        private ILink getElement(string ID)
        { return dctLinks[ID]; }
        

        private void deleteThis()
        {
            onFactoryDestroy();

            dctLinks.Clear();
            dctLinks = null;

            _project.event_projectDelete -= handler_projectDelete;
            _project = null;
        }

        


        #endregion
        #region Interface secondary
        public identity getIDobject() { return _ID; }
        public IProject getProject() { return _project; }
        public int getCountLinks() { return count; }
        public ILink getLastCreated() { return lastCreated; }
        #endregion
    }
}
