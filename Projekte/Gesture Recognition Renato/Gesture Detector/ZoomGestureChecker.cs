using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Gestures.Zoom;
using System.Diagnostics;

namespace MF.Engineering.MF8910.GestureDetector.Gestures.Zoom
{
    class ZoomGestureChecker: GestureChecker
    {
        protected const int CONDITION_TIMEOUT = 2500;

        public override event EventHandler<GestureEventArgs> Successful;
        public override event EventHandler<FailedGestureEventArgs> Failed;

        private double start=-1.0;
        private double last;

        public ZoomGestureChecker(Person p)
            : base(new List<Condition> {

                new ZoomCondition(p)

            }, CONDITION_TIMEOUT) 
        {
        }

        protected override void fireSucessful(object sender, GestureEventArgs e)
        {
            if(start==-1.0)
            {
                start = ((InternalZoomGestureEventArgs)e).Gauge;
                last = ((InternalZoomGestureEventArgs)e).Gauge;
            }
            else
            {
                    //Debug.WriteLine("start: " + start + " last: " + last);
                Successful(this, new ZoomGestureEventArgs(){
                    ZoomFactorFromBegin = ((InternalZoomGestureEventArgs)e).Gauge/start,
                    ZoomFactorFromLast = ((InternalZoomGestureEventArgs)e).Gauge/last
                });
                last = ((InternalZoomGestureEventArgs)e).Gauge;
            }
        }

        protected override void fireFailed(Object sender, FailedGestureEventArgs e)
        {
            //TODO reset Zoom does not work
            start = -1.0;
            Debug.WriteLine("reset zoom");
            Failed(this, e);
        }
    }
}
