using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.types
{ 

    
    public enum e_ValueType { Null = 0, Dot = 1, Period = 2}
    //public enum e_dot_stateMovement { Cant = 0, canMove = 1, canMoveRight = 2, canMoveLeft = 3}
    //помоему Fixed тоже самое описывает
    #region Entity
    public enum e_objectType { task = 1, link = 2, group = 3, project = 4, limit_check = 5 }
    #endregion

    #region Dot
    public enum e_dot_type { start = 1, finish =2}
    public enum e_dot_aspect { Dot = 0, LeftDot = 1, RightDot = 2 }
    
    [Flags]
    public enum e_dot_stateLimit { Free = 0, LimitLeft = 1, LimitRight = 2, notEarlier = 4, notLater = 8 }
    [Flags]
    public enum e_dot_Fixed { Free = 0, FixedLeft = 64, FixedRight = 128, Fixed = 256 }
    public enum e_dot_stateValue { Null = 0, Source = 1, Declared = 2, Changed = 3 }
    public enum e_dot_sender { This = 0, Val_Declare = 1, Val_Source = 2, Val_Fixed = 3 }
    #endregion
    #region Link
    public enum e_linkObject : byte { precursor = 1, follower = 2 }
    [Flags]
    public enum e_sideType : byte
    {
        _Start = 1,
        Start_ = 4,
        _Finish = 16,
        Finish_ = 48
    }
    public enum e_linkType : byte
    {
        none = 0,
        StartFinish = 20,
        FinishStart = 49,
        StartStart = 5,
        FinishFinish = 64
    }
    #endregion
    #region Task
    public enum e_tskLimit
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
    #region Project
    public enum e_prjType
    {
        startDate = 1,
        finishDate = 4
    }
    #endregion
    #region Dot
    public enum e_dot_Limit { inDate = 0, notLater = 2, notEarlier = 4 }
    #endregion
    #region Limit
    public enum e_limDirection
    {
        Fixed = 0,
        Left = 2,
        Right = 4,
        Range = 8
    }
    #endregion
    }
