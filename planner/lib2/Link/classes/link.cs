using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using lib2.types;
using lib2.delegates;
using lib2.classes;
using lib2.Project.classes;
using lib2.Task.classes;


namespace lib2.Link.classes
{
    public class EA_flwChanges : EventArgs
    {
        #region Variables
        private DateTime _oldDate;
        private DateTime _newDate;

        private eLnkDot _oldDot;
        private eLnkDot _newDot;
        #endregion
        #region Properties
        public bool dateChanged { get; private set; }
        public bool dotChanged { get; private set; }

        public DateTime newDate { get { return (dateChanged) ? _newDate : _oldDate; } }
        public DateTime oldDate { get { return _oldDate; } }

        public eLnkDot newDot { get { return (dotChanged) ? _newDot : _oldDot; } }
        public eLnkDot oldDot { get { return _oldDot; } }
        #endregion
        #region Constructor
        public EA_flwChanges()
        {
            reset();
        }
        public EA_flwChanges(DateTime oldDate, DateTime newDate, eLnkDot oldDot, eLnkDot newDot)
            : this()
        {
            setDate(oldDate, newDate);
            setDot(oldDot, newDot);
        }
        public EA_flwChanges(DateTime oldDate, DateTime newDate)
            : this()
        {
            setDate(oldDate, newDate);
        }
        public EA_flwChanges(eLnkDot oldDot, eLnkDot newDot)
            : this()
        {
            setDot(oldDot, newDot);
        }
        #endregion
        #region Methods
        public void setDefault(DateTime oldDate, eLnkDot oldDot)
        {
            _oldDate = oldDate;
            _oldDot = oldDot;
        }
        public void setDate(DateTime oldDate, DateTime newDate)
        {
            _oldDate = oldDate;
            _newDate = newDate;
            dateChanged = true;
        }
        public void setDot(eLnkDot oldDot, eLnkDot newDot)
        {
            _oldDot = oldDot;
            _newDot = newDot;
            dotChanged = true;
        }
        public void reset()
        {
            _oldDate = _newDate = __hlp.initDate;
            _oldDot = _newDot = eLnkDot.Start;
            dateChanged = dotChanged = false;
        }
        #endregion
    }

    public interface ILink : IID
    {
        ITask getPrecursor();
        ITask getFollower();

        eLnkState getStateLink();

        eLnkType getTypeLink();
        void setTypeLink(eLnkType type);

        double getDelay();
        void setDelay(double delay);

        ILimitCheck getCheckDate();

        void deleteLink();



        event EventHandler<EA_valueChange<DateTime>> event_dateManagedChanged;
        event EventHandler<EA_valueChange<eLnkType>> event_typeLinkChanged;
        event EventHandler<EA_flwChanges> event_followerUpdate;
        event EventHandler<EA_value<identity>> event_linkDeleted;
    }


    public class link : ILink
    {
        #region Variables
        private typeManager tpManager;

        private IProject _project;
        private identity _ID;
        private ITask _precursor;
        private ITask _follower;

        private eLnkState _state;
        private eLnkType _type;

        private double _delay;

        private limit lnkLimit;
        #endregion
        #region Properties
        public DateTime date { get { return tpManager.date; } }
        public double delay
        {
            get { return tpManager.delay; }
            set { tpManager.delay = value; }
        }
        public eLnkType type
        {
            get { return tpManager.type; }
            set { tpManager.type = value; }
        }
        public eLnkState state { get { return _state; } }
        #endregion

