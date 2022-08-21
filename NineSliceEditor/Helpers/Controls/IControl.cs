using Microsoft.Xna.Framework;
using System.Windows.Input;

namespace NineSliceEditor.Controls
{
    internal interface IControl
    {
        bool CapturesMouseClick(Point mouse_pos);
        Cursor CursorType(Point mouse_pos);
        void OnMouseDown(Point mouse_pos);
        void OnMouseMove(Point old_pos, Point new_pos);
        void OnMouseUp(Point mouse_pos);
    }
}