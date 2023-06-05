
using System;
using System.Drawing;

namespace DVDSlideshow.GraphicsEng
{
    //
    // Represnets a type of surface that can be rendered to
    //
    // After rendering to, you could also then render this surface to another RenderSurface
    //
    // This class implements IMonitoredObject becuase slideshow objects will need ActiveReaders to this surface as the 
    // RenderSurface is really owned by the device and may be destoyed if the devivce is lost
    //
    //
    public abstract class RenderSurface : Surface, IMonitoredObject
    {
        public enum State
        {
            OK,
            LOST
        }

        private MonitoredObject mMonitoredObject = null;

        public MonitoredObject Monitor
        {
            get { return mMonitoredObject; } 
        }

        public abstract RenderSurface.State GetState();

        public void RemoveReader(GenericActiveReader toRemove)
        {
            if (mMonitoredObject != null)
            {
                mMonitoredObject.RemoveReader(toRemove);
            }
        }

        public void AddReader(GenericActiveReader toInform)
        {
            if (mMonitoredObject == null)
            {
                mMonitoredObject = new MonitoredObject(this);
            }

            mMonitoredObject.AddReader(toInform);
        }

        public override void Dispose()
        {
            if (mMonitoredObject != null)
            {
                mMonitoredObject.Dispose();
                mMonitoredObject = null;
            }
        }


    }
}
