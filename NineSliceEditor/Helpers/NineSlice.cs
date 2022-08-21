using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSliceEditor.Helpers
{
    public class NineSlice
    {
        Texture2D sprite;
        public Rectangle center;

        int LWidth => center.X;
        int RWidth => sprite.Width - center.Right;
        int THeight => center.Y;
        int BHeight => sprite.Height - center.Bottom;

        public NineSlice(Texture2D sprite, Rectangle center)
        {
            this.sprite = sprite;
            this.center = center;
        }

        public void Draw(SpriteBatch batch, Rectangle target)
        {
            Point tl_center = target.Location + center.Location;
            Point br_center = new(target.Right - RWidth, target.Bottom - BHeight);
            Rectangle scaled_center = new(tl_center, br_center - tl_center);

            //TL
            batch.Draw(sprite, new Rectangle(target.X, target.Y, LWidth, THeight), new Rectangle(0, 0, LWidth, THeight), Color.White);
            //TR
            batch.Draw(sprite, new Rectangle(scaled_center.Right, target.Y, RWidth, THeight), new Rectangle(center.Right, 0, RWidth, THeight), Color.White);
            //BL
            batch.Draw(sprite, new Rectangle(target.X, scaled_center.Bottom, LWidth, BHeight), new Rectangle(0, center.Bottom, LWidth, BHeight), Color.White);
            //BR
            batch.Draw(sprite, new Rectangle(br_center, new(RWidth, BHeight)), new Rectangle(center.Right, center.Bottom, RWidth, BHeight), Color.White);

            //Left
            batch.Draw(sprite, new Rectangle(target.X, scaled_center.Top, LWidth, scaled_center.Height), new Rectangle(0, center.Y, center.X, center.Height), Color.White);

            //Right
            batch.Draw(sprite, new Rectangle(scaled_center.Right, scaled_center.Top, RWidth, scaled_center.Height), new Rectangle(center.Right, center.Top, RWidth, center.Height), Color.White);

            //Top
            batch.Draw(sprite, new Rectangle(scaled_center.Left, target.Y, scaled_center.Width, THeight), new Rectangle(center.Left, 0, center.Width, THeight), Color.White);

            //Bottom
            batch.Draw(sprite, new Rectangle(scaled_center.Left, scaled_center.Bottom, scaled_center.Width, BHeight), new Rectangle(center.Left, center.Bottom, center.Width, BHeight), Color.White);

            //center
            batch.Draw(sprite, scaled_center, center, Color.White);
        }
    }
}
