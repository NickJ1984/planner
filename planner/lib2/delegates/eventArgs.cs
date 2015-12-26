using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib2.delegates
{
    public class EA_value<T> : System.EventArgs
    {
        public T Value;

        public EA_value()
        { }
        public EA_value(T Value)
        {
            this.Value = Value;
        }
    }

    public class EA_valueChange<T> : System.EventArgs
    {
        public T oldValue;
        public T newValue;

        public EA_valueChange()
        { }
        public EA_valueChange(T Old, T New)
        {
            oldValue = Old;
            newValue = New;
        }
    }
    
}
