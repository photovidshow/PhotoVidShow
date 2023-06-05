#include "StdAfx.h"
#include ".\ddisk.h"
#include "burner.h"
#include <list>
#include "../VideoTools/UnmanagedErrors.h"
#include <list>
#include <vector>
#include <algorithm>
#include "../VideoTools/UnmanagedErrors.h"


// MY old KEY

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
namespace Burner
{

	
TSpeed m_CDStandardSpeeds[CD_STANDARD_SPEED_COUNT] = {{CD_SPEED_IN_KBPS_1X,"1.0x"},
                                                           {CD_SPEED_IN_KBPS_2X,"2.0x"},
                                                          {CD_SPEED_IN_KBPS_2P2X,"2.2x"},
                                                          {CD_SPEED_IN_KBPS_3X,"3.0x"},
                                                          {CD_SPEED_IN_KBPS_4X, "4.0x"},
                                                          {CD_SPEED_IN_KBPS_8X,"8.0x"},
                                                          {CD_SPEED_IN_KBPS_10X,"10.0x"},
														  {CD_SPEED_IN_KBPS_12X,"12.0x"}, 
                                                          {CD_SPEED_IN_KBPS_16X,"16.0x"},
                                                          {CD_SPEED_IN_KBPS_24X,"24.0x"},
														  {CD_SPEED_IN_KBPS_32X,"32.0x"},
                                                          {CD_SPEED_IN_KBPS_40X,"40.0x"},
                                                          {CD_SPEED_IN_KBPS_44X,"44.0x"},
                                                          {CD_SPEED_IN_KBPS_48X,"48.0x"},
														  {CD_SPEED_IN_KBPS_52X,"52.0x"}
																						};

TSpeed m_DVDStandardSpeeds[DVD_STANDARD_SPEED_COUNT] = {{DVD_SPEED_IN_KBPS_1X,"1.0x"},
                                                             {DVD_SPEED_IN_KBPS_2X,"2.0x"},
                                                             {DVD_SPEED_IN_KBPS_2DOT4X,"2.4x"},
                                                             {DVD_SPEED_IN_KBPS_3X,"3.0x"},
                                                             {DVD_SPEED_IN_KBPS_4X,"4.0x"},
                                                             {DVD_SPEED_IN_KBPS_5X,"5.0x"},
                                                             {DVD_SPEED_IN_KBPS_6X,"6.0x"},
                                                             {DVD_SPEED_IN_KBPS_8X,"8.0x"},
                                                             {DVD_SPEED_IN_KBPS_12X,"12.0x"},
                                                             {DVD_SPEED_IN_KBPS_16X,"16.0x"}};

CCDDisk::CCDDisk(CScsiAddress& address, char* name, char* device_name, char* drive_letter):
CVideoDisk(address,name,device_name,true,false, drive_letter)
{

}

CCDDisk::CCDDisk(CScsiAddress& address, char* name, char* device_name,bool writedCD, bool writedvd,char* drive_letter):
CVideoDisk(address,name,device_name,writedCD,writedvd,drive_letter)
{
}


extern LARGE_INTEGER g__LARGE_INTEGER__LastWrittenPercent;

extern IBurningProgressCallback* prog_callback;

VOID
__stdcall
Callback2(
    IN CALLBACK_NUMBER p__CALLBACK_NUMBER,
    IN PVOID p__PVOID__CallbackContext,
    IN PVOID p__PVOID__CallbackSpecial1,
    IN PVOID p__PVOID__CallbackSpecial2
    )

/*++

Routine Description:

    Callback passed to library objects to show progress indication (tree node item added or removed, LBA assigned, file 
    opened etc)

Arguments:

    Callback number,
    Passed callback context,
    Parameter 1,
    Parameter 2

Return Value:

    Nothing

--*/

{
    LARGE_INTEGER l__LARGE_INTEGER__FileSizeInLBs;

    LARGE_INTEGER l__LARGE_INTEGER__FileSizeInUCHARs;

    LARGE_INTEGER l__LARGE_INTEGER__LBsWritten;

    LARGE_INTEGER l__LARGE_INTEGER__CurrentPercent;

    ULONG l__ULONG__WaitTimeInMs;

    ULONG l__ULONG__BufferSizeInUCHARs=0;

    ULONG l__ULONG__BufferFreeSizeInUCHARs=0;

    ULONG l__ULONG__BufferStatusPercent=0;

	ULONG l__ULONG__TrackPaddingSizeInLBs = 0;

	ULONG l__ULONG__TrackPaddingLeftInLBs =0;

    //
    // Process depending of callback number
    //
    switch ( p__CALLBACK_NUMBER )
    {
        //
        // If this DVD media padding burn started
        //
        case CN_DVD_MEDIA_PADDING_BEGIN:
        {
			prog_callback->BurnPadStarted();
            Trace( "DVDVideoTrackAtOnceFromTree:Callback(): DVD_MEDIA_PADDING_BEGIN" );
        }
        break;

        //
        // If this DVD media padding burn completed
        //
        case CN_DVD_MEDIA_PADDING_END:
        {
            Trace( "DVDVideoTrackAtOnceFromTree:Callback(): DVD_MEDIA_PADDING_END" );
        }
        break;

        //
        // If this DVD media padding size report
        //
        case CN_DVD_MEDIA_PADDING_SIZE:
        {
            //
            // Get parameters passed with callback
            //

            l__LARGE_INTEGER__FileSizeInLBs = *( PLARGE_INTEGER )( p__PVOID__CallbackSpecial1 );

            l__ULONG__TrackPaddingSizeInLBs = *( PULONG )( p__PVOID__CallbackSpecial2 );

            Trace( 
                "DVDVideoTrackAtOnceFromTree:Callback(): Target file size in LBs %I64d, track padding size in LBs %ld",
                l__LARGE_INTEGER__FileSizeInLBs,
				l__ULONG__TrackPaddingSizeInLBs
                );

            Trace( "DVDVideoTrackAtOnceFromTree:Callback(): CN_DVD_MEDIA_PADDING_SIZE" );
        }

	    case CN_DVD_MEDIA_PADDING_WRITE_PROGRESS:
        {
            //
            // Get parameters passed with callback
            //

            l__ULONG__TrackPaddingSizeInLBs = *( PULONG )( p__PVOID__CallbackSpecial1 );

            l__ULONG__TrackPaddingLeftInLBs = *( PULONG )( p__PVOID__CallbackSpecial2 );

            Trace( 
                "TrackAtOnceFromTree:Callback(): Track padding size in LBs %ld, track padding left to write in LBs %ld",
				l__ULONG__TrackPaddingSizeInLBs,
				l__ULONG__TrackPaddingLeftInLBs
                );

			int prog_pec = 100 - (l__ULONG__TrackPaddingLeftInLBs * 100) / l__ULONG__TrackPaddingSizeInLBs; 


			prog_callback->PercentageDone(prog_pec);

            Trace( "TrackAtOnceFromTree:Callback(): CN_DVD_MEDIA_PADDING_WRITE_PROGRESS" );
        }
        break;

        //
        // If this is file analyze begin
        //
        case CN_TARGET_FILE_ANALYZE_BEGIN:
        {
            Trace( "DVDVideoTrackAtOnceFromTree:Callback(): TARGET_FILE_ANALYZE_BEGIN" );
        }
        break;

        //
        // If this is file analyze end
        //
        case CN_TARGET_FILE_ANALYZE_END:
        {
            //
            // Get parameters passed with callback
            //

            l__LARGE_INTEGER__FileSizeInUCHARs = *( PLARGE_INTEGER )( p__PVOID__CallbackSpecial1 );

            l__LARGE_INTEGER__FileSizeInLBs = *( PLARGE_INTEGER )( p__PVOID__CallbackSpecial2 );

            Trace( 
                "DVDVideoTrackAtOnceFromTree:Callback(): Target file size in UCHARs %I64d, LBs %I64d",
                l__LARGE_INTEGER__FileSizeInUCHARs,
                l__LARGE_INTEGER__FileSizeInLBs
                );

            Trace( "DVDVideoTrackAtOnceFromTree:Callback(): TARGET_FILE_ANALYZE_END" );
        }
        break;

        //
        // If this is cache full begin
        //
        case CN_WAIT_CACHE_FULL_BEGIN:
        {
            //
            // Get parameters passed with callback
            //

            l__ULONG__WaitTimeInMs = *( PULONG )( p__PVOID__CallbackSpecial1 );

            Trace( "DVDVideoTrackAtOnceFromTree:Callback(): WAIT_CACHE_FULL_BEGIN" );
            
            Trace( 
                "DVDVideoTrackAtOnceFromTree:Callback(): Waiting for cache full not more then %ld ms... ",
                l__ULONG__WaitTimeInMs
                );
        }
        break;

        //
        // If this is cache full end
        //
        case CN_WAIT_CACHE_FULL_END:
        {
            Trace( "OK!\nDVDVideoTrackAtOnceFromTree:Callback(): WAIT_CACHE_FULL_END" );
        }
        break;

        //
        // If this is synchronize cache begin
        //
        case CN_SYNCHRONIZE_CACHE_BEGIN:
        {
            Trace( "\nDVDVideoTrackAtOnceFromTree:Callback(): SYNCHRONIZE_CACHE_BEGIN" );
        }
        break;

        //
        // If this is synchronize cache end
        //
        case CN_SYNCHRONIZE_CACHE_END:
        {
            Trace( "DVDVideoTrackAtOnceFromTree:Callback(): SYNCHRONIZE_CACHE_END" );
        }
        break;

        //
        // If this is DVD+RW format begin
        //
        case CN_DVDPLUSRW_FORMAT_BEGIN:
        {
            Trace( "\nDVDVideoTrackAtOnceFromTree:Callback(): DVDPLUSRW_FORMAT_BEGIN" );
        }
        break;

        //
        // If this is DVD+RW format end
        //
        case CN_DVDPLUSRW_FORMAT_END:
        {
            Trace( "DVDVideoTrackAtOnceFromTree:Callback(): DVDPLUSRW_FORMAT_END" );
        }
        break;

        //
        // If this is DVD-RW quick format begin
        //
        case CN_DVDRW_QUICK_FORMAT_BEGIN:
        {
            Trace( "\nDVDVideoTrackAtOnceFromTree:Callback(): DVDRW_QUICK_FORMAT_BEGIN" );
        }
        break;

        //
        // If this is DVD-RW quick format end
        //
        case CN_DVDRW_QUICK_FORMAT_END:
        {
            Trace( "DVDVideoTrackAtOnceFromTree:Callback(): DVDRW_QUICK_FORMAT_END" );
        }
        break;

        //
        // If this is packet write
        //
        case CN_CDVD_WRITE_PROGRESS:
        {
            //
            // Get parameters passed with callback
            //

            l__LARGE_INTEGER__FileSizeInLBs = *( PLARGE_INTEGER )( p__PVOID__CallbackSpecial1 );

            l__LARGE_INTEGER__LBsWritten = *( PLARGE_INTEGER )( p__PVOID__CallbackSpecial2 );

            //
            // Calculate number of percent written
            //
            l__LARGE_INTEGER__CurrentPercent.QuadPart = 
                ( ( l__LARGE_INTEGER__LBsWritten.QuadPart * 100 ) / l__LARGE_INTEGER__FileSizeInLBs.QuadPart );

            //
            // Check if current calculated was already written
            //
            if ( l__LARGE_INTEGER__CurrentPercent.QuadPart == g__LARGE_INTEGER__LastWrittenPercent.QuadPart )
            {
                //
                // Do nothing...
                //
            }
            else
            {
                //
                // Update last written percent with current value
                //
                g__LARGE_INTEGER__LastWrittenPercent.QuadPart = l__LARGE_INTEGER__CurrentPercent.QuadPart;

                //
                // Print this percent
                //

				if (prog_callback!=NULL)
				{
					prog_callback->PercentageDone((int)g__LARGE_INTEGER__LastWrittenPercent.QuadPart);
           //     Trace(
             //       "%I64d%% ",
               //     g__LARGE_INTEGER__LastWrittenPercent
                 //   );
				}
            }
        }
        break;


//#ifdef __NEED_BUFFER_STATUS_CALLBACKS__


        //
        // If this is buffer status
        //
        case CN_CDVD_BUFFER_STATUS:
        {
            //
            // Get parameters passed with callback
            //

            l__ULONG__BufferSizeInUCHARs = *( PULONG )( p__PVOID__CallbackSpecial1 );

            l__ULONG__BufferFreeSizeInUCHARs = *( PULONG )( p__PVOID__CallbackSpecial2 );

            //
            // Calculate buffer full ratio
            //

			if ( l__ULONG__BufferSizeInUCHARs != 0 )
			{
	            l__ULONG__BufferStatusPercent = 
					( ( l__ULONG__BufferFreeSizeInUCHARs * 100 ) / l__ULONG__BufferSizeInUCHARs );

	            l__ULONG__BufferStatusPercent = ( 100 - l__ULONG__BufferStatusPercent );
			}
			else
			{
				l__ULONG__BufferStatusPercent = 0; // Unknown if buffer size in UCHARs is zero
			}

            //
            // Print this percent
            //

			if (prog_callback!=NULL)
			{
				prog_callback->BufferStatusCallback((int)l__ULONG__BufferStatusPercent,
													(int)l__ULONG__BufferFreeSizeInUCHARs,
													(int)l__ULONG__BufferSizeInUCHARs);
			}

            Trace(
                "[ Buffer full: %ld%%, %ld free from %ld ] ",
                l__ULONG__BufferStatusPercent,
                l__ULONG__BufferFreeSizeInUCHARs,
                l__ULONG__BufferSizeInUCHARs
                );
        }
        break;


//#endif // __NEED_BUFFER_STATUS_CALLBACKS__


        //
        // Unhandled callback number
        //
        default:
        {
            /*

            Trace( 
                "DVDVideoTrackAtOnceFromTree:Callback(): Unhandled callback number 0x%08X, parameter 0x%08X\n",
                ( ULONG )( p__CALLBACK_NUMBER ),
                ( ULONG )( p__PVOID__CallbackContext )
                );

            */
        }
    }
}
const DISC_TYPE CCDDisk::GetMediaType()
{
	return mDiscType;
}

//const double CCDDisk::GetMediaSize()
//{
//        return mMediaSize;
//}


// **********************************************************************	
// feking hell rockit division, wtf is this
/*
bool CCDDisk::GetWriteSpeeds(TSpeeds * Speeds)
{
	l__PVOID__CdvdBurnerGrabber = NULL;
	EXCEPTION_NUMBER l__EXCEPTION_NUMBER = EN_SUCCESS; // Assume success by default
	ULONG l__ULONG__Status = ERROR_GEN_FAILURE;
    CHAR l__CHAR__ExceptionText[ 1024 ];
	CDB_FAILURE_INFORMATION l__CDB_FAILURE_INFORMATION;
	
	

        STARBURN_TRACK_INFORMATION TrackInfo;
        ULONG l_CurrentReadSpeed;
        ULONG l_MaximumReadSpeed;
        ULONG l_CurrentWriteSpeed;
        ULONG l_MaximumWriteSpeed;
        TSpeed l_Speed,l_tempSpeed;

        Speeds->clear();
//Create CdvdBurnerGrabber object if need
   	l__EXCEPTION_NUMBER = 
			StarBurn_UpStartEx(
				( PVOID )( &g__UCHAR__RegistrationKey ),
				sizeof( g__UCHAR__RegistrationKey )
				);


	bool found_disk = true;

	l__EXCEPTION_NUMBER =
            StarBurn_CdvdBurnerGrabber_Create(
                &l__PVOID__CdvdBurnerGrabber,
                ( PCHAR )( &l__CHAR__ExceptionText ),
                sizeof( l__CHAR__ExceptionText ),
                &l__ULONG__Status,
                &l__CDB_FAILURE_INFORMATION,
                ( PCALLBACK )( Callback2 ),
                NULL,
				this->mScsiAddress.a,
                this->mScsiAddress.b,
                this->mScsiAddress.c,
                this->mScsiAddress.d,
                0 // == DEFAULT_CACHE_SIZE_IN_MBS
                );

        //Get current speeds in order to etimate erase time


        l__EXCEPTION_NUMBER =
            StarBurn_CdvdBurnerGrabber_GetSpeeds(
                l__PVOID__CdvdBurnerGrabber,
                ( PCHAR )( &l__CHAR__ExceptionText ),
                sizeof( l__CHAR__ExceptionText ),
                &l__ULONG__Status,
                &l__CDB_FAILURE_INFORMATION,
                &l_CurrentReadSpeed,
                &l_MaximumReadSpeed,
                &l_CurrentWriteSpeed,
                &l_MaximumWriteSpeed
                );

        // Check for success

        if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
        {
			
			Error("Failed");

            return false;
       }
       else
       {
          switch(GetMediaType())
             {
                case DISC_TYPE_CDR:
                case DISC_TYPE_CDRW:
                case DISC_TYPE_DDCDR:
                case DISC_TYPE_DDCDRW:
                {
                                l_Speed.SpeedKBps = 0;
                                l_tempSpeed.SpeedKBps = 0;
                                TSpeeds::iterator result;
                                for(int i=0; m_CDStandardSpeeds[i].SpeedKBps<=l_MaximumWriteSpeed & CD_STANDARD_SPEED_COUNT>i;i++)
                                {
                                // Try check supported speeds by setting this speed
                                        if(m_CDStandardSpeeds[i].SpeedKBps > l_Speed.SpeedKBps)
                                        {
                                                        l_tempSpeed.SpeedKBps = l_Speed.SpeedKBps;
                                                        l_Speed.SpeedKBps = m_CDStandardSpeeds[i].SpeedKBps;
                                                        // Check for supported
                                                        if (TrySetWriteSpeed(&l_Speed, l__PVOID__CdvdBurnerGrabber))
                                                        {
                                                            for(int j=1;l_tempSpeed.SpeedKBps != l_Speed.SpeedKBps && CD_STANDARD_SPEED_COUNT>j+1 ; j++)
                                                            {

                                                                switch (j)
                                                                {
                                                                        case 0:
                                                                        {
                                                                                if(l_Speed.SpeedKBps <= m_CDStandardSpeeds[j].SpeedKBps)
                                                                                {
																					    l_Speed.SpeedX = m_CDStandardSpeeds[j].SpeedX;
                                                                                        result = std::find(Speeds->begin(),Speeds->end(),l_Speed);
                                                                                        if(!result == Speeds->end())
                                                                                        {
                                                                                                Speeds->push_back(l_Speed);
                                                                                        }

                                                                                }
                                                                        }
                                                                        break;
                                                                        default:
                                                                        {
                                                                           if(l_Speed.SpeedKBps >= m_CDStandardSpeeds[j-1].SpeedKBps & l_Speed.SpeedKBps <= m_CDStandardSpeeds[j].SpeedKBps)
                                                                           {

                                                                                if((abs(m_CDStandardSpeeds[j-1].SpeedKBps - l_Speed.SpeedKBps)) <= (abs(l_Speed.SpeedKBps - m_CDStandardSpeeds[j].SpeedKBps)))
                                                                                {



                                                                                        l_Speed.SpeedX = m_CDStandardSpeeds[j-1].SpeedX;
                                                                                        result = std::find(Speeds->begin(),Speeds->end(),l_Speed);
                                                                                        if(result == Speeds->end())
                                                                                        {
                                                                                                Speeds->push_back(l_Speed);
                                                                                        }


                                                                                }
                                                                                else
                                                                                {


                                                                                        l_Speed.SpeedX = m_CDStandardSpeeds[j].SpeedX;
                                                                                        result = std::find(Speeds->begin(),Speeds->end(),l_Speed);
                                                                                        if(!(result == Speeds->end()))
                                                                                        {
                                                                                                Speeds->push_back(l_Speed);
                                                                                        }


                                                                                }
                                                                            }
                                                                        }
                                                                }
                                                           }
                                                     }
                                        }
                                }

                }
                break;
                case DISC_TYPE_DVDR:
                case DISC_TYPE_DVDRAM:
                case DISC_TYPE_DVDRWRO:
                case DISC_TYPE_DVDRWSR:
                case DISC_TYPE_DVDPLUSRW:
                case DISC_TYPE_DVDPLUSR:
                case DISC_TYPE_DVDPLUSR_DL:
                {
                                l_Speed.SpeedKBps = 0;
                                l_tempSpeed.SpeedKBps = 0;
								TSpeeds::iterator result;
                                for(int i=0; m_DVDStandardSpeeds[i].SpeedKBps<=l_MaximumWriteSpeed & DVD_STANDARD_SPEED_COUNT>i;i++)
                                {
                                // Try check supported speeds by setting this speed
                                        if(m_DVDStandardSpeeds[i].SpeedKBps > l_Speed.SpeedKBps)
                                        {
                                                        l_tempSpeed.SpeedKBps = l_Speed.SpeedKBps;
                                                        l_Speed.SpeedKBps = m_DVDStandardSpeeds[i].SpeedKBps;
                                                        // Check for supported
                                                        if (TrySetWriteSpeed(&l_Speed, l__PVOID__CdvdBurnerGrabber))
                                                        {
                                                            for(int j=1;l_tempSpeed.SpeedKBps != l_Speed.SpeedKBps && DVD_STANDARD_SPEED_COUNT>j+1 ; j++)
                                                            {

                                                                switch (j)
                                                                {
                                                                        case 0:
                                                                        {
                                                                                if(l_Speed.SpeedKBps <= m_DVDStandardSpeeds[j].SpeedKBps)
                                                                                {
																					    l_Speed.SpeedX = m_DVDStandardSpeeds[j].SpeedX;
                                                                                        result = std::find(Speeds->begin(),Speeds->end(),l_Speed);
                                                                                        if(!result == Speeds->end())
                                                                                        {
                                                                                                Speeds->push_back(l_Speed);
                                                                                        }

                                                                                }
                                                                        }
                                                                        break;
                                                                        default:
                                                                        {
                                                                           if(l_Speed.SpeedKBps >= m_DVDStandardSpeeds[j-1].SpeedKBps & l_Speed.SpeedKBps <= m_DVDStandardSpeeds[j].SpeedKBps)
                                                                           {

                                                                                if((abs(m_DVDStandardSpeeds[j-1].SpeedKBps - l_Speed.SpeedKBps)) <= (abs(l_Speed.SpeedKBps - m_DVDStandardSpeeds[j].SpeedKBps)))
                                                                                {



                                                                                        l_Speed.SpeedX = m_DVDStandardSpeeds[j-1].SpeedX;
                                                                                        result = std::find(Speeds->begin(),Speeds->end(),l_Speed);
                                                                                        if(result == Speeds->end())
                                                                                        {
                                                                                                Speeds->push_back(l_Speed);
                                                                                        }


                                                                                }
                                                                                else
                                                                                {


                                                                                        l_Speed.SpeedX = m_DVDStandardSpeeds[j].SpeedX;
                                                                                        result = std::find(Speeds->begin(),Speeds->end(),l_Speed);
                                                                                        if(!(result == Speeds->end()))
                                                                                        {
                                                                                                Speeds->push_back(l_Speed);
                                                                                        }


                                                                                }
                                                                            }
                                                                        }
                                                                }
                                                           }
                                                     }
                                        }
                                }
                }
                break;
                case DISC_TYPE_NO_MEDIA:
                {
                        for(int i=0; m_CDStandardSpeeds[i].SpeedKBps <= l_MaximumWriteSpeed && CD_STANDARD_SPEED_COUNT>i; i++)
                        {
                                Speeds->push_back(m_CDStandardSpeeds[i]);
                        }
                }
                        }

        }
        return true;
}
*/


bool CCDDisk::TrySetWriteSpeed(TSpeed* Speed, void* m_CdvdBurnerGrabber)
{
    ULONG l_CurrentReadSpeed;
    ULONG l_MaximumReadSpeed;
  //  ULONG l_CurrentWriteSpeed;
    ULONG l_MaximumWriteSpeed;

	EXCEPTION_NUMBER l__EXCEPTION_NUMBER = EN_SUCCESS; // Assume success by default
	ULONG l__ULONG__Status = ERROR_GEN_FAILURE;
    CHAR l__CHAR__ExceptionText[ 1024 ];
	CDB_FAILURE_INFORMATION l__CDB_FAILURE_INFORMATION;
	
    //Create CdvdBurnerGrabber object if need
  

   //Try to set current write speed
        l__EXCEPTION_NUMBER =
            StarBurn_CdvdBurnerGrabber_SetSpeeds(
                 m_CdvdBurnerGrabber,
                 ( PCHAR )( &l__CHAR__ExceptionText ),
                 sizeof( l__CHAR__ExceptionText ),
                 &l__ULONG__Status,
                 &l__CDB_FAILURE_INFORMATION,
                 CDVD_SPEED_IS_KBPS_MAXIMUM,
                 Speed->SpeedKBps
                );


        // Check for success
        if (l__EXCEPTION_NUMBER != EN_SUCCESS )
        {
             return false;
        }

        // Try to get current read/write speeds to show what we'll have
        l__EXCEPTION_NUMBER =
            StarBurn_CdvdBurnerGrabber_GetSpeeds(
                 m_CdvdBurnerGrabber,
                 ( PCHAR )( &l__CHAR__ExceptionText ),
                 sizeof( l__CHAR__ExceptionText ),
                 &l__ULONG__Status,
                 &l__CDB_FAILURE_INFORMATION,
                &l_CurrentReadSpeed,
                &l_MaximumReadSpeed,
                &Speed->SpeedKBps,
                &l_MaximumWriteSpeed
                );

        // Check for success
        if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
        {
            return false;
        }

        return true;
}




// DISC_TYPE_UNKNOWN                    Unknown disc type
// DISC_TYPE_CDROM,                     CD-ROM
// DISC_TYPE_CDR,                       CD-R
// DISC_TYPE_CDRW,                      CD-RW
// DISC_TYPE_DVDROM,                    DVD-ROM
// DISC_TYPE_DVDR,                      DVD-R
// DISC_TYPE_DVDRAM,                    DVD-RAM
// DISC_TYPE_DVDRWRO,                   DVD-RW RO (Restricted Overwrite)
// DISC_TYPE_DVDRWSR,                   DVD-RW SR (Sequential Recording)
// DISC_TYPE_DVDPLUSRW,                 DVD+RW
// DISC_TYPE_DDCDROM,                   DD (Double Density) CD-ROM
// DISC_TYPE_DDCDR,                     DD (Double Density) CD-R
// DISC_TYPE_DDCRW                      DD (Double Density) CD-RW
// DISC_TYPE_DVDPLUSR                   DVD+R
// DISC_TYPE_NO_MEDIA					No media is inserted to the disc drive
// DISC_TYPE_DVDPLUSR_DL				DVD+R DL (Double Layer)



/*
bool CCDDisk::GetTrackInfo(int Track,
						   STARBURN_TRACK_INFORMATION *l_TrackInformation,
						   PVOID grabber)
{

    EXCEPTION_NUMBER l__EXCEPTION_NUMBER = EN_SUCCESS; // Assume success by default
	ULONG l__ULONG__Status = ERROR_GEN_FAILURE;
    CHAR l__CHAR__ExceptionText[ 1024 ];
	CDB_FAILURE_INFORMATION l__CDB_FAILURE_INFORMATION;


	Trace("Getting track information");
        l__EXCEPTION_NUMBER =
            StarBurn_CdvdBurnerGrabber_GetTrackInformation(
                 grabber,
                 ( PCHAR )( &l__CHAR__ExceptionText ),
                 sizeof( l__CHAR__ExceptionText ),
                 &l__ULONG__Status,
                 &l__CDB_FAILURE_INFORMATION,
                 Track,
                 l_TrackInformation
                 );

        //
        // Check for success
        //

        if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
        {
			Warning("Could not find track information");

            //
            // Get out of here
            //

            return false;
        }
        return true;
}
*/


bool CCDDisk::TestUnitReady(PVOID grabber)
{
	EXCEPTION_NUMBER l__EXCEPTION_NUMBER = EN_SUCCESS; // Assume success by default
	ULONG l__ULONG__Status = ERROR_GEN_FAILURE;
    CHAR l__CHAR__ExceptionText[ 1024 ];
	CDB_FAILURE_INFORMATION l__CDB_FAILURE_INFORMATION;

	Trace("Testing unit ready");

l__ULONG__Status = ERROR_GEN_FAILURE;
l__EXCEPTION_NUMBER = EN_SUCCESS; // Assume success by default

       //Create CdvdBurnerGrabber object if need
    


    l__EXCEPTION_NUMBER =
	StarBurn_CdvdBurnerGrabber_TestUnitReady(
	grabber,
	( PCHAR )( &l__CHAR__ExceptionText ),
	sizeof( l__CHAR__ExceptionText ),
	&l__ULONG__Status,
	&l__CDB_FAILURE_INFORMATION
	);

    if (l__EXCEPTION_NUMBER != EN_SUCCESS)
{
		Trace("Unit was not Ready");
            return false;
    }
        else
        {

		Trace("Unit was Ready");
            return true;
                   }
};


/*
bool CCDDisk::RefreshMediaSize(DISC_TYPE l_MediaType, PVOID grabber)
{
    TOC_INFORMATION l_TOCInfo;
    double l_DiscSize = 0;
    double l_TrackSize = 0;

	EXCEPTION_NUMBER l__EXCEPTION_NUMBER = EN_SUCCESS; // Assume success by default
	ULONG l__ULONG__Status = ERROR_GEN_FAILURE;
    CHAR l__CHAR__ExceptionText[ 1024 ];
	CDB_FAILURE_INFORMATION l__CDB_FAILURE_INFORMATION;

	Trace("Refreshing media size");

     switch(l_MediaType)
       {

                case DISC_TYPE_DVDRWRO:
                case DISC_TYPE_DVDRWSR:
                case DISC_TYPE_DVDPLUSRW:
                {
                        STARBURN_TRACK_INFORMATION l_TrackInfo;
                        GetTrackInfo(255,&l_TrackInfo, grabber);
						mMediaSize = l_TrackInfo.m__LONG__NextWritableAddress;
                        mMediaSize *= 2048;

						char t[100];

						sprintf(t,"Found DVD +/-RW sisz was %d",mMediaSize);
						Trace(t);
                        return true;
                        break;
                }
                default:
                {
                      //Try to get TOC Info
                        l__EXCEPTION_NUMBER =
                                StarBurn_CdvdBurnerGrabber_GetTOCInformation(
                                 grabber,
                                 (char*)( &l__CHAR__ExceptionText ),
                                 sizeof( l__CHAR__ExceptionText ),
                                 &l__ULONG__Status,
                                 &l__CDB_FAILURE_INFORMATION,
                                 &l_TOCInfo);

                      // Check for success
                        if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
                        {
							Trace("No TOC information disk is blank");

                                mMediaSize = 0;
                                return false;
                         }

                        for(int i=0;i<l_TOCInfo.m__UCHAR__NumberOfTracks;i++)
                        {
                                 l_TrackSize = (l_TOCInfo.m__TOC_ENTRY[i].m__LONG__EndingLBA -  l_TOCInfo.m__TOC_ENTRY[i].m__LONG__StartingLBA);
                                 l_TrackSize = l_TrackSize*l_TOCInfo.m__TOC_ENTRY[i].m__ULONG__LBSizeInUCHARs;
                                 l_DiscSize += l_TrackSize;
                        }
					    mMediaSize = l_DiscSize;
						char t[100];
						sprintf(t,"found TOC information disk size is %l",mMediaSize);
						Trace(t);
                        return true;
                  }
      }
}
*/


bool CCDDisk::IsDiskInDriveBlank(PVOID pCdvdBurnerGrabber)
{
    EXCEPTION_NUMBER l_ExceptionNumber;
	CHAR l_ExceptionText[ 1024 ];
	ULONG l_SystemError;
	CDB_FAILURE_INFORMATION  l_CDBFailureInfo;
	BOOLEAN l_IsDiscBlank(FALSE);

	if(NULL == pCdvdBurnerGrabber )
	{         
			return false;
	}

	//
	//Try to get disc blank state
	//
	l_ExceptionNumber =
		StarBurn_CdvdBurnerGrabber_IsDiscBlank(
			pCdvdBurnerGrabber,
			(char*)( &l_ExceptionText ),
			sizeof( l_ExceptionText ),
			&l_SystemError,
			&l_CDBFailureInfo,
			&l_IsDiscBlank
			);
    // Check for success
    if ( l_ExceptionNumber != EN_SUCCESS )
	{
        Trace("Failed on call to StarBurn_CdvdBurnerGrabber_IsDiscBlank");

        /*
		Error(
			"\nCDeviceFinder::IsDiscBlank(): StarBurn_CdvdBurnerGrabber_IsDiscBlank() failed, exception %ld, status %ld, text '%s'\n",
			l_ExceptionNumber,
			l_SystemError,
			l_ExceptionText
			);   	
        */

		return false;
    }
	
	return l_IsDiscBlank ? true:false;

	
    /*
	 Trace("Checking if disk in drive is blank");

	 RefreshMediaSize(GetMediaType(), grabber);

	 STARBURN_TRACK_INFORMATION l_TrackInfo;
        switch (GetMediaType())
        {
                case DISC_TYPE_DVDRWRO:
                case DISC_TYPE_DVDRWSR:
                case DISC_TYPE_DVDPLUSRW:
                case DISC_TYPE_DVDROM:
                {
					 Trace("found a dvd rw +/- or rom");
                        if(GetMediaSize())
                        {

							Trace("Disk was not blank");
                                return false;
                        }
                        else
                        {
							
							Trace("Disk was blank");
                                return true;
                        }
                break;
                }
                case DISC_TYPE_DVDPLUSR:
                case DISC_TYPE_DVDPLUSR_DL:
                case DISC_TYPE_DVDR:
                case DISC_TYPE_CDR:
                case DISC_TYPE_CDRW:
                {
					 Trace("Found a dvd r +/- or cd r/rw");
                        if(GetTrackInfo(1,&l_TrackInfo, grabber))
                        {
                                if(l_TrackInfo.m__BOOLEAN__IsBlank)
                                {

										Trace("Disk was blank");
                                        return true;
                                }
                                else
                                {
										Trace("Disk was not blank");
                                return false;
                                }
                        }
                        else
                        {
								Trace("Could not get track info");
                                return false;
                        }
                        break;
                 }
        }

	Trace("Was not sure if it was blank as it was an unknown disk type");
  	return false;
    */

}

//
// Return values
// 0 = Could not detect disk of correct type or error when trying
// 1 = Found disk of correct type
// 2 = Found disk of crrect type but needs erasing first
// 3 = Found blank DVD disk when expected type was blu-ray
// 4 = Found DVD which needs eraing when expected type was blu-ray
//
int CCDDisk::ContainsABlankDisk(EDiskType type)
{
	l__PVOID__CdvdBurnerGrabber = NULL;
	EXCEPTION_NUMBER l__EXCEPTION_NUMBER = EN_SUCCESS; // Assume success by default
	ULONG l__ULONG__Status = ERROR_GEN_FAILURE;
    CHAR l__CHAR__ExceptionText[ 1024 ];
	CDB_FAILURE_INFORMATION l__CDB_FAILURE_INFORMATION;
	STARBURN_DISC_INFORMATION l__DISC_INFORMATION;

	Trace("Checking for blank media");

	//
	// Try to initialize StarBurn
	//
	l__EXCEPTION_NUMBER = 
			StarBurn_UpStartEx(
				( PVOID )( &g__UCHAR__RegistrationKey ),
				sizeof( g__UCHAR__RegistrationKey )
				);

	Trace("Starburn key accepted");

	int found_disk = 1;

	l__EXCEPTION_NUMBER =
            StarBurn_CdvdBurnerGrabber_Create(
                &l__PVOID__CdvdBurnerGrabber,
                ( PCHAR )( &l__CHAR__ExceptionText ),
                sizeof( l__CHAR__ExceptionText ),
                &l__ULONG__Status,
                &l__CDB_FAILURE_INFORMATION,
                ( PCALLBACK )( Callback2 ),
                NULL,
				this->mScsiAddress.a,
                this->mScsiAddress.b,
                this->mScsiAddress.c,
                this->mScsiAddress.d,
                0 // == DEFAULT_CACHE_SIZE_IN_MBS
                );

        //
        // Check for success
        //
        if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
        {
			Error("Creating grabber failed");
			found_disk = -1;
			goto end2;
        }

		Trace("Creating grabber success with Scsi %d,%d,%d,%d",this->mScsiAddress.a,this->mScsiAddress.b,this->mScsiAddress.c,this->mScsiAddress.d);
	
        //
        // CHECK INSERTED MEDIA IS OF CORRECT TYPE
        //
		bool test_unit_ready = TestUnitReady(l__PVOID__CdvdBurnerGrabber);
		if (test_unit_ready==false)
		{
//			found_disk =false;
//			goto end2;
		}

        // Try to read disc information
        l__EXCEPTION_NUMBER =
            StarBurn_CdvdBurnerGrabber_GetDiscInformation(
                l__PVOID__CdvdBurnerGrabber,
                ( PCHAR )( &l__CHAR__ExceptionText ),
                sizeof( l__CHAR__ExceptionText ),
                &l__ULONG__Status,
                &l__CDB_FAILURE_INFORMATION,
                ( PSTARBURN_DISC_INFORMATION )( &l__DISC_INFORMATION )
                );

        //
        // Check for success
        //
		bool disk_info_read=false;

		if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
        {
            Trace(
                "\n\nDVDVideoTrackAtOnceFromTree:main(): StarBurn_CdvdBurnerGrabber_GetDiscInformation() failed, exception %ld, status %ld, text '%s'\n",
                l__EXCEPTION_NUMBER,
                l__ULONG__Status,
                l__CHAR__ExceptionText
                );
        }
		else
		{
			disk_info_read=true;
		}
		

		DISC_TYPE d_type= StarBurn_CdvdBurnerGrabber_GetInsertedDiscType(l__PVOID__CdvdBurnerGrabber);

		mDiscType = d_type;

		bool correct_type = false;
        bool foundDVDWhenExpectingTypeWasBluRay = false;

        if (d_type == DISC_TYPE_BDROM ||
            d_type == DISC_TYPE_BDR ||
            d_type == DISC_TYPE_BDRE)
        {
            Trace("Found a BluRay disk");
            if (type ==BLURAY)
            {
                correct_type= true;
            }
        }

		if (d_type==DISC_TYPE_DVDR ||
			d_type==DISC_TYPE_DVDPLUSRW ||
			d_type==DISC_TYPE_DVDPLUSR ||
			d_type==DISC_TYPE_DVDPLUSR_DL ||
			d_type==DISC_TYPE_DVDRWRO ||           
			d_type==DISC_TYPE_DVDRWSR || 
			d_type==DISC_TYPE_DVDROM ||
			d_type==DISC_TYPE_UNKNOWN)
		{
			Trace("Found a DVD disk");
			if (type==DVD) 
			{
				correct_type= true;
			}
            else if (type == BLURAY)
            {
               correct_type= true;
               foundDVDWhenExpectingTypeWasBluRay = true;
               found_disk=3;
            }
		}

		if (d_type==DISC_TYPE_CDR ||
			d_type==DISC_TYPE_CDRW ||
			d_type==DISC_TYPE_DDCDR ||
			d_type==DISC_TYPE_DDCDRW ||
			d_type==DISC_TYPE_UNKNOWN)
		{
			Trace("Found a CD disk");
			if (type==CD) correct_type= true;
		}

		if (correct_type==false) 
		{
			char temp[256];
			sprintf(temp, "Could not find correct type of disk to that required found %d", mDiscType);
			Trace(temp);
			found_disk=0;
			goto end2;
		}
		else
		{
			char temp[256];
			sprintf(temp, "Disk type found was %d\n", mDiscType);
			Trace(temp);
		}


		// is disk valid (what ever that means)
	//	if (l__DISC_INFORMATION.m__BOOLEAN__IsValid=0)
	//	{
	//		found_disk=false;
	//		goto end2;
	//	}


	//	 m_MediaInfo.MediaType = StarBurn_CdvdBurnerGrabber_GetInsertedDiscType( grabber );

	
		bool is_blank = IsDiskInDriveBlank(l__PVOID__CdvdBurnerGrabber);

        //
		// Starburn sometimes reports that no blank erasable disks are blank.  Basically we don't care how many bytes have we been written, unless 
		// it is a total clear disk (i.e. zero bytes written), then it is not blank.
        //
		bool is_erasable = d_type == DISC_TYPE_DVDRWRO ||
				            d_type == DISC_TYPE_DVDRWSR ||
                            d_type == DISC_TYPE_DVDPLUSRW ||
                            d_type == DISC_TYPE_DVDROM ||
                            d_type == DISC_TYPE_BDRE ||
                            d_type == DISC_TYPE_BDROM;

		if (is_blank==true && is_erasable==true && disk_info_read==true)
		{
			if (l__DISC_INFORMATION.m__UCHAR__DiscStatus !=0)
			{
				is_blank=false;
			}
		}

		if (is_blank==false && is_erasable==true && disk_info_read==true)
		{
			if (l__DISC_INFORMATION.m__BOOLEAN__IsErasable==1)
			{
				Trace("The disc was a non blank erasable disc");
				found_disk=2;

                if (foundDVDWhenExpectingTypeWasBluRay==true)
                {
                    found_disk=4;
                }
				goto end1;
			}
		}

		if (is_blank==false) 
		{
			Trace("The disk was not blank");
			found_disk= 0;
			goto end2;
		}

        if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
        {
            Error(
                "\n\nDVDVideoTrackAtOnceFromTree:main(): StarBurn_CdvdBurnerGrabber_GetDiscInformation() failed, exception %ld, status %ld, text '%s'\n",
                l__EXCEPTION_NUMBER,
                l__ULONG__Status,
                l__CHAR__ExceptionText
                );

            //
            // Get out of here
            //

			Error("Failed on finding disc media type");
			found_disk =0;
			goto end2;

        }

end1:
		ULONG l__ULONG__CurrentReadSpeed;
		ULONG l__ULONG__MaximumReadSpeed;
		ULONG l__ULONG__CurrentWriteSpeed= 5540;
		ULONG l__ULONG__MaximumWriteSpeed = 5540;

		l__EXCEPTION_NUMBER =
            StarBurn_CdvdBurnerGrabber_GetSpeeds(
                l__PVOID__CdvdBurnerGrabber,
                ( PCHAR )( &l__CHAR__ExceptionText ),
                sizeof( l__CHAR__ExceptionText ),
                &l__ULONG__Status,
                &l__CDB_FAILURE_INFORMATION,
                &l__ULONG__CurrentReadSpeed,
                &l__ULONG__MaximumReadSpeed,
                &l__ULONG__CurrentWriteSpeed,
                &l__ULONG__MaximumWriteSpeed
                );

		mWriteSpeed = (int)l__ULONG__CurrentWriteSpeed;

		char t[100];
		sprintf(t,"Setting max write speed to %d",mWriteSpeed);
		Trace(t);

end2:
	
		if ( l__PVOID__CdvdBurnerGrabber != NULL )
        {
            //
            // Free allocated memory
            //
            StarBurn_Destroy( &l__PVOID__CdvdBurnerGrabber );
        }

		//
		// Uninitalize StarBurn, do not care about execution status
		//
		StarBurn_DownShut();
		l__PVOID__CdvdBurnerGrabber=NULL;


		sprintf(t,"Returning from Blank disk search with %d",found_disk);
		Trace(t);
	
		return found_disk;
}

//********************************************************************************************
void CCDDisk::AbortBurn()
{
	if (mBurning==false) return ;

	if (mAbortingBurn==true) return ;

	mAbortingBurn = true;
	
	if ((l__PVOID__CdvdBurnerGrabber != NULL))
	{
		StarBurn_CdvdBurnerGrabber_Cancel(l__PVOID__CdvdBurnerGrabber);
	}

}







int CCDDisk::EraseDisk()
{
	//CDDVDERASERINFO* pEraserInfo = (CDDVDERASERINFO*) pParam;
	l__PVOID__CdvdBurnerGrabber = NULL;
	EXCEPTION_NUMBER l__EXCEPTION_NUMBER = EN_SUCCESS; // Assume success by default
	CHAR ExceptionText[ 1024 ];
	ULONG SystemError;
	CDB_FAILURE_INFORMATION  CDBFailureInfo;
	EXCEPTION_NUMBER l_ExceptionNumber;
	ERASE_TYPE l_EraseType;
//	ULONG ErasingTime;

	//strUserLogMsg.LoadString(IDS_USERLOG_STARTED);
	//::SendMessage(pEraserInfo->hwnd, WM_UPDATE_USER_LOG, MSG_INFORMATION, 0);

	ZeroMemory(&ExceptionText, sizeof(ExceptionText));
	ZeroMemory(&CDBFailureInfo, sizeof(CDBFailureInfo));


	l__EXCEPTION_NUMBER = 
			StarBurn_UpStartEx(
				( PVOID )( &g__UCHAR__RegistrationKey ),
				sizeof( g__UCHAR__RegistrationKey )
				);

	//
	//Create CdvdBurnerGrabber object
	//
	l_ExceptionNumber =
	StarBurn_CdvdBurnerGrabber_Create(
		&l__PVOID__CdvdBurnerGrabber,
		(PCHAR)&ExceptionText,
		sizeof( ExceptionText ),
		&SystemError,
		&CDBFailureInfo,
		( PCALLBACK )( Callback2 ),
		NULL,
		this->mScsiAddress.a,
        this->mScsiAddress.b,
        this->mScsiAddress.c,
        this->mScsiAddress.d,
		1024*1024
	);

	//
	// Check for success
	//
	if (l_ExceptionNumber != EN_SUCCESS)
	{
		
		// error

		return 1;
	}

	//strUserLogMsg.LoadString(IDS_USERLOG_TESTUNITREADY);
	//::SendMessage(pEraserInfo->hwnd, WM_UPDATE_USER_LOG, MSG_INFORMATION, 0);

	//
	// Try to test unit ready
	//
	l_ExceptionNumber =
	StarBurn_CdvdBurnerGrabber_TestUnitReady(
		l__PVOID__CdvdBurnerGrabber,
		( PCHAR )( &ExceptionText ),
		sizeof( ExceptionText ),
		&SystemError,
		&CDBFailureInfo
	);

	//
	//Check for success
	//
	if (l_ExceptionNumber != EN_SUCCESS)
	{
		
		//error

		return 1;
	}

//	strUserLogMsg.LoadString(IDS_USERLOG_UNITREADY);
//	::SendMessage(pEraserInfo->hwnd, WM_UPDATE_USER_LOG, MSG_INFORMATION, 0);


	//
	// Try to set CD/DVD speed here
	//
	l_ExceptionNumber =
	StarBurn_CdvdBurnerGrabber_SetSpeeds(
		l__PVOID__CdvdBurnerGrabber,
		( PCHAR )( &ExceptionText ),
		sizeof( ExceptionText ),
		&SystemError,
		&CDBFailureInfo,
		CDVD_SPEED_IS_KBPS_MAXIMUM, // Set maximum supported speed
		CDVD_SPEED_IS_KBPS_MAXIMUM
		);

	//
	// Check for success
	//
	if ( l_ExceptionNumber != EN_SUCCESS )
	{
		// error
		return 1;
    }

	//strUserLogMsg.LoadString(IDS_USERLOG_SPEEDSET);
	//::SendMessage(pEraserInfo->hwnd, WM_UPDATE_USER_LOG, MSG_INFORMATION, 0);

	//if(full_erase==true)
	//{
	//	l_EraseType = ERASE_TYPE_BLANK_DISC_FULL;
	//}
	//else
	//{
		l_EraseType = ERASE_TYPE_BLANK_DISC_FAST;
//	}

//	strUserLogMsg.LoadString(IDS_USERLOG_CALCERASETIME);
//	::SendMessage(pEraserInfo->hwnd, WM_UPDATE_USER_LOG, MSG_INFORMATION, 0);

	//
	// Determine erasing time
	//
		/*
	switch(pEraserInfo->mediatype)
	{
	case DISC_TYPE_CDRW:
		if (pEraserInfo->bFull)
		{
			//
			//CD capacity + TOC capacity
			//
			ErasingTime = 
				(ULONG) (pEraserInfo->usedspace.QuadPart + pEraserInfo->freespace.QuadPart)/1024 + 
				CD_TOC_SIZE_IN_MB * 1024; 
		}
		else
		{
			//
			// ÑD TOC capacity
			//
			ErasingTime = CD_TOC_SIZE_IN_MB * 1024; 
		}
		break;
	case DISC_TYPE_DDCDRW:
	case DISC_TYPE_DVDRWRO:
	case DISC_TYPE_DVDRWSR:
	case DISC_TYPE_DVDRAM:
	case DISC_TYPE_DVDPLUSRW:
		if (pEraserInfo->bFull)
		{
			//
			// Hard code disc size for DVDs
			//
			ErasingTime = 
				(ULONG)(pEraserInfo->usedspace.QuadPart + pEraserInfo->freespace.QuadPart)/1024 + 
				DVD_TOC_SIZE_IN_MB * 1024;
		}
		else
		{
			//
			// DVD TOC capacity
			//
			ErasingTime = DVD_TOC_SIZE_IN_MB * 1024;
		}
	}
	*/


	//ErasingTime = DVD_TOC_SIZE_IN_MB * 1024;


	//ErasingTime/=pEraserInfo->speedinfo.SpeedKBps;
	//ErasingTime/=2048

/*	CString strFormat;
	strFormat.LoadString(IDS_USERLOG_WAITFORMAT);
	strUserLogMsg.Format(strFormat, ErasingTime);
	::SendMessage(pEraserInfo->hwnd, WM_UPDATE_USER_LOG, MSG_INFORMATION, 0);

	strUserLogMsg.LoadString(IDS_USERLOG_ERASING);
	::SendMessage(pEraserInfo->hwnd, WM_UPDATE_USER_LOG, MSG_INFORMATION, 0);

	::SendMessage(
		pEraserInfo->hwnd, 
		WM_START_PROGRESS, 
		(WPARAM)ceil((DOUBLE)ErasingTime/PROGRESS_UPPER_POSITION), 
		0
	);
	*/


	//
	// Try to erase the disc
	//
	l_ExceptionNumber =
	StarBurn_CdvdBurnerGrabber_Blank(
		l__PVOID__CdvdBurnerGrabber,
		( PCHAR )( &ExceptionText ),
		sizeof( ExceptionText ),
		&SystemError,
		&CDBFailureInfo,
		l_EraseType
	);

	//
	// Check for success
	//
	if (l_ExceptionNumber != EN_SUCCESS)
	{
		
		
		// blank failed

		ZeroMemory(&ExceptionText, sizeof(ExceptionText));
		ZeroMemory(&CDBFailureInfo, sizeof(CDBFailureInfo));
	}
	else
	{
		//strUserLogMsg.LoadString(IDC_USERLOG_ERASESUCCESS);
		//::SendMessage(pEraserInfo->hwnd, WM_UPDATE_USER_LOG, MSG_INFORMATION, 0);
	}

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

	//
	// Uninitalize StarBurn, do not care about execution status
	//
	StarBurn_DownShut();


	//SendNotifyMessage(pEraserInfo->hwnd, WM_ERASING_COMPLETE, 0, 0);

	return 0;
}








//********************************************************************************************
// NOTE WE BURN WITH NERO here,   CDBURNER UTILITY IS DONE AT C# level
// !!THIS IS BIN/CUE image WITH NERO ONLY!!!
// SHOULD ONLY BE A VCD OR SVCD BURNING ON A BLANK CD
int CCDDisk::Burn(char* cue_name,
	char* volume_name,
	int minute,
	int hour,
	int day,
	int month,
	int year,
	std::vector<std::string>& original_files_list,
	IBurningProgressCallback* prog_callback,
	bool do_padding,
	int test,
	bool from_bin_cue)
{
	Error("Nero no longer supported for CDs");

	return 1;
}

}
