
/*  A Bison parser, made from vm.y with Bison version GNU Bison version 1.24
  */

#define YYBISON 1  /* Identify Bison output.  */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define yyparse dvdvmparse
#define yylex dvdvmlex
#define yyerror dvdvmerror
#define yylval dvdvmlval
#define yychar dvdvmchar
#define yydebug dvdvmdebug
#define yynerrs dvdvmnerrs
#define	NUM_TOK	258
#define	G_TOK	259
#define	S_TOK	260
#define	ID_TOK	261
#define	ANGLE_TOK	262
#define	AUDIO_TOK	263
#define	BUTTON_TOK	264
#define	CALL_TOK	265
#define	CELL_TOK	266
#define	CHAPTER_TOK	267
#define	CLOSEBRACE_TOK	268
#define	CLOSEPAREN_TOK	269
#define	ELSE_TOK	270
#define	ENTRY_TOK	271
#define	EXIT_TOK	272
#define	FPC_TOK	273
#define	IF_TOK	274
#define	JUMP_TOK	275
#define	MENU_TOK	276
#define	OPENBRACE_TOK	277
#define	OPENPAREN_TOK	278
#define	PROGRAM_TOK	279
#define	PTT_TOK	280
#define	RESUME_TOK	281
#define	ROOT_TOK	282
#define	SET_TOK	283
#define	SUBTITLE_TOK	284
#define	TITLE_TOK	285
#define	TITLESET_TOK	286
#define	VMGM_TOK	287
#define	_OR_TOK	288
#define	XOR_TOK	289
#define	LOR_TOK	290
#define	BOR_TOK	291
#define	_AND_TOK	292
#define	LAND_TOK	293
#define	BAND_TOK	294
#define	NOT_TOK	295
#define	EQ_TOK	296
#define	NE_TOK	297
#define	GE_TOK	298
#define	GT_TOK	299
#define	LE_TOK	300
#define	LT_TOK	301
#define	ADD_TOK	302
#define	SUB_TOK	303
#define	MUL_TOK	304
#define	DIV_TOK	305
#define	MOD_TOK	306
#define	ADDSET_TOK	307
#define	SUBSET_TOK	308
#define	MULSET_TOK	309
#define	DIVSET_TOK	310
#define	MODSET_TOK	311
#define	ANDSET_TOK	312
#define	ORSET_TOK	313
#define	XORSET_TOK	314
#define	SEMICOLON_TOK	315
#define	ERROR_TOK	316

// 1 "vm.y"


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

#include "vm.h"

//static const char RCSID[]="$Id: //depot/dvdauthor/src/vm.y#16 $";

#define YYERROR_VERBOSE


typedef union {
    int int_val;
    char *str_val;
    struct vm_statement *statement;
} YYSTYPE;

#ifndef YYLTYPE
typedef
  struct yyltype
    {
      int timestamp;
      int first_line;
      int first_column;
      int last_line;
      int last_column;
      char *text;
   }
  yyltype;

#define YYLTYPE yyltype
#endif

#include <stdio.h>

#ifndef __cplusplus
#ifndef __STDC__
#define const
#endif
#endif



#define	YYFINAL		148
#define	YYFLAG		-32768
#define	YYNTBASE	62

#define YYTRANSLATE(x) ((unsigned)(x) <= 316 ? yytranslate[x] : 78)

static const char yytranslate[] = {     0,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
     2,     2,     2,     2,     2,     1,     2,     3,     4,     5,
     6,     7,     8,     9,    10,    11,    12,    13,    14,    15,
    16,    17,    18,    19,    20,    21,    22,    23,    24,    25,
    26,    27,    28,    29,    30,    31,    32,    33,    34,    35,
    36,    37,    38,    39,    40,    41,    42,    43,    44,    45,
    46,    47,    48,    49,    50,    51,    52,    53,    54,    55,
    56,    57,    58,    59,    60,    61
};

#if YYDEBUG != 0
static const short yyprhs[] = {     0,
     0,     2,     4,     7,     9,    11,    14,    17,    19,    23,
    25,    28,    30,    31,    34,    36,    40,    44,    48,    52,
    56,    60,    62,    65,    66,    69,    70,    76,    81,    86,
    89,    90,    97,    99,   101,   103,   105,   107,   109,   111,
   113,   117,   119,   123,   127,   131,   135,   139,   143,   147,
   151,   155,   159,   163,   167,   171,   175,   179,   183,   187,
   191,   195,   199,   203,   206,   211,   216,   221,   226,   231,
   236,   241,   246,   251,   257,   259
};

static const short yyrhs[] = {    63,
     0,    64,     0,    64,    63,     0,    68,     0,    70,     0,
    17,    60,     0,    26,    60,     0,    75,     0,    22,    63,
    13,     0,    77,     0,    31,     3,     0,    32,     0,     0,
    21,     3,     0,    21,     0,    21,    16,    30,     0,    21,
    16,    27,     0,    21,    16,    29,     0,    21,    16,     8,
     0,    21,    16,     7,     0,    21,    16,    25,     0,    18,
     0,    30,     3,     0,     0,    12,     3,     0,     0,    20,
    65,    66,    67,    60,     0,    20,    11,     3,    60,     0,
    20,    24,     3,    60,     0,    26,     3,     0,     0,    10,
    65,    66,    67,    69,    60,     0,     4,     0,     5,     0,
     8,     0,    29,     0,     7,     0,     9,     0,    71,     0,
     3,     0,    23,    73,    14,     0,    72,     0,    73,    47,
    73,     0,    73,    48,    73,     0,    73,    49,    73,     0,
    73,    50,    73,     0,    73,    51,    73,     0,    73,    39,
    73,     0,    73,    36,    73,     0,    73,    37,    73,     0,
    73,    33,    73,     0,    73,    34,    73,     0,    23,    74,
    14,     0,    73,    41,    73,     0,    73,    42,    73,     0,
    73,    43,    73,     0,    73,    44,    73,     0,    73,    45,
    73,     0,    73,    46,    73,     0,    74,    35,    74,     0,
    74,    38,    74,     0,    74,    33,    74,     0,    74,    37,
    74,     0,    40,    74,     0,    71,    28,    73,    60,     0,
    71,    52,    73,    60,     0,    71,    53,    73,    60,     0,
    71,    54,    73,    60,     0,    71,    55,    73,    60,     0,
    71,    56,    73,    60,     0,    71,    57,    73,    60,     0,
    71,    58,    73,    60,     0,    71,    59,    73,    60,     0,
    19,    23,    74,    14,    64,     0,    76,     0,    76,    15,
    64,     0
};

