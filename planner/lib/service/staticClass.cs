using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;

namespace lib.service
{
    public static class __hlp
    {
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
    }
}
