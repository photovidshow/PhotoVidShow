#include "StdAfx.h"
#include "Settings.h"

namespace Direct3D 
{

//***************************************************************************
Settings::Settings() :
	mUseManagedPoolForNormalTextures(false)
{
}

	
//***************************************************************************
bool Settings::GetUseManagedPoolForNormalTextures()
{
	return mUseManagedPoolForNormalTextures;
}


//***************************************************************************
void Settings::SetUseManagedPoolForNormalTextures(bool value)
{
	mUseManagedPoolForNormalTextures = value;
}

}
