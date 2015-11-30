using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.period.iFaces;
using lib.types;
using lib.service;

namespace lib.Link.classes
{
    class dot_DateLinker
    {
        private IPeriod _parent;
        private IPeriod _child;
        private e_linkType _link;
        private delegate void followerHandler(object sender, EventArgs e);
        private event EventHandler event_precursorChanged;
        private followerHandler d_follower_handler;

        public dot_DateLinker()
        { }

        public void setPrecursor(IPeriod precursor)
        {
            if (_parent != null) unlink();
            _parent = precursor;
        }
        public void setFollower(IPeriod follower)
        {
            e_sideType follSide = __hlp.getSideType(_link, e_linkObject.follower);

            if (follSide == e_sideType._Finish) d_follower_handler = follower.setFinish;
            else d_follower_handler = follower.setStart;

        }
        public void setLink(e_linkType tLink)
        {
            e_sideType precSide = __hlp.getSideType(_link, e_linkObject.precursor);

            if (precSide == e_sideType.Finish_) _parent.event_finishChanged += __precursor_Handler;
            else _parent.event_startChanged += __precursor_Handler;
        }

        private void unlink()
        {
            
            e_sideType precSide = __hlp.getSideType(_link, e_linkObject.precursor);

            if (precSide == e_sideType.Finish_) _parent.event_finishChanged -= __precursor_Handler;
            else
                _parent.event_startChanged -= __precursor_Handler;
        }

        private void __precursor_Handler(object sender, EventArgs e)
        {
            if(d_follower_handler != null)
            {
                d_follower_handler(sender, e);
            }
        }
    }
}
