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
        event EventHandler<EA_value<identity>> event_linkDeleted;
    }


    public class link : ILink
    {
        #region Variables
        private IProject _project;
        private identity _ID;
        private ITask _precursor;
        private ITask _follower;

        private eLnkState _state;
        private eLnkType _type;
        private eLnkDot _tDotPrecursor;
        private eLnkDot _tDotFollower;

        private double _delay;

        private DateTime _dateControl;

        private limit lnkLimit;

        Func<DateTime> fPDate;
        Func<DateTime> fFDate;



        #endregion
        #region Properties
        public DateTime dateManaged
        {
            get { return _dateControl; }
            private set
            {
                if (value != _dateControl)
                {
                    DateTime temp = _dateControl;
                    _dateControl = value;
                    onManagedDateChanged(new EA_valueChange<DateTime>(temp, _dateControl));
                }
                
            }
        }
        public double delay
        {
            get { return _delay; }
            set { handler_delayChanged(_delay, value); }
        }
        #endregion
        #region Events
        public event EventHandler<EA_valueChange<DateTime>> event_dateManagedChanged;
        public event EventHandler<EA_valueChange<eLnkType>> event_typeLinkChanged;
        public event EventHandler<EA_value<eLnkDot>> event_followerDotChanged;
        public event EventHandler<EA_value<identity>> event_linkDeleted;
        #endregion
        #region Constructors
        public link(IProject Project, ITask Precursor, ITask Follower, eLnkType type, double delay = 0)
        {
            _ID = new identity(eEntity.link);

            _project = Project;
            _precursor = Precursor;
            _follower = Follower;
        }
        #endregion
        #region Handlers
        private void onFollowerDotChange()
        {
            EventHandler<EA_value<eLnkDot>> handler = event_followerDotChanged;
            if (handler != null) handler(this, new EA_value<eLnkDot>(_tDotFollower));
        }
        private void onManagedDateChanged(EA_valueChange<DateTime> args)
        {
            EventHandler<EA_valueChange<DateTime>> handler = event_dateManagedChanged;
            if (handler != null) handler(this, args);
        }
        private void handler_precursorDateChanged(object sender, EA_valueChange<DateTime> e)
        {
            dateManaged = e.newValue.AddDays(_delay);
        }
        private void handler_followerDateChanged(object sender, EA_valueChange<DateTime> e)
        {
            checkLinkState();
        }
        private void handler_delayChanged(double Old, double New)
        {
            if (Old == New) return;
            New = (New < 0) ? 0 : New;
            dateManaged = fPDate().AddDays(New);
        }
        #endregion
        #region Methods
        public void setTypeLink(eLnkType type)
        {
            throw new NotImplementedException();
        }
        public void setDelay(double delay)
        {
            throw new NotImplementedException();
        }
        public void deleteLink()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Service
        private void onTypePropertyWrite(eLnkType Type)
        {
            if (Type == _type) return;

            


        }
        private void updateDots(eLnkType Type)
        {
            eLnkDot tmpPDot = _tDotPrecursor;
            eLnkDot tmpFDot = _tDotFollower;

            DateTime tmpPDate = fPDate();
            _tDotFollower = __hlp.getFollower(Type);
            _tDotPrecursor = __hlp.getPrecursor(Type);

            if (_tDotPrecursor != tmpPDot) updatePrecursorDot(tmpPDate);
            if (_tDotFollower != tmpFDot) updateFollowerDot();

        }
        private void updatePrecursorDot(DateTime oldDate)
        {
            if(_tDotPrecursor == eLnkDot.Start)
            {
                _precursor.event_finishChanged -= handler_precursorDateChanged;
                _precursor.event_startChanged -= handler_precursorDateChanged;
                _precursor.event_startChanged += handler_precursorDateChanged;

                fPDate = () => _precursor.getStart();
            }
            else
            {
                _precursor.event_finishChanged -= handler_precursorDateChanged;
                _precursor.event_startChanged -= handler_precursorDateChanged;
                _precursor.event_finishChanged += handler_precursorDateChanged;

                fPDate = () => _precursor.getFinish();
            }
            DateTime newDate = fPDate();
            if (newDate != oldDate) dateManaged = newDate.AddDays(_delay);
        }
        private void updateFollowerDot()
        {
            if(_tDotFollower == eLnkDot.Start)
            {
                _follower.event_finishChanged -= handler_followerDateChanged;
                _follower.event_startChanged -= handler_followerDateChanged;
                _follower.event_startChanged += handler_followerDateChanged;

                fFDate = () => _follower.getStart();
            }
            else
            {
                _follower.event_finishChanged -= handler_followerDateChanged;
                _follower.event_startChanged -= handler_followerDateChanged;
                _follower.event_finishChanged += handler_followerDateChanged;

                fFDate = () => _follower.getFinish();
            }
            onFollowerDotChange();
        }
        private void checkLinkState()
        {
            DateTime fDate = fFDate();
            DateTime result = lnkLimit.checkDate(fDate);
            if (fDate == result) _state = eLnkState.inTime;
            else if (fDate > result) _state = eLnkState.later;
            else if (fDate < result) _state = eLnkState.early;
        }
        
        
        #endregion
        #region Interface secondary methods
        public identity getIDobject()
        {
            throw new NotImplementedException();
        }
        public ITask getPrecursor()
        {
            throw new NotImplementedException();
        }

        public ITask getFollower()
        {
            throw new NotImplementedException();
        }

        public eLnkState getStateLink()
        {
            throw new NotImplementedException();
        }

        public eLnkType getTypeLink()
        {
            throw new NotImplementedException();
        }
        public double getDelay()
        {
            throw new NotImplementedException();
        }
        public ILimitCheck getCheckDate()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
