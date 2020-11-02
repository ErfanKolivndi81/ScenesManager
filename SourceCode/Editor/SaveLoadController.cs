using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace IRK.Unity.ScenesManager
{
	public static class SaveLoadController 
	{
		public static ScenesManagerGraphView ControllerToGraph(ScenesManagerController controller,ScenesManagerWindow window)
		{
			if (controller == null || controller.nodesData == null || controller.transitions == null)
				controller = ScenesManagerController.CreateEmptyController();

			ScenesManagerGraphView graphView = new ScenesManagerGraphView(window);
			graphView.visibleGrid = controller.visibleGrid;

			//Nodes
			for (int i = 0; i < controller.nodesData.Count; i++)
			{
				GUID id = new GUID(controller.nodesData[i].id);
				int buildIndex = controller.nodesData[i].buildIndex;
				Vector2 pos = controller.nodesData[i].position;
				graphView.CreateSceneNode(id,buildIndex, pos);
			}

			//Transitions
			var nodes = graphView.nodes.ToList();
			for (int i = 0; i < controller.transitions.Count; i++)
			{
				Transition transition = ScriptableObject.CreateInstance<Transition>();
				transition.data = controller.transitions[i];

				GUID id_from = new GUID(transition.data.idSceneNodeFrom);
				GUID id_to = new GUID(transition.data.idSceneNodeTo);

				//Get input,output nodes
				var output = (SceneNode)nodes.Find(n => ((SceneNode)n).id == id_from);
				var input = (SceneNode)nodes.Find(n => ((SceneNode)n).id == id_to);

				//Get input,output ports
				var portOutput = (SceneNodePort)output.outputContainer.ElementAt(0);
				var portInput = (SceneNodePort)input.inputContainer.ElementAt(0);
				
				//Create the transition(Edge)
				TransitionEdge edge = new TransitionEdge()
				{
					transition = transition,
					output = portOutput,
					input = portInput,
				};
				graphView.AddElement(edge);
				portOutput.Connect(edge);
				portInput.Connect(edge);
			}

			return graphView;
		}

		public static ScenesManagerController GraphToController(ScenesManagerGraphView graphView)
		{
			var edges = graphView.edges.ToList().Cast<TransitionEdge>().ToArray();
			var nodes = graphView.nodes.ToList().Cast<SceneNode>().ToArray();

			var positionNodes = new List<ScenesManagerController.SceneNodeData>();

			//Save the transitions
			List<TransitionData> transitions = edges.Select(e => e.transition.data).ToList();

			//Save the nodes(position,buildIndex)
			if (nodes != null)
			{
				for (int i = 0; i < nodes.Length; i++)
				{
					Vector2 pos = nodes[i].GetPosition().position;
					int buildIndex = ((SceneNode)nodes[i]).buildIndex;
					GUID id = ((SceneNode)nodes[i]).id;
					positionNodes.Add(new ScenesManagerController.SceneNodeData(id,buildIndex, pos));
				}
			}

			ScenesManagerController controller = ScriptableObject.CreateInstance<ScenesManagerController>();
			controller.visibleGrid = graphView.visibleGrid;
			controller.nodesData = positionNodes;
			controller.transitions = transitions;
			return controller;
		}
	}
}
