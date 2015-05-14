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
        List<Rectangle> checkPointListAi;
        Motor motor;
        int lapCount;
        int currentLap;
        List<int> currentLapCheckpoints;
        int currentCheckpoint;
        Random random = new Random();
        float speedRandom;
        int handlingDivider;
        int mode;

        public AIMotorMovement(Motor motor, List<Rectangle> checkPointListAi, int mode)
        {
            this.checkPointListAi = checkPointListAi;
            this.motor = motor;
            this.currentLap = 1;
            this.motor.Thrust = false;
            this.motor.Turning = false;
            this.currentCheckpoint = 0;
            this.speedRandom = 0.95F+0.01F*(float)random.Next(1,9);
            this.handlingDivider = 10;
            this.mode = mode;
           
            this.currentLapCheckpoints = new List<int>();
            foreach(Rectangle rectangle in this.checkPointListAi)
            {
                this.currentLapCheckpoints.Add(0);
            }
        }

        public bool CheckCheckPoints()
        {

            if (this.currentLapCheckpoints.Min() > 0)
            {
                return true;
            }
            return false;

        }

        public void Update(GameTime gameTime)
        {
            Target(this.checkPointListAi[this.currentCheckpoint],gameTime);

            if (this.motor.CollisionRectangle.Intersects(checkPointListAi[this.currentCheckpoint]) && this.currentLapCheckpoints[this.currentCheckpoint] == 0)
                {
                    this.currentLapCheckpoints[this.currentCheckpoint] = 1;
                    this.currentCheckpoint++;
                   
                }
            if (CheckCheckPoints())
            {
                this.currentLapCheckpoints.Clear();
                this.currentCheckpoint = 0;
                foreach (Rectangle rectangle in this.checkPointListAi) 
                {
                    this.currentLapCheckpoints.Add(0);
                }
             }
        }

        private void Target(Rectangle targetRectangle, GameTime gameTime)
        {

            Vector2 difference = new Vector2(targetRectangle.Center.X - this.motor.DrawRectangle.Center.X + random.Next(-30, 30), targetRectangle.Center.Y - this.motor.DrawRectangle.Center.Y + random.Next(-50, 50));

            difference.Normalize();

            if (gameTime.TotalGameTime.Milliseconds % this.handlingDivider == 0)
            {
                float accSpeedIndex=1;
                if (this.mode == 1)
                {
                    if (difference.Y < 0.3F && difference.Y > -0.3F)
                    {
                        accSpeedIndex = 1.8F;
                    }
                    else
                    {
                        accSpeedIndex = 1F;
                    }
                }
                else
                {
                    if (difference.Y < 0.2F && difference.Y > -0.2F)
                    {
                        accSpeedIndex = 1.5F;
                    }
                    else
                    {
                        accSpeedIndex = 1;
                    }
                }
                this.motor.Velocity += (difference*accSpeedIndex*speedRandom);
                
              
            }
           
           if(this.motor.Active) this.motor.Angle = VectorToAngle(this.motor.Velocity);
        }


        private Vector2 AngleToVector(float angle)
        {
            Vector2 vectorek = new Vector2((float)Math.Cos(angle), -(float)Math.Sin(angle));

            return vectorek;
        }

        float VectorToAngle(Vector2 vector)
        {
            double anglee = Math.Atan2(-vector.X, -vector.Y) + Math.PI / 2D;
            double angle;

            if (anglee <= 0)
            {
                angle = anglee + 2 * Math.PI;
            }

            else
            {
                angle = anglee;

            }
            return (float)angle;
        }
         
    
    }
}
