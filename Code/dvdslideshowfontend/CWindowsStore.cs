using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Windows.Services.Store;

//
// For this class to access uwp classes you need to include these two assemblies
// C:\Program Files(x86)\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll
// C\Program Files (x86)\Windows Kits\10\UnionMetadata\Facade\Windows.WinMD
// These show up as Windows and System.Runtime\WindowsRuntime in the reference assemblies.  Ans will only 
// work for Windows 10
// 
namespace dvdslideshowfontend
{
    [ComImport]
    [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInitializeWithWindow
    {
        void Initialize(IntPtr hwnd);
    }

    public class CWindowsStore 
    {
        public void Initialise()
        {
            StoreContext context = StoreContext.GetDefault();
            IInitializeWithWindow initWindow = (IInitializeWithWindow)(object)context;
            initWindow.Initialize(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle);
        }

        public bool IsTrial()
        {
            Task<bool> task = Task.Run<bool>(async () => await GetTrialStatusAsync());
            return task.Result;
        }

        private async Task<bool> GetTrialStatusAsync()
        {
            StoreContext context = StoreContext.GetDefault();
            StoreAppLicense appLicense = await context.GetAppLicenseAsync();
            return appLicense.IsTrial;
        }
    }
}
