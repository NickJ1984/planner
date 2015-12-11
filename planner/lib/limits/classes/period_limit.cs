using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

using lib.types;
using lib.service;
using lib.delegates;

using lib.dot.iFaces;

using lib.limits.iFaces;

using lib.project.iFaces;
using lib.period.iFaces;

namespace lib.limits.classes
{
    public class period_localLimit : IPeriod_localLimit
    {
        #region Variables
        private e_tskLimit _limitType;
        private const e_tskLimit defaultLimit = e_tskLimit.Earlier;

        private IProjectInfo projInfo;

        private DateTime _limitDate;

        private IDot start;
        private IDot finish;
        private IDot master;
        private IDot slave;

        private ILimit_check _outerLimit;

        private readonly dotCheckDate dcdSlave;
        private readonly dotCheckDate dcdMaster;

        #region internal methods variables
        private bool __imv_funcFSInvert;
        #endregion

        #region functions
        private Func<DateTime, DateTime> fncLocalCheck;
        private Func<DateTime, DateTime> fncOuterCheck;
        private Func<DateTime, DateTime> fncFirstCheck;
        private Func<DateTime, DateTime> fncSecondCheck;
        #endregion

        #region flags
        private bool isStartMaster = true;
        #endregion

        #region helpers
        private readonly dummyCheck dmyChk = new dummyCheck();
        private readonly Func<double> durNull = () => 0;
        #endregion

        #region expTree variables
        private ParameterExpression pDate = Expression.Parameter(typeof(DateTime));
        private ParameterExpression pResult = Expression.Parameter(typeof(DateTime));

        private Expression<Func<DateTime, DateTime>> eFncEarlier;
        private Expression<Func<DateTime, DateTime>> eFncLater;
        private Expression<Func<DateTime, DateTime>> eFncNotEarlier;
        private Expression<Func<DateTime, DateTime>> eFncNotLater;
        private Expression<Func<DateTime, DateTime>> eFncFixed;

        private InvocationExpression eInvoke;
        #endregion

        #endregion
        #region Properties
        public DateTime limitDate
        {
            get { return _limitDate; }
            set
            {
                if(_limitDate != value)
                {
                    DateTime temp = _limitDate;
                    _limitDate = value;

                    ev_limitDateChanged(this, new EventArgs());
                    onLimitDateChange(new eventArgs_valueChange<DateTime>(temp, _limitDate));
                }
            }
        }

        public e_tskLimit limitType
        {
            get { return _limitType; }
            set { __prp_limitType_write(value); }
        }
        public ILimit_check outerLimit
        {
            get { return _outerLimit; }
            set
            {
                if (_outerLimit != value)
                {
                    onOuterLimitObjectChange(value);
                }
            }
        }
        public Func<double> getDuration
        {
            get { return _getDuration; }
            set
            {
                if (_getDuration != value)
                {
                    _getDuration = (value == null) ? durNull : value;
                    ev_durationParentChanged(this, new EventArgs());
                }
            }
        }

        protected double duration
        {
            get { return getDuration(); }
        }
        
