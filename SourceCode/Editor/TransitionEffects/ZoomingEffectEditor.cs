using UnityEditor;

namespace IRK.Unity.ScenesManager
{
    [CustomEditor(typeof(ZoomingEffect))]
    public class ZoomingEffectEditor : TransitionEffectEditor
    {
        SerializedProperty _teImage;
        SerializedProperty _rotate;
        SerializedProperty _minAngle;
        SerializedProperty _maxAngle;
        DC _dc;

        protected override void OnEnable()
        {
            base.OnEnable();
            _dc = new DC();
            _dc.Add("teImage", "Image");
            _dc.Add("rotate", "Rotate");
            _dc.Add("A_Min", "MinAngle");
            _dc.Add("A_Max", "MaxAngle");

            try
            {
                _rotate = serializedObject.FindProperty("rotate");
                _minAngle = serializedObject.FindProperty("minAngle");
                _maxAngle = serializedObject.FindProperty("maxAngle");
                _teImage = serializedObject.FindProperty("image");
            }
            catch { return; }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            base.OnGUIGeneralSettings();

            EditorGUILayout.PropertyField(_teImage,_dc["teImage"]);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_rotate,_dc["rotate"]);
            using(new EditorGUI.DisabledGroupScope(!_rotate.boolValue))
            {
                EditorGUILayout.PropertyField(_minAngle, _dc["A_Min"]);
                EditorGUILayout.PropertyField(_maxAngle, _dc["A_Max"]);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}