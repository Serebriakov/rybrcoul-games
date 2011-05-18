using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBase;
using Mogre;

namespace TheGame.Controls
{
    public static class AnimController
    {
        /*public static void SetAnimation(GameActorObject obj, String anim)
        { 

        }*/
    }

    public class ObjectAnimation
    {
        public AnimationState animState; // animation
        public GameSceneObject activator; // animated object

        public ObjectAnimation(AnimationState state, GameSceneObject act)
        {
            this.animState = state;
            this.activator = act;
        }
    }
}
