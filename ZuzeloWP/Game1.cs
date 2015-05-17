using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Storage;
using Cocos2D;
using Zuzel;

namespace ZuzeloWP
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
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
        //SmokePlumeParticleSystem smokePlume;

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

        bool keyUup, keyYup, keyTup, keyIup, keyKup;

        //////saving game data
        //StorageDevice device;
        //string containerName = "ZUZELOStorage";
        //string filename = "mysave.sav";
        //[Serializable]
        //public struct SaveGame
        //{
        //    public Skin skinSave;
        //    public bool soundONSave;
        //    public bool showFPSSave;
        //    public Difficulty difficultySave;
        //    public bool classicModeSave;
        //}

        Skin skin;
        Skin skin1;
        Skin skin2;


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
            //#if MACOS
            //            Content.RootDirectory = "AngryNinjas/Content";
            //#else
            Content.RootDirectory = "Content";
            //#endif
            //
            //#if XBOX || OUYA
            //            graphics.IsFullScreen = true;
            //#else
            graphics.IsFullScreen = false;
            //#endif

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333 / 2);

            // Extend battery life under lock.
            //InactiveSleepTime = TimeSpan.FromSeconds(1);

            CCApplication application = new AppDelegate(this, graphics);
            Components.Add(application);
            //#if XBOX || OUYA
            //            CCDirector.SharedDirector.GamePadEnabled = true;
            //            application.GamePadButtonUpdate += new CCGamePadButtonDelegate(application_GamePadButtonUpdate);
            //#endif
        }

        //#if XBOX || OUYA
        //        private void application_GamePadButtonUpdate(CCGamePadButtonStatus backButton, CCGamePadButtonStatus startButton, CCGamePadButtonStatus systemButton, CCGamePadButtonStatus aButton, CCGamePadButtonStatus bButton, CCGamePadButtonStatus xButton, CCGamePadButtonStatus yButton, CCGamePadButtonStatus leftShoulder, CCGamePadButtonStatus rightShoulder, PlayerIndex player)
        //        {
        //            if (backButton == CCGamePadButtonStatus.Pressed)
        //            {
        //                ProcessBackClick();
        //            }
        //        }
        //#endif
        protected override void Initialize()
        {

            base.Initialize();

            //gameState = GameState.Intro;


        }
        protected override void LoadContent()
        {

            // Create a sprite batch to draw those textures
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            Skins();
            //default skint
            skin = skin1;

            difficulty = Difficulty.Easy;

            //InitiateLoad();
            mapTexture = skin.mapTexture;

            //check if images exists;
            try
            {
                mapTexture = skin.mapTexture;
                mapGradTexture = Content.Load<Texture2D>("image\\mapgrad");
                //fonts
                fontArial10 = Content.Load<SpriteFont>("fonts\\Arial10");
            }
            catch
            {
                MyExit();
            }
            if (mapTexture == null || mapGradTexture == null || fontArial10 == null)
            {
                MyExit();
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

        }

        private void ProcessBackClick()
        {
            if (CCDirector.SharedDirector.CanPopScene)
            {
                CCDirector.SharedDirector.PopScene();
            }
            else
            {
                Exit();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                ProcessBackClick();
            }

            // TODO: Add your update logic here


            base.Update(gameTime);
        }
       
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;

            spriteBatch.Begin();
            // Draw texture
            spriteBatch.Draw(mapTexture, GraphicsDevice.Viewport.Bounds, Color.White);
            //spriteBatch.Draw(mapTexture, mapPosition, Color.White);
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
            DrawCheckpoints();

            spriteBatch.DrawString(fontArial10, winner.ToString(), new Vector2(graphics.GraphicsDevice.Viewport.Width / 3.3F, graphics.GraphicsDevice.Viewport.Height / 2.5F), Color.White);
            if (showFps) fpsMonitor.Draw(spriteBatch, fontArial10, new Vector2(20, 20), Color.White);

            spriteBatch.End();


            base.Draw(gameTime);
        }
        private void MyExit()
        {
            //InitiateSave();
            this.Exit();

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

            checkPointsList.Add(new Rectangle((int)((575.0 / 800.0) * GameConstants.WINDOW_WIDTH), (int)((332.0 / 500.0) * GameConstants.WINDOW_HEIGHT), 1, (int)((135.0 / 500.0) * GameConstants.WINDOW_HEIGHT)));
            checkPointsList.Add(new Rectangle((int)((655.0 / 800.0) * GameConstants.WINDOW_WIDTH), (int)((252.0 / 500.0) * GameConstants.WINDOW_HEIGHT), (int)((130.0 / 800.0) * GameConstants.WINDOW_WIDTH), 1));
            checkPointsList.Add(new Rectangle((int)((575.0 / 800.0) * GameConstants.WINDOW_WIDTH), (int)((35.0 / 500.0) * GameConstants.WINDOW_HEIGHT), 1, (int)((135.0 / 500.0) * GameConstants.WINDOW_HEIGHT)));
            checkPointsList.Add(new Rectangle((int)((205.0 / 800.0) * GameConstants.WINDOW_WIDTH), (int)((35.0 / 500.0) * GameConstants.WINDOW_HEIGHT), 1, (int)((135.0 / 500.0) * GameConstants.WINDOW_HEIGHT)));
            checkPointsList.Add(new Rectangle((int)((13.0 / 800.0) * GameConstants.WINDOW_WIDTH), (int)((252.0 / 500.0) * GameConstants.WINDOW_HEIGHT), (int)((134.0 / 800.0) * GameConstants.WINDOW_WIDTH), 1));
            checkPointsList.Add(new Rectangle((int)((200.0 / 800.0) * GameConstants.WINDOW_WIDTH), (int)((332.0 / 500.0) * GameConstants.WINDOW_HEIGHT), 1, (int)((135.0 / 500.0) * GameConstants.WINDOW_HEIGHT)));

            checkAiPointsList = new List<Rectangle>();
            checkAiPointsList.Add(new Rectangle((int)((575.0 / 800.0) * GameConstants.WINDOW_WIDTH), (int)((332.0 / 500.0) * GameConstants.WINDOW_HEIGHT), 1, (int)((115.0 / 500.0) * GameConstants.WINDOW_HEIGHT)));
            //checkAiPointsList.Add(new Rectangle((int)((630.0/ 800.0) * GameConstants.WINDOW_WIDTH), 322, (int)((60.0/800.0)*GameConstants.WINDOW_WIDTH), 60));
            checkAiPointsList.Add(new Rectangle((int)((655.0 / 800.0) * GameConstants.WINDOW_WIDTH), (int)((252.0 / 500.0) * GameConstants.WINDOW_HEIGHT), (int)((120.0 / 800.0) * GameConstants.WINDOW_WIDTH), 1));
            //checkAiPointsList.Add(new Rectangle((int)((640.0/ 800.0) * GameConstants.WINDOW_WIDTH), 132, (int)((60.0/800.0)*GameConstants.WINDOW_WIDTH), 60));
            checkAiPointsList.Add(new Rectangle((int)((575.0 / 800.0) * GameConstants.WINDOW_WIDTH), (int)((55.0 / 500.0) * GameConstants.WINDOW_HEIGHT), 1, (int)((135.0 / 500.0) * GameConstants.WINDOW_HEIGHT)));
            checkAiPointsList.Add(new Rectangle((int)((340.0 / 800.0) * GameConstants.WINDOW_WIDTH), (int)((52.0 / 500.0) * GameConstants.WINDOW_HEIGHT), (int)((60.0 / 800.0) * GameConstants.WINDOW_WIDTH), (int)((80.0 / 500.0) * GameConstants.WINDOW_HEIGHT)));
            checkAiPointsList.Add(new Rectangle((int)((205.0 / 800.0) * GameConstants.WINDOW_WIDTH), (int)((55.0 / 500.0) * GameConstants.WINDOW_HEIGHT), 1, (int)((105.0 / 500.0) * GameConstants.WINDOW_HEIGHT)));
            // checkAiPointsList.Add(new Rectangle((int)((120.0/ 800.0) * GameConstants.WINDOW_WIDTH), 112,(int)((60.0/800.0)*GameConstants.WINDOW_WIDTH), 60));
            checkAiPointsList.Add(new Rectangle((int)((33.0 / 800.0) * GameConstants.WINDOW_WIDTH), (int)((252.0 / 500.0) * GameConstants.WINDOW_HEIGHT), (int)((134.0 / 800.0) * GameConstants.WINDOW_WIDTH), 1));
            //checkAiPointsList.Add(new Rectangle((int)((130.0/ 800.0) * GameConstants.WINDOW_WIDTH), 332, (int)((50.0/800.0)*GameConstants.WINDOW_WIDTH), 50));
            checkAiPointsList.Add(new Rectangle((int)((200.0 / 800.0) * GameConstants.WINDOW_WIDTH), (int)((342.0 / 500.0) * GameConstants.WINDOW_HEIGHT), 1, (int)((125.0 / 500.0) * GameConstants.WINDOW_HEIGHT)));
            //checkAiPointsList.Add(new Rectangle((int)((340.0/ 800.0) * GameConstants.WINDOW_WIDTH), 332, (int)((30.0/800.0)*GameConstants.WINDOW_WIDTH), 130));
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

    }
}