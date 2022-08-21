using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop.Input;
using NineSliceEditor.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NineSliceEditor.Helpers
{
    internal class UIControl
    {
        WpfMouse input;
        System.Windows.FrameworkElement parent;

        Point posLastFrame = Point.Zero;
        bool pressedLastFrame = false;
        IControl? focus = null;

        public UIControl(WpfMouse mouse, System.Windows.FrameworkElement parent)
        {
            input = mouse;
            this.parent = parent;
        }

        public void Update(IEnumerable<IControl> controls)
        {
            var mouse = input.GetState();
            bool focused = focus is not null;
            if (!focused)
            {
                foreach (IControl control in controls)
                {
                    if (control.CapturesMouseClick(mouse.Position))
                    {
                        focus = control;
                        break;
                    }
                }
            }
            else
            {
                focus?.OnMouseMove(posLastFrame, mouse.Position);
            }

            parent.Cursor = focus?.CursorType(mouse.Position) ?? Cursors.Arrow;
            if (focused && mouse.LeftButton == ButtonState.Released)
            {
                focus?.OnMouseUp(mouse.Position);
                focus = null;
            }
            else if (!focused && focus is not null && mouse.LeftButton == ButtonState.Pressed && !pressedLastFrame)
            {
                focus.OnMouseDown(mouse.Position);
            }
            else if (!focused)
            {
                focus = null;
            }

            pressedLastFrame = mouse.LeftButton == ButtonState.Pressed;
            posLastFrame = mouse.Position;
        }

    }
}
