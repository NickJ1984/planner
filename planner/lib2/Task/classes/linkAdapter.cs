﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib2.classes;
using lib2.types;
using lib2.delegates;
using lib2.Link.classes;

namespace lib2.Task.classes
{
    public interface ILinkAdapter
    {
        bool connectAsPrecursor(ILink Link);
        bool connectAsFollower(ILink Link);
        bool unregisterLink(identity idLink);
    }

    public class linkAdapter 
    {
        #region Variables


        #endregion

    }
}
