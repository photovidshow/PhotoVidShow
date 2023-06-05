
#ifndef MANAGED_OBJECT_INCLUDE
#define MANAGED_OBJECT_INCLUDE


#using <mscorlib.dll>
using namespace System;
using namespace System::Text;
using namespace System::Runtime::InteropServices; 
using namespace ManagedCore;
using namespace std;



namespace MangedToUnManagedWrapper
{ 
	public ref class ManagedObject
	{
		private:
			static System::Collections::ArrayList^ mReference = gcnew System::Collections::ArrayList(); 

		bool mAllowedToBeGC ;

		public:

		ManagedObject()
		{
			mAllowedToBeGC=false;
			mReference->Add(this);
		}

		static void Clear()
		{
			for (int i=0;i< mReference->Count;i++)
			{
				static_cast<ManagedObject^>(mReference[i])->mAllowedToBeGC=true;
			}

			mReference->Clear();
		}

		void Release()
		{
			mAllowedToBeGC=true;
			mReference->Remove(this);
		}

	};
}


#endif





