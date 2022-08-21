using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace NineSliceEditor.Controls
{
    internal class RectangleControls : IControl
    {
        public Rectangle Rect;

        public int control_grace = 10;

        enum ManipMode
        {
            None,
            Left,
            Right,
            Top,
            Bottom,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            Move
        }

        ManipMode mode;

        Rectangle InteractRect
        {
            get
            {
                var r = Rect;
                r.Inflate(control_grace, control_grace);
                return r;
            }
        }

        public bool CapturesMouseClick(Point mouse_pos)
        {
            return InteractRect.Contains(mouse_pos);
        }

        public RectangleControls(Rectangle initial)
        {
            Rect = initial;
        }

        ManipMode GetManipMode(Point mouse_pos)
        {
            bool top = Math.Abs(Rect.Top - mouse_pos.Y) <= control_grace;
            bool bottom = Math.Abs(Rect.Bottom - mouse_pos.Y) <= control_grace;
            bool left = Math.Abs(Rect.Left - mouse_pos.X) <= control_grace;
            bool right = Math.Abs(Rect.Right - mouse_pos.X) <= control_grace;

            if(top)
            {
                if (right) return ManipMode.TopRight;
                if (left) return ManipMode.TopLeft;
                return ManipMode.Top;
            }
            if(bottom)
            {
                if (right) return ManipMode.BottomRight;
                if (left) return ManipMode.BottomLeft;
                return ManipMode.Bottom;
            }
            if (left) return ManipMode.Left;
            if (right) return ManipMode.Right;

            return ManipMode.Move;
        }

        public Cursor CursorType(Point mouse_pos)
        {
            return (mode == ManipMode.None ? GetManipMode(mouse_pos) : mode) switch
            {
                ManipMode.None => Cursors.Arrow,
                ManipMode.Left => Cursors.SizeWE,
                ManipMode.Right => Cursors.SizeWE,
                ManipMode.Top => Cursors.SizeNS,
                ManipMode.Bottom => Cursors.SizeNS,
                ManipMode.TopLeft => Cursors.SizeNWSE,
                ManipMode.TopRight => Cursors.SizeNESW,
                ManipMode.BottomLeft => Cursors.SizeNESW,
                ManipMode.BottomRight => Cursors.SizeNWSE,
                ManipMode.Move => Cursors.SizeAll
            };
        }

        public void OnMouseDown(Point mouse_pos)
        {
            mode = GetManipMode(mouse_pos);
        }

        public void OnMouseUp(Point mouse_pos)
        {
            mode = ManipMode.None;
        }

        public void OnMouseMove(Point old_pos, Point new_pos)
        {
            switch (mode)
            {
                case ManipMode.Move:
                    Rect.Offset(new_pos - old_pos);
                    break;
                case ManipMode.Left:
                    AdjustLeft(new_pos);
                    break;
                case ManipMode.Right:
                    AdjustRight(new_pos);
                    break;
                case ManipMode.Top:
                    AdjustTop(new_pos);
                    break;
                case ManipMode.Bottom:
                    AdjustBottom(new_pos);
                    break;
                case ManipMode.TopLeft:
                    AdjustTop(new_pos);
                    AdjustLeft(new_pos);
                    break;
                case ManipMode.TopRight:
                    AdjustTop(new_pos);
                    AdjustRight(new_pos);
                    break;
                case ManipMode.BottomLeft:
                    AdjustBottom(new_pos);
                    AdjustLeft(new_pos);
                    break;
                case ManipMode.BottomRight:
                    AdjustBottom(new_pos);
                    AdjustRight(new_pos);
                    break;
            }
            if(Rect.Width < 0)
            {
                Rect.X += Rect.Width;
                Rect.Width *= -1;
                mode = mode switch
                {
                    ManipMode.Left => ManipMode.Right,
                    ManipMode.Right => ManipMode.Left,
                    ManipMode.TopLeft => ManipMode.TopRight,
                    ManipMode.TopRight => ManipMode.TopLeft,
                    ManipMode.BottomLeft => ManipMode.BottomRight,
                    ManipMode.BottomRight => ManipMode.BottomLeft,
                    _ => mode
                };
            }
            if(Rect.Height < 0)
            {
                Rect.Y += Rect.Height;
                Rect.Height *= -1;
                mode = mode switch
                {
                    ManipMode.TopLeft => ManipMode.BottomLeft,
                    ManipMode.TopRight => ManipMode.BottomRight,
                    ManipMode.BottomRight => ManipMode.TopRight,
                    ManipMode.BottomLeft => ManipMode.TopLeft,
                    ManipMode.Top => ManipMode.Bottom,
                    ManipMode.Bottom => ManipMode.Top,
                    _ => mode
                };
            }

        }

        void AdjustLeft(Point target)
        {
            Rect.Width += Rect.Left - target.X;
            Rect.X = target.X;
        }

        void AdjustRight(Point target)
        {
            Rect.Width = target.X - Rect.Left;
        }

        void AdjustTop(Point target)
        {
            Rect.Height += Rect.Top - target.Y;
            Rect.Y = target.Y;
        }

        void AdjustBottom(Point target)
        {
            Rect.Height = target.Y - Rect.Top;
        }
    }
}
