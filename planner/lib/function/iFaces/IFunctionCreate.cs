using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;


namespace lib.function.iFaces
{
    using alias_fncStatic = System.Func<DateTime, DateTime>;
    using alias_fncDynamic = System.Func<e_limDirection, DateTime, DateTime, DateTime, DateTime>;
    using alias_fncSemiDynamic = System.Func<DateTime, DateTime, DateTime, DateTime>;
    using alias_fncRange = System.Func<DateTime, DateTime, double>; //-1 - бесконечно; 0 - точка

    public interface IFunctionCreate
    {
        alias_fncStatic generateStatic(IFunctionGet function);
        alias_fncDynamic generateDynamic();
        alias_fncSemiDynamic generateDirLimit(e_limDirection direction);
        alias_fncRange generateLimitRange();
        
    }
    
}
