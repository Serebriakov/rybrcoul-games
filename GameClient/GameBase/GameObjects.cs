using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using System.Collections;
using HelpingClasses;
using Map;
using TheGame.Controls;
using TheGame;
using TheGame.HelpingClasses;

// Class defining default Game objects

namespace GameBase
{
    public enum GameState
    {
        PAUSED = 0,
        MAINMENU = 1,
        RUNNING = 2,
        INTRO1 = 3,
        INTRO2 = 4,
    }

    public class Game
    {
        public GameWorld World = null;
        public long Frames = 0;
        public GameSettings GameSettings = new GameSettings();
        public PlayerCamera Camera = null;

        private GameState _State = GameState.RUNNING;
        private OgreEngine OgreEngine;

        public void Update(float time)
        {
            if (State == GameState.RUNNING) World.Update(time);
            if (State == GameState.RUNNING) Frames++;
        }

        public GameState State
        {
            get
            {
                return this._State;
            }

            set
            {
                this._State = value;
            }
        }

        public Game(OgreEngine OgreEngine)
        {
            this.OgreEngine = OgreEngine;
            World = new GameWorld(OgreEngine);
            GameSettings.LoadFromFile(OgreEngine.exeDir + "game.ini");
            this.Camera = new PlayerCamera(this);
        }
    }

    public class GameWorld
    {
        public GameObjectList ObjectList = null;
        public WorldMap Map = null;

        private SceneManager mMgr = null;
        private OgreEngine OgreEngine = null;


        public GameWorld(OgreEngine OgreEngine)
        {
            this.OgreEngine = OgreEngine;
            ObjectList = new GameObjectList();
            this.mMgr = OgreEngine.mMgr;
        }

        /*
        public SceneNode CreateObject(GameActorObject obj)
        {
            SceneNode node = mMgr.RootSceneNode.CreateChildSceneNode("node" + obj.Id);
            Entity ent = mMgr.CreateEntity("entity" + obj.Id, obj.prototype);
            node.AttachObject(ent);
            node.SetScale(1f, 1f, 1f);

            return node;
        }*/

        public void Update(float time)
        {
            Map.Update(time);
        }

        public void Animate()
        {
            /*foreach (GameObject obj in ObjectList)
            { 
                //obj.
            }*/
            // animates animations
        }
        // todo: animationlist
    }

    public class GameObjectList : IEnumerable
    {
        private Dictionary<int, GameObject> actors;

        public GameObjectList()
        {
            actors = new Dictionary<int, GameObject>();
        }

        /// <summary>
        /// Adds actor to actors list.
        /// </summary>
        /// <param name="obj">Actor to be added.</param>
        public void Add(GameObject obj)
        {
            if (obj != null)
                if (!actors.ContainsKey(obj.Id))
                {
                    actors.Add(obj.Id, obj);
                }
        }

        public GameObject this[int id]
        {
            get
            {
                if (actors.ContainsKey(id))
                    return actors[id];
                else
                    return null;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return actors.Values.GetEnumerator();
        }
    }

    public class GameObjectAnimation
    {
        public AnimationState animState;
        public GameSceneObject activator;

        public GameObjectAnimation(AnimationState state, GameSceneObject act)
        {
            this.animState = state;
            this.activator = act;
        }
    }

    public class PlayerCamera
    {
        private Vector3 position = Vector3.ZERO;
        public Vector3 LookAt = Vector3.ZERO;

        private Game game = null;

        public bool FreeCamera = false;

        public PlayerCamera(Game g)
        {
            this.game = g;
            this.position = new Vector3(100, Constants.PlayerCamera.DefaultCamHeight, 100);
        }

        public Vector3 Position
        {
            get { return position; }

            set
            {
                float h = 1f; // o kolik nad zemi kamera ma zustat

                float y = game.World.Map.GetWorldHeight(value.x, value.z);
                position = new Vector3(value.x, System.Math.Max(value.y, y + h), value.z);
                this.Update();
            }
        }

        public void Update()
        {
            LookAt = new Vector3(position.x, 0, position.z + 1 - Constants.PlayerCamera.MinCamHeight);
        }

        public float Height
        {
            get { return this.Position.y; }
        }
    }

    public class GameObject
    {
        public int Id;
        public String prototype = "";
        public int LandId;

        private Quaternion _Orientation = Quaternion.IDENTITY;
        private Vector3 _Position = Vector3.ZERO;

        public GameObject()
        {

        }

        virtual public Vector3 Position
        {
            get
            {
                return _Position;
            }

            set
            {
                _Position = value;
            }
        }

        virtual public Quaternion Orientation
        {
            get
            {
                return _Orientation;
            }

            set
            {
                _Orientation = value;
            }
        }

        public Byte[] Serialize()
        {
            ByteList data = new ByteList();

            data.Add(Id);
            data.Add(prototype);
            data.Add(LandId);

            return data.GetArray();
        }
    }

    public class GameSceneObject : GameObject
    {
        // some scene object, this can be some rock, tree, ...
        internal SceneNode Node = null;

        public void AttachNode(SceneNode node)
        {
            this.Node = node;
        }

        override public Vector3 Position
        {
            get
            {
                if (Node != null)
                    return Node.Position;
                else return base.Position;
            }

            set
            {
                if (Node != null)
                    Node.Position = value;
                base.Position = value;
            }
        }

        override public Quaternion Orientation
        {
            get
            {
                if (Node != null)
                    return Node.Orientation;
                else return base.Orientation;
            }

            set
            {
                if (Node != null)
                    Node.Orientation = value;
                base.Orientation = value;
            }
        }

        public GameSceneObject()
        {

        }

        public void SetPos(Vector3 newPos)
        {
            Node.SetPosition(newPos.x, newPos.y, newPos.z);
        }

        public void SetRot(Quaternion newRot)
        {
            Node.SetOrientation(newRot.w, newRot.x, newRot.y, newRot.z);
        }

        public void SetScale(Vector3 newScale)
        {
            Node.SetScale(newScale);
        }

        public void SetVisible(bool show)
        {
            Node.SetVisible(show);
        }

        public void Show()
        {
            SetVisible(true);
        }

        public void Hide()
        {
            SetVisible(false);
        }
    }
}
