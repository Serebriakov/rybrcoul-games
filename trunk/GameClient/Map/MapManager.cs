using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Mogre;
using System.Windows.Forms;
using HelpingClasses;
using GameBase;
using MET;
using TheGame;
using TheGame.Map;
using TheGame.Drawing;
using TheGame.Controls;

// Class for managing map

namespace Map
{
    /// <summary>
    /// Class controlling showing grass, trees, bushes, rocks and other decoratives
    /// 
    /// </summary>
    public class DecorationManager
    {
        private Vector3 PageSize;
        private OgreEngine OgreEngine;
        private MapLevel Map;

        public DecorationManager(OgreEngine OgreEngine)
        {
            this.OgreEngine = OgreEngine;
            this.Map = OgreEngine.mGame.World.Map.Level;
        }

        public void Update(Vector3 camPos, float timeElapsed)
        {
            Map.Update(timeElapsed);
            //foreach (DecorationPage p in Map)
        }
    }

    public class DecorationPage
    {
        private Random rnd;
        private int Id;
        private SceneNode node = null;
        private SceneManager sMgr = null;
        private MapLevel mapPage = null;
        private StaticGeometry sg = null;

        float xinc = Mogre.Math.PI * 0.4f;
        float zinc = Mogre.Math.PI * 0.55f;
        float xpos = Mogre.Math.RangeRandom(-Mogre.Math.PI, Mogre.Math.PI);
        float zpos = Mogre.Math.RangeRandom(-Mogre.Math.PI, Mogre.Math.PI);

        public DecorationPage(int id, SceneManager sMgr, MapLevel mapPage)
        {
            this.sMgr = sMgr;
            this.Id = id;
            rnd = new Random(Id * 763);
            this.mapPage = mapPage;
        }

        public void CreateGeometry()
        {
            Plane p = new Plane(Vector3.UNIT_X, 0);
            MeshManager.Singleton.CreatePlane("grassplane", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME, p, 1f, 1f, 1, 1, true, 1, 1F, 1F, Vector3.UNIT_Y);

            sg = sMgr.CreateStaticGeometry("Decoration" + Id);
            Entity grass = sMgr.CreateEntity("grass", "grassplane");
            grass.SetMaterialName("Examples/GrassBlades");
            grass.CastShadows = false;

            int id = 0;
            for (int x = 0; x < 1000; x++)
                for (int y = 0; y < 1000; y++)
                {
                    CreateGrassCluster(new Vector3(x * 1, mapPage.GetWorldHeight(x, y) + 1, y * 1), 0, 3, id++, grass);
                }

            sg.RenderingDistance = 100;
            sg.RegionDimensions = new Vector3(50, 50, 50);
            sg.Build();
            sg.CastShadows = false;
            sg.SetVisible(true);//*/

            /*
            Entity e = sMgr.CreateEntity("grassbig", "grassplane");
            e.SetMaterialName("Examples/GrassBlades");
            SceneNode n = sMgr.RootSceneNode.CreateChildSceneNode("grassn");
            n.AttachObject(e);
            n.SetPosition(500,50,500);
            n.SetScale(200,200,200);//*/
        }

        public void DestroyGeometry()
        {
            if (node != null) node.DetachAllObjects();
            sMgr.DestroySceneNode(node);
        }

        private void CreateGrassCluster(Vector3 pos, int type, int planecount, int id, Entity grass)
        {

            for (int i = 0; i < planecount; i++)
            {

                //Vector3 Rotation = new Vector3(0, (float)rnd.NextDouble() * 2 * 3.141593f, 0);
                Vector3 Rotation = new Vector3(0, (float)rnd.NextDouble() * 2f * 3.141593f, 0);
                //Rotation.Normalise();
                sg.AddEntity(grass, pos, Helpers.QFromV3(Rotation), new Vector3(1, 1, 1));
                //node.AttachObject(grass);
                /*Vector3 Rotation = new Vector3(1+rnd.Next(20), 0, 1+rnd.Next(20));
                Rotation.Normalise();
                Entity grass = sMgr.CreateEntity("grass" + id + " " + i, "grassplane" + id + " " + i);
                grass.SetMaterialName("Examples/GrassBlades");
                node.AttachObject(grass);
                node.SetScale(10,10,10);
                grass.CastShadows = false;*/
            }
        }

