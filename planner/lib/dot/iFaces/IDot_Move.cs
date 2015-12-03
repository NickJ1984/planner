using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.delegates;
using lib.dot.classes;
using lib.types;

namespace lib.dot.iFaces
{
    public interface IDot_Move
    {
        bool enabled { get; set; }
        bool canMove { get; }
        bool canMoveLeft { get; } 
        bool canMoveRight { get; } 
        double spaceLeft { get; } // -1 - без ограничений 
        double spaceRight { get; } // -1 - без ограничений 

        d_valueGet<DateTime> linkCurrentDate { set; }
        d_valueGet<DateTime> linkLeftBound { set; }
        d_valueGet<DateTime> linkRightBound { set; }
        d_valueGet<bool> linkIsLeftBound { set; }
        d_valueGet<bool> linkIsRightBound { set; }

        event EventHandler<eventArgs_valueChange<DateTime>> event_dateMoved;

        void connectValue(IDot_Value Value);
        void connectLimit(IDot_Limit Limit);

        DateTime moveDate(DateTime date, out double remains);
        
        void reset();
    }
}
