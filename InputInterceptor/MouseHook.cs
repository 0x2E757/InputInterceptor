using Filter = System.UInt16;

namespace InputInterceptorNS {

    public class MouseHook : Hook<MouseStroke> {

        public MouseHook(MouseFilter filter = MouseFilter.All, CallbackAction callback = null) :
            base((Filter)filter, InputInterceptor.IsMouse, callback) { }

        public MouseHook(CallbackAction callback) :
            base((Filter)MouseFilter.All, InputInterceptor.IsMouse, callback) { }

        protected override void CallbackWrapper(ref Stroke stroke) {
            this.Callback(ref stroke.Mouse);
        }

    }

}
