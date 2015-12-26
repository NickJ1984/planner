using System;

using System.Linq.Expressions;

using lib.types;
using lib.dot.archive.iFaces;
using lib.delegates;

namespace lib.dot.archive.classes
{
    public class dot_Limit : IDot_Limit
    {
        #region Variables
        private e_dot_Limit tmpLim;

        private bool _useLeft;
        private bool _useRight;
        private bool _dotSet;
        private bool _enabled;
        private e_dot_Limit _limit;

        private readonly DateTime initDate;
        private DateTime _limitMin;//используется при состояниях notEarlier, inDate
        private DateTime _limitMax;//используется при состоянии notLater
        private DateTime _eDate;        
        #endregion
        #region Properties
        public e_dot_Limit limit
        {
            get { return _limit; }
            set
            {
                if (_limit == value || !enabled) return;

                e_dot_Limit temp = _limit;
                _limit = value;
                __limitCompare_Init();
                onLimitChange(new eventArgs_valueChange<e_dot_Limit>(temp, _limit));
            }
        }

        public DateTime limitMin
        {
            get { return _limitMin; }
            set
            {
                if (_limitMin == value) return;
                DateTime temp = _limitMin;
                _limitMin = value;

                checkNeedToCompare(true);
                onLimitDateChanged(new eventArgs_valuesChange<DateTime>("limitMin", temp, value));
            }
        }
        public DateTime limitMax
        {
            get { return _limitMax; }

            set
            {
                if (_limitMax == value) return;
                DateTime temp = _limitMax;
                _limitMax = value;

                checkNeedToCompare(false);
                onLimitDateChanged(new eventArgs_valuesChange<DateTime>("limitMax", temp, value));
            }
        }
        public bool isLimitMin { get { return _useLeft; } }
        public bool isLimitMax { get { return _useRight; } }
        public DateTime estimatedDate { get { return _eDate; } }
        public double range { get { return _limitMax.Subtract(limitMin).Days; } }

        public bool enabled
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion
        #region Delegates

        internal d_valueComparator<DateTime> limitCompare;

        protected d_valueReference<DateTime> dotCurrentDate;

        private Func<DateTime, DateTime, DateTime, DateTime> __limitCMP;

