
using lib.interfaces;
using lib.service;
using lib.delegates;
using lib.types;

namespace lib.dot.abstracts
{

    public abstract class ABase : IidentObject
    {
        #region Vars
        internal readonly identify whois;

        private bool Enabled;
        #endregion
        #region Properties

        public bool enabled { get { return Enabled; } }
        #endregion
        #region Constructor
        protected ABase()
        {
            whois = new identify(e_ValueType.Dot);

            Enabled = true;
        }
        #endregion
        #region Information interface
        public void setIndex(int index) { whois.setIndex(index); }
        public int getIndex() { return whois.index; }
        public e_ValueType getType() { return whois.type; }
        public string getID() { return whois.ID; }
        #endregion
        #region Events
        internal event d_singleValue<bool> ev_enableChanged;
        #endregion
        #region Value change
        internal virtual void setEnabled(bool enableValue)
        {
            if (enableValue != Enabled)
            {
                Enabled = enableValue;
                if (ev_enableChanged != null) ev_enableChanged(this, Enabled);
            }
        }
        #endregion
    }

}
