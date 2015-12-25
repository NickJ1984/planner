using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.interfaces;

namespace lib.service
{
    public class identify : IIdentify
    {
        #region Variables
        private readonly Guid _ID;
        private readonly e_objectType _type;
        #endregion
        #region Properties
        public Guid ID
        { get { return _ID; } }
        public e_objectType type
        { get { return _type; } }
        #endregion
        #region Constructors
        public identify(e_objectType type)
        {
            this._type = type;
            _ID = Guid.NewGuid();
        }
        public identify(int type)
            :this((e_objectType)type)
        { }
        public identify(identify idObject)
        {
            _ID = idObject;
            _type = idObject;
        }
        #endregion
        #region Overrides
        public static implicit operator string(identify instance)
        {
            return instance._ID.ToString();
        }
        public static implicit operator Guid(identify instance)
        {
            return instance._ID;
        }
        public static implicit operator e_objectType(identify instance)
        {
            return instance._type;
        }
        public static implicit operator int(identify instance)
        {
            return (int)instance._type;
        }
        #endregion
    }
}