        #endregion
        #region Delegates
        private Func<double> _getDuration;
        #endregion
        #region Constructors
        public period_localLimit(IProjectInfo pInfo, IDot start, IDot finish, e_tskLimit lType)
        {
            init_projectInfo(pInfo);
            this.start = start;
            this.finish = finish;
            _limitDate = start.date;
            _outerLimit = dmyChk;
            _getDuration = durNull;
            
            init_outerLimit();
            init_expTreeParameters();
            init_eventsInternal();

            dcdSlave = new dotCheckDate(slaveCheck);
            dcdMaster = new dotCheckDate(masterCheck);

            limitType = lType;
        }
        public period_localLimit(IProjectInfo pInfo, IDot start, IDot finish)
            :this(pInfo, start, finish, defaultLimit)
        { }
        private void init_projectInfo(IProjectInfo pInfo)
        {
            projInfo = pInfo;

            projInfo.event_startChanged += handler_projectStartChanged;
            projInfo.event_finishChanged += handler_projectFinishChanged;
        }
        #endregion
        #region Methods
        private DateTime masterCheck(DateTime Date)
        {
            DateTime result = fncFirstCheck(Date);
            return fncSecondCheck(result);
        }
        private DateTime slaveCheck(DateTime Date)
        {
            return (isStartMaster) ? master.date.AddDays(duration) : master.date.AddDays(-duration);
        }
        #endregion
        #region Service
        private void init_outerLimit()
        {
            fncOuterCheck = outerLimit.checkDate;
        }
        private void masterUpdate()
        {
            dcdMaster.update();
        }
        private void slaveUpdate()
        {
            dcdSlave.update();
        }
        private void setMasterStart(bool start)
        {
            if(start)
            {
                master = this.start;
                isStartMaster = true;
                slave = finish;
            }
            else
            {
                master = finish;
                isStartMaster = false;
                slave = this.start; 
            }

            slave.dotLimitCheck = dcdSlave;
            master.dotLimitCheck = dcdMaster;
        }
        private void funcFSInvert(bool invert)
        {
            __imv_funcFSInvert = invert;
            if (invert)
            {
                fncFirstCheck = fncLocalCheck;
                fncSecondCheck = fncOuterCheck;
            }
            else
            {
                fncFirstCheck = fncOuterCheck;
                fncSecondCheck = fncLocalCheck;
            }
        }
        private void funcFSInvert()
        {
            funcFSInvert(__imv_funcFSInvert);
        }
        #endregion
        #region Events

        private event EventHandler ev_outerLimitChanged;
        private event EventHandler ev_durationParentChanged;
        private event EventHandler ev_limitTypeChanged;
        private event EventHandler ev_limitDateChanged;

        public event EventHandler<eventArgs_valueChange<DateTime>> event_limitDateChanged;
        public event EventHandler<eventArgs_valueChange<e_tskLimit>> event_limitTypeChanged;

        #endregion
        #region Handlers
        public void handler_durationChanged(object sender, eventArgs_valueChange<double> e)
        {
            slaveUpdate();
        }
        public void connectDuration(IPeriod_duration duration)
        {
            getDuration = duration.getDuration;
            duration.event_durationChanged += handler_durationChanged;
        }

        private void handler_projectStartChanged(object sender, eventArgs_valueChange<DateTime> e)
        {
            if (limitType == e_tskLimit.Earlier)
            {
                masterUpdate();
                slaveUpdate();
            }
        }
        private void handler_projectFinishChanged(object sender, eventArgs_valueChange<DateTime> e)
        {
            if (limitType == e_tskLimit.Later)
            {
                masterUpdate();
                slaveUpdate();
            }
        }
        #endregion
        #region Handlers self
        private void init_eventsInternal()
        {
            ev_durationParentChanged += __handler_durationParentChanged;
            ev_limitTypeChanged += __handler_limitTypeChanged;
            ev_outerLimitChanged += __handler_outerLimitChanged;
            ev_limitDateChanged += __handler_limitDateChanged;
        }


        private void onOuterLimitObjectChange(ILimit_check ilcObject)
        {
            _outerLimit.event_update -= onOuterLimitUpdate;

            _outerLimit = (ilcObject == null) ? dmyChk : ilcObject;

            _outerLimit.event_update += onOuterLimitUpdate;

            ev_outerLimitChanged(this, new EventArgs());
        }
        private void onOuterLimitUpdate(object sender, EventArgs e)
        {
            if (_limitType != e_tskLimit.finishFixed && _limitType != e_tskLimit.startFixed)
            {
                masterUpdate();
                slaveUpdate();
            }
        }
        private void onLimitDateChange(eventArgs_valueChange<DateTime> args)
        {
            EventHandler<eventArgs_valueChange<DateTime>> handler = event_limitDateChanged;

            if (handler != null) handler(this, args);
        }

        private void onLimitTypeChange(eventArgs_valueChange<e_tskLimit> args)
        {
            EventHandler<eventArgs_valueChange<e_tskLimit>> handler = event_limitTypeChanged;

            if (handler != null) handler(this, args);
        }

        private void __handler_limitDateChanged(object sender, EventArgs e)
        {
            masterUpdate();
            slaveUpdate();
        }

