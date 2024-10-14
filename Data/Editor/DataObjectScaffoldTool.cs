using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;
using UnityEditor;
using UnityEngine;

namespace CoolTools.Editor
{
    public class DataObjectScaffoldTool : EditorWindow
    {
        private string _typeName = "";
        private string _typeNamespace = "";
        private string[] _imports = new string[1];
        private string _eventScriptPath = "Assets/Scripts/Generated";
        private string _editorScriptPath = "Assets/Scripts/Editor/Generated";

        private int _importToRemove;
        
        [MenuItem("Window/Data Object Scaffolder")]
        public static void ShowWindow()
        {
            GetWindow<DataObjectScaffoldTool>("Data Object Scaffolder");
        }

        private void OnGUI()
        {
            _importToRemove = -1;
            
            EditorGUILayout.BeginVertical("box");
            _typeName = EditorGUILayout.TextField("Type Name", _typeName);
            EditorGUILayout.Space(5);
            _typeNamespace = EditorGUILayout.TextField("Type Namespace", _typeNamespace);
            EditorGUILayout.Space(5);
            _eventScriptPath = EditorGUILayout.TextField("Event Script Path", _eventScriptPath);
            _editorScriptPath = EditorGUILayout.TextField("Editor Script Path", _editorScriptPath);
            EditorGUILayout.EndVertical();

            // Create a text field to enter an import. Create add and remove buttons for each import in the list
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Imports");
            for (int i = 0; i < _imports.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _imports[i] = EditorGUILayout.TextField(_imports[i]);
                if (GUILayout.Button("-", GUILayout.Width(35)))
                {
                    _imports[i] = "";
                    _importToRemove = i;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            if (GUILayout.Button("New Import"))
            {
                ArrayUtility.Add(ref _imports, "");
            }
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(15);
            var originalColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.14f, 0.64f, 0.2f, 0.62f);
            if (GUILayout.Button("Generate Event and Listener", GUILayout.Height(30)))
            {
                GenerateEventAndListener();
            }
            GUI.backgroundColor = originalColor;
            
            if(_importToRemove > -1)
            {
                ArrayUtility.RemoveAt(ref _imports, _importToRemove);
            }
        }
        
        private void GenerateEventAndListener()
        {
            var provider = new CSharpCodeProvider();
            var compileUnit = new CodeCompileUnit();
            var globalNamespace = new CodeNamespace();
            
            compileUnit.Namespaces.Add(globalNamespace);
            
            var namespaceName = new CodeNamespace(_typeNamespace);
            compileUnit.Namespaces.Add(namespaceName);

            AddImports(globalNamespace);

            // Generate Event class
            var eventClass = GenerateEventClass(namespaceName);

            // Save to .cs file
            var fileName = $"{_eventScriptPath}/{_typeName}Event.cs";
            GenerateCode(provider, compileUnit, fileName);
            
            namespaceName.Types.Remove(eventClass);
            
            // Generate Listener class
            var listenerClass = GenerateListenerClass(namespaceName);

            // Save to .cs file
            fileName = $"{_eventScriptPath}/{_typeName}EventListener.cs";
            GenerateCode(provider, compileUnit, fileName);
            
            namespaceName.Types.Remove(listenerClass);
            
            // Generate Editor class
            var editorClass = GenerateEditorClass(namespaceName);
            
            AddEditorImports(namespaceName);
            
            // Save to .cs file
            fileName = $"{_editorScriptPath}/{_typeName}EventEditor.cs";
            GenerateCode(provider, compileUnit, fileName);

            AssetDatabase.Refresh();
            
            // Trigger a code recompile
            AssetDatabase.ImportAsset(fileName);
        }

        private CodeTypeDeclaration GenerateEventClass(CodeNamespace namespaceName)
        {
            var eventClass = new CodeTypeDeclaration($"{_typeName}Event")
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public,
                BaseTypes = { new CodeTypeReference($"GameEventSO<{_typeName}>") }
            };
            
            // Add CreateAssetMenu attribute
            var attributes = new CodeAttributeDeclaration("CreateAssetMenu")
            {
                Arguments =
                {
                    new CodeAttributeArgument("fileName", new CodePrimitiveExpression($"New {_typeName}Event")),
                    new CodeAttributeArgument("menuName", new CodePrimitiveExpression("Event/" + _typeName))
                }
            };
            
            eventClass.CustomAttributes.Add(attributes);
            
            namespaceName.Types.Add(eventClass);
            return eventClass;
        }

        private CodeTypeDeclaration GenerateListenerClass(CodeNamespace namespaceName)
        {
            var listenerClass = new CodeTypeDeclaration($"{_typeName}EventListener")
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public,
                BaseTypes = { new CodeTypeReference($"GameEventListener<{_typeName}Event, {_typeName}>") }
            };
            namespaceName.Types.Add(listenerClass);

            return listenerClass;
        }

        private CodeTypeDeclaration GenerateEditorClass(CodeNamespace namespaceName)
        {
            var editorClass = new CodeTypeDeclaration($"{_typeName}EventEditor")
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public,
                BaseTypes = { new CodeTypeReference($"GameEventSOEditor<{_typeName}Event, {_typeName}>") }
            };
            namespaceName.Types.Add(editorClass);
            
            // Add CustomEditor attribute
            var customEditorAttribute = new CodeAttributeDeclaration("CustomEditor")
            {
                Arguments =
                {
                    new CodeAttributeArgument(new CodeTypeOfExpression($"{_typeName}Event"))
                }
            };
            editorClass.CustomAttributes.Add(customEditorAttribute);

            return editorClass;
        }

        private void AddImports(CodeNamespace globalNamespace)
        {
            globalNamespace.Imports.Add(new CodeNamespaceImport("CoolTools.Data"));
            globalNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
            
            // Add all _imports
            foreach (var import in _imports)
            {
                if (!string.IsNullOrEmpty(import))
                {
                    globalNamespace.Imports.Add(new CodeNamespaceImport(import));
                }
            }
        }
        
        private void AddEditorImports(CodeNamespace globalNamespace)
        {
            globalNamespace.Imports.Add(new CodeNamespaceImport("CoolTools.Editor"));
            globalNamespace.Imports.Add(new CodeNamespaceImport("UnityEditor"));
            globalNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
            
            // Add all _imports
            foreach (var import in _imports)
            {
                if (!string.IsNullOrEmpty(import))
                {
                    globalNamespace.Imports.Add(new CodeNamespaceImport(import));
                }
            }
        }

        private void GenerateCode(CSharpCodeProvider provider, CodeCompileUnit compileUnit, string path)
        {
            using (var sw = new StreamWriter(path))
            {
                var tw = new IndentedTextWriter(sw, "    ");
                provider.GenerateCodeFromCompileUnit(compileUnit, tw, new CodeGeneratorOptions());
            }
        }
    }
}