using System;
using System.Runtime.InteropServices;

namespace InputInterceptorNS {

    [StructLayout(LayoutKind.Sequential)]
    public struct MouseStroke {

        public MouseState State;
        public MouseFlags Flags;

        public Int16 Rolling;

        public Int32 X;
        public Int32 Y;

        public UInt32 Information;

    }

}
