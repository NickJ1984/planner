using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib2.types
{
    #region Limit
    public enum eFLim { Fixed = 0, Left = 1, Right = 2}
    #endregion

    #region General
    public enum eEntity { none = 0, project = 1, factory = 2, group = 3, link = 4, task = 5}
    #endregion

    #region Task
    public enum eTskLLim
    {
        Earlier = 1,
        startNotEarlier = 2,
        finishNotEarlier = 3,
        Later = 4,
        startNotLater = 5,
        finishNotLater = 6,
        startFixed = 7,
        finishFixed = 8
    }
    #endregion

    #region Links
    public enum eLnkState { inTime = 0, later = 1, early = 2 }

    public enum eLnkDot
    {
        Start = 1,
        Finish = 2
    }
    [Flags]
    public enum eLnkTypeChunk
    {
        Start_ = 2,
        Finish_ = 4,
        _Start = 8,
        _Finish = 16
    }
    public enum eLnkType
    {
        StartStart = 10,
        FinishStart = 12,
        StartFinish = 18,
        FinishFinish = 20
    }
    #endregion
}
