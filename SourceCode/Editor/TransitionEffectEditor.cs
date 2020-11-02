using UnityEditor;
using UnityEngine;

namespace IRK.Unity.ScenesManager
{
    [CustomEditor(typeof(TransitionEffect))]
    public class TransitionEffectEditor : Editor
    {
        private bool _disabledDuration;
        private bool _disabledSleep;

        protected virtual void OnEnable()
        {
            _disabledDuration = !target.GetType().IsDefined(typeof(CustomDurationAttribute),false);
            _disabledSleep = !target.GetType().IsDefined(typeof(CustomSleepAttribute),false);
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();

            SerializedProperty serializedProperty = OnGUIGeneralSettings();
            //bool expanded = true;
            while (serializedProperty.NextVisible(false))
            {
                EditorGUILayout.PropertyField(serializedProperty, true);

               // expanded = false;
            }
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
                OnChangeValue();
        }       

        protected virtual void OnChangeValue()
        {
            Undo.RecordObject(target,"Change value transitionEffect properties");
        }

        protected SerializedProperty OnGUIGeneralSettings()
        {
            SerializedProperty serializedProperty = serializedObject.GetIterator();
            bool expanded = true;
            while (serializedProperty.NextVisible(expanded))
            {
                bool disabledDuration = serializedProperty.name == "duration" && _disabledDuration && ScenesManagerWindow.SMCore.durationTransitionEffectsAll;
                bool disabledSleep = serializedProperty.name == "sleep" && _disabledSleep && ScenesManagerWindow.SMCore.sleepTransitionEffectsAll;

                using (new EditorGUI.DisabledGroupScope(serializedProperty.propertyPath == "m_Script" || disabledDuration || disabledSleep))
                {
                    if (disabledDuration)
                        serializedProperty.floatValue = ScenesManagerWindow.SMCore.durationTransitionEffect;

                    if (disabledSleep)
                        serializedProperty.floatValue = ScenesManagerWindow.SMCore.sleepTransitionEffect;

                    EditorGUILayout.PropertyField(serializedProperty, true);
                }
                expanded = false;

                //last property TransitionEffect
                if (serializedProperty.name == "sleep")
                    break;
            }
            return serializedProperty;
        }
    }
}