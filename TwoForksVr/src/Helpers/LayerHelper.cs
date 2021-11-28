using System;
using System.Linq;
using UnityEngine;

namespace TwoForksVr.Helpers
{
    public enum GameLayer
    {
        // Layers included in base game:
        Default = 0,
        TransparentFX = 1,
        IgnoreRaycast = 2, // Note: layer name is actually "Ignore Raycast".
        Water = 4,
        UI = 5,
        PlayerOnly = 8,
        Terrain = 9,
        DynamicObjects = 10,
        RaycastOnly = 11,
        StopsPlayer = 12,
        MenuBackground = 13,
        RopeClimbCollision = 14,
        PhysicsHackCollision = 15,
        PutBack = 16,
        
        // Custom VR layers:
        PlayerBody = 17,
    }

    public static class LayerHelper
    {
        public static int GetMask(params GameLayer[] layers)
        {
          if (layers == null) throw new ArgumentNullException(nameof (layers));
          return layers.Aggregate(0, (current, layer) => current | 1 << (int) layer);
        }
        
        public static void SetLayer(Component component, GameLayer layer) {
            SetLayer(component.gameObject, layer);
        }
        
        public static void SetLayer(GameObject gameObject, GameLayer layer) {
            gameObject.layer = (int) layer;
        }
    }
}
