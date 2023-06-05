using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.GraphicsEng
{
    public interface IMonitoredObject
    {
        void RemoveReader(GenericActiveReader toRemove);
        void AddReader(GenericActiveReader toInform);
    }
}
