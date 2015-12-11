using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.period.iFaces;
using lib.delegates;

namespace lib.period.classes
{
    public class prd_duration : IPeriod_duration
    {
        #region Variables
        private double _duration;
        private const double minValue = 1;
        #endregion
        #region Properties
        public double duration
        {
            get { return _duration; }
            set
            {
                if(checkEntry(value))
                {
                    value = (value < minValue) ? minValue : value;
                    double temp = _duration;
                    _duration = value;

                    onDurationChange(new eventArgs_valueChange<double>(temp, _duration));
                }
            }
        }

        public Func<double> getDuration
        { get { return () => _duration;  } }
        #endregion
        #region Constructors
        public prd_duration(double duration)
        {
            this.duration = duration;
        }
        public prd_duration()
            :this(minValue)
        { }
        #endregion
        #region Methods
        public void increment(double inc)
        {
            duration += inc;
        }
        public void decrement(double dec)
        {
            duration -= dec;
        }
        #endregion
        #region Service
        private bool checkEntry(double Value)
        {
            if (Value == _duration) return false;
            return true;
        }
        #endregion
        #region Events
        public event EventHandler<eventArgs_valueChange<double>> event_durationChanged;
        #endregion
        #region Handlers self
        private void onDurationChange(eventArgs_valueChange<double> args)
        {
            EventHandler<eventArgs_valueChange<double>> handler = event_durationChanged;

            if (handler != null) handler(this, args);
        }
        #endregion
        #region Overrides
        public static implicit operator double(prd_duration instance)
        {
            return instance.duration;
        }
        #endregion
        #region referred interface implementation
        public int CompareTo(IPeriod_duration other)
        {
            return CompareTo(other.duration);
        }
        public int CompareTo(double other)
        {
            if (other == _duration) return 0;
            if (other < _duration) return 1;
            else return -1;
        }
        public bool Equals(IPeriod_duration other)
        {
            return Equals(other.duration);
        }

        public bool Equals(double other)
        {
            return (other == _duration) ? true : false;
        }
        public int CompareTo(object obj)
        {
            Type tp = obj.GetType();

            if (tp.IsAssignableFrom(typeof(IPeriod_duration))) return CompareTo((IPeriod_duration)obj);
            else if (tp == typeof(double)) return CompareTo((double)obj);
            else return 1;
        }
        #endregion
    }
}
