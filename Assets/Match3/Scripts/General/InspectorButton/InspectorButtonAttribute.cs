using UnityEditor;
using UnityEngine;

namespace General.InspectorButton
{
    public class InspectorButtonAttribute : PropertyAttribute
    {
        public string MethodName { get; private set; }

        public InspectorButtonAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
    [CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
    public class InspectorButtonDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InspectorButtonAttribute buttonAttribute = (InspectorButtonAttribute)attribute;
            Object target = property.serializedObject.targetObject;
            var methodName = buttonAttribute.MethodName;
            if(GUI.Button(position, methodName))
            {
                var method = target.GetType().GetMethod(methodName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (method != null)
                {
                    method.Invoke(target, null);
                }
                else
                {
                    Debug.LogWarning($"Método '{methodName}' no encontrado en {target.GetType()}");
                }
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}