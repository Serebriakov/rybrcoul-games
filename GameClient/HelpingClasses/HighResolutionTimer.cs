using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Class with high precision timer

namespace HelpingClasses
{
    public class HighResolutionTimer
    {
        #region Win32APIs
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long perfcount);

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long freq);
        #endregion

        #region Variables Declaration
        private long mStartCounter;
        private long mFrequency;
        #endregion

        #region Constuctors
        public HighResolutionTimer()
        {
            QueryPerformanceFrequency(out mFrequency);
            Start();
        }
        #endregion

        #region Methods
        public void Start()
        {
            QueryPerformanceCounter(out mStartCounter);
        }

        public double GetTime()
        {
            long endCounter;
            QueryPerformanceCounter(out endCounter);
            long elapsed = endCounter - mStartCounter;
            return (double)elapsed / mFrequency;
        }
        #endregion
    }
}
