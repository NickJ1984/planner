using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib2.types;
using lib2.classes;
using lib2.delegates;

namespace lib2.classes
{
    public interface ILimitCheck
    {
        DateTime getDateLimit();
        eFLim getTypeLimit();

        DateTime checkDate(DateTime Date);

        event EventHandler<EA_valueChange<DateTime>> event_changedDate;
    }

    public interface ILimit : ILimitCheck
    {
        void setDateLimit(DateTime Date);
        void setTypeLimit(eFLim type);


        event EventHandler<EA_valueChange<eFLim>> event_changedType;
    }

    public class limit : ILimit
    {
        private eFLim _type;
        private DateTime _date;
        private Func<DateTime, DateTime, DateTime> checkFunction;

        public eFLim type
        {
            get { return _type; }
            set
            {
                if(type != value)
                {
                    eFLim tmp = _type;
                    _type = value;
                    functionReassign();
                    onTypeChanged(tmp, _type);
                }
            }
        }
        public DateTime date
        {
            get { return _date; }
            set
            {
                if(date != value)
                {
                    DateTime tmp = _date;
                    _date = value;
                    onDateChanged(tmp, _date);
                }
            }
        }

        public event EventHandler<EA_valueChange<eFLim>> event_changedType;
        public event EventHandler<EA_valueChange<DateTime>> event_changedDate;



        public limit(eFLim type, DateTime date)
        {
            this.type = type;
            this.date = date;
        }



        private void functionReassign()
        {
            checkFunction = fncGenerator.generateDynamicDir(_type);
        }
        private void onTypeChanged(eFLim Old, eFLim New)
        {
            EventHandler<EA_valueChange<eFLim>> handler = event_changedType;
            if (handler != null) handler(this, new EA_valueChange<eFLim>(Old, New));
        }
        private void onDateChanged(DateTime Old, DateTime New)
        {
            EventHandler<EA_valueChange<DateTime>> handler = event_changedDate;
            if (handler != null) handler(this, new EA_valueChange<DateTime>(Old, New));
        }



        public DateTime checkDate(DateTime Date)
        {
            return checkFunction(this.date, Date);
        }

        #region interface additional functions
        public DateTime getDateLimit() { return date; }
        public void setDateLimit(DateTime Date) { date = Date; }

        public eFLim getTypeLimit() { return type; }
        public void setTypeLimit(eFLim  type) { this.type = type; }

        #endregion
    }
}
