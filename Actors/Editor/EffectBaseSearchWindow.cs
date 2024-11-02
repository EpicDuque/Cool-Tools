using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CoolTools.Actors.Editor
{
    public class EffectBaseSearchWindow : EditorWindow
    {
        private string _searchTerm = string.Empty;
        private List<Type> _effectTypes = new List<Type>();
        private Action<EffectBase> _onEffectSelected;
        private EffectBase _assignedEffect;

        public static EffectBaseSearchWindow ShowWindow(Action<EffectBase> onEffectSelected)
        {
            var window = CreateInstance<EffectBaseSearchWindow>();
            window.titleContent = new GUIContent("EffectBase Search");
            window._onEffectSelected = onEffectSelected;
            
            // Position the window to where the mouse is
            var mousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            window.position = new Rect(mousePosition.x, mousePosition.y, 300, 450);
            
            window.ShowPopup();

            return window;
        }

        private void OnEnable()
        {
            // Get all EffectBase types from the TypeCache
            _effectTypes = TypeCache.GetTypesDerivedFrom<EffectBase>()
                .Where(t => !t.IsAbstract)
                .ToList();
        }

        private void OnGUI()
        {
            // Make the window background darker
            var rect = new Rect(0, 0, position.width, position.height);
            var color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            EditorGUI.DrawRect(rect, color);
            
            
            EditorGUILayout.LabelField("Search EffectBase Types", EditorStyles.boldLabel);
            _searchTerm = EditorGUILayout.TextField("Search Term", _searchTerm);

            EditorGUILayout.Space();

            var filteredTypes = _effectTypes
                .Where(t => string.IsNullOrEmpty(_searchTerm) || t.Name.IndexOf(_searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            // Begin a scroll view
            var scrollHeight = 300;
            
            EditorGUILayout.BeginScrollView(Vector2.zero, GUILayout.Height(scrollHeight));
            
            foreach (var type in filteredTypes)
            {
                if (GUILayout.Button(type.Name))
                {
                    CreateEffectInstance(type);
                }
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space();
            
            // Draw an EffectBase object field
            _assignedEffect = EditorGUILayout.ObjectField("Manual Assign Effect", _assignedEffect, typeof(EffectBase), false) as EffectBase;
            
            if (GUILayout.Button("Embed Assigned Effect"))
            {
                _onEffectSelected?.Invoke(_assignedEffect);
                Close();
            }
            
            var originalColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            // CLose button
            if (GUILayout.Button("Close", GUILayout.Height(25f)))
            {
                Close();
            }
            
            GUI.backgroundColor = originalColor;
        }

        private void CreateEffectInstance(Type type)
        {
            var instance = CreateInstance(type);
            _onEffectSelected?.Invoke(instance as EffectBase);
            Close();
        }
        
    }
}