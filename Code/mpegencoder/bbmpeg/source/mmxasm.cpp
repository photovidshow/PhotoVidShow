/* mmxasm.c, mmx enabled routines */

#ifdef __BORLANDC__
#define EMIT db
#else
#define EMIT _emit
#endif

#include "main.h"
#include <excpt.h>

/* taken from AMD's CPUID_EX example */

unsigned int get_feature_flags()
{
  unsigned int result    = 0;
  unsigned int signature = 0;
  char vendor[13]        = "UnknownVendr";  /* Needs to be exactly 12 chars */

  /* Define known vendor strings here */

  char vendorAMD[13]     = "AuthenticAMD";  /* Needs to be exactly 12 chars */
  /*;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
    ;; Step 1: Check if processor has CPUID support. Processor faults
    ;; with an illegal instruction exception if the instruction is not
    ;; supported. This step catches the exception and immediately returns
    ;; with feature string bits with all 0s, if the exception occurs.
    ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;*/
  __try {
	__asm xor    eax, eax
	__asm xor    ebx, ebx
	__asm xor    ecx, ecx
	__asm xor    edx, edx
	__asm cpuid
      }

  __except (EXCEPTION_EXECUTE_HANDLER)
  {
    return (0);
  }

  result |= FEATURE_CPUID;

  _asm {
         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
         //;; Step 2: Check if CPUID supports function 1 (signature/std features)
         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

         xor     eax, eax                       //; CPUID function #0
         cpuid                                  //; largest std func/vendor string
         mov     dword ptr [vendor], ebx        //; save
         mov     dword ptr [vendor+4], edx      //;  vendor
         mov     dword ptr [vendor+8], ecx      //;   string
         test    eax, eax                       //; largest standard function==0?
         jz      $no_standard_features          //; yes, no standard features func
         or      [result], FEATURE_STD_FEATURES //; does have standard features

         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
         //;; Step 3: Get standard feature flags and signature
         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

         mov     eax, 1                        //; CPUID function #1
         cpuid                                 //; get signature/std feature flgs
         mov     [signature], eax              //; save processor signature

         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
         //;; Step 4: Extract desired features from standard feature flags
         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

         //;; Check for time stamp counter support

         mov     ecx, CPUID_STD_TSC            //; bit 4 indicates TSC support
         and     ecx, edx                      //; supports TSC ? CPUID_STD_TSC:0
         neg     ecx                           //; supports TSC ? CY : NC
         sbb     ecx, ecx                      //; supports TSC ? 0xffffffff:0
         and     ecx, FEATURE_TSC              //; supports TSC ? FEATURE_TSC:0
         or      [result], ecx                 //; merge into feature flags

         //;; Check for MMX support

         mov     ecx, CPUID_STD_MMX            //; bit 23 indicates MMX support
         and     ecx, edx                      //; supports MMX ? CPUID_STD_MMX:0
         neg     ecx                           //; supports MMX ? CY : NC
         sbb     ecx, ecx                      //; supports MMX ? 0xffffffff:0
         and     ecx, FEATURE_MMX              //; supports MMX ? FEATURE_MMX:0
         or      [result], ecx                 //; merge into feature flags

         //;; Check for CMOV support

         mov     ecx, CPUID_STD_CMOV           //; bit 15 indicates CMOV support
         and     ecx, edx                      //; supports CMOV?CPUID_STD_CMOV:0
         neg     ecx                           //; supports CMOV ? CY : NC
         sbb     ecx, ecx                      //; supports CMOV ? 0xffffffff:0
         and     ecx, FEATURE_CMOV             //; supports CMOV ? FEATURE_CMOV:0
         or      [result], ecx                 //; merge into feature flags

         //;; Check support for P6-style MTRRs

         mov     ecx, CPUID_STD_MTRR           //; bit 12 indicates MTRR support
         and     ecx, edx                      //; supports MTRR?CPUID_STD_MTRR:0
         neg     ecx                           //; supports MTRR ? CY : NC
         sbb     ecx, ecx                      //; supports MTRR ? 0xffffffff:0
         and     ecx, FEATURE_P6_MTRR          //; supports MTRR ? FEATURE_MTRR:0
         or      [result], ecx                 //; merge into feature flags

         //;; Check for initial SSE support. There can still be partial SSE
         //;; support. Step 9 will check for partial support.

         mov     ecx, CPUID_STD_SSE            //; bit 25 indicates SSE support
         and     ecx, edx                      //; supports SSE ? CPUID_STD_SSE:0
         neg     ecx                           //; supports SSE ? CY : NC
         sbb     ecx, ecx                      //; supports SSE ? 0xffffffff:0
         and     ecx, (FEATURE_MMXEXT+FEATURE_SSEFP) //; supports SSE ?
                                               //; FEATURE_MMXEXT+FEATURE_SSEFP:0
         or      [result], ecx                 //; merge into feature flags

         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
         //;; Step 5: Check for CPUID extended functions
         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

         mov     eax, 0x80000000                //; extended function 0x80000000
         cpuid                                  //; largest extended function
         cmp     eax, 0x80000000                //; no function > 0x80000000 ?
         jbe     $no_extended_features          //; yes, no extended feature flags
         or      [result], FEATURE_EXT_FEATURES //; does have ext. feature flags

         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
         //;; Step 6: Get extended feature flags
         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

         mov     eax, 0x80000001               //; CPUID ext. function 0x80000001
         cpuid                                 //; EDX = extended feature flags

         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
         //;; Step 7: Extract vendor independent features from extended flags
         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

         //;; Check for 3DNow! support (vendor independent)

         mov     ecx, CPUID_EXT_3DNOW          //; bit 31 indicates 3DNow! supprt
         and     ecx, edx                      //; supp 3DNow! ?CPUID_EXT_3DNOW:0
         neg     ecx                           //; supports 3DNow! ? CY : NC
         sbb     ecx, ecx                      //; supports 3DNow! ? 0xffffffff:0
         and     ecx, FEATURE_3DNOW            //; support 3DNow!?FEATURE_3DNOW:0
         or      [result], ecx                 //; merge into feature flags

         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
         //;; Step 8: Determine CPU vendor
         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

         lea     esi, vendorAMD                //; AMD's vendor string
         lea     edi, vendor                   //; this CPU's vendor string
         mov     ecx, 12                       //; strings are 12 characters
         cld                                   //; compare lowest to highest
         repe    cmpsb                         //; current vendor str == AMD's ?
         jnz     $not_AMD                      //; no, CPU vendor is not AMD

         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
         //;; Step 9: Check AMD specific extended features
         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

         mov     ecx, CPUID_EXT_AMD_3DNOWEXT   //; bit 30 indicates 3DNow! ext.
         and     ecx, edx                      //; 3DNow! ext?
         neg     ecx                           //; 3DNow! ext ? CY : NC
         sbb     ecx, ecx                      //; 3DNow! ext ? 0xffffffff : 0
         and     ecx, FEATURE_3DNOWEXT         //; 3DNow! ext?FEATURE_3DNOWEXT:0
         or      [result], ecx                 //; merge into feature flags

         test    [result], FEATURE_MMXEXT      //; determined SSE MMX support?
         jnz     $has_mmxext                   //; yes, don't need to check again

         //;; Check support for AMD's multimedia instruction set additions

         mov     ecx, CPUID_EXT_AMD_MMXEXT     //; bit 22 indicates MMX extension
         and     ecx, edx                      //; MMX ext?CPUID_EXT_AMD_MMXEXT:0
         neg     ecx                           //; MMX ext? CY : NC
         sbb     ecx, ecx                      //; MMX ext? 0xffffffff : 0
         and     ecx, FEATURE_MMXEXT           //; MMX ext ? FEATURE_MMXEXT:0
         or      [result], ecx                 //; merge into feature flags

      $has_mmxext:

         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
         //;; Step 10: Check AMD-specific features not reported by CPUID
         //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

         //;; Check support for AMD-K6 processor-style MTRRs

         mov     eax, [signature] 	//; get processor signature
         and     eax, 0xFFF 		//; extract family/model/stepping
         cmp     eax, 0x588 		//; CPU < AMD-K6-2/CXT ? CY : NC
         sbb     edx, edx 		//; CPU < AMD-K6-2/CXT ? 0xffffffff:0
         not     edx 			//; CPU < AMD-K6-2/CXT ? 0:0xffffffff
         cmp     eax, 0x600 		//; CPU < AMD Athlon ? CY : NC
         sbb     ecx, ecx 		//; CPU < AMD-K6 ? 0xffffffff:0
         and     ecx, edx 		//; (CPU>=AMD-K6-2/CXT)&&
					//; (CPU<AMD Athlon) ? 0xffffffff:0
         and     ecx, FEATURE_K6_MTRR 	//; (CPU>=AMD-K6-2/CXT)&&
					//; (CPU<AMD Athlon) ? FEATURE_K6_MTRR:0
         or      [result], ecx 		//; merge into feature flags

         jmp     $all_done 		//; desired features determined

      $not_AMD:

         /* Extract features specific to non AMD CPUs */

      $no_extended_features:
      $no_standard_features:
      $all_done:
  }
   /* The FP part of SSE introduces a new architectural state and therefore
      requires support from the operating system. So even if CPUID indicates
      support for SSE FP, the application might not be able to use it. If
      CPUID indicates support for SSE FP, check here whether it is also
      supported by the OS, and turn off the SSE FP feature bit if there
      is no OS support for SSE FP.

      Operating systems that do not support SSE FP return an illegal
      instruction exception if execution of an SSE FP instruction is performed.
      Here, a sample SSE FP instruction is executed, and is checked for an
      exception using the (non-standard) __try/__except mechanism
      of Microsoft Visual C.
   */

  if (result & FEATURE_SSEFP)
  {
    __try {
          __asm EMIT 0x0f
          __asm EMIT 0x56
          __asm EMIT 0xC0    //;; orps xmm0, xmm0
          return (result);
       }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
      return (result & (~FEATURE_SSEFP));
    }
  }
  else
    return (result);
}

