using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace InputInterceptorNS {

    [StructLayout(LayoutKind.Sequential)]
    public class Win32Point {

        public Int32 X;
        public Int32 Y;

        public Win32Point() {
            this.X = 0;
            this.Y = 0;
        }

        public Win32Point(Int32 x, Int32 y) {
            this.X = x;
            this.Y = y;
        }

        public static implicit operator Point(Win32Point point) {
            return new Point(point.X, point.Y);
        }

        public static implicit operator Win32Point(Point point) {
            return new Win32Point(point.X, point.Y);
        }

        public override String ToString() {
            return this.X.ToString() + "," + this.Y.ToString();
        }

    }

}
