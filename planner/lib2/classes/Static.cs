using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib2.types;

namespace lib2.classes
{
    public static class __hlp
    {
        public static readonly DateTime initDate = new DateTime(1900, 1, 1);

        public static eLnkDot getPrecursor(eLnkType Type)
        {
            eLnkTypeChunk TC = (eLnkTypeChunk)Type;
            return ((TC & eLnkTypeChunk.Finish_) == eLnkTypeChunk.Finish_) ? eLnkDot.Finish : eLnkDot.Start;
        }
        public static eLnkDot getFollower(eLnkType Type)
        {
            eLnkTypeChunk TC = (eLnkTypeChunk)Type;
            return ((TC & eLnkTypeChunk._Finish) == eLnkTypeChunk._Finish) ? eLnkDot.Finish : eLnkDot.Start;
        }
    }
}
