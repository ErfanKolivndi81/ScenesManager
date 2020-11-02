using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace IRK.Unity.ScenesManager
{
    [CustomEditor(typeof(TranslateEffect))]
    public class TranslateEffectEdior : TransitionEffectEditor
    {
        SerializedProperty translateMode;
        SerializedProperty position;
        SerializedProperty direction;
        SerializedProperty teImage;

        protected override void OnEnable()
        {
            base.OnEnable();

            translateMode = serializedObject.FindProperty("mode");
            position = serializedObject.FindProperty("position");
            direction = serializedObject.FindProperty("direction");
            teImage = serializedObject.FindProperty("image");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnGUIGeneralSettings();

            EditorGUILayout.PropertyField(teImage);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(translateMode);
            switch (translateMode.enumValueIndex)
            {
                case 0:// TranslateEffect.TranslateEffectMode.One:
                    EditorGUILayout.PropertyField(position);
                    break;
                case 1:// TranslateEffect.TranslateEffectMode.Two:
                    EditorGUILayout.PropertyField(direction);
                    break;
                case 2:// TranslateEffect.TranslateEffectMode.Four:
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}