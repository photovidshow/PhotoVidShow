#ifndef __SUBFONT_H
#define __SUBFONT_H

#ifdef HAVE_FREETYPE
#ifdef HAVE_FT2BUILD_H
#include <ft2build.h>
#endif
#include FT_FREETYPE_H
#endif

typedef struct {
    unsigned char *bmp;
    unsigned char *pal;
    int w,h,c;
#ifdef HAVE_FREETYPE
    int charwidth,charheight,pen,baseline,padding;
    int current_count, current_alloc;
#endif
} raw_file;

typedef struct {
#ifdef HAVE_FREETYPE
    int dynamic;
#endif
    char *name;
    char *fpath;
    int spacewidth;
    int charspace;
    int height;
//    char *fname_a;
//    char *fname_b;
    raw_file* pic_a[16];
    raw_file* pic_b[16];
    short font[65536];
    int start[65536];   // short is not enough for unicode fonts
    short width[65536];
    int freetype;

#ifdef HAVE_FREETYPE
    int face_cnt;

    FT_Face faces[16];
    FT_UInt glyph_index[65536];

    int max_width, max_height;

    struct
    {
	int g_r;
	int o_r;
	int g_w;
	int o_w;
	int o_size;
	unsigned volume;

	unsigned *g;
	unsigned *gt2;
	unsigned *om;
	unsigned char *omt;
	unsigned short *tmp;
    } tables;
#endif

} font_desc_t;

extern font_desc_t* vo_font;

#ifdef HAVE_FREETYPE

int init_freetype();
int done_freetype();

font_desc_t* read_font_desc_ft(char* fname,int movie_width, int movie_height);
void free_font_desc(font_desc_t *desc);

void render_one_glyph(font_desc_t *desc, int c);
int kerning(font_desc_t *desc, int prevc, int c);

void load_font_ft(int width, int height);

#else

static void render_one_glyph(font_desc_t *desc, int c) {}
static int kerning(font_desc_t *desc, int prevc, int c) { return 0; }

#endif

raw_file* load_raw(char *name,int verbose);
font_desc_t* read_font_desc(char* fname,float factor,int verbose);

#endif /* ! __MPLAYER_FONT_LOAD_H */
