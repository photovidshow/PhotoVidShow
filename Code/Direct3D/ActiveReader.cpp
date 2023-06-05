#include "ActiveReader.h"

//*************************************************************************************************
void GenericActiveReader::SetReader(MonitoredObject* to_read)
{
	if (to_read == mToRead) return ;

	if (mToRead)
	{
		mToRead->RemoveDeletionEvent(this) ;
	}
	mToRead = to_read ; 
	if (mToRead)
	{
		mToRead->AddDeletionEvent(this);
	}
}
