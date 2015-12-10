using System;
using lib.types;
using lib.delegates;
using lib.dot.archive.iFaces;

namespace lib.dot.archive.classes
{
    public class dot_Value : IDot_Value
    {
        #region Variables
        private readonly DateTime initDate;
        private DateTime _current;
        private DateTime _source;

        e_dot_stateValue _state;
        #endregion
        #region Properties
        public DateTime current
        {
            get { return _current; }
            set
            {
                if(_current != value)
                {
                    if (expectedDateChange != null)
                    {
                        eventArgs_Date_expectedChange args = new eventArgs_Date_expectedChange(this,
                            __handler_currentSet, _current, value);
                        onCurrentValueChange(args);
                    }
                    else __handler_currentSet(this, _current, value, true);
                }
            }
        }
        public DateTime source
        {
            get { return _source; }
            protected set
            {
                switch(state)
                {
                    case e_dot_stateValue.Null:
                        _source = value;
                        _state = e_dot_stateValue.Source;
                        break;
                    case e_dot_stateValue.Source:
                        _source = value;
                            break;
                    case e_dot_stateValue.Declared:
                        if (_source != _current) _state = e_dot_stateValue.Changed;
                        break;
                    case e_dot_stateValue.Changed:
                        if (_source == _current) _state = e_dot_stateValue.Declared;
                        break;
                }
            }
        }
        public e_dot_stateValue state
        {
            get { return _state; }
            protected set { _state = value; }
        }
        #endregion
        #region Constructors
        public dot_Value()
        {
            initDate = new DateTime(1900, 1, 1);
            init_Vars();
        }
        private void init_Vars()
        {
            _current = _source = initDate;
            _state = e_dot_stateValue.Null;
        }
        #endregion
        #region Methods
        #endregion
        #region Service
        #endregion
        #region Events
        public event EventHandler<eventArgs_dateChanged> dateChanged;
        public event EventHandler<eventArgs_Date_expectedChange> expectedDateChange;
        #endregion
        #region Handlers
        private void __handler_currentSet(object sender, DateTime oldValue, DateTime newValue, bool allowed = false)
        {
            if (!allowed) return;

            _current = newValue;
            source = _current;

            if(dateChanged != null)
            {
                eventArgs_dateChanged args = new eventArgs_dateChanged(this, oldValue, newValue);
                EventHandler<eventArgs_dateChanged> handler = dateChanged;
                handler(this, args);
            }
        }
        private void onCurrentValueChange(eventArgs_Date_expectedChange args)
        {
            EventHandler<eventArgs_Date_expectedChange> handler = expectedDateChange;
            handler(this, args);
        }
        #endregion
        #region Overrides
        #endregion
        #region internal entities
        #endregion
        #region referred interface implementation
        #endregion
        #region self interface implementation

        public bool declareDate(DateTime date)
        {
            if ((int)state > 1) return false;
            _state = e_dot_stateValue.Declared;

            DateTime temp = source;
            _source = date;

            current = date;
            return true;
        }

        public void reset(DateTime startDate, e_dot_stateValue startState = e_dot_stateValue.Source)
        {
            _source = _source = startDate;
            _state = startState;
        }
        public void reset()
        {
            reset(initDate, e_dot_stateValue.Null);
        }

        #endregion
    }
}
