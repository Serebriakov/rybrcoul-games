using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using MET;
using TheGame;
using HelpingClasses;
using GameBase;
using Map;
using System.Collections;
using TheGame.Drawing;
using TheGame.Controls;
using TheGame.HelpingClasses;

namespace TheGame
{
    public class OgreEngine
    {
        public Root mRoot = null;

        public String exeDir = Path.GetDirectoryName(Application.ExecutablePath) + @"\";

        public SceneManager mMgr = null;
        public Camera mCam = null;
        public RenderWindow mWin = null;

        public ControlDevices mControl = null;

        public Game mGame = null;

        public GameMenu mMenu = null;

        public List<ObjectAnimation> RunningAnimations = new List<ObjectAnimation>();

        public bool exit = false;

        public void Go()
        {
            CreateRoot();
            DefineResources();
            SetupRenderSystem();
            CreateRenderWindow();
            InitResources();
            CreateScene();
            CreateInput();
            CreateWorld();

            StartRenderLoop();
        }

        void CreateRoot()
        {
            mRoot = new Root(exeDir + @"plugins.cfg", exeDir + @"ogre.cfg", exeDir + @"logs\ogre.log");
            //mRoot.LoadPlugin(exeDir + @"Plugin_CgProgramManager.dll");
        }

        void DefineResources()
        {
            ConfigFile cf = new ConfigFile();
            cf.Load("resources.cfg", "\t:=", true);
            ConfigFile.SectionIterator seci = cf.GetSectionIterator();
            String secName, typeName, archName;

            while (seci.MoveNext())
            {
                secName = seci.CurrentKey;
                ConfigFile.SettingsMultiMap settings = seci.Current;
                foreach (KeyValuePair<string, string> pair in settings)
                {
                    typeName = pair.Key;
                    archName = pair.Value;
                    ResourceGroupManager.Singleton.AddResourceLocation(archName, typeName, secName);
                }
            }
        }

        void SetupRenderSystem()
        {
            if (!mRoot.ShowConfigDialog())
            {
                //throw new Exception("The user canceled the configuration dialog.");
            }
            /*
            // Setting up the RenderSystem manually.
            RenderSystem rs = mRoot.GetRenderSystemByName("Direct3D9 Rendering Subsystem");
                                                // or use "OpenGL Rendering Subsystem"
            mRoot.RenderSystem = rs;
            rs.SetConfigOption("Full Screen", "No");
            rs.SetConfigOption("Video Mode", "1024 x 768 @ 32-bit colour");
             * */
        }

        void CreateRenderWindow()
        {
            mWin = mRoot.Initialise(true, "Prehistorik");

            //// Embedding ogre in a windows hWnd.  The "handle" variable holds the hWnd.
            //NameValuePairList misc = new NameValuePairList();
            //misc["externalWindowHandle"] = handle.ToString();
            //mWindow = mRoot.CreateRenderWindow("Main RenderWindow", 800, 600, false, misc);
        }

        void InitResources()
        {
            TextureManager.Singleton.DefaultNumMipmaps = 5;
            ResourceGroupManager.Singleton.InitialiseAllResourceGroups();
        }

        void RunEveryFrame(float d)
        {
            mGame.Update(d);

            mControl.Update(d);
            UpdateCamera(d);
            OnScreenStats.Update(mWin);
        }

        bool FrameEnded(FrameEvent evt)
        {
            RunEveryFrame(evt.timeSinceLastFrame);
            // fps
            return true;
        }