/* RGB to YUV MMX routine */

static short int ycoef[7][4] = {     /* 32768 scaled y table, bgra order */
    {2363, 23442, 6963, 0},
    {3736, 19235, 9798, 0},
    {3736, 19235, 9798, 0},
    {3604, 19333, 9830, 0},
    {3736, 19235, 9798, 0},
    {3736, 19235, 9798, 0},
    {2851, 22970, 6947, 0}};

static short int ucoef[7][4] = {
    {16384, -12648, -3768, 0},
    {16384, -10846, -5538, 0},
    {16384, -10846, -5538, 0},
    {16384, -10846, -5538, 0},
    {16384, -10846, -5538, 0},
    {16384, -10846, -5538, 0},
    {16384, -12583, -3801, 0}};

static short int vcoef[7][4] = {
    {-1507, -14877, 16384, 0},
    {-2654, -13730, 16384, 0},
    {-2654, -13730, 16384, 0},
    {-2589, -13795, 16384, 0},
    {-2654, -13730, 16384, 0},
    {-2654, -13730, 16384, 0},
    {-1802, -14582, 16384, 0}};

static short int *ycoefs, *ucoefs, *vcoefs;

void init_rgb_to_yuv_mmx(int coeffs)
{
  int i;

  i = coeffs;
  if (i > 8)
    i = 3;

  ycoefs = &ycoef[i-1][0];
  ucoefs = &ucoef[i-1][0];
  vcoefs = &vcoef[i-1][0];
}

void RGBtoYUVmmx(unsigned char *src, unsigned char *desty, unsigned char *destu,
                 unsigned char *destv, int srcrowsize, int destrowsize,
                 int width, int height)
{
  unsigned char *yp, *up, *vp;
  unsigned char *prow;
  int i, j;

  _asm {
	xor       edx, edx
    	mov       eax, width
	sar       eax,1
        cmp       edx, eax
        jge       yuvexit

	mov       j, eax
	mov       eax, height
        mov       i, eax
	cmp       edx, eax
	jge       yuvexit

	mov       eax, desty
	mov       yp, eax
	mov       eax, destu
	mov       up, eax
	mov       eax, destv
	mov       vp, eax
	mov       eax, src
	mov       prow, eax
        pxor      MM7, MM7
        mov       eax, i

      heighttop:

        mov       i, eax
        mov       edi, j
        mov       ebx, prow
        mov       ecx, yp
        mov       edx, up
        mov       esi, vp

      widthtop:
        movq      MM5, [ebx]  // MM5 has 0 r2 g2 b2 0 r1 g1 b1, two pixels
        add       ebx, 8
        movq      MM6, MM5
        punpcklbw MM5, MM7 // MM5 has 0 r1 g1 b1
        punpckhbw MM6, MM7 // MM6 has 0 r2 g2 b2

        movq      MM0, MM5
        movq      MM1, MM6
        mov       eax, ycoefs
        pmaddwd   MM0, [eax] // MM0 has r1*cr and g1*cg+b1*cb
        movq      MM2, MM0
        psrlq     MM2, 32
        paddd     MM0, MM2   // MM0 has y1 in lower 32 bits
        pmaddwd   MM1, [eax] // MM1 has r2*cr and g2*cg+b2*cb
        movq      MM2, MM1
        psrlq     MM2, 32
        paddd     MM1, MM2   // MM1 has y2 in lower 32 bits
        movd      eax, MM0
        imul      eax, 219
        shr       eax, 8
        add       eax, 540672
        shr       eax, 15
        mov       [ecx], al
        inc       ecx
        movd      eax, MM1
        imul      eax, 219
        shr       eax, 8
        add       eax, 540672
        shr       eax, 15
        mov       [ecx], al
        inc       ecx

        movq      MM0, MM5
        movq      MM1, MM6
        mov       eax, ucoefs
        pmaddwd   MM0, [eax] // MM0 has r1*cr and g1*cg+b1*cb
        movq      MM2, MM0
        psrlq     MM2, 32
        paddd     MM0, MM2   // MM0 has u1 in lower 32 bits
        pmaddwd   MM1, [eax] // MM1 has r2*cr and g2*cg+b2*cb
        movq      MM2, MM1
        psrlq     MM2, 32
        paddd     MM1, MM2   // MM1 has u2 in lower 32 bits
        movd      eax, MM0
        imul      eax, 224
        sar       eax, 8
        add       eax, 4210688
        shr       eax, 15
        mov       [edx], al
        inc       edx
        movd      eax, MM1
        imul      eax, 224
        sar       eax, 8
        add       eax, 4210688
        shr       eax, 15
        mov       [edx], al
        inc       edx

        mov       eax, vcoefs
        pmaddwd   MM5, [eax] // MM5 has r1*cr and g1*cg+b1*cb
        movq      MM2, MM5
        psrlq     MM2, 32
        paddd     MM5, MM2   // MM5 has v1 in lower 32 bits
        pmaddwd   MM6, [eax] // MM6 has r2*cr and g2*cg+b2*cb
        movq      MM2, MM6
        psrlq     MM6, 32
        paddd     MM6, MM2   // MM6 has v2 in lower 32 bits
        movd      eax, MM5
        imul      eax, 224
        sar       eax, 8
        add       eax, 4210688
        shr       eax, 15
        mov       [esi], al
        inc       esi
        movd      eax, MM6
        imul      eax, 224
        sar       eax, 8
        add       eax, 4210688
        shr       eax, 15
        mov       [esi], al
        inc       esi

        dec       edi
        jnz       widthtop

        mov       eax, destrowsize
        add       yp, eax
        add       up, eax
        add       vp, eax
        mov       eax, srcrowsize
        sub       prow, eax
        mov       eax, i
        dec       eax
        jnz       heighttop

      yuvexit:
        emms
      }
}

