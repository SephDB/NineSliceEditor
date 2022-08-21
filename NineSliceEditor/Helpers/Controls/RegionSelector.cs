using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NineSliceEditor.Controls;
using NineSliceEditor.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NineSliceEditor.Controls
{
    public class RegionSelector : IControl
    {
        readonly PrimitivesDrawer primitives;

        Rectangle position; //Screen coordinates

        int scale_factor;

        Rectangle centerRegion; //Texture-relative coordinates
        readonly Texture2D target;

        public Rectangle Center { get => centerRegion; set => centerRegion = value; }

        enum ManipMode
        {
            Left,
            Right,
            Top,
            Bottom,
            None
        }

        ManipMode mode = ManipMode.None;

        public RegionSelector(PrimitivesDrawer primitives, Rectangle editor_area, Texture2D edited)
        {
            this.primitives = primitives;
            target = edited;

            ResetPosition(editor_area);

            centerRegion = edited.Bounds;
            centerRegion.Inflate(-centerRegion.Width / 4, -centerRegion.Height / 4);
        }

        public void ResetPosition(Rectangle editor_area)
        {
            const int min_padding = 30;
            editor_area.Inflate(-min_padding,-min_padding);
            scale_factor = Math.Min(editor_area.Width / target.Width, editor_area.Height / target.Height);
            scale_factor = Math.Max(scale_factor, 1);

            position = editor_area;
            position.Inflate(-(editor_area.Width - target.Width * scale_factor) / 2, -(editor_area.Height - target.Height * scale_factor) / 2);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(target, position, Color.White);
            primitives.DrawHoriz(new(position.X, position.Y + centerRegion.Top * scale_factor), position.Width, Color.Blue, spriteBatch);
            primitives.DrawHoriz(new(position.X, position.Y + centerRegion.Bottom * scale_factor), position.Width, Color.Blue, spriteBatch);
            primitives.DrawVert(new(position.X + Center.Left * scale_factor, position.Y), position.Height, Color.Blue, spriteBatch);
            primitives.DrawVert(new(position.X + Center.Right * scale_factor, position.Y), position.Height, Color.Blue, spriteBatch);
        }

        ManipMode GetMode(Point mouse_pos)
        {
            if (!position.Contains(mouse_pos)) return ManipMode.None;


            mouse_pos -= position.Location;
            const int control_grace = 10;
            if (Math.Abs(mouse_pos.Y - centerRegion.Top * scale_factor) < control_grace) return ManipMode.Top;
            if (Math.Abs(mouse_pos.Y - centerRegion.Bottom * scale_factor) < control_grace) return ManipMode.Bottom;
            if (Math.Abs(mouse_pos.X - centerRegion.Left * scale_factor) < control_grace) return ManipMode.Left;
            if (Math.Abs(mouse_pos.X - centerRegion.Right * scale_factor) < control_grace) return ManipMode.Right;

            return ManipMode.None;
        }

        public bool CapturesMouseClick(Point mouse_pos)
        {
            return GetMode(mouse_pos) != ManipMode.None;
        }

        public Cursor CursorType(Point mouse_pos)
        {
            return (mode == ManipMode.None ? GetMode(mouse_pos) : mode) switch
            {
                ManipMode.None => Cursors.Arrow,
                ManipMode.Right => Cursors.SizeWE,
                ManipMode.Left => Cursors.SizeWE,
                ManipMode.Top => Cursors.SizeNS,
                ManipMode.Bottom => Cursors.SizeNS
            };
        }

        public void OnMouseDown(Point mouse_pos)
        {
            mode = GetMode(mouse_pos);
        }

        public void OnMouseMove(Point old_pos, Point new_pos)
        {
            Point p = ToCenterLocal(new_pos);
            p.X = Math.Clamp(p.X, 0, target.Width);
            p.Y = Math.Clamp(p.Y, 0, target.Height);
            switch (mode)
            {
                case ManipMode.Left:
                    p.X = Math.Min(centerRegion.Right - 1, p.X);
                    centerRegion.Width = centerRegion.Right - p.X;
                    centerRegion.X = p.X;
                    break;
                case ManipMode.Right:
                    p.X = Math.Max(centerRegion.Left + 1, p.X);
                    centerRegion.Width = p.X - centerRegion.X;
                    break;
                case ManipMode.Top:
                    p.Y = Math.Min(centerRegion.Bottom - 1, p.Y);
                    centerRegion.Height = centerRegion.Bottom - p.Y;
                    centerRegion.Y = p.Y;
                    break;
                case ManipMode.Bottom:
                    p.Y = Math.Max(centerRegion.Top + 1, p.Y);
                    centerRegion.Height = p.Y - centerRegion.Y;
                    break;
                case ManipMode.None:
                    break;
            }
        }

        public void OnMouseUp(Point mouse_pos)
        {
            mode = ManipMode.None;
        }

        Point ToCenterLocal(Point p)
        {
            return Vector2.Round((p - position.Location).ToVector2() / scale_factor).ToPoint();
        }
    }
}
