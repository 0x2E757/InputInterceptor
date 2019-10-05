using System;
using System.Runtime.InteropServices;

namespace InputInterceptorNS {

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyStroke {

        public KeyCode Code;
        public KeyState State;

        public UInt32 Information;

    }

}
