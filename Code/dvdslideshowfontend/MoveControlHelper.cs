using System;
using System.Drawing;
using System.Windows.Forms;

namespace dvdslideshowfontend
{
    public class MoveControlHelper
    {
        public delegate void MovingControlCallback(int pos);
        public delegate void MoveControlCallback();

        public enum Direction
        {
            Any,
            Horizontal,
            Vertical
        }

        public static void Init(Control control)
        {
            Init(control, Direction.Any);
        }

        public static void Init(Control control, Direction direction)
        {
            Init(control, control, direction, null, null, null);
        }

        public static void Init(Control control, Control container, Direction direction, MoveControlCallback movedStartedCallback, MovingControlCallback movingcallback, MoveControlCallback movedCallback)
        {
            bool Dragging = false;
            Point DragStart = Point.Empty;
            MovingControlCallback mMovingCallback = movingcallback;
            MoveControlCallback mMovedCallback = movedCallback;
            MoveControlCallback mMovedStartedCallback = movedStartedCallback;

            control.MouseDown += delegate(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Right) return;

                Dragging = true;
                DragStart = new Point(e.X, e.Y);
                control.Capture = true;
                container.BringToFront();

                if (mMovedStartedCallback != null)
                {
                    mMovedStartedCallback();
                }

            };
            control.MouseUp += delegate(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Right) return;

                Dragging = false;
                control.Capture = false;
                if (mMovedCallback != null)
                {
                    mMovedCallback();
                }

            };
            control.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Right) return;
                if (Dragging)
                {
                    int pos = 0;

                    if (direction != Direction.Vertical)
                    {
                        pos = e.X + container.Left - DragStart.X;

                        int left = pos;

                        if (pos < 0)
                        {
                            left = 0;    
                        }
                        else if (pos > Form1.mMainForm.GetSlideShowPanel().Width)
                        {
                            left = Form1.mMainForm.GetSlideShowPanel().Width;
                        }
                        else
                        {
                            container.Left = left;
                        }

                    }
                    if (direction != Direction.Horizontal)
                        container.Top = Math.Max(0, e.Y + container.Top - DragStart.Y);

                    if (mMovingCallback != null)
                    {
                        mMovingCallback(pos);
                    }
                }
            };
        }
    }
}