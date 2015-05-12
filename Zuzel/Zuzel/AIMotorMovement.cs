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
        

        public AIMotorMovement(Motor motor, List<Rectangle> checkPointListAi)
        {
            this.checkPointListAi = checkPointListAi;
            this.motor = motor;
            this.currentLap = 1;
            this.motor.Thrust = true;
            this.motor.Turning = true;
            this.currentCheckpoint = 0;

            
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

            //for (int idx = 0; idx < this.currentLapCheckpoints.Count(); idx++)
         //   {
            if (this.motor.CollisionRectangle.Intersects(checkPointListAi[this.currentCheckpoint]) && this.currentLapCheckpoints[this.currentCheckpoint] == 0)
                {
                    this.currentLapCheckpoints[this.currentCheckpoint] = 1;
                    this.currentCheckpoint++;
                   
                }
         //   }
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

            Vector2 difference = new Vector2(targetRectangle.Center.X - this.motor.DrawRectangle.Center.X + random.Next(-30, 30), targetRectangle.Center.Y - this.motor.DrawRectangle.Center.Y + random.Next(-30, 30));

            difference.Normalize();

            Console.WriteLine("target" + currentCheckpoint);
            ////Console.WriteLine("target"+targetRectangle);
            //Console.WriteLine("motor ang" + this.motor.Angle);
            //Console.WriteLine("vevv to angk" + VectorToAngle(difference));
            //Console.WriteLine("diff" + (difference));
            if (gameTime.TotalGameTime.Milliseconds % 20 == 0)
            {
                double temp_angle =this.motor.Angle;
                if(temp_angle >= (Math.PI*1.5D))
                {
                   
                    temp_angle= temp_angle - Math.PI;
                }
                
                else if (temp_angle <= -(Math.PI /2D))
                {

                    temp_angle = temp_angle + 2*Math.PI;
                }
                
                
                if (temp_angle >= VectorToAngle(difference))
                {
                    this.motor.AngleVelocity = -0.15F;
                }

                else 
                {
                    this.motor.AngleVelocity = 0.15F;
                }

                //this.motor.Velocity += difference;
                //this.motor.AngleVelocity = 0.04F;

                // this.motor.Angle = VectorToAngle(difference);
            }
            if (gameTime.TotalGameTime.Milliseconds % 4 == 0)
            {
                this.motor.Thrust = true;
            }
            else
            {
                this.motor.Thrust = false;
            }

        }



        private Vector2 AngleToVector(float angle)
        {
            Vector2 vectorek = new Vector2((float)Math.Cos(angle), -(float)Math.Sin(angle));

            return vectorek;
        }

        float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(-vector.X, -vector.Y) + (float)Math.PI / 2;
        }

        //float VectorToAngle(Vector2 vector)
        //{
        //    return (float)Math.Atan2(vector.X, -vector.Y);
        //}
    }
}
