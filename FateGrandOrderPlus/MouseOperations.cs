using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FateGrandOrderPlus
{
    class MouseOperations
    {
        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void SetCursorPosition(int X, int Y)
        {
            SetCursorPos(X, Y);
        }

        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public static MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        public static void MouseEvent(MouseEventFlags value)
        {
            MousePoint position = GetCursorPosition();

            mouse_event
                ((int)value,
                    position.X,
                    position.Y,
                    0,
                    0)
                ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }

        }
           
        private static async Task SwoopToPosition(int x, int y)
        {
            Cursor.Position = new Point(x + 10, y + 5);
            await Task.Delay(5);
            Cursor.Position = new Point(x + 5, y + 2);
            await Task.Delay(5);
            Cursor.Position = new Point(x + 2, y + 1);
            await Task.Delay(5);
            Cursor.Position = new Point(x, y);
            await Task.Delay(5);
        }

        public static async Task MyClickOnce(int x, int y)
        {
            await
                SwoopToPosition(x, y);
            MouseEvent(MouseEventFlags.LeftDown);
            await
                Task.Delay(40);
            MouseEvent(MouseEventFlags.LeftUp);
            await Task.Delay(20);
        }
        
        public static async Task MyClickThrice(int x, int y)
        {
            await SwoopToPosition(x, y);
            for (int i = 0; i < 3; i++)
            {
                MouseEvent(MouseEventFlags.LeftDown);
                await Task.Delay(40);
                MouseEvent(MouseEventFlags.LeftUp);
                await Task.Delay(20);
            }
        }

        public static async Task MyDrag(int x1, int y1, int x2, int y2)
        {
            Cursor.Position = new Point(x1, y1); // TODO: consider replacing with MyPoint generation

            const int steps = 30;
            const double stepportion = 1.0 / ((double)steps);
            double xstep = stepportion * (x2 - x1);
            double ystep = stepportion * (y2 - y1);
            double x = x1;
            double y = y1;
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
            await Task.Delay(3);
            for (int i = 0; i < steps; i++)
            {
                x += xstep;
                y += ystep;
                Cursor.Position = new Point((int)x, (int)y);
                await Task.Delay(5);
            }
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
            await Task.Delay(10);
        }

    }
}
