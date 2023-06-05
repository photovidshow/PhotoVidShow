#include "StdAfx.h"
#include ".\dvddisk.h"
#include <list>
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

CDVDDisk::CDVDDisk(CScsiAddress& address, char* name,char* device_name, char* drive_letter) :
	CCDDisk(address, name,device_name,true,true, drive_letter)
{
	mAbortingBurn=false;
	mBurning=false;
	m_CdvdBurnerGrabber = NULL;
	m_DeviceStatus = NoAction;
	l__PVOID__CdvdBurnerGrabber=NULL;
	m_IsSuspectedBluRayDrive = false;

	//
	// If the devive name starts with 'BD' it is very likely this is a blu-ray device.
	// Unless there is a BD disk in the drive, there is no way to detect this is a Blu-ray
	// drive in StartBurn.  Also just because this is a BD drive does not mean it has
	// write BD write capability as also no way to detect in StartBurn.
	// 
	int deviceNameLength = strlen(device_name);
	if (deviceNameLength > 2)
	{
		if (((device_name[0] == 'B') || (device_name[0] == 'b')) &&
			((device_name[1] == 'D') || (device_name[1] == 'd')))
		{
			m_IsSuspectedBluRayDrive = true;
		}

	}
}

CDVDDisk::~CDVDDisk(void)
{
}


LARGE_INTEGER g__LARGE_INTEGER__LastWrittenPercent;

unsigned char g__UCHAR__IsIfoPatch = 0x00; // 0x01; // Assume IFO patch present by default


unsigned char g__UCHAR__Buffer[ UDF_LOGICAL_BLOCK_SIZE_IN_UCHARS ];


LONG g__LONG__TreeNodes = 0;



IBurningProgressCallback* prog_callback=0;


extern VOID __stdcall Callback2(
    IN CALLBACK_NUMBER p__CALLBACK_NUMBER,
    IN PVOID p__PVOID__CallbackContext,
    IN PVOID p__PVOID__CallbackSpecial1,
    IN PVOID p__PVOID__CallbackSpecial2
    );


bool CDVDDisk::TrySetWriteSpeed(TSpeed* Speed)
{
  
   //Try to set current write speed
        m_ExceptionNumber =
            StarBurn_CdvdBurnerGrabber_SetSpeeds(
                 m_CdvdBurnerGrabber,
                 ( PCHAR )( &m_ExceptionText ),
                 sizeof( m_ExceptionText ),
                 &m_SystemError,
                 &m_CDBFailureInfo,
                 CDVD_SPEED_IS_KBPS_MAXIMUM,
                 CDVD_SPEED_IS_KBPS_MAXIMUM
                );

      
        return true;
}

bool CDVDDisk::CdvdBurnerGrabberCreate(PCALLBACK FCallback)
{

	char t[256];
	sprintf(t, "Creating grabber scsi address = %d,%d,%d,%d", this->mScsiAddress.a,this->mScsiAddress.b,this->mScsiAddress.c,this->mScsiAddress.d);
	Trace(t);

    m_ExceptionNumber = EN_SUCCESS; // Assume success by default
    m_SystemError = ERROR_GEN_FAILURE;

    m_ExceptionNumber =
                    StarBurn_CdvdBurnerGrabber_Create(
                        &m_CdvdBurnerGrabber,
                        ( PCHAR )(&m_ExceptionText),
                        sizeof( m_ExceptionText ),
                        &m_SystemError,
                        &m_CDBFailureInfo,
                        ( PCALLBACK )( FCallback ),
							NULL,
						this->mScsiAddress.a,
						this->mScsiAddress.b,
						this->mScsiAddress.c,
						this->mScsiAddress.d,
                        1
                        );
     if ( m_ExceptionNumber != EN_SUCCESS )
                {
                    Error("StarBurn_CdvdBurnerGrabber_Create() failed, exception %ld, status %ld, text '%s'",
                        m_ExceptionNumber,
                        m_SystemError,
                        m_ExceptionText
                        );

                    return false;
                }
    return true;
}




//*****************************************************************************************
int CDVDDisk::StartUpStarburn()
{
    Trace("Checking starburn key");
    //
    m_ExceptionNumber = 
	    StarBurn_UpStartEx(
		    ( PVOID )( &g__UCHAR__RegistrationKey ),
		    sizeof( g__UCHAR__RegistrationKey )
		    );


    //
    // Check for success
    //
    if ( m_ExceptionNumber != EN_SUCCESS )
    {
	    Error("Starburn key failed");
        Error("StarBurn_UpStart() failed, exception %ld", m_ExceptionNumber );
	    return 30;
    }
    return 0;
}

