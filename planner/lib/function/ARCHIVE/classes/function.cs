using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.service;

using lib.delegates;
using lib.function.temp.delegates;

using lib.function.temp.iFaces;
using lib.limits.iFaces;


namespace lib.function.temp.classes
{
    using alias_fncStatic = System.Func<DateTime, DateTime>;
    using alias_fncDynamic = System.Func<e_limDirection, DateTime, DateTime, DateTime, DateTime>;
    using alias_fncDirDynamic = System.Func<DateTime, DateTime, DateTime, DateTime>;
    using alias_getDate = System.Func<DateTime>;

    public class Function : IFunctionGet, IFunctionSet
    {
        #region Variables
        private DateTime _limitMinDate;
        private DateTime _limitMaxDate;

        private e_limDirection _direction;

        private alias_getDate _fncMin;
        private alias_getDate _fncMax;
        private alias_fncStatic _fncCheck;
        private alias_fncDirDynamic _fncDirDynamic;
        #endregion
        #region Properties
        public e_limDirection direction
        {
            get { return _direction; }
            set
            {
                if (_direction != value)
                {
                    _direction = value;

                    generateFunction();
                    setFuncMinMax();
                }
            }
        }
        public DateTime minLimitDate
        {
            get { return _limitMinDate; }
            set
            {
                if (_limitMinDate != value) _limitMinDate = value;
            }
        }
        public DateTime maxLimitDate
        {
            get { return _limitMaxDate; }
            set
            {
                if (_limitMaxDate != value) _limitMaxDate = value;
            }
        }
        public DateTime minDate { get { return _fncMin(); } }
        public DateTime maxDate { get { return _fncMax(); } }
        #endregion
        #region Delegates
        #endregion
        #region Constructors
        public Function()
        {
            _limitMinDate = _limitMaxDate = __hlp.initDate;
            direction = e_limDirection.Right;
        }
        #endregion
        #region Methods
        #endregion
        #region Service
        private void generateFunction()
        {
            _fncDirDynamic = functionGenerator.generateDynamicDir(direction);
        }
        private void setFuncMinMax()
        {
            switch(direction)
            {
                case e_limDirection.Right:
                case e_limDirection.Fixed:
                    _fncMax = () => minLimitDate;
                    _fncMin = () => minLimitDate;
                    break;
                case e_limDirection.Left:
                    _fncMax = () => maxLimitDate;
                    _fncMin = () => maxLimitDate;
                    break;
                case e_limDirection.Range:
                    _fncMax = () => maxLimitDate;
                    _fncMin = () => minLimitDate;
                    break;
            }
        }
        #endregion
        #region Events
        #endregion
        #region Handlers
        #endregion
        #region Handlers self
        #endregion
        #region Overrides
        #endregion
        #region internal entities
        #endregion
        #region referred interface implementation
        #endregion
        #region self interface implementation
        public DateTime checkDate(DateTime Date)
        { return _fncCheck(Date); }
        public alias_fncStatic getFunction()
        { return _fncCheck; }


        public e_limDirection getDirection()
        { return direction; }
        public void setDirection(e_limDirection direction)
        { this.direction = direction; }


        public DateTime getMinLimitDate()
        { return minLimitDate; }
        public void setMinLimitDate(DateTime Date)
        { minLimitDate = Date; }

        public DateTime getMaxLimitDate()
        { return maxLimitDate; }
        public void setMaxLimitDate(DateTime Date)
        { maxLimitDate = Date; }

        public void setDate(DateTime Date)
        {
            maxLimitDate = minLimitDate = Date;
        }


        public DateTime getMaxDate()
        { return maxDate; }
        public DateTime getMinDate()
        { return minDate; }

        public bool inRange(IFunctionGet functionGet)
        {
            return functionComparer.inRange(this, functionGet);
        }
        public bool inRange(DateTime Date)
        {
            return (Date != checkDate(Date)) ? false : true; 
        }
        #endregion
    }
}