        private void WaveGrass(float timeElapsed)
        {
            xpos += xinc * timeElapsed;
            zpos += zinc * timeElapsed;

            // Update vertex program parameters by binding a value to each renderable
            Vector4 offset = new Vector4(0, 0, 0, 0);

            xpos += 1 * 0.001f;
            zpos += 1 * 0.001f;
            offset.x = Mogre.Math.Sin(xpos) * 1f;
            offset.z = Mogre.Math.Sin(zpos) * 1f;

            Entity grass = sMgr.GetEntity("grass");
            grass.GetSubEntity(0).SetCustomParameter(999, offset);

            /*Entity grass = sMgr.GetEntity("grassbig");
            SceneNode n = sMgr.GetSceneNode("grassn");

            float angle = 0;
            float ttime = timeElapsed;// +pair.Key.ParentSceneNode.Position.x + pair.Key.ParentSceneNode.Position.y;
            bool pravy = System.Math.Sin(ttime) > 0;
            if (pravy) angle = ttime * 2; else angle = -ttime * 2 + (float)System.Math.PI;
            float size = 0.05f;

            Vector4 offset = new Vector4(
                (pravy ? -size : size) + size * (float)System.Math.Cos(angle),
                3 * size / 4 * (float)System.Math.Sin(angle), 0, 0);

            Vector4 leavesCenter = n.Position + (n.Orientation * new Vector3(0,0,0));
                grass.GetSubEntity(0).SetCustomParameter(1, leavesCenter);
                grass.GetSubEntity(0).SetCustomParameter(2, offset);
            */
        }

        public void Update(float timeElapsed)
        {
            //WaveGrass(timeElapsed);
        }
    }

    public class MapManager
    {
        public MapLevel Level = null;

        public MapEditor MapEditor = null;

        private OgreEngine OgreEngine;

        private DecorationPage p = null;

        public void Dispose()
        {
            Level.Dispose();
        }

        public void Update(float time)
        {
            Level.Update(time);

            //p.Update(time);

            if (MapEditor != null)
            {
                if (MapEditor.Edit)
                {
                    Vector3 curPos = MapEditor.GetCursorPosition(MapEditorOperation.OBJECT);

                    MapEditor.SetEditingCursorPos(curPos);
                    MapEditor.SetEditingCursorScale();

                    //if (OgreEngine.mGame.Frames % 10 == 0) MapEditor.Update();
                    //updateLightMap();
                }
            }
        }

        public MapManager(OgreEngine OgreEngine)
        {
            this.OgreEngine = OgreEngine;
        }

        public void LoadMap(String file)
        {
            Level = new MapLevel(0, OgreEngine, Vector3.ZERO, new Vector3(1000, 1000, 1000));
            //MapPage Page2 = new MapPage(1, OgreEngine, new Vector3(1000,0,0), new Vector3(1000, 1000, 1000));
            //Pages.Add(Page);
            MapEditor = new MapEditor(OgreEngine, Level);
            //p = new DecorationPage(0, OgreEngine.mMgr, OgreEngine.mGame.World.Map.Pages[0]);
            long t = DateTime.Now.Ticks;
            //p.CreateGeometry();
            //MessageBox.Show("time: "+(DateTime.Now.Ticks-t));
        }
    }


    public class MapLevel
    {
        public int Id;
        public Vector3 Size; // level size
        public Vector3 Position; // level size

        public TerrainManager mTMgr = null;
        public TerrainInfo mTInfo = null;
        public SplattingManager mSMgr = null;

        private OgreEngine OgreEngine = null;
        private uint TextureSize = 1;

        public MHydrax.MHydrax mWater = null;

        public void Dispose()
        {
            mWater.Dispose();
            mTMgr.Dispose();
            mTInfo.Dispose();
            mSMgr.Dispose();
        }

