using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.delegates;
using lib.dot.iFaces;
using lib.dot.abstracts;

namespace lib.period.abstracts
{


    public abstract class APeriod_Base : ABase
    {
        #region Variables
        internal ADot_MainValues _start;
        internal ADot_MainValues _finish;
        internal double _duration;

        #endregion
        #region Properties
        public DateTime start { get { return _start.current; } }
        public DateTime finish { get { return _finish.current; } }

        public virtual double duration
        {
            get { return _duration; }
            set
            {

            }
        }
        #endregion
        #region Events
        #endregion
        #region Constructors
        #endregion
        #region Methods
        #endregion
        #region Service
        protected bool __service_durChange()
        {
            return false;
        }
        #endregion
    }


}
