using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.interfaces;

namespace lib.service
{
    internal class identify : IidentObject
    {
        private int _index;
        public int index { get { return _index; } }
        public readonly e_ValueType type;
        public readonly string ID;

        public identify(e_ValueType valueType)
        {
            type = valueType;
            ID = Guid.NewGuid().ToString();
            _index = -1;
        }
        public void setIndex(int Index) { _index = Index; }
        public int getIndex() { return index; }
        public e_ValueType getType() { return type; }
        public string getID() { return ID; }

        public override int GetHashCode() { return ID.GetHashCode(); }
        public override bool Equals(object obj)
        {
            if(obj is IidentObject)
                if (((IidentObject)obj).getID() == ID && ((IidentObject)obj).getType() == type) return true;
            return false;
        }
        public override string ToString()
        { return ID; }

    }
}
