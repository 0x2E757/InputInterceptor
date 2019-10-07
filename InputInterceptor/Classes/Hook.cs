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
        public Boolean InterceptionActive { get; private set; }
        public Boolean DeviceCaptureActive { get; private set; }
        public Thread Thread { get; private set; }

        public Boolean IsInitialized { get => this.Context != Context.Zero && this.Device != -1; }
        public Boolean CanSimulateInput { get => this.Context != Context.Zero && (this.Device != -1 || this.RandomDevice != -1); }
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
            if (this.Context != IntPtr.Zero) {
                if (this.Callback != null) {
                    this.InterceptionActive = true;
                    this.DeviceCaptureActive = false;
                    this.Thread = new Thread(this.InterceptionMain);
                    this.Thread.Priority = ThreadPriority.Highest;
                    this.Thread.IsBackground = true;
                    this.Thread.Start();
                } else {
                    this.InterceptionActive = false;
                    this.DeviceCaptureActive = true;
                    this.Thread = new Thread(this.DeviceCaptureMain);
                    this.Thread.Priority = ThreadPriority.Lowest;
                    this.Thread.IsBackground = true;
                    this.Thread.Start();
                }
            } else {
                this.InterceptionActive = false;
                this.DeviceCaptureActive = false;
                this.Thread = null;
            }
        }

        private void InterceptionMain() {
            InputInterceptor.SetFilter(this.Context, this.Predicate, this.FilterMode);
            Device device;
            Stroke stroke = new Stroke();
            while (this.InterceptionActive) {
                if (InputInterceptor.Receive(this.Context, device = InputInterceptor.WaitWithTimeout(this.Context, 100), ref stroke, 1) > 0) {
                    this.Device = device;
                    if (this.InterceptionActive) {
                        try {
                            this.CallbackWrapper(ref stroke);
                        } catch (Exception exception) {
                            Console.WriteLine(exception);
                            this.Exception = exception;
                            this.InterceptionActive = false;
                        }
                    } else {
                        this.InterceptionActive = false;
                    }
                    InputInterceptor.Send(this.Context, device, ref stroke, 1);
                }
            }
        }

        private void DeviceCaptureMain() {
            InputInterceptor.SetFilter(this.Context, this.Predicate, this.FilterMode);
            Device device;
            Stroke stroke = new Stroke();
            while (this.DeviceCaptureActive) {
                if (InputInterceptor.Receive(this.Context, device = InputInterceptor.WaitWithTimeout(this.Context, 100), ref stroke, 1) > 0) {
                    if (this.Predicate(device)) {
                        this.Device = device;
                        this.DeviceCaptureActive = false;
                    }
                    InputInterceptor.Send(this.Context, device, ref stroke, 1);
                }
            }
            InputInterceptor.DestroyContext(this.Context);
            this.Context = InputInterceptor.CreateContext();
        }

        protected abstract void CallbackWrapper(ref Stroke stroke);

        public void Dispose() {
            if (this.Context != Context.Zero) {
                if (this.InterceptionActive) {
                    this.InterceptionActive = false;
                    this.Thread.Join();
                }
                if (this.DeviceCaptureActive) {
                    this.DeviceCaptureActive = false;
                    this.Thread.Join();
                }
                InputInterceptor.DestroyContext(this.Context);
                this.Context = Context.Zero;
            }
        }

    }

}
