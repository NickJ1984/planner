using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;
using lib.types;
using lib.limits.iFaces;

namespace lib.service
{
    public class limitGenerator
    {
        #region expressions
        ConstantExpression cFreeSpace = Expression.Constant((double)-1, typeof(double));
        ConstantExpression cOutOfSpace = Expression.Constant((double)0, typeof(double));

        private ParameterExpression pLeftSpace = Expression.Parameter(typeof(double));
        private ParameterExpression pRightSpace = Expression.Parameter(typeof(double));
        private ParameterExpression pSpace = Expression.Parameter(typeof(double));

        private ParameterExpression pDate = Expression.Parameter(typeof(DateTime));
        private ParameterExpression pLimit = Expression.Parameter(typeof(DateTime));
        private ParameterExpression pLimVal = Expression.Parameter(typeof(ILimit_values));

        
        #endregion
        #region Variables
        #endregion
        #region Properties
        #endregion
        #region Delegates
        public delegate double d_getSpace(ILimit_values limVals, DateTime Date);

        
        #endregion
        #region Constructors
        #endregion
        #region Methods
        public d_getSpace getSpaceLeftFunction()
        {
            return __getSpace();
        }
        public d_getSpace getSpaceRightFunction()
        {
            
            return __getSpace(false);
        }
        #endregion
        #region Service
        private d_getSpace __getSpace(bool left = true)
        {

            Func<DateTime, DateTime, double> rng = (DateTime date, DateTime limit) =>
            {
                double res = (!left) ? limit.Subtract(date).Days : date.Subtract(limit).Days;
                return (res < 0) ? 0 : res;
            };
            Expression<Func<DateTime, DateTime, double>> eRng = (dt, lm) => rng(dt, lm);
            Expression getRange = Expression.Invoke(eRng, pDate, Expression.Property(pLimVal, "date"));

            Expression eSwitch = Expression.Switch(
                Expression.Property(pLimVal, "limitType"),
                Expression.Assign(pSpace, cFreeSpace),
                new SwitchCase[]
                {
                    Expression.SwitchCase(
                        Expression.Assign(pSpace, cOutOfSpace),
                        Expression.Constant(e_dot_Limit.inDate)
                        ),
                    Expression.SwitchCase(
                        (left) ? Expression.Assign(pSpace, getRange) : Expression.Assign(pSpace, cFreeSpace),
                        Expression.Constant(e_dot_Limit.notEarlier)
                        ),
                    Expression.SwitchCase(
                        (!left) ? Expression.Assign(pSpace, getRange) : Expression.Assign(pSpace, cFreeSpace),
                        Expression.Constant(e_dot_Limit.notLater)
                        )
                });

            BlockExpression rBlock = Expression.Block(
                typeof(double),
                new[] {pSpace},
                eSwitch,
                pSpace
                );

            return Expression.Lambda<d_getSpace>(rBlock, pLimVal, pDate).Compile();
        }
        

        #endregion
        #region Events
        #endregion
        #region Handlers
        #endregion
        #region Handlers self
        
        #endregion
        #region Overrides
        #endregion
        #region internal entities
        
        #endregion
        #region referred interface implementation
        #endregion
        #region self interface implementation
        #endregion
    }
}
