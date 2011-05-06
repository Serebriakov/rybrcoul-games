using System;
using System.Collections.Generic;
using System.Text;
using FMOD;

namespace TheGame
{
    public class Sound
    {
        //Sound
        private FMOD.System system = null;
        private FMOD.Sound sound1 = null;
        private FMOD.Sound sound2 = null;
        private FMOD.Channel channel = null;

        public Sound(string fileName)
        {
            FMOD.RESULT result;

            result = FMOD.Factory.System_Create(ref system);
            if (result != FMOD.RESULT.OK)
                throw new Exception();

            result = system.init(32, FMOD.INITFLAG.NORMAL, (IntPtr)null);
            if (result != FMOD.RESULT.OK)
                throw new Exception();

            result = system.createSound(fileName, FMOD.MODE.HARDWARE, ref sound2);
            if (result != FMOD.RESULT.OK)
                throw new Exception();
        }

        public void PlayBackground(string fileName)
        {
            FMOD.RESULT result;

            result = system.createSound(fileName, FMOD.MODE.HARDWARE, ref sound1);
            if (result != FMOD.RESULT.OK)
                throw new Exception();

            result = sound1.setMode(FMOD.MODE.LOOP_NORMAL);
            if (result != FMOD.RESULT.OK)
                throw new Exception();

            result = system.playSound(FMOD.CHANNELINDEX.FREE, sound1, false, ref channel);
            if (result != FMOD.RESULT.OK)
                throw new Exception();
        }

        public void StopBackground()
        {
            FMOD.RESULT result;

            result = sound1.release();
            if (result != FMOD.RESULT.OK)
                throw new Exception();
        }

        public void Play4LineSound()
        {
            system.playSound(FMOD.CHANNELINDEX.FREE, sound2, false, ref channel);
        }
    }
}






















/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using MET;
using FMOD;

namespace TheGame
{
    class Sound
    {
            int freq = 44100;
            int NUMCHANNELS = 20;
            int NUMREALCHANNELS = 10;
            FMOD.System system = null;
            FMOD.RESULT result;
            FMOD.Sound sound = null;
            FMOD.Channel mChannel = null;

        public static void Init()
        {
            
            /*

            result = FMOD.Factory.System_Create(ref system);
            ERRCHECK(result);

            result = system.setSoftwareChannels(NUMREALCHANNELS);
            ERRCHECK(result);

            result = system.init(NUMCHANNELS, FMOD.INITFLAG.NORMAL, (IntPtr)null);
            ERRCHECK(result);

            result = mChannel.setFrequency(freq);
            ERRCHECK(result);

            LogManager.Singleton.LogMessage("FMOD init OK");
        }

        public static void ERRCHECK(FMOD.RESULT result)
        {
            if (result != FMOD.RESULT.OK)
            {
                MessageBox.Show("FMOD error! " + result + " - " + FMOD.Error.String(result));
                Environment.Exit(-1);
            }
        }

        void StopAmbient();

        void PlayAmbient(string fileName, float volume)
        {
            /* ========================================================================================== */
/*                                                                                            */
/* FMOD Ex - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2009.          */
/*                                                                                            */
/* ========================================================================================== */
/*
            
            
            FMOD.System system = null;
            FMOD.Sound sound = null;
            FMOD.Channel mChannel = null;
            FMOD.VECTOR mPos, mVel;
            FMOD.RESULT result;

            int NUMCHANNELS = 20;
            int NUMREALCHANNELS = 10;

            result = FMOD.Factory.System_Create(ref system);
            ERRCHECK(result);

            result = system.setSoftwareChannels(NUMREALCHANNELS);
            ERRCHECK(result);

            result = system.init(NUMCHANNELS, FMOD.INITFLAG.NORMAL, (IntPtr)null);
            ERRCHECK(result);

            result = system.createSound(fileName, (FMOD.MODE.SOFTWARE | FMOD.MODE._2D | FMOD.MODE.LOOP_NORMAL), ref sound);
            ERRCHECK(result);

            result = sound.set3DMinMaxDistance(1.0f, 10000.0f);
            ERRCHECK(result);

            LogManager.Singleton.LogMessage("FMOD init OK");
          

            mPos.x = 4.1f;
            mPos.y = 4.1f;
            mPos.z = 4.1f;

            mVel.x = 5f;
            mVel.y = 5f;
            mVel.z = 5f;
            result = system.playSound(FMOD.CHANNELINDEX.FREE, sound, true, ref mChannel);
            ERRCHECK(result);
            result = mChannel.set3DAttributes(ref mPos, ref mVel);
            ERRCHECK(result);
            result = mChannel.setFrequency(44100);
            ERRCHECK(result);
            result = mChannel.setPaused(false);
            ERRCHECK(result);

        }
          
        private void FMOD_Init()
        {
            throw new NotImplementedException();
        }

    }

}
*/

/*

 FMOD.System system = null;
            FMOD.Sound sound = null;
            FMOD.Channel mChannel = null;
            FMOD.VECTOR mPos, mVel;
            FMOD.RESULT result;

            int NUMCHANNELS = 20;
            int NUMREALCHANNELS = 10;
            int WIDTH = 1024;
            int HEIGHT = 768;

            result = FMOD.Factory.System_Create(ref system);
            ERRCHECK(result);

            result = system.setSoftwareChannels(NUMREALCHANNELS);
            ERRCHECK(result);

            result = system.init(NUMCHANNELS, FMOD.INITFLAG.NORMAL, (IntPtr)null);
            ERRCHECK(result);

            result = system.createSound("Kůň - řehtání a funění.wav", (FMOD.MODE.SOFTWARE | FMOD.MODE._3D | FMOD.MODE.LOOP_NORMAL), ref sound);
            ERRCHECK(result);

            result = sound.set3DMinMaxDistance(4.0f, 10000.0f);
            ERRCHECK(result);

            LogManager.Singleton.LogMessage("FMOD init OK");
            

            mPos.x = 4.1f;
            mPos.y = 4.1f;
            mPos.z = 4.1f;

            mVel.x = 5f;
            mVel.y = 5f;
            mVel.z = 5f;
            result = system.playSound(FMOD.CHANNELINDEX.FREE, sound, true, ref mChannel);
            ERRCHECK(result);
            result = mChannel.set3DAttributes(ref mPos, ref mVel);
            ERRCHECK(result);
            result = mChannel.setFrequency(44100);
            ERRCHECK(result);
            result = mChannel.setPaused(false);
            ERRCHECK(result);
        }

        public static void ERRCHECK(FMOD.RESULT result)
        {
            if (result != FMOD.RESULT.OK)
            {
                MessageBox.Show("FMOD error! " + result + " - " + FMOD.Error.String(result));
                Environment.Exit(-1);
            }
        }

*/