
#ifndef INPUT_PHOTOVIDSHOW_INCLUDE
#define INPUT_PHOTOVIDSHOW_INCLUDE

typedef int(*getVideoFptr)(unsigned int frameNum, unsigned int *bytesInRow, char **buffer);

void photovidshow_set_input_parameters( getVideoFptr video, int width, int height, int frames);


#endif
