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

    }

}