        #region Events
        public event EventHandler<EA_valueChange<DateTime>> event_dateManagedChanged;
        public event EventHandler<EA_valueChange<eLnkType>> event_typeLinkChanged;
        public event EventHandler<EA_flwChanges> event_followerUpdate;
        public event EventHandler<EA_value<identity>> event_linkDeleted;
        #endregion
        #region Constructors
        public link(IProject Project, ITask Precursor, ITask Follower, eLnkType type, double delay = 0)
        {
            _ID = new identity(eEntity.link);

            _project = Project;
            _project.getLinkFactory().event_linkFactoryDelete += handler_factoryDelete;

            _precursor = Precursor;
            _follower = Follower;

            lnkLimit = new limit(link.lnk2lim(type), __hlp.initDate);
            tpManager = new typeManager(this, type, delay);
        }
        #endregion
        #region Handlers self
        private void onDeleteLink()
        {
            EventHandler<EA_value<identity>> handler = event_linkDeleted;
            if(handler != null) event_linkDeleted(this, new EA_value<identity>(_ID));

            tpManager.clear();
            tpManager = null;

            lnkLimit = null;

            _project.getLinkFactory().event_linkFactoryDelete -= handler_factoryDelete;
            _project = null;
            _ID = null;

            _precursor = _follower = null;
        }
        private void onFollowerUpdate(EA_flwChanges args)
        {
            EventHandler<EA_flwChanges> handler = event_followerUpdate;
            if (handler != null) handler(this, args);
        }
        private void onDateChanged(EA_valueChange<DateTime> args)
        {
            EventHandler<EA_valueChange<DateTime>> handler = event_dateManagedChanged;
            if (handler != null) handler(this, args);
        }
        private void onTypeChanged(EA_valueChange<eLnkType> args)
        {
            EventHandler<EA_valueChange<eLnkType>> handler = event_typeLinkChanged;
            if (handler != null) handler(this, args);
        }
        #endregion
        #region Handlers
        private void handler_factoryDelete(object sender, EventArgs e)
        { deleteLink(); }
        private void handler_followerDateChanged(object sender, EA_valueChange<DateTime> e)
        {
            DateTime ctrlDate = lnkLimit.checkDate(e.newValue);
            if (ctrlDate == e.newValue) _state = eLnkState.inTime;
            else if(e.newValue > ctrlDate) _state = eLnkState.later;
            else if (e.newValue < ctrlDate) _state = eLnkState.early;
        }
        #endregion
        #region Interface secondary methods
        public identity getIDobject()
        { return _ID; }
        public eLnkState getStateLink()
        { return _state; }
        public eLnkType getTypeLink()
        { return type; }
        public double getDelay()
        { return delay; }
        public ILimitCheck getCheckDate()
        { return lnkLimit; }



        public ITask getPrecursor()
        { return _precursor; }
        public ITask getFollower()
        { return _follower; }



        public void setTypeLink(eLnkType type)
        { this.type = type; }
        public void setDelay(double delay)
        { this.delay = delay; }
        public void deleteLink()
        { onDeleteLink(); }
        #endregion
        #region inner entity
        private class typeManager
        {
            #region Variables
            private link parent;

            private eLnkType _type;
            private double _delay;

            private Func<DateTime> currentDate;
            private Func<DateTime, double, DateTime> expectedDate;

            private Func<DateTime> pDate;
            private Func<DateTime> fDate;

            #endregion
            #region Properties
            #region base
            public DateTime date { get { return currentDate(); } }
            public double delay
            {
                get { return _delay; }
                set { setDelay(value); }
            }
            public eLnkType type
            {
                get { return _type; }
                set
                {
                    if (value != _type) onTypePropertyWrite(value);
                }
            }
            #endregion
            public eLnkDot pDot { get; private set; }
            public eLnkDot fDot { get; private set; }
            
            public DateTime flwDate {  get { return fDate(); } }
            public DateTime prcDate { get { return pDate(); } }
            #endregion
            #region Constructor
            public typeManager(link parent, eLnkType type, double delay)
            {
                _delay = delay;
                this.parent = parent;
                init_functions();

                eLnkDot precD = __hlp.getPrecursor(type);
                eLnkDot follD = __hlp.getFollower(type);

                pDotRelation(precD);
                fDotRelation(follD);
            }
            ~typeManager()
            {
                if (parent != null)
                {
                    fUnsubscribe();
                    pUnsubscribe();
                    parent = null;
                }
                currentDate = null;
                expectedDate = null;
                pDate = fDate = null;
            }
            #endregion
            #region Initializers
            private void init_functions()
            {
                currentDate = () => pDate().AddDays(_delay);
                expectedDate = (DateTime precursorDate, double delay) => precursorDate.AddDays(delay);
            }
            #endregion
            #region handlers self
            private void onTypePropertyWrite(eLnkType newValue)
            {
                DateTime oldDate = date;
                eLnkDot oldDot = fDot;
                eLnkType oldType = _type;


                _type = newValue;


                eLnkDot precD = __hlp.getPrecursor(newValue);
                eLnkDot follD = __hlp.getFollower(newValue);

                bool prcChanged = false;
                bool flwChanged = false;

                if (precD != pDot) prcChanged = pDotRelation(precD);
                if(follD != fDot) flwChanged = fDotRelation(follD);

                DateTime newDate = date;
                bool dateChanged = oldDate != newDate;


                EA_flwChanges args = new EA_flwChanges();
                if (dateChanged)
                {
                    args.setDate(oldDate, newDate);
                    updateLimitDate();
                }
                if (flwChanged) args.setDot(oldDot, fDot);


                updateLimitType();


                if (dateChanged || flwChanged) parent.onFollowerUpdate(args);
                if (dateChanged) onMngDateChanged(oldDate, newDate);


                parent.onTypeChanged(new EA_valueChange<eLnkType>(oldType, _type));
            }

