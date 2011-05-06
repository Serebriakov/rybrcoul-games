using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using GameBase;
using HelpingClasses;
using TheGame.HelpingClasses;

namespace TheGame.Controls
{
    public class ControlDevices
    {
        private OgreEngine OgreEngine;

        public MOIS.InputManager mIMgr = null;
        public MOIS.Keyboard mKeyb = null;
        public MOIS.Mouse mMouse = null;

        public List<MOIS.KeyCode> PressedKeys = new List<MOIS.KeyCode>();

        public ControlDevices(OgreEngine OgreEngine)
        {
            this.OgreEngine = OgreEngine;
            CreateInput();
        }

        public void Update(float time)
        {
            mKeyb.Capture();
            mMouse.Capture();
            ProcessPressedKeys(time);
            /*
            if (PressedKeys.Contains(MOIS.KeyCode.KC_INSERT))
            {
                GameActorObject obj = new GameActorObject();
                obj.prototype = "cube.mesh";
                obj.Id = (int)OgreEngine.mGame.Frames;

                obj.AttachNode(OgreEngine.mGame.World.CreateObject(obj));
                obj.Position = new Vector3(OgreEngine.mPlayer.Position.x, OgreEngine.mPlayer.Position.y + 100, OgreEngine.mPlayer.Position.z);
                OgreEngine.mGame.World.ObjectList.Add(obj);
            }

            { // player walking
                Vector3 moveVector = Vector3.ZERO;

                if (PressedKeys.Contains(MOIS.KeyCode.KC_W))
                {
                    float X = (Mogre.Math.Cos(OgreEngine.mPlayer.PlayerFacing.x - (float)System.Math.PI / 2f) * Mogre.Math.Cos(OgreEngine.mPlayer.PlayerFacing.y));
                    float Z = (Mogre.Math.Sin(OgreEngine.mPlayer.PlayerFacing.x - (float)System.Math.PI / 2f) * Mogre.Math.Cos(OgreEngine.mPlayer.PlayerFacing.y));
                    float Y = 0;// (Mogre.Math.Sin(OgreEngine.mPlayer.PlayerFacing.y));

                    moveVector += new Vector3(X, Y, Z);
                }

                if (PressedKeys.Contains(MOIS.KeyCode.KC_S))
                {
                    float X = (Mogre.Math.Cos(OgreEngine.mPlayer.PlayerFacing.x - (float)System.Math.PI / 2f) * Mogre.Math.Cos(OgreEngine.mPlayer.PlayerFacing.y));
                    float Z = (Mogre.Math.Sin(OgreEngine.mPlayer.PlayerFacing.x - (float)System.Math.PI / 2f) * Mogre.Math.Cos(OgreEngine.mPlayer.PlayerFacing.y));
                    float Y = 0;// (Mogre.Math.Sin(OgreEngine.mPlayer.PlayerFacing.y));

                    moveVector += new Vector3(X, Y, Z) * (-0.8f);
                }

                if (PressedKeys.Contains(MOIS.KeyCode.KC_A))
                {
                    float X = -(Mogre.Math.Cos(OgreEngine.mPlayer.PlayerFacing.x));
                    float Z = -(Mogre.Math.Sin(OgreEngine.mPlayer.PlayerFacing.x));
                    float Y = 0;

                    moveVector += new Vector3(X, Y, Z) * (0.8f);
                }

                if (PressedKeys.Contains(MOIS.KeyCode.KC_D))
                {
                    float X = (Mogre.Math.Cos(OgreEngine.mPlayer.PlayerFacing.x));
                    float Z = (Mogre.Math.Sin(OgreEngine.mPlayer.PlayerFacing.x));
                    float Y = 0;

                    moveVector += new Vector3(X, Y, Z) * (0.8f);
                }

                moveVector.Normalise();

                
                //if (moveVector != Vector3.ZERO) // moveplayer
                    OgreEngine.mPlayer.Walk(moveVector * Constants.Game.PlayerSpeed);

                // rotateplayer
                OgreEngine.mPlayer.Orientation = Helpers.QFromV3(new Vector3(0, OgreEngine.mPlayer.PlayerFacing.x, 0));

                //Position += moveVector * Constants.Game.PlayerSpeed;
            }
             * */
        }