        #endregion
        #region Constructors
        public dot_Limit()
        {
            _enabled = true;
            initDate = new DateTime(1900, 1, 1);
            //_limit = e_dot_Limit.None;
            _dotSet = false;
            _limitMin = _limitMax = _eDate = initDate;

            limitCompare = __handler_LimitComparator;

            dotCurrentDate = () => limitMin;
            __limitCompare_Init();
        }
        #endregion
        #region Methods
        public DateTime checkDate(DateTime date)
        {
            return __limitCMP(limitMin, limitMax, date);
        }
        #endregion
        #region Service
        private void checkEstimated()
        {
            DateTime temp = _eDate;
            _eDate = checkDate(dotCurrentDate());

            if (_eDate != temp)
                onEstimatedDateChange(new eventArgs_valueChange<DateTime>(temp, _eDate));
        }
        private void __setLR(bool left, bool right)
        {
            _useLeft = left;
            _useRight = right;
        }
        private void __limitCompare_Init()
        {
            ParameterExpression pLimitMin = Expression.Parameter(typeof(DateTime));
            ParameterExpression pLimitMax = Expression.Parameter(typeof(DateTime));
            ParameterExpression pDate = Expression.Parameter(typeof(DateTime));
            ParameterExpression pResult = Expression.Parameter(typeof(DateTime));
            BlockExpression block = null;

            Expression eComparisonGreater = Expression.IfThenElse(
                        Expression.GreaterThanOrEqual(pDate, pLimitMin),
                        Expression.Assign(pResult, pDate),
                        Expression.Assign(pResult, pLimitMin)
                        );
            Expression eComparisonLess = Expression.IfThenElse(
                        Expression.LessThanOrEqual(pDate, pLimitMax),
                        Expression.Assign(pResult, pDate),
                        Expression.Assign(pResult, pLimitMax)
                        );

            Expression eMinMax = Expression.IfThenElse(
                    Expression.GreaterThan(pDate, pLimitMax),
                    Expression.Assign(pResult, pLimitMax),
                    Expression.Assign(pResult, pLimitMin)
                    );

            Expression eComparisonRange = Expression.IfThenElse(
                Expression.AndAlso(
                    Expression.GreaterThanOrEqual(pDate, pLimitMin),
                    Expression.LessThanOrEqual(pDate, pLimitMax)
                    ),
                Expression.Assign(pResult, pDate),
                eMinMax
                );

            switch (limit)
            {
                case e_dot_Limit.inDate:
                    __setLR(true, false);
                    block = Expression.Block(
                        typeof(DateTime),
                        new[] { pResult },
                        Expression.Assign(pResult, pLimitMin),
                        pResult
                        );
                    break;

                case e_dot_Limit.notEarlier:
                    __setLR(true, false);
                    block = Expression.Block(
                        typeof(DateTime),
                        new[] { pResult },
                        eComparisonGreater,
                        pResult
                        );
                    break;

                case e_dot_Limit.notLater:
                    __setLR(false, true);
                    block = Expression.Block(
                        typeof(DateTime),
                        new[] { pResult },
                        eComparisonLess,
                        pResult
                        );
                    break;

                /*case e_dot_Limit.Range:
                    __setLR(true, true);
                    block = Expression.Block(
                        typeof(DateTime),
                        new[] { pResult },
                        eComparisonRange,
                        pResult
                        );
                    break;*/

                /*case e_dot_Limit.None:
                default:
                    __setLR(false, false);
                    block = Expression.Block(
                        typeof(DateTime),
                        new[] { pResult },
                        Expression.Assign(pResult, pDate),
                        pResult
                        );
                    break;*/
            }

            __limitCMP = null;
            __limitCMP = Expression.Lambda<Func<DateTime, DateTime, DateTime, DateTime>>(block, pLimitMin, pLimitMax, pDate).Compile();
        }
        private void checkNeedToCompare(bool min)
        {
            if (!enabled) return;
            switch(limit)
            {
                case e_dot_Limit.inDate:
                    if (min) checkEstimated();
                    break;
                case e_dot_Limit.notEarlier:
                    if (min) checkEstimated();
                    break;
                case e_dot_Limit.notLater:
                    if (!min) checkEstimated();
                    break;
                /*case e_dot_Limit.None:
                    checkEstimated();
                    break;
                /*case e_dot_Limit.Range:
                    if (range < 0 && min) _eDate = limitMin;
                    else checkEstimated();
                    break;*/
            }
        }
        #endregion
        #region Events
        public event EventHandler<eventArgs_valueChange<DateTime>> event_dateChangeWarning;
        public event EventHandler<eventArgs_valueChange<e_dot_Limit>> event_limitTypeChanged;
        public event EventHandler<eventArgs_valuesChange<DateTime>> event_limitDateChanged;
        #endregion
        #region Handlers
        public void dot_expectedDateChange(object sender, eventArgs_Date_expectedChange e)
        {
            if (!enabled) return;
            DateTime result;

            bool bResult = limitCompare(sender, e.oldValue, e.newValue, out result);

            e.expectedDate(this, e.oldValue, result, bResult);
        }
        #endregion
        #region Handlers self
        private bool __handler_LimitComparator(object sender, DateTime oldValue, DateTime newValue, out DateTime result)
        {
            result = __limitCMP(limitMin, limitMax, newValue);

            if (result == oldValue) return false;
            return true;
        }
        private void onEstimatedDateChange(eventArgs_valueChange<DateTime> args)
        {
            EventHandler<eventArgs_valueChange<DateTime>> handler = event_dateChangeWarning;
            if (handler != null) handler(this, args);
        }
        private void onLimitChange(eventArgs_valueChange<e_dot_Limit> args)
        {
            EventHandler<eventArgs_valueChange<e_dot_Limit>> handler = event_limitTypeChanged;
            if (handler != null) handler(this, args);
        }
        private void onLimitDateChanged(eventArgs_valuesChange<DateTime> args)
        {
            EventHandler<eventArgs_valuesChange<DateTime>> handler = event_limitDateChanged;
            if (handler != null) handler(this, args);
        }
        #endregion
        #region Overrides
        #endregion
        #region internal entities

        #endregion
        #region referred interface implementation
        #endregion
        #region self interface implementation
        public void reset()
        {
            _enabled = true;
            _dotSet = false;
            __setLR(false, false);
            //_limit = e_dot_Limit.None;
            _limitMin = _limitMax = _eDate = initDate;
        }
        public void setDot(IDot_Value Dot)
        {
            if (_dotSet && !enabled) return;
            Dot.expectedDateChange += dot_expectedDateChange;
            dotCurrentDate = () => Dot.current;
            checkEstimated();
            _dotSet = true;
        }
        #endregion
    }
}
