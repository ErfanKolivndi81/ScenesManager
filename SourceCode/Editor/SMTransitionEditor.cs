using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace IRK.Unity.ScenesManager
{
	[CustomEditor(typeof(Transition))]
	public class SMTransitionEditor : Editor
	{
		private Transition tr;

		SerializedProperty _isLoadAsync;
		SerializedProperty _loadSceneMode;
		SerializedProperty _localPhysicsMode;

		private string _nameSceneFrom;
		private string _nameSceneTo;
		private TransitionEffectSettingsEditor _editorEffectUnload;
		private TransitionEffectSettingsEditor _editorEffectLoad;
		private DC _dc;

		private void OnEnable()
		{
			_dc = new DC();
			_dc.Add("S_From", "From", "Index builds a scene that starts the transition");
			_dc.Add("S_To", "To", "The index builds the scene you want to load");
			_dc.Add("S_Mode", "Load Scene Mode");
			_dc.Add("S_PMode", "Local Physics Mode");
			_dc.Add("tgl_async", "Load Async Mode");

			tr = (Transition)target;

			_isLoadAsync			=	 serializedObject.FindProperty("_data.isLoadAsync");
			_loadSceneMode		=	 serializedObject.FindProperty("_data.loadSceneMode");
			_localPhysicsMode	=	 serializedObject.FindProperty("_data.localPhysicsMode");

			//Initialize TransitionEffectSettings
			_nameSceneFrom = SMEditorUtility.GetNameSceneBuild(tr.data.fromSceneIndex, false);
			_nameSceneTo = SMEditorUtility.GetNameSceneBuild(tr.data.toSceneIndex, false);

			//TransitionEffectSettings - Unload
			_editorEffectUnload = new TransitionEffectSettingsEditor()
			{
				indexEffectSelected = IndexOfEffect(tr.data.effectUnload),
				type = TransitionEffectType.Unload,
				effect = tr.data.effectUnload,
				onChangeEffect = OnChangeEffect,
			};
			_editorEffectUnload.OnEnable();

			//TransitionEffectSettings - Load
			_editorEffectLoad = new TransitionEffectSettingsEditor()
			{
				indexEffectSelected = IndexOfEffect(tr.data.effectLoad),
				type = TransitionEffectType.Load,
				effect = tr.data.effectLoad,
				onChangeEffect = OnChangeEffect,
			};
			_editorEffectLoad.OnEnable();

			Undo.undoRedoPerformed += OnUndoRedoPerformad;
		}

		private void OnDisable()
		{
			Undo.undoRedoPerformed -= OnUndoRedoPerformad;
		}

		private void OnUndoRedoPerformad()
		{
			Debug.Log("OnUndoRedoPerformad");
			serializedObject.Update();
			serializedObject.ApplyModifiedProperties();

			Repaint();
			_editorEffectUnload.editor.Repaint();
			_editorEffectLoad.editor.Repaint();
		}

		private void OnChangeEffect(TransitionEffect effect,TransitionEffectType type)
		{
			tr.data[type] = effect;
			Repaint();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			OnGUIInfo();
			EditorGUILayout.Space();
			OnGUITransitionSettings();
			EditorGUILayout.Space();

			//Effects
			_editorEffectUnload.OnInspectorGUI();
			EditorGUILayout.Space();
			_editorEffectLoad.OnInspectorGUI();

			serializedObject.ApplyModifiedProperties();
		}        

		private void OnGUIInfo()
		{
			EditorGUI.BeginDisabledGroup(true);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.TextField(_dc["S_From"], _nameSceneFrom);
			EditorGUILayout.IntField("", tr.data.fromSceneIndex, GUILayout.Width(50.0f));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.TextField(_dc["S_To"], _nameSceneTo);
			EditorGUILayout.IntField("", tr.data.toSceneIndex, GUILayout.Width(50.0f));
			EditorGUILayout.EndHorizontal();

			EditorGUI.EndDisabledGroup();
		}

		private void OnGUITransitionSettings()
		{
			EditorGUILayout.PropertyField(_isLoadAsync, _dc["tgl_async"]);
			EditorGUILayout.PropertyField(_loadSceneMode, _dc["S_Mode"]);
			EditorGUILayout.PropertyField(_localPhysicsMode, _dc["S_PMode"]);
		}

		private int IndexOfEffect(TransitionEffect effect)
		{
			int result = -1;
			try 
			{
				string name = effect.GetType().Name;
				result = ScenesManagerWindow.nameTransitionEffects.IndexOf(name); 
			}
			catch 
			{
				result = -1;	
			}

			return result;
		}
	}
}
