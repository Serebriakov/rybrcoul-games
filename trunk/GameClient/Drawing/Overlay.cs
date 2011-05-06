using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace TheGame.Drawing
{
    public static class OnScreenStats
    {
        public static Overlay DebugOverlay;
        public static OverlayElement myAvg;
        public static OverlayElement myCurr;
        public static OverlayElement myBest;
        public static OverlayElement myWorst;
        public static OverlayElement myNumTris;
        public static OverlayElement myNumBatches;

        public static void Show()
        {
            DebugOverlay = OverlayManager.Singleton.GetByName("Core/DebugOverlay");

            myAvg = OverlayManager.Singleton.GetOverlayElement("Core/AverageFps");
            myCurr = OverlayManager.Singleton.GetOverlayElement("Core/CurrFps");
            myBest = OverlayManager.Singleton.GetOverlayElement("Core/BestFps");
            myWorst = OverlayManager.Singleton.GetOverlayElement("Core/WorstFps");
            myNumTris = OverlayManager.Singleton.GetOverlayElement("Core/NumTris");
            myNumBatches = OverlayManager.Singleton.GetOverlayElement("Core/NumBatches");

            DebugOverlay.Show();
        }

        public static void Update(RenderWindow Window)
        {
            myAvg.Caption = "Average FPS: " + Convert.ToString(Window.AverageFPS);
            myCurr.Caption = "Current FPS: " + Convert.ToString(Window.LastFPS);
            myBest.Caption = "Best FPS: " + Convert.ToString(Window.BestFPS);
            myWorst.Caption = "Worst FPS: " + Convert.ToString(Window.WorstFPS);
            myNumTris.Caption = "Triangle Count: " + Convert.ToString(Window.TriangleCount);
            myNumBatches.Caption = "Batch Count: " + Convert.ToString(Window.BatchCount);
        }
    }
}
