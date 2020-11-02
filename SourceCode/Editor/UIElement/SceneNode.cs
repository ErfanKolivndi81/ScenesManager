using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;

namespace IRK.Unity.ScenesManager
{
	public class SceneNode : Node,System.IEquatable<SceneNode>
	{
		private int _buildIndex;
		private GUID _id;

		public int buildIndex { get { return _buildIndex; } }
		public GUID id { get { return _id; } }

		public SceneNode(GUID id,int sceneBuildIndex)
		{
			this._id = id;
			this._buildIndex = sceneBuildIndex;
			styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(SMEditorUtility.pathResourceFolder + "SceneNode.uss"));

			string nameNode = SMEditorUtility.GetNameSceneBuild(buildIndex);
			this.name = nameNode;
			this.title = nameNode;
		}

		public bool Equals(SceneNode node)
		{
			return this.buildIndex == node.buildIndex;
		}
	}
}
