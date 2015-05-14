using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Zuzel
{
    class Laps
    {
        List<Rectangle> checkPointList;
        int lapTime;
        Motor motor;
        int lapCount;
        int currentLap;
        List<int> currentLapCheckpoints;
        int currentCheckpoint;
        Rectangle finishMapRectangle;
        int clock;
        int clock_elapsed;
        bool finishedRace;

        public int CurrentLap
        {
            get { return currentLap; }
            set { currentLap = value; }
        }

        public int LapTime
        {
            get { return lapTime; }
            set { lapTime = value; }
        }
        public string MotorName
        {
            get { return motor.MotorName; }

        }

        public Laps(Motor motor, List<Rectangle> checkPointList, Rectangle finishMapRectangle, int lapsNumber, int clock)
        {
            this.checkPointList = checkPointList;
            this.motor = motor;
            this.finishMapRectangle = finishMapRectangle;
            this.lapCount = lapsNumber;
            this.currentLap = 1;
            currentLapCheckpoints = new List<int>();
            foreach (Rectangle rectangle in checkPointList) 
            {
                currentLapCheckpoints.Add(0);
            }
            this.lapTime = 0;
            this.clock = clock;
            finishedRace = false;
        }

        public bool CheckCheckPoints()
        {

            if (currentLapCheckpoints.Min() > 0)
            {
                return true;
            }
            return false;

        }

        public void Update(GameTime gameTime, int clock)
        {
            if (motor.Active)
            {
                this.clock = clock;
            }

            for (int idx = 0; idx < currentLapCheckpoints.Count(); idx++)
            {
                if (motor.CollisionRectangle.Intersects(checkPointList[idx]))
                {
                    currentLapCheckpoints[idx] = 1;
                }
            }
            if (motor.CollisionRectangle.Intersects(finishMapRectangle) && (motor.Velocity.X > 0) && CheckCheckPoints())
            {

                currentLapCheckpoints.Clear();
                foreach (Rectangle rectangle in checkPointList) 
                {
                    currentLapCheckpoints.Add(0);
                }

                this.currentLap++;

                if (currentLap > lapCount)
                {
                    this.currentLap = lapCount;
                    motor.Active = false;
                    this.finishedRace = true;
                    this.lapTime = clock;
                }
            }
        }

    }
}
