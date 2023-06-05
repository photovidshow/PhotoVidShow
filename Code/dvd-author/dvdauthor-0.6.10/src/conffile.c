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
#include <string.h>
#include <stdlib.h>

void FORCE_CRASH() {int*a=0;*a=0;}

int strcasecmp(const char *s1, const char *s2)
{
	while ((*s1 != '\0')
	&& (tolower(*(unsigned char *) s1) ==
	tolower(*(unsigned char *) s2))) {
	 s1++;
	 s2++;
	}
 
     return tolower(*(unsigned char *) s1) - tolower(*(unsigned char *) s2);
}


 int strncasecmp(const char *s1, const char *s2, unsigned int n)
 {
     if (n == 0)
         return 0;
 
     while ((n-- != 0)
            && (tolower(*(unsigned char *) s1) ==
                tolower(*(unsigned char *) s2))) {
         if (n == 0 || *s1 == '\0' || *s2 == '\0')
             return 0;
         s1++;
         s2++;
     }
 
     return tolower(*(unsigned char *) s1) - tolower(*(unsigned char *) s2);
 }




#include "compat.h"
#include "string.h"

#include "conffile.h"

static char *readconfentryfromfile(const char *s,const char *fname)
{
    FILE *h;
    static char buf[1000];

    h=fopen(fname,"r");
    if( !h ) return 0;
    while(fgets(buf,sizeof(buf),h)) {
        char *p=strchr(buf,'=');
        p[0]=0;
        if(!strcmp(buf,s)) {
            int l;

            p++;
            l=strlen(p);
            if( l>0 && p[l-1]==10 )
                p[l-1]=0;
            fclose(h);
            return strdup(p);
        }
    }
    fclose(h);
    return 0;
}

char *readconfentry(const char *s)
{
	FORCE_CRASH();

	// SRG what is this ???
/*
    char *r;
    char *hd,*hdn;

    hd=getenv("HOME");
    if( hd ) {
        hdn=malloc(strlen(hd)+30);
        sprintf(hdn,"%s/.dvdauthorrc",hd);
        r=readconfentryfromfile(s,hdn);
        free(hdn);
        if( r ) return r;
    }

	
   // r=readconfentryfromfile(s,SYSCONFDIR "/dvdauthor.conf");
    if( r ) return r;
*/
    return 0;
}
