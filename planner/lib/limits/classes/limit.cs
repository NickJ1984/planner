using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.limits.iFaces;
using lib.types;
using lib.service;
using lib.limits.delegates;

using System.Linq.Expressions;

namespace lib.limits.classes
{
    

    public class lim
    {
        #region expressions
        ConstantExpression cFreeSpace = Expression.Constant((double)-1, typeof(double));
        ConstantExpression cOutOfSpace = Expression.Constant((double)0, typeof(double));

        private ParameterExpression pLeftSpace = Expression.Parameter(typeof(double));
        private ParameterExpression pRightSpace = Expression.Parameter(typeof(double));
        private ParameterExpression pDate = Expression.Parameter(typeof(DateTime));
        private ParameterExpression pLimit = Expression.Parameter(typeof(DateTime));
        private ParameterExpression pValue = Expression.Parameter(typeof(DateTime));
        private ParameterExpression pAllow = Expression.Parameter(typeof(bool));
        private ExpressionType etSign = ExpressionType.Equal;

        private Expression setFalse;
        private Expression setTrue;
        private Expression setDate;
        private Expression setLimit;
        private Expression getRightSpace;
        private Expression getLeftSpace;

        private Expression<Func<DateTime, DateTime, double>> eGetRightSpace;
        private Expression<Func<DateTime, DateTime, double>> eGetLeftSpace;

        private NewExpression pResult;
        private NewExpression pSpaceResult;
        #endregion
        #region Variables
        private int _direction;
        /*
            0 - точка
            1 - вперед
            -1 - назад
            -2 - ограничение отсутствует
        */
        private DateTime _date;
        private e_dot_Limit _limit;

        private Func<DateTime, DateTime, result> process;
        private Func<DateTime, DateTime, bool> fnc_isAllowed;
        private Func<DateTime, DateTime, result> nullProcess;
        private Func<DateTime, DateTime, KeyValuePair<double, double>> fnc_freeSpace;

        public int direction
        {
            get { return _direction; }
            private set
            {
                if(_direction != value)
                {
                    _direction = value;
                    fnc_freeSpace = getFuncFreeSpace(_direction);
                }
            }
        }
        public DateTime date
        {
            get { return _date; }
            set { _date = value; }
        }
        public e_dot_Limit limitType
        {
            get { return _limit; }
            set
            {
                if(_limit != value)
                {
                    _limit = value;
                    initInternal();
                }
            }
        }
        #endregion
        #region Constructors
        public lim(e_dot_Limit vLimit, DateTime Date)
        {
            _direction = -10;
            initExpressionValues();
            _limit = vLimit;
            date = Date;
            initInternal();
        }
        public lim()
            :this(e_dot_Limit.None , __hlp.initDate)
        { }

        private void initExpressionValues()
        {
            #region limits
            setFalse = Expression.Assign(pAllow, Expression.Constant(false, typeof(bool)));
            setTrue = Expression.Assign(pAllow, Expression.Constant(true, typeof(bool)));
            setDate = Expression.Assign(pValue, pDate);
            setLimit = Expression.Assign(pValue, pLimit);

            pResult = Expression.New(
                typeof(result).GetConstructor(new[] { typeof(DateTime), typeof(bool) }),
                pValue, pAllow);

            #region null compile
            NewExpression _pRslt = Expression.New(
                        typeof(result).GetConstructor(new[] { typeof(DateTime), typeof(bool) }),
                        pDate, Expression.Constant(true));

            BlockExpression nullBlock = Expression.Block(_pRslt);
            nullProcess = Expression.Lambda<Func<DateTime, DateTime, result>>(nullBlock, pLimit, pDate).Compile();
            #endregion
            #endregion
            #region free space
            pSpaceResult = Expression.New(
                typeof(KeyValuePair<double, double>).GetConstructor(new[] { typeof(double), typeof(double) }),
                pLeftSpace, pRightSpace);

            eGetRightSpace = (x, y) => __hlp.getRange(x, y, false, false);
            eGetLeftSpace = (x, y) => __hlp.getRange(x, y, true, false);

            getRightSpace = Expression.Invoke(eGetRightSpace, pLimit, pDate);
            getLeftSpace = Expression.Invoke(eGetLeftSpace, pLimit, pDate);
            #endregion
        }
        #endregion
        #region Methods
        public getFunctionLimit getFunctionLim(e_dot_Limit Limit)
        {
            int tmp = 0;
            Func<DateTime, DateTime, result> outFnc = getFunction(Limit, ref tmp);

            getFunctionLimit result = (DateTime dLimit, DateTime Date, out DateTime dResult) =>
            {
                result rslt = outFnc(dLimit, Date);
                dResult = rslt.date;
                return rslt.allow;
            };

            return result;
        }
        public bool checkDate(DateTime Date, out DateTime result)
        {
            result rslt = process(date, Date);

            result = rslt.date;
            return rslt.allow;
        }
        public DateTime checkDate(DateTime Date)
        {
            result rslt = process(date, Date);

            return rslt.date;
        }
        #endregion
        #region Service
        private void initInternal()
        {
            int tmpDir = direction;
            process = getFunction(_limit, ref tmpDir);
            direction = tmpDir;
            initFncAllow();
        }
        private void initFncAllow()
        {

            Expression cmpOperator = Expression.MakeBinary(etSign, pDate, pLimit);

            Expression cmp = Expression.IfThenElse(
                    cmpOperator,
                    setTrue,
                    setFalse
                );

            BlockExpression block = Expression.Block(
                new[] { pAllow },
                (limitType != e_dot_Limit.None) ? cmp : setTrue,
                pAllow
                );

            fnc_isAllowed = Expression.Lambda<Func<DateTime, DateTime, bool>>(block, pLimit, pDate).Compile();
        }

