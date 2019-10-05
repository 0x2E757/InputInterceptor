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
            if (this.IsInitialized) {
                Stroke stroke = new Stroke();
                stroke.Mouse.State = state;
                stroke.Mouse.Rolling = rolling;
                return InputInterceptor.Send(this.Context, this.Device, ref stroke, 1) == 1;
            }
            return false;
        }

        private Boolean SimulateButtonClick(MouseState firstState, MouseState secondState, Int32 delay) {
            if (this.SetMouseState(firstState)) {
                Thread.Sleep(delay);
                return this.SetMouseState(secondState);
            }
            return false;
        }

        public Boolean SimulateLeftButtonClick(Int32 delay = 50) {
            return this.SimulateButtonClick(MouseState.LeftButtonDown, MouseState.LeftButtonUp, delay);
        }

        public Boolean SimulateMiddleButtonClick(Int32 delay = 50) {
            return this.SimulateButtonClick(MouseState.MiddleButtonDown, MouseState.MiddleButtonUp, delay);
        }

        public Boolean SimulateRightButtonClick(Int32 delay = 50) {
            return this.SimulateButtonClick(MouseState.RightButtonDown, MouseState.RightButtonUp, delay);
        }

        public Boolean ScrollDown(Int16 rolling = 120) {
            return this.SetMouseState(MouseState.ScrollVertical, (Int16)(-rolling));
        }

        public Boolean ScrollUp(Int16 rolling = 120) {
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
                if (this.IsInitialized) {
                    Stroke stroke = new Stroke();
                    stroke.Mouse.X = (Int32)((x - VirtualScreenOriginLeft) * (65535d / (VirtualScreenWidth - 1)));
                    stroke.Mouse.Y = (Int32)((y - VirtualScreenOriginTop) * (65535d / (VirtualScreenHeight - 1)));
                    stroke.Mouse.Flags = MouseFlags.MoveAbsolute;
                    if (useWinAPI) {
                        return InputInterceptor.Send(this.Context, this.Device, ref stroke, 1) == 1 && NativeMethods.SetCursorPos(x, y);
                    } else {
                        return InputInterceptor.Send(this.Context, this.Device, ref stroke, 1) == 1;
                    }
                }
            }
            return false;
        }

        public Boolean MoveCursorBy(Int32 dx, Int32 dy, Boolean useWinAPI = false, Boolean useWinAPIOnly = false) {
            if (useWinAPIOnly) {
                if (useWinAPI) {
                    Win32Point point = this.GetCursorPosition();
                    return NativeMethods.SetCursorPos(point.X + dx, point.Y + dy);
                }
            } else {
                if (this.IsInitialized) {
                    Stroke stroke = new Stroke();
                    stroke.Mouse.X = dx;
                    stroke.Mouse.Y = dy;
                    stroke.Mouse.Flags = MouseFlags.MoveRelative;
                    if (useWinAPI) {
                        Win32Point point = this.GetCursorPosition();
                        return InputInterceptor.Send(this.Context, this.Device, ref stroke, 1) == 1 && NativeMethods.SetCursorPos(point.X + dx, point.Y + dy);
                    } else {
                        return InputInterceptor.Send(this.Context, this.Device, ref stroke, 1) == 1;
                    }
                }
            }
            return false;
        }

    }

}