            #endregion
            #region handlers
            public void setDelay(double delay)
            {
                if (delay == _delay) return;

                DateTime temp = date;
                _delay = (delay < 0) ? 0 : delay;
                DateTime New = date;

                if (New != temp)
                {
                    updateLimitDate();
                    parent.onFollowerUpdate(new EA_flwChanges(temp, New));
                    onMngDateChanged(temp, New);
                }
            }
            private void handler_precursorDateChanged(object sender, EA_valueChange<DateTime> e)
            {
                DateTime New = expectedDate(e.newValue, _delay);
                DateTime current = expectedDate(e.oldValue, _delay);

                if(New != current)
                {
                    updateLimitDate();
                    parent.onFollowerUpdate(new EA_flwChanges(current, New));
                    onMngDateChanged(current, New);
                }
            }
            #endregion
            #region methods
            public void clear()
            {
                pUnsubscribe();
                fUnsubscribe();
                currentDate = null;
                expectedDate = null;
                pDate = fDate = null;
                parent = null;
            }
            #endregion
            #region service
            #region limit
            private void updateLimitDate()
            { parent.lnkLimit.date = date; }
            private void updateLimitType()
            { parent.lnkLimit.type = link.lnk2lim(_type); }
            #endregion
            #region date
            private void onMngDateChanged(DateTime Old, DateTime New)
            {
                parent.onDateChanged(new EA_valueChange<DateTime>(Old, New));
            }
            #endregion
            #region dot managers
            private bool pDotRelation(eLnkDot dot)
            {
                pDot = dot;

                pUnsubscribe();
                if (dot == eLnkDot.Start)
                {
                    pDate = () => parent._precursor.getStart();
                    parent._precursor.event_startChanged += handler_precursorDateChanged;
                }
                else
                {
                    pDate = () => parent._precursor.getFinish();
                    parent._precursor.event_finishChanged += handler_precursorDateChanged;
                }
                return true;
            }
            private bool fDotRelation(eLnkDot dot)
            {
                fDot = dot;

                fUnsubscribe();
                if (dot == eLnkDot.Start)
                {
                    fDate = () => parent._follower.getStart();
                    parent._follower.event_startChanged += parent.handler_followerDateChanged;
                }
                else
                {
                    fDate = () => parent._follower.getFinish();
                    parent._follower.event_finishChanged += parent.handler_followerDateChanged;
                }
                return true;
            }
            private void pUnsubscribe()
            {
                parent._precursor.event_finishChanged -= handler_precursorDateChanged;
                parent._precursor.event_startChanged -= handler_precursorDateChanged;
            }
            private void fUnsubscribe()
            {
                parent._follower.event_finishChanged -= parent.handler_followerDateChanged;
                parent._follower.event_startChanged -= parent.handler_followerDateChanged;
            }
            #endregion
            #endregion
        }

        #endregion
        #region static
        public static eFLim lnk2lim(eLnkType type)
        {
            switch(type)
            {
                case eLnkType.FinishFinish:
                case eLnkType.FinishStart:
                case eLnkType.StartFinish:
                case eLnkType.StartStart:
                    return eFLim.Fixed;
            }
            throw new Exception("Неверное значение параметра <eLnkType type> функции lnk2lim класса link.");
        }
        #endregion
    }
}
