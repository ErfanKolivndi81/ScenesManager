using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace IRK.Unity.ScenesManager
{
	public class ScenesSearchWindow : ScriptableObject, ISearchWindowProvider
	{
		ScenesManagerWindow _window;
		ScenesManagerGraphView _graphView;

		public void Init(ScenesManagerGraphView graphView)
		{
			_graphView = graphView;
			_window = _graphView.windowMain;
		}

		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
			List<SearchTreeEntry> result = new List<SearchTreeEntry>();

			SearchTreeGroupEntry groupEntry = new SearchTreeGroupEntry(new GUIContent("Scenes"),0);
			result.Add(groupEntry);

			string[] names = SMEditorUtility.GetNameScenesBuild();

			for (int i = 0; i < names.Length; i++)
			{
				GUIContent content = new GUIContent(names[i]);
				SearchTreeEntry item = new SearchTreeEntry(content);
				item.userData = i;
				item.level = 1;
				result.Add(item);
			}

			return result;
		}

		public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
		{
			Vector2 mousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent, context.screenMousePosition - _window.position.position);
			Vector2 graphMousePos = _graphView.WorldToLocal(mousePosition);
			
			return _graphView.CreateSceneNode(UnityEditor.GUID.Generate(),(int)SearchTreeEntry.userData, graphMousePos);		
		}
	}
}