#endif

#if YYDEBUG != 0
static const short yyrline[] = { 0,
    90,    95,    98,   104,   107,   110,   114,   118,   121,   124,
   129,   132,   135,   140,   143,   146,   149,   152,   155,   158,
   161,   164,   167,   170,   175,   178,   183,   190,   195,   202,
   205,   210,   220,   223,   226,   229,   232,   235,   240,   243,
   248,   251,   256,   259,   262,   265,   268,   271,   274,   277,
   280,   283,   288,   291,   294,   297,   300,   303,   306,   309,
   312,   315,   318,   321,   328,   334,   337,   340,   343,   346,
   349,   352,   355,   360,   370,   373
};
regerg
static const char * const yytname[] = {   "$","error","$undefined.","NUM_TOK",
"G_TOK","S_TOK","ID_TOK","ANGLE_TOK","AUDIO_TOK","BUTTON_TOK","CALL_TOK","CELL_TOK",
"CHAPTER_TOK","CLOSEBRACE_TOK","CLOSEPAREN_TOK","ELSE_TOK","ENTRY_TOK","EXIT_TOK",
"FPC_TOK","IF_TOK","JUMP_TOK","MENU_TOK","OPENBRACE_TOK","OPENPAREN_TOK","PROGRAM_TOK",
"PTT_TOK","RESUME_TOK","ROOT_TOK","SET_TOK","SUBTITLE_TOK","TITLE_TOK","TITLESET_TOK",
"VMGM_TOK","_OR_TOK","XOR_TOK","LOR_TOK","BOR_TOK","_AND_TOK","LAND_TOK","BAND_TOK",
"NOT_TOK","EQ_TOK","NE_TOK","GE_TOK","GT_TOK","LE_TOK","LT_TOK","ADD_TOK","SUB_TOK",
"MUL_TOK","DIV_TOK","MOD_TOK","ADDSET_TOK","SUBSET_TOK","MULSET_TOK","DIVSET_TOK",
"MODSET_TOK","ANDSET_TOK","ORSET_TOK","XORSET_TOK","SEMICOLON_TOK","ERROR_TOK",
"finalparse","statements","statement","jtsl","jtml","jcl","jumpstatement","resumel",
"callstatement","reg","regornum","expression","boolexpr","setstatement","ifstatement",
"ifelsestatement",""
};
#endif

static const short yyr1[] = {     0,
    62,    63,    63,    64,    64,    64,    64,    64,    64,    64,
    65,    65,    65,    66,    66,    66,    66,    66,    66,    66,
    66,    66,    66,    66,    67,    67,    68,    68,    68,    69,
    69,    70,    71,    71,    71,    71,    71,    71,    72,    72,
    73,    73,    73,    73,    73,    73,    73,    73,    73,    73,
    73,    73,    74,    74,    74,    74,    74,    74,    74,    74,
    74,    74,    74,    74,    75,    75,    75,    75,    75,    75,
    75,    75,    75,    76,    77,    77
};

static const short yyr2[] = {     0,
     1,     1,     2,     1,     1,     2,     2,     1,     3,     1,
     2,     1,     0,     2,     1,     3,     3,     3,     3,     3,
     3,     1,     2,     0,     2,     0,     5,     4,     4,     2,
     0,     6,     1,     1,     1,     1,     1,     1,     1,     1,
     3,     1,     3,     3,     3,     3,     3,     3,     3,     3,
     3,     3,     3,     3,     3,     3,     3,     3,     3,     3,
     3,     3,     3,     2,     4,     4,     4,     4,     4,     4,
     4,     4,     4,     5,     1,     3
};

static const short yydefact[] = {     0,
    33,    34,    37,    35,    38,    13,     0,     0,    13,     0,
     0,    36,     1,     2,     4,     5,     0,     8,    75,    10,
     0,    12,    24,     6,     0,     0,     0,    24,     0,     7,
     3,     0,     0,     0,     0,     0,     0,     0,     0,     0,
     0,    11,    22,    15,     0,    26,    40,     0,     0,    39,
    42,     0,     0,     0,     0,    26,     9,     0,     0,     0,
     0,     0,     0,     0,     0,     0,     0,    76,    14,     0,
    23,     0,    31,     0,     0,    64,     0,     0,     0,     0,
     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,
     0,     0,     0,     0,     0,     0,     0,    28,    29,     0,
     0,    65,    66,    67,    68,    69,    70,    71,    72,    73,
    20,    19,    21,    17,    18,    16,    25,     0,     0,    41,
    53,    51,    52,    49,    50,    48,    54,    55,    56,    57,
    58,    59,    43,    44,    45,    46,    47,    74,    62,    60,
    63,    61,    27,    30,    32,     0,     0,     0
};

static const short yydefgoto[] = {   146,
    13,    14,    23,    46,    73,    15,   119,    16,    50,    51,
    52,    53,    18,    19,    20
};