        bool FrameStarted(FrameEvent evt)
        {
            foreach (ObjectAnimation anim in RunningAnimations)
            {
                float multi = 1.0f; // default animation speed multiplier

                anim.animState.AddTime(evt.timeSinceLastFrame * multi); //* Rubikon.Game.Prototypes[anim.activator.MeshName].WalkSpeedanim
            }

            return true;
        }

        
        private void CreateGrid(int numcols, int numrows, float unitsize)
        {
            ManualObject grid = mMgr.CreateManualObject("grid");

            //grid.Begin("BaseWhiteNoLighting", RenderOperation.OperationTypes.OT_TRIANGLE_LIST);
            grid.Begin("Examples/chrome", RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            float width = (float)numcols * unitsize;
            float depth = (float)numrows * unitsize;
            Vector3 center = new Vector3(-width / 2.0f, 0, -depth / 2.0f);

            for (int i = 0; i < numrows; ++i)
            {
                Vector3 s, e;
                s.x = 0.0f;
                s.z = i * unitsize;
                s.y = 0.0f;

                e.x = width;
                e.z = i * unitsize;
                e.y = 0.0f;

                grid.Position(s + center);
                grid.Position(e + center);
            }
            grid.Position(new Vector3(0.0f, 0.0f, numrows * unitsize) + center);
            grid.Position(new Vector3(width, 0.0f, numrows * unitsize) + center);

            for (int i = 0; i < numcols; ++i)
            {
                Vector3 s, e;
                s.x = i * unitsize;
                s.z = depth;
                s.y = 0.0f;

                e.x = i * unitsize;
                e.z = 0.0f;
                e.y = 0.0f;

                grid.Position(s + center);
                grid.Position(e + center);
            }
            grid.Position(new Vector3(numcols * unitsize, 0.0f, 0.0f) + center);
            grid.Position(new Vector3(numcols * unitsize, 0.0f, depth) + center);
            grid.End();

            mMgr.RootSceneNode.AttachObject(grid);
        } // pro strycka prihodu


        void StartRenderLoop()
        {
            OnScreenStats.Show();
            HighResolutionTimer timer = new HighResolutionTimer();
            timer.Start();

            while (!exit)
            {
                timer.Start();

                if (!mRoot.RenderOneFrame())
                    LogManager.Singleton.LogMessage("Can not render!"); ;

                WindowEventUtilities.MessagePump();

                /*
                if (Constants.MainLoop.RenderFPS != 0) // pokud mame omezene fps
                {
                    int time = 1000 / Constants.MainLoop.RenderFPS;
                    if (time > 0) Thread.Sleep(time); // cekat jen pokud mame cekat
                }*/
            }
        }

        void CreateWorld()
        {
            mGame = new Game(this);
            mGame.World.Map = new MapManager(this);
            mGame.World.Map.LoadMap("xyz");

            mMgr.SetShadowUseInfiniteFarPlane(false);
            mMgr.AmbientLight = new ColourValue(0.5f, 0.5f, 0.5f);
            mMgr.ShadowTechnique = ShadowTechnique.SHADOWTYPE_STENCIL_ADDITIVE;


            // Create the first light
            /*Light light;
            light = mMgr.CreateLight("Light1");
            light.Type = Light.LightTypes.LT_POINT;
            light.Position = new Vector3(1000, 1000, 1000);
            light.DiffuseColour = ColourValue.Red;
            light.SpecularColour = ColourValue.Red;
            light.PowerScale = 50;*/
            /*
            // Create the second light
            light = mMgr.CreateLight("Light2");
            light.Type = Light.LightTypes.LT_DIRECTIONAL;
            light.DiffuseColour = new ColourValue(.5f, .5f, 0);
            light.SpecularColour = new ColourValue(.5f, .5f, 0);
            light.Direction = new Vector3(0, -1, -1);

            // Create the third light
            light = mMgr.CreateLight("Light3");
            light.Type = Light.LightTypes.LT_SPOTLIGHT;
            light.DiffuseColour = ColourValue.Blue;
            light.SpecularColour = ColourValue.Blue;
            light.Direction = new Vector3(-1, -1, 0);
            light.Position = new Vector3(200, 100, 200);
            light.SetSpotlightRange(new Degree(35), new Degree(50));
            */
            /*
            Plane plane = new Plane(Vector3.UNIT_Y, 0);
            MeshManager.Singleton.CreatePlane("ground", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME,
                plane, 1500, 1500, 20, 20, true, 1, 5, 5, Vector3.UNIT_Z);

            // Create a ground plane
            Entity ent = mMgr.CreateEntity("GroundEntity", "ground");
            mMgr.RootSceneNode.CreateChildSceneNode().AttachObject(ent);

            ent.SetMaterialName("Examples/Rockwall");
            ent.CastShadows = false;*/


            //pl.SetPosition(new Vector3(500,500,500));

            /*
            Entity ent = mMgr.CreateEntity("entity" + "1", "ninja.mesh");
            ent.CastShadows = true;
            SceneNode node = mMgr.RootSceneNode.CreateChildSceneNode("object" + "1", new Vector3(512, 64, 512));
            SceneNode offsetNode = node.CreateChildSceneNode();

            offsetNode.AttachObject(ent);
            offsetNode.SetScale(new Vector3(0.01f, 0.01f, 0.01f));

            pl.Node = node;

            pl.Show();
            */
            // end

            //obj.AttachNode(OgreEngine.mGame.World.CreateObject(obj));

            // */

            //mGame.World.Map = new MapManager(this);

            /*for (int i = 5; i < 20; i++)
            {
                GameActorObject obj = new GameActorObject();
                obj.prototype = "ninja.mesh";
                obj.Position = new Vector3(1000+i*500, i * 200+200, 1000);
                obj.Id = i;
                obj.AttachNode(mGame.World.Map.map.AddObjectToScene(obj, mMgr, mGame.World.Map));
                mGame.World.ObjectList.Add(obj);
            }*/
        }

        public void UpdateCamera(float time)
        {
            //updateFmodEx();
            if (mGame.World.Map.Level.mWater != null)
                mGame.World.Map.Level.mWater.Update(time);

            if (DEBUG.Active)
                mCam.PolygonMode = DEBUG.PolygonMode;
        }

        void CreateScene()
        {
            //SceneManager mgr = mRoot.CreateSceneManager(SceneType.ST_GENERIC);
            mMgr = mRoot.CreateSceneManager(SceneType.ST_EXTERIOR_FAR);

            mCam = mMgr.CreateCamera("Camera");
            mCam.FOVy = new Degree(Constants.PlayerCamera.DefaultCamFOV);
            mCam.NearClipDistance = Constants.Video.NearClip;
            mCam.FarClipDistance = Constants.Video.FarClip;

            //Camera cam = mgr.CreateCamera("Camera");
            mRoot.AutoCreatedWindow.AddViewport(mCam);

            mCam.Position = new Vector3(100, Constants.PlayerCamera.DefaultCamHeight, 100);
            mCam.LookAt(new Vector3(100, 0, 101));
            //mCa

            mRoot.FrameEnded += new FrameListener.FrameEndedHandler(FrameEnded);
            mRoot.FrameStarted += new FrameListener.FrameStartedHandler(FrameStarted);

            Drawing2D.Initialize(this);

            mMgr.SetSkyBox(true, "Examples/Sky", 500, false);
            ColourValue fadeColour = new ColourValue(0.4f, 0.6f, 0.6f);
            //mMgr.SetFog(FogMode.FOG_EXP, fadeColour, 0.005f);

            //String p = Path.GetDirectoryName(Application.ExecutablePath) + @"Media\" + "terrain.cfg";
            //mMgr.SetWorldGeometry(p);
            //CreateTerrain();
        }

        void CreateInput()
        {
            mControl = new ControlDevices(this);
            mMenu = new GameMenu(this);
        }

        public void StartAnim(GameSceneObject obj, String animationName)
        {
            //bool used = false;
            int id = 0;

            for (int animi = 0; animi < RunningAnimations.Count; animi++) // looking for animaction in list
            {
                if ((GameSceneObject)(RunningAnimations[animi].activator) == obj) // pokud uz je v seznamu
                    if (RunningAnimations[animi].animState.AnimationName == animationName)
                    {
                        //used = true;
                        id = animi;
                        break;
                    }
            }

            /*
            if (used) // pokud jsme uz objekt animovali
            {
                ObjectAnimation anim = new ObjectAnimation(mMgr.GetEntity("entity" + obj.Id).GetAnimationState(animationName), obj);
                anim.animState.Enabled = true;
                RunningAnimations[id] = anim;
            }

            if (!used && obj != null) // pokud jeste ne, pricdame
            {
                ObjectAnimation anim = new ObjectAnimation(mMgr.GetEntity("entity" + obj.Id).GetAnimationState(animationName), obj);
                anim.animState.Enabled = true;
                RunningAnimations.Add(anim);
            }
             * */
        }

        public void StopAnim(GameSceneObject ent, String animationName) //zastavi animaci
        {
            for (int animi = 0; animi < RunningAnimations.Count; animi++) // najdeme animaci v seznamu podle objektu
            {
                if ((GameSceneObject)(RunningAnimations[animi].activator) == ent)
                    if (RunningAnimations[animi].animState.AnimationName == animationName)
                    {
                        //TODO: kdyz smazu objekt a zustane tady, tak to hapa
                        if (RunningAnimations[animi].animState != null)
                            RunningAnimations[animi].animState.Enabled = false; // a vypneme ji
                    }
            }
            RunningAnimations.Clear();
        }
    }
}
