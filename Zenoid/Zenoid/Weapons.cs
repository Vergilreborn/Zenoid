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
    class Weapons
    {

        //constant gun types
        const int Normal = 0;
        const int ShotGun = 1;
        const int Plasma = 2;
        const int Wave = 3;
        const int Missile = 4;
        const int Laser = 5;
        const int noShotsFire = 0;
        
        //gun stats
        const int MaxNumOfGuns = 6;
        const int ShootSpeed = 8;

        //weapon width
        public int weaponWidth = 20;
        public int weaponHeight = 40;

        //Const variables
        public int emptyChamber = 0;

        //screen information
        int screenWidth, screenHeight;

        //max shots for each bullet set
        int normalMaxShots = 200;//8;
        int missileMaxShots = 200;// 2;
        int shotgunMaxShots = 200;//2;
        int laserMaxShots = 200;//1;
        int plasmaMaxShots = 200;//6;
        int waveMaxShots = 200;//8;

        //ability to shoot 
        bool ableToShoot = true;

        //const time able to fire
        const float pauseBetweenShotTimer= 125f;
        const float resetTimer = 0f;

        //shoot timer
        float shootTimer = 0f;

        //each bullet type shot
        int normalShotsFired, missileShotsFired, shotgunShotsFired, 
            laserShotsFired, plasmaShotsFired, waveShotsFired;
        
        //bullets that have been shot
        List<Bullet> bulletsFire;

        //Weapons Texture
        Texture2D weaponSheet;

        //selected weapon
        int currentGun;

        public Weapons(Texture2D weaponSheet, int screenWidth, int screenHeight)
        {
            //connecting the sprite sheet
            this.weaponSheet = weaponSheet;

            //setting the current gun to the normal gun
            currentGun = Normal;

            //initializing the bullets list
            bulletsFire = new List<Bullet>();

            //nothing was shot when creating the weapon
            normalShotsFired = missileShotsFired = shotgunShotsFired = 
            laserShotsFired = plasmaShotsFired = waveShotsFired = noShotsFire;

            //connect screen data
            this.screenHeight = screenHeight;
            this.screenWidth = screenWidth;

        }


        public void Update(GameTime timer, Vector2 position, KeyboardState prev, KeyboardState curr,Player player)
        {


            //check to see if the spacebar is pressed
            if (curr.IsKeyDown(Keys.Z) && !player.isDead)
                if (ableToShoot)
                {
                    shoot(currentGun, position,player.level);
                    ableToShoot = !ableToShoot;
                }

            //if the player has shot already then the player must wait
            //before able to shoot again. (Small wait period not noticable)
            if (!ableToShoot)
            {
                shootTimer += timer.ElapsedGameTime.Milliseconds;
                if (shootTimer > pauseBetweenShotTimer)
                {
                    ableToShoot = !ableToShoot;
                    shootTimer = resetTimer;
                }
            }
            

            //Debug purposes to change weapons
            if (curr.IsKeyDown(Keys.S) && prev.IsKeyUp(Keys.S))
                currentGun = (currentGun + 1) % 6;



            //update the bullets
            for (int j = bulletsFire.Count-1; j >= 0; j--)
            {
                Bullet bullet = bulletsFire[j];
                //for each bullet of anything
                bullet.timer -= timer.ElapsedGameTime.Milliseconds;
                if (bullet.timer < 0)
                {
                    bullet.timer = 100f;
                    bullet.animation += 1;
                    bullet.animation %= 2;
                    bullet.source.Y = weaponHeight * bullet.animation;
                }

                for (int i = 0; i < bullet.bulletVectors.Count; i++)
                {
                    if (offScreenBullet(bullet.bulletVectors.ElementAt(i)))
                    {
                        //check to see if the list is empty then remove the currentbullet shot
                        bullet.bulletVectors.RemoveAt(i);
                        i--;
                        if (bullet.bulletVectors.Count == emptyChamber)
                        {
                            removeCurrentShot(bullet.bulletType);
                            bulletsFire.Remove(bullet);
                        }
                    }
                        //otherwise update the bullet positions and trajectory
                    else
                    {
                     Vector2 temp =  bullet.bulletVectors.ElementAt(i);
                     bullet.bulletVectors.RemoveAt(i);
                     temp.Y -= ShootSpeed;
                     bullet.bulletVectors.Insert(i,temp);
                    }
                }
            }

        }

        public List<Bullet> getShotsFired()
        {
            return bulletsFire;
        }

        //remove the bullet that is offscreen
        public void removeCurrentShot(int bulletType)
        {
            switch (bulletType)
            {
                case Normal: normalShotsFired--; break;
                case ShotGun: shotgunShotsFired--; break;
                case Laser: laserShotsFired--;  break;
                case Wave: waveShotsFired--; break;
                case Missile: missileShotsFired--;  break;
                case Plasma: plasmaShotsFired--;  break;
            }

        }
        //checks to see if the bullet went of the screen
        public bool offScreenBullet(Vector2 position)
        {
            //specific to the bullet offset because the way the bullet is drawn
            return position.X < 0 || position.Y+weaponHeight/2 < 0 || position.Y > screenHeight || position.X > screenWidth;  
        }

        //When the player shoots we must get the currentBullet type and the position in which
        //the player shot and represent the bullets accordingly
        public void shoot(int currentBulletType, Vector2 position, int level){

            switch(currentBulletType){
                    //normal bullet is shot
               case Normal:
                    if (normalShotsFired < normalMaxShots)
                    {
                        bulletsFire.Add(new Bullet(Normal, position, ShootSpeed, level));
                        normalShotsFired++;
                    }
                     
                     break;
                    //plasma bullet is shot
               case Plasma:
                     if (plasmaShotsFired < plasmaMaxShots)
                     {
                         bulletsFire.Add(new Bullet(Plasma, position, ShootSpeed, level));
                         plasmaShotsFired++;
                     }
                    break;
                    //wave bullet is shot
               case Wave:
                    if (waveShotsFired < waveMaxShots)
                    {
                        bulletsFire.Add(new Bullet(Wave, position, ShootSpeed, level));
                        waveShotsFired++;
                    }
                     break;
                    //the laser bullet is shot
               case Laser:
                     if (laserShotsFired < laserMaxShots)
                     {
                         bulletsFire.Add(new Bullet(Laser, position, ShootSpeed, level));
                         laserShotsFired++;
                     }
                     break;
                    //shotgun fired
               case ShotGun:
                     if (shotgunShotsFired < shotgunMaxShots)
                     {
                         bulletsFire.Add(new Bullet(ShotGun, position, ShootSpeed, level));
                         shotgunShotsFired++;
                     }
                     break;
                    //missiles shot
               case Missile:
                     if (missileShotsFired < missileMaxShots)
                     {
                         bulletsFire.Add(new Bullet(Missile, position, ShootSpeed, level));
                         missileShotsFired++;
                     }
                    break;

            }
        }

        //Drawing the bullets on the screen
        public void Draw(GameTime timer, SpriteBatch spriteBatch, SpriteFont font)
        {
            //for all the bullets fired
            //get the weapon type and then draw them in the correct position
            foreach (Bullet b in bulletsFire)
            {
                foreach (Vector2 pos in b.bulletVectors)
                {
                    Vector2 temp = pos;
                    temp.X -= weaponWidth/2;
                    spriteBatch.Draw(weaponSheet, temp, b.source, Color.White);
                }
                
            }

            //DEBUGGING PURPOSES 
            //shows what weapon is equipped and 
            //how many of the equipped weapon was shot
            String weaponNow = "";
            switch (currentGun)
            {
                case Normal: weaponNow= "Normal"; break;
                case ShotGun: ; weaponNow = "ShotGun"; break;
                case Laser: weaponNow = "Laser"; break;
                case Wave: weaponNow = "Wave"; break;
                case Missile: weaponNow = "Missile"; break;
                case Plasma: weaponNow = "Plasma"; break;
            }

            spriteBatch.DrawString(font, "Bullets: " + " CurrentWeapon <" + weaponNow + ">" 
                                                    + "\nNormal:  " + normalShotsFired + "/" + normalMaxShots +
                                                      "\nMissile: " + missileShotsFired + "/" + missileMaxShots +
                                                      "\nWave:    " + waveShotsFired + "/" + waveMaxShots +
                                                      "\nPlasma:  " + plasmaShotsFired + "/" + plasmaMaxShots +
                                                      "\nLaser:   " + laserShotsFired + "/" + laserMaxShots +
                                                      "\nShotGun: " + shotgunShotsFired + "/" + shotgunMaxShots, Vector2.Zero, Color.White);
        }
    }
}
