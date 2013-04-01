using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Zenoid
{
    class FloatingText
    {
        String number;
        Vector2 position;
        float liveTimer = 300f;
        public FloatingText(int number,Vector2 position){
            this.number = number + "";
            this.position = position;
        }
        public void Update(GameTime time)
        {
            liveTimer -= time.ElapsedGameTime.Milliseconds;
            position.Y -= 1;
        }
        public bool stillShow()
        {
            return liveTimer > 0;
        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.DrawString(spriteFont, number, position, Color.Cyan);
        }
    }
}
