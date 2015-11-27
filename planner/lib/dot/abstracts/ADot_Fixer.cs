using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;

using lib.interfaces;
using lib.service;
using lib.types;
using lib.delegates;

namespace lib.dot.abstracts
{
    public abstract class ADot_Fixer : ABase
    {
        #region Variables
        public abstract e_dot_Fixed status { get; }
        public abstract bool Fixed { get; set; }
        public abstract DateTime bound { get; set; }
        public abstract DateTime current { get; }
        public abstract double freeSpace { get; }


        public abstract event d_value<e_dot_Fixed> event_statusChanged;

        protected Func<e_dot_aspect, DateTime, DateTime, double> durationCalculate;
        protected Func<e_dot_aspect, DateTime, DateTime, DateTime> inBoundCheck;

        protected d_valueReference<DateTime> getCurrent;
        protected d_value<DateTime> setCurrent;
        #endregion
        #region Constructor
        public ADot_Fixer(ADot_MainValues parent)
        {
            getCurrent = parent.getCurrentDate;
            setCurrent = parent.setCurrentDate;
            initialize_ExpressionTrees();
        }
        #endregion
        #region methods
        public DateTime spotDate(e_dot_aspect dotAspect, DateTime dtBound, DateTime dtDate)
        { return inBoundCheck(dotAspect, dtBound, dtDate); }
        public double getDuration(e_dot_aspect dotAspect, DateTime dtBound, DateTime dtDate)
        { return durationCalculate(dotAspect, dtBound, dtDate); }
        #endregion
        #region expression trees methods
        protected virtual void initialize_ExpressionTrees()
        {
            #region Parameters
            ParameterExpression pBound = Expression.Parameter(typeof(DateTime), "bound");
            ParameterExpression pDate = Expression.Parameter(typeof(DateTime), "date");


            ParameterExpression pResult = Expression.Parameter(typeof(DateTime), "result");
            ParameterExpression pTimeSpan = Expression.Parameter(typeof(TimeSpan), "date");
            ParameterExpression pRange = Expression.Parameter(typeof(double), "range");

            ParameterExpression pSide = Expression.Parameter(typeof(e_dot_aspect), "side");
            #endregion
            #region durationCalculate
            BinaryExpression rngSubtrStart = Expression.Assign(pTimeSpan, Expression.Subtract(pDate, pBound));
            BinaryExpression rngSubtrFinish = Expression.Assign(pTimeSpan, Expression.Subtract(pBound, pDate));

            ConditionalExpression rngIfSide = Expression.IfThenElse(
                Expression.Equal(pSide, Expression.Constant(e_dot_aspect.LeftDot, typeof(e_dot_aspect))),
                rngSubtrStart,
                rngSubtrFinish
                );

            Expression rngConvert = Expression.Convert(
                Expression.Property(pTimeSpan, "Days"),
                typeof(double));

            Expression rngResult = Expression.Assign(pRange, rngConvert);

            BlockExpression eBlock_rng = Expression.Block(
                typeof(double),
                new[] { pTimeSpan, pRange },
                rngIfSide,
                rngResult,
                pRange
                );

            durationCalculate = Expression.Lambda<Func<e_dot_aspect, DateTime, DateTime, double>>(eBlock_rng, pSide, pBound, pDate).Compile();
            #endregion
            #region durationCalculate
            ConstantExpression constNull = Expression.Constant((double)0, typeof(double));

            Expression durAssign = Expression.IfThenElse(

                Expression.GreaterThanOrEqual(pRange, constNull),
                Expression.Assign(pResult, pDate),
                Expression.Assign(pResult, pBound)
                );

            BlockExpression eBlock_bnd = Expression.Block(
                typeof(DateTime),
                new[] { pTimeSpan, pRange, pResult },
                rngIfSide,
                rngResult,
                durAssign,
                pResult
            );

            inBoundCheck = Expression.Lambda<Func<e_dot_aspect, DateTime, DateTime, DateTime>>(eBlock_bnd, pSide, pBound, pDate).Compile();
            #endregion
        }
        #endregion
    }

}