static const short yypact[] = {   291,
-32768,-32768,-32768,-32768,-32768,   -16,   -42,     9,    51,   291,
   -37,-32768,-32768,   291,-32768,-32768,    91,-32768,    28,-32768,
    61,-32768,     4,-32768,   113,    73,    74,     4,    65,-32768,
-32768,   283,   283,   283,   283,   283,   283,   283,   283,   283,
   291,-32768,-32768,    10,    77,    67,-32768,   113,   113,-32768,
-32768,   282,    -2,    21,    24,    67,-32768,   283,   118,   125,
   147,   154,   176,   183,   205,   212,   234,-32768,-32768,   250,
-32768,    82,    60,    55,     7,-32768,   283,   283,   283,   283,
   283,   283,   283,   283,   283,   283,   283,   283,   283,   283,
   283,   283,   291,   113,   113,   113,   113,-32768,-32768,    27,
    76,-32768,-32768,-32768,-32768,-32768,-32768,-32768,-32768,-32768,
-32768,-32768,-32768,-32768,-32768,-32768,-32768,    92,    47,-32768,
-32768,   305,   305,   305,    23,    23,   300,   300,   300,   300,
   300,   300,   -12,   -12,-32768,-32768,-32768,-32768,    -8,    -8,
-32768,-32768,-32768,-32768,-32768,   108,   111,-32768
};

static const short yypgoto[] = {-32768,
    14,   -30,   105,   100,    75,-32768,-32768,-32768,     0,-32768,
   -31,   -29,-32768,-32768,-32768
};


#define	YYLAST		356


static const short yytable[] = {    17,
    59,    60,    61,    62,    63,    64,    65,    66,    67,    17,
    68,    93,    69,    17,    21,    22,    74,    24,    75,    76,
   121,    43,    30,    29,    44,    70,   101,    31,    96,    97,
    94,    25,    95,    45,    96,    97,    90,    91,    92,    94,
    17,    95,    41,    96,    97,   122,   123,   124,   125,   126,
   127,   128,   129,   130,   131,   132,   133,   134,   135,   136,
   137,    26,   138,    42,   139,   140,   141,   142,   120,    88,
    89,    90,    91,    92,    27,    54,    55,    57,    72,    71,
    98,    21,    22,    99,   117,   118,   143,    77,    78,   120,
    79,    80,    17,    81,   144,    82,    83,    84,    85,    86,
    87,    88,    89,    90,    91,    92,   145,   147,    77,    78,
   148,    79,    80,    28,    81,    47,     1,     2,    32,     3,
     4,     5,    88,    89,    90,    91,    92,    56,     0,     0,
   100,     0,     0,     0,     0,    48,     0,     0,     0,     0,
     0,    12,    33,    34,    35,    36,    37,    38,    39,    40,
    77,    78,    49,    79,    80,     0,    81,    77,    78,     0,
    79,    80,     0,    81,    88,    89,    90,    91,    92,     0,
     0,    88,    89,    90,    91,    92,     0,   102,     0,    77,
    78,     0,    79,    80,   103,    81,    77,    78,     0,    79,
    80,     0,    81,    88,    89,    90,    91,    92,     0,     0,
    88,    89,    90,    91,    92,     0,   104,     0,    77,    78,
     0,    79,    80,   105,    81,    77,    78,     0,    79,    80,
     0,    81,    88,    89,    90,    91,    92,     0,     0,    88,
    89,    90,    91,    92,     0,   106,     0,    77,    78,     0,
    79,    80,   107,    81,    77,    78,     0,    79,    80,     0,
    81,    88,    89,    90,    91,    92,   111,   112,    88,    89,
    90,    91,    92,     0,   108,     0,    77,    78,     0,    79,
    80,   109,    81,     0,   113,     0,   114,     0,   115,   116,
    88,    89,    90,    91,    92,    47,     1,     2,     0,     3,
     4,     5,     0,   110,     1,     2,     0,     3,     4,     5,
     6,     0,     0,     0,     0,    58,     0,     7,     0,     8,
     9,    12,    10,     0,    77,    78,    11,    79,    80,    12,
    81,     0,    82,    83,    84,    85,    86,    87,    88,    89,
    90,    91,    92,    78,     0,    79,     0,     0,    81,     0,
     0,    80,     0,    81,     0,     0,    88,    89,    90,    91,
    92,    88,    89,    90,    91,    92
};

static const short yycheck[] = {     0,
    32,    33,    34,    35,    36,    37,    38,    39,    40,    10,
    41,    14,     3,    14,    31,    32,    48,    60,    48,    49,
    14,    18,    60,    10,    21,    16,    58,    14,    37,    38,
    33,    23,    35,    30,    37,    38,    49,    50,    51,    33,
    41,    35,    15,    37,    38,    77,    78,    79,    80,    81,
    82,    83,    84,    85,    86,    87,    88,    89,    90,    91,
    92,    11,    93,     3,    94,    95,    96,    97,    14,    47,
    48,    49,    50,    51,    24,     3,     3,    13,    12,     3,
    60,    31,    32,    60,     3,    26,    60,    33,    34,    14,
    36,    37,    93,    39,     3,    41,    42,    43,    44,    45,
    46,    47,    48,    49,    50,    51,    60,     0,    33,    34,
     0,    36,    37,     9,    39,     3,     4,     5,    28,     7,
     8,     9,    47,    48,    49,    50,    51,    28,    -1,    -1,
    56,    -1,    -1,    -1,    -1,    23,    -1,    -1,    -1,    -1,
    -1,    29,    52,    53,    54,    55,    56,    57,    58,    59,
    33,    34,    40,    36,    37,    -1,    39,    33,    34,    -1,
    36,    37,    -1,    39,    47,    48,    49,    50,    51,    -1,
    -1,    47,    48,    49,    50,    51,    -1,    60,    -1,    33,
    34,    -1,    36,    37,    60,    39,    33,    34,    -1,    36,
    37,    -1,    39,    47,    48,    49,    50,    51,    -1,    -1,
    47,    48,    49,    50,    51,    -1,    60,    -1,    33,    34,
    -1,    36,    37,    60,    39,    33,    34,    -1,    36,    37,
    -1,    39,    47,    48,    49,    50,    51,    -1,    -1,    47,
    48,    49,    50,    51,    -1,    60,    -1,    33,    34,    -1,
    36,    37,    60,    39,    33,    34,    -1,    36,    37,    -1,
    39,    47,    48,    49,    50,    51,     7,     8,    47,    48,
    49,    50,    51,    -1,    60,    -1,    33,    34,    -1,    36,
    37,    60,    39,    -1,    25,    -1,    27,    -1,    29,    30,
    47,    48,    49,    50,    51,     3,     4,     5,    -1,     7,
     8,     9,    -1,    60,     4,     5,    -1,     7,     8,     9,
    10,    -1,    -1,    -1,    -1,    23,    -1,    17,    -1,    19,
    20,    29,    22,    -1,    33,    34,    26,    36,    37,    29,
    39,    -1,    41,    42,    43,    44,    45,    46,    47,    48,
    49,    50,    51,    34,    -1,    36,    -1,    -1,    39,    -1,
    -1,    37,    -1,    39,    -1,    -1,    47,    48,    49,    50,
    51,    47,    48,    49,    50,    51
};
/* -*-C-*-  Note some compilers choke on comments on `' lines.  */

