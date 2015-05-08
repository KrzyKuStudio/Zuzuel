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

namespace Zuzel
{

/// <summary>
/// This is the main type for your game
/// </summary>
public class Game1 : Microsoft.Xna.Framework.Game
{
    GraphicsDeviceManager graphics;

    // The color data for the images; used for per pixel collision
    Texture2D mapGradTexture;
    Texture2D mapTexture;
    Rectangle mapRectangle;
    Vector2 mapPosition;
    Color[] mapGradTextureData;


    Motor motorRed;
    Motor motorGreen;
    Motor motorYellow;
    Motor motorBlue;
    List<Motor> motors = new List<Motor>();

    //Gamestate
    public enum GameState
    {
        NewGame,
        Playing,
        GameOver,
        StartGame
    }

    GameState gameState;

    // The images will be drawn with this SpriteBatch
    SpriteBatch spriteBatch;

    Random random = new Random();

    // The sub-rectangle of the drawable area which should be visible on all TVs
    Rectangle safeBounds;
    // Percentage of the screen on every side is the safe area
    const float SafeAreaPortion = 0.01f;


    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        IsMouseVisible = GameConstants.VISIBLEMOUSE;
        // set resolution
        graphics.PreferredBackBufferWidth = GameConstants.WINDOW_WIDTH;
        graphics.PreferredBackBufferHeight = GameConstants.WINDOW_HEIGHT;
    }


    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to
    /// run. This is where it can query for any required services and load any
    /// non-graphic related content.  Calling base.Initialize will enumerate through
    /// any components and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
        base.Initialize();
        gameState = GameState.NewGame;
        
        // Calculate safe bounds based on current resolution
        Viewport viewport = graphics.GraphicsDevice.Viewport;
        safeBounds = new Rectangle(
            (int)(viewport.Width * SafeAreaPortion),
            (int)(viewport.Height * SafeAreaPortion),
            (int)(viewport.Width * (1 - 2 * SafeAreaPortion)),
            (int)(viewport.Height * (1 - 2 * SafeAreaPortion)));
       
       
    }


    /// <summary>
    /// Load your graphics content.
    /// </summary>
    protected override void LoadContent()
    {

        // Create a sprite batch to draw those textures
        spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
        
        //personTexture = Content.Load<Texture2D>("image\\motorred");
        mapTexture = Content.Load<Texture2D>("image\\map");
        mapGradTexture = Content.Load<Texture2D>("image\\mapgrad");
        mapGradTextureData = new Color[mapGradTexture.Width * mapGradTexture.Height];
        mapGradTexture.GetData(mapGradTextureData);

        // Get the bounding rectangle of this block
        mapRectangle = new Rectangle(0, 0, mapTexture.Width, mapTexture.Height);
        
    }


    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
        // Get input
        KeyboardState keyboard = Keyboard.GetState();
        GamePadState gamePad = GamePad.GetState(PlayerIndex.One);

        // Allows the game to exit
        if (gamePad.Buttons.Back == ButtonState.Pressed ||
            keyboard.IsKeyDown(Keys.Escape))
        {
            this.Exit();
        }
        //new game
        if (keyboard.IsKeyDown(Keys.N))
        {
            gameState = GameState.NewGame;
        }


        //new game
        #region
        if (gameState == GameState.NewGame)
        {
            motors.Clear();
           
            motorRed = new Motor(Content, "image\\motorred", (int)GameConstants.START_POS_RED.X,
            (int)GameConstants.START_POS_RED.Y, new Vector2(0, 0), null);
            motors.Add(motorRed);
            
            motorYellow = new Motor(Content, "image\\motoryellow", (int)GameConstants.START_POS_RED.X,
           (int)GameConstants.START_POS_RED.Y+30, new Vector2(0, 0), null);
            motors.Add(motorYellow);




            gameState = GameState.Playing;
        }
        #endregion

        //game playing
        #region
        if (gameState == GameState.Playing)
        {
            foreach(Motor motorek in motors)
            {
                motorek.AngleVelocity = 0.0F;
                motorek.Turning = false;
            }

            // Move the player 
            #region
            //RED
            if (keyboard.IsKeyDown(Keys.Left) )
            {
                motorRed.AngleVelocity =  -GameConstants.MOTOR_ANGLE;
                motorRed.Turning = true;

            }
            if (keyboard.IsKeyDown(Keys.Right) )
            {
                motorRed.AngleVelocity = GameConstants.MOTOR_ANGLE;
                motorRed.Turning = true;

            }
            if (keyboard.IsKeyDown(Keys.Up) )
            {
                motorRed.Thrust = true;
            }
            else
            {
                motorRed.Thrust = false;
            }

            //YELLOW
            if (keyboard.IsKeyDown(Keys.A))
            {
                motorYellow.AngleVelocity = -GameConstants.MOTOR_ANGLE;
                motorYellow.Turning = true;

            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                motorYellow.AngleVelocity = GameConstants.MOTOR_ANGLE;
                motorYellow.Turning = true;

            }
            if (keyboard.IsKeyDown(Keys.W))
            {
                motorYellow.Thrust = true;
            }
            else
            {
                motorYellow.Thrust = false;
            }
            #endregion

            // Check collision with mapborder
            foreach (Motor motorek in motors)
            {

                if (Intersect.IntersectPixels(motorek.DrawRectangle, motorek.TextureData,
                                  mapRectangle, mapGradTextureData))
                {
                    gameState = GameState.GameOver;
                    motorek.Active = false;
                }
                else
                {
                    gameState = GameState.Playing;
                }
                motorek.Update(gameTime);
            }

        }
        #endregion

        base.Update(gameTime);
    }


    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice device = graphics.GraphicsDevice;

        spriteBatch.Begin();
        // Draw texture
        spriteBatch.Draw(mapTexture, mapPosition, Color.White);

        // Draw motors
        foreach(Motor motorek in motors)
        {
            motorek.Draw(spriteBatch);
        }
       
        if (gameState == GameState.GameOver)
        {

            spriteBatch.Draw(Content.Load<Texture2D>("image\\motorred"), new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2), Color.White);
        }
       


        spriteBatch.End();


        base.Draw(gameTime);
    }


}}