        public void CreateInput()
        {
            LogManager.Singleton.LogMessage("*** Initializing OIS ***");
            MOIS.ParamList pl = new MOIS.ParamList();
            IntPtr windowHnd;
            OgreEngine.mWin.GetCustomAttribute("WINDOW", out windowHnd);
            pl.Insert("WINDOW", windowHnd.ToString());

            mIMgr = MOIS.InputManager.CreateInputSystem(pl);

            mKeyb = (MOIS.Keyboard)mIMgr.CreateInputObject(MOIS.Type.OISKeyboard, true);
            mKeyb.KeyPressed += new MOIS.KeyListener.KeyPressedHandler(Keyboard_KeyPressed);
            mKeyb.KeyReleased += new MOIS.KeyListener.KeyReleasedHandler(Keyboard_KeyReleased);
            mMouse = (MOIS.Mouse)mIMgr.CreateInputObject(MOIS.Type.OISMouse, true);
            mMouse.MouseReleased += new MOIS.MouseListener.MouseReleasedHandler(Mouse_MouseReleased);
            mMouse.MouseMoved += new MOIS.MouseListener.MouseMovedHandler(Mouse_MouseMoved);
            mMouse.MousePressed += new MOIS.MouseListener.MousePressedHandler(Mouse_MousePressed);

            MOIS.MouseState_NativePtr state = mMouse.MouseState;
            state.width = (int)OgreEngine.mWin.Width;
            state.height = (int)OgreEngine.mWin.Height;
        }

        bool Keyboard_KeyPressed(MOIS.KeyEvent arg)
        {
            if (!PressedKeys.Contains(arg.key))
                PressedKeys.Add(arg.key);

            if (OgreEngine.mGame.World.Map.MapEditor != null && OgreEngine.mGame.World.Map.MapEditor.Edit)
            {
                OgreEngine.mGame.World.Map.MapEditor.KeyPressed(arg);
            }

            if (arg.key == MOIS.KeyCode.KC_ESCAPE) OgreEngine.exit = true;

            if (arg.key == MOIS.KeyCode.KC_F4) OgreEngine.exit = true;

            if (arg.key == MOIS.KeyCode.KC_F12) // edit
            {
                OgreEngine.mGame.World.Map.MapEditor.Edit = !OgreEngine.mGame.World.Map.MapEditor.Edit;
            }
            /*
            if (arg.key == MOIS.KeyCode.KC_SPACE) // camera mode
            {
                OgreEngine.mGame.Camera.FreeCamera = !OgreEngine.mGame.Camera.FreeCamera;
            }*/

            if (DEBUG.Active)
                if (arg.key == MOIS.KeyCode.KC_P) DEBUG.PolygonMode = DEBUG.PolygonMode == PolygonMode.PM_POINTS ? PolygonMode.PM_SOLID : DEBUG.PolygonMode == PolygonMode.PM_SOLID ? PolygonMode.PM_WIREFRAME : PolygonMode.PM_POINTS;

            /*
            if (arg.key == MOIS.KeyCode.KC_RETURN)
            {
                GameActorObject obj = new GameActorObject();
                obj.prototype = "cube.mesh";
                obj.Id = (int)OgreEngine.mGame.Frames;

                obj.AttachNode(OgreEngine.mGame.World.CreateObject(obj));
                obj.Position = new Vector3(OgreEngine.mPlayer.Position.x, OgreEngine.mPlayer.Position.y + 100, OgreEngine.mPlayer.Position.z);
                OgreEngine.mGame.World.ObjectList.Add(obj);

            }

            if (arg.key == MOIS.KeyCode.KC_F1)
            {
                if (OgreEngine.mGame.State == GameState.RUNNING)
                {
                    OgreEngine.mMenu.Show();
                    OgreEngine.mGame.State = GameState.MAINMENU;
                }
                else
                    if (OgreEngine.mGame.State == GameState.MAINMENU)
                    {
                        OgreEngine.mMenu.Hide();
                        OgreEngine.mGame.State = GameState.RUNNING;
                    }
            }

            if (arg.key == MOIS.KeyCode.KC_SPACE)
            {
                OgreEngine.mPlayer.Jump(10);
            }

            if (!PressedKeys.Contains(arg.key))
                PressedKeys.Add(arg.key);
            */
            return true;
        }

        bool Keyboard_KeyReleased(MOIS.KeyEvent arg)
        {
            if (PressedKeys.Contains(arg.key))
                PressedKeys.Remove(arg.key);

            return true;
        }

