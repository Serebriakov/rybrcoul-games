using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

// Class for drawing 2D sprites

namespace TheGame.Drawing
{
    public static class Drawing2D
    {
        private static OgreEngine _OgreEngine = null;
        
        public static void Initialize(OgreEngine OgreEngine)
        {
            _OgreEngine = OgreEngine;
        }
        
        public static void Create2DElement(String name, String texture, Vector2 TopLeft, Vector2 BottomRight)
        {
                MaterialPtr material = MaterialManager.Singleton.Create(name, "General");
                material.GetTechnique(0).GetPass(0).CreateTextureUnitState(texture);
                material.GetTechnique(0).GetPass(0).DepthCheckEnabled = false;
                material.GetTechnique(0).GetPass(0).DepthWriteEnabled = false;
                material.GetTechnique(0).GetPass(0).LightingEnabled = false;
                // Create background rectangle covering the whole screen
                Rectangle2D rect = new Rectangle2D(true);
                rect.SetCorners(TopLeft.x * 2 - 1, 1 - TopLeft.y * 2, BottomRight.x * 2 - 1, 1 - BottomRight.y * 2);
                //rect.SetCorners(-1.0f, 1.0f, 1.0f, -1.0f);
                rect.SetMaterial(name);

                // Render the background before everything else
                rect.RenderQueueGroup = (byte)RenderQueueGroupID.RENDER_QUEUE_OVERLAY;

                // Use infinite AAB to always stay visible
                AxisAlignedBox aab = new AxisAlignedBox();
                aab.SetInfinite();
                rect.BoundingBox = aab;

                // Attach background to the scene
                SceneNode node = _OgreEngine.mMgr.RootSceneNode.CreateChildSceneNode("2D__" + name);
                node.AttachObject(rect);

        }

        public static void Destroy2DElement(String name)
        {
                _OgreEngine.mMgr.GetSceneNode("2D__" + name).DetachAllObjects();
                _OgreEngine.mMgr.DestroySceneNode("2D__" + name);

        }
    }
}
