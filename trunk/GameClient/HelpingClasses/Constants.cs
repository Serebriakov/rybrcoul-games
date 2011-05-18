using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

// Class saving all constants that are possible to change

namespace HelpingClasses
{
    // all game constants should be defined here

    public static class Constants
    {
        public static class Video
        {
            public static int ScreenWidth = 1024;
            public static int ScreenHeight = 768;
            public static float NearClip = 0.1f;
            public static float FarClip = 10000f;
        }

        public static class MainLoop
        {
            public static int RenderFPS = 0;
            public static int GameFPS = 120;
            public static int PhysicsFPS = 120;
            public static int ControlsFPS = 120;
        }

        public static class PlayerCamera
        {
            public static float XScroll = 100;
            public static float YScroll = 100;
            public static float ZScroll = 10;
            public static float DefaultCamHeight = 256;
            public static float DefaultCamFOV = 90;
            public static float FreeCamSpeed = 10;
            public static float MinCamHeight = 100;
            public static float MaxCamHeight = 500;
        }

        public static class Land
        {
            public static uint LightmapSize = 512;
            public static int HeightmapSize = 129;  // 2^n + 1
            public static uint SurfacemapSize = 2048; // 2^n
            public static uint SimplemapSize = 2048; // 2^n
        }

        public static class Controls
        {
            public static float Sensitivity = 1;
        }
    }
}