/* motion estimation MMX routines */

/*
 * total absolute difference between two (16*h) blocks
 * including optional half pel interpolation of blk1 (hx,hy)
 * blk1,blk2: addresses of top left pels of both blocks
 * lx:        distance (in bytes) of vertically adjacent pels
 * hx,hy:     flags for horizontal and/or vertical interpolation
 * h:         height of block (usually 8 or 16)
 * distlim:   bail out if sum exceeds this value
 */

/* MMX version */

int dist1mmx(
unsigned char *blk1, unsigned char *blk2,
int lx, int hx, int hy, int h,
int distlim)
{
  int s = 0;
 _asm {
	mov       edi, h
	mov       edx, hy
	mov       eax, hx
	mov       esi, lx

	test      edi, edi     // h = 0?
	jle       d1exit

        pxor      MM7, MM7    // get zeros in MM7

	test      eax, eax     // hx != 0?
	jne       d1is10
	test      edx, edx     // hy != 0?
	jne       d1is10

        xor       edx, edx     // sum
	mov       eax, blk1
	mov       ebx, blk2

d1top00:
	movq	  MM0, [eax]
	movq	  MM1, [ebx]
	movq	  MM2, MM0
	psubusb	  MM0, MM1
	psubusb	  MM1, MM2
	por	  MM0, MM1
	movq	  MM2, [eax+8]
	movq	  MM3, [ebx+8]
	movq	  MM4, MM2
	psubusb	  MM2, MM3
	psubusb	  MM3, MM4
	por	  MM2, MM3
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM1
	paddw	  MM0, MM2
	paddw	  MM0, MM3
	movq	  MM1, MM0
	punpcklwd MM0, MM7
	punpckhwd MM1, MM7
	paddd	  MM0, MM1
	movd	  ecx, MM0
        add       edx, ecx
	psrlq	  MM0, 32
	movd	  ecx, MM0
        add       edx, ecx

	cmp       edx, distlim
	jge       d1exit1
	add       eax, esi
	add       ebx, esi
	dec       edi
	jg        d1top00
	jmp       d1exit1

d1is10:
	test      eax, eax
	je        d1is01
	test      edx, edx
	jne       d1is01

        xor       edx, edx  // s
	mov       eax, blk1
	mov       ebx, blk2

	pxor	  MM6, MM6
	pcmpeqw	  MM1, MM1
	psubw	  MM6, MM1

d1top10:
	movq	  MM0, [eax]
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	movq	  MM2, [eax+1]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psrlw	  MM0, 1
	psrlw	  MM1, 1
	packuswb  MM0, MM1
	movq	  MM1, [ebx]
	movq	  MM2, MM0
	psubusb	  MM0, MM1
	psubusb	  MM1, MM2
	por	  MM0, MM1
	movq	  MM1, [eax+8]
	movq	  MM2, MM1
	punpcklbw MM1, MM7
	punpckhbw MM2, MM7
	movq	  MM3, [eax+9]
	movq	  MM4, MM3
	punpcklbw MM3, MM7
	punpckhbw MM4, MM7
	paddw	  MM1, MM3
	paddw	  MM2, MM4
	paddw	  MM1, MM6
	paddw	  MM2, MM6
	psrlw	  MM1, 1
	psrlw	  MM2, 1
	packuswb  MM1, MM2
	movq	  MM2, [ebx+8]
	movq	  MM3, MM1
	psubusb	  MM1, MM2
	psubusb	  MM2, MM3
	por	  MM1, MM2
	movq	  MM2, MM0
	punpcklbw MM0, MM7
	punpckhbw MM2, MM7
	movq	  MM3, MM1
	punpcklbw MM1, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM1
	paddw	  MM0, MM2
	paddw	  MM0, MM3
	movq	  MM1, MM0
	punpcklwd MM0, MM7
	punpckhwd MM1, MM7
	paddd	  MM0, MM1
	movd	  ecx, MM0
        add       edx, ecx
	psrlq	  MM0, 32
	movd	  ecx, MM0
        add       edx, ecx

	add       eax, esi
	add       ebx, esi
	dec       edi
	jg        d1top10
	jmp       d1exit1

d1is01:
	test      eax, eax
	jne       d1is11
	test      edx, edx
	je        d1is11

	mov       eax, blk1
	mov       edx, blk2
	mov       ebx, eax
	add       ebx, esi // blk1 + lx

	pxor	  MM6, MM6
	pcmpeqw	  MM1, MM1
	psubw	  MM6, MM1

d1top01:
	movq	  MM0, [eax]
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	movq	  MM2, [ebx]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psrlw	  MM0, 1
	psrlw	  MM1, 1
	packuswb  MM0, MM1
	movq	  MM1, [edx]
	movq	  MM2, MM0
	psubusb	  MM0, MM1
	psubusb	  MM1, MM2
	por	  MM0, MM1
	movq	  MM1, [eax+8]
	movq	  MM2, MM1
	punpcklbw MM1, MM7
	punpckhbw MM2, MM7
	movq	  MM3, [ebx+8]
	movq	  MM4, MM3
	punpcklbw MM3, MM7
	punpckhbw MM4, MM7
	paddw	  MM1, MM3
	paddw	  MM2, MM4
	paddw	  MM1, MM6
	paddw	  MM2, MM6
	psrlw	  MM1, 1
	psrlw	  MM2, 1
	packuswb  MM1, MM2
	movq	  MM2, [edx+8]
	movq	  MM3, MM1
	psubusb	  MM1, MM2
	psubusb	  MM2, MM3
	por	  MM1, MM2
	movq	  MM2, MM0
	punpcklbw MM0, MM7
	punpckhbw MM2, MM7
	movq	  MM3, MM1
	punpcklbw MM1, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM1
	paddw	  MM0, MM2
	paddw	  MM0, MM3
	movq	  MM1, MM0
	punpcklwd MM0, MM7
	punpckhwd MM1, MM7
	paddd	  MM0, MM1
	movd	  ecx, MM0
        add       s, ecx
	psrlq	  MM0, 32
	movd	  ecx, MM0
	add	  s, ecx

	mov       eax, ebx
	add       edx, esi
	add       ebx, esi
	dec       edi
	jg        d1top01
	jmp       d1exit

d1is11:
	mov       eax, blk1
	mov       edx, blk2
	mov       ebx, eax
	add       ebx, esi //blk1 + lx

d1top11:
	movq	  MM0, [eax]
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	movq	  MM2, [eax+1]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	movq	  MM2, [ebx]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	movq	  MM4, [ebx+1]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	pxor	  MM6, MM6
	pcmpeqw	  MM5, MM5
	psubw	  MM6, MM5
	paddw	  MM6, MM6
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psrlw	  MM0, 2
	psrlw	  MM1, 2
	packuswb  MM0, MM1
	movq	  MM1, [edx]
	movq	  MM2, MM0
	psubusb	  MM0, MM1
	psubusb	  MM1, MM2
	por	  MM0, MM1
	movq	  MM1, [eax+8]
	movq	  MM2, MM1
	punpcklbw MM1, MM7
	punpckhbw MM2, MM7
	movq	  MM3, [eax+9]
	movq	  MM4, MM3
	punpcklbw MM3, MM7
	punpckhbw MM4, MM7
	paddw	  MM1, MM3
	paddw	  MM2, MM4
	movq	  MM3, [ebx+8]
	movq	  MM4, MM3
	punpcklbw MM3, MM7
	punpckhbw MM4, MM7
	movq	  MM5, [ebx+9]
	movq	  MM6, MM5
	punpcklbw MM5, MM7
	punpckhbw MM6, MM7
	paddw	  MM3, MM5
	paddw	  MM4, MM6
	paddw	  MM1, MM3
	paddw	  MM2, MM4
	pxor	  MM6, MM6
	pcmpeqw	  MM5, MM5
	psubw	  MM6, MM5
	paddw	  MM6, MM6
	paddw	  MM1, MM6
	paddw	  MM2, MM6
	psrlw	  MM1, 2
	psrlw	  MM2, 2
	packuswb  MM1, MM2
	movq	  MM2, [edx+8]
	movq	  MM3, MM1
	psubusb	  MM1, MM2
	psubusb	  MM2, MM3
	por	  MM1, MM2
	movq	  MM2, MM0
	punpcklbw MM0, MM7
	punpckhbw MM2, MM7
	movq	  MM3, MM1
	punpcklbw MM1, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM1
	paddw	  MM0, MM2
	paddw	  MM0, MM3
	movq	  MM1, MM0
	punpcklwd MM0, MM7
	punpckhwd MM1, MM7
	paddd	  MM0, MM1
	movd	  ecx, MM0
        add       s, ecx
	psrlq	  MM0, 32
	movd	  ecx, MM0
	add	  s, ecx

	mov       eax, ebx
	add       ebx, esi
	add       edx, esi
	dec       edi
	jg        d1top11
        jmp       d1exit

d1exit1:
        mov       s, edx

d1exit:
        emms
  }
  return s;
}


