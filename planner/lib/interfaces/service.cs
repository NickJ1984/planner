
using lib.types;

namespace lib.interfaces
{
    public interface IidentObject
    {
        void setIndex(int index);
        int getIndex();
        e_ValueType getType();
        string getID();
    }
}
