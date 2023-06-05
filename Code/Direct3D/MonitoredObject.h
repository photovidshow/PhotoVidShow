#pragma once

#include <list>

class GenericActiveReader;

class MonitoredObject 
{
public:
	MonitoredObject();
	virtual ~MonitoredObject() ;
	void RemoveDeletionEvent(GenericActiveReader* to_remove) ;
	void AddDeletionEvent(GenericActiveReader* to_inform) ;
	virtual void Shutdown() ;

protected:
	std::list<GenericActiveReader*>* mListenersToInformWhenIDie ;
};

