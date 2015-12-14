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

    public class functionGenerator
    {
        #region values
        #region dictionaries
        Dictionary<e_dtMtd, string> dtMtd;
        #endregion
        #region enums
        private enum e_dtMtd
        {
            Subtract = 1,
            AddDays = 2,
        }
        #region constant
        #region direction
        private const ExpressionType ctEqual = ExpressionType.Equal;
        private const ExpressionType ctNotEq = ExpressionType.NotEqual;
        private const ExpressionType ctGrtrEq = ExpressionType.GreaterThanOrEqual;
        private const ExpressionType ctGrtr = ExpressionType.GreaterThan;
        private const ExpressionType ctLsrEq = ExpressionType.LessThanOrEqual;
        private const ExpressionType ctLsr = ExpressionType.LessThan;
        #endregion
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
        #region Parameters

        #region DateTime
        private ParameterExpression pMin = Expression.Parameter(typeof(DateTime));
        private ParameterExpression pMax = Expression.Parameter(typeof(DateTime));
        private ParameterExpression pDate = Expression.Parameter(typeof(DateTime));
        private ParameterExpression pResult = Expression.Parameter(typeof(DateTime));
        #endregion

        #region double
        private ParameterExpression pDouble = Expression.Parameter(typeof(double));
        #endregion

        #region direction
        private ParameterExpression pDirection = Expression.Parameter(typeof(e_limDirection));
        #endregion
        #endregion
        #region expressions

        #region binary
        private BinaryExpression eBinaryTrue;
        private BinaryExpression eBinaryFalse;
        #endregion

        #region comparison
        #endregion
        #region 1
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
        public void init_Dictionaries()
        {
            dtMtd = new Dictionary<e_dtMtd, string>();
            dtMtd.Add(e_dtMtd.AddDays, "AddDays");
            dtMtd.Add(e_dtMtd.Subtract, "Subtract");
        }
        public void init_Binary()
        {
            eBinaryFalse = Expression.MakeBinary(ExpressionType.NotEqual, cDNull, cDNull);
            eBinaryTrue = Expression.MakeBinary(ExpressionType.Equal, cDNull, cDNull);

        }
        public void init_Comparison()
        {

        }
        #endregion
        #region helpers
        #region expCall
        #region DateTime
        private MethodCallExpression CallDT(Expression instance, e_dtMtd Method, params Expression[] arguments)
        {
            return Call(instance, dtMtd[Method], arguments);
        }
        private MethodCallExpression CallDT(Expression instance, e_dtMtd Method)
        {
            return Call(instance, dtMtd[Method]);
        }
        #endregion
        #region Call
        private MethodCallExpression Call(Expression instance, string methodName, params Expression[] arguments)
        { return Expression.Call(instance, instance.Type.GetMethod(methodName), arguments); }
        private MethodCallExpression Call(Expression instance, string methodName)
        { return Expression.Call(instance, instance.Type.GetMethod(methodName)); }
        #endregion
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
        private ConditionalExpression notLesserNull(Expression value, Expression writeResult)
        {
            ConstantExpression cNll = Expression.Constant(0, value.Type);
            return Expression.IfThenElse(
                Expression.LessThan(value, cNll),
                assign(writeResult, cNll),
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
        #region generators
        public Func<DateTime, DateTime, DateTime, DateTime> generateDirLimit(e_limDirection direction)
        {
            throw new NotImplementedException();
        }

        public Func<e_limDirection, DateTime, DateTime, DateTime, DateTime> generateDynamic()
        {
            throw new NotImplementedException();
        }

        /*public Func<e_limDirection, DateTime, DateTime, DateTime, DateTime> generateDynamic(IFunctionGet function)
        {
            throw new NotImplementedException();
        }*/

        public Func<DateTime, DateTime, double> generateLimitRange()
        {
            throw new NotImplementedException();
        }
        public Func<DateTime, DateTime, double> generateLimitRange(e_limDirection direction)
        {
            ConditionalExpression ceIF;
            BlockExpression block;

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
                            assign(pDouble,
                            pDouble
                            );
                    break;

            }
        }
        /*public Func<DateTime, DateTime> generateStatic(IFunctionGet function)
        {
            throw new NotImplementedException();
        }*/
        #endregion

    }
}
