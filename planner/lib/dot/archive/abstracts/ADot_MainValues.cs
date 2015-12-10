using System;

using lib.types;
using lib.delegates;

namespace lib.dot.archive.abstracts
{
    public abstract class ADot_MainValues : ABase
    {
        #region Vars
        private e_dot_aspect _aspect;
        protected DateTime initDate;
        protected DateTime _source;
        protected DateTime _current;
        protected e_dot_stateValue _stateValue;
        #endregion
        #region Properties
        public e_dot_aspect aspect { get { return _aspect; } }
        public e_dot_stateValue state
        {
            get { return _stateValue; }
            protected set { _stateValue = value; }
        }
        public DateTime source { get { return _source; } }
        public virtual DateTime current
        {
            get { return _current; }
            set
            {
                DateTime result;

                if (setCurrentDate(_current, value, out result))
                {
                    DateTime temp = _current;
                    _current = result;
                    service_currentChanged(result);
                    if (ev_currentChanged != null) ev_currentChanged(this, temp, _current);
                }
            }
        }
        #endregion
        #region Events
        internal event d_valueChange<DateTime> ev_currentChanged;
        #endregion
        #region Constructors
        public ADot_MainValues(e_dot_aspect Aspect)
        {
            _aspect = Aspect;
            _stateValue = e_dot_stateValue.Null;
            initDate = new DateTime(1900, 1, 1);

            _source = _current = initDate;
            
        }
        public ADot_MainValues()
        {
            _aspect = e_dot_aspect.Dot;
            _stateValue = e_dot_stateValue.Null;
            initDate = new DateTime(1900, 1, 1);

            _source = _current = initDate;
        }
        #endregion
        #region Methods
        internal bool setAspect(e_dot_aspect Aspect)
        {
            if (Aspect != _aspect)
            {
                _aspect = Aspect;
                return true;
            }
            return false;
        }
        protected abstract bool setCurrentDate(DateTime oldDate, DateTime newDate, out DateTime Result);
        public bool declareDate(DateTime date)
        {
            if (state != e_dot_stateValue.Source) return false;
            state = e_dot_stateValue.Declared;
            _source = date;
            return true;
        }

        #region Service
        private void service_currentChanged(DateTime newValue)
        {
            switch (state)
            {
                case e_dot_stateValue.Null:
                    _source = newValue;
                    state = e_dot_stateValue.Source;
                    break;
                case e_dot_stateValue.Source:
                    _source = newValue;
                    break;
                case e_dot_stateValue.Declared:
                    if (_source != newValue) state = e_dot_stateValue.Changed;
                    break;
                case e_dot_stateValue.Changed:
                    if (_source == newValue) state = e_dot_stateValue.Declared;
                    break;
            }
        }
        #endregion
        #endregion
        #region Interface impl

        public DateTime getCurrentDate() { return current; }
        public void setCurrentDate(DateTime date) { current = date; }

        #endregion
    }
}
