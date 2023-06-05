#ifndef ACTIVE_READER_INCLUDE
#define ACTIVE_READER_INCLUDE


#include "MonitoredObject.h"

class GenericActiveReader 
{
public:

	void SetReader(MonitoredObject* to_read) ; 
    ~GenericActiveReader() { if (mToRead) { mToRead->RemoveDeletionEvent(this) ; }}

	void ToReadDied() { mToRead = NULL ; }

protected:

	MonitoredObject* mToRead ; 
};



template <class T> class ActiveReader : public GenericActiveReader
{
public:
	ActiveReader() { mToRead = NULL ; }
	ActiveReader(T* to_read) {mToRead = (MonitoredObject*)to_read ;  if (mToRead) { mToRead->AddDeletionEvent(this); } }
	ActiveReader(ActiveReader<T>& copy) { mToRead = (MonitoredObject*)copy.mToRead ; if (mToRead) { mToRead->AddDeletionEvent(this); } }
	void	SetReader(T* to_read) { GenericActiveReader::SetReader((MonitoredObject*)to_read);}
	bool	operator ==(T* right_term) { return mToRead == right_term; }
	bool    operator !=(T* right_term) { return mToRead != right_term; }
	void	operator =(T* right_term) { SetReader(right_term); }
	T*		operator ->() { return (T*)mToRead ; }
	T*		ToRead() { return (T*)mToRead ; }
			operator T*() { return (T*)mToRead ; } 
};


#endif
