using System;
using System.Runtime.InteropServices;

namespace InputInterceptorNS
{

    internal class NativeMethods
    {

        private const String KERNEL32 = "kernel32.dll";
        private const String USER32 = "user32.dll";

        [DllImport(KERNEL32, SetLastError = true)]
        public static extern IntPtr LoadLibrary(String lpLibFileName);

        [DllImport(KERNEL32, SetLastError = true)]
        public static extern Boolean FreeLibrary(IntPtr hLibModule);

        [DllImport(KERNEL32, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(String lpModuleName);

        [DllImport(KERNEL32, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, String lpProcName);

        [DllImport(KERNEL32, SetLastError = true)]
        public static extern IntPtr GetCurrentProcess();

        [DllImport(KERNEL32, SetLastError = true)]
        public static extern Boolean IsWow64Process(IntPtr hProcess, out Boolean Wow64Process);

        [DllImport(USER32, SetLastError = true)]
        public static extern Boolean GetCursorPos(out Win32Point lpPoint);

        [DllImport(USER32, SetLastError = true)]
        public static extern Boolean SetCursorPos(Int32 x, Int32 y);

        [DllImport(USER32, SetLastError = true)]
        public static extern Int32 GetSystemMetrics(Int32 nIndex);

    }

}