        private Func<DateTime, DateTime, result> getFunction(e_dot_Limit vLimit, ref int Direction)
        {
            switch (vLimit)
            {
                case e_dot_Limit.inDate:
                    etSign = ExpressionType.Equal;
                    Direction = 0;
                    break;

                case e_dot_Limit.notEarlier:
                    etSign = ExpressionType.GreaterThanOrEqual;
                    Direction = 1;
                    break;

                case e_dot_Limit.notLater:
                    etSign = ExpressionType.LessThanOrEqual;
                    Direction = -1;
                    break;

                case e_dot_Limit.None:
                    Direction = -2; //направление отсутствует
                    return nullProcess;
            }

            Expression cmpOperator = Expression.MakeBinary(etSign, pDate, pLimit);

            Expression cmp = Expression.IfThenElse(
                    cmpOperator,
                Expression.Block(
                    setDate,
                    setTrue),
                Expression.Block(
                    setLimit,
                    setFalse)
                );

            BlockExpression resBlock = Expression.Block(
                new[] {pValue, pAllow},
                cmp,
                pResult
                );

            return Expression.Lambda<Func<DateTime, DateTime, result>>(resBlock, pLimit, pDate).Compile();
        }
        private Func<DateTime, DateTime, KeyValuePair<double, double>> getFuncFreeSpace(int direction)
        {
            BlockExpression frspcBlock = Expression.Block(
                new[] { pLeftSpace, pRightSpace },
                Expression.Assign(pLeftSpace, cFreeSpace),
                Expression.Assign(pRightSpace, cFreeSpace),
                pSpaceResult);

            switch (direction)
            {
                case -1:
                    frspcBlock = Expression.Block(
                        new[] { pLeftSpace, pRightSpace },
                        Expression.Assign(pLeftSpace, cFreeSpace),
                        Expression.Assign(pRightSpace, getRightSpace),
                        pSpaceResult);
                    break;
                case 1:
                    frspcBlock = Expression.Block(
                        new[] { pLeftSpace, pRightSpace },
                        Expression.Assign(pLeftSpace, getLeftSpace),
                        Expression.Assign(pRightSpace, cFreeSpace),
                        pSpaceResult);
                    break;
                case 0:
                    frspcBlock = Expression.Block(
                        new[] { pLeftSpace, pRightSpace },
                        Expression.Assign(pLeftSpace, cOutOfSpace),
                        Expression.Assign(pRightSpace, cOutOfSpace),
                        pSpaceResult);
                    break;
            }

            return Expression.Lambda<Func<DateTime, DateTime, KeyValuePair<double, double>>>(frspcBlock, pLimit, pDate).Compile();
        }
        #endregion
        #region inner Entities
        private class result
        {
            public DateTime date;
            public bool allow;

            public result(DateTime date, bool allow)
            {
                this.date = date;
                this.allow = allow;
            }
        }
        #endregion
        #region Interfaces
        public int CompareTo(object obj)
        {
            Type tObj = obj.GetType();
            DateTime oDate;

            if (tObj == typeof(DateTime)) oDate = (DateTime)obj;
            else if (typeof(ILim).IsAssignableFrom(tObj)) oDate = ((ILim)obj).date;
            else return 1;

            if (date > oDate) return 1;
            else if (date == oDate) return 0;
            else return -1;
        }

        public bool Equals(ILim other)
        {
            if (direction == other.direction && date == other.date) return true;
            return false;
        }
        public bool isAllowed(ILim limit)
        {
            if (limit.limitType == e_dot_Limit.None) return false;

            return isAllowed(limit.date);
        }
        public bool isAllowed(DateTime date)
        {
            return fnc_isAllowed(this.date, date);
        }

        public KeyValuePair<double, double> getFreeSpace(DateTime cDate)
        {
            return fnc_freeSpace(date, cDate);
        }
        #endregion
    }

    


}