/* SSE version */

int dist1sse(
unsigned char *blk1, unsigned char *blk2,
int lx, int hx, int hy, int h,
int distlim)
{
  int s = 0;
 _asm {
	mov       edi, h
	mov       edx, hy
	mov       eax, hx
	mov       esi, lx

	test      edi, edi     // h = 0?
	jle       d1exitsse

        pxor      MM7, MM7    // get zeros in MM7

	test      eax, eax     // hx != 0?
	jne       d1is10sse
	test      edx, edx     // hy != 0?
	jne       d1is10sse

        xor       edx, edx     // sum
	mov       eax, blk1
	mov       ebx, blk2

d1top00sse:
	movq	  MM0, [eax]
        EMIT 15
        EMIT 246
        EMIT 3      // 0x0ff603 = psadbw MM0, [ebx]
	movq	  MM1, [eax+8]
        EMIT 15
        EMIT 246
        EMIT 75
        EMIT 8      // 0x0ff64b08 = psadbw MM1, [ebx+8]
        paddd     MM0, MM1
	movd	  ecx, MM0
        add       edx, ecx

	cmp       edx, distlim
	jge       d1exit1sse
	add       eax, esi
	add       ebx, esi
	dec       edi
	jg        d1top00sse
	jmp       d1exit1sse

d1is10sse:
	test      eax, eax
	je        d1is01sse
	test      edx, edx
	jne       d1is01sse

        xor       edx, edx  // s
	mov       eax, blk1
	mov       ebx, blk2

	pxor	  MM6, MM6
	pcmpeqw	  MM1, MM1
	psubw	  MM6, MM1

d1top10sse:
	movq	  MM0, [eax]
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	movq	  MM2, [eax+1]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psrlw	  MM0, 1
	psrlw	  MM1, 1
	packuswb  MM0, MM1   // MM0 has 8 src half pels
        EMIT 15
        EMIT 246
        EMIT 3      // 0x0ff603 = psadbw MM0, [ebx]

	movq	  MM1, [eax+8]
	movq	  MM2, MM1
	punpcklbw MM1, MM7
	punpckhbw MM2, MM7
	movq	  MM3, [eax+9]
	movq	  MM4, MM3
	punpcklbw MM3, MM7
	punpckhbw MM4, MM7
	paddw	  MM1, MM3
	paddw	  MM2, MM4
	paddw	  MM1, MM6
	paddw	  MM2, MM6
	psrlw	  MM1, 1
	psrlw	  MM2, 1
	packuswb  MM1, MM2      // MM1 has next 8 src half pels
        EMIT 15
        EMIT 246
        EMIT 75
        EMIT 8      // 0x0ff64b08 = psadbw MM1, [ebx+8]

	paddd	  MM0, MM1
	movd	  ecx, MM0
        add       edx, ecx

	add       eax, esi
	add       ebx, esi
	dec       edi
	jg        d1top10sse
	jmp       d1exit1sse

d1is01sse:
	test      eax, eax
	jne       d1is11sse
	test      edx, edx
	je        d1is11sse

	mov       eax, blk1
	mov       ebx, blk2
	mov       edx, eax
	add       edx, esi // blk1 + lx

	pxor	  MM6, MM6
	pcmpeqw	  MM1, MM1
	psubw	  MM6, MM1

d1top01sse:
	movq	  MM0, [eax]
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	movq	  MM2, [edx]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psrlw	  MM0, 1
	psrlw	  MM1, 1
	packuswb  MM0, MM1

        EMIT 15
        EMIT 246
        EMIT 3      // 0x0ff603 = psadbw MM0, [ebx]

	movq	  MM1, [eax+8]
	movq	  MM2, MM1
	punpcklbw MM1, MM7
	punpckhbw MM2, MM7
	movq	  MM3, [edx+8]
	movq	  MM4, MM3
	punpcklbw MM3, MM7
	punpckhbw MM4, MM7
	paddw	  MM1, MM3
	paddw	  MM2, MM4
	paddw	  MM1, MM6
	paddw	  MM2, MM6
	psrlw	  MM1, 1
	psrlw	  MM2, 1
	packuswb  MM1, MM2

        EMIT 15
        EMIT 246
        EMIT 75
        EMIT 8      // 0x0ff64b08 = psadbw MM1, [ebx+8]

	paddd	  MM0, MM1
	movd	  ecx, MM0
	add	  s, ecx

	mov       eax, edx
	add       ebx, esi
	add       edx, esi
	dec       edi
	jg        d1top01sse
	jmp       d1exitsse

d1is11sse:
	mov       eax, blk1
	mov       ebx, blk2
	mov       edx, eax
	add       edx, esi //blk1 + lx

d1top11sse:
	movq	  MM0, [eax]
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	movq	  MM2, [eax+1]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	movq	  MM2, [edx]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	movq	  MM4, [edx+1]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	pxor	  MM6, MM6
	pcmpeqw	  MM5, MM5
	psubw	  MM6, MM5
	paddw	  MM6, MM6
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psrlw	  MM0, 2
	psrlw	  MM1, 2
	packuswb  MM0, MM1

        EMIT 15
        EMIT 246
        EMIT 3      // 0x0ff603 = psadbw MM0, [ebx]

	movq	  MM1, [eax+8]
	movq	  MM2, MM1
	punpcklbw MM1, MM7
	punpckhbw MM2, MM7
	movq	  MM3, [eax+9]
	movq	  MM4, MM3
	punpcklbw MM3, MM7
	punpckhbw MM4, MM7
	paddw	  MM1, MM3
	paddw	  MM2, MM4
	movq	  MM3, [edx+8]
	movq	  MM4, MM3
	punpcklbw MM3, MM7
	punpckhbw MM4, MM7
	movq	  MM5, [edx+9]
	movq	  MM6, MM5
	punpcklbw MM5, MM7
	punpckhbw MM6, MM7
	paddw	  MM3, MM5
	paddw	  MM4, MM6
	paddw	  MM1, MM3
	paddw	  MM2, MM4
	pxor	  MM6, MM6
	pcmpeqw	  MM5, MM5
	psubw	  MM6, MM5
	paddw	  MM6, MM6
	paddw	  MM1, MM6
	paddw	  MM2, MM6
	psrlw	  MM1, 2
	psrlw	  MM2, 2
	packuswb  MM1, MM2

        EMIT 15
        EMIT 246
        EMIT 75
        EMIT 8      // 0x0ff64b08 = psadbw MM1, [ebx+8]

	paddd	  MM0, MM1
	movd	  ecx, MM0
        add       s, ecx

	mov       eax, edx
	add       edx, esi
	add       ebx, esi
	dec       edi
	jg        d1top11sse
        jmp       d1exitsse

d1exit1sse:
        mov       s, edx

d1exitsse:
        emms
  }
  return s;
}
/*
 * total squared difference between two (16*h) blocks
 * including optional half pel interpolation of blk1 (hx,hy)
 * blk1,blk2: addresses of top left pels of both blocks
 * lx:        distance (in bytes) of vertically adjacent pels
 * hx,hy:     flags for horizontal and/or vertical interpolation
 * h:         height of block (usually 8 or 16)
 * MMX version
 */

