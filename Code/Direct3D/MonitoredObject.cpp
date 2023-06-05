#include "MonitoredObject.h"
#include "ActiveReader.h"

//******************************************************************************************
MonitoredObject::MonitoredObject() : 
  mListenersToInformWhenIDie(0) 
{
}

//******************************************************************************************
MonitoredObject::~MonitoredObject() 
{
	if (mListenersToInformWhenIDie != 0)
	{
		MonitoredObject::Shutdown();
	}
}

//******************************************************************************************
void MonitoredObject::Shutdown()
{
	if (!mListenersToInformWhenIDie)
		return ;
	
	std::list<GenericActiveReader*>::iterator it = mListenersToInformWhenIDie->begin();

	for (; it != mListenersToInformWhenIDie->end();  ++it)
	{
		 (*it)->ToReadDied();
	}

	delete mListenersToInformWhenIDie;
}
 


//******************************************************************************************
void MonitoredObject::RemoveDeletionEvent(GenericActiveReader* to_remove) 
{
	if (mListenersToInformWhenIDie)
	{
		mListenersToInformWhenIDie->remove(to_remove) ;
	}
}


//******************************************************************************************
void	MonitoredObject::AddDeletionEvent(GenericActiveReader* to_inform) 
{
	if (!mListenersToInformWhenIDie)
	{
		mListenersToInformWhenIDie = new std::list<GenericActiveReader*> ;
	}

	mListenersToInformWhenIDie->push_back(to_inform) ; 
}

