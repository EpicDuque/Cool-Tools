using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace CoolTools.Editor
{
    public class TransformRulerWindow : ToolWindow
    {
        private Transform[] transforms;
        private int transformCount;
        private float perimeter;
        private float distance;
        private bool showSettings;
        private Color lineColor = Color.green;

        private float dottedLineSize = 5f;
        private const int MinTransformCount = 2;
        private const int MaxTransformCount = 20;

        [MenuItem("Tools/CoolTools/Transform Ruler")]
        private static void ShowWindow()
        {
            var window = GetWindow<TransformRulerWindow>();
            window.titleContent = new GUIContent("Transform Ruler");
            window.Show();
        }

        public TransformRulerWindow()
        {
            transforms = new Transform[MaxTransformCount];
            lineColor = Color.green;
        }

        // Window has been selected
        private void OnFocus()
        {
            // Remove delegate listener if it has previously
            // been assigned.
            SceneView.duringSceneGui -= OnSceneGUI;
            // Add (or re-add) the delegate.
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            MeasureDistances();

            DrawDottedLines();

            DrawHandlesGUI();
        }

        private void OnGUI()
        {
            DrawTransformCountProperty();

            DrawTransformFieldArray();

            DrawSeparator();

            DrawWindowMeasures();

            DrawSeparator(5f, 12f);

            DrawRulerSettings();
        }

        private void DrawRulerSettings()
        {
            if (GUILayout.Button("Toggle Ruler Settings", GUILayout.Width(150)))
            {
                showSettings = !showSettings;
            }

            EditorGUILayout.Space();

            if (showSettings)
            {
                lineColor = EditorGUILayout.ColorField("Segment Color", lineColor);
                dottedLineSize = EditorGUILayout.FloatField("Dotted Line Size", dottedLineSize);
            }
        }

        private void DrawHandlesGUI()
        {
            Handles.BeginGUI();
            // Scene view GUI Layout... very cool
            var rect = new Rect()
            {
                x = 10,
                y = 10,
                width = 150,
                height = 40,
            };
            EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f, 0.5f));

            rect.x += 5;
            rect.height = 20;

            var labelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                normal = { textColor = Color.white }
            };

            EditorGUI.LabelField(rect, $"Distance : {distance}", labelStyle);
            rect.y += 20;

            EditorGUI.LabelField(rect, $"Perimeter : {perimeter}", labelStyle);

            Handles.EndGUI();
        }

        private void DrawDottedLines()
        {
            // Create GUIStyle for labels.
            var labelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                normal = { textColor = Color.white }
            };

            var oldColor = Handles.color;
            Handles.color = lineColor;
            for (var i = 0; i < transformCount; i++)
            {
                if (i == 0 || transforms[i] == null) continue;

                // Draws dotted line between last position and current position
                Handles.DrawDottedLine(transforms[i].position, transforms[i - 1].position, dottedLineSize);

                // Draws distance label in the middle of the line
                var distance = Vector3.Distance(transforms[i].position, transforms[i - 1].position);
                var midPos = Vector3.Lerp(transforms[i - 1].position, transforms[i].position, 0.5f);
                Handles.Label(midPos, distance.ToString(CultureInfo.InvariantCulture), new GUIStyle(labelStyle));

                if (transformCount <= 2 || i != transformCount - 1) continue;

                // Draw extra DottedLine and Label if this is the last point
                Handles.DrawDottedLine(transforms[i].position, transforms[0].position, dottedLineSize);

                distance = Vector3.Distance(transforms[i].position, transforms[0].position);
                midPos = Vector3.Lerp(transforms[i].position, transforms[0].position, 0.5f);
                Handles.Label(midPos, distance.ToString(CultureInfo.InvariantCulture), new GUIStyle(labelStyle));
            }

            Handles.color = oldColor;
        }

        private void DrawTransformCountProperty()
        {
            // Draw transformCount Property
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("Transform Count");
            transformCount = EditorGUILayout.IntField(transformCount);

            if (GUILayout.Button("-"))
            {
                transformCount--;
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("+"))
            {
                transformCount++;
                SceneView.RepaintAll();
            }

            EditorGUILayout.EndHorizontal();

            // Clamps transformCount Value
            transformCount = Mathf.Clamp(transformCount, MinTransformCount, MaxTransformCount);

            EditorGUILayout.Space();
        }



        private void DrawTransformFieldArray()
        {
            for (var i = 0; i < transformCount; i++)
            {
                // transforms[i] = (Transform) EditorGUILayout.ObjectField(transforms[i], typeof(Transform), true);
                DrawTransformField(ref transforms[i], $"Pos {i + 1}");
            }
        }

        private void DrawWindowMeasures()
        {
            EditorGUILayout.HelpBox("Linear distance between all points from start to end.", MessageType.None);
            EditorGUILayout.LabelField($"Distance : {distance}", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Sum of distances of all segments, including between end and start.",
                MessageType.None);
            EditorGUILayout.LabelField($"Perimeter : {perimeter}", EditorStyles.boldLabel);

            EditorGUILayout.Space();
        }

        private void MeasureDistances()
        {
            perimeter = 0f;
            distance = 0f;
            for (var i = 0; i < transformCount; i++)
            {
                if (transforms[i] == null) continue;

                if (i > 0)
                {
                    distance += Vector3.Distance(transforms[i - 1].position, transforms[i].position);
                }

                if (transformCount > 2 && i == transformCount - 1)
                {
                    perimeter = distance;
                    perimeter += Vector3.Distance(transforms[i].position, transforms[0].position);
                }
            }
        }
    }
}