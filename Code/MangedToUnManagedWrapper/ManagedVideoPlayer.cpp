

#include "VideoPlayer.h"
#include "ManagedErrors.h"
#include <iostream>
#include <string>
using namespace std; 
#using <mscorlib.dll>
#using <System.Drawing.dll>


#include <tchar.h>
#include <vcclr.h>

using namespace System;
using namespace Drawing;
using namespace Imaging;
using namespace ManagedCore;



using namespace System::Text;
using namespace System::Runtime::InteropServices; 

struct PixelData
{
    unsigned char blue;
    unsigned char green;
    unsigned char red;
};

std::string ManagedToSTL(System::String^ managed) {
    //get a pointer to an array of ANSI chars
    char *chars = (char*) Marshal::StringToHGlobalAnsi(managed).ToPointer(); 

    //assign the array to an STL string
    std::string stl = chars; 

    //free the memory used by the array
    //since the array is not managed, it will not be claimed by the garbage collector
    Marshal::FreeHGlobal( (System::IntPtr)chars); 

    return stl;
} 


//Converts a std::string to a System::String
//This code assumes that you have used the following namespace:
// using namespace System::Runtime::InteropServices
System::String^ STLToManaged(std::string stl) {
    //the c_str() function gets a char array from the STL string,
    //but the PtrToStringAnsi function wants a int array, so it gets casted
    return Marshal::PtrToStringAnsi((System::IntPtr) (int*)(stl.c_str()));
} 




#include <list>

std::list<CDirectShowPlayer*> mPlayers ;


namespace MangedToUnManagedWrapper
{
	
	
	public ref class CVideoInfo
	{
	public:
		int	mWidth;
		int mHeight;
		int mBPP;
		int mPlanes;
		double mFrameRate;
		Int64 mDuration;
		bool mHasAudio;
		bool mHasAC3Sound;
		bool mLoaded;
		bool mFFDShowDisallowedThisApp;
	};


