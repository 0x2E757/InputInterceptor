using System;
using System.Threading;

using Filter = System.UInt16;

namespace InputInterceptorNS {

    public class MouseHook : Hook<MouseStroke> {

        private static readonly Int32 VirtualScreenOriginLeft;
        private static readonly Int32 VirtualScreenOriginTop;
        private static readonly Int32 VirtualScreenWidth;
        private static readonly Int32 VirtualScreenHeight;

        static MouseHook() {
            VirtualScreenOriginLeft = NativeMethods.GetSystemMetrics(76);
            VirtualScreenOriginTop = NativeMethods.GetSystemMetrics(77);
            VirtualScreenWidth = NativeMethods.GetSystemMetrics(78);
            VirtualScreenHeight = NativeMethods.GetSystemMetrics(79);
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

        private Boolean SimulateButtonClick(MouseState firstState, MouseState secondState, Int32 releaseDelay) {
            if (this.SetMouseState(firstState)) {
                Thread.Sleep(releaseDelay);
                return this.SetMouseState(secondState);
            }
            return false;
        }

        public Boolean SimulateLeftButtonClick(Int32 releaseDelay = 50) {
            return this.SimulateButtonClick(MouseState.LeftButtonDown, MouseState.LeftButtonUp, releaseDelay);
        }

        public Boolean SimulateMiddleButtonClick(Int32 releaseDelay = 50) {
            return this.SimulateButtonClick(MouseState.MiddleButtonDown, MouseState.MiddleButtonUp, releaseDelay);
        }

        public Boolean SimulateRightButtonClick(Int32 releaseDelay = 50) {
            return this.SimulateButtonClick(MouseState.RightButtonDown, MouseState.RightButtonUp, releaseDelay);
        }

        public Boolean SimulateScrollDown(Int16 rolling = 120) {
            return this.SetMouseState(MouseState.ScrollVertical, (Int16)(-rolling));
        }

        public Boolean SimulateScrollUp(Int16 rolling = 120) {
            return this.SetMouseState(MouseState.ScrollVertical, rolling);
        }

        public Win32Point GetCursorPosition() {
            Win32Point result = new Win32Point();
            NativeMethods.GetCursorPos(result);
            return result;
        }

        public Boolean SetCursorPosition(Win32Point point, Boolean useWinAPI = false, Boolean useWinAPIOnly = false) {
            return this.SetCursorPosition(point.X, point.Y, useWinAPI, useWinAPIOnly);
        }

        public Boolean SetCursorPosition(Int32 x, Int32 y, Boolean useWinAPI = false, Boolean useWinAPIOnly = false) {
            if (useWinAPIOnly) {
                if (useWinAPI) {
                    return NativeMethods.SetCursorPos(x, y);
                }
            } else {
                if (this.CanSimulateInput) {
                    Stroke stroke = new Stroke();
                    stroke.Mouse.X = (Int32)((x - VirtualScreenOriginLeft) * (65535d / (VirtualScreenWidth - 1)));
                    stroke.Mouse.Y = (Int32)((y - VirtualScreenOriginTop) * (65535d / (VirtualScreenHeight - 1)));
                    stroke.Mouse.Flags = MouseFlags.MoveAbsolute;
                    if (useWinAPI) {
                        return InputInterceptor.Send(this.Context, this.AnyDevice, ref stroke, 1) == 1 && NativeMethods.SetCursorPos(x, y);
                    } else {
                        return InputInterceptor.Send(this.Context, this.AnyDevice, ref stroke, 1) == 1;
                    }
                }
            }
            return false;
        }

        public Boolean MoveCursorBy(Int32 dX, Int32 dY, Boolean useWinAPI = false, Boolean useWinAPIOnly = false) {
            if (useWinAPIOnly) {
                if (useWinAPI) {
                    Win32Point point = this.GetCursorPosition();
                    return NativeMethods.SetCursorPos(point.X + dX, point.Y + dY);
                }
            } else {
                if (this.CanSimulateInput) {
                    Stroke stroke = new Stroke();
                    stroke.Mouse.X = dX;
                    stroke.Mouse.Y = dY;
                    stroke.Mouse.Flags = MouseFlags.MoveRelative;
                    if (useWinAPI) {
                        Win32Point point = this.GetCursorPosition();
                        return InputInterceptor.Send(this.Context, this.AnyDevice, ref stroke, 1) == 1 && NativeMethods.SetCursorPos(point.X + dX, point.Y + dY);
                    } else {
                        return InputInterceptor.Send(this.Context, this.AnyDevice, ref stroke, 1) == 1;
                    }
                }
            }
            return false;
        }

        private Boolean SmoothMoveCursorBy(Win32Point startPosition, Int32 dX, Int32 dY, Int32 speed = 15, Boolean useWinAPI = false, Boolean useWinAPIOnly = false) {
            if (this.CanSimulateInput == false) return false;
            if (dX == 0 && dY == 0) return true;
            if (Math.Abs(dX) >= Math.Abs(dY)) {
                Double k = (Double)dY / (Double)dX;
                for (Int32 n = 0, nMax = Math.Abs(dX / speed); n < nMax; n++) {
                    if (this.SetCursorPosition(startPosition.X + n * dX / nMax, (Int32)(startPosition.Y + n * dX / nMax * k), useWinAPI, useWinAPIOnly) == false) return false;
                    Thread.Sleep(10);
                }
            } else {
                Double k = (Double)dX / (Double)dY;
                for (Int32 n = 0, nMax = Math.Abs(dY / speed); n < nMax; n++) {
                    if (this.SetCursorPosition((Int32)(startPosition.X + n * dY / nMax * k), startPosition.Y + n * dY / nMax, useWinAPI, useWinAPIOnly) == false) return false;
                    Thread.Sleep(10);
                }
            }
            if (this.SetCursorPosition(startPosition.X + dX, startPosition.Y + dY, useWinAPI, useWinAPIOnly) == false) return false;
            return true;
        }

        public Boolean SimulateMoveTo(Win32Point point, Int32 speed = 15, Boolean useWinAPI = false, Boolean useWinAPIOnly = false) {
            if (this.CanSimulateInput == false) return false;
            Win32Point startPosition = this.GetCursorPosition();
            Int32 dX = point.X - startPosition.X;
            Int32 dY = point.Y - startPosition.Y;
            return this.SmoothMoveCursorBy(startPosition, dX, dY, speed, useWinAPI, useWinAPIOnly);
        }

        public Boolean SimulateMoveTo(Int32 x, Int32 y, Int32 speed = 15, Boolean useWinAPI = false, Boolean useWinAPIOnly = false) {
            if (this.CanSimulateInput == false) return false;
            Win32Point startPosition = this.GetCursorPosition();
            Int32 dX = x - startPosition.X;
            Int32 dY = y - startPosition.Y;
            return this.SmoothMoveCursorBy(startPosition, dX, dY, speed, useWinAPI, useWinAPIOnly);
        }

        public Boolean SimulateMoveBy(Int32 dX, Int32 dY, Int32 speed = 15, Boolean useWinAPI = false, Boolean useWinAPIOnly = false) {
            if (this.CanSimulateInput == false) return false;
            Win32Point startPosition = this.GetCursorPosition();
            return this.SmoothMoveCursorBy(startPosition, dX, dY, speed, useWinAPI, useWinAPIOnly);
        }

    }

}