int dist2mmx(
unsigned char *blk1, unsigned char *blk2,
int lx, int hx, int hy, int h)
{
  int s = 0;
 _asm {
	mov       edi, h
	mov       edx, hy
	mov       eax, hx
	mov       esi, lx

	test      edi, edi     // h = 0?
	jle       d2exit

	pxor	  MM7, MM7     // get zeros i MM7

	test      eax, eax     // hx != 0?
	jne       d2is10
	test      edx, edx     // hy != 0?
	jne       d2is10

        xor       edx, edx                // sum
	mov       eax, blk1
        mov       ebx, blk2

d2top00:
        movq      MM0, [eax]
        movq      MM1, MM0
        punpcklbw MM0, MM7
        punpckhbw MM1, MM7

        movq      MM2, [ebx]
        movq      MM3, MM2
        punpcklbw MM2, MM7
        punpckhbw MM3, MM7

        psubw     MM0, MM2
        psubw     MM1, MM3
        pmaddwd   MM0, MM0
        pmaddwd   MM1, MM1
        paddd     MM0, MM1

        movq      MM1, [eax+8]
        movq      MM2, MM1
        punpcklbw MM1, MM7
        punpckhbw MM2, MM7

        movq      MM3, [ebx+8]
        movq      MM4, MM3
        punpcklbw MM3, MM7
        punpckhbw MM4, MM7

        psubw     MM1, MM3
        psubw     MM2, MM4
        pmaddwd   MM1, MM1
        pmaddwd   MM2, MM2
        paddd     MM1, MM2

        paddd     MM0, MM1
	movd	  ecx, MM0
        add       edx, ecx
	psrlq	  MM0, 32
	movd	  ecx, MM0
        add       edx, ecx

	add       eax, esi
	add       ebx, esi
	dec       edi
	jg        d2top00
	jmp       d2exit1

d2is10:
	test      eax, eax
	je        d2is01
	test      edx, edx
	jne       d2is01

        xor       edx, edx  // s
	mov       eax, blk1
	mov       ebx, blk2

	pxor	  MM6, MM6
	pcmpeqw	  MM1, MM1
	psubw	  MM6, MM1

d2top10:
	movq	  MM0, [eax]
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	movq	  MM2, [eax+1]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psrlw	  MM0, 1
	psrlw	  MM1, 1

	movq	  MM2, [ebx]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7

        psubw     MM0, MM2
        psubw     MM1, MM3
        pmaddwd   MM0, MM0
        pmaddwd   MM1, MM1
        paddd     MM0, MM1

	movq	  MM1, [eax+8]
	movq	  MM2, MM1
	punpcklbw MM1, MM7
	punpckhbw MM2, MM7
	movq	  MM3, [eax+9]
	movq	  MM4, MM3
	punpcklbw MM3, MM7
	punpckhbw MM4, MM7
	paddw	  MM1, MM3
	paddw	  MM2, MM4
	paddw	  MM1, MM6
	paddw	  MM2, MM6
	psrlw	  MM1, 1
	psrlw	  MM2, 1

	movq	  MM3, [ebx+8]
	movq	  MM4, MM3
        punpcklbw MM3, MM7
        punpckhbw MM4, MM7

        psubw     MM1, MM3
        psubw     MM2, MM4
        pmaddwd   MM1, MM1
        pmaddwd   MM2, MM2
        paddd     MM1, MM2

	paddd	  MM0, MM1
	movd	  ecx, MM0
        add       edx, ecx
	psrlq	  MM0, 32
	movd	  ecx, MM0
        add       edx, ecx

	add       eax, esi
	add       ebx, esi
	dec       edi
	jg        d2top10
	jmp       d2exit1

d2is01:
	test      eax, eax
	jne       d2is11
	test      edx, edx
	je        d2is11

	mov       eax, blk1
	mov       edx, blk2
	mov       ebx, eax
	add       ebx, esi // blk1 + lx

	pxor	  MM6, MM6
	pcmpeqw	  MM1, MM1
	psubw	  MM6, MM1

d2top01:
	movq	  MM0, [eax]
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	movq	  MM2, [ebx]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psrlw	  MM0, 1
	psrlw	  MM1, 1

	movq	  MM2, [edx]
	movq	  MM3, MM2
        punpcklbw MM2, MM7
        punpckhbw MM3, MM7

        psubw     MM0, MM2
        psubw     MM1, MM3
        pmaddwd   MM0, MM0
        pmaddwd   MM1, MM1
        paddd     MM0, MM1

	movq	  MM1, [eax+8]
	movq	  MM2, MM1
	punpcklbw MM1, MM7
	punpckhbw MM2, MM7
	movq	  MM3, [ebx+8]
	movq	  MM4, MM3
	punpcklbw MM3, MM7
	punpckhbw MM4, MM7
	paddw	  MM1, MM3
	paddw	  MM2, MM4
	paddw	  MM1, MM6
	paddw	  MM2, MM6
	psrlw	  MM1, 1
	psrlw	  MM2, 1

	movq	  MM3, [edx+8]
	movq	  MM4, MM3
        punpcklbw MM3, MM7
        punpckhbw MM4, MM7

        psubw     MM1, MM3
        psubw     MM2, MM4
        pmaddwd   MM1, MM1
        pmaddwd   MM2, MM2
        paddd     MM1, MM2

	paddd	  MM0, MM1
	movd	  ecx, MM0
        add       s, ecx
	psrlq	  MM0, 32
	movd	  ecx, MM0
	add	  s, ecx

	mov       eax, ebx
	add       edx, esi
	add       ebx, esi
	dec       edi
	jg        d2top01
	jmp       d2exit

d2is11:
	mov       eax, blk1
	mov       edx, blk2
	mov       ebx, eax
	add       ebx, esi //blk1 + lx

d2top11:
	movq	  MM0, [eax]
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	movq	  MM2, [eax+1]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	movq	  MM2, [ebx]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	movq	  MM4, [ebx+1]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	pxor	  MM6, MM6
	pcmpeqw	  MM5, MM5
	psubw	  MM6, MM5
	paddw	  MM6, MM6
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psrlw	  MM0, 2
	psrlw	  MM1, 2

	movq	  MM2, [edx]
	movq	  MM3, MM2
        punpcklbw MM2, MM7
        punpckhbw MM3, MM7

        psubw     MM0, MM2
        psubw     MM1, MM3
        pmaddwd   MM0, MM0
        pmaddwd   MM1, MM1
        paddd     MM0, MM1

	movq	  MM1, [eax+8]
	movq	  MM2, MM1
	punpcklbw MM1, MM7
	punpckhbw MM2, MM7
	movq	  MM3, [eax+9]
	movq	  MM4, MM3
	punpcklbw MM3, MM7
	punpckhbw MM4, MM7
	paddw	  MM1, MM3
	paddw	  MM2, MM4
	movq	  MM3, [ebx+8]
	movq	  MM4, MM3
	punpcklbw MM3, MM7
	punpckhbw MM4, MM7
	movq	  MM5, [ebx+9]
	movq	  MM6, MM5
	punpcklbw MM5, MM7
	punpckhbw MM6, MM7
	paddw	  MM3, MM5
	paddw	  MM4, MM6
	paddw	  MM1, MM3
	paddw	  MM2, MM4
	pxor	  MM6, MM6
	pcmpeqw	  MM5, MM5
	psubw	  MM6, MM5
	paddw	  MM6, MM6
	paddw	  MM1, MM6
	paddw	  MM2, MM6
	psrlw	  MM1, 2
	psrlw	  MM2, 2

	movq	  MM3, [edx+8]
	movq	  MM4, MM3
        punpcklbw MM3, MM7
        punpckhbw MM4, MM7

        psubw     MM1, MM3
        psubw     MM2, MM4
        pmaddwd   MM1, MM1
        pmaddwd   MM2, MM2
        paddd     MM1, MM2

	paddd	  MM0, MM1
	movd	  ecx, MM0
        add       s, ecx
	psrlq	  MM0, 32
	movd	  ecx, MM0
	add	  s, ecx

	mov       eax, ebx
	add       ebx, esi
	add       edx, esi
	dec       edi
	jg        d2top11
        jmp       d2exit

d2exit1:
        mov       s, edx

d2exit:
        emms
  }
  return s;
}

