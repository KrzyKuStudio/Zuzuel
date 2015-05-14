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
    public class Motor : Objekt 
    {
        bool thrust;
        float angle;
        float angleVelocity;
        float friction;
        float accSpeed;
        bool turning;
        string motorName;
        SoundEffectInstance soundMotorInstance;
        bool soundIsPlaying;
     

          public Motor(string name,ContentManager contentManager, string spriteName, int x, int y, Vector2 velocity,
            SoundEffect Sound, float soundVolume) : base(contentManager, spriteName, x, y, velocity, Sound)
            {
              this.angle = 0;  
              this.angleVelocity = 0;
              this.friction = 0.04F;
              this.accSpeed = 0.4F;
              this.motorName = name;


              if (Sound != null)
              {
                  soundMotorInstance = this.Sound.CreateInstance();
                  soundMotorInstance.IsLooped = true;
                  soundMotorInstance.Volume = soundVolume - 0.2F;
                  this.soundMotorInstance.Play();
                  this.soundIsPlaying = true;
              }
              }
        
          public bool Thrust
          {
              get { return thrust; }
              set { thrust = value; }
          }
          public string MotorName
          {
              get { return motorName; }
             
          }
        public float AngleVelocity
          {
              get { return angleVelocity; }
              set { angleVelocity = value; }
          }
        public float Angle
        {
            get { return angle; }
            set { angle = value ; }
        }

        public bool Turning
        {
            get { return turning; }
            set { turning = value; }
           
        }
        public void SoundOnOff(bool soundON, float soundVolume)
        {
            if(!soundON)
            {
                this.soundMotorInstance.Volume = 0F;
                if(this.soundIsPlaying)
                {
                    this.soundMotorInstance.Stop();
                    this.soundIsPlaying = false;
                }
                
            }
            else
            {
                this.soundMotorInstance.Volume = soundVolume;
                if(!this.soundIsPlaying)
                {
                    this.soundMotorInstance.Play();
                    this.soundIsPlaying = true;
                }
                
            }
        }
          /// <summary>
          /// Updates the objektlocation location
          /// </summary>
          /// <param name="gameTime">game time</param>
          /// 
           


        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(drawRectangle.Width/2, drawRectangle.Height/2);
            Vector2 position = new Vector2(drawRectangle.X, drawRectangle.Y);
            //spriteBatch.Draw(sprite, drawRectangle, Color.White);
            spriteBatch.Draw(sprite, position + origin, null, Color.White, -angle, origin, 1F, SpriteEffects.None, 0F);
         

        }

        public void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            
      
            spriteBatch.Draw(sprite, new Vector2(x,y),Color.White);
         


        }

          private Vector2 AngleToVector(float angle)
{
      Vector2 vectorek = new Vector2 ((float)Math.Cos(angle),-(float)Math.Sin(angle));
 
              return vectorek;
}

          private double distance(Vector2 point1, Vector2 point2)
          {
              return Math.Sqrt((point1.X - point2.X) * (point1.X - point2.X) + (point1.Y - point2.Y) * (point1.Y - point2.Y));
          }

          public void Update(GameTime gameTime)
          {

              if (Active == true)
              {
                  this.drawRectangle.X += (int)velocity.X;
                  this.drawRectangle.Y += (int)velocity.Y;

                  this.angle +=  this.angleVelocity;
                  this.angle = this.angle % ((float)Math.PI * 2);
                  
                  Vector2 fowardVelocity = AngleToVector(this.angle);
                  this.velocity.X *= (1 - this.friction);
                  this.velocity.Y *= (1 - this.friction);

                  if(thrust)
                  {
                      this.velocity.X += GameConstants.MOTOR_ACC_SPEED * fowardVelocity.X;
                      this.velocity.Y += GameConstants.MOTOR_ACC_SPEED * fowardVelocity.Y;
                 
                      
                  }
                  if(turning)
                  {
                      this.accSpeed = GameConstants.MOTOR_ACC_SPEED - 0.2F;
                  }
                  else
                  {
                      this.accSpeed = GameConstants.MOTOR_ACC_SPEED;
                  }
                  if(this.Sound!=null)
                  {
                      if (thrust)
                      {
                          this.soundMotorInstance.Pitch = 0.5f;
                          soundMotorInstance.Volume = GameConstants.SFX_VOL;
                      }
                      else
                      {
                          this.soundMotorInstance.Pitch = 0;
                          soundMotorInstance.Volume = GameConstants.SFX_VOL - 0.2F;
                       
                      }
                 
                  }

                  
              }
              else if (this.Sound != null)
              {
                 
                      this.soundMotorInstance.Stop();
                

              }
            
      

          }


    }
}
