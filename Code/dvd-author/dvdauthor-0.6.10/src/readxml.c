/*
 * Copyright (C) 2002 Scott Smith (trckjunky@users.sourceforge.net)
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or (at
 * your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
 * USA
 */

#include "config.h"

#include "compat.h"

#include <assert.h>
#include <ctype.h>
#include <errno.h>

#include "libxml/xmlreader.h"

#include "readxml.h"

#ifdef HAVE_LANGINFO_CODESET
#include <langinfo.h>
#include <locale.h>
#endif
#include <windows.h>

//static const char RCSID[]="$Id: //depot/dvdauthor/src/readxml.c#16 $";

int parser_err=0, parser_acceptbody=0;
char *parser_body=0;



typedef xmlTextReaderPtr (*xmlReaderForFile2) (const char *filename,
					 const char *encoding,
					 int options);



typedef xmlTextReaderPtr (*xmlReaderForMemory2)	(const char *buffer,
					 int size,
					 const char *URL,
					 const char *encoding,
					 int options);


typedef int (*xmlTextReaderRead2) (xmlTextReaderPtr reader);


typedef int (*xmlTextReaderNodeType2)	(xmlTextReaderPtr reader);


typedef xmlChar* (*xmlTextReaderName2)	(xmlTextReaderPtr reader);

typedef int (*xmlTextReaderIsEmptyElement2) (xmlTextReaderPtr reader);

typedef int (*xmlTextReaderDepth2)	(xmlTextReaderPtr reader);

typedef int (*xmlTextReaderMoveToNextAttribute2)(xmlTextReaderPtr reader);

typedef xmlChar * (*xmlTextReaderValue2)	(xmlTextReaderPtr reader);

typedef void (*xmlCleanupParser2)	(void);

extern int read_from_file;


