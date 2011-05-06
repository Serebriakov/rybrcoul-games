using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheGame.Controls
{
    public class GameSettings
    {
        public enum VideoQuality
        {
            VERYLOW = 0,
            LOW = 1,
            MEDIUM = 2,
            HIGH = 3,
            ULTRA = 4
        }

        public VideoQuality Water = VideoQuality.MEDIUM;
        public VideoQuality Grass = VideoQuality.MEDIUM;
        public bool GrassShadows = false;

        public void LoadFromFile(String file)
        {
            IniParser parser = new IniParser(file);
            String res = parser.GetSetting("Video", "Water").ToLower();

            switch (res)
            {
                case "verylow": Water = VideoQuality.LOW;
                    break;

                case "low": Water = VideoQuality.LOW;
                    break;

                case "medium": Water = VideoQuality.MEDIUM;
                    break;

                case "high": Water = VideoQuality.HIGH;
                    break;

                case "ultra": Water = VideoQuality.ULTRA;
                    break;
            }
        }

    }
}
