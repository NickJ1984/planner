using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.service;

namespace lib.delegates
{
    public class eventArgs_valueChange<T> : System.EventArgs
    {
        public T oldValue;
        public T newValue;

        public eventArgs_valueChange()
        { }
        public eventArgs_valueChange(T Old, T New)
        {
            oldValue = Old;
            newValue = New;
        }
    }
    public class eventArgs_expValueChange<T> : System.EventArgs
    {
        private bool isCorrected;
        public readonly T oldValue;
        public readonly T newValue;
        private T _correctedValue;
        public T correctedValue
        {
            get { return (isCorrected) ? _correctedValue : newValue; }
            set
            {
                isCorrected = true;
                _correctedValue = value;
            }
        }
        public bool allow;
        
        public eventArgs_expValueChange(T Old, T New)
        {
            oldValue = Old;
            _correctedValue = newValue = New;
            isCorrected = false;
            allow = false;
        }
        public bool getAnswer(out T Value)
        {
            Value = correctedValue;
            return allow;
        }
    }

    public class eventArgs_expectedValueChange<T> : System.EventArgs
    {
        public T oldValue;
        public T newValue;

        public readonly d_returnValueChange_eventArgs<T> returnDelegate;

        public eventArgs_expectedValueChange(d_returnValueChange_eventArgs<T> returnDelegate)
        {
            this.returnDelegate = returnDelegate;
        }
        public eventArgs_expectedValueChange(d_returnValueChange_eventArgs<T> returnDelegate, T Old, T New)
            :this(returnDelegate)
        {
            oldValue = Old;
            newValue = New;
        }
    }
    public class eventArgs_valuesChange<T> : System.EventArgs
    {
        public string[] valueName;
        public KeyValuePair<T, T>[] oldNewValuePair;

        public eventArgs_valuesChange()
        {
            valueName = new string[0];
            oldNewValuePair = new KeyValuePair<T, T>[0];
        }
        public eventArgs_valuesChange(string name, T oldValue, T newValue)
            : this()
        {
            Array.Resize<string>(ref valueName, valueName.Length + 1);
            valueName[valueName.Length - 1] = name;

            Array.Resize<KeyValuePair<T, T>>(ref oldNewValuePair, oldNewValuePair.Length + 1);
            oldNewValuePair[oldNewValuePair.Length - 1] = new KeyValuePair<T, T>(oldValue, newValue);
        }
        public T getOldValue(int index)
        {
            return oldNewValuePair[index].Key;
        }
        public T getNewValue(int index)
        {
            return oldNewValuePair[index].Value;
        }
        public string getValueName(int index)
        {
            return valueName[index];
        }
    }
}
