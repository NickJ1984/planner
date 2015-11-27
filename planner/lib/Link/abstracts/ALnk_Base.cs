using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.interfaces;
using lib.service;
using lib.period.iFaces;
using lib.dot.iFaces;

namespace lib.Link.abstracts
{
    public abstract class ALnk_Base : IEntity
    {
        #region Variables
        protected bool _enabled = true;
        protected bool _linked = false;
        protected Func<DateTime> datePrecursor;
        protected Func<DateTime> dateFollower;
        internal e_linkType _type;
        internal e_sideType _sideChild;
        internal e_sideType _sideParent;
        internal IPeriod _parent;
        internal IPeriod _child;
        #endregion
        #region Properties
        public bool linked { get { return _linked; } }
        public bool enabled { get { return _enabled; } }
        public virtual e_linkType type { get { return _type; } }
        public virtual object parent { get { return _parent; } }
        public virtual object child { get { return _child; } }
        #endregion
        #region Events
        #endregion
        #region Constructors
        #endregion
        #region Methods
        protected bool setLinkType(e_linkType lType)
        {
            if(!enabled) return false;
            

            return false;
        }
        #endregion
        #region Service
        protected virtual void resetLink()
        {

            _parent.event_finishChanged -= _handler_precursorDateChanged;
            _parent.event_startChanged -= _handler_precursorDateChanged;

            _child.event_finishChanged -= _handler_followerDateChanged;
            _child.event_startChanged -= _handler_followerDateChanged;

            _child = null;
            _parent = null;

            _type = e_linkType.none;
            _sideChild = (int)0;
            _sideParent = (int)0;

            datePrecursor = null;
            dateFollower = null;

        }
        protected virtual void linkDates(IPeriod precursor, IPeriod follower, e_linkType lType)
        {
            if (enabled != true) return;
            _parent = precursor;
            _child = follower;
            _type = lType;
            _sideChild = __hlp.getSideType(lType, e_linkObject.follower);
            _sideParent = __hlp.getSideType(lType, e_linkObject.precursor);
            linkDates();
        }
        protected virtual void linkDates()
        {
            #region Precursor
            switch (_sideParent)
            {
                case e_sideType.Finish_:
                    datePrecursor = () => _parent.getFinish();
                    _parent.event_finishChanged -= _handler_precursorDateChanged;
                    _parent.event_finishChanged += _handler_precursorDateChanged;
                    break;
                case e_sideType.Start_:
                    datePrecursor = () => _parent.getStart();
                    _parent.event_startChanged -= _handler_precursorDateChanged;
                    _parent.event_startChanged += _handler_precursorDateChanged;
                    break;
            }
            #endregion
            #region Follower
            switch (_sideChild)
            {
                case e_sideType._Finish:
                    dateFollower = () => _child.getFinish();
                    _child.event_finishChanged -= _handler_followerDateChanged;
                    _child.event_finishChanged += _handler_followerDateChanged;
                    break;
                case e_sideType._Start:
                    dateFollower = () => _child.getStart();
                    _child.event_startChanged -= _handler_followerDateChanged;
                    _child.event_startChanged += _handler_followerDateChanged;
                    break;
            }
            #endregion
        }
        internal void setEnabled(bool bEnabled)
        { if (_enabled != bEnabled) _enabled = bEnabled; }
        #endregion
        #region Events

        #endregion
        #region Handlers
        protected abstract void _handler_precursorDateChanged(object sender, EventArgs e);
        protected abstract void _handler_followerDateChanged(object sender, EventArgs e);
        #endregion
        #region Overrides
        #endregion
        #region internal entities
        #endregion
        #region referred interface implementation
        public abstract entityInfo getEntityInfo();
        #endregion
        #region self interface implementation
        public abstract bool setParent(IPeriod oParent);
        public abstract IPeriod getParent();
        public abstract bool setChild(IPeriod oChild);
        public abstract IPeriod getChild();
        #endregion
    }
}
