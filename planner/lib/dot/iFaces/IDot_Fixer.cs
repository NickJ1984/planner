using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.delegates;

namespace lib.dot.iFaces
{
    public interface IDot_Fixer
    {
        #region Variables
        e_dot_Fixed status { get; }
        DateTime bound { get; set; }
        DateTime current { get; }
        double freeSpace { get; }
        bool Fixed { get; set; }
        #endregion


        DateTime spotDate(e_dot_aspect dotAspect, DateTime dtBound, DateTime dtDate);
        double getDuration(e_dot_aspect dotAspect, DateTime dtBound, DateTime dtDate);
        DateTime checkOnCurrentModified(DateTime oldValue, DateTime newValue);
        void changeAspect(e_dot_aspect Aspect);
        void fixManual(bool manualFix);
    }
}
