using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBase;
using TheGame.Drawing;
using Mogre;

// Class for drawing and controlling menu

namespace TheGame.Controls
{
    public class GameMenu
    {
        private OgreEngine OgreEngine;
        
        public GameMenu(OgreEngine OgreEngine)
        {
            this.OgreEngine = OgreEngine;
            // konstruktor, klidně sem můžeš něco dopsat, když budeš něco potřebovat :)
        }

        public void Update()
        { 
            // tady řešit, když se něco má měnit v čase (animace apod., podle game.frames)
            // plus třeba aktualizovat pozici myši...
            //
            // k pozici mysi se dostanes napr. pres OgreEngine.mControl.mMouse.MouseState.X
        }

        public void Show()
        {
            // tady menu zobrazit

            // budto vyuzit Drawing2D a napsat si vsechno sam nebo zkusit rozbehat knihovnu MIYAGI - necham na tobe :)

            Drawing2D.Create2DElement("logo", "frozenlogo.png", new Vector2(0.0f, 0.0f), new Vector2(0.5f, 0.5f)); // vytvori logo

            // !!! bylo by dobry si nejak hlidat, jestli uz je menu vytvoreny a kdyz se dvakrat zavola show, 
            // tak podruhy ho znova nevytvaret (pze to pak spadne - nejde mit dva elementy se stejnym jmenem)
        }

        public void Hide()
        { 
            // tady menu schovat - asi ty 2Delementy zase znicit
            Drawing2D.Destroy2DElement("logo");
        }
    }
}
