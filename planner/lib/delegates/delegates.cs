﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.delegates
{
    public delegate void d_value<T>(T Value);
    public delegate void d_singleValue<T>(object sender, T Value);
    public delegate void d_valueChange<T>(object sender, T oldValue, T newValue);

    public delegate T d_valueReference<T>();

}