// 3 "bison.simple"



/* Skeleton output parser for bison,

   Copyright (C) 1984, 1989, 1990 Free Software Foundation, Inc.



   This program is free software; you can redistribute it and/or modify

   it under the terms of the GNU General Public License as published by

   the Free Software Foundation; either version 2, or (at your option)

   any later version.



   This program is distributed in the hope that it will be useful,

   but WITHOUT ANY WARRANTY; without even the implied warranty of

   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the

   GNU General Public License for more details.



   You should have received a copy of the GNU General Public License

   along with this program; if not, write to the Free Software

   Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.  */



/* As a special exception, when this file is copied by Bison into a

   Bison output file, you may use that output file without restriction.

   This special exception was added by the Free Software Foundation

   in version 1.24 of Bison.  */


#include <malloc.h>
#ifndef alloca

#ifdef __GNUC__

#define alloca __builtin_alloca

#else /* not GNU C.  */

#if (!defined (__STDC__) && defined (sparc)) || defined (__sparc__) || defined (__sparc) || defined (__sgi)

#include <alloca.h>

#else /* not sparc */

#if defined (MSDOS) && !defined (__TURBOC__)

#include <malloc.h>

#else /* not MSDOS, or __TURBOC__ */

#if defined(_AIX)

#include <malloc.h>

 #pragma alloca

#else /* not MSDOS, __TURBOC__, or _AIX */

#ifdef __hpux

#ifdef __cplusplus

extern "C" {

void *alloca (unsigned int);

};

#else /* not __cplusplus */

void *alloca ();

#endif /* not __cplusplus */

#endif /* __hpux */

#endif /* not _AIX */

#endif /* not MSDOS, or __TURBOC__ */

#endif /* not sparc.  */

#endif /* not GNU C.  */

#endif /* alloca not defined.  */



/* This is the parser code that is written into each bison parser

  when the %semantic_parser declaration is not specified in the grammar.

  It was written by Richard Stallman by simplifying the hairy parser

  used when %semantic_parser is specified.  */



/* Note: there must be only one dollar sign in this file.

   It is replaced by the list of actions, each action

   as one case of the switch.  */



#define yyerrok		(yyerrstatus = 0)

#define yyclearin	(yychar = YYEMPTY)

#define YYEMPTY		-2

#define YYEOF		0

#define YYACCEPT	return(0)

#define YYABORT 	return(1)

#define YYERROR		goto yyerrlab1

/* Like YYERROR except do call yyerror.

   This remains here temporarily to ease the

   transition to the new meaning of YYERROR, for GCC.

   Once GCC version 2 has supplanted version 1, this can go.  */

#define YYFAIL		goto yyerrlab

#define YYRECOVERING()  (!!yyerrstatus)

#define YYBACKUP(token, value) \
do								\
  if (yychar == YYEMPTY && yylen == 1)				\
    { yychar = (token), yylval = (value);			\
      yychar1 = YYTRANSLATE (yychar);				\
      YYPOPSTACK;						\
      goto yybackup;						\
    }								\
  else								\
    { yyerror ("syntax error: cannot back up"); YYERROR; }	\
while (0)



#define YYTERROR	1

#define YYERRCODE	256



#ifndef YYPURE

#define YYLEX		yylex()

#endif



#ifdef YYPURE

#ifdef YYLSP_NEEDED

#ifdef YYLEX_PARAM

#define YYLEX		yylex(&yylval, &yylloc, YYLEX_PARAM)

#else

#define YYLEX		yylex(&yylval, &yylloc)

#endif

#else /* not YYLSP_NEEDED */

#ifdef YYLEX_PARAM

#define YYLEX		yylex(&yylval, YYLEX_PARAM)

#else

#define YYLEX		yylex(&yylval)

#endif

#endif /* not YYLSP_NEEDED */

#endif



/* If nonreentrant, generate the variables here */



#ifndef YYPURE



int	yychar;			/*  the lookahead symbol		*/

YYSTYPE	yylval;			/*  the semantic value of the		*/

				/*  lookahead symbol			*/



#ifdef YYLSP_NEEDED

YYLTYPE yylloc;			/*  location data for the lookahead	*/

				/*  symbol				*/

#endif



int yynerrs;			/*  number of parse errors so far       */

#endif  /* not YYPURE */



#if YYDEBUG != 0

int yydebug;			/*  nonzero means print parse trace	*/

/* Since this is uninitialized, it does not stop multiple parsers

   from coexisting.  */

#endif



/*  YYINITDEPTH indicates the initial size of the parser's stacks	*/



#ifndef	YYINITDEPTH

#define YYINITDEPTH 200

#endif



/*  YYMAXDEPTH is the maximum size the stacks can grow to

    (effective only if the built-in stack extension method is used).  */



#if YYMAXDEPTH == 0

#undef YYMAXDEPTH

#endif



#ifndef YYMAXDEPTH

#define YYMAXDEPTH 10000

#endif



/* Prevent warning if -Wstrict-prototypes.  */

#ifdef __GNUC__

int yyparse (void);

#endif



#if __GNUC__ > 1		/* GNU C and GNU C++ define this.  */

#define __yy_memcpy(FROM,TO,COUNT)	__builtin_memcpy(TO,FROM,COUNT)

#else				/* not GNU C or C++ */

