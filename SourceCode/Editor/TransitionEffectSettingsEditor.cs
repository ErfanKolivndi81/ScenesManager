using UnityEngine;
using UnityEditor;

namespace IRK.Unity.ScenesManager
{
	public class TransitionEffectSettingsEditor
	{
		public delegate void OnChangeEffect(TransitionEffect e,TransitionEffectType t);

		public TransitionEffectType type;
		public TransitionEffect effect;
		public int indexEffectSelected = -1;
		public Editor editor;

		private bool _foldout = true;
		private GUIContent _content;
		private DC _dc;

		public OnChangeEffect onChangeEffect;

		public TransitionEffectSettingsEditor()
		{
		}

		public void OnEnable()
		{
			_dc = new DC();
			_dc.Add("TE_Load", "Load Transition Effect");
			_dc.Add("TE_Unload", "Unload Transition Effect");
			_dc.Add("Es", "Effects");

			_content = type == TransitionEffectType.Load ? _dc["TE_Load"] : _dc["TE_Unload"];

			if (effect == null)
			{
				indexEffectSelected = -1;
				int indexEffectNone = ScenesManagerWindow.nameTransitionEffects.IndexOf("NoneEffect");
				ChangeEffect(indexEffectNone);
			}
			else
				CreateEditor();
		}

		public void OnInspectorGUI()
		{
			_foldout = EditorGUILayout.BeginFoldoutHeaderGroup(_foldout, _content);
			if (_foldout)
			{
				DrawRectEffectSettings(EditorGUILayout.BeginVertical());

				EditorGUI.BeginChangeCheck();
				int newIndexEffectSelected = EditorGUILayout.Popup(_dc["Es"], indexEffectSelected, ScenesManagerWindow.nameTransitionEffects.ToArray());
				if (EditorGUI.EndChangeCheck())
				{
					ChangeEffect(newIndexEffectSelected);
				}

				editor.OnInspectorGUI();

				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
		}

		private void ChangeEffect(int indexEffect)
		{
			if (indexEffect == indexEffectSelected)
				return;

			indexEffectSelected = indexEffect;

			string nameNewEffect = ScenesManagerWindow.nameTransitionEffects[indexEffectSelected];
			TransitionEffect newEffect = (TransitionEffect)ScriptableObject.CreateInstance(nameNewEffect);
			newEffect.name = nameNewEffect;

			Undo.RegisterCreatedObjectUndo(newEffect, "Chnage TranistionEffect");

			effect = newEffect;

			onChangeEffect?.Invoke(effect,type);
			CreateEditor();
			editor.Repaint();
		}

		private void CreateEditor()
		{
			if (editor != null)
				Editor.DestroyImmediate(editor);

			try
			{
				editor = (TransitionEffectEditor)Editor.CreateEditor(effect);
			}
			catch (System.InvalidCastException)
			{
				editor = Editor.CreateEditor(effect, typeof(TransitionEffectEditor));
			}
		}

		private void DrawRectEffectSettings(Rect rect)
		{
			float offset = 2.0f;
			Rect rectDraw = new Rect()
			{
				x = rect.x - offset,
				y = rect.y - offset,
				width = rect.width + (offset * 2.0f),
				height = rect.height + (offset * 2.0f),
			};

			EditorGUI.DrawRect(rectDraw, new Color(0.1f, 0.1f, 0.1f, 0.35f));
		}
	}
}
