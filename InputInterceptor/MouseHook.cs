using System;

using Filter = System.UInt16;

namespace InputInterceptorNS {

    public class MouseHook : Hook<MouseStroke> {

        public MouseHook(MouseFilter filter = MouseFilter.All, Action<MouseStroke> callback = null) :
            base((Filter)filter, InputInterceptor.IsMouse, callback) { }

        public MouseHook(Action<MouseStroke> callback) :
            base((Filter)MouseFilter.All, InputInterceptor.IsMouse, callback) { }

        protected override void CallbackWrapper(Stroke stroke) {
            this.Callback(stroke.Mouse);
        }

    }

}
