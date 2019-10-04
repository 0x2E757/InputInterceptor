using System;
using System.IO;
using System.Runtime.InteropServices;

namespace InputInterceptorNS {

    internal class DllWrapper {

        private readonly String DllTempName;
        private readonly IntPtr DllPointer;

        public InterceptionMethods.CreateContext CreateContext;
        public InterceptionMethods.DestroyContext DestroyContext;
        public InterceptionMethods.GetPrecedence GetPrecedence;
        public InterceptionMethods.SetPrecedence SetPrecedence;
        public InterceptionMethods.GetFilter GetFilter;
        public InterceptionMethods.SetFilter SetFilter;
        public InterceptionMethods.Wait Wait;
        public InterceptionMethods.WaitWithTimeout WaitWithTimeout;
        public InterceptionMethods.Send Send;
        public InterceptionMethods.Receive Receive;
        public InterceptionMethods.GetHardwareId GetHardwareId;
        public InterceptionMethods.IsInvalid IsInvalid;
        public InterceptionMethods.IsKeyboard IsKeyboard;
        public InterceptionMethods.IsMouse IsMouse;

        public DllWrapper(Byte[] DllBytes) {
            this.DllTempName = Path.GetTempFileName();
            File.WriteAllBytes(this.DllTempName, DllBytes);
            this.DllPointer = NativeMethods.LoadLibrary(this.DllTempName);
            this.CreateContext = this.GetFunction<InterceptionMethods.CreateContext>("interception_create_context");
            this.DestroyContext = this.GetFunction<InterceptionMethods.DestroyContext>("interception_destroy_context");
            this.GetPrecedence = this.GetFunction<InterceptionMethods.GetPrecedence>("interception_get_precedence");
            this.SetPrecedence = this.GetFunction<InterceptionMethods.SetPrecedence>("interception_set_precedence");
            this.GetFilter = this.GetFunction<InterceptionMethods.GetFilter>("interception_get_filter");
            this.SetFilter = this.GetFunction<InterceptionMethods.SetFilter>("interception_set_filter");
            this.Wait = this.GetFunction<InterceptionMethods.Wait>("interception_wait");
            this.WaitWithTimeout = this.GetFunction<InterceptionMethods.WaitWithTimeout>("interception_wait_with_timeout");
            this.Send = this.GetFunction<InterceptionMethods.Send>("interception_send");
            this.Receive = this.GetFunction<InterceptionMethods.Receive>("interception_receive");
            this.GetHardwareId = this.GetFunction<InterceptionMethods.GetHardwareId>("interception_get_hardware_id");
            this.IsInvalid = this.GetFunction<InterceptionMethods.IsInvalid>("interception_is_invalid");
            this.IsKeyboard = this.GetFunction<InterceptionMethods.IsKeyboard>("interception_is_keyboard");
            this.IsMouse = this.GetFunction<InterceptionMethods.IsMouse>("interception_is_mouse");
        }

        public void Dispose() {
            NativeMethods.FreeLibrary(this.DllPointer);
            File.Delete(this.DllTempName);
        }

        private TDelegate GetFunction<TDelegate>(String procedureName) where TDelegate : Delegate {
            IntPtr addressOfFunctionToCallPointer = NativeMethods.GetProcAddress(this.DllPointer, procedureName);
            return (TDelegate)Marshal.GetDelegateForFunctionPointer(addressOfFunctionToCallPointer, typeof(TDelegate));
        }

    }

}
