using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib2.classes;

namespace lib2.exceptions
{
    public class exc_link_LoopException : ApplicationException
    {
        public exc_link_LoopException(IID link, IID loopedTask)
            :base(
                 string.Format(
                     "Link {0} can not create, because of looped task {1}"
                     , link.getIDobject().ID
                     , loopedTask.getIDobject().ID)
                 )
        { }
    }
}
