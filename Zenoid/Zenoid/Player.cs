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
    class Player
    {
       
        //CONST VARIABLES
        const int shipSize = 64;
        const int startShip = 10;
        const int maxLevel = 5;
        const int shipSpeed = 3;
        const int noStat = 0;
        const int thrusterSize = 16;
        const float resetTimer = 0f;
        
        //player stats
        public int maxHealth;
        public int health;
        public int level;
        public int exp;

        //Animation data
        const int maxThrusterAnimationFrame = 2;
        const float thrusterChangeAnimation = 75f;
        int thrusterFrame = 0;
        float thrusterAnimationTimer = 0f;

        //Timers
        float invincibleTimer = 0f;

        //special ship bonuses
        //heal ship
        const int healShipStart = 2;
        const int healShipEnd = 4;
        const int healShipHeal = 2;
        //bash ship
        const int bashShipStart = 7;
        const int bashShipEnd = 9;
        const int bashShipDamage = 5;
        //extra damage ship
        const int damageShipStart = 12;
        const int damageShipEnd = 14;
        const int damageShipDamage = 2;
        //extra defense ship
        const int defShipStart = 17;
        const int defShipEnd = 19;
        const int defShipDef = 2;
        //speed ship
        const int speedShipStart = 22;
        const int speedShipEnd = 24;
        const int speedShipSpeed = 1;
        
        //starting health
        const int  startinghealth= 200;

        //Extra damage,speed,def,bash,heal depending on ship
        int speedPlus, damagePlus, defPlus, bashPlus, healPlus;

        //Holds the ship sprites and thrusters
        Texture2D ships, thruster;

        //Rectangle 
        public Rectangle destRect; //for destination on the screen
        Rectangle sourceRect; //for the current ship that we are using

        //Thruster Data
        Rectangle thrusterSourceRect;


        //Get the currentship
        int currentShip;
        
        //Player center
        public Vector2 center;

        //Player status
        public bool isDead;
        public bool isDamaged;
       

        public Player(Texture2D ships,Texture2D thrusts, Vector2 startPosition)
        {
            //connect the textures for global use when updating
            this.ships = ships;

            //connect thrusters
            this.thruster = thrusts;

            //settin default start ship
            currentShip = startShip;

            //set to default options
            destRect = new Rectangle((int)(startPosition.X - shipSize / 2),
                                     (int)(startPosition.Y), shipSize, shipSize);

            //Center of the ship
            center = new Vector2(destRect.X + shipSize / 2, destRect.Y + shipSize / 2);

            //get the correct source of the ship we wish to show on the screen
            sourceRect = new Rectangle((currentShip % maxLevel) * shipSize, (currentShip / maxLevel) * shipSize,
                                         shipSize, shipSize);

            //Thruster source rectangle initialization
            thrusterSourceRect = new Rectangle(0, 0, thrusterSize, thrusterSize);


            //no extra stats when initializing the ship
            speedPlus = damagePlus = defPlus = bashPlus = healPlus = noStat;

            //isdead = false;
            isDead = isDamaged = false;

            //initiate health
            this.maxHealth = startinghealth;
            this.health = startinghealth;
            level = 1;
            exp = noStat;
        }

        public void changeShip(int newShip)
        {
            //setting the current ship
            currentShip = newShip;

            //reset all additional stast
            speedPlus = damagePlus = defPlus = bashPlus = healPlus = noStat;

            //Depending on the ship we add the correct stats. 
            if (speedShipStart <= newShip && newShip <= speedShipEnd)
                speedPlus = speedShipSpeed;
            if (damageShipStart <= newShip && newShip <= damageShipEnd)
                damagePlus = damageShipDamage;
            if (defShipStart <= newShip && newShip <= defShipEnd)
                defPlus = defShipDef;
            if (bashShipStart <= newShip && newShip <= bashShipEnd)
                bashPlus = bashShipDamage;
            if (healPlus <= newShip && newShip <= healPlus)
                healPlus = healShipHeal;

            //Update the source rectangle
            sourceRect = new Rectangle((currentShip % maxLevel) * shipSize, (currentShip / maxLevel) * shipSize,
                             shipSize, shipSize);
        }
    
        public void Update(GameTime timer, KeyboardState prev, KeyboardState curr)
        {
            if (isDamaged)
            {
                invincibleTimer -= timer.ElapsedGameTime.Milliseconds;
                if (invincibleTimer < 0f)
                    isDamaged = false;
            }


            if (curr.IsKeyDown(Keys.A) && prev.IsKeyUp(Keys.A))
                changeShip((currentShip+1) % 25);

            //Moving in a certain direction depending on movement
            if (curr.IsKeyDown(Keys.Up))
                destRect.Y -= shipSpeed + speedPlus;

            if (curr.IsKeyDown(Keys.Down))
                destRect.Y += shipSpeed + speedPlus;

            if (curr.IsKeyDown(Keys.Left))
                destRect.X -= shipSpeed + speedPlus;

            if (curr.IsKeyDown(Keys.Right))
                destRect.X += shipSpeed + speedPlus;


            //update the center of the spaceship
            center = new Vector2(destRect.X + shipSize / 2, destRect.Y + shipSize / 2);

        }

        public Vector2 getPosition()
        {
            return center;
        }

        public void Draw(GameTime timer, SpriteBatch spriteBatch/*, Texture2D OBVIOUSCOLLISION, Rectangle OBVIOUSBOX*/)
        {
            //update the thruster animation
            updateThrusterAnimation(timer);

            //setposition for thruster to be drawn
            Vector2 thrusterPosition = new Vector2(destRect.X + shipSize/2 - thrusterSize/2,destRect.Y + shipSize - 1);

            //OBVIOUSBOX = destRect;
            //spriteBatch.Draw(OBVIOUSCOLLISION, OBVIOUSBOX, Color.Red);

            //draw the sprites ,thruster,ships
            spriteBatch.Draw(thruster, thrusterPosition, thrusterSourceRect, Color.White);
            spriteBatch.Draw(ships, destRect, sourceRect, Color.White);
        }

        public void updateThrusterAnimation(GameTime timer)
        {
            //update the thruster animation timer
            thrusterAnimationTimer += timer.ElapsedGameTime.Milliseconds;

            //Thruster animation check
            if (thrusterAnimationTimer > thrusterChangeAnimation)
            {
                thrusterAnimationTimer = resetTimer;
                thrusterFrame = (thrusterFrame  + 1) % maxThrusterAnimationFrame;
                thrusterSourceRect.X = thrusterFrame * thrusterSize;
    
            }
        }

        public void isHit(int damage)
        {
            isDamaged = true;
            health -= damage;
            invincibleTimer = 250f;
            if (health <= 0)
            {
                isDead = true;
                health = 0;
            }
        }
    }
}
