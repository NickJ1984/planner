using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

using lib.types;
using lib.function.temp.iFaces;

namespace lib.function.temp.classes
{
    using alias_fncStatic = System.Func<DateTime, DateTime>;
    using alias_fncRange = System.Func<DateTime, DateTime, double>; //-1 - бесконечно; 0 - точка
    using alias_fncDynamic = System.Func<e_limDirection, DateTime, DateTime, DateTime, DateTime>;
    using alias_fncDirDynamic = System.Func<DateTime, DateTime, DateTime, DateTime>;
    using alias_fncDirDynamic1 = System.Func<DateTime, DateTime, DateTime>;

    public static class functionGenerator
    {
        #region values
        private static ConstantExpression cDNull = Expression.Constant((double)0, typeof(double));
        private static ConstantExpression cDUnlim = Expression.Constant((double)-1, typeof(double));
        #endregion
        #region generators
        public static Func<DateTime, DateTime, DateTime, DateTime> generateDynamicDir(e_limDirection direction)
        {
            BlockExpression block;

            ParameterExpression pMin = Expression.Parameter(typeof(DateTime));
            ParameterExpression pMax = Expression.Parameter(typeof(DateTime));
            ParameterExpression pDate = Expression.Parameter(typeof(DateTime));
            ParameterExpression pResult = Expression.Parameter(typeof(DateTime));
            ParameterExpression pTemp = Expression.Parameter(typeof(DateTime));

            switch (direction)
            {
                case e_limDirection.Fixed:
                    block = Expression.Block(
                        Expression.Assign(pResult, pMin)
                        );
                    break;

                case e_limDirection.Left:
                    block = Expression.Block(
                        Expression.IfThenElse(
                            Expression.LessThanOrEqual(pDate, pMax),
                            Expression.Assign(pResult, pDate),
                            Expression.Assign(pResult, pMax))
                        );
                    break;

                case e_limDirection.Right:
                    block = Expression.Block(
                        Expression.IfThenElse(
                            Expression.GreaterThanOrEqual(pDate, pMin),
                            Expression.Assign(pResult, pDate),
                            Expression.Assign(pResult, pMin))
                        );
                    break;

                case e_limDirection.Range:
                    block = Expression.Block(
                        Expression.IfThenElse(
                            Expression.LessThanOrEqual(pDate, pMax),
                            Expression.Assign(pTemp, pDate),
                            Expression.Assign(pTemp, pMax)),
                        Expression.IfThenElse(
                            Expression.GreaterThanOrEqual(pDate, pMin),
                            Expression.Assign(pResult, pTemp),
                            Expression.Assign(pResult, pMin))
                        );
                    break;

                default:
                    throw new Exception("Неверное значение параметра e_limDirection метода generateDynamicDir");
            }

            BlockExpression resBlock = Expression.Block(
                typeof(DateTime),
                new[] { pResult, pTemp },
                block,
                pResult
                );

            return Expression.Lambda<alias_fncDirDynamic>(resBlock, pMin, pMax, pDate).Compile();
        }