//*****************************************************************************************
int CDVDDisk::SetDeviceReadyForBurn()
{
    TBurnOptions BurnOptions1;
	BurnOptions1.Speed.SpeedX=0;
	TBurnOptions* BurnOptions;
    bool l_IsBUPEnabled;
    bool l_IsBUPSupported;
	BurnOptions = & BurnOptions1;
	BurnOptions1.OPC=TRUE;
    bool l_IsSendOPCSuccessful = FALSE;
    bool l_IsTrackAtOnce = FALSE;
    bool l_IsSessionAtOnce = FALSE;
    bool l_IsDiscAtOncePQ = FALSE;
    bool l_IsDiscAtOnceRawPW = FALSE;

     //Create CdvdBurnerGrabber object if need
    if(!CdvdBurnerGrabberCreate(&Callback2))
    {
        Error("CdvdBurnerGrabberCreate failed");
        return -1;
    }

    Trace("Testing unit ready...");
    // Try to test unit ready
    if(!TestUnitReady(m_CdvdBurnerGrabber))
    {
		Error("Burning device not read");
        return 121;
    }
    else
    {
       Trace("Unit is ready!");
    }

	//Probing supported write modes...
    Trace("Probing supported write modes...");
    m_ExceptionNumber =
        StarBurn_CdvdBurnerGrabber_ProbeSupportedWriteModes(
             m_CdvdBurnerGrabber,
             ( PCHAR )( &m_ExceptionText ),
             sizeof( m_ExceptionText ),
             &m_SystemError,
             &m_CDBFailureInfo,
             (PBOOLEAN)&l_IsTrackAtOnce,
             (PBOOLEAN)&l_IsSessionAtOnce,
             (PBOOLEAN)&l_IsDiscAtOncePQ,
             (PBOOLEAN)&l_IsDiscAtOnceRawPW
             );

    if ( m_ExceptionNumber != EN_SUCCESS )
    {
		Error("Failed when probing supported write modes...");
        Error("StarBurn_CdvdBurnerGrabber_ProbeSupportedWriteModes() failed, exception %ld, status %ld, text '%s'",
            m_ExceptionNumber,
            m_SystemError,
            m_ExceptionText
            );

            return 119;
    }

    Trace("Track-At-Once: %s, Session-At-Once: %s, Disc-At-Once PQ: %s, Disc-At-Once raw P-W: %s",
        l_IsTrackAtOnce ? "Yes" : "No",
        l_IsSessionAtOnce ? "Yes" : "No",
        l_IsDiscAtOncePQ ? "Yes" : "No",
        l_IsDiscAtOnceRawPW ? "Yes" : "No"
        );

    // Check do we have Track-At-Once supported
    if ( !l_IsTrackAtOnce )
    {
        Error("Track-At-Once is unsupported" );
        return 120;
    }

    Trace("Track-At-Once is supported");

 
    // Try to get BUP here
    m_ExceptionNumber =
        StarBurn_CdvdBurnerGrabber_GetBUP(
             m_CdvdBurnerGrabber,
             ( PCHAR )( &m_ExceptionText ),
             sizeof( m_ExceptionText ),
             &m_SystemError,
             &m_CDBFailureInfo,
            (PBOOLEAN)&l_IsBUPEnabled,
            (PBOOLEAN)&l_IsBUPSupported
            );

    // Check for success

    if ( m_ExceptionNumber != EN_SUCCESS )
    {
        Error("StarBurn_CdvdBurnerGrabber_GetBUP() failed, exception %ld, status %ld, text '%s'",
            m_ExceptionNumber,
            m_SystemError,
            m_ExceptionText
            );
        return 122;
    }

    Trace("Getting BUP successfully");

    Trace("BUP (Buffer Underrun Protection) Enabled: %s, Supported: %s",
        l_IsBUPEnabled ? "Yes" : "No",
        l_IsBUPSupported ? "Yes" : "No"
        );

    // Check if the BUP supported try to enable it (for now we'll try to enable it ALWAYS)

    if (  true )  //l__BOOLEAN__IsBUPSupported
    {
        Trace("Enabling BUP (Buffer Underrun Protection)...");

        // Try to set BUP status

        m_ExceptionNumber =
            StarBurn_CdvdBurnerGrabber_SetBUP(
             m_CdvdBurnerGrabber,
             ( PCHAR )( &m_ExceptionText ),
             sizeof( m_ExceptionText ),
             &m_SystemError,
             &m_CDBFailureInfo,
             true
                );

        // Check for success

        if ( m_ExceptionNumber != EN_SUCCESS )
        {
            Warning("StarBurn_CdvdBurnerGrabber_SetBUP() failed");
        }
        else
		{
			Trace("Enabling BUP successfully");
        }
    }

    Trace("Setting speed %s (%ld KBps)... ",BurnOptions->Speed.SpeedX,BurnOptions->Speed.SpeedKBps);

    //Try setting burn speed
    if (TrySetWriteSpeed(&BurnOptions->Speed))
    {
        Trace("Setting speed %s (%ld KBps) successfully",BurnOptions->Speed.SpeedX, BurnOptions->Speed.SpeedKBps);
    }
    else
    {
        Error("Setting speed failed");
    }

    // Check is this test burn, if yes we do not need to send OPC as some recorders do not like sending OPC in
    // simulation mode
    if ( BurnOptions->OPC)
    {
        Trace("Sending OPC (Optimum Power Calibration)..." );

        // Set flag OPC was successful, it will be reset in case of exception

        l_IsSendOPCSuccessful = TRUE;

        // Try to send OPC
        m_ExceptionNumber =
            StarBurn_CdvdBurnerGrabber_SendOPC(
             m_CdvdBurnerGrabber,
             ( PCHAR )( &m_ExceptionText ),
             sizeof( m_ExceptionText ),
             &m_SystemError,
             &m_CDBFailureInfo
                );

        // Check for success
        if ( m_ExceptionNumber != EN_SUCCESS )
        {
            Error("Sending OPC Error" );

            // Reset the flag OPC was successful

            l_IsSendOPCSuccessful = FALSE;

        }

        // Check was OPC successful
        if ( l_IsSendOPCSuccessful == TRUE )
        {
            Trace("Writing the stuff to the CD/DVD disc >>>" );
            Trace("Sending OPC successfully." );
        }
        else
        {
            Trace( "Writing the stuff to the CD/DVD disc >>>" );
        }
    }
    else
    {
        Trace("Skipping send OPC (Optimum Power Calibration) in test mode... OK" );
        Trace("Writing the stuff (simulating) to the CD/DVD disc" );
    }
    return 0;

}

//********************************************************************************************
int CDVDDisk::CheckForSuccess(bool* error)
{
    // Check for success
    if (Canceling != m_DeviceStatus)
    {
        m_DeviceStatus = NoAction;

        if ( m_ExceptionNumber != EN_SUCCESS )
        {
            Error("StarBurn_CdvdBurnerGrabber_TrackAtOnceFromTree() failed, exception %ld, status %ld, text '%s'",
                        m_ExceptionNumber,
                        m_SystemError,
                        m_ExceptionText
                        );
           
            Error("Burning Error" );
            return 128;
        }
        else
        {
            Trace("Try to close session by StarBurn_CdvdBurnerGrabber_CloseSession()" );
            Trace("Closing session...");

            m_ExceptionNumber =
                    StarBurn_CdvdBurnerGrabber_CloseSession(
                            m_CdvdBurnerGrabber,
                            (char*)( &m_ExceptionText ),
                            sizeof( m_ExceptionText ),
                            &m_SystemError,
                            &m_CDBFailureInfo
                                );

            // Check for success
            if ( m_ExceptionNumber != EN_SUCCESS )
            {
    			
                Error("StarBurn_CdvdBurnerGrabber_CloseSession() failed, exception %ld, status %ld, text '%s'",
                      m_ExceptionNumber,
                      m_SystemError,
                      m_ExceptionText);

          	    return 129;
            }
            else
            {
                Trace( "StarBurn_CdvdBurnerGrabber_CloseSession(): Close session success" );
            }

			g__LARGE_INTEGER__LastWrittenPercent.QuadPart = 100;
			Trace("Burning complete" );

			*error=false;		
			Trace("Ejecting disk");

			m_ExceptionNumber =
				StarBurn_CdvdBurnerGrabber_Eject(
				m_CdvdBurnerGrabber,
				(char*)( &m_ExceptionText ),
				sizeof( m_ExceptionText ),
				&m_SystemError,
				&m_CDBFailureInfo);
        }
    }
    else
    {
        m_DeviceStatus = NoAction;
        *error=false;
    }
    return 0;
}

//********************************************************************************************
void CDVDDisk::CleanUp()
{ 
	 Trace("Cleaning up after burn");
	// clean up

    // Check was burner grabber allocated
    //
    if ( m_CdvdBurnerGrabber != NULL )
    {
        //
        // Free allocated memory
        //
        StarBurn_Destroy( &m_CdvdBurnerGrabber );
    }

	//
	// Uninitalize StarBurn, do not care about execution status
	//
	StarBurn_DownShut();

	Trace("Finished burning video dvd");
}



//********************************************************************************************
int CDVDDisk::BurnUsingDVDVideo(char* FilePath, 
								 char* volume_name,
								 int minute,
								 int hour,
								 int day,
								 int month,
								 int year,
								 IBurningProgressCallback* pg, 
								 bool do_padding,
								 int test_burn)

