using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow
{
    /// <summary>
    /// This class is a wrapper around objects that implement IDisposable and guarantees to dispose of the object when dispose is called on itself.
    /// This class, however, allows addition features which can not be done with a normal IDisposbable object and the "using" statement.
    ///   * This wrapper allows us to re-assign to different IDisposbable objects (Disposing old objects if required)
    ///   * Take ownership off from another DisposableObject (Disposing old objects if required)
    ///   * Allows us to "Release" the internal disposable object (e.g. like c++ std::ato_ptr )
    ///   
    /// Instances of this class should still typically be done in a "using" statement
    /// 
    /// When this DisposableObject goes out of scope from the "using" statement, if the internal object is still assigned, Dispose() will be called on it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DisposableObject<T> : IDisposable where T : IDisposable
    {
        private T mObject = default(T);

        public DisposableObject()
        {
        }

        public DisposableObject(T disposable)
        {
            mObject = disposable;
        }

        public void Dispose()
        {
            if (mObject != null)
            {
                mObject.Dispose();
                mObject = default(T);
            }
        }

        public void Assign(T assign)
        {
            if (mObject != null)
            {
                mObject.Dispose();
            }
            mObject = assign;
        }

        public void Assign(DisposableObject<T> assign)
        {
            if (mObject != null)
            {
                mObject.Dispose();
            }

            if (assign == null)
            {
                mObject = default(T);
            }
            else
            {
                mObject = assign.Release();
            }
        }

        public T Object
        {
            get { return mObject; }
        }

        public static implicit operator T(DisposableObject<T> m)
		{
			return m.mObject;
		}

        public bool IsNull()
        {
            return mObject==null;
        }

        public T Release()
        {
            T return_object = mObject;
            mObject = default(T);
            return return_object;
        }
    }
}
