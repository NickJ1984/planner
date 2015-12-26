using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib2.types;

namespace lib2.classes
{
    public interface IID
    {
        identity getIDobject();
    }
    public interface IIdentity
    {
        string getID();
        eEntity getType();
    }
    public class identity : IIdentity
    {
        private Guid _ID;
        private eEntity _type;

        public string ID { get { return _ID.ToString(); } }
        public eEntity type { get { return _type; } }



        public identity(eEntity type)
        { _ID = Guid.NewGuid();
          _type = type; }
        public identity(eEntity type, string ID)
        {
            _ID = new Guid(ID);
            _type = type;
        }
        public identity(identity IDobject)
            :this(IDobject.type, IDobject.ID)
        { }



        public void setID(string ID)
        {
            _ID = new Guid(ID);
        }
        public void setType(eEntity type)
        {
            _type = type;
        }
        public void copy(identity IDobject)
        {
            _ID = new Guid(IDobject._ID.ToString());
            _type = IDobject.type;
        }


        public string getID() { return ID; }
        public eEntity getType() { return type; }
    }
}
