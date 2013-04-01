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
    class Enemy
    {

        //enemy width and heigh
        const int width = 40;
        const int height = 40;

        //screen dimensions
        int screenWidth;
        int screenHeight;

        //hitdamage
        public int life = 30;
        public int hitDmg = 8;
        public bool isDead;
        public bool isHit;
        public int giveExp;
        //set up textures for the enemy
        Texture2D spriteSheet, thruster;

        //Source Rectangle and dest rectangle
        Rectangle source, collisionBox;
        Vector2 position;

        //the enemyNumber
        int enemyNumber;

        //List of bullets fired
        List<Bullet> bulletsFired;

        //angle of enemyShip to player
        double angle;

        //Vector2 trajectory
        Vector2 trajectory;

        //Enemy speed
        const int speed = 3;

        float invincibleTimer = 0f;

        //Enemy constructor
        public Enemy(Texture2D spriteSheet, Texture2D thruster, Vector2 startPos, int enemyNumber,
                     Vector2 playerPosition, int screenWidth,int screenHeight, int exp)
        {
            //Setting up the enemy
            this.spriteSheet = spriteSheet;
            this.thruster = thruster;
            this.enemyNumber = enemyNumber;

            //get data from the screen
            this.screenHeight = screenHeight;
            this.screenWidth = screenWidth;

            //setting up source and dest rectangles
            position = startPos;
            collisionBox = new Rectangle((int)(startPos.X), (int)(startPos.Y), width, height);

            //HARD CODED!!!!_________!_!__!_!_!__!_!_!_!_
            source = new Rectangle(0, 0, width, height);

            //Initialize the bullet list
            bulletsFired = new List<Bullet>();

            //calculate trajectory
            double xWidth = startPos.X - playerPosition.X;
            double yHeight = startPos.Y - playerPosition.Y;

            //get angle
            this.angle = Math.Atan(xWidth / yHeight);

            //set tracjectory
            trajectory = new Vector2((float)(Math.Sin(angle) * speed), (float)(Math.Cos(angle) * speed));

            isDead = false;
            isHit = false;
            giveExp = exp;
        }
        public void hitEnemy(int damage, bool isPiercing)
        {

            if (isPiercing)
            {
                isHit = true;
                invincibleTimer = 175f;
            }
            life -= damage;
           
            if (life < 0)
                isDead = true;
        }
        public Rectangle getCollisionBox()
        {
            return collisionBox;
        }

        public void Update(GameTime timer)
        {

            if (isHit)
            {
                invincibleTimer -= timer.ElapsedGameTime.Milliseconds;
                if (invincibleTimer < 0f)
                    isHit = false;
            }

            //update the enemy position
            position += trajectory;
            collisionBox.X = (int)(position.X- width/2);
            collisionBox.Y = (int)(position.Y - height/2);  
        }

        public Vector2 getPosition()
        {
            return position;
        }

        public void Draw(GameTime timer, SpriteBatch spriteBatch , SpriteFont debugFont/*,Texture2D COLLISIONOBVIOUS,Rectangle COLLISIONBOX*/)
        {
            
           
            //setposition for thruster to be drawn
            Vector2 origin = new Vector2(width/2,height/2);

            //COLLISIONBOX = getCollisionBox();
            //spriteBatch.Draw(COLLISIONOBVIOUS, collisionBox, Color.White);

            //draw the sprites ,thruster,ships
            spriteBatch.Draw(spriteSheet, position, source, Color.White, (float)(-angle), origin, 1, SpriteEffects.None, 0);
            
            //debugging
            spriteBatch.DrawString(debugFont, life + "", new Vector2(position.X - 10, position.Y - 40), Color.Violet);
            // spriteBatch.DrawString(debugFont, "TestDummy\nTrajectory = " + trajectory.X + " , " + trajectory.Y + "\nSpeed: " + speed + "\nAngle:" + angle, new Vector2(0, 200), Color.White);  

        }

        //checks to see if the bullet went of the screen
        public bool offScreen()
        {

            //0 is the middle between the positive speed and negative speed
            //as well as the windows start x position
            if (position.Y > screenHeight)
                return true;
            //if we are moving in the positive direction and 
            //the ship is off screen it is off screen
            if (trajectory.X > 0 && position.X > screenWidth)
                return true;
            //if we are moving in the negative direction and
            //the ship is off screen then we are off screen
            if (trajectory.X < 0 && position.X < 0)
                return true;
            return false;
        }
    }
}