{
	
	Trace("Started video burn");
    char FilePath2[512];
	sprintf(FilePath2, "%s\\VIDEO_TS",FilePath);
    void* l_DVDVideo = NULL; // Initialize with bad pointer
    void* l_ISO9660JolietFileTree = NULL; // Initialize with bad pointer

    int error_num=0;
	prog_callback = pg;

	bool error=false;
    LARGE_INTEGER l_FileSizeInUCHARs;

    Trace("Started burn, path was %s", FilePath);

	error_num = StartUpStarburn();
    if (error_num!=0)
    {
        error=true;
        goto end;
    }

    // Try to create DVD-Video file system
    m_ExceptionNumber =
        StarBurn_DVDVideo_Create(
			&l_DVDVideo,
			FilePath2,
			TRUE, // FALSE, // TRUE, // TRUE if we want IFO/BUP to be patched and FALSE otherwise
                             ( PCHAR )( &m_ExceptionText ),
                             sizeof( m_ExceptionText ),
                             &m_SystemError,
                            volume_name,
			"Squidgy Soft",
			"PhotoVidShow",
			year,	        // Year
			month,		// Month
			day,		// Day,
			hour,		// Hour
			minute,		// Minute
			0,		// Second
			50		// Millisecond
                            );
	// Check for success
    if ( m_ExceptionNumber != EN_SUCCESS )
    {
        Error("Creating DVD-Video file system failed: %ld", m_ExceptionNumber);

       	error=true;
		error_num=31;
		goto end;
    }

    error_num = SetDeviceReadyForBurn();
    if (error_num!=0)
    {
        if (error_num==-1)
        {
            return 0;
        }
        error=true;
        goto end;
    }

    // Get size of image we'll store (assume this call cannot fail)
    StarBurn_DVDVideo_GetSizeInUCHARs(l_DVDVideo, &l_FileSizeInUCHARs);
    Trace("Burning %I64d UCHARs of DVD-Video image", l_FileSizeInUCHARs);
 
    Trace("Creating DVD-Video file system success");

    // Reset last written percent to zero
	g__LARGE_INTEGER__LastWrittenPercent.QuadPart = 0;

    m_DeviceStatus = Burning;

	// Get pointer to ISO9660/Joliet file tree (assume this call cannot fail)

	StarBurn_DVDVideo_GetTreePointer(l_DVDVideo,&l_ISO9660JolietFileTree);
	
    //
    // Try to write the generated DVD-Video file system to the optical media
	//
	// ATTENTION! In our sample we'll pass TRUE as "multisession" to make burning process faster (when multisession is
	// disabled and compatible mode is ON we'll burn at least 1GB and it's too long for testing purpose). In a real world
	// application to make resulting DVD-Video disc readable with hardware DVD players you'll need to set "multisession"
	// parameter to FALSE just ALWAYS
    //
	BOOL do_multi = FALSE;
	if (do_padding==false)
	{
		do_multi=TRUE;
	}

	Trace("Started track at once from tree");
    m_ExceptionNumber =
            StarBurn_CdvdBurnerGrabber_TrackAtOnceFromTree(
                m_CdvdBurnerGrabber,
                (char*)( &m_ExceptionText ),
                sizeof( m_ExceptionText ),
                &m_SystemError,
                &m_CDBFailureInfo,
                l_ISO9660JolietFileTree,
                TRUE, // FALSE
                false,
				do_multi,
                WRITE_REPORT_DELAY_IN_SECONDS,
                BUFFER_STATUS_REPORT_DELAY_IN_SECONDS
                );

    error_num = CheckForSuccess(&error);
    if (error_num!=0)
    {
        error=true;
        goto end;
    }

end:

    CleanUp();

	if (error==true) 
	{
		if (error_num==0) return 1;
		return error_num ;
	}

	return 0;
}



//********************************************************************************************
int CDVDDisk::BurnImage(char* isoFile, bool doPadding, IBurningProgressCallback* pg)
{
    int error_num=0;
	prog_callback = pg;
	bool error=false;

    Trace("Started burn, iso was %s", isoFile);
	error_num = StartUpStarburn();
    if (error_num!=0)
    {
        error=true;
        goto end;
    }

    error_num = SetDeviceReadyForBurn();
    if (error_num!=0)
    {
        if (error_num==-1)
        {
            return 0;
        }
        error=true;
        goto end;
    }

    // Reset last written percent to zero
	g__LARGE_INTEGER__LastWrittenPercent.QuadPart = 0;
    m_DeviceStatus = Burning;

    BOOL MULTI_SESSION=FALSE;
    if (doPadding == false)
    {
        MULTI_SESSION = TRUE;
    }

    m_ExceptionNumber =
		StarBurn_CdvdBurnerGrabber_TrackAtOnceFromFile(
			m_CdvdBurnerGrabber,
		    (char*)( &m_ExceptionText ),
            sizeof( m_ExceptionText ),
		    &m_SystemError,
            &m_CDBFailureInfo,
			isoFile,
			FALSE,											// No CD-ROM XA
			false,		
			MULTI_SESSION, 									// padding off/on
			WRITE_REPORT_DELAY_IN_SECONDS,
			BUFFER_STATUS_REPORT_DELAY_IN_SECONDS
			);

    error_num = CheckForSuccess(&error);
    if (error_num!=0)
    {
        error=true;
        goto end;
    }

end:

    CleanUp();
	if (error==true) 
	{
		if (error_num==0) return 1;
		return error_num ;
	}
	return 0;
}


