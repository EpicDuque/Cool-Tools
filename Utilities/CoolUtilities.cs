using UnityEngine;
using Scene = UnityEngine.SceneManagement.Scene;

namespace CoolTools.Utilities
{
    public static class CoolUtilities
    {
        public static int MaxLineOfSightHits = 20;
        private static RaycastHit[] hits = new RaycastHit[100];
        
        // public static bool IsLineObstructed(this Vector3 origin, Vector3 target, LayerMask blockedLayers, PhysicsScene physicsScene, 
        //     QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        // {
        //     var direction = target - origin;
        //
        //     if (direction.sqrMagnitude < 0.75f) return true;
        //
        //     var amount = physicsScene.Raycast(origin, direction, hits, direction.magnitude, blockedLayers,
        //         queryTriggerInteraction);
        //
        //     return amount > 0;
        // }
        //
        // public static bool IsLineObstructed(this Vector3 origin, Vector3 target, LayerMask blockedLayers, Scene fromScene, 
        //     QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        // {
        //     return IsLineObstructed(origin, target, blockedLayers, fromScene.GetPhysicsScene(),
        //         queryTriggerInteraction);
        // }
        
        public static Vector3 GetPointFromDirectionWithOffset(this Vector3 direction, float offset)
        {
            return direction + direction.normalized * offset;
        }
    }
}