using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Linq;

namespace XUUI.UGUIAdapter
{
    [CustomEditor(typeof(ViewBinding))]
    public class ViewBindingEditor : Editor
    {
        private static List<Type> componentTypes = null;

        private ReorderableList list;

        private static void initComponentTypes()
        {
            var assembly = typeof(Text).Assembly;
            componentTypes = new List<Type>();
            var lastComponentType = Enum.GetValues(typeof(ComponentType)).Cast<int>().Max();

            for (int i = 0; i <= lastComponentType; i++)
            {
                componentTypes.Add(assembly.GetType("UnityEngine.UI." + ((ComponentType)i).ToString(), true));
            }
        }

        private void OnEnable()
        {
            const int SPLIT_WIDTH = 30;

            if (componentTypes == null)
            {
                initComponentTypes();
            }

            list = new ReorderableList(serializedObject,
                    serializedObject.FindProperty("Bindings"),
                    true, true, true, true);

            list.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) => {
                    var element = list.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;

                    var type = element.FindPropertyRelative("Type");
                    var component = element.FindPropertyRelative("Component");
                    var componentType = componentTypes[type.intValue];

                    if (component.objectReferenceValue != null && component.objectReferenceValue.GetType() != componentType)
                    {
                        component.objectReferenceValue = null;
                    }

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, 80, EditorGUIUtility.singleLineHeight),
                        type, GUIContent.none);
                    rect.x += 80;

                    EditorGUI.LabelField(new Rect(rect.x + 10, rect.y, SPLIT_WIDTH, EditorGUIUtility.singleLineHeight), "|");
                    rect.x += SPLIT_WIDTH;


                    component.objectReferenceValue = EditorGUI.ObjectField(
                        new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight),
                        component.objectReferenceValue, componentType, true);
                    rect.x += 120;

                    EditorGUI.LabelField(new Rect(rect.x + 10, rect.y, SPLIT_WIDTH, EditorGUIUtility.singleLineHeight), "|");
                    rect.x += SPLIT_WIDTH;

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width - rect.x - 60, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("BindTo"), GUIContent.none);
                    rect.x += rect.width - rect.x - 65;

                    EditorGUI.LabelField(new Rect(rect.x + 10, rect.y, SPLIT_WIDTH, EditorGUIUtility.singleLineHeight), "|");
                    rect.x += SPLIT_WIDTH;

                    EditorGUI.PropertyField(
                        new Rect(rect.x + 20, rect.y, 20, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("MultiFields"), GUIContent.none);
                };

            list.drawHeaderCallback = (Rect rect) =>
            {
                rect.x += 14;

                EditorGUI.LabelField(new Rect(rect.x, rect.y, 80, EditorGUIUtility.singleLineHeight), "Type");
                rect.x += 80;

                EditorGUI.LabelField(new Rect(rect.x + 10, rect.y, SPLIT_WIDTH, EditorGUIUtility.singleLineHeight), "|");
                rect.x += SPLIT_WIDTH;


                EditorGUI.LabelField(new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight), "Component");
                rect.x += 120;

                EditorGUI.LabelField(new Rect(rect.x + 10, rect.y, SPLIT_WIDTH, EditorGUIUtility.singleLineHeight), "|");
                rect.x += SPLIT_WIDTH;

                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - rect.x - 60, EditorGUIUtility.singleLineHeight), "BindTo");
                rect.x += rect.width - rect.x - 79;

                EditorGUI.LabelField(new Rect(rect.x + 10, rect.y, SPLIT_WIDTH, EditorGUIUtility.singleLineHeight), "|");
                rect.x += SPLIT_WIDTH;

                EditorGUI.LabelField(new Rect(rect.x, rect.y, 70, EditorGUIUtility.singleLineHeight), "MultiFields");
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
