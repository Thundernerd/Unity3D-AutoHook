using System;
using UnityEditor;
using UnityEngine;

namespace TNRD.Autohook
{
    [CustomPropertyDrawer(typeof(AutoHookAttribute))]
    public class AutoHookPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            AutoHookAttribute autoHookAttribute = (AutoHookAttribute) attribute;

            if (autoHookAttribute.HideWhenFound && property.objectReferenceValue != null)
            {
                return 0;
            }

            return EditorGUI.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            AutoHookAttribute autoHookAttribute = (AutoHookAttribute) attribute;

            if (autoHookAttribute.StopSearchWhenFound)
            {
                if (property.objectReferenceValue == null)
                {
                    Component component = FindComponent(property);
                    property.objectReferenceValue = component;
                }
            }
            else
            {
                Component component = FindComponent(property);

                if (property.objectReferenceValue == null && component != null)
                {
                    property.objectReferenceValue = component;
                }
            }

            EditorGUI.BeginDisabledGroup(autoHookAttribute.ReadOnlyWhenFound && property.objectReferenceValue != null);
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.EndDisabledGroup();
        }

        private Component FindComponent(SerializedProperty property)
        {
            SerializedObject root = property.serializedObject;
            if (!(root.targetObject is Component))
            {
                return null;
            }

            Type type = fieldInfo.FieldType;
            Component parent = (Component) root.targetObject;
            AutoHookAttribute autoHookAttribute = (AutoHookAttribute) attribute;

            switch (autoHookAttribute.SearchArea)
            {
                case AutoHookSearchArea.Parent:
                    return parent.GetComponentInParent(type);
                case AutoHookSearchArea.Children:
                    return parent.GetComponentInChildren(type);
                case AutoHookSearchArea.DirectChildrenOnly:
                    return FindComponentInDirectChildren(parent.transform, type);
                case AutoHookSearchArea.AllChildrenOnly:
                    return FindComponentInChildrenRecursive(parent.transform, type);
                default:
                    return parent.GetComponent(type);
            }
        }

        private Component FindComponentInDirectChildren(Transform parent, Type type)
        {
            foreach (Transform child in parent)
            {
                Component component = child.GetComponent(type);
                if (component != null)
                {
                    return component;
                }
            }

            return null;
        }

        private Component FindComponentInChildrenRecursive(Transform parent, Type type)
        {
            foreach (Transform child in parent)
            {
                Component component = child.GetComponent(type);
                if (component != null)
                {
                    return component;
                }

                component = FindComponentInChildrenRecursive(child.transform, type);
                if (component != null)
                {
                    return component;
                }
            }

            return null;
        }
    }
}
