using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.function.temp.iFaces;
using lib.types;

namespace lib.function.temp.classes
{
    public static class functionComparer
    {
        public static bool inRange(IFunctionGet main, IFunctionGet function)
        {
            switch(main.getDirection())
            {
                case e_limDirection.Fixed:
                    return false;

                case e_limDirection.Left:
                    if (function.getMaxDate() <= main.getMaxDate()) return true;
                    else return false;

                case e_limDirection.Right:
                    if (function.getMinDate() >= main.getMinDate()) return true;
                    else return false;

                case e_limDirection.Range:
                    if (((function.getMinDate() >= main.getMinDate()) && 
                         (function.getMinDate() <= main.getMaxDate())) ||
                        ((function.getMaxDate() >= main.getMinDate()) &&
                         (function.getMaxDate() <= main.getMaxDate())))
                        return true;
                    else return false;

                default:
                    return false;
            }
        }
    }
}
