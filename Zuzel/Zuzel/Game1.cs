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
using Microsoft.Xna.Framework.Storage;
using System.Text;

namespace Zuzel
{

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Variables


        GraphicsDeviceManager graphics;
        
        bool classicMode = false;
        public enum Difficulty
        {
            Easy,
            Hard
        }
        Difficulty difficulty;
        // The color data for the images; used for per pixel collision
        Texture2D mapGradTexture;
        Texture2D mapTexture;
        SmokePlumeParticleSystem smokePlume;

        Rectangle mapRectangle;
        Vector2 mapPosition;
        Color[] mapGradTextureData;
        Texture2D finishRectangle;
        Rectangle finishMapRectangle;
        SoundEffect motorSound;

        List<Rectangle> checkPointsList;
        List<Rectangle> checkAiPointsList;
        Motor motorRed;
        Motor motorGreen;
        Motor motorYellow;
        Motor motorBlue;
        List<Motor> motors = new List<Motor>();
        List<TireMark> tireMarks = new List<TireMark>();
        bool showFps = false;
        FpsMonitor fpsMonitor;

        Laps lapsMotorRed;
        Laps lapsMotorGreen;
        Laps lapsMotorYellow;
        Laps lapsMotorBlue;
        List<Laps> laps = new List<Laps>();
        List<Laps> lapsWinner = new List<Laps>();

        AIMotorMovement aiYellow;
        AIMotorMovement aiBlue;
        AIMotorMovement aiGreen;
        List<AIMotorMovement> aiMovement = new List<AIMotorMovement>();

        StringBuilder winner;
      

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
            Intro,
            DisplayResults,
            DisplayTournament,
            DisplayTournamentFinal,
            Counting,
            Options,
            Exit
        }

        public enum Players
        {
            AI,
            Player1,
            Player2
        }

        public enum WhatGame
        {
            singleGame,
            Tournament
        }

        Tournament tournament;
        Players player2;
        GameState gameState;
        WhatGame whatGame;

        // The images will be drawn with this SpriteBatch
        
        SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        //Random random = new Random();
        private static Random random = new Random();
        public static Random Random
        {
            get { return random; }
        }

        bool soundPlayingGO; 
        bool soundPlaying1; 
        bool soundPlaying2;
        bool soundPlaying3;
        bool soundON = true;

        bool keyUup, keyYup,keyTup,keyIup,keyKup;

        ////saving game data
        public struct SaveGameData
        {
            public Skin skin;
            public bool soundON;
            public bool showFPS;
            public Difficulty difficulty;
            public bool classicMode;
        }
        Skin skin;
        Skin skin1;
        Skin skin2;

        SaveGameData saveGameData;
        StorageDevice device;

        // The sub-rectangle of the drawable area which should be visible on all TVs
        Rectangle safeBounds;
        // Percentage of the screen on every side is the safe area
        const float SafeAreaPortion = 0.01f;

        // keep a timer that will tell us when it's time to add more particles to the
        // smoke plume.
        const float TimeBetweenSmokePlumePuffs = 0.05f;
        float timeTillPuff = 0.0f;




#endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
           

            IsMouseVisible = GameConstants.VISIBLEMOUSE;
            // set resolution
            graphics.PreferredBackBufferWidth = GameConstants.WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = GameConstants.WINDOW_HEIGHT;

            smokePlume = new SmokePlumeParticleSystem(this, 13);
            Components.Add(smokePlume);
            
           
     
            
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
            gameState = GameState.Intro;

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
            Skins();
            skin = skin1;
            // Create a sprite batch to draw those textures
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            mapTexture = skin.mapTexture;
            try
            {
                mapTexture = skin.mapTexture;
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
            mapRectangle = new Rectangle(0, 0, skin.mapTexture.Width, skin.mapTexture.Height);

            finishMapRectangle = new Rectangle(457, 335, 1, 130);

            //Creating and adding checkpointto list
            CheckPoints();
            winner = new StringBuilder();
            //fps counter
            fpsMonitor = new FpsMonitor();
            keyUup = false;
            keyYup = false;
            keyIup = false;
            keyKup = false;
            difficulty = Difficulty.Easy;

           
           
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            mapTexture = skin.mapTexture;
            // Get input
      
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
            #region Allways keys

            // back to menu
            if ( keyboard.IsKeyDown(Keys.M))
            {
                gameState = GameState.Intro;
                ////turn off all sounds
                foreach(Motor motor in motors)
                {
                    motor.Active = false;
                    motor.Update(gameTime);
                }
            }
            //sounds on off
            if (keyboard.IsKeyDown(Keys.U)&&keyUup)
            {
                if(soundON)
                {
                   soundON = false;
                }
                else
                {
                    soundON = true;
                }
                keyUup = false;
            }
             if (keyboard.IsKeyUp(Keys.U))
            {
                keyUup = true;
            }

            //show fps
            if (keyboard.IsKeyDown(Keys.Y)&&keyYup)
            {
                if (showFps)
                {
                    showFps = false;
                }
                else
                {
                    showFps = true;
                }
                keyYup = false;

            }

                if (keyboard.IsKeyUp(Keys.Y))
                {
                    keyYup = true;
                }
              
#endregion
                    
            #region Intro
            if (gameState == GameState.Intro)
            {
                winner.Clear();
                winner.Append("Welcome to ZUZELO             Steering:\n");
                winner.Append("                                            ONE: Left, Up, Right\n");
                winner.Append("                                            TWO: A, S, D\n");
                winner.Append("Press:                                  \n");
                winner.Append("B - one player single game      G - one player Tournament\n");
                winner.Append("N - two player single game      H - two player Tournament\n");
                winner.Append("O - options");

                //new game
                if (keyboard.IsKeyDown(Keys.B))
                {
                    gameState = GameState.NewGame;
                    player2 = Players.AI;
                    whatGame = WhatGame.singleGame;
                }
                //options
                if (keyboard.IsKeyDown(Keys.O))
                {
                    gameState = GameState.Options;
                    
                }
                //new game
                if (keyboard.IsKeyDown(Keys.N))
                {
                    gameState = GameState.NewGame;
                    player2 = Players.Player2;
                    whatGame = WhatGame.singleGame;
                }

                //new game
                if (keyboard.IsKeyDown(Keys.G))
                {
                    gameState = GameState.NewGame;
                    player2 = Players.AI;
                    whatGame = WhatGame.Tournament;
                    tournament = new Tournament(GameConstants.ROUNDSTOURNAMENT);
                }
                //new game
                if (keyboard.IsKeyDown(Keys.H))
                {
                    gameState = GameState.NewGame;
                    player2 = Players.Player2;
                    whatGame = WhatGame.Tournament;
                    tournament = new Tournament(GameConstants.ROUNDSTOURNAMENT);
                }

                // Allows the game to exit
                if (gamePad.Buttons.Back == ButtonState.Pressed ||
                    keyboard.IsKeyDown(Keys.Escape))
                {
                    this.Exit();
                }

            }
            #endregion

            #region Options
            if(gameState == GameState.Options)
            {
                // Allows the game to exit
                if (gamePad.Buttons.Back == ButtonState.Pressed ||
                    keyboard.IsKeyDown(Keys.Escape))
                {
                    this.Exit();
                }

                winner.Clear();
                if(difficulty == Difficulty.Easy)
                {
                   winner.Append("I  - Difficulty Easy                             Made by:\n");
                }
                else
                {
                    winner.Append("I  - Difficulty Hard                              Made by:\n");
                }
                if(classicMode)
                winner.Append("T - Classic mode ON                www.krzykustudio.pl\n");
                else
                    winner.Append("T - Classic mode OFF              www.krzykustudio.pl\n");
                if (soundON)
                    winner.Append("U - Sound ON\n");
                else
                    winner.Append("U - Sound OFF\n");

                if (showFps)
                    winner.Append("Y - Show fps ON                            Sounds from:\n");
                else
                    winner.Append("Y - Show fps OFF                          Sounds from:\n");

                winner.Append("M - Menu                                   www.freefx.co.uk\n");
                winner.Append("K - "+skin.skinName+"                                             2015\n");


                //classic mode
                if (keyboard.IsKeyDown(Keys.T) && keyTup)
                {
                    if (classicMode)
                    {
                        classicMode = false;
                    }
                    else
                    {
                        classicMode = true;
                    }
                    keyTup = false;
                }
                if (keyboard.IsKeyUp(Keys.T))
                {
                    keyTup = true;
                }

                //difficulty
                if (keyboard.IsKeyDown(Keys.I) && keyIup)
                {
                    if (difficulty==Difficulty.Easy)
                    {
                        difficulty = Difficulty.Hard;
                    }
                    else
                    {
                        difficulty = Difficulty.Easy;
                    }
                    keyIup = false;
                }
                if (keyboard.IsKeyUp(Keys.I))
                {
                    keyIup = true;
                }

                //Skin
                if (keyboard.IsKeyDown(Keys.K) && keyKup)
                {
                    if (skin == skin1)
                    {
                        skin = skin2;
                    }
                    else
                    {
                        skin = skin1;
                    }
                    keyKup = false;
                }
                if (keyboard.IsKeyUp(Keys.K))
                {
                    keyKup = true;
                }
            }
            #endregion

            #region New Game
            if (gameState == GameState.NewGame)
            {
                tireMarks.Clear();
                motors.Clear();
                laps.Clear();
                int index, pos;
                lapsWinner.Clear();
                //sounds counting flag
                soundPlayingGO = false;
                soundPlaying1 = false;
                soundPlaying2 = false;
                soundPlaying3 = false;
                //starting positions
                List<int> positions = new List<int>();
                positions.Add(0);
                positions.Add(1);
                positions.Add(2);
                positions.Add(3);

                int mode;
                float speed = GameConstants.MOTOR_ACC_SPEED;
                
                if(difficulty == Difficulty.Easy)
                {
                    speed = GameConstants.MOTOR_ACC_SPEED;
                }
                if (difficulty == Difficulty.Hard)
                {
                    speed = GameConstants.MOTOR_ACC_SPEED+0.07F;
                }

                if(classicMode)
                {
                    mode = 1;
                }
                else{
                    mode = 0;
                }

                pos = random.Next(positions.Count);
                index = positions[pos];
                positions.RemoveAt(pos);
                motorSound = Content.Load<SoundEffect>("audio\\motorRunning1");
                motorRed = new Motor("Red    ", Content, skin.motorRed, (int)GameConstants.START_POS.X,
                            (int)GameConstants.START_POS.Y + 27 * index, new Vector2(0, 0), motorSound, GameConstants.SFX_VOL,speed,mode);
                motors.Add(motorRed);

                pos = random.Next(positions.Count);
                index = positions[pos];
                positions.RemoveAt(pos);
                motorSound = Content.Load<SoundEffect>("audio\\motorRunning2");
                motorYellow = new Motor("Yellow", Content, skin.motorYellow, (int)GameConstants.START_POS.X,
                             (int)GameConstants.START_POS.Y + 27 * index, new Vector2(0, 0), motorSound, GameConstants.SFX_VOL, speed,mode);
                motors.Add(motorYellow);

                pos = random.Next(positions.Count);
                index = positions[pos];
                positions.RemoveAt(pos);
                motorSound = Content.Load<SoundEffect>("audio\\motorRunning3");
                motorBlue = new Motor("Blue   ", Content, skin.motorBlue, (int)GameConstants.START_POS.X,
                           (int)GameConstants.START_POS.Y + 27 * index, new Vector2(0, 0), motorSound, GameConstants.SFX_VOL, speed,mode);
                motors.Add(motorBlue);

                pos = random.Next(positions.Count);
                index = positions[pos];
                positions.RemoveAt(pos);
                motorSound = Content.Load<SoundEffect>("audio\\motorRunning3");
                motorGreen = new Motor("Green ", Content, skin.motorGreen, (int)GameConstants.START_POS.X,
                           (int)GameConstants.START_POS.Y + 27 * index, new Vector2(0, 0), motorSound, GameConstants.SFX_VOL, speed,mode);
                motors.Add(motorGreen);

                //AI layers
                aiGreen = new AIMotorMovement(motorGreen, checkAiPointsList,mode);
                aiBlue = new AIMotorMovement(motorBlue, checkAiPointsList,mode);
                aiYellow = new AIMotorMovement(motorYellow, checkAiPointsList,mode);
               
                // if tournament add players to tournament
                if(whatGame == WhatGame.Tournament)
                {
                 if(tournament.State == Tournament.TournamentState.NewGame)
                 {
                     foreach(Motor motor in motors)
                     {
                         tournament.AddPlayer(motor.MotorName);
                      }
                     tournament.State = Tournament.TournamentState.Playing;
                 }

                }

                clock = (int)gameTime.TotalGameTime.TotalSeconds;
                gameState = GameState.Counting;


            }
            #endregion

            #region Counting
            //Odliczanie
            

            if (gameState == GameState.Counting)
            {
                SoundEffect counting;
                 counting = Content.Load<SoundEffect>("audio\\tick");
                SoundEffectInstance instance;
                instance = counting.CreateInstance();
                instance.IsLooped = false;
                winner.Clear();

                if ((int)gameTime.TotalGameTime.TotalSeconds - clock == 2)
                {
                  
                    if(!soundPlaying3)
                    { 
                        instance.Play();
                         soundPlaying3 = true;
                    }
                    
                    winner.Append("                                 *********\n");
                    winner.Append("                                         **\n");
                    winner.Append("                                         **\n");
                    winner.Append("                                 *********\n");
                    winner.Append("                                         **\n");
                    winner.Append("                                         **\n");
                    winner.Append("                                 *********\n");
                }
                if ((int)gameTime.TotalGameTime.TotalSeconds - clock == 3)
                {

                    if (!soundPlaying2)
                    {
                        instance.Play();
                        soundPlaying2 = true;
                    }
                    winner.Append("                                 *********\n");
                    winner.Append("                                        **\n");
                    winner.Append("                                       **\n");
                    winner.Append("                                      **\n");
                    winner.Append("                                     **\n");
                    winner.Append("                                   **    \n");
                    winner.Append("                                 *********\n");
                }
                if ((int)gameTime.TotalGameTime.TotalSeconds - clock == 4)
                {

                    if (!soundPlaying1)
                    {
                        instance.Play();
                        soundPlaying1 = true;
                    }
                    winner.Append("                                      *****\n");
                    winner.Append("                                     *   **\n");
                    winner.Append("                                         **\n");
                    winner.Append("                                         **\n");
                    winner.Append("                                         **\n");
                    winner.Append("                                         **\n");
                    winner.Append("                                       ****\n");
                }
                if ((int)gameTime.TotalGameTime.TotalSeconds - clock >=5)
                {
                    if (!soundPlayingGO)
                    {
                        counting = Content.Load<SoundEffect>("audio\\tack"); 
                        counting.Play();
                        soundPlayingGO = true;
                    }
                    gameState = GameState.StartGame;
                }


              
            }
            #endregion

            #region Start Game
            if (gameState == GameState.StartGame)
            {
                clock = (int)gameTime.TotalGameTime.TotalMilliseconds;

                lapsMotorRed = new Laps(motorRed, checkPointsList, finishMapRectangle, GameConstants.LAPS_NUMBER, clock);
                laps.Add(lapsMotorRed);
                lapsMotorYellow = new Laps(motorYellow, checkPointsList, finishMapRectangle, GameConstants.LAPS_NUMBER, clock);
                laps.Add(lapsMotorYellow);
                lapsMotorGreen = new Laps(motorGreen, checkPointsList, finishMapRectangle, GameConstants.LAPS_NUMBER, clock);
                laps.Add(lapsMotorGreen);
                lapsMotorBlue = new Laps(motorBlue, checkPointsList, finishMapRectangle, GameConstants.LAPS_NUMBER, clock);
                laps.Add(lapsMotorBlue);

                gameState = GameState.Playing;
            }
            #endregion

            #region Game Playing
            if (gameState == GameState.Playing)
            {
                      
                foreach (Motor motorek in motors)
                {
                    motorek.AngleVelocity = 0.0F;
                    motorek.Turning = false;
                 
                }
                clock_elapsed = (int)gameTime.TotalGameTime.TotalMilliseconds;
                
               #region Move the player
                
                //RED
                if (keyboard.IsKeyDown(Keys.Left))
                {
                    if(classicMode)
                    {
                        motorRed.AngleVelocity = GameConstants.MOTOR_ANGLE - 0.02F; 
                    }
                    else
                    {
                        motorRed.AngleVelocity = GameConstants.MOTOR_ANGLE;
                    }
                    
                    
                    motorRed.Turning = true;

                }
                if (keyboard.IsKeyDown(Keys.Right)&&classicMode==false)
                {
                    motorRed.AngleVelocity = -GameConstants.MOTOR_ANGLE;
                    motorRed.Turning = true;

                }

                if (keyboard.IsKeyDown(Keys.Up)&&classicMode==false)
                {
                    motorRed.Thrust = true;
                }
                else if(classicMode)
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
                        if(classicMode)
                        {
                            motorYellow.AngleVelocity = GameConstants.MOTOR_ANGLE - 0.02F;
                        }
                        else
                        {
                            motorYellow.AngleVelocity = GameConstants.MOTOR_ANGLE;
                        }
                        
                        motorYellow.Turning = true;

                    }
                    if (keyboard.IsKeyDown(Keys.D) && classicMode == false)
                    {
                        motorYellow.AngleVelocity = -GameConstants.MOTOR_ANGLE;
                        motorYellow.Turning = true;

                    }
                    if (keyboard.IsKeyDown(Keys.W) && classicMode == false)
                    {
                        motorYellow.Thrust = true;
                    }
                    else if(classicMode)
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
                    if (!motorek.Active) allMotorsActive -= 1;

                    if (Intersect.IntersectPixels(motorek.DrawRectangle, motorek.TextureData,
                                      mapRectangle, mapGradTextureData))
                    {
                        motorek.Active = false;
                    }

                    motorek.Update(gameTime);
                    if (motorek.MotorName.Trim() == "Red")
                    {
                        tireMarks.Add(new TireMark(Content, skin.motorRedTires, motorek.DrawRectangle.Center.X, motorek.DrawRectangle.Center.Y));
                    }
                    if (motorek.MotorName.Trim() == "Green")
                    {
                        tireMarks.Add(new TireMark(Content, skin.motorGreenTires, motorek.DrawRectangle.Center.X, motorek.DrawRectangle.Center.Y));
                    }
                    if (motorek.MotorName.Trim() == "Blue")
                    {
                        tireMarks.Add(new TireMark(Content, skin.motorBlueTires, motorek.DrawRectangle.Center.X, motorek.DrawRectangle.Center.Y));
                    }
                    if (motorek.MotorName.Trim() == "Yellow")
                    {
                        tireMarks.Add(new TireMark(Content, skin.motorYellowTires, motorek.DrawRectangle.Center.X, motorek.DrawRectangle.Center.Y));
                    }
                    
                }

                if (allMotorsActive == 0)
                {
                    gameState = GameState.GameOver;
                }
             

                //update playing 
                foreach (Laps lap in laps)
                {
                    lap.Update(gameTime, clock_elapsed - clock);
                   
                }

                // build playing string
                if(gameTime.TotalGameTime.TotalMilliseconds- clock<500)
                {
                    winner.Clear();
                    winner.Append("                           ********      ********\n");
                    winner.Append("                          **      **     **       **\n");
                    winner.Append("                          **             **       **\n");
                    winner.Append("                          **  *****     **       **\n");
                    winner.Append("                          **      **     **       **\n");
                    winner.Append("                          **      **     **       **\n");
                    winner.Append("                           *********     ********\n");
                }
                else
                {
                    winner.Clear();
                    winner.Append("GameTime: " + DisplayClock(clock_elapsed - clock) + "\n\n");
                    int position = 1;
                    foreach (Laps lap in laps)
                    {
                        winner.Append(lap.MotorName + "       Lap:    " + lap.CurrentLap + "/" + GameConstants.LAPS_NUMBER.ToString() + "\n");
                        position++;
                    }
                }

                if (tireMarks.Count > skin.tireLongMark)
                {
                    tireMarks.RemoveAt(0);
                    tireMarks.RemoveAt(0);
                    tireMarks.RemoveAt(0);
                }

            }

            #endregion

            #region Game Over
            if (gameState == GameState.GameOver)
            {
                List<int> temporaryList = new List<int>();

                var winnerMotor =
                    from motor in laps
                    where motor.LapTime > 0
                    orderby motor.LapTime
                    select motor;

                winner.Clear();

                foreach (Laps lap in winnerMotor)
                {
                    lapsWinner.Add(lap);
                }
                //build winner list only in someone true ended race
                if (lapsWinner.Count > 0)
                {

                    if (whatGame == WhatGame.singleGame)
                    {
                        winner.Append("Winner: " + lapsWinner[0].MotorName + "                    SPACE for main screen\n\n");
                    }
                    else
                    {
                        winner.Append("Winner: " + lapsWinner[0].MotorName + "                    SPACE for tournament screen\n\n");
                        int score = 3;
                        foreach (Laps lap in lapsWinner)
                        {
                            tournament.AddTimes(lap.MotorName, score,lap.LapTime);
                            score--;
                        }
                    }
                    int position = 1;
                    foreach (Laps lap in lapsWinner)
                    {
                        winner.Append(position.ToString() + ". " + lap.MotorName + "       time:    " + DisplayClock(lap.LapTime) + "\n");
                        position++;
                    }
                }
                else
                {
                    winner.Clear();
                    if(whatGame == WhatGame.singleGame)
                    {
                        winner.Append("Nobody wins           SPACE for main screen\n\n");

                    }
                    else
                    {
                        winner.Append("Nobody wins           SPACE for tournament screen\n\n");
                    }
                    

                }
                gameState = GameState.DisplayResults;
            }
            #endregion

            #region Display Results
            if (gameState == GameState.DisplayResults)
            {
                //intro screen
                if (keyboard.IsKeyDown(Keys.Space)&&whatGame==WhatGame.singleGame)
                {
                    gameState = GameState.Intro;
                }
                //tournament results
                if (keyboard.IsKeyDown(Keys.Space) && whatGame == WhatGame.Tournament)
                {
                    gameState = GameState.DisplayTournament;
                    clock = (int)gameTime.TotalGameTime.TotalSeconds;
                }

            }
            #endregion

            #region Display Tournament
            if (gameState == GameState.DisplayTournament)
            {
                winner.Clear();
                if (tournament.State == Tournament.TournamentState.Playing)
                {
                    winner.Append("Tournament scores          SPACE for next round\n");
                    winner.Append(tournament.ToString());
                    //new game with 1second delay hiting key
                    if (keyboard.IsKeyDown(Keys.Space) && ((int)gameTime.TotalGameTime.TotalSeconds - clock) >= 1)
                    {
                        gameState = GameState.NewGame;
                        tournament.AddRound();
                    }
                }

                else if (tournament.State == Tournament.TournamentState.Ended)
                {
                    winner.Clear();
                    winner.Append("FINAL SCORES                 SPACE for end Tournament\n");
                    winner.Append(tournament.ToString());
                    //new game with 1second delay hiting key
                    if (keyboard.IsKeyDown(Keys.Space) && ((int)gameTime.TotalGameTime.TotalSeconds - clock) >= 2)
                    {
                        gameState = GameState.Intro;
                        clock = (int)gameTime.TotalGameTime.TotalSeconds;
                    }
                }

            }
            #endregion

            #region Display Tournament Winner
            if (gameState == GameState.DisplayTournamentFinal)
            {
                //tournament results
                if (keyboard.IsKeyDown(Keys.Space) && ((int)gameTime.TotalGameTime.TotalSeconds - clock) >= 1)
                {
                    gameState = GameState.Intro;
                    
                }

            }
            #endregion

            //motors sounds on of
            foreach (Motor motor in motors)
            {
                motor.SoundOnOff(soundON,GameConstants.SFX_VOL);
      
            }
         
            //update fog 
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
           
            if(skin == skin1)
            {
                foreach (Motor motor in motors)
                {
                    //add new smoke
                    if (motor.Active)
                    {
                        int X = (int)(Math.Cos(motor.Angle) * -motor.DrawRectangle.Width / 2) + motor.DrawRectangle.Center.X;
                        int Y = (int)(Math.Sin(motor.Angle) * motor.DrawRectangle.Height) + motor.DrawRectangle.Center.Y;
                        UpdateSmokePlume(dt, X, Y);
                    }
                }
            }
            
            fpsMonitor.Update();
       
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
            foreach (TireMark tireMark in tireMarks)
            {
                tireMark.Draw(spriteBatch);
            }

            // Draw motors
            foreach (Motor motorek in motors)
            {
                motorek.Draw(spriteBatch);
            }
     
            //draw checkpoints
            //DrawCheckpoints();
          
            spriteBatch.DrawString(fontArial10, winner.ToString(), new Vector2(graphics.GraphicsDevice.Viewport.Width / 3.3F, graphics.GraphicsDevice.Viewport.Height / 2.5F), Color.White);
            if(showFps) fpsMonitor.Draw(spriteBatch, fontArial10, new Vector2(20, 20), Color.White);

            spriteBatch.End();


            base.Draw(gameTime);
        }

        private void DrawCheckpoints()
        {
            //draw finishrectangle
            finishRectangle = new Texture2D(GraphicsDevice, 1, 1);
            finishRectangle.SetData(new[] { Color.White });
            spriteBatch.Draw(finishRectangle, finishMapRectangle, Color.Chocolate);

            //draw checkpoint
            foreach (Rectangle rectangle in checkAiPointsList)
            {
                spriteBatch.Draw(finishRectangle, rectangle, Color.Chocolate);
            }
        }

        private string DisplayClock(int clock)
        {
            //convert miliseonds to 0:0:0
            string time;
            time = ((((clock) / 60000) % 60)).ToString() + ":" +
                   (((clock) % 60000) / 1000).ToString() + ":" +
                   (((clock) % 1000) / 10).ToString();

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
            checkAiPointsList.Add(new Rectangle(575, 332, 1, 115));
            //checkAiPointsList.Add(new Rectangle(630, 322, 60, 60));
            checkAiPointsList.Add(new Rectangle(655, 252, 120, 1));
            //checkAiPointsList.Add(new Rectangle(640, 132, 60, 60));
            checkAiPointsList.Add(new Rectangle(575, 55, 1, 135));
            checkAiPointsList.Add(new Rectangle(340, 52, 60, 80));
            checkAiPointsList.Add(new Rectangle(205, 55, 1, 105));
           // checkAiPointsList.Add(new Rectangle(120, 112, 60, 60));
            checkAiPointsList.Add(new Rectangle(33, 252, 134, 1));
            //checkAiPointsList.Add(new Rectangle(130, 332, 50, 50));
            checkAiPointsList.Add(new Rectangle(200, 342, 1, 125));
            //checkAiPointsList.Add(new Rectangle(340, 332, 30, 130));
        }

        private Vector2 AngleToVector(float angle)
        {
            Vector2 vectorek = new Vector2((float)Math.Cos(angle), -(float)Math.Sin(angle));
            return vectorek;
        }

        public void Skins()
        {
            skin = new Skin();
            skin1 = new Skin();
            skin2 = new Skin();

            skin1.background = "image\\map";
            skin1.motorRed = "image\\motorred";
            skin1.motorRedTires = "image\\tires";
            skin1.motorGreen = "image\\motorgreen";
            skin1.motorGreenTires = "image\\tires";
            skin1.motorBlue = "image\\motorblue";
            skin1.motorBlueTires = "image\\tires";
            skin1.motorYellow = "image\\motoryellow";
            skin1.motorYellowTires = "image\\tires";
            skin1.tireLongMark = 3000;
            skin1.mapTexture = Content.Load<Texture2D>(skin1.background);
            skin1.skinName = "Skin1";

            skin2.background = "image\\map2";
            skin2.motorRed = "image\\motorred2";
            skin2.motorRedTires = "image\\tirered";
            skin2.motorGreen = "image\\motorgreen2";
            skin2.motorGreenTires = "image\\tiregreen";
            skin2.motorBlue = "image\\motorblue2";
            skin2.motorBlueTires = "image\\tireblue";
            skin2.motorYellow = "image\\motoryellow2";
            skin2.motorYellowTires = "image\\tireyellow";
            skin2.tireLongMark = 15;
            skin2.mapTexture = Content.Load<Texture2D>(skin2.background);
            skin2.skinName = "Skin2";

        }

        public static float RandomBetween(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        private void UpdateSmokePlume(float dt,int x , int y)
        {
            timeTillPuff -= dt;
            if (timeTillPuff < 0)
            {
                Vector2 where = Vector2.Zero;
                // add more particles at the bottom of the screen, halfway across.
               
                where.X = (float)x;
                where.Y = (float)y;
                //where.X = graphics.GraphicsDevice.Viewport.Width / 2;
                //where.Y = graphics.GraphicsDevice.Viewport.Height;
                smokePlume.AddParticles(where);

                // and then reset the timer.
                timeTillPuff = TimeBetweenSmokePlumePuffs;
            }
        }

        private void LoadState()
        {
            // Open a storage container.
            IAsyncResult result =
                device.BeginOpenContainer("StorageDemo", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();


            string filename = "savegame.sav";

            // Check to see whether the save exists.
            if (container.FileExists(filename))
                // Delete it so that we can create one fresh.
                container.DeleteFile(filename);
        }
    }

}