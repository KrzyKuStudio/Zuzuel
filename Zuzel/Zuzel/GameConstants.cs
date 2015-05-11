using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Zuzel
{
    public static class GameConstants
    {
        // resolution
        public const int WINDOW_WIDTH = 800;
        public const int WINDOW_HEIGHT = 500;

        public const float MOTOR_ANGLE = 0.05F;
        public const float MOTOR_ACC_SPEED = 0.4F;

        public const int LAPS_NUMBER = 1;

        // display support
        public const bool VISIBLEMOUSE = true;
        public const int DISPLAY_OFFSET = 15;
        public const string SCORE_PREFIX = "Score: ";
        public static readonly Vector2 SCORE_LOCATION =
            new Vector2(DISPLAY_OFFSET, DISPLAY_OFFSET);
        public const string HEALTH_PREFIX = "Lives: ";
        public static readonly Vector2 HEALTH_LOCATION =
            new Vector2(DISPLAY_OFFSET, 2 * DISPLAY_OFFSET);


        public const float MUSIC_VOL = 0.3F;
        public const float SFX_VOL = 0.4F;

        public static Vector2 START_POS_RED = new Vector2(430, 360);
        public static Vector2 START_POS_GREEN = new Vector2(100, 100);
        public static Vector2 START_POS_BLUE = new Vector2(100, 100);
        public static Vector2 START_POS_YELLOW = new Vector2(100, 100);
    }
}
