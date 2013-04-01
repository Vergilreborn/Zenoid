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
    class Bullet
    {
        //Bullet main variables
        public List<Vector2> bulletVectors;
        public int bulletType;
        //public int rotationAngle;

        //Bullet types
        const int Normal = 0;
        const int ShotGun = 1;
        const int Plasma = 2;
        const int Wave = 3;
        const int Missile = 4;
        const int Laser = 5;

        const int missileOffset = 20;
        const int shotGunXOffset = 30;
        const int shotGunYOffset = 30;

        public int damage;
        public bool isPiercing;
        public float timer = 100f;
        public int animation = 0;
        //source Rect for the bullet
        public Rectangle source;

        //for the enemies
        public Bullet(int bulletType, Vector2  playerPos, Vector2 trajectory, float bulletSpeed)
        {
        }
        
        //Bullet class holds onto the bullet type for the main player
        public Bullet(int bulletType, Vector2 position, float bulletSpeed, int playerlevel)
        {
            this.bulletType = bulletType;
            bulletVectors = new List<Vector2>();
            double c = 0;
            double b = 0;
            isPiercing = false;
            //Depending on the bullet type the image of the bullet will be shown
            switch (bulletType)
            {
                    //Shooting a normal bullet
                case Normal:
                    source = new Rectangle(23, 1, 20, 40);
                    bulletVectors.Add(position);
                    c = 2;
                    b = 5;
                    break;
                    //plasma fire position
                case Plasma:
                    source = new Rectangle(45, 1, 20, 40);
                    bulletVectors.Add(position);
                    c = 3;
                    b = 1;
                    break;
                    //wave fire position
                case Wave:
                    source = new Rectangle(67, 1, 20, 40);
                    bulletVectors.Add(position);
                    c = 2;
                    b = 3;
                    break;

                    //laser fire position
                case Laser:
                    source = new Rectangle(111, 1, 20, 40);
                    bulletVectors.Add(position);
                    c = 3;
                    b = 1;
                    isPiercing = true;
                    break;

                    //shotgun positions when fired
                case ShotGun:
                    source = new Rectangle(89, 1, 20, 40);
                    bulletVectors.Add(new Vector2(position.X - shotGunXOffset, position.Y));
                    bulletVectors.Add(new Vector2(position.X + shotGunXOffset, position.Y));
                    bulletVectors.Add(new Vector2(position.X , position.Y));
                    bulletVectors.Add(new Vector2(position.X + shotGunXOffset/2, position.Y + shotGunYOffset));
                    bulletVectors.Add(new Vector2(position.X - shotGunXOffset/2, position.Y + shotGunYOffset));
                    c = 5;
                    b = 2;
                    break;
               
               
                case Missile:
                    source = new Rectangle(1,1,20,40);
                    bulletVectors.Add(new Vector2(position.X - missileOffset, position.Y));
                    bulletVectors.Add(new Vector2(position.X + missileOffset, position.Y));
                    c = 8;
                    b = 4;
                    break;

            }
            damage = (int)(Math.Pow(playerlevel / c, 2) + b);
        }

    }
}
