using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Zenoid
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        enum GameState
        {
            Intro,
            MainScreen,
            IntroMenu,
            GamePlay,
            Pause,
            SaveGame,
            LoadGame,
            OptionMenu,
            LevelUpScreen,
            Stats_Weapons
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D enemySpriteSheet, thrusterSpriteSheet;
        Texture2D lifeBar_texture;
        Random random;
 
        SpriteFont debugFont;
        SpriteFont hitFont;

        String hit = "";

        //The main player
        Player player;
        Weapons weapons;
        HUD hud;

        //Test Enemy Test hit fonts
        List<Enemy> enemies;
        List<FloatingText> hitTexts;

        KeyboardState currentState, prevState;
        Texture2D COLLISIONOBVIOUS;
        Rectangle COLLISIONRECTANGLE;

        //setting const non changing variables
        const int SCREEN_WIDTH= 800;
        const int SCREEN_HEIGHT = 600;

        //The state that the game is in
        GameState gameState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
         
            //Init keyboard states
            prevState = currentState = Keyboard.GetState();

            //Set state to gameplay since nothing else has been implemented yet
            gameState = GameState.GamePlay;

            //Initialize the list
            enemies = new List<Enemy>();
            hitTexts = new List<FloatingText>();

            random = new Random(DateTime.Now.Millisecond);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //Debugging string
            debugFont = Content.Load<SpriteFont>("DebugFont");
            hitFont = Content.Load<SpriteFont>("PlainFont");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //load enemy texture and thruster texture
            enemySpriteSheet = Content.Load<Texture2D>("Enemy");
            thrusterSpriteSheet = Content.Load<Texture2D>("thrust");

            //Load player texture
            player = new Player(Content.Load<Texture2D>("Ships"),
                                thrusterSpriteSheet,
                                new Vector2(SCREEN_WIDTH/2,SCREEN_HEIGHT/1.5f));

            //Test enemy
            enemies.Add(new Enemy(enemySpriteSheet,thrusterSpriteSheet,
                                new Vector2(200, 0), 0, player.getPosition(), SCREEN_WIDTH, SCREEN_HEIGHT,3));

            //Weapons
            weapons = new Weapons(Content.Load<Texture2D>("weapons"), SCREEN_WIDTH, SCREEN_HEIGHT);

            //lifebars
            lifeBar_texture = Content.Load<Texture2D>("hudBars");

            //HUD
            hud = new HUD(lifeBar_texture);

            COLLISIONOBVIOUS = new Texture2D(GraphicsDevice, 1, 1);
            COLLISIONOBVIOUS.SetData(new Color[] { Color.White });
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //New state
            currentState = Keyboard.GetState();
            random.Next();

            if (currentState.IsKeyUp(Keys.Escape) && prevState.IsKeyDown(Keys.Escape))
                this.Exit();

            switch (gameState)
            {
                case GameState.GamePlay:

                    if (currentState.IsKeyDown(Keys.R) /*&& prevState.IsKeyUp(Keys.R)*/)
                        enemies.Add(new Enemy(enemySpriteSheet, thrusterSpriteSheet,
                                new Vector2(600, 0), 0, player.getPosition(), SCREEN_WIDTH, SCREEN_HEIGHT,4));

                    if (currentState.IsKeyDown(Keys.E)/* && prevState.IsKeyUp(Keys.E)*/)
                        enemies.Add(new Enemy(enemySpriteSheet, thrusterSpriteSheet,
                                new Vector2(200, 0), 0, player.getPosition(), SCREEN_WIDTH, SCREEN_HEIGHT,5));


                    enemyUpdateBulletCollisions(gameTime);
                    weapons.Update(gameTime, player.getPosition(), prevState, currentState,player);
                    if (!player.isDead)
                        player.Update(gameTime, prevState, currentState);
                    hud.Update(gameTime, player);
                    updateHitFloatingText(gameTime);
                    break;
            }

            
            //prev state
            prevState = currentState;

            base.Update(gameTime);
        }
        //Updates the floating damage counters
        protected void updateHitFloatingText(GameTime gameTime)
        {
            for (int i = hitTexts.Count - 1; i >= 0; i--)
            {
                FloatingText text = hitTexts[i];
                if (!text.stillShow())
                    hitTexts.Remove(text);
                text.Update(gameTime);
            }
            
        }

        protected void enemyUpdateBulletCollisions(GameTime gameTime)
        {
            List<Bullet> shotsOnScreen = weapons.getShotsFired();

            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy temp = enemies.ElementAt(i);
                enemies.RemoveAt(i);
                temp.Update(gameTime);
                if (temp.offScreen())
                {
                    i--;
                    continue;
                }
           
                for (int j = shotsOnScreen.Count-1 ; j >= 0; j--)
                {
                    Bullet mainBullet = shotsOnScreen[j];
                    List<Vector2> bullet = mainBullet.bulletVectors;
                   
                    for (int k = bullet.Count - 1; k >= 0; k--)
                        if (isColliding(temp.getCollisionBox(), bullet[k], weapons.weaponWidth, weapons.weaponHeight))
                        {
                            //DEBUG Data
                            //hit = temp.getCollisionBox() + " \n" + temp.getPosition() + "\n" + bullet[k] + " W: " + weapons.weaponWidth + " H: " + weapons.weaponHeight;
                            int damage = mainBullet.damage  + (random.Next() % mainBullet.damage);
                            
                            if(!mainBullet.isPiercing || (mainBullet.isPiercing && !temp.isHit)){
                                temp.hitEnemy(damage,mainBullet.isPiercing);
                                hitTexts.Add(new FloatingText(damage, temp.getPosition()));
                            }
                            if(!mainBullet.isPiercing){
                            bullet.RemoveAt(k);
                            break;
                                }
                        }
                    if (bullet.Count == weapons.emptyChamber) 
                    {
                        weapons.removeCurrentShot(mainBullet.bulletType);
                        shotsOnScreen.RemoveAt(j);
                        break;
                    }
                    if (temp.isDead)
                        break;
                }
                if (temp.isDead)
                {
                    player.exp += temp.giveExp;
                    continue;
                }
                if (temp.getCollisionBox().Intersects(player.destRect) && !player.isDamaged && !player.isDead)
                {
                    player.isHit(temp.hitDmg);
                    hitTexts.Add(new FloatingText(temp.hitDmg, player.center));
                }
                enemies.Insert(i, temp);
            }
        }
        protected bool isColliding(Rectangle rect, Rectangle rect2)
        {
            return rect.Intersects(rect2);
        }
        //assumes the vector is in the middle of the object for this calculation
        protected bool isColliding(Rectangle rect, Vector2 position, int width, int height)
        {
            Rectangle rect2 = new Rectangle((int)(position.X - width/2), (int)(position.Y), width, height);
            return rect.Intersects(rect2);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            //Drawing the player and the bullets
            weapons.Draw(gameTime, spriteBatch, debugFont);
            if(!player.isDead)
                player.Draw(gameTime, spriteBatch/*, COLLISIONOBVIOUS, COLLISIONRECTANGLE*/);
           
            //debugging
            spriteBatch.DrawString(debugFont, "Number of enemies: " + enemies.Count + "\nLast Hit:" + hit, new Vector2(0, 200), Color.White);

            //draws the enemies
            foreach (Enemy b in enemies)
                b.Draw(gameTime, spriteBatch,debugFont/*, COLLISIONOBVIOUS,COLLISIONRECTANGLE*/);
            hud.Draw(gameTime, spriteBatch,debugFont,player);
            foreach (FloatingText hitMarker in hitTexts)
                hitMarker.Draw(spriteBatch, hitFont);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
