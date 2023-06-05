using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.GraphicsEng
{
    public abstract class GenericActiveReader : IDisposable
    {
        protected object mObject = null;

        public abstract void Dispose();

        public void MonitoredObjectDisposed()
        {
            mObject = null;
        }
    }

    public class ActiveReader<T> : GenericActiveReader where T : IMonitoredObject
    {
        public ActiveReader()
        {
        }

        public ActiveReader(T monitoredObject)
        {
            mObject = monitoredObject;
            ((T)mObject).AddReader(this);
        }

        public override void Dispose()
        {
            if (mObject != null)
            {
                ((T)mObject).RemoveReader(this) ;
                mObject = default(T);
            }
        }

        public void Assign(ActiveReader<T> assign) 
        {
            throw new Exception("Don't call assign with active reader");
        }

        public void Assign(T assign)  
        {
            if (mObject != null)
            {
                ((T)mObject).RemoveReader(this);
            }
            mObject = assign;
            if (mObject != null)
            {
                ((T)mObject).AddReader(this);
            }
        }

        public T Object
        {
            get { return (T)mObject; }
        }

        public static implicit operator T(ActiveReader<T> m)
        {
            return (T)m.mObject;
        }

        public bool IsNull()
        {
            return mObject == null;
        }
    }
}