int readxml(const char *my_xml_buffer,struct elemdesc *elems,struct elemattr *attrs)
{
	int length =0;
    int curstate=0,statehistory[1000];
//	char s2 = 0;
	xmlTextReaderPtr f =NULL;
	HINSTANCE LoadMe = NULL;

	xmlReaderForMemory2 xmlReaderForMemoryfunc;  
	xmlTextReaderRead2 xmlTextReaderReadfunc;
	xmlTextReaderNodeType2 xmlTextReaderNodeTypefunc;
	xmlTextReaderName2 xmlTextReaderNamefunc;
	xmlTextReaderIsEmptyElement2 xmlTextReaderIsEmptyElementfunc;
	xmlTextReaderDepth2 xmlTextReaderDepthfunc;
	xmlTextReaderMoveToNextAttribute2 xmlTextReaderMoveToNextAttributefunc;
	xmlTextReaderValue2 xmlTextReaderValuefunc;
	xmlReaderForFile2 xmlReaderForFilefunc;
	xmlCleanupParser2 xmlCleanupParserfunc;
	
	//Declare an HINSTANCE and load the library dynamically. Don’t forget 
	//to specify a correct path to the location of LoadMe.dll

	parser_err=0;
	parser_acceptbody=0;
	parser_body=0;


	if (LoadMe==NULL)
	{
		LoadMe = LoadLibrary("libxml2.dll");
	}
	//else
	//{
	//	LoadMe = LoadLibrary("c:\\dev\\dvdslideshowalpha\\libxml2.dll");
	//}
	 
	// Check to see if the library was loaded successfully 
	//if (LoadMe != 0)
	//	DebugOutbutMessage("LoadMe library loaded!\n");
	//else
	//	DebugOutbutMessage("LoadMe library failed to load!\n");

	//declare a variable of type pointer to EntryPoint function, a name of 
	// which you will later use instead of EntryPoint

	// GetProcAddress – is a function, which returns the address of the 
	// specified exported dynamic-link library (DLL) function. After 
	// performing this step you are allowed to use a variable 
	// LibMainEntryPoint as an equivalent of the function exported in 
	// LoadMe.dll. In other words, if you need to call 
	// EntryPoint(int, const char *) function, you call it as 
	// LibMainEntryPoint(int, const char *)

	xmlReaderForMemoryfunc = (xmlReaderForMemory2)GetProcAddress(LoadMe,"xmlReaderForMemory");
	xmlReaderForFilefunc = (xmlReaderForFile2)GetProcAddress(LoadMe,"xmlReaderForFile");

	xmlTextReaderReadfunc = (xmlTextReaderRead2)GetProcAddress(LoadMe,"xmlTextReaderRead");
	xmlTextReaderNodeTypefunc = (xmlTextReaderNodeType2)GetProcAddress(LoadMe,"xmlTextReaderNodeType");
	xmlTextReaderNamefunc = (xmlTextReaderName2)GetProcAddress(LoadMe,"xmlTextReaderName");
	xmlTextReaderIsEmptyElementfunc = (xmlTextReaderIsEmptyElement2)GetProcAddress(LoadMe,"xmlTextReaderIsEmptyElement");
	xmlTextReaderDepthfunc = (xmlTextReaderDepth2)GetProcAddress(LoadMe,"xmlTextReaderDepth");
	xmlTextReaderMoveToNextAttributefunc = (xmlTextReaderMoveToNextAttribute2)GetProcAddress(LoadMe,"xmlTextReaderMoveToNextAttribute");
	xmlTextReaderValuefunc = (xmlTextReaderValue2)GetProcAddress(LoadMe,"xmlTextReaderValue");
	xmlCleanupParserfunc = (xmlCleanupParser2)GetProcAddress(LoadMe,"xmlCleanupParser");

	
//	s2= malloc( strlen(my_xml_buffer)+1);
//	strcpy(s2,my_xml_buffer);

	
	
	length = strlen(my_xml_buffer);
	//DebugOutbutMessage("SRG Read  XML %d \n",length);
//	if (read_from_file==2)
//	{
//		DebugOutbutMessage("Read from file\n");
//		f = (*xmlReaderForFilefunc)("c:\\test.xml", NULL, 0);
//	}
//	else
//	{
//		DebugOutbutMessage("Read from memory\n");
		f = (*xmlReaderForMemoryfunc)(my_xml_buffer, length, NULL, NULL,0);
//	}

	//f = (*xmlReaderForMemory)(my_xml_buffer, length, NULL, NULL,0);

//	f = (*LibMainEntryPoint);

//	DebugOutbutMessage("SRG Read  XML2\n");

//	f = xmlReaderForMemory(my_xml_buffer,length,NULL,NULL,0);

//	DebugOutbutMessage("Read  XML2.01\n");


//    if( xmlfile[0]=='&' && isdigit(xmlfile[1]) )
  //      f=xmlReaderForFd(atoi(xmlfile+1),xmlfile,NULL,0);
 //   else
   //     f=xmlNewTextReaderFilename(xmlfile);
   // if(!f) {
   //     fprintf(stderr,"ERR:  Unable to open XML file %s\n",xmlfile);
    //    return 1;
  //  }

    while(1) {
        int r=(*xmlTextReaderReadfunc)(f);
        int i;

        if( !r ) {
          //  fprintf(stderr,"ERR:  Read premature EOF\n");
		//	DebugOutbutMessage("ERR:  Read premature EOF\n");
            return 1;
        }
        if( r!=1 ) {
           // fprintf(stderr,"ERR:  Error in parsing XML\n");
	//		DebugOutbutMessage("ERR:  Error in parsing XML\n");
            return 1;
        }
        switch((*xmlTextReaderNodeTypefunc)(f)) {
        case XML_READER_TYPE_SIGNIFICANT_WHITESPACE:
        case XML_READER_TYPE_WHITESPACE:
        case XML_READER_TYPE_COMMENT:
            break;

        case XML_READER_TYPE_ELEMENT:
            assert(!parser_body);
            for( i=0; elems[i].elemname; i++ )
                if( curstate==elems[i].parentstate &&
                    !strcmp((*xmlTextReaderNamefunc)(f),elems[i].elemname) ) {
                    // reading the attributes causes these values to change
                    // so if you want to use them later, save them now
                    int empty=(*xmlTextReaderIsEmptyElementfunc)(f),
                        depth=(*xmlTextReaderDepthfunc)(f);
                    if( elems[i].start ) {
                        elems[i].start();
                        if( parser_err )
                            return 1;
                    }
                    while((*xmlTextReaderMoveToNextAttributefunc)(f)) {
                        char *nm=(*xmlTextReaderNamefunc)(f),*v=(*xmlTextReaderValuefunc)(f);
                        int j;

                        for( j=0; attrs[j].elem; j++ )
                            if( !strcmp(attrs[j].elem,elems[i].elemname) &&
                                !strcmp(attrs[j].attr,nm )) {
                                attrs[j].f(v);
                                if( parser_err )
                                    return 1;
                                break;
                            }
                        if( !attrs[j].elem ) {
                          //  fprintf(stderr,"ERR:  Cannot match attribute '%s' in tag '%s'.  Valid attributes are:\n",nm,elems[i].elemname);
                            for( j=0; attrs[j].elem; j++ )
                                if( !strcmp(attrs[j].elem,elems[i].elemname) )
                                    fprintf(stderr,"      %s\n",attrs[j].attr);
                            return 1;
                        }
                    }
                    if( empty ) {
                        if( elems[i].end ) {
                            elems[i].end();
                            if( parser_err )
                                return 1;
                        }
                    } else {
                        statehistory[depth]=i;
                        curstate=elems[i].newstate;
                    }
                    break;
                }
            if( !elems[i].elemname ) {
           //     fprintf(stderr,"ERR:  Cannot match start tag '%s'.  Valid tags are:\n",xmlTextReaderName(f));
                for( i=0; elems[i].elemname; i++ )
                    if( curstate==elems[i].parentstate )
                        fprintf(stderr,"      %s\n",elems[i].elemname);
                return 1;
            }
            break;

        case XML_READER_TYPE_END_ELEMENT:
            i=statehistory[(*xmlTextReaderDepthfunc)(f)];
            if( elems[i].end ) {
                elems[i].end();
                if( parser_err )
                    return 1;
            }
            curstate=elems[i].parentstate;
            parser_body=0;
            parser_acceptbody=0;
            if( !curstate )
                goto done_parsing;
            break;

        case XML_READER_TYPE_TEXT: {
            char *v=(*xmlTextReaderValuefunc)(f);

            if( !parser_body ) {
                // stupid buggy libxml2 2.5.4 that ships with RedHat 9.0!
                // we must manually check if this is just whitespace
                for( i=0; v[i]; i++ )
                    if( v[i]!='\r' &&
                        v[i]!='\n' &&
                        v[i]!=' '  &&
                        v[i]!='\t' )
                        goto has_nonws_body;
                break;
            }
        has_nonws_body:
            if( !parser_acceptbody ) {
               // fprintf(stderr,"ERR:  text not allowed here\n");
                return 1;
            }

            if( !parser_body )
                parser_body=strdup(v);
            else {
                parser_body=realloc(parser_body,strlen(parser_body)+strlen(v)+1);
                strcat(parser_body,v);
            }
            break;
        }

        default:
          //  fprintf(stderr,"ERR:  Unknown XML node type %d\n",xmlTextReaderNodeType(f));
            return 1;
        }
    }
 done_parsing:

//	xmlTextReaderRead(f);
	(*xmlCleanupParserfunc)();

	FreeLibrary(LoadMe);

//	f = xmlReaderForFile("c:\\test.xml",fred,2);
	
    return 0;
}

