using System;
using System.Collections.Generic;
using System.Text;

namespace Raquet;
public static partial class Raquet
{
    public static class Settings
    {
        public const int SCREEN_WIDTH = 640;
        public const int SCREEN_HEIGHT = 360;
        public const int SCREEN_SCALE = 2;
        public const bool ALLOW_FULLSCREEN = true;
        public const bool INTEGER_SCALING = true;
        public const bool VSYNC = true;
        public const bool DELTA_TIME = false;
        public const bool BACKGROUND_CLEARING_COLOR = false;
        public const string GAME_NAME = "Raquet Game Engine";
        public const int AUDIO_SAMPLE_RATE = 44100;
        public const int FRAMERATE_CAP = 60;
        public const bool VISUALIZE_BBOX = false;
        public const bool PRINT_DEBUG = true;
        public const uint BBOX_DEBUG_COLOR = 0xFF00FFFF;
    }
}
