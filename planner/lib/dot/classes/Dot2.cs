using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.dot.iFaces;
using lib.delegates;
using lib.types;

using System.Linq.Expressions;

namespace lib.dot.classes
{
    public class Dot2
    {
        #region dot_Value
        private DateTime _date;
        private bool _enabled;
        public DateTime date
        {
            get { return _date; }
            set
            {
                if (_date != value && setLocalEntry(value))
                {
                    DateTime temp = _date;
                    _date = value;
                    if (enabled) onDateChanged(new eventArgs_valueChange<DateTime>(temp, _date));
                }
            }
        }
        public bool enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    if (!value)
                    {
                        __tmp_Limit = _limit;
                        limit = e_dot_Limit2.None;
                    }

                    _enabled = value;

                    if (_enabled)
                    {
                        limit = __tmp_Limit;
                    }
                }
            }
        }

        public event EventHandler<eventArgs_valueChange<DateTime>> event_dateChanged;
        private void onDateChanged(eventArgs_valueChange<DateTime> args)
        {
            EventHandler<eventArgs_valueChange<DateTime>> handler = event_dateChanged;
            if (handler != null) handler(this, args);
        }
        #endregion
        #region dot_limitShared
        public DateTime checkEntry(DateTime Value)
        {
            DateTime processing = checkGlobalEntry(Value);
            return checkLocalEntry(processing);
        }

        private void initFunction(out Func<DateTime, DateTime, DateTime> dateFunc, out Func<DateTime, DateTime, bool> boolFunc, e_dot_Limit2 edLimit)
        {
            initFunctionCheck(edLimit, out dateFunc);
            initFunctionSet(edLimit, out boolFunc);
        }
        private void initFunctionCheck(e_dot_Limit2 edLimit, out Func<DateTime, DateTime, DateTime> dateFunc)
        {
            ParameterExpression pLimit = Expression.Parameter(typeof(DateTime));
            ParameterExpression pDate = Expression.Parameter(typeof(DateTime));
            ParameterExpression pResult = Expression.Parameter(typeof(DateTime));
            Expression eComparison = Expression.Assign(pResult, pDate);

            switch (edLimit)
            {
                case e_dot_Limit2.inDate:
                    eComparison = Expression.Assign(pResult, pLimit);
                    break;
                case e_dot_Limit2.notEarlier:
                    eComparison = Expression.IfThenElse(
                        Expression.GreaterThanOrEqual(pDate, pLimit),
                        Expression.Assign(pResult, pDate),
                        Expression.Assign(pResult, pLimit)
                        );
                    break;
                case e_dot_Limit2.notLater:
                    eComparison = Expression.IfThenElse(
                        Expression.LessThanOrEqual(pDate, pLimit),
                        Expression.Assign(pResult, pDate),
                        Expression.Assign(pResult, pLimit)
                        );
                    break;
            }

            BlockExpression block = Expression.Block(
                typeof(DateTime),
                new[] { pResult },
                eComparison,
                pResult
                );

            dateFunc = Expression.Lambda<Func<DateTime, DateTime, DateTime>>(block, pLimit, pDate).Compile();
        }
        private void initFunctionSet(e_dot_Limit2 edLimit, out Func<DateTime, DateTime, bool> boolFunc)
        {
            ParameterExpression pLimit = Expression.Parameter(typeof(DateTime));
            ParameterExpression pDate = Expression.Parameter(typeof(DateTime));
            ParameterExpression pResult = Expression.Parameter(typeof(bool));
            Expression eComparison = Expression.Assign(pResult, Expression.Constant(true, typeof(bool)));

            switch (edLimit)
            {
                case e_dot_Limit2.inDate:
                    eComparison = Expression.Assign(pResult, pLimit);
                    break;
                case e_dot_Limit2.notEarlier:
                    eComparison = Expression.IfThenElse(
                        Expression.GreaterThanOrEqual(pDate, pLimit),
                        Expression.Assign(pResult, Expression.Constant(true, typeof(bool))),
                        Expression.Assign(pResult, Expression.Constant(false, typeof(bool)))
                        );
                    break;
                case e_dot_Limit2.notLater:
                    eComparison = Expression.IfThenElse(
                        Expression.LessThanOrEqual(pDate, pLimit),
                        Expression.Assign(pResult, Expression.Constant(true, typeof(bool))),
                        Expression.Assign(pResult, Expression.Constant(false, typeof(bool)))
                        );
                    break;
            }

            BlockExpression block = Expression.Block(
                typeof(bool),
                new[] { pResult },
                eComparison,
                pResult
                );

            boolFunc = Expression.Lambda<Func<DateTime, DateTime, bool>>(block, pLimit, pDate).Compile();
        }
        #endregion
        #region dot_localLimit
        private e_dot_Limit2 _limit;
        private e_dot_Limit2 __tmp_Limit;
        private Func<DateTime, DateTime, DateTime> __fnc_checkEntry;
        private Func<DateTime, DateTime, bool> __fnc_setEntry;

        public DateTime limitDate;
        public e_dot_Limit2 limit
        {
            get { return _limit; }
            set
            {
                if (_limit != value && enabled)
                {
                    _limit = value;
                    initFunction(out __fnc_checkEntry, out __fnc_setEntry, _limit);
                }
            }
        }

        public event EventHandler<eventArgs_expValueChange<DateTime>> event_expLimitDateChanged;
        private void onExpLimitDateChanged(eventArgs_expValueChange<DateTime> args)
        {
            EventHandler<eventArgs_expValueChange<DateTime>> handler = event_expLimitDateChanged;
            if (handler != null) handler(this, args);
        }

        public DateTime checkLocalEntry(DateTime Value)
        {
            return __fnc_checkEntry(date, Value);
        }
        public bool setLocalEntry(DateTime Value)
        {
            bool result = __fnc_setEntry(limitDate, Value);
            if(!result)
            {
                eventArgs_expValueChange<DateTime> args =
                    new eventArgs_expValueChange<DateTime>(limitDate, Value);
                onExpLimitDateChanged(args);
                DateTime corrected;
                if (args.getAnswer(out corrected))
                {
                    limitDate = corrected;
                    return true;
                }
                else return false;
            }
            else return true;
        }
        #endregion
        #region dot_globalLimit
        private DateTime glblLimitDate;
        private e_dot_Limit2 glblLimit;

        private Func<DateTime, DateTime, DateTime> __fnc_gCheckEntry;
        private Func<DateTime, DateTime, bool> __fnc_gSetEntry;

        public e_dot_Limit2 globalLimit
        {
            get { return glblLimit; }
            set
            {
                if(glblLimit != value)
                {
                    glblLimit = value;
                    initFunction(out __fnc_gCheckEntry, out __fnc_gSetEntry, glblLimit);
                }
            }
        }
        public DateTime globalDate
        {
            get { return glblLimitDate; }
            set
            {
                if (glblLimitDate != value) glblLimitDate = value;
            }
        }
        public DateTime checkGlobalEntry(DateTime Value)
        {
            return __fnc_gCheckEntry(globalDate, Value);
        }
        

        #endregion
        #region dot_Move
        private double _freeSpace; // -1 не ограничено
        


        #endregion
    }
}
