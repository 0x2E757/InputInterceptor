using System;
using System.Runtime.InteropServices;

namespace InputInterceptorNS {

    internal class NativeMethods {

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr LoadLibrary(String lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, String procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Boolean FreeLibrary(IntPtr hModule);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Boolean GetCursorPos([Out] Win32Point lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Boolean SetCursorPos(Int32 x, Int32 y);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Int32 GetSystemMetrics(Int32 nIndex);

    }

}
