using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.function.iFaces;
using lib.function.classes;
using lib.limitV2.iFaces;
using lib.types;

namespace lib.limitV2.classes
{
    public class limit : ILimit, ILimit_data, ILimit_check
    {
        #region Variables
        private Function _func;
        private e_dot_Limit _typeLim;
        #endregion
        #region Properties
        public DateTime dateLimit
        {
            get { return _func.minDate; }
            set
            {
                if (value != _func.minDate) _func.setDate(value);
            }
        }
        #endregion
        #region Delegates
        #endregion
        #region Constructors
        #endregion
        #region Methods
        public DateTime checkDate(DateTime Date)
        {
            return _func.checkDate(Date);
        }
        #endregion
        #region Service
        #endregion
        #region Events
        public event EventHandler event_update;

        #endregion
        #region Handlers
        #endregion
        #region Handlers self
        private void onUpdate()
        {
            EventHandler handler = event_update;
            if (handler != null) handler(this, new EventArgs());
        }
        #endregion
        #region Overrides
        #endregion
        #region internal entities
        #endregion
        #region referred interface implementation
        #endregion
        #region self interface implementation
        public e_dot_Limit getType()
        {
            throw new NotImplementedException();
        }
        public void setType(e_dot_Limit Type)
        {
            throw new NotImplementedException();
        }

        public DateTime getLimitDate()
        {
            throw new NotImplementedException();
        }
        public void setLimitDate(DateTime date)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
