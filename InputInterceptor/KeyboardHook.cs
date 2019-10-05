using System;

using Filter = System.UInt16;

namespace InputInterceptorNS {

    public class KeyboardHook : Hook<KeyStroke> {

        public KeyboardHook(KeyboardFilter filter = KeyboardFilter.All, Action<KeyStroke> callback = null) :
            base((Filter)filter, InputInterceptor.IsKeyboard, callback) { }

        public KeyboardHook(Action<KeyStroke> callback) :
            base((Filter)KeyboardFilter.All, InputInterceptor.IsKeyboard, callback) { }

        protected override void CallbackWrapper(Stroke stroke) {
            this.Callback(stroke.Key);
        }

    }

}