#ifndef __cplusplus



/* This is the most reliable way to avoid incompatibilities

   in available built-in functions on various systems.  */

static void

__yy_memcpy (from, to, count)

     char *from;

     char *to;

     int count;

{

  register char *f = from;

  register char *t = to;

  register int i = count;



  while (i-- > 0)

    *t++ = *f++;

}



#else /* __cplusplus */



/* This is the most reliable way to avoid incompatibilities

   in available built-in functions on various systems.  */

static void

__yy_memcpy (char *from, char *to, int count)

{

  register char *f = from;

  register char *t = to;

  register int i = count;



  while (i-- > 0)

    *t++ = *f++;

}



#endif

#endif



// 192 "bison.simple"



/* The user can define YYPARSE_PARAM as the name of an argument to be passed

   into yyparse.  The argument should have type void *.

   It should actually point to an object.

   Grammar actions can access the variable by casting it

   to the proper pointer type.  */



#ifdef YYPARSE_PARAM

#define YYPARSE_PARAM_DECL void *YYPARSE_PARAM;

#else

#define YYPARSE_PARAM

#define YYPARSE_PARAM_DECL

#endif



int

yyparse(YYPARSE_PARAM)

     YYPARSE_PARAM_DECL

{

  register int yystate;

  register int yyn;

  register short *yyssp;

  register YYSTYPE *yyvsp;

  int yyerrstatus;	/*  number of tokens to shift before error messages enabled */

  int yychar1 = 0;		/*  lookahead token as an internal (translated) token number */



  short	yyssa[YYINITDEPTH];	/*  the state stack			*/

  YYSTYPE yyvsa[YYINITDEPTH];	/*  the semantic value stack		*/



  short *yyss = yyssa;		/*  refer to the stacks thru separate pointers */

  YYSTYPE *yyvs = yyvsa;	/*  to allow yyoverflow to reallocate them elsewhere */



#ifdef YYLSP_NEEDED

  YYLTYPE yylsa[YYINITDEPTH];	/*  the location stack			*/

  YYLTYPE *yyls = yylsa;

  YYLTYPE *yylsp;



#define YYPOPSTACK   (yyvsp--, yyssp--, yylsp--)

#else

#define YYPOPSTACK   (yyvsp--, yyssp--)

#endif



  int yystacksize = YYINITDEPTH;



#ifdef YYPURE

  int yychar;

  YYSTYPE yylval;

  int yynerrs;

#ifdef YYLSP_NEEDED

  YYLTYPE yylloc;

#endif

#endif



  YYSTYPE yyval;		/*  the variable used to return		*/

				/*  semantic values from the action	*/

				/*  routines				*/



  int yylen;



#if YYDEBUG != 0

  //if (yydebug)

 //   fprintf(stderr, "Starting parse\n");

#endif



  yystate = 0;

  yyerrstatus = 0;

  yynerrs = 0;

  yychar = YYEMPTY;		/* Cause a token to be read.  */



  /* Initialize stack pointers.

     Waste one element of value and location stack

     so that they stay on the same level as the state stack.

     The wasted elements are never initialized.  */



  yyssp = yyss - 1;

  yyvsp = yyvs;

#ifdef YYLSP_NEEDED

  yylsp = yyls;

#endif



/* Push a new state, which is found in  yystate  .  */

/* In all cases, when you get here, the value and location stacks

   have just been pushed. so pushing a state here evens the stacks.  */

yynewstate:



  *++yyssp = yystate;



  if (yyssp >= yyss + yystacksize - 1)

    {

      /* Give user a chance to reallocate the stack */

      /* Use copies of these so that the &'s don't force the real ones into memory. */

      YYSTYPE *yyvs1 = yyvs;

      short *yyss1 = yyss;

#ifdef YYLSP_NEEDED

      YYLTYPE *yyls1 = yyls;

#endif



      /* Get the current used size of the three stacks, in elements.  */

      int size = yyssp - yyss + 1;



#ifdef yyoverflow

      /* Each stack pointer address is followed by the size of

	 the data in use in that stack, in bytes.  */

#ifdef YYLSP_NEEDED

      /* This used to be a conditional around just the two extra args,

	 but that might be undefined if yyoverflow is a macro.  */

      yyoverflow("parser stack overflow",

		 &yyss1, size * sizeof (*yyssp),

		 &yyvs1, size * sizeof (*yyvsp),

		 &yyls1, size * sizeof (*yylsp),

		 &yystacksize);

#else

      yyoverflow("parser stack overflow",

		 &yyss1, size * sizeof (*yyssp),

		 &yyvs1, size * sizeof (*yyvsp),

		 &yystacksize);

#endif



      yyss = yyss1; yyvs = yyvs1;

#ifdef YYLSP_NEEDED

      yyls = yyls1;

#endif

#else /* no yyoverflow */

      /* Extend the stack our own way.  */

      if (yystacksize >= YYMAXDEPTH)

	{

	  yyerror("parser stack overflow");

	  return 2;

	}

      yystacksize *= 2;

      if (yystacksize > YYMAXDEPTH)

	yystacksize = YYMAXDEPTH;

      yyss = (short *) alloca (yystacksize * sizeof (*yyssp));

      __yy_memcpy ((char *)yyss1, (char *)yyss, size * sizeof (*yyssp));

      yyvs = (YYSTYPE *) alloca (yystacksize * sizeof (*yyvsp));

      __yy_memcpy ((char *)yyvs1, (char *)yyvs, size * sizeof (*yyvsp));

#ifdef YYLSP_NEEDED

      yyls = (YYLTYPE *) alloca (yystacksize * sizeof (*yylsp));

      __yy_memcpy ((char *)yyls1, (char *)yyls, size * sizeof (*yylsp));

#endif

#endif /* no yyoverflow */



      yyssp = yyss + size - 1;

      yyvsp = yyvs + size - 1;

#ifdef YYLSP_NEEDED

      yylsp = yyls + size - 1;

#endif



#if YYDEBUG != 0

    //  if (yydebug)

	//fprintf(stderr, "Stack size increased to %d\n", yystacksize);

#endif



      if (yyssp >= yyss + yystacksize - 1)

	YYABORT;

    }



#if YYDEBUG != 0

  //if (yydebug)

   // fprintf(stderr, "Entering state %d\n", yystate);

#endif



  goto yybackup;

 yybackup:



/* Do appropriate processing given the current state.  */

/* Read a lookahead token if we need one and don't already have one.  */

/* yyresume: */



  /* First try to decide what to do without reference to lookahead token.  */



  yyn = yypact[yystate];

  if (yyn == YYFLAG)

    goto yydefault;



  /* Not known => get a lookahead token if don't already have one.  */



  /* yychar is either YYEMPTY or YYEOF

     or a valid token in external form.  */



  if (yychar == YYEMPTY)

    {

#if YYDEBUG != 0

    //  if (yydebug)

//	fprintf(stderr, "Reading a token: ");

#endif

      yychar = YYLEX;

    }



  /* Convert token to internal form (in yychar1) for indexing tables with */



  if (yychar <= 0)		/* This means end of input. */

    {

      yychar1 = 0;

      yychar = YYEOF;		/* Don't call YYLEX any more */



#if YYDEBUG != 0

  //    if (yydebug)

//	fprintf(stderr, "Now at end of input.\n");

#endif

    }

  else

    {

      yychar1 = YYTRANSLATE(yychar);



#if YYDEBUG != 0

      if (yydebug)

	{

	 // fprintf (stderr, "Next token is %d (%s", yychar, yytname[yychar1]);

	  /* Give the individual parser a way to print the precise meaning

	     of a token, for further debugging info.  */

#ifdef YYPRINT

	  YYPRINT (stderr, yychar, yylval);

#endif

	 // fprintf (stderr, ")\n");

	}

#endif

    }



  yyn += yychar1;

  if (yyn < 0 || yyn > YYLAST || yycheck[yyn] != yychar1)

    goto yydefault;



  yyn = yytable[yyn];



  /* yyn is what to do for this token type in this state.

     Negative => reduce, -yyn is rule number.

     Positive => shift, yyn is new state.

       New state is final state => don't bother to shift,

       just return success.

     0, or most negative number => error.  */



  if (yyn < 0)

    {

      if (yyn == YYFLAG)

	goto yyerrlab;

      yyn = -yyn;

      goto yyreduce;

    }

  else if (yyn == 0)

    goto yyerrlab;



  if (yyn == YYFINAL)

    YYACCEPT;



  /* Shift the lookahead token.  */



#if YYDEBUG != 0

 // if (yydebug)

   // fprintf(stderr, "Shifting token %d (%s), ", yychar, yytname[yychar1]);

#endif



  /* Discard the token being shifted unless it is eof.  */

  if (yychar != YYEOF)

    yychar = YYEMPTY;



  *++yyvsp = yylval;

#ifdef YYLSP_NEEDED

  *++yylsp = yylloc;

#endif



  /* count tokens shifted since error; after three, turn off error status.  */

  if (yyerrstatus) yyerrstatus--;



  yystate = yyn;

  goto yynewstate;



/* Do the default action for the current state.  */

yydefault:



  yyn = yydefact[yystate];

  if (yyn == 0)

    goto yyerrlab;



/* Do a reduction.  yyn is the number of a rule to reduce with.  */

yyreduce:

  yylen = yyr2[yyn];

  if (yylen > 0)

    yyval = yyvsp[1-yylen]; /* implement default value of the action */



#if YYDEBUG != 0

  if (yydebug)

    {

      int i;



   //   fprintf (stderr, "Reducing via rule %d (line %d), ",

	 //      yyn, yyrline[yyn]);



      /* Print the symbols being reduced, and their result.  */

      for (i = yyprhs[yyn]; yyrhs[i] > 0; i++)

//	fprintf (stderr, "%s ", yytname[yyrhs[i]]);

  //    fprintf (stderr, " -> %s\n", yytname[yyr1[yyn]]);

    }

#endif




  switch (yyn) {

case 1:
{
    dvd_vm_parsed_cmd=yyval.statement;
;
    break;}
case 2:
{
    yyval.statement=yyvsp[0].statement;
;
    break;}
case 3:
{
    yyval.statement=yyvsp[-1].statement;
    yyval.statement->next=yyvsp[0].statement;
;
    break;}
case 4:
{
    yyval.statement=yyvsp[0].statement;
;
    break;}
case 5:
{
    yyval.statement=yyvsp[0].statement;
;
    break;}
case 6:
{
    yyval.statement=statement_new();
    yyval.statement->op=VM_EXIT;
;
    break;}
case 7:
{
    yyval.statement=statement_new();
    yyval.statement->op=VM_RESUME;
;
    break;}
case 8:
{
    yyval.statement=yyvsp[0].statement;
;
    break;}
case 9:
{
    yyval.statement=yyvsp[-1].statement;
;
    break;}
case 10:
{
    yyval.statement=yyvsp[0].statement;
;
    break;}
case 11:
{
    yyval.int_val=(yyvsp[0].int_val)+1;
;
    break;}
case 12:
// 132 "vm.y"
{
    yyval.int_val=1;
;
    break;}
case 13:
{
    yyval.int_val=0;
;
    break;}
case 14:
{
    yyval.int_val=yyvsp[0].int_val;
;
    break;}
case 15:
{
    yyval.int_val=120; // default entry
;
    break;}
case 16:
{
    yyval.int_val=122;
;
    break;}
case 17:
{
    yyval.int_val=123;
;
    break;}
case 18:
{
    yyval.int_val=124;
;
    break;}
case 19:
{
    yyval.int_val=125;
;
    break;}
case 20:
{
    yyval.int_val=126;
;
    break;}
case 21:
{
    yyval.int_val=127;
;
    break;}
case 22:
{
    yyval.int_val=121;
;
    break;}
case 23:
{
    yyval.int_val=(yyvsp[0].int_val)|128;
;
    break;}
case 24:
{
    yyval.int_val=0;
;
    break;}
case 25:
{
    yyval.int_val=yyvsp[0].int_val;
;
    break;}
case 26:

{
    yyval.int_val=0;
;
    break;}
case 27:
{
    yyval.statement=statement_new();
    yyval.statement->op=VM_JUMP;
    yyval.statement->i1=yyvsp[-3].int_val;
    yyval.statement->i2=yyvsp[-2].int_val;
    yyval.statement->i3=yyvsp[-1].int_val;
;
    break;}
case 28:
{
    yyval.statement=statement_new();
    yyval.statement->op=VM_JUMP;
    yyval.statement->i3=2*65536+yyvsp[-1].int_val;
;
    break;}
case 29:
{
    yyval.statement=statement_new();
    yyval.statement->op=VM_JUMP;
    yyval.statement->i3=65536+yyvsp[-1].int_val;
;
    break;}
case 30:
{
    yyval.int_val=yyvsp[0].int_val;
;
    break;}
case 31:
{
    yyval.int_val=0;
;
    break;}
case 32:
{
    yyval.statement=statement_new();
    yyval.statement->op=VM_CALL;
    yyval.statement->i1=yyvsp[-4].int_val;
    yyval.statement->i2=yyvsp[-3].int_val;
    yyval.statement->i3=yyvsp[-2].int_val;
    yyval.statement->i4=yyvsp[-1].int_val;
;
    break;}
case 33:
{
    yyval.int_val=yyvsp[0].int_val;
;
    break;}
case 34:
{
    yyval.int_val=yyvsp[0].int_val+0x80;
;
    break;}
case 35:
{
    yyval.int_val=0x81;
;
    break;}
case 36:
{
    yyval.int_val=0x82;
;
    break;}
case 37:
{
    yyval.int_val=0x83;
;
    break;}
case 38:
{
    yyval.int_val=0x88;
;
    break;}
case 39:
{
    yyval.int_val=yyvsp[0].int_val-256;
;
    break;}
case 40:
{
    yyval.int_val=yyvsp[0].int_val;
;
    break;}
case 41:
{
    yyval.statement=yyvsp[-1].statement;
;
    break;}
case 42:
{
    yyval.statement=statement_new();
    yyval.statement->op=VM_VAL;
    yyval.statement->i1=yyvsp[0].int_val;
;
    break;}
case 43:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_ADD,yyvsp[0].statement);
;
    break;}
