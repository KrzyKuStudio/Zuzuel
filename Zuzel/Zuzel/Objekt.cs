using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Zuzel
{
    public class Objekt
    {
        #region Fields

        // graphic and drawing info
        protected Texture2D sprite;
        protected Rectangle drawRectangle;
        protected Vector2 velocity;
        protected Color[] textureData;


        // Stats
        bool active;

        // shooting support
        //none

        // sound effect
        protected SoundEffect shootSound;

        #endregion

        #region Constructors

        /// <summary>
        ///  Constructs a objekt
        /// </summary>
        /// 
        /// <param name="contentManager">the content manager for loading content</param>
        /// <param name="spriteName">the sprite name</param>
        /// <param name="x">the x location of the center of the burger</param>
        /// <param name="y">the y location of the center of the burger</param>
        /// <param name="shootSound">the sound the burger plays when shooting</param>
        public Objekt(ContentManager contentManager, string spriteName, int x, int y, Vector2 velocity,
            SoundEffect shootSound)
        {
            LoadContent(contentManager, spriteName, x, y);
            this.shootSound = shootSound;
            this.active = true;
            this.velocity = velocity;
           
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collision rectangle for the objekt
        /// </summary>
        public Rectangle CollisionRectangle
        {
            get { return drawRectangle; }
        }
        /// <summary>
        /// Gets and sets active flag
        /// </summary>
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }
        /// <summary>
        /// Gets, Sets texturecolor data for objekt
        /// </summary>
        public Color[] TextureData
        {
            get { return textureData; }
            set { textureData = value; }
        }

        /// <summary>
        /// Gets and sets the velocity 
        /// </summary>
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        /// <summary>
        /// Gets and sets the draw rectangle 
        /// </summary>
        public Rectangle DrawRectangle
        {
            get { return drawRectangle; }
            set { drawRectangle = value; }
        }
        #endregion

        #region Private properties

        /// <summary>
        /// Gets and sets the x location of the center of the burger
        /// </summary>
        protected int X
        {
            get { return drawRectangle.Center.X; }
            set
            {
                drawRectangle.X = value - drawRectangle.Width / 2;

                // clamp to keep in range
                if (drawRectangle.X < 0)
                {
                    drawRectangle.X = 0;
                }
                else if (drawRectangle.X > GameConstants.WINDOW_WIDTH - drawRectangle.Width)
                {
                    drawRectangle.X = GameConstants.WINDOW_WIDTH - drawRectangle.Width;
                }
            }
        }

        /// <summary>
        /// Gets and sets the y location of the center of the burger
        /// </summary>
        protected int Y
        {
            get { return drawRectangle.Center.Y; }
            set
            {
                drawRectangle.Y = value - drawRectangle.Height / 2;

                // clamp to keep in range
                if (drawRectangle.Y < 0)
                {
                    drawRectangle.Y = 0;
                }
                else if (drawRectangle.Y > GameConstants.WINDOW_HEIGHT - drawRectangle.Height)
                {
                    drawRectangle.Y = GameConstants.WINDOW_HEIGHT - drawRectangle.Height;
                }
            }
        }


        #endregion

        #region Public methods
        /// <summary>
        /// Updates the objektlocation location
        /// </summary>
        /// <param name="gameTime">game time</param>

        public void Update(GameTime gameTime)
        {
            if (Active == true)
            {
                drawRectangle.X += (int)velocity.X;
                drawRectangle.Y += (int)velocity.Y;
            }
        }

        /// <summary>
        /// Draws the objekt
        /// </summary>
        /// <param name="spriteBatch">the sprite batch to use</param>
        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(sprite, drawRectangle, Color.White);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Loads the content for the objekt
        /// </summary>
        /// <param name="contentManager">the content manager to use</param>
        /// <param name="spriteName">the name of the sprite for the objekt</param>
        /// <param name="x">the x location of the center of the objekt</param>
        /// <param name="y">the y location of the center of the objekt</param>
        private void LoadContent(ContentManager contentManager, string spriteName,
            int x, int y)
        {
            // load content and set remainder of draw rectangle
            sprite = contentManager.Load<Texture2D>(spriteName);
            drawRectangle = new Rectangle(x - sprite.Width / 2,
                y - sprite.Height / 2, sprite.Width,
                sprite.Height);

            TextureData = new Color[sprite.Width * sprite.Height];
            this.sprite.GetData(TextureData);
        }

        protected void ClampInWindow()
        {
            // clamp objekt in window
            if (drawRectangle.Left < 0)
            {
                drawRectangle.X = 0;
            }
            else if (drawRectangle.Right > GameConstants.WINDOW_WIDTH)
            {
                drawRectangle.X = GameConstants.WINDOW_WIDTH - drawRectangle.Width;
            }
            if (drawRectangle.Top < 0)
            {
                drawRectangle.Y = 0;
            }
            else if (drawRectangle.Bottom > GameConstants.WINDOW_HEIGHT)
            {
                drawRectangle.Y = GameConstants.WINDOW_HEIGHT - drawRectangle.Height;
            }

        }
        #endregion
    }


}
