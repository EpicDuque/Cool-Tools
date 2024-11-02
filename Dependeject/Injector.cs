using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoolTools.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace CoolTools.Dependeject
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public sealed class InjectAttribute : Attribute
    {
        
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ProvideAttribute : Attribute
    {
        
    }

    public interface IDependencyProvider
    {
    }

    public interface IDependencyProviderEditor : IDependencyProvider
    {
    }

    public class Injector : Singleton<Injector>
    {
        private const BindingFlags k_bindingFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly Dictionary<Type, object> registry = new();

        protected override void Awake()
        {
            base.Awake();

            ResolveDependencies();
        }

#if UNITY_EDITOR
        [MenuItem("Tools/CoolTools/Resolve Dependencies")]
        private static void ResolveDependenciesEditor()
        {
            // Find all Prefabs in the Project
            if (Instance == null) return;
            
            Instance.registry.Clear();
            
            string[] guids = AssetDatabase.FindAssets("t:Prefab", searchInFolders: new []{"Assets/2_Prefabs"});
            var providers = new List<IDependencyProviderEditor>();
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                var components = prefab.GetComponentsInChildren<IDependencyProviderEditor>();
                providers.AddRange(components);
                
                foreach (var provider in providers)
                {
                    Instance.RegisterProvider(provider);
                }
            }

            if (providers.Count == 0)
            {
                Debug.Log($"No Providers found.");
                return;
            }
            
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                var dirty = false;
                
                var injectables = prefab.GetComponentsInChildren<MonoBehaviour>()
                    .Where(IsInjectable);

                foreach (var injectable in injectables)
                {
                    dirty = true;
                    Instance.Inject(injectable);
                }
                
                if(dirty)
                    EditorUtility.SetDirty(prefab);
            }
        }
#endif
        
        private void ResolveDependencies()
        {
            registry.Clear();
            var providers = FindMonoBehaviours().OfType<IDependencyProvider>();

            foreach (var p in providers)
                RegisterProvider(p);

            var injectables = FindMonoBehaviours().Where(IsInjectable);

            foreach (var injectable in injectables)
            {
                Inject(injectable);
            }
        }

        private void Inject(object injectable)
        {
            var type = injectable.GetType();

            var injectableFields = type.GetFields(k_bindingFlags)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

            foreach (var field in injectableFields)
            {
                var fieldType = field.FieldType;

                var resolvedInstance = Resolve(fieldType);

                if (resolvedInstance == null)
                {
                    throw new Exception($"Failed to Inject {fieldType.Name} in {type.Name}");
                }

                field.SetValue(injectable, resolvedInstance);
                Debug.Log($"Injected {fieldType.Name} into {type.Name}");
            }
        }

        object Resolve(Type type)
        {
            registry.TryGetValue(type, out var resolvedInstance);

            return resolvedInstance;
        }

        private void RegisterProvider(IDependencyProvider provider)
        {
            var methods = provider.GetType().GetMethods(k_bindingFlags);

            foreach (var method in methods)
            {
                if (!Attribute.IsDefined(method, typeof(ProvideAttribute))) continue;

                var returnType = method.ReturnType;
                var providedInstance = method.Invoke(provider, null);

                var providerName = provider.GetType().Name;
                if (providedInstance != null)
                {
                    if (registry.TryAdd(returnType, providedInstance))
                    {
                        Debug.Log($"Registered {returnType.Name} from {providerName}");
                    }
                }
                else
                {
                    throw new Exception($"Provider {providerName} returned null for {returnType.Name}");
                }
            }
        }

        static bool IsInjectable(MonoBehaviour obj)
        {
            if (obj == null) return false;
            var members = obj.GetType().GetMembers(k_bindingFlags);

            return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
        }
        
        static MonoBehaviour[] FindMonoBehaviours()
        {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);
        }
    }
}