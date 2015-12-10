using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using System.Linq.Expressions;

namespace lib.service
{
    public static class __hlp
    {
        public static DateTime initDate = new DateTime(1900, 1, 1);

        public static e_sideType getSideType(e_linkType type, e_linkObject tObject)
        {
            KeyValuePair<e_sideType, e_sideType> result = decomposeLink(type);
            e_sideType precursor = result.Key;
            e_sideType follower = result.Value;

            return (tObject == e_linkObject.follower) ? follower : precursor;
        }
        public static e_linkType joinSides(e_sideType precursor, e_sideType follower)
        {
            int iPrec = (int)precursor;
            int iFoll = (int)follower;

            if ((iPrec != 4 || iPrec != 48) || (iFoll != 1 || iFoll != 16)) return e_linkType.none;

            int iResult = iFoll + iPrec;
            return (e_linkType)iResult;
        }
        public static KeyValuePair<e_sideType, e_sideType> decomposeLink(e_linkType link)
        {
            switch(link)
            {
                case e_linkType.FinishFinish:
                    return new KeyValuePair<e_sideType, e_sideType>(e_sideType.Finish_, e_sideType._Finish);
                case e_linkType.FinishStart:
                    return new KeyValuePair<e_sideType, e_sideType>(e_sideType.Finish_, e_sideType._Start);
                case e_linkType.StartStart:
                    return new KeyValuePair<e_sideType, e_sideType>(e_sideType.Start_, e_sideType._Start);
                case e_linkType.StartFinish:
                    return new KeyValuePair<e_sideType, e_sideType>(e_sideType.Start_, e_sideType._Finish);
                default:
                    throw new Exception("argument link has no value");
            }
        }
        public static int determineDirection(DateTime source, DateTime current)
        {
            if (source == current) return 0;
            else if (source > current) return -1;
            else if (source < current) return 1;

            return -2;
        }

        public static string getValueName<T>(Expression<Func<T>> memberExpression)
        {
            MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
            return expressionBody.Member.Name;
        }

        public static double getRange(DateTime dst, DateTime src, bool leftDirection = true, bool negativeResult = true)
        {
            double result = (leftDirection) ? src.Subtract(dst).Days : dst.Subtract(src).Days;
            return (negativeResult) ? result : (result < 0) ? 0 : result;
        }
        public static double getRangeDirection(DateTime src, DateTime dst)
        {
            int mv = determineDirection(src, dst);

            if (mv == 1) return getRange(dst, src, false);
            if (mv == -1) return getRange(dst, src, true);

            return 0;
        }
    }
}
