using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.GraphicsEng
{
    public class GraphicsEngineInitialiseResult
    {      
        private bool mInitialisedOk = false;
        private GraphicsEngineCapabilities mCapabilities;

        // **********************************************************************************************
        public bool InitialisedOK
        {
            get { return mInitialisedOk; }
        }

        // **********************************************************************************************
        public GraphicsEngineCapabilities Capabilities
        {
            get { return mCapabilities; }
        }
       
        // **********************************************************************************************
        public GraphicsEngineInitialiseResult(bool initialisedOk, GraphicsEngineCapabilities capabilities)
        {
            mInitialisedOk = initialisedOk;
            mCapabilities = capabilities;
        }
    }
}
