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
    class HUD
    {

        Texture2D lifeBar_texture;
        Rectangle source_lifeBarOutline;
        Rectangle source_lifeBarColor;
        Rectangle destLifeBar;
        Vector2 location_lifeBar;

        int lifeBarWidth;
        int lifeBarHeight;

        public HUD(Texture2D lifeBar_texture)
        {
            //textures
            this.lifeBar_texture = lifeBar_texture;

            //width height setups
            lifeBarWidth = lifeBar_texture.Width;
            lifeBarHeight = lifeBar_texture.Height / 2;
            
            //rectangle and drawing vectors
            source_lifeBarOutline = new Rectangle(0, 0,lifeBarWidth ,lifeBarHeight );
            source_lifeBarColor = new Rectangle(0, lifeBarHeight, lifeBarWidth, lifeBarHeight);
            location_lifeBar = new Vector2(5, 30);
            destLifeBar = new Rectangle((int)location_lifeBar.X,(int)location_lifeBar.Y, lifeBarWidth, lifeBarHeight);
        }

        public void Update(GameTime gameTime, Player player)
        {
            double doubleOffset = (double)player.health * 1.0 / (double)player.maxHealth;
            destLifeBar.Width = (int)(lifeBarWidth * doubleOffset);

        }

        public void Draw(GameTime gametime, SpriteBatch spriteBatch, SpriteFont font, Player player)
        {
            Vector2 levelPos = new Vector2(5,0);
            Vector2 lifePos = new Vector2(location_lifeBar.X, location_lifeBar.Y + source_lifeBarOutline.Height + 4);
            Vector2 expPos = new Vector2(lifePos.X, lifePos.Y + 20);
            spriteBatch.DrawString(font, "Level: " + player.level ,levelPos, Color.Red); 
            spriteBatch.Draw(lifeBar_texture, destLifeBar, source_lifeBarColor, Color.DarkTurquoise);
            spriteBatch.Draw(lifeBar_texture, location_lifeBar, source_lifeBarOutline, Color.White);
            spriteBatch.DrawString(font, "HP:  " + player.health + " / " + player.maxHealth, lifePos, Color.Red);
            spriteBatch.DrawString(font, "Exp: " + player.exp, expPos, Color.Red);
        }

    }
}
