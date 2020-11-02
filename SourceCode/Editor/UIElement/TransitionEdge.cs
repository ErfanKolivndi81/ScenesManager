using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace IRK.Unity.ScenesManager
{
	public class TransitionEdge : Edge , System.IEquatable<TransitionEdge>
	{
		private Transition _transition;

		public Transition transition
		{
			get { return _transition; }
			set { _transition = value; }
		}

		public SceneNode sceneNodeInput
		{
			get { return (SceneNode)input.node; }
		}
		
		public SceneNode sceneNodeOutput
		{
			get { return (SceneNode)output.node; }
		}

		public TransitionEdge() : base()
		{
			//styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(SMEditorUtility.pathResourceFolder + "TransitionEdge.uss"));
		}

		public void Init()
		{
			_transition = ScriptableObject.CreateInstance<Transition>();
			_transition.data = new TransitionData()
			{
				idSceneNodeFrom = ((SceneNode)output.node).id.ToString(),
				idSceneNodeTo = ((SceneNode)input.node).id.ToString(),
				fromSceneIndex = ((SceneNode)output.node).buildIndex,
				toSceneIndex = ((SceneNode)input.node).buildIndex,
				isLoadAsync = true,
				loadSceneMode = 0,
				localPhysicsMode = 0,
				effectUnload = ScriptableObject.CreateInstance<NoneEffect>(),
				effectLoad = ScriptableObject.CreateInstance<NoneEffect>(),
			};
		}
		
		public bool Equals(TransitionEdge tr)
		{
			if (_transition == null || tr.transition == null)
				return false;

			return  
				sceneNodeInput.buildIndex == tr.sceneNodeInput.buildIndex 
				&& 
				sceneNodeOutput.buildIndex == tr.sceneNodeOutput.buildIndex;
		}
	}
}
