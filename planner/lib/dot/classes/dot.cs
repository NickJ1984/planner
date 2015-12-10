using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.service;
using lib.dot.iFaces;
using lib.delegates;
using lib.limits.iFaces;
using lib.limits.classes;
using lib.types;



namespace lib.dot.classes
{
    public class Dot : IDot
    {
        #region Variables
        private DateTime _date;
        private ILimit_check localLimit;
        #endregion
        #region Properties
        public ILimit_check dotLimitCheck
        {
            get { return localLimit; }
            set
            {
                if (value == null) localLimit = new limitDummy();
                else localLimit = value;
            }
        }
        public DateTime date
        {
            get { return _date; }
            set
            {
                DateTime result = localLimit.checkDate(value);
                if (_date != result)
                {
                    DateTime temp = _date;
                    _date = result;
                    onDateChanged(new eventArgs_valueChange<DateTime>(temp, _date));
                }
            }
        }
        #endregion
        #region Constructors
        public Dot(DateTime date)
        {
            localLimit = new limitDummy();
            _date = date;
        }
        public Dot()
            : this(__hlp.initDate)
        { }
        #endregion
        #region Events
        public event EventHandler<eventArgs_valueChange<DateTime>> event_dateChanged;
        #endregion
        #region Handlers self
        private void onDateChanged(eventArgs_valueChange<DateTime> args)
        {
            EventHandler<eventArgs_valueChange<DateTime>> handler = event_dateChanged;
            if (handler != null) handler(this, args);
        }
        #endregion
        #region internal entities
        private class limitDummy : ILimit_check
        {
            public DateTime checkDate(DateTime Date)
            {
                return Date;
            }
        }
        #endregion
    }
}