/*
 * absolute difference error between a (16*h) block and a bidirectional
 * prediction
 *
 * p2: address of top left pel of block
 * pf,hxf,hyf: address and half pel flags of forward ref. block
 * pb,hxb,hyb: address and half pel flags of backward ref. block
 * h: height of block
 * lx: distance (in bytes) of vertically adjacent pels in p2,pf,pb
 * MMX version
 */

int bdist1mmx(
unsigned char *pf, unsigned char *pb, unsigned char *p2,
int lx, int hxf, int hyf, int hxb, int hyb, int h)
{
  unsigned char *pfa,*pfb,*pfc,*pba,*pbb,*pbc;
  int s;

  _asm {
	mov       edx, hxb
	mov       eax, hxf
	mov       esi, lx

	mov       ecx, pf
	add       ecx, eax
	mov       pfa, ecx
	mov       ecx, esi
	imul      ecx, hyf
	mov       ebx, pf
	add       ecx, ebx
	mov       pfb, ecx
	add       eax, ecx
	mov       pfc, eax
	mov       eax, pb
	add       eax, edx
	mov       pba, eax
	mov       eax, esi
	imul      eax, hyb
	mov       ecx, pb
	add       eax, ecx
	mov       pbb, eax
	add       edx, eax
	mov       pbc, edx
	xor       esi, esi
	mov       s, esi

	mov       edi, h
	test      edi, edi  // h = 0?
	jle       bdist1exit

	pxor	  MM7, MM7
	pxor	  MM6, MM6
	pcmpeqw	  MM5, MM5
	psubw	  MM6, MM5
	psllw	  MM6, 1

bdist1top:
	mov	  eax, pf
	mov	  ebx, pfa
	mov	  ecx, pfb
	mov	  edx, pfc
	movq	  MM0, [eax]
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	movq	  MM2, [ebx]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	movq	  MM2, [ecx]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	movq	  MM2, [edx]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psrlw	  MM0, 2
	psrlw	  MM1, 2
	mov	  eax, pb
	mov	  ebx, pba
	mov	  ecx, pbb
	mov	  edx, pbc
	movq	  MM2, [eax]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	movq	  MM4, [ebx]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	movq	  MM4, [ecx]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	movq	  MM4, [edx]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	paddw	  MM2, MM6
	paddw	  MM3, MM6
	psrlw	  MM2, 2
	psrlw	  MM3, 2
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	psrlw	  MM6, 1
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psllw	  MM6, 1
	psrlw	  MM0, 1
	psrlw	  MM1, 1
	packuswb  MM0, MM1

	mov	  eax, p2
	movq	  MM1, [eax]
	movq	  MM2, MM0
	psubusb	  MM0, MM1
	psubusb	  MM1, MM2
	por	  MM0, MM1
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	paddw	  MM0, MM1
	movq	  MM1, MM0
	punpcklwd MM0, MM7
	punpckhwd MM1, MM7

	paddd	  MM0, MM1
	movd	  eax, MM0
	psrlq	  MM0, 32
	movd	  ebx, MM0
	add	  esi, eax
	add	  esi, ebx
	mov	  eax, pf
	mov	  ebx, pfa
	mov	  ecx, pfb
	mov	  edx, pfc
	movq	  MM0, [eax+8]
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	movq	  MM2, [ebx+8]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	movq	  MM2, [ecx+8]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	movq	  MM2, [edx+8]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psrlw	  MM0, 2
	psrlw	  MM1, 2
	mov	  eax, pb
	mov	  ebx, pba
	mov	  ecx, pbb
	mov	  edx, pbc
	movq	  MM2, [eax+8]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	movq	  MM4, [ebx+8]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	movq	  MM4, [ecx+8]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	movq	  MM4, [edx+8]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	paddw	  MM2, MM6
	paddw	  MM3, MM6
	psrlw	  MM2, 2
	psrlw	  MM3, 2
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	psrlw	  MM6, 1
	paddW	  MM0, MM6
	paddw	  MM1, MM6
	psllw	  MM6, 1
	psrlw	  MM0, 1
	psrlw	  MM1, 1
	packuswb  MM0, MM1
	mov	  eax, p2
	movq	  MM1, [eax+8]
	movq	  MM2, MM0
	psubusb	  MM0, MM1
	psubusb	  MM1, MM2
	por	  MM0, MM1
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	paddw	  MM0, MM1
	movq	  MM1, MM0
	punpcklwd MM0, MM7
	punpckhwd MM1, MM7
	paddd	  MM0, MM1
	movd	  eax, MM0
	psrlq	  MM0, 32
	movd	  ebx, MM0
	add	  esi, eax
	add	  esi, ebx

        mov       eax, lx
	add       p2, eax
	add       pf, eax
	add       pfa, eax
	add       pfb, eax
	add       pfc, eax
	add       pb, eax
	add       pba, eax
	add       pbb, eax
	add       pbc, eax

	dec       edi
	jg        bdist1top
        mov       s, esi

bdist1exit:
        emms
  }
  return s;
}

/* SSE version */

int bdist1sse(
unsigned char *pf, unsigned char *pb, unsigned char *p2,
int lx, int hxf, int hyf, int hxb, int hyb, int h)
{
  unsigned char *pfa,*pfb,*pfc,*pba,*pbb,*pbc;
  int s;

  _asm {
	mov       edx, hxb
	mov       eax, hxf
	mov       esi, lx

	mov       ecx, pf
	add       ecx, eax
	mov       pfa, ecx
	mov       ecx, esi
	imul      ecx, hyf
	mov       ebx, pf
	add       ecx, ebx
	mov       pfb, ecx
	add       eax, ecx
	mov       pfc, eax
	mov       eax, pb
	add       eax, edx
	mov       pba, eax
	mov       eax, esi
	imul      eax, hyb
	mov       ecx, pb
	add       eax, ecx
	mov       pbb, eax
	add       edx, eax
	mov       pbc, edx
	xor       esi, esi
	mov       s, esi

	mov       edi, h
	test      edi, edi  // h = 0?
	jle       bd1exitsse

	pxor	  MM7, MM7
	pxor	  MM6, MM6
	pcmpeqw	  MM5, MM5
	psubw	  MM6, MM5
	psllw	  MM6, 1

bd1topsse:
	mov	  eax, pf
	mov	  ebx, pfa
	mov	  ecx, pfb
	mov	  edx, pfc
	movq	  MM0, [eax]
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	movq	  MM2, [ebx]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	movq	  MM2, [ecx]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	movq	  MM2, [edx]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psrlw	  MM0, 2
	psrlw	  MM1, 2
	mov	  eax, pb
	mov	  ebx, pba
	mov	  ecx, pbb
	mov	  edx, pbc
	movq	  MM2, [eax]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	movq	  MM4, [ebx]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	movq	  MM4, [ecx]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	movq	  MM4, [edx]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	paddw	  MM2, MM6
	paddw	  MM3, MM6
	psrlw	  MM2, 2
	psrlw	  MM3, 2
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	psrlw	  MM6, 1
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psllw	  MM6, 1
	psrlw	  MM0, 1
	psrlw	  MM1, 1
	packuswb  MM0, MM1

	mov	  ebx, p2
        EMIT 15
        EMIT 246
        EMIT 3      // 0x0ff603 = psadbw MM0, [ebx]

	movd	  eax, MM0
	add	  esi, eax
	mov	  eax, pf
	mov	  ebx, pfa
	mov	  ecx, pfb
	mov	  edx, pfc
	movq	  MM0, [eax+8]
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	movq	  MM2, [ebx+8]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	movq	  MM2, [ecx+8]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	movq	  MM2, [edx+8]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psrlw	  MM0, 2
	psrlw	  MM1, 2
	mov	  eax, pb
	mov	  ebx, pba
	mov	  ecx, pbb
	mov	  edx, pbc
	movq	  MM2, [eax+8]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	movq	  MM4, [ebx+8]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	movq	  MM4, [ecx+8]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	movq	  MM4, [edx+8]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	paddw	  MM2, MM6
	paddw	  MM3, MM6
	psrlw	  MM2, 2
	psrlw	  MM3, 2
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	psrlw	  MM6, 1
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psllw	  MM6, 1
	psrlw	  MM0, 1
	psrlw	  MM1, 1
	packuswb  MM0, MM1

	mov	  ebx, p2
        EMIT 15
        EMIT 246
        EMIT 67
        EMIT 8      // 0x0ff64308 = psadbw MM0, [ebx+8]

	movd	  eax, MM0
	add	  esi, eax

        mov       eax, lx
	add       p2, eax
	add       pf, eax
	add       pfa, eax
	add       pfb, eax
	add       pfc, eax
	add       pb, eax
	add       pba, eax
	add       pbb, eax
	add       pbc, eax

	dec       edi
	jg        bd1topsse
        mov       s, esi

bd1exitsse:
        emms
  }
  return s;
}

