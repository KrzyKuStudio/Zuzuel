using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Zuzel
{
    class Skin
    {
        public string background;
        public string motorYellow;
        public string motorYellowTires;
        public string motorRed;
        public string motorRedTires;
        public string motorGreen;
        public string motorGreenTires;
        public string motorBlue;
        public string motorBlueTires;
        public int tireLongMark;
        public Texture2D mapTexture;
        public string skinName;
        
        public Skin()
        {
            ;
        }
    }
}