        private void __handler_outerLimitChanged(object sender, EventArgs e)
        {
            init_outerLimit();
            funcFSInvert();

            masterUpdate();
            slaveUpdate();
        }

        private void __handler_limitTypeChanged(object sender, EventArgs e)
        {
            masterUpdate();
            slaveUpdate();
        }

        private void __handler_durationParentChanged(object sender, EventArgs e)
        {
            slaveUpdate();
        }

        private void __prp_limitType_write(e_tskLimit Value)
        {
            if (_limitType == Value) return;

            e_tskLimit temp = _limitType;
            _limitType = Value;

            switch(Value)
            {
                case e_tskLimit.Earlier:
                    setMasterStart(true);
                    eTree_generateLocalCheck(Value);
                    funcFSInvert(true);
                    break;

                case e_tskLimit.startFixed:
                case e_tskLimit.startNotEarlier:
                case e_tskLimit.startNotLater:
                    setMasterStart(true);
                    eTree_generateLocalCheck(Value);
                    funcFSInvert(false);
                    break;


                case e_tskLimit.Later:
                    setMasterStart(false);
                    eTree_generateLocalCheck(Value);
                    funcFSInvert(true);
                    break;

                case e_tskLimit.finishFixed:
                case e_tskLimit.finishNotEarlier:
                case e_tskLimit.finishNotLater:
                    setMasterStart(false);
                    eTree_generateLocalCheck(Value);
                    funcFSInvert(false);
                    break;
            }

            ev_limitTypeChanged(this, new EventArgs());

            onLimitTypeChange(new eventArgs_valueChange<e_tskLimit>(temp, _limitType));
        }
        #endregion
        #region Expression trees
        private void init_expTreeParameters()
        {
            eFncEarlier = (date) => projInfo.start;
            eFncLater = (date) => projInfo.finish;
            eFncNotEarlier = (date) => (date >= _limitDate) ? date : _limitDate;
            eFncNotLater = (date) => (date <= _limitDate) ? date : _limitDate;
            eFncFixed = (date) => _limitDate;
        }
        
        private void eTree_generateLocalCheck(e_tskLimit Value)
        {
            switch(Value)
            {
                case e_tskLimit.Earlier:
                    __eTree_chngInvokeFunc(eFncEarlier);
                    break;

                case e_tskLimit.Later:
                    __eTree_chngInvokeFunc(eFncLater);
                    break;

                case e_tskLimit.finishFixed:
                case e_tskLimit.startFixed:
                    __eTree_chngInvokeFunc(eFncFixed);
                    break;

                case e_tskLimit.finishNotEarlier:
                case e_tskLimit.startNotEarlier:
                    __eTree_chngInvokeFunc(eFncNotEarlier);
                    break;

                case e_tskLimit.finishNotLater:
                case e_tskLimit.startNotLater:
                    __eTree_chngInvokeFunc(eFncNotLater);
                    break;
            }

            BlockExpression block = Expression.Block(
                typeof(DateTime),
                new[] {pResult},
                Expression.Assign(pResult, eInvoke),
                pResult
                );

            fncLocalCheck = Expression.Lambda<Func<DateTime, DateTime>>(block, pDate).Compile();
        }
        private void __eTree_chngInvokeFunc(Expression<Func<DateTime, DateTime>> expFunc)
        {
            eInvoke = Expression.Invoke(expFunc, pDate);
        }
        #endregion
        #region Overrides
        #endregion
        #region internal entities
        protected class dotCheckDate : ILimit_check
        {
            public delegate DateTime d_checkDate(DateTime Date);
            public Func<DateTime, DateTime> controlMethod;

            public event EventHandler event_update;

            public dotCheckDate(Func<DateTime, DateTime> controlMethod)
            {
                this.controlMethod = controlMethod;
            }
            public DateTime checkDate(DateTime Date)
            {
                return controlMethod(Date);
            }

            public void update()
            {
                if (event_update != null) event_update(this, new EventArgs());
            }
        }
        protected class dummyCheck : ILimit_check
        {
            public event EventHandler event_update;

            public DateTime checkDate(DateTime Date)
            {
                return Date;
            }
        }
        #endregion
        #region referred interface implementation
        #endregion
        #region self interface implementation
        #endregion
    }
}
