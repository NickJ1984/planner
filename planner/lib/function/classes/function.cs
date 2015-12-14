using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.service;

using lib.delegates;
using lib.function.delegates;

using lib.function.iFaces;
using lib.limits.iFaces;


namespace lib.function.classes
{
    using alias_fncStatic = System.Func<DateTime, DateTime>;
    using alias_fncDynamic = System.Func<e_limDirection, DateTime, DateTime, DateTime, DateTime>;

    public class function : IFunctionGet, IFunctionSet, IFunctionInfo
    {
        #region Variables
        private DateTime _dMinDate;
        private DateTime _dMaxDate;
        private e_limDirection _drctn;
        private bool _exist = false;
        #region fnc
        private alias_fncStatic fncStaticCheck;

        private readonly alias_fncStatic fncDummyStaticCheck = (date) => date;
        #endregion
        #endregion
        #region Properties
        #region readonly
        public bool exist { get { return _exist; } }
        #endregion
        #region main
        public alias_fncStatic staticCheck
        {
            get { return fncStaticCheck; }
            set
            {
                if (value != null)
                {
                    fncStaticCheck = value;
                    _exist = true;
                }
                else
                {
                    fncStaticCheck = fncDummyStaticCheck;
                    _exist = false;
                }

            }
        }
        public DateTime minLimit
        {
            get { return _dMinDate; }
            set
            {
                if (value != _dMinDate) _dMinDate = value;
            }
        }
        public DateTime maxLimit
        {
            get { return _dMaxDate; }
            set
            {
                if (value != _dMaxDate) _dMaxDate = value;
            }
        }
        public e_limDirection direction
        {
            get { return _drctn; }
            set
            {
                if (value != _drctn) _drctn = value;
            }
        }
        #endregion
        #endregion
        #region Delegates
        #endregion
        #region Constructors
        public function()
        {
            init_Default();
        }
        #endregion
        #region Methods
        #region Initializers
        public void init_Default()
        {
            fncStaticCheck = fncDummyStaticCheck;

            _dMinDate = _dMaxDate = __hlp.initDate;
            _drctn = e_limDirection.Fixed;
        }
        #endregion
        public DateTime checkDate(DateTime Date)
        {
            return fncStaticCheck(Date);
        }
        #endregion
        #region Service
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
        #region function
        public alias_fncStatic getCheckFunction()
        {
            return fncStaticCheck;
        }
        public void generateStatic()
        {
            throw new NotImplementedException();
        }
        public bool isExist()
        {
            return _exist;
        }
        #endregion
        #region Direction
        public e_limDirection getDirection()
        {
            return direction;
        }
        public void setDirection(e_limDirection direction)
        {
            this.direction = direction;
        }
        #endregion
        #region limit Dates
        public DateTime getMaxDate()
        {
            return maxLimit;
        }
        public void setMaxDate(DateTime Date)
        {
            maxLimit = Date;
        }
        public DateTime getMinDate()
        {
            return minLimit;
        }
        public void setMinDate(DateTime Date)
        {
            minLimit = Date;
        }
        public double getLimitRange()
        {
            if (!exist) return -1;
            if (direction == e_limDirection.Fixed) return 0;
            else if (direction == e_limDirection.Left) return -1;
            else if (direction == e_limDirection.Right) return -1;
            else
            {
                double result = maxLimit.Subtract(minLimit).Days;
                return (result < 0) ? 0 : result;
            }
        }
        #endregion
        #endregion

    }
}