        bool Mouse_MousePressed(MOIS.MouseEvent arg, MOIS.MouseButtonID id)
        {
            if (OgreEngine.mGame.World.Map.MapEditor != null && OgreEngine.mGame.World.Map.MapEditor.Edit)
            {
                OgreEngine.mGame.World.Map.MapEditor.MousePressed(arg, id);
            }

            return true;
        }

        bool Mouse_MouseReleased(MOIS.MouseEvent arg, MOIS.MouseButtonID id)
        {
            if (OgreEngine.mGame.World.Map.MapEditor != null && OgreEngine.mGame.World.Map.MapEditor.Edit)
            {
                OgreEngine.mGame.World.Map.MapEditor.MouseReleased(arg, id);
            }

            return true;
        }

        bool Mouse_MouseMoved(MOIS.MouseEvent arg)
        {
            /*if (OgreEngine.mGame.Camera.FreeCamera)
            {
                float dx = -(float)arg.state.X.rel / 50.0f * Constants.Controls.Sensitivity;
                float dy = -(float)arg.state.Y.rel / 50.0f * Constants.Controls.Sensitivity;

                //OgreEngine.mGame.Camera.Pitch(dy);
                //OgreEngine.mGame.Camera.Yaw(dx);
            }
            else
            {
                // pokud neni free, tak tady nic
            }*/

            return true;
        }

        public void ProcessPressedKeys(float evt)
        {
            if (!OgreEngine.mGame.Camera.FreeCamera)
            {
                if (PressedKeys.Contains(MOIS.KeyCode.KC_LEFT))
                {
                    OgreEngine.mGame.Camera.Position += new Vector3(Constants.PlayerCamera.XScroll*evt, 0, 0);
                }

                if (PressedKeys.Contains(MOIS.KeyCode.KC_RIGHT))
                {
                    OgreEngine.mGame.Camera.Position += new Vector3(-Constants.PlayerCamera.XScroll*evt, 0, 0);
                }

                if (PressedKeys.Contains(MOIS.KeyCode.KC_UP))
                {
                    OgreEngine.mGame.Camera.Position += new Vector3(0, 0, Constants.PlayerCamera.YScroll*evt);
                }

                if (PressedKeys.Contains(MOIS.KeyCode.KC_DOWN))
                {
                    OgreEngine.mGame.Camera.Position += new Vector3(0, 0, -Constants.PlayerCamera.YScroll*evt);
                }
            }
            else
            {
                /*Vector3 moveVector = Vector3.ZERO;
                if (PressedKeys.Contains(MOIS.KeyCode.KC_S))
                {
                    float X = (Mogre.Math.Cos(OgreEngine.mPlayer.PlayerFacing.x - (float)System.Math.PI / 2f) * Mogre.Math.Cos(OgreEngine.mPlayer.PlayerFacing.y));
                    float Z = (Mogre.Math.Sin(OgreEngine.mPlayer.PlayerFacing.x - (float)System.Math.PI / 2f) * Mogre.Math.Cos(OgreEngine.mPlayer.PlayerFacing.y));
                    float Y = (Mogre.Math.Sin(OgreEngine.mPlayer.PlayerFacing.y));

                    moveVector += new Vector3(X, Y, Z) * (-0.8f);
                }

                if (PressedKeys.Contains(MOIS.KeyCode.KC_A))
                {
                    float X = -(Mogre.Math.Cos(OgreEngine.mPlayer.PlayerFacing.x));
                    float Z = -(Mogre.Math.Sin(OgreEngine.mPlayer.PlayerFacing.x));
                    float Y = 0;

                    moveVector += new Vector3(X, Y, Z) * (0.8f);
                }

                if (PressedKeys.Contains(MOIS.KeyCode.KC_D))
                {
                    float X = (Mogre.Math.Cos(OgreEngine.mPlayer.PlayerFacing.x));
                    float Z = (Mogre.Math.Sin(OgreEngine.mPlayer.PlayerFacing.x));
                    float Y = 0;

                    moveVector += new Vector3(X, Y, Z) * (0.8f);
                }
                moveVector.Normalise();
                OgreEngine.mPlayer.Position += moveVector * Constants.PlayerCamera.FreeCamSpeed * evt;
                 */
            }

            if (OgreEngine.mGame.World.Map.MapEditor != null && OgreEngine.mGame.World.Map.MapEditor.Edit)
            {
                OgreEngine.mGame.World.Map.MapEditor.ProcessPressedKeys(PressedKeys, evt);
            }
        }
    }
}
