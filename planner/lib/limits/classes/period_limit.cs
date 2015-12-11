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

        private IProject_dates projectInformation;

        private DateTime _limitDate;

        private IDot start;
        private IDot finish;
        private IDot master;
        private IDot slave;

        private ILimit_check _outerLimit;
        private IPeriod_duration _ipdDuration;

        private readonly dotCheckDate dcdSlave;
        private readonly dotCheckDate dcdMaster;

        #region internal methods variables
        private bool __imv_funcFSInvert;
        #endregion

        #region functions
        private Func<double> fncDuration;

        private Func<DateTime> fncProjectStart;
        private Func<DateTime> fncProjectFinish;
        private Func<DateTime> fncExpLimit;

        private Func<DateTime, DateTime> fncLocalCheck;
        private Func<DateTime, DateTime> fncOuterCheck;
        private Func<DateTime, DateTime> fncFirstCheck;
        private Func<DateTime, DateTime> fncSecondCheck;
        private Func<DateTime, DateTime> fncSlaveCheck;
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
        private ParameterExpression pLimit = Expression.Parameter(typeof(DateTime));
        private ParameterExpression pResult = Expression.Parameter(typeof(DateTime));
        private ParameterExpression pDuration = Expression.Parameter(typeof(double));

        private Expression eCompare;

        private Expression<Func<DateTime>> eFncExpLimit;
        private Expression<Func<double>> eFncGetDuration;

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

        protected double duration
        {
            get { return fncDuration(); }
        }
        protected DateTime prjStart { get { return fncProjectStart(); } }
        protected DateTime prjFinish { get { return fncProjectFinish(); } }
        #endregion
        #region Delegates
        
        #endregion
        #region Constructors
        #region constructors
        public period_localLimit(IDot start, IDot finish, e_tskLimit lType)
        {
            init_eventsInternal();

            dcdSlave = new dotCheckDate(slaveCheck);
            dcdMaster = new dotCheckDate(masterCheck);

            _outerLimit = dmyChk;
            fncDuration = durNull;

            init_outerLimit();
            init_expTreeParameters();

            connectProject(null);

            this.start = start;
            this.finish = finish;
            _limitDate = start.date;

            limitType = lType;
        }
        public period_localLimit(IDot start, IDot finish)
            : this(start, finish, defaultLimit)
        { }
        #endregion
        #region initializers
        private void init_outerLimit()
        {
            fncOuterCheck = outerLimit.checkDate;
        }
        private void init_eventsInternal()
        {
            ev_limitTypeChanged += __handler_limitTypeChanged;
            ev_outerLimitChanged += __handler_outerLimitChanged;
            ev_limitDateChanged += __handler_limitDateChanged;
        }
        private void init_expTreeParameters()
        {
            eFncExpLimit = () => fncExpLimit();
            eFncGetDuration = () => fncDuration();
        }
        #endregion
        #endregion
        #region Methods
        #region connectors
        public void connectDuration(IPeriod_duration duration)
        {
            if(_ipdDuration != null)
            { _ipdDuration.event_durationChanged -= handler_durationChanged; }

            _ipdDuration = duration;

            if (duration != null)
            { _ipdDuration.event_durationChanged += handler_durationChanged; }

            fncDuration = (_ipdDuration != null) ? _ipdDuration.getDuration : durNull;

            slaveUpdate();
        }
        public void connectProject(IProject_dates pInfo)
        {
            if (projectInformation != null)
            {
                projectInformation.event_startChanged -= handler_projectStartChanged;
                projectInformation.event_finishChanged -= handler_projectFinishChanged;
            }

            projectInformation = pInfo;

            if (pInfo != null)
            {
                pInfo.event_startChanged += handler_projectStartChanged;
                pInfo.event_finishChanged += handler_projectFinishChanged;

                fncProjectStart = () => projectInformation.start;
                fncProjectFinish = () => projectInformation.finish;
            }
            else
            {
                fncProjectStart = () => start.date;
                fncProjectFinish = () => finish.date;
            }
            __prp_limitType_write(_limitType);

            if (limitType == e_tskLimit.Earlier || limitType == e_tskLimit.Later)
            {
                masterUpdate();
                slaveUpdate();
            }
        }
        #endregion
        #region checkers
        private DateTime masterCheck(DateTime Date)
        {
            DateTime result = fncFirstCheck(Date);
            return fncSecondCheck(result);
        }
        private DateTime slaveCheck(DateTime Date)
        {
            return fncSlaveCheck(master.date);
        }
        #endregion
        #endregion
        #region Service
        #region master & slave
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

            __eTree_makeSlaveCheck();
            slave.dotLimitCheck = dcdSlave;
            master.dotLimitCheck = dcdMaster;
        }
        #endregion
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
        private event EventHandler ev_limitTypeChanged;
        private event EventHandler ev_limitDateChanged;

        public event EventHandler<eventArgs_valueChange<DateTime>> event_limitDateChanged;
        public event EventHandler<eventArgs_valueChange<e_tskLimit>> event_limitTypeChanged;

        #endregion
        #region Handlers
        private void handler_durationChanged(object sender, eventArgs_valueChange<double> e)
        {
            slaveUpdate();
        }
        private void handler_projectStartChanged(object sender, eventArgs_valueChange<DateTime> e)
        {
            masterUpdate();
            slaveUpdate();
        }
        private void handler_projectFinishChanged(object sender, eventArgs_valueChange<DateTime> e)
        {
            masterUpdate();
            slaveUpdate();
        }
        #endregion
        #region Handlers self
        #region inner events handlers
        private void __handler_durationParentChanged(object sender, EventArgs e)
        {
            slaveUpdate();
        }
        private void __handler_limitTypeChanged(object sender, EventArgs e)
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
        private void __handler_limitDateChanged(object sender, EventArgs e)
        {
            masterUpdate();
            slaveUpdate();
        }

        #endregion
        #region outer events handlers
        private void onOuterLimitUpdate(object sender, EventArgs e)
        {
            if (_limitType != e_tskLimit.finishFixed && _limitType != e_tskLimit.startFixed)
            {
                masterUpdate();
                slaveUpdate();
            }
        }
        #endregion
        #region outer events invokes
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
        #endregion
        #region properties handlers
        private void onOuterLimitObjectChange(ILimit_check ilcObject)
        {
            _outerLimit.event_update -= onOuterLimitUpdate;

            _outerLimit = (ilcObject == null) ? dmyChk : ilcObject;

            _outerLimit.event_update += onOuterLimitUpdate;

            ev_outerLimitChanged(this, new EventArgs());
        }
        private void __prp_limitType_write(e_tskLimit Value)
        {
            e_tskLimit temp = _limitType;
            _limitType = Value;

            switch (Value)
            {
                case e_tskLimit.Earlier:
                    setMasterStart(true);

                    fncExpLimit = fncProjectStart;
                    __eTree_makeBinary(ExpressionType.NotEqual, pDate, pDate);
                    __eTree_generateLocalCheck(Value);

                    funcFSInvert(true);
                    break;


                case e_tskLimit.startFixed:
                    setMasterStart(true);

                    fncExpLimit = () => limitDate;
                    __eTree_makeBinary(ExpressionType.NotEqual, pDate, pDate);
                    __eTree_generateLocalCheck(Value);

                    funcFSInvert(false);
                    break;


                case e_tskLimit.startNotEarlier:
                    setMasterStart(true);

                    fncExpLimit = () => limitDate;
                    __eTree_makeBinary(ExpressionType.GreaterThanOrEqual, pDate, pLimit);
                    __eTree_generateLocalCheck(Value);

                    funcFSInvert(false);
                    break;


                case e_tskLimit.startNotLater:
                    setMasterStart(true);

                    fncExpLimit = () => limitDate;
                    __eTree_makeBinary(ExpressionType.LessThanOrEqual, pDate, pLimit);
                    __eTree_generateLocalCheck(Value);

                    funcFSInvert(false);
                    break;


                case e_tskLimit.Later:
                    setMasterStart(false);

                    fncExpLimit = fncProjectFinish;
                    __eTree_makeBinary(ExpressionType.NotEqual, pDate, pDate);
                    __eTree_generateLocalCheck(Value);

                    funcFSInvert(true);
                    break;


                case e_tskLimit.finishFixed:
                    setMasterStart(false);

                    fncExpLimit = () => limitDate;
                    __eTree_makeBinary(ExpressionType.NotEqual, pDate, pDate);
                    __eTree_generateLocalCheck(Value);

                    funcFSInvert(false);
                    break;


                case e_tskLimit.finishNotEarlier:
                    setMasterStart(false);

                    fncExpLimit = () => limitDate;
                    __eTree_makeBinary(ExpressionType.GreaterThanOrEqual, pDate, pLimit);
                    __eTree_generateLocalCheck(Value);

                    funcFSInvert(false);
                    break;


                case e_tskLimit.finishNotLater:
                    setMasterStart(false);

                    fncExpLimit = () => limitDate;
                    __eTree_makeBinary(ExpressionType.LessThanOrEqual, pDate, pLimit);
                    __eTree_generateLocalCheck(Value);

                    funcFSInvert(false);
                    break;
            }

            ev_limitTypeChanged(this, new EventArgs());

            onLimitTypeChange(new eventArgs_valueChange<e_tskLimit>(temp, _limitType));
        }
        #endregion
        #endregion
        #region Expression trees
        private void __eTree_generateLocalCheck(e_tskLimit Value)
        {
            BlockExpression block = Expression.Block(
                typeof(DateTime),
                new[] { pResult, pLimit },

                Expression.IfThenElse(eCompare,
                Expression.Assign(pResult, pDate),
                Expression.Assign(pResult, Expression.Invoke(eFncExpLimit))),
                pResult
                );

            fncLocalCheck = Expression.Lambda<Func<DateTime, DateTime>>(block, pDate).Compile();
        }
        private void __eTree_makeSlaveCheck()
        {
            Expression eNegate = Expression.Negate(pDuration);
            Expression eCall = Expression.Call(pDate, typeof(DateTime).GetMethod("AddDays"), (isStartMaster) ? pDuration : eNegate);

            BlockExpression block = Expression.Block(
                typeof(DateTime),
                new[] {pResult, pDuration},
                Expression.Assign(pDuration, Expression.Invoke(eFncGetDuration)),
                Expression.Assign(pResult, eCall),
                pResult
                );

            fncSlaveCheck = Expression.Lambda<Func<DateTime, DateTime>>(block, pDate).Compile();
        }
        private void __eTree_makeBinary(ExpressionType et, Expression left, Expression right)
        {
            eCompare = Expression.MakeBinary(et, left, right);
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