int xml_ison(const char *s)
{
    if( !strcmp(s,"1") || !strcasecmp(s,"on") || !strcasecmp(s,"yes") )
        return 1;
    if( !strcmp(s,"0") || !strcasecmp(s,"off") || !strcasecmp(s,"no") )
        return 0;
    return -1;
}

#if defined(HAVE_ICONV) && defined(HAVE_LANGINFO_CODESET)

static iconv_t get_conv()
{
    static iconv_t ic=(iconv_t)-1;

    if( ic==((iconv_t)-1) ) {
        char *enc;

        errno=0;
        enc=setlocale(LC_ALL,"");
        if( enc ) {
           // fprintf(stderr,"INFO: Locale=%s\n",enc);
            if( !setlocale(LC_NUMERIC,"C") ) {
              //  fprintf(stderr,"ERR:  Cannot set numeric locale to C!\n");
                exit(1);
            }
        } else
            fprintf(stderr,"");
        enc=nl_langinfo(CODESET);
       // fprintf(stderr,"INFO: Converting filenames to %s\n",enc);
        ic=iconv_open(enc,"UTF-8");
        if( ic==((iconv_t)-1) ) {
           // fprintf(stderr,"ERR:  Cannot generate charset conversion from UTF8 to %s\n",enc);
            exit(1);
        }
    }
    return ic;
}

char *utf8tolocal(const char *in)
{
    iconv_t c=get_conv();
    int inlen=strlen(in);
    int outlen=inlen*5;
    char *r=malloc(outlen+1);
    char *out=r;
    int v;

    v=iconv(c,ICONV_CAST &in,&inlen,&out,&outlen);
    if(v==-1) {
      //  fprintf(stderr,"ERR:  Cannot convert UTF8 string '%s': %s\n",in,strerror(errno));
        exit(1);
    }
    *out=0;

    r=realloc(r,strlen(r)+1); // reduce memory requirements
    
    return r;
}
#else
char *utf8tolocal(const char *in)
{
    return strdup(in);
}
#endif
