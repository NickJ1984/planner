using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

using lib.types;
using lib.delegates;

using lib.dot.iFaces;

using lib.limits.iFaces;

using lib.project.iFaces;

namespace lib.limits.classes
{
    public class period_localLimit
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
        Func<DateTime, DateTime> fncLocalCheck;
        Func<DateTime, DateTime> fncOuterCheck;
        Func<DateTime, DateTime> fncFirstCheck;
        Func<DateTime, DateTime> fncSecondCheck;
        #endregion
        #region actions
        #endregion
        #region flags
        private bool isStartMaster = true;
        #endregion
        #region helpers
        private readonly dummyCheck dmyChk = new dummyCheck();
        private readonly Func<double> durNull = () => 0;
        #endregion
        #region expTree variables
        ParameterExpression pDate = Expression.Parameter(typeof(DateTime));
        ParameterExpression pResult = Expression.Parameter(typeof(DateTime));

        Expression<Func<DateTime, DateTime>> eFncEarlier;
        Expression<Func<DateTime, DateTime>> eFncLater;
        Expression<Func<DateTime, DateTime>> eFncNotEarlier;
        Expression<Func<DateTime, DateTime>> eFncNotLater;
        Expression<Func<DateTime, DateTime>> eFncFixed;

        InvocationExpression eInvoke;
        #endregion
        #endregion
        #region Properties
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
                    _outerLimit = (value == null) ? dmyChk : value;

                    ev_outerLimitChanged(this, new EventArgs());
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
        public period_localLimit(IDot start, IDot finish)
        {
            this.start = start;
            this.finish = finish;
            _outerLimit = dmyChk;
            _getDuration = durNull;
            limitType = defaultLimit;

            
            init_outerLimit();
            init_expTreeParameters();

            dcdSlave = new dotCheckDate(slaveCheck);
            dcdMaster = new dotCheckDate(masterCheck);
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
            master.date = master.date;
        }
        private void slaveUpdate()
        {
            slave.date = slave.date;
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

        #endregion
        #region Handlers
        public void handler_durationChanged(object sender, eventArgs_valueChange<double> e)
        {
            slaveUpdate();
        }
        #endregion
        #region Handlers self
        private void init_eventsInternal()
        {
            ev_durationParentChanged += __handler_durationParentChanged;
            ev_limitTypeChanged += __handler_limitTypeChanged;
            ev_outerLimitChanged += __handler_outerLimitChanged;
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

            public dotCheckDate(Func<DateTime, DateTime> controlMethod)
            {
                this.controlMethod = controlMethod;
            }
            public DateTime checkDate(DateTime Date)
            {
                return controlMethod(Date);
            }
        }
        protected class dummyCheck : ILimit_check
        {
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
