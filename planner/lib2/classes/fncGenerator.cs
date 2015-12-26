using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;
using lib2.types;

namespace lib2.classes
{

    using alias_fncStatic = System.Func<DateTime, DateTime>;
    using alias_fncRange = System.Func<DateTime, DateTime, double>; //-1 - бесконечно; 0 - точка
    using alias_fncDynamic = System.Func<eFLim, DateTime, DateTime, DateTime, DateTime>;
    using alias_fncDirDynamic = System.Func<DateTime, DateTime, DateTime, DateTime>;
    using alias_fncDirDynamic1 = System.Func<DateTime, DateTime, DateTime>;

    public static class fncGenerator
    {

        public static Func<DateTime, DateTime, DateTime> generateDynamicDir(eFLim direction)
        {
            BlockExpression block;

            ParameterExpression pLim = Expression.Parameter(typeof(DateTime));
            ParameterExpression pDate = Expression.Parameter(typeof(DateTime));
            ParameterExpression pResult = Expression.Parameter(typeof(DateTime));
            ParameterExpression pTemp = Expression.Parameter(typeof(DateTime));

            switch (direction)
            {
                case eFLim.Fixed:
                    block = Expression.Block(
                        Expression.Assign(pResult, pLim)
                        );
                    break;

                case eFLim.Left:
                    block = Expression.Block(
                        Expression.IfThenElse(
                            Expression.LessThanOrEqual(pDate, pLim),
                            Expression.Assign(pResult, pDate),
                            Expression.Assign(pResult, pLim))
                        );
                    break;

                case eFLim.Right:
                    block = Expression.Block(
                        Expression.IfThenElse(
                            Expression.GreaterThanOrEqual(pDate, pLim),
                            Expression.Assign(pResult, pDate),
                            Expression.Assign(pResult, pLim))
                        );
                    break;

                default:
                    throw new Exception("Неверное значение параметра eFLim метода generateDynamicDir");
            }

            BlockExpression resBlock = Expression.Block(
                typeof(DateTime),
                new[] { pResult, pTemp },
                block,
                pResult
                );

            return Expression.Lambda<alias_fncDirDynamic>(resBlock, pLim, pDate).Compile();
        }

        public static Func<eFLim, DateTime, DateTime, DateTime, DateTime> generateDynamic()
        {
            //              direction       limMin    limMax    chkDate

            ParameterExpression pDir = Expression.Parameter(typeof(eFLim));

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
                                Expression.Constant(eFLim.Fixed, typeof(eFLim))),

                            Expression.SwitchCase(
                                cBLK = Expression.Block(
                                Expression.IfThenElse(
                                    Expression.LessThanOrEqual(pDate, pMax),
                                    Expression.Assign(pResult, pDate),
                                    Expression.Assign(pResult, pMax))),
                                Expression.Constant(eFLim.Left, typeof(eFLim))),