case 44:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_SUB,yyvsp[0].statement);
;
    break;}
case 45:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_MUL,yyvsp[0].statement);
;
    break;}
case 46:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_DIV,yyvsp[0].statement);
;
    break;}
case 47:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_MOD,yyvsp[0].statement);
;
    break;}
case 48:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_AND,yyvsp[0].statement);
;
    break;}
case 49:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_OR, yyvsp[0].statement);
;
    break;}
case 50:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_AND,yyvsp[0].statement);
;
    break;}
case 51:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_OR, yyvsp[0].statement);
;
    break;}
case 52:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_XOR,yyvsp[0].statement);
;
    break;}
case 53:
{
    yyval.statement=yyvsp[-1].statement;
;
    break;}
case 54:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_EQ,yyvsp[0].statement);
;
    break;}
case 55:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_NE,yyvsp[0].statement);
;
    break;}
case 56:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_GTE,yyvsp[0].statement);
;
    break;}
case 57:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_GT,yyvsp[0].statement);
;
    break;}
case 58:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_LTE,yyvsp[0].statement);
;
    break;}
case 59:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_LT,yyvsp[0].statement);
;
    break;}
case 60:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_LOR,yyvsp[0].statement);
;
    break;}
case 61:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_LAND,yyvsp[0].statement);
;
    break;}
