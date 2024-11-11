using CoolTools.Actors;
using UnityEditor;
using UnityEngine;

namespace CoolTools.Utilities
{
    [InitializeOnLoad]
    public class HierarchyActors
    {
        private static Texture2D _icon;

        static HierarchyActors()
        {
            _icon = AssetDatabase.LoadAssetAtPath("Assets/Resources/CoolTools/Icons/Icon_Person.png", typeof(Texture2D)) as Texture2D;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGui;
        }

        private static void HierarchyWindowItemOnGui(int instanceid, Rect selectionrect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceid) as GameObject;
            
            if (go != null && go.TryGetComponent<Actor>(out _))
            {
                var r = new Rect(selectionrect)
                {
                    x = selectionrect.x -= 20,
                    width = 18
                };

                GUI.DrawTexture(r, _icon);
            }
        }
    }
}