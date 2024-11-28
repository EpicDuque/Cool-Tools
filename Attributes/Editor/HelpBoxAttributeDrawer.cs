using UnityEditor;
using UnityEngine;

namespace CoolTools.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxAttributeDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            var helpBox = attribute as HelpBoxAttribute;
            
            var helpBoxHeight = GetHelpBoxHeight();
            
            EditorGUI.HelpBox(new Rect(position) {height = helpBoxHeight}, helpBox.Text, MessageType.Info);
        }

        private float GetHelpBoxHeight()
        {
            return EditorGUIUtility.singleLineHeight * 2 + 4;
        }

        public override float GetHeight()
        {
            var helpBox = attribute as HelpBoxAttribute;
            
            return GetHelpBoxHeight() + 5;
        }
    }
}