#include "StdAfx.h"
#include ".\videodiskmanager.h"
#include <windows.h>        		// Target OS
#include <stdio.h>          		// Generic I/O stuff
#include "StarBurn/Include/StarBurn.h"	// Our public header
#include "DVDDisk.h"
#include "../VideoTools/UnmanagedErrors.h"


namespace Burner
{



CVideoDiskManager* CVideoDiskManager::mInstance =0;


CVideoDiskManager&	CVideoDiskManager::GetInstance()
{
	if (mInstance==NULL)
	{
		mInstance = new CVideoDiskManager();
	}
	return *mInstance;
}


CVideoDiskManager::CVideoDiskManager(void) 
{
}

CVideoDiskManager::~CVideoDiskManager(void)
{
}


	
//
// Own includes (begin)
//



// my old key

/*
static unsigned char g__UCHAR__RegistrationKey[ ] = 
{
0x00, 0x8A, 0x04, 0x8A, 0xE9, 0x5F, 0x8C, 0x8E, 0x27, 0xF9, 0x74, 0xFF, 0x61, 0xF8, 0x74, 0xA4,
0x67, 0xE3, 0x6C, 0xE6, 0x61, 0xE7, 0x40, 0xE8, 0x74, 0xE3, 0x6E, 0xFE, 0x65, 0xF8, 0x6E, 0xEF,
0x74, 0xA4, 0x63, 0xE5, 0x6D, 0xAD, 0x00, 0xFF, 0x4C, 0xE4, 0xD5, 0xC1, 0xA8, 0xB5, 0xD7, 0xCD,
0x83, 0xA3, 0x43, 0xEC, 0x67, 0xD3, 0xBF, 0xDD, 0x9C, 0x93, 0xC3, 0x9A, 0xC5, 0xD0, 0x4B, 0xF7,
0x48, 0xAD, 0x15, 0xF1, 0x14, 0xBD, 0xD5, 0x9C, 0x87, 0xC4, 0x06, 0xA6, 0x78, 0xA3, 0x40, 0xCE,
0xBD, 0xF1, 0x39, 0xB0, 0x9C, 0x90, 0xDE, 0x8F, 0x08, 0xB5, 0x27, 0x88, 0xA3, 0x84, 0x91, 0xC8,
0xD1, 0xC3, 0x65, 0xF7, 0x62, 0xA5, 0x31, 0xF3, 0xA1, 0xE0, 0x26, 0x95, 0xCC, 0xCC, 0x2E, 0xAB,
0x36, 0xF1, 0x00, 0xD0, 0xAD, 0xB7, 0x77, 0xF4, 0xD7, 0xFE, 0xF3, 0xB8, 0x9C, 0xF8, 0x9A, 0xE6,
0xE4, 0x94, 0xA7, 0x8F, 0x2A, 0xF7, 0x2C, 0xEF, 0xB1, 0x81, 0xF5, 0xD7, 0x60, 0xBB, 0x6B, 0xCF,
0x94, 0xDD, 0xA9, 0x80, 0x69, 0xFD, 0x22, 0x9D, 0x3D, 0xD4, 0x86, 0xC7, 0x8B, 0xC1, 0xCD, 0xEA,
0x46, 0xD0, 0x8B, 0xC6, 0x5C, 0xF0, 0xFE, 0xCA, 0x0B, 0xA9, 0x75, 0xA2, 0x32, 0xE7, 0xFF, 0xD2,
0xBC, 0xF6, 0x85, 0xD6, 0xDB, 0x90, 0xB9, 0xC0, 0xB1, 0xA4, 0x83, 0xC1, 0x8E, 0xFA, 0xD4, 0xC7,
0xFD, 0xD9, 0x03, 0x94, 0x22, 0xCD, 0x20, 0xAA, 0x45, 0x90, 0xE7, 0xDF, 0x7E, 0xB2, 0x34, 0xDF,
0xD3, 0xE8, 0x25, 0xB1, 0x51, 0xE3, 0x55, 0xC3, 0xE6, 0xF3, 0xCB, 0x8F, 0x03, 0x91, 0x99, 0xB1,
0x4C, 0xD2, 0x40, 0xE3, 0xEB, 0x93, 0x4B, 0xD9, 0x33, 0xA9, 0xCC, 0xEE, 0xC2, 0xD1, 0x92, 0x9E,
0x3D, 0xD4, 0x5A, 0xF7, 0x57, 0xE8, 0x4E, 0x82, 0xD2, 0x89, 0x7D, 0xCE, 0x85, 0xBC, 0x43, 0x91,
0xBA, 0xFD, 0xB2, 0xDA, 0x62, 0x85, 0x79, 0xF2, 0xEB, 0x8D, 0xE5, 0x80, 0xBA, 0xF3, 0xE2, 0xE8,
0x9F, 0xE5, 0x36, 0xFF, 0xBC, 0x99, 0x3E, 0xD8, 0xAA, 0x9A, 0xFC, 0xC4, 0xF2, 0xBA, 0x3C, 0xDD,
0x0B, 0xDD, 0x0A, 0xB0, 0x77, 0xF0, 0xE2, 0x80, 0xC1, 0xF0, 0x32, 0xCF, 0x64, 0xD5, 0x2E, 0x87,
0xE0, 0x99, 0x08, 0x8A, 0x8B, 0xE3, 0xFE, 0xDC, 0xE3, 0xBE, 0xE8, 0xF1, 0x68, 0xC0, 0x2D, 0xE9,
0x44, 0xF5, 0x3B, 0xD4, 0x54, 0xBE, 0xFF, 0xF8, 0x4A, 0xEC, 0xF5, 0xED, 0xFC, 0xD9, 0xBF, 0xFB,
0x23, 0x9E, 0x62, 0x9C, 0x12, 0xF4, 0xA6, 0xB1, 0x32, 0xAF, 0x23, 0xCB, 0x42, 0xEF, 0xFF, 0xC1,
0xAA, 0xA7, 0x74, 0xD3, 0x68, 0xE6, 0x89, 0x9C, 0x5C, 0xB7, 0xB0, 0xA4, 0x00, 0xE3, 0x1D, 0x9A,
0xCD, 0xDF, 0x16, 0xD6, 0xDD, 0xE3, 0x90, 0xA0, 0x8C, 0x8C, 0xCD, 0xC9, 0x1F, 0xDC, 0xDB, 0xCD,
0xC2, 0xA9, 0x26, 0xB1, 0x5F, 0xF2, 0x79, 0xE2, 0x0B, 0xC1, 0x22, 0xC6, 0x2C, 0xC0, 0x11, 0x93,
0x84, 0xA0, 0x34, 0xBA, 0xBB, 0x99, 0x55, 0xE9, 0x26, 0xB4, 0x47, 0xE2, 0xDA, 0xB9, 0x2A, 0xCC,
0x52, 0xFD, 0x03, 0x9C, 0x26, 0xAE, 0x09, 0xF3, 0xAC, 0x88, 0x4C, 0xF0, 0x7E, 0x87, 0xA5, 0xF4,
0x2D, 0xAA, 0x0C, 0xB7, 0xB7, 0x8A, 0xCE, 0xC0, 0x74, 0xCB, 0x33, 0xE8, 0x91, 0xAD, 0x95, 0xF5,
0x5D, 0xE8, 0xFC, 0x85, 0xEA, 0x8B, 0xB1, 0x8F, 0xD5, 0xC5, 0x71, 0x80, 0x32, 0x89, 0x21, 0x8E,
0xED, 0xDA, 0x31, 0x82, 0x1F, 0xAE, 0x13, 0xF1, 0x2D, 0xEC, 0x71, 0xCE, 0xA2, 0xB6, 0x05, 0x8C,
0x2A, 0x99, 0x41, 0xF8, 0x19, 0xB0, 0x2A, 0x83, 0x5A, 0xEC, 0x10, 0xFF, 0xC6, 0x8F, 0x0F, 0xC4,
0x28, 0xB9, 0x73, 0x88, 0x80, 0xA5, 0x7E, 0xD9, 0x44, 0xB8, 0x22, 0x9B, 0xA8, 0xBA, 0xA3, 0xDE,
0x3D, 0xF6, 0x43, 0xB3, 0x5F, 0xD1, 0x6D, 0xC1, 0x51, 0x98, 0xEC, 0xE3, 0xF6, 0xC3, 0x37, 0xF8,
0x84, 0xE3, 0xE3, 0x9D, 0xA6, 0xE8, 0xA8, 0x82, 0xEF, 0xA1, 0xA8, 0xC3, 0x82, 0xC9, 0xD7, 0xD5,
0x5B, 0xAE, 0xB5, 0xAE, 0xA9, 0xA0, 0xB3, 0xFB, 0x0E, 0x82, 0x04, 0xEA, 0xC0, 0xA4, 0xA3, 0x8D,
0xE5, 0x98, 0xD2, 0xB5, 0xA0, 0xD1, 0x69, 0xBE, 0xA3, 0xA3, 0xA3, 0xF4, 0x4B, 0x8D, 0x4C, 0xAF,
0x89, 0xFC, 0x86, 0xCD, 0x25, 0xF9, 0x76, 0x84, 0x27, 0xBE, 0x9A, 0xAF, 0x61, 0x87, 0x9B, 0xBC,
0x71, 0x90, 0xD1, 0x86, 0xB9, 0xEC, 0xDB, 0xEF, 0x15, 0xA8, 0xF4, 0xD5, 0x62, 0xBA, 0xEA, 0xB1,
0x0C, 0x98, 0xE7, 0x9D, 0x3F, 0xDC, 0x6E, 0xBB, 0x6F, 0x81, 0x2F, 0xFA, 0x55, 0x83, 0xA8, 0xC2,
0x8D, 0x8D, 0xB1, 0xB6, 0x7F, 0xC4, 0x57, 0xBB, 0x3A, 0xF6, 0xBC, 0xC6, 0x62, 0xEF, 0xD9, 0xE9,
0x6A, 0x90, 0x4A, 0xF5, 0xA4, 0x91, 0x94, 0xFD, 0xFE, 0xD0, 0x83, 0xE6, 0x59, 0xEA, 0x94, 0xB2,
0xDD, 0xC7, 0x83, 0xC2, 0xBE, 0xA1, 0x75, 0xCA, 0x47, 0xD5, 0x5D, 0xE9, 0x2A, 0x85, 0x84, 0x9C,
0x66, 0x8D, 0x60, 0xE0, 0x41, 0x8B, 0x21, 0x91, 0x26, 0x92, 0x99, 0xC4, 0x6B, 0xB0, 0x68, 0xC8,
0x47, 0x97, 0x98, 0xFE, 0x85, 0xBF, 0x10, 0xE8, 0xB1, 0x88, 0x77, 0x9B, 0xD6, 0xA2, 0x92, 0x9C,
0x07, 0xFF, 0x19, 0xD3, 0x44, 0x81, 0x8E, 0x89, 0x7E, 0xBA, 0xB0, 0xEB, 0xC7, 0xF3, 0x6B, 0xF9,
0xF0, 0xBB, 0x82, 0xEA, 0x1F, 0x9B, 0x3F, 0x90, 0x2C, 0xF4, 0xEC, 0xA9, 0xC2, 0x85, 0xEE, 0xF5,
0x91, 0xF9, 0xA7, 0xE4, 0x19, 0xDE, 0x95, 0xDF, 0xDB, 0xD3, 0x4A, 0x92, 0xEB, 0x80, 0x29, 0xB0,
0x3C, 0xAE, 0x10, 0x9D, 0x1A, 0xA8, 0x5B, 0xB8, 0xAF, 0x93, 0xDC, 0xDC, 0x8D, 0xD9, 0xBF, 0xDD,
0x89, 0xDD, 0x7A, 0x8B, 0x6D, 0xD6, 0x2E, 0x89, 0x46, 0xF8, 0x69, 0xAA, 0x53, 0xEF, 0x70, 0xAA,
0x30, 0xBB, 0x20, 0xBA, 0x39, 0xB0, 0x31, 0xB9, 0x3A, 0xB8, 0x31, 0xAA, 0x32, 0xBA, 0x30, 0xBC,
0x0A, 0x8A, 0x44, 0xA1, 0x2D, 0xD1, 0x92, 0xD1, 0x35, 0x8A, 0x4B, 0xD4, 0xBE, 0xED, 0x34, 0x83,
0x93, 0xBA, 0xA0, 0x8D, 0xA6, 0x83, 0x01, 0xE8, 0xDC, 0xAB, 0xF2, 0x90, 0x70, 0x81, 0xA8, 0xDF,
0x17, 0xDD, 0xF6, 0xD2, 0x87, 0x87, 0x3A, 0x91, 0x1C, 0xA8, 0xF3, 0xAC, 0xA6, 0x85, 0xD3, 0xEE,
0xA9, 0x92, 0x84, 0xF5, 0x91, 0xA4, 0x7D, 0xFC, 0xB1, 0xD5, 0x9A, 0xEA, 0x03, 0xE0, 0x51, 0xAE,
0xF7, 0xCA, 0xC0, 0xD0, 0xE7, 0x8C, 0xE0, 0xF1, 0xDC, 0xB4, 0xA3, 0xAF, 0xCA, 0xEB, 0xD1, 0x96,
0x75, 0x90, 0xD1, 0xD7, 0x91, 0xCF, 0xCB, 0x8E, 0xE1, 0xF5, 0xC1, 0xC5, 0x67, 0x90, 0x95, 0xD8,
0xDA, 0xC6, 0x14, 0xFE, 0xFA, 0x8A, 0x7B, 0xCE, 0x89, 0xAF, 0x1A, 0x8B, 0xE7, 0xA7, 0xF4, 0xFF,
0x9F, 0xC1, 0x98, 0xBF, 0x72, 0x93, 0x83, 0xAA, 0xA1, 0xE1, 0xC8, 0xCA, 0x7B, 0xDD, 0xDB, 0xE9,
0x83, 0xDE, 0xA2, 0xE1, 0xAE, 0xA1, 0x46, 0xD3, 0x79, 0xC7, 0x5A, 0xED, 0xF9, 0xA7, 0x47, 0xFC,
0x0A, 0xF4, 0x2A, 0xEF, 0x43, 0x99, 0x7E, 0x88, 0x66, 0x96, 0x51, 0xD1, 0x5B, 0xE4, 0xCD, 0xAE,
0xF9, 0xDF, 0x5C, 0x91, 0x2E, 0xF7, 0xB9, 0xC1, 0x40, 0xAC, 0xA4, 0x97, 0x3E, 0xDC, 0x13, 0xFF,
0xDC, 0x85, 0x18, 0xAC, 0x4F, 0x80, 0xD7, 0x92, 0xE5, 0xE1, 0x3C, 0xAC, 0x63, 0x84, 0x54, 0xB1,
0x80, 0xC9, 0x72, 0x8A, 0xE9, 0x8B, 0x8D, 0xE0, 0xB4, 0xED, 0x79, 0x9A, 0x32, 0xAF, 0xE0, 0xA0,
0x79, 0xB1, 0x33, 0x97, 0x24, 0x9E, 0xE5, 0x9C, 0x12, 0xEE, 0xAB, 0x9D, 0x32, 0x93, 0x99, 0xBC
};

*/

static unsigned char g__UCHAR__RegistrationKey[ ] = 
{
0x44, 0xB6, 0x4A, 0xB6, 0x8B, 0xB2, 0x44, 0xB6, 0x17, 0xC2, 0x31, 0xD7, 0x36, 0xC2, 0x1B, 0xF1,
0x2D, 0xDA, 0x28, 0xD7, 0x29, 0xE9, 0x27, 0xC3, 0x37, 0xC2, 0x2B, 0xDB, 0x21, 0xC4, 0x1B, 0xE5,
0x30, 0xC3, 0x25, 0xC4, 0x30, 0xE9, 0x03, 0xDF, 0x28, 0xDA, 0x25, 0xDB, 0x44, 0xF4, 0x9C, 0xDB,
0x55, 0x8A, 0xEA, 0xDE, 0xEA, 0xFA, 0x5C, 0xA2, 0x89, 0xA6, 0x07, 0xEC, 0x8B, 0xE3, 0x16, 0xE1,
0xCC, 0xF4, 0x52, 0xBB, 0xCA, 0x80, 0xF5, 0xE3, 0x94, 0xF6, 0x0E, 0xA2, 0x48, 0xE6, 0x63, 0xEA,
0xA7, 0xA5, 0xB1, 0xFF, 0xDE, 0xCD, 0x48, 0xDE, 0x81, 0xD1, 0x9B, 0xCB, 0x35, 0xB8, 0x4C, 0xDA,
0xCB, 0xFB, 0xC3, 0xB7, 0xE0, 0xF3, 0x76, 0xEA, 0x4E, 0x91, 0x23, 0x83, 0x16, 0x98, 0x3D, 0xF3,
0x38, 0xC6, 0x3D, 0xCD, 0x64, 0xE8, 0x00, 0xC2, 0x86, 0xEA, 0xBE, 0x84, 0xA2, 0xB9, 0x11, 0xD8,
0xDB, 0xC1, 0x2A, 0x97, 0xED, 0xCE, 0xB7, 0xF5, 0x10, 0xF4, 0xA0, 0xDA, 0x8E, 0xDE, 0x0C, 0xCA,
0x35, 0xC8, 0x3F, 0xBD, 0x38, 0xBB, 0x4C, 0x97, 0xAE, 0xAC, 0x61, 0xFA, 0x3F, 0xD0, 0x17, 0x9B,
0x29, 0xC1, 0xEF, 0xEA, 0xB8, 0xF5, 0x99, 0xD5, 0xB7, 0xC7, 0xB6, 0xC6, 0x9B, 0xD6, 0x08, 0xE6,
0xD2, 0xC4, 0x38, 0xD9, 0x68, 0xDB, 0xA5, 0xC1, 0xE6, 0xDF, 0x75, 0xF1, 0xF9, 0xAA, 0x1F, 0xCB,
0xAA, 0xC5, 0x24, 0xE4, 0x7C, 0xB8, 0x5C, 0x8A, 0x29, 0xAF, 0x45, 0xDD, 0x05, 0xA8, 0x3A, 0xA8,
0x44, 0xEC, 0x79, 0xA2, 0xA9, 0xCC, 0xDD, 0x85, 0xCA, 0xCA, 0x74, 0xEB, 0x3D, 0xBD, 0x41, 0xFE,
0x13, 0xE2, 0xA9, 0xD6, 0x78, 0xB6, 0x4B, 0xB0, 0xC1, 0xC6, 0x86, 0xB1, 0x9E, 0xA8, 0x53, 0xD7,
0x40, 0xDC, 0xFA, 0xDB, 0xFF, 0xA8, 0x7D, 0xE6, 0xD0, 0xF5, 0x35, 0xA4, 0xFF, 0x9F, 0x68, 0xAA,
0x59, 0xA6, 0xB7, 0x83, 0x67, 0xE3, 0x2D, 0xF2, 0x06, 0xE5, 0x6C, 0xB1, 0x9D, 0x90, 0xD4, 0x89,
0x7C, 0xE6, 0x05, 0xB1, 0x97, 0xCC, 0x4C, 0xAD, 0x9D, 0x8C, 0xC2, 0xF0, 0x95, 0xEA, 0xA2, 0x8B,
0xB0, 0xC8, 0x17, 0xE8, 0x08, 0x9C, 0x55, 0xE5, 0xAC, 0xA7, 0x25, 0xFA, 0xED, 0xA6, 0xE7, 0xA8,
0x35, 0xB4, 0x03, 0xA5, 0xF7, 0x8D, 0xD6, 0xF4, 0x85, 0x8C, 0x05, 0xF2, 0x0C, 0xFE, 0xCB, 0x80,
0x97, 0x9E, 0x6B, 0xE0, 0x16, 0xF9, 0x6A, 0xC1, 0xB8, 0xF6, 0xA5, 0xDB, 0xBE, 0xAA, 0x0C, 0x86,
0x87, 0xB8, 0xD5, 0xCD, 0x5C, 0xDE, 0xDA, 0xD0, 0xB6, 0xF1, 0xF5, 0xE7, 0xA9, 0xAB, 0xFA, 0xD2,
0xA9, 0xA3, 0xAC, 0xAE, 0x50, 0xA0, 0x6F, 0xD9, 0x07, 0xBF, 0x13, 0xF3, 0xF2, 0xF4, 0xFC, 0xB5,
0xC7, 0xDD, 0x64, 0xC7, 0xBA, 0x86, 0x71, 0x9E, 0x15, 0xD5, 0x6D, 0xF6, 0x88, 0xDB, 0x09, 0xBA,
0xFC, 0xA8, 0x59, 0x87, 0xEA, 0x94, 0xEB, 0xB4, 0x86, 0xC1, 0xF0, 0xC9, 0x48, 0xC9, 0x28, 0xA3,
0x02, 0xF1, 0x71, 0xCA, 0xE6, 0xF7, 0xE4, 0x8F, 0x46, 0xEC, 0x7D, 0xCA, 0x77, 0xC0, 0x11, 0xD0,
0xAD, 0x8D, 0xF5, 0xFE, 0x38, 0x8D, 0xE0, 0xA4, 0xFA, 0xFB, 0x6E, 0x9B, 0x2C, 0x93, 0x59, 0xE9,
0xA6, 0xEE, 0x14, 0xBA, 0xC6, 0xB9, 0x2E, 0xD8, 0x2F, 0xA3, 0xF5, 0xAB, 0xEB, 0xC5, 0x6A, 0x87,
0x31, 0xE0, 0x80, 0x8E, 0x59, 0xAF, 0x77, 0x9F, 0xF9, 0xE6, 0x6E, 0xE7, 0x33, 0x99, 0x81, 0xD1,
0x28, 0x99, 0x77, 0xBE, 0x1F, 0x9C, 0x1D, 0xFD, 0x74, 0xDD, 0xE0, 0xC1, 0x49, 0xC7, 0x37, 0xF6,
0xD5, 0xA1, 0xFC, 0xAF, 0x29, 0x80, 0x82, 0xBF, 0xC7, 0xAC, 0x20, 0xC4, 0x62, 0xBB, 0xD8, 0xE5,
0x73, 0xB4, 0x9B, 0xEB, 0x6D, 0xBA, 0x3C, 0xA0, 0x73, 0xDB, 0x26, 0x9E, 0x1E, 0xA1, 0xBE, 0xBA,
0xF6, 0x8E, 0x69, 0xE5, 0xCC, 0x96, 0x40, 0xF7, 0x50, 0xDF, 0x8E, 0x9C, 0xE7, 0xC2, 0x5F, 0xC2,
0x9E, 0xB6, 0xD1, 0xA4, 0x6D, 0xDF, 0x54, 0xD1, 0xE1, 0x8F, 0x38, 0x9F, 0xA6, 0xCB, 0x9B, 0xF2,
0xC9, 0xE5, 0x40, 0x9A, 0x89, 0xF0, 0x93, 0x82, 0xF3, 0xD8, 0xC8, 0x81, 0x1E, 0x84, 0xBF, 0xE1,
0x6E, 0x95, 0x92, 0xE4, 0x9D, 0xC9, 0xED, 0x83, 0x9A, 0xFC, 0x87, 0xE9, 0x77, 0xCC, 0x3D, 0xED,
0x52, 0x84, 0x1A, 0xEC, 0x9C, 0xFC, 0x2C, 0x82, 0xB4, 0x97, 0x58, 0xBF, 0xBC, 0xCE, 0x72, 0xED,
0x7F, 0xCF, 0x27, 0xB4, 0xB2, 0xDE, 0x48, 0xDA, 0xCC, 0xF1, 0x0C, 0x9D, 0x5E, 0xC6, 0x26, 0xAF,
0xC0, 0xA8, 0xD9, 0xA5, 0xC2, 0xE6, 0x64, 0xBE, 0xC1, 0xBB, 0x6F, 0xE5, 0x2F, 0x87, 0x47, 0x81,
0x26, 0xCD, 0x78, 0xFA, 0x38, 0xD1, 0x07, 0xC3, 0xCC, 0xE4, 0x19, 0xC1, 0xE7, 0x97, 0x37, 0x9A,
0x0E, 0x85, 0x25, 0xAA, 0xB7, 0xFB, 0xEC, 0xB7, 0x3D, 0x96, 0xED, 0xAA, 0x9B, 0xFC, 0x99, 0xA7,
0x71, 0xEC, 0xE7, 0x84, 0xED, 0x8E, 0xD2, 0xFB, 0x46, 0xED, 0xD5, 0xFC, 0x59, 0xC9, 0x80, 0x81,
0x5B, 0x9D, 0xC8, 0xEE, 0x3A, 0xED, 0xAD, 0xBD, 0x57, 0xC8, 0x8C, 0xF9, 0x85, 0xBC, 0x38, 0xB9,
0xCC, 0xF8, 0x5D, 0xA9, 0xBD, 0xD5, 0xCB, 0xA4, 0x18, 0xDA, 0xF2, 0xE0, 0x57, 0xC1, 0xEB, 0xD5,
0xB2, 0x86, 0x7A, 0xED, 0x87, 0xB5, 0x48, 0x8D, 0x92, 0xFB, 0xBC, 0xFF, 0xFA, 0xEA, 0x9E, 0xD6,
0x34, 0xDF, 0x1B, 0xC4, 0x6E, 0xBD, 0xB2, 0xA4, 0x7F, 0xC8, 0x9A, 0xBE, 0x16, 0xF0, 0x89, 0xDB,
0x41, 0x86, 0x9C, 0xC0, 0xDB, 0xA5, 0x3E, 0xEA, 0xCD, 0xBD, 0xAF, 0xDD, 0x80, 0x94, 0x6A, 0xE2,
0x07, 0xD2, 0x32, 0xA2, 0x7C, 0x8C, 0x91, 0xE8, 0xC7, 0xF2, 0x1C, 0xAF, 0xAD, 0xF1, 0xB2, 0x8E,
0x85, 0x95, 0xBF, 0xFA, 0x6B, 0xDC, 0x9F, 0xB0, 0x10, 0xC3, 0x21, 0x96, 0x00, 0xD3, 0x27, 0x96,
0x76, 0x85, 0x64, 0x87, 0x76, 0x8C, 0x70, 0x86, 0x7E, 0x87, 0x77, 0x96, 0x76, 0x86, 0x74, 0x8E,
0x4E, 0xB6, 0xCD, 0xE8, 0x8B, 0xFE, 0x5E, 0x9C, 0x54, 0xF3, 0x59, 0xAC, 0x98, 0xF7, 0xA5, 0xFD,
0x78, 0xBE, 0x1E, 0xDF, 0x65, 0xE5, 0x19, 0x89, 0x61, 0xFD, 0x8F, 0xB8, 0xCB, 0xF3, 0x54, 0x8E,
0x17, 0xBE, 0xE6, 0xAD, 0x5A, 0x95, 0x1B, 0xA9, 0xEA, 0x9B, 0xB9, 0xB1, 0x4C, 0xC8, 0xB5, 0xB8,
0x08, 0x8B, 0x2D, 0x96, 0x72, 0x8E, 0x1C, 0xFC, 0xCC, 0xC6, 0xE0, 0xDA, 0xB3, 0xE8, 0xB7, 0xCD,
0x10, 0xA7, 0xEC, 0xC5, 0xBC, 0xD8, 0x56, 0x8F, 0x94, 0xCB, 0xA0, 0xB6, 0x35, 0xA2, 0x18, 0xA7,
0x5F, 0xBC, 0x89, 0xF8, 0xD8, 0x8D, 0x2A, 0x9B, 0x2C, 0x82, 0xD1, 0x8C, 0x2F, 0xB3, 0x81, 0xF6,
0x11, 0xE9, 0x7E, 0xD3, 0x3E, 0xC2, 0x9A, 0xE4, 0x3D, 0xB0, 0x87, 0xD3, 0x07, 0xD0, 0xDF, 0x95,
0x78, 0x97, 0x83, 0xC6, 0xD5, 0x9A, 0xAD, 0x98, 0xA8, 0x91, 0x6A, 0xAF, 0x73, 0xED, 0xE4, 0xA5,
0x49, 0xD9, 0x7F, 0xB8, 0x45, 0xA8, 0xEF, 0xD7, 0xB4, 0xFA, 0x8F, 0xCC, 0xAF, 0x9C, 0x2C, 0xA5,
0xC3, 0x8B, 0x93, 0xDF, 0xD5, 0x92, 0x15, 0x80, 0x33, 0xCE, 0x04, 0xE2, 0xA3, 0xB6, 0x99, 0xB0,
0x2B, 0x9B, 0x88, 0x8D, 0x6E, 0xFE, 0xE5, 0xB5, 0xFC, 0xEC, 0x2F, 0xC1, 0x52, 0xF6, 0xFF, 0x8D,
0x4A, 0xC4, 0x84, 0xA2, 0x1E, 0xE1, 0x34, 0xEA, 0xEB, 0x89, 0x98, 0xC4, 0x0D, 0xD3, 0x50, 0xF4,
0xF5, 0xF1, 0x09, 0xCC, 0x96, 0xA7, 0x6E, 0xA6, 0xDC, 0xBB, 0x13, 0xA0, 0xF6, 0xCF, 0x89, 0xA2,
0x6C, 0xCC, 0xC5, 0xE5, 0xAC, 0xD4, 0x0B, 0xCD, 0x3C, 0xE5, 0x10, 0xF1, 0x64, 0xA4, 0x4C, 0x96
};

//
// Own global data region (end)
//


//
// Own global function bodies (begin)
//


VOID
__stdcall
Callback(
    IN CALLBACK_NUMBER p__CALLBACK_NUMBER,
    IN PVOID p__PVOID__CallbackContext,
    IN PVOID p__PVOID__CallbackSpecial1,
    IN PVOID p__PVOID__CallbackSpecial2
    )

/*++

Routine Description:

    Callback passed to ISO9660/Joliet tree to show progress indication (tree node item added or removed, LBA assigned, file opened, logical block written etc)

Arguments:

    Callback number,
    Passed callback context,
    Parameter 1,
    Parameter 2

Return Value:

    Nothing

--*/

{
    /*
    //
    // Process depending of callback number.
    //
    switch ( p__CALLBACK_NUMBER )
    {
        //
        // Unhandled callback number
        //
        default:
        {
            Trace( 
                "FindDevice:Callback(): Unhandled callback number 0x%08X, parameter 0x%08X\n",
                ( ULONG )( p__CALLBACK_NUMBER ),
                ( ULONG )( p__PVOID__CallbackContext )
                );

            
        }
    }
    */
}


int AddDeviceWhenFound=1;

VOID
__stdcall
FindCallback(
    IN CALLBACK_NUMBER p__CALLBACK_NUMBER,
    IN PVOID p__PVOID__CallbackContext,
    IN PVOID p__PVOID__CallbackSpecial1,
    IN PVOID p__PVOID__CallbackSpecial2
    )

/*++

Routine Description:

    Callback passed to library objects to show progress indication (tree node item added or removed, LBA assigned, file opened, logical block written etc)

Arguments:

    Callback number,
    Passed callback context,
    Parameter 1,
    Parameter 2

Return Value:

    Nothing

--*/

{
    ULONG l__ULONG__Status = ERROR_GEN_FAILURE;

    CHAR l__CHAR__VendorID[ 1024 ];

    CHAR l__CHAR__ProductID[ 1024 ];

    CHAR l__CHAR__ProductRevisionLevel[ 1024 ];

    ULONG l__ULONG__BufferSizeInUCHARs = 0;

    PVOID l__PVOID__CdvdBurnerGrabber = NULL;

    EXCEPTION_NUMBER l__EXCEPTION_NUMBER = EN_SUCCESS; // Assume success by default

    CHAR l__CHAR__ExceptionText[ 1024 ];

    CDB_FAILURE_INFORMATION l__CDB_FAILURE_INFORMATION;

    PSCSI_DEVICE_ADDRESS l__PSCSI_DEVICE_ADDRESS;

    BOOLEAN l__BOOLEAN__IsCDRRead;

    BOOLEAN l__BOOLEAN__IsCDERead;

    BOOLEAN l__BOOLEAN__IsDVDROMRead;

    BOOLEAN l__BOOLEAN__IsDVDRRead;

    BOOLEAN l__BOOLEAN__IsDVDRAMRead;

    BOOLEAN l__BOOLEAN__IsTestWrite;

    BOOLEAN l__BOOLEAN__IsCDRWrite;
    
    BOOLEAN l__BOOLEAN__IsCDEWrite;

    BOOLEAN l__BOOLEAN__IsDVDRWrite;

    BOOLEAN l__BOOLEAN__IsDVDRAMWrite;

    BOOLEAN l__BOOLEAN__IsDVDPLUSRWRead;

    BOOLEAN l__BOOLEAN__IsDVDPLUSRRead;

    BOOLEAN l__BOOLEAN__IsDVDPLUSRDLRead;

    BOOLEAN l__BOOLEAN__IsDVDPLUSRWWrite;

    BOOLEAN l__BOOLEAN__IsDVDPLUSRWrite;

    BOOLEAN l__BOOLEAN__IsDVDPLUSRDLWrite;

	char the_letter[2];


	CHAR l__CHAR__DeviceName[ 1024 ];

	the_letter[0]='E';
	the_letter[1]=NULL;

    //
    // Process depending of callback number.
    //
    switch ( p__CALLBACK_NUMBER )
    {
        //
        // if this is FIND_DEVICE
        //
        case CN_FIND_DEVICE:
        {
            //
            // Prepare data buffer
            //

            RtlZeroMemory(
                &l__CHAR__ExceptionText,
                sizeof( l__CHAR__ExceptionText )
                );

            l__PSCSI_DEVICE_ADDRESS = ( PSCSI_DEVICE_ADDRESS )( p__PVOID__CallbackSpecial1 );

            Trace( "Probing SCSI address %ld:%ld:%ld:%ld for CD/DVD device... ",
                l__PSCSI_DEVICE_ADDRESS->m__UCHAR__PortID,
                l__PSCSI_DEVICE_ADDRESS->m__UCHAR__BusID,
                l__PSCSI_DEVICE_ADDRESS->m__UCHAR__TargetID,
                l__PSCSI_DEVICE_ADDRESS->m__UCHAR__LUN
                );

            //
            // Start processing cleanup
            //
            //__try
            {
                //
                // Try to construct CD/DVD burner, passing 1 as cache size will make the toolkit allocate smallest possible
				// amount of cache memory. Good for searching (fast) and bad for true burning
                //
                l__EXCEPTION_NUMBER =
                    StarBurn_CdvdBurnerGrabber_Create(
                        &l__PVOID__CdvdBurnerGrabber,
                        ( PCHAR )( &l__CHAR__ExceptionText ),
                        sizeof( l__CHAR__ExceptionText ),
                        &l__ULONG__Status,
                        &l__CDB_FAILURE_INFORMATION,
                        ( PCALLBACK )( Callback ),
                        NULL,
                        l__PSCSI_DEVICE_ADDRESS->m__UCHAR__PortID,
                        l__PSCSI_DEVICE_ADDRESS->m__UCHAR__BusID,
                        l__PSCSI_DEVICE_ADDRESS->m__UCHAR__TargetID,
                        l__PSCSI_DEVICE_ADDRESS->m__UCHAR__LUN,
                        1 
                        );

                //
                // Check for success
                //
                if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
                {
                    Error("StarBurn_CdvdBurnerGrabber_Create() failed, exception %ld, status %ld, text '%s'",
                        l__EXCEPTION_NUMBER,
                        l__ULONG__Status,
                        l__CHAR__ExceptionText
                        );
    
                    //
                    // Get out of here
                    //
					return;
                }
    
                //
                // Prepare data buffers
                //

                RtlZeroMemory(
                    &l__CHAR__VendorID,
                    sizeof( l__CHAR__VendorID )
                    );

                RtlZeroMemory(
                    &l__CHAR__ProductID,
                    sizeof( l__CHAR__ProductID )
                    );      

                RtlZeroMemory(
                    &l__CHAR__ProductRevisionLevel,
                    sizeof( l__CHAR__ProductRevisionLevel )
                    );      

                //
                // Try to get CD/DVD burner information
                //
                StarBurn_CdvdBurnerGrabber_GetDeviceInformation(
                    l__PVOID__CdvdBurnerGrabber,
                    ( PCHAR )( &l__CHAR__VendorID ),
                    ( PCHAR )( &l__CHAR__ProductID ),
                    ( PCHAR )( &l__CHAR__ProductRevisionLevel ),
                    &l__ULONG__BufferSizeInUCHARs
                    );

                Trace("Found CD/DVD device '%s' '%s' '%s'",
                    l__CHAR__VendorID,
                    l__CHAR__ProductID,
                    l__CHAR__ProductRevisionLevel
                    );

                Trace("With %ld UCHAR(s) of cache",
                    l__ULONG__BufferSizeInUCHARs
                    );
                
                //                    
                // Get CdvdBurnerGrabber device extended information
                //
                StarBurn_CdvdBurnerGrabber_GetSupportedMediaFormats(
                    l__PVOID__CdvdBurnerGrabber,
                    &l__BOOLEAN__IsCDRRead,
                    &l__BOOLEAN__IsCDERead,
                    &l__BOOLEAN__IsDVDROMRead,
                    &l__BOOLEAN__IsDVDRRead,
                    &l__BOOLEAN__IsDVDRAMRead,
                    &l__BOOLEAN__IsTestWrite,
                    &l__BOOLEAN__IsCDRWrite,
                    &l__BOOLEAN__IsCDEWrite,
                    &l__BOOLEAN__IsDVDRWrite,
                    &l__BOOLEAN__IsDVDRAMWrite
                    );

                Trace( "Reads: CD-R: %s, CD-RW: %s, DVD-ROM: %s, DVD-R: %s, DVD-RAM: %s",
                    ( l__BOOLEAN__IsCDRRead == TRUE ) ? "Yes" : "No",
                    ( l__BOOLEAN__IsCDERead == TRUE ) ? "Yes" : "No",
                    ( l__BOOLEAN__IsDVDROMRead == TRUE ) ? "Yes" : "No",
                    ( l__BOOLEAN__IsDVDRRead == TRUE ) ? "Yes" : "No",
                    ( l__BOOLEAN__IsDVDRAMRead == TRUE ) ? "Yes" : "No"
                    );

                Trace("Writes: Test mode: %s, CD-R: %s, CD-RW: %s, DVD-R: %s, DVD-RAM: %s",
                    ( l__BOOLEAN__IsTestWrite == TRUE ) ? "Yes" : "No",
                    ( l__BOOLEAN__IsCDRWrite == TRUE ) ? "Yes" : "No",
                    ( l__BOOLEAN__IsCDEWrite == TRUE ) ? "Yes" : "No",  
                    ( l__BOOLEAN__IsDVDRWrite == TRUE ) ? "Yes" : "No",
                    ( l__BOOLEAN__IsDVDRAMWrite == TRUE ) ? "Yes" : "No"
                    );

                //
                // Get CdvdBurnerGrabber device extended information for DVD+R(w)
                // 
                StarBurn_CdvdBurnerGrabber_GetSupportedMediaFormatsExEx(
                    l__PVOID__CdvdBurnerGrabber,
                    &l__BOOLEAN__IsDVDPLUSRWRead,
                    &l__BOOLEAN__IsDVDPLUSRRead,
                    &l__BOOLEAN__IsDVDPLUSRDLRead,
                    &l__BOOLEAN__IsDVDPLUSRWWrite,
                    &l__BOOLEAN__IsDVDPLUSRWrite,
                    &l__BOOLEAN__IsDVDPLUSRDLWrite
                    );

                Trace(
                    "Reads: DVD+RW: %s, DVD+R: %s, DVD+R DL: %s, Writes: DVD+RW: %s, DVD+R: %s, DVD+R DL: %s",
                    ( l__BOOLEAN__IsDVDPLUSRWRead == TRUE ) ? "Yes" : "No",
                    ( l__BOOLEAN__IsDVDPLUSRRead == TRUE ) ? "Yes" : "No",  
                    ( l__BOOLEAN__IsDVDPLUSRDLRead == TRUE ) ? "Yes" : "No",  
                    ( l__BOOLEAN__IsDVDPLUSRWWrite == TRUE ) ? "Yes" : "No",
                    ( l__BOOLEAN__IsDVDPLUSRWrite == TRUE ) ? "Yes" : "No",
                    ( l__BOOLEAN__IsDVDPLUSRDLWrite == TRUE ) ? "Yes" : "No"
                    );


				CVideoDiskManager& manager = CVideoDiskManager::GetInstance();

				CVideoDisk* disk=NULL;
				CScsiAddress address;
				address.a =	l__PSCSI_DEVICE_ADDRESS->m__UCHAR__PortID;
                address.b =		l__PSCSI_DEVICE_ADDRESS->m__UCHAR__BusID;
                address.c =		l__PSCSI_DEVICE_ADDRESS->m__UCHAR__TargetID;
                address.d =		l__PSCSI_DEVICE_ADDRESS->m__UCHAR__LUN;

					//
				// Prepare device name
				//
				ZeroMemory(
					&l__CHAR__DeviceName,
					sizeof( l__CHAR__DeviceName )
					);

				//
				// Try to get device name by device address
				//
				l__EXCEPTION_NUMBER =
					StarBurn_GetDeviceNameByDeviceAddress(
                        l__PSCSI_DEVICE_ADDRESS->m__UCHAR__PortID,
                        l__PSCSI_DEVICE_ADDRESS->m__UCHAR__BusID,
                        l__PSCSI_DEVICE_ADDRESS->m__UCHAR__TargetID,
                        l__PSCSI_DEVICE_ADDRESS->m__UCHAR__LUN,
						( PCHAR )( &l__CHAR__DeviceName )
						);

				//
				// Check for success
				//
				if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
				{
                    Error("StarBurn_GetDeviceNameByDeviceAddress() failed, exception %d ( 0x%x )",
                        l__EXCEPTION_NUMBER,
                        l__EXCEPTION_NUMBER
                        );
    
                    //
                    // Get out of here
                    //
                    return;
				}

                Trace("Device symbolic link name: '%s'",
					l__CHAR__DeviceName
					);

				for (int i=0;i<strlen(l__CHAR__DeviceName);i++)
				{
				
					char letter = l__CHAR__DeviceName[i];
					if ((letter >= 'A' && letter <='Z') ||
						(letter >= 'a' && letter <='z') )
					{
						the_letter[0] = letter;
						Trace("Device name %s was and letter was %s",l__CHAR__DeviceName,the_letter);
						break;
					}
				}

				Trace("Done finding device letter");
	
				std::string as(( PCHAR )( &l__CHAR__VendorID ));
				as+=" ";
				as+=( PCHAR )( &l__CHAR__ProductID );

				if (AddDeviceWhenFound==1)
				{
					if (l__BOOLEAN__IsDVDRWrite ||
						l__BOOLEAN__IsDVDPLUSRWrite ||
						l__BOOLEAN__IsDVDPLUSRDLWrite)
					{
						
						disk = new CDVDDisk(address,(char*)as.c_str(),( PCHAR )( &l__CHAR__ProductID ),the_letter);
					}
					else
					{
						disk = new CCDDisk(address,(char*)as.c_str(),( PCHAR )( &l__CHAR__ProductID ),the_letter);
					}

					if (disk)
					{
						manager.AddNewVideoDiskDevice(disk);
					}
				}


			
            }
            //__finally
            {
                //
                // Check was CdvdBurnerGrabber allocated
                //
                if ( l__PVOID__CdvdBurnerGrabber != NULL )
                {
                    //
                    // Free allocated memory
                    //
                    StarBurn_Destroy( &l__PVOID__CdvdBurnerGrabber );
                }   
            }
        }
        break;

        //
        // Unhandled callback number
        //
        default:
        {
            /*

            Trace( 
                "FindDevice:FindCallback(): Unhandled callback number 0x%08X, parameter 0x%08X\n",
                ( ULONG )( p__CALLBACK_NUMBER ),
                ( ULONG )( p__PVOID__CallbackContext )
                );

            */
        }
    }
}


bool CVideoDiskManager::RebuildAvailbleVideoDevicesList()


/*++

Routine Description:

    Main application entry called by OS loader

Arguments:

    Number of virtual arguments,
    Virtual arguments list

Return Value:

    Execution status

--*/

{
	AddDeviceWhenFound=1;

    LONG l__LONG__NumberOfCdvdDevices = 0;

	EXCEPTION_NUMBER l__EXCEPTION_NUMBER = EN_SUCCESS;

    Trace( "Probing all CD/DVD devices in the system..." );

	RemoveAllVideoDiskDevices();

	//
	// Try to initialize StarBurn
	//
	l__EXCEPTION_NUMBER = 
			StarBurn_UpStartEx(
				( PVOID )( &g__UCHAR__RegistrationKey ),
				sizeof( g__UCHAR__RegistrationKey )
				);

	//
    // Check for success
	//
    if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
	{
    	Error("StarBurn_UpStartEx() failed, exception %ld",
		    l__EXCEPTION_NUMBER
        	);

		//
        // Return bad status
		//
        return false;
	}

    //
    // Try to list all CD/DVD devices in the system
    //
    l__LONG__NumberOfCdvdDevices =
        StarBurn_FindDevice(
            SCSI_DEVICE_RO_DIRECT_ACCESS,
            FALSE,
            ( PCALLBACK )( FindCallback ),
            NULL
            );

    //
    // Check for success
    //  
    if ( l__LONG__NumberOfCdvdDevices > 0 )
    {
        Trace( "Exiting with success" );
    }
    else
    {
        Trace( "Exiting with failure" );
    }

	//
	// Uninitalize StarBurn, do not care about execution status
	//
	StarBurn_DownShut();

    //
    // Return success always...
    //
    return true;
}




void CVideoDiskManager::LogAvailbleVideoDevicesList()


/*++

Routine Description:

    Main application entry called by OS loader

Arguments:

    Number of virtual arguments,
    Virtual arguments list

Return Value:

    Execution status

--*/

{
	AddDeviceWhenFound=0;

    LONG l__LONG__NumberOfCdvdDevices = 0;

	EXCEPTION_NUMBER l__EXCEPTION_NUMBER = EN_SUCCESS;

    Trace( "Probing all CD/DVD devices in the system..." );

	//
	// Try to initialize StarBurn
	//
	l__EXCEPTION_NUMBER = 
			StarBurn_UpStartEx(
				( PVOID )( &g__UCHAR__RegistrationKey ),
				sizeof( g__UCHAR__RegistrationKey )
				);

	//
    // Check for success
	//
    if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
	{
    	Error("StarBurn_UpStartEx() failed, exception %ld", l__EXCEPTION_NUMBER );

		//
        // Return bad status
		//
        return ;
	}

    //
    // Try to list all CD/DVD devices in the system
    //
    l__LONG__NumberOfCdvdDevices =
        StarBurn_FindDevice(
            SCSI_DEVICE_RO_DIRECT_ACCESS,
            FALSE,
            ( PCALLBACK )( FindCallback ),
            NULL
            );

    //
    // Check for success
    //  
    if ( l__LONG__NumberOfCdvdDevices > 0 )
    {
        Trace( "Exiting with success" );
    }
    else
    {
        Trace( "Exiting with failure" );
    }

	//
	// Uninitalize StarBurn, do not care about execution status
	//
	StarBurn_DownShut();

    //
    // Return success always...
    //
    return;
}


void	CVideoDiskManager::RemoveAllVideoDiskDevices()
{
	for (std::list<CVideoDisk*>::iterator iter = mVideoDiskDevices.begin(); 
		 iter != mVideoDiskDevices.end(); 
		 iter++)
	{
		CVideoDisk* device = *iter;
		delete device;
	}
	mVideoDiskDevices.clear();
}

void CVideoDiskManager::AddNewVideoDiskDevice(CVideoDisk* disk)
{
	mVideoDiskDevices.push_back(disk);
}


std::list<CVideoDisk*>& CVideoDiskManager::GetVideoDiskDevices()
{
	return mVideoDiskDevices;
}

	
// 
// Own global function bodies (end)
//


}


