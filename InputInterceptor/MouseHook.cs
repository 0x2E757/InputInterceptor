using System;
using System.Threading;

using Filter = System.UInt16;

namespace InputInterceptorNS {

    public class MouseHook : Hook<MouseStroke> {

        private const Int32 SM_CXSCREEN = 0;
        private const Int32 SM_CYSCREEN = 1;

        private static readonly Int32 PrimaryScreenWidth;
        private static readonly Int32 PrimaryScreenHeight;

        static MouseHook() {
            PrimaryScreenWidth = NativeMethods.GetSystemMetrics(SM_CXSCREEN);
            PrimaryScreenHeight = NativeMethods.GetSystemMetrics(SM_CYSCREEN);
        }

        public MouseHook(MouseFilter filter = MouseFilter.All, CallbackAction callback = null) :
            base((Filter)filter, InputInterceptor.IsMouse, callback) { }

        public MouseHook(CallbackAction callback) :
            base((Filter)MouseFilter.All, InputInterceptor.IsMouse, callback) { }

        protected override void CallbackWrapper(ref Stroke stroke) {
            this.Callback(ref stroke.Mouse);
        }

        public Boolean SetMouseState(MouseState state, Int16 rolling = 0) {
            if (this.CanSimulateInput) {
                Stroke stroke = new Stroke();
                stroke.Mouse.State = state;
                stroke.Mouse.Rolling = rolling;
                return InputInterceptor.Send(this.Context, this.AnyDevice, ref stroke, 1) == 1;
            }
            return false;
        }

        public Boolean SimulateLeftButtonDown() {
            return this.SetMouseState(MouseState.LeftButtonDown);
        }

        public Boolean SimulateLeftButtonUp() {
            return this.SetMouseState(MouseState.LeftButtonUp);
        }

        public Boolean SimulateLeftButtonClick(Int32 releaseDelay = 50) {
            if (this.SimulateLeftButtonDown()) {
                Thread.Sleep(releaseDelay);
                return this.SimulateLeftButtonUp();
            }
            return false;
        }

        public Boolean SimulateMiddleButtonDown() {
            return this.SetMouseState(MouseState.MiddleButtonDown);
        }

        public Boolean SimulateMiddleButtonUp() {
            return this.SetMouseState(MouseState.MiddleButtonUp);
        }

        public Boolean SimulateMiddleButtonClick(Int32 releaseDelay = 50) {
            if (this.SimulateMiddleButtonDown()) {
                Thread.Sleep(releaseDelay);
                return this.SimulateMiddleButtonUp();
            }
            return false;
        }

        public Boolean SimulateRightButtonDown() {
            return this.SetMouseState(MouseState.RightButtonDown);
        }

        public Boolean SimulateRightButtonUp() {
            return this.SetMouseState(MouseState.RightButtonUp);
        }

        public Boolean SimulateRightButtonClick(Int32 releaseDelay = 50) {
            if (this.SimulateRightButtonDown()) {
                Thread.Sleep(releaseDelay);
                return this.SimulateRightButtonUp();
            }
            return false;
        }

        public Boolean SimulateScrollDown(Int16 rolling = 120) {
            return this.SetMouseState(MouseState.ScrollVertical, (Int16)(-rolling));
        }

        public Boolean SimulateScrollUp(Int16 rolling = 120) {
            return this.SetMouseState(MouseState.ScrollVertical, rolling);
        }

        public Win32Point GetCursorPosition() {
            Win32Point result;
            NativeMethods.GetCursorPos(out result);
            return result;
        }

        public Boolean SetCursorPosition(Win32Point point, Boolean useWinAPI = false) {
            return this.SetCursorPosition(point.X, point.Y, useWinAPI);
        }

        public Boolean SetCursorPosition(Int32 x, Int32 y, Boolean useWinAPI = false) {
            if (useWinAPI) {
                return NativeMethods.SetCursorPos(x, y);
            } else {
                if (this.CanSimulateInput) {
                    Stroke stroke = new Stroke();
                    stroke.Mouse.X = UInt16.MaxValue * x / (PrimaryScreenWidth - 1);
                    stroke.Mouse.Y = UInt16.MaxValue * y / (PrimaryScreenHeight - 1);
                    stroke.Mouse.Flags = MouseFlags.MoveAbsolute;
                    return InputInterceptor.Send(this.Context, this.AnyDevice, ref stroke, 1) == 1;
                }
            }
            return false;
        }

        public Boolean MoveCursorBy(Int32 dX, Int32 dY, Boolean useWinAPI = false) {
            if (useWinAPI) {
                Win32Point point = this.GetCursorPosition();
                return NativeMethods.SetCursorPos(point.X + dX, point.Y + dY);
            } else {
                if (this.CanSimulateInput) {
                    Stroke stroke = new Stroke();
                    stroke.Mouse.X = dX;
                    stroke.Mouse.Y = dY;
                    stroke.Mouse.Flags = MouseFlags.MoveRelative;
                    return InputInterceptor.Send(this.Context, this.AnyDevice, ref stroke, 1) == 1;
                }
            }
            return false;
        }

        private Boolean SmoothMoveCursorBy(Win32Point startPosition, Int32 dX, Int32 dY, Int32 speed = 15, Boolean useWinAPI = false) {
            if (!this.CanSimulateInput)
                return false;
            if (dX == 0 && dY == 0)
                return true;
            if (Math.Abs(dX) >= Math.Abs(dY)) {
                Double k = (Double)dY / (Double)dX;
                for (Int32 n = 0, nMax = Math.Abs(dX / speed); n < nMax; n += 1) {
                    Int32 x = startPosition.X + n * dX / nMax;
                    Int32 y = (Int32)(startPosition.Y + n * dX / nMax * k);
                    if (!this.SetCursorPosition(x, y, useWinAPI))
                        return false;
                    Thread.Sleep(10);
                }
            } else {
                Double k = (Double)dX / (Double)dY;
                for (Int32 n = 0, nMax = Math.Abs(dY / speed); n < nMax; n += 1) {
                    Int32 x = (Int32)(startPosition.X + n * dY / nMax * k);
                    Int32 y = startPosition.Y + n * dY / nMax;
                    if (!this.SetCursorPosition(x, y, useWinAPI))
                        return false;
                    Thread.Sleep(10);
                }
            }
            if (!this.SetCursorPosition(startPosition.X + dX, startPosition.Y + dY, useWinAPI))
                return false;
            return true;
        }

        public Boolean SimulateMoveTo(Win32Point point, Int32 speed = 15, Boolean useWinAPI = false) {
            return this.SimulateMoveTo(point.X, point.Y, speed, useWinAPI);
        }

        public Boolean SimulateMoveTo(Int32 x, Int32 y, Int32 speed = 15, Boolean useWinAPI = false) {
            if (!this.CanSimulateInput)
                return false;
            Win32Point startPosition = this.GetCursorPosition();
            Int32 dX = x - startPosition.X;
            Int32 dY = y - startPosition.Y;
            return this.SmoothMoveCursorBy(startPosition, dX, dY, speed, useWinAPI);
        }

        public Boolean SimulateMoveBy(Int32 dX, Int32 dY, Int32 speed = 15, Boolean useWinAPI = false) {
            if (!this.CanSimulateInput)
                return false;
            Win32Point startPosition = this.GetCursorPosition();
            return this.SmoothMoveCursorBy(startPosition, dX, dY, speed, useWinAPI);
        }

    }

}
