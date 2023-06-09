/* src/config.h.in.  Generated from configure.ac by autoheader.  */


#ifndef INTTYPE_INCLIDE
#define INTTYPE_INCLUDE


#define CRASHME(A) {int*fred=0;*fred=1;}

extern void FORCE_CRASH() ;
typedef unsigned char uint8_t;
typedef unsigned char u_int8_t;
typedef unsigned short uint16_t;
typedef unsigned int uint32_t;
typedef unsigned int u_int32_t;
typedef int int32_t;
typedef unsigned __int64 uint64_t;
typedef unsigned __int64 u_int64_t;
typedef long int64_t;
typedef signed int ssize_t;



extern void DebugOutbutMessage(char *fmt, ...);

#endif


/* Define to 1 if you have the declaration of `O_BINARY', and to 0 if you
   don't. */
//#undef HAVE_DECL_O_BINARY

/* Whether FreeType is available */
#undef HAVE_FREETYPE

/* Whether FriBiDi is available */
#undef HAVE_FRIBIDI

/* Define to 1 if you have the <ft2build.h> header file. */
#undef HAVE_FT2BUILD_H

/* Define to 1 if you have the <getopt.h> header file. */
#undef HAVE_GETOPT_H

/* Define to 1 if you have the `getopt_long' function. */
#undef HAVE_GETOPT_LONG

/* Define if you have the iconv() function. */
#undef HAVE_ICONV

/* Define to 1 if you have the <inttypes.h> header file. */
#undef HAVE_INTTYPES_H

/* Define to 1 if you have the <io.h> header file. */
#undef HAVE_IO_H

/* Define if you have <langinfo.h> and nl_langinfo(CODESET). */
#undef HAVE_LANGINFO_CODESET

/* Define to 1 if you have the `dvdread' library (-ldvdread). */
#undef HAVE_LIBDVDREAD

/* Define to 1 if you have the `gnugetopt' library (-lgnugetopt). */
#undef HAVE_LIBGNUGETOPT

/* Whether the ImageMagick or GraphicsMagick libraries are available */
#undef HAVE_MAGICK

/* Define to 1 if you have the <memory.h> header file. */
//#undef HAVE_MEMORY_H

/* Define to 1 if you have the `setmode' function. */
//#undef HAVE_SETMODE

/* Define to 1 if you have the <stdint.h> header file. */
//#undef HAVE_STDINT_H

/* Define to 1 if you have the <stdlib.h> header file. */
//#define HAVE_STDLIB_H 1

/* Define to 1 if you have the <strings.h> header file. */
//#undef HAVE_STRINGS_H

/* Define to 1 if you have the <string.h> header file. */
//#undef HAVE_STRING_H

/* Define to 1 if you have the `strsep' function. */
//#undef HAVE_STRSEP

/* Define to 1 if you have the <sys/stat.h> header file. */
//#undef HAVE_SYS_STAT_H

/* Define to 1 if you have the <sys/types.h> header file. */
//#undef HAVE_SYS_TYPES_H

/* Define to 1 if you have the <unistd.h> header file. */
//#undef HAVE_UNISTD_H

/* Define as const if the declaration of iconv() needs const. */
//#undef ICONV_CONST

/* Define to the address where bug reports for this package should be sent. */
//#undef PACKAGE_BUGREPORT

/* Define to the full name of this package. */
//#undef PACKAGE_NAME

/* Define to the full name and version of this package. */
//#undef PACKAGE_STRING

/* Define to the one symbol short name of this package. */
//#undef PACKAGE_TARNAME

/* Define to the version of this package. */
#undef PACKAGE_VERSION

/* Define to 1 if you have the ANSI C header files. */
//#undef STDC_HEADERS

/* Number of bits in a file offset, on hosts where this is settable. */
//#undef _FILE_OFFSET_BITS

/* Define for large files, on AIX-style hosts. */
//#undef _LARGE_FILES