case 62:
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_LOR,yyvsp[0].statement);
;
    break;}
case 63:
 //318 "vm.y"
{
    yyval.statement=statement_expression(yyvsp[-2].statement,VM_LAND,yyvsp[0].statement);
;
    break;}
case 64:
 //321 "vm.y"
{
    yyval.statement=statement_new();
    yyval.statement->op=VM_NOT;
    yyval.statement->param=yyvsp[0].statement;
;
    break;}
case 65:
// 328 "vm.y"
{
    yyval.statement=statement_new();
    yyval.statement->op=VM_SET;
    yyval.statement->i1=yyvsp[-3].int_val;
    yyval.statement->param=yyvsp[-1].statement;
;
    break;}
case 66:
// 334 "vm.y"
{
    yyval.statement=statement_setop(yyvsp[-3].int_val,VM_ADD,yyvsp[-1].statement);
;
    break;}
case 67:
 //337 "vm.y"
{
    yyval.statement=statement_setop(yyvsp[-3].int_val,VM_SUB,yyvsp[-1].statement);
;
    break;}
case 68:
 //340 "vm.y"
{
    yyval.statement=statement_setop(yyvsp[-3].int_val,VM_MUL,yyvsp[-1].statement);
;
    break;}
case 69:
//343 "vm.y"
{
    yyval.statement=statement_setop(yyvsp[-3].int_val,VM_DIV,yyvsp[-1].statement);
;
    break;}
case 70:
 //346 "vm.y"
{
    yyval.statement=statement_setop(yyvsp[-3].int_val,VM_MOD,yyvsp[-1].statement);
;
    break;}
case 71:
// 349 "vm.y"
{
    yyval.statement=statement_setop(yyvsp[-3].int_val,VM_AND,yyvsp[-1].statement);
;
    break;}
case 72:
 //352 "vm.y"
{
    yyval.statement=statement_setop(yyvsp[-3].int_val,VM_OR,yyvsp[-1].statement);
;
    break;}
case 73:
// 355 "vm.y"
{
    yyval.statement=statement_setop(yyvsp[-3].int_val,VM_XOR,yyvsp[-1].statement);
;
    break;}
case 74:
// 360 "vm.y"
{
    yyval.statement=statement_new();
    yyval.statement->op=VM_IF;
    yyval.statement->param=yyvsp[-2].statement;
    yyvsp[-2].statement->next=statement_new();
    yyvsp[-2].statement->next->op=VM_IF;
    yyvsp[-2].statement->next->param=yyvsp[0].statement;
;
    break;}