/*
 * squared error between a (16*h) block and a bidirectional
 * prediction
 *
 * p2: address of top left pel of block
 * pf,hxf,hyf: address and half pel flags of forward ref. block
 * pb,hxb,hyb: address and half pel flags of backward ref. block
 * h: height of block
 * lx: distance (in bytes) of vertically adjacent pels in p2,pf,pb
 * MMX version
 */

int bdist2mmx(
unsigned char *pf, unsigned char *pb, unsigned char *p2,
int lx, int hxf, int hyf, int hxb, int hyb, int h)
{
  unsigned char *pfa,*pfb,*pfc,*pba,*pbb,*pbc;
  int s;

  _asm {
	mov       edx, hxb
	mov       eax, hxf
	mov       esi, lx

	mov       ecx, pf
	add       ecx, eax
	mov       pfa, ecx
	mov       ecx, esi
	imul      ecx, hyf
	mov       ebx, pf
	add       ecx, ebx
	mov       pfb, ecx
	add       eax, ecx
	mov       pfc, eax
	mov       eax, pb
	add       eax, edx
	mov       pba, eax
	mov       eax, esi
	imul      eax, hyb
	mov       ecx, pb
	add       eax, ecx
	mov       pbb, eax
	add       edx, eax
	mov       pbc, edx
	xor       esi, esi
	mov       s, esi

	mov       edi, h
	test      edi, edi  // h = 0?
	jle       bdist2exit

	pxor	  MM7, MM7
	pxor	  MM6, MM6
	pcmpeqw	  MM5, MM5
	psubw	  MM6, MM5
	psllw	  MM6, 1

bdist2top:
	mov	  eax, pf
	mov	  ebx, pfa
	mov	  ecx, pfb
	mov	  edx, pfc
	movq	  MM0, [eax]
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	movq	  MM2, [ebx]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	movq	  MM2, [ecx]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	movq	  MM2, [edx]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psrlw	  MM0, 2
	psrlw	  MM1, 2

	mov	  eax, pb
	mov	  ebx, pba
	mov	  ecx, pbb
	mov	  edx, pbc
	movq	  MM2, [eax]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	movq	  MM4, [ebx]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	movq	  MM4, [ecx]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	movq	  MM4, [edx]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5

	paddw	  MM2, MM6
	paddw	  MM3, MM6
	psrlw	  MM2, 2
	psrlw	  MM3, 2

	paddw	  MM0, MM2
	paddw	  MM1, MM3
	psrlw	  MM6, 1
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psllw	  MM6, 1
	psrlw	  MM0, 1
	psrlw	  MM1, 1

	mov	  eax, p2
	movq	  MM2, [eax]
	movq	  MM3, MM2
        punpcklbw MM2, MM7
        punpckhbw MM3, MM7

        psubw     MM0, MM2
        psubw     MM1, MM3
        pmaddwd   MM0, MM0
        pmaddwd   MM1, MM1
        paddd     MM0, MM1

	movd	  eax, MM0
	psrlq	  MM0, 32
	movd	  ebx, MM0
	add	  esi, eax
	add	  esi, ebx

	mov	  eax, pf
	mov	  ebx, pfa
	mov	  ecx, pfb
	mov	  edx, pfc
	movq	  MM0, [eax+8]
	movq	  MM1, MM0
	punpcklbw MM0, MM7
	punpckhbw MM1, MM7
	movq	  MM2, [ebx+8]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	movq	  MM2, [ecx+8]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	movq	  MM2, [edx+8]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	paddw	  MM0, MM2
	paddw	  MM1, MM3
	paddw	  MM0, MM6
	paddw	  MM1, MM6
	psrlw	  MM0, 2
	psrlw	  MM1, 2

	mov	  eax, pb
	mov	  ebx, pba
	mov	  ecx, pbb
	mov	  edx, pbc
	movq	  MM2, [eax+8]
	movq	  MM3, MM2
	punpcklbw MM2, MM7
	punpckhbw MM3, MM7
	movq	  MM4, [ebx+8]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	movq	  MM4, [ecx+8]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	movq	  MM4, [edx+8]
	movq	  MM5, MM4
	punpcklbw MM4, MM7
	punpckhbw MM5, MM7
	paddw	  MM2, MM4
	paddw	  MM3, MM5
	paddw	  MM2, MM6
	paddw	  MM3, MM6
	psrlw	  MM2, 2
	psrlw	  MM3, 2

	paddw	  MM0, MM2
	paddw	  MM1, MM3
	psrlw	  MM6, 1
	paddW	  MM0, MM6
	paddw	  MM1, MM6
	psllw	  MM6, 1
	psrlw	  MM0, 1
	psrlw	  MM1, 1

	mov	  eax, p2
	movq	  MM2, [eax+8]
	movq	  MM3, MM2
        punpcklbw MM2, MM7
        punpckhbw MM3, MM7

        psubw     MM0, MM2
        psubw     MM1, MM3
        pmaddwd   MM0, MM0
        pmaddwd   MM1, MM1
        paddd     MM0, MM1

	movd	  eax, MM0
	psrlq	  MM0, 32
	movd	  ebx, MM0
	add	  esi, eax
	add	  esi, ebx

        mov       eax, lx
	add       p2, eax
	add       pf, eax
	add       pfa, eax
	add       pfb, eax
	add       pfc, eax
	add       pb, eax
	add       pba, eax
	add       pbb, eax
	add       pbc, eax

	dec       edi
	jg        bdist2top
        mov       s, esi

bdist2exit:
        emms
  }
  return s;
}

/*
 * variance of a (16*16) block, multiplied by 256
 * p:  address of top left pel of block
 * lx: distance (in bytes) of vertically adjacent pels
 * MMX version
 */

