using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

using lib.types;
using lib.function.iFaces;

namespace lib.function.classes
{
    using alias_fncStatic = System.Func<DateTime, DateTime>;
    using alias_fncRange = System.Func<DateTime, DateTime, double>; //-1 - бесконечно; 0 - точка
    using alias_fncDynamic = System.Func<e_limDirection, DateTime, DateTime, DateTime, DateTime>;
    using alias_fncDirDynamic = System.Func<DateTime, DateTime, DateTime, DateTime>;
    using alias_fncDirDynamic1 = System.Func<DateTime, DateTime, DateTime>;

    public class functionGenerator
    {
        #region values

        #region enums
        #region direction
        private const ExpressionType ctEqual = ExpressionType.Equal;
        private const ExpressionType ctNotEq = ExpressionType.NotEqual;
        private const ExpressionType ctGrtrEq = ExpressionType.GreaterThanOrEqual;
        private const ExpressionType ctGrtr = ExpressionType.GreaterThan;
        private const ExpressionType ctLsrEq = ExpressionType.LessThanOrEqual;
        private const ExpressionType ctLsr = ExpressionType.LessThan;
        #endregion
        #endregion
        #region constant
        #region types
        private readonly Type tDT = typeof(DateTime);
        private readonly Type tDBL = typeof(double);
        private readonly Type tINT = typeof(int);
        private readonly Type tTS = typeof(TimeSpan);
        private readonly Type tDIR = typeof(e_limDirection);
        #endregion
        #region functions

        #endregion
        #endregion

        #endregion
        #region expTreeVars
        #region Constants
        private ConstantExpression cDNull = Expression.Constant((double)0, typeof(double));
        private ConstantExpression cDUnlim = Expression.Constant((double)-1, typeof(double));
        #region direction
        private ConstantExpression cDirFixed = Expression.Constant(e_limDirection.Fixed, typeof(e_limDirection));
        private ConstantExpression cDirLeft = Expression.Constant(e_limDirection.Left, typeof(e_limDirection));
        private ConstantExpression cDirRight = Expression.Constant(e_limDirection.Right, typeof(e_limDirection));
        private ConstantExpression cDirRange = Expression.Constant(e_limDirection.Range, typeof(e_limDirection));
        #endregion

        #endregion
        #region expressions

        #region binary
        private BinaryExpression eBinaryTrue;
        private BinaryExpression eBinaryFalse;
        #endregion

