using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using Mogre;
using TheGame;
using TheGame.HelpingClasses;

namespace SomeGame
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            /*
            
            String hw = Registration.GetHwKey();

            if (!Registration.Registered())
            {
                if (DialogResult.Yes == MessageBox.Show("Aplikace není zaregistrována, přejete si ji nyní zdarma zaregistrovat? Bez registrace nelze hru spustit.", "Registrace", MessageBoxButtons.YesNo))
                {

                }
                else
                {
                    return;
                }
            }*/
            
            OgreEngine ogreEngine = null;

            try
            {
                ogreEngine = new OgreEngine();
                ogreEngine.Go();
            }
            catch (System.Runtime.InteropServices.SEHException)
            {
                if (OgreException.IsThrown)
                    MessageBox.Show(OgreException.LastException.FullDescription, "An Ogre exception has occurred!");
                else
                    throw;
            }
        }
    }


}