        public static Func<e_limDirection, DateTime, DateTime, DateTime, DateTime> generateDynamic()
        {
            //              direction       limMin    limMax    chkDate

            ParameterExpression pDir = Expression.Parameter(typeof(e_limDirection));

            ParameterExpression pMin = Expression.Parameter(typeof(DateTime));
            ParameterExpression pMax = Expression.Parameter(typeof(DateTime));
            ParameterExpression pDate = Expression.Parameter(typeof(DateTime));
            ParameterExpression pResult = Expression.Parameter(typeof(DateTime));
            ParameterExpression pTemp = Expression.Parameter(typeof(DateTime));

            Expression cBLK;

            BlockExpression block = Expression.Block(
                typeof(DateTime),
                new[] { pResult, pTemp },
                Expression.Switch(
                    pDir,
                    new SwitchCase[]
                    {
                            Expression.SwitchCase(
                                cBLK = Expression.Block(
                                Expression.IfThen(
                                    Expression.IsTrue(Expression.Constant(true)),
                                    Expression.Assign(pResult, pMin))),
                                Expression.Constant(e_limDirection.Fixed, typeof(e_limDirection))),

                            Expression.SwitchCase(
                                cBLK = Expression.Block(
                                Expression.IfThenElse(
                                    Expression.LessThanOrEqual(pDate, pMax),
                                    Expression.Assign(pResult, pDate),
                                    Expression.Assign(pResult, pMax))),
                                Expression.Constant(e_limDirection.Left, typeof(e_limDirection))),

                            Expression.SwitchCase(
                                cBLK = Expression.Block(
                                Expression.IfThenElse(
                                    Expression.GreaterThanOrEqual(pDate, pMin),
                                    Expression.Assign(pResult, pDate),
                                    Expression.Assign(pResult, pMin))),
                                Expression.Constant(e_limDirection.Right, typeof(e_limDirection))),

                            Expression.SwitchCase(
                                cBLK = Expression.Block(
                                Expression.IfThenElse(
                                    Expression.LessThanOrEqual(pDate, pMax),
                                    Expression.Assign(pTemp, pDate),
                                    Expression.Assign(pTemp, pMax)),
                                Expression.IfThenElse(
                                    Expression.GreaterThanOrEqual(pDate, pMin),
                                    Expression.Assign(pResult, pTemp),
                                    Expression.Assign(pResult, pMin))),
                                Expression.Constant(e_limDirection.Range, typeof(e_limDirection)))
                    }),
                pResult
                );
            return Expression.Lambda<alias_fncDynamic>(block, pDir, pMin, pMax, pDate).Compile();
        }

        public static Func<DateTime, DateTime> generateDynamic
                (Func<e_limDirection> fDirection, Func<DateTime> fMinLimit, Func<DateTime> fMaxLimit)
        {
            ParameterExpression pMax = Expression.Parameter(typeof(DateTime));
            ParameterExpression pMin = Expression.Parameter(typeof(DateTime));
            ParameterExpression pDir = Expression.Parameter(typeof(e_limDirection));

            Expression<Func<DateTime>> efMin = () => fMinLimit();
            Expression<Func<DateTime>> efMax = () => fMaxLimit();
            Expression<Func<e_limDirection>> efDir = () => fDirection();

            Expression eSetMin = Expression.Assign(pMin, Expression.Invoke(efMin));
            Expression eSetMax = Expression.Assign(pMax, Expression.Invoke(efMax));
            Expression eSetDir = Expression.Assign(pDir, Expression.Invoke(efDir));

            ParameterExpression pDate = Expression.Parameter(typeof(DateTime));
            ParameterExpression pResult = Expression.Parameter(typeof(DateTime));
            ParameterExpression pTemp = Expression.Parameter(typeof(DateTime));


            Expression cBLK;

            BlockExpression block = Expression.Block(
                typeof(DateTime),
                new[] { pResult, pTemp, pMin, pMax, pDir },
                eSetDir,
                eSetMin,
                eSetMax,
                Expression.Switch(
                    pDir,
                    new SwitchCase[]
                    {
                            Expression.SwitchCase(
                                cBLK = Expression.Block(
                                Expression.IfThen(
                                    Expression.IsTrue(Expression.Constant(true)),
                                    Expression.Assign(pResult, pMin))),
                                Expression.Constant(e_limDirection.Fixed, typeof(e_limDirection))),

                            Expression.SwitchCase(
                                cBLK = Expression.Block(
                                Expression.IfThenElse(
                                    Expression.LessThanOrEqual(pDate, pMax),
                                    Expression.Assign(pResult, pDate),
                                    Expression.Assign(pResult, pMax))),
                                Expression.Constant(e_limDirection.Left, typeof(e_limDirection))),

                            Expression.SwitchCase(
                                cBLK = Expression.Block(
                                Expression.IfThenElse(
                                    Expression.GreaterThanOrEqual(pDate, pMin),
                                    Expression.Assign(pResult, pDate),
                                    Expression.Assign(pResult, pMin))),
                                Expression.Constant(e_limDirection.Right, typeof(e_limDirection))),

                            Expression.SwitchCase(
                                cBLK = Expression.Block(
                                Expression.IfThenElse(
                                    Expression.LessThanOrEqual(pDate, pMax),
                                    Expression.Assign(pTemp, pDate),
                                    Expression.Assign(pTemp, pMax)),
                                Expression.IfThenElse(
                                    Expression.GreaterThanOrEqual(pDate, pMin),
                                    Expression.Assign(pResult, pTemp),
                                    Expression.Assign(pResult, pMin))),
                                Expression.Constant(e_limDirection.Range, typeof(e_limDirection)))
                    }),
                pResult
                );


            return Expression.Lambda<Func<DateTime, DateTime>>(block, pDate).Compile();
        }