//********************************************************************************************
int CDVDDisk::Burn(char* folder_or_file,
					char* volume_name,
					int minute,
					int hour,
					int day,
					int month,
					int year,
					std::vector<std::string>& original_files_list,
					IBurningProgressCallback* pg, 
					bool do_padding,
					int test_burn,
					bool from_bin_cue_file)
{
	int error_num=0;
    LARGE_INTEGER l__LARGE_INTEGER__FileSizeInUCHARs;

 
    ULONG l__ULONG__Status = ERROR_GEN_FAILURE;

    CHAR l__CHAR__ExceptionText[ 1024 ];

    EXCEPTION_NUMBER l__EXCEPTION_NUMBER = EN_SUCCESS; // Assume success by default

    CDB_FAILURE_INFORMATION l__CDB_FAILURE_INFORMATION;

    STARBURN_TRACK_INFORMATION l__TRACK_INFORMATION;

    STARBURN_DISC_INFORMATION l__DISC_INFORMATION;

    CHAR l__CHAR__VendorID[ 1024 ];

    CHAR l__CHAR__ProductID[ 1024 ];

    CHAR l__CHAR__ProductRevisionLevel[ 1024 ];

    ULONG l__ULONG__BufferSizeInUCHARs = 0;

    BOOLEAN l__BOOLEAN__IsBUPEnabled;

    BOOLEAN l__BOOLEAN__IsBUPSupported;

    ULONG l__ULONG__CurrentReadSpeed;

    ULONG l__ULONG__MaximumReadSpeed;

    ULONG l__ULONG__CurrentWriteSpeed;

    ULONG l__ULONG__MaximumWriteSpeed;

    BOOLEAN l__BOOLEAN__IsSendOPCSuccessful = FALSE;

    DISC_TYPE l__DISC_TYPE = DISC_TYPE_UNKNOWN;

    BOOLEAN l__BOOLEAN__IsTrackAtOnce = FALSE;

    BOOLEAN l__BOOLEAN__IsSessionAtOnce = FALSE;

    BOOLEAN l__BOOLEAN__IsDiscAtOncePQ = FALSE;

    BOOLEAN l__BOOLEAN__IsDiscAtOnceRawPW = FALSE;

    UDF_TREE_ITEM l__UDF_TREE_ITEM__Directory[ 20 ]; // Directory[ 0 ] is not used

   // UDF_TREE_ITEM* l__UDF_TREE_ITEM__File = new UDF_TREE_ITEM[ 200 ]; // File[ 0 ] is not used

    ULONG l__ULONG__GUID = 0; // Start assigning GUIDs with zero

    UDF_CONTROL_BLOCK l__UDF_CONTROL_BLOCK;

    //
    // Is this an ISO file?
    //
    size_t filelength = strlen(folder_or_file);

    if (filelength > 4)
    {
        if ((folder_or_file[filelength-1] == 'o' || folder_or_file[filelength-1] == 'O') &&
            (folder_or_file[filelength-2] == 's' || folder_or_file[filelength-2] == 'S') &&
            (folder_or_file[filelength-3] == 'i' || folder_or_file[filelength-3] == 'I') &&
            (folder_or_file[filelength-4] == '.' ))
        {
            return BurnImage(folder_or_file, do_padding, pg);
        }
    }

	// if a bin/cue file then must bd a SVCD or VCD
	if (from_bin_cue_file==true)
	{
		return CCDDisk::Burn(folder_or_file,
					volume_name,
					 minute,
					 hour,
					 day,
					 month,
					 year,
					original_files_list,
					pg, 
					do_padding,
					test_burn,
					from_bin_cue_file);
	}


	// if we do not need to add extra files use the newer method for authoring video dvd's
	// there's probably nothing wrong with this method but starburn can be a bit odd, so 
	// the newer method is supposed to be the best
	if (original_files_list.size()==0)
	{
		Trace("Burning dvd video via new method");
		return	BurnUsingDVDVideo(folder_or_file,volume_name,minute,hour,day,month,year, pg, do_padding, test_burn);
	}

    // SRG OLD CODE

	Trace("Burning dvd video via old method");
	Trace("Burning with original contents");

	std::list<UDF_TREE_ITEM*> file_items;

	bool error = false;
	
	mAbortingBurn=false;
	mBurning=true;
	l__PVOID__CdvdBurnerGrabber=NULL;


    //
    // Check are there enough input parameters
    //


    //
    // Prepare some memory buffers
    //

	prog_callback = pg;
    memset(
        &l__UDF_TREE_ITEM__Directory[ 0 ],
        0,
        sizeof( l__UDF_TREE_ITEM__Directory )
        );

	


    memset(
        &l__UDF_CONTROL_BLOCK,
        0,
        sizeof( UDF_CONTROL_BLOCK )
        );

    //
    // Prepare the buffer for exception text
    //
    RtlZeroMemory(
        &l__CHAR__ExceptionText,
        sizeof( l__CHAR__ExceptionText )
        );

    //
    // Start processing cleanup
    //
  //  __try
    {
        Trace( "Formatting directories..." );

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
            Error(
                " StarBurn_UpStart() failed, exception %ld",
                l__EXCEPTION_NUMBER
                );

            //
            // Get out of here
            //
			error =true;
			error_num = 11;
			goto end;

        }
		
        //
        // Format Directory[ 1 ] as root
        //
        StarBurn_UDF_FormatTreeItemAsDirectory(
            &l__UDF_TREE_ITEM__Directory[ 1 ],
            ( ++l__ULONG__GUID ),
            "",
            NULL
            );

        //
        // Format VIDEO_TS directory
        //
        StarBurn_UDF_FormatTreeItemAsDirectory(
            &l__UDF_TREE_ITEM__Directory[ 2 ],
            ( ++l__ULONG__GUID ),
            "VIDEO_TS",
            &l__UDF_TREE_ITEM__Directory[ 1 ]
            );

        //
        // Format AUDIO_TS directory
        //

	
        StarBurn_UDF_FormatTreeItemAsDirectory(
            &l__UDF_TREE_ITEM__Directory[ 3 ],
            ( ++l__ULONG__GUID ),
            "AUDIO_TS",
            &l__UDF_TREE_ITEM__Directory[ 1 ]
            );

			

		if (original_files_list.size()!=0)
		{
			
		//	StarBurn_UDF_FormatTreeItemAsDirectory(
         //   &l__UDF_TREE_ITEM__Directory[ 4 ],
           // ( ++l__ULONG__GUID ),
          //  "MediaData",
          //  &l__UDF_TREE_ITEM__Directory[ 2 ]
          //  );
	//		


			for (std::vector<std::string>::iterator iter = original_files_list.begin(); iter != original_files_list.end(); iter++)
			{
				std::string& itemTemp = *iter;
	
				char* name = (char*)itemTemp.c_str();
				size_t len = strlen(name);
				size_t end_path_i=0;
				for (size_t i=len-1;i>=0;i--)
				{
					if ( name[i] == '\\')
					{
					   end_path_i = i+1;
					   break;
					}
				}

				char* dest = new char[len+1];
				strcpy(dest, name+end_path_i);
	 
				UDF_TREE_ITEM* itemof = new UDF_TREE_ITEM;
				memset(itemof,0,sizeof(UDF_TREE_ITEM));
				file_items.push_back(itemof);
						
				l__ULONG__Status =
					StarBurn_UDF_FormatTreeItemAsFile(
						itemof,
						( ++l__ULONG__GUID ),
						(char*)dest,
						(char*)itemTemp.c_str(),
						&l__UDF_TREE_ITEM__Directory[ 2 ]
						);
				if ( l__ULONG__Status != 0 )
				{
					Error("Something went wrong when calling StarBurn_UDF_FormatTreeItemAsFile %d", l__ULONG__Status);
				}
			}
		}

        Trace( "Formatting files..." );

		char filename[512];
		char destname[512];

        //
        // Format files (begin)
        //
		sprintf(destname,"video_ts.ifo");
		sprintf(filename,"%s\\video_ts\\%s",folder_or_file,destname);


		UDF_TREE_ITEM* item1 = new UDF_TREE_ITEM;
		memset(item1,0,sizeof(UDF_TREE_ITEM));
		file_items.push_back(item1);


        l__ULONG__Status =
            StarBurn_UDF_FormatTreeItemAsFile(
                item1,
                ( ++l__ULONG__GUID ),
                destname,
                filename,
                &l__UDF_TREE_ITEM__Directory[ 2 ]
                );

        //
        // Check for success
        //
        if ( l__ULONG__Status != 0 )
        {
            Error(
                "EXITing with failure, StarBurn_UDF_FormatTreeItemAsFile( %p, %ld, '%s', '%s', %p ) failed, status %ld ( 0x%X )",
                item1,
                l__ULONG__GUID,
                destname,
                filename,
                &l__UDF_TREE_ITEM__Directory[ 2 ],
                l__ULONG__Status,
                l__ULONG__Status
                );

            //
            // Get out of here
            //
			error =true;
			error_num = 12;
            goto end;
        }


		// ok here we do video_ts.vob 
		// although our program does not use this file other video tools do, so it would
		// be nice we could still burn other video created in other tools

		sprintf(destname,"video_ts.vob");
		sprintf(filename,"%s\\video_ts\\%s",folder_or_file,destname);

		FILE* fp2 = fopen(filename,"rb");
		if (fp2!=NULL)
		{
			Trace("Adding file %s", filename);
			fclose(fp2);

			UDF_TREE_ITEM* item11 = new UDF_TREE_ITEM;
			memset(item11,0,sizeof(UDF_TREE_ITEM));
			file_items.push_back(item11);


			l__ULONG__Status =
				StarBurn_UDF_FormatTreeItemAsFile(
					item11,
					( ++l__ULONG__GUID ),
					destname,
					filename,
					&l__UDF_TREE_ITEM__Directory[ 2 ]
					);

			//
			// Check for success
			//
			if ( l__ULONG__Status != 0 )
			{
				Error("EXITing with failure, StarBurn_UDF_FormatTreeItemAsFile( %p, %ld, '%s', '%s', %p ) failed, status %ld ( 0x%X )",
					item11,
					l__ULONG__GUID,
					destname,
					filename,
					&l__UDF_TREE_ITEM__Directory[ 2 ],
					l__ULONG__Status,
					l__ULONG__Status
					);

				//
				// Get out of here
				//
				error =true;
				error_num=13;
				goto end;
			}
		}



		UDF_TREE_ITEM* item2 = new UDF_TREE_ITEM;
		memset(item2,0,sizeof(UDF_TREE_ITEM));
		file_items.push_back(item2);


		sprintf(destname,"video_ts.bup");
		sprintf(filename,"%s\\video_ts\\%s",folder_or_file,destname);

	
        l__ULONG__Status =
            StarBurn_UDF_FormatTreeItemAsFile(
                item2,
                ( ++l__ULONG__GUID ),
                destname,
                filename,
                &l__UDF_TREE_ITEM__Directory[ 2 ]
                );

        //
        // Check for success
        //
        if ( l__ULONG__Status != 0 )
        {
            Error("EXITing with failure, StarBurn_UDF_FormatTreeItemAsFile( %p, %ld, '%s', '%s', %p ) failed, status %ld ( 0x%X )",
                item2,
                l__ULONG__GUID,
                destname,
                filename,
                &l__UDF_TREE_ITEM__Directory[ 2 ],
                l__ULONG__Status,
                l__ULONG__Status
                );

            //
            // Get out of here
            //
			error =true;
			error_num=14;
            goto end;
        }

		//////////////////////////////////////////////////////
		//////////////////////////////////////////////////////
		/////////////////////////////////////////////////////

		// start of vob inf bup files !!!!!!!!!!!
		UDF_TREE_ITEM* vts_01_ifo ;
		UDF_TREE_ITEM* vts_01_bup ;

		for (int ii=1;ii<99;ii++)
		{
	
				sprintf(destname,"vts_%02d_0.ifo",ii);
				sprintf(filename,"%s\\video_ts\\%s",folder_or_file,destname);

				FILE* fp = fopen(filename,"rb");
				if (fp!=NULL)
				{
					Trace("Adding file %s", filename);
					fclose(fp);

					UDF_TREE_ITEM* item3 = new UDF_TREE_ITEM;
					memset(item3,0,sizeof(UDF_TREE_ITEM));
					file_items.push_back(item3);


					l__ULONG__Status =
					StarBurn_UDF_FormatTreeItemAsFile(
						item3,
						( ++l__ULONG__GUID ),
						destname,
						filename,
						&l__UDF_TREE_ITEM__Directory[ 2 ]
						);

					vts_01_ifo = item3;


					//
					// Check for success
					//
					if ( l__ULONG__Status != 0 )
					{
						//
						// Get out of here
						//
						Error ("Failed when calling StarBurn_UDF_FormatTreeItemAsFile1");
						error =true;
						goto end;
					}
				}

				for (int jj=0;jj<9;jj++)
				{
					sprintf(destname,"vts_%02d_%d.vob",ii,jj);
					sprintf(filename,"%s\\video_ts\\%s",folder_or_file,destname);

					fp = fopen(filename,"rb");
					if (fp!=NULL)
					{
						fclose(fp);
						Trace("Adding file %s", filename);

						UDF_TREE_ITEM* item3 = new UDF_TREE_ITEM;
						memset(item3,0,sizeof(UDF_TREE_ITEM));
						file_items.push_back(item3);

						l__ULONG__Status =
							StarBurn_UDF_FormatTreeItemAsFile(
								item3,
								( ++l__ULONG__GUID ),
								destname,
								filename,
								&l__UDF_TREE_ITEM__Directory[ 2 ]
								);

						//
						// Check for success
						//
						if ( l__ULONG__Status != 0 )
						{
							//
							// Get out of here
							//
							Error ("Failed when calling StarBurn_UDF_FormatTreeItemAsFile2");
							error =true;
							error_num=15;
							goto end;
						}
					}
				}

				sprintf(destname,"vts_%02d_0.bup",ii);
				sprintf(filename,"%s\\video_ts\\%s",folder_or_file,destname);

				fp = fopen(filename,"rb");
				if (fp!=NULL)
				{
					fclose(fp);

					Trace("Adding file %s", filename);

					UDF_TREE_ITEM* item3 = new UDF_TREE_ITEM;
					memset(item3,0,sizeof(UDF_TREE_ITEM));
					file_items.push_back(item3);

					l__ULONG__Status =
						StarBurn_UDF_FormatTreeItemAsFile(
							item3,
							( ++l__ULONG__GUID ),
							destname,
							filename,
							&l__UDF_TREE_ITEM__Directory[ 2 ]
							);

					vts_01_bup = item3;

					//
					// Check for success
					//
					if ( l__ULONG__Status != 0 )
					{
						//
						// Get out of here
						//
						Error ("Failed when calling StarBurn_UDF_FormatTreeItemAsFile3");
						error =true;
						error_num=16;
						goto end;
					}
				
			}
		}

		if (mAbortingBurn==true)
		{
			goto end;
		}


        // Check do we need to patch IFOs here
        //

        //
        // Try to create UDF
        //
			
		// Create UDF tree with StarBurn_UDF_CreateEx() here
l__EXCEPTION_NUMBER = 
StarBurn_UDF_CreateEx(
    &l__UDF_TREE_ITEM__Directory[ 1 ], // This is ROOT
    &l__UDF_TREE_ITEM__Directory[ 2 ], // This is VIDEO_TS and not ROOT, for VIDEO_TS listing
    &l__UDF_TREE_ITEM__Directory[ 3 ], // This is AUDIO_TS and not ROOT, for AUDIO_TS listing
    &l__UDF_CONTROL_BLOCK,
    ( CHAR * )( &l__CHAR__ExceptionText ),
    sizeof( l__CHAR__ExceptionText ),
    &l__ULONG__Status,
    volume_name,
    "PublisherName",
    "ApplicationName",
    year,   // Year
    month,     // Month
    day,     // Day,
    hour,     // Hour
    minute,     // Minute
    0,      // Second
    50      // Millisecond
    );

        //
        // Check for success
        //
        if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
        {
            Error("StarBurn_UDF_Create() failed, exception %ld, status %ld, text '%s'",
                l__EXCEPTION_NUMBER,
                l__ULONG__Status,
                l__CHAR__ExceptionText
                );

            //
            // Get out of here
            //
			error =true;
			error_num=17;
            goto end;
        }


        Trace(
            "DVDVideoTrackAtOnceFromTree:main(): %ld MBs of cache will be used during burn process",
            CACHE_SIZE_IN_MBS
            );

        Trace( 
            "DVDVideoTrackAtOnceFromTree:main(): Probing SCSI address %ld:%ld:%ld:%ld for CD/DVD burner device... ",
            this->mScsiAddress.a,
            this->mScsiAddress.b,
            this->mScsiAddress.c,
            this->mScsiAddress.d
            );

        //
        // Try to construct CD/DVD burner, passing 0 as cache size will make the toolkit allocate default amount of 
        // cache memory
        //
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
            Error("StarBurn_CdvdBurnerGrabber_Create() failed, exception %ld, status %ld, text '%s'",
                l__EXCEPTION_NUMBER,
                l__ULONG__Status,
                l__CHAR__ExceptionText
                );

            //
            // Get out of here
            //
			error =true;
			error_num=18;
            goto end;
        }

        //
        // Prepare data buffers
        //

		if (mAbortingBurn==true)
		{
			goto end;
		}

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

        Trace("Found CD/DVD device '%s' - '%s' - '%s' with %ld UCHARs of cache",
            l__CHAR__VendorID,
            l__CHAR__ProductID,
            l__CHAR__ProductRevisionLevel,
            l__ULONG__BufferSizeInUCHARs
            );

        Trace("Probing supported write modes... " );

        //
        // Try to probe supported write modes
        //
        l__EXCEPTION_NUMBER =
            StarBurn_CdvdBurnerGrabber_ProbeSupportedWriteModes(
                l__PVOID__CdvdBurnerGrabber,
                ( PCHAR )( &l__CHAR__ExceptionText ),
                sizeof( l__CHAR__ExceptionText ),
                &l__ULONG__Status,
                &l__CDB_FAILURE_INFORMATION,
                &l__BOOLEAN__IsTrackAtOnce,
                &l__BOOLEAN__IsSessionAtOnce,
                &l__BOOLEAN__IsDiscAtOncePQ,
                &l__BOOLEAN__IsDiscAtOnceRawPW );

        //
        // Check for success
        //
        if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
        {
            Error("StarBurn_CdvdBurnerGrabber_ProbeSupportedWriteModes() failed, exception %ld, status %ld, text '%s'",
                l__EXCEPTION_NUMBER,
                l__ULONG__Status,
                l__CHAR__ExceptionText );

            //
            // Get out of here
            //
			error =true;
			error_num=19;
            goto end;
        }

        Trace("Track-At-Once: %s, Session-At-Once: %s, Disc-At-Once PQ: %s, Disc-At-Once raw P-W: %s",
            l__BOOLEAN__IsTrackAtOnce ? "Yes" : "No",
            l__BOOLEAN__IsSessionAtOnce ? "Yes" : "No",
            l__BOOLEAN__IsDiscAtOncePQ ? "Yes" : "No",
            l__BOOLEAN__IsDiscAtOnceRawPW ? "Yes" : "No" );

        //
        // Check do we have Track-At-Once supported
        //

		if (mAbortingBurn==true)
		{
			goto end;
		}


        if ( l__BOOLEAN__IsTrackAtOnce == FALSE )
        {
            Error( "Track-At-Once is unsupported" );

            //
            // Get out of here
            //
			error =true;
			error_num=20;
            goto end;
        }

        Trace( "Testing unit ready... " );

		/////  check for a blank disk in here !!!!

        //
        // Try to test unit ready
        //
        l__EXCEPTION_NUMBER = 
            StarBurn_CdvdBurnerGrabber_TestUnitReady(
                l__PVOID__CdvdBurnerGrabber,
                ( PCHAR )( &l__CHAR__ExceptionText ),
                sizeof( l__CHAR__ExceptionText ),
                &l__ULONG__Status,
                &l__CDB_FAILURE_INFORMATION
                );

        //
        // Check for success
        //
        if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
        {
            Error("StarBurn_CdvdBurnerGrabber_TestUnitReady() failed, exception %ld, status %ld, text '%s'",
                l__EXCEPTION_NUMBER,
                l__ULONG__Status,
                l__CHAR__ExceptionText
                );

            //
            // Get out of here
            //
			error =true;
			error_num=21;
            goto end;
        }

        Trace( "Getting BUP (Buffer Underrun Protection) support... " );

		if (mAbortingBurn==true)
		{
			goto end;
		}


        //
        // Try to get BUP here
        //
        l__EXCEPTION_NUMBER =
            StarBurn_CdvdBurnerGrabber_GetBUP( 
                l__PVOID__CdvdBurnerGrabber,
                ( PCHAR )( &l__CHAR__ExceptionText ),
                sizeof( l__CHAR__ExceptionText ),
                &l__ULONG__Status,
                &l__CDB_FAILURE_INFORMATION,
                &l__BOOLEAN__IsBUPEnabled,
                &l__BOOLEAN__IsBUPSupported
                );

        //
        // Check for success
        //
        if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
        {
            Error("StarBurn_CdvdBurnerGrabber_GetBUP() failed, exception %ld, status %ld, text '%s'",
                l__EXCEPTION_NUMBER,
                l__ULONG__Status,
                l__CHAR__ExceptionText
                );

            //
            // Get out of here
            //
			error =true;
			error_num=22;
            goto end;
        }

        Trace( "BUP (Buffer Underrun Protection) Enabled: %s, Supported: %s",
            l__BOOLEAN__IsBUPEnabled ? "Yes" : "No",
            l__BOOLEAN__IsBUPSupported ? "Yes" : "No"
            );

        //
        // Check if the BUP supported try to enable it
        //
		if (mAbortingBurn==true)
		{
			goto end;
		}


        if ( l__BOOLEAN__IsBUPSupported == TRUE )
        {
            Trace( "Enabling BUP (Buffer Underrun Protection)... " );

            //
            // Try to set BUP status
            //
            l__EXCEPTION_NUMBER = 
                StarBurn_CdvdBurnerGrabber_SetBUP( 
                    l__PVOID__CdvdBurnerGrabber,
                    ( PCHAR )( &l__CHAR__ExceptionText ),
                    sizeof( l__CHAR__ExceptionText ),
                    &l__ULONG__Status,
                    &l__CDB_FAILURE_INFORMATION,
                    TRUE
                    );

            //
            // Check for success
            //
            if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
            {
                Error( "StarBurn_CdvdBurnerGrabber_SetBUP() failed, exception %ld, status %ld, text '%s'",
                    l__EXCEPTION_NUMBER,
                    l__ULONG__Status,
                    l__CHAR__ExceptionText
                    );

                //
                // Get out of here
                //
				error =true;
				error_num=23;
                goto end;
            }
        }

        Trace( "Setting maximum supported CD/DVD speeds... " );

		if (mAbortingBurn==true)
		{
			goto end;
		}

        //
        // Try to set maximum supported CD/DVD speeds here
        //
        l__EXCEPTION_NUMBER =
            StarBurn_CdvdBurnerGrabber_SetSpeeds(
                l__PVOID__CdvdBurnerGrabber,
                ( PCHAR )( &l__CHAR__ExceptionText ),
                sizeof( l__CHAR__ExceptionText ),
                &l__ULONG__Status,
                &l__CDB_FAILURE_INFORMATION,
                CDVD_SPEED_IS_KBPS_MAXIMUM,
                CDVD_SPEED_IS_KBPS_MAXIMUM
                );

        //
        // Check for success
        //
        if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
        {
            Error("StarBurn_CdvdBurnerGrabber_SetSpeeds() failed, exception %ld, status %ld, text '%s'",
                l__EXCEPTION_NUMBER,
                l__ULONG__Status,
                l__CHAR__ExceptionText
                );

            //
            // Get out of here
            //
            //goto end;
        }

        Trace( "Getting current CD/DVD speeds... " );

        //
        // Try to get current read/write speeds to show what we'll have
        //
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

        //
        // Check for success
        //
        if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
        {
            Error("StarBurn_CdvdBurnerGrabber_GetSpeeds() failed, exception %ld, status %ld, text '%s'",
                l__EXCEPTION_NUMBER,
                l__ULONG__Status,
                l__CHAR__ExceptionText
                );

            //
            // Get out of here
            //
			error =true;
			error_num=24;
            goto end;
        }

        Trace( "Current read speed %ld KBps (%ld maximum)",
            l__ULONG__CurrentReadSpeed,
            l__ULONG__MaximumReadSpeed
            );

        Trace(
            "Current write speed %ld KBps (%ld maximum)",
            l__ULONG__CurrentWriteSpeed,
            l__ULONG__MaximumWriteSpeed
            );

        Trace( "Getting track information... " );

		if (mAbortingBurn==true)
		{
			goto end;
		}


        //
        // Try to read track information
        //
        l__EXCEPTION_NUMBER =
            StarBurn_CdvdBurnerGrabber_GetTrackInformation(
                l__PVOID__CdvdBurnerGrabber,
                ( PCHAR )( &l__CHAR__ExceptionText ),
                sizeof( l__CHAR__ExceptionText ),
                &l__ULONG__Status,
                &l__CDB_FAILURE_INFORMATION,
                TRACK_NUMBER_INVISIBLE,
                ( PSTARBURN_TRACK_INFORMATION )( &l__TRACK_INFORMATION )
                );

        //
        // Check for success
        //
        if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
        {
            Error("StarBurn_CdvdBurnerGrabber_GetTrackInformation() failed, exception %ld, status %ld, text '%s'",
                l__EXCEPTION_NUMBER,
                l__ULONG__Status,
                l__CHAR__ExceptionText
                );

            //
            // Get out of here
            //
			error =true;
			error_num=25;
            goto end;
        }

        Trace("Track blank: %s, NWA valid: %s, free LBs %ld, NWA %ld, track number %ld",
            l__TRACK_INFORMATION.m__BOOLEAN__IsBlank ? "Yes" : "No",
            l__TRACK_INFORMATION.m__BOOLEAN__IsNextWritableAddressValid ? "Yes" : "No",
            l__TRACK_INFORMATION.m__LONG__FreeLBs,
            l__TRACK_INFORMATION.m__LONG__NextWritableAddress,
            l__TRACK_INFORMATION.m__UCHAR__TrackNumber
            );

        Trace( "Getting disc information... " );

		long long tot_free_bytes = l__TRACK_INFORMATION.m__LONG__FreeLBs ;
		
		tot_free_bytes = tot_free_bytes * 2048;

		if (mAbortingBurn==true)
		{
			goto end;
		}

        //
        // Try to read disc information
        //
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
        if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
        {
            Error("StarBurn_CdvdBurnerGrabber_GetDiscInformation() failed, exception %ld, status %ld, text '%s'",
                l__EXCEPTION_NUMBER,
                l__ULONG__Status,
                l__CHAR__ExceptionText
                );

            //
            // Get out of here
            //
			error =true;
			error_num=26;
            goto end;
        }

		if (mAbortingBurn==true)
		{
			goto end;
		}


        //
        // Try to get disc type
        //
        l__DISC_TYPE = StarBurn_CdvdBurnerGrabber_GetInsertedDiscType( l__PVOID__CdvdBurnerGrabber );

        Trace("Disc erasable: %s, Disc status: 0x%02X, Last session status: 0x%02X, Disc type: %ld",
            l__DISC_INFORMATION.m__BOOLEAN__IsErasable ? "Yes" : "No",
            l__DISC_INFORMATION.m__UCHAR__DiscStatus,
            l__DISC_INFORMATION.m__UCHAR__LastSessionStatus,
            ( ULONG )( l__DISC_TYPE )
            );

        //
        // Get size of image we'll store
        //
        l__LARGE_INTEGER__FileSizeInUCHARs.LowPart = 
            StarBurn_ISO9660JolietFileTree_GetSizeInUCHARs( 
                l__UDF_CONTROL_BLOCK.m__PVOID__Body,
                &l__LARGE_INTEGER__FileSizeInUCHARs.HighPart
                );             


		long long image_size = l__LARGE_INTEGER__FileSizeInUCHARs.QuadPart;

		if (tot_free_bytes < image_size)
		{
			int id = ::MessageBox(NULL, "Error: the image is too big for the disk. Burn anyway?","Burn error",MB_OKCANCEL);
			if (id == IDCANCEL)
			{
				error = true;
				error_num=27;
				goto end;
			}
		}

        Trace("Burning %I64d UCHARs of DVD-Video image", l__LARGE_INTEGER__FileSizeInUCHARs );

        //
        // Check is this test burn, if yes we do not need to send OPC as some recorders do not like sending OPC in 
        // simulation mode
        //
		if (mAbortingBurn==true)
		{
			goto end;
		}

        if ( ( BOOLEAN )( test_burn ) == FALSE )
        {
            Trace( "Sending OPC (Optimum Power Calibration)... " );

            //
            // Set flag OPC was successful, it will be reset in case of exception
            //
            l__BOOLEAN__IsSendOPCSuccessful = TRUE;
                                                                                         
            //
            // Try to send OPC
            //
            l__EXCEPTION_NUMBER = 
                StarBurn_CdvdBurnerGrabber_SendOPC(
                    l__PVOID__CdvdBurnerGrabber,
                    ( PCHAR )( &l__CHAR__ExceptionText ),
                    sizeof( l__CHAR__ExceptionText ),
                    &l__ULONG__Status,
                    &l__CDB_FAILURE_INFORMATION
                    );

            //
            // Check for success
            //
            if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
            {
                Error( "Failed on call to StarBurn_CdvdBurnerGrabber_SendOPC" );

                //
                // Reset the flag OPC was successful
                //
                l__BOOLEAN__IsSendOPCSuccessful = FALSE;

	            //
                // Do not leave as failing to send OPC is not critical error
                //
    
                //
                // Get out of here
                //
                //goto end;
            }

            //
            // Check was OPC successful
            //
            if ( l__BOOLEAN__IsSendOPCSuccessful == TRUE )
            {
                Trace( "OPC success, writing the stuff to the CD/DVD disc >>>" );
            }
            else
            {
                Trace( "OPC Failed, writing the stuff to the CD/DVD disc >>>" );
            }
        }
        else
        {
            Trace( "Skipping send OPC (Optimum Power Calibration) in test mode..." );

            Trace( "Writing the stuff (simulating) to the CD/DVD disc >>>" );
        }

		if (mAbortingBurn==true)
		{
			goto end;
		}


        //
        // Reset last written percent to zero
        //
        g__LARGE_INTEGER__LastWrittenPercent.QuadPart = 0;

        //
        // Try to write the generated ISO image to the disc as CDROM XA (MODE2 Form1)
        //

		BOOL do_multi = FALSE;
		
		if (do_padding==false)
		{
			do_multi=TRUE;
		}

        l__EXCEPTION_NUMBER =
            StarBurn_CdvdBurnerGrabber_TrackAtOnceFromTree( 
                l__PVOID__CdvdBurnerGrabber,
                ( PCHAR )( &l__CHAR__ExceptionText ),
                sizeof( l__CHAR__ExceptionText ),
                &l__ULONG__Status,
                &l__CDB_FAILURE_INFORMATION,
                l__UDF_CONTROL_BLOCK.m__PVOID__Body,
				TRUE, // FALSE
                ( BOOLEAN )( test_burn ),
				do_multi,
                WRITE_REPORT_DELAY_IN_SECONDS,
                BUFFER_STATUS_REPORT_DELAY_IN_SECONDS
                );

        //
        // Check for success
        //
        if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
        {
            Error("StarBurn_CdvdBurnerGrabber_DVDVideoTrackAtOnceFromTree() failed, exception %ld, status %ld, text '%s'",
                l__EXCEPTION_NUMBER,
                l__ULONG__Status,
                l__CHAR__ExceptionText
                );

            //
            // Get out of here
            //
			error =true;
			error_num=28;
            goto end;
        }

        //
        // Check is this test burn, if yes we do not need to close session as some recorders do not like close session 
        // in simulation mode
        //
		if (mAbortingBurn==true)
		{
			goto end;
		}


        if ( ( BOOLEAN )( test_burn ) == FALSE )
        {
            Trace( "Closing session... " );

            //
            // Try to close the session
            //
            l__EXCEPTION_NUMBER =
                StarBurn_CdvdBurnerGrabber_CloseSession(
                    l__PVOID__CdvdBurnerGrabber,
                    ( PCHAR )( &l__CHAR__ExceptionText ),
                    sizeof( l__CHAR__ExceptionText ),
                    &l__ULONG__Status,
                    &l__CDB_FAILURE_INFORMATION
                    );

            //
            // Check for success
            //
            if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
            {
                Error("StarBurn_CdvdBurnerGrabber_CloseSession() failed, exception %ld, status %ld, text '%s'",
                    l__EXCEPTION_NUMBER,
                    l__ULONG__Status,
                    l__CHAR__ExceptionText
                    );

                //
                // Do not exit here as we need to do some cleanup in case of close session failure or the drive will remain 
                // locked, in addition failed close session not always results unreadable disc...
                //
                
                //
                // Get out of here
                //
                //goto end;
				error_num=29;
				error =true;
            }
        }
        else
        {
            Trace( "Skipping close session in test mode... " );
        }

        Trace( "Ejecting the disc... " );

		if (mAbortingBurn==true)
		{
			goto end;
		}


        //
        // Try to eject the disc
        //
		error = false;

		// who care's if it ejects or not, it's a success if it got this far

		 l__ULONG__Status = ERROR_SUCCESS;

        l__EXCEPTION_NUMBER =
            StarBurn_CdvdBurnerGrabber_Eject(
                l__PVOID__CdvdBurnerGrabber,
                ( PCHAR )( &l__CHAR__ExceptionText ),
                sizeof( l__CHAR__ExceptionText ),
                &l__ULONG__Status,
                &l__CDB_FAILURE_INFORMATION
                );

        //
        // Check for success
        //
        if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
        {
            Error( "StarBurn_CdvdBurnerGrabber_Eject() failed, exception %ld, status %ld, text '%s'",
                l__EXCEPTION_NUMBER,
                l__ULONG__Status,
                l__CHAR__ExceptionText
                );

            //
            // Get out of here
            //

            goto end;
        }

        //
        // Set status to good one
        //
        l__ULONG__Status = ERROR_SUCCESS;
    }