                            Expression.SwitchCase(
                                cBLK = Expression.Block(
                                Expression.IfThenElse(
                                    Expression.GreaterThanOrEqual(pDate, pMin),
                                    Expression.Assign(pResult, pDate),
                                    Expression.Assign(pResult, pMin))),
                                Expression.Constant(eFLim.Right, typeof(eFLim)))
                    }),
                pResult
                );
            return Expression.Lambda<alias_fncDynamic>(block, pDir, pMin, pMax, pDate).Compile();
        }

        public static Func<DateTime, DateTime> generateDynamic
                (Func<eFLim> fDirection, Func<DateTime> fMinLimit, Func<DateTime> fMaxLimit)
        {
            ParameterExpression pMax = Expression.Parameter(typeof(DateTime));
            ParameterExpression pMin = Expression.Parameter(typeof(DateTime));
            ParameterExpression pDir = Expression.Parameter(typeof(eFLim));

            Expression<Func<DateTime>> efMin = () => fMinLimit();
            Expression<Func<DateTime>> efMax = () => fMaxLimit();
            Expression<Func<eFLim>> efDir = () => fDirection();

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
                                Expression.Constant(eFLim.Fixed, typeof(eFLim))),

                            Expression.SwitchCase(
                                cBLK = Expression.Block(
                                Expression.IfThenElse(
                                    Expression.LessThanOrEqual(pDate, pMax),
                                    Expression.Assign(pResult, pDate),
                                    Expression.Assign(pResult, pMax))),
                                Expression.Constant(eFLim.Left, typeof(eFLim))),

                            Expression.SwitchCase(
                                cBLK = Expression.Block(
                                Expression.IfThenElse(
                                    Expression.GreaterThanOrEqual(pDate, pMin),
                                    Expression.Assign(pResult, pDate),
                                    Expression.Assign(pResult, pMin))),
                                Expression.Constant(eFLim.Right, typeof(eFLim)))
                    }),
                pResult
                );


            return Expression.Lambda<Func<DateTime, DateTime>>(block, pDate).Compile();
        }

        public static Func<DateTime, DateTime, double> generateLimitRange(eFLim direction)
        {
            ParameterExpression pMin = Expression.Parameter(typeof(DateTime));
            ParameterExpression pMax = Expression.Parameter(typeof(DateTime));
            ParameterExpression pDouble = Expression.Parameter(typeof(double));
            ConstantExpression cDNull = Expression.Constant((double)0, typeof(double));
            ConstantExpression cDUnlim = Expression.Constant((double)-1, typeof(double));

            Func<DateTime, DateTime, double> fncRange = (min, max) =>
            {
                double result = max.Subtract(min).Days;
                return (result < 0) ? 0 : result;
            };
            Expression<Func<DateTime, DateTime, double>> lmbRange = (min, max) => fncRange(min, max);

            BlockExpression block = Expression.Block(typeof(double), cDNull);

            switch (direction)
            {
                case eFLim.Fixed:
                    block =
                        Expression.Block(
                            typeof(double),
                            new[] { pDouble },
                            Expression.Assign(pDouble, cDNull),
                            pDouble
                            );
                    break;

                case eFLim.Right:
                case eFLim.Left:
                    block =
                        Expression.Block(
                            typeof(double),
                            new[] { pDouble },
                            Expression.Assign(pDouble, cDUnlim),
                            pDouble
                            );
                    break;
            }

            return Expression.Lambda<Func<DateTime, DateTime, double>>(block, pMin, pMax).Compile();
        }

        public static Func<DateTime, DateTime> generateStatic
                (eFLim direction, DateTime minLimit, DateTime maxLimit)
        {
            BlockExpression block;

            ConstantExpression pMin = Expression.Constant(minLimit);
            ConstantExpression pMax = Expression.Constant(maxLimit);
            ParameterExpression pDate = Expression.Parameter(typeof(DateTime));
            ParameterExpression pResult = Expression.Parameter(typeof(DateTime));
            ParameterExpression pTemp = Expression.Parameter(typeof(DateTime));

            switch (direction)
            {
                case eFLim.Fixed:
                    block = Expression.Block(
                        Expression.Assign(pResult, pMin)
                        );
                    break;

                case eFLim.Left:
                    block = Expression.Block(
                        Expression.IfThenElse(
                            Expression.LessThanOrEqual(pDate, pMax),
                            Expression.Assign(pResult, pDate),
                            Expression.Assign(pResult, pMax))
                        );
                    break;

                case eFLim.Right:
                    block = Expression.Block(
                        Expression.IfThenElse(
                            Expression.GreaterThanOrEqual(pDate, pMin),
                            Expression.Assign(pResult, pDate),
                            Expression.Assign(pResult, pMin))
                        );
                    break;

                default:
                    throw new Exception("Неверное значение параметра eFLim метода generateDynamicDir");
            }

            BlockExpression resBlock = Expression.Block(
            typeof(DateTime),
            new[] { pResult, pTemp },
            block,
            pResult
            );

            return Expression.Lambda<Func<DateTime, DateTime>>(resBlock, pDate).Compile();
        }
    }
}