        public static Func<DateTime, DateTime, double> generateLimitRange(e_limDirection direction)
        {
            ParameterExpression pMin = Expression.Parameter(typeof(DateTime));
            ParameterExpression pMax = Expression.Parameter(typeof(DateTime));
            ParameterExpression pDouble = Expression.Parameter(typeof(double));

            Func<DateTime, DateTime, double> fncRange = (min, max) =>
            {
                double result = max.Subtract(min).Days;
                return (result < 0) ? 0 : result;
            };
            Expression<Func<DateTime, DateTime, double>> lmbRange = (min, max) => fncRange(min, max);

            BlockExpression block = Expression.Block(typeof(double), cDNull);

            switch (direction)
            {
                case e_limDirection.Fixed:
                    block =
                        Expression.Block(
                            typeof(double),
                            new[] { pDouble },
                            Expression.Assign(pDouble, cDNull),
                            pDouble
                            );
                    break;

                case e_limDirection.Right:
                case e_limDirection.Left:
                    block =
                        Expression.Block(
                            typeof(double),
                            new[] { pDouble },
                            Expression.Assign(pDouble, cDUnlim),
                            pDouble
                            );
                    break;

                case e_limDirection.Range:
                    block =
                        Expression.Block(
                            typeof(double),
                            new[] { pDouble },
                            Expression.Assign(pDouble, Expression.Invoke(lmbRange, pMin, pMax)),
                            pDouble
                            );
                    break;
            }

            return Expression.Lambda<Func<DateTime, DateTime, double>>(block, pMin, pMax).Compile();
        }

        public static Func<DateTime, DateTime> generateStatic
                (e_limDirection direction, DateTime minLimit, DateTime maxLimit)
        {
            BlockExpression block;

            ConstantExpression pMin = Expression.Constant(minLimit);
            ConstantExpression pMax = Expression.Constant(maxLimit);
            ParameterExpression pDate = Expression.Parameter(typeof(DateTime));
            ParameterExpression pResult = Expression.Parameter(typeof(DateTime));
            ParameterExpression pTemp = Expression.Parameter(typeof(DateTime));

            switch (direction)
            {
                case e_limDirection.Fixed:
                    block = Expression.Block(
                        Expression.Assign(pResult, pMin)
                        );
                    break;

                case e_limDirection.Left:
                    block = Expression.Block(
                        Expression.IfThenElse(
                            Expression.LessThanOrEqual(pDate, pMax),
                            Expression.Assign(pResult, pDate),
                            Expression.Assign(pResult, pMax))
                        );
                    break;

                case e_limDirection.Right:
                    block = Expression.Block(
                        Expression.IfThenElse(
                            Expression.GreaterThanOrEqual(pDate, pMin),
                            Expression.Assign(pResult, pDate),
                            Expression.Assign(pResult, pMin))
                        );
                    break;

                case e_limDirection.Range:
                    block = Expression.Block(
                        Expression.IfThenElse(
                            Expression.LessThanOrEqual(pDate, pMax),
                            Expression.Assign(pTemp, pDate),
                            Expression.Assign(pTemp, pMax)),
                        Expression.IfThenElse(
                            Expression.GreaterThanOrEqual(pDate, pMin),
                            Expression.Assign(pResult, pTemp),
                            Expression.Assign(pResult, pMin))
                        );
                    break;

                default:
                    throw new Exception("Неверное значение параметра e_limDirection метода generateDynamicDir");
            }

            BlockExpression resBlock = Expression.Block(
            typeof(DateTime),
            new[] { pResult, pTemp },
            block,
            pResult
            );

            return Expression.Lambda<Func<DateTime, DateTime>>(resBlock, pDate).Compile();
        }
        #endregion
    }
}
