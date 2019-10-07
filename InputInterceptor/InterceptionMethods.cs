using System;
using System.Runtime.InteropServices;

using Context = System.IntPtr;
using Device = System.Int32;
using Filter = System.UInt16;
using Precedence = System.Int32;

namespace InputInterceptorNS {

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate Boolean Predicate(Device device);

    internal class InterceptionMethods {

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Context CreateContext();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DestroyContext(Context context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Precedence GetPrecedence(Context context, Device device);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetPrecedence(Context context, Device device, Precedence precedence);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Filter GetFilter(Context context, Device device);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFilter(Context context, Predicate interception_predicate, Filter filter);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Device Wait(Context context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Device WaitWithTimeout(Context context, UInt64 milliseconds);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Int32 Send(Context context, Device device, ref Stroke stroke, UInt32 nstroke);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Int32 Receive(Context context, Device device, ref Stroke stroke, UInt32 nstroke);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UInt32 GetHardwareId(Context context, Device device, IntPtr hardware_id_buffer, UInt32 buffer_size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Int32 IsInvalid(Device device);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Int32 IsKeyboard(Device device);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Int32 IsMouse(Device device);

    }

}