case 75:
// 370 "vm.y"
{
    yyval.statement=yyvsp[0].statement;
;
    break;}
case 76:
// 373 "vm.y"
{
    yyval.statement=yyvsp[-2].statement;
    yyval.statement->param->next->next=yyvsp[0].statement;
;
    break;}
}
   /* the action file gets copied in in place of this dollarsign */

// 487 "bison.simple"



  yyvsp -= yylen;

  yyssp -= yylen;

#ifdef YYLSP_NEEDED

  yylsp -= yylen;

#endif



#if YYDEBUG != 0

  if (yydebug)

    {

      short *ssp1 = yyss - 1;

     // fprintf (stderr, "state stack now");

      while (ssp1 != yyssp)

	fprintf (stderr, " %d", *++ssp1);

      fprintf (stderr, "\n");

    }

#endif



  *++yyvsp = yyval;



#ifdef YYLSP_NEEDED

  yylsp++;

  if (yylen == 0)

    {

      yylsp->first_line = yylloc.first_line;

      yylsp->first_column = yylloc.first_column;

      yylsp->last_line = (yylsp-1)->last_line;

      yylsp->last_column = (yylsp-1)->last_column;

      yylsp->text = 0;

    }

  else

    {

      yylsp->last_line = (yylsp+yylen-1)->last_line;

      yylsp->last_column = (yylsp+yylen-1)->last_column;

    }

#endif



  /* Now "shift" the result of the reduction.

     Determine what state that goes to,

     based on the state we popped back to

     and the rule number reduced by.  */



  yyn = yyr1[yyn];



  yystate = yypgoto[yyn - YYNTBASE] + *yyssp;

  if (yystate >= 0 && yystate <= YYLAST && yycheck[yystate] == *yyssp)

    yystate = yytable[yystate];

  else

    yystate = yydefgoto[yyn - YYNTBASE];



  goto yynewstate;



yyerrlab:   /* here on detecting error */



  if (! yyerrstatus)

    /* If not already recovering from an error, report this error.  */

    {

      ++yynerrs;



#ifdef YYERROR_VERBOSE

      yyn = yypact[yystate];



      if (yyn > YYFLAG && yyn < YYLAST)

	{

	  int size = 0;

	  char *msg;

	  int x, count;



	  count = 0;

	  /* Start X at -yyn if nec to avoid negative indexes in yycheck.  */

	  for (x = (yyn < 0 ? -yyn : 0);

	       x < (sizeof("d") / sizeof(char *)); x++)

	    if (yycheck[x + yyn] == x)
		{

			int ssdwd = strlen("Wd");
	      size += ssdwd + 15; count++;
		}

	  msg = (char *) malloc(size + 15);

	  if (msg != 0)

	    {

	      strcpy(msg, "parse error");



	      if (count < 5)

		{

		  count = 0;

		  for (x = (yyn < 0 ? -yyn : 0);

		       x < (sizeof("d") / sizeof(char *)); x++)

		    if (yycheck[x + yyn] == x)

		      {

			strcat(msg, count == 0 ? ", expecting `" : " or `");

			strcat(msg, "d");

			strcat(msg, "'");

			count++;

		      }

		}

	      yyerror(msg);

	      free(msg);

	    }

	  else

	    yyerror ("parse error; also virtual memory exceeded");

	}

      else

#endif /* YYERROR_VERBOSE */

	yyerror("parse error");

    }



  goto yyerrlab1;

yyerrlab1:   /* here on error raised explicitly by an action */



  if (yyerrstatus == 3)

    {

      /* if just tried and failed to reuse lookahead token after an error, discard it.  */



      /* return failure if at end of input */

      if (yychar == YYEOF)

	YYABORT;



#if YYDEBUG != 0

    //  if (yydebug)

//	fprintf(stderr, "Discarding token %d (%s).\n", yychar, yytname[yychar1]);

#endif



      yychar = YYEMPTY;

    }



  /* Else will try to reuse lookahead token

     after shifting the error token.  */



  yyerrstatus = 3;		/* Each real token shifted decrements this */



  goto yyerrhandle;



yyerrdefault:  /* current state does not do anything special for the error token. */



#if 0

  /* This is wrong; only states that explicitly want error tokens

     should shift them.  */

  yyn = yydefact[yystate];  /* If its default is to accept any token, ok.  Otherwise pop it.*/

  if (yyn) goto yydefault;

#endif



yyerrpop:   /* pop the current state because it cannot handle the error token */



  if (yyssp == yyss) YYABORT;

  yyvsp--;

  yystate = *--yyssp;

#ifdef YYLSP_NEEDED

  yylsp--;

#endif



#if YYDEBUG != 0

  if (yydebug)

    {

      short *ssp1 = yyss - 1;

     // fprintf (stderr, "Error: state stack now");

      while (ssp1 != yyssp)

	fprintf (stderr, " %d", *++ssp1);

      fprintf (stderr, "\n");

    }

#endif



yyerrhandle:



  yyn = yypact[yystate];

  if (yyn == YYFLAG)

    goto yyerrdefault;



  yyn += YYTERROR;

  if (yyn < 0 || yyn > YYLAST || yycheck[yyn] != YYTERROR)

    goto yyerrdefault;



  yyn = yytable[yyn];

  if (yyn < 0)

    {

      if (yyn == YYFLAG)

	goto yyerrpop;

      yyn = -yyn;

      goto yyreduce;

    }

  else if (yyn == 0)

    goto yyerrpop;



  if (yyn == YYFINAL)

    YYACCEPT;



#if YYDEBUG != 0

  //if (yydebug)

  //  fprintf(stderr, "Shifting error token, ");

#endif



  *++yyvsp = yylval;

#ifdef YYLSP_NEEDED

  *++yylsp = yylloc;

#endif



  yystate = yyn;

  goto yynewstate;

}

// 378 "vm.y"