int variancemmx(
unsigned char *p,
int lx)
{
  unsigned int s2 = 0;

  _asm {
	mov       eax,p
        mov       edi,lx
	xor       ebx,ebx                // s2
	mov       ecx,ebx                // s
	mov       esi,16                 // loop 16

        pxor      MM7, MM7               // get zeros in MM7

vartop:
        movq      MM0, [eax]
        movq      MM2, [eax+8]

        movq      MM1, MM0
        punpcklbw MM0, MM7
        punpckhbw MM1, MM7

        movq      MM3, MM2
        punpcklbw MM2, MM7
        punpckhbw MM3, MM7

        movq      MM5, MM0
        paddusw   MM5, MM1
        paddusw   MM5, MM2
        paddusw   MM5, MM3

        movq      MM6, MM5
        punpcklwd MM5, MM7
        punpckhwd MM6, MM7
        paddd     MM5, MM6 // MM5 = two 32 bit sums

        pmaddwd   MM0, MM0
        pmaddwd   MM1, MM1
        pmaddwd   MM2, MM2
        pmaddwd   MM3, MM3

        paddd     MM0, MM1
        paddd     MM0, MM2
        paddd     MM0, MM3

        movd      edx, MM5
        add       ecx, edx
        psrlq     MM5, 32
        movd      edx, MM5
        add       ecx, edx
        movd      edx, MM0
        add       ebx, edx
        psrlq     MM0, 32
        movd      edx, MM0
        add       ebx, edx

        add       eax, edi
	dec       esi
	jg        vartop

	imul      ecx, ecx
	shr       ecx, 8
	sub       ebx,ecx
	mov       s2, ebx
        emms
      }

  return s2;
}

/* subtract prediction from block data */
void sub_pred_mmx(unsigned char *pred,
                  unsigned char *cur,
                  int lx, short *blk)
{
  _asm {
        mov   eax, cur
        mov   ebx, pred
        mov   ecx, blk
        mov   edi, lx
        mov   esi, 8
        pxor  MM7, MM7
     sub_top:
        movq  MM0, [eax]
        add   eax, edi
        movq  MM2, [ebx]
        add   ebx, edi
        movq  MM1, MM0
        punpcklbw MM0, MM7
        punpckhbw MM1, MM7
        movq  MM3, MM2
        punpcklbw MM2, MM7
        punpckhbw MM3, MM7

        psubw  MM0, MM2
        psubw  MM1, MM3

        movq   [ecx], MM0
        movq   [ecx+8], MM1
        add    ecx, 16

        dec    esi
        jg     sub_top

        emms
      }
}

/* add prediction and prediction error, saturate to 0...255 */
void add_pred_mmx(unsigned char *pred,
                  unsigned char *cur,
                  int lx, short *blk)
{
  _asm {
        mov   eax, cur
        mov   ebx, pred
        mov   ecx, blk
        mov   edi, lx
        mov   esi, 8
        pxor  MM7, MM7
     add_top:
        movq  MM0, [ecx]
        movq  MM1, [ecx+8]
        add   ecx, 16
        movq  MM2, [ebx]
        add   ebx, edi
        movq  MM3, MM2
        punpcklbw MM2, MM7
        punpckhbw MM3, MM7

        paddw  MM0, MM2
        paddw  MM1, MM3
        packuswb MM0, MM1

        movq   [eax], MM0
        add    eax, edi

        dec    esi
        jg     add_top

        emms
      }
}


int edist1mmx(
unsigned char *blk1, unsigned char *blk2,
int lx, int distlim)
{
  int s = 0;
  _asm {
	mov       edi, distlim
	mov       esi, lx

        pxor      MM7, MM7    // get zeros in MM7

        xor       edx, edx     // sum
	mov       eax, blk1
	mov       ebx, blk2

        // first line
	movd	  MM0, [eax]
	movd	  MM1, [ebx]
	movq	  MM2, MM0
	psubusb	  MM0, MM1
	psubusb	  MM1, MM2
	por	  MM0, MM1
	punpcklbw MM0, MM7
	movq	  MM1, MM0
	punpcklwd MM0, MM7
	punpckhwd MM1, MM7
	paddd	  MM0, MM1
	movd	  ecx, MM0
        add       edx, ecx
	psrlq	  MM0, 32
	movd	  ecx, MM0
        add       edx, ecx

	cmp       edx, edi
	jge       e1exit
	add       eax, esi
	add       ebx, esi

        // second line
	movd	  MM0, [eax]
	movd	  MM1, [ebx]
	movq	  MM2, MM0
	psubusb	  MM0, MM1
	psubusb	  MM1, MM2
	por	  MM0, MM1
	punpcklbw MM0, MM7
	movq	  MM1, MM0
	punpcklwd MM0, MM7
	punpckhwd MM1, MM7
	paddd	  MM0, MM1
	movd	  ecx, MM0
        add       edx, ecx
	psrlq	  MM0, 32
	movd	  ecx, MM0
        add       edx, ecx

	cmp       edx, edi
	jge       e1exit
	add       eax, esi
	add       ebx, esi

        // third line
	movd	  MM0, [eax]
	movd	  MM1, [ebx]
	movq	  MM2, MM0
	psubusb	  MM0, MM1
	psubusb	  MM1, MM2
	por	  MM0, MM1
	punpcklbw MM0, MM7
	movq	  MM1, MM0
	punpcklwd MM0, MM7
	punpckhwd MM1, MM7
	paddd	  MM0, MM1
	movd	  ecx, MM0
        add       edx, ecx
	psrlq	  MM0, 32
	movd	  ecx, MM0
        add       edx, ecx

	cmp       edx, edi
	jge       e1exit
	add       eax, esi
	add       ebx, esi

        // fourth line
	movd	  MM0, [eax]
	movd	  MM1, [ebx]
	movq	  MM2, MM0
	psubusb	  MM0, MM1
	psubusb	  MM1, MM2
	por	  MM0, MM1
	punpcklbw MM0, MM7
	movq	  MM1, MM0
	punpcklwd MM0, MM7
	punpckhwd MM1, MM7
	paddd	  MM0, MM1
	movd	  ecx, MM0
        add       edx, ecx
	psrlq	  MM0, 32
	movd	  ecx, MM0
        add       edx, ecx

e1exit:
        mov       s, edx
        emms
  }
  return s;
}


/* SSE version */

int edist1sse(
unsigned char *blk1, unsigned char *blk2,
int lx, int distlim)
{
  int s = 0;
  _asm {
	mov       edi, distlim
	mov       esi, lx

        pxor      MM7, MM7    // get zeros in MM7

        xor       edx, edx     // sum
	mov       eax, blk1
	mov       ebx, blk2

        // row 1
	movd	  MM0, [eax]
        movd      MM1, [ebx]
        EMIT 0x0f
        EMIT 0xf6
        EMIT 0xc1   // 0x0ff6c1 = psadbw MM0, MM1
	movd	  ecx, MM0
        add       edx, ecx

	cmp       edx, edi
	jge       e1exitsse
	add       eax, esi
	add       ebx, esi

        // row 2
	movd	  MM0, [eax]
        movd      MM1, [ebx]
        EMIT 0x0f
        EMIT 0xf6
        EMIT 0xc1   // 0x0ff6c1 = psadbw MM0, MM1
	movd	  ecx, MM0
        add       edx, ecx

	cmp       edx, edi
	jge       e1exitsse
	add       eax, esi
	add       ebx, esi

        // row 3
	movd	  MM0, [eax]
        movd      MM1, [ebx]
        EMIT 0x0f
        EMIT 0xf6
        EMIT 0xc1   // 0x0ff6c1 = psadbw MM0, MM1
	movd	  ecx, MM0
        add       edx, ecx

	cmp       edx, edi
	jge       e1exitsse
	add       eax, esi
	add       ebx, esi

        // row 4
	movd	  MM0, [eax]
        movd      MM1, [ebx]
        EMIT 0x0f
        EMIT 0xf6
        EMIT 0xc1   // 0x0ff6c1 = psadbw MM0, MM1
	movd	  ecx, MM0
        add       edx, ecx

e1exitsse:
        mov       s, edx
        emms
  }
  return s;
}
