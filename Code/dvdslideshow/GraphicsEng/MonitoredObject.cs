using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.GraphicsEng
{
    public delegate void NoMoreReadersCallback( IMonitoredObject monitoredObject);

    public class MonitoredObject : IMonitoredObject, IDisposable
    {
        public event NoMoreReadersCallback NoMoreReaders;

        private IMonitoredObject mReferenceObject = null;  // if the monitored object is not s child of this class this is set

        public  MonitoredObject()
        {
        }

        public MonitoredObject(IMonitoredObject referencedObject)
        {
            mReferenceObject = referencedObject;
        }

        private List<GenericActiveReader> mReaders = null;

        public void RemoveReader(GenericActiveReader toRemove)
        {
            if (mReaders == null)
            {
                ManagedCore.CDebugLog.GetInstance().Error("MonitoredObject::RemoveReader called but not referencing any readers");
                return;
            }

            if (mReaders.Remove(toRemove) == true)
            {
                if (mReaders.Count == 0)
                {
                    if (NoMoreReaders != null)
                    {
                        if (mReferenceObject != null)
                        {
                            NoMoreReaders(mReferenceObject);
                        }
                        else
                        {
                            NoMoreReaders(this);
                        }
                    }
                }
            }
            else
            {
                ManagedCore.CDebugLog.GetInstance().Error("MonitoredObject::RemoveReader called but not referencing the passed in reader");
            }
        }

        public void AddReader(GenericActiveReader toInform)
        {
            if (mReaders == null) mReaders = new List<GenericActiveReader>();

            mReaders.Add(toInform);
        }

        public void Dispose()
        {
            if (mReaders != null)
            {
                foreach (GenericActiveReader reader in mReaders)
                {
                    reader.MonitoredObjectDisposed();
                }
            }
        }
    }
}