        public uint TerrainTextureSize
        {
            get
            {
                return TextureSize;
            }
        }

        public void Update(float time)
        {

        }

        public MapLevel(int Id, OgreEngine OgreEngine, Vector3 pos, Vector3 size)
        {
            this.OgreEngine = OgreEngine;
            this.Id = Id;
            this.Size = size;
            this.Position = pos;

            CreateTerrain();
        }

        public void UpdateLightMap()
        {
            try
            {
                Mogre.Image lmImage = new Mogre.Image();
                MET.TerrainInfo.CreateTerraingLightmap(mTInfo, lmImage, Constants.Land.LightmapSize, Constants.Land.LightmapSize, new Vector3(-1, -1, -1), new ColourValue(1, 1, 1), new ColourValue(0.5f, 0.5f, 0.5f), true);

                TexturePtr tex = TextureManager.Singleton.GetByName("ETLightmap" + "0");
                tex.GetBuffer(0, 0).BlitFromMemory(lmImage.GetPixelBox(0, 0));
            }
            catch (Exception e)
            {
                LogManager.Singleton.LogMessage("Lightmap exception: "+e.StackTrace);
                // in some cases lightmap crashes...never mind
            }
            ///
        }

        public void UpdateTerrain()
        {

        }

        public float GetWorldHeight(float x, float z)
        {
            return GetVisualWorldHeight(mTInfo, x, z);
        }

        private float GetVisualWorldHeight(TerrainInfo terInfo, float x, float z)
        {
            int ix = terInfo.PosToVertexX(x);
            int iz = terInfo.PosToVertexZ(z);


            if (ix >= terInfo.Width - 1) ix = (int)terInfo.Width - 2;
            if (iz >= terInfo.Height - 1) iz = (int)terInfo.Height - 2;

            Vector3 p11 = new Vector3(terInfo.VertexToPosX(ix), terInfo.VertexToPosZ(iz), terInfo.At((uint)ix, (uint)iz));
            Vector3 p21 = new Vector3(terInfo.VertexToPosX(ix + 1), terInfo.VertexToPosZ(iz), terInfo.At((uint)ix + 1, (uint)iz));
            Vector3 p22 = new Vector3(terInfo.VertexToPosX(ix + 1), terInfo.VertexToPosZ(iz + 1), terInfo.At((uint)ix + 1, (uint)iz + 1));
            Vector3 p12 = new Vector3(terInfo.VertexToPosX(ix), terInfo.VertexToPosZ(iz + 1), terInfo.At((uint)ix, (uint)iz + 1));

            float t, s, ret;

            if (x - p11.x < p12.y - z)
            {
                t = p21.z - p11.z;
                s = p12.z - p11.z;
                ret = p11.z + (x - p11.x) / terInfo.Scaling.x * t + (z - p11.y) / terInfo.Scaling.z * s;
            }
            else
            {
                t = p22.z - p12.z;
                s = p22.z - p21.z;
                ret = p22.z + (x - p22.x) / terInfo.Scaling.x * t + (z - p22.y) / terInfo.Scaling.z * s;
            }

            ret = ret * terInfo.Scaling.y + terInfo.Offset.y;
            //float ret2 = terInfo.GetHeightAt(x, z);
            return ret;
        }

