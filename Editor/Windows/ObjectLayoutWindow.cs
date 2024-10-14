using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoolTools.Editor
{
    public class ObjectLayoutWindow : ToolWindow
    {
        private Transform positionStart;
        private Transform positionEnd;
        private float layoutDistance;
        private Transform[] lastSelection;

        private float Spacing
        {
            get => spacing;
            set => spacing = Mathf.Clamp01(value);
        }

        private bool autoApply;
        private float spacing;

        [MenuItem("CoolTools/Object Layout Tool")]
        private static void ShowWindow()
        {
            var window = GetWindow<ObjectLayoutWindow>();
            window.titleContent = new GUIContent("Object Layout Tool");
            window.Show();
        }

        public ObjectLayoutWindow()
        {
            // lastSelection = Selection.gameObjects.Select(g => g.transform).ToArray();
        }

        private void OnFocus()
        {
            // Remove delegate listener if it has previously
            // been assigned.
            SceneView.duringSceneGui -= OnSceneGUI;
            // Add (or re-add) the delegate.
            SceneView.duringSceneGui += OnSceneGUI;
        }

        public void OnInspectorUpdate()
        {
            // This will only get called 10 times per second.
            Repaint();
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (autoApply)
                ApplyLayout();
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("Assign start and end positions of linear layout", MessageType.None);

            // Draw Start field
            DrawTransformField(ref positionStart, "Layout Start", 80f);

            // Draw End field
            DrawTransformField(ref positionEnd, "Layout End", 80f);

            DrawSeparator();

            if (!ValidPositions())
            {
                EditorGUILayout.HelpBox("Assign valid Start and End positions.", MessageType.Error);
                return;
            }

            // layoutMode = (LayoutMode) EditorGUILayout.EnumPopup("Layout Mode", layoutMode);

            DrawSpacingSlider();

            EditorGUILayout.Space();

            if (lastSelection is { Length: 0 } && Selection.count <= 0)
            {
                autoApply = false;

                EditorGUILayout.HelpBox("Select objects in Hirearchy to apply Layout", MessageType.Info);
                return;
            }

            if (lastSelection != null && lastSelection.Length > 0)
                autoApply = EditorGUILayout.Toggle("Auto Apply", autoApply);

            if (autoApply)
            {
                ApplyLayout();
            }
            else if (GUILayout.Button("Apply Layout to Selection", GUILayout.Height(25)))
            {
                lastSelection = Selection.gameObjects.Select(g => g.transform).ToArray();
                ApplyLayout();
            }

            DrawSeparator(30f, 12f);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Layout Objects");

            if (GUILayout.Button("Select Objects"))
            {
                Selection.objects = lastSelection.Select(t => (Object)t.gameObject).ToArray();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (lastSelection == null || lastSelection.Length == 0) return;

            EditorGUI.BeginDisabledGroup(true);
            foreach (var t in lastSelection)
            {
                EditorGUILayout.ObjectField(t, typeof(Transform), false);
            }

            EditorGUI.EndDisabledGroup();
        }

        private float GetLayoutDistance()
        {
            return Vector3.Distance(positionStart.position, positionEnd.position);
        }

        private bool ValidPositions() => positionStart != null && positionEnd != null;

        private void DrawSpacingSlider()
        {
            // var enabled = layoutMode != LayoutMode.Length;
            //
            EditorGUI.BeginDisabledGroup(true);

            var width = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 55;
            Spacing = EditorGUILayout.Slider("Spacing", Spacing, 0f, 1f);
            EditorGUIUtility.labelWidth = width;

            EditorGUI.EndDisabledGroup();
        }

        private void ApplyLayout()
        {
            if (lastSelection.Length <= 0 || !ValidPositions()) return;

            Spacing = 1f / (lastSelection.Length - 1);

            var lerp = 0f;
            for (var i = 0; i < lastSelection.Length; i++)
            {
                var t = lastSelection[i];

                t.position = Vector3.Lerp(positionStart.position, positionEnd.position, lerp);
                lerp += Spacing;
            }
        }
    }
}