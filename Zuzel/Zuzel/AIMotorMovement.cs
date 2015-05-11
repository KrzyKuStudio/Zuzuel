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
    class AIMotorMovement
    {
        List<Rectangle> checkPointList;
        Motor motor;
        int lapCount;
        int currentLap;
        List<int> currentLapCheckpoints;
        int currentCheckpoint;

        public AIMotorMovement(Motor motor, List<Rectangle> checkPointList)
        {
            this.checkPointList = checkPointList;
            this.motor = motor;
            this.currentLap = 1;
            this.motor.Thrust = true;
            this.currentCheckpoint = 0;
            currentLapCheckpoints = new List<int>();
            foreach(Rectangle rectangle in checkPointList);
            {
                currentLapCheckpoints.Add(0);
            }
        }

        public bool CheckCheckPoints()
        {

            if (currentLapCheckpoints.Min() > 0)
            {
                return true;
            }
            return false;

        }

        public void Update(GameTime gameTime)
        {
            Target(checkPointList[currentCheckpoint]);

            for (int idx = 0; idx < currentLapCheckpoints.Count(); idx++)
            {
                if (motor.CollisionRectangle.Intersects(checkPointList[idx]))
                {
                    currentLapCheckpoints[idx] = 1;
                }
            }
            if (CheckCheckPoints())
            {

                currentLapCheckpoints.Clear();
                foreach (Rectangle rectangle in checkPointList) 
                {
                    currentLapCheckpoints.Add(0);
                }
                              
            }

//            float orcSpeed = .2f;  //the orc's current speed
 
//public void Update(GameTime gameTime)
//{
     
//    //get the difference from orc to player
//    Vector2 differenceToPlayer = playerPosition - orcPosition;
     
//    //** Move the orc towards the player **
//    //first get direction only by normalizing the difference vector
//    //getting only the direction, with a length of one
//    differenceToPlayer.Normalize();
 
//    //then move in that direction
//    //based on how much time has passed
//    orcPosition  += differenceToPlayer * (float)gameTime.ElapsedGameTime.TotalMilliseconds * orcSpeed;
 
//}


        }
   
        private void Target(Rectangle targetRectangle)
        {
       Vector2 difference = new Vector2(targetRectangle.X - this.motor.DrawRectangle.X, targetRectangle.Y - this.motor.DrawRectangle.Y);
       difference.Normalize();
       this.motor.Velocity += difference; ;
        }
    
    }
}
