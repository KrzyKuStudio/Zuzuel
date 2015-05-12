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
    Texture2D finishRectangle;
    Rectangle finishMapRectangle;
    
    List<Rectangle> checkPointsList;
    List<Rectangle> checkAiPointsList;
    Motor motorRed;
    Motor motorGreen;
    Motor motorYellow;
    Motor motorBlue;
    List<Motor> motors = new List<Motor>();

    Laps lapsMotorRed;
    Laps lapsMotorGreen;
    Laps lapsMotorYellow;
    Laps lapsMotorBlue;
    List<Laps> laps = new List<Laps>();

    AIMotorMovement aiYellow;
    AIMotorMovement aiBlue;
    AIMotorMovement aiGreen;
    List<AIMotorMovement> aiMovement = new List<AIMotorMovement>(); 
    string winner;
    Motor winnerMotor;
    
    //clock
    int clock;
    int clock_elapsed;
    
    int allMotorsActive;

    //fonts
    SpriteFont fontArial10;

    //Gamestate
    public enum GameState
    {
        NewGame,
        Playing,
        GameOver,
        StartGame,
        Intro
    }

    public enum Players
    {
        AI,
        Player1,
        Player2
    }

    Players player2;
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
        
       
        try
        {
            mapTexture = Content.Load<Texture2D>("image\\map");
            mapGradTexture = Content.Load<Texture2D>("image\\mapgrad");
            //fonts
            fontArial10 = Content.Load<SpriteFont>("fonts\\Arial10");
        }
       
        catch 
        {
           this.Exit();
        }
        if (mapTexture == null || mapGradTexture == null || fontArial10 == null)
            {
                this.Exit();
            }
        
      
        mapGradTextureData = new Color[mapGradTexture.Width * mapGradTexture.Height];
        mapGradTexture.GetData(mapGradTextureData);

        // Get the bounding rectangle of this block
        mapRectangle = new Rectangle(0, 0, mapTexture.Width, mapTexture.Height);

        finishMapRectangle = new Rectangle(457, 335, 1, 130);
        
        //Creating and adding checkpointto list
        CheckPoints();
                
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
            laps.Clear();
            int index,pos;
            List<int> positions = new List<int>();
            positions.Add(0);
            positions.Add(1);
            positions.Add(2);
            positions.Add(3);
            
            pos = random.Next(positions.Count );
            index = positions[pos];
            positions.RemoveAt(pos);
            motorRed = new Motor("Red Motor",Content, "image\\motorred", (int)GameConstants.START_POS_RED.X,
            (int)GameConstants.START_POS_RED.Y + 27*index, new Vector2(0, 0), null);
            motors.Add(motorRed);
          
            pos = random.Next(positions.Count );
            index = positions[pos];
            positions.RemoveAt(pos);
            motorYellow = new Motor("Yellow Motor",Content, "image\\motoryellow", (int)GameConstants.START_POS_RED.X,
           (int)GameConstants.START_POS_RED.Y + 27  *index, new Vector2(0, 0), null);
            motors.Add(motorYellow);

            pos = random.Next(positions.Count );
            index = positions[pos];
            positions.RemoveAt(pos);
            motorBlue = new Motor("Blue Motor", Content, "image\\motorblue", (int)GameConstants.START_POS_RED.X,
         (int)GameConstants.START_POS_RED.Y + 27*index, new Vector2(0, 0), null);
            motors.Add(motorBlue);

            pos = random.Next(positions.Count );
            index = positions[pos];
            positions.RemoveAt(pos);
            motorGreen = new Motor("Green Motor", Content, "image\\motorgreen", (int)GameConstants.START_POS_RED.X,
(int)GameConstants.START_POS_RED.Y + 27*index, new Vector2(0, 0), null);
            motors.Add(motorGreen);

            aiGreen = new AIMotorMovement(motorGreen, checkAiPointsList);
            aiBlue = new AIMotorMovement(motorBlue, checkAiPointsList);
            aiYellow = new AIMotorMovement(motorYellow, checkAiPointsList);

            gameState = GameState.StartGame;
            player2 = Players.AI;
        }
        #endregion

        //start game
        #region
        if(gameState == GameState.StartGame)
        {
            clock = (int)gameTime.TotalGameTime.TotalMilliseconds;

            lapsMotorRed = new Laps(motorRed, checkPointsList, finishMapRectangle, GameConstants.LAPS_NUMBER, clock);
            laps.Add(lapsMotorRed);
            lapsMotorYellow = new Laps(motorYellow, checkPointsList, finishMapRectangle, GameConstants.LAPS_NUMBER, clock);
            laps.Add(lapsMotorYellow);
                        
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
            clock_elapsed = (int)gameTime.TotalGameTime.TotalMilliseconds;
            // Move the player 
            #region
            //RED
            if (keyboard.IsKeyDown(Keys.Left) )
            {
                motorRed.AngleVelocity =  GameConstants.MOTOR_ANGLE;
                motorRed.Turning = true;

            }
            if (keyboard.IsKeyDown(Keys.Right) )
            {
                motorRed.AngleVelocity = -GameConstants.MOTOR_ANGLE;
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
            //Yellow motor player or AI
            if (player2 == Players.Player2)
            {

                //YELLOW
                if (keyboard.IsKeyDown(Keys.A))
                {
                    motorYellow.AngleVelocity = GameConstants.MOTOR_ANGLE;
                    motorYellow.Turning = true;

                }
                if (keyboard.IsKeyDown(Keys.D))
                {
                    motorYellow.AngleVelocity = -GameConstants.MOTOR_ANGLE;
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
            }
            else
            {
                //AI Yellow
                aiYellow.Update(gameTime);
            }

            aiBlue.Update(gameTime);
            aiGreen.Update(gameTime);
            #endregion

            // Check collision with mapborder
            allMotorsActive = motors.Count();
            foreach (Motor motorek in motors)
            {
                if(!motorek.Active) allMotorsActive-=1;

                if (Intersect.IntersectPixels(motorek.DrawRectangle, motorek.TextureData,
                                  mapRectangle, mapGradTextureData))
                {
                  // motorek.Active = false;
                }
                
               motorek.Update(gameTime);
            }
            if (allMotorsActive==0)
            {
                gameState = GameState.GameOver;
            }
            foreach(Laps lap in laps)
            {
                lap.Update(gameTime, clock_elapsed - clock);
            }

        }
        
        #endregion

        //game over
        #region
        if(gameState == GameState.GameOver)
        {
            List<int> temporaryList = new List<int>();
            foreach(Laps lap in laps)
            {
                temporaryList.Add(lap.LapTime);
            }

             try
             {
                 //linq where min of list without 0
                 winner = laps[temporaryList.IndexOf(temporaryList.Where(f => f > 0).Min())].MotorName;
                        
             }
            catch
             {
                 winner = "Nobody ended the race";
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
       


        //draw checkpoints
        DrawCheckpoints();
        spriteBatch.DrawString(fontArial10, "GameTime " + DisplayClock(clock_elapsed-clock), new Vector2(graphics.GraphicsDevice.Viewport.Width / 3.6F, graphics.GraphicsDevice.Viewport.Height / 2.1F), Color.White);
        spriteBatch.DrawString(fontArial10, "Yellow laps: " + lapsMotorYellow.CurrentLap + "/" + GameConstants.LAPS_NUMBER.ToString()+ "Time: " + DisplayClock(lapsMotorYellow.LapTime), new Vector2(graphics.GraphicsDevice.Viewport.Width / 3.6F, graphics.GraphicsDevice.Viewport.Height / 1.9F), Color.White);
        spriteBatch.DrawString(fontArial10, "Red laps: " + lapsMotorRed.CurrentLap + "/" + GameConstants.LAPS_NUMBER.ToString() + "Time: " + DisplayClock(lapsMotorRed.LapTime), new Vector2(graphics.GraphicsDevice.Viewport.Width / 3.6F, graphics.GraphicsDevice.Viewport.Height / 1.7F), Color.White);
        Vector2 dupa = new Vector2(motorRed.Velocity.X, motorRed.Velocity.Y);
        
        spriteBatch.DrawString(fontArial10, "Angle: " + motorRed.Angle + "angle vel " + motorRed.AngleVelocity + " vel to ang: " + VectorToAngle(dupa)+ " vel: " +motorRed.Velocity+" atngleto vel : "+AngleToVector(motorRed.Angle), new Vector2(graphics.GraphicsDevice.Viewport.Width / 3.6F, graphics.GraphicsDevice.Viewport.Height / 2.4F), Color.White);

       
        if(gameState == GameState.GameOver)
        {
            spriteBatch.DrawString(fontArial10, "Winner: " + winner, new Vector2(graphics.GraphicsDevice.Viewport.Width / 3.6F, graphics.GraphicsDevice.Viewport.Height / 2.3F), Color.White);
            spriteBatch.Draw(Content.Load<Texture2D>("image\\motorred"), new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2), Color.White);
        }
        spriteBatch.End();


        base.Draw(gameTime);
    }


    private void DrawCheckpoints()
    {
        //draw finishrectangle
        finishRectangle = new Texture2D(GraphicsDevice, 1, 1);
        finishRectangle.SetData(new[] { Color.White });
        spriteBatch.Draw(finishRectangle,finishMapRectangle, Color.Chocolate);

        //draw checkpoint
        foreach(Rectangle rectangle in checkAiPointsList)
        {
            spriteBatch.Draw(finishRectangle, rectangle, Color.Chocolate);
        }
         
    }

    private string DisplayClock(int clock)
    {
        string time;
        time = ((((clock)/60000)%60)).ToString()+":"+
               (((clock) %60000)/1000).ToString()+":"+
               (((clock) %1000)/10).ToString();
        
        return time;
    }
    private void CheckPoints()
    {
        checkPointsList = new List<Rectangle>();

        checkPointsList.Add(new Rectangle(575, 332, 1, 135));
        checkPointsList.Add(new Rectangle(655, 252, 130, 1));
        checkPointsList.Add(new Rectangle(575, 35, 1, 135));
        checkPointsList.Add(new Rectangle(205, 35, 1, 135));
        checkPointsList.Add(new Rectangle(13, 252, 134, 1));
        checkPointsList.Add(new Rectangle(200, 332, 1, 135));

        checkAiPointsList = new List<Rectangle>();
       // checkAiPointsList.Add(new Rectangle(575, 332, 1, 135));
        checkAiPointsList.Add(new Rectangle(640, 332, 60, 60));
        checkAiPointsList.Add(new Rectangle(655, 252, 130, 1));
      //  checkAiPointsList.Add(new Rectangle(640, 132, 60,60));
        checkAiPointsList.Add(new Rectangle(575, 35, 1, 135));
      //  checkAiPointsList.Add(new Rectangle(340, 92, 60, 60));
      //  checkAiPointsList.Add(new Rectangle(205, 35, 1, 135));
        checkAiPointsList.Add(new Rectangle(120, 112, 80, 80));
        checkAiPointsList.Add(new Rectangle(13, 252, 134, 1));
       // checkAiPointsList.Add(new Rectangle(130, 332, 50, 50));
      //  checkAiPointsList.Add(new Rectangle(200, 332, 1, 135));
        checkAiPointsList.Add(new Rectangle(340, 332, 30, 130));
    }


    private Vector2 AngleToVector(float angle)
    {
        Vector2 vectorek = new Vector2((float)Math.Cos(angle), -(float)Math.Sin(angle));

        return vectorek;
    }


    float VectorToAngle(Vector2 vector)
    {
        return (float)Math.Atan2(-vector.X, -vector.Y)+(float)Math.PI/2;
    }
}}