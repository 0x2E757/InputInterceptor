using System;
using System.Threading;

using Filter = System.UInt16;

namespace InputInterceptorNS {

    public class KeyboardHook : Hook<KeyStroke> {

        public KeyboardHook(KeyboardFilter filter = KeyboardFilter.All, CallbackAction callback = null) :
            base((Filter)filter, InputInterceptor.IsKeyboard, callback) { }

        public KeyboardHook(CallbackAction callback) :
            base((Filter)KeyboardFilter.All, InputInterceptor.IsKeyboard, callback) { }

        protected override void CallbackWrapper(ref Stroke stroke) {
            this.Callback(ref stroke.Key);
        }

        public Boolean SetKeyState(KeyCode code, KeyState state) {
            if (this.IsInitialized) {
                Stroke stroke = new Stroke();
                stroke.Key.Code = code;
                stroke.Key.State = state;
                return InputInterceptor.Send(this.Context, this.Device, ref stroke, 1) == 1;
            }
            return false;
        }

        private Boolean SimulateKeyPress(KeyCode code, KeyState firstState, KeyState secondState, Int32 delay) {
            if (this.SetKeyState(code, firstState)) {
                Thread.Sleep(delay);
                return this.SetKeyState(code, secondState);
            }
            return false;
        }

        public Boolean SimulateKeyPress(KeyCode code, Int32 delay = 100) {
            return this.SimulateKeyPress(code, KeyState.Down, KeyState.Up, delay);
        }

    }

}
