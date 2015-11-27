using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.types
{ 
    public enum e_dot_aspect { Dot = 0, LeftDot = 1, RightDot = 2 }
    public enum e_dot_stateLimit { Free = 0, LimitLeft = 1, LimitRight = 2, OutOfRange = 6 }
    public enum e_dot_Fixed {Free = 0, FixedLeft = 1, FixedRight = 2, Fixed = 3 }
    public enum e_dot_stateValue { Null = 0, Source = 1, Declared = 2, Changed = 3}
    public enum e_dot_sender { This = 0, Val_Declare = 1, Val_Source = 2, Val_Fixed = 3}
    public enum e_ValueType { Null = 0, Dot = 1, Period = 2}
    //public enum e_dot_stateMovement { Cant = 0, canMove = 1, canMoveRight = 2, canMoveLeft = 3}
    //помоему Fixed тоже самое описывает
    public enum e_linkType : byte
    {
        none = 0,
        StartFinish = 20,
        FinishStart = 39,
        StartStart = 5,
        FinishFinish = 64
    }
}
