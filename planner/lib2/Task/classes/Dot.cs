using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib2.delegates;

namespace lib2.Task.classes
{
    public class Dot
    {
        #region Variables
        private readonly bool _start;

        public Func<DateTime, DateTime> fncCheck;

        private bool _depend;
        private DateTime _date;
        private DateTime temp;
        #endregion
        #region Properties
        public bool start { get { return _start; } }
        public bool depend
        {
            get { return _depend; }
            set
            {
                if (_depend != value) _depend = value;
            }
        }
        public DateTime date
        {
            get { return _date; }
            set
            {
                if(value != _date)
                {
                    DateTime result = fncCheck(value);
                    if(result != _date)
                    {
                        temp = _date;
                        _date = result;
                        onDateChange(temp, _date);
                    }
                }
            }
        }
        #endregion
        #region Events
        public event EventHandler<EA_valueChange<DateTime>> dateChanged;
        #endregion
        #region Handlers
        private void onDateChange(DateTime Old, DateTime New)
        {
            EventHandler<EA_valueChange<DateTime>> handler = dateChanged;
            if (handler != null) handler(this, new EA_valueChange<DateTime>(Old, New));
        }

        public void handler_dependancyChanged()
        {
            throw new NotImplementedException();
        }
        public void handler_onUpdate()
        {
            DateTime result = fncCheck(date);
            if(result != date)
            {
                temp = _date;
                _date = result;
                onDateChange(temp, _date);
            }
        }
        #endregion
        #region Method
        public Dot(bool isStart, bool isDepend, DateTime Date)
        {
            _start = isStart;
            _depend = isDepend;
            fncCheck = (val) => val;
            temp = _date = Date;
        }
        #endregion
    }
}