        #region lambda
        private readonly Expression<Func<DateTime, DateTime, double>> lmbRange = (min, max) => max.Subtract(min).Days;
        #endregion
        #region 2
        #endregion
        #region 3
        #endregion
        #endregion
        #region actions
        #endregion
        #endregion
        #region Initializers
        public void init_Binary()
        {
            eBinaryFalse = Expression.MakeBinary(ExpressionType.NotEqual, cDNull, cDNull);
            eBinaryTrue = Expression.MakeBinary(ExpressionType.Equal, cDNull, cDNull);

        }
        #endregion
        #region helpers
        #region vars & const
        private ParameterExpression eParam(Type Type)
        { return Expression.Parameter(Type); }
        private ConstantExpression eConst(dynamic Value)
        { return Expression.Constant(Value); }
        #endregion
        #region expCall
        private MethodCallExpression Call(Expression instance, string methodName, params Expression[] arguments)
        { return Expression.Call(instance, instance.Type.GetMethod(methodName), arguments); }
        private MethodCallExpression Call(Expression instance, string methodName)
        { return Expression.Call(instance, instance.Type.GetMethod(methodName)); }
        #endregion
        #region expType
        private BinaryExpression DIRtoCMP(e_limDirection Direction, Expression Date, Expression MinLim, Expression MaxLim)
        {
            switch (Direction)
            {
                case e_limDirection.Fixed:
                    return makeBinary(ctEqual, Date, MinLim);

                case e_limDirection.Left:
                    return makeBinary(ctLsrEq, Date, MaxLim);

                case e_limDirection.Right:
                    return makeBinary(ctGrtrEq, Date, MinLim);

                case e_limDirection.Range:
                    return OrElse(makeBinary(ctGrtrEq, Date, MinLim),
                                  makeBinary(ctLsrEq, Date, MaxLim));

                default:
                    throw new Exception("Неверное значение параметра e_limDirection Direction функции DIRtoCMP");
            }
        }
        #endregion
        #region binary
        private BinaryExpression OrElse(Expression param1, Expression param2)
        { return Expression.OrElse(param1, param2); }
        private BinaryExpression makeBinary(ExpressionType type, Expression param1, Expression param2)
        { return Expression.MakeBinary(type, param1, param2); }
        private BinaryExpression assign(Expression left, Expression right)
        { return Expression.Assign(left, right); }
        private BinaryExpression ifLessNull(Expression value)
        {
            ConstantExpression cNll = Expression.Constant(0, value.Type);
            return Expression.MakeBinary(ExpressionType.LessThan, value, cNll);
        }
        #endregion
        #region unary
        private UnaryExpression negate(Expression value)
        { return Expression.Negate(value); }
        #endregion
        #region conditional
        private ConditionalExpression If(BinaryExpression cmp, Expression True, Expression False)
        {
            return Expression.IfThenElse(cmp, True, False);
        }
        private ConditionalExpression notLesserNull(Expression value, Expression writeResult, ConstantExpression cNull)
        {
            return Expression.IfThenElse(
                Expression.LessThan(value, cNull),
                assign(writeResult, cNull),
                assign(writeResult, value)
                );
        }
        private ConditionalExpression Absolute(Expression value, Expression writeResult)
        {
            ConstantExpression cNll = Expression.Constant(0, value.Type);
            return Expression.IfThenElse(
                Expression.LessThan(value, cNll),
                assign(writeResult, negate(value)),
                assign(writeResult, value)
                );
        }
        #endregion
        #region switch
        private SwitchExpression Switch(Expression swtchValue, Expression Default, params SwitchCase[] cases)
        {
            return Expression.Switch(
                swtchValue,
                Default,
                cases
                );
        }
        private SwitchExpression Switch(Expression swtchValue, params SwitchCase[] cases)
        {
            return Expression.Switch(
                swtchValue,
                cases
                );
        }
        private SwitchCase swchCase(ConstantExpression caseValue, Expression body)
        {
            return Expression.SwitchCase(body, caseValue);
        }
        #endregion
        #endregion
        #region constructor
        public functionGenerator()
        {
            init_Binary();
        }
        #endregion
        #region generators
        public Func<DateTime, DateTime, DateTime, DateTime> generateDynamicDir(e_limDirection direction)
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

        public Func<e_limDirection, DateTime, DateTime, DateTime, DateTime> generateDynamic()
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

        public Func<DateTime, DateTime> generateDynamic
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

        /*public Func<e_limDirection, DateTime, DateTime, DateTime, DateTime> generateDynamic(IFunctionGet function)
        {
            throw new NotImplementedException();
        }*/

        public Func<DateTime, DateTime, double> generateLimitRange(e_limDirection direction)
        {
            ParameterExpression pMin = Expression.Parameter(tDT);
            ParameterExpression pMax = Expression.Parameter(tDT);
            ParameterExpression pDouble = Expression.Parameter(tDBL);

            BlockExpression block = Expression.Block(typeof(double), cDNull);

            switch (direction)
            {
                case e_limDirection.Fixed:
                    block =
                        Expression.Block(
                            typeof(double),
                            new[] { pDouble },
                            assign(pDouble, cDNull),
                            pDouble
                            );
                    break;

                case e_limDirection.Right:
                case e_limDirection.Left:
                    block =
                        Expression.Block(
                            typeof(double),
                            new[] { pDouble },
                            assign(pDouble, cDUnlim),
                            pDouble
                            );
                    break;

                case e_limDirection.Range:
                    block =
                        Expression.Block(
                            typeof(double),
                            new[] { pDouble },
                            assign(pDouble, Expression.Invoke(lmbRange, pMin, pMax)),
                            notLesserNull(pDouble, pDouble, Expression.Constant((double)0)),
                            pDouble
                            );
                    break;
            }

            return Expression.Lambda<Func<DateTime, DateTime, double>>(block, pMin, pMax).Compile();
        }

        public Func<DateTime, DateTime> generateStatic
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
