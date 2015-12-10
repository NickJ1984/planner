using System;

using lib.types;

namespace lib.dot.archive.abstracts
{


    public abstract class ADot_Restrictions
    {
        #region Variables
        internal e_dot_Fixed fixedState;
        internal e_dot_stateLimit limitState;

        internal DateTime stopLeft;
        internal DateTime stopRight;
        #endregion
        #region Properties
        public e_dot_Fixed Fixed { get { return fixedState; } }
        public e_dot_stateLimit limits { get { return limitState; } }
        #endregion
        #region Constructors

        #endregion
        #region Constructors

        #endregion
        #region Methods

        #endregion
        #region Handlers
        #region Value handler


        #endregion

        #endregion
    }



}
