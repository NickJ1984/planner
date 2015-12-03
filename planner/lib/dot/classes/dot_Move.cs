using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.dot.iFaces;
using lib.types;
using lib.delegates;
using lib.service;

namespace lib.dot.classes
{
    public class dot_Move : IDot_Move
    {
        #region Variables
        private enum dlgts { boundLeft, boundRight, IsLeftBound, IsRightBound }
        private double _spaceLeft;
        private double _spaceRight;
        private bool _enabled;
        private Dictionary<dlgts, object> temp;
        #endregion
        #region Properties
        private DateTime current { get { return __delegate_currentDate(); } }
        private DateTime left { get { return __delegate_boundLeft(); } }
        private DateTime right { get { return __delegate_boundRight(); } }

        private bool isLeft { get { return __delegate_IsLeftBound(); } }
        private bool isRight { get { return __delegate_IsRightBound(); } }

        public bool enabled
        {
            get { return _enabled; }
            set { __property_enabled(value); }
        }
        public bool canMove { get { return canMoveLeft | canMoveRight; } }
        public bool canMoveLeft
        { get { return (spaceLeft > 0) ? true : false; } }
        public bool canMoveRight
        { get { return (spaceRight > 0) ? true : false; } }
        public double spaceLeft
        { get { return __property_getSpaceLeft(); } }
        public double spaceRight
        { get { return __property_getSpaceRight(); } }
        #endregion
        #region Delegates
        private d_valueGet<DateTime> __delegate_currentDate;
        private d_valueGet<DateTime> __delegate_boundLeft;
        private d_valueGet<DateTime> __delegate_boundRight;
        private d_valueGet<bool> __delegate_IsRightBound;
        private d_valueGet<bool> __delegate_IsLeftBound;

        public d_valueGet<DateTime> linkCurrentDate
        {
            set { if (__delegate_currentDate == null) __delegate_currentDate = value; }
        }
        public d_valueGet<DateTime> linkLeftBound
        {
            set { if (__delegate_boundLeft == null) __delegate_boundLeft = value; }
        }
        public d_valueGet<DateTime> linkRightBound
        {
            set { if (__delegate_boundRight == null) __delegate_boundRight = value; }
        }
        public d_valueGet<bool> linkIsLeftBound
        { set { if (__delegate_IsLeftBound == null) __delegate_IsLeftBound = value; } }
        public d_valueGet<bool> linkIsRightBound
        { set { if (__delegate_IsRightBound == null) __delegate_IsRightBound = value; } }
        #endregion
        #region Constructors
        public dot_Move()
        { reset(); }
        public dot_Move(IDot_Value Value, IDot_Limit Limit)
            :this()
        {
            connectValue(Value);
            connectLimit(Limit);
        }
        #endregion
        #region Methods
        #endregion
        #region Service
        private void __property_enabled(bool Value)
        {
            if (Value == _enabled) return;
            switch(Value)
            {
                case true:
                    __delegate_boundLeft = (d_valueGet<DateTime>)temp[dlgts.boundLeft];
                    __delegate_boundRight = (d_valueGet<DateTime>)temp[dlgts.boundRight];
                    __delegate_IsLeftBound = (d_valueGet<bool>)temp[dlgts.IsLeftBound];
                    __delegate_IsRightBound = (d_valueGet<bool>)temp[dlgts.IsRightBound];
                    temp = null;
                    _enabled = true;
                    break;
                case false:
                    temp = new Dictionary<dlgts, object>();
                    temp.Add(dlgts.boundLeft, __delegate_boundLeft);
                    temp.Add(dlgts.boundRight, __delegate_boundRight);
                    temp.Add(dlgts.IsLeftBound, __delegate_IsLeftBound);
                    temp.Add(dlgts.IsRightBound, __delegate_IsRightBound);

                    __delegate_boundLeft = __delegate_currentDate;
                    __delegate_boundRight = __delegate_currentDate;
                    __delegate_IsLeftBound = () => true;
                    __delegate_IsRightBound = () => true;
                    _enabled = false;
                    break;
            }
        }
        private double __property_getSpaceLeft()
        {
            if (!isLeft) _spaceLeft = -1;
            else
            {
                double space = current.Subtract(left).Days;
                _spaceLeft = (space > 0) ? space : 0;
            }
            return _spaceLeft;
        }
        private double __property_getSpaceRight()
        {
            if (!isRight) _spaceRight = -1;
            else
            {
                double space = right.Subtract(current).Days;
                _spaceRight = (space > 0) ? space : 0;
            }
            return _spaceRight;
        }
        #endregion
        #region Events
        public event EventHandler<eventArgs_valueChange<DateTime>> event_dateMoved;
        #endregion
        #region Handlers
        #endregion
        #region Handlers self
        private void onDateMoved(eventArgs_valueChange<DateTime> args)
        {
            EventHandler<eventArgs_valueChange<DateTime>> handler = event_dateMoved;
            if (handler != null) handler(this, args);
        }
        #endregion
        #region Overrides
        #endregion
        #region internal entities
        #endregion
        #region referred interface implementation
        #endregion
        #region self interface implementation
        public DateTime moveDate(DateTime date, out double remains)
        {
            double dRange = current.Subtract(date).Days;
            remains = Math.Abs(dRange);
            if (dRange == 0) return current;

            Func<bool> bSpace;
            Func<double> dSpace;
            Func<double, DateTime> correctDate;

            if (dRange < 0)
            {
                bSpace = () => __delegate_IsRightBound();
                dSpace = () => __property_getSpaceRight();
                correctDate = (double day) => date.AddDays(-day);
            }
            else
            {
                bSpace = () => __delegate_IsLeftBound();
                dSpace = () => __property_getSpaceLeft();
                correctDate = (double day) => date.AddDays(day);
            }
            if (!bSpace())
            {
                remains = 0;
                return date;
            }
            double spc = dSpace();
            if (spc == 0) return current;

            remains = (spc >= remains) ? 0 : remains - spc;
            DateTime result = correctDate(remains);

            onDateMoved(new eventArgs_valueChange<DateTime>(current, result));

            return result;
        }

        public void connectValue(IDot_Value Value)
        {
            linkCurrentDate = () => Value.current;
        }

        public void connectLimit(IDot_Limit Limit)
        {
            linkLeftBound = () => Limit.limitMin;
            linkRightBound = () => Limit.limitMax;
            linkIsLeftBound = () => Limit.isLimitMin;
            linkIsRightBound = () => Limit.isLimitMax;
        }
        public void reset()
        {
            _enabled = true;
            __delegate_boundLeft = null;
            __delegate_boundRight = null;
            __delegate_IsLeftBound = null;
            __delegate_IsRightBound = null;
            __delegate_currentDate = null;
            _spaceLeft = _spaceRight = -1;
        }
        #endregion
    }
}
