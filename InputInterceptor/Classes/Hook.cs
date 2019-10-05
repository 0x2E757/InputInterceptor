using System;
using System.Threading;

using Context = System.IntPtr;
using Device = System.Int32;
using Filter = System.UInt16;

namespace InputInterceptorNS {

    public abstract class Hook<TCallbackStroke> : IDisposable {

        public Context Context { get; private set; }
        public Device Device { get; private set; }
        public Filter FilterMode { get; private set; }
        public Predicate Predicate { get; private set; }
        public Action<TCallbackStroke> Callback { get; private set; }
        public Boolean Active { get; private set; }
        public Thread InterceptionThread { get; private set; }

        public Hook(Filter filterMode, Predicate predicate, Action<TCallbackStroke> callback) {
            this.Context = InputInterceptor.CreateContext();
            this.Device = -1;
            this.FilterMode = filterMode;
            this.Predicate = predicate;
            this.Callback = callback;
            if (this.Context != IntPtr.Zero) {
                this.Active = true;
                this.InterceptionThread = new Thread(this.Main);
                this.InterceptionThread.Priority = ThreadPriority.Highest;
                this.InterceptionThread.IsBackground = true;
                this.InterceptionThread.Start();
            } else {
                this.Active = false;
                this.InterceptionThread = null;
            }
        }

        private void Main() {
            InputInterceptor.SetFilter(this.Context, this.Predicate, this.FilterMode);
            Stroke stroke = new Stroke();
            while (this.Active) {
                if (InputInterceptor.Receive(this.Context, this.Device = InputInterceptor.WaitWithTimeout(this.Context, 100), ref stroke, 1) > 0) {
                    if (this.Callback != null && this.Active) {
                        try {
                            this.CallbackWrapper(stroke);
                        } catch (Exception exception) {
                            Console.WriteLine(exception);
                            this.Active = false;
                        }
                    } else {
                        this.Active = false;
                    }
                    InputInterceptor.Send(this.Context, this.Device, ref stroke, 1);
                }
            }
            if (this.Callback == null) {
                InputInterceptor.DestroyContext(this.Context);
                this.Context = InputInterceptor.CreateContext();
            }
        }

        protected abstract void CallbackWrapper(Stroke stroke);

        public void Dispose() {
            if (this.Active) {
                this.Active = false;
                this.InterceptionThread.Join();
                InputInterceptor.DestroyContext(this.Context);
            }
        }

    }

}
