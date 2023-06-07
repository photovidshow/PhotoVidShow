This is the complete source code to build the PhotoVidShow software.

Firstly, the App can be built with Visual Studio community (2022).  The solution file can be located in Code/dvdslideshow/dvdslideshow.sln.  (This name dates back to 2004 when this was a simple photos to dvd prototype app).  Once loaded into Visual studio, set the 'PhotoVidShow' project as your startup project.

So, from the outset can I mention that this code was never intended to become available to anyone outside the original developers.  However, for backup reasons, as well as for allowing other developers to modify or add things, the code is now available here.  I am personally no longer actively working on this project.  Some of the code here dates back to 2004 with things then being added and removed over the years.  Expect some legacy code in places!

The code dump is of 4.5.3 of the software (2018 final released version).  However the c# assembly version numbers have been set to 5.  There was going to be a version 5 of the software but that got abandoned because of slow progress and also due to my other commitments.  None of that code is here.

PhotoVidShow is x86 (windows) only.  You can build as debug or release.  x64 windows builds were planed but never happened.

Large parts of this app was coded by myself but also large parts were coded by other people included open source projects.

This app was written in c#, c++ and even some x86 assembly can be found in some of the video encoders.  To make things easier, I pre-assembled the x86 assembly parts into .o or .obj files which get linked into the c++ libraries as dependencies.  The original x86 assembly files are included though not needed in the build process.

PhotoVidShow uses DirectX9 (including DirectShow), Starnburn and LAVFilters.  I simply provide a cut-down version of their SDKs here in this repository with required .lib,. dll, .ax and .h files.

The build order/dependencies does not completely work. This may of been intentional because visual studio kept rebuilding projects it didn't need too.

In short, the code base can be thought of as being in two layers.  The lower layer is c++ whilst the upper layer is in c#.  The c++ eventually gets linked into one big .net assembly which can be used by the c# layer. 

So to build PhotoVidShow, compile projects in this order:-


-Managed core  (used by all layers)

-All the C++ projects except the ManagedToUnManagedWrapper one (Order not important).

-Then compile ManagedToUnManagedWrapper which then creates the managed/clr assembly.


Then build for layer 2...

-dvdslideshow

-Balloon window / color combo/ Font combo and the two treeViewFolder ones.

-CustomButton

-dvdslideshowfontend

-PhotoVidshow (which makes the final .exe file)


The PhotCruz app can now also be built which was a separate app but uses the same libraries as PhotoVidShow.

If Visual Studio errors with “couldn't process file resx due to it being in the Internet or Restricted zone”,  then you need to find the .resx files on your PC, right click on them, select properties, then select “unblock”.  Just do a google search if you get stuck.

There is also a custom build event after building the PhotoVidShow.exe which sets the LARGEADDRESSAWARE flag inside the .exe!   This is a windows things which allows apps(x86) to use memory addresses > 2gig and up to 4 gig.  This is needed as it was typical for users to add 1000s of photos and videos (1080p etc.).  So memory soon gets eaten up!  To allow users to import 4k videos, you really need a x64 build of the software, which like I said never really happened.

The general licence is MIT, however please remember there are some opensource code here which may use their own licence (this is shown in their .c/.h files)

Hope this is of help to someone.














 