        private void CreateTerrain()
        {
            mTMgr = new MET.TerrainManager(OgreEngine.mMgr, "MET" + Id);

            mTMgr.SetLodErrorMargin(2, OgreEngine.mWin.Height);
            mTMgr.SetUseLodMorphing(true, 0.2f, "morphFactor");

            mTInfo = new MET.TerrainInfo();
            Mogre.Image image = new Mogre.Image();

            image.Load("heightmap.png", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
            MET.TerrainInfo.LoadHeightmapFromImage(mTInfo, image);
            //mTInfo.Extents = new Mogre.AxisAlignedBox(0, 0, 0, land.SizeX + stepWidth, land.SizeZ, land.SizeY + stepHeight);
            mTInfo.Extents = new Mogre.AxisAlignedBox(Position, Position + Size);

            //lightmap
            Mogre.Image lmImage = new Mogre.Image();
            MET.TerrainInfo.CreateTerraingLightmap(mTInfo, lmImage, Constants.Land.LightmapSize, Constants.Land.LightmapSize, new Vector3(-1, -1, -1), new ColourValue(1, 1, 1), new ColourValue(0.2f, 0.2f, 0.2f), true);
            //lmImage.Save("lightmapa" + land.Id + ".png");
            TexturePtr lightmapTex = TextureManager.Singleton.CreateManual(
                "ETLightmap" + Id,
                ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME,
                Mogre.TextureType.TEX_TYPE_2D,
                Constants.Land.LightmapSize,
                Constants.Land.LightmapSize,
                1,
                Mogre.PixelFormat.PF_BYTE_RGB);

            TexturePtr tex = TextureManager.Singleton.GetByName("ETLightmap" + Id);
            tex.GetBuffer(0, 0).BlitFromMemory(lmImage.GetPixelBox(0, 0));

            mTMgr.CreateTerrain(mTInfo);
            mTInfo.Dispose();
            mTInfo = mTMgr.TerrainInfo;

            image = new Image();
            image.Load("textureMap0.png", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
            TextureSize = image.Width;
            mSMgr = new MET.SplattingManager("ETSplatting", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME, image.Width, image.Width, 3);
            mSMgr.NumTextures = 6;

            mSMgr.LoadMapFromImage(0, image);
            image = new Image();
            image.Load("textureMap1.png", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
            mSMgr.LoadMapFromImage(1, image);

            mTMgr.Material = MaterialManager.Singleton.GetByName("ETTerrainMaterial");

            // lightmapa
            Pass pass = mTMgr.Material.GetTechnique(0).CreatePass();
            pass.LightingEnabled = false;
            pass.SetSceneBlending(SceneBlendType.SBT_MODULATE);
            pass.SetVertexProgram("ET/Programs/VSLodMorph2");
            pass.SetFragmentProgram("ET/Programs/PSLighting");
            pass.CreateTextureUnitState("ETLightmap");

            pass = mTMgr.Material.GetTechnique(1).CreatePass();
            pass.LightingEnabled = false;
            pass.SetSceneBlending(SceneBlendType.SBT_MODULATE);
            pass.SetVertexProgram("ET/Programs/VSLodMorph2");
            pass.SetFragmentProgram("ET/Programs/PSLighting");
            pass.CreateTextureUnitState("ETLightmap");

            //base texture for non shader graphics
            lmImage = new Mogre.Image();
            //TODO: replace by real bic texture
            lmImage.Load("baseTexture.jpg", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
            //lmImage.Save("lightmapa" + land.Id + ".png");
            lightmapTex = TextureManager.Singleton.CreateManual(
                "ETBasemap",
                ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME,
                Mogre.TextureType.TEX_TYPE_2D,
                Constants.Land.LightmapSize,
                Constants.Land.LightmapSize,
                1,
                Mogre.PixelFormat.PF_BYTE_RGB);

            tex = TextureManager.Singleton.GetByName("ETBasemap");
            tex.GetBuffer(0, 0).BlitFromMemory(lmImage.GetPixelBox(0, 0));

            Technique tech = mTMgr.Material.CreateTechnique();
            pass = tech.CreatePass();
            pass.CreateTextureUnitState("ETBasemap");

            CreateWater();
        }

        public void CreateWater()
        {
            //return;
            if (OgreEngine.mGame.GameSettings.Water == GameSettings.VideoQuality.VERYLOW)
            {
                Plane p;
                p.normal = Vector3.UNIT_Y;
                p.d = 0;
                MeshManager.Singleton.CreatePlane("WaterPlane", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME, p, 6000F, 6000F, 1, 1, true, 1, 500F, 500F, Vector3.UNIT_Z);
                Entity water = OgreEngine.mMgr.CreateEntity("water", "WaterPlane");
                water.CastShadows = false;
                water.SetMaterialName("Examples/TextureEffect4");//
                SceneNode waterNode = OgreEngine.mMgr.RootSceneNode.CreateChildSceneNode("WaterNode", new Mogre.Vector3(1000, 50, 1000));
                waterNode.AttachObject(water);
            }
            else
            {
                mWater = new MHydrax.MHydrax(OgreEngine.mMgr, OgreEngine.mCam, OgreEngine.mWin.GetViewport(0));

                if (OgreEngine.mGame.GameSettings.Water >= GameSettings.VideoQuality.HIGH)
                    mWater.Components |= MHydrax.MHydraxComponent.HYDRAX_COMPONENT_CAUSTICS;

                mWater.Components |= MHydrax.MHydraxComponent.HYDRAX_COMPONENT_DEPTH;

                if (OgreEngine.mGame.GameSettings.Water >= GameSettings.VideoQuality.MEDIUM)
                    mWater.Components |= MHydrax.MHydraxComponent.HYDRAX_COMPONENT_FOAM;

                if (OgreEngine.mGame.GameSettings.Water >= GameSettings.VideoQuality.MEDIUM)
                    mWater.Components |= MHydrax.MHydraxComponent.HYDRAX_COMPONENT_SMOOTH;

                if (OgreEngine.mGame.GameSettings.Water >= GameSettings.VideoQuality.HIGH)
                    mWater.Components |= MHydrax.MHydraxComponent.HYDRAX_COMPONENT_SUN;

                mWater.Components |= MHydrax.MHydraxComponent.HYDRAX_COMPONENT_UNDERWATER;

                /*
                if (OgreEngine.mGame.GameSettings.Water >= GameSettings.VideoQuality.ULTRA)
                    mWater.Components |= MHydrax.MHydraxComponent.HYDRAX_COMPONENT_UNDERWATER_GODRAYS;
                */
                if (OgreEngine.mGame.GameSettings.Water >= GameSettings.VideoQuality.MEDIUM)
                    mWater.Components |= MHydrax.MHydraxComponent.HYDRAX_COMPONENT_UNDERWATER_REFLECTIONS;

                int complexity = 256;
                if (OgreEngine.mGame.GameSettings.Water >= GameSettings.VideoQuality.LOW) complexity = 16;
                if (OgreEngine.mGame.GameSettings.Water >= GameSettings.VideoQuality.MEDIUM) complexity = 32;
                if (OgreEngine.mGame.GameSettings.Water >= GameSettings.VideoQuality.HIGH) complexity = 128;
                if (OgreEngine.mGame.GameSettings.Water >= GameSettings.VideoQuality.ULTRA) complexity = 256;

                MHydrax.MProjectedGrid m = new MHydrax.MProjectedGrid(mWater, new MHydrax.MPerlin(new MHydrax.MPerlin.MOptions(8, 0.085f, 0.49f, 1.4f, 1.27f, 2f, new Vector3(0.5f, 50f, 150000f))),
                                                new Plane(new Vector3(0, 1, 0), new Vector3(0, 0, 0)),
                                                MHydrax.MMaterialManager.MNormalMode.NM_VERTEX,
                                                new MHydrax.MProjectedGrid.MOptions(complexity, 35f, 50f, false, false, true, 3.75f));
                mWater.SetModule(m);

                mWater.Visible = true;

                // #Main options
                mWater.Position = new Vector3(10000, 50, 10000);
                mWater.PlanesError = 100f;
                mWater.ShaderMode = MHydrax.MMaterialManager.MShaderMode.SM_HLSL;
                mWater.FullReflectionDistance = 100000000000;
                mWater.GlobalTransparency = 0;
                mWater.NormalDistortion = 0.075f;
                mWater.WaterColor = new Vector3(0.139765f, 0.359464f, 0.425373f);
                // #Sun parameters
                mWater.SunPosition = new Vector3(0, 10000, 0);
                mWater.SunStrength = 1.75f;
                mWater.SunArea = 150;
                mWater.SunColor = new Vector3(1f, 0.9f, 0.6f);
                // #Foam parameters
                mWater.FoamMaxDistance = 75000000;
                mWater.FoamScale = 0.0075f;
                mWater.FoamStart = 0;
                mWater.FoamTransparency = 1;
                // #Depth parameters
                mWater.DepthLimit = 20;
                // #Smooth transitions parameters
                mWater.SmoothPower = 5;
                // #Caustics parameters
                mWater.CausticsScale = 135;
                mWater.CausticsPower = 10.5f;
                mWater.CausticsEnd = 0.8f;
                // #God rays parameters
                mWater.GodRaysExposure = new Vector3(0.76f, 2.46f, 2.29f);
                mWater.GodRaysIntensity = 0.015f;
                mWater.GodRaysManager.SimulationSpeed = 5;
                mWater.GodRaysManager.NumberOfRays = 100;
                mWater.GodRaysManager.RaysSize = 0.03f;
                mWater.GodRaysManager.ObjectsIntersectionsEnabled = false;

                mWater.Create();//*/

                mWater.MaterialManager.AddDepthTechnique(((MaterialPtr)MaterialManager.Singleton.GetByName("ETTerrainMaterial")).CreateTechnique(), true);

                /*Create.MaterialManager.AddDepthTechnique( _
                    CType(MaterialManager.Singleton.GetByName("Island"), MaterialPtr) _
                    .CreateTechnique())*/
                //mWater.MaterialManager.AddDepthTechnique(new Technique(MaterialManager.Singleton.GetByName("Island")));
                //mWater.MaterialManager.AddDepthTechnique(MaterialManager.Singleton.GetByName("Island"));
            }
        }

        public void Load(Stream s)
        {
            // todo: loading map from xml
            /*
            XmlTextReader reader = new XmlTextReader(File);
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            XmlNode root = doc.GetElementsByTagName("objects")[0];

            if (root == null)
            {

            }
            else
            {
                foreach (XmlNode n in root.ChildNodes)
                {
                    p.PrototypeLoadFromXmlNode(n);

                    if (temp.ContainsKey(p.Name))
                    {
                    }
                    else
                    {
                        temp.Add(p.Name, p);
                    }
                }
            }*/

            /*
             
             if (n == null)
            {
                RbReporter.Write(RbReporterType.TYPE_XML, "*** CHYBA: XmlNode roven null");
            }
            else
            {
                if (n.Name == "object")
                {
                    if (n.Attributes["name"] == null)
                    {
                        RbReporter.Write(RbReporterType.TYPE_XML, "*** CHYBA: objekt nemá definované jméno");
                    }
                    else
                    {
                        this.Name = n.Attributes["name"].Value;
                        RbReporter.Write(RbReporterType.TYPE_XML, "name:" + this.Name);

                        if (n.Attributes["extends"] != null)
                        {
                            if (n.Attributes["extends"] == n.Attributes["name"])
                            {
                                RbReporter.Write(RbReporterType.TYPE_XML, "*** CHYBA: objekt '" + n.Attributes["name"] + " má jako předka nastaven sám sebe");
                            }
                            else
                            {
                                this.Extends = n.Attributes["extends"].Value;
                            }
                        }

                        foreach (XmlNode n2 in n.ChildNodes)
                        {
                            switch (n2.Name.ToLower())
                            {
                                case "mesh":
                                    {
                                        this.Mesh = n2.InnerText;
                                        this.Filled.Add("mesh|");
                                    }
                                    break;

                                case "description":
                                    {
                                        this.Description = n2.InnerText;
                                        this.Filled.Add("desc|");
                                    }
                                    break;

                                case "weight":
                                    {
                                        this.Weight = Convert.ToSingle(n2.InnerText);
                                        this.Filled.Add("weight|");
                                    }
                                    break;

                                case "bearing":
                                    {
                                        this.Bearing = Convert.ToSingle(n2.InnerText);
                                        this.Filled.Add("bearing|");
                                    }
                                    break;
              
            */
        }
    }
}
