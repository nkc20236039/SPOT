using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CheckNullReferenceWhenStartEditor
{
    [InitializeOnLoad]
    public static class CheckNullReferenceWhenStart
    {
        private sealed class NullData
        {
            public Component Component { get; private set; }
            public string RootPath { get; private set; }
            public string ComponentName { get; private set; }
            public string FieldName { get; private set; }

            public NullData
            (
                Component component,
                string rootPath,
                string componentName,
                string fieldName
            )
            {
                Component = component;
                RootPath = rootPath;
                ComponentName = componentName;
                FieldName = fieldName;
            }
        }

        static CheckNullReferenceWhenStart()
        {
            EditorApplication.playModeStateChanged += OnChange;
        }

        private static void OnChange(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode) return;

            var list = Validate().ToArray();

            if (list == null || list.Length <= 0) return;

            foreach (var n in list)
            {
                Debug.LogError($"ŽQÆ‚ªÝ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñF{n.RootPath}, {n.ComponentName}, {n.FieldName}", n.Component);
            }

            EditorApplication.isPlaying = false;
        }

        private static IEnumerable<NullData> Validate()
        {
            var list = Resources
                .FindObjectsOfTypeAll<GameObject>()
                .Where(c => c.scene.isLoaded)
                .Where(c => c.hideFlags == HideFlags.None)
            ;

            foreach (var go in list)
            {
                var components = go.GetComponents<Component>();

                foreach (var component in components)
                {
                    var type = component.GetType();
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    foreach (var field in fields)
                    {
                        var attrs = field.GetCustomAttributes(typeof(JetBrains.Annotations.NotNullAttribute), true)
#if ODIN_INSPECTOR
                            .Concat( field.GetCustomAttributes( typeof( Sirenix.OdinInspector.RequiredAttribute ), true ) )
                            .ToArray()
#endif
                        ;

                        if (attrs == null || attrs.Length <= 0) continue;

                        var value = field.GetValue(component);

                        //if ( value != null ) continue;
                        if (value.ToString() != "null") continue;

                        var data = new NullData
                        (
                            component: component,
                            rootPath: component.gameObject.GetRootPath(),
                            componentName: component.GetType().Name,
                            fieldName: field.Name
                        );

                        yield return data;
                    }
                }
            }
        }

        private static string GetRootPath(this GameObject gameObject)
        {
            var path = gameObject.name;
            var parent = gameObject.transform.parent;

            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }
    }
}