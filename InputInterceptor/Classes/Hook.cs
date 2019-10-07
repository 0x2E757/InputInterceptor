using System;
using System.Collections.Generic;
using System.Threading;

using Context = System.IntPtr;
using Device = System.Int32;
using Filter = System.UInt16;

namespace InputInterceptorNS {

    public abstract class Hook<TCallbackStroke> : IDisposable {

        public delegate void CallbackAction(ref TCallbackStroke stroke);

        public Context Context { get; private set; }
        public Device Device { get; private set; }
        public Device RandomDevice { get; private set; }
        public Filter FilterMode { get; private set; }
        public Predicate Predicate { get; private set; }
        public CallbackAction Callback { get; private set; }
        public Exception Exception { get; private set; }
        public Boolean Active { get; private set; }
        public Thread InterceptionThread { get; private set; }

        public Boolean IsInitialized { get => this.Context != Context.Zero && this.Device != -1; }
        public Boolean CanSimulateInput { get => this.Context != Context.Zero && (this.RandomDevice != -1 || this.Device != -1); }
        public Boolean HasException { get => this.Exception != null; }

        protected Device AnyDevice { get => this.Device != -1 ? this.Device : this.RandomDevice; }

        public Hook(Filter filterMode, Predicate predicate, CallbackAction callback) {
            Context context = InputInterceptor.CreateContext();
            List<DeviceData> devices = InputInterceptor.GetDeviceList(context, predicate);
            this.Context = context;
            this.Device = -1;
            this.RandomDevice = devices.Count > 0 ? devices[0].Device : -1;
            this.FilterMode = filterMode;
            this.Predicate = predicate;
            this.Callback = callback;
            this.Exception = null;
            if (this.Context != IntPtr.Zero && this.Callback != null) {
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
            Device device;
            Stroke stroke = new Stroke();
            while (this.Active) {
                if (InputInterceptor.Receive(this.Context, device = InputInterceptor.WaitWithTimeout(this.Context, 100), ref stroke, 1) > 0) {
                    this.Device = device;
                    if (this.Active) {
                        try {
                            this.CallbackWrapper(ref stroke);
                        } catch (Exception exception) {
                            Console.WriteLine(exception);
                            this.Exception = exception;
                            this.Active = false;
                        }
                    } else {
                        this.Active = false;
                    }
                    InputInterceptor.Send(this.Context, device, ref stroke, 1);
                }
            }
        }

        protected abstract void CallbackWrapper(ref Stroke stroke);

        public void Dispose() {
            if (this.Context != Context.Zero) {
                if (this.Active) {
                    this.Active = false;
                    this.InterceptionThread.Join();
                }
                InputInterceptor.DestroyContext(this.Context);
                this.Context = Context.Zero;
            }
        }

    }

}
