using System;
using lib.dot.abstracts;
using lib.dot.iFaces;
using lib.types;
using lib.delegates;


namespace lib.dot.classes
{

    public class dot_Fixer : ADot_Fixer, IDot_Fixer
    {
        #region Variables
        private e_dot_aspect _aspect;
        private e_dot_Fixed _status;
        private e_dot_Fixed _tmpStatus;
        private DateTime _bound;
        private DateTime _current;
        private double _freeSpace;
        #endregion

        #region Properties
        private DateTime parentDate
        {
            get { return getCurrent(); }
            set
            {
                setCurrent(value);
            }
        }
        public override e_dot_Fixed status { get { return _status; } }
        public override DateTime bound
        {
            get { return _bound; }
            set
            {
                if (_bound != value)
                {
                    _bound = value;

                    _svc_freeSpaceRecalculate();
                    _svc_currentCheck();
                    _svc_statusCheck();
                }
            }
        }
        public override DateTime current { get { return _current; } }
        public override double freeSpace { get { return _freeSpace; } }
        public override bool Fixed
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    setEnabled(value);
                }
            }
        }
        #endregion

        #region Events
        public override event d_value<e_dot_Fixed> event_statusChanged;
        #endregion

        #region Constructors
        public dot_Fixer(IDot_mainValues parent)
            : base(parent)
        {
            _aspect = parent.aspect;
            _status = e_dot_Fixed.Free;

            _current = parentDate;
            _freeSpace = 0;
        }
        public dot_Fixer()
        {
            _status = e_dot_Fixed.Free;

            _current = parentDate;
            _freeSpace = 0;
        }
        #endregion

        #region Methods
        public DateTime checkOnCurrentModified(DateTime oldValue, DateTime newValue)
        {
            DateTime result = spotDate(_aspect, _bound, newValue);
            if (_current != result)
            {
                _current = result;
                _svc_freeSpaceRecalculate();
                _svc_statusCheck();
            }
            return result;
        }
        public void changeAspect(e_dot_aspect Aspect)
        {
            _aspect = Aspect;
            _svc_freeSpaceRecalculate();
            _svc_statusCheck();
            _svc_currentCheck();
        }
        public void fixManual(bool manualFix)
        {
            if (manualFix && status != e_dot_Fixed.Fixed)
            {
                _tmpStatus = _status;
                _status = e_dot_Fixed.Fixed;
            }
            else if (status == e_dot_Fixed.Fixed)
                _status = _tmpStatus;
        }
        public void setParent(IDot_mainValues parent)
        { linkParent(parent); }
        #endregion

        #region Service
        private void _svc_currentCheck()
        {
            if (status != e_dot_Fixed.Fixed)
                _current = inBoundCheck(_aspect, _bound, _current);
        }
        private void _svc_freeSpaceRecalculate()
        {
            _freeSpace = durationCalculate(_aspect, _bound, _current);
        }
        private void _svc_statusCheck()
        {
            e_dot_Fixed temp = _status;

            switch (Fixed)
            {
                case true:

                    temp = e_dot_Fixed.Free;

                    break;
                case false:

                    if (status == e_dot_Fixed.Fixed) return;
                    if (_freeSpace < 0)
                        temp = (e_dot_Fixed)((int)_aspect);
                    else
                        temp = e_dot_Fixed.Free;

                    break;
            }
            if (status != temp && event_statusChanged != null)
            {
                _status = temp;
                event_statusChanged(status);
            }
        }



        internal override void setEnabled(bool enableValue)
        {
            base.setEnabled(enableValue);
            _svc_statusCheck();
        }
        #endregion
    }

}
