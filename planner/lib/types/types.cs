using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.types
{ 

    public enum e_dot_aspect { Dot = 0, LeftDot = 1, RightDot = 2 }
    public enum e_dot_Limit { None = 0, notEarlier = 4, notLater = 8, inDate = 16 }
    [Flags]
    public enum e_dot_stateLimit { Free = 0, LimitLeft = 1, LimitRight = 2, notEarlier = 4, notLater = 8 }
    [Flags]
    public enum e_dot_Fixed {Free = 0, FixedLeft = 32, FixedRight = 64, Fixed = 128, manualFixed = 256 }
    public enum e_dot_stateValue { Null = 0, Source = 1, Declared = 2, Changed = 3}
    public enum e_dot_sender { This = 0, Val_Declare = 1, Val_Source = 2, Val_Fixed = 3}
    public enum e_ValueType { Null = 0, Dot = 1, Period = 2}
    //public enum e_dot_stateMovement { Cant = 0, canMove = 1, canMoveRight = 2, canMoveLeft = 3}
    //помоему Fixed тоже самое описывает
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
    public enum e_task : byte { none = 0, declared = 1, started = 2, finished = 3 }
    public enum e_taskStatus : byte { none = 0, inTime = 1, early = 2, late = 3, delayed = 4 }
    

}
