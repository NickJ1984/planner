using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib2.delegates;

namespace lib2.Task.classes
{
    public interface IDot
    {
        bool isStart();

        void setDependancy(bool isDepend);
        bool getDependancy();

        void setDate(DateTime date);
        DateTime getDate();

        void setCheckFunction(Func<DateTime, DateTime> function);
        Func<DateTime, DateTime> getCheckFunction();

        void update();



        event EventHandler<EA_valueChange<DateTime>> dateChanged;
    }
    public class Dot : IDot
    {
        #region Variables
        public readonly bool _start;

        private bool _depend;

        private DateTime _date;
        private DateTime temp;
        #region functions
        private readonly Func<DateTime, DateTime> fncDummy = (date) => date;

        private Func<DateTime, DateTime> fncCheck;
        #endregion
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
        public Func<DateTime,DateTime> checkFunction
        {
            get { return (fncCheck == fncDummy) ? null : fncCheck; }
            set
            {
                if(value != fncCheck)
                {
                    if (value == null)
                    {
                        if (fncCheck == fncDummy) return;
                        else fncCheck = fncDummy;
                    }

                    fncCheck = value;
                }
            }
        }
        #endregion
        #region Events
        public event EventHandler<EA_valueChange<DateTime>> dateChanged;
        #endregion
        #region Constructors
        public Dot(bool isStart, bool isDepend, DateTime Date)
        {
            _start = isStart;
            _depend = isDepend;
            fncCheck = fncDummy;
            temp = _date = Date;
        }
        #endregion
        #region Handlers
        private void onDateChange(DateTime Old, DateTime New)
        {
            EventHandler<EA_valueChange<DateTime>> handler = dateChanged;
            if (handler != null) handler(this, new EA_valueChange<DateTime>(Old, New));
        }
        #endregion
        #region method
        public void update()
        {
            DateTime result = fncCheck(date);
            if (result != date)
            {
                temp = _date;
                _date = result;
                onDateChange(temp, _date);
            }
        }

        #endregion
        #region Secondary methods
        public bool isStart()
        { return start; }
        public void setDate(DateTime date)
        { this.date = date; }
        public DateTime getDate()
        { return date; }
        public void setDependancy(bool isDepend)
        { depend = isDepend; }
        public bool getDependancy()
        { return depend; }

        public void setCheckFunction(Func<DateTime,DateTime> function)
        { checkFunction = function; }
        public Func<DateTime, DateTime> getCheckFunction()
        { return checkFunction; }
        #endregion
    }
}
