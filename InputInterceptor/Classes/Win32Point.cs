using System;
using System.Runtime.InteropServices;

namespace InputInterceptorNS
{

    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Point
    {

        public Int32 X;
        public Int32 Y;

        public Win32Point(Int32 x, Int32 y)
        {
            this.X = x;
            this.Y = y;
        }

    }

}