end:
  //  __finally 
    {
        //
        // UDF clean up here
        //
	
	
		for(std::list<UDF_TREE_ITEM*>::iterator iter = file_items.begin(); iter != file_items.end(); iter++)
		{
			 UDF_TREE_ITEM* itemTemp = *iter;

			 StarBurn_UDF_CleanUp( 
			     itemTemp,
			     1
		     );
		}

        //
        // UDF tree kill here
        //
        if ( l__UDF_CONTROL_BLOCK.m__PVOID__Body != NULL )
        {
            //
            // Destroy UDF
            //
            StarBurn_UDF_Destroy( 
                &l__UDF_TREE_ITEM__Directory[ 1 ], // Root
                &l__UDF_CONTROL_BLOCK
                );
        }

        //
        // Check was burner grabber allocated
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

		
		for(std::list<UDF_TREE_ITEM*>::iterator iter = file_items.begin(); iter != file_items.end(); iter++)
		{
			 UDF_TREE_ITEM* itemTemp = *iter;

			 delete itemTemp;
		}
		prog_callback=NULL;
    }

    //
    // Check for success
    //
    if ( l__ULONG__Status == ERROR_SUCCESS )
    {
        Trace( "Exiting with success" );
    }
    else
    {
        Trace( "Exiting with failure" );
    }

    //
    // Return execution status
    //

	mAbortingBurn=false;
	mBurning=false;
	l__PVOID__CdvdBurnerGrabber=NULL;

	if (error==true) 
	{
		if (error_num==0) return 1;
		return error_num;
	}

    return 	0;
}


bool CDVDDisk::IsSuspectedBluRayDrive()
{
	return m_IsSuspectedBluRayDrive;
}

}

