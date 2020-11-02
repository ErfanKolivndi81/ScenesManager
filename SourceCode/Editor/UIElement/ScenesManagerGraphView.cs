using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;

namespace IRK.Unity.ScenesManager
{
	public class ScenesManagerGraphView : GraphView
	{
		public bool visibleGrid
		{
			get { return this[0].visible; }
			set { this[0].visible = value; }
		}

		private ScenesManagerWindow _window;

		public ScenesManagerWindow windowMain { get { return _window; } }

		public ScenesManagerGraphView(ScenesManagerWindow window)
		{
			_window = window;
		
			styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(SMEditorUtility.pathResourceFolder + "ScenesManagerGraphView.uss"));
			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			this.AddManipulator(new FreehandSelector());

			InitGrid();
			InitSearchWindow();
		}

		private void InitGrid()
		{
			GridBackground grid = new GridBackground();
			Insert(0, grid);
			grid.StretchToParentSize();
		}

		private void InitSearchWindow()
		{
			ScenesSearchWindow searchWIindow = ScriptableObject.CreateInstance<ScenesSearchWindow>();
			searchWIindow.Init(this);
			nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWIindow);
		}
				
		public bool CreateSceneNode(GUID id,int buildIndex, Vector2 position)
		{
			
			SceneNode node = new SceneNode(id,buildIndex);
			
			node.inputContainer.Add(new SceneNodePort(SceneNodePortType.Load));
			node.outputContainer.Add(new SceneNodePort(SceneNodePortType.Unload));

			node.capabilities &= ~Capabilities.Renamable;
			node.capabilities &= ~Capabilities.Copiable;

			node.RefreshExpandedState();
			node.RefreshPorts();
			node.SetPosition(new Rect(position, new Vector2(200.0f, 150.0f)));

			AddElement(node);

			return true;
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			List<Port> result = new List<Port>();

			var connections = startPort.connections.Select(tr => ((TransitionEdge)tr).sceneNodeInput.buildIndex).ToList();

			ports.ForEach((port) =>
			{
				if(
				startPort.direction != port.direction && 
				startPort.node != port.node &&
				!((SceneNode)port.node).Equals((SceneNode)startPort.node) &&
				!connections.Contains(((SceneNode)port.node).buildIndex)
				)
				{
					result.Add(port);
				}
			});

			return result;
		}

		public override void AddToSelection(ISelectable selectable)
		{
			base.AddToSelection(selectable);

			if(selectable is TransitionEdge)
			{
				List<Object> selection = new List<Object>(Selection.objects);
				selection.Add(((TransitionEdge)selectable).transition);
				Selection.objects = selection.ToArray();
			}
		}

		public override void RemoveFromSelection(ISelectable selectable)
		{
			base.RemoveFromSelection(selectable);

			if (selectable is TransitionEdge)
			{
				List<Object> selection = new List<Object>(Selection.objects);
				selection.Remove(((TransitionEdge)selectable).transition);
				Selection.objects = selection.ToArray();
			}
		}

		public override void ClearSelection()
		{
			base.ClearSelection();
			Selection.objects = null;
		}	
	}
}
