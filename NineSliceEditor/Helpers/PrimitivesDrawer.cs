using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSliceEditor.Helpers
{
    public class PrimitivesDrawer
    {
        Texture2D tex;

        public PrimitivesDrawer(GraphicsDevice graphics)
        {
            tex = new Texture2D(graphics, 2, 2);
            tex.SetData(new Color[] { Color.White, Color.Transparent, Color.Transparent, Color.White });
        }

        public void DrawGrid(Rectangle location, Color c, SpriteBatch batch)
        {
            batch.Draw(tex, location, new Rectangle(0, 0, location.Width/10, location.Height/10), c);
        }

        public void DrawVert(Point a, int length, Color c, SpriteBatch batch)
        {
            batch.Draw(tex, new Rectangle(a, new Point(1, length)), new Rectangle(0, 0, 1, 1), c);
        }

        public void DrawHoriz(Point a, int length, Color c, SpriteBatch batch)
        {
            batch.Draw(tex, new Rectangle(a, new Point(length, 1)), new Rectangle(0, 0, 1, 1), c);
        }
    }
}
