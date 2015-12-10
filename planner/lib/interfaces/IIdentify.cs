using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;

namespace lib.interfaces
{
    public interface IIdentify
    {
        Guid ID { get; }
        e_objectType type { get; }
    }
}
