using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace TheGame.HelpingClasses
{
    public static class DEBUG
    {
        public static bool Active = true;
        
        public static PolygonMode PolygonMode = PolygonMode.PM_SOLID;

        public static void Write(String text)
        {
            Console.WriteLine(text);
        }
    }
}
