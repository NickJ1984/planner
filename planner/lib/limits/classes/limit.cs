﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.limits.iFaces;
using lib.types;
using lib.service;

using System.Linq.Expressions;


namespace lib.limits.classes
{
    public delegate bool getFunctionLimit(DateTime Limit, DateTime Date, out DateTime result);

    public class limit : ILimit
    {
        #region expressions
        private ParameterExpression pDate = Expression.Parameter(typeof(DateTime));
        private ParameterExpression pLimit = Expression.Parameter(typeof(DateTime));
        private ParameterExpression pValue = Expression.Parameter(typeof(DateTime));
        private ParameterExpression pAllow = Expression.Parameter(typeof(bool));
        private ExpressionType etSign = ExpressionType.Equal;

        private Expression setFalse;
        private Expression setTrue;
        private Expression setDate;
        private Expression setLimit;

        private NewExpression pResult;
        #endregion
        #region Variables
        internal int direction;
        /*
            0 - точка
            1 - вперед
            -1 - назад
            -2 - ограничение отсутствует
        */
        private DateTime _date;
        private e_dot_Limit2 _limit;
        private Func<DateTime, DateTime, result> process;
        private Func<DateTime, DateTime, result> nullProcess;
        public DateTime date
        {
            get { return _date; }
            set { _date = value; }
        }
        public e_dot_Limit2 limitType
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
        public limit(e_dot_Limit2 vLimit, DateTime Date)
        {
            initExpressionValues();
            _limit = vLimit;
            date = Date;
            initInternal();
        }
        public limit()
            :this(e_dot_Limit2.None , __hlp.initDate)
        { }
        #endregion
        #region Methods
        public getFunctionLimit getFunctionLim(e_dot_Limit2 Limit)
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

        public ILimit compare(ILimit outer)
        {

            limit result = new limit(_limit, date);
            return result;
        }
        public bool isFits(ILimit outer)
        {
            ExpressionType eType = ExpressionType.Equal;
            switch(direction)
            {
                case 1:
                    eType = ExpressionType.GreaterThanOrEqual;
                    break;
                case -1:
                    eType = ExpressionType.LessThanOrEqual;
                    break;
                case 0:
                    break;
                default:
                    return false;
            }
            
            return result;
        }
        #endregion
        #region Service
        private void initInternal()
        {
            process = getFunction(_limit, ref direction);
        }
        private void initExpressionValues()
        {
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
        }
        private Func<DateTime, DateTime, result> getFunction(e_dot_Limit2 vLimit, ref int Direction)
        {
            switch (vLimit)
            {
                case e_dot_Limit2.inDate:
                    etSign = ExpressionType.Equal;
                    Direction = 0;
                    break;

                case e_dot_Limit2.notEarlier:
                    etSign = ExpressionType.GreaterThanOrEqual;
                    Direction = 1;
                    break;

                case e_dot_Limit2.notLater:
                    etSign = ExpressionType.LessThanOrEqual;
                    Direction = -1;
                    break;

                case e_dot_Limit2.None:
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
        #endregion`
    }


}
