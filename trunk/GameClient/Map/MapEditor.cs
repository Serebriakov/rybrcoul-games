using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Map;
using System.Threading;
using TheGame.Drawing;
using HelpingClasses;

namespace TheGame.Map
{
    public enum MapEditorOperation
    {
        TERRAIN_DEFFORM,
        TERRAIN_TEXTURING,
        OBJECT
    }

    public class MapEditor
    {
        private MapLevel Map;
        private OgreEngine OgreEngine;
        private String MapFolder = "";

        private MET.Brush EditingBrush;
        private uint BufferTextureNumber = 1; // number of texture to be painted

        private bool mouseDown = false;
        private MapEditorOperation Operation = MapEditorOperation.TERRAIN_DEFFORM;

        public bool Edit = false;

        public void Update()
        {

        }

        public MapEditor(OgreEngine OgreEngine, MapLevel Map)
        {
            this.OgreEngine = OgreEngine;
            this.Map = Map;
            SetEditingBrush(20);
            MapFolder = OgreEngine.exeDir + @"Media\terrain\";
        }

        public void SaveTerrain(bool fastSave)
        {
            int textureCount = 6;
            Image[] textures = new Image[textureCount];
            for (int i = 0; i < textureCount; i++)
            {
                textures[i] = new Image();
                textures[i].Load("splatting" + i + ".png", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
            }

            if (!fastSave)
            {
                Image baseTexture = new Image();
                Map.mSMgr.CreateBaseTexture(baseTexture, Constants.Land.SimplemapSize, Constants.Land.SimplemapSize, textures, 40, 40);
                baseTexture.Save(MapFolder + "baseTexture.jpg");
            }

            Image splatting = new Image();
            Map.mSMgr.SaveMapToImage(0, splatting);
            splatting.Save(MapFolder + "textureMap0.png");
            splatting = new Image();
            Map.mSMgr.SaveMapToImage(1, splatting);
            splatting.Save(MapFolder + "textureMap1.png");

            Image heightMap = new Image();
            MET.TerrainInfo.SaveHeightmapToImage(Map.mTInfo, heightMap);
            heightMap.Save(MapFolder + "heightMap.png");

            if (!fastSave)
            {
                uint bigsize = Constants.Land.SimplemapSize;
                Image lmImage = new Image();
                MET.TerrainInfo.CreateTerraingLightmap(Map.mTInfo, lmImage, bigsize, bigsize, new Vector3(1, -1.5f, 1), new ColourValue(1, 1, 1), new ColourValue(0.6f, 0.6f, 0.6f), true);
                Image cmImage = new Image();
                Map.mSMgr.CreateBaseTexture(cmImage, bigsize, bigsize, textures, 20, 20);

                System.Drawing.Bitmap mmImage = new System.Drawing.Bitmap((int)bigsize, (int)bigsize);

                for (int y = 0; y < cmImage.Width; ++y)
                {
                    for (int x = 0; x < cmImage.Height; ++x)
                    {
                        float r = cmImage.GetColourAt(x, y, 0).r * lmImage.GetColourAt(x, y, 0).r;
                        float g = cmImage.GetColourAt(x, y, 0).g * lmImage.GetColourAt(x, y, 0).g;
                        float b = cmImage.GetColourAt(x, y, 0).b * lmImage.GetColourAt(x, y, 0).b;
                        mmImage.SetPixel(x, y, System.Drawing.Color.FromArgb((byte)(255 * r), (byte)(255 * g), (byte)(255 * b)));
                    }
                }

                mmImage.Save(MapFolder + "miniMap.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        public void SetEditingCursorScale()
        {
            if (OgreEngine.mMgr.HasSceneNode("EditorCursorNode"))
            {
                SceneNode ln = OgreEngine.mMgr.GetSceneNode("EditorCursorNode");
                ln.SetScale(Vector3.UNIT_SCALE * EditingBrush.Width / 5f);
            }
        }

        public void SetEditingCursorPos(Vector3 curPos)
        {
            if (OgreEngine.mMgr.HasSceneNode("EditorCursorNode"))
            {

            }
            else
            {
                /*l = OgreEngine.mMgr.CreateLight("EditorCursor");
                l.CastShadows = false;
                l.DiffuseColour = new ColourValue(1, 0, 0);
                l.Direction = new Vector3(0, -1, 0);
                l.PowerScale = 10;
                l.RenderingDistance = 10000;
                l.Visible = true;
                l.SetAttenuation(5000, 10, 10, 20);
                l.SetDiffuseColour(1, 0, 0);
                l.SetSpecularColour(1, 0, 0);
                l.Type = Light.LightTypes.LT_SPOTLIGHT;

                l.Type = Light.LightTypes.LT_SPOTLIGHT;
                l.DiffuseColour = ColourValue.Blue;
                l.SpecularColour = ColourValue.Blue;
                l.Direction = new Vector3(-1, -1, 0);
                l.Position = new Vector3(200, 100, 200);
                l.SetSpotlightRange(new Degree(35), new Degree(50));
                l.PowerScale = 2;*/

                Entity e = OgreEngine.mMgr.CreateEntity("cursorlight", "sphere.mesh");
                e.CastShadows = true;

                //MaterialPtr mat = MaterialManager.Singleton.GetByName("editcursor");

                //e.SetMaterial(mat);

                SceneNode n = OgreEngine.mMgr.RootSceneNode.CreateChildSceneNode("EditorCursorNode");
                //n.AttachObject(l);
                n.AttachObject(e);
                n.SetScale(new Vector3(5f, 5f, 5f));
            }
            SceneNode ln = OgreEngine.mMgr.GetSceneNode("EditorCursorNode");
            ln.SetPosition(curPos.x, curPos.y + 1, curPos.z);

        }

        public Vector3 GetCursorPosition(MapEditorOperation operation)
        {
            Vector3 ret = Vector3.ZERO;
            Mogre.Ray ray = OgreEngine.mCam.GetCameraToViewportRay(0.5f, 0.5f);
            Mogre.Vector3 result = Map.mTInfo.RayIntersects(ray);

            if (result != null)
            {
                if (operation == MapEditorOperation.TERRAIN_DEFFORM)
                {
                    ret.x = Map.mTInfo.PosToVertexX(result.x);
                    ret.z = Map.mTInfo.PosToVertexZ(result.z);
                }
                if (operation == MapEditorOperation.TERRAIN_TEXTURING)
                {
                    ret.x = (float)System.Math.Round(result.x / Map.Size.x * Map.TerrainTextureSize - 0.5);
                    ret.z = (float)System.Math.Round(result.z / Map.Size.z * Map.TerrainTextureSize - 0.5);
                }
                if (operation == MapEditorOperation.OBJECT)
                {
                    ret = result;
                }

                /*if (operation == MapEditorOperation.VEGETATION)
                {
                    /*float snapDelta = (OgreEngine.mCam.Position - result).Length / 10f;
                    MousePosInTerrain.X = result.x;
                    MousePosInTerrain.Y = result.z;
                    RbVegetation oldSelVeg = SelectedVegetation;
                    SelectedVegetation = null;
                    float distanceToClosesVegetation = ActiveLand.RubikonLand.SizeX * 2; //big value
                    foreach (RbVegetation veg in ActiveLand.RubikonLand.VegetationList)
                    {
                        float dist = veg.Position.GetDistanceTo(MousePosInTerrain);
                        if (dist < distanceToClosesVegetation && dist < snapDelta)
                        {
                            distanceToClosesVegetation = dist;
                            SelectedVegetation = veg;
                            MousePosInTerrain = veg.Position.Clone();
                            break;
                        }
                    }


                    // deselect
                    if (SelectedVegetation != oldSelVeg && oldSelVeg != null)
                        foreach (RbVegetationSegment seg in oldSelVeg.Segments)
                        {
                            if (VisibleTrees.Contains(seg))
                            {
                                foreach (RbVegetationElement tree in seg.Trees)
                                {
                                    ActiveLand.Manager.GetEntity(tree.TreeUniqueName()).ParentSceneNode.ShowBoundingBox = false;
                                }
                            }
                        }

                    // select
                    if (SelectedVegetation != null && SelectedVegetation != oldSelVeg)
                        foreach (RbVegetationSegment seg in SelectedVegetation.Segments)
                        {
                            if (VisibleTrees.Contains(seg))
                            {
                                foreach (RbVegetationElement tree in seg.Trees)
                                {
                                    ActiveLand.Manager.GetEntity(tree.TreeUniqueName()).ParentSceneNode.ShowBoundingBox = true;
                                }
                            }
                        }
                }

                DrawEditCursor();
                }
                else
                {

                }*/
            }
            return ret;

        }

        public void SetEditingBrush(ushort size)
        {
            if (size < 1) size = 1;
            Mogre.Image image2 = new Mogre.Image();
            image2.Load("brushsmooth.png", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
            image2.Resize(size, size, Image.Filter.FILTER_BICUBIC);
            EditingBrush = MET.Brush.LoadBrushFromImage(image2);
        }

        public void EditTerrain(MapEditorOperation operation, Vector3 MousePos, MOIS.MouseButtonID id)
        {
            if (!Edit) return;
            float intens = 0;
            if (operation == MapEditorOperation.TERRAIN_DEFFORM)
            {
                intens = 0.05f;
                if (id == MOIS.MouseButtonID.MB_Right) intens = -intens;
                Map.mTMgr.Deform((int)MousePos.x, (int)MousePos.z, EditingBrush, intens);
                //DrawEditCursor();
            }
            if (operation == MapEditorOperation.TERRAIN_TEXTURING)
            {
                intens = 0.5f;
                Map.mSMgr.Paint(BufferTextureNumber, (int)MousePos.x, (int)MousePos.z, EditingBrush, intens);
            }
        }

        public void MousePressed(MOIS.MouseEvent arg, MOIS.MouseButtonID id)
        {
            if (Edit)
            {
                mouseDown = true;

                Vector3 cpos = GetCursorPosition(Operation);
                EditTerrain(Operation, cpos, id);
            }
        }

        public void MouseReleased(MOIS.MouseEvent arg, MOIS.MouseButtonID id)
        {
            if (Edit)
            {
                mouseDown = false;
            }
        }

        public void ProcessPressedKeys(List<MOIS.KeyCode> PressedKeys, float evt)
        {
            
        }

        public void KeyPressed(MOIS.KeyEvent arg)
        {
            if (Edit)
            {
                #region Textures
                if (arg.key == MOIS.KeyCode.KC_F1)
                {
                    BufferTextureNumber = 0;
                }

                if (arg.key == MOIS.KeyCode.KC_F2)
                {
                    BufferTextureNumber = 1;
                }

                if (arg.key == MOIS.KeyCode.KC_F3)
                {
                    BufferTextureNumber = 2;
                }

                if (arg.key == MOIS.KeyCode.KC_F4)
                {
                    BufferTextureNumber = 3;
                }

                if (arg.key == MOIS.KeyCode.KC_F5)
                {
                    BufferTextureNumber = 4;
                }

                if (arg.key == MOIS.KeyCode.KC_F6)
                {
                    BufferTextureNumber = 5;
                }
                #endregion

                if (arg.key == MOIS.KeyCode.KC_U)
                {
                    Map.UpdateLightMap();
                    Map.UpdateTerrain();
                }

                if (arg.key == MOIS.KeyCode.KC_SUBTRACT)
                {
                    SetEditingBrush((ushort)(EditingBrush.Width / 2));
                }

                if (arg.key == MOIS.KeyCode.KC_ADD)
                {
                    SetEditingBrush((ushort)(EditingBrush.Width * 2));
                }

                if (arg.key == MOIS.KeyCode.KC_F12)
                {
                    SaveTerrain(true);
                }

                if (arg.key == MOIS.KeyCode.KC_PGDOWN)
                {
                    Map.mWater.Position += new Vector3(0, -1, 0);
                }

                if (arg.key == MOIS.KeyCode.KC_PGUP)
                {
                    Map.mWater.Position += new Vector3(0, +1, 0);
                }

                if (arg.key == MOIS.KeyCode.KC_F11)
                {
                    SaveTerrain(false);
                }

                if (arg.key == MOIS.KeyCode.KC_TAB)
                {
                    Operation = Operation == MapEditorOperation.TERRAIN_DEFFORM ? MapEditorOperation.TERRAIN_TEXTURING : MapEditorOperation.TERRAIN_DEFFORM;
                }
            }
        }
    }
}