	public ref class VideoPlayer
	{
	public:
	
		static CDirectShowPlayer* GetPlayerFromID(int id)
		{
			std::list<CDirectShowPlayer*>::iterator it;

			if (mPlayers.size()==0)
			{
				return NULL;
			}

			for (it = mPlayers.begin(); it != mPlayers.end(); it++ )
			{
				if ((*it)->mID == id) return (*it);
			}

			return NULL;
		}

        static int GetEstimatedMemoryFootprintOnLoadedPlayers()
        {
            int footPrint =0;
            std::list<CDirectShowPlayer*>::iterator it;

			if (mPlayers.size()==0)
			{
				return footPrint;
			}

			for (it = mPlayers.begin(); it != mPlayers.end(); it++ )
			{
                int size = (*it)->GetWidth() * (*it)->GetHeight();

                //
                // Min 720p
                //
                if (size < 921600)
                {
                    size = 921600;
                }

                footPrint+=size;
			}

			return footPrint;
        }

		static CDirectShowPlayer* AddPlayer(int id)
		{
			CDirectShowPlayer* p =  new CDirectShowPlayer(id) ;
			mPlayers.push_back(p);
			return p;
		}

		// This is the entry point for this application
		static CVideoInfo^ Load(String^ s, int id, bool useffdshowformp4, bool useffdshowformov, bool useffdshowformts, bool quarter1080pVideos)
		{
			std::string s1 = ManagedToSTL(s);
 
			CVideoInfo^ ii = gcnew CVideoInfo;
			ii->mLoaded = false;

			CDirectShowPlayer* player = VideoPlayer::GetPlayerFromID(id);
			if (player==NULL) player = AddPlayer(id);

			char* aa = (char*)s1.c_str();

			int attempts =0;

			bool loaded_or_cant = false;

			while (loaded_or_cant==false && attempts<20)
			{
				try
				{
					int result = player->Load(aa, useffdshowformp4, useffdshowformov, useffdshowformts, quarter1080pVideos) ;
					if (result!=0)
					{
						CDebugLog::GetInstance()->Error("Failed to load video");

						if (result==2)
						{
							ii->mFFDShowDisallowedThisApp = true;
						}
					}
					else
					{
						ii->mLoaded = true;
						ii->mWidth = player->mWidth;
						ii->mHeight = player->mHeight;
						ii->mBPP = player->mBpp;
						ii->mPlanes = player->mPlanes;
						ii->mHasAC3Sound  = player->HasDolbyAC3Sound();
						ii->mFrameRate = player->GetFrameRate();
						ii->mHasAudio = player->mHasAudio;
						hyper d =0;
						player->GetDuration(&d);
						ii->mDuration = d;
					}
					loaded_or_cant=true;
				}
				catch(Exception^ e)
				{
					if (ManagedCore::CDebugLog::GetInstance()->DebugMode==true)
					{
						ManagedPopupError("CVideoPlayer::RenderFile crashed");
					}

                    String^ s1 = gcnew String(". CVideoPlayer::RenderFile probably crashed. ");          
                    String^ s2 = String::Concat(s, s1) ;
                    String^ s3 = String::Concat(s2, e->ToString());
                    ManagedError(s3);

					try
					{
						player->ReleaseAndNullAll();
					}
					catch(...)
					{
						if (ManagedCore::CDebugLog::GetInstance()->DebugMode==true)
						{
						  ManagedPopupError("CVideoPlayer::ReleaseAndNullAll crashed");
						}

						ManagedWarning("CVideoPlayer::ReleaseAndNullAll crashed");
					}
				}
				attempts++;
			}

			if (player->mFrameRate < 0.01)
			{
				char temp[1024];
				sprintf(temp,"Video file '%s' was not loaded correctly. "
					"This may be because an appropriate decoder for this video format is not installed on this machine.",aa);

				ManagedPopupWarning(temp);

				float val = ManagedCore::UserSettings::GetFloatValue("VideoSettings", "FramesPerSecondForUnknownVideos");
				if (val!=0)
				{
					player->mFrameRate = 1 / val;
				}
				else
				{
					player->mFrameRate = 1 / 30.0f;
				}
			}

			return ii;
		}
  

		

		static void Shutdownplayer(int id)
		{
		//	Trace("Called showdown player");
			CDirectShowPlayer* player = GetPlayerFromID(id);
			if (player!=NULL)
			{
				mPlayers.remove(player);
				player->Stop(0,true,false);
				delete player ;
			}
		}

		static void SetFrameStepping(int id, bool value)
		{
			CDirectShowPlayer* player = GetPlayerFromID(id);
			if (player) player->SetFrameStepping(value);
		}

		static void Start(int id, bool mute_sound)
		{
			CDirectShowPlayer* player = GetPlayerFromID(id);
			if (player) player->Start(mute_sound);
		}

		static void SeekToTime(double time, bool wait_for_completion, int id)
		{
			CDirectShowPlayer* player = GetPlayerFromID(id);
			if (player) player->SeekToTime(time, wait_for_completion);
		}

        static double GetLastSeekTime(int id)
        {
            CDirectShowPlayer* player = GetPlayerFromID(id);
			if (player)
            {
                return player->GetLastSeekTime();
            }
            return 0;
        }

		static int IsStopped(int id)
		{
			bool val = false;
			CDirectShowPlayer* player = GetPlayerFromID(id);
			 if (player) val = player->IsStopped();
			 if (val==false) return 0;
			 return 1;
		}

		
		static void Stop(int id, double start_time, bool do_reset, bool do_pause)
		{
			CDirectShowPlayer* player = GetPlayerFromID(id);
			if (player) player->Stop(start_time,do_reset,do_pause);
		}

		static int ReachedEnd(int id)
		{
			bool reached_end = false;

			CDirectShowPlayer* player = GetPlayerFromID(id);
			if (player) reached_end = player->ReachedEnd();

			if (reached_end==true) return 1;
			return 0;
		}

		static double LastSampleTime(int id)
		{
			CDirectShowPlayer* player = GetPlayerFromID(id);
			if (player) return player->mLastSampleTime;
			return 0;
		}

		static void SetVolume(int id, long val)
		{
			CDirectShowPlayer* player = GetPlayerFromID(id);
			if (player) player->SetVolume(val);
		}


		static void AdvanceFrame(int id)
		{
			CDirectShowPlayer* player = GetPlayerFromID(id);
			if (player) player->AdvanceFrame();
		}

		static void CleanInternalBuffers(int id)
		{
			CDirectShowPlayer* player = GetPlayerFromID(id);
			if (player) player->CleanInternalBuffers();
		}


		//*******************************************************************
		static String^ GetFirstFilterDetails(int id)
		{
			CDirectShowPlayer* player = GetPlayerFromID(id);
			if (!player)
			{
				return gcnew String("");
			}
				
			return STLToManaged(player->GetFirstFilterDetails());
		}

		//*******************************************************************
		static String^ GetNextFilterDetails(int id)
		{
			CDirectShowPlayer* player = GetPlayerFromID(id);
			if (!player)
			{
				return gcnew String("");
			}

			return STLToManaged(player->GetNextFilterDetails());
		}


		//*******************************************************************
		static double GetCurrentVideoBuffer(int pBits, int pitch, unsigned int width, unsigned int height, int id, bool quartered)
		{
			static bool doing = false;

            double sampleTime = -1;

			if (doing == true)
			{
#ifdef FULL_VIDEO_TRACE
				Trace("already called higher!!!!");
#endif
				return sampleTime;
			}

			unsigned char* data = (unsigned char*) pBits;

			doing = true ;
			if (data==NULL)
			{
				ManagedError("pBits null in call to GetCurrentVideoBuffer");
				doing = false ;
				return sampleTime;
			}

			CDirectShowPlayer* player = GetPlayerFromID(id);
			if (player==NULL) 
			{
				ManagedError("player null in call to  GetCurrentVideoBuffer");
				doing = false ;
				return sampleTime;
			}

			BOOL quarteredB = TRUE;
			if (quartered == false)
			{
				quarteredB = FALSE;
			}
			player->GetCurrentVideoBuffer(data, pitch, width, height, &sampleTime, quarteredB);
			
			doing = false ;

            return sampleTime;
			
		}
	};
}


